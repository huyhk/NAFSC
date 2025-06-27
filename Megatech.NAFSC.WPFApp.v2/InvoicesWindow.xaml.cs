using Megatech.NAFSC.WPFApp.Data;
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
    /// Interaction logic for InvoicesWindow.xaml
    /// </summary>
    public partial class InvoicesWindow : Window
    {
        public InvoicesWindow()
        {
            InitializeComponent();
            LoadData();
        }
        private DataRepository repo = DataRepository.GetInstance();

        private void LoadData()
        {
            var data = repo.GetInvoices(false);
            ucInvoiceList.SetDataSource(data);
        }
    }
}
