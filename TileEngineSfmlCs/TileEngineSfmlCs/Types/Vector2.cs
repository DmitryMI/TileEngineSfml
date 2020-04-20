using System;
using System.Xml;
using TileEngineSfmlCs.TileEngine.SceneSerialization;

namespace TileEngineSfmlCs.Types
{
    public struct Vector2 : IFieldSerializer
    {
        public const float ComparisonTolerance = 0.001f;

        public float X { get; set; }
        public float Y { get; set; }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object other)
        {
            if (other is Vector2 vector2)
            {
                return vector2.Equals(this);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public bool Equals(Vector2 other)
        {
            return Math.Abs(X - other.X) < ComparisonTolerance && Math.Abs(Y - other.Y) < ComparisonTolerance;
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !(a == b);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator -(Vector2 vector2)
        {
            return new Vector2(-vector2.X, -vector2.Y);
        }

        public static Vector2 operator *(Vector2 vector2, float multiplier)
        {
            return new Vector2(vector2.X * multiplier, vector2.Y * multiplier);
        }

        public static explicit operator Vector2Int(Vector2 vector2)
        {
            return new Vector2Int((int)vector2.X, (int)vector2.Y);
        }

        public static Vector2Int RoundToInt(Vector2 vector2)
        {
            int x = (int)Math.Round(vector2.X);
            int y = (int) Math.Round(vector2.Y);
            return new Vector2Int(x, y);
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
            X = SerializationUtils.ReadFloat(nameof(X), parentElement);
            Y = SerializationUtils.ReadFloat(nameof(Y), parentElement);
        }
    }
}