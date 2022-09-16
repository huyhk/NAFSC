using Megatech.NAFSC.WPFApp.Data;
using Megatech.NAFSC.WPFApp.Global;
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
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private DataRepository repo = DataRepository.GetInstance();
        private void LoadData()
        {
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            cboPort.ItemsSource = ports;
            cboTruck.ItemsSource = repo.GetTrucks();

            if (string.IsNullOrEmpty(AppSetting.CurrentSetting.ComPort))
                cboPort.SelectedValue = 0;
            else
                cboPort.SelectedValue = AppSetting.CurrentSetting.ComPort;

            if (string.IsNullOrEmpty(AppSetting.CurrentSetting.TruckNo))
                cboTruck.SelectedValue = 0;
            else
                cboTruck.SelectedValue = AppSetting.CurrentSetting.TruckNo;

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var setting = AppSetting.CurrentSetting;
            if (cboPort.Items.Count > 0)
                setting.ComPort = cboPort.SelectedValue.ToString();
            if (cboTruck.Items.Count > 0)
                setting.TruckNo = cboTruck.SelectedValue.ToString();
            setting.Save();
            DialogResult = true;
            Close();
        }
    }
}
