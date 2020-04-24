using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.IO.Compression;
using TileEngineSfmlCs.ResourceManagement;
using TileEngineSfmlCs.TileEngine.Logging;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.Utils.Serialization
{
    public class TileEngineMap : IDisposable
    {
        private Stream _mapStream;
        private ZipArchive _mapZipArchive;
        private TreeNode<IFileSystemEntry> _mapTreeNode;


        public TileEngineMap(string fileName)
        {
            _mapStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _mapZipArchive = new ZipArchive(_mapStream, ZipArchiveMode.Update, true);
            CreateTree();
        }

        public TileEngineMap(Stream stream)
        {
            _mapStream = stream;
            _mapZipArchive = new ZipArchive(stream, ZipArchiveMode.Update, true);
            CreateTree();
        }

        public void Save()
        {
            _mapZipArchive.Dispose();
            _mapStream.Flush();
            _mapStream.Seek(0, SeekOrigin.Begin);
            _mapZipArchive = new ZipArchive(_mapStream);
            CreateTree();
        }

        public void Dispose()
        {
            _mapStream?.Dispose();
           _mapZipArchive?.Dispose();
        }

        public bool CanWrite => _mapStream.CanWrite;

        private void CreateTree()
        {
            GameResources.Instance?.RemoveDirectory("User");
            _mapTreeNode = new TreeNode<IFileSystemEntry>();
            foreach (var entry in _mapZipArchive.Entries)
            {
                if(entry.CompressedLength == 0)
                    continue;
                string[] pathFragments = entry.FullName.Split('/');
                var directory = TraverseDirectories(_mapTreeNode, pathFragments,  pathFragments.Length - 1);
                LogManager.EditorLogger.Log($"[TileEngineMap] Pushing {entry.FullName} into {directory}");
                LogManager.RuntimeLogger.Log($"[TileEngineMap] Pushing {entry.FullName} into {directory}");
                IFileSystemEntry file = new MapFileEntry(entry);
                directory.Add(new TreeNode<IFileSystemEntry>(file));
            }

            GameResources.Instance?.AppendToRoot(_mapTreeNode, "User");
        }

        private TreeNode<IFileSystemEntry> TraverseDirectories(TreeNode<IFileSystemEntry> root, string[] path,  int pathLength)
        {
            if (pathLength == 0)
                return root;
            int fragmentIndex = 0;
            TreeNode<IFileSystemEntry> currentEntry = root.FirstOrDefault(e => e.Data.Name.Equals(path[0]));
            if (currentEntry == null)
            {
                currentEntry = new TreeNode<IFileSystemEntry>(new MapFileEntry(path[0]));
                root.Add(currentEntry);
            }

            fragmentIndex++;
            while (fragmentIndex < pathLength)
            {
                var parent = currentEntry;
                currentEntry = currentEntry.FirstOrDefault(n => n.Data.Name.Equals(path[fragmentIndex]));
                if (currentEntry == null)
                {
                    currentEntry = new TreeNode<IFileSystemEntry>(new MapFileEntry(path[fragmentIndex]));
                    parent.Add(currentEntry);
                }
                fragmentIndex++;
            }

            return currentEntry;
        }
        
        public TreeNode<IFileSystemEntry> MapTree => _mapTreeNode;

        public Stream GetEntry(string path)
        {
            var entry = TreeNode<IFileSystemEntry>.SearchPath(_mapTreeNode, path, e => e.Name);
            return entry?.Data?.GetStream();
        }

        public Stream CreateEntry(string path)
        {
            var entry = _mapZipArchive.CreateEntry(path, CompressionLevel.Optimal);
            CreateTree();
            return entry.Open();
        }
    }
}
