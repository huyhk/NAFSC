
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.ComponentModel;
using System.Timers;
using RJCP.IO.Ports;
using System.Configuration;

namespace Megatech.NAFSC.WPFApp.Helpers
{
    class ErmHelper : IDisposable
    {

        private SerialPortStream serial;

        private string comPort = ConfigurationManager.AppSettings["ComPort"];
        private int baurate = 9600;
        private Timer tmr;
        private bool waiting;

        public ErmHelper()
        {
            _data = new Helpers.EMRData();
            Status = EMRSTATUS.STOPPED;
            serial = new SerialPortStream();
            
        }


        private void OnStatusChanged()
        {
            if (this.StatusChanged != null)
                this.StatusChanged(this.Status);

        }

        public EMRSTATUS Status { get; set; }
        public delegate void StatusChangedEventHandler(EMRSTATUS status);
        public event StatusChangedEventHandler StatusChanged;

        public ErmHelper(string comPort) : this()
        {
            this.comPort = comPort;
            try
            {                
                IsError = false;
            }
            catch
            {
                IsError = true;
            }

        }
        public bool IsError { get; set; }

        byte[] data = new byte[48];

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = (SerialPortStream)sender;
            ReadData();
        }
        bool startData = false;
        int idx = 0;
        private void ReadData()
        {
            try
            {
                busy = false;
                Log("Read Data");
                if (serial.CanRead)
                {

                    var l = 0;
                    while (serial.BytesToRead > 0)
                    {
                        //var b = new byte[serial.BytesToRead];
                        //serial.Read(b, 0, serial.BytesToRead);

                        //b.CopyTo(data, l);
                        //l += b.Length;
                        var b = serial.ReadByte();
                        if (b == 0x7e)
                        {
                            if (startData)
                            {
                                startData = false;
                                data[idx++] = (byte)b;
                                idx = 0;
                                ProcessData(data);
                            }
                            else
                            {
                                data[idx++] = (byte)b;
                                startData = true;
                            }
                        }
                        else if (startData)
                        {
                            data[idx++] = (byte)b;
                        }
                    }
                    //if (data[l - 1] == 0x7e)
                    //    ProcessData(data);
                }
                if (serial.BytesToRead <= 0)
                {
                    waiting = false;

                    ProcessQueue();
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        private void ProcessData(byte[] b)
        {
            Log(b);
            if (b.Length >= 7)
            {
                byte[] code = new byte[2] { 0, 0 };
                Array.Copy(b, 3, code, 0, 2);
                if (code[0] == EMRCommand.START.ResponseCode[0])
                {
                    //start or end code
                    if (code[1] == 0 || code[1] == 2)
                    {
                        if (Status == EMRSTATUS.STOPPED)
                            Status = EMRSTATUS.STARTED;
                        else Status = EMRSTATUS.STOPPED;
                        OnStatusChanged();
                    }
                }
                else if (code.SequenceEqual(EMRCommand.VOLUME.ResponseCode))
                    _data.Volume = ToDouble(b);
                else if (code.SequenceEqual(EMRCommand.TOTALIZER.ResponseCode))
                    _data.Totalizer = ToDouble(b);
                else if (code.SequenceEqual(EMRCommand.RATE.ResponseCode))
                    _data.Rate = ToDouble(b);
            }
            data = new byte[48];
        }

        private double ToDouble(byte[] resp)
        {
            try
            {
                byte[] outp = new byte[8];
                int j = 5;
                for (int i = 0; i < 8; i++)
                {
                    if (resp[j] != 0x7d)
                        outp[i] = resp[j];
                    else
                        outp[i] = (byte)(resp[j++ + 1] ^ 0x20);
                    j++;
                }
                //Array.Copy(resp, 5, outp, 0, 8);
                var result = BitConverter.ToDouble(outp, 0);

                if (result < 0) result = 0;
                return result;
            }
            catch (Exception ex)
            { return 0; }
        }

        public bool Open()
        {
            try
            {
                if (serial == null)
                {
                    serial = new SerialPortStream();
                    
                }
                if (!serial.IsOpen)
                {
                    Log("Open");
                    serial.PortName = comPort;                    
                    serial.BaudRate = baurate;
                    serial.DataBits = 8;
                    serial.StopBits = StopBits.One;
                    serial.Parity = Parity.None;
                    

                    //serial.ReadTimeout = 500;
                    //serial.WriteTimeout = 500;
                    //serial.ReadBufferSize = 4096;
                    //serial.WriteBufferSize = 4096;
                    serial.DtrEnable = true;
                    serial.RtsEnable = true;
                    serial.Open();
                    serial.DataReceived += Serial_DataReceived;
                    //serial.DtrEnable = true;
                    //serial.RtsEnable = true;
                }
               
                return serial.IsOpen;
            }
            catch (Exception ex)
            {
                IsError = true;
                Log("Open - " + ex.Message);
                return false;
            }
        }



        public void Close()
        {
            try
            {
                if (serial != null && serial.IsOpen)
                    serial.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EMRData CurrentData
        {
            get { return _data; }
        }

        public byte[] SendRequest(byte[] data)
        {
            try
            {
                if (!serial.IsOpen)
                    Open();
                while (!serial.IsOpen)
                    ;
                string resp = string.Empty;

                Log("Write data");
                Log(data);
                serial.Write(data, 0, data.Length);

                waiting = true;
                
            }
            catch (Exception ex)
            {
                Log("SendRequest - " +  ex.Message);
            }
            return null;
        }
        //send start command
        public void Start()
        {
            
            Request(EMRCommand.START);
        }

        //send finish command
        public void End()
        {
            Request(EMRCommand.END);
        }

        public bool GetStatus()
        {
            byte[] b = GetValue(new byte[] { 0x54, 0x01 });
            return (b[5] & 4) > 0 || (b[5] & 2) > 0;
        }

        private void Log(string data)
        {
            try
            {
                var dir = Directory.GetCurrentDirectory();
                var file = Path.Combine(dir, "emr.log");
                if (File.Exists(file))
                {
                    //check file size
                    FileInfo fi = new FileInfo(file);
                    if (fi.Length > 20 * 1024 * 1024)
                    {
                        File.Move(file, Path.Combine(dir, string.Format("log_{0:yyyyMMdd_HHmm}.log", DateTime.Now)));
                    }

                }
                File.AppendAllText(file, string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}\n", DateTime.Now, data));
            }
            catch (Exception ex)
            { }
        }
        private void Log(byte[] data)
        {
            Log(data, string.Empty);
        }
        private void Log(byte[] data, string error)
        {

            if (data != null)
            {
                try
                {
                    string s = string.Format("{0:x2}", data[0]);
                    
                    for (int i = 1; i < data.Length; i++)
                    {
                        s += string.Format("{0:x2}", data[i]);
                    }
                                        
                    Log(s);
                }
                catch (Exception ex)
                { }
            }
        }

        public byte[] GetValue(params byte[] inputCode)
        {
            byte[] data = new byte[inputCode.Length + 5];
            data[0] = 0x7e;
            data[1] = 0x01;
            data[2] = 0xff;
            byte cs = (byte)(0 - (data[1] + data[2]));
            for (int i = 0; i < inputCode.Length; i++)
            {
                data[i + 3] = inputCode[i];
                cs = (byte)(cs - inputCode[i]);
            }
            data[data.Length - 2] = cs;
            data[data.Length - 1] = 0x7e;

            

            byte[] resp = SendRequest(data);

            
            return resp;
        }

        public double GetDouble(params byte[] inputCode)
        {
            try
            {
                byte[] resp = GetValue(inputCode);
                byte[] outp = new byte[8];
                int j = 5;
                for (int i = 0; i < 8; i++)
                {
                    if (resp[j] != 0x7d)
                        outp[i] = resp[j];
                    else
                        outp[i] = (byte)(resp[j++ + 1] ^ 0x20);
                    j++;
                }
                //Array.Copy(resp, 5, outp, 0, 8);
                var result = BitConverter.ToDouble(outp, 0);

                if (result < 0) result = 0;
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public float GetFloat(params byte[] inputCode)
        {
            try
            {
                byte[] resp = GetValue(inputCode);
                byte[] outp = new byte[4];
                int j = 5;
                for (int i = 0; i < 4; i++)
                {
                    if (resp[j] != 0x7d)
                        outp[i] = resp[j];
                    else
                        outp[i] = (byte)(resp[j++ + 1] ^ 0x20);
                    j++;
                }
                var result = BitConverter.ToSingle(outp, 0);

                if (result < 0) result = 0;
                return result;

            }
            catch (Exception ex)
            {

                Log(inputCode, ex.Message);

                return 0;
            }
        }
        public double GetDisplayVolume()
        {
            return GetDouble(0x47, 0x4B);
        }

        public double GetGross()
        {
            return GetDouble(0x47, 0x4b);
        }
        public double GetDisplayTotalizer()
        {
            return GetDouble(0x47, 0x4c);
        }
        public double GetDisplayRate()
        {
            return GetDouble(0x47, 0x52);
        }

        public float GetTemperature()
        {
            return GetFloat(0x47, 0x74);
        }

        private Queue<byte[]> _queue;
        
        private EMRData _data;
        private bool busy;

        public void GetData()
        {
            

            if (!busy)
            {
                Request(EMRCommand.TOTALIZER);
                Request(EMRCommand.VOLUME);
                Request(EMRCommand.RATE);
            }
        }


        private void ProcessQueue()
        {
            if (_queue != null && _queue.Count > 0)
            {
                if (!waiting)
                {
                    var code = _queue.Dequeue();
                    SendCode(code);
                }
            }


        }

        private void SendCode(byte[] code)
        {
            if (code != null && code.Length >0)
            {
                byte[] data = new byte[code.Length + 5];
                data[0] = 0x7e;
                data[1] = 0x01;
                data[2] = 0xff;
                byte cs = (byte)(0 - (data[1] + data[2]));
                for (int i = 0; i < code.Length; i++)
                {
                    data[i + 3] = code[i];
                    cs = (byte)(cs - code[i]);
                }
                data[data.Length - 2] = cs;
                data[data.Length - 1] = 0x7e;

                

                SendRequest(data);
            }
        }

        public void Request(EMRCommand cmd)
        {
            if (_queue == null)
                _queue = new Queue<byte[]>();
            _queue.Enqueue(cmd.RequestCode);
            if (!waiting)
                ProcessQueue();
        }

        public void Dispose()
        {
            if (serial != null && serial.IsOpen)
                serial.Close();
            serial.Dispose();
        }
    }
    public class EMRCommand
    {
        public byte[] RequestCode { get; set; }
        public byte[] ResponseCode { get; set; }


        public static EMRCommand START = new EMRCommand { RequestCode = new byte[] { 0x4f, 0x01 }, ResponseCode = new byte[] { 0x41} };
        public static EMRCommand END = new EMRCommand { RequestCode = new byte[] { 0x4f, 0x03 }, ResponseCode = new byte[] { 0x41 } };
        public static EMRCommand VOLUME = new EMRCommand { RequestCode = new byte[] { 0x47, 0x4b }, ResponseCode = new byte[] { 0x46, 0x4b } };
        public static EMRCommand TOTALIZER = new EMRCommand { RequestCode = new byte[] { 0x47, 0x4c }, ResponseCode = new byte[] { 0x46, 0x4c } };
        public static EMRCommand RATE = new EMRCommand { RequestCode = new byte[] { 0x47, 0x52 }, ResponseCode = new byte[] { 0x46, 0x52 } };

    }
    public class EMRData : INotifyPropertyChanged
    {
        private double _rate;
        private double _totalizer;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Status { get; set; }
        public double StartMeter
        {
            get;
            set;
        }
        public double EndMeter
        {
            get;
            set;
        }
        public double Volume { get; set; }
        public double Totalizer {
            get
            {
                return _totalizer;
            }
            set {
                if (_totalizer == 0)
                    StartMeter = value;
                EndMeter = value;
                _totalizer = value;

            }
        }
        public double Rate
        {
            get
            {
                return _rate;
            }
            set
            {

                if (MaxRate < value)
                    MaxRate = value;
                _rate = value;
            }
        }
        public float Temperature { get; set; }

        public double MaxRate
        {
            get;
            set;
        }

    }

    public enum EMRSTATUS
    {
        STOPPED,
        STARTED

    }
}
