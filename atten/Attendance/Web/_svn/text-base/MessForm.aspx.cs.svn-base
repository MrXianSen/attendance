using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Attendance.BLL.Service;
using Attendance.Common.Util;

namespace Attendance
{
    public partial class MessForm : System.Web.UI.Page
    {
        protected string strMessColor = "";
        protected string strMess = "";
        protected string strType = "";//1;上显示，2上左显示
        protected string strBorder = "";
        protected string strRed = "";
        protected void Page_Load(object sender, EventArgs e)
        {			// 在此处放置用户代码以初始化页面
            strMess = DataBase.ObjectToString(Request["mess"]);
            strType = DataBase.ObjectToString(Request["type"]);//1是上，2是左上
            strBorder = DataBase.ObjectToString(Request["border"]);
            strRed = DataBase.ObjectToString(Request["red"]);
            strMess = Server.UrlDecode(strMess);
            if (strRed == "1")
                strMessColor = "red";

        }
    }
}