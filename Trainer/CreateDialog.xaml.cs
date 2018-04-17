using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Trainer
{
    public partial class CreateDialog
    {
        private Regex regex = new Regex("[^0-9]+");

        public int PopulationSize
        {
            get { return (int)GetValue(PopulationSizeProperty); }
            set { SetValue(PopulationSizeProperty, value); }
        }
        public static readonly DependencyProperty PopulationSizeProperty = DependencyProperty.Register("PopulationSize", typeof(int), typeof(CreateDialog), new PropertyMetadata(200));

        public int SimulationRuns
        {
            get { return (int)GetValue(SimulationRunsProperty); }
            set { SetValue(SimulationRunsProperty, value); }
        }
        public static readonly DependencyProperty SimulationRunsProperty = DependencyProperty.Register("SimulationRuns", typeof(int), typeof(CreateDialog), new PropertyMetadata(5));

        public CreateDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ValidateNumericInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
