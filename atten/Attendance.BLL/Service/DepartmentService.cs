using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Attendance.MODEL.Model;
using Attendance.DAL.DBHelper;

namespace Attendance.BLL.Service
{
    /// <summary>
    /// 部门管理服务
    /// </summary>
    public class DepartmentService
    {
        public static DepartmentService Instance;
        static DepartmentService()
        {
            Instance = new DepartmentService();
        }

        /// <summary>
        /// 部门名称条件来查询信息
        /// </summary>
        /// <param name="searchDepName">部门名称</param>
        /// <returns></returns>
        public List<Department> GetDepartment(string searchDepName=null) 
        {
            string strWhere = "";
            if (!string.IsNullOrEmpty(searchDepName))
                strWhere = string.Format(@" WHERE F_DepName like '%{0}%'", searchDepName.Replace("'","''"));
            return getList(strWhere);
        }

        /********************************************************************************
         * 功能：部门是否存在
         * 参数：depName 部门名称
         * 返回：1 存在
         *       0 不存在
         * *****************************************************************************/
        public bool isDepartmentExist(string name)
        {
            string strWhere = string.Format(@" where F_DepName='{0}'", name.Replace("'", "''"));
            return getList(strWhere) != null;
        }

        #region 基础方法
        ///*******************************************************************************
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="depName">部门名称</param>
        /// <returns></returns>
        ///*******************************************************************************
        private List<Department> getList(string strWhere)
        {
            string sql = string.Format(@"SELECT * FROM T_Department {0}",strWhere);
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            return getDepartment(rows);
        }

        /// <summary>
        /// 根据depID取部门单条信息
        /// </summary>
        /// <param name="depID">部门id</param>
        /// <returns></returns>
        public Department GetInfo(int id) 
        {
            string sql = string.Format(@"select * from T_Department where F_DepID={0}", id);
            DataRowCollection rows = MrDBAccess.ExecuteDataSet(sql).Tables[0].Rows;
            List<Department> deplist = getDepartment(rows);
            return deplist != null ? deplist[0] : null;
        }
        /********************************************************************************
         * 功能：更新部门表
         * 参数：depID 部门ID，可以为null
         *       depName 部门名称
         * 返回：1 更新成功
         *       0 更新失败
         * *****************************************************************************/
        public int UpdateDepartment(string name, int id = 0)
        {
            string sql = "";
            //判断部门名称是否存在
            if (id == 0)
            {
                //添加
                if (isDepartmentExist(name)) return 0;
                sql = string.Format(@"insert into T_Department(F_DepName) values('{0}')", name.Replace("'", "''"));
                return MrDBAccess.ExecuteNonQuery(sql);
            }
            else
            {
                //修改
                if (isDepartmentExist(name)) return 0;
                int result = 0;
                sql = string.Format(@"update T_Department set F_DepName='{0}' where F_DepID={1}", name.Replace("'", "''"), id);
                string strSql = string.Format(@"update T_EmployeeInfo set F_DepName='{0}' where F_DepID={1}", name.Replace("'", "''"), id);//修改员工信息中的部门名称
                try
                {
                    MrDBAccess.BeginTransaction();
                    result = MrDBAccess.ExecuteNonQuery(sql);
                    MrDBAccess.ExecuteNonQuery(strSql);
                    MrDBAccess.CommitTransaction();
                    return result;
                }
                catch (Exception)
                {
                    MrDBAccess.RollbackTransaction();
                }
                return result;
            }
        }
        /*******************************************************************************
         * 功能：删除部门
         * 参数：depID 部门ID
         * 返回：1 成功
         *       0 失败
         * *****************************************************************************/
        public int DeleteDepartment(string id)
        { 
            /*1. 删除部门后对其部门下的员工的处理*/
            string strSql = string.Format(@"SELECT COUNT(1) as count FROM T_EmployeeInfo WHERE F_DepID={0}", id);
            int count = int.Parse(MrDBAccess.ExecuteDataSet(strSql).Tables[0].Rows[0]["count"].ToString());
            if (count <= 0)
            {
                string sql = string.Format(@"delete from T_Department where F_DepID={0}", id);
                int res = MrDBAccess.ExecuteNonQuery(sql);
                if (res != 0) return 1;
            }
            return 0;
        }
        /********************************************************************************
         * 功能：将DataRowCollection转化为Department List
         * 参数：rows
         * 返回：List集合
         * *****************************************************************************/
        private List<Department> getDepartment(DataRowCollection rows)
        { 
            List<Department> depList = new List<Department>();
            if (rows != null && rows.Count > 0)
            {
                int iRowLength = rows.Count;
                for (int i = 0; i < iRowLength; i++)
                {
                    Department dep = new Department()
                    {
                        DepID = rows[i]["F_DepID"].ToString(),
                        DepName = rows[i]["F_DepName"].ToString()
                    };
                    depList.Add(dep);
                }
                return depList;
            }
            return null;
        }
        #endregion
    }
}
