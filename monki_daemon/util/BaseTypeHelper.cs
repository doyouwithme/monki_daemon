using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonkiDaemon.util
{
    public static class BaseTypeHelper
    {
        private static readonly DateTime _baseDateTime = new DateTime(1970, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        private static Random _rnd = new Random();


        public static void Swap<T>(ref T t1, ref T t2)
        {
            T obj = t1;
            t1 = t2;
            t2 = obj;
        }


        public static List<string> GetPropertyNameList(object instance)
        {
            List<string> propertyNameList = new List<string>();
            if (instance != null)
            {
                foreach (PropertyInfo property in instance.GetType().GetProperties())
                    propertyNameList.Add(property.Name);
            }
            return propertyNameList;
        }

        public static List<object> GetPropertyValueList(object instance)
        {
            List<object> propertyValueList = new List<object>();
            if (instance != null)
            {
                foreach (PropertyInfo property in instance.GetType().GetProperties())
                {
                    object obj = property.GetValue(instance, (object[])null);
                    if (obj != null)
                        propertyValueList.Add(obj);
                }
            }
            return propertyValueList;
        }

        public static bool InheritsFrom(Type type, Type baseType)
        {
            if (type == (Type)null)
                return false;
            if (baseType == (Type)null)
                return type.IsInterface || type == typeof(object);
            if (baseType.IsInterface)
                return ((IEnumerable<Type>)type.GetInterfaces()).Contains<Type>(baseType);
            for (Type type1 = type; type1 != (Type)null; type1 = type1.BaseType)
            {
                if (type1.BaseType == baseType)
                    return true;
            }
            return false;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source) => source != null ? new ObservableCollection<T>(source) : throw new ArgumentNullException(nameof(source));

        public static void AddRange<T>(this ObservableCollection<T> oc, IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            foreach (T obj in collection)
                oc.Add(obj);
        }


        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N) => source.Skip<T>(Math.Max(0, source.Count<T>() - N));

        public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T obj = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = obj;
            return list;
        }

        public static IEnumerable<List<T>> SplitList<T>(List<T> sourceList, int splitSize)
        {
            for (int i = 0; i < sourceList.Count; i += splitSize)
                yield return sourceList.GetRange(i, Math.Min(splitSize, sourceList.Count - i));
        }

        public static List<T> MergeList<T>(List<List<T>> sourceList)
        {
            List<T> objList = new List<T>();
            if (sourceList == null || sourceList.Count == 0)
                return objList;
            foreach (List<T> source in sourceList)
                objList.AddRange((IEnumerable<T>)source);
            return objList;
        }


        public static void Remove<T>(this ConcurrentBag<T> source, T removeItem)
        {
            List<T> objList = new List<T>();
            while (!source.IsEmpty)
            {
                T result;
                source.TryTake(out result);
                if ((object)result != (object)removeItem)
                    objList.Add(result);
            }
            foreach (T obj in objList)
                source.Add(obj);
        }

        public static void Clear<T>(this ConcurrentBag<T> source)
        {
            while (!source.IsEmpty)
                source.TryTake(out T _);
        }

        public static string GetLast(this string source, int tail_length)
        {
            if (source == null)
                return (string)null;
            return tail_length >= source.Length ? source : source.Substring(source.Length - tail_length);
        }

        public static string GetLeft(this string source, int maxLength)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            maxLength = Math.Abs(maxLength);
            return source.Length <= maxLength ? source : source.Substring(0, maxLength);
        }

        public static string GetRight(this string source, int maxLength)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            maxLength = Math.Abs(maxLength);
            return source.Length <= maxLength ? source : source.Substring(source.Length - maxLength, maxLength);
        }

        public static int GetByteLength(string source, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(source))
                return 0;
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetByteCount(source);
        }

        public static string SubstringByte(string source, int length, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            if (encoding == null)
                encoding = Encoding.UTF8;
            byte[] bytes = encoding.GetBytes(source);
            return encoding.GetString(bytes, 0, length);
        }

        public static byte[] StringToByte(string source, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(source))
                return (byte[])null;
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetBytes(source);
        }

        public static string ByteToString(byte[] source, Encoding encoding = null)
        {
            if (source == null)
                return (string)null;
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetString(source);
        }

        public static string TrimString(
          string source,
          bool isDeleteNewLineChar = true,
          bool isDeleteDoubleSpace = true,
          bool isTrimJsonString = true)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            source = source.Trim();
            if (isDeleteNewLineChar)
                source = source.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
            if (isDeleteDoubleSpace)
            {
                while (source.Contains("  "))
                    source = source.Replace("  ", " ");
            }
            if (isTrimJsonString)
                source = source.Replace("[ {", "[{").Replace("} ]", "}]");
            return source;
        }

        public static string EditCharacterFollowingParam(string source, string param = " _")
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(param))
                return source;
            StringBuilder sb = new StringBuilder(source);
            int startIndex = 0;
            while (true)
            {
                int num = sb.IndexOf(param, startIndex);
                if (num != -1)
                {
                    if (sb.Length > num + param.Length)
                    {
                        char c = sb[num + param.Length];
                        sb[num + param.Length] = char.ToLower(c);
                    }
                    startIndex = num + 1;
                }
                else
                    break;
            }
            return sb.ToString();
        }

        public static string GetStringAfterLastParam(string source, string param)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(param))
                return source;
            int num = source.LastIndexOf(param);
            if (num < 0)
                return (string)null;
            int startIndex = num + param.Length;
            return source.Length < startIndex ? (string)null : source.Substring(startIndex);
        }

        public static int IndexOf(
          this StringBuilder sb,
          string value,
          int startIndex,
          bool ignoreCase = false)
        {
            int length = value.Length;
            int num = sb.Length - length + 1;
            if (ignoreCase)
            {
                for (int index1 = startIndex; index1 < num; ++index1)
                {
                    if ((int)char.ToLower(sb[index1]) == (int)char.ToLower(value[0]))
                    {
                        int index2 = 1;
                        while (index2 < length && (int)char.ToLower(sb[index1 + index2]) == (int)char.ToLower(value[index2]))
                            ++index2;
                        if (index2 == length)
                            return index1;
                    }
                }
                return -1;
            }
            for (int index3 = startIndex; index3 < num; ++index3)
            {
                if ((int)sb[index3] == (int)value[0])
                {
                    int index4 = 1;
                    while (index4 < length && (int)sb[index3 + index4] == (int)value[index4])
                        ++index4;
                    if (index4 == length)
                        return index3;
                }
            }
            return -1;
        }

        public static string ConvertPascalCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < s.Length; ++index)
            {
                if (char.IsUpper(s[index]))
                    stringBuilder.Append(" ");
                stringBuilder.Append(s[index]);
            }
            s = stringBuilder.ToString();
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower().Replace("_", " ")).Replace(" ", string.Empty);
        }

        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            char[] charArray = s.ToCharArray();
            charArray[0] = char.ToUpper(charArray[0]);
            return new string(charArray);
        }

        public static string LowercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            char[] charArray = s.ToCharArray();
            charArray[0] = char.ToLower(charArray[0]);
            return new string(charArray);
        }

        public static string ConvertStringForTTS(string ttsText)
        {
            ttsText = ttsText.Replace("A", "에이");
            ttsText = ttsText.Replace("B", "비");
            ttsText = ttsText.Replace("C", "씨");
            ttsText = ttsText.Replace("D", "디");
            ttsText = ttsText.Replace("E", "이");
            ttsText = ttsText.Replace("F", "에프");
            ttsText = ttsText.Replace("G", "지");
            ttsText = ttsText.Replace("H", "에이치");
            ttsText = ttsText.Replace("I", "아이");
            ttsText = ttsText.Replace("J", "제이");
            ttsText = ttsText.Replace("K", "케이");
            ttsText = ttsText.Replace("L", "엘");
            ttsText = ttsText.Replace("M", "엠");
            ttsText = ttsText.Replace("N", "엔");
            ttsText = ttsText.Replace("O", "오");
            ttsText = ttsText.Replace("P", "피");
            ttsText = ttsText.Replace("Q", "큐");
            ttsText = ttsText.Replace("R", "알");
            ttsText = ttsText.Replace("S", "에스");
            ttsText = ttsText.Replace("T", "티");
            ttsText = ttsText.Replace("U", "유");
            ttsText = ttsText.Replace("V", "브이");
            ttsText = ttsText.Replace("W", "더블유");
            ttsText = ttsText.Replace("X", "엑스");
            ttsText = ttsText.Replace("Y", "와이");
            ttsText = ttsText.Replace("Z", "지");
            ttsText = ttsText.Replace("1", "일");
            ttsText = ttsText.Replace("2", "이");
            ttsText = ttsText.Replace("3", "삼");
            ttsText = ttsText.Replace("4", "사");
            ttsText = ttsText.Replace("5", "오");
            ttsText = ttsText.Replace("6", "육");
            ttsText = ttsText.Replace("7", "칠");
            ttsText = ttsText.Replace("8", "팔");
            ttsText = ttsText.Replace("9", "구");
            ttsText = ttsText.Replace("0", "영");
            ttsText = ttsText.Replace("/", " ");
            return ttsText;
        }

        public static string GetSplitAndGetOnlyPart(string s, string splitChar, bool isLastString = true)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            string[] source = Regex.Split(s, splitChar);
            return source.Length == 1 || !isLastString ? ((IEnumerable<string>)source).FirstOrDefault<string>() : ((IEnumerable<string>)source).LastOrDefault<string>();
        }

        public static string GetPropertyValueInJson(string jsonString, string propertyName)
        {
            if (string.IsNullOrEmpty(jsonString) || string.IsNullOrEmpty(propertyName))
                return (string)null;
            try
            {
                return (string)JObject.Parse(jsonString)[propertyName];// JToken.op_Explicit(JObject.Parse(jsonString)[propertyName]);
            }
            catch
            {
                return (string)null;
            }
        }

        public static string ConvertPhoneNumberFormat(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return phoneNumber;
            if (phoneNumber.Length == 11)
                return Regex.Replace(phoneNumber, "(\\d{3})(\\d{4})(\\d{4})", "$1-$2-$3");
            if (phoneNumber.Length == 9)
                return Regex.Replace(phoneNumber, "(\\d{2})(\\d{4})(\\d{4})", "$1-$2-$3");
            return phoneNumber.Length == 8 ? Regex.Replace(phoneNumber, "(\\d{2})(\\d{3})(\\d{4})", "$1-$2-$3") : phoneNumber;
        }

        public static string FillWithSpaces(string text, int count, bool isLeftPad = true)
        {
            while (BaseTypeHelper.GetByteLength(text, Encoding.GetEncoding("ks_c_5601-1987")) < count)
                text = !isLeftPad ? text + " " : " " + text;
            return text;
        }

        public static string FillWithSpaces(int text, int count, bool isLeftPad = true) => BaseTypeHelper.FillWithSpaces(text.ToString("N0"), count, isLeftPad);

        public static bool IsNumber(string number) => int.TryParse(number, out int _);

        public static int CalculatePercentage(int currentValue, int maxValue, bool isRound = true)
        {
            if (maxValue == 0)
                return 0;
            return isRound ? Convert.ToInt32(Math.Round(Convert.ToDouble(currentValue) / Convert.ToDouble(maxValue) * 100.0, 1)) : Convert.ToInt32(Math.Ceiling(Convert.ToDouble(currentValue) / Convert.ToDouble(maxValue) * 100.0));
        }

        public static int CalculatePercentage(string currentValue, string maxValue) => BaseTypeHelper.CalculatePercentage(Convert.ToInt32(currentValue), Convert.ToInt32(maxValue));

        public static double LimitToRange(double value, double min, double max) => value < min ? min : (value > max ? max : value);

        public static int LimitToRange(int value, int min, int max) => value < min ? min : (value > max ? max : value);

        public static bool IsEqualInRange(double number1, double number2, double toleranceRange)
        {
            double num = number1 - number2;
            return -toleranceRange < num && num < toleranceRange;
        }

        public static bool IsNumberRange(int value, int start, int end) => value >= start && value <= end;

        public static bool IsNumberRange(double value, double start, double end) => value >= start && value <= end;

        public static string ConvertFileSizeToString(long filesize)
        {
            double num = (double)filesize;
            return filesize < 1073741824L ? (filesize < 1048576L ? (filesize < 1024L ? string.Format("{0:n0} Byte", (object)filesize) : string.Format("{0:0.00}KB ({1:n0}Byte)", (object)(num / 1024.0), (object)filesize)) : string.Format("{0:0.00}MB ({1:n0}Byte)", (object)(num / 1048576.0), (object)filesize)) : string.Format("{0:0.00}GB ({1:n0}Byte)", (object)(num / 1073741824.0), (object)filesize);
        }

        public static int ConvertBoolToNumber(bool param) => param ? 1 : 0;

        public static int DivideRoundingUp(int total, int value)
        {
            int result;
            int num = Math.DivRem(total, value, out result);
            return result == 0 ? num : num + 1;
        }

        public static int DivideRoundingDown(int total, int value) => Math.DivRem(total, value, out int _);

        public static bool IsDouble(string number) => double.TryParse(number, out double _);

        public static bool IsNanOrZero(double param) => double.IsNaN(param) || double.IsInfinity(param) || param == 0.0;

        public static bool IsNanOrZero(double? param) => !param.HasValue || BaseTypeHelper.IsNanOrZero(param.Value);

        public static int GetDecimalPointCount(double param)
        {
            if (BaseTypeHelper.IsNanOrZero(param))
                return 0;
            string str = param.ToString();
            int num = str.IndexOf(".", 0);
            return num == -1 ? 0 : str.Length - num - 1;
        }

        public static double Truncate(double value, int precision) => Math.Truncate(value * Math.Pow(10.0, (double)precision)) / Math.Pow(10.0, (double)precision);

        public static double PlusTwoDouble(double value1, double value2) => (double)((Decimal)value1 + (Decimal)value2);

        public static long ConvertDateTimeToLong(DateTime date) => (long)date.Subtract(BaseTypeHelper._baseDateTime).TotalMilliseconds;

        public static DateTime ConvertLongToDateTime(long ms) => ms < 0L || ms > 99999999999999L ? new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc) : BaseTypeHelper._baseDateTime.AddMilliseconds((double)ms);

        public static string ConvertDateTimeToString(DateTime date, string format = "yyyy-MM-dd HH:mm:ss") => BaseTypeHelper.BaseDateTimeFormat((object)date, (object)format);

        public static DateTime ConvertStringToDateTime(string dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            if (string.IsNullOrEmpty(dateTime))
                return DateTime.Now;
            if (dateTime.Length == 19)
                format = "yyyy-MM-dd HH:mm:ss";
            else if (dateTime.Length > 19)
                format = "yyyy-MM-dd HH:mm:ss.fff";
            try
            {
                return DateTime.ParseExact(dateTime, format, (IFormatProvider)CultureInfo.InvariantCulture);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static DateTime ChangeTime(
          this DateTime dateTime,
          int hours,
          int minutes,
          int seconds,
          int milliseconds)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hours, minutes, seconds, milliseconds, dateTime.Kind);
        }

        public static string BaseDateTimeFormat(object param, object option)
        {
            int num = 0;
            if (param is DateTime)
            {
                num = 1;
            }
            else
            {
                num = param != null ? 1 : 0;
            }

            if (num != 0)
            {
                DateTime dateTime = (DateTime)param;
                if (option?.ToString() == "YMD")
                    return dateTime.ToString("yyyy-MM-dd");
                if (option?.ToString() == "HM")
                    return string.Format("{0:00}:{1:00}", (object)dateTime.Hour, (object)dateTime.Minute);
                if (option?.ToString() == "HMS")
                    return string.Format("{0:00}:{1:00}:{2:00}", (object)dateTime.Hour, (object)dateTime.Minute, (object)dateTime.Second);
                if (option?.ToString() == "yyyy-MM-dd HH:mm:ss.fff")
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                return option != null && option.ToString().Length > 0 ? dateTime.ToString(option?.ToString()) : dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            switch (param)
            {
                case DateTimeOffset dateTimeOffset:
                    DateTime dateTime1 = dateTimeOffset.DateTime;
                    return option?.ToString() == "YMD" ? dateTime1.ToString("yyyy-MM-dd") : dateTime1.ToString("yyyy-MM-dd HH:mm:ss");
                case TimeSpan timeSpan:
                    return option?.ToString() == "HM" ? string.Format("{0:00}:{1:00}", (object)timeSpan.Hours, (object)timeSpan.Minutes) : string.Format("{0:00}:{1:00}:{2:00}", (object)timeSpan.Hours, (object)timeSpan.Minutes, (object)timeSpan.Seconds);
                default:
                    return "";
            }
        }

        public static bool EqualsDate(DateTime date1, DateTime date2) => date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day;

        public static bool EqualsDateTime(DateTime date1, DateTime date2) => date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day && date1.Hour == date2.Hour && date1.Minute == date2.Minute && date1.Second == date2.Second && date1.Millisecond == date2.Millisecond;

        public static string GetNowDateTime(string dateFormat = "yyyy-MM-dd HH:mm:ss") => DateTime.Now.ToString(dateFormat);

        public static string GetNowDateTimeForFileName(string dateFormat = "yyMMdd_HHmmss") => DateTime.Now.ToString(dateFormat);

        public static double CalculateDateTime(DateTime startDateTime, DateTime endDateTime) => (endDateTime - startDateTime).TotalSeconds;

        public static string DateTimeNowForLog() => BaseTypeHelper.BaseDateTimeFormat((object)DateTime.Now, (object)"yyyy-MM-dd HH:mm:ss.fff");

        public static double ConvertSecondToMinute(double second, bool isCeiling = true)
        {
            double totalMinutes = TimeSpan.FromSeconds(second).TotalMinutes;
            return isCeiling ? Math.Ceiling(totalMinutes) : totalMinutes;
        }

        public static string GetEnumDescription(Enum value)
        {
            object[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return ((IEnumerable<object>)customAttributes).Any<object>() ? (((IEnumerable<object>)customAttributes).First<object>() as DescriptionAttribute).Description : value.ToString().Replace("_", " ");
        }

        public static bool CheckEnumType<T>(int value) => Enum.IsDefined(typeof(T), (object)value);

        public static bool CheckEnumType<T>(string value) => Enum.IsDefined(typeof(T), (object)value);

        public static T EnumParse<T>(string value, bool ignoreCase = true) where T : struct
        {
            T result = default(T);
            Enum.TryParse<T>(value, ignoreCase, out result);
            return result;
        }

        public static T ConvertNumberToEnumType<T>(int value)
        {
            Type enumType1 = typeof(T);
            if (!enumType1.IsEnum)
                return default(T);
            if (!BaseTypeHelper.CheckEnumType<T>(value))
                return default(T);
            foreach (T enumType2 in Enum.GetValues(enumType1))
            {
                if (Convert.ToInt32((object)(Enum.Parse(typeof(T), enumType2.ToString()) as Enum)) == value)
                    return enumType2;
            }
            return default(T);
        }


        public static T[] Slice<T>(this T[] source, int index, int length)
        {
            T[] destinationArray = new T[length];
            if (source == null || source.Length == 0 || source.Length < index + length)
                return destinationArray;
            Array.Copy((Array)source, index, (Array)destinationArray, 0, length);
            return destinationArray;
        }

        public static T[] Append<T>(this T[] source, T[] target)
        {
            T[] dst = new T[source.Length + target.Length];
            Buffer.BlockCopy((Array)source, 0, (Array)dst, 0, source.Length);
            Buffer.BlockCopy((Array)target, 0, (Array)dst, source.Length, target.Length);
            return dst;
        }

        public static string ToHexString(this byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
            foreach (byte num in bytes)
                stringBuilder.AppendFormat("{0:x2}", (object)num);
            return stringBuilder.ToString();
        }

        public static DateTime ConvertByteToDateTime(byte[] dateByte)
        {
            if (dateByte == null || dateByte.Length != 6)
                return BaseTypeHelper.ConvertLongToDateTime(0L);
            int num1 = 0;
            long ms = 0;
            long num2 = 1;
            for (int index = 0; index < 6; ++index)
            {
                ms += (long)dateByte[num1 + index] * num2;
                num2 *= 256L;
            }
            return BaseTypeHelper.ConvertLongToDateTime(ms);
        }

        public static bool IsBitSet(double value, Enum type)
        {
            long int64_1 = Convert.ToInt64(value);
            long int64_2 = Convert.ToInt64((object)type);
            return (int64_1 & int64_2) == int64_2;
        }

        public static object GetPropertyValue(this object obj, string name)
        {
            if (obj == null || string.IsNullOrEmpty(name))
                return (object)null;
            string str = name;
            char[] chArray = new char[1] { '.' };
            foreach (string name1 in str.Split(chArray))
            {
                PropertyInfo property = obj.GetType().GetProperty(name1);
                if (property == (PropertyInfo)null)
                    return (object)null;
                obj = property.GetValue(obj, (object[])null);
            }
            return obj;
        }

        public static T GetPropertyValue<T>(this object obj, string name)
        {
            object propertyValue = obj.GetPropertyValue(name);
            return propertyValue == null ? default(T) : (T)propertyValue;
        }

    }
}
