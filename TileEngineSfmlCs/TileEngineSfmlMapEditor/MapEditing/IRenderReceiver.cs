using SFML.Graphics;
using SFML.System;

namespace TileEngineSfmlMapEditor.MapEditing
{
    public interface IRenderReceiver
    {
        void DrawSprite(Drawable sprite);
        View RenderView { get; }

        Vector2f PixelToSfml(Vector2i pixel);
    }
}