using Microsoft.Win32;
using monki_okpos_daemon.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkiDaemon.util
{
    public static class WBEmulator
    {
        private static readonly string InternetExplorerRootKey = "Software\\Microsoft\\Internet Explorer";
        private static readonly string BrowserEmulationKey = WBEmulator.InternetExplorerRootKey + "\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";

        public static int GetInternetExplorerMajorVersion()
        {
            int result = 0;
            try
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(WBEmulator.InternetExplorerRootKey);
                if (registryKey != null)
                {
                    object obj = registryKey.GetValue("svcVersion", (object)null) ?? registryKey.GetValue("Version", (object)null);
                    if (obj != null)
                    {
                        string str = obj.ToString();
                        int length = str.IndexOf('.');
                        if (length != -1)
                            int.TryParse(str.Substring(0, length), out result);
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(String.Format("Except {0}", e));
            }
            return result;
        }

        public static BrowserEmulationVersion GetBrowserEmulationVersion()
        {
            BrowserEmulationVersion emulationVersion = BrowserEmulationVersion.Default;
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(WBEmulator.BrowserEmulationKey, true);
                if (registryKey != null)
                {
                    string fileName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                    object obj = registryKey.GetValue(fileName, (object)null);
                    if (obj != null)
                        emulationVersion = (BrowserEmulationVersion)Convert.ToInt32(obj);
                }
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(String.Format("Except {0}", e));
            }
            return emulationVersion;
        }

        public static bool SetBrowserEmulationVersion(BrowserEmulationVersion browserEmulationVersion)
        {
            bool flag = false;
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(WBEmulator.BrowserEmulationKey, true);
                if (registryKey == null)
                    Registry.CurrentUser.CreateSubKey(WBEmulator.BrowserEmulationKey);
                if (registryKey != null)
                {
                    string fileName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                    if (browserEmulationVersion != 0)
                        registryKey.SetValue(fileName, (object)(int)browserEmulationVersion, RegistryValueKind.DWord);
                    else
                        registryKey.DeleteValue(fileName, false);
                    flag = true;
                }
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(String.Format("Except {0}", e));
            }
            return flag;
        }

        public static bool SetBrowserEmulationVersion()
        {
            int explorerMajorVersion = WBEmulator.GetInternetExplorerMajorVersion();
            BrowserEmulationVersion browserEmulationVersion;
            if (explorerMajorVersion >= 11)
            {
                browserEmulationVersion = BrowserEmulationVersion.Version11;
            }
            else
            {
                switch (explorerMajorVersion - 8)
                {
                    case 0:
                        browserEmulationVersion = BrowserEmulationVersion.Version8;
                        break;
                    case 1:
                        browserEmulationVersion = BrowserEmulationVersion.Version9;
                        break;
                    case 2:
                        browserEmulationVersion = BrowserEmulationVersion.Version10;
                        break;
                    default:
                        browserEmulationVersion = BrowserEmulationVersion.Version7;
                        break;
                }
            }
            return WBEmulator.SetBrowserEmulationVersion(browserEmulationVersion);
        }

        public static bool IsBrowserEmulationSet()
        {
            BrowserEmulationVersion emulationVersion = WBEmulator.GetBrowserEmulationVersion();
            int explorerMajorVersion = WBEmulator.GetInternetExplorerMajorVersion();
            if (WBEmulator.GetBrowserEmulationVersion() == BrowserEmulationVersion.Default)
                return false;
            if (explorerMajorVersion >= 11)
                return emulationVersion == BrowserEmulationVersion.Version11;
            bool flag;
            switch (explorerMajorVersion - 8)
            {
                case 0:
                    flag = emulationVersion == BrowserEmulationVersion.Version8;
                    break;
                case 1:
                    flag = emulationVersion == BrowserEmulationVersion.Version9;
                    break;
                case 2:
                    flag = emulationVersion == BrowserEmulationVersion.Version10;
                    break;
                default:
                    flag = emulationVersion == BrowserEmulationVersion.Version7;
                    break;
            }
            return flag;
        }
    }

    public enum BrowserEmulationVersion
    {
        Default = 0,
        Version7 = 7000, // 0x00001B58
        Version8 = 8000, // 0x00001F40
        Version8Standards = 8888, // 0x000022B8
        Version9 = 9000, // 0x00002328
        Version9Standards = 9999, // 0x0000270F
        Version10 = 10000, // 0x00002710
        Version10Standards = 10001, // 0x00002711
        Version11 = 11000, // 0x00002AF8
        Version11Edge = 11001, // 0x00002AF9
    }
}
