using EQC.Detection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace EQC.Services
{
    public partial class SqlDB
    {
        
        /// <summary>資料庫連線字串
        /// </summary>
        private  string _connectionString;

        private int _commandTimeOut = 60;

        /// <summary>SqlDB
        /// </summary>
        /// <param name="connectionString">資料庫連線字串</param>
        public SqlDB(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(_connectionString);
        }

        /// <summary>傳入之前使用的Transaction，讓此資料庫存取，使用同一個Transaction
        /// </summary>
        /// <param name="transaction">transaction</param>
        public SqlDB(SqlTransaction transaction)
        {
            this._transaction = transaction;
        }

        #region 資料庫連線 Connection

        /// <summary>SqlConnection
        /// </summary>
        private SqlConnection _connection;

        private static Queue<int> threadPriorityQueue = new Queue<int>();

        /// <summary>取得Connection物件
        /// </summary>
        public SqlConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        /// <summary>開啟連線
        /// </summary>
        public void ConnectionOpen()
        {
            if (Connection.State == ConnectionState.Closed && _transaction == null)
            {

                    Connection.Open();

 
            }
        }

        /// <summary>結束連線
        /// </summary>
        public void ConnectionClose()
        {
            if (Connection.State == ConnectionState.Open && _transaction == null)
            {
                Connection.Close();

            }
        }

        #endregion

        #region 資料庫指令 Command

        /// <summary>取得SqlCommand物件，預設CommandType=Text
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <returns>SqlCommand物件</returns>
        public SqlCommand GetCommand(string sql)
        {
            return GetCommand(sql, CommandType.Text);
        }

        /// <summary>取得SqlCommand物件，預設CommandType=Text
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <param name="arr_sParam">要傳入的參數，名稱與值的對應</param>
        /// <returns>SqlCommand物件</returns>
        public SqlCommand GetCommand(string sql, params string[] arr_sParam)
        {
            return GetCommand(sql, CommandType.Text, arr_sParam);
        }

        /// <summary>取得SqlCommand物件
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <param name="comType">Command類型</param>
        /// <returns>SqlCommand物件</returns>
        public SqlCommand GetCommand(string sql, CommandType comType)
        {
            SqlCommand command = new SqlCommand();
            command.CommandTimeout = _commandTimeOut;
            command.CommandText = sql;
            command.CommandType = comType;
            if (this._transaction == null)
            {
                command.Connection = this.Connection;
            }
            else
            {
                command.Transaction = this._transaction;
            }
            return command;
        }

        /// <summary>取得SqlCommand物件
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <param name="comType">Command類型</param>
        /// <param name="fields">參數名稱與值的對應</param>
        /// <returns>SqlCommand物件</returns>
        public SqlCommand GetCommand(string sql, CommandType comType, NameValueCollection fields)
        {
            SqlCommand command = this.GetCommand(sql, comType);
            command.CommandTimeout = _commandTimeOut;
            if (fields != null)
            {
                for (int i = 0; i < fields.Count; i++)
                {
                    command.Parameters.Add(new SqlParameter(fields.Keys[i].ToString(), fields[i]));
                }
            }
            return command;
        }

        /// <summary>取得SqlCommand物件
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <param name="comType">Command類型</param>
        /// <param name="arr_sParam">參數名稱與值的對應</param>
        /// <returns>SqlCommand物件</returns>
        public SqlCommand GetCommand(string sql, CommandType comType, params string[] arr_sParam)
        {
            SqlCommand command = this.GetCommand(sql, comType);
            command.CommandTimeout = _commandTimeOut;
            for (int iParamIndex = 0; iParamIndex < arr_sParam.Length; iParamIndex++)
            {
                command.Parameters.Add(new SqlParameter(arr_sParam[iParamIndex], arr_sParam[++iParamIndex]));
            }

            return command;
        }

        ///// <summary>取得SqlCommand物件
        ///// </summary>
        ///// <param name="cdataCommand">CDataCommand</param>
        ///// <returns>SqlCommand物件</returns>
        //public SqlCommand getCommand(CDataCommand cdataCommand)
        //{
        //    return this.getCommand(cdataCommand.CommandText,
        //        cdataCommand.CommandType, cdataCommand.getFields());

        //}

        #endregion

        #region 資料庫存取

        /// <summary>執行Command，並傳回異動筆數
        /// </summary>
        /// <param name="command">SqlCommand物件</param>
        /// <returns>執行結果，傳回資料易動筆數</returns>
        public int ExecuteNonQuery(SqlCommand command)
        {
            if (command.Connection == null)
            {
                command.Connection = this.Connection;
            }
            int iValue = 0;
            try
            {
                this.ConnectionOpen();
                iValue = command.ExecuteNonQuery();
                Record(command);
            }
            catch(SqlException ex)
            {
                APIDetection.SetError(Thread.CurrentThread.ManagedThreadId, ex.Message);
                throw ex;
            }
            finally
            {
                this.ConnectionClose();
            }
            return iValue;
        }

        /// <summary>執行Command，並傳回執行結果
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <returns>執行結果，傳回資料易動筆數</returns>
        public int ExecuteNonQuery(string sql)
        {
            SqlCommand command = this.GetCommand(sql);
            return this.ExecuteNonQuery(command);
        }

        /// <summary>執行Command，並傳回執行結果
        /// </summary>
        /// <param name="command">SqlCommand物件</param>
        /// <returns>執行結果，只傳回第一筆第一列的資料</returns>
        public object ExecuteScalar(SqlCommand command)
        {
            if (command.Connection == null)
            {
                command.Connection = this.Connection;
            }
            object obj = null;
            try
            {
                this.ConnectionOpen();
                obj = command.ExecuteScalar();
            }
            catch
            {
          
                throw ;
            }
            finally
            {
                this.ConnectionClose();
            }
            return obj;
        }

        /// <summary>執行Command，並傳回執行結果
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <returns>執行結果，只傳回第一筆第一列的資料</returns>
        public object ExecuteScalar(string sql)
        {
            SqlCommand command = this.GetCommand(sql);
            return this.ExecuteScalar(command);
        }

        /// <summary>取得資料
        /// </summary>
        /// <param name="command">SqlCommand物件</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(SqlCommand command)
        {
            if (command.Connection == null)
            {
                command.Connection = this.Connection;
            }
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                this.ConnectionClose();
            }

            return dt;
        }

        /// <summary>取得DataSet資料
        /// </summary>
        /// <param name="command">SqlCommand物件</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(SqlCommand command)
        {
            if (command.Connection == null)
            {
                command.Connection = this.Connection;
            }
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ConnectionClose();
            }
            return ds;
        }

        /// <summary>填滿資料到傳入的DataTable
        /// </summary>
        /// <param name="dt">要填滿的DataTable</param>
        /// <param name="command">SqlCommand物件</param>
        public void FillDataTable(DataTable dt, string command)
        {
            FillDataTable(dt, this.GetCommand(command));
        }

        /// <summary>依傳入的DS結構，來填資料。
        /// </summary>
        /// <param name="dt">DT結構</param>
        /// <param name="command">SqlCommand物件</param>
        public void FillDataTable(DataTable dt, SqlCommand command)
        {
            if (command.Connection == null)
            {
                command.Connection = this.Connection;
            }
            SqlDataAdapter da = new SqlDataAdapter(command);
            try
            {
                da.Fill(dt);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ConnectionClose();
            }
        }

        /// <summary>依傳入的DataSet結構，來填資料。
        /// </summary>
        /// <param name="ds">DataSet結構</param>
        /// <param name="command">SqlCommand物件</param>
        public void FillDataSet(DataSet ds, SqlCommand command)
        {
            if (command.Connection == null)
            {
                command.Connection = this.Connection;
            }
            SqlDataAdapter da = new SqlDataAdapter(command);
            try
            {
                da.Fill(ds);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ConnectionClose();
            }

            //if (command.Connection == null)
            //    command.Connection = this.Connection;
            //try
            //{
            //    this.ConnectionOpen();
            //    SqlDataReader dr = command.ExecuteReader();
            //    foreach (DataTable dt in ds.Tables)
            //    {
            //        dt.Load(dr);
            //        //dr.NextResult();//此行不需要，Load會自動next
            //    }
            //    dr.Close();
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            //finally
            //{
            //    this.ConnectionClose();
            //}
        }

        /// <summary>將查詢結果轉換至List
        /// </summary>
        /// <typeparam name="T">目標Model</typeparam>
        /// <param name="command">查詢語法</param>
        /// <returns></returns>
        public List<T> GetDataTableWithClass<T>(SqlCommand command)
        {


            //if (!threadPriorityQueue.Contains(Thread.CurrentThread.ManagedThreadId))
            //    threadPriorityQueue.Enqueue(Thread.CurrentThread.ManagedThreadId);

            //while (threadPriorityQueue.Count > 0 && threadPriorityQueue.Peek() != Thread.CurrentThread.ManagedThreadId )
            //{
            //    Thread.Sleep(500);
            //}
             ConnectionOpen();

            List<T> targetList = new List<T>();
            command.Connection = _connection;
            SqlDataReader reader = null;
            using (command)
            {
                try
                {
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                    //Type underlyingType = typeof(T);
                    while (reader.Read())
                    {
                        T targetClass = Activator.CreateInstance<T>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = targetClass.GetType().GetProperty(reader.GetName(i));
                            if (property != null)
                            {
                                property.SetValue(targetClass, (reader.IsDBNull(i)) ? null : reader.GetValue(i), null);
                            }
                        }
                        targetList.Add(targetClass);
                    }

                }
                finally
                {
                    //if (threadPriorityQueue.Peek() == Thread.CurrentThread.ManagedThreadId)
                    //{
                    //    ConnectionClose();
                    //    threadPriorityQueue.Dequeue();
                    //}
                    reader?.Close();
                }


                return targetList;
            }
        }

        /// <summary>將查詢結果轉換至Model
        /// </summary>
        /// <typeparam name="T">目標Model</typeparam>
        /// <param name="cmd">查詢語法</param>
        /// <returns></returns>
        public T TransformToType<T>(SqlCommand cmd)
        {
            _connection.Open();
            cmd.Connection = _connection;
            T targetClass = Activator.CreateInstance<T>();

            using (cmd)
            {
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                //Type underlyingType = typeof(T);
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        PropertyInfo property = targetClass.GetType().GetProperty(reader.GetName(i));
                        if (property != null)
                        {
                            property.SetValue(targetClass, (reader.IsDBNull(i)) ? null : reader.GetValue(i), null);
                        }
                    }
                }
                _connection.Close();

                return targetClass;
            }
        }

        #endregion

        #region 交易 Transaction

        /// <summary>SqlTransaction
        /// </summary>
        private SqlTransaction _transaction;

        /// <summary>取得Transaction物件
        /// </summary>
        public SqlTransaction Transaction
        {
            get
            {
                //不使用，原因是需要明確BeginTransaction，才會有Transaction，不應該自動建立
                //否則使用上會不明確
                //且外界可依 Transaction == null 來判別是否有用Transaction
                //if (_transaction == null)
                //    _transaction = this.BeginTransaction();
                return _transaction;
            }
        }

        /// <summary>開始交易，並傳回Transaction物件
        /// </summary>
        /// <returns>Transaction物件</returns>
        public SqlTransaction BeginTransaction()
        {
            this.ConnectionOpen();
            this._transaction = this.Connection.BeginTransaction();
            return _transaction;
        }

        /// <summary>還原交易
        /// </summary>
        public void TransactionRollback()
        {
            if (_transaction != null)
            {
                this._transaction.Rollback();
            }
            this.ConnectionClose();
            this._transaction = null;
        }

        /// <summary>確認交易
        /// </summary>
        public void TransactionCommit()
        {
            if (_transaction != null)
            {
                this._transaction.Commit();
            }
            this.ConnectionClose();
            this._transaction = null;
        }
        #endregion

        public class ColumnType {

            public string TABLE_SCHEMA { get; set; }
            public string TABLE_NAME { get; set; }
            public int ORDINAL_POSITION { get; set; }
            public string COLUMN_NAME { get; set; }
            public string FULL_DATA_TYPE { get; set; }

        }
        public static Dictionary<string, string> getTableColumnType(string tableName)
        {
            string sql = @"
                WITH q AS (
    
                    SELECT
                        c.TABLE_SCHEMA,
                        c.TABLE_NAME,
                        c.ORDINAL_POSITION,
                        c.COLUMN_NAME,
                        c.DATA_TYPE,
                        CASE
                            WHEN c.DATA_TYPE IN ( N'binary', N'varbinary'                    ) THEN ( CASE c.CHARACTER_OCTET_LENGTH   WHEN -1 THEN N'(max)' ELSE CONCAT( N'(', c.CHARACTER_OCTET_LENGTH  , N')' ) END )
                            WHEN c.DATA_TYPE IN ( N'char', N'varchar', N'nchar', N'nvarchar' ) THEN ( CASE c.CHARACTER_MAXIMUM_LENGTH WHEN -1 THEN N'(max)' ELSE CONCAT( N'(', c.CHARACTER_MAXIMUM_LENGTH, N')' ) END )
                            WHEN c.DATA_TYPE IN ( N'datetime2', N'datetimeoffset'            ) THEN CONCAT( N'(', c.DATETIME_PRECISION, N')' )
                            WHEN c.DATA_TYPE IN ( N'decimal', N'numeric'                     ) THEN CONCAT( N'(', c.NUMERIC_PRECISION , N',', c.NUMERIC_SCALE, N')' )
                        END AS DATA_TYPE_PARAMETER,
                        CASE c.IS_NULLABLE
                            WHEN N'NO'  THEN N' NOT NULL'
                            WHEN N'YES' THEN     N' NULL'
                        END AS IS_NULLABLE2
                    FROM
                        INFORMATION_SCHEMA.COLUMNS AS c
                )
                SELECT
                    q.TABLE_SCHEMA,
                    q.TABLE_NAME,
                    q.ORDINAL_POSITION,
                    q.COLUMN_NAME,
                    CONCAT( q.DATA_TYPE, ISNULL( q.DATA_TYPE_PARAMETER, N'' ), q.IS_NULLABLE2 ) AS FULL_DATA_TYPE

                FROM
                    q
                WHERE
                    q.TABLE_SCHEMA = 'dbo' AND
                    q.TABLE_NAME   =  @TableName
                ORDER BY
                    q.TABLE_SCHEMA,
                    q.TABLE_NAME,
                    q.ORDINAL_POSITION;
            ";
            var sqldb = new SqlDB(ConfigManager.EQC_ConnString);
            var cmd = sqldb.GetCommand(sql);
            cmd.Parameters.AddWithValue("@TableName", tableName);
            return sqldb
                .GetDataTableWithClass<ColumnType>(cmd)
                .ToDictionary(r => r.COLUMN_NAME, r => r.FULL_DATA_TYPE);
        }
    }
}