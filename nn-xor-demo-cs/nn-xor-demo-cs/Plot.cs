using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace nn_xor_demo_cs
{
    public partial class Plot : Form
    {
        public Plot()
        {
            InitializeComponent();

            Console.WriteLine("Hello World!");

            string cmd = Console.ReadLine();

            if (cmd == "plot")
                DrawTestPlot();
        }

        private void DrawTestPlot()
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
