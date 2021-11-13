using System;
using System.Numerics;
using System.Threading;

namespace Simulation
{
    /// <summary>
    /// Handles all things related to random generation.
    /// </summary>
    internal static class StaticRandom
    {
        private static readonly ThreadLocal<Random> _random = new(() => new Random());

        public static Random Generator => _random.Value!;

        public static Vector2 GenerateRandomPosition(World world)
        {
            float x = Generator.Next(world.WorldMap.Size.Item1);
            float y = Generator.Next(world.WorldMap.Size.Item2);
            return new Vector2(x, y);
        }
    }
}