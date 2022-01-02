using Simulation.Attributes;
using SimulationStandard;
using SimulationStandard.Interfaces;
using System.Reflection;

namespace Simulation;

public class SimulationBuilder : ISimulationBuilder
{
    public enum SimulationParamsEnum
    {
        // Rabbits Settings
        [Type(typeof(long))]
        RabbitsInitialPopulation,

        [Type(typeof(long))]
        RabbitsMinChildren,

        [Type(typeof(long))]
        RabbitsMaxChildren,

        [Type(typeof(long))]
        RabbitsPregnancyDuration,

        [Type(typeof(long))]
        RabbitsLifeExpectancy,

        // Wolves Settings
        [Type(typeof(long))]
        WolvesInitialPopulation,

        [Type(typeof(long))]
        WolvesMinChildren,

        [Type(typeof(long))]
        WolvesMaxChildren,

        [Type(typeof(long))]
        WolvesPregnancyDuration,

        [Type(typeof(long))]
        WolvesLifeExpectancy,

        // Simulation Settings
        [Type(typeof(double))]
        TimeRate,

        [Type(typeof(bool))]
        DeathFromOldAge,

        [Type(typeof(long))]
        MaxCreatures,

        [Type(typeof(long))]
        FruitsPerDay,

        [Type(typeof(long))]
        MapSize,

        [Type(typeof(double))]
        MutationChance,

        [Type(typeof(double))]
        MutationImpact,

        [Type(typeof(long))]
        OffspringGenerationMethod,

        [Type(typeof(long))]
        Timeout,
    }

    public enum SimulationResultsEnum
    {
        [Type(typeof(IList<long>))]
        SimulationTime,

        [Type(typeof(IList<long>))]
        RabbitsAlive,

        [Type(typeof(IList<long>))]
        WolvesAlive,

        [Type(typeof(IList<long>))]
        FruitsPresent,

        [Type(typeof(long))]
        TotalRabbits,

        [Type(typeof(long))]
        TotalWolves,

        [Type(typeof(long))]
        TotalFruits,

        [Type(typeof(long))]
        RabbitsGenerations,

        [Type(typeof(long))]
        WolvesGenerations,
    }

    public ISimulation CreateSimulation(ISimulationParams simulationParams)
    {
        foreach (var param in simulationParams.Params)
        {
            // TODO Validate params -- Create Method
        }

        var simulation = new Simulation()
        {
            Params = simulationParams,
            World = new()
            {
                WorldConfig = CreateWorldConfig(simulationParams),
                WorldMap = new Map((int)(long)simulationParams.Params[SimulationParamsEnum.MapSize.ToString()])
            }
        };

        return simulation;
    }

    public SimulationResultsTemplate CreateSimulationResultsTemplate()
    {
        var simulationResultTemplate = new SimulationResultsTemplate();
        var simulationResultsEnumFields = typeof(SimulationResultsEnum).GetFields();
        foreach (var value in Enum.GetValues(typeof(SimulationResultsEnum)).OfType<SimulationResultsEnum>())
        {
            if (simulationResultsEnumFields.FirstOrDefault(x => x.Name == value.ToString()) is FieldInfo fieldInfo
                && fieldInfo.GetCustomAttribute(typeof(TypeAttribute)) is TypeAttribute typeAttribute)
            {
                simulationResultTemplate[value.ToString()] = typeAttribute.Type;
            }
        }

        return simulationResultTemplate;
    }

    public SimulationParamsTemplate CreateSimulationParamsTemplate()
    {
        var simulationParamsTemplate = new SimulationParamsTemplate();
        var simulationResultsEnumFields = typeof(SimulationParamsEnum).GetFields();
        foreach (var value in Enum.GetValues(typeof(SimulationParamsEnum)).OfType<SimulationParamsEnum>())
        {
            if (simulationResultsEnumFields.FirstOrDefault(x => x.Name == value.ToString()) is FieldInfo fieldInfo
                && fieldInfo.GetCustomAttribute(typeof(TypeAttribute)) is TypeAttribute typeAttribute)
            {
                simulationParamsTemplate[value.ToString()] = typeAttribute.Type;
            }
        }

        return simulationParamsTemplate;
    }

    /// <summary>
    /// Creates <see cref="Config"/> from <see cref="ISimulationParams"/>.
    /// </summary>
    /// <param name="simulationParams">Parameters from which to create configuration.</param>
    /// <returns>Configuration.</returns>
    private Config CreateWorldConfig(ISimulationParams simulationParams)
    {
        // TODO Improve
        var config = new Config
        {
            TimeRate = (double)simulationParams.Params[SimulationParamsEnum.TimeRate.ToString()],
            DeathFromOldAge = (bool)simulationParams.Params[SimulationParamsEnum.DeathFromOldAge.ToString()],
            MaxCreatures = (long)simulationParams.Params[SimulationParamsEnum.MaxCreatures.ToString()],
            FruitsPerDay = (long)simulationParams.Params[SimulationParamsEnum.FruitsPerDay.ToString()],
            MapSize = ((long)simulationParams.Params[SimulationParamsEnum.MapSize.ToString()], (long)simulationParams.Params[SimulationParamsEnum.MapSize.ToString()]),
            SelectedOffspringGenerationMethod = (long)simulationParams.Params[SimulationParamsEnum.OffspringGenerationMethod.ToString()] switch
            {
                0 => OffspringGeneration.OffspringGenerationWithMutations,
                1 => OffspringGeneration.BasicOffspringGeneration,
                _ => throw new ArgumentException("Invalid OffspringGenerationMethod selected"),
            },
            MutationChance = (double)simulationParams.Params[SimulationParamsEnum.MutationChance.ToString()],
            MutationImpact = (double)simulationParams.Params[SimulationParamsEnum.MutationImpact.ToString()],
            Timeout = (long)simulationParams.Params[SimulationParamsEnum.Timeout.ToString()],
        };

        config.RabbitConfig.InitialPopulation = (long)simulationParams.Params[SimulationParamsEnum.RabbitsInitialPopulation.ToString()];
        config.RabbitConfig.MinChildren = (long)simulationParams.Params[SimulationParamsEnum.RabbitsMinChildren.ToString()];
        config.RabbitConfig.MaxChildren = (long)simulationParams.Params[SimulationParamsEnum.RabbitsMaxChildren.ToString()];
        config.RabbitConfig.PregnancyDuration = (long)simulationParams.Params[SimulationParamsEnum.RabbitsPregnancyDuration.ToString()];
        config.RabbitConfig.LifeExpectancy = (long)simulationParams.Params[SimulationParamsEnum.RabbitsLifeExpectancy.ToString()];

        config.WolvesConfig.InitialPopulation = (long)simulationParams.Params[SimulationParamsEnum.WolvesInitialPopulation.ToString()];
        config.WolvesConfig.MinChildren = (long)simulationParams.Params[SimulationParamsEnum.WolvesMinChildren.ToString()];
        config.WolvesConfig.MaxChildren = (long)simulationParams.Params[SimulationParamsEnum.WolvesMaxChildren.ToString()];
        config.WolvesConfig.PregnancyDuration = (long)simulationParams.Params[SimulationParamsEnum.WolvesPregnancyDuration.ToString()];
        config.WolvesConfig.LifeExpectancy = (long)simulationParams.Params[SimulationParamsEnum.WolvesLifeExpectancy.ToString()];

        return config;
    }
}