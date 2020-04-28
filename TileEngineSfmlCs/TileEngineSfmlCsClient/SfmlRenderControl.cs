using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFML.Graphics;
using SFML.Window;
using TileEngineSfmlCs.TileEngine.TimeManagement;
using Color = SFML.Graphics.Color;

namespace TileEngineSfmlCsClient
{
    public partial class SfmlRenderControl : UserControl
    {
        private RenderWindow _renderWindow;
        public SfmlRenderControl()
        {
            InitializeComponent();

            VideoMode videoMode = new VideoMode((uint)Size.Width, (uint)Size.Height);
            _renderWindow = new RenderWindow(videoMode, "TileEngine", Styles.None);

            TimeManager.Instance.NextFrameEvent += OnNextFrame;
        }

        public void OnNextFrame()
        {
            _renderWindow.Clear(Color.Black);
            // Drawing
            _renderWindow.Display();
        }
    }
}
