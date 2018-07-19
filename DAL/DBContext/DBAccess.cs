using DAL.ImportControl;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL.DBContext
{
    public class DBAccess
    {
        //WriteLogFile wlf = new WriteLogFile();
        //readonly ImportControl.ImportControlRepository ipr = new ImportControl.ImportControlRepository();

        public enum DBConnection : int
        {
            SqlMainNew,
            SqlProd01,
            NameSys
        }

        private SqlConnection DataConnection(DBConnection dataConnectionIndicator)
        {
            SqlConnection cnnRtn = null;
            try
            {
                switch (dataConnectionIndicator)
                {
                    case DBConnection.SqlMainNew:
                        cnnRtn = SqlMainNewDBConnection;
                        break;
                    case DBConnection.SqlProd01:
                        cnnRtn = SqlProd01DBConnection;
                        break;
                    case DBConnection.NameSys:
                        cnnRtn = SqlNameSysDBConnection;
                        break;
                }
            }
            catch (Exception ex)
            {
                DAL.ImportControl.ImportControlRepository icr = new ImportControlRepository();
                icr.writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1, "Error getting db connection", ex.Message);        
                //ApplicationLogging.WriteCriticalExceptionMessage(ex, "Error getting database connection", "DBConnection");
                throw;
            }
            return cnnRtn;
        }

        public SqlConnection SqlMainNewDBConnection
        {
            get
            {
                try
                {
                    string sCnn = ConfigurationManager.ConnectionStrings["NameSys.Properties.Settings.SqlMainNewDB"].ConnectionString;
                    if (sCnn.Length == 0)
                    {
                        throw new DataException("Connection string to SqlMainNew DB not available.");
                    }
                    SqlConnection cnnDB = new SqlConnection(sCnn);
                    cnnDB.Open();

                    return cnnDB;
                }
                catch (Exception ex)
                {
                    DAL.ImportControl.ImportControlRepository icr = new ImportControlRepository();
                    icr.writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1, "Error getting SqlMainnew connection", ex.Message);        
                    //ApplicationLogging.WriteCriticalExceptionMessage(ex, command: "DB Connect");
                    throw;
                }

            }
        }

        public SqlConnection SqlProd01DBConnection
        {
            get
            {
                try
                {
                    string sCnn = ConfigurationManager.ConnectionStrings["NameSys.Properties.Settings.SqlProd01DB"].ConnectionString;
                    if (sCnn.Length == 0)
                    {
                        throw new DataException("Connection string to SqlProd01 DB not available.");
                    }
                    SqlConnection cnnDB = new SqlConnection(sCnn);
                    cnnDB.Open();

                    return cnnDB;
                }
                catch (Exception ex)
                {
                    //DAL.ImportControl.ImportControlRepository ipr = new ImportControlRepository();
                    //ipr.writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1, "Error getting SqlProd01 connection", ex.Message);        
                    //ApplicationLogging.WriteCriticalExceptionMessage(ex, command: "DB Connect");
                    throw;
                }

            }
        }

        public SqlConnection SqlNameSysDBConnection
        {
            get
            {
                try
                {
                    string sCnn = ConfigurationManager.ConnectionStrings["NameSys.Properties.Settings.NameSysDB"].ConnectionString;
                    if (sCnn.Length == 0)
                    {
                        throw new DataException("Connection string to NameSys DB not available.");
                    }
                    SqlConnection cnnDB = new SqlConnection(sCnn);
                    cnnDB.Open();

                    return cnnDB;
                }
                catch (Exception ex)
                {
                    //ipr.writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1, "Error getting sql NameSys connection", ex.Message);        
                    //ApplicationLogging.WriteCriticalExceptionMessage(ex, command: "DB Connect");
                    throw;
                }

            }
        }

        public SqlDataReader ExecuteReader(string procedureName, DBConnection dbConnection, System.Collections.Specialized.NameValueCollection args = null)
        {
            System.Data.SqlClient.SqlCommand cmd = null;
            SqlDataReader dr = null;
            string argValues = string.Empty;
            try
            {
                cmd = new SqlCommand(procedureName, DataConnection(dbConnection));
                cmd.CommandType = CommandType.StoredProcedure;
                if (args != null)
                {
                    foreach (string key in args)
                    {
                        cmd.Parameters.AddWithValue(key, args[key]);
                        argValues = argValues + key + " val:" + args[key] + " / ";
                    }
                }
                cmd.CommandTimeout = 0;
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                //wtf.writeSyncLog(9, "ExecuteReader error: Proc. = " + procedureName + " Args = " + args + " Sql error: ", ex.Message);
                string err = "Error Found at ExecuteReader: procedure name " + procedureName + " parms:" + argValues;
                //DAL.ImportControl.ImportControlRepository ipr = new ImportControlRepository();
                //ipr.writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1, err, ex.Message);        
                //wtf.writeSyncLog(9, err, ex.Message + ex.StackTrace);
                throw;
            }
            finally
            {
                //   we do not want to close SQLCommand.DataConnection because the connection is needed for '
                //   data reader but we can destroy this reference to the object 
                cmd = null;
            }

            return dr;

        }

        public object ExecuteScalar(string procedureName, DBConnection dbConnection, System.Collections.Specialized.NameValueCollection args)
        {
            object rtn = null;

            using (SqlCommand cmd = new SqlCommand(procedureName, DataConnection(dbConnection)))
            {
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (string key in args)
                    {
                        cmd.Parameters.AddWithValue(key, args[key]);
                    }
                    cmd.CommandTimeout = 0;
                    rtn = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    string err = "Error ExecuteScalar error: Proc. = " + procedureName + " Args = " + args.Keys;
                    DAL.ImportControl.ImportControlRepository icr = new ImportControlRepository();
                    icr.writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1, err, ex.Message);        
                    //wtf.writeSyncLog(9, "ExecuteScalar error: Proc. = " + procedureName + " Args = " + args.Keys, ex.Message);
                    //wlf.WriteToFile("WSP Sync Service ExecuteScalar error: Proc. = " + procedureName + " Args = " + args.Keys + " Log: {0} - Sql error: " + ex);    
                    throw;
                }
                finally
                {
                    if ((cmd != null) && (cmd.Connection != null) && (cmd.Connection.State != ConnectionState.Closed))
                    {
                        cmd.Connection.Close();
                    }
                }
            }
            return rtn;

        }

        public int ExecuteNonQuery(string procedureName, DBConnection dbConnection, System.Collections.Specialized.NameValueCollection args = null)
        {
            SqlCommand cmd = null;
            int iRtn = 0;

            try
            {
                using (cmd = new SqlCommand(procedureName, DataConnection(dbConnection)))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (args != null)
                    {
                        foreach (string key in args)
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("{0} = {1}", key, args[key]));
                            cmd.Parameters.AddWithValue(key, args[key]);
                        }
                    }
                    cmd.CommandTimeout = 0;
                    iRtn = cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                string err = "Error ExecuteNonQuery error: Proc. = " + procedureName + " Args = " + args.Keys;
                //ipr.writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1, err, ex.Message);        
                //wtf.writeSyncLog(9, "ExecuteScalar error: Proc. = " + procedureName + " Args = " + args.Keys + " Log: {0} - Sql error: ", ex.Message);
                //wtf.WriteToFile("WSP Sync Service ExecuteScalar error: Proc. = " + procedureName + " Args = " + args.Keys + " Log: {0} - Sql error: " + ex);    
                throw;
            }
            finally
            {
                if ((cmd != null) && (cmd.Connection != null) && (cmd.Connection.State != ConnectionState.Closed))
                {
                    cmd.Connection.Close();
                }
            }

            return iRtn;

        }

    }
}
