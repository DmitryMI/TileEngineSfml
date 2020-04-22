﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ResourcesManager;
using ResourcesManager.ResourceTypes;
using SFML.Graphics;
using SFML.System;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.SceneSerialization;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.TileEngine.TypeManagement;
using TileEngineSfmlCs.Types;
using Color = SFML.Graphics.Color;
using Icon = TileEngineSfmlCs.Types.Icon;
using Image = SFML.Graphics.Image;

namespace TileEngineSfmlMapEditor.MapEditing
{
    public class TileEngineEditor
    {
        public int PixelsPerUnit = 32;

        private Scene _scene;
        private IRenderReceiver _renderReceiver;
        private Vector2 _cameraPosition;
        private ResourcesManager.GameResources _resources;
        private Random _rnd = new Random();
        private TypeManager _typeManager;
        private List<Vector2Int> _selectedCells = new List<Vector2Int>();
        private List<Vector2Int> _highlightedCellsTemp = new List<Vector2Int>();
        private bool[] _layersVisibility;
        private bool[] _layersEnabled;

        #region Layers

        public bool[] GetLayersVisibility()
        {
            return _layersVisibility;
        }

        public bool[] GetLayersEnabled()
        {
            return _layersEnabled;
        }

        public void SetLayerVisibility(int layerIndex, bool visible)
        {
            _layersVisibility[layerIndex] = visible;
        }

        public void SetLayerEnabled(int layerIndex, bool enabled)
        {
            _layersEnabled[layerIndex] = enabled;
        }

        public int GetLayerIndex(string layerName)
        {
            TileLayer layer = (TileLayer)Enum.Parse(typeof(TileLayer), layerName);
            return (int) layer;
        }

        public string[] GetLayerNames()
        {
            return Enum.GetNames(typeof(TileLayer));
        }

        #endregion

        #region Initialization
        private void InitLayers()
        {
            Array layers = Enum.GetValues(typeof(TileLayer));
            _layersVisibility = new bool[layers.Length];
            _layersEnabled = new bool[layers.Length];
            for (int i = 0; i < _layersVisibility.Length ; i++)
            {
                _layersVisibility[i] = true;
                _layersEnabled[i] = true;
            }
        }

        private Texture GetTexture(int resourceId)
        {
            ResourceEntry resourceEntry = ResourcesManager.GameResources.Instance.GetEntry(resourceId);
            if (resourceEntry.LoadedValue == null)
            {
                FileStream fs = ResourcesManager.GameResources.Instance.GetFileStream(resourceEntry);
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();
                fs.Dispose();
                Texture texture = new Texture(data);
                resourceEntry.LoadedValue = texture;
            }

            return (Texture)resourceEntry.LoadedValue;
        }

        private void LoadResources()
        {
            string resourcesPath = Path.Combine(Environment.CurrentDirectory, "Resources");
            _resources = new GameResources(resourcesPath);
            GameResources.Instance = _resources;
        }

        private void InitializeFields()
        {
            _cameraPosition = new Vector2Int(_scene.Width / 2, _scene.Height / 2);

            if (TypeManager.Instance == null)
            {
                _typeManager = new TypeManager();
                TypeManager.Instance = _typeManager;
            }

            GridThickness = 0.5f;
            Color color = Color.Green;
            color.A = 50;
            SelectionColor = color;

            InitLayers();
        }

        public TileEngineEditor(Stream mapStream, IRenderReceiver renderReceiver)
        {
            LoadResources();
            _renderReceiver = renderReceiver;
            _scene = Serializer.DeserializeScene(mapStream);
            InitializeFields();
        }

        public TileEngineEditor(int width, int height, IRenderReceiver renderReceiver)
        {
            LoadResources();
            _renderReceiver = renderReceiver;
            _scene = new Scene(width, height);
            ProcessNewScene();
            InitializeFields();
        }

        private void ProcessNewScene()
        {
            
        }


        #endregion

        #region Drawing

        private void DrawSpriteResource(Icon icon, int orderIndex, Vector2f position)
        {
            int resourceId = icon.GetResourceId(orderIndex);
            Texture texture = GetTexture(resourceId);
            Sprite sprite = new Sprite(texture);
            ColorB color = icon.GetColor(orderIndex);
            sprite.Color = new Color(color.R, color.G, color.B, color.A);

            sprite.Position = position;
            _renderReceiver.DrawSprite(sprite);
        }

        private void DrawCellObjects(Vector2Int cell)
        {
            TileObject[] objects = _scene.GetObjects(cell);
            Vector2f sfmlPosition = WorldToSfml(cell);
            foreach (var obj in objects)
            {
                Icon icon = obj.Icon;
                if (_layersVisibility[(int) obj.Layer])
                {
                    for (int i = 0; i < icon.SpritesCount; i++)
                    {
                        DrawSpriteResource(icon, i, sfmlPosition);
                    }
                }
            }
        }

        private void DrawColorRect(Vector2Int cell, Color color)
        {
            Shape cellShape = new RectangleShape(new Vector2f(PixelsPerUnit, PixelsPerUnit));
            cellShape.FillColor = color;

            Vector2f position = WorldToSfml(cell);

            cellShape.Position = position;
            _renderReceiver.DrawSprite(cellShape);
        }

        private void DrawGridCell(float leftX, float bottomY, float width, float height)
        {
            Shape shape = new RectangleShape(new Vector2f(width, height));
            shape.Position = new Vector2f(leftX, bottomY);
            shape.OutlineColor = Color.White;
            shape.FillColor = Color.Transparent;
            shape.OutlineThickness = GridThickness;
            _renderReceiver.DrawSprite(shape);
        }


        private void DrawGrid(int minX, int maxX, int minY, int maxY)
        {
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    Vector2f cellPos = WorldToSfml(new Vector2Int(x, y));
                    //cellPos.X = PixelsPerUnit;
                    //cellPos.Y -= PixelsPerUnit;
                    DrawGridCell(cellPos.X, cellPos.Y, PixelsPerUnit, PixelsPerUnit);
                }
            }
        }

        private void DrawSelectedCells()
        {
            foreach (var cell in _selectedCells)
            {
                //_highlightedCellsTemp.Add(cell);
                DrawColorRect(cell, SelectionColor);
            }
        }

        private void HighlightTempCells()
        {
            foreach (var cell in _highlightedCellsTemp)
            {
                HighlightCell(cell);
            }
            _highlightedCellsTemp.Clear();
        }

        public void UpdateGraphics()
        {
            Vector2Int viewportSize = GetViewportSize();

            int minX = (int)Math.Floor(-viewportSize.X / 2.0f + _cameraPosition.X);
            int maxX = (int)Math.Ceiling(viewportSize.X / 2.0f + _cameraPosition.X) + 1;
            int minY = (int)Math.Floor(-viewportSize.Y / 2.0f + _cameraPosition.Y);
            int maxY = (int)Math.Ceiling(viewportSize.Y / 2.0f + _cameraPosition.Y) + 1;

            if (minX < 0)
            {
                minX = 0;
            }
            if (maxX > _scene.Width)
            {
                maxX = _scene.Width;
            }

            if (minY < 0)
            {
                minY = 0;
            }
            if (maxY > _scene.Height)
            {
                maxY = _scene.Height;
            }

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    DrawCellObjects(new Vector2Int(x, y));
                }
            }
            
            DrawSelectedCells();

            if (ShowGrid)
            {
                DrawGrid(minX, maxX, minY, maxY);
            }

            HighlightTempCells();
        }

        #endregion

        #region Utils

        private void DestroyTileObject(TileObject tileObject)
        {
            _scene.DestroyEditor(tileObject);
        }

        public void ForEachSelectedCell(Action<Vector2Int> action)
        {
            foreach (var cell in _selectedCells)
            {
                action(cell);
            }
        }

        public void ForEachInCell(Vector2Int cell, Action<TileObject> action)
        {
            var objs = _scene.GetObjects(cell);
            foreach (var obj in objs)
            {
                action(obj);
            }
        }

        private void ForeachCell(CellRect rect, Action<Vector2Int> action)
        {
            for (int x = rect.XLeft; x <= rect.XRight; x++)
            {
                for (int y = rect.YBottom; y <= rect.YTop; y++)
                {
                    action?.Invoke(new Vector2Int(x, y));
                }
            }
        }

        public Bitmap GetEditorImage(Type tileObjectType)
        {
            if (typeof(TileObject).IsAssignableFrom(tileObjectType))
            {
                if (tileObjectType.IsAbstract)
                    return null;
                if (tileObjectType.GetConstructors().All(c => c.GetParameters().Length != 0))
                {
                    return null;
                }

                TileObject tileObject = (TileObject)Activator.CreateInstance(tileObjectType);
                Icon editorIcon = tileObject.EditorIcon;
                if (editorIcon == null)
                    return null;

                Texture texture = GetTexture(editorIcon.GetResourceId(0));
                Image image = texture.CopyToImage();
                
                byte[] pixels = image.Pixels;

                int width = (int)image.Size.X;
                int height = (int)image.Size.Y;

                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                /*ColorPalette ncp = bmp.Palette;
                for (int i = 0; i < 256; i++)
                    ncp.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);
                bmp.Palette = ncp;*/

                var boundsRect = new Rectangle(0, 0, width, height);
                BitmapData bmpData = bmp.LockBits(boundsRect,
                    ImageLockMode.WriteOnly,
                    bmp.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                int bytes = bmpData.Stride * bmp.Height;
                var rgbValues = new byte[bytes];
                
                Array.Copy(pixels, 0, rgbValues, 0, pixels.Length);

                Marshal.Copy(rgbValues, 0, ptr, bytes);
                bmp.UnlockBits(bmpData);
                return bmp;
            }

            return null;
        }
        
        #endregion

        #region CellSelecting

        public Color SelectionColor { get; set; }

        public void SelectCell(Vector2Int cell)
        {
            if (!_selectedCells.Contains(cell))
            {
                _selectedCells.Add(cell);
            }
        }

        public void ClearSelection()
        {
            _selectedCells.Clear();
        }

        public void HighlightRect(CellRect rect)
        {
            ForeachCell(rect, _highlightedCellsTemp.Add);
        }

        public void SelectRect(CellRect rect)
        {
            ForeachCell(rect, SelectCell);
        }

        public void HighlightCell(Vector2Int cell)
        {
            Color color = new Color(0, 0, 255, 50);
            DrawColorRect(cell, color);
        }

        public void HighlightContainingCell(int pixelX, int pixelY)
        {
            Vector2Int cellStart = GetPosition(pixelX, pixelY);

            HighlightCell(cellStart);
        }

        #endregion

        #region Grid
        public bool ShowGrid { get; set; }
        public float GridThickness { get; set; }

        #endregion

        #region Transformations

        public CellRect GetCellRect(int x0, int y0, int x1, int y1)
        {
            Vector2Int cell0 = GetPosition(x0, y0);
            Vector2Int cell1 = GetPosition(x1, y1);
            return new CellRect(cell0.X, cell0.Y, cell1.X, cell1.Y);
        }

        public Vector2Int GetPosition(int pixelX, int pixelY)
        {
            Vector2i pixelPosition = new Vector2i(pixelX, pixelY);
            Vector2f sfmlPosition = _renderReceiver.PixelToSfml(pixelPosition);
            Vector2Int cell = SfmlToWorld(sfmlPosition);
            return cell;
        }

        public void GetPositionWithOffset(int pixelX, int pixelY, out Vector2Int cell, out Vector2 offset)
        {
            Vector2i pixelPosition = new Vector2i(pixelX, pixelY);
            Vector2f sfmlPosition = _renderReceiver.PixelToSfml(pixelPosition);
            SfmlToWorld(sfmlPosition, out cell, out offset);
        }

        private Vector2f WorldToSfml(Vector2Int cell)
        {
            Vector2 shifted = cell - CameraPosition;
            float shiftX = _renderReceiver.RenderView.Size.X / 2;
            float shiftY = _renderReceiver.RenderView.Size.Y / 2;
            float spriteSizeShiftX = -16;
            float spriteSizeShiftY = -16;
            float x = shifted.X * PixelsPerUnit + shiftX + spriteSizeShiftX;
            float y = shifted.Y * PixelsPerUnit + shiftY + spriteSizeShiftY;
            return new Vector2f(x, y);
        }

        private Vector2Int SfmlToWorld(Vector2f pos)
        {
            float shiftX = _renderReceiver.RenderView.Size.X / 2;
            float shiftY = _renderReceiver.RenderView.Size.Y / 2;
            //float spriteSizeShiftX = +16;
            //float spriteSizeShiftY = +16;
            float spriteSizeShiftX = 0;
            float spriteSizeShiftY = 0;

            float shiftedX = (pos.X - spriteSizeShiftX - shiftX) / PixelsPerUnit;
            float shiftedY = (pos.Y - spriteSizeShiftY - shiftY) / PixelsPerUnit;
            Vector2 cellFloat = new Vector2(shiftedX, shiftedY) + CameraPosition;

            int xCell = (int)Math.Round(cellFloat.X);
            int yCell = (int)Math.Round(cellFloat.Y);
            return new Vector2Int(xCell, yCell);
        }

        private void SfmlToWorld(Vector2f pos, out Vector2Int cell, out Vector2 offset)
        {
            float shiftX = _renderReceiver.RenderView.Size.X / 2;
            float shiftY = _renderReceiver.RenderView.Size.Y / 2;
            //float spriteSizeShiftX = +16;
            //float spriteSizeShiftY = +16;
            float spriteSizeShiftX = 0;
            float spriteSizeShiftY = 0;

            float shiftedX = (pos.X - spriteSizeShiftX - shiftX) / PixelsPerUnit;
            float shiftedY = (pos.Y - spriteSizeShiftY - shiftY) / PixelsPerUnit;
            Vector2 cellFloat = new Vector2(shiftedX, shiftedY) + CameraPosition;

            int xCell = (int)Math.Round(cellFloat.X);
            int yCell = (int)Math.Round(cellFloat.Y);
            float xOffset = cellFloat.X - xCell;
            float yOffset = cellFloat.Y - yCell;
            cell = new Vector2Int(xCell, yCell);
            offset = new Vector2(xOffset, yOffset);
        }

        private Vector2Int GetViewportSize()
        {
            float pixelWidth = _renderReceiver.RenderView.Size.X;
            float pixelHeight = _renderReceiver.RenderView.Size.Y;
            return new Vector2Int((int)(pixelWidth / PixelsPerUnit), (int)(pixelHeight / PixelsPerUnit));
        }

        #endregion

        #region MapEditor

        public TreeNode<Type> TypeTreeRoot => TypeManager.Instance.TreeRoot;

        public void SaveScene(Stream serializationStream)
        {
            Serializer.SerializeScene(_scene, serializationStream);
        }

        public Vector2 CameraPosition
        {
            get => _cameraPosition;
            set
            {
                _cameraPosition = value;
                UpdateGraphics();
            }
        }

        public void InsertTileObject(Type tileObjectType, Vector2Int cell, Vector2 offset)
        {
            bool isImplementation = !tileObjectType.IsAbstract;
            bool isTileObject = typeof(TileObject).IsAssignableFrom(tileObjectType);
            bool hasParameterlessConstructor = tileObjectType.GetConstructors().Any(c => c.GetParameters().Length == 0);
            if (isTileObject && hasParameterlessConstructor && isImplementation)
            {
                TileObject instance = (TileObject)Activator.CreateInstance(tileObjectType);
                instance.Position = cell;
                instance.Offset = offset;
                _scene.InstantiateEditor(instance);
            }
        }

        public void DeleteSelectedTileObjects()
        {
            void DeleteObject(TileObject to)
            {
                if (_layersEnabled[(int) to.Layer])
                {
                    _scene.Destroy(to);
                }
            }

            void ForEach(Vector2Int forCell)
            {
                ForEachInCell(forCell, DeleteObject);
            }

            ForEachSelectedCell(ForEach);
        }

        

        #endregion
    }
}
