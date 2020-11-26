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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Megatech.NAFSC.WPFApp.UC
{
    /// <summary>
    /// Interaction logic for UCDocPreviewSkypec.xaml
    /// </summary>
    public partial class UCDocPreviewSkypec : UserControl
    {
        public UCDocPreviewSkypec()
        {
            InitializeComponent();
        }

        public void SetDataSource(FlightViewModel model)
        {
            DataContext = model;
            lvItems.ItemsSource = model.RefuelItems;
        }
    }
}
