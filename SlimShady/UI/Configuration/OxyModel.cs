using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using SlimShady.DataFeed;
using System;

namespace SlimShady
{
    public class OxyModel
    {
        public PlotModel MyModel { get; private set; }

        public OxyModel()
        {
            MyModel = new PlotModel();// { Title = "Example 1" };
            MyModel.Series.Add(new FunctionSeries(Math.Acos, 0, 1024, 1024, "monitor Brightness by light level"));
        }

        public static void RefreshModel(PlotView view)
        {
            view.Model = new PlotModel();
            view.Model.Series.Add(new FunctionSeries(ThingSpeakLightConverter.Eval, 0, 1024, 1024, "monitor Brightness by light level"));
            OxyPlot.Axes.LinearAxis x = new OxyPlot.Axes.LinearAxis() { Position = AxisPosition.Bottom, Minimum = -1, Maximum = 1025 };
            OxyPlot.Axes.LinearAxis y = new OxyPlot.Axes.LinearAxis() { Position = AxisPosition.Left, Minimum = -1, Maximum = 101 };
            view.Model.Axes.Add(x);
            view.Model.Axes.Add(y);
            view.InvalidatePlot();
            view.Controller.UnbindAll();
        }
    }
}