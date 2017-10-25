using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Impl;
using System.Configuration;
using System.Reflection;
using TaskManagerCommon;
using Quartz.Impl.Triggers;
using System.Collections.Specialized;

namespace TaskManagerUtility
{
    public static class QuartzHelper
    {

        /// <summary>
        /// 程序调度
        /// </summary>
        private static IScheduler _scheduler;

        private static string scheme = null;

        private static string server = null;

        private static string port = null;

        private static string remoteName = null;


        /// <summary>
        /// 缓存任务所在程序集信息
        /// </summary>
        private static Dictionary<string, Assembly> AssemblyDict = new Dictionary<string, Assembly>();


        /// <summary>
        /// 锁对象
        /// </summary>
        private static readonly object Obj = new object();

        static QuartzHelper()
        {
        }

        /// <summary>
        /// 初始化任务调度对象
        /// </summary>
        public static IScheduler InitScheduler()
        {
            try
            {
                lock (Obj)
                {
                    if (_scheduler != null) return _scheduler;

                    #region Quartz配置
                    //NameValueCollection properties = new NameValueCollection();


                    //properties["quartz.scheduler.instanceName"] = "QuartzDBDemo";

                    //// 设置线程池
                    //properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
                    //properties["quartz.threadPool.threadCount"] = "5";
                    //properties["quartz.threadPool.threadPriority"] = "Normal";

                    //// 远程输出配置
                    //properties["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz";
                    //properties["quartz.scheduler.exporter.port"] = "556";
                    //properties["quartz.scheduler.exporter.bindName"] = "QuartzScheduler";
                    //properties["quartz.scheduler.exporter.channelType"] = "tcp";


                    //// 驱动类型，这里用的mysql，目前支持如下驱动：
                    ////Quartz.Impl.AdoJobStore.FirebirdDelegate
                    ////Quartz.Impl.AdoJobStore.MySQLDelegate
                    ////Quartz.Impl.AdoJobStore.OracleDelegate
                    ////Quartz.Impl.AdoJobStore.SQLiteDelegate
                    ////Quartz.Impl.AdoJobStore.SqlServerDelegate
                    //properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz";

                    //// 数据源名称
                    //properties["quartz.jobStore.dataSource"] = "QuartzDB";

                    //// 数据库版本
                    ///* 数据库版本    MySql.Data.dll版本,二者必须保持一致
                    // * MySql-10    1.0.10.1
                    // * MySql-109   1.0.9.0
                    // * MySql-50    5.0.9.0
                    // * MySql-51    5.1.6.0
                    // * MySql-65    6.5.4.0
                    // * MySql-695   6.9.5.0
                    // *             System.Data
                    // * SqlServer-20         2.0.0.0
                    // * SqlServerCe-351      3.5.1.0
                    // * SqlServerCe-352      3.5.1.50
                    // * SqlServerCe-400      4.0.0.0
                    // * 其他还有OracleODP，Npgsql，SQLite，Firebird，OleDb
                    //*/
                    //properties["quartz.dataSource.QuartzDB.provider"] = "SqlServer-20";

                    //// 连接字符串
                    //properties["quartz.dataSource.QuartzDB.connectionString"] = "server=192.168.2.88;database=QuartzDB;uid=sa;pwd=winner@001";

                    //// 事物类型JobStoreTX自动管理 JobStoreCMT应用程序管理
                    //properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";

                    //// 表明前缀
                    //properties["quartz.jobStore.tablePrefix"] = "QRTZ_";

                    //// Quartz Scheduler唯一实例ID，auto：自动生成
                    //properties["quartz.scheduler.instanceId"] = "AUTO";

                    //// 集群
                    ////properties["quartz.jobStore.clustered"] = "true";

                    #endregion
                    //ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);
                    ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                    _scheduler = schedulerFactory.GetScheduler();

                    //MyListener listener = new MyListener();
                    //_scheduler.ListenerManager.AddJobListener(listener, GroupMatcher<JobKey>.AnyGroup());


                    return _scheduler;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 初始化任务调度对象
        /// </summary>
        public static void InitRemoteScheduler()
        {
            try
            {

                //lock (Obj)
                //{
                scheme = ConfigurationManager.AppSettings["QuartzScheme"].ToString();

                server = ConfigurationManager.AppSettings["QuartzServer"].ToString();

                port = ConfigurationManager.AppSettings["QuartzPort"].ToString();

                remoteName = ConfigurationManager.AppSettings["QuartzRemoteName"].ToString();

                NameValueCollection properties = new NameValueCollection();

                properties["quartz.scheduler.instanceName"] = "RemoteQuartzScheduler";
                properties["quartz.scheduler.proxy"] = "true";
                properties["quartz.scheduler.proxy.address"] = string.Format("{0}://{1}:{2}/{3}", scheme, server, port, remoteName);

                ISchedulerFactory sf = new StdSchedulerFactory(properties);

                //ISchedulerFactory sf = new StdSchedulerFactory();

                _scheduler = sf.GetScheduler();
                //}
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 获取作业
        /// </summary>
        /// <returns></returns>
        public static IScheduler GetScheduler()
        {
            try
            {
                if (_scheduler != null)
                {

                    return _scheduler;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        #region 启动，暂停
        /// <summary>
        /// 删除现有任务
        /// </summary>
        /// <param name="jobKey"></param>
        public static void DeleteJob(JobKey jk)
        {
            try
            {
                if (_scheduler.CheckExists(jk))
                {
                    //任务已经存在则删除
                    _scheduler.DeleteJob(jk);

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="jobKey"></param>
        public static void PauseJob(JobKey jk)
        {
            try
            {
                if (_scheduler.CheckExists(jk))
                {
                    //任务已经存在则暂停任务
                    _scheduler.PauseJob(jk);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static int PauseJob(string name, string group)
        {
            int result = -1;
            try
            {
              JobKey jobkey = JobKey.Create(name,group);
              PauseJob(jobkey);
              result = 0;
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 恢复运行暂停的任务
        /// </summary>
        /// <param name="jobKey">任务key</param>
        public static void ResumeJob(JobKey jk)
        {
            try
            {
                if (_scheduler.CheckExists(jk))
                {
                    //任务已经存在则暂停任务
                    _scheduler.ResumeJob(jk);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 恢复运行暂停的任务
        /// </summary>
        /// <param name="jobKey">任务key</param>
        public static int ResumeJob(string name ,string group)
        {
            int result = -1;
            try
            {
                JobKey jobkey = JobKey.Create(name, group);
                ResumeJob(jobkey);
                result = 0;
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }


        /// <summary>
        /// 停止任务调度
        /// </summary>
        public static void StartSchedule()
        {
            try
            {
                //判断调度是否已经关闭
                if (!_scheduler.IsShutdown)
                {
                    //等待任务运行完成
                    _scheduler.Start();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 停止任务调度
        /// </summary>
        public static void StopSchedule()
        {
            try
            {
                //判断调度是否已经关闭
                if (!_scheduler.IsShutdown)
                {
                    //等待任务运行完成
                    _scheduler.Shutdown(true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Pauses all activity in scheduler.
        /// </summary>
        public static void PauseAll()
        {
            try
            {
                _scheduler.PauseAll();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// Resumes all activity in server.
        /// </summary>
        public static void ResumeAll()
        {
            try
            {
                _scheduler.ResumeAll();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }


        #endregion


        /// <summary>
        /// 清除job
        /// </summary>
        public static void ClearSchedule()
        {

            try
            {
                //判断调度是否已经关闭
                if (!_scheduler.IsShutdown)
                {
                    //等待任务运行完成
                    _scheduler.Clear();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// 获取任务详细信息
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        public static IJobDetail GetJobDetail(JobKey jk)
        {
            try
            {
                //判断调度是否已经关闭
                if (_scheduler.CheckExists(jk))
                {
                    //任务已经存在则暂停任务
                    return _scheduler.GetJobDetail(jk);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 获取当前执行的任务
        /// </summary>
        /// <returns></returns>
        public static IList<IJobExecutionContext> GetCurrentlyExecutingJobs()
        {
            try
            {
                return _scheduler.GetCurrentlyExecutingJobs();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// 关联任务和触发器
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="trigger"></param>
        public static void ScheduleJob(IJobDetail jobDetail, ITrigger trigger)
        {
            try
            {
                if (_scheduler != null)
                {
                    //先删除现有已存在任务
                    DeleteJob(jobDetail.Key);


                    _scheduler.ScheduleJob(jobDetail, trigger);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 按分组获取所有任务
        /// </summary>
        /// <returns></returns>
        public static List<IJobDetail> GetAllJobs()
        {
            IList<string> jobgroups = _scheduler.GetJobGroupNames();

            List<IJobDetail> result = null;

            foreach (string str in jobgroups)
            {
                foreach (JobKey job in _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(str)))
                {
                    IJobDetail jobDetail = _scheduler.GetJobDetail(job);

                    if (jobDetail != null)
                    {
                        result.Add(jobDetail);
                    }
                }

            }
            return result;
        }

        /// <summary>
        /// 获取任务关联的触发器
        /// </summary>
        /// <param name="jobkey"></param>
        /// <returns></returns>
        public static IList<ITrigger> GetTriggersOfJob(JobKey jobkey)
        {
            try
            {
                return _scheduler.GetTriggersOfJob(jobkey);
            }
            catch (Exception)
            {

                throw;
            }

        }



        /// 获取类的属性、方法  
        /// </summary>  
        /// <param name="assemblyName">程序集</param>  
        /// <param name="className">类名</param>  
        public static Type GetClassInfo(string assemblyName, string className)
        {
            try
            {
                assemblyName = FileHelper.GetAbsolutePath(assemblyName + ".dll");
                System.Reflection.Assembly assembly = null;
                if (!AssemblyDict.TryGetValue(assemblyName, out assembly))
                {
                    assembly = System.Reflection.Assembly.LoadFrom(assemblyName);
                    AssemblyDict[assemblyName] = assembly;
                }
                Type type = assembly.GetType(className, true, true);
                return type;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 新增任务
        /// </summary>
        /// <param name="task"></param>
        public static void AddNewJob(TaskEntity task)
        {
            IJobDetail jobDetail = null;
            ITrigger trigger = null;
            try
            {
                if (task != null)
                {

                    jobDetail = new JobDetailImpl(task.Name, task.Group, QuartzHelper.GetClassInfo(task.AssemblyName, task.ClassName), true, true);
            
                    if (task.Type == 0)
                    {
                        trigger = new SimpleTriggerImpl()
                        {
                            Name = task.Name + "_Trigger",
                            Group = task.Group,
                            Description = task.Description,
                            RepeatInterval = task.RepeatInterval,
                            RepeatCount = task.RepeatCount,
                            StartTimeUtc = task.BeginTime,
                            EndTimeUtc = task.EndTime
                        };

                    }
                    else if (task.Type == 1)
                    {
                        trigger = new CronTriggerImpl()
                        {
                            Name = task.Name + "_Trigger",
                            Group = task.Group,
                            Description = task.Description,
                            CronExpressionString = task.CronExpression,
                            StartTimeUtc = task.BeginTime,
                            EndTimeUtc = task.EndTime
                        };
                    }

                    ScheduleJob(jobDetail, trigger);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        /// <summary>
        /// 验证是否Cron表达式
        /// </summary>
        /// <param name="CronExpression"></param>
        /// <returns></returns>
        public static bool IsCronExpression(string cronExpression)
        {
            return CronExpression.IsValidExpression(cronExpression);
        }


        /// <summary>
        /// 获取所有任务组
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetTaskGroups()
        {
            return _scheduler.GetJobGroupNames();
        }





        /// <summary>
        /// 根据组名获取任务
        /// </summary>
        /// <param name="GroupName"></param>
        /// <returns></returns>
        public static List<TaskEntity> GetTasks(string GroupName)
        {

            List<TaskEntity> jobs = new List<TaskEntity>();
            foreach (JobKey job in _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(GroupName)))
            {
                IJobDetail jobDetail = _scheduler.GetJobDetail(job);
                if (jobDetail != null)
                {
                    IList<ITrigger> triggers = GetTriggersOfJob(jobDetail.Key);
                    TaskEntity task = new TaskEntity();


                    TriggerState State = _scheduler.GetTriggerState(triggers[0].Key);


                    task.Name = jobDetail.Key.Name;
                    task.Group = jobDetail.Key.Group;
                    //task.Description = jobDetail.Description;
                    task.AssemblyName = jobDetail.JobType.Namespace.ToString();
                    task.ClassName = jobDetail.JobType.FullName.ToString();
                    task.BeginTime = triggers[0].StartTimeUtc;
                    if (triggers[0].EndTimeUtc != null)
                    {
                        task.EndTime = triggers[0].EndTimeUtc;
                    }
                    else
                    {
                        task.EndTime = null;
                    }
                    if (triggers[0] is SimpleTriggerImpl)
                    {
                        task.Type = 0;
                        task.RepeatCount = ((SimpleTriggerImpl)triggers[0]).RepeatCount;
                        task.RepeatInterval = ((SimpleTriggerImpl)triggers[0]).RepeatInterval;
                        task.FiredCount = ((SimpleTriggerImpl)triggers[0]).TimesTriggered;
                        task.Description = ((SimpleTriggerImpl)triggers[0]).Description;
                    }
                    else if (triggers[0] is CronTriggerImpl)
                    {
                        task.Type = 1;
                        task.CronExpression = ((CronTriggerImpl)triggers[0]).CronExpressionString.ToString();
                        task.Description = ((CronTriggerImpl)triggers[0]).Description;
                    }
                    if (triggers[0].GetNextFireTimeUtc() != null)
                    {
                        task.NextFireTime = triggers[0].GetNextFireTimeUtc();
                    }
                    else
                    {
                        task.NextFireTime = null;
                    }
                    task.State = State.ToString();
                    jobs.Add(task);
                }
            }
            return jobs;
        }

        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static TaskEntity GetTask(string name, string group)
        {

            JobKey jobkey = new JobKey(name, group);
            IJobDetail jobDetail = _scheduler.GetJobDetail(jobkey);

            if (jobDetail != null)
            {
                TaskEntity task = new TaskEntity();

                IList<ITrigger> triggers = GetTriggersOfJob(jobDetail.Key);

                TriggerState State = _scheduler.GetTriggerState(triggers[0].Key);

                task.Name = jobDetail.Key.Name;
                task.Group = jobDetail.Key.Group;
                //task.Description = jobDetail.Description;
                task.AssemblyName = jobDetail.JobType.Namespace.ToString();
                task.ClassName = jobDetail.JobType.FullName.ToString();
                task.BeginTime = triggers[0].StartTimeUtc;
                if (triggers[0].EndTimeUtc != null)
                {
                    task.EndTime = triggers[0].EndTimeUtc;
                }
                else
                {
                    task.EndTime = null;
                }
                if (triggers[0] is SimpleTriggerImpl)
                {
                    task.Type = 0;
                    task.RepeatCount = ((SimpleTriggerImpl)triggers[0]).RepeatCount;
                    task.RepeatInterval = ((SimpleTriggerImpl)triggers[0]).RepeatInterval;
                    task.FiredCount = ((SimpleTriggerImpl)triggers[0]).TimesTriggered;
                    task.Description = ((SimpleTriggerImpl)triggers[0]).Description;
                }
                else if (triggers[0] is CronTriggerImpl)
                {
                    task.Type = 1;
                    task.CronExpression = ((CronTriggerImpl)triggers[0]).CronExpressionString.ToString();
                    task.Description = ((CronTriggerImpl)triggers[0]).Description;
                }
                if (triggers[0].GetNextFireTimeUtc() != null)
                {
                    task.NextFireTime = triggers[0].GetNextFireTimeUtc();
                }
                else
                {
                    task.NextFireTime = null;
                }
                task.State = State.ToString();
                return task;
            }
            return null;
        }



    }


}
