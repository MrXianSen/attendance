using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Attendance.Common.Util;
using Attendance.BLL.Service;
using Attendance.MODEL.Model;

namespace Attendance.Handler
{
    /// <summary>
    /// Login 
    /// 登录处理
    /// </summary>
    public class Login : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string funC = context.Request.QueryString["func"];
            try
            {
                switch (funC)
                { 
                    case "Login":
                        EmpLogin(context);
                        break;
                    case "Logout":
                        Logout(context);
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
        /******************************************************
         *员工登录
         ******************************************************/
        private void EmpLogin(HttpContext context)
        {
            string strType = "0";
            string strData = "";
            string empAccount = context.Request["empAccount"];
            string empPwd = DataBase.StringToMD5Hash(context.Request["pwd"]);//加密后的密码
            string remember = context.Request["remember"];  //是否记住密码
            string checkCode = context.Request["checkCode"];
            if (string.IsNullOrEmpty(empAccount) || string.IsNullOrEmpty(empPwd) || string.IsNullOrEmpty(checkCode))
            {
                strData = "登录信息不能为空";
                context.Response.Write("{Code:\"" + strType + "\",Msg:\"" + strData + "\"}");
                return;
            }
            //验证码校验
            if (context.Session["needLoginCode"] != null && context.Session["needLoginCode"].ToString() == "1")
            {
                if (!string.Equals(context.Session["CheckCode"].ToString(), context.Request["CheckCode"], StringComparison.InvariantCultureIgnoreCase))
                {
                    strData = "验证码错误！";
                    context.Response.Write("{Code:\"" + strType + "\",Msg:\"" + strData + "\"}");
                    return;
                }
            }
            //登录校验
            EmployeeInfo empInfo = EmployeeService.Instance.IsEmployeeExist(empAccount, empPwd);
            if (empInfo == null)
            {
                strData = "用户名或者密码错误";
                context.Response.Write("{Code:\"" + strType + "\",Msg:\"" + strData + "\"}");
                return;
            }
            UsrAuth.SetLogin(context.Session, empInfo);
            //记住用户，下次直接登录
            if (remember == "1")
            {
                UsrAuth.SetLoginCookie(context.Response, empInfo.EmpAccount, empPwd);
            }
            strType = "1";
            strData = "登录成功";
            context.Response.Write("{Code:\"" + strType + "\",Msg:\"" + strData + "\"}");
        }
        /******************************************************
         *退出登录
         ******************************************************/
        private void Logout(HttpContext context)
        {
            UsrAuth.LoginOut(context.Session);
            UsrAuth.RemoveLoginCookie(context.Response);
            context.Response.Write("{Code:\"1\"}");
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