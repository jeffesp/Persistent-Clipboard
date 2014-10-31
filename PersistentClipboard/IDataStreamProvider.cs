using System;
using System.IO;

namespace PersistentClipboard
{
    public interface IDataStreamProvider
    {
        Stream GetStream();
    }

    public class FileStreamProvider : IDataStreamProvider
    {
        string persistenceFile;
        public FileStreamProvider()
        {
            string appDataFolder; 
            appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"JeffEsp\PersistentClipboard");
            persistenceFile = Path.Combine(appDataFolder, "PersistentDictionary.dat");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
        }

        public Stream GetStream()
        {
            return File.Open(persistenceFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

    }
}
