using SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Newtonsoft.Json;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.ServiceModel.Web;
using System.Reflection;

namespace SyncTrackLog
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SyncTrackLogService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SyncTrackLogService.svc or SyncTrackLogService.svc.cs at the Solution Explorer and start debugging.
    public class SyncTrackLogService : ISyncTrackLogService
    {
        public object JsonConvert { get; private set; }

        public string GetInterval()
        {
            return "{LocationInterval : 40, SyncInterval : 90 }";

        }

        //public string SendTrackLog(List<LocationLog> log)
        //{



        //    return log.Count + " records inserted successfully";
        //}


        public void WebService_UserLog(string outParam1, string dtTracklog)
        {
            string User = HttpContext.Current.Session["User"].ToString();
            int result = 0;
            string strSql = "SyncTracklogData";

            SqlParameter[] SqlParam = new SqlParameter[2];
            try
            {
                SqlParam[0] = new SqlParameter("@dtTracklog", dtTracklog);
                SqlParam[1] = new SqlParameter("@outParam1", outParam1);

                SQLHelper objSQL = new SQLHelper();
                result = objSQL.ExecuteNonQuery(strSql, SqlParam);
                objSQL = null;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return result;
        }


        //private DataSet ConvertJsonStringToDataSet(string jsonString)
        //{
        //    try
        //    {
        //        XmlDocument xd = new XmlDocument();
        //        jsonString = "{ \"rootNode\": {" + jsonString.Trim().TrimStart('{').TrimEnd('}') + "} }";
        //        xd = (XmlDocument)JsonConvert.DeserializeXmlNode(jsonString);
        //        DataSet ds = new DataSet();
        //        ds.ReadXml(new XmlNodeReader(xd));
        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ArgumentException(ex.Message);
        //    }
        //}



        public SqlParameter[] ExecNonQueryWithOutParameter(string strProc, SqlParameter[] parCollection = null)
        {
            bool result = false;
            int count = 0;
            SqlConnection lCon;
            lCon = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Scon"].ToString());
            lCon.Open();
            SqlParameter[] sqlPara = new SqlParameter[2];
            try
            {

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = lCon;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = strProc;

                if (parCollection != null)
                {
                    for (int i = 0; i < parCollection.Length; i++)
                    {
                        if (parCollection[i] != null)
                        {
                            cmd.Parameters.AddWithValue(parCollection[i].ParameterName, parCollection[i].Value);
                            cmd.Parameters[i - count].Direction = parCollection[i].Direction;
                            cmd.Parameters[i - count].SqlDbType = parCollection[i].SqlDbType;
                            cmd.Parameters[i - count].Size = parCollection[i].Size;
                        }
                        else
                        {
                            count++;
                        }
                    }
                }

                if (cmd.ExecuteNonQuery() > 0)
                    result = true;
                else
                    result = false;

                sqlPara[0] = new SqlParameter("result", result);    //set ExecuteNonQuery result                         
                sqlPara[1] = new SqlParameter("outParam1", cmd.Parameters["@outParam1"].Value); //set out parameter value
                lCon.Close();
                lCon.Dispose();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (lCon != null)
                {
                    lCon.Close();
                    lCon.Dispose();
                }
            }
            return sqlPara;
        }

        public Stream GetJSONString(string status)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer =
             new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, string>> rows =
           new List<Dictionary<string, string>>();
            Dictionary<string, string> row = null;
            row = new Dictionary<string, string>();
            row.Add("status", status);
            rows.Add(row);
           // return serializer.Serialize(rows);
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            return new MemoryStream(Encoding.UTF8.GetBytes(serializer.Serialize(rows)));
        }
        public DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            if (jsonStringArray[0].ToString() != string.Empty)
            {
                List<string> ColumnsName = new List<string>();
                foreach (string jSA in jsonStringArray)
                {
                    string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                    foreach (string ColumnsNameData in jsonStringData)
                    {
                        try
                        {
                            int idx = ColumnsNameData.IndexOf(":");
                            string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                            if (!ColumnsName.Contains(ColumnsNameString))
                            {
                                ColumnsName.Add(ColumnsNameString);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                        }
                    }
                    break;
                }
                foreach (string AddColumnName in ColumnsName)
                {
                    dt.Columns.Add(AddColumnName);
                }
                foreach (string jSA in jsonStringArray)
                {
                    string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                    DataRow nr = dt.NewRow();
                    foreach (string rowData in RowData)
                    {
                        try
                        {
                            int idx = rowData.IndexOf(":");
                            string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
                            string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                            if (RowDataString != "null")
                            {
                                nr[RowColumns] = RowDataString;
                            }
                            else
                            {
                                nr[RowColumns] = null;
                            }

                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                    dt.Rows.Add(nr);
                }
            }
            return dt;
        }

        //public DataTable ToDataTable<T>(List<T> items)
        //{
        //    DataTable dataTable = new DataTable(typeof(T).Name);
        //    //Get all the properties by using reflection   
        //    PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    foreach (PropertyInfo prop in Props)
        //    {
        //        //Setting column names as Property names  
        //        dataTable.Columns.Add(prop.Name);
        //    }
        //    foreach (T item in items)
        //    {
        //        var values = new object[Props.Length];
        //        for (int i = 0; i < Props.Length; i++)
        //        {

        //            values[i] = Props[i].GetValue(item, null);
        //        }
        //        dataTable.Rows.Add(values);
        //    }

        //    return dataTable;
        //}
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
           
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }


        static DataTable ConvertListToDataTable(List<string[]> list)
        {
            // New table.
            DataTable table = new DataTable();

            // Get max columns.
            int columns = 0;
            foreach (var array in list)
            {
                if (array.Length > columns)
                {
                    columns = array.Length;
                }
            }

            // Add columns.
            for (int i = 0; i < columns; i++)
            {
                table.Columns.Add();
            }

            // Add rows.
            foreach (var array in list)
            {
                table.Rows.Add(array);
            }

            return table;
        }

        public Stream SendTrackLog(Stream log)
        {
            StreamReader reader = new StreamReader(log);
            string text = reader.ReadToEnd();


            //   string res = Encoding.UTF8.GetString(Tracklog.GetBuffer(), 0, Tracklog.GetBuffer().Length)




            // DataTable dtTracklog = ToDataTable(log);
            DataTable dtTracklog = JsonStringToDataTable(text);

            Boolean status = false;
            try
            {
                SqlParameter[] parCollection = new SqlParameter[4];

                if (dtTracklog.Rows.Count >= 1)
                {
                    parCollection[1] = new SqlParameter("@dtTracklog", dtTracklog);
                }
                else
                {
                    parCollection[1] = null;
                }

                parCollection[2] = new SqlParameter("@outParam1", "-99"); //value is not important here as this is out parameter 
                parCollection[2].Direction = ParameterDirection.Output;
                parCollection[2].SqlDbType = SqlDbType.Int;
                SqlParameter[] parCol = new SqlParameter[2];

                parCol = ExecNonQueryWithOutParameter("SyncTracklogData", parCollection);
                if ((bool)parCol[0].Value == true) //result of exeNonQuery function from datalogic
                {
                    status = Convert.ToBoolean(parCol[1].Value); //set out parameter value into variable

                }
                else //if executeNonQuery() Fails
                {
                    status = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return GetJSONString(status.ToString());

        }
    }

    public class ListtoDataTable
    {
        public DataTable ToDataTable<T>(List<T> items)
        {

            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }

}
