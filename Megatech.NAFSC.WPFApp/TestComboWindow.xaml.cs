using Megatech.FMS.WebAPI.Models;
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
    /// Interaction logic for TestComboWindow.xaml
    /// </summary>
    public partial class TestComboWindow : Window
    {
        public TestComboWindow()
        {
            InitializeComponent();

            this.cboUser.ItemsSource = UserViewModel.CreateList();
        }
    }
}
