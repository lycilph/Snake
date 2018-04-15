using OxyPlot;
using OxyPlot.Series;
using System;
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
        public static readonly DependencyProperty PopulationSizeProperty = DependencyProperty.Register("PopulationSize", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

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
            fitness_series.Points.Add(new DataPoint(0, 0));
            Model.Series.Add(fitness_series);

            progress = new Progress<int>(i => 
            {
                Counter += i;
                if (Counter > PopulationSize)
                    Counter = 0;

                Messages.Insert(0, $"Counter {Counter}");
                if (Messages.Count > 15)
                    Messages.RemoveAt(Messages.Count - 1);
            });

            Counter = 0;
            PopulationSize = 100;
            Generation = 1;
            Messages = new ObservableCollection<string>();
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            Running = true;
            cts = new CancellationTokenSource();

            Counter = 0;
            PopulationSize = 200;
            var population = Enumerable.Range(0, PopulationSize).Select(_ => new Individual()).ToList();

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Run(() =>
            {
                try
                {
                    var po = new ParallelOptions { CancellationToken = cts.Token };
                    Parallel.ForEach(population, po, i =>
                    {
                        i.Simulate(5);
                        progress.Report(1);
                        Thread.Sleep(100);
                    });

                }
                catch (Exception)
                {
                    Debug.WriteLine("Simulation was cancelled");
                }
            })
            .ContinueWith(a => 
            {
                fitness_series.Points.Add(new DataPoint(Generation, rnd.NextDouble() * 10.0));
                Running = false;
                Generation++;
                Model.InvalidatePlot(true);
            }, scheduler);
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            Running = false;
            cts.Cancel();
        }
    }
}
