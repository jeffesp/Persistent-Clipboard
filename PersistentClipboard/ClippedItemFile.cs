using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Collections;
using System.Xml.Linq;
using System.Xml;

namespace PersistentClipboard
{
    public class ClippedItemFile
    {
        private static string appDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"JeffEsp\PersistentClipboard");
        private static string persistenceFile = Path.Combine(appDataFolder, "PersistentDictionary.xml");


        public static CircularQueue<ClippedItem> Load()
        {
            List<ClippedItem> items = new List<ClippedItem>();

            EnsureDataDirectoryExists();

            using (var persistentData = File.Open(persistenceFile, FileMode.OpenOrCreate, FileAccess.Read))
            {
                if (persistentData.Length > 0)
                {
                    LoadFromStream(persistentData, items);
                }
            }

            var result = new CircularQueue<ClippedItem>(250);
            foreach (ClippedItem item in items)
            {
                result.Enqueue(item);
            }

            return result.Reverse();
        }

        public static void Save(List<ClippedItem> items)
        {
            EnsureDataDirectoryExists();

            using (var persistentData = File.Open(persistenceFile, FileMode.Truncate, FileAccess.Write))
            {
                SaveToStream(persistentData, items);
            }
        }

        private static void LoadFromStream(Stream s, List<ClippedItem> items)
        {
            // TODO: this can blow up in loading the file, or parsing ints from the file.
            using (var reader = new XmlTextReader(s))
            {
                var document = XDocument.Load(reader);

                foreach (XElement item in document.Element("items").Elements())
                {
                    items.Add(new ClippedItem { Id = Int64.Parse(item.Element("id").Value), Content = item.Element("content").Value });
                }
            }
        }

        private static void SaveToStream(Stream s, List<ClippedItem> items)
        {
            using (var writer = new XmlTextWriter(s, UTF8Encoding.UTF8))
            {
                var elements = new XElement("items");

                foreach (ClippedItem item in items)
                {
                    elements.Add(new XElement("item", new XElement("id", item.Id), new XElement("content", item.Content)));
                }

                var document = new XDocument(elements);
                document.WriteTo(writer);
            }
        }

        private static void EnsureDataDirectoryExists()
        {
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
        }
    }
}
