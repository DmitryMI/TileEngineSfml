using System;
using System.IO;
using System.IO.Compression;

namespace TileEngineSfmlCs.Utils.Serialization.ZipContainer
{
    public class ZipFileEntry : IFileSystemEntry
    {
        private readonly ZipArchiveEntry _zipArchiveEntry;
        private readonly string _name;
        private readonly string _path;
        private Stream _stream;

        /// <summary>
        /// MapFileEntry represents files in .temap archive
        /// </summary>
        /// <param name="entry">Must not be null</param>
        public ZipFileEntry(ZipArchiveEntry entry)
        {
            if (entry != null)
            {
                _zipArchiveEntry = entry;
                _name = _zipArchiveEntry.Name;
                _path = _zipArchiveEntry.FullName;
            }
            else
            {
                throw new ArgumentNullException(nameof(entry));
            }
        }

        public override string ToString()
        {
            return _name;
        }

        /// <summary>
        /// Creates directory
        /// </summary>
        public ZipFileEntry(string name)
        {
            _zipArchiveEntry = null;
            _name = name;
        }

        public string Name => _name;
        public bool IsDirectory => _zipArchiveEntry == null;

        public Stream OpenStream()
        {
            if (_stream == null)
            {
                _stream = _zipArchiveEntry?.Open();
            }
            else if (!_stream.CanRead)
            {
                _stream.Close();
                _stream = _zipArchiveEntry?.Open();
            }
            return _stream;
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
