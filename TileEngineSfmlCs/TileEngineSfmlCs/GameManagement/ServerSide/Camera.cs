using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.ServerSide
{
    public class Camera
    {
        public TileObject TrackingTarget { get; set; }
        public Vector2Int Center { get; set; }
        public Vector2Int Size { get; set; }

        public Camera(TileObject trackingTarget, Vector2Int size)
        {
            TrackingTarget = trackingTarget;
            Size = size;
        }

        public Camera(Vector2Int center, Vector2Int size)
        {
            Center = center;
            Size = size;
        }
    }
}
