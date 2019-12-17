using System;
using Common.Logging;
using Quartz.Impl;
using Topshelf;
using Quartz;
using TaskManagerUtility;
using TaskManagerCommon;

namespace TaskManagerServer
{
	/// <summary>
	/// The main server logic.
	/// </summary>
	public class QuartzServer : ServiceControl, IQuartzServer
	{

		private IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzServer"/> class.
        /// </summary>
	    public QuartzServer()
	    {

	    }

	    /// <summary>
		/// Initializes the instance of the <see cref="QuartzServer"/> class.
		/// </summary>
		public virtual void Initialize()
		{
			try
			{
                scheduler = QuartzHelper.InitScheduler();
			}
			catch (Exception e)
			{
				LogHelper.ServerError("服务初始化失败:" + e.Message, e);
				throw;
			}
		}



        /// <summary>
        /// Returns the current scheduler instance (usually created in <see cref="Initialize" />
        /// using the <see cref="GetScheduler" /> method).
        /// </summary>
	    protected virtual IScheduler Scheduler
	    {
	        get { return scheduler; }
	    }

	    /// <summary>
        /// Creates the scheduler factory that will be the factory
        /// for all schedulers on this instance.
        /// </summary>
        /// <returns></returns>
	    protected virtual ISchedulerFactory CreateSchedulerFactory()
	    {
	        return new StdSchedulerFactory();
	    }

	    /// <summary>
		/// Starts this instance, delegates to scheduler.
		/// </summary>
		public virtual void Start()
		{
	        try
	        {
                QuartzHelper.StartSchedule();
	        }
	        catch (Exception ex)
	        {
                LogHelper.ServerError(string.Format("服务启动失败: {0}", ex.Message), ex);
	            throw;
	        }

            LogHelper.ServerInfo("作业启动成功");
		}

		/// <summary>
		/// Stops this instance, delegates to scheduler.
		/// </summary>
		public virtual void Stop()
		{
            try
            {
                QuartzHelper.StopSchedule();
            }
            catch (Exception ex)
            {
                LogHelper.ServerError(string.Format("作业停止失败: {0}", ex.Message), ex);
                throw;
            }

            LogHelper.ServerInfo("作业停止完成");
		}

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
	    public virtual void Dispose()
	    {
	        // no-op for now
	    }

        /// <summary>
        /// Pauses all activity in scheduler.
        /// </summary>
	    public virtual void Pause()
	    {
            QuartzHelper.PauseAll();
	    }

        /// <summary>
        /// Resumes all activity in server.
        /// </summary>
	    public void Resume()
	    {
            QuartzHelper.ResumeAll();
	    }

	    /// <summary>
        /// TopShelf's method delegated to <see cref="Start()"/>.
	    /// </summary>
	    public bool Start(HostControl hostControl)
	    {
	        Start();
	        return true;
	    }

        /// <summary>
        /// TopShelf's method delegated to <see cref="Stop()"/>.
        /// </summary>
        public bool Stop(HostControl hostControl)
	    {
	        Stop();
	        return true;
	    }

        /// <summary>
        /// TopShelf's method delegated to <see cref="Pause()"/>.
        /// </summary>
        public bool Pause(HostControl hostControl)
	    {
	        Pause();
	        return true;
	    }

        /// <summary>
        /// TopShelf's method delegated to <see cref="Resume()"/>.
        /// </summary>
        public bool Continue(HostControl hostControl)
	    {
	        Resume();
	        return true;
	    }
	}
}
