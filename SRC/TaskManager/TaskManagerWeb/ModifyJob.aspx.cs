using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TaskManagerUtility;
using TaskManagerCommon;

namespace TaskManagerWeb
{
    public partial class ModifyJob : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string type = string.Empty;
                string name = string.Empty;
                string group = string.Empty;
                if (Request.QueryString["type"] != null)
                {
                    type = Request.QueryString["type"].ToString();
                }
                if (Request.QueryString["name"] != null)
                {
                    name = Request.QueryString["name"].ToString();
                }
                if (Request.QueryString["group"] != null)
                {
                    group = Request.QueryString["group"].ToString();
                }
                switch (type.ToUpper())
                {
                    case "ADD":
                        pagetitle.InnerHtml = "新增任务";
                        break;
                    case "EDIT":
                        InitJobInfo(name, group);
                        pagetitle.InnerHtml = "编辑任务";
                        break;
                }
            }
        }

        /// <summary>
        /// 初始化任务信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        public void InitJobInfo(string name, string group)
        {
            try
            {
                TaskEntity task = QuartzHelper.GetTask(name, group);

                this.txtTaskName.Value = task.Name;
                this.txtTaskGroup.Value = task.Group;
                this.txtAssemblyName.Value = task.AssemblyName;
                this.txtClassName.Value = task.ClassName;
                this.txtTaskDecs.Value = task.Description;
                this.txtBeginTime.Value = task.BeginTime.ToString("yyyy-MM-dd HH:mm:ss");
                this.txtEndTime.Value = task.EndTime == null ? "" : task.EndTime.ToString();
                if (task.Type == 0)
                {
                    this.radTypeSimple.Checked = true;
                    this.txtRepeatCount.Value = task.RepeatCount.ToString();
                    this.txtRepeatHours.Value = task.RepeatInterval.Hours.ToString();
                    this.txtRepeatMinutes.Value = task.RepeatInterval.Minutes.ToString();
                    this.txtRepeatSeconds.Value = task.RepeatInterval.Seconds.ToString();
                    divCronPanel.Style.Add("display", "none");
                    divSimplePanel.Style.Add("display", ""); 
                }
                else
                {
                    this.radTypeCron.Checked = true;
                    this.txtCronExp.Value = task.CronExpression.ToString();
                    divCronPanel.Style.Add("display", "");
                    divSimplePanel.Style.Add("display", "none"); 
                }

            }
            catch
            {
                OpenAlertModel("error", "获取任务信息错误");
            }
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!ValidTaskInput())
                //{
                  
                //    return;
                //}


                if (string.IsNullOrEmpty(this.txtTaskName.Value))
                {
                    OpenAlertModel("error", "任务名称不能为空");
                    return ;
                }
                if (string.IsNullOrEmpty(this.txtTaskGroup.Value))
                {
                    OpenAlertModel("error", "任务组名不能为空");
                    return ;
                }
                if (string.IsNullOrEmpty(this.txtAssemblyName.Value))
                {
                    OpenAlertModel("error", "程序集名称不能为空");
                    return ;
                }
                if (string.IsNullOrEmpty(this.txtClassName.Value))
                {
                    OpenAlertModel("error", "类名称不能为空");
                    return ;
                }
                DateTimeOffset begintime;
                DateTimeOffset endtime;
                if (string.IsNullOrEmpty(this.txtBeginTime.Value) && DateTimeOffset.TryParse(this.txtBeginTime.Value, out begintime))
                {
                    OpenAlertModel("error", "开始时间错误");
                    return ;
                }
                if (!string.IsNullOrEmpty(this.txtEndTime.Value) && DateTimeOffset.TryParse(this.txtEndTime.Value, out endtime))
                {
                    OpenAlertModel("error", "结束时间错误");
                    return ;
                }
                int RepeatHour = 0;
                int RepeatMins = 0;
                int RepeatSec = 0;
                int RepeatCount = 0;

                if (this.radTypeSimple.Checked)
                {
                    if (!string.IsNullOrEmpty(this.txtRepeatHours.Value) && !int.TryParse(this.txtRepeatHours.Value, out RepeatHour))
                    {
                        OpenAlertModel("error", "间隔时间错误");
                        return ;
                    }
                    if (!string.IsNullOrEmpty(this.txtRepeatMinutes.Value) && !int.TryParse(this.txtRepeatMinutes.Value, out RepeatMins))
                    {
                        OpenAlertModel("error", "间隔时间错误");
                        return ;
                    }
                    if (!string.IsNullOrEmpty(this.txtRepeatSeconds.Value) && !int.TryParse(this.txtRepeatSeconds.Value, out RepeatSec))
                    {
                        OpenAlertModel("error", "间隔时间错误");
                        return ;
                    }
                    if (string.IsNullOrEmpty(this.txtRepeatCount.Value) && !int.TryParse(this.txtRepeatCount.Value, out RepeatCount))
                    {
                        OpenAlertModel("error", "重复次数错误");
                        return ;
                    }
                }
                else
                {
                    if (!QuartzHelper.IsCronExpression(this.txtCronExp.Value.Trim()))
                    {
                        OpenAlertModel("error", "Cron表达式错误");
                        return ;
                    }
                }




                TaskEntity task = new TaskEntity();
                task.Name = this.txtTaskName.Value;
                task.Group = this.txtTaskGroup.Value;
                task.AssemblyName = this.txtAssemblyName.Value;
                task.ClassName = this.txtClassName.Value;
                task.Description = this.txtTaskDecs.Value;
                task.BeginTime = DateTimeOffset.Parse(this.txtBeginTime.Value);
                if (string.IsNullOrEmpty(this.txtEndTime.Value))
                {
                    task.EndTime = null;
                }
                else
                {
                    task.EndTime = DateTimeOffset.Parse(this.txtEndTime.Value);
                }
                task.Type = int.Parse(radTypeSimple.Checked ? radTypeSimple.Value : radTypeCron.Value);
                if (task.Type == 0)
                {

                    task.RepeatInterval = new TimeSpan(int.Parse(this.txtRepeatHours.Value), int.Parse(this.txtRepeatMinutes.Value), int.Parse(this.txtRepeatSeconds.Value));
                    task.RepeatCount = int.Parse(this.txtRepeatCount.Value);
                }
                else
                {

                    task.CronExpression = this.txtCronExp.Value;
                }

                QuartzHelper.AddNewJob(task);
                OpenAlertModel("success", "添加成功", "JobList.aspx");
            }
            catch (Exception ex)
            {
                OpenAlertModel("error", "添加错误");
                //throw ex;
            }
        }

        /// <summary>
        /// 验证输入
        /// </summary>
        /// <returns></returns>
        public bool ValidTaskInput()
        {
            if (string.IsNullOrEmpty(this.txtTaskName.Value))
            {
                return false;
            }
            if (string.IsNullOrEmpty(this.txtTaskGroup.Value))
            {
                return false;
            }
            if (string.IsNullOrEmpty(this.txtAssemblyName.Value))
            {
                return false;
            }
            if (string.IsNullOrEmpty(this.txtClassName.Value))
            {
                return false;
            }
            DateTimeOffset begintime;
            DateTimeOffset endtime;
            if (string.IsNullOrEmpty(this.txtBeginTime.Value) && DateTimeOffset.TryParse(this.txtBeginTime.Value, out begintime))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(this.txtEndTime.Value) && DateTimeOffset.TryParse(this.txtEndTime.Value, out endtime))
            {
                return false;
            }
            int RepeatHour = 0;
            int RepeatMins = 0;
            int RepeatSec = 0;
            int RepeatCount = 0;

            if (this.radTypeSimple.Checked)
            {
                if (!string.IsNullOrEmpty(this.txtRepeatHours.Value) && !int.TryParse(this.txtRepeatHours.Value, out RepeatHour))
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(this.txtRepeatMinutes.Value) && !int.TryParse(this.txtRepeatMinutes.Value, out RepeatMins))
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(this.txtRepeatSeconds.Value) && !int.TryParse(this.txtRepeatSeconds.Value, out RepeatSec))
                {
                    return false;
                }
                if (string.IsNullOrEmpty(this.txtRepeatCount.Value) && !int.TryParse(this.txtRepeatCount.Value, out RepeatCount))
                {
                    return false;
                }
            }
            else
            {
                if (!QuartzHelper.IsCronExpression(this.txtCronExp.Value.Trim()))
                {
                    return false;
                }

            }
            return true;
        }


        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public void OpenAlertModel(string type, string msg)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "", "panelAddResultShow('" + type + "','" + msg + "');", true);
        }


        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public void OpenAlertModel(string type, string msg, string url)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "", "panelAddResultShow('" + type + "','" + msg + "','" + url + "');", true);
        }
    }
}