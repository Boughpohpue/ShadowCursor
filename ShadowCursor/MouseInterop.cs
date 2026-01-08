using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using System.Windows;

namespace ShadowCursor
{
    public enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
    }

    public static class MouseInterop
    {
        private const int _mouseIdHook = 14;
        private static IntPtr _mouseHookPointer = IntPtr.Zero;
        private static LowLevelMouseProc _mouseHookProc = HookCallback;
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static event EventHandler<MouseInteropEventArgs> MouseHookEvent = delegate { };

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            
            return lpPoint;
        }

        public static void StartMouseHook() => _mouseHookPointer = SetHook(_mouseHookProc);
        public static void StopMouseHook() => UnhookWindowsHookEx(_mouseHookPointer);

        #region Mouse hook methods

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(_mouseIdHook, proc,
                  GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_MOUSEMOVE == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseHookEvent(null, new MouseInteropEventArgs(MouseMessages.WM_MOUSEMOVE, hookStruct.pt));
            }

            return CallNextHookEx(_mouseHookPointer, nCode, wParam, lParam);
        }

        #endregion

        #region Extern dll import

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
          LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
          IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        #region Hook structs

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData, flags, time;
            public IntPtr dwExtraInfo;
        }

        #endregion
    }

    public class MouseInteropEventArgs : EventArgs
    {
        public MouseMessages MouseMessage { get; set; }
        public Point Point { get; set; }

        public MouseInteropEventArgs(MouseMessages msg, Point p)
        {
            MouseMessage = msg;
            Point = p;
        }
    }
}
