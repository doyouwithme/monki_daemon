using MonkiDaemon.communcation;
using MonkiDaemon.model.master;
using MonkiDaemon.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace MonkiDaemon.OKPOS
{
    public class OKDC_Request
    {
        public static OKDC_Request Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OKDC_Request();
                }

                return _instance;
            }
            set { _instance = value; }
        }
        private static OKDC_Request _instance;

        public Dictionary<string, string> dictErrorList = null;

        public OKDC_Request()
        {
            dictErrorList = new Dictionary<string, string>();

            dictErrorList.Add(    "0", ERROR_CODE.ERR_0000);
            dictErrorList.Add("-1001", ERROR_CODE.ERR_1001);
            dictErrorList.Add("-1002", ERROR_CODE.ERR_1002);
            dictErrorList.Add("-1003", ERROR_CODE.ERR_1003);
            dictErrorList.Add("-1004", ERROR_CODE.ERR_1004);
            dictErrorList.Add("-1005", ERROR_CODE.ERR_1005);
        }


        public int Connect()
        {
            int rc = OKDC_DLL.Dll_CheckConnect();

            if (rc == 1)
            {
                OKDC_INFO.CONNECT = true;
                if (!OKDC_INFO.IsRegisterCallback)
                {
                    LogHelper.Instance.Info(String.Format("DAEMON <- POS Request OK"));
                    rc = OKDC_Callback.Instance.RegisterCallback();
                    if (rc == 1)
                    {
                        LogHelper.Instance.Info(String.Format("DAEMON <- POS Callback OK"));
                    }
                    else
                    {
                        if (OKDC_Request.Instance.dictErrorList.ContainsKey(rc.ToString()))
                        {
                            LogHelper.Instance.Error(String.Format("DAEMON <- POS error code = {0} {1}", rc, OKDC_Request.Instance.dictErrorList[rc.ToString()]));
                        }
                    }
                }
            }
            else
            {
                OKDC_INFO.CONNECT = false;
                if (OKDC_Request.Instance.dictErrorList.ContainsKey(rc.ToString()))
                {
                    LogHelper.Instance.Error(String.Format("DAEMON <- POS error code = {0} {1}", rc, OKDC_Request.Instance.dictErrorList[rc.ToString()]));
                }
                else
                {
                    LogHelper.Instance.Error(String.Format("DAEMON <- POS error code = {0}", rc));
                }
            }
            return rc;
        }


        public string GetResponse(string json, int timeout)
        {
            if (!OKDC_INFO.CONNECT)
            {
                Connect();
                if (!OKDC_INFO.CONNECT) return "";
            }

            string response = "";
            LogHelper.Instance.Info(String.Format("DAEMON -> POS {0}", json));
            int rc = OKDC_DLL.Dll_RequestPosTimeout(json.ToString(), ref response, timeout);
            if (rc > 0)
            {
                response = response.Substring(0, rc);
                response = response.Replace((char)0, (char)32);
                response = response.Trim();
                LogHelper.Instance.Info(String.Format("DAEMON <- POS rc = {0} data = {1}", rc, response));
                return response;
            }
            else
            {
                if (OKDC_Request.Instance.dictErrorList.ContainsKey(rc.ToString()))
                {
                    LogHelper.Instance.Error(String.Format("DAEMON <- POS error code = {0} {1}", rc, OKDC_Request.Instance.dictErrorList[rc.ToString()]));
                }
                return rc.ToString();
            }
        }


        private async Task<string> GetRequestData(string json, int timeout)        
        {
            return await Task.Run(() =>
            {
                return GetResponse(json, timeout);
            });
        }



        public async Task<string> GetResponseAsync(string json, int timeout)
        {
            return await GetRequestData(json, timeout);
        }



        public async Task<string> GetConnectKey()
        {
            JObject json = new JObject();
            json.Add("COMMAND", "store/req_connkey");
            json.Add("DIRECTION", DIRECTION.EXT_TO_POS);
            json.Add("EXTERNAL_CODE", OKDC_INFO.EXTERNAL_CODE);
            json.Add("EXTERNAL_KEY", OKDC_INFO.EXTERNAL_KEY); ;
            json.Add("EXTERNAL_SERVICE", "001");

            string request = JsonConvert.SerializeObject(json, Formatting.None);
            string response = GetResponse(request, 10);

            try
            {
                if (response.Length > 5)
                {
                    var resObj = JObject.Parse(response);
                    var resultCode = resObj.GetValue("RESULT_CODE");
                    resultCode = resultCode != null ? resultCode.ToString() : "";
                    if (resultCode.ToString() == "0000")
                    {
                        var connKey = resObj.GetValue("CONNKEY");
                        var shopCode = resObj.GetValue("SHOP_CD");
                        OKDC_INFO.CONN_KEY = connKey != null ? connKey.ToString() : "";
                        OKDC_INFO.SHOP_CD = shopCode != null ? shopCode.ToString() : "";

                        string connkey = UtilHelper.GetIniFile("OKPOS", "CONN_KEY", OKDC_INFO.INI_FILE);
                        string shopcd = UtilHelper.GetIniFile("OKPOS", "SHOP_CD", OKDC_INFO.INI_FILE);

                        if (OKDC_INFO.CONN_KEY != connkey || OKDC_INFO.SHOP_CD != shopcd)
                        {
                            bool result = await RestAPIHelper.Instance.apiBranch(RestSharp.Method.POST, API_LIST.POST_CONN_KEY, response);
                            if (result)
                            {
                                FileSystemHelper.DeleteFile(OKDC_INFO.INI_FILE);
                                UtilHelper.SetIniFile("OKPOS", "CONN_KEY", OKDC_INFO.CONN_KEY, OKDC_INFO.INI_FILE);
                                UtilHelper.SetIniFile("OKPOS", "SHOP_CD", OKDC_INFO.SHOP_CD, OKDC_INFO.INI_FILE);
                                OKDC_INFO.IsFirstConnect = true;
                            }
                        }
                    }
                    else
                    {
                        OKDC_INFO.CONN_KEY = "";
                        OKDC_INFO.SHOP_CD = "";
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(string.Format("Exception : {0}", e.Message));
            }
            return response;
        }


        public async Task<string> GetStoreInfo()
        {
            JObject json = new JObject();
            json.Add("COMMAND", "store/store_info");
            json.Add("DIRECTION", DIRECTION.EXT_TO_POS);
            json.Add("EXTERNAL_CODE", OKDC_INFO.EXTERNAL_CODE);
            json.Add("CONN_KEY", OKDC_INFO.CONN_KEY);

            string request = JsonConvert.SerializeObject(json, Formatting.None);
            string response = GetResponse(request, OKDC_INFO.TIMEOUT);

            var resObj = JObject.Parse(response);
            var resultCode = resObj.GetValue("RESULT_CODE");
            resultCode = resultCode != null ? resultCode.ToString() : "";
            if (resultCode.ToString() == "0000")
            {
                var result = await RestAPIHelper.Instance.apiBranch(RestSharp.Method.POST, API_LIST.POST_STORE_INFO, response);
            }

            return response;
        }

        public string GetMasterInfo(string target)
        {
            JObject json = new JObject();
            json.Add("COMMAND", "store/master_info");
            json.Add("DIRECTION", DIRECTION.EXT_TO_POS);
            json.Add("EXTERNAL_CODE", OKDC_INFO.EXTERNAL_CODE);
            json.Add("CONN_KEY", OKDC_INFO.CONN_KEY);
            json.Add("SHOP_CD", OKDC_INFO.SHOP_CD);
            json.Add("LAST_DATE", OKDC_INFO.LAST_DATE);
            json.Add("TARGET", target);
            json.Add("COUNT_PER_PAGE", "9999");
            json.Add("CURRENT_PAGE_NUMBER", "1");

            string request = JsonConvert.SerializeObject(json, Formatting.None);
            string response = GetResponse(request, OKDC_INFO.TIMEOUT);

            var resObj = JObject.Parse(response);
            var resultCode = resObj.GetValue("RESULT_CODE");
            resultCode = resultCode != null ? resultCode.ToString() : "";
            if (resultCode.ToString() != "0000")
            {
                response = resultCode.ToString();
            }

            return response;
        }


        public async Task<bool> GetAllMasterInfo()
        {
            string currentDateTime = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            string response = "";
            response = GetMasterInfo(TARGET.PRODS);
            if (response.Length <= 5) return false;
            var PRODS = JsonConvert.DeserializeObject<MasterInfoModel>(response);

            response = GetMasterInfo(TARGET.SDACLCODE);
            if (response.Length <= 5) return false;
            var SDACLCODE = JsonConvert.DeserializeObject<MasterInfoModel>(response);

            response = GetMasterInfo(TARGET.SDACDCODE);
            if (response.Length <= 5) return false;
            var SDACDCODE = JsonConvert.DeserializeObject<MasterInfoModel>(response);

            response = GetMasterInfo(TARGET.SDSGRCODE);
            if (response.Length <= 5) return false;
            var SDSGRCODE = JsonConvert.DeserializeObject<MasterInfoModel>(response);

            response = GetMasterInfo(TARGET.SDSCLCODE);
            if (response.Length <= 5) return false;
            var SDSCLCODE = JsonConvert.DeserializeObject<MasterInfoModel>(response);

            response = GetMasterInfo(TARGET.SDSCDCODE);
            if (response.Length <= 5) return false;
            var SDSCDCODE = JsonConvert.DeserializeObject<MasterInfoModel>(response);

            response = GetMasterInfo(TARGET.TABLE);
            if (response.Length <= 5) return false;
            var TABLE = JsonConvert.DeserializeObject<MasterInfoModel>(response);

            response = GetMasterInfo(TARGET.CLSLMCODE);
            if (response.Length <= 5) return false;
            var CLSLMCODE = JsonConvert.DeserializeObject<MasterInfoModel>(response);

            response = GetMasterInfo(TARGET.CLSMMCODE);
            if (response.Length <= 5) return false;
            var CLSMMCODE = JsonConvert.DeserializeObject<MasterInfoModel>(response);

            response = GetMasterInfo(TARGET.CLSSMCODE);
            if (response.Length <= 5) return false;
            var CLSSMCODE = JsonConvert.DeserializeObject<MasterInfoModel>(response);

            response = GetMasterInfo(TARGET.TUCLSCODE);
            if (response.Length <= 5) return false;
            var TUCLSCODE = JsonConvert.DeserializeObject<MasterInfoModel>(response);

            response = GetMasterInfo(TARGET.TUKEYCODE);
            var TUKEYCODE = JsonConvert.DeserializeObject<MasterInfoModel>(response);

            response = GetMasterInfo(TARGET.TUKEYCODE_WITH_PROD);
            if (response.Length <= 5) return false;
            var TUKEYCODE_WITH_PROD = JsonConvert.DeserializeObject<MasterInfoModel>(response);

            DataModel data = new DataModel();
            MasterInfoModel MasterList = new MasterInfoModel();
            MasterList.COMMAND = PRODS.COMMAND;
            MasterList.DIRECTION = PRODS.DIRECTION;
            MasterList.EXTERNAL_CODE = PRODS.EXTERNAL_CODE;
            MasterList.RESULT_CODE = PRODS.RESULT_CODE;
            MasterList.RESULT_MSG = PRODS.RESULT_MSG;
            MasterList.SHOP_CD = PRODS.SHOP_CD;
            MasterList.CURRENT_PAGE_NUMBER = PRODS.CURRENT_PAGE_NUMBER;
            MasterList.LAST_DATE = PRODS.LAST_DATE;
            MasterList.TARGET = PRODS.TARGET;
            MasterList.COUNT_PER_PAGE = PRODS.COUNT_PER_PAGE;
            MasterList.TOTAL_PAGE_COUNT = PRODS.TOTAL_PAGE_COUNT;

            data.PRODS = PRODS.DATA.PRODS;
            data.SDACLCODE = SDACLCODE.DATA.SDACLCODE;
            data.SDACLCODE = SDACLCODE.DATA.SDACLCODE;
            data.SDACDCODE = SDACDCODE.DATA.SDACDCODE;
            data.SDSGRCODE = SDSGRCODE.DATA.SDSGRCODE;
            data.SDSCLCODE = SDSCLCODE.DATA.SDSCLCODE;
            data.SDSCDCODE = SDSCDCODE.DATA.SDSCDCODE;
            data.TABLE = TABLE.DATA.TABLE;
            data.CLSLMCODE = CLSLMCODE.DATA.CLSLMCODE;
            data.CLSMMCODE = CLSMMCODE.DATA.CLSMMCODE;
            data.CLSSMCODE = CLSSMCODE.DATA.CLSSMCODE;
            data.TUCLSCODE = TUCLSCODE.DATA.TUCLSCODE;
            data.TUKEYCODE = TUKEYCODE.DATA.TUKEYCODE;
            data.TUKEYCODE_WITH_PROD = TUKEYCODE_WITH_PROD.DATA.TUKEYCODE_WITH_PROD;
            MasterList.DATA = data;

            string json = JsonConvert.SerializeObject(MasterList, Formatting.None);
            bool result = await RestAPIHelper.Instance.apiBranch(RestSharp.Method.POST, API_LIST.POST_MASTER_INFO, json);
            if (result)
            {
                OKDC_INFO.LAST_DATE = currentDateTime;
                UtilHelper.SetIniFile("OKPOS", "LAST_DATE", OKDC_INFO.LAST_DATE, OKDC_INFO.INI_FILE);                
            }

            return true;
        }


        public string table_state_info(string tableCode)
        {
            JObject json = new JObject();
            JObject data = new JObject();
            json.Add("COMMAND", "table/table_state_info");
            json.Add("DIRECTION", DIRECTION.EXT_TO_POS);
            json.Add("EXTERNAL_CODE", OKDC_INFO.EXTERNAL_CODE);
            json.Add("CONN_KEY", OKDC_INFO.CONN_KEY);
            json.Add("SHOP_CD", OKDC_INFO.SHOP_CD);

            data.Add("TABLE_CD",tableCode); //테이블코드(전체 테이블 조회시는 빈값사용)
            json.Add("DATA", data);

            return GetResponse(json.ToString(), OKDC_INFO.TIMEOUT);
        }


    }
}
