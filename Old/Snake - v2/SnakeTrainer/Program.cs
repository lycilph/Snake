using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace SnakeTrainer
{
    class Program
    {
        static void Main(string[] args)
        {
            var population = new Population(1000);

            population.Run();
            population.Select();


            //var sims = Enumerable.Range(0, 50)
            //                     .Select(_ => new Simulation())
            //                     .ToList();
            //sims.ForEach(s => s.Run());

            //sims.OrderBy(s => s.GetFitness())
            //    .ToList()
            //    .ForEach(s => Console.WriteLine($"Fitness: {s.GetFitness()}"));


            //for (int i = 0; i < 50; i++)
            //{
            //    var sim = new Simulation();
            //    sim.Run();

            //    Console.WriteLine($"Average score: {sim.average_score}");
            //    Console.WriteLine($"Average age: {sim.average_age}");
            //    Console.WriteLine($"Fitness: {sim.GetFitness()}");
            //}


            //var max_fitness = 0.0;
            //for (int i = 0; i < 100000; i++)
            //{
            //    var sim = new Simulation();
            //    sim.Run();

            //    var fitness = sim.GetFitness();
            //    if (fitness > max_fitness)
            //    {
            //        max_fitness = fitness;
            //        Console.WriteLine($"New fitness record {max_fitness} - in iteration {i}");
            //    }

            //    if (i % 100 == 0)
            //        Console.WriteLine($"Iteration {i}");
            //}
            //Console.WriteLine($"Max fitness found: {max_fitness}");



            //var rnd = new Random((int)DateTime.Now.Ticks);
            //var m = Matrix<double>.Build.Dense(4, 3, (x, y) => rnd.NextDouble());

            //var column_major = m.AsColumnMajorArray();
            //var str = JsonConvert.SerializeObject(column_major);

            //var storage = JsonConvert.DeserializeObject<double[]>(str);
            //var n = Matrix<double>.Build.Dense(4, 3, storage);

            //var comp = Precision.AlmostEqual(m, n, 0.001);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
