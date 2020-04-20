using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourcesManager.ResourceTypes;

namespace ResourcesManager
{
    public class GameResources
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

        public ResourceEntry ResourcesRoot => _resourcesList[0];

        private void LoadEntry(ResourceEntry parent, string entryPath)
        {
            ResourceEntry entry = null;
            string relativePath = entryPath.Remove(0, _resourcesRootPath.Length);
            if (File.Exists(entryPath))
            {
                string fileName = new FileInfo(entryPath).Name;
                entry = new ResourceEntry(false, _resourcesList.Count, relativePath, fileName);
            }
            else if (Directory.Exists(entryPath))
            {
                string directoryName = new DirectoryInfo(entryPath).Name;
                entry = new ResourceEntry(true, _resourcesList.Count, relativePath, directoryName);
                foreach (var subEntry in Directory.EnumerateFileSystemEntries(entryPath))
                {
                    LoadEntry(entry, subEntry);
                }
            }

            if (entry != null)
            {
                _resourcesList.Add(entry);
                parent.Add(entry);
            }
        }

        private void LoadResources()
        {
            ResourceEntry root = new ResourceEntry(true, 0, null, null);
            _resourcesList = new List<ResourceEntry>();
            _resourcesList.Add(root);
            foreach (var entry in Directory.EnumerateFileSystemEntries(_resourcesRootPath))
            {
                LoadEntry(root, entry);
            }
        }

        public GameResources(string resourcesPath)
        {
            _resourcesRootPath = resourcesPath;
            LoadResources();
        }

        public ResourceEntry GetEntry(string path)
        {
            string[] pathFragments = path.Split(Path.DirectorySeparatorChar);
            int fragmentIndex = 0;
            ResourceEntry currentEntry = ResourcesRoot.FirstOrDefault(e => e.Name.Equals(pathFragments[0]));
            fragmentIndex++;
            if (currentEntry == null)
            {
                return null;
            }
            while (fragmentIndex < pathFragments.Length)
            {
                Debug.WriteLine($"{currentEntry.Path}");
                currentEntry = currentEntry.FirstOrDefault(n => n.Name.Equals(pathFragments[fragmentIndex]));
                if (currentEntry == null)
                    return null;
                fragmentIndex++;
            }

            return currentEntry;
        }

        public ResourceEntry GetEntry(int id)
        {
            return _resourcesList[id];
        }

        public int GetResourceId(string path)
        {
            ResourceEntry entry = GetEntry(path);
            if (entry != null)
                return entry.ResourceId;
            return -1;
        }

        public FileStream GetFileStream(string path)
        {
            if (path.StartsWith("\\"))
            {
                path = path.Remove(0, 1);
            }
            string absPath = Path.Combine(_resourcesRootPath, path);
            return new FileStream(absPath, FileMode.Open);
        }

        public FileStream GetFileStream(int id)
        {
            string relativePath = _resourcesList[id].Path;
            return GetFileStream(relativePath);
        }

        public FileStream GetFileStream(ResourceEntry entry)
        {
            return GetFileStream(entry.Path);
        }
    }
}
