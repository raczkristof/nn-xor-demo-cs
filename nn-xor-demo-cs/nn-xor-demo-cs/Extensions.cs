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
            chart.ChartAreas[0].AxisX.Minimum = -5; //Set X minimum
            chart.ChartAreas[0].AxisX.Maximum = 5; //Set X maximum
            chart.ChartAreas[0].AxisX.MajorGrid.Interval = 5.0;
            chart.ChartAreas[0].AxisX.MajorTickMark.Interval = 1;
            chart.ChartAreas[0].AxisX.Interval = 1;
        }

        public static void SetDataPlot(this Chart chart, double[,] dataPoints, double[] labels)
        {
            chart.Series.Clear();
            chart.Titles.Clear();

            chart.Titles.Add("Data");

            Series falseData = chart.Series.Add("False");
            falseData.ChartType = SeriesChartType.Point;

            Series trueData = chart.Series.Add("True");
            trueData.ChartType = SeriesChartType.Point;

            for (int i = 0; i < labels.Length; i++)
                if (labels[i] == 1) trueData.Points.AddXY(dataPoints[i, 0], dataPoints[i, 1]);  // Add points with 1 label to true data series
                else falseData.Points.AddXY(dataPoints[i, 0], dataPoints[i, 1]);                // Add points with 0 label to false data series

            chart.Legends[0].Enabled = false; //Turn off legends
            falseData.Color = System.Drawing.Color.Blue;
            falseData.MarkerStyle = MarkerStyle.Circle;

            trueData.Color = System.Drawing.Color.Red;
            trueData.MarkerStyle = MarkerStyle.Circle;

            chart.ChartAreas[0].AxisX.Minimum = 0; //Set X minimum
            chart.ChartAreas[0].AxisX.Maximum = 1; //Set X maximum
            chart.ChartAreas[0].AxisX.MajorGrid.Interval = 0.5;
            chart.ChartAreas[0].AxisX.MajorTickMark.Interval = 0.5;
            chart.ChartAreas[0].AxisX.Interval = 1;

            chart.ChartAreas[0].AxisY.Minimum = 0; //Set X minimum
            chart.ChartAreas[0].AxisY.Maximum = 1; //Set X maximum
            chart.ChartAreas[0].AxisY.MajorGrid.Interval = 0.5;
            chart.ChartAreas[0].AxisY.MajorTickMark.Interval = 0.5;
            chart.ChartAreas[0].AxisY.Interval = 1;
        }
    }
}
