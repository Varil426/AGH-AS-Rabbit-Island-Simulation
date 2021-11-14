using Simulation.Entities;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Visualization.EntitiesVisualization
{
    internal class FruitVisualization : EntityVisualization<Fruit>
    {
        public FruitVisualization(Fruit fruit) : base(fruit)
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
            canvas.Children.Add(localCanvas);
            Canvas.SetLeft(localCanvas, VisualizedEntity.Position.X);
            Canvas.SetTop(localCanvas, VisualizedEntity.Position.Y);
        }

        public override SolidColorBrush Color => Brushes.White;
    }
}
