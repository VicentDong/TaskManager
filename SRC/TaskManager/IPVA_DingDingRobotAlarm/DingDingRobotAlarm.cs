using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManagerTaskSet;
using TaskManagerCommon;
using System.Net;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;

namespace IPVA_DingDingRobotAlarm
{
    public class DingDingRobotAlarm : JobBase
    {
        public Config config = new Config();
        /// <summary>
        /// 接口需要执行的方法
        /// </summary>
        public override void ExecuteMethod()
        {
            SendAlarmMsg();
        }

        //自定义方法
        /// <summary>
        /// 钉钉推送消息方法
        /// </summary>
        public void SendAlarmMsg()
        {
            try
            {
                SQLHelper db = new SQLHelper();

                DataTable dt = db.ExecuteDataSet(config.DBConn, CommandType.StoredProcedure, "usp_job_ISR_GetUrl", null).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string URL = dt.Rows[i]["Url"].ToString();
                        LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】获取推送URL" + URL);
                        //推送数据方法
                        SqlParameter[] commandParamers = new SqlParameter[1];
                        commandParamers[0] = new SqlParameter("@Url", URL);
                        DataTable dt1 = db.ExecuteDataSet(config.DBConn, CommandType.StoredProcedure, "usp_job_ISR_GetInformationTable", commandParamers).Tables[0];
                        LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】获取待推送数据" + dt1.Rows.Count.ToString() + "条");
                        if (dt1 != null && dt1.Rows.Count > 0)
                        {
                            for (int j = 0; j < dt1.Rows.Count; j++)
                            {
                                int ID = Convert.ToInt32(dt1.Rows[j]["ID"]);
                                string AlarmContent = dt1.Rows[j]["AlarmContent"].ToString();

                                //推送方法
                                //序列化处理json转义
                                JsonClass js = new JsonClass();
                                js.msgtype = "text";
                                js.text = new ContextObj();
                                js.text.content = AlarmContent+ "\n推送时间:" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                                string textMsg = getJsonByObject(js);

                                //string textMsg = "{ \"msgtype\": \"text\", \"text\": {\"content\": \"" + msg + "\"}}";
                                LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】推送消息 json" + textMsg);
                                string result = PostSendMessages(URL, textMsg, null);

                                // 解析json数据responseText
                                ParsingResults(result, ID);

                                //更新发送状态
                                Update_SendingState(ID);
                            }
                        }
                        else
                        {
                            string SiteName = "";
                            DataTable dt2 = db.ExecuteDataSet(config.DBConn, CommandType.StoredProcedure, "usp_job_ISR_GetProject", null).Tables[0];
                            for (int k = 0; k < dt2.Rows.Count; k++)
                            {
                                SiteName = dt2.Rows[i]["SiteName"].ToString();
                            }
                            //无发送数据
                            string msg = "告警项目:" + SiteName + "\n" + "告警内容:" + "数据正常\n" + "告警时间：" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                            string textMsg = "{ \"msgtype\": \"text\", \"text\": {\"content\": \"" + msg + "\"}}";
                            LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】推送消息 json" + textMsg);
                            string result = PostSendMessages(URL, textMsg, null);
                        }
                    }
                }
                else
                {
                    LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】获取usp_job_ISR_GetUrl中URL失败！");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】失败" + ex.Message);
            }
        }
        /// <summary>
        /// 更新数据发送表中状态
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="newid">newid</param>
        public void Update_SendingState(int id)
        {
            try
            {
                SQLHelper db = new SQLHelper();
                SqlParameter[] commandParamers = new SqlParameter[1];
                commandParamers[0] = new SqlParameter("@ID", id);
                db.ExecuteNonQuery(config.DBConn, CommandType.StoredProcedure, "usp_job_ISR_UpdateSendState", commandParamers);
                LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】发送消息状态更新成功 ");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】更新数据发送表中状态更新失败" + ex.Message);
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
        /// <summary>
        /// 解析执行结果
        /// </summary>
        /// <param name="result"></param>
        public void ParsingResults(string result,int id)
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
                    LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】发送消息状态失败: id=" + id + "*****异常消息:" + message);
                }
            }
            catch (IOException ex)
            {
                LogHelper.WriteLog(this.GetType().Name, "【钉钉推送消息方法】解析执行结果失败！" + ex.Message);
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
}
