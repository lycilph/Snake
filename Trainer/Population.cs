using LyCilph.Elements;
using LyCilph.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Trainer
{
    public class Population
    {
        private Random rnd = new Random((int)DateTime.Now.Ticks);
        private double mutation_rate;
        private double percent_to_keep;

        public List<Individual> old_generation;
        public List<Individual> new_generation;

        public int Size { get { return new_generation.Count; } }

        public Population(int size, double mutation_rate, double percent_to_keep)
        {
            this.mutation_rate = mutation_rate;
            this.percent_to_keep = percent_to_keep;
            old_generation = Enumerable.Range(0, size).Select(_ => new Individual()).ToList();
            new_generation = Enumerable.Range(0, size).Select(_ => new Individual()).ToList();
        }

        public void Simulate(int generations, int runs)
        {
            for (int gen = 0; gen < generations; gen++)
            {
                Console.WriteLine($"Generation {gen} of {generations}");

                // Start timing
                var sw = Stopwatch.StartNew();

                // Simulate the new generation
                Parallel.ForEach(new_generation, i => i.Simulate(runs));

                Console.WriteLine("   Top 5");
                var top5 = new_generation.OrderByDescending(i => i.fitness).Take(5).ToList();
                top5.ForEach(i => Console.WriteLine($"   Fitness {i.fitness}, Age {i.average_age}, Score {i.average_score}"));

                // Swap generations
                (old_generation, new_generation) = (new_generation, old_generation);

                // Keep the best as is
                var amount_to_keep = (int)Math.Round(Size * (percent_to_keep / 100.0));
                var old_generation_sorted = old_generation.OrderByDescending(s => s.fitness).ToList();
                for (int i = 0; i < amount_to_keep; i++)
                {
                    new_generation[i].Chromosome = old_generation_sorted[i].Chromosome;
                }

                var total_fitness = old_generation.Sum(s => s.fitness);
                for (int i = amount_to_keep; i < Size; i++)
                {
                    var parent1_chromosome = Select(total_fitness);
                    var parent2_chromosome = Select(total_fitness);
                    var child_chromosome = Chromosome.Cross(parent1_chromosome, parent2_chromosome);
                    child_chromosome.Mutate(mutation_rate);

                    new_generation[i].Chromosome = child_chromosome;
                }

                sw.Stop();
                Console.WriteLine($"   Elapsed time: {sw.ElapsedMilliseconds} ms");
            }
            
            Console.WriteLine("Finding top 10 individuals");
            Parallel.ForEach(new_generation, i => i.Simulate(1));
            var top10 = new_generation.OrderByDescending(i => i.fitness).Take(10).ToList();
            top10.ForEach(i => Console.WriteLine($"Fitness {i.fitness}, Age {i.average_age}, Score {i.average_score}"));

            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(dir, "best.snake");
            JsonUtils.WriteToFile(filename, top10.First().Chromosome);
        }

        private Chromosome Select(double total_fitness)
        {
            var s = rnd.NextDouble() * total_fitness;
            var running_sum = 0.0;
            foreach (var specimen in old_generation)
            {
                running_sum += specimen.fitness;
                if (running_sum > s)
                    return specimen.Chromosome;
            }
            return null;
        }
    }
}
