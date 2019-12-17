<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JobList.aspx.cs" Inherits="TaskManagerWeb.JobList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="Style/bootstrap/css/bootstrap.min.css" rel="stylesheet" media="screen" />
    <link href="Style/bootstrap/css/doc.css" rel="stylesheet" media="screen" />
    <link href="Style/bootstrap/css/patch.css" rel="stylesheet" media="screen" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div class="container-fluid">
        <div class="row">
            <div class="col-md-12  col-md-offset-0">
                <h2>
                    任务查看</h2>
                <nav aria-label="...">
                    <ul class="pager">
                        <%--<li class="previous"><a href="CrystalQuartzPanel.axd"><span aria-hidden="true">&larr;</span>作业管理</a></li>--%>
                        <li class="next"><a href="ModifyJob.aspx">添加任务<span aria-hidden="true">&rarr;</span></a></li>
                    </ul>
                </nav>
                <div class="panel panel-default" id="divJob">
                </div>
            </div>
        </div>
    </div>
    </form>
    <div class="modal fade" id="logModal" data-backdrop="false">
        <div class="modal-dialog" style="width: 1000px;">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" aria-hidden="true">
                        &times;
                    </button>
                    <h4 class="modal-title" id="myModalLabel">
                        任务信息
                    </h4>
                </div>
                <div class="modal-body" style="height: 270px;">
                    <div id="divDetail">
                    </div>
                </div>
                <div class="modal-header">
                    <h4 class="modal-title" id="H1">
                        执行日志
                    </h4>
                </div>
                <div class="modal-body" id="divlogpan" style="text-align: center;">
                    <table class="table" style="text-align: left">
                        <thead>
                            <tr>
                                <th style="width: 30%">
                                    时间
                                </th>
                                <th style="width: 20%">
                                    日志类型
                                </th>
                                <th>
                                    信息
                                </th>
                            </tr>
                        </thead>
                        <tbody id="divModal">
                        </tbody>
                    </table>
                    <ul id="Pager" style="text-align: center; display: -webkit-inline-box;">
                    </ul>
                </div>
            </div>
        </div>
    </div>
    <script id="jobTemplate" type="text/template">
    {@each jobs as job,index}
        <div class="panel-heading">${parseInt(index) + 1} - ${job.group}</div>
        <table class="table">
            <thead>
                <tr>
                    <th>序号</th>
                    <th>任务名称</th>
                    <th style="display:none;">任务组名</th>
                    <th>任务状态</th>
                
                    <th>程序集名称</th>
                    <th>类名称</th>
                    <th>间隔时间</th>
                    <th>重复次数</th>
                    <th>执行次数</th>
                    <th>Cron表达式</th>
                    <th>下次执行时间</th>
                    <th>开始时间</th>
                    <th>结束时间</th>
                    <th>查看</th>
                 </tr>
            </thead>
            <tbody>
                {@each job.tasks as task,index}
                <tr>
                    <th scope="row">${parseInt(index) + 1}</th>
                    <td><a class="taskname" data-toggle="modal" href="ModifyJob.aspx?type=edit&name=${task.Name}&group=${task.Group}">${task.Name}</a></td>
                    <td style="display:none;" class="taskgroup">${task.Group}</td>
                    <td>${task.State}</td>
                
                    <td>${task.AssemblyName}</td>
                    <td>${task.ClassName}</td>
                    <td>{@if task.Type == 0}${task.RepeatInterval}{@else} {@/if}</td>
                    <td>{@if task.Type == 0}${task.RepeatCount}{@else} {@/if}</td>
                    <td>{@if task.FiredCount}${task.FiredCount}{@else} {@/if}</td>
                    <td>{@if task.Type == 1}${task.CronExpression}{@else} {@/if}</td>
                    <td>${task.NextFireTime|timeConverter}</td>
                    <td>${task.BeginTime|timeConverter}</td>
                    <td>${task.EndTime|timeConverter}</td>
                    <td><a class="viewlog" style="cursor:pointer;">日志</a>&nbsp;
                        {@if task.State != "Paused"}
                             <a class="enabletask" style="cursor:pointer;">禁用</a>&nbsp;
                        {@else}
                             <a class="abletask" style="cursor:pointer;">启用</a>&nbsp;
                        {@/if}
                    </td>
                </tr>
                {@/each}
 
            </tbody>
        </table>
    {@/each}
    </script>
    <script id="logTemplate" type="text/template">

                {@each logs as it}
                <tr>
                    <td>${it.CreateTime}</td>
                    <td>{@if it.Type==1}运行{@else if it.Type==2}错误{@/if}</td>
                    <td>${it.Message}</td>
                </tr>
                {@/each}

    </script>
    <script id="detailTemplate" type="text/template">
    
     <div class="col-sm-6">
           <div >
               <label class="col-sm-4 control-label">任务名称</label>
               <div class="col-sm-8">
                    <p class="form-control-static">${Name}</p>
               </div>
            </div>
     </div>
     <div class="col-sm-6">
           <div >
               <label class="col-sm-4 control-label">任务状态</label>
               <div class="col-sm-8">
                    <p class="form-control-static">${State}</p>
               </div>
             </div>
     </div>
     <div class="col-sm-6">
           <div >
               <label class="col-sm-4 control-label">任务组名</label>
               <div class="col-sm-8">
                    <p class="form-control-static">${Group}</p>
               </div>
             </div>
     </div>
     <div class="col-sm-6">
           <div >
               <label class="col-sm-4 control-label">任务描述</label>
               <div class="col-sm-8">
                    <p class="form-control-static">{@if Description}${Description}{@else} {@/if}</p>
               </div>
             </div>
     </div>
     <div class="col-sm-6">
           <div >
               <label class="col-sm-4 control-label">程序集名</label>
               <div class="col-sm-8">
                    <p class="form-control-static">${AssemblyName}</p>
               </div>
             </div>
     </div>
      <div class="col-sm-6">
           <div >
               <label class="col-sm-4 control-label">类库名称</label>
               <div class="col-sm-8">
                    <p class="form-control-static">${ClassName}</p>
               </div>
             </div>
     </div>
          <div class="col-sm-6">
           <div>
               <label class="col-sm-4 control-label">下次执行时间</label>
               <div class="col-sm-8">
                    <p class="form-control-static">${NextFireTime|timeConverter}</p>
               </div>
             </div>
     </div>
     <div class="col-sm-6">
           <div>
               <label class="col-sm-4 control-label">开始时间</label>
               <div class="col-sm-8">
                    <p class="form-control-static">${BeginTime|timeConverter}</p>
               </div>
             </div>
     </div>
      <div class="col-sm-6">
           <div >
               <label class="col-sm-4 control-label">结束时间</label>
               <div class="col-sm-8">
                    <p class="form-control-static">${EndTime|timeConverter}</p>
               </div>
             </div>
     </div>
      <div class="col-sm-6">
           <div>
               <label class="col-sm-4 control-label">任务类型</label>
               <div class="col-sm-8">
                    <p class="form-control-static">{@if Type==0}简单{@else}复杂{@/if}</p>
               </div>
             </div>
     </div>

     {@if Type==1}
         <div class="col-sm-6">
               <div>
                   <label class="col-sm-4 control-label">Cron表达式</label>
                   <div class="col-sm-8">
                        <p class="form-control-static">${CronExpression}</p>
                   </div>
                 </div>
         </div>
     {@/if}

     {@if Type==0}
         <div class="col-sm-6">
               <div>
                   <label class="col-sm-4 control-label">触发次数</label>
                   <div class="col-sm-8">
                        <p class="form-control-static">${FiredCount}</p>
                   </div>
                 </div>
         </div>

         <div class="col-sm-6">
               <div>
                   <label class="col-sm-4 control-label">重复次数</label>
                   <div class="col-sm-8">
                        <p class="form-control-static">${RepeatCount}</p>
                   </div>
                 </div>
         </div>
         <div class="col-sm-6">
               <div>
                   <label class="col-sm-4 control-label">重复间隔</label>
                   <div class="col-sm-8">
                        <p class="form-control-static">${RepeatInterval}</p>
                   </div>
                 </div>
         </div>  
    {@/if}
    </script>
    <script src="Script/jquery-1.11.3.min.js" type="text/javascript"></script>
    <script src="Style/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="Script/Base.js" type="text/javascript"></script>
    <script src="Script/juicer-min.js" type="text/javascript"></script>
    <script src="Script/bootstrap-paginator.min.js" type="text/javascript"></script>
    <script src="Script/JobList.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(function () {
            juicer.register('utcToDate', utcToDate); //注册自定义函数
            InitJob.Init();
        });

    </script>
</body>
</html>
