using LyCilph.Controllers;
using LyCilph.Elements;
using System;
using System.Diagnostics;
using System.Linq;

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
            for (int i = 0; i < generations; i++)
            {
                Console.WriteLine($"Generation {i + 1} of {generations} - population size {population.Size}");

                var sw = Stopwatch.StartNew();
                Run(population);

                // Debug information
                var best = population.current_generation.OrderBy(s => s.fitness).Last();
                Console.WriteLine($"   Fitness: {best.fitness:N2}");
                Console.WriteLine($"   Age: {best.average_age:N2}");
                Console.WriteLine($"   Score: {best.average_score:N2}");
                Console.WriteLine($"   Generation simulation: {sw.ElapsedMilliseconds / 1000.0} s");

                population.NextGeneration();
                sw.Stop();
            }
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
