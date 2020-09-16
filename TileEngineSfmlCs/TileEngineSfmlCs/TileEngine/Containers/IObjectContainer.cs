using TileEngineSfmlCs.GameManagement;
using TileEngineSfmlCs.TileEngine.TileObjects;

namespace TileEngineSfmlCs.TileEngine.Containers
{
    public interface IObjectContainer : IPositionProvider
    {
        TileObject[] Contents { get; }

        float LightDecayFactor { get; }
    }
}