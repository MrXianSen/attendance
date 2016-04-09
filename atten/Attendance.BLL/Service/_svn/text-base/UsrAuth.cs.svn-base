using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Attendance.MODEL.Model;
using Attendance.Common.Util;
using Attendance.DAL.DBHelper;
using Newtonsoft.Json;

namespace Attendance.BLL.Service
{
	/// <summary>
	///UsrAuth 用户认证
	/// </summary>
	public class UsrAuth
	{
        private static EmployeeService employeeService = new EmployeeService();
        /// <summary>
        /// 是否已经登录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
		public static bool IsLogin(HttpSessionState session, HttpRequest request, HttpResponse response)
		{
            //HttpContext.Current
			if (session["empID"] == null || string.IsNullOrEmpty(DataBase.ObjectToString(session["empID"])))
			{
				UserCookie userCookie = GetLoginCookie(request);
				if (userCookie != null)
				{
                    EmployeeInfo employee = employeeService.IsEmployeeExist(userCookie.UserCode, userCookie.UserPwd);
					if (employee != null)
					{
                        UsrAuth.SetLogin(session, employee);
						SetLoginCookie(response, employee.DepName, userCookie.UserPwd);
						return true;
					}
				}
				return false;
			}
			return true;
		}
		/// <summary>
		/// 登出
		/// </summary>
		/// <param name="session"></param>
        public static void LoginOut(HttpSessionState session)
		{
			session.Remove("empID");
		}
        /// <summary>
        /// 设置登录Session
        /// </summary>
        /// <param name="session"></param>
        /// <param name="employee"></param>
		public static void SetLogin(HttpSessionState session, EmployeeInfo employee)
		{
			session["empID"] = employee.EmpID;
            session["empInfo"] = employee;
			session.Timeout = 60 * 24;
		}
        /// <summary>
        /// 删除Cookie
        /// </summary>
        /// <param name="response"></param>
        public static void RemoveLoginCookie(HttpResponse response)
        {
            if (response.Cookies["usrCode"] != null)
                response.Cookies.Remove("userCode");
        }
        /// <summary>
        /// 设置登录Cookie
        /// </summary>
        /// <param name="response"></param>
        /// <param name="code"></param>
        /// <param name="pwd"></param>
        public static void SetLoginCookie(HttpResponse response, string code, string pwd)
        {
            UserCookie userCookie = new UserCookie() { UserCode = code, UserPwd = pwd };
            HttpCookie cookie = new HttpCookie("usrCode") { Expires = DateTime.Now.AddDays(365 * 100), Value = DataBase.SimpleEncrypt(JsonConvert.SerializeObject(userCookie)) };
            if (response.Cookies["usrCode"] == null)
                response.Cookies.Add(cookie);
            else
                response.Cookies.Set(cookie);
        }
        /// <summary>
        /// 获取登录Cookie
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static UserCookie GetLoginCookie(HttpRequest request)
        {
            if (request.Cookies["usrCode"] == null)
                return null;
            HttpCookie cookie = request.Cookies["usrCode"];
            return JsonConvert.DeserializeObject<UserCookie>(DataBase.SimpleDecrypt(cookie.Value));
        }
		public static EmployeeInfo GetempInfo(HttpSessionState session, bool reData)
		{
            EmployeeInfo employee = null;
			if (reData)
			{
                employee = employeeService.GetEmployee(DataBase.GetInt(GetempID(session)));
				if (employee != null)
				{
                    UsrAuth.SetLogin(session, employee);
                    return employee;
				}
				return null;
			}
            employee = session["empInfo"] as EmployeeInfo;
            if (employee == null)
				return null;
            return employee;
		}
		public static string GetUsrName(HttpSessionState session)
		{
            EmployeeInfo employee = session["empInfo"] as EmployeeInfo;
            if (employee == null)
				return null;
            return employee.EmpName;
		}
		public static string GetUsrCode(HttpSessionState session)
		{
            EmployeeInfo employee = session["empInfo"] as EmployeeInfo;
            if (employee == null)
				return null;
            return employee.DepName;
		}
		public static string GetempID(HttpSessionState session)
		{
			return DataBase.ObjectToString(session["empID"]);
		}
        public static string GetempName(HttpSessionState session)
        {
            EmployeeInfo employee = session["empInfo"] as EmployeeInfo;
            if (employee == null)
            {
                return "-1";
            }
            return DataBase.ObjectToString(employee.EmpName);
        }
        public static string GetdepID(HttpSessionState session)
        {
            EmployeeInfo employee = session["empInfo"] as EmployeeInfo;
            if (employee == null)
            {
                return "-1";
            }
            return DataBase.ObjectToString(employee.DepID);
        }
        public static string GetdepName(HttpSessionState session)
        {
            EmployeeInfo employee = session["empInfo"] as EmployeeInfo;
            if (employee == null)
            {
                return "-1";
            }
            return DataBase.ObjectToString(employee.DepName);
        }
		public static string GetUsrRoleID(HttpSessionState session)
		{
            EmployeeInfo employee = session["empInfo"] as EmployeeInfo;
            if (employee == null)
            {
                return "-1";
            }
            return employee.EmpRole;
		}
		public static bool IsAdminister(HttpSessionState session)
		{
            if (GetUsrRoleID(session).IndexOf(",4,") != -1)
                return true;
            return false;
		}
        public static bool IsTopManager(HttpSessionState session)
        {
            if (GetUsrRoleID(session).IndexOf(",3,") != -1)
                return true;
            return false;
        }
		public static bool IsDepManager(HttpSessionState session)
		{
			//return (GetUsrRoleID(session) == "2");
            if (GetUsrRoleID(session).IndexOf(",2,") != -1)
                return true;
            return false;
		}
	}
	public class UserCookie
	{
		public string UserCode { get; set; }
		public string UserPwd { get; set; }
	}
}