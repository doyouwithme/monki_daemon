using monki_okpos_daemon.config;
using monki_okpos_daemon.model;
using monki_okpos_daemon.OKPOS;
using monki_okpos_daemon.util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace monki_okpos_daemon.communcation
{
    class MqttHelper
    {
        MqttClient MqttClient;
        ConcurrentQueue<string> MqttQueue = null;


        string userName { get; set; } = "client";
        string password { get; set; } = "1234qwer!@#$";
        string topic { get; set; } 
        string clientId { get; set; }

        public bool IsConnected
        {
            get
            {
                return MqttClient.IsConnected;
            }
        }

        public MqttHelper()
        {
            getMqttInfo();
            initMqtt();
        }

        void getMqttInfo()
        {
            MqttQueue = new ConcurrentQueue<string>();
            if (Config.EnvironmentMode == "DEV")
            {
                userName = "client";
                password = "1234qwer!@#$";
            }

            topic = string.Format("/monki/{0}/notification/#", Config.StoreNo);
            clientId = string.Format("client_{0}", UtilHelper.GetMacAddress());
        }

        async void initMqtt()
        {
            _ = await MqttConnection();
        }


        private async Task<bool> MqttConnection()
        {
            return await Task.Run(() =>
            {

                LogHelper.Instance.Info("********************************************************************************************************");
                LogHelper.Instance.Info("                                    ****   MQTT START    ****                                           ");
                LogHelper.Instance.Info("********************************************************************************************************");


                try
                {
                    MqttClient = new MqttClient(Config.MQTT_URL, 8883, true, null, null, MqttSslProtocols.None);

                    MqttClient.ProtocolVersion = MqttProtocolVersion.Version_3_1_1;
                    MqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
                    MqttClient.ConnectionClosed += MqttClient_MqttclientConnectionClosed;
                    MqttClient.Connect(clientId, userName, password, true, 300);

                    if (MqttClient.IsConnected == false)
                    {
                        LogHelper.Instance.Info("                                ****    NOT CONNECTION   ****                                           ");
                        LogHelper.Instance.Info("********************************************************************************************************");
                        return false;
                    }

                    MqttClient.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                    LogHelper.Instance.Info("********************************************************************************************************");
                    LogHelper.Instance.Info("                               ****    MQTT CONNECTION OK   ****                                        ");
                    LogHelper.Instance.Info(string.Format(">  clientId:{0}    topic:{1}", clientId, topic));
                    LogHelper.Instance.Info("********************************************************************************************************");

                    return true;
                }
                catch (Exception e)
                {
                    LogHelper.Instance.Error(string.Format("> mqtt connect failed : {0}", e.Message));
                    LogHelper.Instance.Info("********************************************************************************************************");
                    UtilHelper.SentryException(e);
                }
                return false;
            });
        }


        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            return;

            if (e == null || e.Message == null) return;

            string message = "";

            try
            {
                LogHelper.Instance.Info(">>> table mqtt message received");

                message = BaseTypeHelper.ByteToString(e.Message);
                if (string.IsNullOrEmpty(message))
                {
                    LogHelper.Instance.Info(string.Format(">>> table mqtt Publish Message : {0}", message));
                    return;
                }

                string msgId = BaseTypeHelper.GetPropertyValueInJson(message, "msgId");
                string notification = BaseTypeHelper.GetPropertyValueInJson(message, "notification");

                if (!string.IsNullOrEmpty(notification))
                {
                    LogHelper.Instance.Info("********************************************************************************************************");
                    LogHelper.Instance.Info("                                             NOTIFICATION - " + notification);
                    LogHelper.Instance.Info("********************************************************************************************************");
                    LogHelper.Instance.Info(string.Format("*** : {0}", message));

                    //requestQueue.Enqueue(message);
                    LogHelper.Instance.Info("********************************************************************************************************");
                }

            }
            catch (Exception m)
            {
                LogHelper.Instance.Error(string.Format("Exception.Message : {0}, Received data : {1}", m.Message, message));
                UtilHelper.SentryException(m);
            }
        }


        public void Disconnect()
        {
            if (MqttClient == null) return;

            try
            {
                if (MqttClient.IsConnected)  MqttClient.Disconnect();
                if (MqttClient != null) MqttClient = null;

                LogHelper.Instance.Info("********************************************************************************************************");
                LogHelper.Instance.Info("                                 ****   MQTT DISCONNECT    ****                                         ");
                LogHelper.Instance.Info("********************************************************************************************************");
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(string.Format("Disconnect failed : {0}", e.Message));
            }
        }

        private void MqttClient_MqttclientConnectionClosed(object sender, EventArgs e)
        {

        }


        public async Task<bool> CheckMqttStatus()
        {
            if (MqttClient == null) return false;

            if (MqttClient.IsConnected) return true;

            bool result = await MqttConnection();

            return result;
        }

        int connectCount = 0;
        System.Windows.Threading.DispatcherTimer _processTimer;

        public void Start_Timer(double interval = 1000)
        {
            if (_processTimer == null)
            {
                _processTimer = new System.Windows.Threading.DispatcherTimer();
                _processTimer.Interval = TimeSpan.FromMilliseconds(interval);
                _processTimer.Tick += new EventHandler(_timer_Tick);
                _processTimer.Start();
            }
        }



        public void Stop_Timer()
        {
            if (_processTimer != null)
            {
                _processTimer.Stop();
                _processTimer = null;
            }
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            Process();
        }


        void Process()
        {
            try
            {
                if (Config.PosInterfaceType == POS_MODEL_TYPES.OKPOS)
                {
                    if (!OKDC_INFO.CONNECT)
                    {
                        if (connectCount == 0 || connectCount == 60)
                        {
                            connectCount = 1;
                            OKDC_Request.Instance.Connect();
                            if (!OKDC_INFO.CONNECT) return;
                        }
                        else
                        {
                            connectCount++;
                            return;
                        }
                    }
                

                    //Check CONN KEY
                    if (string.IsNullOrEmpty(OKDC_INFO.CONN_KEY))
                    {
                        if (connectCount == 0 || connectCount == 60)
                        {
                            connectCount = 1;
                            var connkey = OKDC_Request.Instance.GetConnectKey();
                            if (string.IsNullOrEmpty(OKDC_INFO.CONN_KEY)) return;
                        }
                        else
                        {
                            connectCount++;
                            return;
                        }
                    }
                }

                while (!MqttQueue.IsEmpty)
                {
                    string message;
                    MqttQueue.TryDequeue(out message);
                    if (Config.PosInterfaceType == POS_MODEL_TYPES.OKPOS) { 
                         var response = OKDC_Request.Instance.GetResponseAsync(message, 5);

                        try
                        {
                            RequestModel obj = new RequestModel();
                            obj.message = response.ToString();
                            string recource = string.Format("/proxy/{0}/request", Config.StoreNo);
                            //var result = RestAPIHelper.Instance.apiBranch(RestSharp.Method.POST, recource, obj);
                        }
                        catch (Exception e)
                        {
                            LogHelper.Instance.Error(string.Format("Exception : {0}", e.Message));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(string.Format("Exception : {0}", e.Message));
            }
        }

        public void Cleanup()
        {
            Stop_Timer();
        }

    }
}
