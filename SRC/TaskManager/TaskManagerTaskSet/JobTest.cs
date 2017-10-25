using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using TaskManagerUtility;
using Common.Logging;
using TaskManagerCommon;

namespace TaskManagerTaskSet
{
    public class JobTest :  JobBase, IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(JobTest));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                base.ExecuteJob(context, () => {
                    //执行方法
                    SyncHeatMapData();
                });

            }
            catch (Exception ex)
            {

                JobExecutionException e2 = new JobExecutionException(ex);
                //1.立即重新执行任务 
                e2.RefireImmediately = true;
            }

        }

        public void SyncHeatMapData()
        {

            string sss = "11";

        }
    }
}
