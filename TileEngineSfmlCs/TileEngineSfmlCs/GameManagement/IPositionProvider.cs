using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement
{
    public interface IPositionProvider
    {
        int InstanceId { get; }
        Vector2Int Position { get; }
        Vector2 Offset { get; }
    }
}