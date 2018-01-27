using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;

namespace nfrnew.lib
{
    class PortCOM : SerialPort
    {

        public delegate void Wskaznik(object sender, SerialDataReceivedEventArgs e);
        public Wskaznik f;

        public PortCOM(string _portName, int _baudrate, Wskaznik _sf)
        {
            this.PortName = _portName;
            this.BaudRate = _baudrate;

            Console.WriteLine("Param write");
            PortName = _portName;
            BaudRate = _baudrate;
            Parity = Parity.None;
            DataBits = 8;
            RtsEnable = false;
            DtrEnable = false;
            f = _sf;
            DataReceived += new SerialDataReceivedEventHandler(f);
            ReadBufferSize = 8192;
            Console.WriteLine("Param writed");

            try
            {
                Open();
                Console.WriteLine("Opened");
            }
            catch (Exception)
            {
                MessageBox.Show("Problem z otwarciem COMu");
            }
        }

        public void Send(byte[] data)
        {
            if (this == null) return;

            if (IsOpen) Write(data, 0, data.Length);
            else Console.WriteLine("Port Write Error: Port not open");
        }
        public void Send(byte data)
        {
            byte[] dat = new byte[1]{ data };

            if (IsOpen) Write(dat, 0, dat.Length);
            else Console.WriteLine("Port Write Error: Port not open");
        }

        public void Send(string data)
        {
            byte[] dat = Encoding.ASCII.GetBytes(data);

            if (IsOpen) Write(dat, 0, dat.Length);
            else Console.WriteLine("Port Write Error: Port not open");
        }

        public void DisableReceivedEventHandler()
        {
            DataReceived -= new SerialDataReceivedEventHandler(f);
        }
        public void EnableReceivedEventHandler()
        {
            DataReceived += new SerialDataReceivedEventHandler(f);
        }


        public bool WaitForResult(string ToSend, ref string Result, string[] ReqResult, int Timeout)
        {
            DisableReceivedEventHandler();

            if(ToSend != "" && ToSend != null)
                Write(ToSend);

            string Answer = string.Empty;
            double WaitTimeout = Timeout + DateTime.Now.TimeOfDay.TotalMilliseconds;
            while (!(DateTime.Now.TimeOfDay.TotalMilliseconds >= WaitTimeout))
            {
                int btr = BytesToRead;

                if (btr > 0)
                {
                    byte[] Bytes = new byte[btr];
                    Read(Bytes, 0, btr);
                   
                    Answer += System.Text.Encoding.ASCII.GetString(Bytes);
                    bool found = false;

                    foreach (string s in ReqResult)
                        if (Answer.IndexOf(s) != -1)
                            found = true;

                    if (found)
                    {
                        Result = Answer;
                        EnableReceivedEventHandler();
                        return true;
                    }
                    else
                        System.Windows.Forms.Application.DoEvents();
                }
            }

            Result = Answer;
            EnableReceivedEventHandler();
            return false;
        }
    }
}
