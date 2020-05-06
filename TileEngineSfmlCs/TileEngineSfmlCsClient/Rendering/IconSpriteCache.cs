using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace TileEngineSfmlCsClient.Rendering
{
    class IconSpriteCache
    {
        public List<Sprite> Sprites { get; }

        public IconSpriteCache()
        {
            Sprites = new List<Sprite>();
        }
    }
}
