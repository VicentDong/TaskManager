using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using System.IO;

namespace TaskManagerCommon
{
    public static class LogHelper
    {
        private static readonly ILog loggerServer = LogManager.GetLogger(typeof(LogHelper));

        private static readonly ILog loggerCommon = LogManager.GetLogger("CommonLog");


        public static void ServerInfo(string msg)
        {
            try
            {
                //记录文件日志- 开始执行相关任务
                loggerServer.Info(msg);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static void ServerError(string msg,Exception exception)
        {
            try
            {
                //记录文件日志- 开始执行相关任务
                loggerServer.Error(msg, exception);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static void Log(string msg)
        {
            try
            {
                //记录文件日志- 开始执行相关任务
                loggerCommon.Info(msg);
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        /// <summary>
        /// 写日志文件
        /// </summary>
        /// <param name="message">日志消息</param>
        public static void WriteLog(string Filename, string message)
        {
            try
            {
                //检查日志目录是否存在
                if (!Directory.Exists(System.Environment.CurrentDirectory + @"\Logs\Task"))
                    Directory.CreateDirectory(System.Environment.CurrentDirectory + @"Logs\Task");

                string fullFileName = System.Environment.CurrentDirectory + @"\Logs\Task\" + Filename + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

                FileInfo fi = new FileInfo(fullFileName);
                if (fi.Exists)
                {
                    using (StreamWriter sw = fi.AppendText())
                    {
                        sw.WriteLine(DateTime.Now.ToString("MM-dd HH:mm:ss ") + message);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = fi.CreateText())
                    {
                        sw.WriteLine(DateTime.Now.ToString("MM-dd HH:mm:ss ") + message);
                        sw.Close();
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
