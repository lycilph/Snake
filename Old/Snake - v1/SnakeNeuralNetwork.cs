using System;
using System.Diagnostics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Snake
{
    [DebuggerDisplay("{Head.X}, {Head.Y}")]
    public class SnakeNeuralNetwork : SnakeBase
    {
        private Random rnd = new Random((int)DateTime.Now.Ticks);

        private Vector<double> input;
        private Vector<double> output;

        private Matrix<double> input_to_hidden1_weights;
        private Matrix<double> hidden1_to_hidden2_weights;
        private Matrix<double> hidden2_to_output_weights;

        public SnakeNeuralNetwork()
        {
            input = Vector<double>.Build.Dense(24);
            output = Vector<double>.Build.Dense(4);

            input_to_hidden1_weights = Matrix<double>.Build.Dense(24, 18);
            hidden1_to_hidden2_weights = Matrix<double>.Build.Dense(18, 18);
            hidden2_to_output_weights = Matrix<double>.Build.Dense(18, 4);

            // randomize weights
            input_to_hidden1_weights.Map(_ => rnd.NextDouble() * 2.0 - 1.0, input_to_hidden1_weights);
            hidden1_to_hidden2_weights.Map(_ => rnd.NextDouble() * 2.0 - 1.0, hidden1_to_hidden2_weights);
            hidden2_to_output_weights.Map(_ => rnd.NextDouble() * 2.0 - 1.0, hidden2_to_output_weights);
        }


        /* Distances to edges (actually inverse distance)
           7   0   1
            
           6   *   2

           5   4   3

           Distances to body (actually inverse distance)
           15  8   9
            
           14  *   10

           13  12  11

           Food found in direction (0 == no food, 1 == food in that direction)
           23  16  17
            
           22  *   18

           21  20  19
        */

        public void UpdateInput(Food food)
        {
            input.Clear();

            // Check up direction (distance_to_edge, distance_to_body, found_food)
            (input[0], input[8], input[16]) = CheckDirection(food, 0, -1);

            // Check up and right direction (distance_to_edge, distance_to_body, found_food)
            (input[1], input[9], input[17]) = CheckDirection(food, 1, -1);

            // Check right direction (distance_to_edge, distance_to_body, found_food)
            (input[2], input[10], input[18]) = CheckDirection(food, 1, 0);

            // Check down and right direction (distance_to_edge, distance_to_body, found_food)
            (input[3], input[11], input[19]) = CheckDirection(food, 1, 1);

            // Check down direction (distance_to_edge, distance_to_body, found_food)
            (input[4], input[12], input[20]) = CheckDirection(food, 0, 1);

            // Check down and left direction (distance_to_edge, distance_to_body, found_food)
            (input[5], input[13], input[21]) = CheckDirection(food, -1, 1);

            // Check left direction (distance_to_edge, distance_to_body, found_food)
            (input[6], input[14], input[22]) = CheckDirection(food, -1, 0);

            // Check up and left direction (distance_to_edge, distance_to_body, found_food)
            (input[7], input[15], input[22]) = CheckDirection(food, -1, -1);
        }

        private (double, double, double) CheckDirection(Food food, int x, int y)
        {
            var p = new Cell(Head);
            p.Add(x, y);

            var diff = Math.Abs(x) + Math.Abs(y);
            var distance = diff;

            double found_food = 0;
            double distance_to_edge = 0;
            double distance_to_body = 0;
            while (p.X >= 0 && p.X < 40 && p.Y >= 0 && p.Y < 30)
            {
                // Check for food
                if (food.Hit(p))
                    found_food = 1;

                // Check if body is hit
                if (Hit(p))
                    distance_to_body = 1.0 / distance;

                p.Add(x, y);
                distance += diff;
            }

            distance_to_edge = 1.0 / distance;

            return (distance_to_edge, distance_to_body, found_food);
        }

        public void Run()
        {
            var hidden1_output = input * input_to_hidden1_weights;
            hidden1_output.Map(d => SpecialFunctions.Logistic(d), hidden1_output);

            var hidden2_output = hidden1_output * hidden1_to_hidden2_weights;
            hidden2_output.Map(d => SpecialFunctions.Logistic(d), hidden2_output);

            output = hidden2_output * hidden2_to_output_weights;
            output.Map(d => SpecialFunctions.Logistic(d), output);
        }

        public void SetDirection()
        {
            dir = output.MaximumIndex();
        }
    }
}
