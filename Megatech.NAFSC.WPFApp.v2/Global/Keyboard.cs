using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Megatech.NAFSC.WPFApp.Global
{
    public class ScreenKeyboard
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(String sClassName, String sAppName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, String lpszClass, String lpszWindow);

        /// <summary>
        /// Show the On Screen Keyboard
        /// </summary>
        #region ShowOSK
        public static void ShowOnScreenKeyboard()
        {
            IntPtr parent = FindWindow("Shell_TrayWnd", null);
            IntPtr child1 = FindWindowEx(parent, IntPtr.Zero, "TrayNotifyWnd", "");
            IntPtr keyboardWnd = FindWindowEx(child1, IntPtr.Zero, "TIPBand", "Touch keyboard");

            uint WM_LBUTTONDOWN = 0x0201;
            uint WM_LBUTTONUP = 0x0202;
            UIntPtr x = new UIntPtr(0x01);
            UIntPtr x1 = new UIntPtr(0x0);
            IntPtr y = new IntPtr(0x0100020);
            PostMessage(keyboardWnd, WM_LBUTTONDOWN, x, y);
            PostMessage(keyboardWnd, WM_LBUTTONUP, x1, y);
        }
        #endregion ShowOSK

        /// <summary>
        /// Hide the On Screen Keyboard
        /// </summary>
        #region HideOSK
        public static void HideOnScreenKeyboard()
        {
            uint WM_SYSCOMMAND = 0x0112;
            UIntPtr SC_CLOSE = new UIntPtr(0xF060);
            IntPtr y = new IntPtr(0);
            IntPtr KeyboardWnd = FindWindow("IPTip_Main_Window", null);
            PostMessage(KeyboardWnd, WM_SYSCOMMAND, SC_CLOSE, y);
        }
        #endregion HideOSK
    }
}
