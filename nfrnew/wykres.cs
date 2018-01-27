using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace nfrnew
{
    public partial class wykres : Form
    {
        delegate void ClearPointCallback(TypLini typLin);
        delegate void DisplayPointCallback(int x, int y, Color kolor, TypLini typLin);
        delegate void setStateCuUltraCallback(int stateCuUltra);

        int maxX = 650, minX = 0, minY = 1700, maxY = 2200;
        int lowPassFilterSamples = 5;

        public bool clearRaw = true;

        public Chart chWykres1 = new Chart();

        CheckBox chbPower, chbLowPassFilter, chbBasePower, chbMeasPower, chbOcuppy, chbAfaFilter;


        CheckBox[] rbStateCuUltra = new CheckBox[8]; 

        public enum TypLini {
            BasePowerData,
            PowerData,  //Dane z pomiaru

            SD_TresholdDown,
            SD_TresholdUp,

            ConstantComponentOfPowerSignalLine, //Linia składowej stałej sygnału (gdy nie nadawane jest echo) 
            VibrationsEndLine,  //Pionowa linia wyznaczająca koniec wibracji piezo
            DegreeICalibrationThresholdLine,   //Stopień 1 kalibracji - linia pozioma
            DegreeIICalibrationTresholdLine,   //Stopien 2 kalibracji - linia pozioma
            CalibrationPowerTresholdLine,       //Linia pozioma progu kalibracji mocy sygnału
            CalibrationMargin, //Linia pionowa określająca tolerację odległości od pozycji bazowej w której nie szuka się przeszkód
            MeasPowerTresholdLine,  //Linia pionowa wyznaczająca granicę pomiaru mocy sygnału
            BaseDistanceLine,    //Linia okreslająca odległość jaką widzi czujnik bez przeszkód
            DetectionIArea, //Obszar detekcji I
            DetectionIIArea, //Obszar detekcji II
            DetectedEcho,

            BasePower,
            MeasPower,
    
            LowPassFilter,
            AFAFilter,

            Ocuppy,

            OldEchoes
        }


        public wykres()
        {
            InitializeComponent();

            this.Hide();
            this.Visible = false;

            chWykres1.ChartAreas.Add("ChartArea1");
            chWykres1.ChartAreas["ChartArea1"].AxisX.Maximum = maxX;
            chWykres1.ChartAreas["ChartArea1"].AxisX.Minimum = minX;
            chWykres1.ChartAreas["ChartArea1"].AxisY.Maximum = maxY;
            chWykres1.ChartAreas["ChartArea1"].AxisY.Minimum = minY;
            chWykres1.ChartAreas["ChartArea1"].AxisX.Interval = 100;
            chWykres1.ChartAreas["ChartArea1"].AxisY.Interval = 100;

            chWykres1.DoubleClick += new EventHandler(clearChart);

            chWykres1.Series.Add(TypLini.BasePowerData.ToString());
            chWykres1.Series[TypLini.BasePowerData.ToString()].ChartType = SeriesChartType.Line;

            chWykres1.Series.Add(TypLini.SD_TresholdUp.ToString());
            chWykres1.Series[TypLini.SD_TresholdUp.ToString()].ChartType = SeriesChartType.Line;

            chWykres1.Series.Add(TypLini.SD_TresholdDown.ToString());
            chWykres1.Series[TypLini.SD_TresholdDown.ToString()].ChartType = SeriesChartType.Line;

            chWykres1.Series.Add(TypLini.PowerData.ToString());
            chWykres1.Series[TypLini.PowerData.ToString()].ChartType = SeriesChartType.Line;

            chWykres1.Series.Add(TypLini.VibrationsEndLine.ToString());
            chWykres1.Series[TypLini.VibrationsEndLine.ToString()].ChartType = SeriesChartType.Line;

            chWykres1.Series.Add(TypLini.CalibrationMargin.ToString());
            chWykres1.Series[TypLini.CalibrationMargin.ToString()].ChartType = SeriesChartType.Line;

            chWykres1.Series.Add(TypLini.LowPassFilter.ToString());
            chWykres1.Series[TypLini.LowPassFilter.ToString()].ChartType = SeriesChartType.Line;

            chWykres1.Series.Add(TypLini.AFAFilter.ToString());
            chWykres1.Series[TypLini.AFAFilter.ToString()].ChartType = SeriesChartType.Point;
            chWykres1.Series[TypLini.AFAFilter.ToString()].BorderWidth = 4;

            chWykres1.Series.Add(TypLini.OldEchoes.ToString());
            chWykres1.Series[TypLini.OldEchoes.ToString()].ChartType = SeriesChartType.Point;
            chWykres1.Series[TypLini.OldEchoes.ToString()].BorderWidth = 4;

            chWykres1.Series.Add(TypLini.BasePower.ToString());
            chWykres1.Series[TypLini.BasePower.ToString()].ChartType = SeriesChartType.Line;

            chWykres1.Series.Add(TypLini.MeasPower.ToString());
            chWykres1.Series[TypLini.MeasPower.ToString()].ChartType = SeriesChartType.Line;

            chWykres1.Series.Add(TypLini.Ocuppy.ToString());
            chWykres1.Series[TypLini.Ocuppy.ToString()].ChartType = SeriesChartType.Column;

            chWykres1.Series.Add(TypLini.DegreeIICalibrationTresholdLine.ToString());
            chWykres1.Series[TypLini.DegreeIICalibrationTresholdLine.ToString()].ChartType = SeriesChartType.Line;



            chbPower = setCheckButton(500, 20, "PWR");
            chbLowPassFilter = setCheckButton(620, 20, "LP Filter");
            chbAfaFilter = setCheckButton(740, 20, "AFA Filter");
            chbMeasPower = setCheckButton(860, 20, "Part Meas PWR");
            chbBasePower = setCheckButton(980, 20, "Part Base PWR");
            chbOcuppy = setCheckButton(1100, 20, "Ocuppy");

            int xLoc = 200;


            for (int i = 0; i < rbStateCuUltra.Length; i++)
            {
                rbStateCuUltra[i] = new CheckBox();
            }

            rbStateCuUltra[0].Text = "Ocuppy";
            rbStateCuUltra[1].Text = "Bad Park";
            rbStateCuUltra[2].Text = "Calib";
            rbStateCuUltra[3].Text = "Obj";
            rbStateCuUltra[4].Text = "Echo exceeded";
            rbStateCuUltra[5].Text = "No cal len";
            rbStateCuUltra[6].Text = "Reserv";
            rbStateCuUltra[7].Text = "Piezo Damaged";

            foreach (CheckBox r in rbStateCuUltra)
            {
                r.Size = new Size(150, 40);
                r.TextAlign = ContentAlignment.BottomCenter;
                r.Location = new Point(xLoc, 60);
                r.CheckAlign = ContentAlignment.BottomCenter;
                r.Enabled = false;
                r.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                xLoc += 150;

                this.Controls.Add(r);
            }


            chWykres1.Width = 1500;
            chWykres1.Height = 550;
            this.chWykres1.Location = new Point(50, 150);
            this.Width = 1600;
            this.Height = 800;
            this.MaximumSize = new Size(1600, 800);
            this.MinimumSize = new Size(1600, 800);
            this.Controls.Add(chWykres1);
            //this.Show();

        }

        private void clearChart(object sender, EventArgs e)
        {
            foreach(Series s in chWykres1.Series)
                s.Points.Clear();
        }


        private CheckBox setCheckButton(int X, int Y, string Text)
        {
            CheckBox chb = new CheckBox();
            chb.Location = new Point(X, Y);
            chb.Text = Text;
            chb.Checked = true;
            chb.Width = 120;
            chb.CheckedChanged += new EventHandler(chb_CheckedChanged);

            this.Controls.Add(chb);

            return chb;
        }

        private void chb_CheckedChanged(object sender, EventArgs e)
        {
            if (chbPower.Checked) chWykres1.Series[TypLini.PowerData.ToString()].Enabled = true;
            else chWykres1.Series[TypLini.PowerData.ToString()].Enabled = false;

            if (chbLowPassFilter.Checked) chWykres1.Series[TypLini.LowPassFilter.ToString()].Enabled = true;
            else chWykres1.Series[TypLini.LowPassFilter.ToString()].Enabled = false;

            if (chbAfaFilter.Checked) chWykres1.Series[TypLini.AFAFilter.ToString()].Enabled = true;
            else chWykres1.Series[TypLini.AFAFilter.ToString()].Enabled = false;

            if (chbMeasPower.Checked) chWykres1.Series[TypLini.MeasPower.ToString()].Enabled = true;
            else chWykres1.Series[TypLini.MeasPower.ToString()].Enabled = false;

            if (chbBasePower.Checked) chWykres1.Series[TypLini.BasePower.ToString()].Enabled = true;
            else chWykres1.Series[TypLini.BasePower.ToString()].Enabled = false;

            if (chbOcuppy.Checked) chWykres1.Series[TypLini.Ocuppy.ToString()].Enabled = true;
            else chWykres1.Series[TypLini.Ocuppy.ToString()].Enabled = false;
        }

        private void wykres_Load(object sender, EventArgs e)
        {
  
        }

        public string getEnabledSeries()
        {

            string enSeries = string.Empty;

            foreach (Series s in chWykres1.Series)
            {
                if(s.Enabled == true)
                    enSeries += s.ToString().Replace("Series-", "") + ",";
            }

            return "Length," + enSeries;
        }
        public Series[] getEnabledSeriesType()
        {

            Series[] enSeries = new Series[chWykres1.Series.Count];
            int cnt = 0;

            foreach (Series s in chWykres1.Series)
            {
                if (s.Enabled == true)
                    enSeries[cnt++] = s;
            }

            return enSeries;
        }


        public void makeChart() { 
        
            
        }

        public void DisplayColumn(int x, int y, Color kolor, TypLini typLin)
        {
           // this.chWykres1.Series[typLin.ToString()].Points.DataBindXY(x, y);
        }

        public void DisplayPoint(int x, int y, Color kolor, TypLini typLin)
        {

            if (this.chWykres1.InvokeRequired)
            {
                DisplayPointCallback d = new DisplayPointCallback(DisplayPoint);
                this.BeginInvoke(d, new object[] { x,  y, kolor, typLin});
            }
            else
            {
                if (y == 0 || x == 0) return;



                if (!chWykres1.Series[typLin.ToString()].Enabled) return;


                if (!chbLockY.Checked)
                {
                    if (y > maxY)
                    {
                        maxY = y;
                        chWykres1.ChartAreas["ChartArea1"].AxisY.Maximum = ((maxY / 200) + 1) * 200;
                    }

                    if (y < minY)
                    {
                        minY = y;
                        chWykres1.ChartAreas["ChartArea1"].AxisY.Minimum = (minY / 200) * 200;
                    }
                }


                if (this.chWykres1.InvokeRequired)
                {
                    DisplayPointCallback d = new DisplayPointCallback(DisplayPoint);
                    this.BeginInvoke(d, new object[] { x, y, kolor, typLin });
                }
                else
                {
                    this.chWykres1.Series[typLin.ToString()].Color = kolor;
                    this.chWykres1.Series[typLin.ToString()].Points.AddXY(x, y);
                }
            }


        }
        public void ClearPoint(TypLini typLin)
        {

            if (this.chWykres1.InvokeRequired)
            {
                ClearPointCallback d = new ClearPointCallback(ClearPoint);
                this.BeginInvoke(d, new object[] { typLin });
            }
            else
            {
                this.chWykres1.Series[typLin.ToString()].Points.Clear();
                maxY = 0;
                minY = 30000;
            }
        }


        public void DisplayVerLine(int val, Color color, TypLini typLin) 
        {
            DisplayPoint(val, (int)chWykres1.ChartAreas["ChartArea1"].AxisY.Minimum + 1, color, typLin);
            DisplayPoint(val, (int)chWykres1.ChartAreas["ChartArea1"].AxisY.Maximum - 1, color, typLin);
        }
        public void DisplayHorLine(int val, Color color, TypLini typLin)
        {
            DisplayPoint((int)chWykres1.ChartAreas["ChartArea1"].AxisX.Minimum + 1, val, color, typLin);
            DisplayPoint((int)chWykres1.ChartAreas["ChartArea1"].AxisX.Maximum - 1, val, color, typLin);
        }

        private void wykres_FormClosing(object sender, FormClosingEventArgs e)
        {
            (sender as wykres).Hide();
            e.Cancel = true;
        }

        public void showHide()
        {
            if (!this.Visible) this.Show();
            else this.Hide();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
        }
        public uint getLowPassFilterSamples() 
        {
            return (uint)lowPassFilterSamples;
        }

        private void bSave_Click(object sender, EventArgs e)
        {
          
        }

        public void setStateCuUltra(int stateCuUltra)
        {

            if (this.chWykres1.InvokeRequired)
            {
                setStateCuUltraCallback d = new setStateCuUltraCallback(setStateCuUltra);
                this.BeginInvoke(d, new object[] { stateCuUltra });
            }
            else
            {
                if ((stateCuUltra & 0x01) == 0x01) rbStateCuUltra[0].Checked = true;
                else rbStateCuUltra[0].Checked = false;

                if ((stateCuUltra & 0x02) == 0x02) rbStateCuUltra[1].Checked = true;
                else rbStateCuUltra[1].Checked = false;

                if ((stateCuUltra & 0x04) == 0x04) rbStateCuUltra[2].Checked = true;
                else rbStateCuUltra[2].Checked = false;

                if ((stateCuUltra & 0x08) == 0x08) rbStateCuUltra[3].Checked = true;
                else rbStateCuUltra[3].Checked = false;

                if ((stateCuUltra & 0x10) == 0x10) rbStateCuUltra[4].Checked = true;
                else rbStateCuUltra[4].Checked = false;

                if ((stateCuUltra & 0x20) == 0x20) rbStateCuUltra[5].Checked = true;
                else rbStateCuUltra[5].Checked = false;

                if ((stateCuUltra & 0x40) == 0x40) rbStateCuUltra[6].Checked = true;
                else rbStateCuUltra[6].Checked = false;

                if ((stateCuUltra & 0x80) == 0x80) rbStateCuUltra[7].Checked = true;
                else rbStateCuUltra[7].Checked = false;
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked) clearRaw = true;
            else clearRaw = false;
        }
        


    }
}
