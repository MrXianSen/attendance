using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Xml;

namespace Attendance.DAL.DBHelper
{
	public sealed class SqlHelper
	{
		private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
		{
			if ((commandParameters != null) && (parameterValues != null))
			{
				if (commandParameters.Length != parameterValues.Length)
					throw new ArgumentException("Parameter count does not match Parameter Value count.");

				int index = 0;
				int length = commandParameters.Length;
				while (index < length)
				{
					commandParameters[index].Value = parameterValues[index];
					index++;
				}
			}
		}
		private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
		{
			foreach (SqlParameter parameter in commandParameters)
			{
				if ((parameter.Direction == ParameterDirection.InputOutput) && (parameter.Value == null))
					parameter.Value = DBNull.Value;

				command.Parameters.Add(parameter);
			}
		}
		public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
		{
			return ExecuteDataset(connection, null, commandType, commandText);
		}
		public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
		{
			return ExecuteDataset(connection, null, spName, parameterValues);
		}
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
		{
			return ExecuteDataset(connectionString, commandType, commandText, null);
		}
		public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
		{
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				AssignParameterValues(spParameterSet, parameterValues);
				return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
		}
		public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			return ExecuteDataset(connection, null, commandType, commandText, commandParameters);
		}
		public static DataSet ExecuteDataset(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return ExecuteDataset(connection, transaction, commandType, commandText, null);
		}
		public static DataSet ExecuteDataset(SqlConnection connection, SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				AssignParameterValues(spParameterSet, parameterValues);
				return ExecuteDataset(connection, transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return ExecuteDataset(connection, transaction, CommandType.StoredProcedure, spName);
		}
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				return ExecuteDataset(connection, commandType, commandText, commandParameters);
			}
		}
		public static DataSet ExecuteDataset(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand command = new SqlCommand();
			PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters);
			SqlDataAdapter adapter = new SqlDataAdapter(command);
			DataSet ds = new DataSet();
			adapter.Fill(ds);
			return ds;
		}
		public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
		{
			return ExecuteNonQuery(connection, null, commandType, commandText);
		}
		public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
		{
			return ExecuteNonQuery(connection, null, spName, parameterValues);
		}
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
		{
			return ExecuteNonQuery(connectionString, commandType, commandText, null);
		}
		public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
		{
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				AssignParameterValues(spParameterSet, parameterValues);
				return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
		}
		public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			return ExecuteNonQuery(connection, null, commandType, commandText, commandParameters);
		}
		public static int ExecuteNonQuery(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return ExecuteNonQuery(connection, transaction, commandType, commandText, null);
		}
		public static int ExecuteNonQuery(SqlConnection connection, SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				AssignParameterValues(spParameterSet, parameterValues);
				return ExecuteNonQuery(connection, transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return ExecuteNonQuery(connection, transaction, CommandType.StoredProcedure, spName);
		}
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
			}
		}
		public static int ExecuteNonQuery(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand command = new SqlCommand();
			PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters);
			return command.ExecuteNonQuery();
		}
		public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
		{
			return ExecuteReader(connection, null, commandType, commandText);
		}
		public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
			return ExecuteReader(connection, null, spName, parameterValues);
		}
		public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
		{
			return ExecuteReader(connectionString, commandType, commandText, null);
		}
		public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
		{
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				AssignParameterValues(spParameterSet, parameterValues);
				return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
		}
		public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			return ExecuteReader(connection, null, commandType, commandText, commandParameters);
		}
		public static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return ExecuteReader(connection, transaction, commandType, commandText, null);
		}
		public static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				AssignParameterValues(spParameterSet, parameterValues);
				return ExecuteReader(connection, transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return ExecuteReader(connection, transaction, CommandType.StoredProcedure, spName);
		}
		public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlDataReader reader;
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			try
			{
				reader = ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
			}
			catch
			{
				connection.Close();
				throw;
			}
			return reader;
		}
		public static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			return ExecuteReader(connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
		}
		private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
		{
			SqlCommand command = new SqlCommand();
			PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters);
			if (connectionOwnership == SqlConnectionOwnership.External)
				return command.ExecuteReader();

			return command.ExecuteReader(CommandBehavior.CloseConnection);
		}
		public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
		{
			return ExecuteScalar(connection, null, commandType, commandText);
		}
		public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
		{
			return ExecuteScalar(connection, null, spName, parameterValues);
		}
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
		{
			return ExecuteScalar(connectionString, commandType, commandText, null);
		}
		public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
		{
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				AssignParameterValues(spParameterSet, parameterValues);
				return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
		}
		public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			return ExecuteScalar(connection, null, commandType, commandText, commandParameters);
		}
		public static object ExecuteScalar(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return ExecuteScalar(connection, transaction, commandType, commandText, null);
		}
		public static object ExecuteScalar(SqlConnection connection, SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				AssignParameterValues(spParameterSet, parameterValues);
				return ExecuteScalar(connection, transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return ExecuteScalar(connection, transaction, CommandType.StoredProcedure, spName);
		}
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				return ExecuteScalar(connection, commandType, commandText, commandParameters);
			}
		}
		public static object ExecuteScalar(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand command = new SqlCommand();
			PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters);
			return command.ExecuteScalar();
		}
		public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
		{
			return ExecuteXmlReader(connection, null, commandType, commandText);
		}
		public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
			return ExecuteXmlReader(connection, null, spName, parameterValues);
		}
		public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			return ExecuteXmlReader(connection, null, commandType, commandText, commandParameters);
		}
		public static XmlReader ExecuteXmlReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return ExecuteXmlReader(connection, transaction, commandType, commandText, null);
		}
		public static XmlReader ExecuteXmlReader(SqlConnection connection, SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				AssignParameterValues(spParameterSet, parameterValues);
				return ExecuteXmlReader(connection, transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return ExecuteXmlReader(connection, transaction, CommandType.StoredProcedure, spName);
		}
		public static XmlReader ExecuteXmlReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand command = new SqlCommand();
			PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters);
			return command.ExecuteXmlReader();
		}
		public static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
		{
			if (connection.State != ConnectionState.Open)
				connection.Open();
			command.CommandTimeout = 3600;
			command.Connection = connection;
			command.CommandText = commandText;
			if (transaction != null)
				command.Transaction = transaction;
			command.CommandType = commandType;
			if (commandParameters != null)
				AttachParameters(command, commandParameters);
		}
		// Nested Types
		private enum SqlConnectionOwnership
		{
			Internal,
			External
		}
	}
}