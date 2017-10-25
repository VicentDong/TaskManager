using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Common.Logging;
using System.Diagnostics;

namespace TaskManagerUtility
{
    public static class PerformanceTracer
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(PerformanceTracer));

        private static int performanceTracer;


        static PerformanceTracer()
        {
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["PerformanceTracer"]))
            {
                int.TryParse(ConfigurationManager.AppSettings["PerformanceTracer"], out performanceTracer);
            }
            else
            {
                performanceTracer = -1;
            }
        }


        /// <summary>
        /// 调用并且跟踪指定代码块的执行时间。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="traceName">跟踪名称。</param>
        /// <param name="enableThrow">指定是否允许抛出异常。</param>
        public static void Invoke(Action action, string traceName = null, bool enableThrow = true)
        {
            if (performanceTracer >= 0)
            {
                if (string.IsNullOrWhiteSpace(traceName))
                {
                    var method = new StackTrace().GetFrames()[1].GetMethod();

                    traceName = string.Format("{0}.{1}", method.ReflectedType.FullName, method.Name);
                }

                try
                {
                    Stopwatch stopwatch = new Stopwatch();

                    stopwatch.Start();

                    action();

                    stopwatch.Stop();

                    if (stopwatch.Elapsed.TotalMilliseconds > performanceTracer)
                    {
                        if (!Debugger.IsAttached)
                        {
                            logger.Info("性能问题(" + traceName + ")" + stopwatch.Elapsed.TotalMilliseconds + "ms");
                        }

                        Debug.WriteLine("性能问题({0})，耗时{1}毫秒。", traceName, stopwatch.Elapsed.TotalMilliseconds);
                    }
                }
                catch (Exception ex)
                {
                    if (enableThrow)
                    {
                        throw;
                    }
                    logger.Error(ex.Message, ex);
                }
            }
            else
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    if (enableThrow)
                    {
                        throw;
                    }

                    logger.Error(ex.Message, ex);
                }
            }
        }


    }
}
