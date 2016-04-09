<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detailed.aspx.cs" Inherits="Attendance.Detailed" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>签到详细记录信息</title>
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
            日期：
            <input class="txtStartTime" />
            至
            <input class="txtEndTime" />
            <samp class="samp">
                姓名：
                <input class="txtEmpName" type="text" /></samp>
            考勤状态：
            <select class="slState">
                <option value="">所有</option>
                <option value="1">全勤</option>
                <option value="0">非全勤</option>
            </select>
            <a href="#" class="easyui-linkbutton btnSearch" iconcls="icon-search">查询</a>
        </div>
    </div>
    <script type="text/javascript">

        $(function () {
            if (IsLogin("<%=strNoLogin %>")) return; //验证是否登录
            var isPage = new Object();
            isPage.isDepManage = "<%=isDepManage %>" == "1" ? true : false;
            isPage.isTopManage = "<%=isTopManage %>" == "1" ? true : false;
            var load = new DetailedLoad(isPage);
            load.LayoutTableORData();
            $(".btnSearch").click();
        });
        var DetailedLoad = function (isPage) {
            var pageType = "1"; //页面标记  0：我的签到详细记录信息；1：员工签到详细记录信息；2：某个人的签到记录信息。
            this.LayoutTableORData = function () {
                var type = getQueryString("type"); //获取URL中type的值
                if (type == "1") {//== 1:员工签到详细记录信息
                    var role = "0"; //验证是否有页面权限
                    if (isPage.isDepManage || isPage.isTopManage)
                        role = "1";
                    if (PageAuthentication(role))
                        return;
                } else {//==0:我的签到详细记录信息
                    $(".samp").hide(); //隐藏员工姓名搜索框
                    pageType = "0";
                }
                var pagEmpID = getQueryString("empID");
                if (pagEmpID != null) { //签到记录信息按钮|2：某个人的签到记录信息
                    pageType = "2";
                    $(".tba").hide();
                }
                //时间
                datetimespinner_YTD($(".txtStartTime"));
                datetimespinner_YTD($(".txtEndTime"));
                //设置当前时间
                var initialDateArry = GetDate().split("-");
                $('.txtStartTime').datetimespinner('setValue', initialDateArry[0] + "-" + initialDateArry[1] + "-01");
                $(".txtEndTime").datetimespinner('setValue', GetDate());

                //列表
                $(".dg").css("height", "800px").datagrid({
                    singleSelect: true, //如果为true，则只允许选择一行。
                    autoRowHeight: false, //设置为false可以提高负载性能。
                    striped: true, //斑马线
                    fit: true,
                    fitColumns: true, //适应网格的宽度
                    border: false,
                    loadMsg: "正在加载，请稍等...",
                    toolbar: ".tb",
                    columns: [[
                { field: 'Date', title: '考勤日期', width: 150, formatter: function (value, row) { return getDataTime(value, 0) } },
                { field: 'EmpName', title: '姓名', width: 150, formatter: function (value, row) { return row.EmpName; } },
                { field: 'DepName', title: '所属部门', width: 150, formatter: function (value, row) { return row.DepName; } },
                { field: 'CISignInDate', title: '签到记录', width: 150, formatter: function (value, row) { return getDataTime(value, 1) } },
                { field: 'CISignOutDate', title: '签退记录', width: 150, formatter: function (value, row) { return getDataTime(value, 1) } },
                { field: 'CIRealityWorkDuration', title: '实际工时', width: 120, formatter: function (value, row) { return row.CIRealityWorkDuration; } },
                { field: 'CIIsLate', title: '迟到', width: 100, formatter: function (value, row) {
                    if (row.CIIsLate) return "是";
                    else if (row.CISignOutDate == "00:00:00") return "—";
                    else return "否";
                }
                },
                { field: 'CIIsLeaveEavly', title: '早退', width: 100, formatter: function (value, row) {
                    if (row.CISignOutDate == "00:00:00") return "—";
                    if (row.CIIsLeaveEavly) return "是";
                    else return "否";
                }
                },
                { field: 'CIIsAbsenteeism', title: '旷工', width: 100, formatter: function (value, row) {
                    if (row.CISignOutDate == "00:00:00") return "—";
                    if (row.CIIsAbsenteeism) return "是";
                    else return "否";
                }
                },
                { field: 'CIIsNormal', title: '正常上班', width: 100, formatter: function (value, row) {
                    if (row.CISignOutDate == "00:00:00") return "—";
                    if (row.CIIsNormal) return "是";
                    else return "否";
                }
                }
            ]]
                });
                //查询按钮事件
                $(".btnSearch").click(function () {
                    var objData = new Object();
                    objData.searchEmpID = pageType == "2" ? pagEmpID : "0";
                    var date = getQueryString('date');
                    if (date != null) {
                        var dateArry = date.split("-");
                        objData.searchStartDate = date + "-01";
                        var endDate = new Date(dateArry[0], dateArry[1], 0).getDate();
                        objData.searchEndDate = date + "-" + endDate;
                    }
                    else {
                        objData.searchStartDate = $('.txtStartTime').datetimespinner('getValue');
                        objData.searchEndDate = $(".txtEndTime").datetimespinner('getValue');
                    }
                    objData.searchRate = $(".slState").val();
                    objData.searchName = $(".txtEmpName").val();
                    objData.isSelf = pageType != "1" ? "1" : "0"; //1:查询我的签到记录 0：查询员工签到记录
                    Ajax_Style("Attendance.ashx", "GetDetailList", objData, true, function (data) {
                        $('.dg').datagrid({
                            data: data.Obj
                        });
                    });
                });
            }

        }
    </script>
</body>
</html>
