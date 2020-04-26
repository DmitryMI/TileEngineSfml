using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using TileEngineSfmlCs.TileEngine.Logging;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.Utils.Serialization.ZipContainer
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
                IFileSystemEntry file = new ZipFileEntry(entry);
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
                currentEntry = new TreeNode<IFileSystemEntry>(new ZipFileEntry(path[0]));
                root.Add(currentEntry);
            }

            fragmentIndex++;
            while (fragmentIndex < pathLength)
            {
                var parent = currentEntry;
                currentEntry = currentEntry.FirstOrDefault(n => n.Data.Name.Equals(path[fragmentIndex]));
                if (currentEntry == null)
                {
                    currentEntry = new TreeNode<IFileSystemEntry>(new ZipFileEntry(path[fragmentIndex]));
                    parent.Add(currentEntry);
                }
                fragmentIndex++;
            }

            return currentEntry;
        }
        
        public TreeNode<IFileSystemEntry> MapTree => _mapTreeNode;

        public IFileSystemEntry GetEntry(string path)
        {
            var entry = TreeNode<IFileSystemEntry>.SearchPath(_mapTreeNode, path, e => e.Name);
            return entry?.Data;
        }

        public IFileSystemEntry CreateEntry(string path)
        {
            string[] pathFragments = path.Split('\\');
            var parent =
                TreeNode<IFileSystemEntry>.SearchPath(_mapTreeNode, pathFragments, pathFragments.Length - 1,
                    f => f.Name);

            if (parent == null)
            {
                parent = CreatePath(pathFragments, pathFragments.Length - 1);
            }

            path = path.Replace('\\', '/');
            var entry = _mapZipArchive.CreateEntry(path, CompressionLevel.Optimal);
            IFileSystemEntry fs = new ZipFileEntry(entry);
            parent.Add(new TreeNode<IFileSystemEntry>(fs));
            return fs;
        }

        private TreeNode<IFileSystemEntry> CreatePath(string[] fragments, int count)
        {
            int fragIndex = 0;
            TreeNode<IFileSystemEntry> currentNode = _mapTreeNode;
            while (fragIndex < count)
            {
                var parent = currentNode;
                currentNode = currentNode.FirstOrDefault(c => c.Data.Name == fragments[fragIndex]);
                if (currentNode == null)
                {
                    TreeNode<IFileSystemEntry> folder = new TreeNode<IFileSystemEntry>(new ZipFileEntry(fragments[fragIndex]));
                    parent.Add(folder);
                    currentNode = folder;
                }

                fragIndex++;
            }

            return currentNode;
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
            string[] pathFragments = path.Split('\\');
            var entryNode =
                TreeNode<IFileSystemEntry>.SearchPath(_mapTreeNode, pathFragments, pathFragments.Length,
                    f => f.Name);

            if(entryNode == null)
                return;

            entryNode.ParentNode?.Remove(entryNode);

            path = path.Replace('\\', '/');
            var entry = _mapZipArchive.GetEntry(path);
            entry?.Delete();

            
        }
    }
}
