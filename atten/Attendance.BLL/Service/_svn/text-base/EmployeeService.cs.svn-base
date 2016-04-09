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
    /// 员工管理服务
    /// </summary>
    public class EmployeeService
    {
        public static EmployeeService Instance;
        static EmployeeService()
        {
            Instance = new EmployeeService();
        }

        /// <summary>
        /// 查询用户列表信息
        /// </summary>
        /// <param name="searchName">用户名称</param>
        /// <returns></returns>
        public List<EmployeeInfo> GetList(string searchName=null, int searchDepID=0) 
        {
            string strWhere = " WHERE";
            if (string.IsNullOrEmpty(searchName)) searchName = "";
            strWhere += " F_EmpName LIKE '%" + searchName + "%'";
            if (searchDepID != 0)
                strWhere += string.Format(@" AND F_DepID={0}", searchDepID);
            return getList(strWhere);
        }

        /***************************************************************
         * 功能：判断用户是否存在
         * 参数：empAccount 登录账号
         *       empPwd 登录密码
         * 返回：成功“1”，否则“0”
         * *************************************************************/
        public EmployeeInfo IsEmployeeExist(string empAccount, string empPwd)
        {
            string strWhere = string.Format(@" WHERE F_EmpAccount='{0}' AND F_EmpPwd='{1}'", empAccount.Replace("'", "''"), empPwd.Replace("'", "''"));
            List<EmployeeInfo> emplist=getList(strWhere);
            return emplist!=null ? emplist[0] : null;
        }

        /// ******************************************************************
        /// <summary>
        /// 获取部门员工列表， 部门经理
        /// </summary>
        /// <param name="depID">部门ID</param>
        /// <param name="empName">员工姓名，可选参数</param>
        /// <returns>
        /// 部门中所有员工的数据列表
        /// </returns>
        /// ******************************************************************
        public List<EmployeeInfo> GetEmployee(int id, string name = null)
        {
            string strWhere = string.Format(@" where F_DepID={0}", id);
            if (string.IsNullOrEmpty(name))
                strWhere += string.Format(@" and F_EmpName='{0}'", name.Replace("'", "''"));
            return getList(strWhere);
        }

        /// ******************************************************************
        /// <summary>
        /// 功能：修改密码
        /// 参数：empID 员工ID
        ///       newPwd 新密码
        ///       oldPwd旧密码
        /// 返回：成功“1”，失败“0”
        /// </summary>
        /// <param name="newPwd">新密码</param>
        /// <param name="empID">员工ID</param>
        /// <param name="oldPwd">旧密码</param>
        /// <returns>成功“1”，失败“0”</returns>
        /// ******************************************************************
        public int ModifyPwd(string id, string newPwd, string oldPwd)
        {
            /*1. 修改密码*/
            string confirmSql = string.Format("SELECT COUNT(1) FROM T_EmployeeInfo WHERE F_EmpID={0} AND F_EmpPwd='{1}'", id, oldPwd);
            int count = MrDBAccess.ExecuteNonQuery(confirmSql);
            if (count == 0) return 0;
            string sql = string.Format(@"UPDATE T_EmployeeInfo SET F_EmpPwd='{0}' WHERE F_EmpID={1}", newPwd, id);
            int res = MrDBAccess.ExecuteNonQuery(sql);
            if (res == 0) return 0;
            return 1;
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns>成功“1”，失败“0”</returns>
        public int ResetPwd(string empID)
        {
            /*1. 重置员工密码*/
            string sql = string.Format(@"UPDATE T_EmployeeInfo SET F_EmpPwd='{0}' WHERE F_EmpID={1}", DataBase.StringToMD5Hash("123456"), empID);
            int res = MrDBAccess.ExecuteNonQuery(sql);
            if (res == 0) return 0;
            return 1;
        }

        
        #region -------------------------基础方法
        /// ******************************************************************
        /// <summary>
        /// 获取所有员工列表, 管理员、总经理
        /// </summary>
        /// <returns>
        /// 返回所有员工的数据集合
        /// </returns>
        /// ******************************************************************
        private List<EmployeeInfo> getList(string strWhere)
        {
            string sql = string.Format(@"SELECT * FROM T_EmployeeInfo {0}", strWhere);
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            return getEmployeeInfo(rows);
        }

        /// ******************************************************************
        /// <summary>
        /// 获取本人信息， 普通员工
        /// </summary>
        /// <param name="empID">员工ID</param>
        /// <returns>
        /// 返回一个EmployeeInfo对象
        /// </returns>
        /// ******************************************************************
        public EmployeeInfo GetEmployee(int empID)
        {
            string sql = string.Format(@"SELECT * FROM T_EmployeeInfo WHERE F_EmpID={0}", empID);
            DataRowCollection row = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            return getEmployeeInfo(row)[0];
        }
        
        /// ******************************************************************
        /// <summary>
        /// 更新Employee数据表数据
        /// </summary>
        /// <param name="employee">员工对象</param>
        /// <returns>
        /// 更新成功“1”
        /// 否则返回“0”
        /// </returns>
        /// ******************************************************************
        public int Update(EmployeeInfo employee)
        {
            string sql = "";
            /*1. empID为null，执行添加操作*/
            if (string.IsNullOrEmpty(employee.EmpID))
            {
                sql = string.Format(@"INSERT INTO T_EmployeeInfo(F_EmpName, F_EmpRole, F_DepID, F_DepName, F_EmpAccount, F_EmpPwd, F_EmpCreateDate) VALUES('{0}','{1}',{2},'{3}','{4}','{5}','{6}')",
                    employee.EmpName, employee.EmpRole, employee.DepID, employee.DepName, employee.EmpAccount, employee.EmpPwd, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            /*2. empID不为null，执行修改操作*/
            else
            {
                sql = string.Format(@"UPDATE T_EmployeeInfo SET F_EmpName='{0}',F_EmpRole='{1}',F_EmpAccount='{2}',F_DepID={3},F_DepName='{4}' WHERE F_EmpID={5}",
                    employee.EmpName, employee.EmpRole, employee.EmpAccount, employee.DepID, employee.DepName, employee.EmpID);
            }
            int res = MrDBAccess.ExecuteNonQuery(sql);
            if (res == 0) return 0;
            return 1;
        }
        /// ******************************************************************
        /// <summary>
        /// 功能：删除员工
        /// 参数：员工ID
        /// 返回：成功“1”，失败“0”
        /// </summary>
        /// <param name="empID">员工ID</param>
        /// <returns>成功返回“1”，否则返回“0”</returns>
        /// ******************************************************************
        public int DeleteEmployee(string empID)
        {
            /*1. 执行删除操作*/
            MonthAttendanceService.Instance.Delete(int.Parse(empID));
            CheckInInfoService.Instance.DeleteCheckInInfo(int.Parse(empID));
            string sql = string.Format(@"DELETE FROM T_EmployeeInfo WHERE F_EmpID={0}", empID);
            int res = MrDBAccess.ExecuteNonQuery(sql);
            if (res == 0) return 0;
            return 1;
        }
        
        private List<EmployeeInfo> getEmployeeInfo(DataRowCollection rows)
        {
            if (rows != null && rows.Count > 0)
            {
                List<EmployeeInfo> list = new List<EmployeeInfo>();
                int iRowLength = rows.Count;
                for (int i = 0; i < iRowLength; i++)
                {
                    EmployeeInfo employee = new EmployeeInfo();
                    employee.EmpID = DataBase.ObjectToString(rows[i]["F_EmpID"]);
                    employee.EmpRole = DataBase.ObjectToString(rows[i]["F_EmpRole"]);
                    employee.EmpName = DataBase.ObjectToString(rows[i]["F_EmpName"]);
                    employee.EmpAccount = DataBase.ObjectToString(rows[i]["F_EmpAccount"]);
                    employee.DepID = rows[i]["F_DepID"].ToString();
                    employee.DepName = DataBase.ObjectToString(rows[i]["F_DepName"]);
                    if (employee.EmpRole.IndexOf(",4,") != -1)
                        employee.IsAdminister = 1;
                    if (employee.EmpRole.IndexOf(",3,") != -1)
                        employee.IsTopManager = 1;
                    if (employee.EmpRole.IndexOf(",2,") != -1)
                        employee.IsDepManager = 1;
                    list.Add(employee);
                }
                return list;
            }
            return null;
        }
        #endregion
    }
}
