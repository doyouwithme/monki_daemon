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
    public static class UninstallHelper
    {
        private static readonly string UninstallKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

        public static void SetUninstallInfo()
        {
            try
            {
                FileInfo fileInfo = new FileInfo("Assets/Icons/posfeed.ico");
                if (!fileInfo.Exists)
                    return;
                using (RegistryKey registryKey1 = Registry.CurrentUser.OpenSubKey(UninstallHelper.UninstallKey))
                {
                    foreach (string subKeyName in registryKey1.GetSubKeyNames())
                    {
                        RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName, true);
                        object obj1 = registryKey2.GetValue("DisplayName");
                        object obj2 = registryKey2.GetValue("Publisher");
                        object obj3 = registryKey2.GetValue("DisplayIcon");
                        if (obj1 != null)
                        {
                            if (obj3.Equals((object)fileInfo.FullName))
                                break;
                            registryKey2.SetValue("DisplayIcon", (object)fileInfo.FullName);
                            registryKey2.SetValue("Publisher", (object)"HelloWorld Co., Ltd.");
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(String.Format("Except {0}", e));
            }
        }
    }
}
