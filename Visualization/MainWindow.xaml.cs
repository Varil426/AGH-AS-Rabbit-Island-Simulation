using CsvHelper;
using Simulation;
using SimulationStandard;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Visualization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Simulation.Simulation? _simulation;

        private SimulationWindow? _simulationWindow;

        private GraphsWindow? _graphsWindow;

        // TODO Add check for invalid values (or too big/small)

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumericTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void FloatTextBox(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                Regex regex = new Regex(@"^([0-9]+\.[0-9]*|[0-9]+)$");
                e.Handled = !regex.IsMatch(textBox.Text + e.Text);
            }
        }

        private void TimeRateChange(object sender, TextChangedEventArgs args)
        {
            Regex regex = new Regex("^0+$");
            if (TimeRateInput.Text.Length == 0 || regex.IsMatch(TimeRateInput.Text))
            {
                TimeRateLabel.Content = "Invalid Value!";
            }
            else
            {
                var timeRate = Double.Parse(TimeRateInput.Text);
                var newText = $"Time Rate ({3600 / timeRate} seconds = 1 real-time hour)";
                TimeRateLabel.Content = newText;
            }
        }

        private void StartSimulation(object sender, RoutedEventArgs e)
        {
            ConfigGrid.ColumnDefinitions[0].IsEnabled = false;
            ConfigGrid.ColumnDefinitions[1].IsEnabled = false;
            StartStopButton.Content = "Stop";
            StartStopButton.Click -= StartSimulation;
            StartStopButton.Click += StopSimulation;

            var simulationBuilder = new SimulationBuilder();
            var simulationParams = CreateConfigFromUserInput();
            _simulation = (Simulation.Simulation)simulationBuilder.CreateSimulation(simulationParams);

            _simulationWindow = new SimulationWindow(_simulation);
            _graphsWindow = new GraphsWindow(_simulation);
            _graphsWindow.Show();
            _simulationWindow.Show();

            new Thread(() => _simulation.Run()).Start();
        }

        private SimulationParams CreateConfigFromUserInput()
        {
            var rabbitsInitialPopulation = int.Parse(RabbitsInitialPopulationInput.Text);
            var rabbitsMinChildren = int.Parse(RabbitsMinChildrenInput.Text);
            var rabbitsMaxChildren = int.Parse(RabbitsMaxChildrenInput.Text);
            var rabbitsPregnancyDuration = int.Parse(RabbitsPregnancyDurationInput.Text);
            var rabbitsLifeExpectancy = int.Parse(RabbitsLifeExpectancy.Text);

            var wolvesInitialPopulation = int.Parse(WolvesInitialPopulationInput.Text);
            var wolvesMinChildren = int.Parse(WolvesMinChildrenInput.Text);
            var wolvesMaxChildren = int.Parse(WolvesMaxChildrenInput.Text);
            var wolvesPregnancyDuration = int.Parse(WolvesPregnancyDurationInput.Text);
            var wolvesLifeExpectancy = int.Parse(WolvesLifeExpectancy.Text);

            var timeRate = double.Parse(TimeRateInput.Text);
            var deathFromOldAge = (bool)DeathFromOldAgeInput.IsChecked!;
            var maxCreatures = int.Parse(MaxCreaturesInput.Text);
            var fruitsPerDay = int.Parse(FruitsPerDayInput.Text);
            var mapSize = int.Parse(MapSizeInput.Text);
            VisualizationConfig.Instance.DrawRanges = (bool)DrawRangesInput.IsChecked!;
            // TODO Fix
            var exportResultsToCSV = (bool)ExportResultsToCSVInput.IsChecked!;
            var mutationChance = double.Parse(MutationChanceInput.Text, CultureInfo.InvariantCulture);
            var mutationImpact = double.Parse(MutationImpactInput.Text, CultureInfo.InvariantCulture);

            var simulationParams = new SimulationParams();

            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.RabbitsInitialPopulation.ToString()] = rabbitsInitialPopulation;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.RabbitsMinChildren.ToString()] = rabbitsMinChildren;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.RabbitsMaxChildren.ToString()] = rabbitsMaxChildren;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.RabbitsPregnancyDuration.ToString()] = rabbitsPregnancyDuration;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.RabbitsLifeExpectancy.ToString()] = rabbitsLifeExpectancy;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.WolvesInitialPopulation.ToString()] = wolvesInitialPopulation;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.WolvesMinChildren.ToString()] = wolvesMinChildren;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.WolvesMaxChildren.ToString()] = wolvesMaxChildren;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.WolvesPregnancyDuration.ToString()] = wolvesPregnancyDuration;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.WolvesLifeExpectancy.ToString()] = wolvesLifeExpectancy;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.TimeRate.ToString()] = timeRate;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.DeathFromOldAge.ToString()] = deathFromOldAge;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.MaxCreatures.ToString()] = maxCreatures;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.FruitsPerDay.ToString()] = fruitsPerDay;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.MapSize.ToString()] = mapSize;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.MutationChance.ToString()] = mutationChance;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.MutationImpact.ToString()] = mutationImpact;
            simulationParams.Params[SimulationBuilder.SimulationParamsEnum.OffspringGenerationMethod.ToString()] = OffspringGenerationMethodInput.SelectedIndex;

            return simulationParams;
        }

        private void ExportResultsToCSV()
        {
            // TODO
            /*var pathToFileFormat = $"results{Path.DirectorySeparatorChar}{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}--{OffspringGenerationMethodInput.Text}--{{0}}.csv";

            var pathToRabbitsFile = string.Format(pathToFileFormat, "Rabbits");
            var rabbitsFileInfo = new FileInfo(pathToRabbitsFile);
            if (rabbitsFileInfo.Directory != null && !rabbitsFileInfo.Directory.Exists)
            {
                Directory.CreateDirectory(rabbitsFileInfo.DirectoryName!);
            }
            using var rabbitsWriter = new StreamWriter(pathToRabbitsFile);
            using var rabbitsCSV = new CsvWriter(rabbitsWriter, CultureInfo.InvariantCulture);
            rabbitsCSV.Context.RegisterClassMap<Creature.CreatureMap<Creature>>();
            rabbitsCSV.Context.RegisterClassMap<Rabbit.RabbitMap>();
            rabbitsCSV.WriteHeader<Rabbit>();
            rabbitsCSV.NextRecord();
            world.GetLegacyCreatures().OfType<Rabbit>().ToList().ForEach(rabbit =>
            {
                rabbitsCSV.WriteRecord(rabbit);
                rabbitsCSV.NextRecord();
            });

            var pathToWolvesFile = string.Format(pathToFileFormat, "Wolves");
            var wolvesFileInfo = new FileInfo(pathToWolvesFile);
            if (wolvesFileInfo.Directory != null && !wolvesFileInfo.Directory.Exists)
            {
                Directory.CreateDirectory(wolvesFileInfo.DirectoryName!);
            }
            using var wolvesWriter = new StreamWriter(pathToWolvesFile);
            using var wolvesCSV = new CsvWriter(wolvesWriter, CultureInfo.InvariantCulture);
            wolvesCSV.Context.RegisterClassMap<Creature.CreatureMap<Creature>>();
            wolvesCSV.Context.RegisterClassMap<Wolf.WolfMap>();
            wolvesCSV.WriteHeader<Wolf>();
            wolvesCSV.NextRecord();
            world.GetLegacyCreatures().OfType<Wolf>().ToList().ForEach(wolf =>
            {
                wolvesCSV.WriteRecord(wolf);
                wolvesCSV.NextRecord();
            });*/
        }

        private void StopSimulation(object sender, RoutedEventArgs e)
        {
            ConfigGrid.ColumnDefinitions[0].IsEnabled = true;
            ConfigGrid.ColumnDefinitions[1].IsEnabled = true;
            StartStopButton.Content = "Run";
            StartStopButton.Click -= StopSimulation;
            StartStopButton.Click += StartSimulation;

            _graphsWindow?.StopAndClose();
            _simulationWindow?.StopAndClose();

            // TODO
            /*
            if (world.WorldConfig.ExportResultsToCSV)
            {
                ExportResultsToCSV();
            }

            world.Reset();
            */
        }
    }
}