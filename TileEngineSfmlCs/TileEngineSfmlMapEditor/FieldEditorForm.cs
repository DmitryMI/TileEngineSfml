using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TileEngineSfmlCs.TileEngine.TileObjects;
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
            var fieldDescriptors = _targetTileObject.GetEntityType().GetFieldDescriptors();

            foreach (var fieldDescriptor in fieldDescriptors)
            {
                string name = fieldDescriptor.Name;
                bool isRuntimeOnly = fieldDescriptor.IsRuntimeOnly;
                bool isReadOnly = fieldDescriptor.IsReadOnly;
                
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewCell nameCell = new DataGridViewTextBoxCell();
                nameCell.Value = name;

                DataGridViewCell isRuntimeCell = new DataGridViewTextBoxCell();
                DataGridViewCell isReadOnlyCell = new DataGridViewTextBoxCell();
                DataGridViewCell valueTypeCell = new DataGridViewTextBoxCell();
                DataGridViewCell valueCell = new DataGridViewTextBoxCell();

                row.Cells.Add(nameCell);
                row.Cells.Add(isRuntimeCell);
                row.Cells.Add(isReadOnlyCell);
                row.Cells.Add(valueTypeCell);
                row.Cells.Add(valueCell);

                if (isRuntimeOnly)
                {
                    isRuntimeCell.Value = fieldDescriptor.RuntimeOnlyMessage;
                    valueCell.ReadOnly = true;
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
                }
                else
                {
                    isReadOnlyCell.Value = false;
                }

               

                TileObjectFieldsView.Rows.Add(row);
            }
        }

        private void FieldEditorForm_Load(object sender, EventArgs e)
        {
            Text = _targetTileObject.VisibleName;
            PopulateTable();
        }
    }
}
