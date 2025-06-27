using Megatech.FMS.WebAPI.Models;
using Megatech.NAFSC.WPFApp.Models;
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
using WPFTabTip;

namespace Megatech.NAFSC.WPFApp
{
    /// <summary>
    /// Interaction logic for InvoiceWindow.xaml
    /// </summary>
    public partial class InvoiceWindow : Window
    {
        public InvoiceWindow()
        {
            InitializeComponent();
            TabTipAutomation.IgnoreHardwareKeyboard = HardwareKeyboardIgnoreOptions.IgnoreAll;
            TabTipAutomation.BindTo<TextBox>();
        }
        public InvoiceWindow(decimal maxSplit) : this()
        {
            _maxSplit = maxSplit;
        }
        public InvoiceWindow(decimal maxSplit, bool autoNum ) : this()
        {
            _maxSplit = maxSplit;
            _autoNum = autoNum;
            chkauto.IsChecked = _autoNum;
            chkauto.Visibility = _autoNum ? Visibility.Visible : Visibility.Collapsed;
            if (_autoNum)
            {
                grid.RowDefinitions[1].Height = new GridLength(0);
                grid.RowDefinitions[4].Height = new GridLength(0);
            }
        }
        private decimal _maxSplit;
        private bool _autoNum = false;
        public InvoiceOption Model { get; set; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Model = (InvoiceOption)this.DataContext;

            if (Validate())
            {
                DialogResult = true;
                this.Close();
            }

        }

        private void CheckBoxChanged(object sender, EventArgs e)
        {
            if (chkauto.IsChecked ?? false)
                _autoNum = true;
            else _autoNum = false;
            if (_autoNum)
            {
                grid.RowDefinitions[1].Height = new GridLength(0);
                grid.RowDefinitions[4].Height = new GridLength(0);
            }
            else
            {
                grid.RowDefinitions[1].Height = new GridLength(1.0, GridUnitType.Auto);
                grid.RowDefinitions[4].Height = new GridLength(1.0, GridUnitType.Auto);
            }
        }
        private bool Validate()
        {
            var isValid = true;
            string msg = string.Empty;
            if (string.IsNullOrEmpty(Model.InvoiceNumber) && !Model.AutoNumber)
            {
                isValid = false;
                msg = "Nhập số hóa đơn";
            }
            else if (Model.Split)
            {
                if (Model.SplitAmount <=0)
                {
                    isValid = false;
                    msg = "Nhập số lượng cần tách";
                }
                else if (Model.SplitAmount>= _maxSplit)
                {
                    isValid = false;
                    msg = "Số lượng cần tách phải nhỏ hơn " + _maxSplit.ToString("#,##0");
                }
                else if (string.IsNullOrEmpty(Model.InvoiceNumber2) && !Model.AutoNumber)
                {
                    isValid = false;
                    msg = "Nhập số hóa đơn thứ 2";
                }
            }
           
            if (!isValid)
            {
                MessageBox.Show(msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            

            return isValid;
        }
    }
}
