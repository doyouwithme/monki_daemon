using Microsoft.Win32;
using MonkiDaemon.config;
using MonkiDaemon.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sentry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MonkiDaemon.util
{
    public static class UtilHelper
    {

        #region ini 입력 메소드
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        #endregion

        private const string RUN_LOCATION = @"Software\Microsoft\Windows\CurrentVersion\Run";


        public static void SetAutoStart(string keyName, string assemblyLocation)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
            key.SetValue(keyName, assemblyLocation);
        }


        public static bool IsAutoStartEnabled(string keyName, string assemblyLocation)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RUN_LOCATION);
            if (key == null)
                return false;

            string value = (string)key.GetValue(keyName);
            if (value == null)
                return false;

            return (value == assemblyLocation);
        }

        public static void UnSetAutoStart(string keyName)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
            key.DeleteValue(keyName);
        }

        public static void SentryException(Exception e)
        {
            string mode = Config.EnvironmentMode == "1" ? "PROD" : "DEV";
            string errmsg = string.Format("Monki Daemon Mode : {0} Store# : {1}({2}) Message : {3}", mode, Config.StoreNm, Config.StoreNo, e.Message.ToString()); 
            SentrySdk.CaptureMessage(errmsg);
        }

        public static Process[] GetProcessesByName(string name, bool throw_exception = false)
        {
            try
            {
                return Process.GetProcessesByName(name);
            }
            catch (InvalidOperationException)
            {
                try
                {
                    bool success = false;
                    string o1, e1, o2 = string.Empty, e2 = string.Empty;
                    do
                    {
                        if (!RunCmd("winmgmt /resyncperf", out o1, out e1, out _))
                            break;

                        if (!RunCmd("wmiadap /f", out o2, out e2, out _))
                            break;
                        success = true;
                    } while (false);

                    if (!string.IsNullOrWhiteSpace(o1) ||
                        !string.IsNullOrWhiteSpace(o2) ||
                        !string.IsNullOrWhiteSpace(e1) ||
                        !string.IsNullOrWhiteSpace(e2))
                    {
                        RunCmd("tasklist", out var o3, out var e3, out _);
                    }

                    if (success)
                        return Process.GetProcessesByName(name);
                }
                catch
                {
                }
            }
            catch
            {
            }
            if (throw_exception)
                throw new ApplicationException("GetProcessesByName Failed #3");
            return new Process[0];
        }

        public static bool RunCmd(string cmd, out string output, out string err, out int exit_code)
        {
            output = string.Empty;
            err = string.Empty;
            exit_code = -1;
            try
            {
                var ProcStartInfo =
                    new System.Diagnostics.ProcessStartInfo("CMD.exe")
                    {
                        Arguments = $"/C \"{cmd}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    };
                var p = Process.Start(ProcStartInfo);
                p.WaitForExit();
                output = p.StandardOutput.ReadToEnd();
                err = p.StandardError.ReadToEnd();
                exit_code = p.ExitCode;
                return true;
            }
            catch (Exception e)
            {
                err += "\n\n===========================\n" + e.ToString();
                return false;
            }
        }

        public static T Max<T>(T first, T second)
        {
            if (Comparer<T>.Default.Compare(first, second) > 0)
                return first;
            return second;
        }

        public static string ToStandardTimeFormat(this DateTime me)
        {
            return me.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        private const string client_key = "ORC16FUAvRuAOzuhDzglOOnZpHxLuYByQDBhLz4LlcA=";
        private const string client_iv = "s1kJCDIAwFqNq6pTfK0lpw==";
        private const string data_prefix = "PIN ";
        public static string EV(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            try
            {
                var save_data = data_prefix + value;
                using (var aes = new RijndaelManaged
                {
                    KeySize = 256,
                    BlockSize = 128,
                    Key = Convert.FromBase64String(client_key),
                    IV = Convert.FromBase64String(client_iv)
                })
                {
                    using (var encryptedStream = new MemoryStream())
                    {
                        using (ICryptoTransform encryptor = aes.CreateEncryptor())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write))
                            {
                                byte[] inputBytes = Encoding.UTF8.GetBytes(save_data);
                                cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                            }
                        }
                        return Convert.ToBase64String(encryptedStream.ToArray());
                    }
                }
            }
            catch
            {
            }
            return string.Empty;
        }
        public static string DV(string value)
        {
            try
            {
                //used for the blob stream from Azure
                using (var encryptedStream = new MemoryStream(Convert.FromBase64String(value)))
                {
                    //stream where decrypted contents will be stored
                    using (var decryptedStream = new MemoryStream())
                    {
                        using (var aes = new RijndaelManaged
                        {
                            KeySize = 256,
                            BlockSize = 128,
                            Key = Convert.FromBase64String(client_key),
                            IV = Convert.FromBase64String(client_iv)
                        })
                        {
                            using (var decryptor = aes.CreateDecryptor())
                            {
                                //decrypt stream and write it to parent stream
                                using (var cryptoStream =
                                    new CryptoStream(encryptedStream, decryptor, CryptoStreamMode.Read))
                                {
                                    int data;

                                    while ((data = cryptoStream.ReadByte()) != -1)
                                        decryptedStream.WriteByte((byte)data);
                                }
                            }
                        }

                        var decrypted_str = Encoding.UTF8.GetString(decryptedStream.ToArray());
                        if (!decrypted_str.StartsWith(data_prefix))
                            throw new ApplicationException($"Invalid Data : '{decrypted_str}'");

                        return decrypted_str.Substring(data_prefix.Length);
                    }
                }
            }
            catch
            {
            }
            return string.Empty;
        }

        public static void KeepTryGetData(Func<string> get_data, out string data, TimeSpan? try_interval = null, DateTime? call_until = null)
        {
            var interval = try_interval ?? TimeSpan.FromSeconds(1);
            data = string.Empty;

            while (true)
            {
                if (call_until.HasValue && DateTime.Now > call_until.Value)
                    break;

                data = get_data.Invoke();
                if (string.IsNullOrEmpty(data))
                    Thread.Sleep(interval);
                else
                    break;
            }
        }

        public static void KeepTryUntilTrue(Func<bool> success, TimeSpan? try_interval = null, DateTime? call_until = null)
        {
            var interval = try_interval ?? TimeSpan.FromSeconds(1);

            while (true)
            {
                if (call_until.HasValue && DateTime.Now > call_until.Value)
                    break;

                if (success.Invoke())
                    break;
                Thread.Sleep(interval);
            }
        }

        public static string ToKorean(this DayOfWeek me)
        {
            switch (me)
            {
                case DayOfWeek.Sunday:
                    return "일";
                case DayOfWeek.Monday:
                    return "월";
                case DayOfWeek.Tuesday:
                    return "화";
                case DayOfWeek.Wednesday:
                    return "수";
                case DayOfWeek.Thursday:
                    return "목";
                case DayOfWeek.Friday:
                    return "금";
                case DayOfWeek.Saturday:
                    return "토";
            }

            return string.Empty;
        }

        public static byte[] ReadAllBytes(this Stream in_stream)
        {
            if (in_stream is MemoryStream)
                return ((MemoryStream)in_stream).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                in_stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        public static string RemoveLineEndings(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace("\r\n", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace(lineSeparator, string.Empty)
                .Replace(paragraphSeparator, string.Empty);
        }


        public static List<string> SplitStringWithBytesCount(this string me, Encoding encoding, int first_line_max_bytes, int max_bytes)
        {
            List<string> ret = new List<string>();

            int l2_bytes = -1;
            var str = me;
            while (true)
            {
                var mb = (ret.Count == 0) ? first_line_max_bytes : max_bytes;

                str.SplitStringWithBytesCount(encoding, mb, out int l1_bytes, out int total_bytes, out int l1, out int l2);
                l2_bytes = total_bytes - l1_bytes;
                if (l1 <= 0)
                    break;

                ret.Add(str.Substring(0, l1));

                if (l2 <= 0)
                    break;

                str = str.Substring(l1, l2);
                if (l2_bytes <= max_bytes)
                {
                    ret.Add(str);
                    break;
                }
            }

            return ret;
        }

        public static void SplitStringWithBytesCount(
            this string me,
            Encoding encoding,
            int max_bytes,
            out int length1_bytes_count,
            out int string_bytes_count,
            out int length1,
            out int length2)
        {
            length1 = 0;
            length2 = 0;
            length1_bytes_count = 0;
            string_bytes_count = 0;

            if (!(encoding.CodePage == 949 || encoding.CodePage == 51949))
                return;

            if (string.IsNullOrEmpty(me))
                return;

            if (max_bytes <= 0)
                return;

            var str_length = 0;

            var bytes = encoding.GetBytes(me);
            string_bytes_count = bytes.Length;
            // cp949는 가변길이인코딩
            for (int i = 0; i < bytes.Length; ++i)
            {
                if (bytes[i] >= 0x80)
                {
                    // 다음 문자는 스킵
                    ++i;
                }
                if (i + 1 > max_bytes)
                    break;

                length1_bytes_count = i + 1;
                ++str_length;
            }

            length1 = str_length;
            length2 = me.Length - length1;
        }

        public static void AddWithMaxBytes(this StringBuilder me, Encoding encoding, string text, int max_bytes, out int added_bytes, out int added_length, out int left_length)
        {
            text.SplitStringWithBytesCount(encoding, max_bytes, out added_bytes, out var string_bytes_length, out added_length, out left_length);
            if (added_length > 0)
                try
                {
                    me.Append(text.Take(added_length));
                }
                catch
                {
                    me.Append(text);
                    added_bytes = string_bytes_length;
                    added_length = text.Length;
                    left_length = 0;
                }
        }

        public static void AddBytesOfText(this StringBuilder me, Encoding encoding, string text, int byte_count, bool left_align, out int left)
        {
            left = -1;

            text.SplitStringWithBytesCount(encoding, byte_count, out var length1BytesCount, out var stringBytesCount, out var length1, out var length2);

            var space_required = byte_count - length1BytesCount;
            if (space_required > 0)
            {
                if (!left_align)
                {
                    for (int i = 0; i < space_required; ++i)
                        me.Append(' ');
                }

                if (length1 > 0)
                {
                    if (text.Length < length1)
                        me.Append(text);
                    else
                        me.Append(text.Substring(0, length1));
                }

                if (left_align)
                {
                    for (int i = 0; i < space_required; ++i)
                        me.Append(' ');
                }
            }
            else
            {
                if (length1 > 0)
                {
                    if (text.Length < length1)
                        me.Append(text);
                    else
                        me.Append(text.Substring(0, length1));
                }
            }
        }

        public static bool IsOrderThanVista()
        {
            var v = Environment.OSVersion;
            return v.Version.Major < 6;
        }

        public static bool IsOrderThanWin7()
        {
            var v = Environment.OSVersion;
            return v.Version.Major < 6 || (v.Version.Major == 6 && v.Version.Minor < 1);
        }


        public static readonly Regex PHONE_NUMBER_REGEX = new Regex(@"(^|[^\d])0\d{1,3}\s{0,2}-?\s{0,2}\d{3,5}\s{0,2}-?\s{0,2}\d{4}($|[^\d])");

        public static bool MaskAllPhoneNumber(string one_line, out string masked)
        {
            masked = one_line;
            var ms = PHONE_NUMBER_REGEX.Matches(masked);
            foreach (Match m in ms)
            {
                masked = masked.Remove(m.Index, m.Length).Insert(m.Index, m.Value.MaskingNumber());
            }
            return ms.Count > 0;
        }

        private static Regex number_regex = new Regex(@"\d");
        public static string MaskingNumber(this string me)
        {
            return number_regex.Replace(me, "@");
        }

        private static Regex all_except_space = new Regex(@"[^\s]");
        public static string MaskingAll(this string me)
        {
            return all_except_space.Replace(me, "@");
        }

        public static string GetMaskedString(int count)
        {
            return new string('@', count);
        }


        public static string GetNowDateTime()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");            
        }

        public static bool IsNumeric(string s)
        {
            int output;
            return int.TryParse(s, out output);
        }

        public static string BeautifyJson(string jsonString)
        {
            string beautifiedJson = JValue.Parse(jsonString).ToString(Formatting.Indented);
            return beautifiedJson;
        }

        public static string GetMacAddress() => NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();


        public static bool GetProcessActive() {
            Process[] _processes = null;
            string strCurrentProcess = Process.GetCurrentProcess().ProcessName;
            _processes = Process.GetProcessesByName(strCurrentProcess);

            if (_processes.Length > 1)
            {
                return true;
            }

            return false;
        }


        public static string GetIniFile(string section, string key, string filepath)
        {
            StringBuilder sb = new StringBuilder(255);
            try
            {
                GetPrivateProfileString(section, key, "", sb, sb.Capacity, filepath);
            }
            catch
            {
            }
            return sb.ToString();
        }


        public static void SetIniFile(string section, string key, string data, string filepath)
        {            
            try
            {
                WritePrivateProfileString(section, key, data, filepath);
            }
            catch
            {
            }
        }
    }   
    
}

