using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.GameManagement.SoundManagement;
using TileEngineSfmlCs.TileEngine.TileObjects.Turfs;
using TileEngineSfmlCs.TileEngine.TileObjects.Turfs.Passable;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Mobs.Livings.Carbons.Humanoids.Humans
{
    public class Human : Humanoid
    {
        public override Icon EditorIcon { get; }
        public override void TryPass(TileObject sender)
        {
            throw new NotImplementedException();
        }

        public override TileLayer Layer { get; }
        public override string VisibleName { get; } = "Unknown";
        public override string ExamineDescription { get; } = "Lifeless piece of meat";

        // Humanoid if passable, if it is lying on the floor. Standing humanoid is not passable
        public override bool IsPassable { get; }

        protected override void AppendUserFields(XmlElement baseElement)
        {
            
        }

        protected override void ReadUserFields(XmlElement baseElement)
        {
            
        }

        protected override Icon UpFacingIcon { get; }
        protected override Icon DownFacingIcon { get; }
        protected override Icon LeftFacingIcon { get; }
        protected override Icon RightFacingIcon { get; }
        protected override bool IgnoreObstacles { get; }
        protected override double CellsPerSecond { get; }

        protected override bool CanMove { get; } = true;

        protected override void OnMobUpdate()
        {
            
        }

        protected virtual void OnMoveStartHuman(Vector2Int nextCell)
        {
            PassableTurf turf = Scene.GetTopMost<PassableTurf>(nextCell);
            SoundClip clip = CollectionUtils.GetRandomItem(turf.FootstepClips);
            SoundManager.Instance.PlaySound(null, clip, 1, this);
        }

        protected override void OnMoveStartHumanoid(Vector2Int nextCell)
        {
            OnMoveStartHuman(nextCell);
        }

        public override bool IsAlive { get; }
    }
}
