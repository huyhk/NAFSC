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
using Megatech.NAFSC.WPFApp.Helpers;

namespace Megatech.NAFSC.WPFApp.UC
{
    /// <summary>
    /// Interaction logic for UCEMRMeter.xaml
    /// </summary>
    public partial class UCEMRMeter : UserControl
    {
        public UCEMRMeter()
        {
            InitializeComponent();
        }

        internal void SetDataSource(EMRData ermData)
        {
            this.DataContext = null;
            this.DataContext = ermData;
            
        }
    }
}
