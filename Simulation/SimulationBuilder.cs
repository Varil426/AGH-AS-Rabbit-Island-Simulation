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
        [Type(typeof(uint))]
        RabbitsInitialPopulation,

        [Type(typeof(uint))]
        RabbitsMinChildren,

        [Type(typeof(uint))]
        RabbitsMaxChildren,

        [Type(typeof(uint))]
        RabbitsPregnancyDuration,

        [Type(typeof(uint))]
        RabbitsLifeExpectancy,

        // Wolves Settings
        [Type(typeof(uint))]
        WolvesInitialPopulation,

        [Type(typeof(uint))]
        WolvesMinChildren,

        [Type(typeof(uint))]
        WolvesMaxChildren,

        [Type(typeof(uint))]
        WolvesPregnancyDuration,

        [Type(typeof(uint))]
        WolvesLifeExpectancy,

        // Simulation Settings
        [Type(typeof(double))]
        TimeRate,

        [Type(typeof(bool))]
        DeathFromOldAge,

        [Type(typeof(int))]
        MaxCreatures,

        [Type(typeof(int))]
        FruitsPerDay,

        [Type(typeof(int))]
        MapSize,

        [Type(typeof(double))]
        MutationChance,

        [Type(typeof(double))]
        MutationImpact,

        [Type(typeof(int))]
        OffspringGenerationMethod,
    }

    public enum SimulationResultsEnum
    {
        // TODO
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
                WorldMap = new Map((int)simulationParams.Params[SimulationParamsEnum.MapSize.ToString()])
            }
        };

        return simulation;
    }

    public ISimulationResultsTemplate CreateSimulationResultsTemplate()
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

    public ISimulationParamsTemplate CreateSimulationParamsTemplate()
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
            MaxCreatures = (int)simulationParams.Params[SimulationParamsEnum.MaxCreatures.ToString()],
            FruitsPerDay = (int)simulationParams.Params[SimulationParamsEnum.FruitsPerDay.ToString()],
            MapSize = ((int)simulationParams.Params[SimulationParamsEnum.MapSize.ToString()], (int)simulationParams.Params[SimulationParamsEnum.MapSize.ToString()]),
            SelectedOffspringGenerationMethod = (int)simulationParams.Params[SimulationParamsEnum.TimeRate.ToString()] switch
            {
                0 => OffspringGeneration.OffspringGenerationWithMutations,
                1 => OffspringGeneration.BasicOffspringGeneration,
                _ => throw new ArgumentException("Invalid OffspringGenerationMethod selected"),
            },
            MutationChance = (double)simulationParams.Params[SimulationParamsEnum.MutationChance.ToString()],
            MutationImpact = (double)simulationParams.Params[SimulationParamsEnum.MutationImpact.ToString()],
        };

        config.RabbitConfig.InitialPopulation = (int)simulationParams.Params[SimulationParamsEnum.RabbitsInitialPopulation.ToString()];
        config.RabbitConfig.MinChildren = (int)simulationParams.Params[SimulationParamsEnum.RabbitsMinChildren.ToString()];
        config.RabbitConfig.MaxChildren = (int)simulationParams.Params[SimulationParamsEnum.RabbitsMaxChildren.ToString()];
        config.RabbitConfig.PregnancyDuration = (int)simulationParams.Params[SimulationParamsEnum.RabbitsPregnancyDuration.ToString()];
        config.RabbitConfig.LifeExpectancy = (int)simulationParams.Params[SimulationParamsEnum.RabbitsLifeExpectancy.ToString()];

        config.WolvesConfig.InitialPopulation = (int)simulationParams.Params[SimulationParamsEnum.WolvesInitialPopulation.ToString()];
        config.WolvesConfig.MinChildren = (int)simulationParams.Params[SimulationParamsEnum.WolvesMinChildren.ToString()];
        config.WolvesConfig.MaxChildren = (int)simulationParams.Params[SimulationParamsEnum.WolvesMaxChildren.ToString()];
        config.WolvesConfig.PregnancyDuration = (int)simulationParams.Params[SimulationParamsEnum.WolvesPregnancyDuration.ToString()];
        config.WolvesConfig.LifeExpectancy = (int)simulationParams.Params[SimulationParamsEnum.WolvesLifeExpectancy.ToString()];

        return config;
    }
}