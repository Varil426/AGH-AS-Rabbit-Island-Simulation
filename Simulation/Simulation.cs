using Simulation.Entities;
using SimulationStandard;
using SimulationStandard.Interfaces;

namespace Simulation;

public sealed class Simulation : ISimulation
{

    public Simulation()
    {
        World = new();
        Params = new SimulationParams();
    }

    public ISimulationParams Params { get; init; }

    public void Dispose()
    {
        // There is nothing to dispose.
    }

    public ISimulationResults Run()
    {
        World.WorldConfig.RabbitConfig.RefreshValues(World);
        World.WorldConfig.WolvesConfig.RefreshValues(World);

        CreateInitialCreatures();

        var results = new SimulationResults(this);
        results.StoreInitialValues();

        var simulationThread = World.StartSimulation();

        simulationThread.Join();

        return results;
    }

    public World World { get; init; }

    public event Action<Entity> EntityAdded
    {
        add { World.AddedEntity += value; }
        remove { World.AddedEntity -= value; }
    }

    public event Action<Entity> EntityRemoved
    {
        add { World.RemovedEntity += value; }
        remove { World.RemovedEntity -= value; }
    }

    /// <summary>
    /// Stops the simulation.
    /// </summary>
    public void Stop() => World.Reset();

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
}
