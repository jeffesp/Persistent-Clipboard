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
        private FileStream persistentData;
        private static string appDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"JeffEsp\PersistentClipboard");
        private static string persistenceFile = Path.Combine(appDataFolder, "PersistentDictionary.xml");

        public CollectionForm()
        {
            InitializeComponent();
            clippedText = new CircularQueue<ClippedItem>(250);
            if (clippedText.Count > 0)
                lastClippedText = clippedText.Reverse().First().Content;

            Program.Logger.InfoFormat("Loaded database in {0} and starting to collect clippings.", appDataFolder);

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }            

            persistentData = File.Open(persistenceFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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
                        clippedText.Enqueue(new ClippedItem { Id = DateTime.UtcNow.Ticks, Content = currentText });
                        Program.Logger.DebugFormat("Added: {0}", lastClippedText);
                    }
                }
                m.Result = (IntPtr)0;
            }
            base.WndProc(ref m);
        }

        public new void Dispose()
        {
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
                return clippedText.OrderByDescending(item => item.Id);
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
            clippedText.Remove(item);
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
