namespace Simulation.Entities
{
    /// <summary>
    /// Represents a group of entities.
    /// </summary>
    internal class EntitiesGroup : Entity
    {
        /// <summary>
        /// All entities in a group.
        /// </summary>
        public List<Entity> Entities { get; }

        public EntitiesGroup(List<Entity> entities, World world) : base(0, 0, world)
        {
            Entities = entities;
        }
    }
}