using LyCilph.Elements;
using System.Collections.Generic;

namespace Trainer
{
    public class SerializedSimulation
    {
        public int SimulationRuns { get; set; }
        public int PercentToKeep { get; set; }
        public double MutationRate { get; set; }
        public int Generation { get; set; }
        public double TotalRuntime { get; set; }
        public List<double> FitnessStatistics { get; set; }
        public List<Chromosome> Chromosomes { get; set; }
    }
}
