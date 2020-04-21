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
            this.MainTimer = new System.Windows.Forms.Timer(this.components);
            this.TileObjectsListView = new System.Windows.Forms.ListView();
            this.TileObjectsSearchBox = new System.Windows.Forms.TextBox();
            this.TypePathLabel = new System.Windows.Forms.Label();
            this.CoordinateLabel = new System.Windows.Forms.Label();
            this.RenderingCanvas = new TileEngineSfmlMapEditor.SfmlRenderControl();
            this.MainMenyStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenyStrip
            // 
            this.MainMenyStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenuItem,
            this.viewToolStripMenuItem});
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
            this.TileObjectsListView.Size = new System.Drawing.Size(220, 494);
            this.TileObjectsListView.TabIndex = 3;
            this.TileObjectsListView.UseCompatibleStateImageBehavior = false;
            this.TileObjectsListView.View = System.Windows.Forms.View.List;
            this.TileObjectsListView.ItemActivate += new System.EventHandler(this.TileObjectsListView_ItemActivate);
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
            // RenderingCanvas
            // 
            this.RenderingCanvas.BackColor = System.Drawing.Color.White;
            this.RenderingCanvas.Location = new System.Drawing.Point(238, 27);
            this.RenderingCanvas.Name = "RenderingCanvas";
            this.RenderingCanvas.OnMouseGrabEvent = null;
            this.RenderingCanvas.Size = new System.Drawing.Size(932, 541);
            this.RenderingCanvas.TabIndex = 2;
            this.RenderingCanvas.MouseClick += new System.Windows.Forms.MouseEventHandler(this.RenderingCanvas_MouseClick);
            this.RenderingCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RenderingCanvas_MouseDown);
            this.RenderingCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RenderingCanvas_MouseMove);
            this.RenderingCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RenderingCanvas_MouseUp);
            // 
            // MapViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 580);
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
    }
}

