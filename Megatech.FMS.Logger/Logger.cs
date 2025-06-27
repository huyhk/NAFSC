using System;
using System.IO;

namespace Megatech.FMS.Logging
{
    public class Logger
    {
        private static string logFolder;

        public static void SetPath(string folder)
        {
            logFolder = folder;
        }
        public static string GetPath()
        {
            return logFolder;
        }
        public static void AppendLog(string tag, string log)
        {
            AppendLog(tag, log, "post");
        }
        public static void AppendLog(string tag, string log, string fileName)
        {
            try
            {
                if (logFolder == null)
                {
                    logFolder = Path.Combine(Directory.GetCurrentDirectory(),"Logs");
                    if (!Directory.Exists(logFolder))
                        Directory.CreateDirectory(logFolder);
                }
                var filePath = Path.Combine(logFolder, fileName + ".log");
                FileInfo fi = new FileInfo(filePath);
                if (fi.Exists && fi.Length > 4 * 1024 * 1024)
                {
                    var i = 1;

                    var archive = Path.Combine(logFolder, fileName + "-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                    while (File.Exists(archive))
                    {
                        archive = Path.Combine(logFolder, fileName + "-" + DateTime.Now.ToString("yyyyMMdd") + "-" + i.ToString() + ".log");
                        i++;
                    }
                    fi.MoveTo(archive);
                }

                File.AppendAllText(filePath, string.Format("[{0:dd-MM-yyyy HH:mm:ss}] [{1}] {2}\n", DateTime.Now, tag, log));
            }
            catch (IOException ex)
            {
            }
        }
        public static void LogException(Exception ex)
        {
            LogException(ex, "exception");
        }
        public static void LogException(Exception ex, string filename)
        {
            Logger.AppendLog("EXP", ex.StackTrace, filename);
            Logger.AppendLog("EXP", ex.Message, filename);
            var exp = ex.InnerException;
            while (exp != null)
            {
                Logger.AppendLog("EXP", exp.Message, filename);
                exp = exp.InnerException;
            }
        }

        public static void WriteString(string log, string fileName)
        {
            if (logFolder == null)
            {
                logFolder = Directory.GetCurrentDirectory();
            }
            var filePath = Path.Combine(logFolder, fileName + ".log");
            FileInfo fi = new FileInfo(filePath);
            if (fi.Exists && fi.Length > 4 * 1024 * 1024)
            {
                var i = 1;

                var archive = Path.Combine(logFolder, fileName + "-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                while (File.Exists(archive))
                {
                    archive = Path.Combine(logFolder, fileName + "-" + DateTime.Now.ToString("yyyyMMdd") + "-" + i.ToString() + ".log");
                    i++;
                }
                fi.MoveTo(archive);
            }

            File.AppendAllText(filePath, string.Format("{0}\n", log));
        }
    }
}