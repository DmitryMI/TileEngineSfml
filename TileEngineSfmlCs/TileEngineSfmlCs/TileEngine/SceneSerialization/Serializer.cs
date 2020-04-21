using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.SceneSerialization
{
    public static class Serializer
    {

        private static void SerializeTileObject(TileObject tileObject, XmlElement parentElement)
        {
            XmlDocument xmlDocument = parentElement.OwnerDocument;
            Type type = tileObject.GetType();
            if (xmlDocument != null)
            {
                XmlElement objectElement = xmlDocument.CreateElement("TileObject");
                XmlAttribute typeAttribute = xmlDocument.CreateAttribute("Type");
                string typePath = type.AssemblyQualifiedName;
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
            Type type = Type.GetType(fullTypeName);
            if (type == null)
            {
                throw new TypeNotFound(fullTypeName);
            }
            TileObject instance = (TileObject)Activator.CreateInstance(type);
            instance.ReadFields(element);
            return instance;
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

            if(root == null)
                throw new XmlHierarchyException("Root element not found");

            XmlElement scenePreferences = (XmlElement)root.GetElementsByTagName("ScenePreferences")[0];

            int width = SerializationUtils.ReadInt("Width", scenePreferences);
            int height = SerializationUtils.ReadInt("Height", scenePreferences);

            Scene scene = new Scene(width, height);

            XmlElement tileObjectsXml = (XmlElement)root.GetElementsByTagName("TileObjects")[0];

            IList<TileObject> tileObjects = DeserializeTileObjects(tileObjectsXml);

            foreach (var tileObject in tileObjects)
            {
                scene.Instantiate(tileObject);
                Debug.WriteLine($"TileObject loaded. Position: {tileObject.Position.X}, {tileObject.Position.Y}");
            }

            return scene;
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
    }
}
