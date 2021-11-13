using Simulation.Entities;
using SimulationStandard.Interfaces;

namespace Simulation;

public sealed class Simulation : ISimulation
{

    public Simulation()
    {
        World = new();
    }

    public ISimulationParams Params { get => throw new System.NotImplementedException(); init => throw new System.NotImplementedException(); }

    public void Dispose()
    {
        // There is nothing to dispose.
    }

    public ISimulationResults Run()
    {
        World.WorldConfig.RabbitConfig.RefreshValues(World);
        World.WorldConfig.WolvesConfig.RefreshValues(World);
        CreateInitialCreatures();

        World.StartSimulation();

        // TODO Return results
        throw new NotImplementedException();
    }

    private void CreateInitialCreatures()
    {
        // Create Rabbits
        for (int i = 0; i < World.WorldConfig.RabbitConfig.InitialPopulation; i++)
        {
            World.AddCreatureWithoutStartingAThread(new Rabbit(StaticRandom.GenerateRandomPosition(World), World));
        }
        // Create Wolves
        for (int i = 0; i < World.WorldConfig.WolvesConfig.InitialPopulation; i++)
        {
            World.AddCreatureWithoutStartingAThread(new Wolf(StaticRandom.GenerateRandomPosition(World), World));
        }
    }

    internal World World { get; init; }
}
