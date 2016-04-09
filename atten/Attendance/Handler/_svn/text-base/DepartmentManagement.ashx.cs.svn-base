using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Attendance.BLL.Service;
using Attendance.MODEL.Model;
using Attendance.Common.Util;
using Attendance.Model;

namespace Attendance.Handler
{
    /// <summary>
    /// DeapartmentManagement
    /// 部门管理
    /// </summary>
    public class DepartmentManagement : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string funC = context.Request.QueryString["func"];
            //登录验证
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
                        Update(context);
                        break;
                    case "Delete":
                        Delete(context);
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
        /**************************************************
         * 更新部门
         * 功能：添加，修改
         * 页面参数：depID 部门ID，可以为null
         *           depName 添加或者修改后的部门名称
         * ************************************************/
        public void Update(HttpContext context)
        {
            int code = 0;
            string msg = "";
            string json = "";
            string depID = context.Request["id"];
            string depName = context.Request["name"];

            #region 验证操作权限
            if (!UsrAuth.IsAdminister(context.Session))
            {//无管理员权限
                code = 0;
                msg = "无‘管理员’权限";
                json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
                context.Response.Write(json);
                return;
            }
            #endregion

            DepartmentService departmentService = new DepartmentService();
            if (string.IsNullOrEmpty(depID))
            {
                //添加
                int res = departmentService.UpdateDepartment(depName);
                if (res == 0) { code = 0; msg = "添加失败"; }
                else { code = 1; msg = "添加成功"; }
            }
            else
            {
                //修改
                int res = departmentService.UpdateDepartment(depName, int.Parse(depID));
                if (res == 0) { code = 0; msg = "修改失败"; }
                else { code = 1; msg = "修改成功"; }
            }
            json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
            context.Response.Write(json);
        }
        /**************************************************
         * 删除部门
         * 功能：删除
         * 页面参数：depID 部门ID
         * ************************************************/
        public void Delete(HttpContext context)
        {
            int code = 0;
            string msg = "";
            string json = "";
            string depID = context.Request["id"];

            #region 验证操作权限
            if (!UsrAuth.IsAdminister(context.Session))
            {//无管理员权限
                code = 0;
                msg = "无‘管理员’权限";
                json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
                context.Response.Write(json);
                return;
            }
            #endregion

            //处理删除部门下的所属员工
            int res = DepartmentService.Instance.DeleteDepartment(depID);
            if (res == 0) { code = 0; msg = "删除失败"; }
            else { code = 1; msg = "删除成功"; }
            json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
            context.Response.Write(json);
        }
        /**************************************************
         * 部门列表
         * 可用于查询、点击部门管理时显示部门列表
         * 页面参数：depName 部门名称，当为空是获取部门列表
         * ***********************************************/
        public void GetList(HttpContext context)
        {
            int code = 0;
            string msg = "";
            string json = "";
            string depName = context.Request["searchName"];

            #region 验证操作权限
            if (!UsrAuth.IsAdminister(context.Session))
            {//无管理员权限
                code = 0;
                msg = "无‘管理员’权限";
                json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
                context.Response.Write(json);
                return;
            }
            #endregion

            DepartmentService departmentService = new DepartmentService();
            List<Department> depList = departmentService.GetDepartment(depName);
            FeedBackMsg<List<Department>> feedBack = new FeedBackMsg<List<Department>>();
            if (depList != null && depList.Count > 0)
            {
                feedBack.Code = 1;
                feedBack.Msg = "部门列表";
                feedBack.Obj = depList;
            }
            else
            {
                feedBack.Code = 0;
                feedBack.Msg = "没有数据";
                feedBack.Obj = null;
            }
            json = ObjToJson.ToJson(feedBack);
            context.Response.Write(json);
        }

        public void GetInfo(HttpContext context) 
        {
            string json = "";
            int  depId = int.Parse(context.Request["id"]);

            DepartmentService departmentService = new DepartmentService();
            Department dep = departmentService.GetInfo(depId);
            FeedBackMsg<Department> feedBack = new FeedBackMsg<Department>();
            if (dep != null )
            {
                feedBack.Code = 1;
                feedBack.Msg = "部门列表";
                feedBack.Obj = dep;
            }
            else
            {
                feedBack.Code = 0;
                feedBack.Msg = "没有数据";
                feedBack.Obj = null;
            }
            json = ObjToJson.ToJson(feedBack);
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