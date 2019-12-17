using System;
using TaskManagerCommon;

namespace TaskManagerServer
{
    /// <summary>
    /// Factory class to create Quartz server implementations from.
    /// </summary>
    public class QuartzServerFactory
    {
        /// <summary>
        /// Creates a new instance of an Quartz.NET server core.
        /// </summary>
        /// <returns></returns>
        public static QuartzServer CreateServer()
        {
            string typeName = Configuration.ServerImplementationType;

            Type t = Type.GetType(typeName, true);

            LogHelper.ServerInfo("创建新的服务实例 '" + typeName + "'");

            QuartzServer retValue = (QuartzServer) Activator.CreateInstance(t);

            LogHelper.ServerInfo("创建新的服务实例成功");
            return retValue;
        }
    }
}