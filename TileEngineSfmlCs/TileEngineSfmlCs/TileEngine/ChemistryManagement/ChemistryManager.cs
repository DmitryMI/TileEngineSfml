using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.ResourceManagement;
using TileEngineSfmlCs.ResourceManagement.ResourceTypes;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.TileEngine.ChemistryManagement
{
    public class ChemistryManager
    {
        #region Singleton

        private static ChemistryManager _instance;

        public static ChemistryManager Instance
        {
            get => _instance;
            set => _instance = value;
        }

        #endregion

        #region Reactions

        private readonly List<Reaction> _loadedReactions = new List<Reaction>();

        private Reaction[] GetDebugReactions()
        {
            Reaction[] reactions =
            {
                new Reaction(
                    new[]
                    {
                        new Reagent(SubstanceId.Water, 1), new Reagent(SubstanceId.CalciumOxide, 1) // Source reagents
                    },
                    new[]
                    {
                        new Reagent(SubstanceId.CalciumHydroxide, 2) // Resulting reagents
                    }),

                new Reaction(
                    new[]
                    {
                        new Reagent(SubstanceId.Hydrogen, 3), new Reagent(SubstanceId.Nitrogen, 1) // Source reagents
                    },
                    new[]
                    {
                        new Reagent(SubstanceId.Ammonia, 3) // Resulting reagents
                    }),
            };

            return reactions;
        }


        public Reaction[] AvailableReactions => _loadedReactions.ToArray();

        public void LoadChemistryFromResources()
        {
            _loadedReactions.AddRange(GetDebugReactions());

            ResourceEntry chemSettings = GameResources.Instance.GetEntry("Chemistry\\Chemistry.xml");
            ResourceEntry reactionList = GameResources.Instance.GetEntry("Chemistry\\Reactions.xml");

            // TODO Load data
        }

        #endregion

        public ChemistryManager()
        {
            LoadChemistryFromResources();
        }
    }
}
