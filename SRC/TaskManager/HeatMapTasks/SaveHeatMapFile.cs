using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManagerCommon;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Quartz;
using TaskManagerTaskSet;

namespace HeatMapTasks
{
    /// <summary>
    ///  用这个[PersistJobDataAfterExecution]标注任务是有状态的,
    ///  有状态的任务不允许并发执行 [DisallowConcurrentExecution]
    /// </summary>
    [PersistJobDataAfterExecution]//标注任务是有状态的
    [DisallowConcurrentExecution]//禁止相同JobDetail同时执行，而不是禁止多个不同JobDetail同时执行
    public class SaveHeatMapFile : JobBase
    {
        public Config config = new Config();

        /// <summary>
        /// 接口需要执行的方法
        /// </summary>
        public override void ExecuteMethod()
        {
            Save();
        }

        //自定义方法
        public void Save()
        {
            try
            {
                string FilePath = config.FilePath;
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                //WLog.WriteLog(Config.LogFileName, "【热力图报表数据服务】 ******获取场所开始*****");
                DataTable dtSites = GetSites();
                foreach (DataRow dr in dtSites.Rows)
                {
                    string SiteKey = dr["Sitekey"].ToString().Trim();
                    DateTime LastTime = GetSiteLastTime(SiteKey);

                    DateTime EndTime = DateTime.Parse(LastTime.ToString("yyyy-MM-dd")).AddHours(23).AddMinutes(59).AddSeconds(59);

                    string FileName = SiteKey + LastTime.ToString("yyyyMMdd").ToString();
                    string FullPath = FilePath + "\\" + FileName + ".csv";

                    LogHelper.Log("【热力图报表数据服务】 ******获取" + FileName + "数据开始*****");

                    DataTable dtData = GetHeatMapData(SiteKey, LastTime.ToString("yyyy-MM-dd").ToString(), EndTime.ToString("yyyy-MM-dd HH:mm:ss"));

                    if (dtData != null && dtData.Rows.Count > 0)
                    {
                        DataTableToCsv(dtData, FullPath);
                    }

                    //当天文件
                    if (EndTime.Date >= DateTime.Now.Date)
                    {
                        EndTime = DateTime.Now;
                    }
                    SetSiteLastTime(SiteKey, EndTime);
                    LogHelper.Log("【热力图报表数据服务】 *******获取" + FileName + "结束********");
                }

            }
            catch (Exception ex)
            {
                LogHelper.Log("【热力图报表数据服务】 ******* 异常错误 " + ex.Message + "********");
                throw ex;
            }

        }




        /// <summary>
        /// 获取热力数据
        /// </summary>
        /// <param name="SiteKey"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public DateTime GetSiteLastTime(string SiteKey)
        {
            SQLHelper db = new SQLHelper();
            try
            {
                SqlParameter[] commandParamers = new SqlParameter[1];
                commandParamers[0] = new SqlParameter("@SiteKey", SiteKey);

                string SQL_Text = @" IF NOT EXISTS(select * from dbo.System_HeatMap_File where SiteKey = @SiteKey)
                                     BEGIN
	                                    INSERT INTO dbo.System_HeatMap_File(SiteKey, FileTime) VALUES(@SiteKey,GETDATE())
                                     END
                                     SELECT * FROM dbo.System_HeatMap_File where SiteKey = @SiteKey";
                DataTable dt = db.ExecuteDataSet(config.DBConn, CommandType.Text, SQL_Text, commandParamers).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    return DateTime.Parse(dt.Rows[0]["FileTime"].ToString());
                }
                return DateTime.Now;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 设置本次同步时间
        /// </summary>
        /// <param name="SiteKey"></param>
        /// <param name="SyncTime"></param>
        public void SetSiteLastTime(string SiteKey, DateTime SyncTime)
        {
            SQLHelper db = new SQLHelper();
            try
            {
                SqlParameter[] commandParamers = new SqlParameter[2];
                commandParamers[0] = new SqlParameter("@SyncTime", SyncTime);
                commandParamers[1] = new SqlParameter("@SiteKey", SiteKey);

                string SQL_Text = "update dbo.System_HeatMap_File set FileTime = @SyncTime where SiteKey = @SiteKey";

                db.ExecuteNonQuery(config.DBConn, CommandType.Text, SQL_Text, commandParamers);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取上次热力图生成时间
        /// </summary>
        /// <param name="SiteKey"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public DataTable GetHeatMapData(string SiteKey, string StartDate, string EndDate)
        {
            SQLHelper db = new SQLHelper();
            try
            {
                SqlParameter[] commandParamers = new SqlParameter[4];
                commandParamers[0] = new SqlParameter("@SiteKey", SiteKey);
                commandParamers[1] = new SqlParameter("@StartTime", StartDate);
                commandParamers[2] = new SqlParameter("@EndTime", EndDate);
                commandParamers[3] = new SqlParameter("@TimeGradingCode", "D");

                return db.ExecuteDataSet(config.DBConn, CommandType.StoredProcedure, "usp_Report_Heat", commandParamers).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取场所
        /// </summary>
        /// <returns></returns>
        public DataTable GetSites()
        {
            SQLHelper db = new SQLHelper();
            DataTable dt = null;
            try
            {
                //dt = db.ExecuteDataTable(DBConn, CommandType.Text, "select SiteKey from dbo.Traffic_Sites where SiteType =300", null);
                //兼容百丽
                dt = db.ExecuteDataTable(config.DBConn, CommandType.StoredProcedure, "usp_GetHeatMapSyncSites", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// DataTable写入CSV
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public bool DataTableToCsv(DataTable dt, string strFilePath)
        {
            try
            {
                string strBufferLine = "";

                //删除文件 
                if (File.Exists(strFilePath))
                {
                    FileInfo fi = new FileInfo(strFilePath);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;

                    File.Delete(strFilePath);
                }

                StreamWriter strmWriterObj = new StreamWriter(strFilePath, false, System.Text.Encoding.UTF8);
                string ColumnName = string.Empty;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        ColumnName += dt.Columns[i].ColumnName;
                    }
                    else
                    {

                        ColumnName += dt.Columns[i].ColumnName + ",";
                    }
                }

                strmWriterObj.WriteLine(ColumnName);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strBufferLine = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j > 0)
                            strBufferLine += ",";
                        strBufferLine += dt.Rows[i][j].ToString();
                    }
                    strmWriterObj.WriteLine(strBufferLine);
                }
                strmWriterObj.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
