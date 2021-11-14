using Simulation.Entities;
using System.Numerics;

namespace Simulation
{
    public class World
    {
        private readonly List<Creature> _legacyCreatures;

        private readonly List<Entity> _entities;

        private Map _worldMap;

        private readonly Director _director;

        private readonly Config _worldConfig;

        public Config WorldConfig
        {
            get => _worldConfig;
            
            init
            {
                _worldConfig = value;
                GenerateOffspring = value.SelectedOffspringGenerationMethod ?? OffspringGeneration.BasicOffspringGeneration;
            }
        }

        public World()
        {
            _entities = new List<Entity>();
            _legacyCreatures = new List<Creature>();
            _worldMap = new Map((1000, 1000));
            _director = new(this);
            _worldConfig = new Config();
            GenerateOffspring = OffspringGeneration.BasicOffspringGeneration;
        }

        /// <summary>
        /// Adds entity to the world.
        /// </summary>
        /// <param name="entity">Entity to be added to the world.</param>
        public void AddEntity(Entity entity)
        {
            lock (_entities)
            {
                _entities.Add(entity);
                AddedEntity?.Invoke(entity);
            }
        }

        /// <summary>
        /// Adds and starts thread for creature.
        /// </summary>
        /// <param name="creature">Creature to be added to the world.</param>
        public void AddCreature(Creature creature)
        {
            lock (_entities)
            {
                var th = new Thread(creature.Act)
                {
                    IsBackground = true
                };
                if (_entities.OfType<Creature>().Count() < WorldConfig.MaxCreatures)
                {
                    AddEntity(creature);
                    _legacyCreatures.Add(creature);
                    creature.CreatureThread = th;
                    th.Start();
                }
            }
        }

        /// <summary>
        /// Adds creature to the world.
        /// </summary>
        /// <param name="creature">Creature to be added to the world.</param>
        public void AddCreatureWithoutStartingAThread(Creature creature)
        {
            lock (_entities)
            {
                AddEntity(creature);
                _legacyCreatures.Add(creature);
            }
        }

        /// <summary>
        /// Removes entity from the world.
        /// </summary>
        /// <param name="entity">Entity to be removed.</param>
        /// <returns>Returs if operation was successful.</returns>
        public bool RemoveEntity(Entity entity)
        {
            lock (_entities)
            {
                if (_entities.Remove(entity))
                {
                    RemovedEntity?.Invoke(entity);
                    return true;
                }
                return false;
            }
        }

        public void StartSimulation()
        {
            lock (_entities)
            {
                _director.Start();

                var threads = new List<Thread>();
                foreach (ICreature creature in _entities.Where(x => x is ICreature))
                {
                    var th = new Thread(creature.Act)
                    {
                        IsBackground = true
                    };
                    threads.Add(th);
                    creature.CreatureThread = th;
                }

                SetStartTime();
                threads.ForEach(x => x.Start());
            }
        }

        public List<Entity> GetAllEntities()
        {
            lock (_entities)
            {
                return new List<Entity>(_entities);
            }
        }

        public List<Entity> GetCloseByEntities(Entity entity)
        {
            lock (_entities)
            {
                return _entities.Where(x =>
                {
                    var distance = Vector2.Distance(entity.Position, x.Position);
                    return distance <= 10 && x != entity;
                }).ToList();
            }
        }

        /// <summary>
        /// Returns all creatures that have ever exsisted in simulation.
        /// </summary>
        /// <returns>List of creatures that have ever exsisted in simulation.</returns>
        public List<Creature> GetLegacyCreatures()
        {
            lock (_entities)
            {
                return new List<Creature>(_legacyCreatures);
            }
        }

        public List<Entity> GetCloseByEntities(Creature creature)
        {
            lock (_entities)
            {
                return _entities.Where(x =>
                {
                    var distance = Vector2.Distance(creature.Position, x.Position);
                    return distance <= creature.SightRange && x != creature;
                }).ToList();
            }
        }

        public void Reset()
        {
            _director.Stop();
            lock (_entities)
            {
                _entities.ForEach(x =>
                {
                    if (x is Creature creature)
                    {
                        creature.StopThread();
                    }
                });
                _entities.Clear();
                _legacyCreatures.Clear();
            }
        }

        private void SetStartTime()
        {
            StartTime = DateTime.Now;
        }

        public Map WorldMap
        {
            get => _worldMap;
            set => _worldMap = value;
        }

        public DateTime StartTime { get; private set; }

        public delegate List<Creature> GenerateOffspringMethod(Creature mother, Creature father);

        public GenerateOffspringMethod GenerateOffspring;

        internal event Action<Entity>? AddedEntity;

        internal event Action<Entity>? RemovedEntity;
    }
}