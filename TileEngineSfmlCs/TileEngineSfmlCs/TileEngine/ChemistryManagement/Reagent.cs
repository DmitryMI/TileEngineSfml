using System.Xml;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.TileEngine.ChemistryManagement
{
    public struct Reagent : IFieldSerializer
    {
        public SubstanceId SubstanceId;
        public int Mole;

        public Reagent(SubstanceId id, int mole)
        {
            SubstanceId = id;
            Mole = mole;
        }

        public void AppendFields(XmlElement parentElement)
        {
            SerializationUtils.Write(SubstanceId.ToString(), nameof(SubstanceId), parentElement);
            SerializationUtils.Write(Mole, nameof(Mole), parentElement);
        }

        public void ReadFields(XmlElement parentElement)
        {
            SubstanceId =
                SerializationUtils.ReadEnum(nameof(SubstanceId), parentElement, SubstanceId.IncorrectSubstance);
            Mole = SerializationUtils.ReadInt(nameof(Mole), parentElement, Mole);
        }
    }
}
