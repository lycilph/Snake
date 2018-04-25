using LyCilph.Elements;
using LyCilph.Utils;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Trainer
{
    public partial class MainWindow
    {
        private Random rnd = new Random((int)DateTime.Now.Ticks);
        private LineSeries fitness_series;
        private LineSeries score_series;
        private IProgress<int> progress;
        private CancellationTokenSource cts;
        private List<Individual> population;
        private DispatcherTimer timer;
        private string filename;

        public PlotModel Model
        {
            get { return (PlotModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(PlotModel), typeof(MainWindow), new PropertyMetadata(null));

        public bool Running
        {
            get { return (bool)GetValue(RunningProperty); }
            set { SetValue(RunningProperty, value); }
        }
        public static readonly DependencyProperty RunningProperty = DependencyProperty.Register("Running", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public double TotalRunTime
        {
            get { return (double)GetValue(TotalRunTimeProperty); }
            set { SetValue(TotalRunTimeProperty, value); }
        }
        public static readonly DependencyProperty TotalRunTimeProperty = DependencyProperty.Register("TotalRunTime", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));

        public int Counter
        {
            get { return (int)GetValue(CounterProperty); }
            set { SetValue(CounterProperty, value); }
        }
        public static readonly DependencyProperty CounterProperty = DependencyProperty.Register("Counter", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int PopulationSize
        {
            get { return (int)GetValue(PopulationSizeProperty); }
            set { SetValue(PopulationSizeProperty, value); }
        }
        public static readonly DependencyProperty PopulationSizeProperty = DependencyProperty.Register("PopulationSize", typeof(int), typeof(MainWindow), new PropertyMetadata(1000));

        public int SimulationRuns
        {
            get { return (int)GetValue(SimulationRunsProperty); }
            set { SetValue(SimulationRunsProperty, value); }
        }
        public static readonly DependencyProperty SimulationRunsProperty = DependencyProperty.Register("SimulationRuns", typeof(int), typeof(MainWindow), new PropertyMetadata(5));

        public int PercentToKeep
        {
            get { return (int)GetValue(PercentToKeepProperty); }
            set { SetValue(PercentToKeepProperty, value); }
        }
        public static readonly DependencyProperty PercentToKeepProperty = DependencyProperty.Register("PercentToKeep", typeof(int), typeof(MainWindow), new PropertyMetadata(20));

        public double MutationRate
        {
            get { return (double)GetValue(MutationRateProperty); }
            set { SetValue(MutationRateProperty, value); }
        }
        public static readonly DependencyProperty MutationRateProperty = DependencyProperty.Register("MutationRate", typeof(double), typeof(MainWindow), new PropertyMetadata(0.01));

        public int Generation
        {
            get { return (int)GetValue(GenerationProperty); }
            set { SetValue(GenerationProperty, value); }
        }
        public static readonly DependencyProperty GenerationProperty = DependencyProperty.Register("Generation", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public ObservableCollection<string> Messages
        {
            get { return (ObservableCollection<string>)GetValue(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }
        public static readonly DependencyProperty MessagesProperty = DependencyProperty.Register("Messages", typeof(ObservableCollection<string>), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Model = new PlotModel { Title = "Statistics" };
            fitness_series = new LineSeries { Title = "Fitness" };
            score_series = new LineSeries { Title = "Score" };
            Model.Series.Add(fitness_series);
            Model.Series.Add(score_series);

            progress = new Progress<int>(i => 
            {
                Counter += i;
                if (Counter > PopulationSize)
                    Counter = 0;
            });

            Messages = new ObservableCollection<string>();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (s, e) => TotalRunTime += 0.1;

            Reset();
        }

        private void AddMessage(string msg)
        {
            Messages.Add(msg);
            if (Messages.Count > 15)
                Messages.RemoveAt(0);
        }

        private void Reset()
        {
            Messages.Clear();
            Counter = 0;
            Generation = 1;
            TotalRunTime = 0.0;

            fitness_series.Points.Clear();
            fitness_series.Points.Add(new DataPoint(0, 0));
            score_series.Points.Clear();
            score_series.Points.Add(new DataPoint(0, 0));
            Model.InvalidatePlot(true);

            population = Enumerable.Range(0, PopulationSize).Select(_ => new Individual()).ToList();
        }

        private void CreateClick(object sender, RoutedEventArgs e)
        {
            var create_dialog = new CreateDialog
            {
                Owner = this,
                PopulationSize = PopulationSize,
                SimulationRuns = SimulationRuns,
                PercentToKeep = PercentToKeep,
                MutationRate = MutationRate
            };

            var result = create_dialog.ShowDialog();
            if (result == true)
            {
                PopulationSize = create_dialog.PopulationSize;
                SimulationRuns = create_dialog.SimulationRuns;
                PercentToKeep = create_dialog.PercentToKeep;
                MutationRate = create_dialog.MutationRate;
                filename = string.Empty;
                Reset();
            }
        }

        private void LoadClick(object sender, RoutedEventArgs e)
        {
            var sfd = new OpenFileDialog
            {
                DefaultExt = ".pop",
                Filter = "Population file (.pop)|*.pop",
                InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            };

            if (sfd.ShowDialog() == true)
            {
                filename = sfd.FileName;
            }

            if (string.IsNullOrWhiteSpace(filename))
                return;

            var sim = JsonUtils.ReadFromFile<SerializedSimulation>(filename);
            Reset();

            SimulationRuns = sim.SimulationRuns;
            PercentToKeep = sim.PercentToKeep;
            MutationRate = sim.MutationRate;
            Generation = sim.Generation;
            TotalRunTime = sim.TotalRuntime;
            
            for (int i = 1; i < sim.FitnessStatistics.Count; i++)
                fitness_series.Points.Add(new DataPoint(i, sim.FitnessStatistics[i]));
            for (int i = 1; i < sim.ScoreStatistics.Count; i++)
                score_series.Points.Add(new DataPoint(i, sim.ScoreStatistics[i]));
            Model.InvalidatePlot(true);

            population = sim.Chromosomes.Select(chromosome => new Individual { Chromosome = chromosome }).ToList();
            PopulationSize = population.Count;

            Messages.Add("Loaded " + filename);
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                var sfd = new SaveFileDialog
                {
                    DefaultExt = ".pop",
                    Filter = "Population file (.pop)|*.pop",
                    InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            };

                if (sfd.ShowDialog() == true)
                {
                    filename = sfd.FileName;
                }
            }

            if (string.IsNullOrWhiteSpace(filename))
                return;

            var sim = new SerializedSimulation
            {
                SimulationRuns = SimulationRuns,
                PercentToKeep = PercentToKeep,
                MutationRate = MutationRate,
                Generation = Generation,
                TotalRuntime = TotalRunTime,
                FitnessStatistics = fitness_series.Points.Select(p => p.Y).ToList(),
                ScoreStatistics = score_series.Points.Select(p => p.Y).ToList(),
                Chromosomes = population.Select(p => p.Chromosome).ToList()
            };
            JsonUtils.WriteToFile(filename, sim);

            Messages.Add("Saved " + filename);
        }

        private void SaveBestClick(object sender, RoutedEventArgs e)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrWhiteSpace(filename))
            {
                dir = Path.GetDirectoryName(filename);
            }

            var sfd = new SaveFileDialog
            {
                FileName = Path.GetFileNameWithoutExtension(filename),
                DefaultExt = ".snake",
                Filter = "Snake file (.snake)|*.snake",
                InitialDirectory = dir
            };

            if (sfd.ShowDialog() == true)
            {
                var best = population.OrderByDescending(i => i.fitness).First();
                JsonUtils.WriteToFile(sfd.FileName, best.Chromosome);

                Messages.Add("Saved " + sfd.FileName);
            }
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            Counter = 0;
            Running = true;
            cts = new CancellationTokenSource();
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var simulation_runs = SimulationRuns;
            var percent_to_keep = PercentToKeep;
            var mutation_rate = MutationRate;

            // This starts the total run time
            timer.Start();

            Task.Run(() => 
            {
                try
                {
                    var sw = new Stopwatch();

                    while (true)
                    {
                        // Check if simulation was cancelled
                        cts.Token.ThrowIfCancellationRequested();

                        sw.Restart();
                        Task.Factory.StartNew(() =>
                        {
                            AddMessage($"Starting generation {Generation}");
                        }, CancellationToken.None, TaskCreationOptions.None, scheduler);

                        // Simulate all individuals in a population
                        var po = new ParallelOptions { CancellationToken = cts.Token };
                        Parallel.ForEach(population, po, i =>
                        {
                            i.Simulate(simulation_runs);
                            progress.Report(1);
                        });

                        // Order population by fitness
                        var ordered_population = population.OrderByDescending(i => i.fitness).ToList();
                        var best = ordered_population.First();

                        // Setup new population
                        var new_generation = Enumerable.Range(0, population.Count).Select(_ => new Individual()).ToList();

                        // Keep the best
                        var amount_to_keep = (int)Math.Round(ordered_population.Count * (percent_to_keep / 100.0));
                        for (int i = 0; i < amount_to_keep; i++)
                        {
                            new_generation[i] = ordered_population[i];
                        }

                        // Breed the rest
                        var total_fitness = ordered_population.Sum(s => s.fitness);
                        for (int i = amount_to_keep; i < population.Count; i++)
                        {
                            var parent1_chromosome = Select(ordered_population, total_fitness);
                            var parent2_chromosome = Select(ordered_population, total_fitness);
                            var child_chromosome = Chromosome.Cross(parent1_chromosome, parent2_chromosome);
                            child_chromosome.Mutate(mutation_rate);

                            new_generation[i].Chromosome = child_chromosome;

                            // Check if simulation was cancelled
                            cts.Token.ThrowIfCancellationRequested();
                        }

                        sw.Stop();
                        var elapsed = sw.ElapsedMilliseconds;
                        Task.Factory.StartNew(() =>
                        {
                            AddMessage($" * Simulation took {elapsed} ms");
                            AddMessage($" * Maximum fitness {best.fitness:N2}, Age {best.average_age}, Score {best.average_score}");

                            fitness_series.Points.Add(new DataPoint(Generation, ordered_population.First().fitness));
                            score_series.Points.Add(new DataPoint(Generation, ordered_population.First().average_score));
                            Model.InvalidatePlot(true);

                            Generation++;
                            Counter = 0;

                            // Update the population
                            population = new_generation;
                        }, CancellationToken.None, TaskCreationOptions.None, scheduler)
                        .Wait();
                    };
                }
                catch (OperationCanceledException)
                {
                    Task.Factory.StartNew(() =>
                    {
                        AddMessage("Simulation stopped");
                    }, CancellationToken.None, TaskCreationOptions.None, scheduler);
                }
            }, cts.Token);
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            Running = false;
            cts.Cancel();
            timer.Stop(); // This stops the total run time
        }

        private Chromosome Select(List<Individual> population, double total_fitness)
        {
            var s = rnd.NextDouble() * total_fitness;
            var running_sum = 0.0;
            foreach (var specimen in population)
            {
                running_sum += specimen.fitness;
                if (running_sum > s)
                    return specimen.Chromosome;
            }
            return null;
        }
    }
}
