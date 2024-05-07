using System;
using System.Runtime.InteropServices;

namespace KeepMeOnline;

internal static class ImmortalMethods
{
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

    // Define enumeration for execution state flags
    [Flags]
    private enum ExecutionState : uint
    {
        EsAwayModeRequired = 0x00000040,
        EsContinuous = 0x80000000,
        EsDisplayRequired = 0x00000002,
        EsSystemRequired = 0x00000001
    }

    //Methods to prevent system sleep and keep system awake
    public static void PreventSleep()
    {
        SetThreadExecutionState(ExecutionState.EsContinuous | ExecutionState.EsSystemRequired);
    }

    //Methods to allow the system to sleep
    public static void AllowSleep()
    {
        SetThreadExecutionState(ExecutionState.EsContinuous);
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    // Input related constants
    private const int INPUT_MOUSE = 0;
    private const int MOUSEEVENTF_MOVE = 0x0001;

    // Input structure
    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public int type;
        public MouseInput mi;
    }
    // MouseInput structure
    [StructLayout(LayoutKind.Sequential)]
    private struct MouseInput
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    // simulate mouse movement
    public static void SimulateMouseMovement()
    {
        INPUT[] input = new INPUT[1];
        input[0] = new INPUT();
        input[0].type = INPUT_MOUSE;
        input[0].mi.dx = 1;
        input[0].mi.dy = 1;
        input[0].mi.mouseData = 0;
        input[0].mi.dwFlags = MOUSEEVENTF_MOVE;
        input[0].mi.time = 0;
        input[0].mi.dwExtraInfo = IntPtr.Zero;

        SendInput(1, input, Marshal.SizeOf(typeof(INPUT)));
    }
}