<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Attendance.aspx.cs" Inherits="Attendance.Attendance" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>打卡考勤</title>
    <script src="../App_Themes/Scripts/jquery-2.1.1.min.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/public.js" type="text/javascript"></script>
    <link href="../App_Themes/Css/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../App_Themes/Css/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../App_Themes/Css/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div>
        <table align="center" style="margin-top: 10%;">
            <tr>
                <td style="text-align: right">
                    <strong>今日状态：</strong>
                </td>
                <td style="text-align: left">
                    <strong class="state"></strong>
                </td>
            </tr>
            <tr>
                <td style="text-align: right">
                    <strong>时间：</strong>
                </td>
                <td style="text-align: left">
                    <strong class="showTime"></strong>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <input class="easyui-linkbutton btnSign" type="button" style="width: 80px; height: 30px;" />
                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">
        $(function () {
            if ("<%=strNoLogin %>" == "1") {
                TurnToLogin();
                return;
            }
            var Load = new AttendanceLoad();
            Load.attendanceLoad();
        });
        var AttendanceLoad = function () {
            var ciId = 0;
            this.attendanceLoad = function () {
                SignState();
                RegisterFuncSign();
            }
            //查询打卡状态
            var SignState = function () {
                $(".state").text("未签到").css("color", "Red");
                $(".btnSign").val("签到").show();
                Ajax_Style("Attendance.ashx", "GetSignStatus", {}, false, function (data) {
                    var signData = data.Obj;
                    //已签到
                    if (signData != null) {
                        if (signData.CISignInDate != "") {
                            $(".state").text("已签到").css("color", "#006400");
                            $(".showTime").text(signData.CISignInDate).css("color", "#006400");
                            $(".btnSign").val("签退");
                            ciId = signData.CIID;
                        }
                        if (signData.CISignOutDate != "") {
                            $(".state").text("已签退").css("color", "#009ACD");
                            $(".btnSign").hide();
                            $(".showTime").text(signData.CISignOutDate).css("color", "#009ACD");
                        } 
                    }
                });
            }
            //事件注册  签到|签退
            function RegisterFuncSign() {
                $(".btnSign").linkbutton({
                    onClick: function () {
                        var signText = $(".btnSign").val();
                        confirmStyle('您确认' + signText + '吗？', function () {
                            //提交打卡信息
                            Ajax_Style("Attendance.ashx", "Sign", { id: ciId }, false, function (data) {
                                messagerLowerRightCorner(data.Msg);
                                SignState();
                            })
                        });
                    }
                })
            }
        }
    </script>
</body>
</html>
