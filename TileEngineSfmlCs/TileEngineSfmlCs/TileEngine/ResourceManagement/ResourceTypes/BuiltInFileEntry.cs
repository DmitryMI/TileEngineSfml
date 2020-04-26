using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.TileEngine.ResourceManagement.ResourceTypes
{
    public class BuiltInFileEntry : IFileSystemEntry
    {
        private readonly string _path;
        private readonly string _name;
        private readonly bool _isDirectory;
        private Stream _stream;

        public BuiltInFileEntry(string path)
        {
            _path = path;

            if (File.Exists(path))
            {
                _isDirectory = false;
                FileInfo info = new FileInfo(path);
                _name = info.Name;
            }
            else if (Directory.Exists(path))
            {
                _isDirectory = true;
                DirectoryInfo info = new DirectoryInfo(path);
                _name = info.Name;
            }
        }

        public BuiltInFileEntry()
        {

        }

        public string AbsolutePath => _path;
        public string Name => _name;
        public bool IsDirectory => _isDirectory;
        public Stream OpenStream()
        {
            if (_isDirectory)
            {
                throw new InvalidOperationException($"Entry {_path} is a directory");
            }

            if (_stream == null)
            {
                _stream = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite);
            }
            else if (!_stream.CanRead)
            {
                _stream.Close();
                _stream.Dispose();
                _stream = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite);
            }
            return _stream;
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
