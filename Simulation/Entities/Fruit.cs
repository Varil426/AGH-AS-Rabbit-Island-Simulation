using System.Numerics;

namespace Simulation.Entities
{
    public class Fruit : Entity
    {
        public Fruit(float x, float y, World world) : base(x, y, world)
        {
        }

        public Fruit(Vector2 position, World world) : base(position, world)
        {
        }
    }
}