using Megatech.NAFSC.WPFApp.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Megatech.NAFSC.WPFApp
{
    public class BaseWindow: Window
    {
        public AppSetting Setting { get; set; }

        public bool IsLoggedIn()
        {
            return true;
        }
    }
}
