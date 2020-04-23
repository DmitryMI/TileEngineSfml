using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.Logging;

namespace TileEngineSfmlCs.Utils
{
    public static class ParsingUtils
    {
        private static MethodInfo GetDefaultParser(Type type)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            var parser = methods.FirstOrDefault(m => m.ReturnType == type && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(string));
            return parser;
        }

        public static bool IsTextParseable(Type type)
        {
            return GetDefaultParser(type) != null;
        }

        public static T Parse<T>(string text)
        {
            return (T)Parse(typeof(T), text);
        }

        public static object Parse(Type type, string text)
        {
            MethodInfo defaultParser = GetDefaultParser(type);
            if (defaultParser == null)
            {
                throw new NotImplementedException($"{type.Name} cannot be parsed");
            }

            try
            {
                object result = defaultParser.Invoke(null, new object[] { text });
                return result;
            }
            catch (Exception ex)
            {
                LogManager.EditorLogger.LogError("Parsing error: " + ex.Message);
            }

            return null;
        }
    }
}
