using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FMS.Data;
using Megatech.FMS.WebAPI.Models;
using Megatech.NAFSC.WPFApp.Data;
using Megatech.NAFSC.WPFApp.Global;
using Megatech.NAFSC.WPFApp.Helpers;
namespace Megatech.NAFSC.WPFApp
{
    /// <summary>
    /// Interaction logic for ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        public ResultWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var start = txtStart.Text;
            var end = txtEnd.Text;
            if (!string.IsNullOrEmpty(end) && !string.IsNullOrEmpty(start))
            {
                txtVolume.Text = (decimal.Parse(end) - decimal.Parse(start)).ToString();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            txt.Dispatcher.BeginInvoke(new Action(() => txt.SelectAll()));
            if (!txt.IsReadOnly)
            {
                Window wnd = Window.GetWindow(txt);
                var inputScope = txt.InputScope ?? new InputScope { Names = { new InputScopeName(InputScopeNameValue.Default) } };


                var name = (InputScopeName)inputScope.Names[0];
                var text = txt.Text;
                if (name.NameValue.HasFlag(InputScopeNameValue.Number) && txt.Tag.ToString() != FindResource("parking_lot").ToString())
                    text = wnd.GetInput<decimal>(inputScope, txt.Text, txt.Tag.ToString()).ToString();
                else
                    text = wnd.GetInput<string>(inputScope, txt.Text, txt.Tag.ToString()).ToString();

                if (text!=null)
                    txt.Text = text;





            }
        }
    }
}
