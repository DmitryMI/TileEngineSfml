using System;
using System.IO;

namespace TileEngineSfmlCs.ResourceManagement.ResourceTypes
{
    public class ResourceEntry : IDisposable
    {
        private Stream _stream;

        public bool IsDirectory { get; }

        public int ResourceId { get;  }
        public string Name { get; }

        public object LoadedValue { get; set; }

        public override string ToString()
        {
            return $"{Name}({ResourceId})";
        }

        public Stream DataStream => _stream;

        public ResourceEntry(int resourceId, string name, Stream stream)
        {
            IsDirectory = _stream == null;
            ResourceId = resourceId;
            Name = name;
            _stream = stream;
        }

        /// <summary>
        /// Creates directory
        /// </summary>
        /// <param name="resourceId">Resource's Id</param>
        /// <param name="name">Name</param>
        public ResourceEntry(int resourceId, string name)
        {
            IsDirectory = true;
            ResourceId = resourceId;
            Name = name;
            _stream = null;
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
