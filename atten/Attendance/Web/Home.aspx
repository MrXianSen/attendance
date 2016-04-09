<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Attendance.Home" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>考勤管理系统</title>
    <script src="../App_Themes/Scripts/jquery-2.1.1.min.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/public.js" type="text/javascript"></script>
    <link href="../App_Themes/Css/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../App_Themes/Css/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../App_Themes/Css/Site.css" rel="stylesheet" type="text/css" />
</head>
<body class="easyui-layout" oncontextmenu="return false">
    <div data-options="region:'north',border:false" style="height: 60px; background: #2E70CC;
        padding: 10px; color: #ffffff">
        <div class="link" style="float: right; font-size: 14px; padding-right: 100px; padding-top: 10px;
            background: #2E70CC;">
            <a href="#" class="easyui-menubutton" data-options="menu:'.mm1',iconCls:'icon-man'">
                账号管理</a>
        </div>
        <div class="mm1" style="width: 150px;">
            <div data-options="iconCls:'icon-edit'" class="ExchangePassword">
                <a href="#" style="text-decoration: none">修改密码</a></div>
            <div class="menu-sep">
            </div>
            <div data-options="iconCls:'icon-back'" class="ExchangeClose">
                <a href="#" style="text-decoration: none">退出系统</a></div>
        </div>
    </div>
    <div data-options="region:'west',split:true,title:'菜单导航',minWidth:180" style="width: 180px;
        text-align: center">
        <div class="easyui-accordion" data-options="fit:true,border:false">
            <div title="打卡考勤" style="padding: 10px;">
                <ul class='ul-menu'>
                    <li><a url="Attendance.aspx">打卡考勤</a></li>
                </ul>
            </div>
            <div title="我的签到记录" style="padding: 10px;">
                <ul class='ul-menu'>
                    <li><a url="Detailed.aspx">我的签到记录详细</a></li>
                    <li><a url="MonthlyStatistics.aspx">我的签到记录月统计</a></li>
                </ul>
            </div>
            <%if (isDepManage == "1" || isTopManage == "1")
              { %>
            <div title="员工签到记录" style="padding: 10px;">
                <ul class='ul-menu'>
                    <li><a url="Detailed.aspx?type=1">员工签到记录详细</a></li>
                    <li><a url="MonthlyStatistics.aspx?type=1">员工签到记录月统计</a></li>
                </ul>
            </div>
            <%}
              if (isAdmin == "1")
              { %>
            <div title="系统设置" style="padding: 10px;">
                <ul class='ul-menu'>
                    <li><a url="DepartmentManage.aspx">部门管理</a></li>
                    <li><a url="UserManage.aspx">员工管理</a></li>
                    <li><a url="TimeManagement.aspx">时间管理</a></li>
                    <li><a url="FillClock.aspx">补打卡</a></li>
                </ul>
            </div>
            <%} %>
        </div>
    </div>
    <div data-options="region:'center'" style="overflow: hidden;">
        <div class="ttTab" class="easyui-tabs" data-options="tools:'#tab-tools',border:false,fit:true"
            style="overflow: hidden;">
        </div>
    </div>
    <div data-options="region:'south',border:false" style="height: 40px; padding: 10px;
        background: #2E70CC; text-align: center; color: #ffffff">
        <samp>
            当前用户：<%=GetempName%></samp>&nbsp;&nbsp;&nbsp;
        <label class="LblTime">
        </label>
    </div>
    <!--个人密码修改弹框-->
    <div class="win" style="display: none">
    </div>
    <script type="text/javascript">

        $(function () {
            if (IsLogin("<%=strNoLogin %>"))
                return; //验证是否登录
            initData();
            //BindMenuClickHrefEvent();			
            $(".ul-menu li a[url='Attendance.aspx']").click();
        });
        function initData() {
            LayoutTab();
            ReadDateTimeShow();
            Time = setInterval(ReadDateTimeShow, 1000);
            RegisterFunc();
        }
        //布局table
        function LayoutTab() {
            $(".ttTab").tabs({ tabWidth: 200 });
            $(".ul-menu li a").css("display", "block").css("height", "auto").css("width", "auto").css("cursor", "pointer");
            $(".ul-menu li").hover(
                function () {
                    b_c = $(this).css("background-color");
                    $(this).css("background-color", "#34AFFF");
                    c = $(this).css("color");
                    $(this).css("color", "#ffffff");
                },
                function () {
                    $(this).css("background-color", b_c);
                    $(this).css("color", c);
                }
            );
            BindMenuClickHrefEvent();
        }
        //读取动态时间的变化
        function ReadDateTimeShow() {
            var year = new Date().getFullYear();
            var Month = new Date().getMonth() + 1;
            var Day = new Date().getDate();
            var Time = new Date().toLocaleTimeString();
            var AddDate = year + "年" + Month + "月" + Day + "日 " + Time;
            $(".LblTime").text(AddDate);
        }

        //事件注册
        function RegisterFunc() {
            //修改个人密码
            $(".ExchangePassword").click(function () {
                popping($(".win"), "修改个人密码", 450, 300);
                FrameHTML_UsrUpdatePwd($(".win"));
            });
            //==退出系统
            $(".ExchangeClose").click(function () {
                confirmStyle("您确认退出系统吗?", function () {
                    Ajax_Style("Login.ashx", "Logout", {}, false, function (data) {
                        if (data.Code == '1') {
                            document.location = "Login.aspx";
                        }
                    });
                });
            });
        }
        //实现用户单击导航栏跳转页面的方法
        function BindMenuClickHrefEvent() {
            $(".ul-menu li a").click(function () {
                var src = $(this).attr("url");
                //Tab页面新增页面标签，每当单击左边的导航栏的时候跳转
                var titleShow = $(this).text();
                var strHtml = '<iframe width="100%" height="100%"  frameborder="0" scrolling="no" src="' + src + '"></iframe>';
                //判断Tab标签中是否有相同的数据标签
                var isExist = $(".ttTab").tabs('exists', titleShow);
                if (!isExist) {
                    $(".ttTab").tabs('add', {
                        title: titleShow,
                        content: strHtml,
                        closable: true
                    });
                }
                else {
                    $('.ttTab').tabs('select', titleShow);
                }
            });
        }

        //弹框
        function FrameHTML_UsrUpdatePwd(win) {
            if (!$(".tbPwd").length > 0) {
                var html = "<table class='tbPwd' align='center' style='height: 100%'>";
                html += " <tr><td>旧密码:</td>";
                html += "<td><input type='password' class='txtOldPwd'/></td></tr>";
                html += "<tr><td>新密码:</td>";
                html += "<td> <input type='password' class='txtNewPwd'/></td></tr>";
                html += "<tr><td>确认密码:</td>";
                html += "<td><input type='password' class='txtNewPwdOK'/> </td></tr>";
                html += "<tr> <td colspan='2'><a href='#' class='btnOK'>保 存 </a>";
                html += "<a href='#' class='btnEsc'>取 消</a></td></tr>";
                html += "</table>";
                $(win).append(html);
                var _fulldiv = $(win).find($(".tbPwd"));
                _fulldiv.find("input[type='password']").validatebox({ required: true });
                //保存按钮
                _fulldiv.find(".btnOK").linkbutton({ iconCls: 'icon-ok',
                    onClick: function () {
                        var objEmpPwd = new Object();
                        objEmpPwd.oldPwd = $(".txtOldPwd").val();
                        objEmpPwd.newPwd = $(".txtNewPwd").val();
                        var pwdQ = $(".txtNewPwdOK").val();
                        if (CheckNull($(".txtOldPwd"), "旧密码"))
                            return;
                        if (CheckNull($(".txtNewPwd"), "新密码"))
                            return;
                        if (isPasswd(objEmpPwd.newPwd))
                            return;
                        if ((objEmpPwd.newPwd) != (pwdQ)) {
                            messagerTop("两次密码填写不相同！");
                            $(".txtNewPwdOK").focus();
                            return;
                        }
                        Ajax_Style("EployeeManagement.ashx", "UpdatePwd", objEmpPwd, true, function (data) {
                            messagerLowerRightCorner(data.Msg);
                            win.window('close');
                        });
                    }
                }).css("margin-right", "6px").css("width", "70px");
                //取消按钮
                _fulldiv.find(".btnEsc").linkbutton({ iconCls: 'icon-cancel',
                    onClick: function () {
                        _fulldiv.find("input[type='password']").val(""); //清空文本值
                        //关闭窗体
                        $('.win').window('close');
                    }
                }).css("width", "70px");
            }
        }
        
    </script>
</body>
</html>
