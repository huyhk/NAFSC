using Megatech.NAFSC.WPFApp.Global;
using Osklib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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


namespace Megatech.NAFSC.WPFApp
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        

        public InputDialog()
        {
            InitializeComponent();
            txtValue.Focus();
        }

        public InputDialog(InputScope scopeName, string value, string title):this()
        {
            if (((InputScopeName)scopeName.Names[0]).NameValue == InputScopeNameValue.Number && value.EndsWith("%"))
                value = value.Replace("%", "");
            txtValue.InputScope = scopeName;
            _text = value;            
            txtValue.Text = _text;
            txtValue.Focus();
            txtValue.SelectAll();
            OnScreenKeyboard.Show();
            this.Title = title;
            lblTitle.Text = title;
        }
        private string _text;
        public T GetValue<T>()
        {
            try
            {
                return (T)Convert.ChangeType(_text, typeof(T));
                
            }
            catch
            {
                return default(T);
            }
        }
        
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            
            _text = txtValue.Text;
            DialogResult = true; Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            
            DialogResult = true; Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            OnScreenKeyboard.Close();
            base.OnClosing(e);
        }
    }
}
