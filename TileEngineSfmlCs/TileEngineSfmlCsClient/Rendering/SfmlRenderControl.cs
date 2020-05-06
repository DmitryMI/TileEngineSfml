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
using TileEngineSfmlCs.GameManagement.BinaryEncoding.ControlInput;
using TileEngineSfmlCs.GameManagement.ClientControlInput;
using TileEngineSfmlCs.GameManagement.ClientSide;
using TileEngineSfmlCs.GameManagement.ClientSide.TileObjects;
using TileEngineSfmlCs.Networking;
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

        public void Render(Vector2 iconPosition, Icon icon, TileObjectSpirit spirit)
        {
            for (int i = 0; i < icon.SpritesCount; i++)
            {
                Sprite sprite;

                if (spirit.Tag == null)
                {
                    IconSpriteCache cache = new IconSpriteCache();
                    var resourceEntry = GameResources.Instance.GetEntry(icon.ResourceIds[i]);
                    ColorB color = icon.Colors[i];
                    float scale = icon.Scales[i];
                    if (resourceEntry.LoadedValue == null)
                    {
                        byte[] textureBytes = resourceEntry.ToByteArray();
                        Texture texture = new Texture(textureBytes);
                        resourceEntry.LoadedValue = texture;
                    }

                    sprite = new Sprite((Texture)resourceEntry.LoadedValue)
                    {
                        Color = new Color(color.R, color.G, color.B, color.A),
                        Scale = new Vector2f(scale, scale)
                    };

                    cache.Sprites.Add(sprite);
                    spirit.Tag = cache;
                    spirit.IconDirty = false;
                }
                
                if (spirit.IconDirty)
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

                    sprite = new Sprite((Texture)resourceEntry.LoadedValue)
                    {
                        Color = new Color(color.R, color.G, color.B, color.A),
                        Scale = new Vector2f(scale, scale)
                    };
                    ((IconSpriteCache) spirit.Tag).Sprites[i] = sprite;
                    spirit.IconDirty = false;
                }
                else
                {
                    sprite = ((IconSpriteCache)spirit.Tag).Sprites[i];
                }

                if (spirit.LocationDirty)
                {
                    sprite.Position = new Vector2f(iconPosition.X * 32, iconPosition.Y * 32);
                    spirit.LocationDirty = false;
                }

                _renderWindow.Draw(sprite);
            }
        }

        public void SetViewCenter(Vector2 viewCenter)
        {
            var view = _renderWindow.GetView();
            view.Center = new Vector2f(viewCenter.X * 32, viewCenter.Y * 32);
            _renderWindow.SetView(view);
        }

        public void PostRendering()
        {
            _renderWindow.Display();
        }


        public void OnNextFrame()
        {
            InputKeyState ctrl = Keyboard.IsKeyPressed(Keyboard.Key.LControl) ? InputKeyState.KeyDown : InputKeyState.KeyUp;
            InputKeyState shift = Keyboard.IsKeyPressed(Keyboard.Key.LShift) ? InputKeyState.KeyDown : InputKeyState.KeyUp;
            InputKeyState alt = Keyboard.IsKeyPressed(Keyboard.Key.LAlt) ? InputKeyState.KeyDown : InputKeyState.KeyUp;
            /*
            InputKeyState up = Keyboard.IsKeyPressed(Keyboard.Key.W) ? InputKeyState.KeyDown : InputKeyState.KeyUp;
            InputKeyState right = Keyboard.IsKeyPressed(Keyboard.Key.D) ? InputKeyState.KeyDown : InputKeyState.KeyUp;
            InputKeyState down = Keyboard.IsKeyPressed(Keyboard.Key.S) ? InputKeyState.KeyDown : InputKeyState.KeyUp;
            InputKeyState left = Keyboard.IsKeyPressed(Keyboard.Key.A) ? InputKeyState.KeyDown : InputKeyState.KeyUp;
            */
            MovementKey movementKey = MovementKey.None;
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                movementKey = MovementKey.Up;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                movementKey = MovementKey.Right;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                movementKey = MovementKey.Down;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                movementKey = MovementKey.Left;
            }

            if (movementKey != MovementKey.None)
            {
                ControlInputPackage updatePackage = new ControlInputPackage(movementKey);
                ClientNetworkManager.Instance?.SendUserInput(updatePackage, Reliability.Unreliable);
            }
        }
    }
}
