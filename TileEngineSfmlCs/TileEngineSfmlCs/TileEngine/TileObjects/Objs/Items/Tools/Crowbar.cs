using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.TileEngine.Containers;
using TileEngineSfmlCs.TileEngine.Interaction;
using TileEngineSfmlCs.TileEngine.TileObjects.Mobs;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Objs.Items.Tools
{
    public class Crowbar : Tool
    {
        private Icon _icon = new Icon("Images\\Items\\Crowbar\\Crowbar.png");
        private Icon _editorIcon = new Icon("Images\\Items\\Crowbar\\Crowbar.png");
        private string _visibleName = "Crowbar";
        private string _examineDescription = "A small crowbar. This handy tool is useful for lots of things, such as prying floor tiles or opening unpowered doors.";
        private bool _requiresUpdates = false;
        private IObjectContainer _container;

        #region Item Realization 
        public override InteractionResult ApplyItem(Item appliedItem, Mob interactionSource,
            InteractionDescriptor descriptor)
        {
            return InteractionResult.ContinueChain;
        }

        public override InteractionResult DragDrop(TileObject draggedObject, Mob interactionSource,
            InteractionDescriptor descriptor)
        {
            return InteractionResult.ContinueChain;
        }

        public override InteractionResult DragStart(Mob interactionSource, InteractionDescriptor descriptor)
        {
            return InteractionResult.ContinueChain;
        }
        #endregion

        #region TileObjectRealization
        public override Icon Icon => _icon;

        public override Icon EditorIcon => _editorIcon;

        public override string VisibleName => _visibleName;

        public override string ExamineDescription => _examineDescription;

        public override bool RequiresUpdates => _requiresUpdates;

        public override IObjectContainer Container
        {
            get => _container;
            set => _container = value;
        }

        public override bool IsActiveOnScene => GetSceneActivity();

        #endregion

        #region IFieldSerializer Realization
        
        protected override void AppendUserFields(XmlElement baseElement)
        {

        }

        protected override void ReadUserFields(XmlElement baseElement)
        {

        }
        #endregion

        #region IconSettings

        public override Icon RightHandOverlayFront { get; } =
            new Icon("Images\\Items\\Crowbar\\CrowbarRightHandFront.png");

        public override Icon RightHandOverlayRight { get; } =
            new Icon("Images\\Items\\Crowbar\\CrowbarRightHandRight.png");

        public override Icon RightHandOverlayBack { get; } =
            new Icon("Images\\Items\\Crowbar\\CrowbarRightHandBack.png");

        public override Icon RightHandOverlayLeft { get; } =
            new Icon("Images\\Items\\Crowbar\\CrowbarRightHandLeft.png");

        public override Icon LeftHandOverlayFront { get; } =
            new Icon("Images\\Items\\Crowbar\\CrowbarLeftHandFront.png");

        public override Icon LeftHandOverlayRight { get; } =
            new Icon("Images\\Items\\Crowbar\\CrowbarLeftHandRight.png");

        public override Icon LeftHandOverlayBack { get; } = new Icon("Images\\Items\\Crowbar\\CrowbarLeftHandBack.png");
        public override Icon LeftHandOverlayLeft { get; } = new Icon("Images\\Items\\Crowbar\\CrowbarLeftHandLeft.png");

        #endregion

        #region ToolRealization

        public override ToolBehaviour Behaviour => ToolBehaviour.Crowbar;

        #endregion

        private bool GetSceneActivity()
        {
            if (_container != null)
                return false;
            return true;
        }
    }
}
