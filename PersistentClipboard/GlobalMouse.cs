using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PersistentClipboard
{
    internal class GlobalMouse
    {
        private const int WH_MOUSE_LL = 14;
        private readonly IntPtr hookId = IntPtr.Zero;
        private Action<PointerData> onMouseMove;

        internal GlobalMouse(Action<PointerData> onMouseMove)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                hookId = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProc, GetModuleHandle(curModule.ModuleName), 0);
            }

            this.onMouseMove = onMouseMove;
        }

        ~GlobalMouse()
        {
            UnhookWindowsHookEx(hookId);
        }

        [StructLayout(LayoutKind.Sequential)]
        public class Point 
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class PointerData
        {
            public Point pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        public IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(WH_MOUSE_LL, nCode, wParam, lParam);
            }

            //Marshall the data from the callback.
            PointerData pointerData = (PointerData) Marshal.PtrToStructure(lParam, typeof(PointerData));
            if (pointerData != null)
                onMouseMove(pointerData);

            return CallNextHookEx(WH_MOUSE_LL, nCode, wParam, lParam); 
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
