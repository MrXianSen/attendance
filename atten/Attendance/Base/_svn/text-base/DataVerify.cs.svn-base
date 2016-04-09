using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Base
{
    public class DataVerify
    {
        /// <summary>
        /// 验证数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns>0</returns>
        public static int IntTryParse(string str) 
        {
            int result = 0;
            bool bol = int.TryParse(str, out result);
            if (bol)
                return int.Parse(str);
            else
                return result;
        }

        public static int IntTryParse(string str,int result)
        {
            bool bol = int.TryParse(str, out result);
            if (bol)
                return int.Parse(str);
            else
                return result;
        }

        /// <summary>
        /// yyyy-MM-dd hh:mm:ss
        /// </summary>
        public static DateTime DateTimeTryParse(string str)
        {
            DateTime date = DateTime.Parse("0001-1-1 00:00:00");
            bool bol = DateTime.TryParse(str, out date);
            if (bol)
                return DateTime.Parse(str);
            else
                return date;
        }

        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public static DateTime DateTryParse(string str)
        {
            DateTime date = DateTime.Parse("0001-1-1");
            bool bol = DateTime.TryParse(str, out date);
            if (bol)
                return DateTime.Parse(str);
            else
                return date;
        }
    }
}