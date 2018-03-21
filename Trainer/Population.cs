using LyCilph.Controllers;
using LyCilph.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trainer
{
    public class Population
    {
        private Random rnd = new Random((int)DateTime.Now.Ticks);

        //private Chromosome global_best_chromosome;
        //private double global_best_fitness;
        private double mutation_rate;

        public List<Specimen> old_generation = new List<Specimen>();
        public List<Specimen> current_generation = new List<Specimen>();

        public int Size { get { return current_generation.Count; } }

        public Population(Cell food, Snake snake, int size, double mutation_rate)
        {
            this.mutation_rate = mutation_rate;
            for (int i = 0; i < size; i++)
            {
                old_generation.Add(new Specimen(food, snake));
                current_generation.Add(new Specimen(food, snake));
            }
        }

        public void NextGeneration()
        {
            // Swap generations
            (old_generation, current_generation) = (current_generation, old_generation);

            // Keep 10% best (not just global best)
            var top_10_percent_count = current_generation.Count / 10;
            var old_generation_sorted = old_generation.OrderByDescending(s => s.fitness).ToList();
            for (int i = 0; i < top_10_percent_count; i++)
            {
                current_generation[i].Chromosome = old_generation_sorted[i].Chromosome;
            }

            var total_fitness = old_generation.Sum(s => s.fitness);
            for (int i = top_10_percent_count; i < current_generation.Count; i++)
            {
                var parent1_chromosome = Select(total_fitness);
                var parent2_chromosome = Select(total_fitness);
                var child_chromosome = Chromosome.Cross(parent1_chromosome, parent2_chromosome);
                child_chromosome.Mutate(mutation_rate);

                current_generation[i].Chromosome = child_chromosome;
            }
        }

        public Chromosome Select(double total_fitness)
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
