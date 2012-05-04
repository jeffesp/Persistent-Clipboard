﻿using System;
using System.Threading;
using System.Windows.Forms;

namespace PersistentClipboard
{
    static class Program
    {
        public static ILog Logger { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Logger = new SimpleLogger(false);

            Application.ThreadException += new ThreadExceptionEventHandler(new ThreadExceptionHandler().ApplicationThreadException);

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }
            catch (Exception e)
            {
                // Probably want something a little more sophisticated than this
                MessageBox.Show(e.Message, "The application could not initialize.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(0);
            }

            Application.Run(new HostForm(Logger));
        }
    }

}
