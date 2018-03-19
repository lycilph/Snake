using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeTrainer
{
    public class Population
    {
        public List<Simulation> simulations;

        public Population(int size)
        {
            simulations = Enumerable.Range(0, size).Select(_ => new Simulation()).ToList();
        }

        public void Run()
        {
            var percent = simulations.Count / 10;
            for (int i = 0; i < simulations.Count; i++)
            {
                simulations[i].Run();

                if (i % percent == 0)
                    Console.WriteLine($"Iteration: {i} of {simulations.Count}");
            }
            Console.WriteLine($"Max fitness: {simulations.Max(s => s.fitness)}");
        }

        public void Select()
        {

        }
    }
}
