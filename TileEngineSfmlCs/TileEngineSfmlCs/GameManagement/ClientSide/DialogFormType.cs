using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.TileEngine.TypeManagement;

namespace TileEngineSfmlCs.GameManagement.ClientSide
{
    public abstract class DialogFormType
    {
        public abstract string Name { get; }

        public Type FormBaseType { get; set; }

        public abstract bool CanActivate { get; }

        public abstract DialogFormSpirit Activate();
    }
}
