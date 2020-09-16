using TileEngineSfmlCs.TileEngine.Interaction;
using TileEngineSfmlCs.TileEngine.TileObjects.Mobs;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Objs.Items
{
    public abstract class Item : Obj
    {
        public abstract Icon RightHandOverlayFront { get; }
        public abstract Icon RightHandOverlayBack{ get; }
        public abstract Icon RightHandOverlayRight { get; }
        public abstract Icon RightHandOverlayLeft { get; }
        public abstract Icon LeftHandOverlayFront { get; }
        public abstract Icon LeftHandOverlayBack { get; }
        public abstract Icon LeftHandOverlayRight { get; }
        public abstract Icon LeftHandOverlayLeft { get; }

        public override bool IsPassable => true;
        public override bool IsLightTransparent => true;
        public override bool IsGasTransparent => true;

        public override TileLayer Layer => TileLayer.Items;

        public override void TryPass(TileObject sender)
        {
            
        }

        /// <summary>
        /// Handles actions before item was applied on any TileObject
        /// </summary>
        /// <param name="target">Interaction target</param>
        /// <param name="interactionSource">Interacting mob</param>
        /// <returns>If interaction chain must be finished, return FinishChain. Else return ContinueChain</returns>
        public virtual InteractionResult OnPreApply(TileObject target, Mob interactionSource)
        {
            return InteractionResult.ContinueChain;
        }

        /// <summary>
        /// Handles actions after item was applied on any TileObject
        /// </summary>
        /// <param name="target">Interaction target</param>
        /// <param name="interactionSource">Interacting mob</param>
        /// <returns>If interaction chain must be finished, return FinishChain. Else return ContinueChain</returns>
        public virtual InteractionResult OnPostApply(TileObject target, Mob interactionSource)
        {
            return InteractionResult.ContinueChain;
        }
    }
}
