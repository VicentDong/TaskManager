using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using TaskManagerCommon;
using System.Configuration;
using System.Data;
using TaskManagerTaskSet;
using System.Data.SqlClient;

namespace BatchTrafficTasks
{
    /// <summary>
    ///  用这个[PersistJobDataAfterExecution]标注任务是有状态的,
    ///  有状态的任务不允许并发执行 [DisallowConcurrentExecution]
    /// </summary>
    [PersistJobDataAfterExecution]//标注任务是有状态的
    [DisallowConcurrentExecution]//禁止相同JobDetail同时执行，而不是禁止多个不同JobDetail同时执行

    public class SummaryBatch : JobBase
    {
        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        private string CONNECTION_STRINGS = ConfigurationManager.ConnectionStrings["BatchTrafficConnectionString"].ToString();

        /// <summary>
        /// 接口需要执行的方法
        /// </summary>
        public override void ExecuteMethod()
        {
            SummaryBatchData();
            SummaryBatchThirty();
            SummaryBatchSixty();
            SummaryBatchDay();
            SummaryBatchWeek();
            SummaryBatchMonth();
            SummaryBatchYear();
        }

        /// <summary>
        /// 基础数据入批次数据表
        /// </summary>
        public void SummaryBatchData()
        {
            SQLHelper sqlHelper = new SQLHelper();
            try
            {
                sqlHelper.ExecuteNonQuery(CONNECTION_STRINGS, CommandType.StoredProcedure, "usp_Job_TrafficDataToBatch", null);

                LogHelper.WriteLog(this.GetType().Name, "执行基础数据入批次数据表usp_Job_TrafficDataToBatch成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(this.GetType().Name, "执行基础数据入批次数据表usp_Job_TrafficDataToBatch失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 30分钟汇总
        /// </summary>
        public void SummaryBatchThirty()
        {
            SQLHelper sqlHelper = new SQLHelper();
            try
            {
                SqlParameter[] pars = new SqlParameter[1]{
                    new SqlParameter("@TimeGradingCode","30"),
                };
                sqlHelper.ExecuteNonQuery(CONNECTION_STRINGS, CommandType.StoredProcedure, "usp_Job_SummaryData_Batch", pars);

                LogHelper.WriteLog(this.GetType().Name, "执行30分钟汇总usp_Job_SummaryData_Batch 30成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(this.GetType().Name, "执行30分钟汇总usp_Job_SummaryData_Batch 30失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 60分钟汇总
        /// </summary>
        public void SummaryBatchSixty()
        {
            SQLHelper sqlHelper = new SQLHelper();
            try
            {
                SqlParameter[] pars = new SqlParameter[1]{
                    new SqlParameter("@TimeGradingCode","60"),
                };
                sqlHelper.ExecuteNonQuery(CONNECTION_STRINGS, CommandType.StoredProcedure, "usp_Job_SummaryData_Batch", pars);

                LogHelper.WriteLog(this.GetType().Name, "执行60分钟汇总usp_Job_SummaryData_Batch 60成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(this.GetType().Name, "执行60分钟汇总usp_Job_SummaryData_Batch 60失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 天汇总
        /// </summary>
        public void SummaryBatchDay()
        {
            SQLHelper sqlHelper = new SQLHelper();
            try
            {
                SqlParameter[] pars = new SqlParameter[1]{
                    new SqlParameter("@TimeGradingCode","D"),
                };
                sqlHelper.ExecuteNonQuery(CONNECTION_STRINGS, CommandType.StoredProcedure, "usp_Job_SummaryData_Batch", pars);

                LogHelper.WriteLog(this.GetType().Name, "执行天汇总usp_Job_SummaryData_Batch D 成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(this.GetType().Name, "执行天汇总usp_Job_SummaryData_Batch D 失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 周汇总
        /// </summary>
        public void SummaryBatchWeek()
        {
            SQLHelper sqlHelper = new SQLHelper();
            try
            {
                SqlParameter[] pars = new SqlParameter[1]{
                    new SqlParameter("@TimeGradingCode","W"),
                };
                sqlHelper.ExecuteNonQuery(CONNECTION_STRINGS, CommandType.StoredProcedure, "usp_Job_SummaryData_Batch", pars);

                LogHelper.WriteLog(this.GetType().Name, "执行天汇总usp_Job_SummaryData_Batch W 成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(this.GetType().Name, "执行天汇总usp_Job_SummaryData_Batch W 失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 月汇总
        /// </summary>
        public void SummaryBatchMonth()
        {
            SQLHelper sqlHelper = new SQLHelper();
            try
            {
                SqlParameter[] pars = new SqlParameter[1]{
                    new SqlParameter("@TimeGradingCode","M"),
                };
                sqlHelper.ExecuteNonQuery(CONNECTION_STRINGS, CommandType.StoredProcedure, "usp_Job_SummaryData_Batch", pars);

                LogHelper.WriteLog(this.GetType().Name, "执行天汇总usp_Job_SummaryData_Batch M 成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(this.GetType().Name, "执行天汇总usp_Job_SummaryData_Batch M 失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 年汇总
        /// </summary>
        public void SummaryBatchYear()
        {
            SQLHelper sqlHelper = new SQLHelper();
            try
            {
                SqlParameter[] pars = new SqlParameter[1]{
                    new SqlParameter("@TimeGradingCode","Y"),
                };
                sqlHelper.ExecuteNonQuery(CONNECTION_STRINGS, CommandType.StoredProcedure, "usp_Job_SummaryData_Batch", pars);

                LogHelper.WriteLog(this.GetType().Name, "执行天汇总usp_Job_SummaryData_Batch Y 成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(this.GetType().Name, "执行天汇总usp_Job_SummaryData_Batch Y 失败：" + ex.Message);
            }
        }
    }
}
