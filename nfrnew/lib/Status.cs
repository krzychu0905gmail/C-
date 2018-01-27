﻿using System;
using System.Timers;
using System.Windows.Forms;

namespace nfrnew.lib
{
    class Status :  Form
    {
        private Label tsAplicationStatus;
        private System.Timers.Timer TimStatus = new System.Timers.Timer();

        public Status(Label _toolStrip)
        {
            tsAplicationStatus = _toolStrip;

            TimStatus.Interval = 3000;
            TimStatus.Elapsed += new ElapsedEventHandler(TimerEventStatus);//new EventHandler(TimerEventStatus);
        }

        public void setApplicationStatus(string status)
        {
            if (status == tsAplicationStatus.Text) return;

            tsAplicationStatus.Text = status;
            TimStatus.Enabled = true;
        }

        private void TimerEventStatus(object sender, EventArgs e)
        {
            try
            {
                this.tsAplicationStatus.Invoke(
                     new Action(() =>
                     {
                         tsAplicationStatus.Text = "Ready";

                     }));
                TimStatus.Enabled = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
         }
    }
}
