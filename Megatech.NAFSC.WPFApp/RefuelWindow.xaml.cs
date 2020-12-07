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
            this.item = _db.GetRefuel(item.Id);
            if (this.item == null)
                this.item = item;
           
            LoadData();
        }

      

      

        private DataRepository _db = DataRepository.GetInstance();
        private RefuelViewModel item;

        private void LoadData()
        {

            erm = new ErmHelper(AppSetting.CurrentSetting.ComPort);
            erm.StatusChanged += Erm_StatusChanged;
            erm.Open();
            if (erm.IsError)
            {
                btnStart.IsEnabled = false;
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
                
                btn.Content = FindResource("starting");
                erm.Start();
                tmr.Start();
                btn.Content = FindResource("end");
                    
                ((RefuelViewModel)this.DataContext).StartTime = DateTime.Now;

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
                btn.Content = FindResource("stopping");                
                erm.End();
                tmr.Stop();
                started = false;
                btn.Content = FindResource("start");                
                ((RefuelViewModel)this.DataContext).EndTime = DateTime.Now;
                StopRefuel();
            }
        }
        private bool allowClose = true;
        private void StopRefuel()
        {            
            ermData = erm.CurrentData;
            RefuelViewModel model = (RefuelViewModel )this.DataContext;
            model.RealAmount = (decimal)ermData.Volume;
            model.StartNumber = (decimal)ermData.StartMeter;
            model.EndNumber = (decimal)ermData.EndMeter;
            model.ManualTemperature = (decimal)ermData.Temperature;
            model.Status = REFUEL_ITEM_STATUS.DONE;
            model.RefuelTime = model.EndTime;
            _db.PostRefuel(model);
            allowClose = true;
            Close();    
            RefuelPreviewWindow preview = new RefuelPreviewWindow(model);
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

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }
    }
}
