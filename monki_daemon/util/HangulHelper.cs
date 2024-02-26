using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonkiDaemon.util
{
    public static class HangulHelper
    {
        private static readonly string _choSeong = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
        private static readonly string _joongSeong = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
        private static readonly string _jongSeong = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㄹㅊㅋㅌㅍㅎ";
        private static readonly ushort _uniCodeHangulFirst = 44032;
        private static readonly ushort _uniCodeHangulLast = 55199;

        private static string[] Divide(string source)
        {
            string[] strArray1 = new string[3]
            {
        string.Empty,
        string.Empty,
        string.Empty
            };
            ushort uint16 = Convert.ToUInt16(source[0]);
            if ((int)uint16 < (int)HangulHelper._uniCodeHangulFirst || (int)uint16 > (int)HangulHelper._uniCodeHangulLast)
                return strArray1;
            int num1 = (int)uint16 - (int)HangulHelper._uniCodeHangulFirst;
            int index1 = num1 / 588;
            int num2 = num1 % 588;
            int index2 = num2 / 28;
            int index3 = num2 % 28;
            string[] strArray2 = strArray1;
            char ch = HangulHelper._choSeong[index1];
            string str1 = ch.ToString();
            strArray2[0] = str1;
            string[] strArray3 = strArray1;
            ch = HangulHelper._joongSeong[index2];
            string str2 = ch.ToString();
            strArray3[1] = str2;
            string[] strArray4 = strArray1;
            ch = HangulHelper._jongSeong[index3];
            string str3 = ch.ToString();
            strArray4[2] = str3;
            return strArray1;
        }

        private static string Chosung(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            string empty = string.Empty;
            foreach (char ch in source)
            {
                string[] strArray = HangulHelper.Divide(ch.ToString());
                if (strArray.Length != 0)
                    empty += strArray[0];
            }
            return empty;
        }

        public static bool IsStartWith(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
                return false;
            string str = HangulHelper.Divide(source)[0];
            return str.Equals(target) || str.Equals(HangulHelper.PairConsonant(target));
        }

        public static bool IsStartWithNonHangul(string source) => !string.IsNullOrEmpty(source) && Regex.IsMatch(source[0].ToString(), "^([A-Z]|[a-z]|[0-9])");

        public static bool IsContainsChosung(string source, string target) => !string.IsNullOrEmpty(source) && HangulHelper.Chosung(source).Contains(target);

        public static bool IsFullHangul(string source)
        {
            foreach (char ch in Regex.Replace(source, "[^ㄱ-힗]", string.Empty))
            {
                if (string.IsNullOrEmpty(HangulHelper.Divide(ch.ToString())[1]))
                    return false;
            }
            return true;
        }

        private static string PairConsonant(string target)
        {
            switch (target)
            {
                case "ㄱ":
                    return "ㄲ";
                case "ㄷ":
                    return "ㄸ";
                case "ㅂ":
                    return "ㅃ";
                case "ㅅ":
                    return "ㅆ";
                case "ㅈ":
                    return "ㅉ";
                default:
                    return (string)null;
            }
        }
    }
}
