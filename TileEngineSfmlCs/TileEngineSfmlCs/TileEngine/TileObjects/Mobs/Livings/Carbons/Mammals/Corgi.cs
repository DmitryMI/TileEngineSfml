using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.GameManagement.SoundManagement;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.TileEngine.TileObjects.Turfs.Passable;
using TileEngineSfmlCs.TileEngine.TypeManagement;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Mobs.Livings.Carbons.Mammals
{
    public class Corgi : Living
    {
        private string _dogName = "Unnamed doggo";
        private double _cellsPerSecond = 3;

        [FieldEditorReadOnly("This is determined by the Mob during runtime")]
        private bool _isLying = false;

        public override void TryPass(TileObject sender)
        {
            
        }

        public override TileLayer Layer { get; } = TileLayer.Mobs;

        public override string VisibleName => _dogName;
        public override string ExamineDescription { get; } = "It's a cute doggo! Pet it please :3";
        public override bool IsPassable => !_isLying;
        protected override void AppendUserFields(XmlElement baseElement)
        {
            SerializationUtils.Write(_dogName, nameof(_dogName), baseElement);
        }

        protected override void ReadUserFields(XmlElement baseElement)
        {
            _dogName = SerializationUtils.ReadString(nameof(_dogName), baseElement, _dogName);
        }

        protected override Icon UpFacingIcon { get; } = new Icon("Images\\Mobs\\Corgi\\CorgiBack.png");
        protected override Icon DownFacingIcon { get; } = new Icon("Images\\Mobs\\Corgi\\CorgiFront.png");
        protected override Icon LeftFacingIcon { get; } = new Icon("Images\\Mobs\\Corgi\\CorgiLeft.png");
        protected override Icon RightFacingIcon { get; } = new Icon("Images\\Mobs\\Corgi\\CorgiRight.png");
        public override Icon EditorIcon { get; } = new Icon("Images\\Mobs\\Corgi\\CorgiFront.png");
        protected override bool IgnoreObstacles => false;
        protected override double CellsPerSecond => _cellsPerSecond;
        protected override bool CanMove { get; } = true;
        public override bool IsAlive { get; } = true;

        public void SetDogName(string name)
        {
            _dogName = name;
        }

        protected override void OnMobUpdate()
        {
            
        }

        internal override void OnCreate()
        {
            LogManager.RuntimeLogger.Log("Corgi created! Woof woof!");
        }

        protected override void OnMoveStartLiving(Vector2Int nextCell)
        {
            OnMoveStartCorgi(nextCell);
        }

        protected virtual void OnMoveStartCorgi(Vector2Int nextCell)
        {
            PassableTurf turf = Scene.GetTopMost<PassableTurf>(nextCell);
            if (turf != null)
            {
                SoundClip clip = CollectionUtils.GetRandomItem(turf.FootstepClips);
                SoundManager.Instance.PlaySound(null, clip, 1, this);
            }
        }
    }
}
