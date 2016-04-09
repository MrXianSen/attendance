using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.MODEL.Model
{
    /// <summary>
    /// 工作时长信息
    /// </summary>
    public class WorkDuration
    {
        private int _WDID;                      //工作时长ID
        private int _WDMonthDuration;           //月工作总天长
        private string _WDSignInMonth;             //签到月份
        private string _NormalTime;      //时间点1
        private string _AbsentSignInTime;      //时间点2
        private string _AbsentSignOutTime;      //时间点3
        private string _AbsentLeaveEarlyTime;      //时间点4 
        private string _WDCreateDate;         //创建时间
        private string _WDFloatTime;          //浮动时间
        private int _WDDayDuration;           //每天标准工作时间

        public int WDDayDuration
        {
            get { return _WDDayDuration; }
            set { _WDDayDuration = value; }
        }

        

        /*说明：F_WDTimeQuanturnA，F_WDTimeQuanturnB，F_WDTimeQuanturnC，F_WDTimeQuanturnD对应的是9:00~10:00为迟到，
	    17:00~18:00为早退,分别存的就是9点、10点、17点、18点这个四数字*/
        /// <summary>
        /// 工作时长ID
        /// </summary>
        public int WDID
        {
            get { return _WDID; }
            set { _WDID = value; }
        }

        /// <summary>
        /// 月工作总天长
        /// </summary>
        public int WDMonthDuration
        {
            get { return _WDMonthDuration; }
            set { _WDMonthDuration = value; }
        }

        /// <summary>
        /// 签到月份
        /// </summary>
        public string WDSignInMonth
        {
            get { return _WDSignInMonth; }
            set { _WDSignInMonth = value; }
        }

        /// <summary>
        /// 时间点1，格式:21:00:00
        /// </summary>
        public string NormalTime
        {
            get { return _NormalTime; }
            set { _NormalTime = value; }
        }

        /// <summary>
        /// 时间点2
        /// </summary>
        public string AbsentSignInTime
        {
            get { return _AbsentSignInTime; }
            set { _AbsentSignInTime = value; }
        }

        /// <summary>
        /// 时间点3
        /// </summary>
        public string AbsentSignOutTime
        {
            get { return _AbsentSignOutTime; }
            set { _AbsentSignOutTime = value; }
        }

        /// <summary>
        /// 时间点4
        /// </summary>
        public string LeaveEarlyTime
        {
            get { return _AbsentLeaveEarlyTime; }
            set { _AbsentLeaveEarlyTime = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string WDCreateDate
        {
            get { return _WDCreateDate; }
            set { _WDCreateDate = value; }
        }

        public string WDFloatTime
        {
            get { return _WDFloatTime; }
            set { _WDFloatTime = value; }
        }
    }
}