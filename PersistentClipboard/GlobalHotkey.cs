/* Copyright (c) 2010 Jeff Espenschied

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PersistentClipboard
{
    internal class GlobalHotkey
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private LowLevelKeyboardProc proc;
        private IntPtr hookID = IntPtr.Zero;
        private HandleKeyDown keyDown;

        private Keys watchKey;
        private Keys watchModifers;

        internal GlobalHotkey(HandleKeyDown down, Keys key, Keys modifiers)
        {
            keyDown = down;
            watchKey = key;
            watchModifers = modifiers;
            proc = HookCallback;
            hookID = SetHook(proc);
        }

        ~GlobalHotkey()
        {
            UnhookWindowsHookEx(hookID);
        }

        internal delegate void HandleKeyDown();

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)GlobalHotkey.WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys k = (Keys)vkCode;
                if (k == watchKey && Control.ModifierKeys == watchModifers)
                    keyDown();
            }
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }


        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}

//namespace PersistentClipboard
//{
//    internal class KeyboardHook
//    {
//        public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);

//        public struct keyboardHookStruct
//        {
//            public int vkCode;
//            public int scanCode;
//            public int flags;
//            public int time;
//            public int dwExtraInfo;
//        }

//        const int WH_KEYBOARD_LL = 13;
//        const int WM_KEYDOWN = 0x100;
//        const int WM_KEYUP = 0x101;
//        const int WM_SYSKEYDOWN = 0x104;
//        const int WM_SYSKEYUP = 0x105;
		


//    }
//}
