<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FillClock.aspx.cs" Inherits="Attendance.FillClock" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>补打卡</title>
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
            <a href="#" class="easyui-linkbutton allSign" style="width: 120px">补签到/补签退</a> <a
                href="#" class="easyui-linkbutton aSignIn">补签到</a> <a href="#" class="easyui-linkbutton aSignBack">
                    补签退</a> 签到时间:
            <input class="txtBeginTime" />
            至
            <input class="txtEndTime" />
            员工姓名：
            <input class="txtUName" type="text" />
            <a href="#" class="easyui-linkbutton btnSearch" iconcls="icon-search">查询</a>
        </div>
    </div>
    <!--弹框-->
    <div class="winFillClock" style="display: none">
    </div>
</body>
<script type="text/javascript">
    //==load
    $(function () {
        if (IsLogin("<%=strNoLogin %>")) return; //验证是否登录
        if (PageAuthentication("<%=isAdmin %>")) return; //验证是否有页面权限
        var fcLoad = new FCLoad();
        fcLoad.LayoutTableOrData();
    });
    var FCLoad = function () {

        this.LayoutTableOrData = function () {
            datetimespinner_YTD($(".txtBeginTime"));
            datetimespinner_YTD($(".txtEndTime"));
            var initialDateArry = GetDate().split("-");
            $('.txtBeginTime').datetimespinner('setValue', initialDateArry[0] + "-" + initialDateArry[1] + "-01");
            $(".txtEndTime").datetimespinner('setValue', GetDate());
            //列表布局 绑定
            $(".dg").css("height", "800px").datagrid({
                singleSelect: true, //如果为true，则只允许选择一行。
                autoRowHeight: false, //设置为false可以提高负载性能。
                striped: true, //斑马线
                fitColumns: true, //适应网格的宽度
                border: false,
                loadMsg: "正在加载，请稍等...",
                toolbar: ".tb",
                columns: [[
                { field: 'CIID', title: '打卡编号', hidden: true, formatter: function (value, row) { return row.CIID; } },
                { field: 'EmpID', title: '姓名ID', hidden: true, formatter: function (value, row) { return row.EmpID; } },
                { field: 'Date', title: '考勤日期', width: 230, formatter: function (value, row) { return getDataTime(value, 0); } },
                { field: 'EmpName', title: '姓名', width: 230, formatter: function (value, row) { return row.EmpName; } },
                { field: 'DepID', title: '所属部门ID', hidden: true, formatter: function (value, row) { return row.DepID; } },
                { field: 'DepName', title: '所属部门', width: 230, formatter: function (value, row) { return row.DepName; } },
                { field: 'CISignInDate', title: '签到时间', width: 230, formatter: function (value, row) { return getDataTime(value, 1); } },
                { field: 'CISignOutDate', title: '签退时间', width: 230, formatter: function (value, row) { return getDataTime(value, 1); } }

                    ]]
            });
            RegisterFunc();
            searchDataList();
        }
        //==签到查询 获取所有签到数据,绑定于列表
        function searchDataList() {
            var _objdata = new Object();
            _objdata.searchStartDate = $(".txtBeginTime").datetimespinner('getValue');
            _objdata.searchEndDate = $(".txtEndTime").datetimespinner('getValue');
            _objdata.searchName = $(".txtUName").val();
            _objdata.searchRate = _objdata.isSelf = _objdata.searchEmpID = "0";
            Ajax_Style("Attendance.ashx", "GetDetailList", _objdata, false, function (data) {
                $('.dg').datagrid({ data: data.Obj });
            });
        }
        //工具栏事件注册
        function RegisterFunc() {
            //==查询
            $('.btnSearch').click(function () {
                searchDataList();
            });

            //==签到按钮单击事件
            $('.aSignIn').click(function () {
                popping(".winFillClock", "补签到", 500, 350);
                FrameHTML_FillClock(".winFillClock", "SignIn");
            });
            //==补签到/补签退
            $(".allSign").click(function () {
                popping(".winFillClock", "补签到|补签退", 500, 400); //弹框
                FrameHTML_FillClock(".winFillClock", "allSign");
            });
            //==签退按钮单击事件
            $('.aSignBack').click(function () {
                isDataGridRow($(".dg"), function (row) {
                    popping(".winFillClock", "补签退", 500, 350); //弹框
                    FrameHTML_FillClock(".winFillClock", "SignBack", row.CIID);
                });
            });
        }
        var EmpList; //存储部门信息与员工信息
        function FrameHTML_FillClock(win, why, id) {
            if ($(".tbFillClock").length <= 0) {
                var html = " <table class='tbFillClock'  align='center'>";
                html += "<tr><td style='text-align: right'>日期：</td>";
                html += "<td style='text-align: left'>";
                html += "<input type='text' class='attenDate'/></td></tr>";
                html += "<tr><td style='text-align: right'><label class='lblUName'>姓名：</label></td>";
                html += "<td style='text-align: left'><nobr>";
                html += "<select class='selName' style='width: 70%'></select>";
                html += "<samp class='sAllSign'><input class='cbAllSign' type='checkbox' />全部签到/签退</samp>";
                html += "</nobr></td></tr>";
                html += "<tr><td style='text-align: right'><label class='lblDepartment'>所属部门：</label></td>";
                html += "<td style='text-align: left; padding-left: 25px'>";
                html += "<label class='lblDeptID' style='width: 65%'></label>";
                html += "<input type='hidden' class='hDeptID' /></td></tr>";
                html += "<tr class='trSignIn'><td style='text-align: right'><label class='lblSignInTime'>补签到时间：</label></td>";
                html += "<td style='text-align: left'><input class='txtSignIn'  /></td></tr>";
                html += "<tr class='trSignBack'><td style='text-align: right'><label class='lblSignBackTime'>补签退时间：</label></td>";
                html += "<td style='text-align: left'><input class='txtSignBack'/></td></tr>";
                html += "<tr><td style='text-align: right'>补签说明：</td>";
                html += "<td style='text-align: left'><textarea class='txtExplain'></textarea></td></tr>";
                html += "<tr><td colspan='2' style='text-align: center'>";
                html += "<a href='#' class='btnSave''> 保 存 </a>";
                html += "<a href='#' class='btnEsc'> 取 消  </a> <input type='hidden' class='txtCIID'/><input type='hidden' class='txtAllSignInOrOut'/>";
                html += "</td></tr>";
                html += "</table>";
                $(win).html(html);
                var _fulldiv = $(win).find($(".tbFillClock"));
                _fulldiv.find(".attenDate").datetimespinner({
                    width: '120'
                });
                datetimespinner_YTD(_fulldiv.find(".attenDate"));
                _fulldiv.find(".txtSignIn,.txtSignBack").timespinner({
                    width: '120',
                    value: '00:00'
                });
                //保存按钮事件
                _fulldiv.find(".btnSave").linkbutton({
                    iconCls: 'icon-ok',
                    width: 80,
                    onClick: function () {
                        //数据验证
                        var objData = {};
                        var _type = $(".txtAllSignInOrOut").val();
                        objData.allSignInOrOut = _type == "allSign" ? "1" : "0"; // 是否是为所有人|某个人进行补签，是“1”不是“0”
                        objData.id = $(".txtCIID").val(); //签到记录ID
                        objData.appendSignDate = $(".attenDate").datetimespinner("getValue"); // 补签日期
                        var currentDateArry = GetDate().split("-"); //当前系统日期
                        var starttime = new Date(currentDateArry[0], currentDateArry[1], currentDateArry[2]);
                        var starttimes = starttime.getTime();
                        var entryDateArry = (objData.appendSignDate).split("-"); //用户输入时间
                        var lktime = new Date(entryDateArry[0], entryDateArry[1], entryDateArry[2]);
                        var lktimes = lktime.getTime();
                        if (starttimes < lktimes) {
                            messagerTop("补签日期需小于当前日期！");
                            return;
                        }
                        objData.empID = $(".selName").combobox("getValue"); //被补签员工的ID
                        if (objData.empID == "0" && CheckCheckbox($(".cbAllSign")) == "0") {
                            messagerTop("请选择员工！");
                            return;
                        }
                        objData.empName = $(".selName").combobox("getText"); // 被补签员工的姓名
                        objData.depID = $(".hDeptID").val(); // 被补签员工的部门ID
                        objData.depName = $(".lblDeptID").text(); // 被补签员工的部门名称
                        objData.signInTime = $('.txtSignIn').timespinner('getValue'); //补签到的具体时间
                        objData.signOutTime = $('.txtSignBack').timespinner('getValue'); //补签退时间
                        objData.ciNote = Trim($(".txtExplain").val()); // 补签说明
                        if (CheckNull($(".txtExplain"), "补签说明")) return;
                        Ajax_Style("Attendance.ashx", "UpdateSupplement", objData, true, function (data) {
                            messagerLowerRightCorner(data.Msg);
                            clearContents();
                            $('.btnSearch').click();
                        });
                    }
                }).css("margin-right", "6px");
                //取消按钮
                _fulldiv.find(".btnEsc").linkbutton({
                    iconCls: 'icon-cancel',
                    width: 80,
                    onClick: clearContents
                });
                function clearContents() {
                    $(".attenDate").datetimespinner({ readonly: false });
                    $(".selName").combobox({ readonly: false });
                    $(".selName").combobox("setValue", "0");
                    $(".txtSignIn").timespinner("setValue", "");
                    $(".txtSignBack").timespinner("setValue", "");
                    $(".lblDeptID").text("");
                    $(".hDeptID,.txtCIID,.txtExplain").val("");
                    $(".cbAllSign").prop("checked", false);
                    $('.winFillClock').window('close');
                };
                //用户信息下拉狂
                _fulldiv.find(".selName").combobox({
                    valueField: 'EmpID',
                    textField: 'EmpName',
                    width: 120,
                    onChange: function (value) {//员工ID
                        if (value == 0) {
                            $(".hDeptID").val(""); // 被补签员工的部门ID
                            $(".lblDeptID").text(""); // 被补签员工的部门名称
                        }
                        else
                            for (var i = 0; i < EmpList.length; i++) {
                                if (value == EmpList[i].EmpID) {
                                    $(".hDeptID").val(EmpList[i].DepID); //被补签员工的部门ID
                                    $(".lblDeptID").text(EmpList[i].DepName); // 被补签员工的部门名称
                                }
                            }
                    }
                });
                //获取用户信息列表
                Ajax_Style("EployeeManagement.ashx", "GetList", {
                    searchName: "",
                    searchDepID: "",
                    isDepList: "0"
                }, false, function (data) {
                    EmpList = data.Obj.EmpList; //存储所有员工信息
                    EmpList.unshift({ "EmpID": "0", "EmpName": "请选择" });
                    $(".selName").combobox({ data: EmpList, value: "0" });
                });
                //是否全选补签到/签退
                $(".cbAllSign").click(function () {
                    if (CheckCheckbox($(".cbAllSign")) == "1") {
                        $(".selName").combobox({ readonly: true });
                        $(".lblDeptID").text("");
                        $(".hDeptID").val("");
                    }
                    else
                        $(".selName").combobox({ readonly: false });
                });
            }
            $(".txtAllSignInOrOut").val(why);
            var initialDateArry = GetDate().split("-");
            var initialDate = initialDateArry[0] + "-" + initialDateArry[1] + "-" + (initialDateArry[2] - 1);
            switch (why) {
                case "SignBack": //签退
                    $(".trSignBack").show();
                    $(".winFillClock .tbFillClock .sAllSign,.winFillClock .tbFillClock .trSignIn").hide();
                    $(".attenDate").datetimespinner({ readonly: true });
                    $(".selName").combobox({ readonly: true });
                    $(".txtSignBack").timespinner("setValue", "18:00");
                    $(".txtSignIn").timespinner("setValue", "");
                    break;
                case "SignIn": //签到
                    $(".trSignIn").show();
                    $(".sAllSign,.trSignBack").hide();
                    $(".attenDate").datetimespinner("setValue", initialDate);
                    $(".txtSignIn").timespinner("setValue", "09:00");
                    $(".txtSignBack").timespinner("setValue", "");
                    break;
                case "allSign": //签退|签到
                    $(".trSignBack,.trSignIn,.sAllSign").show();
                    $(".selUName,.lblUName,.selDepartment,.lblDepartment,.btnSave_allSign").css("display", "block");
                    $(".attenDate").datetimespinner("setValue", initialDate);
                    $(".txtSignIn").timespinner("setValue", "09:00");
                    $(".txtSignBack").timespinner("setValue", "18:00");
                    break;
            }
            if (id)
                Ajax_Style("Attendance.ashx", "GetInfo", { id: id }, false, function (data) {
                    var dataRow = data.Obj;
                    $(".txtCIID").val(dataRow.CIID);
                    $(".attenDate").datetimespinner("setValue", getDataTime(dataRow.Date, 0));
                    $(".selName").combobox("setValue", dataRow.EmpID);
                    $(".lblDeptID").text(dataRow.DepName);
                    $(".hDeptID").val(dataRow.DepID);
                }, function () {
                    $('.winFillClock').window('close');
                });
        }
    }
</script>
</html>
