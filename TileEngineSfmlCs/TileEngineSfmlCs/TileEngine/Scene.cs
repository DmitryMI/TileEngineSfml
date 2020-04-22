using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils;

namespace TileEngineSfmlCs.TileEngine
{
    public class Scene
    {
        internal readonly List<TileObject>[,] ObjectMatrix;

        private int _instanceCounter = 1;

        private List<Subsystem> _subsystems;

        public int Width { get; }
        public int Height { get; }

        public bool IsInBounds(Vector2Int cell)
        {
            if (cell.X < 0 || cell.Y < 0 || cell.X >= Width || cell.Y >= Height)
            {
                return false;
            }

            return true;
        }

        public Scene(int width, int height)
        {
            Width = width;
            Height = height;
            ObjectMatrix = new List<TileObject>[width, height];
            _subsystems = new List<Subsystem>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    ObjectMatrix[x, y] = new List<TileObject>();
                }
            }
        }

        public void NextFrame()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    foreach (var tileObject in ObjectMatrix[x, y])
                    {
                        tileObject.OnUpdate();
                    }
                }
            }

            foreach (var subsystem in _subsystems)
            {
                subsystem.OnUpdate();
            }
        }

        #region Subsystem management

        public void RegisterSubsystem(Subsystem subsystem)
        {
            _subsystems.Add(subsystem);
            subsystem.SetScene(this);
            subsystem.OnRegister();
        }

        public void UnregisterSubsystem(Subsystem subsystem)
        {
            subsystem.SetScene(null);
            subsystem.OnUnregister();
        }

        #endregion

        #region TileObjectManagements

        #region ObjectQuerying

        public T[] GetObjectsOfType<T>(Vector2Int cell)
        {
            List<T> result = new List<T>();
            foreach (var obj in ObjectMatrix[cell.X, cell.Y])
            {
                if (obj is T t)
                {
                    result.Add(t);
                }
            }

            return result.ToArray();
        }

        public T[] GetObjectsOfType<T>(Vector2Int cell, Func<T, bool> filter)
        {
            List<T> result = new List<T>();
            foreach (var obj in ObjectMatrix[cell.X, cell.Y])
            {
                if (obj is T t && filter(t))
                {
                    result.Add(t);
                }
            }

            return result.ToArray();
        }

        public TileObject[] GetObjects(Vector2Int cell)
        {
            return ObjectMatrix[cell.X, cell.Y].ToArray();
        }

        public TileObject[] GetObjects(Vector2Int cell, Func<TileObject, bool> filter)
        {
            List<TileObject> result = new List<TileObject>();
            foreach (var obj in ObjectMatrix[cell.X, cell.Y])
            {
                if (filter(obj))
                {
                    result.Add(obj);
                }
            }

            return result.ToArray();
        }

        public TileObject[] GetObjectsOnLayer(Vector2Int cell, TileLayer layer)
        {
            List<TileObject> result = new List<TileObject>();
            foreach (var obj in ObjectMatrix[cell.X, cell.Y])
            {
                if (obj.Layer == layer)
                {
                    result.Add(obj);
                }
            }

            return result.ToArray();
        }

        public bool IsPassable(Vector2Int cell)
        {
            foreach (var obj in ObjectMatrix[cell.X, cell.Y])
            {
                if (!obj.IsPassable)
                    return false;
            }

            return true;
        }
        public bool IsLightTransparent(Vector2Int cell)
        {
            foreach (var obj in ObjectMatrix[cell.X, cell.Y])
            {
                if (!obj.IsLightTransparent)
                    return false;
            }

            return true;
        }

        public bool IsGasTransparent(Vector2Int cell)
        {
            foreach (var obj in ObjectMatrix[cell.X, cell.Y])
            {
                if (!obj.IsGasTransparent)
                    return false;
            }

            return true;
        }

        #endregion

        public void RegisterPosition(TileObject obj)
        {
            List<TileObject> list = ObjectMatrix[obj.Position.X, obj.Position.Y];
            
            list.InsertSortedDescending(obj, new FuncComparer<TileObject>((a,b) => ((int)(b.Layer)) - ((int)a.Layer)));
        }

        public void UnregisterPosition(TileObject obj)
        {
            ObjectMatrix[obj.Position.X, obj.Position.Y].Remove(obj);
        }

        public void ChangePosition(TileObject obj, Vector2Int prevPosition)
        {
            ObjectMatrix[prevPosition.X, prevPosition.Y].Remove(obj);
            RegisterPosition(obj);
        }

        public void Instantiate(TileObject tileObject)
        {
            RegisterPosition(tileObject);
            tileObject.SetScene(this);
            tileObject.SetInstanceId(_instanceCounter);
            _instanceCounter++;
            tileObject.OnCreate();
        }

        public void InstantiateEditor(TileObject tileObject)
        {
            RegisterPosition(tileObject);
            tileObject.SetScene(this);
            tileObject.SetInstanceId(_instanceCounter);
            _instanceCounter++;
            tileObject.OnEditorCreate();
        }

        public void DestroyEditor(TileObject tileObject)
        {
            UnregisterPosition(tileObject);
            tileObject.SetScene(null);

            if (tileObject.GetInstanceId() == _instanceCounter - 1)
            {
                _instanceCounter--;
            }
        }

        public void Destroy(TileObject tileObject)
        {
            UnregisterPosition(tileObject);
            tileObject.SetScene(null);
            tileObject.OnDestroy();

            if (tileObject.GetInstanceId() == _instanceCounter + 1)
            {
                _instanceCounter--;
            }
        }
        #endregion
    }
}
