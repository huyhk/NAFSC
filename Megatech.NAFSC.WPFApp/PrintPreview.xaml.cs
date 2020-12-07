using Megatech.FMS.WebAPI.Models;
using Megatech.NAFSC.WPFApp.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
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
    /// Interaction logic for PrintPreview.xaml
    /// </summary>
    public partial class PrintPreview : Window
    {
        public PrintPreview()
        {
            InitializeComponent();
        }

        private FlowDocument docPreview;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintText();
        }

        private void Print()
        {

            PrintDialog dlg = new PrintDialog();


            var paginator = (docPreview as IDocumentPaginatorSource).DocumentPaginator;
            paginator.PageSize = new System.Windows.Size(96 * 5.5, 96 * 8.2);

            if (dlg.ShowDialog() == true)
            {
                HideUIElement(docPreview);


                dlg.PrintDocument(paginator, "FMSInvoice");

                this.Close();
            }
        }
        private Font printFont;
        private string[] textToPrint;
        private void PrintText()
        {
            try
            {
                textToPrint = BuildTextPrint(tabCtl.SelectedIndex == 0 ? model : model.ChildInvoice);
                if (textToPrint != null)
                {
                    printFont = new Font("Arial", 9);
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += new PrintPageEventHandler
                       (this.pd_PrintPage);

                    pd.Print();
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(FindResource("printer_error_msg").ToString(), FindResource("printer_error").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {

            }
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = 0;// ev.MarginBounds.Left;
            float topMargin = 5;// ev.MarginBounds.Top - 40;
            string line = null;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
               printFont.GetHeight(ev.Graphics);
            int i = 0;
            
            // Print each line of the file.
            while (count < linesPerPage && i < textToPrint.Length)
            {
                line = textToPrint[i++];

                yPos = topMargin + (count *
                   (printFont.GetHeight(ev.Graphics)+5));
                ev.Graphics.DrawString(line, printFont, System.Drawing.Brushes.Black,
                   leftMargin, yPos, new StringFormat());
                count++;
            }

            // If more lines exist, print another page.
            
                ev.HasMorePages = false;
        }

        private string[] BuildTextPrint(InvoiceViewModel model)
        {
            if (model != null)
            {
                if (model.Vendor == Vendor.SKYPEC)
                    return BuildTextPrintSkypec(model);
                else
                    return BuildTextPrintPA(model);
            }
            else return null;
        }

        private string[] BuildTextPrintSkypec(InvoiceViewModel model)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 1; i++)
                builder.AppendLine();
            builder.Append(new string(' ',20));
            builder.AppendFormat("{0:dd}", model.RefuelTime);
            builder.Append(new string(' ', 20));
            builder.AppendFormat("{0:MM}", model.RefuelTime);
            builder.Append(new string(' ', 16));
            builder.AppendFormat("{0:yyyy}", model.RefuelTime);
            for(int i=0;i<3;i++)
                builder.AppendLine();
            builder.Append(new string(' ', 50));
            builder.AppendFormat("{0}\n", model.QualityNo);
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0:HH:mm}", model.StartTime);
            builder.Append(new string(' ', 20));
            builder.AppendFormat("{0:HH:mm}", model.EndTime);
            builder.Append(new string(' ', 50));
            builder.AppendFormat("{0}\n", model.ProductName);
            for (int i = 0; i < 1; i++)
                builder.AppendLine();
            foreach (var item in model.Items)
            {
                builder.Append(new string(' ', 5));
                builder.AppendFormat("{0}", item.TruckNo);
                builder.Append(new string(' ', 30));
                builder.AppendFormat("{0:#,0}", item.StartNumber);
                builder.Append(new string(' ', 10));
                builder.AppendFormat("{0:#,0}", item.EndNumber);
                builder.Append(new string(' ', 30));
                builder.AppendFormat("{0:#,0}\n", item.Volume);
            }

            for (int i = 0; i < 2 - model.Items.Count; i++)
                builder.AppendLine();
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}\n", model.CustomerName);
            for (int i = 0; i < 1; i++)
                builder.AppendLine();
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.AircraftType);
            builder.Append(new string(' ', 65));
            builder.AppendFormat("{0}\n", model.AircraftCode);

            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.FlightCode);
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}\n", model.RouteName);

            builder.Append(new string(' ', 65));
            builder.AppendFormat("{0:#,0.00}", model.Temperature);
            builder.Append(new string(' ', 55));
            builder.AppendFormat("{0:#,0.0000}\n", model.Density);

            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0:#,0}", model.Volume);
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0:#,0.00}\n", model.Weight);

            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0:#,0.00}", model.Gallon);
            builder.Append(new string(' ', 45));
            builder.AppendFormat(model.Currency == Currency.USD ? "{0:#,0.00}\n" : "{0:#,0}\n", model.Price);

            builder.Append(new string(' ', 50));
            builder.AppendFormat(model.Currency == Currency.USD?"{0:#,0.00}\n": "{0:#,0}\n", model.Subtotal);

            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0:P0}", model.TaxRate);
            builder.Append(new string(' ', 45));
            builder.AppendFormat(model.Currency == Currency.USD ? "{0:#,0.00}\n" : "{0:#,0}\n", model.Tax);


            builder.Append(new string(' ', 45));
            builder.AppendFormat(model.Currency == Currency.USD ? "{0:#,0.00}\n" : "{0:#,0}\n", model.TotalAmount);

            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}\n", model.InWords);
            return builder.ToString().Split(new char[] { '\r', '\n' });
        }

        private string[] BuildTextPrintPA(InvoiceViewModel model)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 1; i++)
                builder.AppendLine();
            builder.Append(new string(' ', 20));
            builder.AppendFormat("{0:dd}", model.RefuelTime);
            builder.Append(new string(' ', 20));
            builder.AppendFormat("{0:MM}", model.RefuelTime);
            builder.Append(new string(' ', 16));
            builder.AppendFormat("{0:yyyy}", model.RefuelTime);
            for (int i = 0; i < 5; i++)
                builder.AppendLine();
            builder.Append(new string(' ', 50));
            builder.AppendFormat("{0}", model.QualityNo);
            builder.Append(new string(' ', 50));
            builder.AppendFormat("{0}\n", model.ProductName);
            builder.Append(new string(' ', 50));
            builder.AppendFormat("{0}\n", model.ParkingLot);
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0:HH:mm} / {1:HH:mm}\n", model.StartTime, model.EndTime);
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}\n", model.CustomerName);

            for (int i = 0; i <1; i++)
                builder.AppendLine();
            foreach (var item in model.Items)
            {
                builder.Append(new string(' ', 5));
                builder.AppendFormat("{0}", item.TruckNo);
                builder.Append(new string(' ', 30));
                builder.AppendFormat("{0:#,0}", item.StartNumber);
                builder.Append(new string(' ', 10));
                builder.AppendFormat("{0:#,0}", item.EndNumber);
                builder.Append(new string(' ', 30));
                builder.AppendFormat("{0:#,0}\n", item.Volume);
            }

            for (int i = 0; i < 3 - model.Items.Count; i++)
                builder.AppendLine();
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.AircraftType);
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}\n", model.AircraftCode);
            
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.FlightCode);
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}\n", model.RouteName);

            builder.Append(new string(' ', 65));
            builder.AppendFormat("{0:#,0.00}", model.Temperature);
            builder.Append(new string(' ', 55));
            builder.AppendFormat("{0:#,0.0000}\n", model.Density);

            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0:#,0}", model.Volume);
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0:#,0.00}\n", model.Weight);

            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0:#,0.00}\n", model.Gallon);
            for (int i = 0; i < 7; i++)
                builder.AppendLine();
            builder.Append(new string(' ', 15));
            builder.AppendFormat("{0}", model.OperatorName);
            return builder.ToString().Split(new char[] { '\r', '\n' });
        }
        private InvoiceViewModel model;
        private DataRepository db = DataRepository.GetInstance();
        public void SetDataSource(int invoiceId)
        {
            var label = FindResource("invoice_number").ToString();
            model = db.GetInvoice(invoiceId);
            ucPreview.SetDataSource(model);
            (tabCtl.Items[0] as TabItem).Header = label + ": " + model.InvoiceNumber;
            if (model.ChildInvoice != null)
            {
            
                ucPreview2.SetDataSource(model.ChildInvoice);
                (tabCtl.Items[1] as TabItem).Header = label +": "+model.ChildInvoice.InvoiceNumber;
            }
            else
                (tabCtl.Items[1] as TabItem).Visibility = Visibility.Collapsed;


        }
        private void HideUIElement(FlowDocument doc)
        {
            foreach (var item in doc.Blocks)
            {
                HideUIElement(item);
            }
            
                

            
        }

        private void HideUIElement(Block block)
        {
            if (block is Table)
            {
                
                foreach (var rowGroup in ((Table)block).RowGroups)
                {
                    foreach (var row in rowGroup.Rows)
                    {
                        foreach (var cell in row.Cells)
                        {
                            foreach (var bl in cell.Blocks)
                                HideUIElement(bl);
                        }
                    }
                }
            }
            if (block is BlockUIContainer)
            {
                var lvItems = (block as BlockUIContainer).Child as ListView;
                if (lvItems != null)
                {
                    var v = lvItems.View as GridView;
                    Style style = new Style();
                    style.Setters.Add(new Setter { Property = ContentControl.VisibilityProperty, Value = Visibility.Hidden });
                    v.ColumnHeaderContainerStyle = style;
                }
            }
            if (block is Paragraph)
            {
                var children = ((Paragraph)block).Inlines
                    .OfType<InlineUIContainer>()
                    
                    .Select(x => x.Child as UIElement);
                foreach (var item in children)
                {
                    if (item is TextBlock)
                        item.Visibility = Visibility.Hidden;

                }
            }
                        
        }
    }
}
