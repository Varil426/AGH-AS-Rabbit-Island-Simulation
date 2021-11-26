using Simulation.Entities;

namespace Visualization.CsvExport;

internal class RabbitMap : CreatureMap<Rabbit>
{
    public RabbitMap()
    {
        Map(rabbit => rabbit.Fear).Name("fear");
    }
}