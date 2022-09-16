using FMS.Data;
using Megatech.FMS.WebAPI.Models;
using Megatech.NAFSC.WPFApp.Data;
using Megatech.NAFSC.WPFApp.Global;
using Megatech.NAFSC.WPFApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.ComponentModel;

namespace Megatech.NAFSC.WPFApp
{
    /// <summary>
    /// Interaction logic for RefuelWindow.xaml
    /// </summary>
    public partial class RefuelWindow : Window
    {
        
        private ErmHelper erm = new ErmHelper(AppSetting.CurrentSetting.ComPort);

        public RefuelWindow()
        {
            InitializeComponent();
            tmr = new Timer(1000);
            tmr.Elapsed += Tmr_Elapsed;
        }

    

        public RefuelWindow(RefuelViewModel item):this()
        {
            if (item != null)
                this.item = _db.GetRefuel(item.Id);
            if (this.item == null)
                this.item = item;
            
            LoadData();
        }

      

      

        private DataRepository _db = DataRepository.GetInstance();
        private RefuelViewModel item;

        private void LoadData()
        {
            
            // new item
            if (this.item == null)
            {
                this.item = new RefuelViewModel();
                item.ArrivalTime = DateTime.Now.AddMinutes(-30);
                item.DepartureTime= DateTime.Now.AddMinutes(30);
            }
            this.item.PropertyChanged += Item_PropertyChanged;
            erm = new ErmHelper(AppSetting.CurrentSetting.ComPort);
            erm.StatusChanged += Erm_StatusChanged;
            erm.Open();
            if (erm.IsError)
            {
                bool isDebugMode = false;
#if DEBUG
                isDebugMode = true;
#endif
                btnStart.IsEnabled = isDebugMode;
                MessageBox.Show(FindResource("device_error_msg").ToString(), FindResource("device_error_title").ToString(),MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ucMeter.SetDataSource(erm.CurrentData);
            this.DataContext = item;

            this.cboAirline.ItemsSource = _db.GetAirlines();
            var lstUser = _db.GetUsers();
            cboDriver.ItemsSource = cboOperator.ItemsSource = lstUser;

            cboDriver.SelectedValue = item.DriverId;
            cboOperator.SelectedValue = item.OperatorId;

        }
        bool isModified = false;
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            isModified = true;
            if (e.PropertyName.Equals("DENSITY", StringComparison.InvariantCultureIgnoreCase)
                || e.PropertyName.Equals("VOLUME", StringComparison.InvariantCultureIgnoreCase))
                ((RefuelViewModel)sender).CalculateWeight();
        }

        private void Erm_StatusChanged(EMRSTATUS status)
        {
            if (status == EMRSTATUS.STARTED)
            {
                started = true;
                UpdateMeter();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "btnBack")
                this.Close();

        }
        private Timer tmr = new Timer();
        private bool started = false;
        
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (!started)
            {
                if (Validate())
                {
                    if (MessageBox.Show(FindResource("starting_confirm").ToString(), FindResource("confirm_title").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        btn.Content = FindResource("starting");

                        btn.Content = FindResource("end");
                        btnBack.IsEnabled = false;
                        StartRefuel();
                    }
                }
                //btnBack.IsEnabled = false;
                //allowClose = false;
            }
            else
            {
                if (erm.CurrentData.Rate > 0.001)
                {
                    MessageBox.Show(FindResource("processing_alert").ToString(), FindResource("processing_alert_title").ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (MessageBox.Show(FindResource("stopping_confirm").ToString(), FindResource("confirm_title").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    btn.Content = FindResource("stopping");
                    erm.End();
                    tmr.Stop();
                    started = false;
                    btn.Content = FindResource("start");
                    btnBack.IsEnabled = true;
                    ((RefuelViewModel)this.DataContext).EndTime = DateTime.Now;
                    StopRefuel();
                }
            }
        }

        private void StartRefuel()
        {

            erm.Start();
            tmr.Start();
            RefuelViewModel model = (RefuelViewModel)this.DataContext;
            model.IsReadOnly = true;
            model.StartTime = DateTime.Now;
            model.Status = REFUEL_ITEM_STATUS.PROCESSING;
            var newModel = _db.PostRefuel(model);
            model.Id = newModel.Id;
            model.LocalGuid = newModel.LocalGuid;

        }

        private bool Validate()
        {
            RefuelViewModel item = (RefuelViewModel)this.DataContext;
            bool valid = false;
            string msg = "";
            if (string.IsNullOrEmpty(item.FlightCode))
            {
                msg = "invalid_data_flight_code";

            }
            else if (string.IsNullOrEmpty(item.AircraftType))
            {
                msg = "invalid_data_aircraft_type";

            }
            else if (string.IsNullOrEmpty(item.RouteName))
            {
                msg = "invalid_data_route_name";

            }
            else if (string.IsNullOrEmpty(item.AircraftCode))
            {
                msg = "invalid_data_aircraft_code";
            }
            else if (string.IsNullOrEmpty(item.ParkingLot))
            {
                msg = "invalid_data_parking_lot";
            }
            else if (item.ArrivalTime == DateTime.MinValue)
            {
                msg = "invalid_data_arrival_time";
            }
            else if (item.DepartureTime == DateTime.MinValue)
            {
                msg = "invalid_data_departure_time";
            }
            else if (item.AirlineId == 0)
                msg = "invalid_data_airline";
            else if (item.DriverId == 0)
                msg = "invalid_data_driver";
            else if (item.OperatorId == 0)
                msg = "invalid_data_operator";
            else
                valid = true;
            if (!valid)
                MessageBox.Show(FindResource(msg).ToString()  , FindResource("invalid_data").ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
            
            return valid;
        }

        private bool allowClose = true;
        private void StopRefuel()
        {            
            ermData = erm.CurrentData;
            RefuelViewModel model = (RefuelViewModel )this.DataContext;
            model.RealAmount = (decimal)ermData.Volume;
            //model.Weight = Math.Round(model.Volume * model.Density;
            model.StartNumber = (decimal)ermData.StartMeter;
            model.EndNumber = (decimal)ermData.EndMeter;
            model.ManualTemperature = (decimal)ermData.Temperature;
            model.Status = REFUEL_ITEM_STATUS.DONE;
            model.RefuelTime = model.EndTime;
            var respItem = _db.PostRefuel(model);
            allowClose = true;
            Close();    
            RefuelPreviewWindow preview = new RefuelPreviewWindow(respItem);
            preview.ShowDialog();
            
        }
        
        private void UpdateMeter()
        {
            tmr.Stop();
            erm.GetData();
            if (tmr != null)
                tmr.Start();
        }


        EMRData ermData = null;
        private void Tmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateMeter();
            //Dispatcher.BeginInvoke(new Action(()=> UpdateMeter()));
            
        }

        private void TextBox_GotKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateMeter();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!allowClose)
            {
                if (MessageBox.Show(FindResource("not_allow_closing").ToString(), FindResource("not_allow_closing_title").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                e.Cancel = true;
            }
            else
            {
                base.OnClosing(e);
                tmr.Stop();

                if (erm != null)
                    erm.Dispose();
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
                RefuelViewModel model = (RefuelViewModel)this.DataContext;
                Window wnd = Window.GetWindow(txt);
                var inputScope = txt.InputScope ?? new InputScope { Names = { new InputScopeName(InputScopeNameValue.Default) } };


                var name = (InputScopeName)inputScope.Names[0];
                var text = txt.Text;
                if (txt.Tag.ToString() == FindResource("route_name").ToString())
                    text += model.FlightType == FlightType.OVERSEA ? "/1" : "/0";

                if (name.NameValue.HasFlag(InputScopeNameValue.Number) && txt.Tag.ToString() != FindResource("parking_lot").ToString())
                    text = wnd.GetInput<decimal>(inputScope, text, txt.Tag.ToString()).ToString();
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

                        model.FlightType = text.Substring(text.Length - 1) == "1" ? FlightType.OVERSEA : FlightType.DOMESTIC;

                    }
                    else
                        txt.Text = text;

                }
            }
        }

        private void btnManual_Click(object sender, RoutedEventArgs e)
        {
            ResultWindow wnd = new ResultWindow();
            wnd.ShowDialog();
        }

        private void cboAirline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((RefuelViewModel)DataContext).Airline = (AirlineViewModel)((ComboBox)sender).SelectedItem;
        }
    }
}
