<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonthlyStatistics.aspx.cs"
    Inherits="Attendance.MonthlyStatistics" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>月签到记录</title>
    <script src="../App_Themes/Scripts/jquery-2.1.1.min.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="../App_Themes/Scripts/public.js" type="text/javascript"></script>
    <link href="../App_Themes/Css/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../App_Themes/Css/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../App_Themes/Css/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <table class="dg">
    </table>
    <div class="tb">
        <div class="tba">
            <a href="#" class="easyui-linkbutton btnDetails" iconcls="icon-save" style="width: 150px;">
                签到详细记录信息</a> 日期：
            <input class="searchDate" />
            <samp class="samp">
                姓名：
                <input class="txtUName" type="text" /></samp>
            考勤状态：
            <select class="slState">
                <option value="">所有</option>
                <option value="1">全勤</option>
                <option value="0">非全勤</option>
            </select>
            <a href="#" class="easyui-linkbutton btnSearch" iconcls="icon-search">查询</a>
        </div>
    </div>
    <!--弹框-->
    <div class="winSave" style="display: none">
        <iframe class="frm" width="100%" height="99%" frameborder="0"></iframe>
    </div>
    <script type="text/javascript">
        $(function () {
            if (IsLogin("<%=strNoLogin %>"))
                return; //验证是否登录
            var isPage = new Object();
            isPage.isDepManage = "<%=isDepManage %>" == "1" ? true : false;
            isPage.isTopManage = "<%=isTopManage %>" == "1" ? true : false;
            var load = new MsLoad(isPage);
            load.LayoutTableORData();
        });
        var MsLoad = function (isPage) {

            var pageType = "1"; //页面标记0：我的签到记录月统计信息  1：员工签到记录月统计信息
            this.LayoutTableORData = function () {
                datetimespinner_Years($(".searchDate"));
                var type = getQueryString("type"); //获取URL中type的值
                if (type == "1") {//员工签到记录月统计信息
                    var role = "0"; //验证是否有页面权限	
                    if (isPage.isDepManage || isPage.isTopManage)
                        role = "1";
                    if (PageAuthentication(role))
                        return;
                } else { //我的签到记录月统计信息
                    $(".samp").hide(); //隐藏员工姓名搜索框
                    pageType = "0";
                }

                //列表布局
                $(".dg").css("height", "800px").datagrid({
                    singleSelect: true, //如果为true，则只允许选择一行。
                    autoRowHeight: false, //设置为false可以提高负载性能。
                    fit: true, //当设置为true的时候面板大小将自适应父容器。
                    striped: true, //斑马线
                    fitColumns: true, //适应网格的宽度
                    border: false,
                    loadMsg: "正在加载，请稍等...",
                    toolbar: ".tb",
                    columns: [[
                { field: 'ASMonth', title: '签到月份', width: 150, align: 'center', formatter: function (value, row) { return value; } },
                { field: 'EmpID', title: '用户ID', hidden: true, formatter: function (value, row) { return value; } },
                { field: 'EmpName', title: '姓名', width: 150, align: 'center', formatter: function (value, row) { return value; } },
                { field: 'DepName', title: '所属部门', width: 150, align: 'center', formatter: function (value, row) { return value; } },
                { field: 'ASStandardDuration', title: '工作时间', width: 150, align: 'center', formatter: function (value, row) { return value; } },
                { field: 'ASRealityDuration', title: '实际工时', width: 150, align: 'center', formatter: function (value, row) { return value; } },
                { field: 'ASLateNumber', title: '迟到次数', width: 150, align: 'center', formatter: function (value, row) { return value; } },
                { field: 'ASLeaveEavlyNumber', title: '早退次数', width: 150, align: 'center', formatter: function (value, row) { return value; } },
                { field: 'ASAbsenteeismNumber', title: '旷工次数', width: 150, align: 'center', formatter: function (value, row) { return value; } },
                //{ field: 'ASGooutNumber', title: '外出次数', width: 150, formatter: function (value, row) { return value; } },
                {field: 'ASNormalNumber', title: '正常上班次数', width: 150, align: 'center', formatter: function (value, row) { return value; } }
               ]]
                });
                RegisterFunc();
                $(".btnSearch").click();
            }
            function RegisterFunc() {
                //月签到信息查询
                $(".btnSearch").click(function () {
                    var objData = new Object();
                    var searchMonth = $(".searchDate").datetimespinner('getValue');
                    objData.searchMonth = searchMonth.replace("-", "");
                    objData.searchRate = $(".slState").val();
                    objData.searchName = $(".txtUName").val();
                    objData.isSelf = pageType == "0" ? "1" : "0";
                    Ajax_Style("Attendance.ashx", "GetMonthAttendanceList", objData, true, function (data) {
                        $('.dg').datagrid({
                            data: data.Obj
                        });
                    });
                });
                //签到详细记录信息按钮事件
                $(".btnDetails").click(function () {
                    isDataGridRow($(".dg"), function (row) {
                        popping($(".winSave"), "签到详细记录信息", 1000, 500);
                        $('.winSave').window({ closable: true });
                        var Date = $(".searchDate").datetimespinner('getValue');
                        //员工ID
                        $(".frm").attr("src", "Detailed.aspx?date=" + Date + "&empID=" + row.EmpID); //设置iframe标签的属性src的值
                    });
                });
            }
        }


    </script>
</body>
</html>
