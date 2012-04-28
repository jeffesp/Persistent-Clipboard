using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PersistentClipboard
{
    public class DesktopWindow : IWin32Window
    {
        private static DesktopWindow m_DesktopWindow = new DesktopWindow();

        public static IWin32Window Instance
        {
            get { return m_DesktopWindow; }
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
