using FMS.Data;
using Megatech.FMS.WebAPI.Models;
using Megatech.NAFSC.WPFApp.Data;
using Megatech.NAFSC.WPFApp.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Megatech.NAFSC.WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowLogin();
        }

        public void ShowLogin()
        {
            if (!AppSetting.CurrentSetting.IsLoggedIn())
            {
                LoginWindow login = new LoginWindow();
                if (!login.ShowDialog().Value)
                {
                    Close();
                    return;
                }
            }

            if (AppSetting.CurrentSetting.IsFirstUse())
            {
                SettingWindow setting = new SettingWindow();
                if (!setting.ShowDialog().Value)
                {
                    Close();
                    return;
                }
            }
            this.Title = FindResource("app_name").ToString() + " - " + AppSetting.CurrentSetting.TruckNo;
            LoadData();
            tmr.Interval = new TimeSpan(0, 0, 1, 0);
            tmr.Tick += Tmr_Tick;
            tmr.Start();
            dtPicker.SelectedDate = DateTime.Today;
        }

        private void Tmr_Tick(object sender, EventArgs e)
        {
            UpdateData();
        }

        DispatcherTimer tmr = new DispatcherTimer();
        private DataRepository repo = DataRepository.GetInstance();
        private ICollection<RefuelViewModel> _refuelList, _filteredList;
        public void LoadData()
        {
            _refuelList = repo.GetRefuelList(AppSetting.CurrentSetting.TruckNo);
            _filteredList = _refuelList;

            //lvRefuelList.ItemsSource = _filteredList;
            ucActiveRefuelList.SetDataSource (_filteredList);
        }

        private void TabItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UpdateData();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(FindResource("exit_confirmation_msg").ToString(),FindResource("exit_confirmation_title").ToString(),MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                AppSetting.Logout();

                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow setting = new SettingWindow();
            setting.ShowDialog();    
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateData();

        }

        private void UpdateData()
        {
            tmr.Stop();
            LoadData();
            tmr.Start();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            var filterText = txtFilter.Text;
            Filter(filterText);
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SelectUserWindow();
            dlg.ShowDialog();
        }

        private void Filter(string text)
        {
            _filteredList = _refuelList.Where(r => r.FlightCode.Contains(text)).ToList();
            ucActiveRefuelList.SetDataSource(_filteredList);

        }

        protected void ListView_ItemClicked(object sender, EventArgs args)
        {

            var item = (ListViewItem)sender;
            SelectUserWindow su = new SelectUserWindow();
            su.ShowDialog();
            if (su.DialogResult.Value)
            {
            }



            RefuelViewModel model = (RefuelViewModel)item.DataContext;



            if (model != null)
            {
                if (model.Status != REFUEL_ITEM_STATUS.DONE)
                {
                    RefuelWindow refuelwd = new RefuelWindow((RefuelViewModel)item.DataContext);
                    Window wnd = Window.GetWindow(this);

                    refuelwd.ShowDialog();
                    if (wnd is MainWindow)
                    {
                        (wnd as MainWindow).LoadData();
                    }
                }
                else
                {
                    RefuelPreviewWindow refuelwd = new RefuelPreviewWindow((RefuelViewModel)item.DataContext);

                    refuelwd.ShowDialog();
                }

            }


        }
    }
}
