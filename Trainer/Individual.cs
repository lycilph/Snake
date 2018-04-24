using LyCilph.Controllers;
using LyCilph.Elements;
using System;

namespace Trainer
{
    public class Individual
    {
        private Cell food;
        private Snake snake;
        private NeuralNetworkController controller;
        private bool alive;

        public double average_score;
        public double average_age;
        public double fitness;

        public Chromosome Chromosome
        {
            get { return controller.GetChromosome(); }
            set { controller.SetChromosome(value); }
        }

        public Individual()
        {
            food = new Cell();
            snake = new Snake();
            controller = new NeuralNetworkController(food, snake);
        }

        public void Simulate(int runs)
        {
            average_age = 0;
            average_score = 0;

            for (int i = 0; i < runs; i++)
            {
                SimulateSingleRun();

                average_age += snake.Age;
                average_score += snake.Score;
            }

            average_age /= runs;
            average_score /= runs;
            CalculateFitness();
        }


        public void SimulateSingleRun()
        {
            snake.Reset();
            CreateFood();

            alive = true;
            while (alive)
            {
                Update();
            }
        }

        public void CalculateFitness()
        {
            // Old fitness function
            //fitness = Math.Log10(average_age) + average_score * average_score;

            // New fitness function
            if (snake.Body.Count < 10)
                fitness = average_age * average_age * Math.Pow(2, snake.Body.Count);
            else
                fitness = average_age * average_age * Math.Pow(2, 10) * (snake.Body.Count - 9);
        }

        private void Update()
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
