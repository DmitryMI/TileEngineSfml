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
using SFML.System;
using SFML.Window;
using TileEngineSfmlCs.GameManagement.ClientSide.TileObjects;
using TileEngineSfmlCs.TileEngine.ResourceManagement;
using TileEngineSfmlCs.TileEngine.TimeManagement;
using TileEngineSfmlCs.Types;
using Color = SFML.Graphics.Color;
using Icon = TileEngineSfmlCs.Types.Icon;

namespace TileEngineSfmlCsClient.Rendering
{
    public partial class SfmlRenderControl : UserControl, ISpiritRenderer
    {
        private RenderWindow _renderWindow;
        public SfmlRenderControl()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            this.Name = "SfmlRenderControl";
            this.ResumeLayout(false);

            //VideoMode videoMode = new VideoMode(800, 600);
            //_renderWindow = new RenderWindow(videoMode, "Rendering text", Styles.Default);
            _renderWindow = new RenderWindow(this.Handle);

            TimeManager.Instance.NextFrameEvent += OnNextFrame;
        }

        public void PreRendering()
        {
            _renderWindow.Clear(Color.Black);
        }

        public void Render(Vector2 cameraPosition, Vector2 iconPosition, Icon icon)
        {
            for (int i = 0; i < icon.SpritesCount; i++)
            {
                var resourceEntry = GameResources.Instance.GetEntry(icon.ResourceIds[i]);
                ColorB color = icon.Colors[i];
                float scale = icon.Scales[i];
                if (resourceEntry.LoadedValue == null)
                {
                    byte[] textureBytes = resourceEntry.ToByteArray();
                    Texture texture = new Texture(textureBytes);
                    resourceEntry.LoadedValue = texture;
                }

                Sprite sprite = new Sprite((Texture) resourceEntry.LoadedValue)
                {
                    Color = new Color(color.R, color.G, color.B, color.A), Scale = new Vector2f(scale, scale)
                };
                //Vector2 projection = (iconPosition - cameraPosition) * 32.0f;
                var view = _renderWindow.GetView();
                view.Center = new Vector2f(cameraPosition.X * 32, cameraPosition.Y * 32);
                _renderWindow.SetView(view);
                sprite.Position = new Vector2f(iconPosition.X * 32, iconPosition.Y * 32);
                _renderWindow.Draw(sprite);
            }
        }

        public void PostRendering()
        {
            _renderWindow.Display();
        }


        public void OnNextFrame()
        {
           
        }
    }
}
