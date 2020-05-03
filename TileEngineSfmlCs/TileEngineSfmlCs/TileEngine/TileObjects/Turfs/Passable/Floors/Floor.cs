using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Turfs.Passable.Floors
{
    public abstract class Floor : PassableTurf
    {
        public override TileLayer Layer => TileLayer.Floor;
    }
}
