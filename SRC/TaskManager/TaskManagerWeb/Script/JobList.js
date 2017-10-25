
var InitJob = InitJob || (function (win, jQuery) {
    var jobTemplate = $("#jobTemplate").html(), divJob = $("#divJob"),
        logModal = $("#logModal"), logModal = $("#logModal"), divModal = $("#divModal"),
        logTemplate = $("#logTemplate").html(), Pager = $("#Pager"),
        detailTemplate = $("#detailTemplate").html(), divDetail = $("#divDetail"),
        taskname, taskgroup, pageSize = 10, pageIndex = 1, total = 0, pageCount = 1; divlogpan = $("#divlogpan");
    return {
        Init: function () {

            AJAX.GetData({ "MethodID": "GetTasksByGroup" }, function (data) {
                var data = { "jobs": data };


                var timeConverter = function (dateoffset) {
                    if (dateoffset != null) {
                        return (new Date(dateoffset)).Format("yyyy-MM-dd hh:mm:ss");
                    }
                    return "";
                }
                juicer.register('timeConverter', timeConverter);
                divJob.html(juicer(jobTemplate, data));

                InitJob.InitAction();
            });

        },
        InitLogModel: function () {
            AJAX.GetData({ "MethodID": "GetTaskLog", "taskName": taskname, "groupName": taskgroup, "pageSize": pageSize, "pageIndex": pageIndex }, function (data) {
                if (data) {
                    divlogpan.hide();
                    Pager.hide();
                    if (data) {
                        divModal.html(juicer(logTemplate, data));
                        divlogpan.show();
                        total = data.total;
                        pageCount = data.count;
                        if (pageCount > 2) {
                            var options = {
                                bootstrapMajorVersion: 3,
                                currentPage: pageIndex,
                                totalPages: pageCount,
                                numberOfPages: pageSize,
                                onPageClicked: function (event, originalEvent, type, page) {
                                    pageIndex = page;   
                                    InitJob.InitLogModel();
                                }
                            }
                            Pager.bootstrapPaginator(options);
                            Pager.show();
                        }

                    }
                    logModal.modal();
                }
                else {
                    pageIndex = 1;
                }
            });
        },
        InitTaskDetail: function () {
            AJAX.GetData({ "MethodID": "GetTask", "taskName": taskname, "groupName": taskgroup }, function (data) {
                if (data) {
                    divDetail.html(juicer(detailTemplate, data));
                }
            });
        },
        InitAction: function () {

            //查看日志
            $(".viewlog").on("click", function (e) {

                var target = $(e.currentTarget);
                taskname = target.parent().parent().find(".taskname").html();
                taskgroup = target.parent().parent().find(".taskgroup").html();

                InitJob.InitTaskDetail();
                InitJob.InitLogModel();
            });

            //禁用job
            $(".enabletask").on("click", function (e) {

                var target = $(e.currentTarget);
                taskname = target.parent().parent().find(".taskname").html();
                taskgroup = target.parent().parent().find(".taskgroup").html();

                AJAX.GetData({ "MethodID": "PauseJob", "taskName": taskname, "groupName": taskgroup }, function (data) {
                    InitJob.Init();
                });

            });
            $(".abletask").on("click", function (e) {

                var target = $(e.currentTarget);
                taskname = target.parent().parent().find(".taskname").html();
                taskgroup = target.parent().parent().find(".taskgroup").html();

                AJAX.GetData({ "MethodID": "ResumeJob", "taskName": taskname, "groupName": taskgroup }, function (data) {
                    InitJob.Init();
                });
            });

            logModal.find(".close").on("click", function () {

                logModal.modal('hide');
                location.herf = 'JobList.aspx';
            });
        }
    }


})(window, jQuery);