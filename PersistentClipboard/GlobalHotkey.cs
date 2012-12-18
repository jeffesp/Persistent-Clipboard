using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PersistentClipboard
{
    internal class GlobalHotkey : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private readonly LowLevelKeyboardProc proc;
        private readonly IntPtr hookId = IntPtr.Zero;
        private readonly HandleKeyDown keyHandleKeyDown;
        private readonly Keys watchKey;
        private readonly Keys watchModifers;

        internal GlobalHotkey(HandleKeyDown handleKeyDown, Keys key, Keys modifiers)
        {
            keyHandleKeyDown = handleKeyDown;
            watchKey = key;
            watchModifers = modifiers;
            proc = HookCallback;
            hookId = SetHook(proc);
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(hookId);
        }

        internal delegate void HandleKeyDown();

        private IntPtr SetHook(LowLevelKeyboardProc keyboardProc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, keyboardProc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys k = (Keys)vkCode;
                if (k == watchKey && Control.ModifierKeys == watchModifers)
                    keyHandleKeyDown();
            }
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}

