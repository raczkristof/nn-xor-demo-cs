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
        // Add multiple points to a series from two arrays
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

        // Plot an activation function
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

        // Plot XOR data
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
                if (labels[i] > 0.5) trueData.Points.AddXY(dataPoints[i, 0], dataPoints[i, 1]);  // Add points with 1 label to true data series
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

        // Append a bottom row of ones for bias
        public static double[,] AppendColOnes(this double[,] matrix)
        {
            double[,] output = new double[matrix.GetLength(0), matrix.GetLength(1) + 1];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    output[i, j] = matrix[i, j];

                output[i, output.GetLength(1) - 1] = 1.0;
            }

            return output;
        }

        // Flatten a 1 by m matrix to a vector
        public static double[] Flatten(this double[,] matrix)
        {
            if (matrix.GetLength(1) != 1) throw new ArgumentException("The given matrix is not a one-column matrix.");
            else
            {
                double[] vector = new double[matrix.GetLength(0)];
                for (int i = 0; i < vector.Length; i++) vector[i] = matrix[i, 0];

                return vector;
            }
        }

        // Multiply two vectors
        public static double DotProduct(double[] vect1, double[] vect2)
        {
            if (vect1.Length != vect2.Length) throw new ArgumentException("Vector sizes are not equal. Can not calculate Dot product.");
            else
                return Enumerable.Range(0, vect1.Length).Select(x => vect1[x] * vect2[x]).Sum();
        }

        // Multiply a vector with an appropriately sized matrix.
        public static double[] DotProduct(double[,] matrix, double[] vector)
        {
            if (matrix.GetLength(1) != vector.Length) throw new ArgumentException("Vector and Matrix sizes are incosistent for Dot product.");
            else
            { 
                // Iterate oever the rows of the matrix. In each row, get the DotProduct for the row and the vector (scalar result)
                return Enumerable.Range(0, matrix.GetLength(0)).Select(x => Enumerable.Range(0, matrix.GetLength(1)).Select(y => matrix[x,y]*vector[y]).Sum()).ToArray();
            }
        }

        // Multiply a matrix with an appropriately sized matrix.
        public static double[,] DotProduct(double[,] matrixLeft, double[,] matrixRight)
        {
            double[,] output;
            if (matrixLeft.GetLength(1) != matrixRight.GetLength(0)) throw new ArgumentException("Matrix sizes are incosistent for Dot product.");
            else
            {
                output = new double[matrixLeft.GetLength(0), matrixRight.GetLength(1)];

                for (int i = 0; i < matrixLeft.GetLength(0); i++)
                {
                    for (int j = 0; j < matrixRight.GetLength(1); j++)
                        // Iterate oever the rows of the matrix. In each row, get the DotProduct for the row and the vector (scalar result)
                        output[i,j] = Enumerable.Range(0, matrixLeft.GetLength(1)).Select(x => matrixLeft[i,x] * matrixRight[x, j]).Sum();
                }

                return output;
            }
        }
    }
}
