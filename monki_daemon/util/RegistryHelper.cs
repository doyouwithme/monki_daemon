using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monki_okpos_daemon.util
{
    public static class RegistryHelper
    {
        public static bool ExportRegistry(string key, out string value, out string err)
        {
            var temp_path = Path.GetTempFileName();
            value = string.Empty;
            err = string.Empty;
            try
            {
                using (var proc = new Process())
                {
                    proc.StartInfo.FileName = "reg.exe";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.Arguments = "export \"" + key + "\" \"" + temp_path + "\" /y";
                    proc.Start();
                    string stdout = proc.StandardOutput.ReadToEnd();
                    string stderr = proc.StandardError.ReadToEnd();
                    err += stderr;
                    proc.WaitForExit();
                }
            }
            catch (Exception e)
            {
                err += e.ToString();
                return false;
            }

            if (File.Exists(temp_path))
            {
                value = File.ReadAllText(temp_path);
                try { File.Delete(temp_path); } catch { }
                return true;
            }

            err += "\nFile Not Exist";
            return false;
        }

        public static bool ImportRegistgry(string value, out string err)
        {
            err = string.Empty;
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var temp_path = Path.GetTempFileName();
            File.WriteAllText(temp_path, value);
            try
            {
                using (var proc = new Process())
                {
                    proc.StartInfo.FileName = "reg.exe";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.Arguments = "import \"" + temp_path + "\"";
                    proc.Start();
                    string stdout = proc.StandardOutput.ReadToEnd();
                    string stderr = proc.StandardError.ReadToEnd();
                    err += stderr;
                    proc.WaitForExit();
                }

                return true;
            }
            catch (Exception ex)
            {
                err += ex.ToString();
                return false;
            }
            finally
            {
                try { File.Delete(temp_path); } catch { }
            }
        }

        public static object ReadRegistry(RegistryKey root, string path, string value_name = "")
        {
            try
            {
                using (var key = root.OpenSubKey(path, false))
                {
                    return key.GetValue(value_name);
                }
            }
            catch
            {
            }
            return null;
        }
    }
}
