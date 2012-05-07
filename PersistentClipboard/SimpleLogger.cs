using System;
using System.Text;

namespace PersistentClipboard
{
    public interface ILog
    {
        void Debug(string message);
        void DebugFormat(string message, params object[] args);
        void Info(string message);
        void InfoFormat(string message, params object[] args);
        void Error(string message);
        void Error(string message, Exception e);
        void ErrorFormat(string message, params object[] args);
    }

    internal class SimpleLogger : ILog
    {
        private readonly bool enabled = false;

        public SimpleLogger(bool enable)
        {
            enabled = enable;
        }

        public void Debug(string message)
        {
            Log(Status.Debug, message, null);
        }

        public void DebugFormat(string message, params object[] args)
        {
            Log(Status.Debug, message, args);
        }

        public void Info(string message)
        {
            Log(Status.Info, message, null);
        }

        public void InfoFormat(string message, params object[] args)
        {
            Log(Status.Info, message, args);
        }

        public void Error(string message)
        {
            Log(Status.Error, message, null);
        }

        public void Error(string message, Exception e)
        {
            Log(Status.Error, message + e, null);
        }

        public void ErrorFormat(string message, params object[] args)
        {
            Log(Status.Error, message, args);
        }


        private void Log(Status status, string message, params object[] args)
        {
            var formattedMessage = new StringBuilder();
            if (enabled)
            {
                formattedMessage.AppendFormat("{0}: ", status);
                if (args == null)
                {
                    formattedMessage.Append(message);
                }
                else
                {
                    formattedMessage.AppendFormat(message, args);
                }
                System.Diagnostics.Debug.WriteLine(formattedMessage);
            }
        }
    }

    internal enum Status
    {
        Debug,
        Info,
        Error,
    }
}
