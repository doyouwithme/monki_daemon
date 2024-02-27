using MonkiDaemon.config;
using MonkiDaemon.util;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MonkiDaemon.communcation
{
    public class RestAPIHelper
    {
        public static RestAPIHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RestAPIHelper();
                }

                return _instance;
            }
            set { _instance = value; }
        }
        private static RestAPIHelper _instance;

        IRestClient restApiClient = null;
        private CookieContainer cookieJar = null;

        public RestAPIHelper()
        {
            restApiClient = new RestClient(Config.API_URL);
            cookieJar = new CookieContainer();
            restApiClient.CookieContainer = cookieJar;
        }


        public async Task<string> apiProcess(Method method, string recource, object obj)
        {
            return await Task.Run(() =>
            {
                IRestRequest request = new RestRequest(recource, method);
                request.AddHeader("Authorization", "Bearer " + Config.Token);

                string json = obj.ToString();
                request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);

                try
                {
                    LogHelper.Instance.Info(string.Format("DAEMON -> AWS : {0}", request.Resource));

                    IRestResponse response = restApiClient.Execute(request);
                    if (response != null && response.Content != null)
                    {
                        if (response.IsSuccessful)
                        {
                            LogHelper.Instance.Info(string.Format("DAEMON <- AWS : {0}", response.Content));
                            return response.Content.ToString();
                        }
                    }
                    if (response.Content != null && response.ErrorMessage != null)
                        LogHelper.Instance.Error(string.Format("DAEMON <- AWS : {0}{1}", response.Content, response.ErrorMessage));
                    else if (response.Content != null)
                        LogHelper.Instance.Error(string.Format("DAEMON <- AWS : {0}", response.Content));
                }
                catch (Exception e)
                {
                    LogHelper.Instance.Error(string.Format("Exception : {0}", e.Message));
                    UtilHelper.SentryException(e);
                }
                finally
                {
                }
                return "";
            });
        }



        public async Task<bool> apiBranch(Method method, string recource, object obj)
        {
            string response = await apiProcess(method, recource, obj);

            if (string.IsNullOrEmpty(response))
            {
                return false;
            }

            return true;
        }

    }
}
