using System;
using System.IO;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.TileEngine.ResourceManagement.ResourceTypes
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

        public ResourceEntry(int resourceId, IFileSystemEntry fileSystemEntry)
        {
            ResourceId = resourceId;
            if (fileSystemEntry != null)
            {
                IsDirectory = fileSystemEntry.IsDirectory;
                Name = fileSystemEntry.Name;
                if (!IsDirectory)
                {
                    using (Stream stream = fileSystemEntry.OpenStream())
                    {
                        CopyStream(stream);
                    }
                }
            }
            else
            {
                Name = null;
                IsDirectory = true;
            }
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
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }

        public byte[] ToByteArray()
        {
            byte[] data = new byte[_stream.Length];
            ToByteArray(data, 0);
            return data;
        }

        public void ToByteArray(byte[] array, int position)
        {
            _stream.Seek(0, SeekOrigin.Begin);
            _stream.Read(array, position, (int)_stream.Length);
            _stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
