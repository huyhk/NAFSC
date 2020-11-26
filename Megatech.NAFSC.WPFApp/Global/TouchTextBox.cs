using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Megatech.NAFSC.WPFApp.Controls
{
    public class TouchEnabledTextBox : TextBox
    {
        public TouchEnabledTextBox()
        {
            this.GotTouchCapture += TouchEnabledTextBox_GotTouchCapture;
            this.LostFocus += TouchEnabledTextBox_LostFocus;
            this.GotFocus += TouchEnabledTextBox_GotFocus;
        }

        private void TouchEnabledTextBox_GotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ShowKeyboard();
        }

        private void TouchEnabledTextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            ShowKeyboard();
        }

        private void TouchEnabledTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ShowKeyboard();
        }

        private void ShowKeyboard()
        {
            string touchKeyboardPath =
              @"C:\Program Files\Common Files\Microsoft Shared\Ink\TabTip.exe";
            _touchKeyboardProcess = Process.Start(touchKeyboardPath);
        }

        //added field
        private Process _touchKeyboardProcess = null;

        //replace Process.Start line from the previous listing with
       
        private void TouchEnabledTextBox_GotTouchCapture(
           object sender,
           System.Windows.Input.TouchEventArgs e)
        {
            ShowKeyboard();
        }
       

        private void TouchEnabledTextBox_LostFocus(object sender, RoutedEventArgs eventArgs)
        {
            if (_touchKeyboardProcess != null && !_touchKeyboardProcess.HasExited)
            {
                _touchKeyboardProcess.Kill();
                //nullify the instance pointing to the now-invalid process
                _touchKeyboardProcess = null;
            }
        }
    }
}
