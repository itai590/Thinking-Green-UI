using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Timers;


namespace ListViewDemo
{

    public partial class Form1 : Form
    {
        string c0, c1, c2, c3, c4, c5, Lo;
        int state, num = 1, num_temp, gas1, gas2, la_78_en = 0, pb_cnt = 0, pb_rst = 0, fr = 1, seconds = 0, minutes = 5, sec_en = 1;

        public Form1()
        {
            InitializeComponent();
            cmb_PortName.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames()); // insert all ports
            state = 0;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            num = 1;
            la_78_en = 0;
            label7.Hide();
            label8.Hide();
        }

        private void chkConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkConnect.Checked == true)
                {
                    progressBar1.Show();
                    label4.Show();
                    comPort.PortName = cmb_PortName.Text;
                    comPort.Open();
                    chkConnect.Text = "Disconnect";
                    cmb_PortName.Enabled = false;
                }
                else
                {
                    progressBar1.Hide();
                    progressBar1.Value = progressBar1.Minimum;
                    label4.Hide();
                    label4.Text = "Recieving Data";
                    comPort.Close();
                    chkConnect.Text = "Connect";
                    cmb_PortName.Enabled = true;

                }
            }
            catch (Exception ex)
            {
                progressBar1.Hide();
                label4.Hide();
                MessageBox.Show(ex.Message);
            }
        }
        /*port selection*/
        private void comPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte data;
            ListViewItem item;

            while (comPort.BytesToRead != 0)
            {
                data = Convert.ToByte(comPort.ReadByte());
                switch (state)
                {
                    case 0:
                        if (data == 0xAA)
                            state = 1;
                        pb_cnt = 1;
                        break;
                    case 1: c2 = data.ToString();
                        state = 2;
                        break;
                    case 2: c3 = (data * 100).ToString();
                        state = 3;
                        gas1 = data * 100;
                        break;
                    case 3: c4 = data.ToString();
                        state = 4;
                        break;
                    case 4: c5 = (data * 100).ToString();
                        state = 5;
                        gas2 = data * 100;
                        break;
                    case 5: c1 = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
                        state = 6;
                        break;
                    case 6: c0 = num.ToString();
                        num++;

                        item = new ListViewItem(c0);
                        item.SubItems.Add(c1);
                        item.SubItems.Add(c2);
                        item.SubItems.Add(c3);
                        item.SubItems.Add(c4);
                        item.SubItems.Add(c5);

                        if (gas1 > 700 || gas2 > 700)
                        {
                            la_78_en = 1;
                            SystemSounds.Beep.Play();
                            item.BackColor = Color.Red;
                            item.ForeColor = Color.White;
                            num_temp = num - 1;
                            if (gas1 > 700 && gas2 > 700)
                            {
                                Lo = "1,2";
                            }
                            else if (gas1 > 700)
                            {
                                Lo = "1";
                            }
                            else Lo = "2";
                        }

                        pb_cnt = 1;

                        listView1.Invoke(new EventHandler(delegate
                        {
                            listView1.Items.Add(item);
                        }));

                        pb_rst = 1;
                        state = 0;
                        break;
                    default: break;
                }

            }
        }

        private void chkConnect_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //update runing clock
            label3.Text = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
            label2.Text = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;

            //recieving Data runing
            if (label4.Visible == true)
            {
                label4.Text = label4.Text + ".";
            }
            if (label4.Text.Equals("Recieving Data...."))
            {
                label4.Text = "Recieving Data";
            }

            //counting down clock
            if (minutes == 0 && seconds == 0)
            {
                label10.Hide();
                label9.ForeColor = Color.Green;
                label9.Text = "Gas sensors are ready to use";
                sec_en = 0;
            }
            if (seconds == 0)
            {
                seconds = 60;
                minutes = minutes - 1;
            }
            if (sec_en == 1)
            {
                seconds = seconds - 1;
                label10.Text = minutes + ":" + (seconds).ToString() + " minutes";
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //progress bar runing config
            if (pb_rst == 1)
            {
                progressBar1.Value = progressBar1.Minimum;
            }
            if (pb_cnt == 1)
            {
                progressBar1.Increment(progressBar1.Maximum / 2);
                if (fr == 1)
                {
                    progressBar1.Increment(progressBar1.Maximum / 2);
                    fr = 0;
                }
                pb_cnt = 0;
            }
            pb_rst = 0;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (la_78_en == 1)
            {
                label7.Text = "Dangerous concentration was measured at measure No." + num_temp + " (Gas " + Lo + ")";
                label7.Show();
                label8.Show();
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (sec_en == 0)
            {
                label9.Hide();
            }
        }

        private void cmb_PortName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
        /*-----init-- */
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;//start timer #1
            timer2.Enabled = true;//start timer #2
            progressBar1.Hide();
            progressBar1.Value = progressBar1.Minimum; 
           
            label4.Hide();
            label7.Hide();
            label8.Hide();
        }
    }
}