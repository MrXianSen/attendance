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
    /// TimeManagement
    /// 时间管理
    /// </summary>
    public class TimeManagement : IHttpHandler,IRequiresSessionState
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
                        Update(context);
                        break;
                    case "GetInfo":
                        GetInfo(context);
                        break;
                    default:
                        break;
                }
            }
            catch(Exception e)
            {
                context.Response.ClearContent();
                context.Response.Write("{data:\"参数错误！\"}");
            }
        }
        /**********************************************************
         * 更新上班时间
         * ********************************************************/
        private void Update(HttpContext context)
        {
            int code = 0;
            string msg = "";
            string json = "";
            WorkDuration wd = new WorkDuration();
            #region 参数值赋给wd对象
            wd.NormalTime = context.Request["normalTime"];
            wd.AbsentSignInTime = context.Request["absentTimeSignIn"];
            wd.AbsentSignOutTime = context.Request["absenTimeSignOut"];
            wd.LeaveEarlyTime = context.Request["leaveEarlyTime"];
            wd.WDFloatTime = context.Request["floatTime"];
            wd.WDMonthDuration = int.Parse(context.Request["monthWorkDuration"]);
            wd.WDSignInMonth = context.Request["month"];
            #endregion

            #region 验证操作权限
            if (!UsrAuth.IsAdminister(context.Session))
            {
                code = 0;
                msg = "无‘管理员’权限";
                json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
                context.Response.Write(json);
                return;
            }
            #endregion

            int res = WorkDurationService.Instance.UpdateWorkDuration(wd);
            if (res == 0)
            {
                code = 0;
                msg = "更新失败";
                json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
            }
            else
            {
                code = 1;
                msg = "更新成功";
                json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
            }
            context.Response.Write(json);
        }
        /// *******************************************************
        /// <summary>
        /// 获取当前的工作时间设置
        /// </summary>
        /// <param name="context"></param>
        /// *******************************************************
        private void GetInfo(HttpContext context)
        { 
            int code = 0;
            string msg = "";
            string json = "";

            #region 验证管理员权限
            if (!UsrAuth.IsAdminister(context.Session))
            {
                code = 0;
                msg = "没有管理员权限";
                json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
                context.Response.Write(json);
                return;
            }
            #endregion

            WorkDuration workDuration = WorkDurationService.Instance.GetWorkDuration();
            FeedBackMsg<WorkDuration> feedBack = new FeedBackMsg<WorkDuration>();
            if (WorkDurationService.Instance != null)
            {
                feedBack.Code = 1;
                feedBack.Msg = "当前的工作时间设置";
                feedBack.Obj = workDuration;
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