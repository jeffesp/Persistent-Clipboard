using System.Threading;

namespace PersistentClipboard
{
    public class ThreadExceptionHandler
    {
        public void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Program.Logger.Error("Caught thread exception: ", e.Exception);
        }
    }
}
