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
        }
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

        private bool Validate()
        {
            var isValid = false;
            string msg = string.Empty;
            if (string.IsNullOrEmpty(Model.InvoiceNumber))
            {
                msg = "Nhập số hóa đơn";
            }
            else if (Model.Split)
            {
                if (Model.SplitAmount <=0)
                {
                    msg = "Nhập số lượng cần tách";
                }
                if (string.IsNullOrEmpty(Model.InvoiceNumber2))
                {
                    msg = "Nhập số hóa đơn thứ 2";
                }
            }
            else isValid = true;
            if (!isValid)
            {
                MessageBox.Show(msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            

            return isValid;
        }
    }
}
