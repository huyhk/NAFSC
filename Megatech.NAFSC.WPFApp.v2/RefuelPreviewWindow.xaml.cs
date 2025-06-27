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
using System.Windows.Shapes;
using System.ComponentModel;
using FMS.Data;
using Megatech.NAFSC.WPFApp.Global;

namespace Megatech.NAFSC.WPFApp
{
    /// <summary>
    /// Interaction logic for RefuelPreviewWindow.xaml
    /// </summary>
    public partial class RefuelPreviewWindow : Window
    {
        private DataRepository repo = DataRepository.GetInstance();

        public RefuelPreviewWindow():this(null)
        { }
        public RefuelPreviewWindow(RefuelViewModel model)
        {
            InitializeComponent();
            this.model = repo.GetRefuel(model.Id);
            if (this.model == null)
                this.model = model;
            LoadData();
            SetDataSource(this.model);
            
        }
        public void SetDataSource(RefuelViewModel model)
        {
            this.model = model;
            model.PropertyChanged += Model_PropertyChanged;
            SetEnable();
        }
        private RefuelViewModel model;
        private void LoadData()
        {
            
            this.ucDetail.SetDataSource(model);            
            isChanged = false;
            btnSave.IsEnabled = false;
        }

        private void SetEnable()
        {
            btnCancel.Visibility = model.Printed ? Visibility.Visible : Visibility.Collapsed;
            btnInvoice.Content = FindResource(model.Printed ? "view_invoice" : "create_invoice").ToString();
            ucDetail.SetReadOnly(model.Printed);
        }
        private bool isChanged = false;

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            isChanged = true;
            btnSave.IsEnabled = true;
            if (e.PropertyName.Equals("DENSITY", StringComparison.InvariantCultureIgnoreCase)
                || e.PropertyName.Equals("VOLUME", StringComparison.InvariantCultureIgnoreCase))
                ((RefuelViewModel)sender).CalculateWeight();
        }
              

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
        }

        private void SaveChanges()
        {
            foreach (BindingExpressionBase be in BindingOperations.GetSourceUpdatingBindings(ucDetail))
            {
                be.UpdateSource();
            }
            
            var updated = repo.PostRefuel(this.model);
            if (updated != null)
            {
                updated.PropertyChanged += Model_PropertyChanged;
                this.model = updated;
            }
            btnSave.IsEnabled = false;
            isChanged = false;
            ucDetail.DataContext = this.model;
        }

       

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private DataRepository db = DataRepository.GetInstance();
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(FindResource("cancel_invoice_confirm").ToString(), FindResource("confirm").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (model.InvoiceId == 0 && model.InvoiceGuid != Guid.Empty)
                    repo.CancelInvoice(model.InvoiceGuid);
                else
                {
                    db.CancelInvoice(model.InvoiceId);
                }
                    model.InvoiceId = 0;
                    model.InvoiceGuid = Guid.Empty;
                    model.Printed = false;
                    db.PostRefuel(model);
                
                SetEnable();
            }
        }
        private void btnInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (isChanged)
            {
                if (MessageBox.Show(FindResource("data_change_confirm").ToString(), FindResource("data_change_title").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
                else SaveChanges();
            }
            int invoiceId = model.InvoiceId;
            Guid invoiceGuid = model.InvoiceGuid;
            if (!model.Printed)
            {

                if (!IsError())
                {
                    InvoiceWindow wnd = new InvoiceWindow(model.Volume, model.VendorModelCode != "PA");

                    if (wnd.ShowDialog().Value)
                    {
                        var option = wnd.Model;
                        //var model = (RefuelViewModel)ucDetail.DataContext;
                        InvoiceViewModel invoice = new InvoiceViewModel(model, option);

                        var inv = db.PostInvoice(invoice, invoice.Vendor != Vendor.SKYPEC);
                        if (inv != null)
                        {
                            model.Printed = true;
                            invoiceId = inv.Id;
                            invoiceGuid = inv.LocalGuid;
                            model.InvoiceId = inv.Id;

                        }

                        PreviewInvoice(invoiceId, invoiceGuid);
                    }
                    else
                    {

                      

                    }

                }


            }
            else
            {
                PreviewInvoice(invoiceId, invoiceGuid);

            }
            
        }

        private void PreviewInvoice(int invoiceId, Guid invoiceGuid)
        {
            if (invoiceId > 0 || invoiceGuid != Guid.Empty)
            {
                SetEnable();
                if (model.Airline.VendorModelCode == Vendor.PA.ToString())
                {
                    PrintPreview preview = new PrintPreview();

                    preview.SetDataSource(invoiceId, invoiceGuid);
                    preview.ShowDialog();
                }
                else
                {
                    SkypecPrintPreview preview = new SkypecPrintPreview();

                    preview.SetDataSource(invoiceId, invoiceGuid);
                    preview.ShowDialog();
                }
            }
        }

        private bool IsError()
        {
            var msg = "";
            bool isError = false;
            if (isError = model.AirlineId<=0)
                msg = FindResource("invalid_data_airline").ToString();
            else if (isError = String.IsNullOrEmpty(model.AircraftType))
                msg = FindResource("invalid_data_aircraft_type").ToString();
            else if (isError = string.IsNullOrEmpty(model.AircraftCode))
                msg = FindResource("invalid_data_aircraft_code").ToString();
            else if (isError = string.IsNullOrEmpty(model.QualityNo))
                msg = FindResource("invalid_data_qc_no").ToString();
            else if (isError = model.RealAmount<=0)
                msg = FindResource("invalid_data_real_amount").ToString();
            else if (isError = model.Temperature <= 0)
                msg = FindResource("invalid_data_temperature").ToString();
            else if (isError = model.Density <= 0)
                msg = FindResource("invalid_data_density").ToString();
            if (isError)
                MessageBox.Show(msg, FindResource("invalid_data").ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);


            //throw new Exception(msg);
            return isError;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (isChanged)
            {
                var dlg = MessageBox.Show(FindResource("data_change_confirm").ToString(), FindResource("data_change_title").ToString(), MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (dlg == MessageBoxResult.Yes)

                    SaveChanges();
                else if (dlg == MessageBoxResult.Cancel)
                    e.Cancel = true;
            }
            base.OnClosing(e);
        }

        private void btnNewRefuel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(FindResource("new_refuel_msg").ToString(), FindResource("confirm").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var newModel = model.Copy();
                
                if (newModel != null)
                {
                    SelectUserWindow su = new SelectUserWindow(newModel);
                    su.ShowDialog();
                    if (su.DialogResult.Value)
                    {
                        newModel.OperatorId = su.OperatorId;
                        newModel.DriverId = su.DriverId;
                        newModel.RealAmount = 0;
                        newModel.Weight = 0;
                        
                        newModel.Status = REFUEL_ITEM_STATUS.PROCESSING;
                        var respItem = db.PostRefuel(newModel);
                        if (respItem != null)
                        {
                            newModel.Id = respItem.Id;
                            RefuelWindow refuelwd = new RefuelWindow(newModel);
                            Window wnd = Window.GetWindow(this);

                            refuelwd.ShowDialog();
                            if (wnd is MainWindow)
                            {
                                (wnd as MainWindow).LoadData();
                            }
                        }
                    }
                    Close();
                }
            }
        }
    }
}
