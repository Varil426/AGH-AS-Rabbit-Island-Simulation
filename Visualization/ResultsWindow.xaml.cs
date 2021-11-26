using Simulation;
using Simulation.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Visualization
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        public ResultsWindow(SimulationResults simulationResults)
        {
            InitializeComponent();

            TextBlockFruitsTotal.Text += simulationResults.Results[SimulationBuilder.SimulationResultsEnum.TotalFruits.ToString()].ToString();
            TextBlockRabbitsTotal.Text += simulationResults.Results[SimulationBuilder.SimulationResultsEnum.TotalRabbits.ToString()].ToString();
            TextBlockWolvesTotal.Text += simulationResults.Results[SimulationBuilder.SimulationResultsEnum.TotalWolves.ToString()].ToString();
            TextBlockRabbitsGeneration.Text += simulationResults.Results[SimulationBuilder.SimulationResultsEnum.RabbitsGenerations.ToString()].ToString();
            TextBlockWolvesGeneration.Text += simulationResults.Results[SimulationBuilder.SimulationResultsEnum.WolvesGenerations.ToString()].ToString();
            var time = (simulationResults.Results[SimulationBuilder.SimulationResultsEnum.SimulationTime.ToString()] as List<long>)?.Last();
            if (time != null)
                TextBlockSimulationTime.Text = GenerateTimeString(time.Value / 60);

            var rabbitsStatistics = CreateCreaturesStatistics(simulationResults.Rabbits.ToList());
            var wolvesStatistics = CreateCreaturesStatistics(simulationResults.Wolves.ToList());

            PopulateStatisticContainer(StackPanelRabbitsStatistics, rabbitsStatistics);
            PopulateStatisticContainer(StackPanelWolvesStatistics, wolvesStatistics);
        }

        private static string GenerateTimeString(long simulationTimeMinutes)
        {
            var minutes = (simulationTimeMinutes % 60);
            var hours = (simulationTimeMinutes % (60 * 24) / 60);
            var days = (simulationTimeMinutes / (60 * 24));
            return $"Simulation Time: Days: {days}, Hours: {hours}, Minutes: {minutes}";
        }

        private static Dictionary<string, object> CreateCreaturesStatistics<TCreature>(ICollection<TCreature> creatures) where TCreature : Creature
        {
            // TODO Move to ResultsBuilder. Add method in Director on stop to generate those statistics
            var result = new Dictionary<string, object>();

            result["Average lifespan"] = GenerateTimeString((long)creatures.Where(creature => creature.DeathAt != default(System.DateTime))
                .Select(creature => (creature.DeathAt - creature.CreatedAt).TotalMilliseconds * creature.World.WorldConfig.TimeRate / (1000 * 60)).DefaultIfEmpty().Average());
            result["Average sight range"] = creatures.Select(creature => creature.SightRange).DefaultIfEmpty().Average();
            result["Average interaction range"] = creatures.Select(creature => creature.InteractionRange).DefaultIfEmpty().Average();
            result["Average health"] = creatures.Select(creature => creature.Health).DefaultIfEmpty().Average();
            result["Average movement speed"] = creatures.Select(creature => creature.MovementSpeed).DefaultIfEmpty().Average();
            result["Male count"] = creatures.Count(creature => creature.Gender == Creature.GenderType.Male);
            result["Female count"] = creatures.Count(creature => creature.Gender == Creature.GenderType.Female);
            
            if (typeof(TCreature) == typeof(Rabbit))
            {
                var rabbits = creatures.OfType<Rabbit>();
                result["Average fear"] = rabbits.Select(rabbit => rabbit.Fear).DefaultIfEmpty().Average();
            }
            else if (typeof(TCreature) == typeof(Wolf))
            {
                var wolves = creatures.OfType<Wolf>();
                result["Average attack"] = wolves.Select(wolf => wolf.Attack).DefaultIfEmpty().Average();
            }

            return result;
        }

        private static void PopulateStatisticContainer(IAddChild container, Dictionary<string, object> values)
        {
            foreach (var kvp in values)
            {
                if (double.TryParse(kvp.Value.ToString(), out _))
                    container.AddChild(new TextBlock { Text = $"{kvp.Key}: {kvp.Value:0.####}"});
                else
                    container.AddChild(new TextBlock { Text = $"{kvp.Key}: {kvp.Value}" });
            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e) => Close();
    }
}
