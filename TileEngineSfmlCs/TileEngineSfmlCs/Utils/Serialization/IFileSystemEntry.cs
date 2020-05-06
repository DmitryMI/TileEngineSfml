using System;
using System.IO;

namespace TileEngineSfmlCs.Utils.Serialization
{
    public interface IFileSystemEntry : IDisposable
    {
        string Name { get; }
        bool IsDirectory { get; }
        Stream OpenStream();
    }
}
