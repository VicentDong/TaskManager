using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManagerTaskSet;
using TaskManagerCommon;
using System.Data.SqlClient;
using System.Data;

namespace IPVA_ISR_LevelDrops
{
    public class SubpackageLevelDrops : JobBase
    {
        public Config config = new Config();
        /// <summary>
        /// 接口需要执行的方法
        /// </summary>
        public override void ExecuteMethod()
        {
            Get_LevelDrops();
        }
        /// <summary>
        /// 封装点位掉线存储过程
        /// </summary>
        public void Get_LevelDrops()
        {
            try
            {
                SQLHelper db = new SQLHelper();
                int result = db.ExecuteNonQuery(config.DBConn, CommandType.StoredProcedure, "usp_job_ISR_LevelDrops", null);
                LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】定时执行usp_job_ISR_LevelDrops成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】定时执行usp_job_ISR_LevelDrops出现异常！" + ex.Message);
            }
        }
    }
}
