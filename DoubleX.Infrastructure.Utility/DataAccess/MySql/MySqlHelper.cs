using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DoubleX.Infrastructure.Utility
{
    public class MySqlHelper
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
        public static int ExecuteNonQuery(string connectionString, string commandText, CommandType commandType = CommandType.Text, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                PrepareCommand(conn, cmd, commandType, commandText, commandParameters);
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
        private static void PrepareCommand(MySqlConnection connection, MySqlCommand command, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (connection == null)
                return;

            if (connection.State != ConnectionState.Open)
                connection.Open();

            command.Connection = connection;
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (commandParameters != null)
            {
                command.Parameters.Clear();
                foreach (MySqlParameter parm in commandParameters)
                    command.Parameters.Add(parm);
            }
        }

        #endregion
    }
}
