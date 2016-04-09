
/**
*考勤管理系统
*/

/**
*弹框
*
*@param   {$}  frame    弹框面板对象
*@param   {string} title    头部面板显示的标题文本
*@param   {int} width   弹框宽
*@param   {int} height  弹框高
*/
var popping = function (frame, title, width, height) {
    var $fr = frame;
    $($fr).show();
    $($fr).window({
        title: title,
        iconCls: 'icon-save',
        width: width,
        height: height,
        modal: true,
        resizable: false,
        collapsible: false,
        minimizable: false,
        maximizable: false,
        closable: false
    });
}
/**
*窗体顶部消息框_Top（1秒后自动销毁）
*
*@param {string}message 显示的消息文本
*/
var messagerTop = function (message) {
    $.messager.show({
        title: "温馨提示",
        msg: message,
        timeout: 1000,
        showType: 'slide',
        style: {
            right: '',
            top: document.body.scrollTop + document.documentElement.scrollTop,
            bottom: ''
        }
    });
}
/**
*窗体右下角消息框（2秒后自动销毁）
*
*@param {string}message 显示的消息文本
*/
var messagerLowerRightCorner = function (message) {
    $.messager.show({
        title: '我的消息',
        msg: message,
        timeout: 2000,
        showType: 'slide'
    });
}
/**
*消息框
*
*@param    {string}   message    显示的消息文本
*@param    {string}   type  消息类型(可选值为'error' |'info'|'question'|'warning' ，默认为'info')
*@param    {function}  fn  确认之后触发该回调函数(可选)
*/
var messagerStyle = function (message, type, fn) {
    var _title = "我的消息"; //type=='info'
    if (type == 'error')
        _title = '错误';
    if (type == 'question')
        _title = "问题";
    if (type == 'warning')
        _title = "警号";
    $.messager.alert(_title, message, type, function () {
        if (fn)
            fn();
    });
}
/**
*确认消息框
*
*@param    {string}   message   显示的消息文本
*@param    {function}   fn  确认触发该回调函数
*/
var confirmStyle = function (message, fn) {
    $.messager.confirm("温馨提示", message, function (_try) {
        if (_try) {
            fn();
        }
    });
}
/**
*输入消息框
*
*@param   {string} title    头部面板显示的标题文本
*@param   {string} message  显示的消息文本，
*@param    {function}   fn  确认触发该回调函数
*/
var promptStyle = function (title, message, fn) {
    $.messager.prompt(title, message, function (_val) {
        if (_val)
            fn(_val);
    });
}
/**
*自定义linkbutton
*
*@param   {$}   node    btn对象
*@param   {string}  iconCls     btn图标
*@param   {int} width   btn宽度
*@param   {int} mgRight     btn右边距
*@param   {Boolean} plain   是否为简洁
*/
var linkbuttonStyle = function (node, iconCls, width, mgRight, plain) {
    var objectLinkbtn_Style = new Object();
    if (iconCls !== "")
        objectLinkbtn_Style.iconCls = 'icon-' + iconCls;
    objectLinkbtn_Style.plain = plain;
    objectLinkbtn_Style.width = width;
    node.linkbutton(objectLinkbtn_Style).css({
        "margin-right": mgRight + "px"
    });
}
/*
*时间微调器_年月日
*@param {$} date 时间控件对象
*/
var datetimespinner_YTD = function (date) {
    $(date).datetimespinner({
        formatter: function (date) {
            if (!date) { return ''; }
            return $.fn.datebox.defaults.formatter.call(this, date);
        },
        parser: function (s) {
            if (!s) { return null; }
            return $.fn.datebox.defaults.parser.call(this, s);
        },
        value: '12/01/2015'
    });
}
/*
*时间微调器_年月
*@param {$} date 时间控件对象
*/
var datetimespinner_Years = function (date) {
    $(date).datetimespinner({
        formatter: function (date) {
            if (!date) { return ''; }
            var y = date.getFullYear();
            var m = date.getMonth() + 1;
            return y + '-' + (m < 10 ? ('0' + m) : m);
        }, parser: function (s) {
            if (!s) { return null; }
            var ss = s.split('-');
            var y = parseInt(ss[0], 10);
            var m = parseInt(ss[1], 10);
            if (!isNaN(y) && !isNaN(m)) {
                return new Date(y, m - 1, 1);
            } else {
                return new Date();
            }
        },
        value: '12/01/2015'
    });
}
/*
获取时间中的年月日或时分秒
*@param    time            
*@param    type    0-年月日  1-时分秒
*/
function getDataTime(time,type) {
    var timeArry = [];
    timeArry = time.split(" ");
    var date;
    if(type ==0)
       // date = timeArry[0].replace(/\//g, "-"); //正则表达式中使用特殊的标点符号,必须在它们之前加上一个 "\" 
        date = timeArry[0].replace(/[^0-9]/g, "-");
    else 
        date = timeArry[1];
    if(date==null||date==undefined||date=="00:00:00")
        date = "--"
    return date;
}
/*
 比较两个时间的大小
 *@param      startTime
 *@param      endtime
 *@param      msg
*/
function checkTime(startTime,endTime,msg) { 
   var startTime=new Date(startTime.replace("-", "/").replace("-", "/")); 
   var endTime=new Date(endTime.replace("-", "/").replace("-", "/"));
   if (endTime < startTime) {
       messagerTop(msg);
       return false;
   } else return true;

}
/**
*判断datagrid是否选中了行
*
*@param   {$}   dataGrid    dataGrid对象
*@param   {function}   fn   选中后触发该回调函数，返回选中行数据
*/
var isDataGridRow = function (dataGrid, fn) {
    var _row = $(dataGrid).datagrid('getSelected'); //选中行
    if (_row != null)
        fn(_row);
    else
        messagerStyle("请选中需要操作的行！", 'warning');
}
/**
*删除datagrid选中行
*
*@param   {$}   dataGrid   dataGrid对象
*@param   {row}     row  选中行行记录
*/
var deleteDataGridRow = function (dataGrid, row) {
    var rowIndex = $(dataGrid).datagrid("getRowIndex", row);
    $(dataGrid).datagrid("deleteRow", rowIndex);
}

/**
*ajaxPOST请求

*@param   {string}   url:发送请求的地址名。
*@param   {string}   func:请求的方法名。
*@param   {Object}   setData：参数。
*@param   {Boolean}   async:异步true,同步false
*@param   {function}   callbark：请求数据正常触发该回调函数。
*@param   {function}   errorCallbark：请求数据异常触发该回调函数。(可选)
*/
var Ajax_Style = function (url, func, setData, async, callback, errorCallbark) {
    $.ajax({
        url: "../Handler/" + url + "?func=" + func + "&ts=" + new Date().getTime(),
        data: setData,
        type: 'POST',
        async: async,
        success: function (data) {//请求成功后的回调函数
            var getData = eval("(" + data + ")");
            if (getData.noLogin == "1") {
                TurnToLogin();
                return; 
            }

            if (getData.Code == "1") {
                if (callback)
                    callback(getData);
            }
            else {
                messagerStyle(getData.Msg, 'error', function () {
                    if (errorCallbark)
                        errorCallbark();
                });
            }
        }
    });
}
/*
*
*@param   {string}   name 参数名
*@returm   {string}  null/参数值
*/
var getQueryString = function (name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    return r == null ? null : unescape(r[2]);
}

/*
*
*获取当前年月日
*/
function GetDate() {
    var de = new Date();
    var y = de.getFullYear();
    var m = de.getMonth() + 1;
    var d = de.getDate();
    var startDate = y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
    return startDate;
}

/**
*检查 Checkbox是否选中
*
*@param   {$}   elem:checkbox 对象。
*@returm   {string}  选中：1，未选中：0。
*/
var CheckCheckbox = function (elem) {
    return $(elem).prop("checked") ? "1" : "0";
}
/**
*检查登录名是否存在汉字
*
*@param   {string}   str    需要验证的字符串
*@return   {Boolean}  存在：true,不存在：false
*/
var CheckAccount = function (str) {
    return /[^\x00-\xff]/g.test(str) ? true : false;
}
/**
*校验密码：以字母开头，长度在6~18之间，只能包含字符、数字和下划线。
*@param   {string}   s   需要验证的字符串
*@return   {Boolean}  错误：true,正确：false
*/
function isPasswd(s) {
    var patrn = /^[a-zA-Z]\w{5,17}$/;
    if (!patrn.exec(s)) {
        messagerTop("密码必须以字母开头，长度在6~18之间，只能包含字符、数字和下划线！");
        return true;
    }
    return false;
}
/**
*去除字符串所有空格
*
*@param   {string}   str    需要去除空格的字符串
*@return   {string}   去除空格后的字符串
*/
var Trim = function (str) {
    return str.replace(/\s+/g, "");
}
/**
*去掉字符串首尾空格
*
*@param   {string}   str    需要去除空格的字符串
*@return  {string}   去除空格后的字符串
*/
var TrimHeadTail = function (str) {
    return str.replace(/^\s*|\s*$/g, '');
}

/**
*文本非空验证
*
*@param   {$}   elem:需要验证的对象
*@param   {string}   mess：文本描述
*@return   {Boolean}   为空true,不为空false
*/
var CheckNull = function (elem, mess) {
    if (elem[0].value == "") {
        if (mess != null && mess != "") {
            messagerTop(mess + "不能为空！");
            try {
                elem[0].focus();
            } catch (ex) { }
        }
        return true;
    }
    return false;
}
/**
*验证字符串长度
*
*@param   {$}   elem:需要验证的对象
*@param   {string}   mess：文本描述
*@param   {number}   len：文本长度最大值
*@return   {Boolean}   为空true,不为空false
*/
function CheckLength(elem, mess, length) {
    if (length == null)
        length = 100;
    if (elem[0].value != "" && elem[0].value.length > length) {
        if (mess != null && mess != "") {
            messagerTop(mess + "文本长度不能大于" + length + "！");
            try {
                elem[0].focus();
            } catch (ex) { }
        }
        return true;
    }
    return false;
}
/**
*验证字符串长度
*
*@param   {string}   elem:需要验证的字符串
*@param   {string}   mess：文本描述
*@param   {number}   len：文本长度最大值
*@return   {Boolean}   为空true,不为空false
*/
function CheckStringLength(elem, mess, length) {
    if (length == null)
        length = 100;
    if (elem != "" && elem.length > length) {
        if (mess != null && mess != "") {
            messagerTop(mess + "文本长度不能大于" + length + "！");
        }
        return true;
    }
    return false;
}
/**
*页面权限验证
*
*@param   {string} el:需要验证的数据
*/
var PageAuthentication = function (el) {
    if (el == "1")
        return false;
    else {
        messagerStyle("没有相应权限！", "info", function () {
            // history.go(-1);
            location = "Home.aspx";
        });
        return true;
    }
}
/**
*登录权限验证
*
*@param   {string}  el:需要验证的数据
*@return   {Boolean}   未登录true,登录false
*/
var IsLogin = function (el) {
    if (el == 1) {
        TurnToLogin();       
        return true;
    }
    return false;
}

/**
*返回登录界面
*
*/
var TurnToLogin = function () {
    var win = window;
    while (win.parent != win)
        win = win.parent;
    var dialogWin = null;
    do {
        if (win.dialogArguments) {
            dialogWin = win;
            win = win.dialogArguments.window;
        }
        while (win.parent != win)
            win = win.parent;
    } while (win.dialogArguments);
    if (win.location.href.toLowerCase().indexOf("home.aspx") > -1)
        win.location.href = "Login.aspx";
    else
        win.location.href = win.location.href;
    try {
        if (dialogWin != null)
            dialogWin.close();
    } catch (ex) { }
}
