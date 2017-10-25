using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TaskManagerCommon
{
    public class LogDataAccess
    {
        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        private string CONNECTION_STRINGS = ConfigurationManager.ConnectionStrings["SqlConnectionString"].ToString();


        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="Task"></param>
        /// <param name="Group"></param>
        /// <param name="Type">0:运行 1：错误</param>
        /// <param name="Message"></param>
        public void AddLog(string Task, string Group, int Type, string Message)
        {

            SQLHelper SqlHelper = new SQLHelper();
            try
            {
                string INSERT_LOG_STRING = "INSERT INTO [dbo].[QRTZ_TaskLog]([Task],[Group],[Type],[Message],[CreateTime]) VALUES(@Task,@Group,@Type,@Message,@CreateTime)";

                SqlParameter[] pars = new SqlParameter[5]{
                    new SqlParameter("@Task",Task),
                    new SqlParameter("@Group",Group),
                    new SqlParameter("@Type",Type),
                    new SqlParameter("@Message",Message),
                    new SqlParameter("@CreateTime",DateTime.Now)
                };
                int result = SqlHelper.ExecuteNonQuery(CONNECTION_STRINGS, CommandType.Text, INSERT_LOG_STRING, pars);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// 获取任务日志
        /// </summary>
        /// <param name="Task"></param>
        /// <param name="Group"></param>
        public DataTable GetLogOfTask(string Task, string Group, int PageSize, int PageCount)
        {
            SQLHelper SqlHelper = new SQLHelper();
            DataTable dt = new DataTable();
            try
            {
                string SELECT_LOG_STRING = @"SELECT b.n as ID, a.Task,a.[Group],a.[Type],a.[Message],Convert(nvarchar(30),a.CreateTime,20) as CreateTime 
                    FROM dbo.QRTZ_TaskLog a,(SELECT TOP " + (PageCount * PageSize).ToString()
                     + @" row_number() OVER (ORDER BY CreateTime DESC) n, ID FROM dbo.QRTZ_TaskLog 
                    where Task = @Task and [Group] =@Group) b WHERE a.ID = b.ID AND b.n > " + (PageSize * (PageCount - 1)).ToString() + " order by CreateTime desc";

                SqlParameter[] pars = new SqlParameter[2]{
                    new SqlParameter("@Task",Task),
                    new SqlParameter("@Group",Group)
                };
                dt = SqlHelper.ExecuteDataTable(CONNECTION_STRINGS, CommandType.Text, SELECT_LOG_STRING, pars);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return dt;
        }

        public List<int> GetTaskLogPageDict(string Task, string Group, int PageSize)
        {
            SQLHelper SqlHelper = new SQLHelper();
            DataTable dt = new DataTable();
            List<int> pageDict = new List<int>();
            try
            {
                string SELECT_LOG_STRING = @"select count(ID) as total, case when count(ID) % @PageSize = 0  then count(ID) / @PageSize  else count(ID) / @PageSize + 1 END as [pagecount] 
from dbo.QRTZ_TaskLog where task = @Task and [Group] = @Group ";

                SqlParameter[] pars = new SqlParameter[3]{
                    new SqlParameter("@Task",Task),
                    new SqlParameter("@Group",Group),
                    new SqlParameter("@PageSize",PageSize)
                };

                dt = SqlHelper.ExecuteDataTable(CONNECTION_STRINGS, CommandType.Text, SELECT_LOG_STRING, pars);
                if (dt != null && dt.Rows.Count > 0)
                {
                    pageDict.Add(int.Parse(dt.Rows[0]["total"].ToString()));
                    pageDict.Add(int.Parse(dt.Rows[0]["pagecount"].ToString()));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return pageDict;
        }
    }
}
