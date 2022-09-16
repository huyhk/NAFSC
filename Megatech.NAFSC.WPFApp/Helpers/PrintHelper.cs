using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using System.Diagnostics;

using System.Management;
using System.IO;

namespace TestTMU590
{
    class PrintHelper
    {



        /// <summary>

        /// Add the printer connection for specified pName.

        /// </summary>

        /// <param name="pName"></param>

        /// <returns></returns>

        [DllImport("winspool.drv")]

        public static extern bool AddPrinterConnection(string pName);



        /// <summary>

        /// Set the added printer as default printer.

        /// </summary>

        /// <param name="Name"></param>

        /// <returns></returns>

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]

        public static extern bool SetDefaultPrinter(string Name);



        #region Print File

        /// <summary>

        /// Sends the file to the printer choosed.

        /// </summary>

        /// <param name="fileName">Name & path of the file to be printed</param>

        /// <param name="printerPath">The path of printer</param>

        /// <param name="numCopies">The number of copies send to printer</param>

        public bool PrintFile(string fileName, string printerPath, int numCopies)

        {

            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(printerPath))

            {

                return false;

            }



            //Instantiate the object of ProcessStartInfo

            Process objProcess = new Process();

            try

            {

                ProcessStartInfo psi = new ProcessStartInfo(fileName);

                //psi.Arguments = "/pt " + fileName + " \""+printer +"\"";

                psi.Verb = "Print";

                psi.WindowStyle = ProcessWindowStyle.Hidden;

                psi.UseShellExecute = true;

                Process.Start(psi);



                return true;

            }

            catch (Exception ex)

            {

                // Log the exception here...

                return false;

            }

            finally

            {

                objProcess.Close();

            }

        }
        private string printer = "EPSON TM-U590 Slip";
        public void PrintText(string[] text)
        {
            printer = new System.Drawing.Printing.PrinterSettings().PrinterName;

            var path = Path.GetTempFileName().Replace("tmp","txt");

            File.AppendAllLines(path, text);

            new RawPrint.Printer().PrintRawFile( printer, path, false);
        }
        #endregion
    }
}
