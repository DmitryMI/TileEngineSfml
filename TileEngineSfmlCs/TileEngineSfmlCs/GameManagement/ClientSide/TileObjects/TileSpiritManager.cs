﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Scripting.Utils;
using TileEngineSfmlCs.GameManagement.BinaryEncoding;
using TileEngineSfmlCs.GameManagement.ServerSide;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.TimeManagement;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils;
using TileEngineSfmlCs.Utils.RandomGenerators;
using CollectionUtils = TileEngineSfmlCs.Utils.CollectionUtils;

namespace TileEngineSfmlCs.GameManagement.ClientSide.TileObjects
{
    public class TileSpiritManager : IComparer<TileObjectSpirit>
    {
        public const int DefaultSize = 1024;

        #region Singleton

        private static TileSpiritManager _intance;

        public static TileSpiritManager Instance
        {
            get => _intance;
            set => _intance = value;
        }

        #endregion

        private TileObjectSpirit[] _spiritsHashTable;
        private List<TileObjectSpirit>[,,] _sceneMatrix;

        private ISpiritRenderer _renderer;

        public Camera PlayerCamera { get; set; }

        public int Width { get; }
        public int Height { get; }

        public TileSpiritManager(int width, int height, ISpiritRenderer renderer)
        {
            Width = width;
            Height = height;
            _spiritsHashTable = new TileObjectSpirit[DefaultSize];

            int layersCount = Enum.GetValues(typeof(TileLayer)).Length;

            _sceneMatrix = new List<TileObjectSpirit>[width,height, layersCount];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < layersCount; k++)
                    {
                        _sceneMatrix[i, j, k] = new List<TileObjectSpirit>();
                    }
                }
            }
            PlayerCamera = new Camera(new Vector2Int(Width / 2, Height / 2), new Vector2Int(20, 20));
            _renderer = renderer;
            TimeManager.Instance.NextFrameEvent += OnNextFrame;
        }

        private void OnNextFrame()
        {
            Rendering();
        }

        public void UpdateCamera(CameraUpdatePackage package)
        {
            if (package.TrackingInstanceId != -1)
            {
                TileObjectSpirit spirit = _spiritsHashTable[package.TrackingInstanceId];
                PlayerCamera.TrackingTarget = spirit;
            }
            else
            {
                PlayerCamera.Center = package.CameraCenter;
            }

            PlayerCamera.Size = package.ViewportSize;
        }

        private void UnregisterPosition(TileObjectSpirit spirit)
        {
            int x = spirit.Position.X;
            int y = spirit.Position.Y;
            int layer = (int) spirit.Layer;
            _sceneMatrix[x, y, layer].Remove(spirit);
        }

        private void RegisterPosition(TileObjectSpirit spirit)
        {
            int x = spirit.Position.X;
            int y = spirit.Position.Y;
            int layer = (int)spirit.Layer;
            _sceneMatrix[x, y, layer].InsertSortedDescending(spirit, this);
        }

        private void ChangePosition(TileObjectSpirit spirit, Vector2Int oldPosition, TileLayer oldLayer)
        {
            int x = oldPosition.X;
            int y = oldPosition.Y;
            int layer = (int)oldLayer;
            _sceneMatrix[x, y, layer].Remove(spirit);

            RegisterPosition(spirit);
        }

        public void UpdateTileSpirit(TileObjectUpdatePackage updatePackage)
        {
            int instanceId = updatePackage.InstanceId;

            if (instanceId >= _spiritsHashTable.Length)
            {
                ResizeTable();
            }

            TileObjectSpirit spirit = _spiritsHashTable[instanceId];
            

            if (spirit == null)
            {
                spirit = new TileObjectSpirit();
                spirit.LayerOrder = updatePackage.LayerOrder;
                spirit.Layer = updatePackage.Layer;
                spirit.Position = updatePackage.Position;
                _spiritsHashTable[instanceId] = spirit;
                RegisterPosition(spirit);
                //LogManager.RuntimeLogger.Log($"TileObject {updatePackage.VisibleName} spawned on coordinates {spirit.Position.X}, {spirit.Position.Y}");
            }
            else
            {
                var oldPosition = spirit.Position;
                var oldLayer = spirit.Layer;

                spirit.LayerOrder = updatePackage.LayerOrder;
                spirit.Layer = updatePackage.Layer;
                spirit.Position = updatePackage.Position;

                ChangePosition(spirit, oldPosition, oldLayer);
            }

            spirit.Offset = updatePackage.Offset;

            spirit.Icon = updatePackage.Icon;
            spirit.IsPassable = updatePackage.IsPassable;
            spirit.IsLightTransparent = updatePackage.IsLightTransparent;
            spirit.VisibleName = updatePackage.VisibleName;
            spirit.IsActiveOnScene = updatePackage.IsActiveOnScene;
        }

        public void UpdatePosition(PositionUpdatePackage updatePackage)
        {
            int instanceId = updatePackage.InstanceId;
            if (instanceId > _spiritsHashTable.Length)
            {
                ResizeTable();
            }
            TileObjectSpirit spirit = _spiritsHashTable[instanceId];
            if (spirit == null)
            {
                // We need full update for not spawned object
                return;
            }
            var oldPosition = spirit.Position;
            var oldLayer = spirit.Layer;
            spirit.Position = updatePackage.Position;
            spirit.Offset = updatePackage.Offset;
            ChangePosition(spirit, oldPosition, oldLayer);
        }

        private void ResizeTable()
        {
            Array.Resize(ref _spiritsHashTable, _spiritsHashTable.Length * 2);
        }

        public int Compare(TileObjectSpirit x, TileObjectSpirit y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            return x.LayerOrder - y.LayerOrder;
        }

        private Vector2 GetCameraCenter()
        {
            if (PlayerCamera.TrackingTarget != null)
            {
                return PlayerCamera.TrackingTarget.Position + PlayerCamera.TrackingTarget.Offset;
            }
            else
            {
                return PlayerCamera.Center;
            }
        }

        private bool IsInBounds(Vector2Int cell)
        {
            if (cell.X < 0 || cell.Y < 0 || cell.X >= Width || cell.Y >= Height)
            {
                return false;
            }

            return true;
        }

        private void Rendering()
        {
            var layers = Enum.GetValues(typeof(TileLayer));
            // Drawing
            int width = PlayerCamera.Size.X;
            int height = PlayerCamera.Size.Y;
            Vector2 cameraCenter = GetCameraCenter();
            int minX = (int)Math.Floor(cameraCenter.X - width / 2);
            int maxX = (int)Math.Ceiling(cameraCenter.X + width / 2);
            int minY = (int)Math.Floor(cameraCenter.Y - height / 2);
            int maxY = (int)Math.Ceiling(cameraCenter.Y + height / 2);

            if (minX < 0)
            {
                minX = 0;
            }

            if (maxX >= Width)
            {
                maxX = Width - 1;
            }

            if (minY < 0)
            {
                minY = 0;
            }

            if (maxY >= Height)
            {
                maxY = Height - 1;
            }

            _renderer.SetViewCenter(cameraCenter);

            _renderer.PreRendering();
            
            for (int layer = 0; layer < layers.Length; layer++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        for (int order = 0; order < _sceneMatrix[x, y, layer].Count; order++)
                        {
                            var spirit = _sceneMatrix[x, y, layer][order];
                            if (spirit.IsActiveOnScene)
                            {
                                Vector2 position = spirit.Position + spirit.Offset;
                                _renderer.Render(position, spirit.Icon, spirit);
                            }
                        }
                    }
                }
            }

            _renderer.PostRendering();
        }
    }
}
