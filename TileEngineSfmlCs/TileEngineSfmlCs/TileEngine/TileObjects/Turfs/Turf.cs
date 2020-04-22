using System.Linq;
using TileEngineSfmlCs.TileEngine.Logging;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Turfs
{
    /// <summary>
    /// Turfs are objects that cover entire tile. Examples: walls, floor tiles
    /// </summary>
    public abstract class Turf : TileObject
    {
        /// <summary>
        /// Checks if turf's layer is already occupied. Can be called before Instantiate
        /// </summary>
        /// <param name="scene">Target scene</param>
        /// <param name="cell">Target position</param>
        /// <returns>True if layer is occupied</returns>
        protected bool IsLayerOccupied(Scene scene, Vector2Int cell)
        {
            var tos = scene.GetObjectsOnLayer(cell, Layer);

            for (int i = 0; i < tos.Length; i++)
            {
                if (tos[i] != this)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Ensures that this turf can be placed on specified cell.  Can be called before Instantiate
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="cell">Target position</param>
        /// <returns>True if placement is available</returns>
        public virtual bool ValidatePlacement(Scene scene, Vector2Int cell)
        {
            return !IsLayerOccupied(scene, cell);
        }

        internal override void OnEditorCreate()
        {
            if (!ValidatePlacement(Scene, Position))
            {
                Scene.DestroyEditor(this);
                LogManager.EditorLogger.LogError($"Turf [{GetType().Name}]({InstanceId}) - placement invalid");
            }
        }
    }
}
