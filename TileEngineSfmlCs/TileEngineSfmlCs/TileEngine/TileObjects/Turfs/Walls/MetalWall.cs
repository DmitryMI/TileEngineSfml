using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.TileEngine.Interaction;
using TileEngineSfmlCs.TileEngine.TileObjects.Mobs;
using TileEngineSfmlCs.TileEngine.TileObjects.Objs.Items;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Turfs.Walls
{
    public class MetalWall : Wall
    {
        public override Icon Icon { get; } = new Icon("Images\\Walls\\MetalWall.png");
        public override Icon EditorIcon { get; } = new Icon("Images\\Walls\\MetalWall.png");
        public override void TryPass(TileObject sender)
        {
            
        }

        public override string VisibleName { get; } = "Metal wall";
        public override string ExamineDescription { get; } = "A wall made of some metal. Very solid";
        public override bool RequiresUpdates { get; } = false;
        public override bool IsLightTransparent { get; } = false;
        public override bool IsGasTransparent { get; } = false;
        protected override void AppendUserFields(XmlElement baseElement)
        {
           
        }

        protected override void ReadUserFields(XmlElement baseElement)
        {
            
        }

        public override InteractionResult ApplyItem(Item appliedItem, Mob interactionSource, InteractionDescriptor descriptor)
        {
            return InteractionResult.ContinueChain;
        }

        public override InteractionResult DragDrop(TileObject draggedObject, Mob interactionSource, InteractionDescriptor descriptor)
        {
            return InteractionResult.ContinueChain;
        }

        public override InteractionResult DragStart(Mob interactionSource, InteractionDescriptor descriptor)
        {
            return InteractionResult.ContinueChain;
        }
    }
}
