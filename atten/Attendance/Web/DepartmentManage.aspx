<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepartmentManage.aspx.cs"
    Inherits="Attendance.DepartmentManage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>部门管理</title>
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
            <a href="#" class="easyui-linkbutton btnInsert" iconcls="icon-add">新增</a> <a href="#"
                class="easyui-linkbutton btnEdit" iconcls="icon-edit">修改</a> <a href="#" class="easyui-linkbutton btnDelete"
                    iconcls="icon-remove">删除</a> 部门名称：<input class="txtSearchDepName" type="text" />
            <a href="#" class="easyui-linkbutton btnSearch" iconcls="icon-search">查询</a>
        </div>
    </div>
    <!--弹框-->
    <div class="winDept" style="width: 500px; height: 200px; display: none">
    </div>
    <label class="dataArea">
    </label>
    <script type="text/javascript" language="javascript">
        $(function () {
            if (IsLogin("<%=strNoLogin %>"))
                return; //验证是否登录
            if (PageAuthentication("<%=isAdmin %>"))
                return; //验证是否有页面权限
            initData();
        });

        //初始化
        function initData() {
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
                { field: 'DepID', title: '部门编号', width: 300, formatter: function (value, row) { return value; } },
                { field: 'DepName', title: '部门名称', width: 300, formatter: function (value, row) { return value; } }
            ]]
            });

            RegisterFunc();
            depSearch(''); //信息加载
        }
        //更新数据,SearchDepName为空时，查询所有部门的数据
        function depSearch(SearchDepName) {
            Ajax_Style("DepartmentManagement.ashx", "GetList", { searchName: SearchDepName }, false, function (data) {
                $('.dg').datagrid({ data: data.Obj });
            });
        }
        //表格工具栏事件注册
        function RegisterFunc() {
            //===========查询事件
            $(".btnSearch").linkbutton({
                onClick: function () {
                    var SearchDepName = Trim($(".txtSearchDepName").val());
                    depSearch(SearchDepName);
                }
            })
            //===========添加单击设置
            $(".btnInsert").linkbutton({
                onClick: function () {
                    popping(".winDept", "添加部门信息", 500, 200); //弹框
                    FrameHTML_Department(".winDept");
                }
            })
            //===========修改单击设置
            $(".btnEdit").linkbutton({
                onClick: function () {
                    isDataGridRow($(".dg"), function (row) {
                        popping(".winDept", "修改部门信息", 500, 200); //弹框
                        FrameHTML_Department(".winDept", row.DepID);
                    });
                }
            });
            //============删除单击事件
            $(".btnDelete").linkbutton({
                onClick: function () {
                    isDataGridRow($(".dg"), function (row) {
                        confirmStyle("您确定要删除选中的信息吗？", function () {
                            Ajax_Style("DepartmentManagement.ashx", "Delete", { id: row.DepID }, false, function (data) {
                                messagerLowerRightCorner(data.Msg);
                                deleteDataGridRow($(".dg"), row);
                            })
                        });
                    })
                }
            })
        }
        //弹框
        function FrameHTML_Department(win, depID) {
            var chekDepList = {};
            if (!$(".tbDep").length > 0) {//判断是否存在
                var html = " <table class='tbDep' align='center' style='margin-top: 6%'>";
                html += " <tr>";
                html += "<td>部门名称：</td>";
                html += "<td><input type='text' class='txtDepName'/><input type='hidden' class='txtDepID'/></td>";
                html += " </tr>";
                html += "<tr>";
                html += "<td colspan='2'><a class='btn_ok' >保存</a><a class='btn_cancel' >取消</a></td>";
                html += " </tr>";
                html += "</table>";
                $(win).append(html);
                var tbDep = $(win).find(".tbDep");
                tbDep.find(".txtDepName").validatebox({ required: true });
                //保存按钮事件
                tbDep.find(".btn_ok").linkbutton({
                    iconCls: 'icon-ok',
                    onClick: btnOk_onClick
                }).css({ "width": "80px", "margin-right": "6px" });
                //取消按钮事件
                tbDep.find(".btn_cancel").linkbutton({
                    iconCls: 'icon-cancel',
                    onClick: function () {
                        $(".txtDepName,.txtDepID").val("");
                        $('.winDept').window('close'); //关闭窗体
                        $(".btnSearch").click();
                    }
                }).css("width", "80px");
            }
            //显示原有数据
            if (depID)
                Ajax_Style("DepartmentManagement.ashx", "GetInfo", { id: depID }, false, function (data) {
                    chekDepList.DepID = data.Obj.DepID;
                    chekDepList.DepName = data.Obj.DepName;
                    $(".txtDepName").val(chekDepList.DepName);
                    $(".txtDepID").val(chekDepList.DepID);
                });
             function btnOk_onClick(){
               /* if (chekDepList.DepName == Trim($(".txtDepName").val())) {
                    messagerTop("您没有修改数据！");
                    return;
                }*/
                if (CheckNull($(".txtDepName"), "部门名称"))
                    return;
                if (CheckLength($(".txtDepName"), "部门名称", 20))
                    return;
                var objData = new Object();
                objData.id = $(".txtDepID").val();
                objData.name = Trim($(".txtDepName").val());
                Ajax_Style("DepartmentManagement.ashx", "Update", objData, false, function (data) {
                    $(".txtDepName,.txtDepID").val("");
                    $(".btnSearch").click();
                    $('.winDept').window('close');
                    messagerLowerRightCorner(data.Msg);
                });
            }
        }

    </script>
</body>
</html>
