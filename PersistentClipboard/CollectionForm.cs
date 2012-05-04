using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Permissions;
using System.IO;

using Collections;

namespace PersistentClipboard
{
    public partial class CollectionForm : Form, IClicpboardCollector
    {        
        private const int WM_CLIPBOARDUPDATE = 0x31d;
        private CircularQueue<ClippedItem> clippedText;
        private string lastClippedText;
        private Timer persistenceTimer;

        public CollectionForm()
        {
            InitializeComponent();
            clippedText = ClippedItemFile.Load();
            lock (clippedText)
            {
                if (clippedText.Count > 0)
                    lastClippedText = clippedText.First().Content;
            }

            persistenceTimer = new Timer();
            persistenceTimer.Interval = 60 * 60 * 30; // 30 min 
            persistenceTimer.Tick += persistenceTimer_Tick;
            persistenceTimer.Start();

            Program.Logger.InfoFormat("Loaded database and starting to collect clippings.");
        }

        private void persistenceTimer_Tick(object sender, EventArgs e)
        {
            persistenceTimer.Stop();
            List<ClippedItem> items;
            lock (clippedText) items = clippedText.ToList();
            ClippedItemFile.Save(items);
            persistenceTimer.Start();
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
                        Program.Logger.DebugFormat("Added: {0}", lastClippedText);
                    }
                }
                m.Result = (IntPtr)0;
            }
            base.WndProc(ref m);
        }

        public new void Dispose()
        {
            base.Dispose();
            Program.Logger.Info("Closed database and stopped collecting clippings.");
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
            Program.Logger.DebugFormat("Removed: {0}", item);
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
