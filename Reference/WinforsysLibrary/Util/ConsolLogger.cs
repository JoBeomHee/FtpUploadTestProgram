using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using System.Security;

namespace Winforsys.Util
{
    public class ConsoleLogger
    {
        [SuppressUnmanagedCodeSecurityAttribute]
        internal static class UnsafeNativeMethods   
        {
            [DllImport("kernel32")]
            public static extern bool AllocConsole();

            [DllImport("kernel32")]
            public static extern bool FreeConsole();

            [DllImport("user32.dll")]
            public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

            [DllImport("user32.dll")]
            public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
            internal const UInt32 SC_CLOSE = 0xF060;
            internal const UInt32 MF_ENABLED = 0x00000000;
            internal const UInt32 MF_GRAYED = 0x00000001;
            internal const UInt32 MF_DISABLED = 0x00000002;
            internal const uint MF_BYCOMMAND = 0x00000000;

            [DllImport("kernel32")]
            public static extern IntPtr GetConsoleWindow();

            [DllImport("user32")]
            public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        }

        public static bool IsShow = false;       
        
        const int SW_HIDE = 0;
        const int SW_SHOWNORMAL = 1;
        const int SW_NORMAL = 1;
        const int SW_SHOWMINIMIZED = 2;
        const int SW_SHOWMAXIMIZED = 3;
        const int SW_MAXIMIZE = 3;
        const int SW_SHOWNOACTIVATE = 4;
        const int SW_SHOW = 5;
        const int SW_MINIMIZE = 6;
        const int SW_SHOWMINNOACTIVE = 7;
        const int SW_SHOWNA = 8;
        const int SW_RESTORE = 9;
        const int SW_SHOWDEFAULT = 10;

        const int SW_MAX = 10;

        public static void Init()
        {
            UnsafeNativeMethods.AllocConsole();

            IntPtr hwnd = UnsafeNativeMethods.GetConsoleWindow();

            IntPtr hSystemMenu = UnsafeNativeMethods.GetSystemMenu(hwnd, false);
            UnsafeNativeMethods.EnableMenuItem(hSystemMenu, UnsafeNativeMethods.SC_CLOSE, (uint)(UnsafeNativeMethods.MF_ENABLED | (false ? UnsafeNativeMethods.MF_ENABLED : UnsafeNativeMethods.MF_GRAYED)));
        }

        public static void Show()
        {
            IntPtr hwnd = UnsafeNativeMethods.GetConsoleWindow();

            if (hwnd != IntPtr.Zero)
                UnsafeNativeMethods.ShowWindow(hwnd, SW_SHOW);

            IsShow = true;
        }
        
        public static void Hide()
        {
            IntPtr hwnd = UnsafeNativeMethods.GetConsoleWindow();

            if (hwnd != IntPtr.Zero)
                UnsafeNativeMethods.ShowWindow(hwnd, SW_HIDE);

            IsShow = false;
        }

        public static void ShowLog(LogType type, string format, params object[] arg)
        {
            string msg = string.Format(format, arg) + Environment.NewLine;

            if (type == LogType.Error)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (type == LogType.Warning)
                Console.ForegroundColor = ConsoleColor.Magenta;
            else if (type == LogType.SendMsg)
                Console.ForegroundColor = ConsoleColor.Green;
            else if (type == LogType.ReceiveMsg)
                Console.ForegroundColor = ConsoleColor.Cyan;
            else
                Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(string.Format("{0} {1} >> {2}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), type.ToString(), msg));
            LogManager.Instance.WriteLog(type, format, arg);
        }  
    }
}
