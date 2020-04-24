using System.IO;

namespace TileEngineSfmlCs.Utils.Serialization
{
    public interface IFileSystemEntry
    {
        string Name { get; }
        bool IsDirectory { get; }
        Stream GetStream();
    }
}
