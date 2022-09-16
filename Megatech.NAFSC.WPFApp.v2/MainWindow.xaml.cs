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
            //tmr.Interval = new TimeSpan(0, 0, 1, 0);
            //tmr.Tick += Tmr_Tick;
            //tmr.Start();
            dtPicker.SelectedDate = DateTime.Today;
        }

        private void Tmr_Tick(object sender, EventArgs e)
        {
            UpdateData();
        }

        DispatcherTimer tmr = new DispatcherTimer();
        private DataRepository repo = DataRepository.GetInstance();
        private ICollection<RefuelViewModel> _refuelList, _filteredList;
        private DateTime _date = DateTime.Today;
        public void LoadData()
        {
            _refuelList = repo.GetRefuelList(_date.ToString("yyyyMMdd"));
            //_filteredList = _refuelList.Take(pageSize).ToList();

            //lvRefuelList.ItemsSource = _filteredList;
            //ucActiveRefuelList.SetDataSource(_filteredList);
            if (_refuelList != null)
                Navigate((int)PagingMode.First);
        }

        private void TabItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UpdateData();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(FindResource("exit_confirmation_msg").ToString(), FindResource("exit_confirmation_title").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
            if (dtPicker.SelectedDate.HasValue)
            {
                _date = dtPicker.SelectedDate.Value;
                LoadData();
            }

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

            RefuelViewModel model = (RefuelViewModel)item.DataContext;
            SelectUserWindow su = new SelectUserWindow(model);
            
            su.ShowDialog();
            if (su.DialogResult.Value)
            {
            }
            

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

        int page = 1;
        private int pageSize = 10;
        private int pageCount = 1;
        //To check the paging direction according to use selection.
        private void Navigate(int mode)
        {
            int count = 1;
            int from = 1; int to = 10;
            pageCount = (int)Math.Ceiling((decimal)_refuelList.Count / pageSize);
            switch (mode)
            {
                case (int)PagingMode.Next:
                    btnPrev.IsEnabled = true;
                    btnFirst.IsEnabled = true;
                    if (_refuelList.Count >= (page * pageSize))
                    {
                        if (_refuelList.Skip(page * pageSize).Take(pageSize).Count() == 0)
                        {

                            //_filteredList = _refuelList.Skip((page * pageSize) - pageSize).Take(pageSize).ToList();
                            //count = (page * pageSize) + (_refuelList.Skip(page * pageSize).Take(pageSize)).Count();
                        }
                        else
                        {
                            //_filteredList = _refuelList.Skip(page * pageSize).Take(pageSize).ToList();
                            //count = (page * pageSize) + (_refuelList.Skip(page * pageSize).Take(pageSize)).Count();
                            page++;
                        }


                    }

                    if (page >= pageCount)
                    {
                        btnNext.IsEnabled = false;
                        btnLast.IsEnabled = false;
                    }

                    break;
                case (int)PagingMode.Previous:
                    btnNext.IsEnabled = true;
                    btnLast.IsEnabled = true;
                    if (page > 1)
                    {
                        page -= 1;

                        if (page == 1)
                        {
                            //_filteredList = _refuelList.Take(pageSize).ToList();


                        }
                        else
                        {
                            //_filteredList = _refuelList.Skip(page * pageSize).Take(pageSize).ToList();


                        }
                    }
                    if (page <= 1)
                    {
                        btnPrev.IsEnabled = false;
                        btnFirst.IsEnabled = false;
                    }
                    break;

                case (int)PagingMode.First:
                    page = 2;
                    Navigate((int)PagingMode.Previous);
                    break;
                case (int)PagingMode.Last:
                    page = (_refuelList.Count / pageSize);
                    Navigate((int)PagingMode.Next);
                    break;

            }
            _filteredList = _refuelList.Skip((page * pageSize) - pageSize).Take(pageSize).ToList();
            from = (page - 1) * pageSize + 1; to = Math.Min(_refuelList.Count, from + pageSize - 1);
            lblpageInformation.Content = from + " to " + to + " of " + _refuelList.Count;
            ucActiveRefuelList.SetDataSource(_filteredList);
        }

        private void btnFirst_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.First);
        }

        private void btnNext_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.Next);

        }

        private void btnPrev_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.Previous);

        }



        private void btnLast_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.Last);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            RefuelWindow wnd = new RefuelWindow(null);
            wnd.ShowDialog();
            this.LoadData();
        }
    }

    enum PagingMode
    {
        First,
        Next,
        Previous,
        Last
    }
}
