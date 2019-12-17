using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManagerTaskSet;
using System.Net;
using System.IO;
using TaskManagerCommon;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;

namespace IPVA_PlanTaskAlarm
{
    public class PlanTaskAlarm : JobBase
    {
        public Config config = new Config();
        /// <summary>
        /// 接口需要执行的方法
        /// </summary>
        public override void ExecuteMethod()
        {
            SendPlanTask();
        }
        public void SendPlanTask()
        {
            SQLHelper db = new SQLHelper();
            //获取URL
            DataTable dt = db.ExecuteDataSet(config.DBConn, CommandType.StoredProcedure, "usp_job_ISR_GetUrl", null).Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string URL = dt.Rows[i]["Url"].ToString();
                    LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】获取推送URL" + URL);

                    //获取执行成功数据
                    DataSet ds = db.ExecuteDataSet(config.DBConn, CommandType.StoredProcedure, "usp_job_ISR_PlanTask", null);
                    LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】获取成功待推送数据" + ds.Tables[0].Rows.ToString() + "条");
                    
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            string SuccessMsg = item["SuccessMsg"].ToString();

                            //序列化处理json转义
                            JsonClass js = new JsonClass();
                            js.msgtype = "text";
                            js.text = new ContextObj();
                            js.text.content = SuccessMsg;

                            string textMsg = getJsonByObject(js);

                            //推送方法
                            //   string textMsg = "{ \"msgtype\": \"text\", \"text\": {\"content\": \"" + SuccessMsg + "\"}}";

                            LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】推送消息 json" + textMsg);
                            string result = PostSendMessages(URL, textMsg, null);

                            // 解析json数据responseText
                            ParsingResults(result);
                        }
                    }
                    else
                    {
                        LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】计划任务执行成功数据，无待发数据！");
                    }

                    LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】获取失败待推送数据" + ds.Tables[1].Rows.ToString() + "条");
                    //获取执行失败数据
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow item in ds.Tables[1].Rows)
                        {
                            string FailedMsg = item["FailedMsg"].ToString();

                            //序列化处理json转义
                            JsonClass js = new JsonClass();
                            js.msgtype = "text";
                            js.text = new ContextObj();
                            js.text.content = FailedMsg;

                            string textMsg = getJsonByObject(js);
                            //推送方法
                            //   string textMsg = "{ \"msgtype\": \"text\", \"text\": {\"content\": \"" + FailedMsg + "\"}}";
                            LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】推送消息 json" + textMsg);

                            string result = PostSendMessages(URL, textMsg, null);

                            // 解析json数据responseText
                            ParsingResults(result);
                        }
                    }
                    else
                    {
                        LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】计划任务执行失败数据，无待发数据！");
                    }
                }
            }
        }


        private static string getJsonByObject(Object obj)
        {
            //实例化DataContractJsonSerializer对象，需要待序列化的对象类型
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            //实例化一个内存流，用于存放序列化后的数据
            MemoryStream stream = new MemoryStream();
            //使用WriteObject序列化对象
            serializer.WriteObject(stream, obj);
            //写入内存流中
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            //通过UTF8格式转换为字符串
            return Encoding.UTF8.GetString(dataBytes);
        }

        /// <summary>
        /// 解析执行结果
        /// </summary>
        /// <param name="result"></param>
        public void ParsingResults(string result)
        {
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(result.ToString());
                string message = jo["errmsg"].ToString();
                if (message == "ok")
                {
                    //发送消息成功
                    LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】发送消息成功");
                }
                else
                {
                    LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】发送消息失败:" + message);
                }
            }
            catch (IOException ex)
            {
                LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】解析执行结果失败！" + ex.Message);
            }
        }
        /// <summary>
        /// 以Post方式提交命令 
        /// </summary>
        /// <param name="apiurl">请求的URL</param>
        /// <param name="jsonString">请求的json参数</param>
        /// <param name="headers">请求头的key-value字典</param>
        /// <returns></returns>
        public static String PostSendMessages(string apiurl, string jsonString, Dictionary<String, String> headers = null)
        {
            WebRequest request = WebRequest.Create(@apiurl);
            request.Method = "POST";
            request.ContentType = "application/json";
            if (headers != null)
            {
                foreach (var keyValue in headers)
                {
                    if (keyValue.Key == "Content-Type")
                    {
                        request.ContentType = keyValue.Value;
                        continue;
                    }
                    request.Headers.Add(keyValue.Key, keyValue.Value);
                }
            }
            if (String.IsNullOrEmpty(jsonString))
            {
                request.ContentLength = 0;
            }
            else
            {
                byte[] bs = Encoding.UTF8.GetBytes(jsonString);
                request.ContentLength = bs.Length;
                Stream newStream = request.GetRequestStream();
                newStream.Write(bs, 0, bs.Length);
                newStream.Close();
            }
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            Encoding encode = Encoding.UTF8;
            StreamReader reader = new StreamReader(stream, encode);
            string resultJson = reader.ReadToEnd();
            return resultJson;
        }
    }

    public class JsonClass
    {
        public string msgtype { get; set; }
        public ContextObj text { get; set; }
    }
    public class ContextObj
    {
        public string content { get; set; }
    }
}
