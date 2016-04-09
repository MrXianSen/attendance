<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeManagement.aspx.cs"
    Inherits="Attendance.TimeManagement" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>时间管理</title>
    <script src="../App_Themes/Scripts/jquery-2.1.1.min.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/public.js" type="text/javascript"></script>
    <link href="../App_Themes/Css/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../App_Themes/Css/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../App_Themes/Css/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div class="tb" style="padding: 5px; height: auto; text-align: center; margin-top: 10%">
        <input type="hidden" class="WDID" />
        设置每月工作日：
        <select class="jobYear">
        </select>
        年
        <select class="jobMonth">
        </select>
        月， 有
        <input class="easyui-numberspinner jobDateDay" style="width: 80px;" />
        工作日！
    </div>
    <table align="center" style="margin-bottom: 5px">
        <tr style="margin: 10px">
            <td colspan="3">
                <h4>
                    考勤时间设置</h4>
            </td>
        </tr>
        <tr>
            <td>
                迟到时间：
            </td>
            <td>
                <input class="easyui-timespinner WDTimeQuanturnA" style="width: 80px;" data-options="min:'00:00',value:'09:00'" />
                ——
                <input class="easyui-timespinner WDTimeQuanturnB" style="width: 80px;" data-options="min:'00:00',value:'09:00'" />
            </td>
        </tr>
        <tr>
            <td>
                早退时间：
            </td>
            <td>
                <input class="easyui-timespinner WDTimeQuanturnC" style="width: 80px;" data-options="min:'00:00',value:'09:00'" />
                ——
                <input class="easyui-timespinner WDTimeQuanturnD" style="width: 80px;" data-options="min:'00:30',value:'09:00'" />
            </td>
        </tr>
        <tr>
            <td>
                浮动时间有：
            </td>
            <td>
                <input class="easyui-numberspinner  FloatTime" style="width: 80px;" data-options="min:0,max:60,increment:5,value:20" />分钟
            </td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center">
                <a href="#" class="easyui-linkbutton btnSave" style="width: 80px;" iconcls="icon-ok">
                    保存</a>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        //==load
        $(function () {
            if (IsLogin("<%=strNoLogin %>")) return; //验证是否登录
            if (PageAuthentication("<%=isAdmin %>")) return; //验证是否有页面权限
            initData();
        });
        function initData() {
            ReadDateTimeShow();
            AttendanceTime();
            RegisterFunc();
        }
        //==读取动态年月
        function ReadDateTimeShow() {
            var y, m, year, month;
            year = new Date().getFullYear();
            month = new Date().getMonth() + 1;
            //动态绑定下拉列表年月
            for (y = year - 5; y <= year + 30; y++) {
                $(".jobYear").append("<option value='" + y + "'>" + y + "</option>");
            }
            for (m = 1; m <= 12; m++) {
                $(".jobMonth").append("<option value='" + m + "'>" + m + "</option>");
            }
            $(".jobMonth").change(function () {
                var y = $(".jobYear").val();
                var m = $(".jobMonth").val();
                setDateDay(m, y);
            });
        }
        //根据年月设置当月天数
        function setDateDay(month, year) {
           //month = parseInt(month, 10);
            var day = new Date(year, month, 0).getDate();
           $('.jobDateDay').numberspinner({ min: 1, max: day });
           $('.jobDateDay').numberspinner("setValue",22);
        }

        //==获取工作时间，并显示
        function AttendanceTime() {
        	Ajax_Style("TimeManagement.ashx", "GetInfo", {}, false, function (data) {
                var objectWorkDuration = data.Obj;
                var monthstr = objectWorkDuration.WDSignInMonth;
                var monthAry = monthstr.split("-");
                $(".jobYear").val(monthAry[0]);
                $(".jobMonth").val(monthAry[1]);
                $('.jobDateDay').numberspinner("setValue", objectWorkDuration.WDMonthDuration);
                $('.WDTimeQuanturnA').timespinner('setValue', objectWorkDuration.NormalTime);
                $('.WDTimeQuanturnB').timespinner('setValue', objectWorkDuration.AbsentSignInTime);
                $('.WDTimeQuanturnC').timespinner('setValue', objectWorkDuration.AbsentSignOutTime);
                $('.WDTimeQuanturnD').timespinner('setValue', objectWorkDuration.LeaveEarlyTime);
                $('.FloatTime').numberspinner('setValue', objectWorkDuration.WDFloatTime);
            })
        }
        function RegisterFunc() {
            //提交工作时间并更新数据
            $(".btnSave").click(function () {
                var objData = {};
                var monthDuration = $('.jobDateDay').numberspinner('getValue');
                objData.normalTime = $('.WDTimeQuanturnA').timespinner('getValue');
                objData.absentTimeSignIn = $('.WDTimeQuanturnB').timespinner('getValue');
                objData.absenTimeSignOut = $('.WDTimeQuanturnC').timespinner('getValue');
                objData.leaveEarlyTime = $('.WDTimeQuanturnD').timespinner('getValue');
                objData.floatTime = $('.FloatTime').numberspinner('getValue');
                objData.month = $(".jobYear").val() + "-" + $(".jobMonth").val();
                objData.monthWorkDuration = $('.jobDateDay').numberspinner("getValue");
                Ajax_Style("TimeManagement.ashx", "Update", objData, false, function (data) {
                    messagerLowerRightCorner(data.Msg);
                    AttendanceTime();
                });
            });
        }
    </script>
</body>
</html>
