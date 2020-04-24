using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TileEngineSfmlMapEditor.MapEditing;
using View = SFML.Graphics.View;

namespace TileEngineSfmlMapEditor
{
    public class SfmlRenderControl : UserControl, IRenderReceiver
    {
        private RenderWindow _renderWindow;
        private Queue<Drawable> _renderingQueue = new Queue<Drawable>();
        private Vector2f _mousePrevPosition;

        public Action<float, float> OnMouseGrabEvent { get; set; }

        public RenderWindow RenderWindow
        {
            get => _renderWindow;
        }

        public void UpdateGraphics()
        {
            DrawingIteration();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (_renderWindow == null)
                base.OnPaint(e);
            else
            {
                
            }
        }

        private void DrawStuff()
        {
            lock (_renderingQueue)
            {
                while (_renderingQueue.Count > 0)
                {
                    Drawable drawable = _renderingQueue.Dequeue();
                    _renderWindow.Draw(drawable);
                }
            }
        }

        public Vector2f PixelToSfml(Vector2i pixel)
        {
            return _renderWindow.MapPixelToCoords(pixel);
        }

        private void DrawingIteration()
        {
            _renderWindow.DispatchEvents();
            _renderWindow.Clear();
            DrawStuff();
            _renderWindow.Display();
        }

        public void Initialize()
        {
            this.SuspendLayout();
            // 
            // SfmlRenderControl
            // 
            this.Name = "SfmlRenderControl";
            this.ResumeLayout(false);

            VideoMode videoMode = new VideoMode((uint)this.Size.Width, (uint)this.Size.Height);
            //_renderWindow = new RenderWindow(videoMode, "Rendering", Styles.None);
            _renderWindow = new RenderWindow(this.Handle);
        }

        public void DrawSprite(Drawable drawable)
        {
            lock (_renderingQueue)
            {
                _renderingQueue.Enqueue(drawable);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Middle) != 0)
            {
                float deltaX = e.X - _mousePrevPosition.X;
                float deltaY = e.Y - _mousePrevPosition.Y;
                OnMouseGrabEvent?.Invoke(deltaX, deltaY);
            }
            _mousePrevPosition = new Vector2f(e.X, e.Y);
            
            base.OnMouseMove(e);
        }

        public View RenderView => _renderWindow.GetView();
    }
}
