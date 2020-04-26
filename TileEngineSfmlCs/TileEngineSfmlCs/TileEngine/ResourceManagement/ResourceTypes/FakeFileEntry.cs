using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.TileEngine.ResourceManagement.ResourceTypes
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
