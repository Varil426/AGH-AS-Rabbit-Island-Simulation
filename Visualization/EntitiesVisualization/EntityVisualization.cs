using System;
using System.Windows.Controls;
using System.Windows.Media;
using Simulation.Entities;

namespace Visualization.EntitiesVisualization;

internal abstract class EntityVisualization<TEntity> : ISimulationEntityVisualization where TEntity : Entity
{
    public EntityVisualization(TEntity entity)
    {
        VisualizedEntity = entity;
        Color = Brushes.Black;
    }

    public TEntity VisualizedEntity { get; }

    public virtual SolidColorBrush Color { get; }

    public abstract void DrawSelf(Canvas canvas);
}
