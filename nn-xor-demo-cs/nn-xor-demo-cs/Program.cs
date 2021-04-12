using System;
using System.Windows.Forms;

namespace nn_xor_demo_cs
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Plot());

            // NOTE: any console command here would run AFTER the form is closed.
            // For "paralell" work, place console command in the code of the main form.
        }
    }
}
