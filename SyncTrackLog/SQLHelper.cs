using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Configuration;
using System.Collections.Generic;

namespace SQL
{
    public class SQLHelper
    {
        #region Global_Declaration

        public static string connectionstring = string.Empty;

        #endregion

        #region Constructor

        public SQLHelper()
        {
            connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings[1].ToString();
        }

        #endregion

        #region ExecuteNonQuery

        public int ExecuteNonQuery(String sp_name)
        {
            return ExecuteNonQuery(sp_name, (SqlParameter[])null, (String)null);
        }
        public int ExecuteNonQuery(String sp_name, SqlParameter[] param)
        {
            return ExecuteNonQuery(sp_name, param, (String)null);
        }

        public int ExecuteNonQuery(String sp_name, SqlParameter[] param, String outputparam_name)
        {
            int result = 0;

            connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings[1].ToString();
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = new SqlCommand(sp_name, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            try
            {
                if (param != null)
                {
                    for (int i = 0; i < param.Length; i++)
                    {
                        //cmd.Parameters.AddWithValue(param[i].ParameterName, param[i].Value);
                        cmd.Parameters.Add(param[i]);
                    }
                }
                SqlConnection.ClearPool(conn);
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                else
                    conn.Open();

                if (outputparam_name != null)
                {
                    SqlParameter outparam = new SqlParameter(outputparam_name, SqlDbType.Int);
                    outparam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outparam);
                    cmd.ExecuteNonQuery();
                    result = (int)outparam.Value;
                }
                else
                    result = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Parameters.Clear();
                cmd.Dispose();
                cmd = null;
                conn.Close();
                conn = null;
            }
            return result;
        }
        
        #endregion
        public int ExecuteQuery(String Qry)
        {
            return ExecuteQuery(Qry, null);
        }
        public int ExecuteQuery(String Qry, SqlParameter[] param)
        {
            int result = 0;
            connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings[1].ToString();
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = new SqlCommand(Qry, conn);
            cmd.CommandType = CommandType.Text;
            try
            {
                if (param != null)
                {
                    for (int i = 0; i < param.Length; i++)
                    {
                        cmd.Parameters.Add(param[i]);
                    }
                }
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                else
                    conn.Open();
                result = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                cmd = null;
                conn.Close();
                conn = null;
            }
            return result;
        }       
        #region ExecuteScaler

        public object ExecuteScalar(String sp_name)
        {
            return ExecuteScalar(sp_name, (SqlParameter[])null);
        }

        public object ExecuteScalar(String sp_name, SqlParameter[] param)
        {
            object result = null;
            connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings[1].ToString();
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = new SqlCommand(sp_name, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                if (param != null)
                {
                    for (int i = 0; i < param.Length; i++)
                        cmd.Parameters.AddWithValue(param[i].ParameterName, param[i].Value);
                }
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                else
                    conn.Open();

                result = cmd.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Parameters.Clear();
                cmd.Dispose();
                cmd = null;
                conn.Close();
                conn = null;
            }
            return result;
        }
        public object ExecuteScalarSQL(String sql)
        {
            object result = null;
            connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings[1].ToString();
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            try
            {                
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                else
                    conn.Open();

                result = cmd.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                cmd = null;
                conn.Close();
                conn = null;
            }
            return result;
        }

        #endregion

        #region ExecuteReader

        public SqlDataReader ExecuteReader(String sp_name)
        {
            return ExecuteReader(sp_name, (SqlParameter[])null);
        }

        public SqlDataReader ExecuteReader(String sp_name, SqlParameter[] param)
        {
            SqlDataReader reader;
            connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings[1].ToString();
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = new SqlCommand(sp_name, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                if (param != null)
                {
                    for (int i = 0; i < param.Length; i++)
                        cmd.Parameters.AddWithValue(param[i].ParameterName, param[i].Value);
                }
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                else
                    conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Parameters.Clear();
                cmd.Dispose();
                cmd = null;
                //conn.Close();
                conn = null;
            }
            return reader;
        }
        public SqlDataReader ExecuteReaderSQL(String sql)
        {
            return ExecuteReaderSQL(sql, null);
        }            
        public SqlDataReader ExecuteReaderSQL(String sql, SqlParameter[] param)
        {
            SqlDataReader reader;
            connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings[1].ToString();
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            try
            {
                if (param != null)
                {
                    for (int i = 0; i < param.Length; i++)
                        cmd.Parameters.AddWithValue(param[i].ParameterName, param[i].Value);
                }
                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Parameters.Clear();
                cmd.Dispose();
                cmd = null;
                //conn.Close();
                conn = null;
            }
            return reader;
        }
        #endregion

        #region ExecuteDataTable

        public DataTable ExecuteDataTableSql(String sql)
        {
            return ExecuteDataTableSql(sql, null);
        }
        public DataTable ExecuteDataTableSql(String sql, SqlParameter[] param)
        {
            DataTable Dt = new DataTable();
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand =cmd;
            try
            {
                if (param != null)
                {
                    for (int i = 0; i < param.Length; i++)
                        cmd.Parameters.AddWithValue(param[i].ParameterName, param[i].Value);
                }
                conn.Open();
                adp.Fill(Dt);                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                adp.Dispose();
                conn.Close();
                conn.Dispose();
            }            
            return Dt;
        }

        public DataTable ExecuteDataTable(String sp_name)
        {
            return ExecuteDataTable(sp_name, (SqlParameter[])null);
        }

        public DataTable ExecuteDataTable(String sp_name, SqlParameter[] param)
        {
            DataTable dt = new DataTable();
            connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings[1].ToString();
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = new SqlCommand(sp_name, conn);
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                if (param != null)
                {
                    for (int i = 0; i < param.Length; i++)
                        cmd.Parameters.AddWithValue(param[i].ParameterName, param[i].Value);
                }
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                else
                    conn.Open();

                da.Fill(dt);
                da.Dispose();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Parameters.Clear();
                cmd.Dispose();
                cmd = null;
                conn.Close();
                conn.Dispose();
                conn = null;
            }
            return dt;
        }

        #endregion

        #region ExecuteDataSet

        public DataSet ExecuteDataSet(String sp_name)
        {
            return ExecuteDataSet(sp_name, (SqlParameter[])null);
        }

        public DataSet ExecuteDataSet(String sp_name, SqlParameter[] param)
        {
            DataSet ds = new DataSet();
            connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings[1].ToString();
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = new SqlCommand(sp_name, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                if (param != null)
                {
                    for (int i = 0; i < param.Length; i++)
                        cmd.Parameters.AddWithValue(param[i].ParameterName, param[i].Value);
                }
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                else
                    conn.Open();

                da.Fill(ds);
                da.Dispose();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Parameters.Clear();
                cmd.Dispose();
                cmd = null;
                conn.Close();
                conn = null;
            }
            return ds;
        }

        #endregion
    }
}
