

//弹框设置open
function FrameProperty(win, title, width, height) {
    $(win).show();
    $(win).window({
        title: title, iconCls: 'icon-save', width: width, height: height, modal: true, resizable: false, collapsible: false, minimizable: false, maximizable: false, closable: false
    });
}

//==ajax
function AJAX(url, Callbark) {
    $.ajax({
        url: url,
        type: "POST",
        async: false,
        success: function (data) {
            var msg = eval("(" + data + ")");
            if (data.noLogin == "1") { return; }
            Callbark(msg);
        }
        //error: function (msg) {alert(msg)}
    });
}
//登录名验证
function ValidationAccount(account) {
    if (/[^\x00-\xff]/g.test(account)) return true; //含有汉字
    else return false;
}
//去除字符串空格
function Trim(str) {
    return str.replace(/\s+/g, "");
}
//非空验证
function ValidationN0nNull(elem, mess) {
    if (elem[0].value == "") {
        if (mess != null && mess != "") {
            $.messager.alert("温馨提示", mess + "不能为空！", "'info'");
            // elem[0].focus();
        }
        return false;
    }
    return true;
}
//获取当前年月日
function GetDate() {
    var de = new Date();
    var y = de.getFullYear();
    var m = de.getMonth() + 1;
    var d = de.getDate();
    var startDate = y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
    return startDate;
}
///获取URl参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null)
        return unescape(r[2]);
    return null;
}

function TurnToLogin() {
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

