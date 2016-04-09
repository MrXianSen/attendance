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
    /// Attendance
    /// 考勤处理
    /// </summary>
    public class Attendance : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //登录认证
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
                    case "GetSignStatus":
                        GetSignStatus(context);
                        break;
                    case "Sign":
                        Sign(context);
                        break;
                    case "GetDetailList":
                        GetDetailList(context);
                        break;
                    case "UpdateSupplement":
                        UpdateSupplement(context);
                        break;
                    case "GetMonthAttendanceList":
                        GetMonthAttendanceList(context);
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
        #region --------------------------------------1. 签到
        /********************************************************
         * 获取用户签到状态
         * date：查询日期 可以为null
         * rate：出勤率，全勤1，非全勤0，所有null
         * ******************************************************/
        private void GetSignStatus(HttpContext context)
        {
            //登录账号的ID
            string empID = UsrAuth.GetempID(context.Session);
            FeedBackMsg<CheckInInfo> feedBackMsg = new FeedBackMsg<CheckInInfo>();
            //获取当前的考勤信息
            CheckInInfo checkInInfo = CheckInInfoService.Instance.GetCurrentCheckInInfo(int.Parse(empID));
            if (checkInInfo != null)
            {
                //已经签过到
                feedBackMsg.Code = 1;
                feedBackMsg.Msg = "已经签到";
                feedBackMsg.Obj = checkInInfo;
            }
            else
            {
                //还没有签到
                feedBackMsg.Code = 1;
                feedBackMsg.Msg = "还没签到";
                feedBackMsg.Obj = null;
            }
            string json = ObjToJson.ToJson(feedBackMsg);
            context.Response.Write(json);
        }
        /********************************************************
         * 签到：插入一条考勤信息，初始化
         *       empID          员工ID
         *       empName        员工姓名
         *       depID          部门ID     
         *       depName        部门名称
         *       signInDate     签到时间
         *       isLate         是否迟到
         *       
         * 签退：修改考勤数据
         *       checkInInfoID  考勤信息ID
         *       empID          员工ID
         *       empName        员工姓名
         *       depID          部门ID     
         *       depName        部门名称
         *       signOutDate    签退时间
         * ******************************************************/
        private void Sign(HttpContext context)
        {
            int code = 0;
            string msg = "";
            int checkInInfoID = 0;
            string strCheckInInfoID = context.Request["id"];
            if (!string.IsNullOrEmpty(strCheckInInfoID))
            {
                checkInInfoID = int.Parse(strCheckInInfoID);
            }
            string empID = UsrAuth.GetempID(context.Session);
            string empName = UsrAuth.GetempName(context.Session);
            string depID = UsrAuth.GetdepID(context.Session);
            string depName = UsrAuth.GetdepName(context.Session);
            if (checkInInfoID<=0)
            {
                //签到
                string signInDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                int res = CheckInInfoService.Instance.UpdateCheckInInfo(int.Parse(empID), empName, int.Parse(depID), 
                    depName, signInDate);
                if (res != 1) { code = 0; msg = "签到失败"; }
                else { code = 1; msg = "签到成功"; }
            }
            else
            {
                //签退
                string signOutDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                int res = CheckInInfoService.Instance.UpdateCheckInInfo(int.Parse(empID), empName, int.Parse(depID), 
                    depName, signOutDate, checkInInfoID);
                if (res != 1) { code = 0; msg = "签退失败"; }
                else { code = 1; msg = "签退成功"; }
            }
            string json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
            context.Response.Write(json);
        }
        #endregion
        #region -----------------------------------------2. 考勤详细记录
        /********************************************************
         * 获取签到信息
         * date：查询日期 可以为null
         * rate：出勤率，全勤1，非全勤0，所有null
         * empName：员工姓名
         * ******************************************************/
        private void GetDetailList(HttpContext context)
        {
            string json = "";
            //验证操作权限
            string empID = UsrAuth.GetempID(context.Session);
            string searchStartDate = context.Request["searchStartDate"];
            string searchEndDate = context.Request["searchEndDate"];
            string searchRate = context.Request["searchRate"];
            string searchName = context.Request["searchName"];
            //MODIFY
            string isSelf = context.Request["isSelf"];
            int searchEmpID = int.Parse(context.Request["searchEmpID"]);

            FeedBackMsg<List<CheckInInfo>> feedBack = new FeedBackMsg<List<CheckInInfo>>();
            
            if (string.IsNullOrEmpty(searchStartDate)) { searchStartDate = null; }
            if (string.IsNullOrEmpty(searchRate)) { searchRate = null; }
            if (string.IsNullOrEmpty(searchName)) { searchName = null; }
            string depID = UsrAuth.GetdepID(context.Session);
            string depName = UsrAuth.GetdepName(context.Session);
            if (searchEmpID > 0) 
            {
                List<CheckInInfo> checkInInfoList = CheckInInfoService.Instance.GetCheckInInfoList(searchEmpID.ToString(), searchStartDate, searchEndDate, searchRate);
                feedBack.Code = 1;
                feedBack.Msg = "员工签到信息";//获取某个员工记录
                feedBack.Obj = checkInInfoList;
            }
            else
            {
                if (!string.IsNullOrEmpty(isSelf) && isSelf.Equals("1"))    //获取登录人记录
                {
                    List<CheckInInfo> checkInInfoList = CheckInInfoService.Instance.GetCheckInInfoList(empID, searchStartDate, searchEndDate, searchRate);
                    feedBack.Code = 1;
                    feedBack.Msg = "我的签到信息";
                    feedBack.Obj = checkInInfoList;
                }
                else
                {
                    if (UsrAuth.IsAdminister(context.Session) || UsrAuth.IsTopManager(context.Session))
                    {
                        //总经理或者管理员
                        List<CheckInInfo> allCheckInInfoList = CheckInInfoService.Instance.GetCheckInInfoList(0, searchName, searchStartDate, searchEndDate, searchRate);
                        feedBack.Code = 1;
                        feedBack.Msg = "所有员工的签到信息";
                        feedBack.Obj = allCheckInInfoList;
                    }
                    else if (UsrAuth.IsDepManager(context.Session))
                    {
                        //部门经理
                        List<CheckInInfo> allCheckInInfoList = CheckInInfoService.Instance.GetCheckInInfoList(int.Parse(depID), searchName, searchStartDate, searchEndDate, searchRate);
                        feedBack.Code = 1;
                        feedBack.Msg = "部门" + depName + "员工列表";
                        feedBack.Obj = allCheckInInfoList;
                    }
                    else
                    {
                        feedBack.Code = 0;
                        feedBack.Msg = "无相应权限";
                        feedBack.Obj = null;
                    }
                }
            }
            json = ObjToJson.ToJson(feedBack);
            context.Response.Write(json);
        }
        #endregion
        #region -----------------------------------3. 月考勤记录
        /********************************************************
         * 描述：获取当前登录账户的月签到统计
         * 参数：month 查询月份
         *       depName 部门名称
         *       empName 员工姓名
         *       rate    出勤率，全勤1，非全勤0，所有null
         * ******************************************************/
        //月考勤记录查询
        private void GetMonthAttendanceList(HttpContext context)
        {
            string empID = UsrAuth.GetempID(context.Session);
            string searchMonth = context.Request["searchMonth"];
            string searchRate = context.Request["searchRate"];
            string searchEmpName = context.Request["searchName"];
            //MODIFY
            string isSelf = context.Request["isSelf"];
            if(string.IsNullOrEmpty(searchEmpName)) {searchEmpName = null;}
            if(string.IsNullOrEmpty(searchRate)){searchRate = null;}
            string depID = "";
            List<MonthAttendance> list = new List<MonthAttendance>();
            FeedBackMsg<List<MonthAttendance>> feedBack = new FeedBackMsg<List<MonthAttendance>>();
            if (!string.IsNullOrEmpty(isSelf) && isSelf.Equals("1"))
            {
                list = MonthAttendanceService.Instance.GetAttendanceStatistics(empID, searchMonth, searchRate);
            }
            else
            {
                if (UsrAuth.IsTopManager(context.Session) || UsrAuth.IsAdminister(context.Session))
                {
                    depID = null;
                    list = MonthAttendanceService.Instance.GetAttendanceStatistics(searchMonth, depID, searchEmpName, searchRate);
                }
                else if (UsrAuth.IsDepManager(context.Session))
                {
                    //部门经理
                    depID = UsrAuth.GetdepID(context.Session);
                    list = MonthAttendanceService.Instance.GetAttendanceStatistics(searchMonth, depID,searchEmpName, searchRate);
                }
            }
            if (list != null && list.Count > 0)
            {
                feedBack.Code = 1;
                feedBack.Msg = "月考勤信息";
                feedBack.Obj = list;
            }
            else
            {
                feedBack.Code = 1;
                feedBack.Msg = "沒有数据";
                feedBack.Obj = null;
            }
            string json = ObjToJson.ToJson(feedBack);
            context.Response.Write(json);
        }
        
        #endregion
        #region -------------------------------------4. 补签
        /**********************************************************
         * 补签到
         * 管理员权限
         * ********************************************************/
        private void UpdateSupplement(HttpContext context)
        {
            int code = 0;
            string msg = "";
            string json = "";
            //验证操作权限
            if (!UsrAuth.IsAdminister(context.Session))
            {//无管理员权限
                code = 0;
                msg = "无‘管理员’权限";
                json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
                context.Response.Write(json);
                return;
            }
            string checkInInfoID = context.Request["id"];
            string currentEmpID = UsrAuth.GetempID(context.Session);
            string currentEmpName = UsrAuth.GetempName(context.Session);
            string empID = context.Request["empID"];
            string empName = context.Request["empName"];
            string depID = context.Request["depID"];
            string depName = context.Request["depName"];
            string date = context.Request["appendSignDate"];
            string signInTime = context.Request["signInTime"];
            string signOutTime = context.Request["signOutTime"];
            string note = context.Request["ciNote"];
            string allSignInOrOut = context.Request["allSignInOrOut"];
            string signInDate = DateTime.Parse(date + " " + signInTime).ToString();
            string signOutDate = DateTime.Parse(date + " " + signOutTime).ToString();
            if (string.IsNullOrEmpty(note))
            {
                code = 0;
                msg = "补签说明不能为空";
                json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
                context.Response.Write(json);
                return;
            }
            if (!string.IsNullOrEmpty(allSignInOrOut) && allSignInOrOut.Equals("1")) //给所有人补签到补签退
            {
                int res = CheckInInfoService.Instance.UpdateCheckInInfoBySystem(date, signInTime, 
                    signOutTime, note, int.Parse(currentEmpID), currentEmpName, int.Parse(empID));
                if (res != 1) { code = 0; msg = "补签失败"; }
                else { code = 1; msg = "补签成功"; }
            }
            else
            {
                if (string.IsNullOrEmpty(checkInInfoID))
                {
                    //补签到
                    if (CheckInInfoService.Instance.hasSignIn(int.Parse(empID), date))
                    {
                        code = 0; msg = "员工已经签过到";
                        json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
                        context.Response.Write(json);
                        return;
                    }
                    int res = CheckInInfoService.Instance.UpdateCheckInInfoBySystem(empID, empName, int.Parse(depID), 
                        depName, signInDate, note, currentEmpID, currentEmpName);
                    if (res != 1) { code = 0; msg = "补签到失败"; }
                    else { code = 1; msg = "补签到成功"; }
                }
                else
                {
                    //补签退
                    if (CheckInInfoService.Instance.hasSingOut(int.Parse(empID), date))
                    {
                        code = 0; msg = "员工已经签过退";
                        json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
                        context.Response.Write(json);
                        return;
                    }
                    //2. 补签到正常
                    int res = CheckInInfoService.Instance.UpdateCheckInInfoBySystem(empID, empName, int.Parse(depID), depName, 
                        signOutDate, note, currentEmpID, currentEmpName, int.Parse(checkInInfoID));
                    if (res != 1) { code = 0; msg = "补签退失败"; }
                    else { code = 1; msg = "补签退成功"; }
                }
            }
            json = "{\"Code\":\"" + code + "\",\"Msg\":\"" + msg + "\"}";
            context.Response.Write(json);
        }

        private void GetInfo(HttpContext context)
        {
            int selectedCheckInInfoID = int.Parse(context.Request["id"]);

            CheckInInfo checkInInfo = CheckInInfoService.Instance.GetCheckInInfo(selectedCheckInInfoID);

            FeedBackMsg<CheckInInfo> feedBack = new FeedBackMsg<CheckInInfo>()
            {
                Code = 1,
                Msg = "选中的签到项的具体信息",
                Obj = checkInInfo
            };
            string json = ObjToJson.ToJson(feedBack);
            context.Response.Write(json);
        }
        #endregion
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}