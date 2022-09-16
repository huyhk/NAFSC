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
    /// Interaction logic for UCDocPriview.xaml
    /// </summary>
    public partial class UCDocPriview : UserControl
    {
        public UCDocPriview()
        {
            InitializeComponent();
        }
        public void SetDataSource(InvoiceViewModel model)
        {
            SetDataSource(model, 0);
        }
        public void SetDataSource(InvoiceViewModel model, int template)
        {
            switch (template)
            {
                case 2:
                    docSkypec.Visibility = Visibility.Collapsed;
                    docSkypecNoPrice.Visibility = Visibility.Collapsed;
                    docPA.Visibility = Visibility.Visible;
                    docPA.SetDataSource(model);
                    break;
                case 1:
                    docPA.Visibility = Visibility.Collapsed;
                    docSkypecNoPrice.Visibility = Visibility.Collapsed;
                    docSkypec.Visibility = Visibility.Visible;
                    docSkypec.SetDataSource(model);
                    break;
                case 0:
                    docPA.Visibility = Visibility.Collapsed;
                    docSkypec.Visibility = Visibility.Collapsed;
                    docSkypecNoPrice.Visibility = Visibility.Visible;
                    docSkypecNoPrice.SetDataSource(model);
                    break;
            }
            
        }
    }
}
