using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Attendance.MODEL.Model;

namespace Attendance.Model
{
    public class DepartmentAndEmployee
    {
        public List<EmployeeInfo> EmpList { get; set; }
        public List<Department> DepList { get; set; }
    }
}