using Microsoft.Xna.Framework.Input;
using Snake;
using System;
using System.Diagnostics;

namespace SnakeTrainer
{
    public class Simulation
    {
        public bool silent = true;

        public int average_score;
        public int average_age;
        public int runs;
        public double fitness;

        public Board board;
        public Food food;
        public Snake.Snake snake;
        public Brain brain;

        private bool alive;
        private KeyboardState dummy;

        public Simulation()
        {
            board = new Board(null, 0, 60);
            food = new Food(null, board);
            snake = new Snake.Snake(null, board);
            brain = new NeuralNetworkBrain(board, food, snake);
            dummy = new KeyboardState();

            average_score = 0;
            average_age = 0;
            runs = 5;
        }

        public void Reset()
        {
            food.Random();
            snake.Reset();
            alive = true;
        }

        public void Run()
        {
            for (int i = 0; i < runs; i++)
            {
                Reset();
                RunInternal();
            }

            average_score /= runs;
            average_age /= runs;
            fitness = Math.Log10(average_age) + average_score;

            if (!silent)
            {
                Console.WriteLine($"Average score: {average_score}");
                Console.WriteLine($"Average age: {average_age}");
            }
        }

        private void RunInternal()
        {
            if (!silent)
                Console.WriteLine("Running simulation");
            var sw = Stopwatch.StartNew();
            while (alive)
            {
                brain.React(dummy, dummy);
                snake.Move();

                // Was the snake out of bounds?
                if (snake.Head.X < 0 || snake.Head.X >= board.BoardSizeInCells || snake.Head.Y < 0 || snake.Head.Y >= board.BoardSizeInCells)
                {
                    snake.CauseOfDeath = "Hit border";
                    alive = false;
                }

                // Did the snake hit itself?
                if (snake.HitSelf())
                {
                    snake.CauseOfDeath = "Hit self";
                    alive = false;
                }

                // Did the snake eat the food?
                if (snake.Head.X == food.X && snake.Head.Y == food.Y)
                {
                    snake.HasEaten = true;
                    food.Random();
                }

                // Did the snake die of hunger?
                if (snake.Energy <= 0)
                {
                    snake.CauseOfDeath = "Died of hunger";
                    alive = false;
                }
            }
            sw.Stop();

            if (!silent)
            {
                Console.WriteLine("Game over");
                Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds}");
                Console.WriteLine($"Score: {snake.Score}");
                Console.WriteLine($"Age: {snake.Age}");
                Console.WriteLine($"Energy: {snake.Energy}");
                Console.WriteLine($"Death: {snake.CauseOfDeath}");
            }

            average_score += snake.Score;
            average_age += snake.Age;
        }
    }
}
