using Simulation.Entities;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Visualization.EntitiesVisualization
{
    internal class FruitVisualization : EntityVisualization<Fruit>
    {
        private const int FRUIT_REPRESENTATION_SIZE = 4;
        public FruitVisualization(Fruit fruit) : base(fruit)
        {
        }

        public override void DrawSelf(Canvas canvas)
        {
            var localCanvas = new Canvas();
            var rectangle = new Rectangle()
            {
                Width = FRUIT_REPRESENTATION_SIZE,
                Height = FRUIT_REPRESENTATION_SIZE,
                Fill = Color
            };
            localCanvas.Children.Add(rectangle);
            Canvas.SetLeft(rectangle, -FRUIT_REPRESENTATION_SIZE / 2);
            Canvas.SetTop(rectangle, -FRUIT_REPRESENTATION_SIZE / 2);
            canvas.Children.Add(localCanvas);
            Canvas.SetLeft(localCanvas, VisualizedEntity.Position.X);
            Canvas.SetTop(localCanvas, VisualizedEntity.Position.Y);
        }

        public override SolidColorBrush Color => Brushes.White;
    }
}
