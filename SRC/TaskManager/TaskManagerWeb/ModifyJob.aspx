<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModifyJob.aspx.cs" Inherits="TaskManagerWeb.ModifyJob" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link href="Style/bootstrap/css/bootstrap.min.css" rel="stylesheet" media="screen">
    <link href="Style/bootstrap/css/doc.css" rel="stylesheet" media="screen">
    <link href="Style/bootstrap/css/patch.css" rel="stylesheet" media="screen">
    <style type="text/css">
        .algin_r
        {
            text-align: right;
        }
        .error
        {
            color: red;
        }
    </style>
    <script src="Script/jquery-1.11.3.min.js" type="text/javascript"></script>
    <script src="Style/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="Script/jquery.validate.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="Script/My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript">
        $(function () {
            if ($("#radTypeCron").attr("checked") == "checked") {
                $("#divSimplePanel").hide();
                $("#divCronPanel").show();
            }
            else {
                $("#divSimplePanel").show();
                $("#divCronPanel").hide();
            }


            $("#radTypeSimple").on("click", function () {
                $("#divSimplePanel").show();
                $("#divCronPanel").hide();
            });
            $("#radTypeCron").on("click", function () {
                $("#divSimplePanel").hide();
                $("#divCronPanel").show();
            });
            $('#tskForm').validate({
                rules: {
                    txtTaskName: "required",
                    txtTaskGroup: "required",
                    txtAssemblyName: "required",
                    txtClassName: "required",
                    txtBeginTime: "required",
                    txtRepeatHours: { required: "#radTypeSimple:checked", number: true },
                    txtRepeatMinutes: { required: "#radTypeSimple:checked", number: true },
                    txtRepeatSeconds: { required: "#radTypeSimple:checked", number: true },
                    txtRepeatCount: { required: "#radTypeSimple:checked", number: true },
                    txtCronExp: { required: "#radTypeCron:checked" }

                },
                messages: {
                    txtTaskName: "请输入任务名称",
                    txtTaskGroup: "请输入任务组名",
                    txtAssemblyName: "请输入程序集名称",
                    txtClassName: "请输入类名称",
                    txtBeginTime: {
                        required: "请输入开始时间"
                    },
                    txtRepeatHours: {
                        required: "请输入小时数",
                        number: "请输入数字"
                    },
                    txtRepeatMinutes: {
                        required: "请输入分钟数",
                        number: "请输入数字"
                    },
                    txtRepeatSeconds: {
                        required: "请输入秒数",
                        number: "请输入数字"
                    },
                    txtRepeatCount: {
                        required: "请输入重复次数",
                        number: "请输入数字"
                    },
                    txtCronExp: "请输入Cron表达式"
                }
            });

            $('#myModal .close').on("click", function () {
                if (RedirectUrl && RedirectUrl != "") {
                    location.href = RedirectUrl;
                }
            });

        });
        var RedirectUrl = "";


        function panelAddResultShow(type, msg, url) {
            if (type == "error") {
                $('#myModal').find(".alert").addClass("alert-danger").html(msg);
            }
            else {
                $('#myModal').find(".alert").addClass("alert-success").html(msg);
                RedirectUrl = url;
            }
            $('#myModal').modal();
        }




    </script>
</head>
<body>
    <div class="container-fluid">
        <div class="row">
            <div class="col-md-6  col-md-offset-3">
                <h2 runat="server" id="pagetitle">
                    新增任务</h2>
                <div class="alert alert-success alert-dismissible" role="alert" id="panelAddResult"
                    style="display: none;">
                    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                        <span aria-hidden="true">&times;</span></button>
                    <strong></strong>
                </div>
                <nav aria-label="...">
                    <ul class="pager">
                        <li class="previous"><a href="JobList.aspx"><span aria-hidden="true">&larr;</span>查看任务</a></li>
                        <%--<li class="next"><a href="CrystalQuartzPanel.axd">作业管理<span aria-hidden="true">&rarr;</span></a></li>--%>
                    </ul>
                </nav>
                <div class="bs-example" data-example-id="horizontal-static-form-control">
                    <form id="tskForm" runat="server" class="form-horizontal" method="post">
                    <div class="form-group">
                        <div class="col-sm-2 algin_r">
                            <span style="color: Red">*</span>
                            <label for="txtTaskName">
                                任务名称</label>
                        </div>
                        <div class="col-sm-10">
                            <input type="text" class="form-control" name="txtTaskName" id="txtTaskName" runat="server" />
                            <span id="Span7" class="help-block">任务名称，同一个group中多个job的name不能相同</span>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-2 algin_r">
                            <label for="txtTaskDecs">
                                任务描述</label>
                        </div>
                        <div class="col-sm-10">
                            <input type="text" class="form-control" id="txtTaskDecs" runat="server" />
                            <span id="Span9" class="help-block">任务描述说明</span>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-2 algin_r">
                            <span style="color: Red">*</span>
                            <label for="txtTaskGroup">
                                任务组名</label>
                        </div>
                        <div class="col-sm-10">
                            <input type="text" class="form-control" id="txtTaskGroup" runat="server" />
                            <span id="Span6" class="help-block">任务所属分组，用于标识任务所属分组，若未设置group则所有未设置group的job为同一个分组（必须设置）</span>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-2 algin_r">
                            <span style="color: Red">*</span>
                            <label for="txtAssemblyName">
                                程序集名称</label>
                        </div>
                        <div class="col-sm-10">
                            <input type="text" class="form-control" id="txtAssemblyName" runat="server" />
                            <span id="Span5" class="help-block">实现了IJob接口的程序集名称</span>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-2 algin_r">
                            <span style="color: Red">*</span>
                            <label for="txtAssemblyName">
                                类名称</label>
                        </div>
                        <div class="col-sm-10">
                            <input type="text" class="form-control" id="txtClassName" runat="server" />
                            <span id="Span4" class="help-block">实现了IJob接口的包含完整命名空间的类名</span>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-2 algin_r">
                            <span style="color: Red">*</span>
                            <label for="txtBeginTime">
                                开始时间</label>
                        </div>
                        <div class="col-sm-10">
                            <input type="text" class="form-control" id="txtBeginTime" runat="server" readonly="readonly"
                                onclick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss',realDateFmt:'yyyy-MM-dd HH:mm:ss',realTimeFmt:'HH:mm:ss HH:mm:ss',maxDate:'#F{$dp.$D(\'txtBeginTime\')}'})" />
                            <span id="Span8" class="help-block">任务开始执行时间,小于当前时间立即执行</span>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-2 algin_r">
                            <label for="txtBeginTime">
                                结束时间</label>
                        </div>
                        <div class="col-sm-10">
                            <input type="text" class="form-control" id="txtEndTime" runat="server" readonly="readonly"
                                onclick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss',realDateFmt:'yyyy-MM-dd HH:mm:ss',realTimeFmt:'HH:mm:ss HH:mm:ss',minDate:'#F{$dp.$D(\'txtBeginTime\')}'})" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-2 algin_r">
                            <span style="color: Red">*</span>
                            <label for="txtAssemblyName">
                                任务类型</label>
                        </div>
                        <div class="col-sm-10">
                            <label class="radio-inline">
                                <input type="radio" name="inlineRadioOptions" id="radTypeSimple" value="0" checked="true"
                                    runat="server" />
                                简单
                            </label>
                            <label class="radio-inline">
                                <input type="radio" name="inlineRadioOptions" id="radTypeCron" value="1" runat="server" />
                                复杂
                            </label>
                            <span id="Span3" class="help-block">1)简单任务的触发器，可以调度用于重复执行的任务 2)复杂任务触发器使用cron表达式定制任务调度</span>
                        </div>
                    </div>
                    <div id="divSimplePanel" runat="server">
                        <div class="form-group">
                            <div class="col-sm-2 algin_r">
                                <span style="color: Red">*</span>
                                <label for="txtBeginTime">
                                    间隔时间</label>
                            </div>
                            <div class="col-sm-2">
                                <input type="text" class="form-control col-sm-3" id="txtRepeatHours" runat="server"
                                    value="0" />
                            </div>
                            <div class="col-sm-1" style="line-height:30px;">
                                小时</div>
                            <div class="col-sm-2">
                                <input type="text" class="form-control col-sm-3" id="txtRepeatMinutes" runat="server"
                                    value="0" />
                            </div>
                            <div class="col-sm-1" style="line-height:30px;">
                                分钟</div>
                            <div class="col-sm-2">
                                <input type="text" class="form-control col-sm-3" id="txtRepeatSeconds" runat="server"
                                    value="5" />
                            </div>
                            <div class="col-sm-1" style="line-height:30px;">
                                秒</div>
                            <div class="col-sm-10 col-sm-offset-2">
                                <span id="Span2" class="help-block">任务触发间隔</span>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-sm-2 algin_r">
                                <span style="color: Red">*</span>
                                <label for="txtBeginTime">
                                    重复次数</label>
                            </div>
                            <div class="col-sm-10">
                                <input type="text" class="form-control" id="txtRepeatCount" runat="server" />
                                <span id="Span1" class="help-block">任务执行次数，如: -1 表示无限次执行</span>
                            </div>
                        </div>
                    </div>
                    <div id="divCronPanel" style="display: none;" runat="server">
                        <div class="form-group">
                            <div class="col-sm-2 algin_r">
                                <span style="color: Red">*</span>
                                <label for="txtBeginTime">
                                    Cron表达式</label>
                            </div>
                            <div class="col-sm-10">
                                <input type="text" class="form-control" id="txtCronExp" runat="server" data-toggle="tooltip"
                                    title="一个Cron-表达式是一个由六至七个字段组成由空格分隔的字符串，其中6个字段是必须的而一个是可选的
                                    常用示例:
                                    0 0 12 * * ? 	每天12点触发 
                                    0 15 10 ? * * 	每天10点15分触发 
                                    0 15 10 * * ? 	每天10点15分触发 
                                    0 15 10 * * ? * 	每天10点15分触发 
                                    0 15 10 * * ? 2005 	2005年每天10点15分触发 
                                    0 * 14 * * ? 	每天下午的 2点到2点59分每分触发 
                                    0 0/5 14 * * ? 	每天下午的 2点到2点59分(整点开始，每隔5分触发) 
                                    0 0/5 14,18 * * ? 	每天下午的 2点到2点59分(整点开始，每隔5分触发) 
                                    每天下午的 18点到18点59分(整点开始，每隔5分触发) 
                                    0 0-5 14 * * ? 	每天下午的 2点到2点05分每分触发 
                                    0 10,44 14 ? 3 WED 	3月分每周三下午的 2点10分和2点44分触发 
                                    0 15 10 ? * MON-FRI 	从周一到周五每天上午的10点15分触发 
                                    0 15 10 15 * ? 	每月15号上午10点15分触发 
                                    0 15 10 L * ? 	每月最后一天的10点15分触发 
                                    0 15 10 ? * 6L 	每月最后一周的星期五的10点15分触发 
                                    0 15 10 ? * 6L 2002-2005 	从2002年到2005年每月最后一周的星期五的10点15分触发 
                                    0 15 10 ? * 6#3 	每月的第三周的星期五开始触发 
                                    0 0 12 1/5 * ? 	每月的第一个中午开始每隔5天触发一次 
                                    0 11 11 11 11 ? 	每年的11月11号 11点11分触发(光棍节)" />
                                <span id="helpBlock2" class="help-block">Cron表达式，如：0/10 * * * * ?</span>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-12" style="text-align: center;">
                            <input id="btnSubmit" type="submit" class="btn btn-default" runat="server" onserverclick="btnSubmit_Click"
                                value="提 交">
                        </div>
                    </div>
                    <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                        aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                                        &times;
                                    </button>
                                    <h4 class="modal-title" id="myModalLabel">
                                        信息
                                    </h4>
                                </div>
                                <div class="modal-body">
                                    <div class="alert " role="alert">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
