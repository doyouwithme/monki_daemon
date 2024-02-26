using monki_okpos_daemon.config;
using monki_okpos_daemon.OKPOS;
using monki_okpos_daemon.util;
using Sentry;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace monki_okpos_daemon
{
    static class Program
    {     

        [STAThread]
        static void Main(string[] args)
        {
            PathHelper.SetAppBaseDirectory(Config.RootDirectory);

            if (UtilHelper.GetProcessActive())
            {
                LogHelper.Instance.Info(string.Format("{0} 프로그램이 이미 실행 중입니다!!", Process.GetCurrentProcess().ProcessName));
                Environment.Exit(0);
            }


            if (args.Length != 7)
            {
                Config.API_URL = "http://10.10.239.31:9090/"; ;
                Config.MQTT_URL = "b-51bcb17d-4d28-4687-a3a4-963cbd2e5819-1.mq.ap-northeast-2.amazonaws.com";
                Config.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IjEifQ.eyJpZCI6ImxvY2FsLXN0b3JlLTE2MiIsImlhdCI6MTcwNjI1Njc2NCwiYXVkIjoibXlhdWQiLCJpc3MiOiJteWlzc3VlciIsInN1YiI6InVzZXIiLCJqdGkiOiIxIn0.wlA0NX1Mxf4HrWRl5q0he7kH_QYIq8sjyMh2eeeyQt8";
                Config.StoreNo = "162";
                Config.StoreNm = "마라통닭";
                Config.EnvironmentMode = "2";
                Config.PosInterfaceType = POS_MODEL_TYPES.OKPOS;
                //TODO 삭제
                //Environment.Exit(0);
            }
            else {
                Config.API_URL = args[0];
                Config.MQTT_URL = args[1];
                Config.Token = args[2];
                Config.StoreNo = args[3];
                Config.StoreNm = args[4];
                Config.EnvironmentMode = args[5];
                Config.PosInterfaceType = args[6];
            }


            LogHelper.Instance.Info("********************************************************************************************************");
            LogHelper.Instance.Info("                                ****   MONKI DAEMON START    ****                                       ");
            LogHelper.Instance.Info("********************************************************************************************************");
            LogHelper.Instance.Info(string.Format("monki daemon Start..."));
            LogHelper.Instance.Info(string.Format("API_URL   : {0}", Config.API_URL));
            LogHelper.Instance.Info(string.Format("MQTT_URL  : {0}", Config.MQTT_URL));
            LogHelper.Instance.Info(string.Format("StoreNo   : {0}", Config.StoreNo));
            LogHelper.Instance.Info(string.Format("StoreNm   : {0}", Config.StoreNm));
            LogHelper.Instance.Info(string.Format("Interface : {0}", Config.PosInterfaceType));
            LogHelper.Instance.Info(string.Format("Mode      : {0}", Config.EnvironmentMode));

            if (string.IsNullOrEmpty(Config.API_URL) ||
                string.IsNullOrEmpty(Config.MQTT_URL) ||
                string.IsNullOrEmpty(Config.StoreNo) ||
                string.IsNullOrEmpty(Config.Token))
            {
                LogHelper.Instance.Error("설정 정보 오류");
                Environment.Exit(0);
            }

            LogHelper.Instance.Info("********************************************************************************************************");
            LogHelper.Instance.Info("                               ****   OKDC CONNECTION INFO    ****                                      ");
            LogHelper.Instance.Info("********************************************************************************************************");
            OKDC_Request.Instance.Connect();
            LogHelper.Instance.Info("********************************************************************************************************");

            OKDC_INFO.LAST_DATE = UtilHelper.GetIniFile("OKPOS", "LAST_DATE", OKDC_INFO.INI_FILE);

            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            LogHelper.Instance.Info(string.Format("Version : {0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision, Assembly.GetEntryAssembly().GetName().Name));

            SentryInitialize();

            Application.Run(new frmMain());
        }


        static void SentryInitialize()
        {
            SentrySdk.Init(options =>
            {
                options.Dsn = "https://6d1d3894233c7f6c4285a99edc4eb2bf@o1397078.ingest.sentry.io/4505995018829824";

                // When debug is enabled, the Sentry client will emit detailed debugging information to the console.
                // This might be helpful, or might interfere with the normal operation of your application.
                // We enable it here for demonstration purposes when first trying Sentry.
                // You shouldn't do this in your applications unless you're troubleshooting issues with Sentry.
                options.Debug = true;

                // This option is recommended. It enables Sentry's "Release Health" feature.
                options.AutoSessionTracking = true;

                // Enabling this option is recommended for client applications only. It ensures all threads use the same global scope.
                options.IsGlobalModeEnabled = false;

                // This option will enable Sentry's tracing features. You still need to start transactions and spans.
                options.EnableTracing = true;

                // Example sample rate for your transactions: captures 10% of transactions
                options.TracesSampleRate = 0.1;
            });
        }
    }
}
