using System;
using System.Xml;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.TileEngine.ChemistryManagement
{
    public struct Reaction : IFieldSerializer
    {
        public Reaction(Reagent[] reagents, Reagent[] results, float minTemp = 0)
        {
            Reagents = reagents;
            Results = results;
            MinimalTemperature = minTemp;
            ThermalFactor = 1;
        }

        public Reagent[] Reagents { get; set; }
        public Reagent[] Results { get; set; }
        public float MinimalTemperature { get; set; }
        public float ThermalFactor { get; set; }

        public void AppendFields(XmlElement parentElement)
        {
            SerializationUtils.WriteParseables(Reagents, nameof(Reagents), parentElement);
            SerializationUtils.WriteParseables(Results, nameof(Results), parentElement);
            SerializationUtils.Write(MinimalTemperature, nameof(MinimalTemperature), parentElement);
            SerializationUtils.Write(ThermalFactor, nameof(ThermalFactor), parentElement);
        }

        public void ReadFields(XmlElement parentElement)
        {
            Reagents = SerializationUtils.ReadParseables(nameof(Reagents), parentElement, new Reagent[0]);
            Results = SerializationUtils.ReadParseables(nameof(Results), parentElement, new Reagent[0]);
            MinimalTemperature =
                SerializationUtils.ReadFloat(nameof(MinimalTemperature), parentElement, MinimalTemperature);
            ThermalFactor = SerializationUtils.ReadFloat(nameof(ThermalFactor), parentElement, ThermalFactor);
        }
    }
}
