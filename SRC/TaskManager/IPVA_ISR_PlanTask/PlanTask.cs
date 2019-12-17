using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManagerTaskSet;
using System.Net;
using System.IO;
using TaskManagerCommon;
using System.Data;

namespace IPVA_ISR_PlanTask
{
    public class PlanTask : JobBase
    {
        public Config config = new Config();
        /// <summary>
        /// 接口需要执行的方法
        /// </summary>
        public override void ExecuteMethod()
        {
            Get_PlanTask();
        }
        public void Get_PlanTask()
        {
            try
            {
                SQLHelper db = new SQLHelper();
                int result = db.ExecuteNonQuery(config.DBConn, CommandType.StoredProcedure, "usp_job_ISR_PlanTask", null);
                LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】定时执行usp_job_ISR_PlanTask成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】定时执行usp_job_ISR_PlanTask出现异常！" + ex.Message);
            }
        }
    }
}
