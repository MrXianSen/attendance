using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Attendance.Common.Util;
using System.Configuration;


namespace Attendance.DAL.DBHelper
{
	public class MrDBAccess
	{
		private static object objLock = new object();
		private static Dictionary<string, int> dicTableKey = new Dictionary<string, int>();
		public static int GetTableKey(string key, int iGetCnt = 1, int iGroupNum = 50, int iStart = 10000)
		{
			return GetTableKey(ConnectionString(), key, iGetCnt, iGroupNum, iStart);
		}
		public static int GetTableKey(string connectionString, string key, int iGetCnt = 1, int iGroupNum = 50, int iStart = 10000)
		{
			lock (objLock)
			{
				bool inDB = true;
				string strSql = "";
				#region 查找可使用的ID
				int iValue = iStart;
				if (dicTableKey.ContainsKey(key))
					iValue = dicTableKey[key];
				else
				{
					strSql = "SELECT F_Value FROM T_SEQUENCE WHERE F_Name='" + key.Replace("'", "''") + "'";
					DataRowCollection rows = MrDBAccess.ExecuteDataSet(connectionString, strSql).Tables[0].Rows;
					if (rows.Count > 0)
						iValue = DataBase.ObjectToInt(rows[0]["F_Value"]);
					else
						inDB = false;
				}
				if (iValue < iStart)
					iValue = iStart;
				#endregion
				#region 保存到新内存和数据库
				if (dicTableKey.ContainsKey(key))
					dicTableKey[key] = iValue + iGetCnt;
				else
					dicTableKey.Add(key, iValue + iGetCnt);
				strSql = "";
				if (iGetCnt == 1)
				{
					if (!inDB)
						strSql = "INSERT INTO T_SEQUENCE(F_Value,F_Name) VALUES(" + (iValue + iGroupNum) + ",'" + key.Replace("'", "''") + "')";
					else if ((iValue - 1) % iGroupNum == 0)
						strSql = "UPDATE T_SEQUENCE SET F_Value=" + (iValue + iGroupNum) + " WHERE F_Name='" + key.Replace("'", "''") + "'";
				}
				else
				{
					if (!inDB)
						strSql = "INSERT INTO T_SEQUENCE(F_Value,F_Name) VALUES(" + (iValue + iGetCnt - 1 + iGroupNum) + ",'" + key.Replace("'", "''") + "')";
					else
						strSql = "UPDATE T_SEQUENCE SET F_Value=" + (iValue + iGetCnt - 1 + iGroupNum) + " WHERE F_Name='" + key.Replace("'", "''") + "'";
				}
				if (strSql != "")
					MrDBAccess.ExecuteNonQuery(connectionString, strSql);
				#endregion
				return iValue;
			}
		}
		public static void RemoveTableKey(string key, int iNum, int iGroupNum = 50)
		{
			RemoveTableKey(ConnectionString(), key, iNum, iGroupNum);
		}
		public static void RemoveTableKey(string connectionString, string key, int iNum, int iGroupNum = 50)
		{
			lock (objLock)
			{
				string strSql = "SELECT * FROM T_SEQUENCE WHERE F_Name='" + key.Replace("'", "''") + "'";
				DataRowCollection rows = MrDBAccess.ExecuteDataSet(connectionString, strSql).Tables[0].Rows;
				if (rows.Count > 0 && DataBase.ObjectToInt(rows[0]["F_Value"]) == iNum)
				{
					strSql = "UPDATE T_SEQUENCE SET F_Value=" + (iNum - iGroupNum) + " WHERE F_Name='" + key.Replace("'", "''") + "'";
					MrDBAccess.ExecuteNonQuery(connectionString, strSql);
				}
				if (dicTableKey.ContainsKey(key) && dicTableKey[key] == iNum)
					dicTableKey[key] = iNum - iGroupNum;
			}
		}
		private static Mutex m_Mutex = new Mutex();
		public static SqlConnection STConnection;
		public static SqlTransaction STTransaction;
		// Methods
		public static void BeginTransaction()
		{
			BeginTransaction(ConnectionString());
		}
		public static void BeginTransaction(string ConnectionString)
		{
			EndTransaction();
			STConnection = new SqlConnection(ConnectionString);
			STConnection.Open();
			STTransaction = STConnection.BeginTransaction(IsolationLevel.ReadCommitted);
		}
		public static string CharIndex(string sFieldName, string sList)
		{
			return CharIndex(ConnectionString(), sFieldName, sList);
		}
		public static string CharIndex(string ConnectionString, string sFieldName, string sList)
		{
			return string.Format("CHARINDEX(',' + CAST({0} AS VARCHAR(10)) + ',', ',{1},')", sFieldName, sList.Trim());
		}
		public static void CommitTransaction()
		{
			if (STTransaction != null)
				STTransaction.Commit();
			EndTransaction();
		}
		private static string _dBConnect;
		public static string ConnectionString()
		{
#if winform
#else
			if (_dBConnect == null)
				_dBConnect = ConfigurationSettings.AppSettings["ConnectionString"];
#endif
			return _dBConnect;
		}
		private static void EndTransaction()
		{
			if (STTransaction != null)
			{
				STTransaction.Dispose();
				STTransaction = null;
			}
			if (STConnection != null)
			{
				STConnection.Close();
				STConnection.Dispose();
				STConnection = null;
			}
		}
		public static DataSet ExecuteDataSet(string commandText)
		{
			return ExecuteDataSet(ConnectionString(), commandText);
		}
		public static DataSet ExecuteDataSet(string sqlString, params SqlParameter[] commandParameters)
		{
			return ExecuteDataSet(ConnectionString(), sqlString, commandParameters);
		}
		public static DataSet ExecuteDataSet(string connectionString, string commandText)
		{
			try
			{
				return SqlHelper.ExecuteDataset(connectionString, CommandType.Text, commandText);
			}
			catch (Exception exception)
			{
				throw new Exception(exception.Message + "\n" + commandText, exception);
			}
		}
		public static DataSet ExecuteDataSet(string connectionString, string sqlString, params SqlParameter[] commandParameters)
		{
			string commandText = sqlString;
			try
			{
				FillParameter(commandParameters);
				return SqlHelper.ExecuteDataset(connectionString, CommandType.Text, commandText, commandParameters);
			}
			catch (Exception exception)
			{
				throw new Exception(exception.Message + "\n" + commandText, exception);
			}
		}
		public static int GetPage(string strId, string strTable, string strCol, string strOrder, string strValue, int iPerPageCount, ref int iNextID)
		{
			return GetPage(ConnectionString(), strId, strTable, strCol, strOrder, strValue, iPerPageCount, ref iNextID);
		}
		//获得转到某行的分页页数。
		public static int GetPage(string connectionString, string strId, string strTable, string strCol, string strOrder, string strValue, int iPerPageCount, ref int iNextID)
		{//"USR_ID","USR WHERE USR_DEP_ID=3","USR_SEX DESC,USR_ID ASC","30",20
			try
			{
				if (strOrder == "")
					strOrder = strId;
				int iRowNum = 0;
				string strSql = "";
				string strId1 = strId;
				if (strId1.LastIndexOf(".") > -1)
					strId1 = strId1.Substring(strId1.LastIndexOf(".") + 1);
				strSql = "BEGIN " + (iNextID < 0 ? "DECLARE @RowNum int;" : "") + "SELECT IDENTITY(INT,1,1) ROWNUM" + (string.IsNullOrEmpty(strId) || !string.IsNullOrEmpty(strCol) ? "" : ("," + strId)) + (string.IsNullOrEmpty(strCol) ? "" : ("," + strCol)) + " INTO #TEMP" + strTable + " ORDER BY " + strOrder + ";SELECT " + (iNextID < 0 ? "@RowNum=ROWNUM" : "ROWNUM AS RootRowNum") + " FROM #TEMP WHERE " + strId1 + "='" + strValue.Replace("'", "''") + "';" + (iNextID < 0 ? "SELECT @RowNum AS RootRowNum,* FROM #TEMP WHERE ROWNUM IN (@RowNum-1,@RowNum,@RowNum+1) ORDER BY ROWNUM DESC;" : "") + "DROP TABLE #TEMP;END;";
				DataRowCollection rows = ExecuteDataSet(connectionString, strSql).Tables[0].Rows;
				if (rows != null && rows.Count > 0)
				{
					iRowNum = DataBase.ObjectToInt(rows[0]["RootRowNum"]);
					if (iNextID < 0)
					{
						for (int i = 0; i < rows.Count; i++)
						{
							int iRowNumTemp = DataBase.ObjectToInt(rows[i]["ROWNUM"]);
							if (iRowNumTemp == iRowNum + 1)
							{
								iNextID = DataBase.ObjectToInt(rows[i][strId1]);
								break;
							}
							else if (iRowNumTemp == iRowNum - 1)
							{
								iNextID = DataBase.ObjectToInt(rows[i][strId1]);
								iRowNum = iRowNum - 1;
								break;
							}
						}
					}
				}
				return CalculatePageIndex(iPerPageCount, iRowNum);
			}
			catch { }
			return 1;
		}
		public static void UpdateIndex(string strID, string strNextID, string strTable, string where = "")
		{
			UpdateIndex(ConnectionString(), strID, strNextID, strTable, where);
		}
		public static void UpdateIndex(string connectionString, string strID, string strNextID, string strTable, string where = "")
		{
			int ID = DataBase.ObjectToInt(strID);
			int NextID = DataBase.ObjectToInt(strNextID);
			string strSql = "SELECT F_ID,F_Index FROM " + strTable + " WHERE F_ID = " + ID + " OR F_ID = " + NextID;
			DataRowCollection rows = MrDBAccess.ExecuteDataSet(strSql).Tables[0].Rows;
			int iOldIndex1 = -1;
			int iOldIndex2 = -1;
			for (int i = 0; i < rows.Count; i++)
			{
				if (DataBase.ObjectToString(rows[i]["F_ID"]) == ID + "")
					iOldIndex1 = DataBase.ObjectToInt(rows[i]["F_Index"]);
				else if (DataBase.ObjectToString(rows[i]["F_ID"]) == NextID + "")
					iOldIndex2 = DataBase.ObjectToInt(rows[i]["F_Index"]);
			}
			try
			{
				MrDBAccess.BeginTransaction(connectionString);
				strSql = "UPDATE " + strTable + " SET F_Index=" + iOldIndex2 + " WHERE F_ID=" + ID + where;
				MrDBAccess.ExecuteNonQuery(connectionString, strSql);
				strSql = "UPDATE " + strTable + " SET F_Index=" + iOldIndex1 + " WHERE F_ID=" + NextID + where;
				MrDBAccess.ExecuteNonQuery(connectionString, strSql);
				MrDBAccess.CommitTransaction();
			}
			catch
			{
				MrDBAccess.RollbackTransaction();
			}
		}
		public static int SetIndex(string strTable1, string strTable2, string strWhere1, string strWhere2, string strOrderCol1, string strOrderCol2, string strFID1, string strValue, string strOldOrderValue)
		{
			return SetIndex(ConnectionString(), strTable1, strTable2, strWhere1, strWhere2, strOrderCol1, strOrderCol2, strFID1, strValue, strOldOrderValue);
		}
		public static int SetIndex(string connectionString, string strTable1, string strTable2, string strWhere1, string strWhere2, string strOrderCol1, string strOrderCol2, string strFID1, string strValue, string strOldOrderValue)
		//服务器修改排序。//"USR","USR","","","USR_ORDER","USR_ID","30",
		//参数：新表名(不带WHERE语句)，旧表名(不带WHERE语句，如果是同表则传"")，新排序范围搜索条件(不包含表名及WHERE的SQL语句),旧排序范围搜索条件(不包含表名及WHERE的SQL语句,如果是同数据集则传"")，新表排序列名，旧表排序列名，主键列名，主键列的值(删除传空),排序列的原始值(可为"")
		{
			try
			{
				if (strValue == "")
				{//删除时候调用
					ExecuteNonQuery(connectionString, "UPDATE " + strTable1 + " SET " + strOrderCol1 + "=" + strOrderCol1 + "-1 WHERE " + (strWhere1 == "" ? "" : (strWhere1 + " AND ")) + strOrderCol1 + " >" + strOldOrderValue);
					return 1;
				}
				//修改本行排序可能的错误排序值
				string strSql = "SELECT * FROM (SELECT " + strOrderCol1 + " MYORDER,0 TYPE FROM " + strTable1 + " WHERE " + (strWhere1 == "" ? "" : (strWhere1 + " AND ")) + strFID1 + "='" + strValue + "'" + " UNION SELECT COUNT(" + strOrderCol1 + ") MYORDER,1 TYPE FROM " + strTable1 + " WHERE " + (strWhere1 == "" ? "" : (strWhere1 + " AND ")) + strFID1 + "<>'" + strValue + "'" + ") A ORDER BY TYPE ASC";
				DataRowCollection rows = ExecuteDataSet(connectionString, strSql).Tables[0].Rows;
				string strNowOrder = rows[0]["MYORDER"].ToString();
				string strMaxOrder = rows.Count > 0 ? (rows[1]["MYORDER"].ToString() == "" ? "0" : rows[1]["MYORDER"].ToString()) : "0";
				int iMaxOrder = DataBase.ObjectToInt(strMaxOrder) + 1;
				int iNowOrder = iMaxOrder;
				if (strNowOrder.Trim() != "")
				{
					try
					{
						iNowOrder = DataBase.ObjectToInt(strNowOrder);
					}
					catch { }
				}
				if ((iNowOrder < 1) || (iNowOrder >= iMaxOrder))
				{
					iNowOrder = (iNowOrder < 1 ? 1 : iMaxOrder);
					strSql = "UPDATE " + strTable1 + " SET " + strOrderCol1 + "=" + iNowOrder + " WHERE " + (strWhere1 == "" ? "" : (strWhere1 + " AND ")) + strFID1 + "='" + strValue + "'";
					ExecuteNonQuery(connectionString, strSql);
				}
				if ((strTable2 == "" || strTable1 == strTable2) && (strWhere2 == "" || strWhere1 == strWhere2))
				{//同数据集
					//修改原来数据集
					if (strOldOrderValue != "")
					{
						if (DataBase.ObjectToInt(strOldOrderValue) > iNowOrder)
							strSql = "UPDATE " + strTable1 + " SET " + strOrderCol1 + "=" + strOrderCol1 + "+1 WHERE " + (strWhere1 == "" ? "" : (strWhere1 + " AND ")) + strOrderCol1 + ">=" + iNowOrder + " AND " + strOrderCol1 + " <" + strOldOrderValue + " AND " + strFID1 + " <>'" + strValue + "'";
						else
							strSql = "UPDATE " + strTable1 + " SET " + strOrderCol1 + "=" + strOrderCol1 + "-1 WHERE " + (strWhere1 == "" ? "" : (strWhere1 + " AND ")) + strOrderCol1 + "<=" + iNowOrder + " AND " + strOrderCol1 + " >" + strOldOrderValue + " AND " + strFID1 + " <>'" + strValue + "'";
						ExecuteNonQuery(connectionString, strSql);
					}
					else
					{//直接插入
						strSql = "UPDATE " + strTable1 + " SET " + strOrderCol1 + "=" + strOrderCol1 + "+1 WHERE " + (strWhere1 == "" ? "" : (strWhere1 + " AND ")) + strOrderCol1 + ">=" + iNowOrder + " AND " + strFID1 + " <>'" + strValue + "'";
						ExecuteNonQuery(connectionString, strSql);
					}
				}
				else
				{//在不同表或不同数据集之间排序
					if (strOldOrderValue != "")
					{//修改原来的表
						strSql = "UPDATE " + strTable2 + " SET " + strOrderCol2 + " = " + strOrderCol2 + "-1 WHERE " + (strWhere2 == "" ? "" : (strWhere2 + " AND ")) + strOrderCol2 + ">" + strOldOrderValue;
						ExecuteNonQuery(connectionString, strSql);
					}
					//修改新表
					strSql = "UPDATE " + strTable1 + " SET " + strOrderCol1 + " = " + strOrderCol1 + "+1 WHERE " + (strWhere1 == "" ? "" : (strWhere1 + " AND ")) + strOrderCol1 + ">=(SELECT " + strOrderCol1 + " FROM " + strTable1 + " WHERE " + (strWhere1 == "" ? "" : (strWhere1 + " AND ")) + strFID1 + "='" + strValue + "') AND " + strFID1 + "<>'" + strValue + "'";
					ExecuteNonQuery(connectionString, strSql);
				}
				return iNowOrder;
			}
			catch
			{
				return 0;
			}
		}
		public static PageDataResult ExecuteDataSetPage(PageDataArg args)
		{
			return ExecuteDataSetPage(ConnectionString(), args);
		}
		public static PageDataResult ExecuteDataSetPage(string connectionString, PageDataArg args)
		{
			try
			{
				args.SQL = args.SQL.ToUpper();
				args.Order = args.Order.ToUpper();

				string strSql = "";
				string strCol = "";
				DataRowCollection rows;
				#region
				if (args.SQL.StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase))
				{
					int index = args.SQL.IndexOf("FROM", StringComparison.InvariantCultureIgnoreCase);
					strCol = args.SQL.Substring(6, index - 6);
					args.SQL = " " + args.SQL.Substring(index);
				}
				else
				{
					strCol = "*";
					args.SQL = " FROM " + args.SQL;
				}
				if (args.Count < 1)
				{
					strSql = "SELECT COUNT(1) AS Cnt" + args.SQL;
					rows = MrDBAccess.ExecuteDataSet(connectionString, strSql).Tables[0].Rows;
					if (rows.Count == 0)
						args.Count = 0;
					else
						args.Count = DataBase.ObjectToInt(rows[0]["Cnt"]);
				}
				if (!string.IsNullOrEmpty(args.TurnPageKey) && !string.IsNullOrEmpty(args.TurnPageValue))
				{
					int iNextID = 0;
					args.Page = GetPage(connectionString, args.TurnPageKey, args.SQL, strCol, args.Order, args.TurnPageValue, args.PerPage, ref iNextID);
				}
				#endregion
				args.AllPage = DataBase.ObjectToInt((args.Count / args.PerPage) + (args.Count % args.PerPage != 0 ? 1 : 0));
				if (args.Page > args.AllPage)
					args.Page = args.AllPage;
				if (args.Page < 1)
					args.Page = 1;
				strSql = "BEGIN SELECT IDENTITY(INT,1,1) ROWNUM," + strCol + " INTO #TEMP" + args.SQL + " ORDER BY " + args.Order + ";SELECT * FROM #TEMP WHERE ROWNUM>" + (args.PerPage * (args.Page - 1)) + " AND ROWNUM<=" + (args.PerPage * args.Page) + ";DROP TABLE #TEMP;END;";
				rows = MrDBAccess.ExecuteDataSet(connectionString, strSql).Tables[0].Rows;
				return new PageDataResult() { Count = args.Count, AllPage = args.AllPage, Page = args.Page, Data = rows };
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message, ex);
			}
		}
		public static int ExecuteNonQuery(string commandText)
		{
			return ExecuteNonQuery(ConnectionString(), commandText);
		}
		public static int ExecuteNonQuery(string commandText, params SqlParameter[] commandParameters)
		{
			return ExecuteNonQuery(ConnectionString(), commandText, commandParameters);
		}
		public static int ExecuteNonQuery(string connectionString, string commandText)
		{
			try
			{
				if (STConnection != null)
					return SqlHelper.ExecuteNonQuery(STConnection, STTransaction, CommandType.Text, commandText);
				return SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, commandText);
			}
			catch (Exception exception)
			{
				throw new Exception(exception.Message + "\n" + commandText, exception);
			}
		}
		public static int ExecuteNonQuery(string connectionString, string commandText, params SqlParameter[] commandParameters)
		{
			string strCommand = commandText;
			try
			{
				if (STConnection != null)
				{
					FillParameter(commandParameters);
					return SqlHelper.ExecuteNonQuery(STConnection, STTransaction, CommandType.Text, strCommand, commandParameters);
				}
				FillParameter(commandParameters);
				return SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, strCommand, commandParameters);
			}
			catch (Exception exception)
			{
				throw new Exception(exception.Message + "\n" + strCommand, exception);
			}
		}
		public static DataSet ExecuteProcedure(string spName, params SqlParameter[] commandParameters)
		{
			return ExecuteProcedure(ConnectionString(), spName, commandParameters);
		}
		public static DataSet ExecuteProcedure(string connectionString, string spName, params SqlParameter[] commandParameters)
		{
			FillParameter(commandParameters);
			return SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
		}
		public static int ExecuteProcedureNonQuery(string spName, params SqlParameter[] commandParameters)
		{
			return ExecuteProcedureNonQuery(ConnectionString(), spName, commandParameters);
		}
		public static int ExecuteProcedureNonQuery(string connectionString, string spName, params SqlParameter[] commandParameters)
		{
			int iNum = 0;
			m_Mutex.WaitOne();
			try
			{
				FillParameter(commandParameters);
				return SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			finally
			{
				m_Mutex.ReleaseMutex();
			}
			return iNum;
		}
		private static void FillParameter(params SqlParameter[] commandParameters)
		{
			for (int i = 0; i < commandParameters.Length; i++)
			{
				commandParameters[i].ParameterName = ((commandParameters[i].ParameterName[0] == '@') ? commandParameters[i].ParameterName : ("@" + commandParameters[i].ParameterName));
			}
		}
		public static string InStr(string sSubStr, string sFieldName)
		{
			return InStr(ConnectionString(), sSubStr, sFieldName);
		}
		public static string InStr(string ConnectionString, string sSubStr, string sFieldName)
		{
			return string.Format("CHARINDEX(',{0},',',' + {1} + ',')", sSubStr.Trim(), sFieldName);
		}
		public static string IsNull()
		{
			return IsNull(ConnectionString());
		}
		public static string IsNull(string ConnectionString)
		{
			return "IsNull";
		}
		private static string ParamSQL(string commandText, SqlParameter[] commandParameters)
		{
			string str = commandText;
			string str2 = "";
			for (int i = 0; i < commandParameters.Length; i++)
			{
				str2 = (commandParameters[i].ParameterName[0] == '@') ? commandParameters[i].ParameterName.Substring(1, commandParameters[i].ParameterName.Length - 1) : commandParameters[i].ParameterName;
				str = str.Replace("@" + str2, ":" + str2);
			}
			return str;
		}
		public static void RollbackTransaction()
		{
			if (STTransaction != null)
				STTransaction.Rollback();
			EndTransaction();
		}
		public static string SQLParam(string sParamName)
		{
			return "@" + sParamName;
		}
		public static string UniqueName(string TableName, string ParentFieldName, string NameFieldName, string sParentNodeID, string SeedName, bool bIsInteger, string Condition)
		{
			int i = 0;
			string SeedNames = "";
			string LastName = "";
			int iNum = 10;
			while (true)
			{
				SeedNames = "";
				for (int j = i; j < i + iNum; j++, i++)
				{
					SeedNames += " OR {0}='" + ValidCharValue(SeedName + (i == 0 ? "" : ("(" + i + ")"))) + "'";
				}
				string strSql = string.Format("SELECT {0} AS GName FROM {1} WHERE {2}={3} AND ({4}) {5} ORDER BY {0} ASC", new object[] { NameFieldName, TableName, ParentFieldName, bIsInteger ? sParentNodeID : ("'" + sParentNodeID + "'"), SeedNames.Substring(4), Condition });
				DataRowCollection rows = MrDBAccess.ExecuteDataSet(strSql).Tables[0].Rows;
				if (rows.Count < iNum)
				{
					string tempName = LastName;
					if (rows.Count > 0)
						tempName = DataBase.ObjectToString(rows[rows.Count - 1]["GName"]);
					if (string.IsNullOrEmpty(tempName))
						return SeedName;
					int index = tempName.LastIndexOf("(");
					tempName = tempName.Substring(index + 1);
					i = DataBase.ObjectToInt(tempName.Substring(0, tempName.Length - 1));
					return SeedName + "(" + (i + 1) + ")";
				}
				LastName = DataBase.ObjectToString(rows[iNum - 1]["GName"]);
			}
		}
		public static int CalculatePageIndex(int iPerPageCount, int iRowNum)
		{
			if (iRowNum == 0 || iPerPageCount >= iRowNum)
				return 1;
			return (DataBase.ObjectToInt(iRowNum / iPerPageCount) - (iRowNum % iPerPageCount == 0 ? 0 : -1));
		}
		public static string SqlFormat(string strSql, params object[] objs)
		{
			if (objs != null && objs.Length > 0)
			{
				for (int i = 0; i < objs.Length; i++)
				{
					if (objs[i] is string)
						objs[i] = ValidCharValue(objs[i] as string);
				}
			}
			return string.Format(strSql, objs);
		}
		public static string ValidCharValue(string Value)
		{
			if (Value == null)
				return null;
			return Value.Replace("'", "''");
		}
	}
	public class PageDataArg
	{
		public string SQL { get; set; }
		public string Order { get; set; }
		public int Count { get; set; }
		public int PerPage { get; set; }
		public int Page { get; set; }
		public int AllPage { get; set; }

		public string TurnPageKey { get; set; }
		public string TurnPageValue { get; set; }
	}
	public class PageDataResult
	{
		public DataRowCollection Data { get; set; }
		public int AllPage { get; set; }
		public int Count { get; set; }
		public int Page { get; set; }
	}
}