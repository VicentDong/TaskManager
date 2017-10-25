using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManagerCommon
{
    public class TaskEntity
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 任务组名
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// 任务类名
        /// </summary>
        public string ClassName { get; set; }


        /// <summary>
        /// 任务类型 0： SimpleType   1: CronType
        /// </summary>
        public int Type { get; set; }


        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTimeOffset BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }


        /// <summary>
        /// Cron表达式
        /// </summary>
        public string CronExpression { get; set; }


        /// <summary>
        /// 重复周期
        /// </summary>
        public TimeSpan RepeatInterval { get; set; }


        /// <summary>
        /// 重复次数
        /// </summary>
        public int RepeatCount { get; set; }


        /// <summary>
        /// 下次触发时间
        /// </summary>
        public DateTimeOffset? NextFireTime { get; set; }


        /// <summary>
        /// 触发次数
        /// </summary>
        public int FiredCount { get; set; }


        /// <summary>
        /// 执行状态
        /// </summary>
        public string State { get; set; }


        /// <summary>
        /// 执行时间
        /// </summary>
        public double ExcuteTime { get; set; }
    }
}
