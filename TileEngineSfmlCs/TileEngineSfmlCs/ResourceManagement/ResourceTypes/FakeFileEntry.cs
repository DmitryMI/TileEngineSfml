using System;
using System.IO;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.ResourceManagement.ResourceTypes
{
    public class FakeFileEntry : IFileSystemEntry
    {
        public FakeFileEntry(string name, bool isDirectory)
        {
            Name = name;
            IsDirectory = isDirectory;
        }
        public void Dispose()
        {
            
        }

        public string Name { get; set; }
        public bool IsDirectory { get; set; }
        public Stream OpenStream()
        {
            throw new NotImplementedException();
        }
    }
}
