using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TaskManagerCommon
{
    public static class DBLogHelper
    {
        /// <summary>
        /// 记录运行日志
        /// </summary>
        /// <param name="Task"></param>
        /// <param name="Group"></param>
        /// <param name="Message"></param>
        public static void WriteRunLog(string Task, string Group, string Message)
        {
            try
            {
                LogDataAccess logDA = new LogDataAccess();

                logDA.AddLog(Task, Group, 1, Message);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="Task"></param>
        /// <param name="Group"></param>
        /// <param name="Message"></param>
        public static void WriteErrorLog(string Task, string Group, string Message)
        {
            try
            {
                LogDataAccess logDA = new LogDataAccess();

                logDA.AddLog(Task, Group, 0, Message);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public static DataTable GetTaksLog(string taskName, string groupName, int pageSize, int pageIndex)
        {
            try
            {
                LogDataAccess logDA = new LogDataAccess();
                return logDA.GetLogOfTask(taskName, groupName, pageSize, pageIndex);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public static Dictionary<string, object> GetTaskLogByPage(string taskName, string groupName, int pageSize, int pageIndex)
        {
            LogDataAccess logDA = new LogDataAccess();
            Dictionary<string, object> taskLog = new Dictionary<string, object>();
            try
            {
                DataTable dtLog = logDA.GetLogOfTask(taskName, groupName, pageSize, pageIndex);
                List<int> pageDict = logDA.GetTaskLogPageDict(taskName, groupName, pageSize);

                if (dtLog != null && pageDict != null && dtLog.Rows.Count > 0 && pageDict.Count > 0 )
                {
                    taskLog.Add("logs", dtLog);
                    taskLog.Add("total", pageDict[0]);
                    taskLog.Add("count", pageDict[1]);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return taskLog;
        }
    }
}


