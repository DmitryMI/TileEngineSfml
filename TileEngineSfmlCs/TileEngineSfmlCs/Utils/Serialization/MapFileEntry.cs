using System;
using System.IO;
using System.IO.Compression;

namespace TileEngineSfmlCs.Utils.Serialization
{
    public class MapFileEntry : IFileSystemEntry
    {
        private readonly ZipArchiveEntry _zipArchiveEntry;
        private readonly string _name;
        private readonly string _path;

        /// <summary>
        /// MapFileEntry represents files in .temap archive
        /// </summary>
        /// <param name="entry">Must not be null</param>
        public MapFileEntry(ZipArchiveEntry entry)
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

        /// <summary>
        /// Creates directory
        /// </summary>
        public MapFileEntry(string name)
        {
            _zipArchiveEntry = null;
            _name = name;
        }

        public string Name => _name;
        public bool IsDirectory => _zipArchiveEntry == null;

        public Stream GetStream()
        {
            return _zipArchiveEntry?.Open();
        }
    }
}
