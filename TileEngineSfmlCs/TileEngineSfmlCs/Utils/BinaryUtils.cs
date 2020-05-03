using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.Utils
{
    public static class BinaryUtils
    {
        public static int Encode(int value, byte[] array, int pos)
        {
            byte[] b = BitConverter.GetBytes(value);
            Array.Copy(b, 0, array, pos, b.Length);
            return b.Length;
        }

        public static int Encode(double value, byte[] array, int pos)
        {
            byte[] b = BitConverter.GetBytes(value);
            Array.Copy(b, 0, array, pos, b.Length);
            return b.Length;
        }

        public static int Encode(float value, byte[] array, int pos)
        {
            byte[] b = BitConverter.GetBytes(value);
            Array.Copy(b, 0, array, pos, b.Length);
            return b.Length;
        }

        public static int Encode(bool value, byte[] array, int pos)
        {
            byte b = (byte)(value ? 1 : 0);
            array[pos] = b;
            return 1;
        }

        public static int Encode(byte value, byte[] array, int pos)
        {
            byte b = value;
            array[pos] = b;
            return 1;
        }

        public static string PrintBinaryArray(byte[] array, int start, int length)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('[');
            for (var i = start; i < array.Length && i < length; i++)
            {
                var item = array[i];
                builder.Append(item);
                if (i < length - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.Append(']');

            return builder.ToString();
        }
    }
}
