using System;

namespace TileEngineSfmlCs.TileEngine.ChemistryManagement
{
    public struct Substance
    {
        private float _volume;
        private SubstanceId _substanceId;

        public Substance(SubstanceId id, float volume)
        {
            _substanceId = id;
            _volume = volume;
        }

        public SubstanceId SubstanceId => _substanceId;

        public float Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        public int CompareTo(object obj)
        {
            Substance other = (Substance)obj;
            return Math.Sign(_substanceId - other.SubstanceId);
        }

        public override string ToString()
        {
            return $"Id: {_substanceId}, Volume: {_volume}";
        }
    }
}