using System;
using System.Windows.Forms;

namespace TileEngineSfmlMapEditor
{
    public partial class NewMapForm : Form
    {
        public NewMapForm()
        {
            InitializeComponent();
        }

        public bool ResultOk { get; private set; }
        public int ResultWidth { get; private set; }
        public int ResultHeight { get; private set; }


        private void OkButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(WidthBox.Text, out int width))
            {
                MessageBox.Show("Format error in width input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(HeightBox.Text, out int height))
            {
                MessageBox.Show("Format error in height input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ResultOk = true;
            ResultWidth = width;
            ResultHeight = height;

            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            ResultOk = false;
            Close();
        }
    }
}
