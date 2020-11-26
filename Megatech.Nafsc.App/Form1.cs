using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Megatech.Nafsc.App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ports = SerialPort.GetPortNames();
            foreach (var item in ports)
            {
                var port = new SerialPort(item, 9600);
                port.Open();
                port.Close();
            }
        }
    }
}
