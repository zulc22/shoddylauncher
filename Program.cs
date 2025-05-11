using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ShoddyLauncher
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
            Setup i = new Setup();
            Application.Run(i);
            if (i.archiveContents != null) {
                Application.Run(new Player(i.archiveContents));
            }
        }
    }
}
