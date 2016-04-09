using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.MODEL.Model
{
    /// <summary>
    /// 员工信息
    /// </summary>
    public class EmployeeInfo
    {
        private string _EmpID;//员ID
        private string _EmpName; //员工姓名
        private string _DepID;  //所属部门的ID
        private string _DepName;//所属部门的名称
        private string _EmpRole; //角色类别1、员工,2、部门经理,3、总经理,4、系统管理员,
        private string _EmpAccount; //登陆帐号
        private string _EmpPwd; //登陆密码
        private DateTime _EmpCreateDate;//创建时间
        private int _IsAdminister;
        private int _IsDepManager;
        private int _IsTopManager;

        public int IsAdminister
        {
            get { return _IsAdminister; }
            set { _IsAdminister = value; }
        }
        public int IsDepManager
        {
            get { return _IsDepManager; }
            set { _IsDepManager = value; }
        }
        public int IsTopManager
        {
            get { return _IsTopManager; }
            set { _IsTopManager = value; }
        }
        /// <summary>
        /// 员工ID
        /// </summary>
        public string EmpID
        {
            get { return _EmpID; }
            set { _EmpID = value; }
        }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmpName
        {
            get { return _EmpName; }
            set { _EmpName = value; }
        }
        public string DepID
        {
            get { return _DepID; }
            set { _DepID = value; }
        }
        public string DepName
        {
            get { return _DepName; }
            set { _DepName = value; }
        }
        /// <summary>
        /// 角色类别：1、员工,2、部门经理,3、总经理,4、系统管理员,
        /// </summary>
        public string EmpRole
        {
            get { return _EmpRole; }
            set { _EmpRole = value; }
        }

        /// <summary>
        /// 登陆帐号
        /// </summary>
        public string EmpAccount
        {
            get { return _EmpAccount; }
            set { _EmpAccount = value; }
        }

        /// <summary>
        /// 登陆密码
        /// </summary>
        public string EmpPwd
        {
            get { return _EmpPwd; }
            set { _EmpPwd = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime EmpCreateDate
        {
            get { return _EmpCreateDate; }
            set { _EmpCreateDate = value; }
        }
    }
}