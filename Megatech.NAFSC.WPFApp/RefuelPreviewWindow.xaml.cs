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
            SetEnable();
        }
        private RefuelViewModel model;
        private void LoadData()
        {
            model.PropertyChanged += Model_PropertyChanged;
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
                db.CancelInvoice(model.InvoiceId);
                model.InvoiceId = 0;
                model.Printed = false;
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
            if (!model.Printed) {               
            
                InvoiceWindow wnd = new InvoiceWindow(model.Volume);
               
                if (wnd.ShowDialog().Value)
                {
                    var option = wnd.Model;
                    //var model = (RefuelViewModel)ucDetail.DataContext;
                    InvoiceViewModel invoice = new InvoiceViewModel(model, option);

                    var inv = db.PostInvoice(invoice);
                    if (inv != null)
                    {
                        model.Printed = true;
                        invoiceId = inv.Id;
                        model.InvoiceId = inv.Id;
                    }
                   
                }
            }
            if (invoiceId > 0)
            {
                SetEnable();
                PrintPreview preview = new PrintPreview();
                preview.SetDataSource(invoiceId);
                preview.ShowDialog();
            }
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
    }
}
