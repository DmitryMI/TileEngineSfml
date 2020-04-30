using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using TileEngineSfmlCs.GameManagement.ServerSide;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.TileEngine.TypeManagement.EntityTypes;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.TileEngine
{
    public class Scene
    {
        internal readonly List<TileObject>[,] ObjectMatrix;

        private List<Subsystem> _subsystems;
        private List<TileObject> _updateableObjects = new List<TileObject>();
        private List<TileObject> _instantiatedTileObjects = new List<TileObject>();

        public List<TileObject> TileObjects => _instantiatedTileObjects;

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

            LogManager.RuntimeLogger.Log($"New scene created with dimensions {width}, {height}");
        }

        public void OnNextFrame()
        {
            foreach (var updateable in _updateableObjects)
            {
                updateable.OnUpdate();
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
            if (!IsInBounds(cell))
            {
                return new TileObject[0];
            }
            return ObjectMatrix[cell.X, cell.Y]?.ToArray();
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

        public void RegisterUpdateable(TileObject tileObject)
        {
            _updateableObjects.Add(tileObject);
        }

        public void UnregisterUpdateable(TileObject tileObject)
        {
            _updateableObjects.Remove(tileObject);
        }

        private void RegisterTileObject(TileObject tileObject)
        {
            int instanceId = 0;
            while (instanceId < _instantiatedTileObjects.Count)
            {
                if (_instantiatedTileObjects[instanceId] == null)
                {
                    break;
                }

                instanceId++;
            }

            if (instanceId == _instantiatedTileObjects.Count)
            {
                _instantiatedTileObjects.Add(tileObject);
            }
            else
            {
                _instantiatedTileObjects[instanceId] = tileObject;
            }
            tileObject.SetInstanceId(instanceId);
        }

        public void Instantiate(TileObject tileObject)
        {
            RegisterPosition(tileObject);
            if (tileObject.RequiresUpdates)
            {
                RegisterUpdateable(tileObject);
            }
            tileObject.SetScene(this);
            
            RegisterTileObject(tileObject);

            tileObject.OnCreate();
        }

        public void InstantiateEditor(TileObject tileObject)
        {
            RegisterPosition(tileObject);
            tileObject.SetScene(this);
            RegisterTileObject(tileObject);
            tileObject.OnEditorCreate();
        }

        public void DestroyEditor(TileObject tileObject)
        {
            UnregisterPosition(tileObject);
            tileObject.SetScene(null);

            if (tileObject.GetInstanceId() == -1)
            {
                LogManager.EditorLogger.LogError("Do not destroy objects, that are not instantiated!");
            }

            _instantiatedTileObjects[tileObject.GetInstanceId()] = null;
            tileObject.SetInstanceId(-1);
        }

        public void Destroy(TileObject tileObject)
        {
            NetworkManager.Instance.DestroyTileObject(tileObject);
            UnregisterPosition(tileObject);
            UnregisterUpdateable(tileObject);
            tileObject.OnDestroy();
            tileObject.SetScene(null);

            if (tileObject.GetInstanceId() == -1)
            {
                LogManager.RuntimeLogger.LogError($"[Scene] {tileObject.VisibleName} is not instantiated");
            }

            _instantiatedTileObjects[tileObject.GetInstanceId()] = null;
            tileObject.SetInstanceId(-1);
        }
        #endregion

        #region Scene  creation

        public static Scene CreateFromMap(IMapContainer map, string scenePath)
        {
            var entry = map.GetEntry(scenePath);
            if (entry == null)
            {
                LogManager.EditorLogger.LogError("[Scene] main.scene not found!");
                return null;
            }
            using (Stream mapXmlStream = entry.OpenStream())
            {
                if (mapXmlStream == null)
                {
                    return null;
                }
                return DeserializeScene(mapXmlStream);
            }
        }

        public static void SaveToMap(Scene scene, IMapContainer map, string scenePath)
        {
            map.DeleteEntry(scenePath);
            using (Stream mapXmlStream = map.CreateEntry(scenePath).OpenStream())
            {
                SerializeScene(scene, mapXmlStream);
                mapXmlStream.Flush();
                mapXmlStream.Close();
            }
        }

        public static void SerializeScene(Scene scene, Stream serializationStream)
        {
            XmlDocument document = new XmlDocument();

            XmlElement rootElement = document.CreateElement("Root");
            document.AppendChild(rootElement);

            // Scene preferences
            XmlElement scenePreferences = document.CreateElement("ScenePreferences");
            SerializationUtils.Write(scene.Width, "Width", scenePreferences);
            SerializationUtils.Write(scene.Height, "Height", scenePreferences);
            rootElement.AppendChild(scenePreferences);

            // Tile objects
            XmlElement tileObjects = document.CreateElement("TileObjects");
            SerializeTileObjects(scene.ObjectMatrix, tileObjects);
            rootElement.AppendChild(tileObjects);

            document.Save(serializationStream);
        }

        public static Scene DeserializeScene(Stream serializedStream)
        {

            XmlDocument document = new XmlDocument();
            document.Load(serializedStream);

            XmlElement root = (XmlElement)document.GetElementsByTagName("Root")[0];

            if (root == null)
                throw new XmlHierarchyException("Root element not found");

            XmlElement scenePreferences = (XmlElement)root.GetElementsByTagName("ScenePreferences")[0];

            int width = SerializationUtils.ReadInt("Width", scenePreferences, 50);
            int height = SerializationUtils.ReadInt("Height", scenePreferences, 50);

            Scene scene = new Scene(width, height);

            XmlElement tileObjectsXml = (XmlElement)root.GetElementsByTagName("TileObjects")[0];

            IList<TileObject> tileObjects = DeserializeTileObjects(tileObjectsXml);

            foreach (var tileObject in tileObjects)
            {
                scene.Instantiate(tileObject);
                //Debug.WriteLine($"TileObject loaded. Position: {tileObject.Position.X}, {tileObject.Position.Y}");
            }

            LogManager.RuntimeLogger.Log($"Scene deserialized. {scene._instantiatedTileObjects.Count} objects created");

            return scene;
        }

        private static IList<TileObject> DeserializeTileObjects(XmlElement listRoot)
        {
            List<TileObject> result = new List<TileObject>(listRoot.ChildNodes.Count);
            foreach (var child in listRoot.ChildNodes)
            {
                if (child is XmlElement element)
                {
                    TileObject tileObject = DeserializeTileObject(element);
                    if (tileObject != null)
                    {
                        result.Add(tileObject);
                    }
                }
            }

            return result;
        }


        private static void SerializeTileObject(TileObject tileObject, XmlElement parentElement)
        {
            XmlDocument xmlDocument = parentElement.OwnerDocument;
            EntityType type = tileObject.GetEntityType();
            if (xmlDocument != null)
            {
                XmlElement objectElement = xmlDocument.CreateElement("TileObject");
                XmlAttribute typeAttribute = xmlDocument.CreateAttribute("Type");
                string typePath = type.FullName;
                typeAttribute.Value = typePath;
                objectElement.Attributes.Append(typeAttribute);
                tileObject.AppendFields(objectElement);

                parentElement.AppendChild(objectElement);
            }
            else
            {
                throw new XmlHierarchyException("parentElement has no XmlDocument attached");
            }
        }

        private static TileObject DeserializeTileObject(XmlElement element)
        {
            XmlAttribute typeAttribute = element.GetAttributeNode("Type");
            if (typeAttribute == null)
            {
                return null;
            }
            string fullTypeName = typeAttribute.Value;
            //Type type = Type.GetType(fullTypeName);
            EntityType type = TypeManagement.TypeManager.Instance.GetEntityType(fullTypeName);
            if (type == null)
            {
                Logging.LogManager.EditorLogger.LogError($"Type {fullTypeName} was not found!");
                return null;
            }
            else
            {
                TileObject instance = type.Activate();
                instance.ReadFields(element);
                return instance;
            }
        }

        private static void SerializeTileObjects(List<TileObject>[,] objectMatrix, XmlElement tileObjects)
        {
            int width = objectMatrix.GetLength(0);
            int height = objectMatrix.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    List<TileObject> list = objectMatrix[x, y];

                    foreach (var tileObject in list)
                    {
                        SerializeTileObject(tileObject, tileObjects);
                    }
                }
            }
        }

        public class TypeNotFound : Exception
        {
            public TypeNotFound(string typeName) : base($"Type {typeName} was not found")
            {

            }
        }

        public class XmlHierarchyException : Exception
        {
            public XmlHierarchyException(string message) : base(message)
            {

            }
        }

        #endregion
    }
}
