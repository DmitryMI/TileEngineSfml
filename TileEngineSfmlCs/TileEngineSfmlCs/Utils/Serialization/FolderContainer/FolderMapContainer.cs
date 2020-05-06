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
            FolderFileEntry fileEntry = new FolderFileEntry(entryInfo.FullName);
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
            var node = TreeNode<IFileSystemEntry>.SearchPath(_treeNode, path, f => f.Name);
            return node.Data;
        }

        public IFileSystemEntry CreateEntry(string path)
        {
            string absolutePath = Path.Combine(_directoryInfo.FullName, path);
            using (Stream stream = File.Create(absolutePath))
            {

            }

            FolderFileEntry entry = new FolderFileEntry(absolutePath);
            string[] pathFragments = path.Split('\\');
            var parent =
                TreeNode<IFileSystemEntry>.SearchPath(_treeNode, pathFragments, pathFragments.Length - 1, f => f.Name);
            parent.Add(new TreeNode<IFileSystemEntry>(entry));

            return entry;
        }

        public void DeleteEntry(string path)
        {
            var entry =
                TreeNode<IFileSystemEntry>.SearchPath(_treeNode, path, f => f.Name);
            
            if(entry == null)
                return;

            string fullPath = ((FolderFileEntry)entry.Data).AbsolutePath;

            if (entry.Data.IsDirectory)
            {
                if (entry.Count != 0)
                {
                    throw new InvalidOperationException("Directory contains subentries. Delete them first");
                }
                else
                {
                    entry.ParentNode.Remove(entry);
                    Directory.Delete(fullPath);
                }
            }
            else
            {
                entry.ParentNode.Remove(entry);
                File.Delete(fullPath);
            }
        }


        public TreeNode<IFileSystemEntry> GetTreeNode(string directoryPath)
        {
            var entry =
                TreeNode<IFileSystemEntry>.SearchPath(_treeNode, directoryPath, f => f.Name);
            return entry;
        }

        public void Reload()
        {
            ClearTree();
            CreateTree();
        }
    }
}
