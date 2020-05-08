using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.TileEngine.TileObjects;

namespace TileEngineSfmlCs.TileEngine.ChemistryManagement
{
    public class SimpleSubstanceContainer : ISubstanceContainer
    {
        private float _maxVolume;
        private float _transferAmount;

        private SubstanceMixture _mixture = new SubstanceMixture();

        public SimpleSubstanceContainer(float maxVolume, float transferAmount)
        {
            _maxVolume = maxVolume;
            _transferAmount = transferAmount;
        }

        public void DoReactions()
        {
            _mixture.React();
        }

        public TileObject ContainingTileObject { get; set; }
        public float RemainingVolume => _maxVolume - _mixture.Volume;
        public float MaximumVolume => _maxVolume;

        public SubstanceMixture Contents
        {
            get { return _mixture; }
        }


        public void TransferInto(SubstanceMixture incomingMixture)
        {
            float remainingVolume = RemainingVolume;

            SubstanceMixture concatinationMixture;

            if (remainingVolume >= incomingMixture.Volume)
            {
                concatinationMixture = incomingMixture.Clone() as SubstanceMixture;
                incomingMixture.Clear();
            }
            else
            {
                concatinationMixture = incomingMixture.SubtractVolume(remainingVolume);
            }

            _mixture.Concatinate(concatinationMixture);

            LogManager.RuntimeLogger.Log($"{ContainingTileObject?.VisibleName} received substance mixture. New contents: " + _mixture);
        }

        public void TransferToAnother(ISubstanceContainer otherContainer)
        {
            LogManager.RuntimeLogger.Log($"{ContainingTileObject?.VisibleName} transfers substance mixture into other container. New contents: " + _mixture);

            float amount = Math.Min(_transferAmount, _mixture.Volume);

            if (Math.Abs(amount) < 0.0001f)
            {
                LogManager.RuntimeLogger.Log("Nothing to transfer!");
            }
            else
            {
                LogManager.RuntimeLogger.Log("Amount: " + amount);
                SubstanceMixture subtractedMixture = _mixture.SubtractVolume(amount);

                otherContainer.TransferInto(subtractedMixture);

                _mixture.Concatinate(subtractedMixture);
            }
        }
    }
}
