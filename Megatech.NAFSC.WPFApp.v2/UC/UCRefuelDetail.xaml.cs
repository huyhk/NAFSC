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
using FMS.Data;

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

            var lstUser = repo.GetUsers();
            cboDriver.ItemsSource = cboOperator.ItemsSource = lstUser;

            cboDriver.SelectedValue = _model.DriverId;
            cboOperator.SelectedValue = _model.OperatorId;

        }
        private RefuelViewModel _model;
        private List<RefuelViewModel> _allItems;
        public void SetDataSource(RefuelViewModel model)
        {
            ICollection<AirlineViewModel> airlines = repo.GetAirlines();
            cboAirline.ItemsSource = airlines;
            
                var lstUser = repo.GetUsers();
            cboDriver.ItemsSource = cboOperator.ItemsSource = lstUser;

            _model = model;
          
            DataContext = _model;
            _allItems = new List<RefuelViewModel>();
            
            _allItems.Insert(0, _model);
            if (_model.Others != null)
                _allItems.AddRange(_model.Others);
            this.lvItems.ItemsSource = _allItems;
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

            foreach (var item in this.grid.Children.OfType<TextBox>())
            {
                item.IsReadOnly = printed;
            }
            foreach (var item in this.grid.Children.OfType<ComboBox>())
            {
                item.IsEditable = !printed;
                item.IsHitTestVisible = !printed;
            }

        }

        private void TextBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ShowInput(sender);
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {

            ShowInput(sender);
        }

        private void ShowInput(object sender)
        {
            TextBox txt = (TextBox)sender;
            txt.Dispatcher.BeginInvoke(new Action(() => txt.SelectAll()));
            if (!txt.IsReadOnly)
            {
                Window wnd = Window.GetWindow(txt);
                var inputScope = txt.InputScope ?? new InputScope { Names = { new InputScopeName(InputScopeNameValue.Default) } };
                RefuelViewModel model = (RefuelViewModel)this.DataContext;

                var name = (InputScopeName)inputScope.Names[0];
                var text = txt.Text;

                if (txt.Tag.ToString() == FindResource("route_name").ToString())
                    text += model.FlightType == FlightType.OVERSEA ? "/1" : "/0";

                if (name.NameValue.HasFlag(InputScopeNameValue.Number) && txt.Tag.ToString() != FindResource("parking_lot").ToString())
                {
                    text = wnd.GetInput<decimal>(inputScope, text, txt.Tag.ToString()).ToString();
                }
                else
                    text = wnd.GetInput<string>(inputScope, text, txt.Tag.ToString()).ToString();

                if (text != null)
                {

                    if (txt.Tag.ToString() == FindResource("parking_lot").ToString() && txt.Text != "")
                    {

                        model.ParkingLot = text.Substring(0, text.LastIndexOf("/"));

                        model.ValvePit = (ValvePit)Enum.Parse(typeof(ValvePit), text.Substring(txt.Text.Length - 1));

                    }
                    else if (txt.Tag.ToString() == FindResource("route_name").ToString() && text != "")
                    {

                        model.RouteName = text.Substring(0, text.LastIndexOf("/"));

                        model.FlightType = text.EndsWith("/1") ? FlightType.OVERSEA : FlightType.DOMESTIC;

                    }
                    else if (txt.Tag.ToString() == FindResource("weight").ToString() ||
                        txt.Tag.ToString() == FindResource("real_amount").ToString())
                    {
                        decimal num = 0M;
                        if (decimal.TryParse(text, out num))
                        {
                            text = Math.Round(num, 0, MidpointRounding.AwayFromZero).ToString();
                            txt.Text = text;
                        }
                    }
                    else
                        txt.Text = text;

                }
            }
        }

        private void cboAirline_Selected(object sender, RoutedEventArgs e)
        {
            _model = (RefuelViewModel)DataContext;
            _model.Airline = (AirlineViewModel)((ComboBox)sender).SelectedItem;
            _model.Price = _model.Airline.Price;
        }

        private void lvItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (RefuelViewModel) ((ListView)sender).SelectedItem;
            if (selected != null)
            {
                DataContext = selected;
                SetReadOnly(selected.Printed);
                var parent = Window.GetWindow(this);
                if (parent is RefuelPreviewWindow)
                {
                    ((RefuelPreviewWindow)parent).SetDataSource(selected);
                }
            }

        }
    }
}
