using System;
using System.Collections.Generic;

namespace PersistentClipboard
{
    public interface IClicpboardCollector : IDisposable
    {
        string GetLastItem();
        bool HasItems { get; }
        IEnumerable<ClippedItem> Search(string text);

        void RemoveItem(ClippedItem item);
        
        void EnableCollection();
        void DisableCollection();

        new void Dispose();
    }
}
