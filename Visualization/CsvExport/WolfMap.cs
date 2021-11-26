using Simulation.Entities;

namespace Visualization.CsvExport;

internal class WolfMap : CreatureMap<Wolf>
{
    public WolfMap()
    {
        Map(wolf => wolf.Attack).Name("attack");
    }
}