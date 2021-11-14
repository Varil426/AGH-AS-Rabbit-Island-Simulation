using Simulation.Entities;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Visualization.EntitiesVisualization
{
    internal class CreatureVisualization<TCreature> : EntityVisualization<TCreature> where TCreature : Creature
    {
        public CreatureVisualization(TCreature creature) : base(creature)
        {
        }

        public override void DrawSelf(Canvas canvas)
        {
            var localCanvas = new Canvas();
            var rectangle = new Rectangle()
            {
                Width = 1,
                Height = 1,
                Fill = Color
            };
            localCanvas.Children.Add(rectangle);
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
