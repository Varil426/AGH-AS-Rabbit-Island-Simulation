using Simulation.Entities;
using System.Windows.Media;

namespace Visualization.EntitiesVisualization;

internal class RabbitVisualization : CreatureVisualization<Rabbit>
{
    public RabbitVisualization(Rabbit rabbit) : base(rabbit)
    {
    }

    public override SolidColorBrush Color => VisualizedEntity.IsAlive ? Brushes.Blue : Brushes.Purple;
}
