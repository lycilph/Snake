using LyCilph.Elements;
using System;

namespace Trainer
{
    class Program
    {
        static void Main(string[] args)
        {
            var food = new Cell();
            var snake = new Snake();
            var sim = new Simulation(food, snake, runs_per_specimen: 5 );
            var population = new Population(food, snake, size: 1000, mutation_rate: 0.01);

            sim.Run(population, generations: 10);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
