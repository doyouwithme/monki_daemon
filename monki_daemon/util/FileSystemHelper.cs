using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonkiDaemon.util
{
    public static class FileSystemHelper
    {
        public static string CreateDirIfNotExist(string dir_name)
        {
            var dir = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                dir_name);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        public static void DeleteAllInDirectory(string path)
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                directory.Empty();
            }
            catch
            {
            }
        }

        public static void Empty(this System.IO.DirectoryInfo directory)
        {
            foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }

        public static void DeleteFileIfCanNotRename(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
                try
                {
                    var original = path;
                    var bak = $"{original}.{DateTime.Now:yyyyMMddHHmmssfff}.bak";
                    var dir_path = Path.GetDirectoryName(path);
                    DeleteAllBak(dir_path);
                    File.Move(original, bak);
                }
                catch (Exception e)
                {
                    LogHelper.Instance.Error(string.Format("SH DFICNR : {0}", e.Message));
                }
            }
        }


        public static void CreateEmptyFile(string path)
        {
            using (File.Create(path)) { }
        }

        public static bool CheckCRC32(string path, uint origin_crc32)
        {
            try
            {
                var bts = File.ReadAllBytes(path);
            }
            catch
            {
            }
            return false;
        }



        public static void DeleteAllBak(string path)
        {
            var files = GetFiles(path, ".bak");
            foreach (var f in files)
            {
                try
                {
                    File.Delete(f);
                }
                catch
                {
                }
            }
        }

        public static IEnumerable<string> GetFiles(string path,
            string searchPatternExpression = "",
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Regex reSearchPattern = new Regex(searchPatternExpression, RegexOptions.IgnoreCase);
            return Directory.EnumerateFiles(path, "*", searchOption)
                .Where(file =>
                    reSearchPattern.IsMatch(Path.GetExtension(file)));
        }

        // Takes same patterns, and executes in parallel
        public static IEnumerable<string> GetFiles(string path,
            string[] searchPatterns,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return searchPatterns.AsParallel()
                .SelectMany(searchPattern =>
                    Directory.EnumerateFiles(path, searchPattern, searchOption));
        }

        public static Encoding GetEncoding(string filename)
        {
            using (var reader = new StreamReader(filename, Encoding.Default, true))
            {
                if (reader.Peek() >= 0) // you need this!
                    reader.Read();

                return reader.CurrentEncoding;
            }
        }

        public static string AppendTimeStamp(this string fileName)
        {
            return string.Concat(
                Path.GetFileNameWithoutExtension(fileName),
                "-",
                DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Path.GetExtension(fileName)
            );
        }

        public static bool IsSubPathOf(this string path, string baseDirPath)
        {
            string normalizedPath = Path.GetFullPath(path.Replace('/', '\\').WithEnding("\\"));

            string normalizedBaseDirPath = Path.GetFullPath(baseDirPath.Replace('/', '\\').WithEnding("\\"));

            return normalizedPath.StartsWith(normalizedBaseDirPath, StringComparison.OrdinalIgnoreCase);
        }

        public static string WithEnding(this string str, string ending)
        {
            if (str == null)
                return ending;

            string result = str;

            for (int i = 0; i <= ending.Length; i++)
            {
                string tmp = result + ending.Right(i);
                if (tmp.EndsWith(ending))
                    return tmp;
            }
            return result;
        }

        static string Right(this string value, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "Length is less than zero");
            }

            return (length < value.Length) ? value.Substring(value.Length - length) : value;
        }

        public static void FileMoveMigration(string prev_path, string new_path)
        {
            if (File.Exists(new_path))
                return;

            if (File.Exists(prev_path))
            {
                try { File.Move(prev_path, new_path); } catch { }

                if (File.Exists(prev_path))
                {
                    // 이동 실패
                    try
                    {
                        var bytes = File.ReadAllBytes(prev_path);
                        File.WriteAllBytes(new_path, bytes);
                    }
                    catch (Exception e)
                    {
                        LogHelper.Instance.Error(string.Format("Failed to move file : {0}", e.Message));
                        return;
                    }
                    DeleteFileIfCanNotRename(prev_path);
                }
            }
        }

        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }

            int i = 0;
            decimal dValue = (decimal)value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
        }

        /// <summary>
        /// 최소 저장 공간을 가진 drive를 찾는다. (1 GB이상 가진 드라이브를 찾는다.)
        /// </summary>
        /// <returns></returns>
        public static string GetAvailableDisk()
        {
            var d_infos = DriveInfo.GetDrives();
            var output_drive = d_infos
                .FirstOrDefault(d => d.IsReady && d.DriveType == DriveType.Fixed && d.AvailableFreeSpace > 1024L * 1024 * 1024);
            if (null != output_drive)
                return output_drive.Name;

            throw new IOException("Not enough space to process\n" +
                                  string.Join("\n",
                                      d_infos
                                          .Where(d => d.IsReady && d.DriveType == DriveType.Fixed)
                                          .Select(d => $"Name : {d.Name} / {SizeSuffix(d.AvailableFreeSpace)}")));
        }

        public static string GetAllDriveStatus()
        {
            try
            {
                var d_infos = DriveInfo.GetDrives();
                return string.Join("\n",
                    d_infos
                        .Where(d => d.IsReady && d.DriveType == DriveType.Fixed)
                        .Select(d => $"Name : {d.Name} ({SizeSuffix(d.AvailableFreeSpace)} / {SizeSuffix(d.TotalSize)})"));
            }
            catch (Exception e)
            {
                return $"Unknown : {e}";
            }
        }


        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }
        public static string GetProductVersion(string exe_path)
        {
            try
            {
                if (!File.Exists(exe_path))
                    return string.Empty;
                var versionInfo = FileVersionInfo.GetVersionInfo(exe_path);
                if (string.IsNullOrWhiteSpace(versionInfo.FileVersion))
                    return string.Empty;
                return versionInfo.ProductVersion;
            }
            catch
            {
                return string.Empty;
            }
        }


        public static void DeleteFile(string filename)
        {
            try
            {
                File.Delete(filename);
            }
            catch
            {
            }
        }
    }
}
