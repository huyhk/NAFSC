using Megatech.NAFSC.WPFApp.Global;
using Megatech.NAFSC.WPFApp.Models;
using Osklib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using WPFTabTip;

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
            
            TabTipAutomation.IgnoreHardwareKeyboard = HardwareKeyboardIgnoreOptions.IgnoreAll;
            TabTipAutomation.BindTo<TextBox>();
            txtValue.Focus();
        }
        bool isParkingLot = false, isDateTime = false, isRoute = false;
        ICollection<AirportModel> airports;
        public InputDialog(InputScope scopeName, string value, string title):this()
        {
            InputScopeNameValue nameValue = ((InputScopeName)scopeName.Names[0]).NameValue;
            if (nameValue == InputScopeNameValue.Number && value.EndsWith("%"))
                value = value.Replace("%", "");
   
            if (nameValue == InputScopeNameValue.Time)
            {
                isDateTime = true;
                timePicker.Visibility = Visibility.Visible;
                txtValue.Visibility = Visibility.Collapsed;

                timePicker.Value = DateTime.ParseExact(value, "HH:mm", CultureInfo.CurrentCulture);
            }

            _text = value;

            if (title == FindResource("parking_lot").ToString())
            {
                isParkingLot = true;
                
                RadioList.Visibility = Visibility.Visible;
                if (value.EndsWith("L"))
                {
                    radioL.IsChecked = true;
                    radioR.IsChecked = false;
                }
                else
                {
                    radioL.IsChecked = false;
                    radioR.IsChecked = true;
                }
                value = value.Remove(value.Length - 2, 2);
            }
            else
                RadioList.Visibility = Visibility.Collapsed;

            if (title == FindResource("route_name").ToString())
            {
                isRoute = true;
                RadioInter.Visibility = Visibility.Visible;

                radioO.IsChecked = value.EndsWith("/1");
                radioD.IsChecked = !value.EndsWith("/1");

                value = value.Remove(value.Length - 2, 2);
                value = value.Replace(lblRoute.Content.ToString(), "");
                lblRoute.Visibility = Visibility.Visible;

            }
            else
            {
                RadioInter.Visibility = Visibility.Collapsed;
            }

            txtValue.InputScope = scopeName;
            
                       
            txtValue.Text = value;
            txtValue.Focus();
            txtValue.SelectAll();
            //OnScreenKeyboard.Show();
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
            if (isParkingLot)
                _text = txtValue.Text + "/" + ((bool)radioL.IsChecked ? "L" : "R");
            //MessageBox.Show(_text + (radioL.IsChecked.ToString()));
            if (isDateTime)
                _text = timePicker.Text;

            if (isRoute)
                _text = lblRoute.Content.ToString() + txtValue.Text.Trim() + ((bool)radioO.IsChecked ? "/1" : "/0");
            DialogResult = true;
            //OnScreenKeyboard.Close();
            Close();
        }

        private void txtValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isRoute)
            {
                var val = txtValue.Text;
                bool isInternational = !AirportModel.Check(val);
                radioD.IsChecked = !isInternational;
                radioO.IsChecked = isInternational;
            } 

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //OnScreenKeyboard.Close();
            DialogResult = false; Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //TabTipAutomation.Unbind<TextBox>();
            //OnScreenKeyboard.Close();
            base.OnClosing(e);
        }
    }
}
