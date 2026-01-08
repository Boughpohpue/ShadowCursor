using System;
using System.Runtime.InteropServices;

namespace ShadowCursor
{
    public static class InteropExtensions
    {
        public const int SM_CMONITORS = 80;
        private const int ABM_GETTASKBARPOS = 5;

        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public int width { get { return right - left; } }
            public int height { get { return bottom - top; } }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
        public class MONITORINFOEX
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szDevice = new char[32];
            public int dwFlags;
        }


        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int smIndex);

        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(int nAction, int nParam, ref RECT rc, int nUpdate);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX info);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(HandleRef handle, int flags);

        [DllImport("shell32.dll")]
        private static extern IntPtr SHAppBarMessage(int msg, ref APPBARDATA data);

        public static int GetTaskbarHeight()
        {
            APPBARDATA data = new APPBARDATA();
            data.cbSize = Marshal.SizeOf(data);
            SHAppBarMessage(ABM_GETTASKBARPOS, ref data);

            return data.rc.bottom - data.rc.top;
        }

        [DllImport("user32.dll")]
        static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        public static string GetDisplays()
        {
            var result = string.Empty;

            DEVMODE vDevMode = new DEVMODE();
            int i = 0;
            while (EnumDisplaySettings(null, i, ref vDevMode))
            {
                result += $"Width:{vDevMode.dmPelsWidth} Height:{vDevMode.dmPelsHeight} Color:{1 << vDevMode.dmBitsPerPel} Frequency:{vDevMode.dmDisplayFrequency}{Environment.NewLine}";
                i++;
            }

            return result;
        }

        public static string GetDisplay(string deviceName)
        {
            DEVMODE vDevMode = new DEVMODE();
            EnumDisplaySettings(deviceName, 0, ref vDevMode);
            return $"Width:{vDevMode.dmPelsWidth} Height:{vDevMode.dmPelsHeight} Color:{1 << vDevMode.dmBitsPerPel} Frequency:{vDevMode.dmDisplayFrequency}{Environment.NewLine}";
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            //public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }
    }
}
