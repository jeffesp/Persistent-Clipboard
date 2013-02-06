using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using SimpleLogger;

namespace PersistentClipboard
{
    public partial class CollectionForm : Form, IClicpboardCollector
    {        
        private const int WM_CLIPBOARDUPDATE = 0x31d;
        private readonly ISimpleLogger logger;
        private readonly ClippedItemFile file;
        private string lastClippedText;

        public CollectionForm(ISimpleLogger logger)
        {
            this.logger = logger;
            file = new ClippedItemFile();
            InitializeComponent();
            lock (file)
            {
                file.Load();
                if (file.Count > 0)
                    lastClippedText = file.First().Content;
            }

            logger.InfoFormat("Loaded database and starting to collect clippings.");
        }

        private void SaveList()
        {
            lock (file)
            {
                file.Save();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                if (Clipboard.ContainsText())
                {
                    string currentText = Clipboard.GetText().Trim();
                    if (!currentText.Equals(lastClippedText) && !String.IsNullOrEmpty(currentText.Trim()))
                    {
                        lastClippedText = currentText;
                        lock (file) file.Add(new ClippedItem { Timestamp = DateTime.UtcNow.Ticks, Content = currentText });
                        logger.DebugFormat("Added: {0}", lastClippedText);
                        ThreadPool.QueueUserWorkItem(arg => SaveList());
                    }
                }
                m.Result = (IntPtr)0;
            }
            base.WndProc(ref m);
        }

        void IDisposable.Dispose()
        {
            lock (file)
            {
                if (file != null)
                    file.Dispose();
            }
            Dispose();
            logger.Info("Closed database and stopped collecting clippings.");
        }

        public bool HasItems
        {
            get
            {
                lock(file) return file.Count > 0;
            }
        }

        private IEnumerable<ClippedItem> OrderedItems
        {
            get
            {
                List<ClippedItem> items;
                lock (file)
                {
                    items = file.ToList();
                }
                return items.OrderByDescending(item => item.Timestamp);
            }
        }

        public IEnumerable<ClippedItem> Search(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                return OrderedItems.Where(ci => ci.Content.Trim().ToLowerInvariant().Contains(text.ToLowerInvariant().Trim()));
            }

            return OrderedItems;
        }

        public void RemoveItem(ClippedItem item)
        {
            lock (file) file.Remove(item);
            // do in background as to not block UI thread for file IO.
            ThreadPool.QueueUserWorkItem(arg => SaveList());
            logger.DebugFormat("Removed: {0}", item);
        }

        public void EnableCollection()
        {
            AddClipboardFormatListener(Handle);
        }

        public void DisableCollection()
        {
            RemoveClipboardFormatListener(Handle);
        }

        [DllImport("user32.dll", EntryPoint = "AddClipboardFormatListener")]
        private static extern bool AddClipboardFormatListener(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "RemoveClipboardFormatListener")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

    }
}
