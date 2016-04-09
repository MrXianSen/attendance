 <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Attendance.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>考勤登陆</title>
    <script src="../App_Themes/Scripts/jquery-2.1.1.min.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/public.js" type="text/javascript"></script>
    <link href="../App_Themes/Css/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../App_Themes/Css/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../App_Themes/Css/Site.css" rel="stylesheet" type="text/css" />
</head>
<body oncontextmenu="return false">
    <table border="0" align="center" cellspacing="14" style="width: 400px; height: 350px;
        border: 1px solid #e5e5e5; background-color: #63B8FF; text-align: center; margin-top: 10%;
        border-top-left-radius: 15px; border-top-right-radius: 15px; border-bottom-left-radius: 15px;
        border-bottom-right-radius: 15px;">
        <tr>
            <td colspan="2">
                <h1 style="line-height: 40px; font-size: 32px; font-weight: bolder;">
                    协同办公考勤登录</h1>
            </td>
        </tr>
        <tr>
            <td>
                账 号：
            </td>
            <td>
                <input class="EmpAccount" type="text" style="width: 200px; float: left; ime-mode: disabled;"  />
            </td>
        </tr>
        <tr>
            <td>
                密 码：
            </td>
            <td>
                <input class="EmpPwd" type="password"  onpaste="return false" style="width: 200px;
                    float: left;" />
            </td>
        </tr>
        <tr>
            <td>
                验证码：
            </td>
            <td>
                <input type="text" class="verificationCode" style="width: 120px; float: left; margin-top: 1px;" /><div
                    style="float: left; width: 5px;">
                    &nbsp;
                </div>
                <a href="#" onclick="ImageCode()" style="float: left; color: White;" title="点击刷新验证码">
                    <img class="LoginImage" alt="点击刷新验证码" src="" style="width: 70px; height: 22px;" /></a>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <a href="#" class="easyui-linkbutton btnLogin" style="width: 100px">登 录 </a>
                <input class="cbLogin" type="checkbox" /><samp style="font-size: 12">自动登录</samp>
            </td>
        </tr>
    </table>
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            $(".EmpAccount").focus();
            ImageCode();
            //登录按钮
            $(".btnLogin").linkbutton({
                onClick: function () {
                    Login();
                }
            });
        });
        //注册登录事件
        function Login() {
            //验证输入数据
            if (CheckNull($(".EmpAccount"), "登录账号"))
            	return;
            if (CheckNull($(".EmpPwd"), "密码"))
                return;
            if (CheckNull($(".verificationCode"), "验证码"))
                return ;
            //存储数据empAccount,pwd,remember,checkCode
            var objEmpLogin = new Object();
            var strAccount = $(".EmpAccount").val()
            var strCode = $(".verificationCode").val()
            objEmpLogin.empAccount = Trim(strAccount);
            objEmpLogin.pwd = $(".EmpPwd").val();
            objEmpLogin.checkCode = Trim(strCode);
            objEmpLogin.remember = "0";
            if ($(".cbLogin").prop("checked"))
                objEmpLogin.Remember = "1"; //记住密码
            //提交登录数据
            Ajax_Style("Login.ashx", "Login", objEmpLogin, false, function (data) {
                location = "Home.aspx";
            }, function () {
                ImageCode();
            })
        }
        //获取验证码
        var ImageCode = function () { $(".LoginImage").attr("src", "ImageCode.aspx?ts=" + new Date().getTime()); }
    </script>
</body>

</html>
