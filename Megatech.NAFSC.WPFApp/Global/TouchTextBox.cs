using Osklib;
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
            //this.GotTouchCapture += TouchEnabledTextBox_GotTouchCapture;
            this.GotFocus += TouchEnabledTextBox_GotFocus;
            //this.GotKeyboardFocus += TouchEnabledTextBox_GotKeyboardFocus;
            //this.GotMouseCapture += TouchEnabledTextBox_GotMouseCapture;
            this.LostFocus += TouchEnabledTextBox_LostFocus;
        }

        private void TouchEnabledTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            HideKeyboard();
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
            if (!IsReadOnly)
                OnScreenKeyboard.Show();
        }
        private void HideKeyboard()
        {

            OnScreenKeyboard.Close();
        }

        private void TouchEnabledTextBox_GotTouchCapture(
           object sender,
           System.Windows.Input.TouchEventArgs e)
        {
            ShowKeyboard();
        }
       

       
    }
}
