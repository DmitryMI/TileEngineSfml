using System;
using System.IO;

namespace TileEngineSfmlCs.ResourceManagement.ResourceTypes
{
    public class ResourceEntry : IDisposable
    {
        private MemoryStream _stream;

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
            IsDirectory = false;
            ResourceId = resourceId;
            Name = name;
            CopyStream(stream);
        }

        private void CopyStream(Stream stream)
        {
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            _stream = new MemoryStream();
            _stream.Write(data, 0, data.Length);
            _stream.Seek(0, SeekOrigin.Begin);
            stream.Flush();
            stream.Close();
            stream.Dispose();
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
