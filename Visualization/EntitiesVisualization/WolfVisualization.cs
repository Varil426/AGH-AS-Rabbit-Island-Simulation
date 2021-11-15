using Simulation.Entities;
using System.Windows.Media;

namespace Visualization.EntitiesVisualization;

internal class WolfVisualization : CreatureVisualization<Wolf>
{
    public WolfVisualization(Wolf wolf) : base(wolf)
    {
    }

    public override SolidColorBrush Color => VisualizedEntity.IsAlive ? Brushes.Red : Brushes.Purple;
}
