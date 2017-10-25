using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using TaskManagerCommon;
using Common.Logging;

namespace TaskManagerUtility
{

    public class JobBase
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(JobBase));


        /// <summary>
        /// 执行指定任务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="action"></param>
        public void ExecuteJob(IJobExecutionContext context, Action action)
        {
            try
            {
                //记录文件日志- 开始执行相关任务
                logger.Info(context.Trigger.Key.Name + "开始执行");

                //记录数据库日志
                DBLogHelper.WriteRunLog(context.Trigger.JobKey.Name, context.Trigger.JobKey.Group, "开始执行");

                //开始执行
                PerformanceTracer.Invoke(action, context.Trigger.JobKey.Name);

                //记录文件日志- 开始执行相关任务
                logger.Info(context.Trigger.Key.Name + "成功执行");
                //记录数据库日志
                DBLogHelper.WriteRunLog(context.Trigger.JobKey.Name, context.Trigger.JobKey.Group, "成功执行");

            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                //true  是立即重新执行任务 
                e2.RefireImmediately = true;

                //记录文件日志- 开始执行相关任务
                //logger.Warn(ex.Message);
                logger.Error(e2.Message);

                // 记录异常到数据库和 log 文件中。
                DBLogHelper.WriteErrorLog(context.Trigger.JobKey.Name, context.Trigger.JobKey.Group, ex.Message);

            }
        }




    }
}
