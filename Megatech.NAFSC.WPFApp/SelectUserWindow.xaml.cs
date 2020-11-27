using Megatech.FMS.WebAPI.Models;
using Megatech.NAFSC.WPFApp.Data;
using Megatech.NAFSC.WPFApp.Helpers;
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
    /// Interaction logic for SelectUserWindow.xaml
    /// </summary>
    public partial class SelectUserWindow : Window
    {
        public SelectUserWindow(bool exit) : this()
        {
            this.exit = exit;
        }

        public SelectUserWindow()
        {
            InitializeComponent();
            LoadData();
        }
        private DataRepository _db = DataRepository.GetInstance();
        private bool exit = false;
        private void LoadData()
        {
            var lstUser = _db.GetUsers();
            cboDriver.ItemsSource = cboOperator.ItemsSource = lstUser;
            
            var firstUser = lstUser.FirstOrDefault();
            if (firstUser != null)
            {
                //cboDriver.SelectedIndex = cboOperator.SelectedIndex = 0;
                DriverId = OperatorId = firstUser.Id;
            }
        }

        public int DriverId { get; set; }

        public int OperatorId { get; set; }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        
    }
}
