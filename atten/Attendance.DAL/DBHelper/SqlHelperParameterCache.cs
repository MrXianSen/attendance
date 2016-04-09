using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Attendance.DAL.DBHelper
{
	public sealed class SqlHelperParameterCache
	{
		// Fields
		private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());
		private static Hashtable paramDirections = Hashtable.Synchronized(new Hashtable());
		private static Hashtable paramTypes = Hashtable.Synchronized(new Hashtable());
		// Methods
		static SqlHelperParameterCache()
		{
			paramTypes.Add("bigint", SqlDbType.BigInt);
			paramTypes.Add("binary", SqlDbType.Binary);
			paramTypes.Add("bit", SqlDbType.Bit);
			paramTypes.Add("char", SqlDbType.Char);
			paramTypes.Add("datetime", SqlDbType.DateTime);
			paramTypes.Add("decimal", SqlDbType.Decimal);
			paramTypes.Add("float", SqlDbType.Float);
			paramTypes.Add("image", SqlDbType.Image);
			paramTypes.Add("int", SqlDbType.Int);
			paramTypes.Add("money", SqlDbType.Money);
			paramTypes.Add("nchar", SqlDbType.NChar);
			paramTypes.Add("ntext", SqlDbType.NText);
			paramTypes.Add("numeric", SqlDbType.Decimal);
			paramTypes.Add("nvarchar", SqlDbType.NVarChar);
			paramTypes.Add("real", SqlDbType.Real);
			paramTypes.Add("smalldatetime", SqlDbType.SmallDateTime);
			paramTypes.Add("smallint", SqlDbType.SmallInt);
			paramTypes.Add("smallmoney", SqlDbType.SmallMoney);
			paramTypes.Add("sql_variant", SqlDbType.Variant);
			paramTypes.Add("text", SqlDbType.Text);
			paramTypes.Add("timestamp", SqlDbType.Timestamp);
			paramTypes.Add("tinyint", SqlDbType.TinyInt);
			paramTypes.Add("uniqueidentifier", SqlDbType.UniqueIdentifier);
			paramTypes.Add("varbinary", SqlDbType.VarBinary);
			paramTypes.Add("varchar", SqlDbType.VarChar);
			paramDirections.Add((short)1, ParameterDirection.Input);
			paramDirections.Add((short)2, ParameterDirection.InputOutput);
			paramDirections.Add((short)4, ParameterDirection.ReturnValue);
		}
		public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
		{
			string str = connectionString + ":" + commandText;
			paramCache[str] = commandParameters;
		}
		private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
		{
			SqlParameter[] parameterArray = new SqlParameter[originalParameters.Length];
			int index = 0;
			int length = originalParameters.Length;
			while (index < length)
			{
				parameterArray[index] = originalParameters[index];//.Clone()
				index++;
			}
			return parameterArray;
		}
		private static SqlParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			SqlParameter[] parameterArray;
			int num;
			DataTable table = new DataTable("paramDescriptions");
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				SqlCommand command = new SqlCommand("sp_procedure_params_rowset", connection);
				command.CommandTimeout = 3600;
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.Add("@procedure_name", spName);
				new SqlDataAdapter(command).Fill(table);
			}
			if (table.Rows.Count <= 0)
				throw new ArgumentException("Stored procedure '" + spName + "' not found", "spName");

			if (includeReturnValueParameter)
			{
				parameterArray = new SqlParameter[table.Rows.Count];
				num = 0;
			}
			else
			{
				parameterArray = new SqlParameter[table.Rows.Count - 1];
				num = 1;
			}
			int index = 0;
			int length = parameterArray.Length;
			while (index < length)
			{
				DataRow row = table.Rows[index + num];
				parameterArray[index] = new SqlParameter();
				parameterArray[index].ParameterName = (string)row["PARAMETER_NAME"];
				parameterArray[index].SqlDbType = (SqlDbType)paramTypes[(string)row["TYPE_NAME"]];
				parameterArray[index].Direction = (ParameterDirection)paramDirections[(short)row["PARAMETER_TYPE"]];
				parameterArray[index].Size = ((row["CHARACTER_OCTET_LENGTH"] == DBNull.Value) ? 0 : (int)row["CHARACTER_OCTET_LENGTH"]);
				parameterArray[index].Precision = ((row["NUMERIC_PRECISION"] == DBNull.Value) ? (byte)0 : (byte)(row["NUMERIC_PRECISION"]));
				parameterArray[index].Scale = ((row["NUMERIC_SCALE"] == DBNull.Value) ? (byte)0 : (byte)(row["NUMERIC_SCALE"]));
				index++;
			}
			return parameterArray;
		}
		public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			string str = connectionString + ":" + commandText;
			SqlParameter[] originalParameters = (SqlParameter[])paramCache[str];
			if (originalParameters == null)
				return null;

			return CloneParameters(originalParameters);
		}
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
		{
			return GetSpParameterSet(connectionString, spName, false);
		}
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			string str = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");
			SqlParameter[] originalParameters = (SqlParameter[])paramCache[str];
			if (originalParameters == null)
			{
				object obj = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter);
				paramCache[str] = obj;
				originalParameters = (SqlParameter[])obj;
			}
			return CloneParameters(originalParameters);
		}
	}
}