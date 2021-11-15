using Simulation.Entities;

namespace Simulation;

internal class SimulationResults : SimulationStandard.SimulationResults
{
    private readonly Simulation _simulation;

    public SimulationResults(Simulation simulation)
    {
        _simulation = simulation;
        CreateResultsObjects();
        SubscribeToSimulationEvents();
    }

    private void SubscribeToSimulationEvents()
    {
        _simulation.EntityAdded -= SimulationEntityAdded;
        _simulation.EntityAdded += SimulationEntityAdded;

        _simulation.EntityRemoved -= SimulationEntityRemoved;
        _simulation.EntityRemoved += SimulationEntityRemoved;
    }

    private void SimulationEntityRemoved(Entity obj)
    {
        CreateSimulationSnapshot();
    }

    private void SimulationEntityAdded(Entities.Entity entity)
    {
        CreateSimulationSnapshot();

        switch (entity)
        {
            case Rabbit:
                TotalRabbits++;
                break;
            case Wolf:
                TotalWolves++;
                break;
        }
    }

    public void StoreInitialValues()
    {
        var allEntities = _simulation.World.GetAllEntities();

        AddTimestamp(0);
        AddRabbitsAlive(allEntities.Count(x => x is Rabbit));
        AddWolvesAlive(allEntities.Count(x => x is Wolf));
        AddFruitsPresent(allEntities.Count(x => x is Fruit));
    }

    private void CreateSimulationSnapshot()
    {
        var allEntities = _simulation.World.GetAllEntities();
        var timeStamp = _simulation.World.CurrentSimulationTime;

        AddTimestamp(timeStamp);
        AddRabbitsAlive(allEntities.Count(x => x is Rabbit));
        AddWolvesAlive(allEntities.Count(x => x is Wolf));
        AddFruitsPresent(allEntities.Count(x => x is Fruit));
    }

    private void CreateResultsObjects()
    {
        Results[SimulationBuilder.SimulationResultsEnum.SimulationTime.ToString()] = new List<long>();
        Results[SimulationBuilder.SimulationResultsEnum.RabbitsAlive.ToString()] = new List<int>();
        Results[SimulationBuilder.SimulationResultsEnum.WolvesAlive.ToString()] = new List<int>();
        Results[SimulationBuilder.SimulationResultsEnum.FruitsPresent.ToString()] = new List<int>();

        Results[SimulationBuilder.SimulationResultsEnum.TotalRabbits.ToString()] = _simulation.World.GetAllEntities().OfType<Rabbit>().Count();
        Results[SimulationBuilder.SimulationResultsEnum.TotalWolves.ToString()] = _simulation.World.GetAllEntities().OfType<Wolf>().Count();
    }

    public void AddTimestamp(long timestamp) => ((IList<long>)Results[SimulationBuilder.SimulationResultsEnum.SimulationTime.ToString()]).Add(timestamp);
    public void AddRabbitsAlive(int rabbitsAlive) => ((IList<int>)Results[SimulationBuilder.SimulationResultsEnum.RabbitsAlive.ToString()]).Add(rabbitsAlive);
    public void AddWolvesAlive(int wolvesAlive) => ((IList<int>)Results[SimulationBuilder.SimulationResultsEnum.WolvesAlive.ToString()]).Add(wolvesAlive);
    public void AddFruitsPresent(int fruitsPresent) => ((IList<int>)Results[SimulationBuilder.SimulationResultsEnum.FruitsPresent.ToString()]).Add(fruitsPresent);
    public int TotalRabbits { get => (int)Results[SimulationBuilder.SimulationResultsEnum.TotalRabbits.ToString()]; set => Results[SimulationBuilder.SimulationResultsEnum.TotalRabbits.ToString()] = value; }
    public int TotalWolves { get => (int)Results[SimulationBuilder.SimulationResultsEnum.TotalWolves.ToString()]; set => Results[SimulationBuilder.SimulationResultsEnum.TotalWolves.ToString()] = value; }
}
