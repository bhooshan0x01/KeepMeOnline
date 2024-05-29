using Avalonia;
using Avalonia.Threading;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace KeepMeOnline
{
    public class GlobalKeyboardHook : IDisposable
    {
        private static IntPtr _hookId = IntPtr.Zero;
        private static LowLevelKeyboardProc _proc = HookCallback;

        public event EventHandler? KeyPressed;

        public GlobalKeyboardHook()
        {
            _hookId = SetHook(_proc);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if(vkCode == VK_L)
                {
                    Logger.LogMessage($" L call back : {vkCode}");

                }
                Logger.LogMessage($"call back: {vkCode}");
                ((App)Application.Current!)._keyboardHook?.OnKeyPressed(EventArgs.Empty);

            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public static bool IsWindowsKeyDown()
        {
            return (GetKeyState(VK_LWIN) & 0x8000) != 0 || (GetKeyState(VK_RWIN) & 0x8000) != 0;
        }

        public static bool IsCtrlShiftDown()
        {
            return (GetKeyState(VK_CONTROL) & 0x8000) != 0 &&
                   (GetKeyState(VK_SHIFT) & 0x8000) != 0;
        }

        protected virtual void OnKeyPressed(EventArgs e)
        {
            KeyPressed?.Invoke(this, e);
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(_hookId);
        }

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int VK_CONTROL = 0x11;
        private const int VK_SHIFT = 0x10;
        private const int VK_LWIN = 0x5B;
        private const int VK_RWIN = 0x5C;
        private const int VK_L = 0x4C; // Virtual key code for 'L'

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);
    }

}
