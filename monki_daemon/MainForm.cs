using monki_okpos_daemon.communcation;
using monki_okpos_daemon.config;
using monki_okpos_daemon.OKPOS;
using monki_okpos_daemon.util;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace monki_okpos_daemon
{
    public partial class frmMain : Form
    {
        MqttHelper mqttHelper = null;

        public frmMain()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            lblMQTT.Text = "MQTT : Connected";
            lblOKDC.Text = "OKDC : Connected";
            InitMQTTHelper();
            MqttStart_timer();

            notifyIcon1.Visible = true;
            this.Hide();
            this.ShowInTaskbar = false;
        }


        int _CheckMqttStatusCounter = 0;

        DispatcherTimer _timer;

        void MqttStart_timer(double interval = 1000)
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromMilliseconds(interval);
                _timer.Tick += new EventHandler(_timer_Tick);
                _timer.Start();
            }
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (mqttHelper == null) return;
            if (_CheckMqttStatusCounter >= 10 || _CheckMqttStatusCounter == 0)
            {
                CheckMqttConnection();
                _CheckMqttStatusCounter = 1;
            }
            else
            {
                _CheckMqttStatusCounter++;
            }
            if (OKDC_INFO.CONNECT)
                lblOKDC.Text = "OKDC : Connected";
            else
                lblOKDC.Text = "OKDC : Disconnected";

            if (mqttHelper != null) mqttHelper.Start_Timer();
        }


        void InitMQTTHelper()
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1000));
                if (mqttHelper == null)
                {
                    mqttHelper = new MqttHelper();
                }
            });
        }

        async void CheckMqttConnection()
        {
            if (mqttHelper == null) return;

            var result = await mqttHelper.CheckMqttStatus();
            if (result)
                lblMQTT.Text = "MQTT : Connected";
            else
                lblMQTT.Text = "MQTT : Disconnected";
        }




        private void Form1_Shown(object sender, EventArgs e)
        {
            if (Config.PosInterfaceType == POS_MODEL_TYPES.OKPOS)
            {
                OKDC_INFO.IsRegisterCallback = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ProcessClose();
        }


        private void hideToolStripMenuItem_Click(object sender, EventArgs e) => Hide();

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }
 
        private void exitToolStripMenuItem1_Click(object sender, EventArgs e) => Application.Exit();
 

        private void ProcessClose()
        {
            Stop_timer();

            if (mqttHelper != null)
            {
                mqttHelper.Disconnect();
                mqttHelper.Stop_Timer();
            }

            if (Config.PosInterfaceType == POS_MODEL_TYPES.OKPOS)
            {
                if (OKDC_INFO.IsRegisterCallback)
                {
                    OKDC_Callback.Instance.UnRegisterCallback();
                }

                OKDC_Callback.Instance.Cleanup();
            }
        }

        void Stop_timer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }


    }
}
