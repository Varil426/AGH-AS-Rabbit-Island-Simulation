using Simulation;
using SimulationStandard.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Visualization
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        public ResultsWindow(ISimulationResults simulationResults)
        {
            InitializeComponent();

            TextBlockFruitsTotal.Text += simulationResults.Results[SimulationBuilder.SimulationResultsEnum.TotalFruits.ToString()].ToString();
            TextBlockRabbitsTotal.Text += simulationResults.Results[SimulationBuilder.SimulationResultsEnum.TotalRabbits.ToString()].ToString();
            TextBlockWolvesTotal.Text += simulationResults.Results[SimulationBuilder.SimulationResultsEnum.TotalWolves.ToString()].ToString();
            TextBlockRabbitsGeneration.Text += simulationResults.Results[SimulationBuilder.SimulationResultsEnum.RabbitsGenerations.ToString()].ToString();
            TextBlockWolvesGeneration.Text += simulationResults.Results[SimulationBuilder.SimulationResultsEnum.WolvesGenerations.ToString()].ToString();
            var time = (simulationResults.Results[SimulationBuilder.SimulationResultsEnum.SimulationTime.ToString()] as List<long>)?.Last();
            if (time != null)
                TextBlockSimulationTime.Text = GenerateTimeString(time.Value);
        }

        private static string GenerateTimeString(long simulationTimeMinutes)
        {
            var minutes = (simulationTimeMinutes % 60);
            var hours = (simulationTimeMinutes % (60 * 24) / 60);
            var days = (simulationTimeMinutes / (60 * 24));
            return $"Simulation Time: Days: {days}, Hours: {hours}, Minutes: {minutes}";
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e) => Close();
    }
}
