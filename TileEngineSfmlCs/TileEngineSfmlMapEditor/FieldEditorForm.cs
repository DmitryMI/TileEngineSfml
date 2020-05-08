using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.TypeManagement;
using TileEngineSfmlMapEditor.MapEditing;

namespace TileEngineSfmlMapEditor
{
    public partial class FieldEditorForm : Form
    {
        private TileEngineEditor _editor;
        private TileObject _targetTileObject;

        public FieldEditorForm(TileEngineEditor editor, TileObject targetTileObject)
        {
            InitializeComponent();

            _editor = editor;
            _targetTileObject = targetTileObject;
        }

        private void PopulateTable()
        {
            TileObjectFieldsView.CellValueChanged -= TileObjectFieldsView_CellValueChanged;

            var fieldDescriptors = _targetTileObject.GetEntityType().GetFieldDescriptors();

            foreach (var fieldDescriptor in fieldDescriptors)
            {
                string name = fieldDescriptor.Name;
                bool isRuntimeOnly = fieldDescriptor.IsRuntimeOnly;
                bool isReadOnly = fieldDescriptor.IsReadOnly;
                
                DataGridViewRow row = new DataGridViewRow();
                row.Tag = fieldDescriptor;

                DataGridViewCell nameCell = new DataGridViewTextBoxCell();
                nameCell.Value = name;

                DataGridViewCell isRuntimeCell = new DataGridViewTextBoxCell();
                DataGridViewCell isReadOnlyCell = new DataGridViewTextBoxCell();
                DataGridViewCell valueTypeCell = new DataGridViewTextBoxCell();

                DataGridViewCell valueCell;

                if (!fieldDescriptor.IsStringParseable)
                {
                    var buttonCell = new DataGridViewButtonCell();
                    valueCell = buttonCell;
                }
                else
                {
                    valueCell = new DataGridViewTextBoxCell();
                }

                row.Cells.Add(nameCell);
                row.Cells.Add(isRuntimeCell);
                row.Cells.Add(isReadOnlyCell);
                row.Cells.Add(valueTypeCell);
                row.Cells.Add(valueCell);

                if (isRuntimeOnly)
                {
                    isRuntimeCell.Value = fieldDescriptor.RuntimeOnlyMessage;
                    valueCell.ReadOnly = true;

                    nameCell.Style.BackColor = Color.Gray;
                    isRuntimeCell.Style.BackColor = Color.Gray;
                    isReadOnlyCell.Style.BackColor = Color.Gray;
                    valueTypeCell.Style.BackColor = Color.Gray;
                    valueCell.Style.BackColor = Color.Gray;
                }
                else
                {
                    isRuntimeCell.Value = false;
                    object value = fieldDescriptor.GetValue(_targetTileObject);
                    valueTypeCell.Value = value.GetType().Name;
                    valueCell.Value = value.ToString();
                }

                
                if (isReadOnly)
                {
                    isReadOnlyCell.Value = fieldDescriptor.ReadOnlyMessage;
                    valueCell.ReadOnly = true;

                    nameCell.Style.BackColor = Color.Gray;
                    isRuntimeCell.Style.BackColor = Color.Gray;
                    isReadOnlyCell.Style.BackColor = Color.Gray;
                    valueTypeCell.Style.BackColor = Color.Gray;
                    valueCell.Style.BackColor = Color.Gray;
                }
                else
                {
                    isReadOnlyCell.Value = false;
                }

               

                TileObjectFieldsView.Rows.Add(row);
            }

            TileObjectFieldsView.CellValueChanged += TileObjectFieldsView_CellValueChanged;
        }

        private void FieldEditorForm_Load(object sender, EventArgs e)
        {
            Text = _targetTileObject.VisibleName;
            PopulateTable();
        }

        private void TileObjectFieldsView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex == -1)
                return;
            if (e.ColumnIndex == ValueColumn.Index)
            {
                int rowIndex = e.RowIndex;
                var row = TileObjectFieldsView.Rows[rowIndex];
                FieldDescriptor fieldDescriptor = (FieldDescriptor) row.Tag;

                string value = (string) row.Cells[e.ColumnIndex].Value;
                try
                {
                    fieldDescriptor.ParseAndSet(_targetTileObject, value);
                }
                catch (FormatException exception)
                {
                    LogManager.EditorLogger.LogError("FieldEditor parsing failed. " + exception.Message);
                    Debug.WriteLine(exception.StackTrace);
                }

                TileObjectFieldsView.CellValueChanged -= TileObjectFieldsView_CellValueChanged;
                row.Cells[e.ColumnIndex].Value = fieldDescriptor.GetValue(_targetTileObject);
                TileObjectFieldsView.CellValueChanged += TileObjectFieldsView_CellValueChanged;
            }
        }

        private void EditValueClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
    }
}
