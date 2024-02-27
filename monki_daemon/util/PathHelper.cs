using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MonkiDaemon.util
{
    public class PathHelper
    {
        private static string _appBaseDirectory;

        public static string AppBaseDirectory
        {
            get => string.IsNullOrEmpty(PathHelper._appBaseDirectory) ? AppDomain.CurrentDomain.BaseDirectory : PathHelper._appBaseDirectory;
            set => PathHelper._appBaseDirectory = value;
        }

        public static void SetAppBaseDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            PathHelper.AppBaseDirectory = path;
        }

        public static string GetLogPath()
        {
            string path = Path.Combine(PathHelper.AppBaseDirectory, "Log");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static string GetEXEPath() => Assembly.GetEntryAssembly().Location;
    }
}
