using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Scripting.Utils;
using TileEngineSfmlCs.GameManagement.BinaryEncoding;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils;
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
            set => _intance = Instance;
        }

        #endregion

        private TileObjectSpirit[] _spiritsHashTable;
        private List<TileObjectSpirit>[,,] _sceneMatrix;

        public TileSpiritManager(int width, int height)
        {
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

            if (instanceId > _spiritsHashTable.Length)
            {
                ResizeTable();
            }

            TileObjectSpirit spirit = _spiritsHashTable[instanceId];

            if (spirit == null)
            {
                spirit = new TileObjectSpirit();
                spirit.Layer = updatePackage.Layer;
                spirit.Position = updatePackage.Position;
                _spiritsHashTable[instanceId] = spirit;
                RegisterPosition(spirit);
            }
            else
            {
                var oldPosition = spirit.Position;
                var oldLayer = spirit.Layer;

                spirit.Layer = updatePackage.Layer;
                spirit.Position = updatePackage.Position;

                ChangePosition(spirit, oldPosition, oldLayer);
            }

            spirit.Offset = updatePackage.Offset;

            spirit.Icon = updatePackage.Icon;
            spirit.IsPassable = updatePackage.IsPassable;
            spirit.IsLightTransparent = updatePackage.IsLightTransparent;
            
            spirit.LayerOrder = updatePackage.LayerOrder;
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
    }
}
