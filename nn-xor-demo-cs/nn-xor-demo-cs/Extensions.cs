using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace nn_xor_demo_cs
{
    public static class Extensions
    {
        public static void AddXYpoints(this DataPointCollection dataPoints, double[] xArray, double[] yArray)
        {
            if (xArray.Length != yArray.Length)
                throw new ArgumentException("Array lenghts does not match.");
            else
            {
                for (int i = 0; i < xArray.Length; i++)
                    dataPoints.AddXY(xArray[i], yArray[i]);
            }
        }

        public static void SetActivationPlot(this Chart chart, string title, double[] input, double[] output)
        {
            chart.Series.Clear();
            chart.Titles.Clear();

            chart.Titles.Add(title);

            Series series = chart.Series.Add("Points");
            series.ChartType = SeriesChartType.Point;

            series.Points.AddXYpoints(input, output);

            chart.Legends[0].Enabled = false; //Turn off legends
            chart.ChartAreas[0].AxisX.Minimum = -1; //Set X minimum
            chart.ChartAreas[0].AxisX.Maximum = 1; //Set X maximum
            chart.ChartAreas[0].AxisX.MajorGrid.Interval = 1.0;
            chart.ChartAreas[0].AxisX.MajorTickMark.Interval = 0.25;
            chart.ChartAreas[0].AxisX.Interval = 0.25;

            chart.ChartAreas[0].AxisY.Minimum = -1; //Set X minimum
            chart.ChartAreas[0].AxisY.Maximum = 1; //Set X maximum
            chart.ChartAreas[0].AxisY.MajorGrid.Interval = 1.0;
            chart.ChartAreas[0].AxisY.MajorTickMark.Interval = 0.25;
            chart.ChartAreas[0].AxisX.Interval = 0.25;
        }
    }
}
