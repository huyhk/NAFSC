using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Megatech.NAFSC.WPFApp.Global
{
    public static class Extensions
    {

        public static InputValue<T> GetInput<T>(this Window window, InputScope scopeName, string text, string title)

        {
            InputDialog dlg = new InputDialog(scopeName,  text, title);

            if ((bool)dlg.ShowDialog())

                return new InputValue<T> { Cancelled = false, Value = dlg.GetValue<T>() };

            else return new InputValue<T> { Cancelled = true };
        }
    }

    public class InputValue<T>
    {
        public bool Cancelled { get; set; } = false;
        public T Value { get; set; }
        public override string ToString()
        {
            if (Cancelled)
                return null;
            else return Value.ToString();
        }
    }
}
