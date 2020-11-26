using Megatech.FMS.WebAPI.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Megatech.NAFSC.WPFApp.UC
{
    /// <summary>
    /// Interaction logic for UCRefuelDetail.xaml
    /// </summary>
    public partial class UCRefuelDetail : UserControl
    {
        public UCRefuelDetail()
        {
            InitializeComponent();
            //LoadData();
        }

        DataRepository repo = DataRepository.GetInstance();
        protected void LoadData()
        {

            ICollection<AirlineViewModel> airlines = repo.GetAirlines();
            cboAirline.ItemsSource = airlines;
        }
        private RefuelViewModel _model;
        public void SetDataSource(RefuelViewModel model)
        {
            ICollection<AirlineViewModel> airlines = repo.GetAirlines();
            cboAirline.ItemsSource = airlines;
            _model = model;
          
            DataContext = _model;
            this.lvItems.ItemsSource = _model.Others;
        }

        private void TextBox_GotKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }
    }
}
