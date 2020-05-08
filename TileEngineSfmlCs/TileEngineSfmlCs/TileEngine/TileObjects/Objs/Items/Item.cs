using TileEngineSfmlCs.TileEngine.Interaction;
using TileEngineSfmlCs.TileEngine.TileObjects.Mobs;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Objs.Items
{
    public abstract class Item : Obj
    {
        public override bool IsPassable => true;
        public override bool IsLightTransparent => true;
        public override bool IsGasTransparent => true;

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
