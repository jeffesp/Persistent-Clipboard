using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Permissions;
using System.IO;

namespace PersistentClipboard
{
    public partial class CollectionForm : Form, IClicpboardCollector
    {        
        private const int WM_CLIPBOARDUPDATE = 0x31d;
        private Dictionary<long, string> clippedText;
        private string lastClippedText;
        private Timer cleanupTimer;
        private FileStream persistentData;
        private static string appDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"JeffEsp\PersistentClipboard");
        private static string persistenceFile = Path.Combine(appDataFolder, "PersistentDictionary.xml");

        public CollectionForm()
        {
            InitializeComponent();
            clippedText = new Dictionary<long, string>();
            if (clippedText.Count > 0)
                lastClippedText = clippedText.OrderByDescending(x => x.Key).First().Value;

            Program.Logger.InfoFormat("Loaded database in {0} and starting to collect clippings.", appDataFolder);

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }            

            persistentData = File.Open(persistenceFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            cleanupTimer = new Timer();
            cleanupTimer.Interval = 10 * 1000;
            cleanupTimer.Tick += new EventHandler(cleanupTimer_Tick);
            cleanupTimer.Start();
        }

        void cleanupTimer_Tick(object sender, EventArgs e)
        {
            cleanupTimer.Stop();
            CleanOldEntries();
            cleanupTimer.Interval = 60 * 60 * 4 * 1000; // 4 hours
            cleanupTimer.Start();
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
                        clippedText.Add(DateTime.UtcNow.Ticks, currentText);
                        Program.Logger.DebugFormat("Added: {0}", lastClippedText);
                    }
                }
                m.Result = (IntPtr)0;
            }
            base.WndProc(ref m);
        }

        public new void Dispose()
        {
            cleanupTimer.Stop();
            CleanOldEntries();
            persistentData.Dispose();
            base.Dispose();
            Program.Logger.Info("Closed database and stopped collecting clippings.");
        }

        public bool HasItems
        {
            get
            {
                return clippedText.Count > 0;
            }
        }

        private IOrderedEnumerable<ClippedItem> OrderedItems
        {
            get
            {
                return clippedText.Select(kv => new ClippedItem() { Id = kv.Key, Content = kv.Value }).OrderByDescending(ci => ci.Id);
            }
        }

        public IEnumerable<ClippedItem> Search(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                return OrderedItems.Where(ci => ci.Content.Trim().ToLowerInvariant().Contains(text.ToLowerInvariant().Trim()));
            }
            else
                return OrderedItems;

        }

        public void RemoveItem(ClippedItem item)
        {
            clippedText.Remove(item.Id);
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

        private void CleanOldEntries()
        {
            var ticks = DateTime.Today.AddDays(-30).Ticks;
            foreach (var item in clippedText.Where(ci => ci.Key < ticks))
                clippedText.Remove(item.Key);
        }

        [DllImport("user32.dll", EntryPoint = "AddClipboardFormatListener")]
        private static extern bool AddClipboardFormatListener(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "RemoveClipboardFormatListener")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

    }
}
