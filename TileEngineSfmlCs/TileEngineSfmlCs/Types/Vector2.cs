﻿using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using TileEngineSfmlCs.GameManagement.BinaryEncoding;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.Types
{
    public struct Vector2 : IFieldSerializer, IBinaryEncodable
    {
        public const float ComparisonTolerance = 0.001f;
        public static Vector2 Zero { get; } = new Vector2(0, 0);

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
            X = SerializationUtils.ReadFloat(nameof(X), parentElement, X);
            Y = SerializationUtils.ReadFloat(nameof(Y), parentElement, Y);
        }

        public override string ToString()
        {
            return $"({X:0.00}; {Y:0.00})";
        }

        public static Vector2 Parse(string text)
        {
            text = text.Replace(',', '.');
            if (text[0] == '(' && text.Last() == ')')
            {
                text = text.Remove(0, 1);
                text = text.Remove(text.Length - 1, 1);
            }
            string[] words = text.Split(new char[] { ' ', ';' });
            if (words.Length > 1)
            {
                float x = float.Parse(words[0], CultureInfo.InvariantCulture);
                float y = float.Parse(words.Last(), CultureInfo.InvariantCulture);
                return new Vector2(x, y);
            }
            else
            {
                throw new FormatException("Splitting failed");
            }
        }

        public int ByteLength => sizeof(float) + sizeof(float);
        public int ToByteArray(byte[] package, int index)
        {
            int pos = index;
            byte[] xBytes = BitConverter.GetBytes(X);
            byte[] yBytes = BitConverter.GetBytes(Y);
           
            Array.Copy(xBytes, 0, package, pos, xBytes.Length);
            pos += sizeof(float);
            Array.Copy(yBytes, 0, package, pos, yBytes.Length);
            pos += sizeof(float);
            return ByteLength;
        }

        public void FromByteArray(byte[] data, int index)
        {
            int pos = index;
            X = BitConverter.ToSingle(data, pos);
            pos += sizeof(float);
            Y = BitConverter.ToSingle(data, pos);
            pos += sizeof(float);
        }
    }
}