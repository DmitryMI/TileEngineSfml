using System;
using System.Collections;
using System.Collections.Generic;

namespace TileEngineSfmlCs.TileEngine.ChemistryManagement
{
    public class SubstanceMixture : IList<Substance>
    {
        private IList<Substance> _listImplementation;

        private bool _wasModified;
        private float _volume;
        private float _temperature;

        public SubstanceMixture(int capacity = 0, float temperature = 0)
        {
            _listImplementation = new List<Substance>(capacity);

            _temperature = 0;
        }

        public float Volume
        {
            get
            {
                if (_wasModified)
                    RecalculateValues();
                return _volume;
            }
        }

        public float Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }

        public SubstanceMixture SubtractPart(float part)
        {
            if (part > 1 || part <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(part), "Must be inside (0, 1]");
            }


            SubstanceMixture mixture = new SubstanceMixture(_listImplementation.Count);

            for (int i = 0; i < _listImplementation.Count; i++)
            {
                Substance info = _listImplementation[i];
                Substance subtracted = info;
                subtracted.Volume *= part;
                info.Volume -= subtracted.Volume;
                this.Add(subtracted);
                _listImplementation[i] = info;
            }

            return this;
        }

        public SubstanceMixture SubtractVolume(float volume)
        {
            if (volume > Volume)
                throw new ArgumentOutOfRangeException(nameof(volume), "Must be inside (0, Volume of this instance]. Current volume: " + Volume);

            SubstanceMixture mixture = new SubstanceMixture(_listImplementation.Count);

            float volumeTmp = Volume;

            for (int i = 0; i < _listImplementation.Count; i++)
            {
                Substance info = _listImplementation[i];
                Substance subtracted = info;
                subtracted.Volume = volume * info.Volume / volumeTmp;
                info.Volume -= subtracted.Volume;
                _listImplementation[i] = info;

                this.Add(subtracted);
            }

            return this;
        }

        public void Concatinate(SubstanceMixture otherMixture)
        {

            _temperature = (_temperature * Volume + otherMixture.Temperature * otherMixture.Volume) /
                           (Volume + otherMixture.Volume);

            foreach (var substance in otherMixture)
            {
                int index = IndexOfSubstance(substance.SubstanceId);

                if (index == -1)
                {
                    _listImplementation.Add(substance);
                    //otherMixture.Remove(Substance);
                }
                else
                {
                    Substance info = _listImplementation[index];
                    /*info.Temperature = (Substance.Temperature * Substance.Volume + info.Temperature * info.Volume) / 
                        (info.Volume + Substance.Volume);*/

                    info.Volume += substance.Volume;
                    _listImplementation[index] = info;
                }
            }

            _wasModified = true;
        }

        public int IndexOfSubstance(SubstanceId substanceId)
        {
            for (int i = 0; i < _listImplementation.Count; i++)
            {
                if (_listImplementation[i].SubstanceId == substanceId)
                    return i;
            }
            return -1;
        }

        public float GetElementPart(int index)
        {
            return _listImplementation[index].Volume / Volume;
        }

        public float GetElementPart(Substance info)
        {
            int index = _listImplementation.IndexOf(info);
            return GetElementPart(index);
        }

        private void RecalculateValues()
        {
            _volume = GetVolume();

            _wasModified = false;
        }

        private float GetVolume()
        {
            float volume = 0;
            foreach (var substance in _listImplementation)
            {
                volume += substance.Volume;
            }
            return volume;
        }

        public IEnumerator<Substance> GetEnumerator()
        {
            return _listImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_listImplementation).GetEnumerator();
        }

        public void Add(Substance item)
        {
            _wasModified = true;
            _listImplementation.Add(item);
        }

        public void Clear()
        {
            _listImplementation.Clear();
            _wasModified = true;
        }

        public bool Contains(Substance item)
        {
            return _listImplementation.Contains(item);
        }

        public void CopyTo(Substance[] array, int arrayIndex)
        {
            _listImplementation.CopyTo(array, arrayIndex);
        }

        public bool Remove(Substance item)
        {
            bool result = _listImplementation.Remove(item);
            if (result)
                _wasModified = true;
            return result;
        }

        public int Count
        {
            get { return _listImplementation.Count; }
        }

        public bool IsReadOnly
        {
            get { return _listImplementation.IsReadOnly; }
        }

        public int IndexOf(Substance item)
        {
            return _listImplementation.IndexOf(item);
        }

        public void Insert(int index, Substance item)
        {
            _wasModified = true;
            _listImplementation.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _listImplementation.RemoveAt(index);
            _wasModified = true;
        }

        public Substance this[int index]
        {
            get { return _listImplementation[index]; }
            set
            {
                _wasModified = true;
                _listImplementation[index] = value;
            }
        }

        public void AddRange(ICollection<Substance> collection)
        {
            foreach (var elem in collection)
            {
                _listImplementation.Add(elem);
            }

            _wasModified = true;
        }

        public override string ToString()
        {
            if (_listImplementation.Count == 0)
                return "<Empty>";

            string message = "";
            for (int i = 0; i < _listImplementation.Count; i++)
            {
                message += $"[{i}]: {_listImplementation[i]}; ";
            }
            return message;
        }

        public object Clone()
        {
            SubstanceMixture mixture = new SubstanceMixture(_listImplementation.Count);
            this.AddRange(_listImplementation);
            return this;
        }

        public void React()
        {
            foreach (var reaction in ChemistryManager.Instance.AvailableReactions)
            {
                int[] reagentMixtureIndexes;

                bool allReagentsArePresent = FindReagentIndexes(reaction, out reagentMixtureIndexes);

                if (allReagentsArePresent)
                {
                    float[] reagentVolumes = GetReagentVolumes(reagentMixtureIndexes);

                    float[] factors = GetFactors(reagentVolumes, reaction.Reagents);

                    int minFactorIndex = FindMinIndex(factors);

                    float reactionFactor = factors[minFactorIndex];

                    DoReaction( reaction, reagentMixtureIndexes, reactionFactor);
                }
            }
        }

        private void DoReaction(Reaction reaction, int[] reagentsIndexes, float factor)
        {
            for (int i = 0; i < reagentsIndexes.Length; i++)
            {
                int reagentIndex = reagentsIndexes[i];
                if (reagentIndex < 0 || reagentIndex >= this.Count || this.Count == 0)
                {
                    return;
                }
                Substance substanceInfo = this[reagentIndex];
                float reactionVolume = factor * reaction.Reagents[i].Mole;
                substanceInfo.Volume -= reactionVolume;

                if (substanceInfo.Volume > 0)
                {
                    this[reagentIndex] = substanceInfo;
                }
                else
                {
                    this.RemoveAt(reagentIndex);
                }
            }

            SubstanceMixture additions = new SubstanceMixture(0, this.Temperature + reaction.ThermalFactor * factor);

            for (int i = 0; i < reaction.Results.Length; i++)
            {
                //SubstanceId id = ChemistryController.Current.GetSubstance(reaction.Results[i].SubstanceName).Id;
                SubstanceId id = reaction.Results[i].SubstanceId;
                if (id != SubstanceId.IncorrectSubstance)
                    additions.Add(new Substance(id, factor * reaction.Results[i].Mole));
            }

            this.Concatinate(additions);
        }

        private float[] GetFactors(float[] reagentVolumes, Reagent[] reagents)
        {
            float[] factors = new float[reagentVolumes.Length];

            for (int i = 0; i < factors.Length; i++)
            {
                factors[i] = reagentVolumes[i] / reagents[i].Mole;
            }

            return factors;
        }

        private int FindMinIndex<T>(T[] floats) where T : IComparable
        {
            int min = 0;
            for (int i = 0; i < floats.Length; i++)
                if (floats[i].CompareTo(floats[min]) == -1)
                    min = i;

            return min;
        }

        private float[] GetReagentVolumes(int[] indexes)
        {
            float[] reagentVolumes = new float[indexes.Length];

            for (int i = 0; i < indexes.Length; i++)
                reagentVolumes[i] = this[indexes[i]].Volume;

            return reagentVolumes;
        }

        private int FindMaxMoleReagent(Reaction reaction)
        {
            int maxMoleIndex = 0;

            for (int i = 1; i < reaction.Reagents.Length; i++)
            {
                if (reaction.Reagents[i].Mole > reaction.Reagents[maxMoleIndex].Mole)
                    maxMoleIndex = i;
            }

            return maxMoleIndex;
        }

        private bool FindReagentIndexes(Reaction reaction, out int[] reagentIndexes)
        {
            reagentIndexes = new int[reaction.Reagents.Length];

            bool ok = true;

            for (int i = 0; i < reagentIndexes.Length && ok; i++)
            {
                SubstanceId reagentId = reaction.Reagents[i].SubstanceId;
                int index = FindSubstanceIndex(reagentId);

                if (index != -1)
                {
                    reagentIndexes[i] = index;
                }
                else
                {
                    ok = false;
                }
            }

            return ok;
        }

        private int FindSubstanceIndex(SubstanceId id)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].SubstanceId == id)
                    return i;
            }

            return -1;
        }
    }
}
