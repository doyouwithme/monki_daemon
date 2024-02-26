using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonkiDaemon.util
{
    public class RegexUtil
    {
        public static string GetOnlyNumber(string value)
        {
            string onlyNumber = "0";
            if (!string.IsNullOrEmpty(value))
                onlyNumber = Regex.Replace(value, "\\D", string.Empty);
            return onlyNumber;
        }

        public static string GetOnlyAlphabatAndNumber(string value)
        {
            string alphabatAndNumber = string.Empty;
            if (!string.IsNullOrEmpty(value))
                alphabatAndNumber = Regex.Replace(value, "a-zA-Z0-9", string.Empty);
            return alphabatAndNumber;
        }

        public static int LastIndexOf(string value, string pattern)
        {
            MatchCollection matchCollection = Regex.Matches(value, pattern);
            int count = matchCollection.Count;
            return matchCollection.Count > 0 ? matchCollection[count - 1].Index : -1;
        }

        public static string ConvertPhoneRegex(string value)
        {
            string str = value;
            if (value != null)
            {
                string input = value.Replace("-", string.Empty).Trim();
                switch (input.Length)
                {
                    case 7:
                        str = Regex.Replace(input, "(\\d{3})(\\d{4})", "$1-$2");
                        break;
                    case 9:
                        str = Regex.Replace(input, "(\\d{2})(\\d{3})(\\d{4})", "$1-$2-$3");
                        break;
                    case 10:
                        str = Regex.Replace(input, str.StartsWith("02") ? "(\\d{2})(\\d{4})(\\d{4})" : "(\\d{3})(\\d{3})(\\d{4})", "$1-$2-$3");
                        break;
                    case 11:
                        str = Regex.Replace(input, "(\\d{3})(\\d{4})(\\d{4})", "$1-$2-$3");
                        break;
                    case 12:
                        str = Regex.Replace(input, "(\\d{4})(\\d{4})(\\d{4})", "$1-$2-$3");
                        break;
                }
            }
            return str;
        }
    }
}
