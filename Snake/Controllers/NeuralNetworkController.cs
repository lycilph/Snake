using LyCilph.Elements;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;

namespace LyCilph.Controllers
{
    public class NeuralNetworkController : Controller
    {
        private Random rnd = new Random((int)DateTime.Now.Ticks);

        private Matrix<double> input_to_hidden1_weights;
        private Matrix<double> hidden1_to_hidden2_weights;
        private Matrix<double> hidden2_to_output_weights;

        public Vector<double> Input { get; set; }
        public Vector<double> Output { get; set; }

        /* Input indexes and their meaning
         
           Distances to edges (distance normalized by board size)
           7   0   1
            
           6   *   2

           5   4   3

           Distances to body (distance normalized by board size)
           15  8   9
            
           14  *   10

           13  12  11

           Food found in direction (0 == no food, 1 == food in that direction)
           23  16  17
            
           22  *   18

           21  20  19

           24 bias input
        */

        public NeuralNetworkController(Cell food, Snake snake, Chromosome chromosome = null) : base(food, snake)
        {
            Input = Vector<double>.Build.Dense(25); // Add a bias input here
            Output = Vector<double>.Build.Dense(4);

            input_to_hidden1_weights = Matrix<double>.Build.Dense(25, 18); // Add a bias to the output
            hidden1_to_hidden2_weights = Matrix<double>.Build.Dense(19, 18); // Add a bias to the output
            hidden2_to_output_weights = Matrix<double>.Build.Dense(19, 4);

            Reset();
            if (chromosome != null)
                SetChromosome(chromosome);
        }

        public void Reset()
        {
            Input[24] = 1;

            // randomize weights
            input_to_hidden1_weights.Map(_ => rnd.NextDouble() * 2.0 - 1.0, input_to_hidden1_weights);
            hidden1_to_hidden2_weights.Map(_ => rnd.NextDouble() * 2.0 - 1.0, hidden1_to_hidden2_weights);
            hidden2_to_output_weights.Map(_ => rnd.NextDouble() * 2.0 - 1.0, hidden2_to_output_weights);
        }

        public Chromosome GetChromosome()
        {
            return new Chromosome
            {
                input_to_hidden1_weights = input_to_hidden1_weights.ToColumnMajorArray(),
                hidden1_to_hidden2_weights = hidden1_to_hidden2_weights.ToColumnMajorArray(),
                hidden2_to_output_weights = hidden2_to_output_weights.ToColumnMajorArray()
            };
        }

        public void SetChromosome(Chromosome chromosome)
        {
            input_to_hidden1_weights = Matrix<double>.Build.Dense(25, 18, chromosome.input_to_hidden1_weights);
            hidden1_to_hidden2_weights = Matrix<double>.Build.Dense(19, 18, chromosome.hidden1_to_hidden2_weights);
            hidden2_to_output_weights = Matrix<double>.Build.Dense(19, 4, chromosome.hidden2_to_output_weights);
        }

        public override void HandleInput(InputManager input)
        {
            // Get input for ai
            UpdateInput();
            // Run neural network
            Execute();
            // Get output from ai and move
            SetDirection();
        }

        private void UpdateInput()
        {
            // Check up direction (distance_to_edge, distance_to_body, found_food)
            (Input[0], Input[8], Input[16]) = CheckDirection(food, 0, -1);

            // Check up and right direction (distance_to_edge, distance_to_body, found_food)
            (Input[1], Input[9], Input[17]) = CheckDirection(food, 1, -1);

            // Check right direction (distance_to_edge, distance_to_body, found_food)
            (Input[2], Input[10], Input[18]) = CheckDirection(food, 1, 0);

            // Check down and right direction (distance_to_edge, distance_to_body, found_food)
            (Input[3], Input[11], Input[19]) = CheckDirection(food, 1, 1);

            // Check down direction (distance_to_edge, distance_to_body, found_food)
            (Input[4], Input[12], Input[20]) = CheckDirection(food, 0, 1);

            // Check down and left direction (distance_to_edge, distance_to_body, found_food)
            (Input[5], Input[13], Input[21]) = CheckDirection(food, -1, 1);

            // Check left direction (distance_to_edge, distance_to_body, found_food)
            (Input[6], Input[14], Input[22]) = CheckDirection(food, -1, 0);

            // Check up and left direction (distance_to_edge, distance_to_body, found_food)
            (Input[7], Input[15], Input[23]) = CheckDirection(food, -1, -1);
        }

        private (double, double, double) CheckDirection(Cell food, int x, int y)
        {
            var p = new Cell(snake.Head);
            p.Add(x, y);

            var diff = Math.Abs(x) + Math.Abs(y);
            var distance = diff;

            double found_food = 0;
            double distance_to_edge = 0;
            double distance_to_body = 0;
            while (p.X >= 0 && p.X < Settings.board_size && p.Y >= 0 && p.Y < Settings.board_size)
            {
                // Check for food
                if (food.Hit(p))
                    found_food = 1;

                // Check if body is hit
                if (snake.Hit(p) && distance_to_body == 0)
                    distance_to_body = (double)distance / Settings.board_size;

                p.Add(x, y);
                distance += diff;
            }

            distance_to_edge = (double)distance / Settings.board_size;

            return (distance_to_edge, distance_to_body, found_food);
        }

        private void Execute()
        {
            var hidden1_output = Input * input_to_hidden1_weights;
            hidden1_output.Map(d => SpecialFunctions.Logistic(d), hidden1_output);

            var hidden1_output_with_bias = Vector<double>.Build.Dense(19);
            hidden1_output.CopySubVectorTo(hidden1_output_with_bias, 0, 0, 18);
            hidden1_output_with_bias[18] = 1; // Add a bias input for the next hidden layer


            var hidden2_output = hidden1_output_with_bias * hidden1_to_hidden2_weights;
            hidden2_output.Map(d => SpecialFunctions.Logistic(d), hidden2_output);

            var hidden2_output_with_bias = Vector<double>.Build.Dense(19);
            hidden2_output.CopySubVectorTo(hidden2_output_with_bias, 0, 0, 18);
            hidden2_output_with_bias[18] = 1; // Add a bias input for the next hidden layer


            Output = hidden2_output_with_bias * hidden2_to_output_weights;
            Output.Map(d => SpecialFunctions.Logistic(d), Output);
        }

        private void SetDirection()
        {
            var new_dir = Output.MaximumIndex();
            var illegal_dir = (snake.Dir + 2) % 4;

            if (new_dir == illegal_dir)
                return;

            snake.Dir = new_dir;
        }
    }
}
