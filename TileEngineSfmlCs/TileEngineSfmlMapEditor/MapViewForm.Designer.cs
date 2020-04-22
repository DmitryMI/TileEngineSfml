namespace TileEngineSfmlMapEditor
{
    partial class MapViewForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MainMenyStrip = new System.Windows.Forms.MenuStrip();
            this.FileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.snapToCellToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layerVisibleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layersActiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainTimer = new System.Windows.Forms.Timer(this.components);
            this.TileObjectsListView = new System.Windows.Forms.ListView();
            this.TileObjectsSearchBox = new System.Windows.Forms.TextBox();
            this.TypePathLabel = new System.Windows.Forms.Label();
            this.CoordinateLabel = new System.Windows.Forms.Label();
            this.SelectedTypePreviewBox = new System.Windows.Forms.PictureBox();
            this.SelectedTypeNameBox = new System.Windows.Forms.Label();
            this.MainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.ErrorLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.UnderCursorLabel = new System.Windows.Forms.Label();
            this.RenderingCanvas = new TileEngineSfmlMapEditor.SfmlRenderControl();
            this.MapEditorMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.makeShitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenyStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedTypePreviewBox)).BeginInit();
            this.MainStatusStrip.SuspendLayout();
            this.MapEditorMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenyStrip
            // 
            this.MainMenyStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenuItem,
            this.viewToolStripMenuItem,
            this.preferencesToolStripMenuItem,
            this.layerVisibleMenuItem,
            this.layersActiveToolStripMenuItem});
            this.MainMenyStrip.Location = new System.Drawing.Point(0, 0);
            this.MainMenyStrip.Name = "MainMenyStrip";
            this.MainMenyStrip.Size = new System.Drawing.Size(1182, 24);
            this.MainMenyStrip.TabIndex = 1;
            this.MainMenyStrip.Text = "menuStrip1";
            // 
            // FileMenuItem
            // 
            this.FileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMapToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.saveMapToolStripMenuItem,
            this.openMapToolStripMenuItem});
            this.FileMenuItem.Name = "FileMenuItem";
            this.FileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.FileMenuItem.Text = "File";
            // 
            // newMapToolStripMenuItem
            // 
            this.newMapToolStripMenuItem.Name = "newMapToolStripMenuItem";
            this.newMapToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.newMapToolStripMenuItem.Text = "New map";
            this.newMapToolStripMenuItem.Click += new System.EventHandler(this.newMapToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.saveAsToolStripMenuItem.Text = "Save map as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // saveMapToolStripMenuItem
            // 
            this.saveMapToolStripMenuItem.Name = "saveMapToolStripMenuItem";
            this.saveMapToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.saveMapToolStripMenuItem.Text = "Save map";
            this.saveMapToolStripMenuItem.Click += new System.EventHandler(this.saveMapToolStripMenuItem_Click);
            // 
            // openMapToolStripMenuItem
            // 
            this.openMapToolStripMenuItem.Name = "openMapToolStripMenuItem";
            this.openMapToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.openMapToolStripMenuItem.Text = "Open map";
            this.openMapToolStripMenuItem.Click += new System.EventHandler(this.openMapToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showGridToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // showGridToolStripMenuItem
            // 
            this.showGridToolStripMenuItem.CheckOnClick = true;
            this.showGridToolStripMenuItem.Name = "showGridToolStripMenuItem";
            this.showGridToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.showGridToolStripMenuItem.Text = "Show grid";
            this.showGridToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showGridToolStripMenuItem_CheckedChanged);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.snapToCellToolStripMenuItem});
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            // 
            // snapToCellToolStripMenuItem
            // 
            this.snapToCellToolStripMenuItem.Checked = true;
            this.snapToCellToolStripMenuItem.CheckOnClick = true;
            this.snapToCellToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.snapToCellToolStripMenuItem.Name = "snapToCellToolStripMenuItem";
            this.snapToCellToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.snapToCellToolStripMenuItem.Text = "Snap to cell";
            // 
            // layerVisibleMenuItem
            // 
            this.layerVisibleMenuItem.Enabled = false;
            this.layerVisibleMenuItem.Name = "layerVisibleMenuItem";
            this.layerVisibleMenuItem.Size = new System.Drawing.Size(88, 20);
            this.layerVisibleMenuItem.Text = "Layers visible";
            // 
            // layersActiveToolStripMenuItem
            // 
            this.layersActiveToolStripMenuItem.Enabled = false;
            this.layersActiveToolStripMenuItem.Name = "layersActiveToolStripMenuItem";
            this.layersActiveToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.layersActiveToolStripMenuItem.Text = "Layers active";
            // 
            // MainTimer
            // 
            this.MainTimer.Enabled = true;
            this.MainTimer.Interval = 1;
            this.MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
            // 
            // TileObjectsListView
            // 
            this.TileObjectsListView.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.TileObjectsListView.HideSelection = false;
            this.TileObjectsListView.Location = new System.Drawing.Point(12, 74);
            this.TileObjectsListView.MultiSelect = false;
            this.TileObjectsListView.Name = "TileObjectsListView";
            this.TileObjectsListView.ShowGroups = false;
            this.TileObjectsListView.Size = new System.Drawing.Size(220, 456);
            this.TileObjectsListView.TabIndex = 3;
            this.TileObjectsListView.UseCompatibleStateImageBehavior = false;
            this.TileObjectsListView.View = System.Windows.Forms.View.List;
            this.TileObjectsListView.ItemActivate += new System.EventHandler(this.TileObjectsListView_ItemActivate);
            this.TileObjectsListView.SelectedIndexChanged += new System.EventHandler(this.TileObjectsListView_SelectedIndexChanged);
            // 
            // TileObjectsSearchBox
            // 
            this.TileObjectsSearchBox.Location = new System.Drawing.Point(12, 31);
            this.TileObjectsSearchBox.Name = "TileObjectsSearchBox";
            this.TileObjectsSearchBox.Size = new System.Drawing.Size(220, 20);
            this.TileObjectsSearchBox.TabIndex = 4;
            // 
            // TypePathLabel
            // 
            this.TypePathLabel.AutoSize = true;
            this.TypePathLabel.Location = new System.Drawing.Point(12, 58);
            this.TypePathLabel.Name = "TypePathLabel";
            this.TypePathLabel.Size = new System.Drawing.Size(12, 13);
            this.TypePathLabel.TabIndex = 5;
            this.TypePathLabel.Text = "/";
            // 
            // CoordinateLabel
            // 
            this.CoordinateLabel.Location = new System.Drawing.Point(987, 9);
            this.CoordinateLabel.Name = "CoordinateLabel";
            this.CoordinateLabel.Size = new System.Drawing.Size(183, 13);
            this.CoordinateLabel.TabIndex = 6;
            this.CoordinateLabel.Text = "(X, Y) : (x, y)";
            this.CoordinateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectedTypePreviewBox
            // 
            this.SelectedTypePreviewBox.Location = new System.Drawing.Point(12, 536);
            this.SelectedTypePreviewBox.Name = "SelectedTypePreviewBox";
            this.SelectedTypePreviewBox.Size = new System.Drawing.Size(32, 32);
            this.SelectedTypePreviewBox.TabIndex = 7;
            this.SelectedTypePreviewBox.TabStop = false;
            // 
            // SelectedTypeNameBox
            // 
            this.SelectedTypeNameBox.AutoSize = true;
            this.SelectedTypeNameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SelectedTypeNameBox.Location = new System.Drawing.Point(47, 543);
            this.SelectedTypeNameBox.Name = "SelectedTypeNameBox";
            this.SelectedTypeNameBox.Size = new System.Drawing.Size(35, 20);
            this.SelectedTypeNameBox.TabIndex = 8;
            this.SelectedTypeNameBox.Text = "N/A";
            // 
            // MainStatusStrip
            // 
            this.MainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ErrorLabel});
            this.MainStatusStrip.Location = new System.Drawing.Point(0, 572);
            this.MainStatusStrip.Name = "MainStatusStrip";
            this.MainStatusStrip.Size = new System.Drawing.Size(1182, 22);
            this.MainStatusStrip.TabIndex = 9;
            this.MainStatusStrip.Text = "statusStrip1";
            // 
            // ErrorLabel
            // 
            this.ErrorLabel.Name = "ErrorLabel";
            this.ErrorLabel.Size = new System.Drawing.Size(23, 17);
            this.ErrorLabel.Text = "OK";
            // 
            // UnderCursorLabel
            // 
            this.UnderCursorLabel.Location = new System.Drawing.Point(801, 9);
            this.UnderCursorLabel.Name = "UnderCursorLabel";
            this.UnderCursorLabel.Size = new System.Drawing.Size(180, 13);
            this.UnderCursorLabel.TabIndex = 10;
            this.UnderCursorLabel.Text = "N/A";
            this.UnderCursorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RenderingCanvas
            // 
            this.RenderingCanvas.BackColor = System.Drawing.Color.White;
            this.RenderingCanvas.Location = new System.Drawing.Point(238, 27);
            this.RenderingCanvas.Name = "RenderingCanvas";
            this.RenderingCanvas.OnMouseGrabEvent = null;
            this.RenderingCanvas.Size = new System.Drawing.Size(932, 541);
            this.RenderingCanvas.TabIndex = 2;
            this.RenderingCanvas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RenderingCanvas_KeyDown);
            this.RenderingCanvas.MouseClick += new System.Windows.Forms.MouseEventHandler(this.RenderingCanvas_MouseClick);
            this.RenderingCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RenderingCanvas_MouseDown);
            this.RenderingCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RenderingCanvas_MouseMove);
            this.RenderingCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RenderingCanvas_MouseUp);
            // 
            // MapEditorMenuStrip
            // 
            this.MapEditorMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.makeShitToolStripMenuItem});
            this.MapEditorMenuStrip.Name = "MapEditorMenuStrip";
            this.MapEditorMenuStrip.Size = new System.Drawing.Size(126, 26);
            // 
            // makeShitToolStripMenuItem
            // 
            this.makeShitToolStripMenuItem.Name = "makeShitToolStripMenuItem";
            this.makeShitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.makeShitToolStripMenuItem.Text = "Make shit";
            // 
            // MapViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 594);
            this.Controls.Add(this.UnderCursorLabel);
            this.Controls.Add(this.MainStatusStrip);
            this.Controls.Add(this.SelectedTypeNameBox);
            this.Controls.Add(this.SelectedTypePreviewBox);
            this.Controls.Add(this.CoordinateLabel);
            this.Controls.Add(this.TypePathLabel);
            this.Controls.Add(this.TileObjectsSearchBox);
            this.Controls.Add(this.TileObjectsListView);
            this.Controls.Add(this.RenderingCanvas);
            this.Controls.Add(this.MainMenyStrip);
            this.MainMenuStrip = this.MainMenyStrip;
            this.Name = "MapViewForm";
            this.Text = "Map view";
            this.MainMenyStrip.ResumeLayout(false);
            this.MainMenyStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedTypePreviewBox)).EndInit();
            this.MainStatusStrip.ResumeLayout(false);
            this.MainStatusStrip.PerformLayout();
            this.MapEditorMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip MainMenyStrip;
        private System.Windows.Forms.ToolStripMenuItem FileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newMapToolStripMenuItem;
        private SfmlRenderControl RenderingCanvas;
        private System.Windows.Forms.Timer MainTimer;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ListView TileObjectsListView;
        private System.Windows.Forms.TextBox TileObjectsSearchBox;
        private System.Windows.Forms.Label TypePathLabel;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGridToolStripMenuItem;
        private System.Windows.Forms.Label CoordinateLabel;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem snapToCellToolStripMenuItem;
        private System.Windows.Forms.PictureBox SelectedTypePreviewBox;
        private System.Windows.Forms.Label SelectedTypeNameBox;
        private System.Windows.Forms.StatusStrip MainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel ErrorLabel;
        private System.Windows.Forms.ToolStripMenuItem layerVisibleMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layersActiveToolStripMenuItem;
        private System.Windows.Forms.Label UnderCursorLabel;
        private System.Windows.Forms.ContextMenuStrip MapEditorMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem makeShitToolStripMenuItem;
    }
}

