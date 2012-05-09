using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PersistentClipboard
{
    public class DesktopWindow : IWin32Window
    {
        private static readonly DesktopWindow MDesktopWindow = new DesktopWindow();

        public static IWin32Window Instance
        {
            get { return MDesktopWindow; }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        IntPtr IWin32Window.Handle
        {
            get
            {
                return GetForegroundWindow();
            }
        }
    }

}
