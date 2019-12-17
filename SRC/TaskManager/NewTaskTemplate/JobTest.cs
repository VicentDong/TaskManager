using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using TaskManagerUtility;
using TaskManagerCommon;
using TaskManagerTaskSet;

namespace NewTaskTemplate
{
    public class JobTest : JobBase
    {
        /// <summary>
        /// 接口需要执行的方法
        /// </summary>
        public override void ExecuteMethod()
        {
            Test();
        }

        //自定义方法
        public void Test()
        {

            //LogHelper.Log("asdfasdfasdfa");
            LogHelper.WriteLog(this.GetType().Name, "test job asdfasdfasdfa");
        }
    }
}
