using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO.Ports;

namespace nfrnew.lib
{
    class parserCommand
    {
        public PortCOM port;
        TCPClient ClientTCP;
        Stopwatch timeFromLastPacket = Stopwatch.StartNew();
        List<AFAComm> tWithCommad = new List<AFAComm>();

        List<AFAComm> tWithCommadClassAddres = new List<AFAComm>();

        public bool readEnable = true;

        public delegate void Wskaznik(byte[] a);
        public delegate void WskaznikExceded(int devClass, int address, byte[] a);

        public class ComDataReceivedEventArgs : EventArgs
        {
            public string data { get; set; }

            public ComDataReceivedEventArgs(string s)
            {
                this.data = s;
            }
        }

        public event EventHandler<ComDataReceivedEventArgs> ComReceived_Calback;

        public class AFAComm
        {
            public byte type;
            public bool Enabled;
            public Wskaznik f;
            public WskaznikExceded fe;

            public AFAComm(bool Enabled, byte type, Wskaznik sf)
            {
                this.type = type;
                this.Enabled = Enabled;

                f = new Wskaznik(sf);
            }
            
            public AFAComm(bool Enabled, byte type, WskaznikExceded sf)
            {
                this.type = type;
                this.Enabled = Enabled;

                fe = new WskaznikExceded(sf);
            }
        }

        public parserCommand(TCPClient _clientTCP)
        {
            this.ClientTCP = _clientTCP;
            this.ComReceived_Calback += new EventHandler<ComDataReceivedEventArgs>(comParse);
        }

        public bool openPortCom(string _name, int _baud)
        {
            try
            {
                if(port != null) closePortCom();
                this.port = new PortCOM(_name, _baud, serialPort1_DataReceived);

                if (!port.IsOpen)
                {
                    port = null;
                    return false;
                }


                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                port = null;
                return false;
            }
        }


        public bool closePortCom()
        {
            port.ReadExisting();
            port.Close();
            port.Dispose();
            this.port = null;

            return false;
        }

        public bool changeBaudrate(int _baud)
        {
            if (port != null)
            {
                this.port.BaudRate = _baud;
                return true;
            }
            else
                return false;
        }

        public void comParse(object sender, ComDataReceivedEventArgs data) 
        {
            if (data.data.IndexOf("+CMTI") != -1)
            {
                Console.WriteLine("Received message");
            }
        }
        




        public void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;

            if (sp == null)
                return;

            if(sp.IsOpen)
            {
                int bytesToRead = port.BytesToRead;

                byte[] bytesFromCom = new byte[bytesToRead];
                port.Read(bytesFromCom, 0, bytesToRead);

                try
                {
                    ComReceived_Calback?.Invoke(this, new ComDataReceivedEventArgs(Encoding.ASCII.GetString(bytesFromCom)));
                    Console.WriteLine(Encoding.ASCII.GetString(bytesFromCom));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error parsing " + ex.ToString());
                }
            }

            
        }
    }
}
