using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nfrnew.lib
{
    class ATCommand
    {

        public delegate void Wskaznik(string a);

        public class AT
        {
            public string Name;
            public bool Enabled;
            public bool ModifyInAutomaticMode;
            public Wskaznik f;

            //public string WriteConfing;
            //public bool flagChange;

            public AT(bool Enabled, bool ModifyInAutomaticMode, string Name, Wskaznik sf)
            {
                this.Name = Name;
                this.Enabled = Enabled;
                this.ModifyInAutomaticMode = ModifyInAutomaticMode;

                f = new Wskaznik(sf);
            }
            public void wyswietl(int a)
            {

            }

        }
        public class cAT
        {

            public enum eAtName
            {

                AT_CONF,
                AT_ENAA,
                AT_RXADDR,
                AT_SETUPAW,
                AT_SETUPRETR,
                AT_RFCH,
                AT_RFSETUP,
                AT_RXPW,
                AT_DYPD,
                AT_FEATURE,
                AT_TX_ACK,
                AT_RX_A,
                AT_UACK,
                AT_ECHO,
                AT_RESTART,
                AT_RELOAD,

                AT_ALL,
                AT_DFT,
                AT_RES,
            };
            public string[] sAtName = {

                "AT+CONF",
                "AT+ENAA",
                "AT+RXADDR",
                "AT+SETUPAW",
                "AT+SETUPRETR",
                "AT+RFCH",
                "AT+RFSETUP",
                "AT+RXPW",
                "AT+DYPD",
                "AT+FEATURE",
                "AT+TX_ACK",
                "AT+RX_A",
                "AT+UACK",
                "AT+ECHO",
                "AT+RESTART",
                "AT+RELOAD",

                //NIESPRAWDZANE PRZEZ ATPARSE
                "AT+ALL",
                "AT+DFT",
                "AT+RES"
                

            };

            public AT[] ATW;


            //public string At_Name;
            public const int MAX_ATW = 16;

            public cAT(int ile)
            {
                ATW = new AT[ile];


            }
        }
        public class NFR
        {
            public bool FlagUpdate;
           // public bool FlagWrite;

            public int zCONFIG;
            public int zEN_AA;
            public int zEN_RXADDR;
            public int zSETUP_AW;
            public int zSETUP_RETR;
            public int zRF_CH;
            public int zRF_SETUP;
            public int zRX_PW_P0;
            public int zDYNPD;
            public int zFEATURE;

            public string[] zRX_ADDRESS = new string[6];
            public string zTX_ADDRESS;

            public int zUEcho;
            public int zUAck;

            //public bool FlagReadEnd;

            public void fAtConfig(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zCONFIG = Convert.ToInt32(Settings);
                }
            }
            public void fAtEnAA(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zEN_AA = Convert.ToInt32(Settings);
                }
            }
            public void fAtEnRxAddress(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zEN_RXADDR = Convert.ToInt32(Settings);
                }
            }
            public void fAtSetupAw(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zSETUP_AW = Convert.ToInt32(Settings);
                }
            }
            public void fAtSetupRetr(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zSETUP_RETR = Convert.ToInt32(Settings);
                }
            }
            public void fAtRfCh(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zRF_CH = Convert.ToInt32(Settings);
                }
            }
            public void fAtRfSetup(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zRF_SETUP = Convert.ToInt32(Settings);
                }
            }
            public void fAtRxPw(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zRX_PW_P0 = Convert.ToInt32(Settings);
                }
            }
            public void fAtDyPd(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zDYNPD = Convert.ToInt32(Settings);
                }
            }
            public void fAtFeature(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zFEATURE = Convert.ToInt32(Settings);
                }
            }
            public void fAtRxAddress(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zRX_ADDRESS[0] = Settings;
                }
            }
            public void fAtTxAddress(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zTX_ADDRESS = Settings;
                    this.FlagUpdate = true;
                }
            }
            public void fAt_echo(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zUEcho = Convert.ToInt32(Settings);
                }
            }
            public void fAt_uart_ack(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.zUAck = Convert.ToInt32(Settings);
                }
            }
            public void fAtPlus(string Settings)
            {

                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    // this.FlagUpdate = true;
                }
            }
            public void fAtReset(string Settings)
            {
                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.FlagUpdate = true;
                }
            }
            public void fAtReload(string Settings)
            {
                if (Settings[0] == 'R')
                {
                    Settings = Settings.Trim('R');
                    this.FlagUpdate = true;
                }
            }
        }

        //KOMENDY AT
        /*private void AT_init()
        {
            AT_Command.At_Name = string.Empty;
            AT_Command.ATW[(int)cAT.eAtName.AT_CONF] = new AT(true, true, "AT+CONF", MyNFR.fAtConfig);
            AT_Command.ATW[(int)cAT.eAtName.AT_ENAA] = new AT(true, true, "AT+ENAA", MyNFR.fAtEnAA);
            AT_Command.ATW[(int)cAT.eAtName.AT_RXADDR] = new AT(true, true, "AT+RXADDR", MyNFR.fAtEnRxAddress);
            AT_Command.ATW[(int)cAT.eAtName.AT_SETUPAW] = new AT(true, true, "AT+SETUPAW", MyNFR.fAtSetupAw);
            AT_Command.ATW[(int)cAT.eAtName.AT_SETUPRETR] = new AT(true, true, "AT+SETUPRETR", MyNFR.fAtSetupRetr);
            AT_Command.ATW[(int)cAT.eAtName.AT_RFCH] = new AT(true, true, "AT+RFCH", MyNFR.fAtRfCh);
            AT_Command.ATW[(int)cAT.eAtName.AT_RFSETUP] = new AT(true, true, "AT+RFSETUP", MyNFR.fAtRfSetup);
            AT_Command.ATW[(int)cAT.eAtName.AT_RXPW] = new AT(true, true, "AT+RXPW", MyNFR.fAtRxPw);
            AT_Command.ATW[(int)cAT.eAtName.AT_DYPD] = new AT(true, true, "AT+DYPD", MyNFR.fAtDyPd);
            AT_Command.ATW[(int)cAT.eAtName.AT_FEATURE] = new AT(true, true, "AT+FEATURE", MyNFR.fAtFeature);
            AT_Command.ATW[(int)cAT.eAtName.AT_TX_ACK] = new AT(true, true, "AT+TX_A", MyNFR.fAtTxAddress);
            AT_Command.ATW[(int)cAT.eAtName.AT_RX_A] = new AT(true, true, "AT+RX_A", MyNFR.fAtRxAddress);
            AT_Command.ATW[(int)cAT.eAtName.AT_UACK] = new AT(true, true, "AT+UACK", MyNFR.fAt_uart_ack);
            AT_Command.ATW[(int)cAT.eAtName.AT_ECHO] = new AT(true, true, "AT+ECHO", MyNFR.fAt_echo);
            AT_Command.ATW[(int)cAT.eAtName.AT_RESTART] = new AT(true, true, "AT+RESTART", MyNFR.fAtReset);
            AT_Command.ATW[(int)cAT.eAtName.AT_RELOAD] = new AT(true, true, "AT+RELOAD", MyNFR.fAtReload);
        }*/
        /*private string AT_CQ(String ATCode)
        {
            AT_Command.At_Name = string.Empty;
            char[] Temp = new char[30];


            int wsk_NewValue = ATCode.IndexOf("=");
            int wsk_NewQuestion = ATCode.IndexOf("?");


            //Jezeli wystepuje znak '='
            if (wsk_NewValue != -1 && wsk_NewValue < 15)
            {
                ATCode.CopyTo(0, Temp, 0, wsk_NewValue);
                string[] words = ATCode.Split('=');

                AT_Command.At_Name = words[0];


                words[1].Replace('.', '1');

                string Score = "R" + words[1];

                return Score;
            }
            else if (wsk_NewQuestion != -1)
            {
                //Jezeli wystepuje znak '?'
                return "Q";
            }
            else
            {
                //Jesli nie odnaleziono znakow specyficznych
                return "N";
            }
        }
        void AT_PARSE(string At_Code)
        {
            int i;
            string Data = string.Empty;



            if (At_Code == string.Empty) return;

            // if (At_Code == "+") MyWozek1.ReceivedCommand++;

            //Liczba po '=' lub Status
            Data = AT_CQ(At_Code);


            for (i = 0; i < AT_Command.ATW.Length; i++)
            {
                if (AT_Command.ATW[i].Enabled == true && AT_Command.At_Name == AT_Command.ATW[i].Name)
                {
                    //if(TrybRecznyOrAutomatyczny == TRYB_AUTOMATYCZNY && ATW_List_Code[i].ModifyInAutomaticMode != TRUE) break;

                    AT_Command.ATW[i].f(Data);

                    break;

                }
            }
        }*/

    }
}
