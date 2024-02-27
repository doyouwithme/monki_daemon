using Microsoft.Win32;
using MonkiDaemon.util;
using MonkiDaemon.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkiDaemon.util
{
    public static class AutoStartHelper
    {
        private static readonly string RunKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private static readonly string AssemblyName = "POSFEED";

        public static bool IsExistKeyAutoStart(string title)
        {
            try
            {
                if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs) + title + ".appref-ms")))
                {
                    using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(AutoStartHelper.RunKey, true))
                    {
                        if (registryKey?.GetValue(AutoStartHelper.AssemblyName) != null)
                            return true;
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(String.Format("Except {0}", e));
            }
            return false;
        }

        public static void SetAutoStartConfig(bool isAutoStart, string title)
        {
            try
            {
                if (isAutoStart)
                {
                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs) + title) + ".appref-ms";
                    if (!File.Exists(path))
                        return;
                    using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(AutoStartHelper.RunKey, true))
                        registryKey?.SetValue(AutoStartHelper.AssemblyName, (object)("\"" + path + "\""));
                }
                else
                {
                    using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(AutoStartHelper.RunKey, true))
                        registryKey?.DeleteValue(AutoStartHelper.AssemblyName, false);
                }
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(String.Format("Except {0}", e));
            }
        }
    }
}
