using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFML.System;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.Logging;
using TileEngineSfmlCs.TileEngine.SceneSerialization;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.TileEngine.TypeManagement;
using TileEngineSfmlCs.TileEngine.TypeManagement.EntityTypes;
using TileEngineSfmlCs.Types;
using TileEngineSfmlMapEditor.MapEditing;

namespace TileEngineSfmlMapEditor
{
    public partial class MapViewForm : Form, ILogger
    {
        public const int MouseGrabThreshold = 2;
        public const int ListIconDefaultSize = 16;

        private TileEngineEditor _editor;
        private string _mapFilePath;
        private TreeNode<EntityType> _typeTreeNode;

        private Point? _firstSelectionPoint;
        private Point? _lastMousePosition;

        private EntityType _selectedType;

        #region Initialization
        public MapViewForm()
        {
            InitializeComponent();

            LogManager.EditorLogger = this;

            RenderingCanvas.Initialize();
            RenderingCanvas.OnMouseGrabEvent = OnMouseGrab;

            if (TypeManager.Instance == null)
            {
                TypeManager.Instance = new TypeManager();
            }

            _typeTreeNode = TypeManager.Instance.TreeRoot;
        }

        private void LoadMap(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            _editor = new TileEngineEditor(fs, RenderingCanvas);
            fs.Close();
            fs.Dispose();
            _mapFilePath = fileName;
            OnEditorCreated();
        }

        private void InitLayerMenuItems()
        {
            string[] layers = _editor.GetLayerNames();
            layerVisibleMenuItem.DropDownItems.Clear();
            layersActiveToolStripMenuItem.DropDownItems.Clear();

            bool[] layersVisible = _editor.GetLayersVisibility();
            bool[] layersEnabled = _editor.GetLayersEnabled();
            for (int i = 0; i < layers.Length; i++)
            {
                int index = _editor.GetLayerIndex(layers[i]);
                bool visibility = layersVisible[index];
                bool activated = layersEnabled[index];

                ToolStripMenuItem layerVisibleItem = new ToolStripMenuItem(layers[i]);
                //layerVisibleItem.CheckOnClick = true;
                layerVisibleItem.MouseUp += LayerVisibilityItemMouseUp;
                layerVisibleMenuItem.DropDownItems.Add(layerVisibleItem);

                ToolStripMenuItem layerActiveItem = new ToolStripMenuItem(layers[i]);
                //layerActiveItem.CheckOnClick = true;
                layerActiveItem.MouseUp += LayerActivateItemMouseUp;
                layersActiveToolStripMenuItem.DropDownItems.Add(layerActiveItem);
            }

            layerVisibleMenuItem.Enabled = true;
            layersActiveToolStripMenuItem.Enabled = true;

            UpdateLayerMenuItems();
        }

        private void UpdateLayerMenuItems()
        {
            bool[] layersVisible = _editor.GetLayersVisibility();
            bool[] layersEnabled = _editor.GetLayersEnabled();
            for (int i = 0; i < layerVisibleMenuItem.DropDownItems.Count; i++)
            {
                var visibilityItem = (ToolStripMenuItem)layerVisibleMenuItem.DropDownItems[i];
                visibilityItem.Checked = layersVisible[i];

                var activationItem = (ToolStripMenuItem)layersActiveToolStripMenuItem.DropDownItems[i];
                activationItem.Checked = layersEnabled[i];
            }
        }

        private void OnEditorCreated()
        {
            InitLayerMenuItems();
        }


        #endregion

        #region FileManagement

        private void CreateNewMap()
        {
            NewMapForm newMapForm = new NewMapForm();
            newMapForm.ShowDialog();
            if (newMapForm.ResultOk)
            {
                _editor = new TileEngineEditor(newMapForm.ResultWidth, newMapForm.ResultHeight, RenderingCanvas);
            }

            OnEditorCreated();
        }

        private void SaveMap(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            _editor.SaveScene(fs);
            fs.Close();
            fs.Dispose();
            _mapFilePath = path;
        }

        private void SaveMap(bool resetPath)
        {
            if (_mapFilePath != null && !resetPath)
            {
                SaveMap(_mapFilePath);
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.CreatePrompt = false;
                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.Filter = "TileEngine map (*.temap)|*.temap|All files (*.*)|*.*";
                saveFileDialog.DefaultExt = "*.temap";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveMap(saveFileDialog.FileName);
                }
            }
        }

        private void OpenMapFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "TileEngine map (*.temap)|*.temap|All files (*.*)|*.*"; ;
            fileDialog.Multiselect = false;
            fileDialog.InitialDirectory = Application.StartupPath;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = fileDialog.FileName;
                if (File.Exists(path))
                {
                    LoadMap(path);
                }
            }
        }

        #endregion

        #region UiHandlers

        private void RenderingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            _lastMousePosition = new Point(e.X, e.Y);

            if (_editor != null)
            {
                _editor.GetPositionWithOffset(e.X, e.Y, out var cell, out var offset);
                CoordinateLabel.Text = $@"({cell.X}, {cell.Y}) : ({offset.X:0.00}, {offset.Y:0.00})";
                TileObject underCursor = _editor.GetObjectUnderPoint(e.X, e.Y);
                UnderCursorLabel.Text = underCursor != null ? underCursor.VisibleName : String.Empty;
            }
        }

        private void RenderingCanvas_MouseClick(object sender, MouseEventArgs e)
        {

        }
        

        private void LayerVisibilityItemMouseUp(object sender, MouseEventArgs args)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            string layerName = item.Text;
            int index = _editor.GetLayerIndex(layerName);

            item.Checked = !item.Checked;

            if ((args.Button & MouseButtons.Left) != 0)
            {
                _editor.SetLayerVisibility(index, item.Checked);
            }
            else if((args.Button & MouseButtons.Right) != 0)
            {
                bool[] visible = _editor.GetLayersVisibility();

                if (IsOnlySelected(visible, index))
                {
                    DeselectOnly(visible, index);
                }
                else
                {
                    SelectOnly(visible, index);
                }
                
                _editor.SetLayersVisibility(visible);
            }
            UpdateLayerMenuItems();
            layerVisibleMenuItem.ShowDropDown();
        }

        private void LayerActivateItemMouseUp(object sender, MouseEventArgs args)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            string layerName = item.Text;
            int index = _editor.GetLayerIndex(layerName);
            item.Checked = !item.Checked;
            if ((args.Button & MouseButtons.Left) != 0)
            {
                _editor.SetLayerEnabled(index, item.Checked);
            }
            else if((args.Button & MouseButtons.Right) != 0)
            {
                bool[] enabled = _editor.GetLayersEnabled();
                if (IsOnlySelected(enabled, index))
                {
                    DeselectOnly(enabled, index);
                }
                else
                {
                    SelectOnly(enabled, index);
                }
                _editor.SetLayersEnabled(enabled);
            }
            UpdateLayerMenuItems();
            layersActiveToolStripMenuItem.ShowDropDown();
        }
       

        private void openMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenMapFile();
            UpdateImage();
            UpdateTypeList();
        }

        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewMap();
            UpdateImage();
            UpdateTypeList();
        }

        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMap(false);
            UpdateImage();
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            _editor?.UpdateGraphics();
            if (_lastMousePosition != null)
                _editor?.HighlightContainingCell(_lastMousePosition.Value.X, _lastMousePosition.Value.Y);
            RenderingCanvas?.UpdateGraphics();
            HighlightSelectionRect();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMap(true);
        }

        private void TileObjectsListView_ItemActivate(object sender, EventArgs e)
        {
            ListView listView = (ListView)sender;
            if (listView.SelectedItems.Count == 0)
            {
                return;
            }
            ListViewItem activatedItem = listView.SelectedItems[0];
            if (activatedItem.Name.Equals("Up"))
            {
                if (_typeTreeNode.ParentNode != null)
                {
                    _typeTreeNode = _typeTreeNode.ParentNode;
                    UpdateTypeList();
                }

                return;
            }

            string itemTypeName = activatedItem.Text;

            TreeNode<EntityType> childNode = _typeTreeNode.FirstOrDefault(n => n.Data.Name == itemTypeName);

            if (childNode != null && childNode.Count > 0)
            {
                _typeTreeNode = childNode;
                UpdateTypeList();
            }
        }

        private void showGridToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (_editor != null)
            {
                _editor.ShowGrid = showGridToolStripMenuItem.Checked;
            }
            else
            {
                showGridToolStripMenuItem.CheckStateChanged -= showGridToolStripMenuItem_CheckedChanged;
                showGridToolStripMenuItem.Checked = false;
                showGridToolStripMenuItem.CheckStateChanged += showGridToolStripMenuItem_CheckedChanged;
            }
        }

        private void RenderingCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            _firstSelectionPoint = new Point(e.X, e.Y);
        }


        private void RenderingCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            Point? firstSelectionPoint = _firstSelectionPoint;

            _firstSelectionPoint = null;
            _lastMousePosition = null;

            if (firstSelectionPoint == null)
                return;

            if (e.Button == MouseButtons.Right)
            {
                _editor.ClearSelection();
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            int firstX = firstSelectionPoint.Value.X;
            int firstY = firstSelectionPoint.Value.Y;
            int lastX = e.X;
            int lastY = e.Y;

            if (firstX > lastX)
            {
                int t = lastX;
                lastX = firstX;
                firstX = t;
            }
            if (firstY > lastY)
            {
                int t = lastY;
                lastY = firstY;
                firstY = t;
            }

            int delta = lastX - firstX + lastY - firstY;
            if (delta > MouseGrabThreshold)
            {
                _editor?.SelectRect(_editor.GetCellRect(firstX, firstY, lastX, lastY));
            }
            else
            {
                AttemptInsertObject(lastX, lastY);
            }
        }

        private void TileObjectsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TileObjectsListView.SelectedItems.Count != 1)
                return;
            _selectedType = GetSelectedTileObject();
        }

        private void RenderingCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            ClearErrorMessage();
            if (e.KeyCode == Keys.Insert)
            {
                _editor.ForEachSelectedCell(AttemptInsertObject);
                _editor.ClearSelection();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                _editor.DeleteSelectedTileObjects();
                _editor.ClearSelection();
            }
        }


        #endregion

        #region Utils

        private void SelectOnly(bool[] values, int onlyIndex)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (i == onlyIndex)
                {
                    values[i] = true;
                }
                else
                {
                    values[i] = false;
                }
            }
        }

        private bool IsOnlySelected(bool[] values, int index)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (i == index && !values[i])
                {
                    return false;
                }

                if (i != index && values[i])
                    return false;
            }

            return true;
        }

        private void DeselectOnly(bool[] values, int index)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (i == index)
                {
                    values[i] = false;
                }
                else
                {
                    values[i] = true;
                }
            }
        }

        #endregion

        #region UiUpdate

        private void UpdateImage()
        {
            _editor?.UpdateGraphics();
        }

        private void UpdateTypeList()
        {
            if (_editor == null)
            {
                return;
            }

            TileObjectsListView.Items.Clear();

            ListViewItem upItem = new ListViewItem();
            upItem.Name = "Up";
            upItem.Text = "..";

            TileObjectsListView.Items.Add(upItem);

            foreach (var typeNode in _typeTreeNode)
            {
                EntityType type = typeNode.Data;
                ListViewItem item = new ListViewItem();
                item.Text = type.Name;
                TileObjectsListView.Items.Add(item);

                Image image = _editor.GetEditorImage(type);
                if (image != null)
                {
                    if (TileObjectsListView.SmallImageList == null)
                    {
                        TileObjectsListView.SmallImageList = new ImageList();
                        TileObjectsListView.SmallImageList.ImageSize = new Size(ListIconDefaultSize, ListIconDefaultSize);
                    }
                    TileObjectsListView.SmallImageList.Images.Add(image);
                    item.ImageIndex = TileObjectsListView.SmallImageList.Images.Count - 1;
                }
            }

            string path = TreeNode<EntityType>.GetPath(_typeTreeNode, t => t.Name);
            TypePathLabel.Text = path;
        }

        #endregion

        #region ILogger implementation

        public void Log(string message)
        {
            ErrorLabel.Text = message;
        }

        public void LogError(string message)
        {
            ErrorLabel.Text = message;
            ErrorLabel.ForeColor = Color.Red;
        }

        private void ClearErrorMessage()
        {
            ErrorLabel.Text = "OK";
            ErrorLabel.ForeColor = Color.Black;
        }


        #endregion

        #region SpecialActions

        private void OnMouseGrab(float dx, float dy)
        {
            if (_editor == null)
                return;

            bool gridEnabled = _editor.ShowGrid;
            _editor.ShowGrid = false;
            _editor.CameraPosition += new Vector2(-dx / 32, -dy / 32);
            _editor.ShowGrid = gridEnabled;
        }

        public EntityType SelectedTileObject => _selectedType;



        private void HighlightSelectionRect()
        {
            if (_firstSelectionPoint != null && _lastMousePosition != null)
            {
                int firstX = _firstSelectionPoint.Value.X;
                int firstY = _firstSelectionPoint.Value.Y;
                int lastX = _lastMousePosition.Value.X;
                int lastY = _lastMousePosition.Value.Y;

                if (firstX > lastX)
                {
                    int t = lastX;
                    lastX = firstX;
                    firstX = t;
                }
                if (firstY > lastY)
                {
                    int t = lastY;
                    lastY = firstY;
                    firstY = t;
                }
                _editor?.HighlightRect(_editor.GetCellRect(firstX, firstY, lastX, lastY));
            }
        }

        private EntityType GetSelectedTileObject()
        {
            if (TileObjectsListView.SelectedItems.Count == 0)
            {
                return null;
            }
            ListViewItem activatedItem = TileObjectsListView.SelectedItems[0];
            string itemTypeName = activatedItem.Text;

            TreeNode<EntityType> childNode = _typeTreeNode.FirstOrDefault(n => n.Data.Name == itemTypeName);
            if (childNode == null)
            {
                return null;
                throw new InvalidOperationException("UI Exception");
            }

            EntityType type = childNode.Data;
            if (type != null)
            {
                SelectedTypePreviewBox.Image = _editor.GetEditorImage(type);
                SelectedTypeNameBox.Text = type.Name;
            }

            return type;
        }
        private void AttemptInsertObject(Vector2Int cell)
        {
            EntityType selectedObjectType = SelectedTileObject;
            if (selectedObjectType != null && selectedObjectType.CanActivate)
            {
                _editor.InsertTileObject(selectedObjectType, cell, new Vector2(0, 0));
            }
        }

        private void AttemptDeleteObjects(Vector2Int cell)
        {

        }

        private void AttemptInsertObject(int x, int y)
        {
            ClearErrorMessage();
            EntityType selectedObjectType = SelectedTileObject;
            
            if(!selectedObjectType.CanActivate)
                return;

            _editor.GetPositionWithOffset(x, y, out var cell, out var offset);

            if (snapToCellToolStripMenuItem.Checked)
            {
                offset = new Vector2(0, 0);
            }

            _editor.InsertTileObject(selectedObjectType, cell, offset);
        }


        #endregion


    }
}
