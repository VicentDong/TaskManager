var AJAX = AJAX || (function (w, jQuery) {
    //request url
    var URL = '/Handler/TaskManager.ashx';
    function ajax(data, onSucc, url) {

        jQuery.ajax({
            'type': data.type !== undefined ? data.type : "GET",
            'data': data,
            'url': (url ? url : URL) + ((url && (url.indexOf("?") !== -1)) ? "&" : "?") + "temistamp=" + Common.GenerateTemistamp(),
            'async': data.asy !== undefined ? data.asy : true,
            'dataType': data.dataType ? data.dataType : "json",
            'success': function (msg) {
                onSucc(msg);
            },
            'error': function (error, errorText) {
                var status = error.status,
					message;
                switch (status) {
                    case 500:
                    case 502:
                        break;
                    default:
                        break;

                }
            },
            'complete': function (XMLHttpRequest, textStatus) {
                ajaxRequest = "";
            }
        });
    }
    return {
        GetData: function (data, succ) {
            ajax(data, function (msg) {
                succ(msg);
            });
        }
    };
})(window, jQuery);



//扩展日期方法，增加格式化日期的方法
Date.prototype.Format = function (fmt) { //author: meizz
    var o = {
        "M+": this.getMonth() + 1,                 //月份
        "d+": this.getDate(),                    //日
        "h+": this.getHours(),                   //小时
        "m+": this.getMinutes(),                 //分
        "s+": this.getSeconds(),                 //秒
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度
        "S": this.getMilliseconds()             //毫秒
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

var utcToDate = function(utcCurrTime) {
    utcCurrTime = utcCurrTime + "";
    var date = "";
    var month = new Array();
    month["Jan"] = 1;
    month["Feb"] = 2;
    month["Mar"] = 3;
    month["Apr"] = 4;
    month["May"] = 5;
    month["Jun"] = 6;
    month["Jul"] = 7;
    month["Aug"] = 8;
    month["Sep"] = 9;
    month["Oct"] = 10;
    month["Nov"] = 11;
    month["Dec"] = 12;
    var week = new Array();
    week["Mon"] = "一";
    week["Tue"] = "二";
    week["Wed"] = "三";
    week["Thu"] = "四";
    week["Fri"] = "五";
    week["Sat"] = "六";
    week["Sun"] = "日";

    str = utcCurrTime.split(" ");
    date = str[5] + "-";
    date = date + month[str[1]] + "-" + str[2] + "-" + str[3];
    return date;
}

function FormatDate(t) {
    var n = t.toString();
    var tt = eval(n.replace(/\/Date(\d+)\+\d+\//gi, "new Date($1)"));
    var d = new Date(tt);
    var time = d.format("yyyy-MM-dd");
    return time;
}  

var Common = {
    FormatNum:
    function (num) {
        num += '';
        var x = num.split('.');
        var x1 = x[0];
        var x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1 + x2;
    },
    GenerateTemistamp:
    function () {
        var Timistamp = new Date();
        var CurMin = Timistamp.getMinutes();
        if (CurMin % 5 != 0) {
            Timistamp.setMinutes(Math.floor(CurMin / 5) * 5);
        }
        return Timistamp.Format("yyyyMMddhhmm");
    },
    //获取url参数值
    GetRequest: function () {
        var url = location.search; //获取url中"?"符后的字串 
        var theRequest = new Object();
        //拆分url
        if (url.indexOf("?") != -1) {
            var str = url.substr(1);
            //按“&”拆分参数
            strs = str.split("&");
            //按“=”拆分参数
            for (var i = 0; i < strs.length; i++) {
                theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
            }
        }
        return theRequest;
    }
}

var ObjUrl = function (url) {
    this.url = url ? url : window.location.href;
    this.path = ""; //路径
    this.params = {}; //传递参数对象
    this.anchor = ""; //锚点
    this.GetAttrs();
}
ObjUrl.prototype.GetAttrs = function () {
    var str = this.url;
    var index = str.indexOf("#");
    if (index > 0) {
        this.anchor = str.substr(index);
        str = str.substr(0, index);
    }
    index = str.indexOf("?");
    if (index > 0) {
        this.path = str.substr(0, index);
        str = str.substr(index + 1);
        var paramArr = str.split("&&");
        for (var i = 0; i < paramArr.length; i++) {
            var paraArr = paramArr[i].split("=")
            this.params[paraArr[0]] = paraArr[1];
        }
    }
    else {
        this.path = this.url;
        this.params = {};
    }
}
ObjUrl.prototype.SetParam = function (key, val) {
    if (val) {
        this.Params[key] = val;
    }
    else this.Params[key] = undefined;
}
ObjUrl.prototype.GetParamVal = function (key) {
    return this.params[key];
}