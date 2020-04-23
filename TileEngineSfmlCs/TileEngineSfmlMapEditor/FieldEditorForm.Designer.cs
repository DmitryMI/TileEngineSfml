namespace TileEngineSfmlMapEditor
{
    partial class FieldEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TileObjectFieldsView = new System.Windows.Forms.DataGridView();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsRuntimeOnlyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsReadOnlyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.TileObjectFieldsView)).BeginInit();
            this.SuspendLayout();
            // 
            // TileObjectFieldsView
            // 
            this.TileObjectFieldsView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TileObjectFieldsView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.IsRuntimeOnlyColumn,
            this.IsReadOnlyColumn,
            this.DataTypeColumn,
            this.ValueColumn});
            this.TileObjectFieldsView.Location = new System.Drawing.Point(12, 12);
            this.TileObjectFieldsView.Name = "TileObjectFieldsView";
            this.TileObjectFieldsView.Size = new System.Drawing.Size(608, 459);
            this.TileObjectFieldsView.TabIndex = 0;
            this.TileObjectFieldsView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.EditValueClick);
            this.TileObjectFieldsView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.TileObjectFieldsView_CellValueChanged);
            // 
            // NameColumn
            // 
            this.NameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.NameColumn.HeaderText = "Name";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            this.NameColumn.Width = 60;
            // 
            // IsRuntimeOnlyColumn
            // 
            this.IsRuntimeOnlyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.IsRuntimeOnlyColumn.HeaderText = "Is Runtime Only";
            this.IsRuntimeOnlyColumn.Name = "IsRuntimeOnlyColumn";
            this.IsRuntimeOnlyColumn.ReadOnly = true;
            this.IsRuntimeOnlyColumn.Width = 97;
            // 
            // IsReadOnlyColumn
            // 
            this.IsReadOnlyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.IsReadOnlyColumn.HeaderText = "Is Read Only";
            this.IsReadOnlyColumn.Name = "IsReadOnlyColumn";
            this.IsReadOnlyColumn.ReadOnly = true;
            this.IsReadOnlyColumn.Width = 86;
            // 
            // DataTypeColumn
            // 
            this.DataTypeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DataTypeColumn.HeaderText = "Data Type";
            this.DataTypeColumn.Name = "DataTypeColumn";
            this.DataTypeColumn.ReadOnly = true;
            this.DataTypeColumn.Width = 76;
            // 
            // ValueColumn
            // 
            this.ValueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ValueColumn.HeaderText = "Value";
            this.ValueColumn.Name = "ValueColumn";
            // 
            // FieldEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 483);
            this.Controls.Add(this.TileObjectFieldsView);
            this.Name = "FieldEditorForm";
            this.Text = "Field editor";
            this.Load += new System.EventHandler(this.FieldEditorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.TileObjectFieldsView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView TileObjectFieldsView;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsRuntimeOnlyColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsReadOnlyColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataTypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValueColumn;
    }
}