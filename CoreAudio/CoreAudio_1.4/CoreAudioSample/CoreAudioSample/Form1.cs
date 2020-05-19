using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CoreAudio;
using System.Diagnostics;

namespace CoreAudioSample
{
    public partial class Form1 : Form
    {
        private MMDevice device;

        int dupCounter = 0;
        int queueCounter = 0;
        String oldValue = "";
        String highStateValue = "1";
        private NotifyIcon Tray = new NotifyIcon();
        public Form1()
        {
            InitializeComponent();
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            tbMaster.Value = (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            device.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);
            timer1.Enabled = true;
            serial_Initialize();
            Tray = new NotifyIcon();
            Tray.Icon = (Icon)global::CoreAudioSample.Properties.Resources.ResourceManager.GetObject("AudioListener");
            Tray.Text = "AlarmListener";
            Tray.DoubleClick += new System.EventHandler(this.Tray_DoubleClick);
            textBox1.Text = Convert.ToString(Properties.Settings.Default.comNum);
            textBox2.Text = Properties.Settings.Default.procName;
        }

        void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            if (this.InvokeRequired)
            {
                object[] Params = new object[1];
                Params[0] = data;
                this.Invoke(new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification), Params);
            }
            else
            {
                tbMaster.Value = (int)(data.MasterVolume * 100);
            }
        }

        private void tbMaster_Scroll(object sender, EventArgs e)
        {
            device.AudioEndpointVolume.MasterVolumeLevelScalar = ((float)tbMaster.Value / 100.0f);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pkMaster.Value = (int)(device.AudioMeterInformation.MasterPeakValue * 100);
            pkLeft.Value = (int)(device.AudioMeterInformation.PeakValues[0]*100);
            pkRight.Value = (int)(device.AudioMeterInformation.PeakValues[1]* 100);

            String currentValue = (device.AudioMeterInformation.MasterPeakValue).ToString();
            if (processExist(textBox2.Text))
            {
                //contain sound, dB > 12345-10 else, dB > 0.0
                if (!(device.AudioMeterInformation.MasterPeakValue).ToString().Contains("-"))
                {
                    if (queueCounter > 5)
                    {
                        queueCounter = 0;
                        try
                        {
                            serialPort1.WriteLine(highStateValue);
                            label7.Text = "SUCCESS : (" + DateTime.Now.ToString("mm:ss") + ") Successfully send '" + highStateValue + "'";
                        }
                        catch
                        {
                            label7.Text = "ERROR : (" + DateTime.Now.ToString("mm:ss") + ") Fail to send '" + highStateValue + "'";
                        }
                    }

                    queueCounter++;

                    if (currentValue == oldValue)
                    {
                        dupCounter++;
                    }
                    else
                    {
                        highStateValue = "1";
                        oldValue = currentValue;
                        dupCounter = 0;
                    }

                    if (dupCounter == 50)
                    {
                        dupCounter = 0;
                        highStateValue = "0";
                    }

                }
                else
                {
                    if (queueCounter > 5)
                    {
                        queueCounter = 0;
                        try
                        {
                            serialPort1.WriteLine("0");
                            label7.Text = "SUCCESS : (" + DateTime.Now.ToString("mm:ss") + ") Successfully send '0'";
                        }
                        catch
                        {
                            label7.Text = "ERROR : (" + DateTime.Now.ToString("mm:ss") + ") Fail to send '0'";
                        }
                    }
                    queueCounter++;
                }
            }
            else
            {
                if (queueCounter > 5)
                {
                    queueCounter = 0;
                    try
                    {
                        serialPort1.WriteLine("1");
                        label7.Text = "NO-PROC : (" + DateTime.Now.ToString("mm:ss") + ") Successfully send '1'";
                    }
                    catch
                    {
                        label7.Text = "NO-PROC : (" + DateTime.Now.ToString("mm:ss") + ") Fail to send '1'";
                    }
                }
                queueCounter++;
            }
        }

        private Boolean processExist(String processname)
        {
            Process[] pname = Process.GetProcessesByName(processname);
            if (pname.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void serial_Initialize()
        {
            serialPort1.BaudRate = 9600;
            serialPort1.PortName = "COM" + Convert.ToString(Properties.Settings.Default.comNum);
            serialPort1.DataBits = 8;
            try
            {
                serialPort1.Open();
            }
            catch
            {
                label7.Text = "ERROR : (" + DateTime.Now.ToString("mm:ss") + ") Cannot open Serial Port";
            }
        }

        private void serial_Send(String input_Level)
        {
            serialPort1.WriteLine(input_Level);
        }

        private void Tray_DoubleClick(object sender, EventArgs e)
        {
            this.Focus();
            ShowInTaskbar = true;
            Tray.Visible = false;
            this.WindowState = FormWindowState.Maximized;
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.BringToFront();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowInTaskbar = false;
            Tray.Visible = true;
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.comNum = Convert.ToInt32(textBox1.Text);
            Properties.Settings.Default.procName = textBox2.Text;
            Properties.Settings.Default.Save();
        }
        
    }
}