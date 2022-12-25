using Simulation.Entities;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Visualization.EntitiesVisualization
{
    internal class CreatureVisualization<TCreature> : EntityVisualization<TCreature> where TCreature : Creature
    {
        private const int CREATURE_REPRESENTATION_SIZE = 4;
        
        public CreatureVisualization(TCreature creature) : base(creature)
        {
        }

        public override void DrawSelf(Canvas canvas)
        {
            var localCanvas = new Canvas();
            var rectangle = new Rectangle()
            {
                Width = CREATURE_REPRESENTATION_SIZE,
                Height = CREATURE_REPRESENTATION_SIZE,
                Fill = Color
            };
            localCanvas.Children.Add(rectangle);
            // ReSharper disable once PossibleLossOfFraction
            Canvas.SetLeft(rectangle, -CREATURE_REPRESENTATION_SIZE / 2);
            // ReSharper disable once PossibleLossOfFraction
            Canvas.SetTop(rectangle, -CREATURE_REPRESENTATION_SIZE / 2);
            if (VisualizationConfig.Instance.DrawRanges && VisualizedEntity.IsAlive)
            {
                var sightRange = new Ellipse()
                {
                    Width = VisualizedEntity.SightRange,
                    Height = VisualizedEntity.SightRange,
                    Stroke = Color
                };
                localCanvas.Children.Add(sightRange);
                Canvas.SetLeft(sightRange, -VisualizedEntity.SightRange / 2);
                Canvas.SetTop(sightRange, -VisualizedEntity.SightRange / 2);
            }
            canvas.Children.Add(localCanvas);
            Canvas.SetLeft(localCanvas, VisualizedEntity.Position.X);
            Canvas.SetTop(localCanvas, VisualizedEntity.Position.Y);
        }
    }
}
