using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace nn_xor_demo_cs
{
    public partial class Plot : Form
    {
        private bool isRunning; // Track if the user exited the program vie the console.
        Thread consoleThread; // Thread for reading console commands.

        // Delegate to pass back the console commands to the main thread
        // One thread can§t run a while loop to watch the console and also
        // perform the actions at the same time.
        private delegate void ConsoleCmd(string cmd);

        public Plot()
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
            switch (cmd)
            {
                case "plot":
                    DrawPlot();
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

        private void DrawPlot()
        {
            chart1.Series.Clear();

            chart1.Titles.Add("Total Income");

            Series series = chart1.Series.Add("Total Income");
            series.ChartType = SeriesChartType.Spline;
            series.Points.AddXY("September", 100);
            series.Points.AddXY("Obtober", 300);
            series.Points.AddXY("November", 800);
            series.Points.AddXY("December", 200);
            series.Points.AddXY("January", 600);
            series.Points.AddXY("February", 400);
        }
    }
}
