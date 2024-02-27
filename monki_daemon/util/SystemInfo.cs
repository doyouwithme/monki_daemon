using MonkiDaemon.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonkiDaemon.util
{
    public class SystemInfo
    {
        public string OS { get; private set; } = string.Empty;

        public string Processor { get; private set; } = string.Empty;

        public string PhysicalMemory { get; private set; } = string.Empty;

        public string IE { get; private set; } = string.Empty;

        public EnvChecker.ApplicationType ApplicationBrand { get; private set; } = EnvChecker.ApplicationType.POSFEED;

        public Version CurrentVersion { get; private set; } = new Version();

        public string UserSettingSystemInfo { get; private set; } = string.Empty;

        public bool IsKakaoMapView => Environment.OSVersion.Version.Major >= 6 && WBEmulator.GetBrowserEmulationVersion() >= BrowserEmulationVersion.Version9;

        public SystemInfo() => Task.Run((Func<Task>)(async () =>
        {
            await Task.Delay(1);
            if (!WBEmulator.IsBrowserEmulationSet())
                WBEmulator.SetBrowserEmulationVersion();
            this.CurrentVersion = EnvChecker.CurrentVersion;
            this.OS = EnvChecker.CurrentOS;
            string str = await this.FindLocalIP();
            this.Processor = str + " / " + EnvChecker.Processor;
            str = (string)null;
            this.PhysicalMemory = string.Format("{0:N0}MB", (object)(EnvChecker.PhysicalMemory / 1024));
            this.IE = WBEmulator.GetBrowserEmulationVersion().ToString();
            this.ApplicationBrand = EnvChecker.ApplicationBrand;
            this.UserSettingSystemInfo = string.Format("{0} / {1}", (object)this.OS, (object)this.CurrentVersion).Trim();
            //SystemInfo.Logger.Info("Environment information");
            //SystemInfo.Logger.Info("OS: " + this.OS);
            //SystemInfo.Logger.Info("Processor: " + this.Processor);
            //SystemInfo.Logger.Info("PhysicalMemory: " + this.PhysicalMemory);
            //SystemInfo.Logger.Info("IE Version: " + this.IE);
            //SystemInfo.Logger.Info(string.Format("CurrentVersion: {0}", (object)this.CurrentVersion));
            //SystemInfo.Logger.Info(string.Format("Application: {0}", (object)this.ApplicationBrand));
        }));

        private async Task<string> FindLocalIP()
        {
            string ip = string.Empty;
            try
            {
                ip = await this.FindPublicIP();
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(String.Format("Except {0}", e));
            }
            finally
            {
                ip = Regex.Match(ip, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}").Value;
                if (string.IsNullOrEmpty(ip))
                    ip = this.FindPrivateIP();
            }
            string localIp = ip;
            ip = (string)null;
            return localIp;
        }

        private async Task<string> FindPublicIP()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://api.ipify.org");
            client.Timeout = TimeSpan.FromSeconds(2.0);
            string stringAsync = await client.GetStringAsync("/");
            client = (HttpClient)null;
            return stringAsync;
        }

        private string FindPrivateIP()
        {
            string empty = string.Empty;
            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    empty = address.ToString();
                    break;
                }
            }
            return empty;
        }
    }
}
