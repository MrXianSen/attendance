using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.IO;
using Attendance.BLL.Service;

namespace Attendance
{
    /// <summary>
    /// 登录
    /// </summary>
    public partial class Login : System.Web.UI.Page
    {
        protected string strNeedLoginCode = ConfigurationManager.AppSettings["IdentifyingCode"];
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["needLoginCode"] = strNeedLoginCode;
            string ii = Session["needLoginCode"].ToString();
            if (UsrAuth.IsLogin(Session, Request, Response))
                Response.Redirect("Home.aspx");
        }

    }
}