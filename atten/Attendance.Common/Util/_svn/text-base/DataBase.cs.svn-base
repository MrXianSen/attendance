using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace Attendance.Common.Util
{
	/// <summary>
	/// DataBase 的摘要说明。
	/// </summary>
	public class DataBase
	{
		public static string StringToMD5Hash(string inputString)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(inputString));
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < encryptedBytes.Length; i++)
				sb.AppendFormat("{0:x2}", encryptedBytes[i]);
			return sb.ToString();
		}
		private static string FloatFormat = "0.00";
		public static string DateToString(DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm:ss");
		}
		public static DateTime StringToDate(string strDate)
		{
			if (string.IsNullOrEmpty(strDate))
				return DateTime.Now; ;
			return DateTime.Parse(strDate);
		}
		public static string ObjectToDay(object obj)
		{
			if (obj == null || obj.ToString() == "")
				return "";
			return DateTime.Parse(obj.ToString()).ToString("yyyy-MM-dd");
		}
		public static string ObjectToDate(object obj)
		{
			if (obj == null || obj.ToString() == "")
				return "";
			return DateTime.Parse(obj.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
		}
		public static int GetIntMonth(DateTime date)
		{
			return DataBase.ObjectToInt(date.ToString("yyyyMM"));
		}
		public static string NowToDay()
		{
			return DateTime.Now.ToString("yyyy-MM-dd");
		}
		public static string NowToDate()
		{
			return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}
		public static int GetIntMonth()
		{
			return DataBase.ObjectToInt(DateTime.Now.ToString("yyyyMM"));
		}
		public static string GetFristDayInMonth()
		{
			DateTime date = DateTime.Now;
			return (date.Year + "-" + (date.Month < 10 ? ("0" + date.Month) : ("" + date.Month)) + "-01");
		}
		public static bool ObjectToBool(object obj)
		{
			if (obj == null || obj == "")
				return false;
			if (obj.ToString().ToLower() == "true" || obj.ToString() == "1")
				return true;
			return false;
		}
		public static string ObjectToBoolString(object obj)
		{
			return (ObjectToBool(obj) ? "1" : "0");
		}
		public static string BoolToString(bool b)
		{
			return (b ? "1" : "0");
		}
		public static string BoolStringToString(string str)
		{
			if (string.IsNullOrEmpty(str))
				return "0";
			if (str.ToLower() == "true" || str == "1")
				return "1";
			return "0";
		}
		public static string ObjectToConverJsonString(object obj)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(ObjectToString(obj));
		}
		public static string ObjectToMoreString(object obj)
		{
			return ObjectToString(obj).Replace("\n", "");
		}
		public static string ObjectToString(object obj)
		{
			if (obj == null)
				return "";
			return obj.ToString();
		}
		public static int ObjectToInt(object obj)
		{
			if (obj == null || obj.ToString() == "")
				return 0;
			return int.Parse(obj.ToString());
		}
		public static long ObjectToLong(object obj)
		{
			if (obj == null || obj.ToString() == "")
				return 0;
			return long.Parse(obj.ToString());
		}
		public static float ObjectToFloat(object obj)
		{
			if (obj == null || obj.ToString() == "")
				return 0f;
			return float.Parse(obj.ToString());
		}
		public static string ObjectToDivide(object obj1, object obj2)
		{
			if (obj1 == null || obj1.ToString() == "" || obj2 == null || obj2.ToString() == "")
				return float.Parse("0").ToString(FloatFormat);
			return (float.Parse(obj1.ToString()) / float.Parse(obj2.ToString())).ToString(FloatFormat);
		}
		public static string ObjectToMultiply(object obj1, object obj2, bool isInt)
		{
			if (obj1 == null || obj1.ToString() == "" || obj2 == null || obj2.ToString() == "")
				return "0";
			if (isInt)
				return ((int)(float.Parse(obj1.ToString()) * float.Parse(obj2.ToString()))).ToString();
			return (float.Parse(obj1.ToString()) * float.Parse(obj2.ToString())).ToString(FloatFormat);
		}
		public static string ObjectToFloatFormat(object obj)
		{
			if (obj == null || obj.ToString() == "")
				return "";
			return float.Parse(obj.ToString()).ToString(FloatFormat);
		}
		public static string ObjectToShowFloatFormat(object obj)
		{
			if (obj == null || obj.ToString() == "")
				return 0.ToString(FloatFormat);
			return float.Parse(obj.ToString()).ToString(FloatFormat);
		}
		public static string StringToIDCard(string obj, bool isMobile)
		{
			if (string.IsNullOrEmpty(obj))
				return "";
			if (obj.Length > 4)
			{
				if (isMobile)
				{
					if (obj.Length > 8)
						obj = obj.Substring(0, obj.Length - 8) + "****" + obj.Substring(obj.Length - 4);
				}
				else
					obj = obj.Substring(0, obj.Length - 4) + "****";
			}
			return obj;
		}
		public static string FloatToDataBase(object obj)
		{
			if (obj == null || obj.ToString() == "")
				return "null";
			return obj.ToString();
		}
		public static bool CheckIsNull(string text, ref string strType, ref string strData, string strMess)
		{
			if (!string.IsNullOrEmpty(strData))
				return false;
			if (string.IsNullOrEmpty(text))
			{
				if (!string.IsNullOrEmpty(strMess))
					strData = strMess + "不能为空！";
				return false;
			}
			return true;
		}
		public static bool CheckLength(string text, ref string strType, ref string strData, string strMess, int len = 100)
		{
			if (!string.IsNullOrEmpty(strData))
				return false;
			if (!string.IsNullOrEmpty(text) && text.Length > len)
			{
				strData = strMess + "文本长度不能大于" + len + "！";
				return false;
			}
			return true;
		}
		public static bool CheckText(string text, ref string strType, ref string strData, string strMess)
		{
			if (!string.IsNullOrEmpty(strData))
				return false;
			if (!string.IsNullOrEmpty(text) && (text.IndexOf("|") > -1 || text.IndexOf(",") > -1))
			{
				strData = strMess + "不能包含竖线和逗号！";
				return false;
			}
			return true;
		}
		public static bool CheckMoblie(string text, ref string strType, ref string strData, string strMess)
		{
			if (!string.IsNullOrEmpty(strData))
				return false;
			if (!string.IsNullOrEmpty(text))
			{
				if (!Regex.IsMatch(text, "^[0-9]{11}$"))
				{
					strData = "请输入正确的手机号！";
					return false;
				}
			}
			return true;
		}
		public static bool ParseIndex(string strIndex, ref int Index, ref string strType, ref string strData, string strMess = "序号")
		{
			if (!string.IsNullOrEmpty(strData))
				return false;
			if (!string.IsNullOrEmpty(strIndex))
			{
				int index = -1;
				try
				{
					index = DataBase.ObjectToInt(strIndex);
				}
				catch { }
				if (strIndex != "" + index || index < 0)
				{
					strData = strMess + "必须是正整数！";
					return false;
				}
				Index = index;
			}
			else
				Index = int.MaxValue;
			return true;
		}
		public static bool ParseInt(string text, ref string strType, ref string strData, string strMess)
		{
			if (!string.IsNullOrEmpty(strData))
				return false;
			if (!string.IsNullOrEmpty(text))
			{
				if (!Regex.IsMatch(text, "^\\-?[0-9]*[1-9][0-9]*$"))
				{
					strData = strMess + "必须是整数！";
					return false;
				}
			}
			return true;
		}
		public static bool ParseIntOver(string text, ref string strType, ref string strData, string strMess)
		{
			if (!string.IsNullOrEmpty(strData))
				return false;
			if (!string.IsNullOrEmpty(text))
			{
				if (!Regex.IsMatch(text, "^[0-9]*[1-9][0-9]*$"))
				{
					strData = strMess + "必须是正整数！";
					return false;
				}
			}
			return true;
		}
		public static bool ParseFloat(string text, ref string strType, ref string strData, string strMess)
		{
			if (!string.IsNullOrEmpty(strData))
				return false;
			if (!string.IsNullOrEmpty(text))
			{
				if (!Regex.IsMatch(text, "^\\-?\\d+\\.?\\d+$|^\\d+$"))
				{
					strData = strMess + "必须是数字！";
					return false;
				}
			}
			return true;
		}
		public static bool ParseFloatOver(string text, ref string strType, ref string strData, string strMess)
		{
			if (!string.IsNullOrEmpty(strData))
				return false;
			if (!string.IsNullOrEmpty(text))
			{
				if (!Regex.IsMatch(text, "^\\d+\\.?\\d+$|^\\d+$"))
				{
					strData = strMess + "必须是正数！";
					return false;
				}
			}
			return true;
		}
		public static bool CheckFloatCompare(float fMin, float fMax, ref string strType, ref string strData, string strMessMin, string strMessMax)
		{
			if (!string.IsNullOrEmpty(strData))
				return false;
			if (fMin > fMax)
			{
				strData = strMessMin + "不能大于" + strMessMax + "！";
				return false;
			}
			return true;
		}
		public static bool CheckFloatCompare(string txtMin, string txtMax, ref string strType, ref string strData, string strMessMin, string strMessMax)
		{
			float fMin = 0f;
			float fMax = 0f;
			try
			{
				fMin = float.Parse(txtMin);
			}
			catch { }
			try
			{
				fMax = float.Parse(txtMax);
			}
			catch { }
			return CheckFloatCompare(fMin, fMax, ref strType, ref strData, strMessMin, strMessMax);
		}
		public static int GetInputIntOver(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				if (!Regex.IsMatch(text, "^[0-9]*[1-9][0-9]*$"))
					return 0;
				return DataBase.ObjectToInt(text);
			}
			return 0;
		}
		public static float GetInputFloatOver(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				if (!Regex.IsMatch(text, "^\\d+\\.?\\d+$|^\\d+$"))
					return 0f;
				return float.Parse(text);
			}
			return 0f;
		}
		public static int GetInputInt(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					return DataBase.ObjectToInt(text);
				}
				catch { }
			}
			return 0;
		}
		public static float GetInputFloat(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					return float.Parse(text);
				}
				catch { }
			}
			return 0f;
		}
		/// <summary> 
		/// 加密数据 
		/// </summary> 
		/// <param name="input">加密前的字符串</param> 
		/// <returns>加密后的字符串</returns> 
		public static string SimpleEncrypt(string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;
			// 盐值 
			string saltValue = "saltWeBangzhu";
			// 密码值 
			string pwdValue = "pwdWeBangzhu";
			byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(input);
			byte[] salt = System.Text.UTF8Encoding.UTF8.GetBytes(saltValue);
			// AesManaged - 高级加密标准(AES) 对称算法的管理类 
			System.Security.Cryptography.AesManaged aes = new System.Security.Cryptography.AesManaged();
			// Rfc2898DeriveBytes - 通过使用基于 HMACSHA1 的伪随机数生成器，实现基于密码的密钥派生功能 (PBKDF2 - 一种基于密码的密钥派生函数) 
			// 通过 密码 和 salt 派生密钥 
			System.Security.Cryptography.Rfc2898DeriveBytes rfc = new System.Security.Cryptography.Rfc2898DeriveBytes(pwdValue, salt);
			aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
			aes.KeySize = aes.LegalKeySizes[0].MaxSize;
			aes.Key = rfc.GetBytes(aes.KeySize / 8);
			aes.IV = rfc.GetBytes(aes.BlockSize / 8);
			// 用当前的 Key 属性和初始化向量 IV 创建对称加密器对象 
			System.Security.Cryptography.ICryptoTransform encryptTransform = aes.CreateEncryptor();
			// 加密后的输出流 
			System.IO.MemoryStream encryptStream = new System.IO.MemoryStream();
			// 将加密后的目标流（encryptStream）与加密转换（encryptTransform）相连接 
			System.Security.Cryptography.CryptoStream encryptor = new System.Security.Cryptography.CryptoStream(encryptStream, encryptTransform, System.Security.Cryptography.CryptoStreamMode.Write);
			// 将一个字节序列写入当前 CryptoStream （完成加密的过程） 
			encryptor.Write(data, 0, data.Length);
			encryptor.Close();
			// 将加密后所得到的流转换成字节数组，再用Base64编码将其转换为字符串 
			string encryptedString = Convert.ToBase64String(encryptStream.ToArray());
			return encryptedString;
		}
		/// <summary> 
		/// 解密数据 
		/// </summary> 
		/// <param name="input">加密后的字符串</param> 
		/// <returns>加密前的字符串</returns> 
		public static string SimpleDecrypt(string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;
			// 盐值（与加密时设置的值一致） 
			string saltValue = "saltWeBangzhu";
			// 密码值（与加密时设置的值一致） 
			string pwdValue = "pwdWeBangzhu";
			byte[] encryptBytes = Convert.FromBase64String(input);
			byte[] salt = Encoding.UTF8.GetBytes(saltValue);
			System.Security.Cryptography.AesManaged aes = new System.Security.Cryptography.AesManaged();
			System.Security.Cryptography.Rfc2898DeriveBytes rfc = new System.Security.Cryptography.Rfc2898DeriveBytes(pwdValue, salt);
			aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
			aes.KeySize = aes.LegalKeySizes[0].MaxSize;
			aes.Key = rfc.GetBytes(aes.KeySize / 8);
			aes.IV = rfc.GetBytes(aes.BlockSize / 8);
			// 用当前的 Key 属性和初始化向量 IV 创建对称解密器对象 
			System.Security.Cryptography.ICryptoTransform decryptTransform = aes.CreateDecryptor();
			// 解密后的输出流 
			System.IO.MemoryStream decryptStream = new System.IO.MemoryStream();
			// 将解密后的目标流（decryptStream）与解密转换（decryptTransform）相连接 
			System.Security.Cryptography.CryptoStream decryptor = new System.Security.Cryptography.CryptoStream(decryptStream, decryptTransform, System.Security.Cryptography.CryptoStreamMode.Write);
			// 将一个字节序列写入当前 CryptoStream （完成解密的过程） 
			decryptor.Write(encryptBytes, 0, encryptBytes.Length);
			decryptor.Close();
			// 将解密后所得到的流转换为字符串 
			byte[] decryptBytes = decryptStream.ToArray();
			string decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
			return decryptedString;
		}
        public static int SetInt(string date)
        {
            DateTime dt = Convert.ToDateTime(date);
            date = dt.ToString("yyyy-MM").Replace("-", "");
            return int.Parse(date);
        }
        public static int SetBool(bool bol)
        {
            return bol == true ? 1 : 0;
        }
        public static int GetInt(object obj)
        {
            if (obj == null || obj.ToString() == "")
                return 0;
            return Convert.ToInt32(obj);
        }
        public static int SetIntNull(object obj)
        {
            if (obj == null || obj.ToString() == "")
                return 0;
            return (bool)obj == true ? 1 : 0;
        }
        public static bool SetBooltNull(object obj)
        {
            if (obj == null || obj.ToString() == "")
                return false;
            return Convert.ToBoolean(obj);
        }
        public static string SetStringNull(object obj)
        {
            if (obj == null || obj.ToString() == "")
                return "";
            return obj.ToString();
        }
        public static int GetDateNull(object obj)
        {
            if (obj == null || obj.ToString() == "")
                return 0;
            else if (((DateTime)obj).ToString("yyyy-MM-dd") == "1990-01-01")
                return 0;
            return int.Parse(((DateTime)obj).ToString("yyyy-MM").Replace("-", ""));
        }
    }
}