using System.Windows.Controls;
using System.Windows.Media;
using Simulation.Entities;

namespace Visualization.EntitiesVisualization;

/// <summary>
/// Base for vizualization of <see cref="Entity"/>.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
internal abstract class EntityVisualization<TEntity> : ISimulationEntityVisualization where TEntity : Entity
{
    public EntityVisualization(TEntity entity)
    {
        VisualizedEntity = entity;
        Color = Brushes.Black;
    }

    public TEntity VisualizedEntity { get; }

    public virtual SolidColorBrush Color { get; }

    public Entity Entity => VisualizedEntity;

    public abstract void DrawSelf(Canvas canvas);
}
