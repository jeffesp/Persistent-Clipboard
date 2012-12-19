using System;
using System.Collections.Generic;

namespace PersistentClipboard
{
    public interface IClicpboardCollector : IDisposable
    {
        IEnumerable<ClippedItem> Search(string text);

        bool HasItems { get; }
        void RemoveItem(ClippedItem item);
        
        void EnableCollection();
        void DisableCollection();
    }
}
