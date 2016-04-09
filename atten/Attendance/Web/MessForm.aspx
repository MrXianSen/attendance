<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" CodeBehind="MessForm.aspx.cs"
    Inherits="Attendance.MessForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>异常消息</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
</head>
<body style="margin: 0px; vertical-align: top; width: 100%; height: 100%" scroll="auto"
    onload="PageLoad()">
    <table height="100%" cellspacing="0" cellpadding="0" width="100%" id="Table0">
        <tr valign="middle" id="TrMess">
            <td align="center">
                <label id="mess">
                    <%=strMess%>
                </label>
                <a href="javascript:history.go(-1)">返回</a>
            </td>
        </tr>
    </table>
    <script language="javascript" type="text/javascript">
		<!--
        function PageLoad() {
            var strType = "<%=strType%>";
            if (strType == "1") {
                TrMess.vAlign = "top";
            }
            else if (strType == "2") {
                TrMess.vAlign = "top";
                TrMess.children[0].align = "left";
            }
            if ("<%=strBorder%>" == "1")
                Table0.border = "1";
            if ("<%=strMessColor%>" != "")
                mess.style.color = "<%=strMessColor%>";
        }
		-->
    </script>
</body>
</html>
