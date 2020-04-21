using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFML.System;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.SceneSerialization;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.TileEngine.TypeManagement;
using TileEngineSfmlCs.Types;
using TileEngineSfmlMapEditor.MapEditing;

namespace TileEngineSfmlMapEditor
{
    public partial class MapViewForm : Form
    {
        private TileEngineEditor _editor;
        private string _mapFilePath;
        private TreeNode<Type> _typeTreeNode;

        private Point? _firstSelectionPoint;
        private Point? _lastMousePosition;

        public MapViewForm()
        {
            InitializeComponent();

            RenderingCanvas.Initialize();
            RenderingCanvas.OnMouseGrabEvent = OnMouseGrab;

            if (TypeManager.Instance == null)
            {
                TypeManager.Instance = new TypeManager();
            }

            _typeTreeNode = TypeManager.Instance.TreeRoot;
            UpdateTypeList();
        }

        private void LoadMap(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            _editor = new TileEngineEditor(fs, RenderingCanvas);
            fs.Close();
            fs.Dispose();
            _mapFilePath = fileName;
        }

        private void CreateNewMap()
        {
            NewMapForm newMapForm = new NewMapForm();
            newMapForm.ShowDialog();
            if (newMapForm.ResultOk)
            {
                _editor = new TileEngineEditor(newMapForm.ResultWidth, newMapForm.ResultHeight, RenderingCanvas);
            }
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

        private void openMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenMapFile();
            UpdateImage();
        }
        
        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewMap();
            UpdateImage();
        }

        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMap(false);
            UpdateImage();
        }

        private void UpdateImage()
        {
            _editor?.UpdateGraphics();
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            _editor?.UpdateGraphics();
            if (_lastMousePosition != null)
                _editor?.HighlightContainingCell(_lastMousePosition.Value.X, _lastMousePosition.Value.Y);
            RenderingCanvas?.UpdateGraphics();
            HighlightSelectionRect();
        }

        private void OnMouseGrab(float dx, float dy)
        {
            if(_editor == null)
                return;

            bool gridEnabled = _editor.ShowGrid;
            _editor.ShowGrid = false;
            _editor.CameraPosition += new Vector2(-dx / 32, -dy / 32);
            _editor.ShowGrid = gridEnabled;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMap(true);
        }

        private void UpdateTypeList()
        {
            TileObjectsListView.Items.Clear();

            ListViewItem upItem = new ListViewItem();
            upItem.Name = "Up";
            upItem.Text = "..";

            TileObjectsListView.Items.Add(upItem);

            foreach (var typeNode in _typeTreeNode)
            {
                Type type = typeNode.Data;
                ListViewItem item = new ListViewItem();
                item.Text = type.Name;
                TileObjectsListView.Items.Add(item);
            }

            string path = TreeNode<Type>.GetPath(_typeTreeNode, t => t.Name);
            TypePathLabel.Text = path;
        }

        private Type GetSelectedTileObject()
        {
            if (TileObjectsListView.SelectedItems.Count == 0)
            {
                return null;
            }
            ListViewItem activatedItem = TileObjectsListView.SelectedItems[0];
            string itemTypeName = activatedItem.Text;

            TreeNode<Type> childNode = _typeTreeNode.FirstOrDefault(n => n.Data.Name == itemTypeName);
            if (childNode == null)
            {
                throw new InvalidOperationException("UI Exception");
            }

            return childNode.Data;
        }

        public Type SelectedTileObject => GetSelectedTileObject();

        private void TileObjectsListView_ItemActivate(object sender, EventArgs e)
        {
            ListView listView = (ListView) sender;
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

            TreeNode<Type> childNode = _typeTreeNode.FirstOrDefault(n => n.Data.Name == itemTypeName);

            if (childNode != null && childNode.Count > 0)
            {
                _typeTreeNode = childNode;
                UpdateTypeList();
            }
        }

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

                int deltaX = lastX - firstX;
                int deltaY = lastY - firstY;

                _editor?.HighlightRect(firstX, firstY, deltaX, deltaY);
            }
        }

        private void RenderingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            _lastMousePosition = new Point(e.X, e.Y);

            if (_editor != null)
            {
                _editor.GetPositionWithOffset(e.X, e.Y, out var cell, out var offset);
                CoordinateLabel.Text = $@"({cell.X}, {cell.Y}) : ({offset.X:0.00}, {offset.Y:0.00})";
            }
        }

        private void RenderingCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            Type selectedObjectType = SelectedTileObject;
            if (selectedObjectType == null)
            {
                return;
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

            int deltaX = lastX - firstX;
            int deltaY = lastY - firstY;

            _editor.SelectRectBySize(firstX, firstY, deltaX, deltaY);
        }
    }
}
