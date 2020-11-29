using Megatech.FMS.WebAPI.Models;
using System;
using System.Collections;
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
using FMS.Data;
using Megatech.NAFSC.WPFApp.Data;

namespace Megatech.NAFSC.WPFApp.UC
{
    /// <summary>
    /// Interaction logic for UCRefuelList.xaml
    /// </summary>
    public partial class UCRefuelList : UserControl
    {
        private DataRepository _db = DataRepository.GetInstance();
        public UCRefuelList()
        {
            InitializeComponent();
        }
        
        public void SetDataSource(IEnumerable dataSource)
        {
            lvRefuelList.ItemsSource = dataSource;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvRefuelList.ItemsSource);
            if (view != null)
                view.Filter = FlightFilter;
            //if (lvRefuelList.Items.Count > 0)
            //{
            //    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvRefuelList.ItemsSource);
            //    PropertyGroupDescription groupDescription = new PropertyGroupDescription("FlightId");
            //    view.GroupDescriptions.Add(groupDescription);
            //}

        }
        private string _filter = string.Empty;
        private bool FlightFilter(object item)
        {
            if (String.IsNullOrEmpty(_filter))
                return true;
            else
                return ((item as RefuelViewModel).FlightCode.IndexOf(_filter, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public void SetFilter(string text)
        {
            _filter = text;
            CollectionViewSource.GetDefaultView(lvRefuelList.ItemsSource).Refresh();
        }
        protected void ListView_ItemClicked(object sender, EventArgs args)
        {
            var item = (ListViewItem)sender;
            

           
                
                RefuelViewModel model = (RefuelViewModel)item.DataContext;

            

                if (model != null)
                {
                    if (model.Status != REFUEL_ITEM_STATUS.DONE)
                    {
                    SelectUserWindow su = new SelectUserWindow();
                    su.ShowDialog();
                    if (su.DialogResult.Value)
                    {
                        model.OperatorId = su.OperatorId;
                        model.DriverId = su.DriverId;
                        model.Status = REFUEL_ITEM_STATUS.PROCESSING;
                        _db.PostRefuel(model);

                        RefuelWindow refuelwd = new RefuelWindow((RefuelViewModel)item.DataContext);
                        Window wnd = Window.GetWindow(this);

                        refuelwd.ShowDialog();
                        if (wnd is MainWindow)
                        {
                            (wnd as MainWindow).LoadData();
                        }
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
