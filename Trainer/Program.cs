using LyCilph.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trainer
{
    class Program
    {
        static void Main(string[] args)
        {
            var runs = 1000;
            var sim = new Simulation();
            var nns = Enumerable.Range(0, runs).Select(_ => new NeuralNetworkController(sim.food, sim.snake)).ToList();
            var results = Enumerable.Range(0, runs).ToList();

            for (int i = 0; i < runs; i++)
            {
                sim.Run(nns[i]);
                results[i] = sim.snake.Age;
            }
            Console.WriteLine($"Max age {results.Max()}");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
