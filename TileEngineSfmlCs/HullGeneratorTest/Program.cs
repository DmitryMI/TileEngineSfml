using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ResourcesManager;
using ResourcesManager.ResourceTypes;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils.Graphics;

namespace HullGeneratorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int width = 640;
            int height = 380;
            string image = "Images\\MultipleFracture.png";

            VideoMode videoMode = new VideoMode((uint)width, (uint)height);
            RenderWindow renderWindow = new RenderWindow(videoMode, "Hull generator test");

            ResourcesManager.GameResources resources = new GameResources("C:\\Users\\Dmitry\\Documents\\GitHub\\TileEngineSfml\\TileEngineSfmlCs\\TileEngineSfmlMapEditor\\Resources");
            GameResources.Instance = resources;
            ResourceEntry entry = resources.GetEntry(image);

            FileStream fs = resources.GetFileStream(entry);

            Texture texture = new Texture(fs);
            Sprite sprite = new Sprite(texture);

            fs.Close();
            fs.Dispose();

            Icon icon = new Icon(image);

            Debug.WriteLine("Generating hulls...");

            List<Polygon> hulls = HullGenerator.GetConvexHulls(icon);

            Debug.WriteLine("Rendering loop starts");

            View view = renderWindow.GetView();
            view.Center = new Vector2f(-8, 0);
            view.Size = new Vector2f(128, (uint)(128.0f * height / width));
            renderWindow.SetView(view);

            while (renderWindow.IsOpen)
            {
                renderWindow.DispatchEvents();
                renderWindow.Clear();
                renderWindow.Draw(sprite);

                foreach (var polygon in hulls)
                {
                    DrawPolygon(renderWindow, polygon);
                }

                renderWindow.Display();
            }
        }

        static void DrawPolygon(RenderWindow renderWindow, Polygon polygon)
        {
            for (var i = 1; i < polygon.Count; i++)
            {
                var pointA = polygon[i];
                var pointB = polygon[i - 1];

                Vector2f pointAf = new Vector2f(pointA.X + 0.5f, pointA.Y + 0.5f);
                Vector2f pointBf = new Vector2f(pointB.X + 0.5f, pointB.Y + 0.5f);

                

                DrawLine(renderWindow, pointAf, pointBf);
            }

            Vector2i start = polygon[0];
            Vector2i end = polygon.Last();

            Vector2f startF = new Vector2f(start.X + 0.5f, start.Y + 0.5f);
            Vector2f endF = new Vector2f(end.X + 0.5f, end.Y + 0.5f);

            DrawLine(renderWindow, startF, endF);
        }

        static void DrawLine(RenderWindow renderWindow, Vector2f pointA, Vector2f pointB)
        {
            VertexArray array = new VertexArray(PrimitiveType.Lines);


            array.Append(new Vertex(pointA){Color = Color.Red});
            array.Append(new Vertex(pointB) { Color = Color.Red });
            

            renderWindow.Draw(array);
        }
    }
}
