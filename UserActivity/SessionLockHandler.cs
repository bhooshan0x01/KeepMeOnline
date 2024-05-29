using System;
using System.Runtime.InteropServices;
using Avalonia.Threading;

namespace KeepMeOnline
{
    public class SessionLockHandler : IDisposable
    {
        private const int WTS_SESSION_LOCK = 0x7;
        private const int WTS_SESSION_UNLOCK = 0x8;
        private const int WM_WTSSESSION_CHANGE = 0x02B1;
        private const int NOTIFY_FOR_THIS_SESSION = 0;

        private IntPtr _hiddenWindowHandle;
        private WindowProc _windowProc;
        private IntPtr _prevWndProc;

        [DllImport("wtsapi32.dll")]
        private static extern bool WTSRegisterSessionNotification(IntPtr hWnd, [MarshalAs(UnmanagedType.U4)] int dwFlags);

        [DllImport("wtsapi32.dll")]
        private static extern bool WTSUnRegisterSessionNotification(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateWindowEx(
            int dwExStyle, string lpClassName, string lpWindowName,
            int dwStyle, int x, int y, int nWidth, int nHeight,
            IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr WindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        public event EventHandler? SessionLocked;
        public event EventHandler? SessionUnlocked;

        public SessionLockHandler()
        {
            CreateHiddenWindow();
        }

        private void CreateHiddenWindow()
        {
            _windowProc = new WindowProc(WindowProcCallback);
            var hInstance = GetModuleHandle(null);
            _hiddenWindowHandle = CreateWindowEx(0, "STATIC", "Hidden Window",
                0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);

            if (_hiddenWindowHandle != IntPtr.Zero)
            {
                if (IntPtr.Size == 8)
                {
                    _prevWndProc = SetWindowLongPtr64(_hiddenWindowHandle, -4, Marshal.GetFunctionPointerForDelegate(_windowProc));
                }
                else
                {
                    _prevWndProc = SetWindowLong32(_hiddenWindowHandle, -4, Marshal.GetFunctionPointerForDelegate(_windowProc));
                }
                WTSRegisterSessionNotification(_hiddenWindowHandle, NOTIFY_FOR_THIS_SESSION);
            }
        }

        private IntPtr WindowProcCallback(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            if (uMsg == WM_WTSSESSION_CHANGE)
            {
                if ((int)wParam == WTS_SESSION_LOCK)
                {
                    Dispatcher.UIThread.Post(() => SessionLocked?.Invoke(this, EventArgs.Empty));
                }
                else if ((int)wParam == WTS_SESSION_UNLOCK)
                {
                    Dispatcher.UIThread.Post(() => SessionUnlocked?.Invoke(this, EventArgs.Empty));
                }
            }

            return CallWindowProc(_prevWndProc, hWnd, uMsg, wParam, lParam);
        }

        public void Dispose()
        {
            if (_hiddenWindowHandle != IntPtr.Zero)
            {
                WTSUnRegisterSessionNotification(_hiddenWindowHandle);

                if (IntPtr.Size == 8)
                {
                    SetWindowLongPtr64(_hiddenWindowHandle, -4, _prevWndProc);
                }
                else
                {
                    SetWindowLong32(_hiddenWindowHandle, -4, _prevWndProc);
                }
                DestroyWindow(_hiddenWindowHandle);
            }
        }
    }
}
