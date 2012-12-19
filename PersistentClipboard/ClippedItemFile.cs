using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;

using Collections;

namespace PersistentClipboard
{
    public class ClippedItemFile : IDisposable
    {
        private static readonly string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"JeffEsp\PersistentClipboard");
        private static readonly string persistenceFile = Path.Combine(appDataFolder, "PersistentDictionary.dat");
        private static readonly byte[] entropy = new byte[] { 127,133,211,54,65,125,183,19,157,13,70,171,176,7,251,68 };

        private long lastSavedTimestamp;

        public CircularQueue<ClippedItem> Load()
        {
            var items = new List<ClippedItem>();

            EnsureDataDirectoryExists();

            using (var persistentData = File.Open(persistenceFile, FileMode.OpenOrCreate, FileAccess.Read))
            {
                if (persistentData.Length > 0)
                {
                    LoadFromStream(persistentData, items);
                }
            }

            // TODO: some sort of creational pattern would allow this to be swapped out if necessary
            var result = new CircularQueue<ClippedItem>(250);
            foreach (ClippedItem item in items)
            {
                result.Enqueue(item);
            }

            return result.Reverse();
        }

        public void Save(List<ClippedItem> items)
        {
            EnsureDataDirectoryExists();

            using (var persistentData = File.Open(persistenceFile, FileMode.Open, FileAccess.Write))
            {
                SaveToStream(persistentData, items);
            }
        }

        private void LoadFromStream(Stream s, List<ClippedItem> items)
        {
            // TODO: this can blow up in loading the file, or parsing ints from the file.
            // should handle this by deleting the file, and then letting the user know about this.
            s.Seek(0, SeekOrigin.Begin);
            while (s.Position != s.Length) 
            {
                byte[] timestampBytes = new byte[8];
                byte[] lengthBytes = new byte[4];
                s.Read(timestampBytes, 0, 8);
                s.Read(lengthBytes, 0, 4);
                int length = BitConverter.ToInt32(lengthBytes, 0);

                byte[] contentBytes = new byte[length];
                s.Read(contentBytes, 0, length);
                ClippedItem item = new ClippedItem {Timestamp = BitConverter.ToInt64(timestampBytes, 0), Content = Encoding.Default.GetString(ProtectedData.Unprotect(contentBytes, entropy, DataProtectionScope.CurrentUser))};
                items.Add(item);

                lastSavedTimestamp = lastSavedTimestamp < item.Timestamp ? item.Timestamp : lastSavedTimestamp;
            }
        }

        private void SaveToStream(Stream s, IEnumerable<ClippedItem> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            var clippedItems = items.Where(item => item.Timestamp > lastSavedTimestamp).ToList();

            if (clippedItems.Count <= 0)
                return;

            s.Seek(0, SeekOrigin.End);
            using (var writer = new StreamWriter(s))
            {
                foreach (ClippedItem item in clippedItems)
                {
                    s.Write(BitConverter.GetBytes(item.Timestamp), 0, 8);
                    var content = ProtectedData.Protect(Encoding.Default.GetBytes(item.Content), entropy, DataProtectionScope.CurrentUser);
                    s.Write(BitConverter.GetBytes(content.Length), 0, 4);
                    s.Write(content, 0, content.Length);
                }

            }
            lastSavedTimestamp = clippedItems.OrderByDescending(i => i.Timestamp).First().Timestamp;
        }

        private static void EnsureDataDirectoryExists()
        {
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
        }

        public void Dispose()
        {
        }
    }
}
