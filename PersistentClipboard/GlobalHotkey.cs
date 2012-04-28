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

