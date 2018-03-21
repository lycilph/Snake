using MathNet.Numerics.Distributions;
using System;

namespace LyCilph.Controllers
{
    public class Chromosome
    {
        private static Random rnd = new Random((int)DateTime.Now.Ticks);
        private static Normal normal = Normal.WithMeanStdDev(0.0, 0.1);

        public double[] input_to_hidden1_weights { get; set; }
        public double[] hidden1_to_hidden2_weights { get; set; }
        public double[] hidden2_to_output_weights { get; set; }

        public static Chromosome Cross(Chromosome parent1, Chromosome parent2)
        {
            var child = new Chromosome
            {
                input_to_hidden1_weights = parent1.input_to_hidden1_weights.Clone() as double[],
                hidden1_to_hidden2_weights = parent1.hidden1_to_hidden2_weights.Clone() as double[],
                hidden2_to_output_weights = parent1.hidden2_to_output_weights.Clone() as double[]
            };

            // Cross over for input_to_hidden1_weights
            var index = rnd.Next(child.input_to_hidden1_weights.Length); // Select random cross-over point
            var length = child.input_to_hidden1_weights.Length - index;
            Array.Copy(parent2.input_to_hidden1_weights, index, child.input_to_hidden1_weights, index, length);

            // Cross over for hidden1_to_hidden2_weights
            index = rnd.Next(child.hidden1_to_hidden2_weights.Length); // Select random cross-over point
            length = child.hidden1_to_hidden2_weights.Length - index;
            Array.Copy(parent2.hidden1_to_hidden2_weights, index, child.hidden1_to_hidden2_weights, index, length);

            // Cross over for hidden2_to_output_weights
            index = rnd.Next(child.hidden2_to_output_weights.Length); // Select random cross-over point
            length = child.hidden2_to_output_weights.Length - index;
            Array.Copy(parent2.hidden2_to_output_weights, index, child.hidden2_to_output_weights, index, length);

            return child;
        }

        public void Mutate(double mutation_rate)
        {
            for (int i = 0; i < input_to_hidden1_weights.Length; i++)
            {
                var r = rnd.NextDouble();
                if (r < mutation_rate)
                {
                    var s = normal.Sample();
                    input_to_hidden1_weights[i] += s;
                    input_to_hidden1_weights[i] = Math.Max(-1, input_to_hidden1_weights[i]);
                    input_to_hidden1_weights[i] = Math.Min(1, input_to_hidden1_weights[i]);
                }
            }
        }
    }
}
