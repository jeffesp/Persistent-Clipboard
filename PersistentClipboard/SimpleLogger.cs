using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentClipboard
{
    internal interface ILog
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
        private bool enabled = false;
        public SimpleLogger(bool enable)
        {
            enabled = enable;
        }

        public void Debug(string message)
        {
            if (enabled)
                throw new NotImplementedException();
        }

        public void DebugFormat(string message, params object[] args)
        {
            if (enabled)
                throw new NotImplementedException();
        }

        public void Info(string message)
        {
            if (enabled)
                throw new NotImplementedException();
        }

        public void InfoFormat(string message, params object[] args)
        {
            if (enabled)
                throw new NotImplementedException();
        }

        public void Error(string message)
        {
            if (enabled)
                throw new NotImplementedException();
        }

        public void Error(string message, Exception e)
        {
            if (enabled)
                throw new NotImplementedException();
        }

        public void ErrorFormat(string message, params object[] args)
        {
            if (enabled)
                throw new NotImplementedException();
        }
    }
}
