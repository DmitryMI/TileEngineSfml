﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlMapEditor.MapEditing
{
    public class TreeNode<T> : IList<TreeNode<T>>
    {
        private List<TreeNode<T>> _childNodes;

        public TreeNode()
        {
            _childNodes = new List<TreeNode<T>>();
        }

        public TreeNode(T data)
        {
            _childNodes = new List<TreeNode<T>>();
            Data = data;
        }

        public TreeNode<T> ParentNode { get; set; }

        public T Data { get; set; }

        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            return _childNodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _childNodes).GetEnumerator();
        }

        public void Add(TreeNode<T> item)
        {
            if (item != null)
            {
                _childNodes.Add(item);
                item.ParentNode = this;
            }
        }

        public void Clear()
        {
            foreach (var childNode in _childNodes)
            {
                childNode.ParentNode = null;
            }
            _childNodes.Clear();
        }

        public bool Contains(TreeNode<T> item)
        {
            return _childNodes.Contains(item);
        }

        public void CopyTo(TreeNode<T>[] array, int arrayIndex)
        {
            _childNodes.CopyTo(array, arrayIndex);
        }

        public bool Remove(TreeNode<T> item)
        {
            if (item != null)
            {
                int index = IndexOf(item);
                if (index == -1)
                    return false;
                _childNodes.RemoveAt(index);
                return true;
            }

            return false;
        }

        public int Count => _childNodes.Count;

        public bool IsReadOnly => false;
        public int IndexOf(TreeNode<T> item)
        {
            return _childNodes.IndexOf(item);
        }

        public void Insert(int index, TreeNode<T> item)
        {
            if (item != null)
            {
                item.ParentNode = this;
            }
            _childNodes.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            TreeNode<T> node = _childNodes[index];
            node.ParentNode = null;
            _childNodes.RemoveAt(index);
        }

        public void Sort(IComparer<T> comparer)
        {
            TreeNodeComparer treeNodeComparer = new TreeNodeComparer(comparer);
            _childNodes.Sort(treeNodeComparer);
        }

        public TreeNode<T> this[int index]
        {
            get => _childNodes[index];
            set => _childNodes[index] = value;
        }

        private class TreeNodeComparer : IComparer<TreeNode<T>>
        {
            private readonly IComparer<T> _dataComparer;
            public TreeNodeComparer(IComparer<T> dataComparer)
            {
                _dataComparer = dataComparer;
            }

            public int Compare(TreeNode<T> x, TreeNode<T> y)
            {
                if (x == null || y == null)
                {
                    return 0;
                }

                T xData = x.Data;
                T yData = y.Data;

                return _dataComparer.Compare(xData, yData);
            }
        }
    }
}