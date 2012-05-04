using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Collections;

using Timer = System.Windows.Forms.Timer;

namespace PersistentClipboard
{
    public partial class CollectionForm : Form, IClicpboardCollector
    {        
        private const int WM_CLIPBOARDUPDATE = 0x31d;
        private readonly object fileLocker = new object();
        private readonly ILog logger;
        private readonly CircularQueue<ClippedItem> clippedText;
        private readonly Timer persistenceTimer;
        private string lastClippedText;

        public CollectionForm(ILog logger)
        {
            this.logger = logger;
            InitializeComponent();
            lock (fileLocker)
            {
                clippedText = ClippedItemFile.Load();
            }
            lock (clippedText)
            {
                if (clippedText.Count > 0)
                    lastClippedText = clippedText.First().Content;
            }

            persistenceTimer = new Timer();
            persistenceTimer.Interval = 60 * 60 * 30; // 30 min 
            persistenceTimer.Tick += PersistenceTimerTick;
            persistenceTimer.Start();

            logger.InfoFormat("Loaded database and starting to collect clippings.");
        }

        private void PersistenceTimerTick(object sender, EventArgs e)
        {
            persistenceTimer.Stop();
            SaveList();
            persistenceTimer.Start();
        }

        private void SaveList()
        {
            List<ClippedItem> items;
            lock (clippedText) items = clippedText.ToList();
            lock (fileLocker) ClippedItemFile.Save(items);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                if (Clipboard.ContainsText())
                {
                    string currentText = Clipboard.GetText().Trim();
                    if (!currentText.Equals(lastClippedText) && !String.IsNullOrEmpty(currentText.Trim()))
                    {
                        lastClippedText = currentText;
                        lock (clippedText) clippedText.Enqueue(new ClippedItem { Id = DateTime.UtcNow.Ticks, Content = currentText });
                        logger.DebugFormat("Added: {0}", lastClippedText);
                    }
                }
                m.Result = (IntPtr)0;
            }
            base.WndProc(ref m);
        }

        public new void Dispose()
        {
            SaveList();
            base.Dispose();
            logger.Info("Closed database and stopped collecting clippings.");
        }

        public bool HasItems
        {
            get
            {
                lock(clippedText) return clippedText.Count > 0;
            }
        }

        private IOrderedEnumerable<ClippedItem> OrderedItems
        {
            get
            {
                List<ClippedItem> items;
                lock (clippedText)
                {
                    items = clippedText.ToList();
                }
                return items.OrderByDescending(item => item.Id);
            }
        }

        public IEnumerable<ClippedItem> Search(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                return OrderedItems.Where(ci => ci.Content.Trim().ToLowerInvariant().Contains(text.ToLowerInvariant().Trim()));
            }
            else
            {
                return OrderedItems;
            }
        }

        public void RemoveItem(ClippedItem item)
        {
            lock (clippedText) clippedText.Remove(item);
            // do in background as to not block UI thread for file IO.
            ThreadPool.QueueUserWorkItem((arg) => SaveList());
            logger.DebugFormat("Removed: {0}", item);
        }

        public string GetLastItem()
        {
            return lastClippedText;
        }

        public void EnableCollection()
        {
            AddClipboardFormatListener(this.Handle);
        }

        public void DisableCollection()
        {
            RemoveClipboardFormatListener(this.Handle);
        }

        [DllImport("user32.dll", EntryPoint = "AddClipboardFormatListener")]
        private static extern bool AddClipboardFormatListener(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "RemoveClipboardFormatListener")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

    }
}
