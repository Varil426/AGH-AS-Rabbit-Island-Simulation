using System.Windows.Controls;

namespace Visualization;

internal interface IWPFDrawable
{
    public void DrawSelf(Canvas canvas);
}