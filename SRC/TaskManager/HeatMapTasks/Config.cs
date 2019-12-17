using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using TaskManagerCommon;

namespace HeatMapTasks
{
    public class Config
    {
        public string DBConn;
        public string FilePath;
        public Config()
        {

            string ConfigPath = System.Environment.CurrentDirectory + @"\Config\SaveHeatMapFile.xml";
            FileInfo fi = new FileInfo(ConfigPath);
            if (!fi.Exists)
            {
                LogHelper.Log("[Error] 配置文件" + @"\config\S0400HeatMapReportService.cfg 不存在。");
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
                    //文件路径
                    XmlNode FilePathNode = root.SelectSingleNode("FilePath");
                    if (FilePathNode != null)
                    {
                        FilePath = FilePathNode.InnerText.Trim();
                    }

                }
                catch (Exception ex)
                {
                   // WLog.WriteLog(Config.LogFileName, "[Error] 配置文件" + @"\config\S0400HeatMapReportService.cfg" + "解析错误:" + ex.ToString());
                    LogHelper.Log("[Error] 配置文件" + @"\config\S0400HeatMapReportService.cfg" + "解析错误:" + ex.ToString());
                }
            }
        }
    }
}
