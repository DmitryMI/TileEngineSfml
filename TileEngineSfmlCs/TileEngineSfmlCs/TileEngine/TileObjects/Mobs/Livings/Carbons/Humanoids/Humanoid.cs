using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Mobs.Livings.Carbons.Humanoids
{
    /// <summary>
    /// Humanoid is a mob, that has limbs, organs and can carry items in various slots
    /// </summary>
    public abstract class Humanoid : Living
    {
        protected override void OnMoveStartLiving(Vector2Int nextCell)
        {
            OnMoveStartHumanoid(nextCell);
        }

        protected virtual void OnMoveStartHumanoid(Vector2Int nextCell)
        {

        }
    }
}
