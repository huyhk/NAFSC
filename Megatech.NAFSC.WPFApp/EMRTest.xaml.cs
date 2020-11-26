using Megatech.NAFSC.WPFApp.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
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
using System.Windows.Threading;

namespace Megatech.NAFSC.WPFApp
{
    /// <summary>
    /// Interaction logic for EMRTest.xaml
    /// </summary>
    public partial class EMRTest : Window
    {
        private ErmHelper erm;
        DispatcherTimer tmr = new DispatcherTimer();
        public EMRTest()
        {
            InitializeComponent();
            erm = new ErmHelper("COM1");
            tmr.Tick += Tmr_Tick;
            tmr.Interval = new TimeSpan(0, 0, 0, 0, 500);
        }

        private void Tmr_Tick(object sender, EventArgs e)
        {

            //var gross = erm.GetDisplayVolume();
            //var total = erm.GetDisplayTotalizer();
            //var rate = erm.GetDisplayRate();
            
            erm.GetData();

            textBox.Text = string.Format("Total: {0:#,0.00}    Volume: {1:#,#0.00}   FlowRate: {2:#,0.00}", erm.CurrentData.Totalizer, erm.CurrentData.Volume, erm.CurrentData.Rate);

        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            
                
        }
        private Font printFont;
        private StreamReader streamToPrint;

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
               printFont.GetHeight(ev.Graphics);
            int i = 0;
            var lines = new string[] { "một hai ba bốn", "123.56" };
            // Print each line of the file.
            while (count < linesPerPage && i <=1               )
            {
                line = lines[i++];

                yPos = topMargin + (count *
                   printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, printFont, System.Drawing.Brushes.Black,
                   leftMargin, yPos, new StringFormat());
                count++;
            }

            // If more lines exist, print another page.
            if (i<1)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            erm.End();
        }

        private void btnGross_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
