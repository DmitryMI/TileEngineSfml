using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesManager.ResourceTypes
{
    public class ResourceEntry : IList<ResourceEntry>
    {
        private List<ResourceEntry> _childEntries = null;

        public bool IsDirectory { get; }

        public int ResourceId { get;  }
        public string Path { get; }
        public string Name { get; }

        public object LoadedValue { get; set; }

        public ResourceEntry(bool isDirectory, int resourceId, string path, string name)
        {
            IsDirectory = isDirectory;
            ResourceId = resourceId;
            Path = path;
            Name = name;
            _childEntries = new List<ResourceEntry>();
        }

        public IEnumerator<ResourceEntry> GetEnumerator()
        {
            return _childEntries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _childEntries).GetEnumerator();
        }

        public void Add(ResourceEntry item)
        {
            _childEntries.Add(item);
        }

        public void Clear()
        {
            _childEntries.Clear();
        }

        public bool Contains(ResourceEntry item)
        {
            return _childEntries.Contains(item);
        }

        public void CopyTo(ResourceEntry[] array, int arrayIndex)
        {
            _childEntries.CopyTo(array, arrayIndex);
        }

        public bool Remove(ResourceEntry item)
        {
            return _childEntries.Remove(item);
        }

        public int Count
        {
            get => _childEntries.Count;
        }

        public bool IsReadOnly
        {
            get => ((ICollection<ResourceEntry>) _childEntries).IsReadOnly;
        }

        public int IndexOf(ResourceEntry item)
        {
            return _childEntries.IndexOf(item);
        }

        public void Insert(int index, ResourceEntry item)
        {
            _childEntries.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _childEntries.RemoveAt(index);
        }

        public ResourceEntry this[int index]
        {
            get => _childEntries[index];
            set => _childEntries[index] = value;
        }
    }
}
