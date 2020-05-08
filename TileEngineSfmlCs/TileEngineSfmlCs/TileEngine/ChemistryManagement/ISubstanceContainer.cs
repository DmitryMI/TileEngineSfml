using TileEngineSfmlCs.TileEngine.TileObjects;

namespace TileEngineSfmlCs.TileEngine.ChemistryManagement
{
    public interface ISubstanceContainer
    { 
        TileObject ContainingTileObject { get; }
        float RemainingVolume { get; }
        float MaximumVolume { get; }
        void TransferInto(SubstanceMixture mixture);
        void TransferToAnother(ISubstanceContainer container);

        SubstanceMixture Contents { get; }
    }
}