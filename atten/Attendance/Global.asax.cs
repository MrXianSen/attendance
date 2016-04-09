using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Attendance.BLL.Service;

namespace Attendance
{
    public class Global : System.Web.HttpApplication
    {
        public static int ActionResult = 0;
        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            //定时执行事件
            System.Timers.Timer time = new System.Timers.Timer();
            //执行的间隔时间
            //time.Interval = 30 * 60 * 1000;
            time.Interval = 30 * 1000;
            //到达时间的时候执行事
            time.Elapsed += new System.Timers.ElapsedEventHandler(ActionService);
            time.AutoReset = true;
            //是否执行System.Timers.Timer.Elapsed事件； 
            time.Enabled = true;
        }

        void Application_End(object sender, EventArgs e)
        {
            //  在应用程序关闭时运行的代码
        }

        public void ActionService(object o, System.Timers.ElapsedEventArgs e)
        {
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;

            try
            {
                if (ActionResult == 0)
                {
                    System.Timers.Timer _time = (System.Timers.Timer)o;
                    _time.Enabled = false;
                    MonthAttendanceService.Instance.UpdateForAll();
                    _time.Enabled = true;
                    ActionResult = 1;
                }

                if (intHour == 06 && (intMinute >= 00 && intMinute <= 30))
                {
                    ActionResult = 0;
                }
            }
            catch (Exception ex)
            {
                 Response.Write("<script>alert(定时执行服务报错：" + ex.Message + ")</script>");
            }
        }

        void Application_Error(object sender, EventArgs e)
        {
            // 在出现未处理的错误时运行的代码
        }

        void Session_Start(object sender, EventArgs e)
        {
            // 在新会话启动时运行的代码

        }

        void Session_End(object sender, EventArgs e)
        {
            // 在会话结束时运行的代码。 
            // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
            // InProc 时，才会引发 Session_End 事件。如果会话模式设置为 StateServer 
            // 或 SQLServer，则不会引发该事件。

        }

    }
}