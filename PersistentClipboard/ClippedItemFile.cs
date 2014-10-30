using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace PersistentClipboard
{
    public class ClippedItemFile : IDisposable, ICollection<ClippedItem>
    {
        private static readonly string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"JeffEsp\PersistentClipboard");
        private static readonly string persistenceFile = Path.Combine(appDataFolder, "PersistentDictionary.dat");
        private const UInt32 blockSize = 4096;

        private long lastSavedTimestamp;
        private List<ClippedItem> items;
        private bool writeFullFile;
        private readonly Stream persistentData;
        private readonly IContentEncoder contentEncoder;
        private readonly FileBase file;
        private readonly DirectoryWrapper directory;

        public ClippedItemFile() : this(new ProtectedDataEncoder(), new FileWrapper(), new DirectoryWrapper()) { }

        public ClippedItemFile(IContentEncoder contentEncoder, FileBase file, DirectoryWrapper directory)
        {
            this.contentEncoder = contentEncoder;
            this.file = file;
            this.directory = directory;

            EnsureDataDirectoryExists();
            persistentData = file.Open(persistenceFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (persistentData.Length == 0)
                persistentData.SetLength(blockSize * 4);
            WriteHeaderSize();
        }

        private void WriteHeaderSize()
        {
            persistentData.Write(BitConverter.GetBytes(4), 0, 4);
        }

        public void Load()
        {
            items = new List<ClippedItem>();

            if (persistentData.Length > 0)
            {
                LoadFromStream();
            }
        }

        public void Add(ClippedItem item)
        {
            lock (items) items.Add(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(ClippedItem item)
        {
            return items.Contains(item);
        }

        public void CopyTo(ClippedItem[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(ClippedItem item)
        {
            var result = items.Remove(item);
            if (result)
                writeFullFile = true;
            return result;
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<ClippedItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Save()
        {
            if (writeFullFile)
                SaveAllToStream();
            else
                SaveToStream();
            writeFullFile = false;
        }

        private void LoadFromStream()
        {
            // TODO: this can blow up in loading the file, or parsing ints from the file.
            // should handle this by deleting the file, and then letting the user know about this.
            persistentData.Seek(0, SeekOrigin.Begin);

            var headerSize = GetHeaderSize();
            persistentData.Seek(headerSize, SeekOrigin.Begin);

            while (persistentData.Position < persistentData.Length)
            {
                byte[] timestampBytes = new byte[8];
                byte[] lengthBytes = new byte[4];
                persistentData.Read(timestampBytes, 0, 8);
                ContentType contentType = (ContentType)persistentData.ReadByte();
                persistentData.Read(lengthBytes, 0, 4);
                int length = BitConverter.ToInt32(lengthBytes, 0);

                byte[] contentBytes = new byte[length];
                persistentData.Read(contentBytes, 0, length);
                ClippedItem item = new ClippedItem { 
                    Timestamp = BitConverter.ToInt64(timestampBytes, 0),
                    ContentType = contentType,
                    Content = Encoding.Default.GetString(contentEncoder.Decode(contentBytes))
                    //Content = Encoding.Default.GetString(
                    //    ProtectedData.Unprotect(contentBytes, entropy, DataProtectionScope.CurrentUser)
                    //)
                };
                items.Add(item);

                UpdateLastSavedTimestamp(item);
            }
        }


        private long GetHeaderSize()
        {
            byte[] headerCount = new byte[4];
            persistentData.Read(headerCount, 0, 4);
            long headerSize = (BitConverter.ToInt32(headerCount, 0) * blockSize);
            return headerSize;
        }

        private void SaveToStream()
        {
            var clippedItems = items.Where(item => item.Timestamp > lastSavedTimestamp).ToList();

            if (clippedItems.Count <= 0)
                return;

            persistentData.Seek(0, SeekOrigin.End);
            SaveData(clippedItems);
        }

        private void SaveAllToStream()
        {
            persistentData.SetLength(0);
            persistentData.SetLength(blockSize * 4);
            WriteHeaderSize();
            persistentData.Seek(0, SeekOrigin.End);
            SaveData(items);
        }

        private void SaveData(List<ClippedItem> clippedItems)
        {
            foreach (ClippedItem item in clippedItems)
            {
                //var content = ProtectedData.Protect(Encoding.Default.GetBytes(item.Content), entropy, DataProtectionScope.CurrentUser);
                var content = contentEncoder.Encode(Encoding.Default.GetBytes(item.Content));

                persistentData.Write(BitConverter.GetBytes(item.Timestamp), 0, 8);
                persistentData.WriteByte((byte)item.ContentType);
                persistentData.Write(BitConverter.GetBytes(content.Length), 0, 4);
                persistentData.Write(content, 0, content.Length);
            }

            lastSavedTimestamp = clippedItems.OrderByDescending(i => i.Timestamp).First().Timestamp;
        }

        private void EnsureDataDirectoryExists()
        {
            if (!directory.Exists(appDataFolder))
            {
                directory.CreateDirectory(appDataFolder);
            }
        }

        public void Dispose()
        {
            if (persistentData != null)
                persistentData.Close();
        }

        private void UpdateLastSavedTimestamp(ClippedItem item)
        {
            lastSavedTimestamp = lastSavedTimestamp < item.Timestamp ? item.Timestamp : lastSavedTimestamp;
        }
    }
}
