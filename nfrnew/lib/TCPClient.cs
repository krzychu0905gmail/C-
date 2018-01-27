using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;

namespace nfrnew.lib
{

    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class MyDataReceivedEventArgs : EventArgs
    {
        public byte[] Bytes { get; set; }
    }

    public class TCPClient
    {
        Thread ThrCheckConnected;
        Socket ClientTCP;
        public string uniqueIDClients;
        private bool disposed = false;
        public string IP;
        public bool IsConnectedPrev = false;
        public bool IsConnect = false;

        public event EventHandler<MyDataReceivedEventArgs> EreceiveCalback;
        public event EventHandler OnConnectCallback;
        public event EventHandler OnDisconnectCallback;

        public void StartClient(string _IP, int _port)
        {
            // Connect to a remote device.
            try
            {
                IP = _IP;

                Dispose();

                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(_IP), _port);

                // Create a TCP/IP socket.
                ClientTCP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                ClientTCP.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), ClientTCP);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void StopClient(string statement)
        {
            if (IsConnected())
            {
                ClientTCP.Shutdown(SocketShutdown.Both);
                ClientTCP.Close();
            }
            Dispose();

            Console.WriteLine("Client " + IP + " " + statement);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                Receive(client);

                ThrCheckConnected = new Thread(checkConnectStatus);
                ThrCheckConnected.Start();

                if (this.OnConnectCallback != null)
                {
                    this.OnConnectCallback(this, null);
                }

                IsConnect = true;

            }
            catch (System.Net.Sockets.SocketException)
            {
                Console.WriteLine("Nie można połączyć z serwerem " + IP);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                StopClient("Canot connect");
            }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Clear();
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    Console.WriteLine("TCPClientAsync " + IP + " received data: > " + state.sb.ToString().Replace("\r\n", ""));

                    if (this.EreceiveCalback != null)
                    {
                        MyDataReceivedEventArgs data = new MyDataReceivedEventArgs();
                        data.Bytes = state.buffer;

                        this.EreceiveCalback(this, data);
                    }

                    // Get the rest of the data.
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                StopClient("Err rec callback");
            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                StopClient("Canot send");
            }
        }

        public bool IsConnected()
        {
            try
            {
                ClientTCP.Send(new byte[] { 0 });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                StopClient("Canot recrive from " + IP);
            }
        }
        public void Send(string data)
        {
            try
            {
                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(data + "\r\n");

                // Begin sending the data to the remote device.
                ClientTCP.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), ClientTCP);
            }
            catch (System.Net.Sockets.SocketException)
            {
                Console.WriteLine("Brak połączenia z serwerem " + IP);
                StopClient("Canot send");
            }
            catch (System.NullReferenceException)
            {
                Console.WriteLine("Brak połączenia z serwerem " + IP);
                StopClient("Canot send");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                StopClient("Can't send");
            }
        }
        public void Send(byte[] data)
        {
            try
            {
                // Begin sending the data to the remote device.
                ClientTCP.BeginSend(data, 0, data.Length, 0,
                    new AsyncCallback(SendCallback), ClientTCP);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Can't send " + IP.ToString() + ex.ToString());
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    if (this.ClientTCP != null)
                    {
                        this.ClientTCP.Close();
                        this.ClientTCP = null;
                    }
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            disposed = true;

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            // base.Dispose(disposing);
        }

        private void checkConnectStatus()
        {
            while (true)
            {
                if (IsConnected() == false)
                {
                    IsConnect = true;
                    this.OnDisconnectCallback.Invoke(this, null);
                    return;
                }
                Thread.Sleep(500);
            }
        }




        public void sendFrame(byte[] dataToSend, byte dstAddress, byte dstClass, byte comName, bool formatMaster)
        {
            byte[] frame = new byte[dataToSend.Length + 9];
             byte[] prefix;

            if(!formatMaster) prefix = new byte[] { 0xAB, 0x00, 0x00, dstClass, dstAddress, comName };
            else prefix = new byte[] { 0xAB, dstClass, dstAddress, comName};

            List<byte> frameToSend = new List<byte>();

            Array.Copy(prefix, frame, prefix.Length);
            Array.Copy(dataToSend, 0, frame, prefix.Length, dataToSend.Length);

            int crc = Common.MakeCrc(frame, frame.Length - 3);

            frame[frame.Length - 3] = (byte)crc;
            frame[frame.Length - 2] = (byte)(crc >> 8);
            frame[frame.Length - 1] = 0xAC;

            for (int i = 0; i < frame.Length; i++)
            {
                byte data = frame[i];

                if (i != 0 && i != frame.Length - 1)
                {
                    if (data == 0xAA || data == 0xAB || data == 0xAC)
                    {
                        frameToSend.Add(0xAA);
                        data = (byte)~data;
                    }
                }

                frameToSend.Add(data);
            }


            Send(frameToSend.ToArray());
        }

     
    }
}

 

