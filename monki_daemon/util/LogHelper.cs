using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace monki_okpos_daemon.util
{
    public class LogHelper
    {
        private static LogHelper _instance;
        private ILog _log;

        public static LogHelper Instance
        {
            get
            {
                if (LogHelper._instance == null)
                    LogHelper._instance = new LogHelper();
                return LogHelper._instance;
            }
            set => LogHelper._instance = value;
        }

        public LogHelper()
        {
            string str = Path.Combine(PathHelper.GetLogPath(), "okpos.log");
            log4net.Repository.Hierarchy.Hierarchy repository = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
            ((LoggerRepositorySkeleton)repository).Configured = true;
            RollingFileAppender rollingFileAppender = new RollingFileAppender();
            ((AppenderSkeleton)rollingFileAppender).Name = "logger";
            ((FileAppender)rollingFileAppender).File = str;
            ((FileAppender)rollingFileAppender).AppendToFile = true;
            rollingFileAppender.StaticLogFileName = true;
            rollingFileAppender.CountDirection = 1;
            rollingFileAppender.RollingStyle = (RollingFileAppender.RollingMode)2;
            ((FileAppender)rollingFileAppender).LockingModel = (FileAppender.LockingModelBase)new FileAppender.MinimalLock();
            rollingFileAppender.DatePattern = "_yyyyMMdd\".log\"";
            PatternLayout patternLayout = new PatternLayout("%date [%-5level] %message%newline");
            ((AppenderSkeleton)rollingFileAppender).Layout = (ILayout)patternLayout;
            repository.Root.AddAppender((IAppender)rollingFileAppender);
            ((AppenderSkeleton)rollingFileAppender).ActivateOptions();
            repository.Root.Level = Level.All;
            this._log = LogManager.GetLogger("logger");
            Logger logger = (Logger)((ILoggerWrapper)this._log).Logger;
        }

        public void Info(string logMsg, [CallerFilePath] string file = null, [CallerMemberName] string method = null, [CallerLineNumber] int lineNumber = 0)
        {
            logMsg = string.Format("[{0}][{1}][line:{2}]{3}", (object)Path.GetFileName(file), (object)method, (object)lineNumber, (object)logMsg);
            this._log.Info((object)logMsg);
        }

        public void Info<T>(string logMsg, T data, [CallerFilePath] string file = null, [CallerMemberName] string method = null, [CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                string str = JsonConvert.SerializeObject((object)data);
                logMsg = string.Format("[{0}][{1}][line:{2}]{3}, data : {4}", (object)Path.GetFileName(file), (object)method, (object)lineNumber, (object)logMsg, (object)str);
                this._log.Info((object)logMsg);
            }
            catch (Exception ex)
            {
                this.Info(string.Format("Log Info Fail, {0}", (object)ex.Message), "", nameof(Info), 132);
            }
        }

        public void Error(string logMsg, [CallerFilePath] string file = null, [CallerMemberName] string method = null, [CallerLineNumber] int lineNumber = 0)
        {
            logMsg = string.Format("[{0}][{1}][line:{2}]{3}", (object)Path.GetFileName(file), (object)method, (object)lineNumber, (object)logMsg);
            this._log.Error((object)logMsg);
            this.TRACE(string.Format("[ERROR] {0}", (object)logMsg), method, lineNumber);
        }

        public void TRACE(string logMsg, [CallerMemberName] string method = null, [CallerLineNumber] int lineNumber = 0)
        {
            if (logMsg.LastIndexOf("\n") != logMsg.Length - 1)
                logMsg += "\n";
            Trace.Write(string.Format("[{0}][line:{1}] {2}", (object)method, (object)lineNumber, (object)logMsg));
        }

        public void CleanUp(DateTime date)
        {
            string empty1 = string.Empty;
            string empty2 = string.Empty;
            IAppender iappender = ((IEnumerable<IAppender>)(((IEnumerable<ILoggerRepository>)LogManager.GetAllRepositories()).FirstOrDefault<ILoggerRepository>() ?? throw new NotSupportedException("Log4Net has not been configured yet.")).GetAppenders()).Where<IAppender>((Func<IAppender, bool>)(x => ((object)x).GetType() == typeof(RollingFileAppender))).FirstOrDefault<IAppender>();
            if (iappender == null)
                return;
            RollingFileAppender rollingFileAppender = iappender as RollingFileAppender;
            this.CleanUp(Path.GetDirectoryName(((FileAppender)rollingFileAppender).File), Path.GetFileName(((FileAppender)rollingFileAppender).File), date);
        }

        public void CleanUp(string logDirectory, string logPrefix, DateTime date)
        {
            if (string.IsNullOrEmpty(logDirectory))
                throw new ArgumentException("logDirectory is missing");
            if (string.IsNullOrEmpty(logPrefix))
                throw new ArgumentException("logPrefix is missing");
            DirectoryInfo directoryInfo = new DirectoryInfo(logDirectory);
            if (!directoryInfo.Exists)
                return;
            FileInfo[] files = directoryInfo.GetFiles(string.Format("{0}*.*", (object)logPrefix));
            if (files.Length == 0)
                return;
            foreach (FileInfo fileInfo in files)
            {
                if (fileInfo.LastWriteTime < date)
                    fileInfo.Delete();
            }
        }

        

        private void DeleteFile(string path)
        {
            try
            {
                new FileInfo(path)?.Delete();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        public void Close() => LogManager.Shutdown();
    }
}