using System;

namespace Trainer
{
    class Program
    {
        static void Main(string[] args)
        {
            var population = new Population(size: 2000, mutation_rate: 0.01, percent_to_keep: 25);
            population.Simulate(generations: 100, runs: 5);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
