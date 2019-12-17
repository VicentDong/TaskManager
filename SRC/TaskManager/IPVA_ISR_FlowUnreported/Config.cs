using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TaskManagerCommon;
using System.Xml;

namespace IPVA_ISR_FlowUnreported
{
    public class Config
    {
        public string DBConn;
        public Config()
        {
            string ConfigPath = System.Environment.CurrentDirectory + @"\Config\IPVA_DingDingRobotAlarm.xml";
            FileInfo fi = new FileInfo(ConfigPath);
            if (!fi.Exists)
            {
                LogHelper.Log("[Error] 配置文件" + @"\config\IPVA_DingDingRobotAlarm.cfg 不存在。");
            }
            else
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(ConfigPath);
                    XmlNode root = xmlDoc.SelectSingleNode("Config");

                    //数据库链接
                    XmlNode DBNode = root.SelectSingleNode("DBConnectionString");
                    if (DBNode != null)
                    {
                        DBConn = DBNode.Attributes["conn"].Value.ToString().Trim();
                    }
                }
                catch (Exception ex)
                {
                    // WLog.WriteLog(Config.LogFileName, "[Error] 配置文件" + @"\config\S0400HeatMapReportService.cfg" + "解析错误:" + ex.ToString());
                    LogHelper.Log("[Error] 配置文件" + @"\config\IPVA_DingDingRobotAlarm.cfg" + "解析错误:" + ex.ToString());
                }
            }
        }
    }
}
