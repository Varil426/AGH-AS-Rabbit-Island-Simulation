using Simulation.Entities;
using System.Threading;

namespace Simulation
{
    /// <summary>
    /// Object responsible for managing of not-alive entities.
    /// </summary>
    internal class Director
    {
        private readonly World _world;

        public Director(World world)
        {
            _world = world;
        }

        private Thread? _directorThread;
        private bool _shouldRun;

        public void Stop()
        {
            _shouldRun = false;
            _directorThread?.Interrupt();
        }

        public Thread Start()
        {
            _directorThread = new Thread(Run)
            {
                Name = "Director Thread",
                IsBackground = true
            };
            _directorThread.Start();
            return _directorThread;
        }

        private void Run()
        {
            lock (this)
            {
                int dayDuration = (int)((60 * 60 * 24 * 1000) / _world.WorldConfig.TimeRate);
                int expirencyTime = dayDuration * 2;
                int fruitsPerDay = _world.WorldConfig.FruitsPerDay;
                bool foodExpires = _world.WorldConfig.FoodExpires;
                _shouldRun = true;
                try
                {
                    while (_shouldRun && _world.GetAllEntities().OfType<ICreature>().Any(creature => creature.IsAlive))
                    {
                        if (foodExpires)
                        {
                            // Remove old fruits
                            _world.GetAllEntities().FindAll(entity => entity is Fruit).ForEach(fruit =>
                            {
                                if (fruit.CreatedAt.AddMilliseconds(expirencyTime) <= DateTime.Now)
                                {
                                    _world.RemoveEntity(fruit);
                                }
                            });
                            // Remove dead creatures
                            _world.GetAllEntities().FindAll(entity => entity is Creature).ForEach(entity =>
                            {
                                if (entity is Creature creature && !creature.IsAlive && creature.DeathAt.AddMilliseconds(expirencyTime) <= DateTime.Now)
                                {
                                    _world.RemoveEntity(creature);
                                }
                            });
                        }
                        // Add new fruits
                        for (int i = 0; i < fruitsPerDay; i++)
                        {
                            var newFruit = new Fruit(StaticRandom.GenerateRandomPosition(_world), _world);
                            _world.AddEntity(newFruit);
                        }
                        Thread.Sleep(dayDuration);
                    }
                }
                catch (ThreadInterruptedException)
                {
                }
            }
        }
    }
}