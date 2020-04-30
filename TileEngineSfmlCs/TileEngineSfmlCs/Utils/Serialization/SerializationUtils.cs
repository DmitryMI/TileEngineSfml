using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using TileEngineSfmlCs.Logging;

namespace TileEngineSfmlCs.Utils.Serialization
{
    public static class SerializationUtils
    {
        public static void Write<T>(T[] fieldSerializers, string arrayName, XmlElement parent) where T : IFieldSerializer
        {
            XmlDocument document = parent.OwnerDocument;
            if (document == null)
                return;
            XmlElement arrayElement = document.CreateElement(arrayName);
            T[] serializers = fieldSerializers.ToArray();
            for (var i = 0; i < serializers.Length; i++)
            {
                var serializer = serializers[i];
                XmlElement serialized = document.CreateElement(arrayName + i);
                serializer.AppendFields(serialized);
                arrayElement.AppendChild(serialized);
            }

            parent.AppendChild(arrayElement);
        }

        public static void WriteParseables<T>(T[] parseables, string arrayName, XmlElement parent)
        {
            XmlDocument document = parent.OwnerDocument;
            if (document == null)
                return;
            XmlElement arrayElement = document.CreateElement(arrayName);

            for (var i = 0; i < parseables.Length; i++)
            {
                WriteParseable(parseables[i], arrayName + i, arrayElement);
            }

            parent.AppendChild(arrayElement);
        }

        public static T[] ReadParseables<T>(string arrayName, XmlElement parent, T[] defaultValue)
        {
            var nodeList = parent.GetElementsByTagName(arrayName);
            if (nodeList.Count == 0)
            {
                LogManager.EditorLogger.LogError("[Serialization] Array not found. Map may be created in older version of TileEngine");
                return defaultValue;
            }
            XmlElement node = (XmlElement)nodeList[0];

            T[] result = new T[node.ChildNodes.Count];

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                T serializer = ReadParseable<T>(arrayName + i, node, defaultValue[i]);
                result[i] = serializer;
            }

            return result;
        }

        public static void Write(IFieldSerializer fieldSerializer, string name, XmlElement parent)
        {
            XmlDocument document = parent.OwnerDocument;
            if (document == null)
                return;
            XmlElement element = document.CreateElement(name);
            fieldSerializer.AppendFields(element);
            parent.AppendChild(element);
        }

        public static void Write(int value, string name, XmlElement parent)
        {
            XmlDocument document = parent.OwnerDocument;
            if(document == null)
                return;
            XmlElement element = document.CreateElement(name);
            SetNodeText(element, value.ToString());
            parent.AppendChild(element);
        }

        public static void Write(float value, string name, XmlElement parent)
        {
            XmlDocument document = parent.OwnerDocument;
            if (document == null)
                return;
            XmlElement element = document.CreateElement(name);
            SetNodeText(element, value.ToString(CultureInfo.InvariantCulture));
            parent.AppendChild(element);
        }  

        public static void Write(string value, string name, XmlElement parent)
        {
            XmlDocument document = parent.OwnerDocument;
            if (document == null)
                return;
            XmlElement element = document.CreateElement(name);
            SetNodeText(element, value);
            parent.AppendChild(element);
        }
        

        public static void WriteParseable(object value, string name, XmlElement parent)
        {
            XmlDocument document = parent.OwnerDocument;
            if (document == null)
                return;
            XmlElement element = document.CreateElement(name);
            string strValue;
            if (value is float fValue)
            {
                strValue = fValue.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                strValue = value.ToString();
            }

            SetNodeText(element, strValue);

            parent.AppendChild(element);
        }

        public static void SetNodeText(XmlNode node, string text)
        {
            node.InnerText = text;
        }

        public static string GetNodeText(string nodeName, XmlElement parentElement)
        {
            XmlNode node = parentElement.GetElementsByTagName(nodeName)[0];
            if (node == null)
            {
                LogManager.EditorLogger.LogError(
                    "[Serialization] Key not found. Map may be created in other version of TileEngine");
                return null;
            }
            return node.InnerText;
        }

        public static int ReadInt(string name, XmlElement parentElement, int defaultValue)
        {
            string value = GetNodeText(name, parentElement);
            if (value == null)
            {
                return defaultValue;
            }
            return int.Parse(value);
        }

        public static string ReadString(string name, XmlElement parentElement, string defaultValue)
        {
            string value = GetNodeText(name, parentElement);
            if (value == null)
            {
                return defaultValue;
            }
            return value;
        }

        public static float ReadFloat(string name, XmlElement parentElement, float defaultValue)
        {
            string value = GetNodeText(name, parentElement);
            if (value == null)
            {
                return defaultValue;
            }
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        public static T ReadParseable<T>(string name, XmlElement parentElement, T defaultValue)
        {
            string strValue = GetNodeText(name, parentElement);
            if (strValue == null)
            {
                return defaultValue;
            }

            Type type = typeof(T);
            var parser = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(p => p.GetParameters().Length == 1 && p.ReturnType == typeof(T));
            if (parser == null)
            {
                throw new ArgumentException($"{type.Name} is not parseable");
            }
            T value = (T)parser.Invoke(null, new object[]{strValue});
            return value;
        }

        public static IFieldSerializer ReadFieldSerializer(string name, IFieldSerializer serializer, XmlElement parentElement)
        {
            XmlElement node = (XmlElement)parentElement.GetElementsByTagName(name)[0];
            serializer.ReadFields(node);
            return serializer;
        }

        public static T ReadFieldSerializer<T>(string name, XmlElement parentElement, T defaultValue) where T : IFieldSerializer
        {
            var nodeList = parentElement.GetElementsByTagName(name);
            if (nodeList.Count == 0)
            {
                LogManager.EditorLogger.LogError("[Serialization] Key not found. Map may be created in older version of TileEngine");
                return defaultValue;
            }
            XmlElement node = (XmlElement)nodeList[0];
            T serializer = (T)Activator.CreateInstance(typeof(T));
            serializer.ReadFields(node);
            return serializer;
        }

        public static T[] ReadFieldSerializers<T>(string arrayName, XmlElement parentElement, T[] defaultValue) where T : IFieldSerializer
        {
            var nodeList = parentElement.GetElementsByTagName(arrayName);
            if (nodeList.Count == 0)
            {
                LogManager.EditorLogger.LogError("[Serialization] Array not found. Map may be created in older version of TileEngine");
                return defaultValue;
            }
            XmlElement node = (XmlElement)nodeList[0];
            T[] serializers = new T[node.ChildNodes.Count];

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                T serializer = ReadFieldSerializer<T>(arrayName + i, node, defaultValue[i]);
                serializers[i] = serializer;
            }

            return serializers;
        }
    }
}
