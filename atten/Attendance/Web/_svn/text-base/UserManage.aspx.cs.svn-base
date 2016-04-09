using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Attendance.BLL.Service;

namespace Attendance
{
    /// <summary>
    /// 员工管理
    /// </summary>
    public partial class UserManage : System.Web.UI.Page
    {
        protected string strUsrRole = "";
        protected string strNoLogin = "";
        protected string isAdmin = "";
        protected string isDepManage = "";
        protected string isTopManage = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UsrAuth.IsLogin(Session, Request, Response))
            {
                strNoLogin = "1";
                return;
            }
            isAdmin = UsrAuth.IsAdminister(Session) ? "1" : "0";
            isDepManage = UsrAuth.IsDepManager(Session) ? "1" : "0";
            isTopManage = UsrAuth.IsTopManager(Session) ? "1" : "0";

        }
    }
}