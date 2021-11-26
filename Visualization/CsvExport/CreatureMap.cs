using CsvHelper.Configuration;
using Simulation.Entities;

namespace Visualization.CsvExport;


internal abstract class CreatureMap<TCreatureType> : ClassMap<TCreatureType> where TCreatureType : Creature
{
    public CreatureMap()
    {
        Map(creature => creature.CreatedAt).Name(nameof(Creature.CreatedAt));
        Map(creature => creature.DeathAt).Name(nameof(Creature.DeathAt));
        Map(creature => creature.CreatedAtInSimulationTime).Name(nameof(Creature.CreatedAtInSimulationTime));
        Map(creature => creature.DeathAtInSimulationTime).Name(nameof(Creature.DeathAtInSimulationTime));
        Map(creature => creature.Generation).Name(nameof(Creature.Generation));
        Map(creature => creature.MaxHealth).Name(nameof(Creature.MaxHealth));
        Map(creature => creature.MaxEnergy).Name(nameof(Creature.MaxEnergy));
        Map(creature => creature.MovementSpeed).Name(nameof(Creature.MovementSpeed));
        Map(creature => creature.SightRange).Name(nameof(Creature.SightRange));
        Map(creature => creature.InteractionRange).Name(nameof(Creature.InteractionRange));
        Map(creature => creature.Gender).Name(nameof(Creature.Gender));
    }
}