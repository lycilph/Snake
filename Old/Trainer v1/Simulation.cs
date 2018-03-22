using LyCilph.Controllers;
using LyCilph.Elements;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Trainer
{
    public class Simulation
    {
        public Cell food;
        public Snake snake;
        public bool alive;
        public int runs_per_specimen;

        public Simulation(Cell food, Snake snake, int runs_per_specimen)
        {
            this.food = food;
            this.snake = snake;
            this.runs_per_specimen = runs_per_specimen;
        }

        public void Run(Population population, int generations)
        {
            var exe_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(exe_dir, "fitness.csv");
            File.Delete(filename);

            var sw_total = Stopwatch.StartNew();
            for (int i = 0; i < generations; i++)
            {
                Console.WriteLine($"Generation {i + 1} of {generations} - population size {population.Size}");

                var sw = Stopwatch.StartNew();
                Run(population);

                // Debug information
                var ordered = population.current_generation.OrderByDescending(s => s.fitness).ToList();
                var best = ordered.First();
                Console.WriteLine($"   Fitness: {best.fitness:N2}");
                Console.WriteLine($"   Age: {best.average_age:N2}");
                Console.WriteLine($"   Score: {best.average_score:N2}");
                Console.WriteLine($"   Simulation: {sw.ElapsedMilliseconds / 1000.0} s");

                var fitness_values = ordered.Select(s => s.fitness);
                var fitness_values_text = string.Join(";", fitness_values);
                File.AppendAllText(filename, fitness_values_text + Environment.NewLine);

                population.NextGeneration();
                sw.Stop();
            }
            sw_total.Stop();
            Console.WriteLine($"Total time: {sw_total.ElapsedMilliseconds / 1000.0} s");
        }

        public void Run(Population population)
        {
            population.current_generation.ForEach(s => Run(s));
        }

        public void Run(Specimen specimen)
        {
            for (int i = 0; i < runs_per_specimen; i++)
            {
                Simulate(specimen);
            }
            specimen.average_age /= runs_per_specimen;
            specimen.average_score /= runs_per_specimen;
            specimen.CalculateFitness();
        }

        private void Simulate(Specimen specimen)
        {
            snake.Reset();

            alive = true;
            while (alive)
            {
                Update(specimen.controller);
            }

            specimen.average_age += snake.Age;
            specimen.average_score += snake.Score;
        }

        private void Update(NeuralNetworkController controller)
        {
            controller.HandleInput(null);

            snake.Move();

            // Did the snake eat the food?
            if (food.Hit(snake.Head))
            {
                snake.HasEaten = true;
                CreateFood();
            }

            // Did the snake die
            if (snake.OutOfBoard() || snake.HitSelf() || snake.Energy <= 0)
            {
                alive = false;
                return;
            }
        }

        private void CreateFood()
        {
            var retry = true;
            while (retry)
            {
                food.Random();
                retry = snake.Hit(food);
            }
        }
    }
}
