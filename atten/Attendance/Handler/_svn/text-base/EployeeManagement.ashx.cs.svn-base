using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Attendance.BLL.Service;
using Attendance.Common.Util;
using Attendance.Model;
using Attendance.MODEL.Model;

namespace Attendance.Handler
{
    /// <summary>
    /// EployeeManagement
    /// 员工管理
    /// </summary>
    public class EployeeManagement : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string funC = context.Request["func"];
            if (!UsrAuth.IsLogin(context.Session, context.Request, context.Response))
            {
                context.Response.Write("{noLogin:\"1\"}");
                return;
            }
            try
            {
                switch (funC)
                {
                    case "Update":
                        if (IsAdminister(context))
                            Update(context);
                        break;
                    case "Delete":
                        if (IsAdminister(context))
                            Delete(context);
                        break;
                    case "ResetPwd":
                        if (IsAdminister(context))
                            ResetPwd(context);
                        break;
                    case "UpdatePwd":
                        UpdatePwd(context);
                        break;
                    case "GetList":
                        GetList(context);
                        break;
                    case "GetInfo":
                        GetInfo(context);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                context.Response.ClearContent();
                context.Response.Write("{data:\"参数错误！\"}");
            }
        }

        private bool IsAdminister(HttpContext context)
        {
            if (!UsrAuth.IsAdminister(context.Session))
            {//无管理员权限
                context.Response.Write("{\"Code\":\"0\",\"Msg\":\"无‘管理员’权限！\"}");
                return false;
            }
            return true;
        }

        /**************************************************
         * 功能：添加员工
         * ************************************************/
        private void Update(HttpContext context)
        {
            int code = 0;
            string msg = "";
            string json = "";
            EmployeeInfo employee = new EmployeeInfo();
            #region 参数值赋给employee对象
            employee.EmpID = context.Request["id"];
            employee.EmpName = context.Request["name"];
            employee.EmpAccount = context.Request["account"];
            employee.DepID = context.Request["depID"];
            employee.DepName = context.Request["depName"];
            employee.EmpRole = context.Request["empRole"];
            employee.EmpPwd = DataBase.StringToMD5Hash("123456");
            employee.EmpCreateDate = DateTime.Now;
            #endregion
            EmployeeInfo empInfo = EmployeeService.Instance.IsEmployeeExist(employee.EmpAccount, employee.EmpPwd);
            if (string.IsNullOrEmpty(employee.EmpID))
            {
                //验证员工的登录账号是否已经存在
                if (empInfo != null)
                {
                    code = 0;
                    msg = "该账号已经被其他员工使用，请重新输入员工登录账号！";
                }
                else
                {
                    if (!DepartmentService.Instance.isDepartmentExist(employee.DepName))
                    { code = 0; msg = "你输入的部门名称存在错误，请检查后重新操作！"; }
                    else
                    {
                        int res = EmployeeService.Instance.Update(employee);
                        if (res == 0)
                            msg = "添加失败";
                        else { code = 1; msg = "添加成功"; }
                    }
                }
            }
            else
            {
                int res = EmployeeService.Instance.Update(employee);
                if (res == 0)
                    msg = "修改失败";
                else { code = 1; msg = "修改成功"; }
            }
            json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
            context.Response.Write(json);
        }
        /**************************************************
         * 删除员工
         * ************************************************/
        private void Delete(HttpContext context)
        {
            int code = 0;
            string msg = "";
            string json = "";
            string empID = context.Request["id"];

            int res = EmployeeService.Instance.DeleteEmployee(empID);
            if (res == 0) { msg = "删除失败"; }
            else { code = 1; msg = "删除成功"; }
            json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
            context.Response.Write(json);
        }
        /**************************************************
         * 重置密码
         * ************************************************/
        private void ResetPwd(HttpContext context)
        {
            int code = 0;
            string msg = "";
            string json = "";
            string empID = context.Request["id"];

            int res = EmployeeService.Instance.ResetPwd(empID);
            if (res == 0) { code = 0; msg = "密码重置失败"; }
            else { code = 1; msg = "密码重置成功"; }
            json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
            context.Response.Write(json);
        }
        /**************************************************
         * 修改密码
         * ************************************************/
        private void UpdatePwd(HttpContext context)
        {
            int code = 0;
            string msg = "";
            string json = "";
            string empID = UsrAuth.GetempID(context.Session);
            string oldPwd = DataBase.StringToMD5Hash((context.Request["oldPwd"]));
            string newPwd = DataBase.StringToMD5Hash(context.Request["newPwd"]);

            int res = EmployeeService.Instance.ModifyPwd(empID, newPwd, oldPwd);
            if (res == 0) { msg = "修改失败"; }
            else
            {
                code = 1; msg = "修改成功";
                //修改成功之后修改Cookie中的值
                if (UsrAuth.GetLoginCookie(context.Request) != null)
                {
                    EmployeeInfo emp = EmployeeService.Instance.GetEmployee(int.Parse(empID));
                    UsrAuth.SetLoginCookie(context.Response, emp.EmpAccount, newPwd);
                }
            }
            json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
            context.Response.Write(json);
        }
        /**************************************************
         * 获取员工信息列表
         * MODIFY
         * ************************************************/
        private void GetList(HttpContext context)
        {
            string currentEmpName = UsrAuth.GetempName(context.Session);
            string searchName = context.Request["searchName"];
            string searchDepID = context.Request["searchDepID"];
            string isDepList = context.Request["isDepList"];
            if (string.IsNullOrEmpty(searchDepID)) searchDepID = "0";

            FeedBackMsg<DepartmentAndEmployee> feedBack = new FeedBackMsg<DepartmentAndEmployee>();
            List<EmployeeInfo> empList = new List<EmployeeInfo>();
            if (UsrAuth.IsAdminister(context.Session))
            {
                //管理员或总经理
                empList = EmployeeService.Instance.GetList(searchName, int.Parse(searchDepID));
            }
            else if (UsrAuth.IsDepManager(context.Session))
            {
                //部门经理
                string depID = UsrAuth.GetdepID(context.Session);
                empList = EmployeeService.Instance.GetEmployee(int.Parse(depID), searchName);
            }
            else
            {
                feedBack.Code = 0;
                feedBack.Msg = "没有相应的权限";
                feedBack.Obj = null;
            }
            DepartmentAndEmployee depAndEmp;
            if (!string.IsNullOrEmpty(isDepList) && isDepList.Equals("1"))
            {
                List<Department> depList = DepartmentService.Instance.GetDepartment();
                depAndEmp = new DepartmentAndEmployee()
                {
                    DepList = depList,
                    EmpList = empList
                };
            }
            else
            {
                depAndEmp = new DepartmentAndEmployee()
                {
                    EmpList = empList
                };
            }
            feedBack.Code = 1;
            feedBack.Msg = "员工和部门列表";
            feedBack.Obj = depAndEmp;
            string json = ObjToJson.ToJson(feedBack);
            context.Response.Write(json);
        }

        private void GetInfo(HttpContext context)
        {
            int selectedEmpID = int.Parse(context.Request["id"]);

            EmployeeInfo empInfo = EmployeeService.Instance.GetEmployee(selectedEmpID);


            FeedBackMsg<EmployeeInfo> feedBack = new FeedBackMsg<EmployeeInfo>()
            {
                Code = 1,
                Msg = "选中员工信息",
                Obj = empInfo
            };
            string json = ObjToJson.ToJson(feedBack);
            context.Response.Write(json);
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}