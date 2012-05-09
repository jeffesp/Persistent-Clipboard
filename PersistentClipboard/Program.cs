using System;
using System.Threading;
using System.Windows.Forms;
using SimpleLogger;

namespace PersistentClipboard
{
    static class Program
    {
        public static ISimpleLogger Logger { get; set; }

        [STAThread]
        static void Main()
        {
            Logger = new SimpleLogger.SimpleLogger(true, Status.Debug, new ILogDestination[] { new DebugWindowLogDestination() });

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
                Environment.Exit(0);
            }

            Application.Run(new HostForm(Logger));
        }
    }

}
