using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.MODEL.Model
{
    public enum ATTENDANCERATE { ALL = -1, NOABSENT = 0, HASABSENT = 1 }
    /// <summary>
    /// 考勤信息
    /// </summary>
    public class CheckInInfo
    {
        private int _CIID;
        private int _EmpID;
        private string _EmpName;
        private int _DepID;
        private string _DepName;

        private int _AppendSignInPersonID;
        private string _AppendSignPersonName;
        private string _AppendSignNote;

        private int _AppendSignOutPersonID;
        private string _AppendSignOutPersonName;
        private string _AppendSignOutPersonNote;

        private string _CISignInDate;
        private string _CISignOutDate;
        private int _CIRealityWorkDuration;
        private bool _CIIsLate;
        private bool _CIIsLeaveEarvly;
        private bool _CIIsAbsenteeism;
        private bool _CIIsNormal;
        private string _CICreateDate;
        private bool _IsSignIn;
        private bool _IsSignOut;
        private bool _ISCalculate;
        private string _Date;
        

        public string Date
        {
            get { return _Date; }
            set { _Date = value; }
        }

        public int AppendSignOutPersonID
        {
            get { return _AppendSignOutPersonID; }
            set { _AppendSignOutPersonID = value; }
        }
        public string AppendSignOutPersonName
        {
            get { return _AppendSignOutPersonName; }
            set { _AppendSignOutPersonName = value; }
        }
        public string AppendSignOutPersonNote
        {
            get { return _AppendSignOutPersonNote; }
            set { _AppendSignOutPersonNote = value; }
        }
        public bool ISCalculate
        {
            get { return _ISCalculate; }
            set { _ISCalculate = value; }
        }

        public int AppendSignInPersonID
        {
            get { return _AppendSignInPersonID; }
            set { _AppendSignInPersonID = value; }
        }
        public string AppendSignInNote
        {
            get { return _AppendSignNote; }
            set { _AppendSignNote = value; }
        }
        public bool IsSignIn
        {
            get { return _IsSignIn; }
            set { _IsSignIn = value; }
        }
        public bool IsSignOut
        {
            get { return _IsSignOut; }
            set { _IsSignOut = value; }
        }
        public string AppendSignInPersonName
        {
            get { return _AppendSignPersonName; }
            set { _AppendSignPersonName = value; }
        }
        public string CICreateDate
        {
            get { return _CICreateDate; }
            set { _CICreateDate = value; }
        }

        public bool CIIsNormal
        {
            get { return _CIIsNormal; }
            set { _CIIsNormal = value; }
        }

        public bool CIIsAbsenteeism
        {
            get { return _CIIsAbsenteeism; }
            set { _CIIsAbsenteeism = value; }
        }

        public bool CIIsLeaveEavly
        {
            get { return _CIIsLeaveEarvly; }
            set { _CIIsLeaveEarvly = value; }
        }

        public bool CIIsLate
        {
            get { return _CIIsLate; }
            set { _CIIsLate = value; }
        }

        public int CIRealityWorkDuration
        {
            get { return _CIRealityWorkDuration; }
            set { _CIRealityWorkDuration = value; }
        }

        public string CISignOutDate
        {
            get { return _CISignOutDate; }
            set { _CISignOutDate = value; }
        }

        public string CISignInDate
        {
            get { return _CISignInDate; }
            set { _CISignInDate = value; }
        }

        public string DepName
        {
            get { return _DepName; }
            set { _DepName = value; }
        }

        public int DepID
        {
            get { return _DepID; }
            set { _DepID = value; }
        }

        public string EmpName
        {
            get { return _EmpName; }
            set { _EmpName = value; }
        }

        public int EmpID
        {
            get { return _EmpID; }
            set { _EmpID = value; }
        }

        public int CIID
        {
            get { return _CIID; }
            set { _CIID = value; }
        }
    }
}