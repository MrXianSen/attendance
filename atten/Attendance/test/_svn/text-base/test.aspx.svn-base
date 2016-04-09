<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="Attendance.test.test" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>测试</title>
    <script src="jquery-2.1.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $('#test_btn').click(function(){
                btn_Click();
            });
        });
        function btn_Click() {
            $.ajax({
                type: 'POST',
                url: '../Handler/EployeeManagement.ashx?func=empInfoList&t=' + new Date().getTime(),
                data: {empName:'张建国'},
                success: function (data) {
                    callBack(data);
                },
                error: function () {
                    alert("调用失败");
                }
            });
        }
        function callBack(data) {
            $('#test_label').text = data.Msg;
        }
    </script>
</head>
<body>
    <div>
        <input type="button" id="test_btn" class="test" value="测试" />
        <label class="test" id="test_label">
        </label>
    </div>
</body>
</html>
