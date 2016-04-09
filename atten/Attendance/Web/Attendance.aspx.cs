using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Attendance.BLL.Service;

namespace Attendance
{
    public partial class Attendance : System.Web.UI.Page
    {
        protected string strUsrRole = "";
        protected string strNoLogin = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UsrAuth.IsLogin(Session, Request, Response))
            {
                strNoLogin = "1";
                return;
            }
            strUsrRole = UsrAuth.GetUsrRoleID(Session);

        }
    }
}