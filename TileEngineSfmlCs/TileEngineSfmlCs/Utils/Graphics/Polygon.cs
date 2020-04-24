using System.Collections;
using System.Collections.Generic;
using SFML.System;

namespace TileEngineSfmlCs.Utils.Graphics
{
    public class Polygon : IList<Vector2i>
    {
        private bool _middlePointDirty = true;
        private Vector2f _middlePoint;
        private List<Vector2i> _points = new List<Vector2i>();

        public Vector2f MiddlePoint => GetMiddlePoint();

        internal List<Vector2i> Points
        {
            get => _points;
            set
            {
                _middlePointDirty = true;
                _points = value;
            }
        }

        public IEnumerator<Vector2i> GetEnumerator()
        {
            return Points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Points).GetEnumerator();
        }

        public void Add(Vector2i item)
        {
            _middlePointDirty = true;
            Points.Add(item);
        }

        public void Clear()
        {
            _middlePointDirty = true;
            Points.Clear();
        }

        public bool Contains(Vector2i item)
        {
            return Points.Contains(item);
        }

        public void CopyTo(Vector2i[] array, int arrayIndex)
        {
            Points.CopyTo(array, arrayIndex);
        }

        public bool Remove(Vector2i item)
        {
            _middlePointDirty = true;
            return Points.Remove(item);
        }

        public int Count
        {
            get => Points.Count;
        }

        public bool IsReadOnly
        {
            get => ((ICollection<Vector2i>) Points).IsReadOnly;
        }

        public int IndexOf(Vector2i item)
        {
            return Points.IndexOf(item);
        }

        public void Insert(int index, Vector2i item)
        {
            _middlePointDirty = true;
            Points.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _middlePointDirty = true;
            Points.RemoveAt(index);
        }

        public Vector2i this[int index]
        {
            get => Points[index];
            set => Points[index] = value;
        }

        private Vector2f GetMiddlePoint()
        {
            if (_middlePointDirty)
            {
                float x = 0;
                float y = 0;
                foreach (var point in Points)
                {
                    x += point.X;
                    y += point.Y;
                }

                x /= Points.Count;
                y /= Points.Count;
                _middlePointDirty = false;
                _middlePoint = new Vector2f(x, y);
                return _middlePoint;
            }
            else
            {
                return _middlePoint;
            }
        }
    }
}
