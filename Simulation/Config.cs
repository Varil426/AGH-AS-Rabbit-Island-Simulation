using Simulation.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation
{
    internal class Config
    {
        // TODO Add check for invalid values (or too big/small)
        public interface ICreatureConfig<Creature> where Creature : Entities.Creature
        {
            Type CreatureType { get; set; }

            int InitialPopulation { get; set; }

            int MinChildren { get; set; }

            int MaxChildren { get; set; }

            int PregnancyDuration { get; set; }

            int LifeExpectancy { get; set; }

            int LifeExpectancyScaled { get; }

            int MatingTimeScaled { get; }

            int MoveInOneDirectionTimeScaled { get; }

            int PregnancyTimeScaled { get; }

            int WaitToMateTimeScaled { get; }

            int EatingTimeScaled { get; }

            int InitialPopulationCredits { get; }

            void RefreshValues(World world);
        }

        private class CreatureConfig<TCreature> : ICreatureConfig<TCreature> where TCreature : Entities.Creature
        {
            public CreatureConfig()
            {
                CreatureType = typeof(TCreature);
            }

            public Type CreatureType { get; set; }

            public int InitialPopulation { get; set; }

            public int MinChildren { get; set; }

            public int MaxChildren { get; set; }

            public int PregnancyDuration { get; set; }

            public int LifeExpectancy { get; set; }

            /// <summary>
            /// Scales values to match the Time Rate of the World.
            /// </summary>
            public void RefreshValues(World world)
            {
                // Time in seconds scaled to simulation time rate
                var timeScalar = 1000 / world.WorldConfig.TimeRate;
                EatingTimeScaled = (int)(120 * timeScalar);
                MatingTimeScaled = (int)(300 * timeScalar);
                WaitToMateTimeScaled = (int)(50 * timeScalar);
                PregnancyTimeScaled = (int)(3600 * 24 * (CreatureType == typeof(Rabbit) ? world.WorldConfig.RabbitConfig.PregnancyDuration : world.WorldConfig.WolvesConfig.PregnancyDuration) * timeScalar);
                MoveInOneDirectionTimeScaled = (int)(300 * timeScalar);
                LifeExpectancyScaled = (int)(3600 * 24 * (CreatureType == typeof(Rabbit) ? world.WorldConfig.RabbitConfig.LifeExpectancy : world.WorldConfig.WolvesConfig.LifeExpectancy) * timeScalar);
            }

            /// <summary>
            /// Time needed to eat a fruit. (Scaled to simulation time rate)
            /// </summary>
            public int EatingTimeScaled { get; private set; }

            /// <summary>
            /// Time neede to mate. (Scaled to simulation time rate)
            /// </summary>
            public int MatingTimeScaled { get; private set; }

            /// <summary>
            /// Wait for other rabbit to mate time. (Scaled to simulation time rate)
            /// </summary>
            public int WaitToMateTimeScaled { get; private set; }

            /// <summary>
            /// Time that pregnancy takes. (Scaled to simulation time rate)
            /// </summary>
            public int PregnancyTimeScaled { get; private set; }

            /// <summary>
            /// Represents how long rabbit should move in one direction while searching for food. (Scaled to simulation time rate)
            /// </summary>
            public int MoveInOneDirectionTimeScaled { get; private set; }

            /// <summary>
            /// Represents how long should rabbit live until death from natural causes. (Scaled to simulation time rate)
            /// </summary>
            public int LifeExpectancyScaled { get; private set; }

            /// <summary>
            /// Represents value used in generating creatures (so they would not be too powerful).
            /// </summary>
            public int InitialPopulationCredits { get; init; }
        }

        public Config()
        {
            RabbitConfig = new CreatureConfig<Rabbit> { InitialPopulationCredits = 150};
            WolvesConfig = new CreatureConfig<Wolf> { InitialPopulationCredits = 100};
        }

        private (int, int) _mapSize;

        public double TimeRate { get; set; }

        public bool DeathFromOldAge { get; set; }

        public int MaxCreatures { get; set; }

        private double _mutationChance;

        public double MutationChance
        {
            get => _mutationChance;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentException("Value must be between 0 and 1");
                }
                _mutationChance = value;
            }
        }

        private double _mutationImpact;

        public double MutationImpact
        {
            get => _mutationImpact;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Value must be greater than or equal to 0");
                }
                _mutationImpact = value;
            }
        }

        public int FruitsPerDay { get; set; }

        public bool FoodExpires { get; set; }

        public bool DrawRanges { get; set; }

        public bool ExportResultsToCSV { get; set; }

        public World.GenerateOffspringMethod? SelectedOffspringGenerationMethod { get; set; }

        public (int, int) MapSize
        {
            get => _mapSize;

            set
            {
                if (value.Item1 <= 0 || value.Item2 <= 0)
                {
                    throw new ArgumentException("Map size should be greater than 0");
                }

                _mapSize = value;
            }
        }

        public ICreatureConfig<Rabbit> RabbitConfig { get; }

        public ICreatureConfig<Wolf> WolvesConfig { get; }
    }
}