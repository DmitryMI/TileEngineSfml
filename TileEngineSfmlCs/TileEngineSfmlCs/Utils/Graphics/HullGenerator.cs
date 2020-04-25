using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using TileEngineSfmlCs.Types;
using GameResources = TileEngineSfmlCs.TileEngine.ResourceManagement.GameResources;
using ResourceEntry = TileEngineSfmlCs.TileEngine.ResourceManagement.ResourceTypes.ResourceEntry;

namespace TileEngineSfmlCs.Utils.Graphics
{
    public static class HullGenerator
    {
        public const byte AlphaThreshold = 0;

        public static List<Polygon> GetConvexHulls(Icon icon)
        {
            List<Polygon> polygons = new List<Polygon>(icon.SpritesCount);
            for (int i = 0; i < icon.SpritesCount; i++)
            {
                polygons.Add(GetPolygon(icon, i));
            }

            return polygons;
        }

        private static Polygon GetPolygon(Icon icon, int spriteIndex)
        {
            ResourceEntry resourceEntry =
                GameResources.Instance.GetEntry(icon.GetResourceId(spriteIndex));
          
            if (resourceEntry.LoadedValue == null)
            {
                using (Stream fs = GameResources.Instance.CopyStream(resourceEntry))
                {
                    byte[] data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                    fs.Close();
                    fs.Dispose();
                    Texture texture = new Texture(data);
                    resourceEntry.LoadedValue = texture;
                }
            }

            Image image = ((Texture) (resourceEntry.LoadedValue)).CopyToImage();

            return GetPolygon(image);
        }

        private static Polygon GetPolygon(Image image)
        {
            Polygon polygon = new Polygon();
           
            List<Vector2i> rightSidePoints = new List<Vector2i>();

            int scanLine = 0;
            while (scanLine < image.Size.Y)
            {
                int x = 0;
                while (x < image.Size.X)
                {
                    Color pixel = image.GetPixel((uint) x, (uint) scanLine);
                    if (pixel.A > AlphaThreshold)
                    {
                        break;
                    }

                    x++;
                }

                if (x < image.Size.X)
                {
                    if (polygon.Points.Count > 0)
                    {
                        int prevX = polygon.Points.Last().X;
                        if (x - prevX > 1) // Current is more right
                        {
                            polygon.Points.Add(new Vector2i(x, scanLine - 1));
                        }
                        else if (x - prevX < 1)
                        {
                            polygon.Points.Add(new Vector2i(prevX, scanLine));
                        }
                    }

                    polygon.Points.Add(new Vector2i(x, scanLine));
                }

                x = (int)(image.Size.X - 1);
                while (x >= 0)
                {
                    Color pixel = image.GetPixel((uint)x, (uint)scanLine);
                    if (pixel.A > AlphaThreshold)
                    {
                        break;
                    }

                    x--;
                }

                if (x >= 0)
                {
                    if (rightSidePoints.Count > 0)
                    {
                        int prevX = rightSidePoints.Last().X;
                        if (x - prevX > 1) // Current is more right
                        {
                            rightSidePoints.Add(new Vector2i(prevX, scanLine));
                        }
                        else if (x - prevX < 1)
                        {
                            rightSidePoints.Add(new Vector2i(x, scanLine - 1));
                        }
                    }

                    rightSidePoints.Add(new Vector2i(x, scanLine));
                }

                scanLine++;
            }

            for(int i = rightSidePoints.Count - 1; i > 0; i--)
            {
                polygon.Points.Add(rightSidePoints[i]);
            }

            if (polygon.Points[0] != rightSidePoints[0])
            {
                polygon.Add(rightSidePoints[0]);
            }

            return polygon;
        }
    }
}
