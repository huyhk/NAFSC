
using Megatech.FMS.WebAPI.Models;
using Megatech.NAFSC.WPFApp.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TestTMU590;
using VNS.Utils;

namespace Megatech.NAFSC.WPFApp
{
    /// <summary>
    /// Interaction logic for PrintPreview.xaml
    /// </summary>
    public partial class SkypecPrintPreview : Window
    {
        public SkypecPrintPreview()
        {
            InitializeComponent();
            //AddFonts();
        }

        //private void AddFonts()
        //{
        //    List<string> fonts = new List<string>();
        //    foreach (System.Drawing.FontFamily font in System.Drawing.FontFamily.Families)
        //    {
        //        fonts.Add(font.Name);
        //    }

        //    cboFont.ItemsSource = fonts;
        //}

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

        private int print_template = 0;
        private Font printFont;
        private string[] textToPrint;
        bool isV2 = false;
        private void PrintText()
        {
            try
            {
                textToPrint = BuildTextPrint(tabCtl.SelectedIndex == 0 ? model : model.ChildInvoice);
                
                if (textToPrint != null)
                {
                   
                    printFont = new Font("Consolas", 10);
                    PrintDocument pd = new PrintDocument();
                    
                    if (isV2)
                        pd.PrintPage += new PrintPageEventHandler
                    (this.pd_PrintPageV2);

                    else
                        pd.PrintPage += new PrintPageEventHandler
                       (this.pd_PrintPage);
                    
                    //pd.QueryPageSettings += Pd_QueryPageSettings;

                    
                    //pd.BeginPrint += Pd_BeginPrint;
                    pd.Print();
                    
                    //new PrintHelper().PrintText(textToPrint);
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
        
        private float topMargin = 0;
        private float lineSpacing = 0;
        private float nextSpace = 0;
        private void Pd_BeginPrint(object sender, PrintEventArgs e)
        {
            
        }

        private void Pd_QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
            e.PageSettings.Margins.Right = 0;
        }
        private string ESC = new string (new char[] {(char)27});

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = 0;// ev.MarginBounds.Left;
                                 // float topMargin = 0;// ev.MarginBounds.Top - 40;
            string line = null;
            ev.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height / printFont.GetHeight(ev.Graphics);
            int i = 0;

            // Print each line of the file.
            while (count < linesPerPage && i < textToPrint.Length)
            {
                line = textToPrint[i++];
                int margin = 0;
                if (Regex.IsMatch(line, @"(?s)%(-?\d+)%.*"))
                {
                    var match = Regex.Match(line, @"(?s)%(-?\d+)%(.*)");
                    margin = int.Parse(match.Groups[1].Value);
                    line = match.Groups[2].Value;
                }

                yPos = topMargin + margin + (count * (printFont.GetHeight(ev.Graphics) + lineSpacing));
                ev.Graphics.DrawString(line, printFont, System.Drawing.Brushes.Black,
                   leftMargin, yPos, new StringFormat());
                count++;
            }

            // If more lines exist, print another page.

            ev.HasMorePages = false;
        }

        private void pd_PrintPageV2(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = 0;// ev.MarginBounds.Left;
           // float topMargin = 0;// ev.MarginBounds.Top - 40;
            string line = null;
            ev.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height / printFont.GetHeight(ev.Graphics);
            int i = 0;
            
            // Print each line of the file.
            while (count < linesPerPage && i < textToPrint.Length)
            {
                line = textToPrint[i++];
       
                if (Regex.IsMatch(line, @"(?s)#(-?\d+)#.*"))
                {
                    var match = Regex.Match(line, @"(?s)#(-?\d+)#(.*)");
                    nextSpace += int.Parse(match.Groups[1].Value);
                    line = match.Groups[2].Value;
                }

                if (Regex.IsMatch(line, @"(?s)%(-?\d+)%.*"))
                {
                    var match = Regex.Match(line, @"(?s)%(-?\d+)%(.*)");
                    lineSpacing = int.Parse(match.Groups[1].Value);
                    line = match.Groups[2].Value;
                }
                nextSpace += lineSpacing;
                yPos = topMargin + nextSpace + (count * (printFont.GetHeight(ev.Graphics)) ) ;
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
                //if (model.Vendor == Vendor.SKYPEC)
                //    return BuildTextPrintSkypec(model);
                //else
                //    return BuildTextPrintPA(model);
                switch (print_template)
                {
                    case 2:
                        return BuildTextPrintPA(model);

                    //case 1:
                    //    return BuildSkypecInvoiceV2(model);

                    //case 0:
                    //    return BuildSkypecBillV2(model);
                    //default:
                    //    return BuildSkypecBillV2(model);
                    default:
                        return BuildPrinText(model);

                }
            }
            else return null;
        }

        private string[] BuildTextPrintSkypec(InvoiceViewModel model)
        {
            if (print_template ==1) {
                return BuildSkypecInvoice( model);
            }
            else
                return BuildSkypecBill( model);
        }
        public string[] BuildPrinText(InvoiceViewModel model)
        {

            StringBuilder builder = new StringBuilder();
            builder.Append("The Seller: " + model.SellerName + "\n");
            builder.Append("Tax code: " + model.SellerTaxCode + "\n");
            builder.Append("Address: " + model.SellerAddress + "\n");
            builder.Append("------------------------------------------------------------------\n");
            builder.Append("                    FUEL DELIVERY RECEIPT                         \n");
            builder.Append("                    PHIẾU GIAO NHIÊN LIỆU                         \n");
            builder.Append("------------------------------------------------------------------\n");
            builder.Append(String.Format("No.: {0}            {1:dd/MM/yyyy}\n", model.InvoiceNumber, model.EndTime));
            builder.Append(String.Format("Buyer: {0}\n", model.CustomerName));
            builder.Append("\n");
            builder.Append(String.Format("A/C Type         : {0,-16} A/C reg     : {1}\n", model.AircraftType, model.AircraftCode));
            builder.Append(String.Format("Flight No.       : {0,-16} Route       : {1}\n", model.FlightCode, model.RouteName));
            builder.Append(String.Format("Cert No.         : {0,-16} Product Name: {1}\n", model.QualityNo, model.ProductName));
            builder.Append(String.Format("Start Time       : {0:HH:mm dd/MM/yyyy} End Time    : {1:HH:mm dd/MM/yyyy}\n", model.StartTime, model.EndTime));
            builder.Append("Refueling Method : FHS" + "\n");
            builder.Append("------------------------------------------------------------------\n");
            builder.Append("| # |  Refueler No.     |   Temp.   |   USG  |  Litter |    Kg   |\n");
            builder.Append("|   | Start/End Meter   | Density   |        |         |         |\n");
            builder.Append("------------------------------------------------------------------\n");
            int i = 1;
            foreach (var itemModel in model.Items)
            {
                builder.Append(String.Format("|{0,2} |{1,-19}|{2,8:#.00} oC|{3,8:#}|{4,9:#}|{5,9:#}|\n", i++, itemModel.TruckNo, itemModel.Temperature, itemModel.Gallon, itemModel.Volume, itemModel.Weight));
                builder.Append(String.Format("|   |{0,9:#}/{0,-9:#}|{2,6:#.0000} kg/l|        |         |         |\n", itemModel.StartNumber, itemModel.EndNumber, itemModel.Density));
                builder.Append("------------------------------------------------------------------\n");
            }
            builder.Append(String.Format("|   | Total                         |{0,8:#}|{1,9:#}|{2,9:#}|\n", model.Gallon, model.Volume, model.Weight));
            builder.Append("------------------------------------------------------------------\n");
            builder.Append("           Buyer                            Seller        \n");
            builder.Append("  (Signature and full name)      (Signature and full name)     \n");
            return builder.ToString().Split(new char[] { '\n' });

        }
        private string[] BuildSkypecBill(InvoiceViewModel model)
        {
            topMargin = 1;
            lineSpacing = 2;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 3; i++)
                builder.Append("\n");
            builder.Append(new string(' ', 20));
            builder.AppendFormat("{0:dd}", model.RefuelTime);
            builder.Append(new string(' ', 20));
            builder.AppendFormat("{0:MM}", model.RefuelTime);
            builder.Append(new string(' ', 20));
            builder.AppendFormat("{0:yyyy}", model.RefuelTime);
            for (int i = 0; i <= 4; i++)
                builder.Append("\n");
            builder.Append("%-5%");
            builder.Append(new string(' ', 60));
            builder.AppendFormat("{0}\n", model.QualityNo);
            builder.Append("%-10%");
            builder.Append(new string(' ', 50));
            builder.AppendFormat("{0:HH:mm}", model.StartTime);
            builder.Append(new string(' ', 5));
            builder.AppendFormat("{0:HH:mm}", model.EndTime);
            builder.Append(new string(' ', 50));
            builder.AppendFormat("{0}\n", model.ProductName);
            for (int i = 0; i < 2; i++)
                builder.Append("\n");
            foreach (var item in model.Items)
            {
                builder.Append(new string(' ', 5));
                builder.AppendFormat("{0}", item.TruckNo);
                builder.Append(new string(' ', 20));
                builder.AppendFormat("{0:#,0}", item.StartNumber);
                builder.Append(new string(' ', 15));
                builder.AppendFormat("{0:#,0}", item.EndNumber);
                builder.Append(new string(' ', 20));
                builder.AppendFormat("{0:#,0}\n", item.Volume);
            }

            for (int i = 0; i < 5 - model.Items.Count; i++)
                builder.Append("\n");

            builder.Append("%-10%");
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.CustomerName);
            //for (int i = 0; i <= 1; i++)
            //    builder.Append("\n");
            builder.Append("%-8%");
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.TaxCode);
            builder.Append("%-6%");
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.Address);
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.AircraftType);
            builder.Append(new string(' ', 75));
            builder.AppendFormat("{0}\n", model.AircraftCode);

            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.FlightCode);
            builder.Append(new string(' ', 55));
            builder.AppendFormat("{0}\n", model.RouteName);

            builder.Append(new string(' ', 55));
            builder.AppendFormat("{0:#,0}", model.Gallon);
            builder.Append(new string(' ', 65));
            builder.AppendFormat("{0:#,0}\n", model.Volume);

            builder.Append(new string(' ', 55));
            builder.AppendFormat("{0:#,0.00}", model.Temperature);
            builder.Append(new string(' ', 60));
            builder.AppendFormat("{0:#,0.0000}\n", model.Density);

           

            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0:#,0}", model.Weight);
            
            //builder.AppendFormat("{0}\n", model.InWords);
            return builder.ToString().Split(new char[] { '\r', '\n' });
        }
        private string[] BuildSkypecBillV2(InvoiceViewModel model)
        {
            isV2 = true;
            topMargin = 1;
            
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 3; i++)
                builder.Append("\n");
            builder.Append("#5#");
            builder.Append(new string(' ', 20));
            builder.AppendFormat("{0:dd}", model.RefuelTime);
            builder.Append(new string(' ', 21));
            builder.AppendFormat("{0:MM}", model.RefuelTime);
            builder.Append(new string(' ', 20));
            builder.AppendFormat("{0:yyyy}", model.RefuelTime);
            for (int i = 0; i <= 4; i++)
                builder.Append("\n");
            builder.Append("#7#");
            builder.Append(new string(' ', 62));
            builder.AppendFormat("{0}\n", model.QualityNo);
            
            builder.Append(new string(' ', 52));
            builder.AppendFormat("{0:HH:mm} / {1:HH:mm}", model.StartTime, model.EndTime);
            //builder.Append(new string(' ', 5));
            //builder.AppendFormat("{0:HH:mm}", model.EndTime);
            builder.Append(new string(' ', 55));
            builder.AppendFormat("{0}\n", model.ProductName);
            for (int i = 0; i < 1; i++)
                builder.Append("\n");
            builder.Append("#8#");
            foreach (var item in model.Items)
            {
                builder.Append(new string(' ', 5));
                builder.AppendFormat("{0}", item.TruckNo);
                builder.Append(new string(' ', 20));
                builder.AppendFormat("{0:#,0}", item.StartNumber);
                builder.Append(new string(' ', 15));
                builder.AppendFormat("{0:#,0}", item.EndNumber);
                builder.Append(new string(' ', 20));
                builder.AppendFormat("{0:#,0}\n", item.Volume);
            }

            for (int i = 0; i < 6 - model.Items.Count; i++)
                builder.Append("\n");

            //builder.Append("#-10#");
            builder.Append("%-2%");
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.Names[0]);               
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.Names.Length > 1 ? model.Names[1] : "");
            

            //for (int i = 0; i <= 1; i++)
            //    builder.Append("\n");
            //builder.Append("%-8%");
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.TaxCode);
            //builder.Append("%-6%");
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.Addresses[0]);
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.Addresses.Length > 1 ? model.Addresses[1] : "");

            builder.Append("#-3#");
            builder.Append("%3%");
            //builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.AircraftType.PadLeft(60, ' '));
           // builder.Append(new string(' ', 75));
            builder.AppendFormat("{0}\n", model.AircraftCode.PadLeft(75, ' '));

            //builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.FlightCode.PadLeft(60,' '));
            //builder.Append(new string(' ', 55));
            builder.AppendFormat("{0}\n", model.RouteName.PadLeft(72,' '));

            //builder.Append(new string(' ', 55));
            builder.AppendFormat("{0}", model.Gallon.Value.ToString("#,0").PadLeft(60,' '));
            //builder.Append(new string(' ', 65));
            builder.AppendFormat("{0}\n", model.Volume.Value.ToString("#,0").PadLeft(75,' '));

            //builder.Append(new string(' ', 55));
            builder.AppendFormat("{0}", model.Temperature.ToString("#,0.00").PadLeft(60, ' '));
            //builder.Append(new string(' ', 60));
            builder.AppendFormat("{0}\n", model.Density.Value.ToString("#,0.0000").PadLeft(75, ' '));
            
            //builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}", model.Weight.Value.ToString("#,0").PadLeft(60, ' '));

            //builder.AppendFormat("{0}\n", model.InWords);
            return builder.ToString().Split(new char[] { '\r', '\n' });
        }

        private string[] BuildSkypecInvoice(InvoiceViewModel model)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 2; i++)
                builder.Append("\n");
            builder.Append(new string(' ', 25));
            builder.AppendFormat("{0:dd}", model.RefuelTime);
            builder.Append(new string(' ', 25));
            builder.AppendFormat("{0:MM}", model.RefuelTime);
            builder.Append(new string(' ', 20));
            builder.AppendFormat("{0:yyyy}", model.RefuelTime);
            for (int i = 0; i <= 5; i++)
                builder.Append("\n");
            builder.Append(new string(' ', 55));
            builder.AppendFormat("{0}\n", model.QualityNo);
            builder.Append(new string(' ', 50));
            builder.AppendFormat("{0:HH:mm}", model.StartTime);
            builder.Append(new string(' ', 15));
            builder.AppendFormat("{0:HH:mm}", model.EndTime);
            builder.Append(new string(' ', 50));
            builder.AppendFormat("{0}\n", model.ProductName);
            for (int i = 0; i < 2; i++)
                builder.Append("\n");
            foreach (var item in model.Items)
            {
                builder.Append(new string(' ', 5));
                builder.AppendFormat("{0}", item.TruckNo);
                builder.Append(new string(' ', 15));
                builder.AppendFormat("{0:#,0}", item.StartNumber);
                builder.Append(new string(' ', 15));
                builder.AppendFormat("{0:#,0}", item.EndNumber);
                builder.Append(new string(' ', 30));
                builder.AppendFormat("{0:#,0}\n", item.Volume);
            }

            for (int i = 0; i < 4 - model.Items.Count; i++)
                builder.Append("\n");
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.CustomerName);
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.TaxCode);
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.Address);
            //for (int i = 0; i <= 1; i++)
            //    builder.Append("\n");
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.AircraftType);
            builder.Append(new string(' ', 75));
            builder.AppendFormat("{0}\n", model.AircraftCode);

            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.FlightCode);
            builder.Append(new string(' ', 55));
            builder.AppendFormat("{0}\n", model.RouteName);

            builder.Append(new string(' ', 60));
            builder.AppendFormat("{0:#,0.00}", model.Temperature);
            builder.Append(new string(' ', 55));
            builder.AppendFormat("{0:#,0.0000}\n", model.Density);

            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0:#,0}", model.Volume);
            builder.Append(new string(' ', 65));
            builder.AppendFormat("{0:#,0}\n", model.Weight);

            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0:#,0}", model.Gallon);
            builder.Append(new string(' ', 55));
            builder.AppendFormat(model.Currency == Currency.USD ? "{0:#,0.00} {1}/{2}\n" : "{0:#,0} {1}/{2}\n", model.Price, model.Currency, model.Unit == Unit.KG?"KG":"USG");

            builder.Append(new string(' ', 50));
            builder.AppendFormat(model.Currency == Currency.USD ? "{0:#,0.00} {1}\n" : "{0:#,0}  {1}\n", model.Subtotal, model.Currency);

            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0:P0}", model.TaxRate);
            builder.Append(new string(' ', 65));
            builder.AppendFormat(model.Currency == Currency.USD ? "{0:#,0.00} {1}\n" : "{0:#,0} {1}\n", model.Tax, model.Currency);


            builder.Append(new string(' ', 55));
            builder.AppendFormat(model.Currency == Currency.USD ? "{0:#,0.00} {1}\n" : "{0:#,0} {1}\n", model.TotalAmount, model.Currency);

            builder.Append(new string(' ', 40));
            var words = model.InWords.Split(new char[] { ' ' });
            var c = 0;
            for (int i = 0; i < words.Length; i++)
            {
                builder.AppendFormat("{0} ", words[i]);
                c += words[i].Length + 1;
                if (c >= 50)
                {
                    builder.Append("\n" + new string(' ', 10));
                    c = 0;
                }
            }
            //builder.AppendFormat("{0}\n", model.InWords);
            return builder.ToString().Split(new char[] { '\r', '\n' });
        }

        private string[] BuildSkypecInvoiceV2(InvoiceViewModel model)
        {
            isV2 = true;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 2; i++)
                builder.Append("\n");
            
            builder.Append(new string(' ', 18));
            builder.AppendFormat("{0:dd}", model.RefuelTime);
            builder.Append(new string(' ', 25));
            builder.AppendFormat("{0:MM}", model.RefuelTime);
            builder.Append(new string(' ', 20));
            builder.AppendFormat("{0:yyyy}", model.RefuelTime);
            for (int i = 0; i <= 4; i++)
                builder.Append("\n");
            builder.Append("#8#");
            builder.Append(new string(' ', 60));
            builder.AppendFormat("{0}\n", model.QualityNo);
            builder.Append("#4#");
            builder.Append(new string(' ', 57));
            builder.AppendFormat("{0:HH:mm} / {1:HH:mm}", model.StartTime,model.EndTime);
            //builder.Append(new string(' ', 15));
            //builder.AppendFormat("{0:HH:mm}", model.EndTime);
            builder.Append(new string(' ', 54));
            builder.AppendFormat("{0}\n", model.ProductName);
            for (int i = 0; i < 2; i++)
                builder.Append("\n");
            builder.Append("#-4#");
            foreach (var item in model.Items)
            {
                builder.Append(new string(' ', 5));
                builder.AppendFormat("{0}", item.TruckNo);
                builder.Append(new string(' ', 15));
                builder.AppendFormat("{0:#,0}", item.StartNumber);
                builder.Append(new string(' ', 15));
                builder.AppendFormat("{0:#,0}", item.EndNumber);
                builder.Append(new string(' ', 30));
                builder.AppendFormat("{0:#,0}\n", item.Volume);
            }

            for (int i = 0; i < 5 - model.Items.Count; i++)
                builder.Append("\n");
            builder.Append("#-2#");
            builder.Append("%-3%");
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model. Names[0]);
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.Names.Length > 1 ? model.Names[1] : "");
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.TaxCode);
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.Addresses[0]);
            builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}\n", model.Addresses.Length > 1 ? model.Addresses[1] : "");
            //for (int i = 0; i <= 1; i++)
            //    builder.Append("\n");

            builder.Append("#-5#");
            builder.Append("%1%");

            //builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.AircraftType.PadLeft(65,' '));
            //builder.Append(new string(' ', 75));
            builder.AppendFormat("{0}\n", model.AircraftCode.PadLeft(65, ' '));

            //builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.FlightCode.PadLeft(65, ' '));
            //builder.Append(new string(' ', 55));
            builder.AppendFormat("{0}\n", model.RouteName.PadLeft(65, ' '));

            //builder.Append(new string(' ', 60));
            builder.AppendFormat("{0}", model.Temperature.ToString("#,0.00").PadLeft(65, ' '));
            //builder.Append(new string(' ', 55));
            builder.AppendFormat("{0}\n", model.Density.Value.ToString("#,0.0000").PadLeft(65, ' '));

            //builder.Append(new string(' ', 40));
            builder.AppendFormat("{0}", model.Volume.Value.ToString("#,0").PadLeft(65, ' '));
            //builder.Append(new string(' ', 65));
            builder.AppendFormat("{0}\n", model.Weight.Value.ToString("#,0").PadLeft(65, ' '));

            //builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.Gallon.Value.ToString("#,0").PadLeft(65, ' '));
            builder.Append(new string(' ', 55));
            builder.AppendFormat(model.Currency == Currency.USD ? "{0:#,0.00} {1}/{2}\n" : "{0:#,0} {1}/{2}\n", model.Price, model.Currency, model.Unit == Unit.KG ? "KG" : "USG");

            builder.Append(new string(' ', 60));
            builder.AppendFormat(model.Currency == Currency.USD ? "{0:#,0.00} {1}\n" : "{0:#,0}  {1}\n", model.Subtotal, model.Currency);

            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0:P0}", model.TaxRate);
            builder.Append(new string(' ', 65));
            builder.AppendFormat(model.Currency == Currency.USD ? "{0:#,0.00} {1}\n" : "{0:#,0} {1}\n", model.Tax, model.Currency);


            builder.Append(new string(' ', 60));
            builder.AppendFormat(model.Currency == Currency.USD ? "{0:#,0.00} {1}\n" : "{0:#,0} {1}\n", model.TotalAmount, model.Currency);

            builder.Append("%0%");
            builder.Append(new string(' ', 45));
            var words = model.InWords.Split(new char[] { ' ' });
            var c = 0;
            for (int i = 0; i < words.Length; i++)
            {
                builder.AppendFormat("{0} ", words[i]);
                c += words[i].Length + 1;
                if (c >= 50)
                {
                    builder.Append("\n" + new string(' ', 45));
                    c = 0;
                }
            }
            //builder.AppendFormat("{0}\n", model.InWords);
            return builder.ToString().Split(new char[] { '\r', '\n' });
        }

        private string[] BuildTextPrintPA(InvoiceViewModel model)
        {
            isV2 = false;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i <= 4; i++)
                builder.Append("\n");
            builder.Append(new string(' ', 20));
            builder.AppendFormat("{0:dd}", model.RefuelTime);
            builder.Append(new string(' ', 25));
            builder.AppendFormat("{0:MM}", model.RefuelTime);
            builder.Append(new string(' ', 16));
            builder.AppendFormat("{0:yyyy}", model.RefuelTime);
            for (int i = 0; i < 9; i++)
                builder.Append("\n");
            builder.Append(new string(' ', 70));
            builder.AppendFormat("{0}", model.QualityNo);
            builder.Append(new string(' ', 50));
            builder.AppendFormat("{0}\n", model.ProductName);
            builder.Append(new string(' ', 50));
            builder.AppendFormat("{0}\n", model.ParkingLot);
            builder.Append(new string(' ', 55));
            builder.AppendFormat("{0:HH:mm} / {1:HH:mm}\n", model.StartTime, model.EndTime);
            builder.Append(new string(' ', 65));
            builder.AppendFormat("{0}\n", model.CustomerName);

            for (int i = 0; i <3; i++)
                builder.Append("\n");
            foreach (var item in model.Items)
            {
                builder.Append(new string(' ', 5));
                builder.AppendFormat("{0}", item.TruckNo);
                builder.Append(new string(' ', 15));
                builder.AppendFormat("{0:#,0}", item.StartNumber);
                builder.Append(new string(' ', 10));
                builder.AppendFormat("{0:#,0}", item.EndNumber);
                builder.Append(new string(' ', 35));
                builder.AppendFormat("{0:#,0}\n", item.Volume);
            }

            for (int i = 0; i < 4 - model.Items.Count; i++)
                builder.Append("\n");
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}", model.AircraftType);
            builder.Append(new string(' ', 75));
            builder.AppendFormat("{0}\n", model.AircraftCode);
            
            builder.Append(new string(' ', 50));
            builder.AppendFormat("{0}", model.FlightCode);
            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0}\n", model.RouteName);

            builder.Append(new string(' ', 60));
            builder.AppendFormat("{0:#,0.00}", model.Temperature);
            builder.Append(new string(' ', 55));
            builder.AppendFormat("{0:#,0.0000}\n", model.Density);

            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0:#,0}", model.Volume);
            builder.Append(new string(' ', 60));
            builder.AppendFormat("{0:#,0}\n", model.Weight);

            builder.Append(new string(' ', 45));
            builder.AppendFormat("{0:#,0}\n", model.Gallon);
            for (int i = 0; i < 7; i++)
                builder.AppendLine();
            builder.Append(new string(' ', 15));
            builder.AppendFormat("{0}", model.OperatorName);
            return builder.ToString().Split(new char[] { '\r', '\n' });
        }
        private InvoiceViewModel model;
        private DataRepository db = DataRepository.GetInstance();
        public void SetDataSource(int invoiceId, Guid invoiceGuid)
        {
            var label = FindResource("invoice_number").ToString();
            
            model = db.GetInvoice(invoiceId, invoiceGuid);
            

            print_template = model.Vendor == Vendor.PA? 2 :( model.IsInternational ? 1 : 0);

            (tabCtl.Items[0] as TabItem).Header = label + ": " + model.InvoiceNumber;
            if (model.ChildInvoice != null)
            {

                
                (tabCtl.Items[1] as TabItem).Header = label + ": " + model.ChildInvoice.InvoiceNumber;
            }
            else
                (tabCtl.Items[1] as TabItem).Visibility = Visibility.Collapsed;

            SetPreviewSource();



        }

        private void SetPreviewSource()
        {
            ucPreview.SetDataSource(model);
            SetImage(0, model.ImagePath);


            if (model.ChildInvoice != null)
            {
                ucPreview2.SetDataSource(model.ChildInvoice);
                SetImage(0, model.ChildInvoice.ImagePath);

            }
            
        }
        private void SetImage(int index, string filePath)
        {
            TabItem tabItem = tabCtl.Items[index] as TabItem;
            var grid = (Grid)tabItem.Content;
            if (grid != null)
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    var img = grid.Children[1] as System.Windows.Controls.Image;
                    img.Source = GetBitmapImage(filePath);
                }
            }
        }

        private BitmapImage GetBitmapImage(string path)
        {
            if (!File.Exists(path))
                return null;
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;

                image.StreamSource = fileStream;
                image.EndInit();
                return image;
            }
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

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            print_template = int.Parse(((RadioButton)sender).Tag.ToString());
            SetPreviewSource();

        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            var file = tabCtl.SelectedIndex == 0 ? model.InvoiceNumber : model.ChildInvoice.InvoiceNumber;
            var window = new ImageCaptureWindow(file);
            window.ShowDialog();
            if (tabCtl.SelectedIndex == 0)
            {
                model.ImagePath = window.ImagePath;
                
            }
            else
                model.ChildInvoice.ImagePath = window.ImagePath;
            SetImage(tabCtl.SelectedIndex, window.ImagePath);
        }

        private void btnPost_Click(object sender, RoutedEventArgs e)
        {
            if (model.Exported)
                MessageBox.Show("Warning: This invoice is already posted");
            else if (string.IsNullOrEmpty(model.ImagePath))
                MessageBox.Show("Warning: This invoice has no image captured");
            else if (!File.Exists(model.ImagePath))
                MessageBox.Show("Warning: Image not found");
            else
            {
                var db = new DataRepository();

                var inv = db.PostInvoice(model,false);

                if (inv != null)
                {
                    MessageBox.Show(FindResource("invoice_saved_ok").ToString());
                    this.Close();
                }
                else MessageBox.Show("Error");

            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
