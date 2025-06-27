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
    public partial class UCInvoiceList : UserControl
    {
        private DataRepository _db = DataRepository.GetInstance();
        public UCInvoiceList()
        {
            InitializeComponent();
        }

        public void SetDataSource(IEnumerable dataSource)
        {
            lvInvoiceList.ItemsSource = dataSource;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvInvoiceList.ItemsSource);
           

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
            CollectionViewSource.GetDefaultView(lvInvoiceList.ItemsSource).Refresh();
        }
        protected void ListView_ItemClicked(object sender, EventArgs args)
        {
            var item = (ListViewItem)sender;
            InvoiceViewModel model = (InvoiceViewModel)item.DataContext;

            if (model != null)
            {

                if (model.VendorModelCode == Vendor.PA.ToString())
                {
                    PrintPreview preview = new PrintPreview();

                    preview.SetDataSource(model.Id, model.LocalGuid);
                    preview.ShowDialog();
                }
                else
                {
                    SkypecPrintPreview preview = new SkypecPrintPreview();

                    preview.SetDataSource(model.Id, model.LocalGuid);
                    preview.ShowDialog();
                }
                //if (model.StReatus != REFUEL_ITEM_STATUS.DONE)
                //{
                //    SelectUserWindow su = new SelectUserWindow(model);
                //    su.ShowDialog();
                //    if (su.DialogResult.Value)
                //    {
                //        model.OperatorId = su.OperatorId;
                //        model.DriverId = su.DriverId;
                //        model.Status = REFUEL_ITEM_STATUS.PROCESSING;
                //        _db.PostRefuel(model);

                //        RefuelWindow refuelwd = new RefuelWindow((RefuelViewModel)item.DataContext);
                //        Window wnd = Window.GetWindow(this);

                //        refuelwd.ShowDialog();
                //        if (wnd is MainWindow)
                //        {
                //            (wnd as MainWindow).LoadData();
                //        }

                //        //refuelwd.ShowDialog();
                //    }

                //}

                //else
                //{
                //    RefuelPreviewWindow refuelwd = new RefuelPreviewWindow((RefuelViewModel)item.DataContext);

                //    refuelwd.ShowDialog();
                //}
            }
        }
    }
}
