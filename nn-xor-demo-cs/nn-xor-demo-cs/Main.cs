using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace nn_xor_demo_cs
{
    public partial class Main : Form
    {
        private bool isRunning; // Track if the user exited the program vie the console. 
        Thread consoleThread; // Thread for reading console commands. 

        // Delegate to pass back the console commands to the main thread 
        // One thread can§t run a while loop to watch the console and also 
        // perform the actions at the same time. 
        private delegate void ConsoleCmd(string cmd);

        double[,] trainingInput; // Data to be used as input to the NN
        double[] trainingLabels; // Labels of the input data

        const int TrainingDataNum = 500; // Number of training datapoints.

        NNModel model; // Neural Network model

        public Main()
        {
            InitializeComponent();

            model = new NNModel(2, new int[] { 5, 13, 24, 7, 1 }, new ActivationFunctions[] { ActivationFunctions.Sigmoid, ActivationFunctions.Sigmoid, ActivationFunctions.Sigmoid, ActivationFunctions.Sigmoid, ActivationFunctions.Sigmoid });

            isRunning = true;

            // Start up a thread to watch for console commands, 
            // so the main thread can execute commands. 
            ConsoleCmd d = new ConsoleCmd(HandleConsoleCmd);
            consoleThread = new Thread(() => {
                while (isRunning)
                {
                    string cmd = Console.ReadLine();
                    this.Invoke(d, cmd);
                }
            });
            consoleThread.Start();
        }

        // Parse console commands 
        private void HandleConsoleCmd(string cmd)
        {
            // See if the input is a plot command 
            if (Regex.IsMatch(cmd, @"plot\(.*\)"))  // use regex to match "plot(*)" 
            {
                DrawPlot(cmd.Substring(5, cmd.Length - 6));
            }
            else
            {
                switch (cmd)
                {
                    case "testDotProduct":
                        TestDot();
                        break;
                    case "resetWeights":
                        model.ResetWeights();
                        break;
                    case "exit":
                        consoleThread.Abort();
                        this.Close();
                        break;
                    default:
                        Console.WriteLine("\"" + cmd + "\" is not a valid command.");
                        break;
                }
            }
        }

        private void DrawPlot(string plotType)
        {
            double[] tmp = Enumerable.Range(-500, 1001).Select(x => x / 100.0).ToArray();
            double[,] input = new double[tmp.Length, 1];

            for (int i = 0; i < tmp.Length; i++) input[i, 0] = tmp[i];

            switch (plotType)
            {
                case "":
                    {
                        chart1.Series.Clear();
                        chart1.Titles.Clear();

                        chart1.Titles.Add("Total Income");

                        Series series = chart1.Series.Add("Total Income");
                        series.ChartType = SeriesChartType.Spline;
                        series.Points.AddXY("September", 100);
                        series.Points.AddXY("Obtober", 300);
                        series.Points.AddXY("November", 800);
                        series.Points.AddXY("December", 200);
                        series.Points.AddXY("January", 600);
                        series.Points.AddXY("February", 400);

                        break;
                    }

                case "ident":
                    chart1.SetActivationPlot("Identity", input.Flatten(), input.Identity().Flatten());
                    break;
                case "ident.deriv":
                    chart1.SetActivationPlot("Derivate of Identity", input.Flatten(), input.Identity(deriv: true).Flatten());
                    break;

                case "sigmoid":
                    chart1.SetActivationPlot("Sigmoid", input.Flatten(), input.Sigmoid().Flatten());
                    break;
                case "sigmoid.deriv":
                    chart1.SetActivationPlot("Derivate of Sigmoid", input.Flatten(), input.Sigmoid(deriv: true).Flatten());
                    break;

                case "tanh":
                    chart1.SetActivationPlot("tanH", input.Flatten(), input.TanH().Flatten());
                    break;
                case "tanh.deriv":
                    chart1.SetActivationPlot("Derivate of tanH", input.Flatten(), input.TanH(deriv: true).Flatten());
                    break;

                case "relu":
                    chart1.SetActivationPlot("ReLU", input.Flatten(), input.ReLU().Flatten());
                    break;
                case "relu.deriv":
                    chart1.SetActivationPlot("Derivate of ReLU", input.Flatten(), input.ReLU(deriv: true).Flatten());
                    break;

                case "lrelu":
                    chart1.SetActivationPlot("Leaky ReLU", input.Flatten(), input.LReLU(alpha: 0.5).Flatten());
                    Console.WriteLine("alpha = 0.5 on plot");
                    break;
                case "lrelu.deriv":
                    chart1.SetActivationPlot("Derivate of leaky ReLU", input.Flatten(), input.LReLU(alpha: 0.5, deriv: true).Flatten());
                    Console.WriteLine("alpha = 0.5 on plot");
                    break;

                case "trainData":
                    if (trainingLabels == null) GenerateXORData(TrainingDataNum);
                    chart1.SetDataPlot(trainingInput, trainingLabels);
                    break;
                case "predictData":
                    if (trainingLabels == null) GenerateXORData(TrainingDataNum);
                    if (model == null)
                    {
                        Console.WriteLine("No network initialised.");
                        break;
                    }
                    chart1.SetDataPlot(trainingInput, model.Predict(trainingInput).Flatten());
                    break;

                default:
                    Console.WriteLine("You asked to plot \"" + plotType + "\". It is not a known plot type.");
                    break;
            }
        }
        private void GenerateXORData(int n)
        {
            trainingInput = new double[n, 2];
            trainingLabels = new double[n];

            Random rnd = new Random();

            for (int i = 0; i < n; i++)
            {
                double x = rnd.NextDouble();
                double y = rnd.NextDouble();

                trainingInput[i, 0] = x;
                trainingInput[i, 1] = y;

                if ((x - 0.05 + rnd.NextDouble() * 0.1 > 0.5) ^ (y - 0.05 + rnd.NextDouble() * 0.1 > 0.5)) // ^ is XOR operator. added random is used to fuzz edges a bit (false labeling)
                    trainingLabels[i] = 1;
                else
                    trainingLabels[i] = 0;
            }
        }

        // Arguably a better way to do this would be unit test
        private void TestDot()
        {
            Console.WriteLine("Testing Dot Product of 2 vectors.");

            double[] vect1 = new double[] { 1, 2 };
            Console.WriteLine(String.Format("vector1 is [{0} {1}]", vect1[0], vect1[1]));
            
            double[] vect2 = new double[] { 3, 7};
            Console.WriteLine(String.Format("vector2 is [{0} {1}]", vect2[0], vect2[1]));

            Console.WriteLine(String.Format("Expected result of DotProduct(vector1, vector2) is {0}.", vect1[0]*vect2[0] + vect1[1]*vect2[1]));
            Console.WriteLine(String.Format("Actual result of DotProduct(vector1, vector2) is {0}.", Extensions.DotProduct(vect1, vect2)));

            Console.WriteLine();
            Console.WriteLine("Testing Dot product of matrix and vector.");

            double[,] matrix = new double[,] { { 3, 54 }, { 5, 9 } };
            Console.WriteLine(String.Format("matrix is [{0} {1}; {2} {3}]", matrix[0, 0], matrix[0, 1], matrix[1, 0], matrix[1, 1]));

            double[] vect = new double[] { 42, 2 };
            Console.WriteLine(String.Format("vector is [{0} {1}]", vect[0], vect[1]));

            Console.WriteLine(String.Format("Expected result of DotProduct(matrix, vector) is [{0}; {1}].", matrix[0, 0] * vect[0] + matrix[0, 1] * vect[1], matrix[1, 0] * vect[0] + matrix[1, 1] * vect[1]));
            double[] resultVec = Extensions.DotProduct(matrix, vect);
            Console.WriteLine(String.Format("Actual result of DotProduct(matrix, vector) is [{0}; {1}].", resultVec[0], resultVec[1]));

            Console.WriteLine();
            Console.WriteLine("Testing Dot product of vector and matrix.");

            Console.WriteLine(String.Format("vector is [{0} {1}]", vect[0], vect[1]));

            Console.WriteLine(String.Format("matrix is [{0} {1}; {2} {3}]", matrix[0,0], matrix[0,1], matrix[1,0], matrix[1,1]));

            Console.WriteLine(String.Format("Expected result of DotProduct(vector, matrix) is [{0} {1}].", vect[0] * matrix[0, 0] + vect[1] * matrix[1, 0], vect[0] * matrix[0, 1] + vect[1] * matrix[1, 1]));
            resultVec = Extensions.DotProduct(vect, matrix);
            Console.WriteLine(String.Format("Actual result of DotProduct(vector, matrix) is [{0} {1}].", resultVec[0], resultVec[1]));

            Console.WriteLine();
            Console.WriteLine("Testing Dot product of 2 matrices");

            double[,] matrixLeft = new double[,] { { 42, 2 }, { 5, 7} };
            Console.WriteLine(String.Format("matrixLeft is [{0} {1}; {2} {3}]", matrixLeft[0, 0], matrixLeft[0, 1], matrixLeft[1, 0], matrixLeft[1, 1]));

            double[,] matrixRight = new double[,] { { 3, 54 }, { 5, 9 } };
            Console.WriteLine(String.Format("matrixRight is [{0} {1}; {2} {3}]", matrixRight[0, 0], matrixRight[0, 1], matrixRight[1, 0], matrixRight[1, 1]));

            Console.WriteLine(String.Format("Expected result of DotProduct(matrix, vector) is [{0} {1}; {2} {3}].", matrixLeft[0, 0] * matrixRight[0, 0] + matrixLeft[0, 1] * matrixRight[1, 0], matrixLeft[0, 0] * matrixRight[0, 1] + matrixLeft[0, 1] * matrixRight[1, 1], matrixLeft[1, 0] * matrixRight[0, 0] + matrixLeft[1, 1] * matrixRight[1, 0], matrixLeft[1, 0] * matrixRight[0, 1] + matrixLeft[1, 1] * matrixRight[1, 1]));
            double[,] resultMat = Extensions.DotProduct(matrixLeft, matrixRight);
            Console.WriteLine(String.Format("Actual result of DotProduct(matrix, vector) is [{0} {1}; {2} {3}]].", resultMat[0, 0], resultMat[0, 1], resultMat[1, 0], resultMat[1, 1]));
        }
    }
}
