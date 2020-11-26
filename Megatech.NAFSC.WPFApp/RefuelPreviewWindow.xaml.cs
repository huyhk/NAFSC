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
            
           
        }

        private bool isChanged = false;

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            isChanged = true;
            btnSave.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
            PrintDialog dlg = new PrintDialog();
            FlowDocument doc = new FlowDocument(new Paragraph(new Run(model.FlightCode)));
            doc.Name = "NAFSC";
            IDocumentPaginatorSource idp = doc;
            dlg.PrintDocument(idp.DocumentPaginator, "First Print");
            */
            if (isChanged)
            {
                if (MessageBox.Show(FindResource("data_change_confirm").ToString(), FindResource("data_change_title").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
                else SaveChanges();
            }

            PrintPreview prw = new PrintPreview();

            prw.SetDataSource(new FlightViewModel((RefuelViewModel)ucDetail.DataContext));
            prw.ShowDialog();

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
    }
}
