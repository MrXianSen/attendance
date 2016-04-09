using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Attendance.MODEL.Model;
using Attendance.Common.Util;
using Attendance.DAL.DBHelper;

namespace Attendance.BLL.Service
{
    /*****************************************************************************
    /// 考勤统计信息
    /// 统计一个月每个员工的考勤信息
    /// 每天定时运行，不需要用户操作
    * ***************************************************************************/
    public class MonthAttendanceService
    {

        public static MonthAttendanceService Instance;
        static MonthAttendanceService()
        {
            Instance = new MonthAttendanceService();
        }
        /// <summary>
        /// 更新考勤统计数据表
        /// </summary>
        /// <returns>成功“1”，否则“0”</returns>
        public void UpdateForAll()
        {
            string sql = "";
            //所有没有统计的签到信息
            List<CheckInInfo> checkInInfoListWithNoCal = CheckInInfoService.Instance.GetCheckInInfoListNoCalculate();
            
            if (checkInInfoListWithNoCal != null && checkInInfoListWithNoCal.Count > 0)
            {
                for (int i = 0; i < checkInInfoListWithNoCal.Count; i++)
                {
                    //统计月份
                    int asMonth = DataBase.SetInt(checkInInfoListWithNoCal[i].CISignInDate);
                    int empID = checkInInfoListWithNoCal[i].EmpID;
                    //1. 判断当月该员工是否已经存在统计记录
                    if (!isMonthAttendanceExist(asMonth, empID))
                    {
                        #region -----------------------------------1. 封装插入语句
                        WorkDuration workDuration = WorkDurationService.Instance.GetWorkDuration();
                        sql += string.Format(@"INSERT INTO T_AttendanceStatistics(F_EmpID, 
                                                                                  F_EmpName,
                                                                                  F_DepID,
                                                                                  F_DepName,
                                                                                  F_ASMonth,
                                                                                  F_ASStandardDuration,
                                                                                  F_ASRealityDuration,
                                                                                  F_ASLateNumber,
                                                                                  F_ASLeaveEavlyNumber,
                                                                                  F_ASAbsenteeismNumber,
                                                                                  F_ASNormalNumber,
                                                                                  F_ASCreateDate) {0}", makeInsertSql(checkInInfoListWithNoCal[i], workDuration, asMonth));
                        #endregion
                    }
                    else
                    {
                        #region -----------------------------------2. 更新语句
                        int normalCount = 0, lateCount = 0, absentCount = 0, leaveEarlyCount = 0;
                        if (checkInInfoListWithNoCal[i].CIIsNormal) normalCount = 1;
                        if (checkInInfoListWithNoCal[i].CIIsLeaveEavly) leaveEarlyCount = 1;
                        if (checkInInfoListWithNoCal[i].CIIsLate) lateCount = 1;
                        if (checkInInfoListWithNoCal[i].CIIsAbsenteeism) absentCount = 1;
                        sql += string.Format(@"UPDATE T_AttendanceStatistics {0}", makeUpdateSql(checkInInfoListWithNoCal[i],normalCount, lateCount, absentCount, leaveEarlyCount));
                        #endregion
                    }
                }
                try
                {
                    MrDBAccess.BeginTransaction();
                    MrDBAccess.ExecuteNonQuery(sql);
                    MrDBAccess.CommitTransaction();
                    //将统计过的签到信息设置为已经被统计过
                    CheckInInfoService.Instance.SetCheckInInfoIsCalculate(checkInInfoListWithNoCal);
                }
                catch (Exception e)
                { 
                    //事务回滚
                    MrDBAccess.RollbackTransaction();
                    //将事务异常信息存日志
                    //TODO
                }

            }
        }
        /// *********************************************************************
        /// <summary>
        /// 描述：为补签的人实时更新统计信息
        /// </summary>
        /// <param name="asMonth">补签月份</param>
        /// <param name="empID">被补签员工的ID</param>
        /// *********************************************************************
        public void UpdateForAppendSignEmp(List<CheckInInfo> list, bool check, int normalCount=0, int lateCoutn=0, int absentCount=0,int leaveEarlyCount=0)
        {
            //MODIFY
            string sql = "";
            WorkDuration workDuration = WorkDurationService.Instance.GetWorkDuration();
            
            //判断被补签人是否已经被统计过
            //统计表中
            int iLengthList = list.Count;
            for (int i = 0; i < iLengthList; i++)
            {
                if (!isMonthAttendanceExist(DataBase.SetInt(list[i].CISignInDate), list[i].EmpID))
                {
                    sql += string.Format(@"INSERT INTO T_AttendanceStatistics(F_EmpID, 
                                                                                  F_EmpName,
                                                                                  F_DepID,
                                                                                  F_DepName,
                                                                                  F_ASMonth,
                                                                                  F_ASStandardDuration,
                                                                                  F_ASRealityDuration,
                                                                                  F_ASLateNumber,
                                                                                  F_ASLeaveEavlyNumber,
                                                                                  F_ASAbsenteeismNumber,
                                                                                  F_ASNormalNumber,
                                                                                  F_ASCreateDate) {0}", makeInsertSql(list[i], workDuration, DataBase.SetInt(list[i].CISignInDate)));
                }
                else
                {
                    if (check)
                    {
                        if (list[i].CIIsLeaveEavly) leaveEarlyCount = 1;
                        if (list[i].CIIsNormal) normalCount = 1;
                        if (list[i].CIIsLate) lateCoutn = 1;
                        if (list[i].CIIsAbsenteeism) absentCount = 1;
                    }
                    sql += string.Format(@"UPDATE T_AttendanceStatistics {0}", makeUpdateSql(list[i], normalCount, lateCoutn, absentCount, leaveEarlyCount));
                }
            }
            try
            {
                MrDBAccess.BeginTransaction();
                MrDBAccess.ExecuteNonQuery(sql);
                MrDBAccess.CommitTransaction();
                //将统计过的签到信息设置为已经被统计
                CheckInInfoService.Instance.SetCheckInInfoIsCalculate(list);
            }
            catch (Exception e)
            {
                MrDBAccess.RollbackTransaction();
            }
        }
        private string makeInsertSql(CheckInInfo checkInInfo, WorkDuration workDuration, int asMonth)
        {
            string sql = "";
            int normal = 0, late = 0, leaveEarly = 0, absent = 0;
            if (checkInInfo.CIIsNormal)//正常
                normal = 1;
            if (checkInInfo.CIIsAbsenteeism)//缺席
                absent = 1;
            if (checkInInfo.CIIsLate)//迟到
                late = 1;
            if (checkInInfo.CIIsLeaveEavly)//早退
                leaveEarly = 1;
            sql = string.Format(@"VALUES({0},'{1}',{2},'{3}',{4},{5},{6},{7},{8},{9},{10},'{11}');",
                                                                      checkInInfo.EmpID,
                                                                      checkInInfo.EmpName,
                                                                      checkInInfo.DepID,
                                                                      checkInInfo.DepName,
                                                                      asMonth,
                                                                      workDuration.WDMonthDuration * workDuration.WDDayDuration,
                                                                      checkInInfo.CIRealityWorkDuration,
                                                                      late,//迟到次数
                                                                      leaveEarly,//早退次数
                                                                      absent,//缺席次数
                                                                      normal,//正常次数
                                                                      DateTime.Now.ToString("yyyy-MM-dd"));

            return sql;
        }
        private string makeUpdateSql(CheckInInfo checkInInfo, int normalCount, int lateCoutn, int absentCount, int leaveEarlyCount)
        {
            string sql = string.Format(@"SET F_ASRealityDuration=F_ASRealityDuration+({0}),
                                        F_ASNormalNumber=F_ASNormalNumber+({1}),F_ASLeaveEavlyNumber=F_ASLeaveEavlyNumber+({2}),
                                        F_ASLateNumber=F_ASLateNumber+({3}),F_ASAbsenteeismNumber=F_ASAbsenteeismNumber+({4}) WHERE F_EmpID={5}",
                                        checkInInfo.CIRealityWorkDuration, normalCount, leaveEarlyCount, lateCoutn, absentCount, checkInInfo.EmpID);
            return sql;
        }
        /// *********************************************************************
        /// <summary>
        /// 描述：当系统管理员修改了某个月份的工作时间时，将当月统计的所有信息
        /// </summary>
        /// *********************************************************************
        public void UpdateWhenDurationChange(int changeTime, int changeDay, int asMonth)
        {
            //MODIFY
            string sql = "";
            WorkDuration workDuration = WorkDurationService.Instance.GetWorkDuration();
            sql = string.Format(@"UPDATE T_AttendanceStatistics SET");
            if (changeTime != 0)
                sql += string.Format(@" F_ASStandardDuration=F_ASStandardDuration+({0})", workDuration.WDMonthDuration * changeTime);
            if (changeDay != 0)
                sql += string.Format(@" F_ASStandardDuration=F_ASStandardDuration+({0})", workDuration.WDDayDuration * changeDay);
            sql += string.Format(@" WHERE F_ASMonth={0}", asMonth);
            try
            {
                MrDBAccess.ExecuteNonQuery(sql);
            }
            catch (Exception e)
            {/*TODO:*/}
        }
        /// *********************************************************************
        /// <summary>
        /// 描述：判断该员工当月是否已经统计过
        /// </summary>
        /// <param name="asMonth">员工签到的月份</param>
        /// <param name="empID">员工ID</param>
        /// <returns>
        /// 返回：存在;true
        ///       不存在:false
        /// </returns>
        /// *********************************************************************
        private bool isMonthAttendanceExist(int asMonth, int empID)
        {
            string sql = string.Format(@"SELECT * FROM T_AttendanceStatistics WHERE F_ASMonth={0} AND F_EmpID={1}", asMonth, empID);
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            if (rows.Count > 0) return true;
            return false;
        }
        #region ---------------------------获取考勤月统计数据
        /*************************************************************************
         * 描述：获取月考勤信息
         * 参数：empID 员工ID
         *       empName 员工姓名
         *       date 查询日期
         *       rate 出勤率，全勤1，非全勤0，所有null
         *       depID 部门ID
         * **********************************************************************/
        /*获取部门员工考勤月统计数据*/
        public List<MonthAttendance> GetAttendanceStatistics(string date, string depID = null, string empName = null, string rate = null)
        {
            string sql = string.Format(@"SELECT * FROM T_AttendanceStatistics WHERE F_ASMonth={0} ", date);
            if (!string.IsNullOrEmpty(depID))
            {
                sql += string.Format(@"AND F_DepID={0} ", depID);
            }
            if (!string.IsNullOrEmpty(empName))
                sql += string.Format(@"AND F_EmpName LIKE '%{0}%' ", empName);
            if (!string.IsNullOrEmpty(rate) && rate.Equals("1"))
                sql += string.Format(@"AND F_ASLateNumber=0 AND F_ASLeaveEavlyNumber=0 AND F_ASAbsenteeismNumber=0 ");
            else if (!string.IsNullOrEmpty(rate) && rate.Equals("0"))
                sql += string.Format(@"AND F_ASLateNumber!=0 OR F_ASLeaveEavlyNumber!=0 OR F_ASAbsenteeismNumber!=0 ");
            sql += string.Format(@"ORDER BY F_ASMonth");
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            if (rows != null && rows.Count > 0)
                return getAttendanceStatistics(rows);
            return null;
        }
        /*当前员工的考勤信息*/
        public List<MonthAttendance> GetAttendanceStatistics(string empID, string date, string rate)
        {
            string sql = string.Format(@"SELECT * FROM T_AttendanceStatistics WHERE F_ASMonth={0} AND F_EmpID={1} ", date, empID);
            if (!string.IsNullOrEmpty(rate) && rate.Equals("0"))
                sql += "AND F_ASStandardDuration > F_ASRealityDuration";
            else if (!string.IsNullOrEmpty(rate) && rate.Equals("1"))
                sql += "AND F_ASStandardDuration = F_ASRealityDuration";
            sql += string.Format(@"ORDER BY F_ASMonth");
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            if (rows != null && rows.Count > 0)
                return getAttendanceStatistics(rows);
            return null;
        }
        #endregion
        public void Delete(int empID)
        {
            string sql = string.Format(@"DELETE FROM T_AttendanceStatistics WHERE F_EmpID={0}", empID);
            MrDBAccess.ExecuteNonQuery(sql);
        }
        /*************************************************************************
         * 将DataRowCollection转化为List
         * **********************************************************************/
        private List<MonthAttendance> getAttendanceStatistics(DataRowCollection rows)
        {
            if (rows != null && rows.Count > 0)
            {
                int iRowLength = rows.Count;
                List<MonthAttendance> list = new List<MonthAttendance>();
                for (int i = 0; i < iRowLength; i++)
                {
                    MonthAttendance monthAttendance = new MonthAttendance()
                    {
                        ASID = int.Parse(rows[i]["F_ASID"].ToString()),
                        EmpID = rows[i]["F_EmpID"].ToString(),
                        EmpName = rows[i]["F_EmpName"].ToString(),
                        DepID = rows[i]["F_DepID"].ToString(),
                        DepName = rows[i]["F_DepName"].ToString(),
                        ASMonth = int.Parse(rows[i]["F_ASMonth"].ToString()),
                        ASStandardDuration = int.Parse(rows[i]["F_ASStandardDuration"].ToString()),
                        ASRealityDuration = int.Parse(rows[i]["F_ASRealityDuration"].ToString()),
                        ASLateNumber = int.Parse(rows[i]["F_ASLateNumber"].ToString()),
                        ASLeaveEavlyNumber = int.Parse(rows[i]["F_ASLeaveEavlyNumber"].ToString()),
                        ASAbsenteeismNumber = int.Parse(rows[i]["F_ASAbsenteeismNumber"].ToString()),
                        ASNormalNumber = int.Parse(rows[i]["F_ASNormalNumber"].ToString()),
                        ASCreateDate = DateTime.Parse(rows[i]["F_ASCreateDate"].ToString())
                    };
                    list.Add(monthAttendance);
                }
                return list;
            }
            return null;
        }
    }
}
