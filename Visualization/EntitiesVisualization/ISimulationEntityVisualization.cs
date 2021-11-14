using Simulation.Entities;

namespace Visualization.EntitiesVisualization;

internal interface ISimulationEntityVisualization : IWPFDrawable
{
    Entity Entity { get; } 
}
