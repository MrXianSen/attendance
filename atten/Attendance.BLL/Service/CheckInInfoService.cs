using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Attendance.MODEL.Model;
using Attendance.DAL.DBHelper;
using Attendance.Common.Util;
using Newtonsoft.Json;

namespace Attendance.BLL.Service
{
    /// <summary>
    /// 员工出勤服务
    /// </summary>
    public class CheckInInfoService
    {
        public static CheckInInfoService Instance;
        static CheckInInfoService()
        {
            Instance = new CheckInInfoService();
        }

        #region ----------------------------------1. 更新CheckInInfo
        /*****************************************************************
         * 功能：正常情况下
         *       签到
         *       签退
         * 参数：checkInInfoID      被操作的考勤信息的ID（可选参数）
         *       empID              员工ID
         *       empName            员工姓名
         *       depID              员工所属部门
         *       depName            员工所属部门名称
         *       date               签到签退时间
         * 返回：成功“1”，失败“0”
         * **************************************************************/
        public int UpdateCheckInInfo(int empID, string empName, int depID, string depName, string date, int checkInInfoID = 0)
        {
            //MODIFY
            string sql = "";
            /*是否有签到记录*/
            string checkSql = string.Format(@"select * from T_CheckingInInfo where F_EmpID={0} and (F_CISignInDate>'{1}' and F_CISignInDate<'{2}') ",
                empID, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(checkSql).Tables[0].Rows;
            WorkDuration workDuration = WorkDurationService.Instance.GetWorkDuration();
            /*1. checkInInfoID为0时，添加（签到）*/
            if (checkInInfoID == 0)
            {
                //已经签过到，退出，返回0
                if (rows != null && rows.Count > 0) return 0;
                //签到时间
                DateTime signInTime = DateTime.Parse(date);
                sql = string.Format(@"Insert into T_CheckingInInfo(
                                                                F_EmpID,
                                                                F_EmpName,
                                                                F_DepID,
                                                                F_DepName,
                                                                F_CISignInDate,
                                                                F_CIRealityWorkDuration,
                                                                F_AppendSignInPersonID,
                                                                F_AppendSignInPersonName,
                                                                F_AppendSignInPersonNote,
                                                                F_CIIsLate,
                                                                F_CIIsLeaveEarvly,
                                                                F_CIIsAbsenteeism,
                                                                F_CIIsNormal,
                                                                F_CICreateDate,
                                                                F_CIIsSignIn,
                                                                F_CIIsSignOut,
                                                                F_CIIsCalculate,
                                                                F_Date
                                                ) 
                                                values({0},'{1}',{2},'{3}','{4}',{5},{6},'{7}','{8}',{9},{10},{11},{12},'{13}',{14},{15},{16}, '{17}')",
                                    empID,
                                    empName,
                                    depID,
                                    depName,
                                    signInTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                    0,
                                    0,
                                    "",
                                    "",
                                    "0 ",//F_CIIsLate
                                    "0",//F_CIIsLeaveEavly
                                    "0",//F_CIIsAbsenteeism
                                    "0",//F_CIIsNormal
                                    DateTime.Now.ToString("yyyy-MM-dd"),
                                    "1",
                                    "0",
                                    "0",
                                                    signInTime.ToString("yyyy-MM-dd"));
            }
            /*2. checkInInfoID不为0时，修改（签退）*/
            else
            {
                bool isLate = false;
                bool isAbsent = false;
                bool isLeaveEarly = false;
                bool isNormal = false;
                //还没签到，不能进行签退，正常情况下是不会执行句代码
                if (rows == null && rows.Count == 0) return 0;
                //用户点击签退时时间
                DateTime signOutTime = DateTime.Parse(date);
                //签到时间
                DateTime signInTime = DateTime.Parse(rows[0]["F_CISignInDate"].ToString());
                //正常签到时间
                DateTime normalSignInTime = DateTime.Parse(workDuration.NormalTime);
                //缺席时间
                DateTime absentTimeSignIn = DateTime.Parse(workDuration.AbsentSignInTime);
                DateTime absentTimeSignOut = DateTime.Parse(workDuration.AbsentSignOutTime);
                //正常下班时间
                DateTime normalLeaveTime = DateTime.Parse(workDuration.LeaveEarlyTime);
                //浮动时间，单位：分钟
                int floatTime = int.Parse(workDuration.WDFloatTime);
                //一天工作时长计算
                TimeSpan ts = signOutTime - signInTime;
                int workTime = 0;
                if (ts.Minutes > 40) workTime = ts.Hours + 1;
                workTime = ts.Hours;
                if (workTime > workDuration.WDDayDuration) workTime = workDuration.WDDayDuration;

                //出现旷工情况
                if (signInTime > absentTimeSignIn || signOutTime < absentTimeSignOut) isAbsent = true;
                //迟到
                if ((signInTime > normalSignInTime && signInTime < absentTimeSignIn && workTime < 9) ||
                    (signInTime > normalSignInTime.AddMinutes(floatTime) && signInTime < absentTimeSignIn))
                    isLate = true;
                //早退
                if (signOutTime < normalLeaveTime && signOutTime > absentTimeSignOut) isLeaveEarly = true;
                //正常
                if (!isLate && !isAbsent && !isLeaveEarly) isNormal = true;
                int normalCount = 0, lateCount = 0, absentCount = 0, leaveEarlyCount = 0;
                
                if (isNormal)
                    normalCount = 1;
                if (isAbsent)
                    absentCount = 1;
                if (isLate)
                    lateCount = 1;
                if (isLeaveEarly)
                    leaveEarlyCount = 1;
                sql = string.Format(@"UPDATE T_CheckingInInfo SET F_CISignOutDate='{0}', 
                                      F_CIRealityWorkDuration={1},F_CIIsLate='{2}', F_CIIsLeaveEarvly='{3}', F_CIIsAbsenteeism='{4}', F_CIIsNormal='{5}' WHERE F_CIID={6}",
                                      signOutTime, workTime, lateCount, leaveEarlyCount, absentCount, normalCount, checkInInfoID);
            }
            int result = MrDBAccess.ExecuteNonQuery(sql);
            //更新失败
            if (result == 0) return 0;
            return 1;
        }
        /*****************************************************************
         * 功能：补签到补签退
         *       补签到
         *       补签退
         * 参数：checkInInfoID      被操作的考勤信息的ID（可选参数)
         *       empID              补签到（补签退）员工ID
         *       empName            补签到（补签退）员工姓名
         *       note               补签到（补签退）说明
         * 返回：成功“1”，失败“0”
         * **************************************************************/
        public int UpdateCheckInInfoBySystem(string empID, string empName, int depID, string depName, string date, string note, string appendEmpID = null, string appendEmpName = null, int checkInInfoID = 0)
        {
            //MODIFY
            string sql = "";
            if (checkInInfoID == 0)
            {
                //补签到
                //1. 如果已经被补签到过，更新操作
                CheckInInfo oldCheckInfo = isCheckInInfoExisted(DateTime.Parse(date).ToString("yyyy-MM-dd"), int.Parse(empID));
                WorkDuration workDuration = WorkDurationService.Instance.GetWorkDuration();
                if (oldCheckInfo != null)
                {
                    DateTime signInTime = DateTime.Parse(date);
                    DateTime signOutTime = DateTime.Parse(oldCheckInfo.CISignOutDate);
                    TimeSpan ts = signOutTime - signInTime;
                    int workTime = 0;
                    if (ts.Minutes > 40) workTime = ts.Hours + 1;
                    workTime = ts.Hours;
                    if (workTime > workDuration.WDDayDuration) workTime = workDuration.WDDayDuration;
                    sql = string.Format(@"UPDATE T_CheckingInInfo SET F_CISignInDate='{0}', F_AppendSignOutPersonID={1}, F_AppendSignOutPersonName='{2}', 
                                                                F_AppendSignOutPersonNote='{3}', F_CIRealityWorkDuration={4} 
                                                                {5} WHERE F_CIID={6}",
                                                                date, appendEmpID, appendEmpName, note.Replace("'","''"), workTime,
                                                                makeUpdateSql(signInTime, signOutTime, workDuration, DateTime.Parse(date).ToString("yyyy-MM-dd")), oldCheckInfo.CIID);
                    int res = MrDBAccess.ExecuteNonQuery(sql);
                    if (res == 0) return 0;
                    CheckInInfo newCheckInfo = GetCheckInInfo(oldCheckInfo.CIID);
                    doCalculate(newCheckInfo, oldCheckInfo);
                    return 1;
                }
                #region --------------------------------------------插入Sql语句
                else
                {
                    //2. 没有被补签到过，进行插入操作
                    sql = string.Format(@"Insert into T_CheckingInInfo(
                                                                F_EmpID,
                                                                F_EmpName,
                                                                F_DepID,
                                                                F_DepName,
                                                                F_CISignInDate,
                                                                F_CIRealityWorkDuration,
                                                                F_AppendSignInPersonID,
                                                                F_AppendSignInPersonName,
                                                                F_AppendSignInPersonNote,
                                                                F_CIIsLate,
                                                                F_CIIsLeaveEarvly,
                                                                F_CIIsAbsenteeism,
                                                                F_CIIsNormal,
                                                                F_CICreateDate,
                                                                F_CIIsSignIn,
                                                                F_CIIsSignOut,
                                                                F_CIIsCalculate,
                                                                F_Date
                                                ) 
                                                values({0},'{1}',{2},'{3}','{4}',{5},{6},'{7}','{8}',{9},{10},{11},{12},'{13}',{14},{15},{16}, '{17}')",
                                                        empID,
                                                        empName,
                                                        depID,
                                                        depName,
                                                        date,
                                                        0,
                                                        int.Parse(appendEmpID),
                                                        appendEmpName,
                                                        note.Replace("'", "''"),
                                                        "0 ",//F_CIIsLate
                                                        "0",//F_CIIsLeaveEavly
                                                        "0",//F_CIIsAbsenteeism
                                                        "0",//F_CIIsNormal
                                                        DateTime.Now.ToString("yyyy-MM-dd"),
                                                        "0",
                                                        "0",
                                                        "0",
                                                        DateTime.Parse(date).ToString("yyyy-MM-dd"));//设置补签为true
                    int res = MrDBAccess.ExecuteNonQuery(sql);
                    if (res == 0) return 0;
                    return 1;
                }
                #endregion
            }
            else
            {
                #region ------------------------------------------补签退
                CheckInInfo oldCheckInInfo = GetCheckInInfo(checkInInfoID);
                DateTime signInTime = DateTime.Parse(oldCheckInInfo.CISignInDate);
                DateTime signOutTime = DateTime.Parse(date);
                //MODIFY 补签退时进行这一天的工作装填统计
                TimeSpan ts = signOutTime - signInTime;
                int workTime = 0;
                if (ts.Minutes > 40) workTime = ts.Hours + 1;
                workTime = ts.Hours;

                sql = string.Format(@"UPDATE T_CheckingInInfo SET F_CISignOutDate='{0}', F_AppendSignOutPersonID={1}, F_AppendSignOutPersonName='{2}', 
                                                                F_AppendSignOutPersonNote='{3}', F_CIRealityWorkDuration={4} 
                                                                {5} WHERE F_CIID={6}",
                   signOutTime, appendEmpID, appendEmpName, note, workTime,
                   makeUpdateSql(signInTime, signOutTime, WorkDurationService.Instance.GetWorkDuration(), DateTime.Parse(date).ToString("yyyy-MM-dd")), checkInInfoID);
                int res = MrDBAccess.ExecuteNonQuery(sql);
                #endregion

                #region ---------------------------------------补签退后面的统计处理
                //补签退失败
                if (res == 0) return 0;
                else
                {
                    //补签退成功
                    //对被补签的员工进行统计
                    CheckInInfo newCheckInfo = GetCheckInInfo(checkInInfoID);
                    doCalculate(newCheckInfo, oldCheckInInfo);
                    return 1;
                }
                #endregion
            }
        }
        private void doCalculate(CheckInInfo newCheckInfo, CheckInInfo oldCheckInInfo)
        {
            int normalCount = 0, lateCount = 0, absentCount = 0, leaveEarlyCount = 0;
            if (oldCheckInInfo.ISCalculate)
            {

                if (oldCheckInInfo.CIIsNormal != newCheckInfo.CIIsNormal && oldCheckInInfo.CIIsNormal) normalCount = -1;
                if (oldCheckInInfo.CIIsNormal != newCheckInfo.CIIsNormal && newCheckInfo.CIIsNormal) normalCount = 1;
                if (oldCheckInInfo.CIIsLate != newCheckInfo.CIIsLate && oldCheckInInfo.CIIsLate) lateCount = -1;
                if (oldCheckInInfo.CIIsLate != newCheckInfo.CIIsLate && newCheckInfo.CIIsLate) lateCount = 1;
                if (oldCheckInInfo.CIIsLeaveEavly != newCheckInfo.CIIsLeaveEavly && oldCheckInInfo.CIIsLeaveEavly) leaveEarlyCount = -1;
                if (oldCheckInInfo.CIIsLeaveEavly != newCheckInfo.CIIsLeaveEavly && newCheckInfo.CIIsLeaveEavly) leaveEarlyCount = 1;
                if (oldCheckInInfo.CIIsAbsenteeism != newCheckInfo.CIIsAbsenteeism && oldCheckInInfo.CIIsAbsenteeism) absentCount = -1;
                if (oldCheckInInfo.CIIsAbsenteeism != newCheckInfo.CIIsAbsenteeism && newCheckInfo.CIIsAbsenteeism) absentCount = 1;
                int oldWorkTime = oldCheckInInfo.CIRealityWorkDuration;
                int newWorkTime = newCheckInfo.CIRealityWorkDuration;
                if (newWorkTime != oldWorkTime)
                    newCheckInfo.CIRealityWorkDuration = newWorkTime - oldWorkTime;
            }
            else
            {
                if (newCheckInfo.CIIsNormal) normalCount = 1;
                if (newCheckInfo.CIIsLeaveEavly) leaveEarlyCount = 1;
                if (newCheckInfo.CIIsLate) lateCount = 1;
                if (newCheckInfo.CIIsAbsenteeism) absentCount = 1;
            }
            List<CheckInInfo> checkInfoList = new List<CheckInInfo>();
            checkInfoList.Add(newCheckInfo);
            MonthAttendanceService.Instance.UpdateForAppendSignEmp(checkInfoList, false, normalCount, lateCount, absentCount, leaveEarlyCount);
        }
        private string makeUpdateSql(DateTime signInTime, DateTime signOutTime, WorkDuration workDuration, string currentDate)
        {
            bool isLate = false;
            bool isAbsent = false;
            bool isLeaveEarly = false;
            bool isNormal = false;
            string returnSql = "";
            //正常签到时间
            DateTime normalSignInTime = DateTime.Parse(currentDate + " " + workDuration.NormalTime);
            //缺席时间
            DateTime absentTimeSignIn = DateTime.Parse(currentDate + " " + workDuration.AbsentSignInTime);
            DateTime absentTimeSignOut = DateTime.Parse(currentDate + " " + workDuration.AbsentSignOutTime);
            //正常下班时间
            DateTime normalLeaveTime = DateTime.Parse(currentDate + " " + workDuration.LeaveEarlyTime);
            //浮动时间，单位：分钟
            int floatTime = int.Parse(workDuration.WDFloatTime);
            //一天工作时长计算
            TimeSpan ts = signOutTime - signInTime;
            int workTime = 0;
            if (ts.Minutes > 40) workTime = ts.Hours + 1;
            workTime = ts.Hours;

            //出现旷工情况
            if (signInTime > absentTimeSignIn || signOutTime < absentTimeSignOut) isAbsent = true;
            //迟到
            if ((signInTime > normalSignInTime && signInTime <= absentTimeSignIn && workTime < 9) ||
                (signInTime > normalSignInTime.AddMinutes(floatTime) && signInTime < absentTimeSignIn))
                isLate = true;
            //早退
            if (signOutTime < normalLeaveTime && signOutTime >= absentTimeSignOut) isLeaveEarly = true;
            //正常
            if (!isLate && !isAbsent && !isLeaveEarly) isNormal = true;
            int late = 0, leaveEarly = 0, absent = 0, normal = 0;
            if (isNormal)
                normal = 1;
            if (isAbsent)
                absent = 1;
            if (isLate)
                late = 1;
            if (isLeaveEarly)
                leaveEarly = 1;
            returnSql = string.Format(@",F_CIIsLate='{0}', F_CIIsLeaveEarvly='{1}', F_CIIsAbsenteeism='{2}', F_CIIsNormal='{3}'", late, leaveEarly, absent, normal);
            return returnSql;
        }
        /// **************************************************************
        /// <summary>
        /// 描述：为所有人补签到签退
        /// </summary>
        /// <param name="date">签到或者签退的日期</param>
        /// <param name="signInTime">签到时间</param>
        /// <param name="signOutTime">签退时间</param>
        /// <param name="appendForEmpID">被补签到人的ID</param>
        /// <param name="note">说明</param>
        /// <returns></returns>
        /// **************************************************************
        public int UpdateCheckInInfoBySystem(string date, string signInTime, string signOutTime, string note, int appendEmpID, string appendEmpName, int appendForEmpID)
        {
            string sql = "";
            DateTime signInDate = DateTime.Parse(date + " " + signInTime);
            DateTime signOutDate = DateTime.Parse(date + " " + signOutTime);
            TimeSpan timeSpan = signOutDate - signInDate;
            WorkDuration workDuration = WorkDurationService.Instance.GetWorkDuration();
            List<CheckInInfo> modifyedEmpList = new List<CheckInInfo>();
            int workTime = timeSpan.Hours;
            if (workTime > workDuration.WDDayDuration) workTime = workDuration.WDDayDuration;
            if (appendForEmpID == 0)
            {
                List<EmployeeInfo> empList = EmployeeService.Instance.GetList();
                
                if (empList != null && empList.Count > 0)
                {
                    for (int i = 0; i < empList.Count; i++)
                    {
                        //为所有人进行签到签退
                        //判断是否已经签过到/退
                        //MODIFY
                        CheckInInfo checkInfo = isCheckInInfoExisted(date, int.Parse(empList[i].EmpID));
                        if (checkInfo != null) return 0;
                        #region ------------------1. 插入语句
                        //MODIFY
                        //SQL语句过长问题
                        sql = string.Format(@"Insert into T_CheckingInInfo(
                                                                F_EmpID,
                                                                F_EmpName,
                                                                F_DepID,
                                                                F_DepName,
                                                                F_CISignInDate,
                                                                F_CISignOutDate,
                                                                F_CIRealityWorkDuration,
                                                                F_AppendSignInPersonID,
                                                                F_AppendSignInPersonName,
                                                                F_AppendSignInPersonNote,
                                                                F_CIIsLate,
                                                                F_CIIsLeaveEarvly,
                                                                F_CIIsAbsenteeism,
                                                                F_CIIsNormal,
                                                                F_CICreateDate,
                                                                F_CIIsSignIn,
                                                                F_CIIsCalculate,
                                                                F_Date
                                                ) 
                                                values({0},'{1}',{2},'{3}','{4}','{5}',{6},{7},'{8}','{9}',{10}, '{11}',{12},{13},'{14}');",
                                                        empList[i].EmpID,
                                                        empList[i].EmpName,
                                                        empList[i].DepID,
                                                        empList[i].DepName,
                                                        signInDate,
                                                        signOutDate,
                                                        workTime,
                                                        appendEmpID,
                                                        appendEmpName,
                                                        note.Replace("'", "''"),
                                                        makeSql(signOutDate, signInDate, workDuration),
                                                        DateTime.Now.ToString("yyyy-MM-dd"),
                                                        "0",
                                                        "0",
                                                        date);
                        #endregion
                        int res = MrDBAccess.ExecuteNonQuery(sql);
                        if (res == 0) return 0;
                        modifyedEmpList.Add(GetCheckInfo(int.Parse(empList[i].EmpID), date));
                    }
                }
            }
            else
            {
                //为某个员工进行签到签退
                #region ---------------------------sql语句
                EmployeeInfo employeeInfo = EmployeeService.Instance.GetEmployee(appendForEmpID);
                CheckInInfo checkInfo = isCheckInInfoExisted(date, int.Parse(employeeInfo.EmpID));
                if (checkInfo != null) return 0;
                sql = string.Format(@"INSERT INTO T_CheckingInInfo(F_EmpID,
                                                                F_EmpName,
                                                                F_DepID,
                                                                F_DepName,
                                                                F_CISignInDate,
                                                                F_CISignOutDate,
                                                                F_CIRealityWorkDuration,
                                                                F_AppendSignInPersonID,
                                                                F_AppendSignInPersonName,
                                                                F_AppendSignInPersonNote,
                                                                F_CIIsLate,
                                                                F_CIIsLeaveEarvly,
                                                                F_CIIsAbsenteeism,
                                                                F_CIIsNormal,
                                                                F_CICreateDate,
                                                                F_CIIsSignIn,
                                                                F_CIIsCalculate,
                                                                F_Date) 
                                                                VALUES({0},'{1}',{2},'{3}','{4}','{5}',{6},{7},'{8}','{9}',{10}, '{11}',{12},{13},'{14}')",
                                                                employeeInfo.EmpID,
                                                                employeeInfo.EmpName,
                                                                employeeInfo.DepID,
                                                                employeeInfo.DepName,
                                                                signInDate,
                                                                signOutDate,
                                                                workTime,
                                                                appendEmpID,
                                                                appendEmpName,
                                                                note.Replace("'", "''"),
                                                                makeSql(signOutDate, signInDate, workDuration),
                                                                DateTime.Now.ToString("yyyy-MM-dd"),
                                                                "0",
                                                                "0",
                                                                date);
                #endregion
                int res = MrDBAccess.ExecuteNonQuery(sql);
                if (res == 0) return 0;
                modifyedEmpList.Add(GetCheckInfo(int.Parse(employeeInfo.EmpID), date));
            }
            //List<CheckInInfo> modifyedEmpList = GetCheckInInfoListNoCalculate();
            MonthAttendanceService.Instance.UpdateForAppendSignEmp(modifyedEmpList, true);
            return 1;
        }
        #endregion
        #region -------------------------获取员工出勤信息
        /*******************************************************************
         *获取员工的出勤信息，部门经理、总经理和管理与昂
         * 参数：depID      部门ID
         *       empName    员工姓名，nullable
         *       date       查询时间，nullable
         *       rate       出勤率 枚举类型
         ******************************************************************/
        public List<CheckInInfo> GetCheckInInfoList(int depID = 0, string empNameLike = null, string startDate = null, string endDate = null, string rate = null)
        {
            string sql = "SELECT * FROM T_CheckingInInfo WHERE 1=1 ";
            if (!string.IsNullOrEmpty(empNameLike))
                sql += string.Format(@"AND F_EmpName LIKE '%{0}%' ", empNameLike);
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                sql += string.Format(@"AND (F_Date>='{0}' AND F_Date<='{1}') ", startDate, endDate);
            if (!string.IsNullOrEmpty(rate) && rate == "1")
                sql += string.Format(@"AND F_CIIsNormal='1' ");
            if (!string.IsNullOrEmpty(rate) && rate == "0")
                sql += string.Format(@"AND F_CIIsNormal!='1'");
            if (depID != 0)
                sql += string.Format(@"AND F_DepID={0} ", depID);
            sql += string.Format(@" order by F_Date desc");
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            if (rows != null && rows.Count > 0)
                return getCheckInInfo(rows);
            return null;
        }
        /*******************************************************************
         * 功能：获取个人的出勤信息，普通员工
         *       可用于查询
         * 参数：empID      员工ID
         *       empName    员工姓名
         *       date       查询时间，nullable
         *       rate       出勤率
         *           1全勤，0非全勤，null所有
         * ****************************************************************/
        public List<CheckInInfo> GetCheckInInfoList(string empID, string startDate, string endDate, string rate)
        {
            string sql = string.Format(@"SELECT * FROM T_CheckingInInfo WHERE F_EmpID={0} ", empID);
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                sql += string.Format(@" AND (F_Date>='{0}' AND F_Date<='{1}') ", startDate, endDate);
            }
            if (!string.IsNullOrEmpty(rate))
            {
                //MODIFYED
                if (rate.Equals("1"))
                    sql += " AND F_CIIsNormal='1'";
                else
                    sql += " AND F_CIIsNormal!='1'";
            }
            sql += string.Format(@" order by F_Date desc");
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            if (rows != null && rows.Count > 0)
                return getCheckInInfo(rows);
            return null;

        }
        /// *************************************************************
        /// <summary>
        /// 描述：根据员工的ID和姓名获取员工信息
        /// </summary>
        /// <param name="empName">员工姓名</param>
        /// <param name="empID">员工ID</param>
        /// <returns>
        /// 返回：存在，返回员工对象
        ///       不存在，返回NULL
        /// </returns>
        /// *************************************************************
        public CheckInInfo GetCurrentCheckInInfo(int empID)
        {
            //MODIFY
            string sql = string.Format(@"SELECT * FROM T_CheckingInInfo WHERE F_EmpID={0} AND F_Date='{1}'",
                    empID, DateTime.Now.ToString("yyyy-MM-dd"));
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            List<CheckInInfo> list = getCheckInInfo(rows);
            if (list != null && list.Count > 0) return list[0];
            return null;
        }
        public CheckInInfo GetCheckInfo(int empID, string date)
        {
            string sql = string.Format(@"SELECT * FROM T_CheckingInInfo WHERE F_EmpID={0} AND F_Date='{1}'",
                    empID, date);
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            List<CheckInInfo> list = getCheckInInfo(rows);
            if (list != null && list.Count > 0) return list[0];
            return null;
        }
        public CheckInInfo GetCheckInInfo(int checkInInfoID)
        {
            string sql = string.Format(@"SELECT * FROM T_CheckingInInfo WHERE F_CIID={0}", checkInInfoID);
            DataRowCollection row = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            if (row != null && row.Count > 0)
                return getCheckInInfo(row)[0];
            return null;
        }
        #endregion
        public void DeleteCheckInInfo(int empID)
        {
            string sql = string.Format(@"DELETE FROM T_CheckingInInfo WHERE F_EmpID={0}", empID);
            MrDBAccess.ExecuteNonQuery(sql);
        }
        public bool hasSignIn(int empID, string appendDate)
        {
            string sql = string.Format(@"SELECT F_CIID FROM T_CheckingInInfo WHERE F_EmpID={0} AND F_Date='{1}' AND F_CIIsSignIn='1'", empID, appendDate);
            int count = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows.Count;
            if (count == 0) return false;
            return true;
        }
        public bool hasSingOut(int empID, string appendDate)
        {
            string sql = string.Format(@"SELECT F_CIID FROM T_CheckingInInfo WHERE F_EmpID={0} AND F_Date='{1}' AND F_CIIsSignOut='1'", empID, appendDate);
            int count = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows.Count;
            if (count == 0) return false;
            return true;
        }
        /// *************************************************************
        /// <summary>
        /// 查询没有统计的所有签到信息
        /// </summary>
        /// <returns></returns>
        /// *************************************************************
        public List<CheckInInfo> GetCheckInInfoListNoCalculate()
        {
            string sql = string.Format(@"SELECT * FROM T_CheckingInInfo WHERE F_CIIsCalculate='0' AND F_CISignOutDate >= '1900-01-01'");
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            List<CheckInInfo> list = new List<CheckInInfo>();
            if (rows != null && rows.Count > 0)
            {
                list = getCheckInInfo(rows);
                return list;
            }
            return null;
        }
        /// <summary>
        /// 描述：将没有统计过的签到信息设置为已经被统计过
        /// </summary>
        public void SetCheckInInfoIsCalculate(List<CheckInInfo> checkInInfoList)
        {
            string sql = "";
            if (checkInInfoList != null && checkInInfoList.Count > 0)
            {
                for (int i = 0; i < checkInInfoList.Count; i++)
                {
                    sql += string.Format(@"UPDATE T_CheckingInInfo  SET F_CIIsCalculate='1' WHERE F_CIID={0};", checkInInfoList[i].CIID);
                }
                try
                {
                    MrDBAccess.ExecuteNonQuery(sql);
                }
                catch (Exception e)
                {
                    //do something
                    //TODO
                }
            }
        }
        private CheckInInfo isCheckInInfoExisted(string date/*2015-11-23*/, int empID)
        {
            string sql = string.Format(@"SELECT * FROM T_CheckingInInfo WHERE F_Date='{0}' AND F_EmpID={1}", date, empID);
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            if (rows.Count == 0) return null;
            return getCheckInInfo(rows)[0];
        }
        private string makeSql(DateTime signOutTime, DateTime signInTime, WorkDuration workDuration)
        {
            bool isLate = false;
            bool isAbsent = false;
            bool isLeaveEarly = false;
            bool isNormal = false;
            int late = 0, leaveEarly = 0, absent = 0, normal = 0;
            string sql = "";
            string asMonth = signInTime.ToString("yyyy-MM-dd");
            //计算一天的工作时长
            TimeSpan timeSpan = signOutTime - signInTime;
            int workTime = timeSpan.Hours;
            //出现旷工情况
            if (signInTime > DateTime.Parse(asMonth + " " + workDuration.AbsentSignInTime) || signOutTime < DateTime.Parse(asMonth + " " + workDuration.AbsentSignOutTime)) isAbsent = true;
            //迟到
            if ((signInTime > DateTime.Parse(asMonth + " " + workDuration.NormalTime) && signInTime <= DateTime.Parse(asMonth + " " + workDuration.AbsentSignInTime) && workTime < 9) ||
                (signInTime > DateTime.Parse(asMonth + " " + workDuration.NormalTime).AddMinutes(Double.Parse(workDuration.WDFloatTime)) && signInTime < DateTime.Parse(asMonth + " " + workDuration.AbsentSignInTime)))
                isLate = true;
            //早退
            if (signOutTime < DateTime.Parse(asMonth + " " + workDuration.LeaveEarlyTime) && signOutTime >= DateTime.Parse(asMonth + " " + workDuration.AbsentSignInTime)) isLeaveEarly = true;
            //正常
            if (!isLate && !isAbsent && !isLeaveEarly) isNormal = true;
            if (isAbsent)
                absent = 1;
            if (isLate)
                late = 1;
            if (isLeaveEarly)
                leaveEarly = 1;
            if (isNormal)
                normal = 1;
            sql = string.Format(@"'{0}','{1}','{2}','{3}'", late, leaveEarly, absent, normal);
            return sql;
        }
        private static List<CheckInInfo> getCheckInInfo(DataRowCollection rows)
        {
            if (rows == null || rows.Count <= 0) return null;
            int iRowLength = rows.Count;
            List<CheckInInfo> list = new List<CheckInInfo>();
            for (int i = 0; i < iRowLength; i++)
            {
                CheckInInfo checkInInfo = new CheckInInfo()
                {
                    CIID = DataBase.ObjectToInt(rows[i]["F_CIID"]),
                    EmpID = DataBase.ObjectToInt(rows[i]["F_EmpID"]),
                    EmpName = DataBase.ObjectToString(rows[i]["F_EmpName"]),
                    DepID = DataBase.ObjectToInt(rows[i]["F_DepID"]),
                    DepName = DataBase.ObjectToString(rows[i]["F_DepName"]),
                    CISignInDate = DataBase.ObjectToDate(rows[i]["F_CISignInDate"].ToString()),
                    CISignOutDate = DataBase.ObjectToDate(rows[i]["F_CISignOutDate"].ToString()),
                    CIRealityWorkDuration = DataBase.ObjectToInt(rows[i]["F_CIRealityWorkDuration"]),
                    AppendSignInPersonID = DataBase.ObjectToInt(rows[i]["F_AppendSignInPersonID"]),
                    AppendSignInPersonName = DataBase.ObjectToString(rows[i]["F_AppendSignInPersonName"]),
                    AppendSignInNote = DataBase.ObjectToString(rows[i]["F_AppendSignInPersonNote"]),
                    AppendSignOutPersonID = DataBase.ObjectToInt(rows[i]["F_AppendSignOutPersonID"]),
                    AppendSignOutPersonName = DataBase.ObjectToString(rows[i]["F_AppendSignOutPersonName"]),
                    AppendSignOutPersonNote = DataBase.ObjectToString(rows[i]["F_AppendSignOutPersonNote"]),
                    CIIsLate = DataBase.ObjectToBool(rows[i]["F_CIIsLate"]),
                    CIIsLeaveEavly = DataBase.ObjectToBool(rows[i]["F_CIIsLeaveEarvly"]),
                    CIIsAbsenteeism = DataBase.ObjectToBool(rows[i]["F_CIIsAbsenteeism"]),
                    IsSignIn = DataBase.ObjectToBool(rows[i]["F_CIIsSignIn"]),
                    IsSignOut = DataBase.ObjectToBool(rows[i]["F_CIIsSignOut"]),
                    ISCalculate = DataBase.ObjectToBool(rows[i]["F_CIIsCalculate"]),
                    CIIsNormal = DataBase.ObjectToBool(rows[i]["F_CIIsNormal"]),
                    CICreateDate = rows[i]["F_CICreateDate"].ToString(),
                    Date = DataBase.ObjectToString(rows[i]["F_Date"])
                };
                list.Add(checkInInfo);
            }
            return list;
        }
    }
}