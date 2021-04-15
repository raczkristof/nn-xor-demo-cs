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

        public Main()
        {
            InitializeComponent();

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
            double[] input = Enumerable.Range(-500, 1001).Select(x => x / 100.0).ToArray();

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
                    chart1.SetActivationPlot("Identity", input, input.Identity());
                    break;

                case "ident.deriv":
                    chart1.SetActivationPlot("Derivate of Identity", input, input.Identity(deriv: true));
                    break;

                case "sigmoid":
                    chart1.SetActivationPlot("Sigmoid", input, input.Sigmoid());
                    break;

                case "sigmoid.deriv":
                    chart1.SetActivationPlot("Derivate of Sigmoid", input, input.Sigmoid(deriv: true));
                    break;
                case "tanh":
                    chart1.SetActivationPlot("tanH", input, input.TanH());
                    break;

                case "tanh.deriv":
                    chart1.SetActivationPlot("Derivate of tanH", input, input.TanH(deriv: true));
                    break;
                case "relu":
                    chart1.SetActivationPlot("ReLU", input, input.ReLU());
                    break;

                case "relu.deriv":
                    chart1.SetActivationPlot("Derivate of ReLU", input, input.ReLU(deriv: true));
                    break;
                case "lrelu":
                    chart1.SetActivationPlot("Leaky ReLU", input, input.LReLU(alpha: 0.5));
                    Console.WriteLine("alpha = 0.5 on plot");
                    break;

                case "lrelu.deriv":
                    chart1.SetActivationPlot("Derivate of leaky ReLU", input, input.LReLU(alpha: 0.5, deriv: true));
                    Console.WriteLine("alpha = 0.5 on plot");
                    break;
                default:
                    Console.WriteLine("You asked to plot \"" + plotType + "\". It is not a known plot type.");
                    break;
            }


        }
    }
}
