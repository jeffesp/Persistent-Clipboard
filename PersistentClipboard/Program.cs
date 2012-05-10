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
            FileLogDestination fileLogger = null;
            try
            {
                fileLogger = new FileLogDestination("test.log");
                Logger = new SimpleLogger.SimpleLogger(true, Status.Error, new ILogDestination[] { fileLogger });

                Application.ThreadException += new ThreadExceptionEventHandler(new ThreadExceptionHandler().ApplicationThreadException);

                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                }
                catch (Exception e)
                {
                    Logger.Error("App startup", e);
                    // Probably want something a little more sophisticated than this
                    MessageBox.Show(e.Message, "The application could not initialize.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }

                Application.Run(new HostForm(Logger));
            }
            finally
            {
                if (fileLogger != null) fileLogger.Dispose();
            }
        }
    }

}
