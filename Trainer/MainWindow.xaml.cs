using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Trainer
{
    public partial class MainWindow
    {
        private Random rnd = new Random((int)DateTime.Now.Ticks);
        private LineSeries fitness_series;
        private IProgress<int> progress;
        private CancellationTokenSource cts;
        private List<Individual> population;

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
            Model.Series.Add(fitness_series);

            progress = new Progress<int>(i => 
            {
                Counter += i;
                if (Counter > PopulationSize)
                    Counter = 0;
            });

            Messages = new ObservableCollection<string>();

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

            fitness_series.Points.Clear();
            fitness_series.Points.Add(new DataPoint(0, 0));
            Model.InvalidatePlot(true);

            population = Enumerable.Range(0, PopulationSize).Select(_ => new Individual()).ToList();
        }

        private void CreateClick(object sender, RoutedEventArgs e)
        {
            var create_dialog = new CreateDialog
            {
                Owner = this,
                PopulationSize = PopulationSize,
                SimulationRuns = SimulationRuns
            };

            var result = create_dialog.ShowDialog();
            if (result == true)
            {
                PopulationSize = create_dialog.PopulationSize;
                SimulationRuns = create_dialog.SimulationRuns;
                Reset();
            }
        }

        private void LoadClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this, "Not implemented yet", "Information");
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            Running = true;
            cts = new CancellationTokenSource();
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var simulation_runs = SimulationRuns;

            use cts.Token.ThrowIfCancellationRequested

            Task.Run(()=> 
            {
                var sw = new Stopwatch();

                while (!cts.IsCancellationRequested)
                {
                    sw.Restart();

                    Task.Factory.StartNew(() =>
                    {
                        AddMessage($"Starting generation {Generation}");
                    }, CancellationToken.None, TaskCreationOptions.None, scheduler);

                    // Simulate all individuals in a population
                    try
                    {
                        var po = new ParallelOptions { CancellationToken = cts.Token };
                        Parallel.ForEach(population, po, i =>
                        {
                            i.Simulate(simulation_runs);
                            progress.Report(1);
                        });

                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Simulation was cancelled");
                    }

                    // Order population by fitness
                    if (!cts.IsCancellationRequested)
                    {
                        var ordered_population = population.OrderByDescending(i => i.fitness).ToList();

                        sw.Stop();
                        var elapsed = sw.ElapsedMilliseconds;
                        Task.Factory.StartNew(() =>
                        {
                            AddMessage($" * Simulation took {elapsed} ms");
                            AddMessage($" * Maximum fitness {ordered_population.First().fitness}");

                            fitness_series.Points.Add(new DataPoint(Generation, ordered_population.First().fitness));
                            Model.InvalidatePlot(true);

                            Generation++;
                            Counter = 0;
                        }, CancellationToken.None, TaskCreationOptions.None, scheduler)
                        .Wait();
                    }
                    else
                    {
                        Task.Factory.StartNew(() =>
                        {
                            AddMessage("Simulation stopped");
                        }, CancellationToken.None, TaskCreationOptions.None, scheduler);
                    }
                }
            });
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            Running = false;
            cts.Cancel();
        }
    }
}
