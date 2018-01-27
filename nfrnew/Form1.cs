using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;
using nfrnew.lib;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting;

namespace nfrnew
{
    public partial class Form1 : Form
    {
        TCPClient ClientMaster;
        parserCommand parserCommand;
        bool AppRuning = false;

        enum eTypeDeleteMessage { READ, UNREAD, SENT, UNSENT, INBOX, ALL };
        enum eTypeReadMessage { REC_READ, REC_UNREAD, STO_SENT, STO_UNSENT, ALL };

        pass AdminPass = new pass("colorado");
        Status ApplicationStatus;
        Stopwatch timeBeginSesion = Stopwatch.StartNew();
        Plik configFile = new Plik(@"..\..\..\conf\AfaConf.xml");

        //SYSTEM
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Text += " " + v;

            string ans = configFile.Open();

            //ConsoleShow(configFile.AfaContrConf.consoleEnable);

            this.MaximumSize = new Size(600, 523);
            this.MinimumSize = new Size(600, 523);
            this.Size = new Size(600, 523);

            ApplicationStatus = new Status(lStatus);

            AllocConsole();

            ClientMaster = new TCPClient();
            ClientMaster.OnDisconnectCallback += new EventHandler(TCPClientDisconnectCallback);
            ClientMaster.EreceiveCalback += new EventHandler<MyDataReceivedEventArgs>(TCPClient_receivedEvent);

            parserCommand = new parserCommand(ClientMaster);

            foreach (string p in SerialPort.GetPortNames())
                cbWyborPortu.Items.Add(p);

            cbSMSErase.DataSource = Enum.GetValues(typeof(eTypeDeleteMessage));
            cbSMSReadGroup.DataSource = Enum.GetValues(typeof(eTypeReadMessage));

            cbWyborPortu.Text = configFile.AfaContrConf.LastComName;

            this.Text += " Admin";


            foreach (KeyValuePair<string, Plik.Contact> entry in configFile.AfaContrConf.Phonebook)
            {
                cbSMSContact.Items.Add(entry.Value.Name);
            }

            if (configFile.AfaContrConf.Phonebook.Count != 0)
            {
                if (configFile.AfaContrConf.Phonebook.ContainsKey(configFile.AfaContrConf.LastContactKey))
                {
                    cbSMSContact.Text = configFile.AfaContrConf.Phonebook[configFile.AfaContrConf.LastContactKey].Name;
                    tbSMSContactNumber.Text = configFile.AfaContrConf.Phonebook[configFile.AfaContrConf.LastContactKey].Number;
                }
                else
                {
                    if (configFile.AfaContrConf.Phonebook.ContainsKey(cbSMSContact.Items[0].ToString()))
                    {
                        cbSMSContact.Text = configFile.AfaContrConf.Phonebook[cbSMSContact.Items[0].ToString()].Name;
                        tbSMSContactNumber.Text = configFile.AfaContrConf.Phonebook[cbSMSContact.Items[0].ToString()].Number;
                    }
                    else
                    {
                        cbSMSContact.Text = "none";
                        tbSMSContactNumber.Text = "none";
                    }
                }
            }
            else
            {
                cbSMSContact.Text = "none";
                tbSMSContactNumber.Text = "none";
            }



            AppRuning = true;

        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        private void WyborPortu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AppRuning)
            {
                configFile.AfaContrConf.LastComName = cbWyborPortu.SelectedItem.ToString();
                configFile.SaveParam();
            }

            if (parserCommand.port == null)
                portChangeOpenClose(parserCommand.openPortCom(cbWyborPortu.SelectedItem.ToString(), checkBaud()));
            else if (cbWyborPortu.SelectedItem.ToString() != parserCommand.port.PortName)
                portChangeOpenClose(parserCommand.openPortCom(cbWyborPortu.SelectedItem.ToString(), checkBaud()));

        }
        private void portChangeOpenClose(bool portOpen)
        {
            //bOpenClosePort.Enabled = false;
            //cbWyborPortu.Enabled = false;
            //tim500ms.Enabled = true;

            if (portOpen)
            {
                bOpenClosePort.BackColor = Color.Green;
                bOpenClosePort.Text = "Opened";
                bOpenClosePort.Visible = true;
                //fElementVisible(true);
            }
            else
            {
                bOpenClosePort.BackColor = Color.Red;
                bOpenClosePort.Text = "Closed";
                bOpenClosePort.Visible = true;
                //fElementVisible(false);
            }

        }

        private void ConsoleShow(bool show)
        {
            const int SW_HIDE = 0;
            const int SW_SHOW = 5;

            var handle = GetConsoleWindow();

            if (show)
            {
                configFile.AfaContrConf.consoleEnable = true;
                ShowWindow(handle, SW_SHOW);
            }
            else
            {
                configFile.AfaContrConf.consoleEnable = false;
                ShowWindow(handle, SW_HIDE);
            }

            configFile.SaveParam();
        }

        private void rbBaudrate_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBaudrate115200.Checked) parserCommand.changeBaudrate(115200);

            parserCommand.openPortCom(cbWyborPortu.SelectedItem.ToString(), checkBaud());
        }

        void TCPClientDisconnectCallback(object sender, EventArgs e)
        {
            try
            {
                this.Invoke(
                    new Action(() =>
                    {
                        ApplicationStatus.setApplicationStatus("AGV Disconnected: " + ((TCPClient)sender).IP);
                        //bTCPOpen.BackColor = Color.Red;
                        //bTCPOpen.Text = "Closed";
                    }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        void TCPClient_receivedEvent(object sender, MyDataReceivedEventArgs data)
        {
            this.Invoke(
                new Action(() =>
                {
                    //parserCommand.comParse(data.Bytes);
                }
            ));
        }
        public void tcpClient_DataReceived(byte[] data)
        {
            //parserCommand.comParse(data);
        }

        private void bForm_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            string Name = b.Name;

            if (Name == bGetVersion.Name)
            {
                parserCommand.port.Write("AT\r");
            }
            else if (Name == bReset.Name)
            {
                ApplicationStatus.setApplicationStatus("Reset device");
            }
            else if (Name == bOpenClosePort.Name)
            {
                if (parserCommand.port == null)
                    portChangeOpenClose(parserCommand.openPortCom(cbWyborPortu.SelectedItem.ToString(), checkBaud()));
                else
                    portChangeOpenClose(parserCommand.closePortCom());
            }
        }



        private int checkBaud()
        {
            if (rbBaudrate115200.Checked) return 115200;

            return 57600;
        }


        private void bSMSSend_Click(object sender, EventArgs e)
        {
            string number = tbSMSContactNumber.Text;
            string message = tbSMSMessageToSend.Text;
            string result = string.Empty;

            if (parserCommand.port.WaitForResult("AT+CMGS=\"" + number + "\"\r", ref result, new string[] { ">" }, 2000))
            {
                parserCommand.port.Write(message + "\r");
                parserCommand.port.Write(new byte[] { 0x1A }, 0, 1);

                ApplicationStatus.setApplicationStatus("Message sending");

                if (parserCommand.port.WaitForResult("", ref result, new string[] { "OK" }, 20000))
                {
                    tbSMSMessageToSend.Clear();
                    ApplicationStatus.setApplicationStatus("Message send");
                }
                else
                    ApplicationStatus.setApplicationStatus("Cannot send");
            }
            else
                ApplicationStatus.setApplicationStatus("Cannot set number");
        }
        private void bSMSSetMode_Click(object sender, EventArgs e)
        {
            string result = string.Empty;

            if (parserCommand.port.WaitForResult("AT+CREG?\r\n", ref result, new string[] { "+CREG: 0,1", "+CREG: 0,5" }, 500))
            {
                ApplicationStatus.setApplicationStatus("The module is connected to network");

                if (parserCommand.port.WaitForResult("AT+CMGF=1\r\n", ref result, new string[] { "OK" }, 1000))
                    ApplicationStatus.setApplicationStatus("SMS mode is set");
                else
                    ApplicationStatus.setApplicationStatus("Cannot set SMS mode");

            }
            else
                ApplicationStatus.setApplicationStatus("The module is not connected to network");

        }

        private void bSMSAddContact_Click(object sender, EventArgs e)
        {
            if (cbSMSContact.Text == "" || tbSMSContactNumber.TextLength != 12 || tbSMSContactNumber.Text[0] != '+')
                ApplicationStatus.setApplicationStatus("Please corect the contact before save");
            else
            {
                if (!configFile.AfaContrConf.Phonebook.ContainsKey(cbSMSContact.Text))
                {
                    cbSMSContact.Items.Add(cbSMSContact.Text);
                    configFile.AfaContrConf.Phonebook.Add(cbSMSContact.Text, new Plik.Contact(cbSMSContact.Text, tbSMSContactNumber.Text));
                    configFile.SaveParam();
                    ApplicationStatus.setApplicationStatus("Contact saved");
                }
                else
                    ApplicationStatus.setApplicationStatus("Contact exist");
            }
        }

        private void cbSMSContact_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AppRuning) return;

            if (configFile.AfaContrConf.Phonebook.ContainsKey(cbSMSContact.SelectedItem.ToString()))
            {
                tbSMSContactNumber.Text = configFile.AfaContrConf.Phonebook[cbSMSContact.SelectedItem.ToString()].Number;
            }
        }

        private void SetSim900Status(string res, string message)
        {
            ApplicationStatus.setApplicationStatus(message);
            res = res.Replace('\r', ' ');
            res = res.Replace('\n', ' ');

            Console.WriteLine(message + "\t" + res);
        }

        private void bTCPServConnect_Click(object sender, EventArgs e)
        {
            string result = string.Empty;
            bool sim_error_with_open_tcp = false;

            if (parserCommand.port.WaitForResult("AT+CREG?\r", ref result, new string[] { "+CREG: 0,1", "+CREG: 0,5" }, 500))
            {
                SetSim900Status(result, "The module is connected to network");
                if (parserCommand.port.WaitForResult("AT+CIPMUX=0\r", ref result, new string[] { "OK" }, 1000))
                {
                    SetSim900Status(result, "Set single-connection mode");
                    if (parserCommand.port.WaitForResult("AT+CIPSTATUS\r", ref result, new string[] { "INITIAL" }, 500))
                    {
                        SetSim900Status(result, "Status initial");
                        if (parserCommand.port.WaitForResult("AT+CSTT=\"plus\",\"\",\"\"\r", ref result, new string[] { "OK" }, 30000))
                        {
                            SetSim900Status(result, "APN set ok");
                            if (parserCommand.port.WaitForResult("AT+CIPSTATUS\r", ref result, new string[] { "START" }, 500))
                            {
                                SetSim900Status(result, "CIPSTATUS START");
                                Thread.Sleep(5000);
                                if (parserCommand.port.WaitForResult("AT+CIICR\r", ref result, new string[] { "OK" }, 30000))
                                {
                                    SetSim900Status(result, "Brings Up Wireless Connection");
                                    if (parserCommand.port.WaitForResult("AT+CIPSTATUS\r", ref result, new string[] { "GPRSACT" }, 500))
                                    {
                                        SetSim900Status(result, "GPRS Active");
                                        Thread.Sleep(5000);
                                        if (parserCommand.port.WaitForResult("AT+CIFSR\r", ref result, new string[] { "." }, 10000))
                                        {
                                            SetSim900Status(result, "IP " + result);
                                            if (parserCommand.port.WaitForResult("AT+CIPSTATUS\r", ref result, new string[] { "IP STATUS" }, 500))
                                            {
                                                SetSim900Status(result, "IP status OK");
                                                Thread.Sleep(5000);
                                                if (parserCommand.port.WaitForResult("AT+CIPSTART=\"TCP\",\"0.tcp.ngrok.io\",\"18471\"\r", ref result, new string[] { "CONNECT OK" }, 30000))
                                                {
                                                    SetSim900Status(result, "Connected to server TCP/IP");
                                                    bTCPServerSend.Enabled = true;
                                                }
                                                else
                                                {
                                                    SetSim900Status(result, "Can't connect to server TCP/IP");
                                                }
                                            }
                                            else
                                            {
                                                SetSim900Status(result, "IP status NOK");
                                            }
                                        }
                                        else
                                        {
                                            SetSim900Status(result, "GPRS inactive");
                                        }
                                    }
                                    else
                                    {
                                        SetSim900Status(result, "GPRS Inactive");
                                    }
                                }
                                else
                                {
                                    SetSim900Status(result, "Can't brings Up Wireless Connection");
                                }
                            }
                            else
                            {
                                SetSim900Status(result, "Status isn't START");
                            }
                        }
                        else
                        {
                            SetSim900Status(result, "Can't set APN");
                        }
                    }
                    else
                    {
                        SetSim900Status(result, "Status isn't INITIAL");
                    }
                }
                else
                {
                    SetSim900Status(result, "Can't set single-connection mode");
                }
            }
            else
            {
                SetSim900Status(result, "Can't connect to network");
            }
        }

        private void bTCPServDisconnect_Click(object sender, EventArgs e)
        {
            string result = string.Empty;
            bool sim_error = false;

            if (parserCommand.port.WaitForResult("AT+CIPCLOSE\r", ref result, new string[] { "CLOSE OK" }, 10000))
            {
                SetSim900Status(result, "Closed connection");
            }
            else
            {
                SetSim900Status(result, "Can't close");
                sim_error = true;
            }

            if (parserCommand.port.WaitForResult("AT+CIPSHUT\r", ref result, new string[] { "OK" }, 10000))
            {
                SetSim900Status(result, "CIPSHUT OK");
            }
            else
            {
                SetSim900Status(result, "Can't cipshut");
                sim_error = true;
            }
        }

        private void bTCPServerSend_Click(object sender, EventArgs e)
        {
            string result = string.Empty;
            bool sim_error = false;

            bTCPServerSend.Enabled = false;

            if (parserCommand.port.WaitForResult("AT+CIPSEND=" + tbTCPServToSend.TextLength + "\r", ref result, new string[] { ">" }, 10000))
            {
                ApplicationStatus.setApplicationStatus("Ready to receive data to send");
                Console.WriteLine(result);

                if (parserCommand.port.WaitForResult(tbTCPServToSend.Text + "\r", ref result, new string[] { "SEND OK" }, 10000))
                {
                    SetSim900Status(result, "Sended");

                    if (chbTCPServCyclic.Checked)
                        bTCPServerSend_Click(this, null);
                }
                else
                {
                    SetSim900Status(result, "Can't send");
                    sim_error = true;
                }
            }
            else
            {
                SetSim900Status(result, "No waiting chart '>'");
                sim_error = true;
            }

            if (sim_error)
            {
                bTCPServDisconnect_Click(this, null); //disconnect
                bTCPServConnect_Click(this, null); //connect
                bTCPServerSend_Click(this, null); //send
            }

            bTCPServerSend.Enabled = true;
        }

        private void bSMSRead_Click(object sender, EventArgs e)
        {
            string result = string.Empty;

            if (parserCommand.port.WaitForResult("AT+CMGF=\r", ref result, new string[] { "OK" }, 1000))
            {
                ApplicationStatus.setApplicationStatus("SMS mode set");
                Console.WriteLine(result);

                if (parserCommand.port.WaitForResult("AT+CPMS=\"SM\",\"SM\",\"SM\"\r", ref result, new string[] { "OK" }, 1000))
                {
                    int idxcpms = result.LastIndexOf("+CPMS");
                    if (idxcpms != -1)
                    {
                        string rescpms = result.Substring(idxcpms);

                        int idxs = rescpms.IndexOf(":") + 2;
                        int idxe = rescpms.IndexOf(",");

                        string cntsms = rescpms.Substring(idxs, idxe - idxs);

                        int countsms = 0;
                        Int32.TryParse(cntsms, out countsms );
                        if (countsms == 0) countsms = 1;
                        nudSMSNumber.Value = countsms;
                        nudSMSNumber.Maximum = countsms;
                    }

                    SetSim900Status(result, "Loaded sms box");
                }
                else
                {
                    SetSim900Status(result, "Can't read message box");
                }
            }
            else
            {
                SetSim900Status(result, "SMS mode not set");
            }
        }

        private void bSMSReads_Click(object sender, EventArgs e)
        {
            string result = string.Empty;

            if (parserCommand.port.WaitForResult("AT+CMGR=" + nudSMSNumber.Value.ToString() + "\r", ref result, new string[] { "OK" }, 1000))
            {
                SetSim900Status(result, "SMS readed");
            }
            else
            {
                SetSim900Status(result, "SMS not read");
            }
        }

        private void bSMSErase_Click(object sender, EventArgs e)
        {
            string result = string.Empty;

            if (parserCommand.port.WaitForResult("AT+CMGDA=\"DEL " + cbSMSErase.SelectedItem.ToString() + "\"\r", ref result, new string[] { "OK" }, 1000))
            {
                SetSim900Status(result, "SMS group erased");
            }
            else
            {
                SetSim900Status(result, "SMS not erased");
            }
        }

        private void bSMSOne_Click(object sender, EventArgs e)
        {
            string result = string.Empty;

            if (parserCommand.port.WaitForResult("AT+CMGD=" + nudSMSNumber.Value.ToString() + "\r", ref result, new string[] { "OK" }, 1000))
            {
                SetSim900Status(result, "SMS erased");
            }
            else
            {
                SetSim900Status(result, "SMS not erased");
            }
        }

        private void bSMSReadGroup_Click(object sender, EventArgs e)
        {
            string result = string.Empty;

            if (parserCommand.port.WaitForResult("AT+CMGL=\"" + cbSMSReadGroup.SelectedItem.ToString().Replace("_", " ") + "\"\r", ref result, new string[] { "OK" }, 1000))
            {
                SetSim900Status(result, "SMS readed");
            }
            else
            {
                SetSim900Status(result, "SMS not readed");
            }
        }
    }
}
