using System;
using System.Threading;
using System.Windows.Forms;

namespace PersistentClipboard
{
    static class Program
    {
        public static ILog Logger { get; set; }

        [STAThread]
        static void Main()
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
        }
    }

}
