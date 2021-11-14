using System.Numerics;

namespace Simulation.Entities
{
    public class Point : Entity
    {
        public Point(float x, float y, World world) : base(x, y, world)
        {
        }

        public Point(Vector2 destination, World world) : base(destination, world)
        {
        }
    }
}