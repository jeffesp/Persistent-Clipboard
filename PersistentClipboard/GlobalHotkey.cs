using System;
using System.Diagnostics;
using System.Windows.Forms;

using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;

using SimpleLogger;

namespace PersistentClipboard
{
    internal class GlobalHotkey : IDisposable
    {
        private readonly HandleKeyDown keyHandleKeyDown;
        private readonly Keys watchKey;
        private readonly Keys watchModifers;
        private readonly KeyboardHookListener listener;

        internal GlobalHotkey(HandleKeyDown handleKeyDown, Keys key, Keys modifiers)
        {
            keyHandleKeyDown = handleKeyDown;
            watchKey = key;
            watchModifers = modifiers;
            listener = new KeyboardHookListener(new GlobalHooker());
            listener.Enabled = true;
            listener.KeyDown += HookCallback;
        }

        internal delegate void HandleKeyDown();

        private void HookCallback(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == watchKey && Control.ModifierKeys == watchModifers)
            {
                keyHandleKeyDown();
                e.Handled = true;
            }
        }

        public void Dispose()
        {
            if (listener != null)
                listener.Dispose();
        }
    }
}

