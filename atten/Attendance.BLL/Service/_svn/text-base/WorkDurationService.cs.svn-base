using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Attendance.MODEL.Model;
using Attendance.DAL.DBHelper;
using Attendance.Common.Util;

namespace Attendance.BLL.Service
{
    /// <summary>
    /// 工作时长服务
    /// </summary>
    public class WorkDurationService
    {
        public static WorkDurationService Instance;
        static WorkDurationService()
        {
            Instance = new WorkDurationService();
        }
        /// ********************************************************************
        /// <summary>
        /// 更新工作时长表
        /// </summary>
        /// <param name="workDuration">WorkDuration对象</param>
        /// <returns>更新成功“1”，否则“0”</returns>
        /// ********************************************************************
        public int UpdateWorkDuration(WorkDuration wd)
        {
            //更新WorkDuration数据表中的数据
            WorkDuration workDuration = GetWorkDuration();
            int oldWorkDayDuration = workDuration.WDDayDuration;
            TimeSpan timeSpan = DateTime.Parse(wd.LeaveEarlyTime) - DateTime.Parse(wd.NormalTime);
            int newWorkDayDuration = timeSpan.Hours;
            string sql = string.Format(@"UPDATE T_WorkDuration SET F_WDMonthDuration={0}, F_WDSignInMonth='{1}', F_NormalTime='{2}', F_AbsentSignInTime='{3}', 
                                        F_AbsentSignOutTime='{4}', F_LeaveEarlyTime='{5}', F_WDFloatTime={6}, F_WDDayDurattion={7}",
                wd.WDMonthDuration, wd.WDSignInMonth.Replace("'", "''"), wd.NormalTime.Replace("'", "''"), wd.AbsentSignInTime.Replace("'", "''"), wd.AbsentSignOutTime.Replace("'", "''"), wd.LeaveEarlyTime.Replace("'", "''"), wd.WDFloatTime.Replace("'", "''"), newWorkDayDuration);
            int res = MrDBAccess.ExecuteNonQuery(sql);
            if (oldWorkDayDuration != newWorkDayDuration || wd.WDMonthDuration != workDuration.WDMonthDuration)
            {
                MonthAttendanceService.Instance.UpdateWhenDurationChange(newWorkDayDuration - oldWorkDayDuration, wd.WDMonthDuration - workDuration.WDMonthDuration, 
                    DataBase.SetInt(DateTime.Now.ToString()));
            }
            if (res == 0) return 0;
            return 1;
        }
        /**********************************************************************
         * 获取WorkDuration数据
         * *******************************************************************/
        public WorkDuration GetWorkDuration()
        {
            string sql = string.Format(@"select * from T_WorkDuration");
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            if (rows != null && rows.Count > 0)
            {
                return getWorkDuration(rows);
            }
            return null;
        }
        /*********************************************************************
         * 将Row转化为WorkDuration
         * ******************************************************************/
        private WorkDuration getWorkDuration(DataRowCollection rows)
        {
            if (rows != null && rows.Count > 0)
            {
                WorkDuration workDuration = new WorkDuration()
                {
                    WDID = int.Parse(rows[0]["F_WDID"].ToString()),
                    WDMonthDuration = int.Parse(rows[0]["F_WDMonthDuration"].ToString()),
                    WDSignInMonth = rows[0]["F_WDSignInMonth"].ToString(),
                    NormalTime = rows[0]["F_NormalTime"].ToString(),
                    AbsentSignInTime = rows[0]["F_AbsentSignInTime"].ToString(),
                    AbsentSignOutTime = rows[0]["F_AbsentSignOutTime"].ToString(),
                    LeaveEarlyTime = rows[0]["F_LeaveEarlyTime"].ToString(),
                    WDFloatTime = rows[0]["F_WDFloatTime"].ToString(),
                    WDDayDuration = int.Parse(rows[0]["F_WDDayDurattion"].ToString())
                };
                return workDuration;
            }
            return null;
        }
    }
}