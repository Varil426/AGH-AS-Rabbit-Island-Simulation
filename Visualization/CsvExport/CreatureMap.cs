using CsvHelper.Configuration;
using Simulation.Entities;

namespace Visualization.CsvExport;


internal abstract class CreatureMap<TCreatureType> : ClassMap<TCreatureType> where TCreatureType : Creature
{
    public CreatureMap()
    {
        Map(creature => creature.CreatedAt).Name("createdAt");
        Map(creature => creature.DeathAt).Name("deathAt");
        Map(creature => creature.Generation).Name("generation");
        Map(creature => creature.MaxHealth).Name("maxHealth");
        Map(creature => creature.MaxEnergy).Name("maxEnergy");
        Map(creature => creature.MovementSpeed).Name("movementSpeed");
        Map(creature => creature.SightRange).Name("sightRange");
        Map(creature => creature.InteractionRange).Name("interactionRange");
        Map(creature => creature.Gender).Name("gender");
    }
}