using LyCilph.Controllers;
using LyCilph.Elements;
using System;
using System.Diagnostics;

namespace Trainer
{
    [DebuggerDisplay("Fitness {fitness}")]
    public class Specimen
    {
        public NeuralNetworkController controller;
        public double average_score;
        public double average_age;
        public double fitness;

        public Chromosome Chromosome
        {
            get { return controller.GetChromosome(); }
            set { controller.SetChromosome(value); }
        }

        public Specimen(Cell food, Snake snake)
        {
            controller = new NeuralNetworkController(food, snake);
        }

        public void CalculateFitness()
        {
            fitness = Math.Log10(average_age) + average_score * 100.0;
        }
    }
}
