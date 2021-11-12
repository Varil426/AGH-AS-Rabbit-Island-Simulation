using System.Windows.Controls;

namespace Simulation.Entities
{
    internal interface IWPFDrawable
    {
        public void DrawSelf(Canvas canvas);
    }
}