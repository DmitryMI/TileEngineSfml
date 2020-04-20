using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.SceneSerialization
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
            return node.InnerText;
        }

        public static int ReadInt(string name, XmlElement parentElement)
        {
            string value = GetNodeText(name, parentElement);
            return int.Parse(value);
        }

        public static float ReadFloat(string name, XmlElement parentElement)
        {
            string value = GetNodeText(name, parentElement);
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        public static T ReadParseable<T>(string name, XmlElement parentElement)
        {
            string strValue = GetNodeText(name, parentElement);

            Type type = typeof(T);
            var parser = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(p => p.GetParameters().Length == 1);
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

        public static T ReadFieldSerializer<T>(string name, XmlElement parentElement) where T : IFieldSerializer
        {
            XmlElement node = (XmlElement)parentElement.GetElementsByTagName(name)[0];
            T serializer = (T)Activator.CreateInstance(typeof(T));
            serializer.ReadFields(node);
            return serializer;
        }

        public static T[] ReadFieldSerializers<T>(string arrayName, XmlElement parentElement) where T : IFieldSerializer
        {
            XmlElement node = (XmlElement)parentElement.GetElementsByTagName(arrayName)[0];

            T[] serializers = new T[node.ChildNodes.Count];

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                T serializer = ReadFieldSerializer<T>(arrayName + i, node);
                serializers[i] = serializer;
            }

            return serializers;
        }
    }
}
