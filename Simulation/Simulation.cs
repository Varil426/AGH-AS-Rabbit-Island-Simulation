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
        // TODO Move scaled values to simulation config
        Rabbit.RaceValues.RefreshValues();
        Wolf.RaceValues.RefreshValues();
        CreateInitiazlCreatures();

        World.StartSimulation();

        // TODO Return results
        throw new NotImplementedException();
    }

    private void CreateInitiazlCreatures()
    {
        // Create Rabbits
        for (int i = 0; i < World.WorldConfig.RabbitConfig.InitialPopulation; i++)
        {
            World.AddCreatureWithoutStartingAThread(new Rabbit(StaticRandom.GenerateRandomPosition()));
        }
        // Create Wolves
        for (int i = 0; i < World.WorldConfig.WolvesConfig.InitialPopulation; i++)
        {
            World.AddCreatureWithoutStartingAThread(new Wolf(StaticRandom.GenerateRandomPosition()));
        }
    }

    internal World World { get; init; }
}
