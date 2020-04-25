using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils.Serialization;
using ResourceEntry = TileEngineSfmlCs.ResourceManagement.ResourceTypes.ResourceEntry;

namespace TileEngineSfmlCs.ResourceManagement
{
    public class GameResources : IDisposable
    {
        #region Singleton
        private static GameResources _instance;

        public static GameResources Instance
        {
            get => _instance;
            set => _instance = value;
        }
        #endregion
        
        private List<ResourceEntry> _resourcesList;
        private readonly string _resourcesRootPath;

        private TreeNode<ResourceEntry> _resourceTreeRoot;

        public TreeNode<ResourceEntry> ResourcesRoot => _resourceTreeRoot;

        public void DisposeResources()
        {
            foreach (var resource in _resourcesList)
            {
                resource.Dispose();
            }
            _resourcesList.Clear();
        }

        private void LoadBuiltInEntry(TreeNode<ResourceEntry> parent, string entryPath)
        {
            ResourceEntry entry = null;
            string relativePath = entryPath.Remove(0, _resourcesRootPath.Length);
            if (File.Exists(entryPath))
            {
                string fileName = new FileInfo(entryPath).Name;
                FileStream fs = new FileStream(entryPath, FileMode.Open, FileAccess.ReadWrite);
                entry = new ResourceEntry(_resourcesList.Count, fileName, fs);
                _resourcesList.Add(entry);
                TreeNode<ResourceEntry> node = new TreeNode<ResourceEntry>(entry);
                parent.Add(node);
            }
            else if (Directory.Exists(entryPath))
            {
                string directoryName = new DirectoryInfo(entryPath).Name;
                entry = new ResourceEntry(_resourcesList.Count, directoryName);
                _resourcesList.Add(entry);
                TreeNode<ResourceEntry> node = new TreeNode<ResourceEntry>(entry);
                parent.Add(node);
                foreach (var subEntry in Directory.EnumerateFileSystemEntries(entryPath))
                {
                    LoadBuiltInEntry(node, subEntry);
                }
            }
        }

        private void LoadBuiltInResources()
        {
            /*ResourceEntry root = new ResourceEntry(0, null, null);
            _resourcesList = new List<ResourceEntry>();
            _resourcesList.Add(root);*/
            ResourceEntry root = CreateRoot();
            _resourceTreeRoot = new TreeNode<ResourceEntry>(root);
            foreach (var entry in Directory.EnumerateFileSystemEntries(_resourcesRootPath))
            {
                LoadBuiltInEntry(_resourceTreeRoot, entry);
            }
        }

        public GameResources(string resourcesPath)
        {
            _resourcesRootPath = resourcesPath;
            LoadBuiltInResources();
        }

        public ResourceEntry GetEntry(string path)
        {
            var childNode = TreeNode<ResourceEntry>.SearchPath(_resourceTreeRoot, path, entry => entry.Name);
            return childNode?.Data;
        }

        public ResourceEntry GetEntry(int id)
        {
            if (id < 0)
            {
                return null;
            }
            return _resourcesList[id];
        }

        public int GetResourceId(string path)
        {
            ResourceEntry entry = GetEntry(path);
            if (entry != null)
                return entry.ResourceId;
            return -1;
        }

        public Stream CopyStream(ResourceEntry entry)
        {
            MemoryStream stream = new MemoryStream();
            byte[] data = new byte[entry.DataStream.Length];
            entry.DataStream.Read(data, 0, data.Length);
            stream.Write(data, 0, data.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public void AppendToRoot(TreeNode<IFileSystemEntry> fsTree, string subRootName)
        {
            var subRoot = TreeNode<ResourceEntry>.SearchPath(_resourceTreeRoot, subRootName, entry => entry.Name);
            if (subRoot == null)
            {
                subRoot = new TreeNode<ResourceEntry>(CreateDirectory(subRootName));
                _resourceTreeRoot.Add(subRoot);
            }

            foreach (var child in fsTree)
            {
                // We ignore files in root of .temap
                if (child.Data.IsDirectory)
                {
                    TreeNode<ResourceEntry> childFile = new TreeNode<ResourceEntry>(AddResourceToList(child.Data));
                    subRoot.Add(childFile);
                    TraverseChildNodes(child, childFile);
                }
            }
            

#if DEBUG
            Debug.WriteLine("Resource tree updated: ");
            PrintResourcePath(_resourceTreeRoot, String.Empty);
#endif
        }

        private void PrintResourcePath(TreeNode<ResourceEntry> resourceEntryNode, string path)
        {
            Debug.WriteLine(path + resourceEntryNode.Data?.Name);
            foreach (var childNode in resourceEntryNode)
            {
                PrintResourcePath(childNode, path + resourceEntryNode.Data?.Name + "\\");
            }
        }

        public void RemoveDirectory(string subRootName)
        {
            var subRoot = TreeNode<ResourceEntry>.SearchPath(_resourceTreeRoot, subRootName, entry => entry.Name);
            if (subRoot == null)
            {
                return;
            }
            subRoot.ParentNode.Remove(subRoot);
        }

        private void TraverseChildNodes(TreeNode<IFileSystemEntry> fsNode, TreeNode<ResourceEntry> resourceNode)
        {
            foreach (var childFsNode in fsNode)
            {
                Debug.WriteLine($"Processing {childFsNode} from {fsNode}");
                TreeNode<ResourceEntry> childResourceNode = new TreeNode<ResourceEntry>(AddResourceToList(childFsNode.Data));
                resourceNode.Add(childResourceNode);
                Debug.WriteLine($"Appending {childResourceNode} from {fsNode} to {resourceNode}");
                foreach (var subChild in childFsNode)
                {
                    TreeNode<ResourceEntry> subChildResourceNode = new TreeNode<ResourceEntry>(AddResourceToList(subChild.Data));
                    childResourceNode.Add(subChildResourceNode);
                    TraverseChildNodes(subChild, subChildResourceNode);
                }
            }
        }

        private ResourceEntry AddResourceToList(IFileSystemEntry fileSystemEntry)
        {
            ResourceEntry entry;
            if (fileSystemEntry.IsDirectory)
            {
                entry = new ResourceEntry(_resourcesList.Count, fileSystemEntry.Name);
            }
            else
            {
                entry = new ResourceEntry(_resourcesList.Count, fileSystemEntry.Name, fileSystemEntry.GetStream());
            }
            _resourcesList.Add(entry);
            return entry;
        }

        private ResourceEntry CreateDirectory(string name)
        {
            var entry = new ResourceEntry(_resourcesList.Count, name);
            _resourcesList.Add(entry);
            return entry;
        }

        private ResourceEntry CreateRoot()
        {
            _resourcesList = new List<ResourceEntry>();
            _resourceTreeRoot = null;
            var entry = new ResourceEntry(_resourcesList.Count, null);
            _resourcesList.Add(entry);
            return entry;
        }

        public void Dispose()
        {
            DisposeResources();
        }

        public void LoadResourcesFromMap(IMapContainer map)
        {
            var root = map.MapTree;
            AppendToRoot(root, "User");
        }

        public void SaveResourcesToMap(IMapContainer map)
        {
            var userResources = _resourceTreeRoot.FirstOrDefault(e => e.Data != null && e.Data.Name == "User");

            if (userResources == null)
            {
                Debug.WriteLine("[GameResources] Nothing to save");
                return;
            }

            foreach (var child in userResources)
            {
                SaveResourceEntry(map, child);
            }
        }

        private void SaveResourceEntry(IMapContainer map, TreeNode<ResourceEntry> resourceNode)
        {
            foreach (var child in resourceNode)
            {
                SaveResourceEntry(map, child);
            }

            if (resourceNode.Data.IsDirectory)
            {
                return;
            }

            //var parent = resourceNode.ParentNode;
            //resourceNode.ParentNode = null;
            string path = TreeNode<ResourceEntry>.GetPath(resourceNode, r => r.Name);
            //resourceNode.ParentNode = parent;

            path = path.Replace("User\\", "");
            if (path[0] == '\\')
            {
                path = path.Remove(0, 1);
            }
            Debug.WriteLine($"[GameResources] Pushing {resourceNode} to TileEngineMap with path {path}");
            //var entryStream = map.GetEntry(path);
            map.DeleteEntry(path);
            
            var entryStream = map.CreateEntry(path);

            byte[] data = new byte[resourceNode.Data.DataStream.Length];
            resourceNode.Data.DataStream.Seek(0, SeekOrigin.Begin);
            resourceNode.Data.DataStream.Read(data, 0, data.Length);
            entryStream.Write(data, 0, data.Length);
            entryStream.Close();
            entryStream.Dispose();
        }

        
    }
}
