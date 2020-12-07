using Megatech.FMS.WebAPI.Models;
using Megatech.NAFSC.WPFApp.Controls;
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
using Megatech.NAFSC.WPFApp.Global;

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
            //TextBox tb = (TextBox)sender;
            //tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }

        internal void SetReadOnly(bool printed)
        {
            foreach (var item in this.grid.Children.OfType<TouchEnabledTextBox>())
            {
                item.IsReadOnly = printed;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            Window wnd = Window.GetWindow(txt);
            var inputScope = txt.InputScope ?? new InputScope { Names = { new InputScopeName(InputScopeNameValue.Default) } };
            
            
                var name = (InputScopeName)inputScope.Names[0];
                if (name.NameValue.HasFlag(InputScopeNameValue.Number))
                    txt.Text = wnd.GetInput<decimal>(inputScope, txt.Text, txt.Tag.ToString()).ToString();
                else
                    txt.Text = wnd.GetInput<string>(inputScope, txt.Text,txt.Tag.ToString()).ToString();
            
        }
    }
}
