using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// SQLServer 数据库辅助类
    /// </summary>
    public class SQLServerHelper
    {
        #region ExecuteNonQuery

        /// <summary>
        /// 执行SQL语句，返回被操作的行数
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="commandText">执行语句或存储过程名</param>
        /// <param name="commandType">执行类型</param>
        /// <param name="cmdParms">SQL参数对象</param>
        /// <returns>所受影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(conn, cmd, commandType, commandText, false, null, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 预处理Command对象,数据库链接,事务,需要执行的对象,参数等的初始化
        /// </summary>
        /// <param name="conn">Connection对象</param>
        /// <param name="cmd">Command对象</param>
        /// <param name="cmdType">SQL字符串执行类型</param>
        /// <param name="cmdText">SQL Text</param>
        /// <param name="useTrans">是否使用事务</param>
        /// <param name="trans">Transcation对象</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        private static void PrepareCommand(SqlConnection conn, SqlCommand cmd, CommandType cmdType, string cmdText, bool useTrans, SqlTransaction trans, params SqlParameter[] cmdParms)
        {
            if (conn == null)
                return;

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (useTrans)
            {
                if (trans == null)
                {
                    trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                }
                cmd.Transaction = trans;
            }

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        #endregion
    }

}
