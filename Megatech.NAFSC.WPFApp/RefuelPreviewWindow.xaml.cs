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
        }
        private RefuelViewModel model;
        private void LoadData()
        {
            model.PropertyChanged += Model_PropertyChanged;
            this.ucDetail.SetDataSource(model);

            btnCancel.Visibility = model.Printed ? Visibility.Visible : Visibility.Hidden;
            //btnInvoice.Visibility = !model.Printed ? Visibility.Visible : Visibility.Hidden;
            btnInvoice.Content = FindResource(model.Printed ? "view_invoice": "create_invoice").ToString();

            ucDetail.SetReadOnly(model.Printed);
            isChanged = false;
            btnSave.IsEnabled = false;
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

            repo.PostRefuel(this.model);
            btnSave.IsEnabled = false;
            isChanged = false;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Visibility = Visibility.Collapsed;
            btnSave.Visibility = Visibility.Visible;
        }

        private DataRepository db = DataRepository.GetInstance();
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(FindResource("cancel_invoice_confirm").ToString(), FindResource("confirm").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                db.CancelInvoice(model.InvoiceId);
                model.Printed = false;
                LoadData();
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

            InvoiceWindow wnd = new InvoiceWindow();
            if (wnd.ShowDialog().Value)
            {
                var option = wnd.Model;
                //var model = (RefuelViewModel)ucDetail.DataContext;
                InvoiceViewModel invoice = new InvoiceViewModel(model, option);
                var inv = db.PostInvoice(invoice);
                model.Printed = true;
                
                LoadData();
                PrintPreview preview = new PrintPreview();
                preview.SetDataSource(invoice);
                preview.ShowDialog();
            }
        }
    }
}
