using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManagerCommon;
using Quartz;
using TaskManagerUtility;

namespace TaskManagerTaskSet
{
    /// <summary>
    ///  用这个[PersistJobDataAfterExecution]标注任务是有状态的,
    ///  有状态的任务不允许并发执行 [DisallowConcurrentExecution]
    /// </summary>
    [PersistJobDataAfterExecution]//标注任务是有状态的
    [DisallowConcurrentExecution]//禁止相同JobDetail同时执行，而不是禁止多个不同JobDetail同时执行
    public class JobBase : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                ExecuteJob(context, () =>
                {
                    //执行方法
                    ExecuteMethod();
                });
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                //1.立即重新执行任务 
                e2.RefireImmediately = true;
            }

        }

        public virtual void ExecuteMethod()
        {


        }

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
                LogHelper.ServerInfo(context.Trigger.Key.Name + "开始执行");

                //记录数据库日志
                DBLogHelper.WriteRunLog(context.Trigger.JobKey.Name, context.Trigger.JobKey.Group, "开始执行");

                //开始执行
                PerformanceTracer.Invoke(action, context.Trigger.JobKey.Name);

                //记录文件日志- 开始执行相关任务
                LogHelper.ServerInfo(context.Trigger.Key.Name + "成功执行");
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
                LogHelper.ServerError(e2.Message, ex);

                // 记录异常到数据库和 log 文件中。
                DBLogHelper.WriteErrorLog(context.Trigger.JobKey.Name, context.Trigger.JobKey.Group, ex.Message);

            }
        }




    }
}
