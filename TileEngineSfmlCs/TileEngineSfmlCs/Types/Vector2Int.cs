using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using TileEngineSfmlCs.TileEngine.SceneSerialization;

namespace TileEngineSfmlCs.Types
{
    public struct Vector2Int : IFieldSerializer
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object other)
        {
            if (other is Vector2Int vector2Int)
            {
                return vector2Int.X == X && vector2Int.Y == Y;
            }

            return false;
        }

        public bool Equals(Vector2Int other)
        {
            return X == other.X && Y == other.Y;
        }

        public static  bool operator ==(Vector2Int a, Vector2Int b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2Int a, Vector2Int b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2Int operator -(Vector2Int vector2)
        {
            return new Vector2Int(-vector2.X, -vector2.Y);
        }

        public static Vector2Int operator *(Vector2Int vector2, int multiplier)
        {
            return new Vector2Int(vector2.X * multiplier, vector2.Y * multiplier);
        }

        public static implicit operator Vector2(Vector2Int vector2Int)
        {
            return new Vector2(vector2Int.X, vector2Int.Y);
        }

        public float SqrMagnitude => X * X + Y * Y;
        public float Magnitude => (float)Math.Sqrt(SqrMagnitude);
        public void AppendFields(XmlElement parentElement)
        {
            SerializationUtils.Write(X, nameof(X), parentElement);
            SerializationUtils.Write(Y, nameof(Y), parentElement);
        }

        public void ReadFields(XmlElement parentElement)
        {
            X = SerializationUtils.ReadInt(nameof(X), parentElement, X);
            Y = SerializationUtils.ReadInt(nameof(Y), parentElement, Y);
        }

        public override string ToString()
        {
            return $"({X}; {Y})";
        }

        public static Vector2Int Parse(string text)
        {
            if (text[0] == '(' && text.Last() == ')')
            {
                text = text.Remove(0, 1).Remove(text.Length - 1, 1);
            }
            string[] words = text.Split(new char[] {' ', ';', ','});
            if (words.Length == 2)
            {
                int x = int.Parse(words[0]);
                int y = int.Parse(words[1]);
                return new Vector2Int(x, y);
            }
            throw new FormatException();
        }
    }
}