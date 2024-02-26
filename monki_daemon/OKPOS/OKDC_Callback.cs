using monki_okpos_daemon.communcation;
using monki_okpos_daemon.util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace monki_okpos_daemon.OKPOS
{
    public class OKDC_Callback
    {
        ConcurrentQueue<string> CallBackMessageQueue = null;

        static OKDC_DLL.CallbackProcServer CallbackProcServer;

        public event EventHandler DoWorkEventHandler;

        public class DoWorkEventArgs : EventArgs
        {
            public int length;
            public string data;
            public DoWorkEventArgs(int length, string data)
            {
                this.length = length;
                this.data = data;
            }
        }


        public static OKDC_Callback Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OKDC_Callback();
                }

                return _instance;
            }
            set { _instance = value; }
        }
        private static OKDC_Callback _instance;


        public OKDC_Callback()
        {
            CallBackMessageQueue = new ConcurrentQueue<string>();
            DoWorkEventHandler += new EventHandler(EntryQueue);
            Start_Timer();
        }


        public void CalledFunction(int length, string data)
        {
            if (DoWorkEventHandler != null)
            {
                DoWorkEventArgs args = new DoWorkEventArgs(length, data);
                DoWorkEventHandler(null, args);
            }
        }


        private void EntryQueue(object sender, EventArgs e)
        {
            string data = ((DoWorkEventArgs)e).data;
            if (!string.IsNullOrEmpty(data))
            {
                data = data.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Trim();
                CallBackMessageQueue.Enqueue(data);
            }            
        }


        public int RegisterCallback()
        {
            OKDC_INFO.IsRegisterCallback = false;
            CallbackProcServer = new OKDC_DLL.CallbackProcServer(CalledFunction);

            GCHandle GCHandle = GCHandle.Alloc(CallbackProcServer);
            IntPtr lpCallbackFunc = Marshal.GetFunctionPointerForDelegate(CallbackProcServer);

            int rc = OKDC_DLL.Dll_RegServerCallback(lpCallbackFunc);
            if (rc == 1)
            {
                OKDC_INFO.IsRegisterCallback = true;
            }

            GCHandle.Free();

            return rc;
        }

        public bool UnRegisterCallback()
        {
            int rc = OKDC_DLL.Dll_UNRegServerCallback();

            if (rc == 1)
            {
                LogHelper.Instance.Info(String.Format("Callback Unregister OK {0} ", rc));
                OKDC_INFO.IsRegisterCallback = false;
            }

            return OKDC_INFO.IsRegisterCallback;
        }


        int callbackCount = 0;

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

            if (callbackCount == 60)
            {
                callbackCount = 0;
                if (!OKDC_INFO.IsRegisterCallback)
                {
                    OKDC_Callback.Instance.RegisterCallback();
                }
            }
            else
            {
                callbackCount++;
            }
        }


        async void Process()
        {
            try
            {
                if (!OKDC_INFO.CONNECT)
                {
                    if (callbackCount == 0 || callbackCount == 60)
                    {
                        callbackCount = 1;
                        OKDC_Request.Instance.Connect();
                        if (!OKDC_INFO.CONNECT) return;
                    }
                    else
                    {
                        callbackCount++;
                        return;
                    }
                }


                //Check CONN KEY
                if (string.IsNullOrEmpty(OKDC_INFO.CONN_KEY))
                {
                    if (callbackCount == 0 || callbackCount == 60)
                    {
                        callbackCount = 1;
                        var connkey = OKDC_Request.Instance.GetConnectKey();
                        if (string.IsNullOrEmpty(OKDC_INFO.CONN_KEY)) return;
                    }
                    else
                    {
                        callbackCount++;
                        return;
                    }
                }

                if (OKDC_INFO.IsFirstConnect)
                {
                    OKDC_INFO.LAST_DATE = "";
                    bool result = await OKDC_Request.Instance.GetAllMasterInfo();
                    if (result)
                    {
                        OKDC_INFO.IsFirstConnect = false;
                    }
                }


                while (!CallBackMessageQueue.IsEmpty)
                {
                    string message;
                    CallBackMessageQueue.TryDequeue(out message);
                    LogHelper.Instance.Info(String.Format("DAEMON <- POS Callback {0} ", message));
                    message = message.TrimEnd((char)10);
                    await ParseCommand(message);
                }
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(string.Format("Exception : {0}", e.Message));
            }
        }



        private async Task<bool> MasterInfoProcess(JObject resObj)
        {
            var data = resObj.GetValue("DATA");
            data = data != null ? data.ToString() : "";

            if (!string.IsNullOrEmpty(data.ToString()))
            {
                resObj = JObject.Parse(data.ToString());
                var deleteFlag = resObj.GetValue("DELETE_YN");
                deleteFlag = deleteFlag != null ? deleteFlag.ToString() : "";
                if (deleteFlag.ToString() == "Y")
                {
                    OKDC_INFO.LAST_DATE = "";
                }
                var result = await OKDC_Request.Instance.GetAllMasterInfo();
                if (result) return true;
            }
            return false;
        }


        private async Task<bool> CheckOKDCStatus(JObject resObj)
        {
            var status = resObj.GetValue("STATUS");
            status = status != null ? status.ToString() : "";
            if (status.ToString() == OKDC_STATUS.START)
            {
                var connKey = await OKDC_Request.Instance.GetConnectKey();
                var storeInfo = await OKDC_Request.Instance.GetStoreInfo();
                if (connKey != null && storeInfo != null) return true;
            }
            return false;
        }

        private async Task ParseCommand(string response)
        {
            var resObj = JObject.Parse(response);
            var command = resObj.GetValue("COMMAND");
            command = command != null ? command.ToString() : "";

            try
            {
                switch (command.ToString().ToLower())
                {
                    case "callback/order/sale/all":
                        var result = await RestAPIHelper.Instance.apiBranch(RestSharp.Method.POST, API_LIST.POST_SALE_ALL, response);
                        break;
                    case "callback/order/sale/partial":
                        result = await RestAPIHelper.Instance.apiBranch(RestSharp.Method.POST, API_LIST.POST_SALE_PARTIAL, response);
                        break;
                    case "callback/order/return/all":
                        result = await RestAPIHelper.Instance.apiBranch(RestSharp.Method.POST, API_LIST.POST_RETURN_ALL, response);
                        break;
                    case "callback/order/sale/cancel":
                        result = await RestAPIHelper.Instance.apiBranch(RestSharp.Method.POST, API_LIST.POST_SALE_CANCEL, response);
                        break;
                    case "callback/order/change":
                        result = await RestAPIHelper.Instance.apiBranch(RestSharp.Method.POST, API_LIST.POST_ORDER_CHANGE, response);
                        break;
                    case "callback/master_change":
                        result = await MasterInfoProcess(resObj);
                        break;
                    case "callback/table_change/move":
                        result = await RestAPIHelper.Instance.apiBranch(RestSharp.Method.POST, API_LIST.POST_TABLE_CHANGE, response);
                        break;
                    case "callback/sold_out/change":
                        result = await RestAPIHelper.Instance.apiBranch(RestSharp.Method.POST, API_LIST.POST_SOLDOUT_CHANGE, response);
                        break;
                    case "callback/req_policy":
                        result = await RestAPIHelper.Instance.apiBranch(RestSharp.Method.POST, API_LIST.POST_REQ_POLICY, response);
                        break;
                    case "callback/status":
                        result = await CheckOKDCStatus(resObj);
                        break;
                    default:
                        break;
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
