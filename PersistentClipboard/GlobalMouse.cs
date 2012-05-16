using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;

namespace PersistentClipboard
{
    internal class GlobalMouse
    {
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        private readonly IntPtr hookId = IntPtr.Zero;
        private Action<PointerData> onMouseMove;
        private bool buttonDown;

        public class PointerData
        {
            public Point Coordinates;
        }

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

        // This is the POINT struct from windows.h
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT 
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        // This is the MSLLHOOKSTRUCT struct from windows.h
        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public UIntPtr dwExtraInfo;
        }

        private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(WH_MOUSE_LL, nCode, wParam, lParam);
            }

            if ((int)wParam == WM_LBUTTONDOWN)
                buttonDown = true;
            if ((int)wParam == WM_LBUTTONUP)
                buttonDown = false;

            if (buttonDown)
            {
                //Marshall the data from the callback.
                MSLLHOOKSTRUCT hookData = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof (MSLLHOOKSTRUCT));
                PointerData pointer = new PointerData();
                pointer.Coordinates = hookData.pt;
                
                onMouseMove(pointer);
            }
            return CallNextHookEx(WH_MOUSE_LL, nCode, wParam, lParam); 
        }

        private delegate IntPtr MouseHookHandler(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll",CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
        private static extern IntPtr SetWindowsHookEx(int idHook, MouseHookHandler lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll",CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(IntPtr hookId);
                
        [DllImport("user32.dll",CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
        private static extern IntPtr CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
