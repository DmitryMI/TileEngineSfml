using System;
using System.Collections.Generic;
using System.IO;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.Utils.Serialization.FolderContainer
{
    public class FolderMapContainer : IMapContainer
    {
        private DirectoryInfo _directoryInfo;
        private TreeNode<IFileSystemEntry> _treeNode;
        private List<IFileSystemEntry> _registeredEntries = new List<IFileSystemEntry>();
        public FolderMapContainer(string path)
        {
            _directoryInfo = new DirectoryInfo(path);
            if (!_directoryInfo.Exists)
            {
                throw new ArgumentException($"File system entry {path} does not exist");
            }

            CreateTree();
        }

        private void CreateTree()
        {
            _treeNode = new TreeNode<IFileSystemEntry>();
            foreach (var entry in _directoryInfo.EnumerateFileSystemInfos())
            {
                TraverseFileSystem(_treeNode, entry);
            }
        }
        
        private void TraverseFileSystem(TreeNode<IFileSystemEntry> parent, FileSystemInfo entryInfo)
        {
            FolderFileEntry fileEntry = new FolderFileEntry(entryInfo.Name);
            _registeredEntries.Add(fileEntry);
            TreeNode<IFileSystemEntry> node = new TreeNode<IFileSystemEntry>(fileEntry);
            parent.Add(node);

            if (entryInfo is DirectoryInfo directoryInfo)
            {
                foreach (var childEntry in directoryInfo.EnumerateFileSystemInfos())
                {
                    TraverseFileSystem(node, childEntry);
                }
            }
        }

        private void ClearTree()
        {
            foreach (var registeredFile in _registeredEntries)
            {
                registeredFile.Dispose();
            }
            _registeredEntries.Clear();
            _treeNode = null;
        }

        public void Dispose()
        {
            ClearTree();
        }

        public void Save()
        {
            
        }

        public bool CanWrite => true;

        public TreeNode<IFileSystemEntry> MapTree => _treeNode;

        public IFileSystemEntry GetEntry(string path)
        {
            throw new NotImplementedException();
        }

        public IFileSystemEntry CreateEntry(string path)
        {
            throw new NotImplementedException();
        }

        public void DeleteEntry(string path)
        {
            throw new NotImplementedException();
        }

        public void UpdateTree()
        {
            throw new NotImplementedException();
        }

        public TreeNode<IFileSystemEntry> GetTreeNode(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public void Reload()
        {
            throw new NotImplementedException();
        }
    }
}
