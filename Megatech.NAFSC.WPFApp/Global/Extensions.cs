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

        public static T GetInput<T>(this Window window, InputScope scopeName, string text, string title)

        {
            InputDialog dlg = new InputDialog(scopeName,  text, title);

            dlg.ShowDialog();
            
                return dlg.GetValue<T>();
            
        }
    }
}
