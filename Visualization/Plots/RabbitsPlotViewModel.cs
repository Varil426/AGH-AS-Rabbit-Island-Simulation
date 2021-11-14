using OxyPlot;
using OxyPlot.Series;
using System;

namespace Visualization.Plots
{
    internal class RabbitsPlotViewModel
    {
        public RabbitsPlotViewModel()
        {
            RabbitsPlot = new PlotModel
            {
                Title = "Rabbits"
            };
            RabbitsPlot.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
        }

        public PlotModel RabbitsPlot { get; private set; }
    }
}