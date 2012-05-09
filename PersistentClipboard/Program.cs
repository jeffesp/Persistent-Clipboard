using System;
using System.Threading;
using System.Windows.Forms;

namespace PersistentClipboard
{
    static class Program
    {
        static readonly Mutex Mutex = new Mutex(true, "{1A31407E-EDFB-43B8-95B5-D288CA052622}");
        public static ILog Logger { get; set; }

        [STAThread]
        static void Main()
        {
            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
                Logger = new SimpleLogger(true);

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
                Mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("Another instance of Persistent Clipboard is already running.");
            }
        }
    }

}
