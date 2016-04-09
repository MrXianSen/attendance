<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserManage.aspx.cs" Inherits="Attendance.UserManage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>用户管理</title>
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
                    iconcls="icon-remove">删除</a> <a href="#" class="easyui-linkbutton resetPassword"
                        iconcls="icon-reload" style="width: 120px;">重置密码</a> 员工姓名：<input class="txtUName"
                            type="text" />
            &nbsp;部门列表：<input class="deplist" />
            <a href="#" class="easyui-linkbutton btnSearch" iconcls="icon-search">查询</a>
        </div>
    </div>
    <!--弹框 用户信息编辑-->
    <div class="winUser" style="display: none">
    </div>
    <script type="text/javascript">
        //==load
        $(function () {
            if (IsLogin("<%=strNoLogin %>")) return; //验证是否登录
            if (PageAuthentication("<%=isAdmin %>")) return; //验证是否有页面权限
            var initdata = new init();
            initdata.Upload();
        });
        var init = function () {
        	var comDepList; //部门下拉列表数据
        	this.Upload = function () {
        		$(".deplist").combobox({
        			valueField: 'DepID',
        			textField: 'DepName'
        		});
        		//用户信息列表
        		$(".dg").datagrid({
        			singleSelect: true, //如果为true，则只允许选择一行。
        			autoRowHeight: false, //设置为false可以提高负载性能。
        			striped: true, //斑马线
        			fit: true,
        			fitColumns: true, //适应网格的宽度
        			border: false,
        			toolbar: ".tb",
        			columns: [[
                { field: 'EmpID', title: '用户ID', hidden: true, formatter: function (value, row) { return value; } },
                { field: 'EmpAccount', title: '登录账号', width: 230, formatter: function (value, row) { return value; } },
                { field: 'EmpName', title: '姓名', width: 230, formatter: function (value, row) { return value; } },
                { field: 'DepName', title: '所属部门', width: 230, formatter: function (value, row) { return value; } },
                { field: 'DepID', title: '所属部门ID', hidden: true, formatter: function (value, row) { return value; } },
                { field: 'IsDepManager', title: '部门经理', width: 230, formatter: function (value, row) { return value == "1" ? "√" : "—"; } },
                { field: 'IsTopManager', title: '总经理', width: 230, formatter: function (value, row) { return value == "1" ? "√" : "—"; } },
                { field: 'IsAdminister', title: '系统管理员', width: 230, formatter: function (value, row) { return value == "1" ? "√" : "—"; } }
                ]]
        		});

        		//搜索获取列表信息，empName为空时获取所有员工的信息
        		EmpSarcher("", 1);
        		RegisterFunc();
        	}
        	//查询员工列表@empName 员工姓名   @isDepList：是否需要部门列表信息   1：true|0：false
        	function EmpSarcher(empName, isDepList) {
        		var objData = {};
        		objData.searchName = empName;
        		objData.searchDepID = $(".deplist").combobox("getValue");
        		objData.isDepList = isDepList;
        		Ajax_Style("EployeeManagement.ashx", "GetList", objData, true, function (data) {
        			$(".dg").datagrid({ data: data.Obj.EmpList });
        			if (isDepList == 1) {
        				comDepList = data.Obj.DepList;
        				comDepList.unshift({ "DepID": "0", "DepName": "请选择" }); //在数组开始位置添加默认元素
        				$(".deplist").combobox({
        					data: comDepList,
        					value: "0"
        				});
        			}
        		});
        	}

        	//为表格工具栏注册查询、添加、修改、重置密码、删除信息事件
        	function RegisterFunc() {
        		//查询单击事件
        		$(".btnSearch").linkbutton({
        			onClick: function () {
        				var empName = Trim($(".txtUName").val());
        				EmpSarcher(empName, 0);
        			}
        		});
        		//添加单击设置
        		$(".btnInsert").linkbutton({
        			onClick: function () {
        				popping($(".winUser"), "添加员工信息", 500, 300);
        				FrameHTML_EmployeeManage(".winUser");
        			}
        		});
        		//修改单击事件
        		$(".btnEdit").linkbutton({
        			onClick: function () {
        				isDataGridRow($(".dg"), function (row) {
        					popping($(".winUser"), "修改员工信息", 500, 300);
        					FrameHTML_EmployeeManage(".winUser", row.EmpID);
        				});
        			}
        		});
        		//重置密码单击事件
        		$(".resetPassword").linkbutton({
        			onClick: function () {
        				isDataGridRow($(".dg"), function (row) {
        					confirmStyle("您确定要重置选中用户的密码吗？", function () {
        						Ajax_Style("EployeeManagement.ashx", "ResetPwd", { id: row.EmpID }, true, function (data) {
        							messagerLowerRightCorner(data.Msg);
        						})
        					});
        				});
        			}
        		});
        		//删除事件
        		$(".btnDelete").linkbutton({
        			onClick: function () {
        				isDataGridRow($(".dg"), function (row) {
        					confirmStyle("您确定要删除选中的信息吗?", function () {
        						Ajax_Style("EployeeManagement.ashx", "Delete", { id: row.EmpID }, false, function (data) {
        							messagerLowerRightCorner(data.Msg);
        							deleteDataGridRow($(".dg"), row);
        						});
        					});
        				});
        			}
        		});
        	}

        	//编辑用户信息弹框
        	function FrameHTML_EmployeeManage(win, id) {
        		if (!$(".tbEmp").length > 0) {
        			var html = " <table align='center' class='table tbEmp' style='height: 100%'>"
        			html += " <tr><td style='text-align: right'>所属部门：</td>";
        			html += "<td style='text-align: left'>";
        			html += "<input class=' sDepartment' style='width: 72%'/></td></tr>";
        			html += "<tr><td style='text-align: right'>登录账号：</td>";
        			html += "<td style='text-align: left'><input type='text' class='txtLogAccount'  /></td></tr>";
        			html += "<tr><td style='text-align: right'>姓名：</td>";
        			html += "<td style='text-align: left'><input type='text' class='txtName'/></td></tr>";
        			html += "<tr><td style='text-align: right'>用户授权：</td>";
        			html += "<td><input class='rdDept_manager' name='EmpRole' type='checkbox' value='2'/>部门经理<input class='rdGeneral_manager' name='EmpRole' type='checkbox'  value='3'/>总经理<input class='rdAdmin_manager' name='EmpRole' type='checkbox' value='4'/>系统管理员";
        			html += "</td></tr>";
        			html += "<tr><td colspan='2' style='text-align: center'><a  class='btnSave'> 保 存 </a>";
        			html += "<a  class='btnEsc'> 取 消 </a><input type='hidden' class='txtEmpID'/></td></tr>";
        			html += "</table>";
        			$(win).html(html);
        			var _fulldiv = $(win).find($(".tbEmp"));
        			_fulldiv.find("input[type='text']").validatebox({ required: true });
        			//绑定部门下拉列表
        			$(".sDepartment").combobox({
        				valueField: 'DepID',
        				textField: 'DepName',
        				data: comDepList,
        				value: '0',
        				onSelect: function () {
        					$(".btnSearch").click();
						}
        			});
        			//保存按钮
        			_fulldiv.find(".btnSave").linkbutton({
        				iconCls: 'icon-ok',
        				onClick: function () {
        					//获取数据并验证
        					if ($(".sDepartment").combobox("getValue") == 0) {
        						messagerTop("请选择部门！");
        						return
        					};
        					if (CheckNull($(".txtLogAccount"), "登录账号"))
        						return;
        					if (CheckAccount($(".txtLogAccount").val())) {
        						messagerStyle("登录名不能存在汉字!", 'warning', function () {
        							$(".txtLogAccount").focus();
        						});
        						return;
        					}
        					if (CheckLength($(".txtLogAccount"), "登录账号", 18))
        						return;
        					if (CheckNull($(".txtName"), "姓名"))
        						return;
        					if (CheckLength($(".txtName"), "姓名", 12))
        						return;
        					var objEmp_Svae = new Object();
        					objEmp_Svae.depID = $(".sDepartment").combobox('getValue');
        					objEmp_Svae.depName = $(".sDepartment").combobox('getText');
        					objEmp_Svae.name = Trim($(".txtName").val());
        					objEmp_Svae.account = Trim($(".txtLogAccount").val());
        					objEmp_Svae.id = $(".txtEmpID").val();
        					obj = document.getElementsByName("EmpRole");
        					empRole = [1];
        					for (k = 0; k < 3; k++) {
        						if (obj[k].checked)
        							empRole.push(obj[k].value);
        					}
        					objEmp_Svae.empRole = ',' + empRole.toString() + ",";
        					Ajax_Style("EployeeManagement.ashx", "Update", objEmp_Svae, false, function (data) {
        						messagerLowerRightCorner(data.Msg);
        						ClearContents();
        						$(".btnSearch").click();
        						$('.winUser').window('close');
        					})
        				}
        			}).css("width", "80px").css("margin-right", "6px");
        			//取消按钮
        			_fulldiv.find(".btnEsc").linkbutton({
        				iconCls: 'icon-cancel',
        				onClick: function () {
        					ClearContents(); //重置输入框
        					$('.winUser').window('close'); //关闭窗体
        					$(".btnSearch").click();
        				}
        			}).css("width", "80px");
        		}
        		//打开修改信息弹框时显示的内容
        		if (id)
        			Ajax_Style("EployeeManagement.ashx", "GetInfo", { id: id }, false, function (data) {
        				var objEmp = data.Obj;
        				var depID = 0;
        				for (var i = 0; i < comDepList.length; i++) {
        					if ((comDepList[i].DepID) == (objEmp.DepID))
        						depID = comDepList[i].DepID;
        				}
        				$(".sDepartment").combobox('setValue', depID);
        				$(".txtName").val(objEmp.EmpName);
        				$(".txtEmpID").val(objEmp.EmpID);
        				$(".txtLogAccount").val(objEmp.EmpAccount);
        				if (objEmp.IsDepManager == "1")
        					$(".rdDept_manager").prop("checked", true);
        				if (objEmp.IsTopManager == "1")
        					$(".rdGeneral_manager").prop("checked", true);
        				if (objEmp.IsAdminister == "1")
        					$(".rdAdmin_manager").prop("checked", true);
        			});
        	}
        	//重置弹框控件数据
        	function ClearContents() {
        		$(".sDepartment").combobox('setValue', "0");
        		$(".txtLogAccount,.txtName,.txtEmpID").val("");
        		$("input[type='checkbox']").prop("checked", false);
        	}

        }		
    </script>
</body>
</html>
