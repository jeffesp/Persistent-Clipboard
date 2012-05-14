using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PersistentClipboard
{
    internal class GlobalMouse
    {
        private const int WH_MOUSE_LL = 14;
        private readonly IntPtr hookId = IntPtr.Zero;

        internal GlobalMouse()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                hookId = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        ~GlobalMouse()
        {
            UnhookWindowsHookEx(hookId);
        }


        [StructLayout(LayoutKind.Sequential)]
        public class POINT 
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct 
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }
        public IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //Marshall the data from the callback.
            MouseHookStruct MyMouseHookStruct = (MouseHookStruct) Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

            if (nCode < 0)
            {
                return CallNextHookEx(WH_MOUSE_LL, nCode, wParam, lParam);
            }
            else
            {
                String strCaption = "x = " + MyMouseHookStruct.pt.x.ToString("d") + "  y = " + MyMouseHookStruct.pt.y.ToString("d");
                Debug.WriteLine(strCaption);
                return CallNextHookEx(WH_MOUSE_LL, nCode, wParam, lParam); 
            }
        }

        public delegate IntPtr MouseHookProcs(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll",CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
        private static extern IntPtr SetWindowsHookEx(int idHook, MouseHookProcs lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll",CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(IntPtr hookId);
                
        [DllImport("user32.dll",CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
        private static extern IntPtr CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
