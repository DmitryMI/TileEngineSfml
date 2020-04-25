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
    public class ZipMapContainer : IMapContainer
    {
        private Stream _mapStream;
        private ZipArchive _mapZipArchive;
        private TreeNode<IFileSystemEntry> _mapTreeNode;
        private string _fileName;


        public ZipMapContainer(string fileName)
        {
            _mapStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _mapZipArchive = new ZipArchive(_mapStream, ZipArchiveMode.Update, true);
            CreateTree();
            _fileName = fileName;
        }

        public void Save()
        {
            _mapZipArchive.Dispose();
            _mapZipArchive = new ZipArchive(_mapStream);
        }

        public void Dispose()
        {
            try
            {
                _mapZipArchive?.Dispose();

                _mapStream?.Close();
                _mapStream?.Dispose();
                LogManager.EditorLogger.Log("[ZipMapContainer] Zip archive closed and disposed");
            }
            catch (Exception ex)
            {
                LogManager.EditorLogger.Log("[ZipMapContainer] Dispose exception: " + ex.Message);
            }
        }

        public bool CanWrite => _mapStream.CanWrite;

        private void CreateTree()
        {
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
            path = path.Replace('\\', '/');
            var entry = _mapZipArchive.CreateEntry(path, CompressionLevel.Optimal);
            return entry.Open();
        }

        public void UpdateTree()
        {
            CreateTree();
        }

        public TreeNode<IFileSystemEntry> GetTreeNode(string directoryPath)
        {
            var directory = TreeNode<IFileSystemEntry>.SearchPath(_mapTreeNode, directoryPath, entry => entry.Name);
            return directory;
        }

        public void Reload()
        {
            _mapZipArchive?.Dispose();
            _mapStream?.Close();
            _mapStream?.Dispose();

            _mapStream = new FileStream(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _mapZipArchive = new ZipArchive(_mapStream, ZipArchiveMode.Update, true);
            CreateTree();
        }

        public void DeleteEntry(string path)
        {
            path = path.Replace('\\', '/');
            var entry = _mapZipArchive.GetEntry(path);
            entry?.Delete();
        }
    }
}
