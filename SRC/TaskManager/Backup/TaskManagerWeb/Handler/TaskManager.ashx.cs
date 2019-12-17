using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO.Compression;
using System.Data;
using TaskManagerUtility;
using TaskManagerCommon;
using Newtonsoft.Json;

namespace TaskManagerWeb.Handler
{
    /// <summary>
    /// TaskManager1 的摘要说明
    /// </summary>
    public class TaskManager : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest httpRequest = context.Request;
            HttpResponse httpResponse = context.Response;

            #region 参数
            string MethodID = string.Empty;
            string taskName = string.Empty;
            string groupName = string.Empty;
            string pageSize = string.Empty;
            string pageIndex = string.Empty;

            if (httpRequest.Form["MethodID"] != null)
            {
                MethodID = httpRequest.Form["MethodID"].ToString();
            }
            else if (httpRequest.QueryString["MethodID"] != null)
            {
                MethodID = httpRequest.QueryString["MethodID"].ToString();
            }
            if (httpRequest.Form["taskName"] != null)
            {
                taskName = httpRequest.Form["taskName"].ToString();
            }
            else if (httpRequest.QueryString["taskName"] != null)
            {
                taskName = httpRequest.QueryString["taskName"].ToString();
            }
            if (httpRequest.Form["groupName"] != null)
            {
                groupName = httpRequest.Form["groupName"].ToString();
            }
            else if (httpRequest.QueryString["groupName"] != null)
            {
                groupName = httpRequest.QueryString["groupName"].ToString();
            }
            if (httpRequest.Form["pageSize"] != null)
            {
                pageSize = httpRequest.Form["pageSize"].ToString();
            }
            else if (httpRequest.QueryString["pageSize"] != null)
            {
                pageSize = httpRequest.QueryString["pageSize"].ToString();
            }
            if (httpRequest.Form["pageIndex"] != null)
            {
                pageIndex = httpRequest.Form["pageIndex"].ToString();
            }
            else if (httpRequest.QueryString["pageIndex"] != null)
            {
                pageIndex = httpRequest.QueryString["pageIndex"].ToString();
            }
            #endregion


            switch (MethodID)
            {
                case "GetTasksByGroup":
                    httpResponse.Write(GetTasksByGroup());
                    break;
                case "GetTask":
                    httpResponse.Write(GetTask(taskName, groupName));
                    break;
                case "GetTaskLog":
                    httpResponse.Write(GetTaskLog(taskName, groupName, pageSize, pageIndex));
                    break;
                case "PauseJob":
                    httpResponse.Write(PauseJob(taskName, groupName));
                    break;
                case "ResumeJob":
                    httpResponse.Write(ResumeJob(taskName, groupName));
                    break;
            }

            //对Response 启用Gzip 压缩
            httpResponse.ContentType = "text/plain";
            string acceptEncoding = httpRequest.Headers["Accept-Encoding"].ToString().ToUpperInvariant();

            if (!String.IsNullOrEmpty(acceptEncoding))
            {
                if (acceptEncoding.Contains("GZIP"))
                {
                    //输出流头部<span style="font-family: Arial, Helvetica, sans-serif;">GZIP</span>压缩  
                    httpResponse.AppendHeader("Content-encoding", "gzip");
                    httpResponse.Filter = new GZipStream(httpResponse.Filter, CompressionMode.Compress);
                }
                else if (acceptEncoding.Contains("DEFLATE"))
                {
                    //<span style="font-family: Arial, Helvetica, sans-serif;">输出流头部</span><span style="font-family: Arial, Helvetica, sans-serif;">DEFLATE</span><span style="font-family: Arial, Helvetica, sans-serif;">压缩</span><span style="font-family: Arial, Helvetica, sans-serif;">  </span>   
                    httpResponse.AppendHeader("Content-encoding", "deflate");
                    httpResponse.Filter = new DeflateStream(httpResponse.Filter, CompressionMode.Compress);
                }
            }



            httpResponse.Flush();
            //清空缓存
            httpResponse.Clear();
            //关闭与客户端的链接
            httpResponse.End();

        }

        /// <summary>
        /// 获取所有分组任务
        /// </summary>
        /// <returns></returns>
        public string GetTasksByGroup()
        {
            List<JobEntity> jobs = new List<JobEntity>();
            IList<string> groups = QuartzHelper.GetTaskGroups();
            foreach (string group in groups)
            {
                JobEntity job = new JobEntity();
                job.group = group;
                job.tasks = QuartzHelper.GetTasks(group);
                jobs.Add(job);
            }
            return JsonConvert.SerializeObject(jobs);

        }

        /// <summary>
        /// 获取任务日志
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public string GetTaskLog(string taskName, string groupName, string pageSize, string pageIndex)
        {
            // DataTable dtLog = DBLogHelper.GetTaksLog(taskName, groupName, int.Parse(pageSize), int.Parse(pageIndex));

            Dictionary<string, object> pageDict = DBLogHelper.GetTaskLogByPage(taskName, groupName, int.Parse(pageSize), int.Parse(pageIndex));

            return JsonConvert.SerializeObject(pageDict);
        }
        /// <summary>
        /// 获取任务信息
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public string GetTask(string taskName, string groupName)
        {
            return JsonConvert.SerializeObject(QuartzHelper.GetTask(taskName, groupName));
        }


        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public string PauseJob(string taskName, string groupName)
        {
            int result = QuartzHelper.PauseJob(taskName, groupName);
            return JsonConvert.SerializeObject(result);

        }

        /// <summary>
        /// 恢复任务
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public string ResumeJob(string taskName, string groupName)
        {
            int result = QuartzHelper.ResumeJob(taskName, groupName);
            return JsonConvert.SerializeObject(result);

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }



    }

    public class JobEntity
    {
        public string group { get; set; }

        public List<TaskEntity> tasks { get; set; }

    }
}