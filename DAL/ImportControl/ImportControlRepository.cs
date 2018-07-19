using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DAL.ImportControl
{
    /// <summary>
    ///     Version 2.0 Feb13-17
    ///     
    /// </summary>
    public class ImportControlRepository
    {
        readonly DAL.DBContext.DBAccess access = new DBContext.DBAccess();

        /// <summary>
        /// Get syncControlTable status for an specific type of table.
        ///     10 - NIS Imports
        ///     11 - NIB Imports
        ///     12 - Shipping address
        /// </summary>
        /// <param name="type">Service type code</param>
        /// <returns>Boolean  true if service is active and not busy</returns>
        public EtlTimer getImportControl(int type)
        {
            SqlDataReader dr = null;
            //Boolean active = false;
            EtlTimer row = new EtlTimer();
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@etlTaskId", type.ToString());
                //DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("[MW_GetETLTaskTimer]", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //
                //EtlTimer row = new EtlTimer();
                //string serviceAvailability = "not active";
                while (dr != null && dr.Read())
                {
                    row.MwEtlTimerId        = (Int32)(dr["MwEtlTimerId"].ToString().Length > 0 ? Int32.Parse(dr["MwEtlTimerId"].ToString()) : 0);
                    row.Description         = dr["Description"].ToString().Length > 0 ? dr["Description"].ToString() : "";
                    row.ServiceName         = dr["ServiceName"].ToString().Length > 0 ? dr["ServiceName"].ToString() : "";
                    row.IsActive            = Convert.ToBoolean(dr["IsActive"]);
                    row.FrequencyInSecs     = (Int64)(dr["FrequencyInSecs"].ToString().Length > 0 ? Int64.Parse(dr["FrequencyInSecs"].ToString()) : 0);
                    row.IsRunning           = Convert.ToBoolean(dr["IsRunning"]);
                    row.DailyRunTime        = dr["DailyRunTime"].ToString().Length > 0 ? TimeSpan.Parse(dr["DailyRunTime"].ToString()) : TimeSpan.Zero;
                    row.LastRunDate         = dr["LastRunDate"].ToString().Length > 0 ? DateTime.Parse(dr["LastRunDate"].ToString()) : DateTime.MinValue;
                    row.IsApiRunning        = Convert.ToBoolean(dr["IsApiRunning"]);
                    row.WebServiceOrders    = dr["WebServiceOrders"].ToString().Length > 0 ? dr["WebServiceOrders"].ToString() : "";
                    row.WebServiceAccounts  = dr["WebServiceAccounts"].ToString().Length > 0 ? dr["WebServiceAccounts"].ToString() : "";
                    row.WebServiceDesigns   = dr["WebServiceDesigns"].ToString().Length > 0 ? dr["WebServiceDesigns"].ToString() : "";
                    row.WebServiceRaw       = dr["WebServiceRaw"].ToString().Length > 0 ? dr["WebServiceRaw"].ToString() : "";
                    row.WebServiceErrors    = dr["WebServiceErrors"].ToString().Length > 0 ? dr["WebServiceErrors"].ToString() : "";
                    row.InputFileFolder     = dr["InputFileFolder"].ToString().Length > 0 ? dr["InputFileFolder"].ToString() : "";
                    row.ExcelFileFolder     = dr["ExcelFileFolder"].ToString().Length > 0 ? dr["ExcelFileFolder"].ToString() : "";
                    row.ProblemFileFolder   = dr["ProblemFileFolder"].ToString().Length > 0 ? dr["ProblemFileFolder"].ToString() : "";
                    row.ProcessedFileFolder = dr["ProcessedFileFolder"].ToString().Length > 0 ? dr["ProcessedFileFolder"].ToString() : "";
                }
                return row;
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
        }

        public int updImportControl(int type, int status)
        {
            int resp = 0;
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@etlTaskId", type.ToString());
                args.Add("@status", status.ToString());
                DBContext.DBAccess access = new DBContext.DBAccess();
                //  ===== >
                resp = access.ExecuteNonQuery("[MW_UpdETLTaskTimer]", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //  < =====
            }
            catch
            {
                //writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1,
                              //ConfigurationManager.AppSettings["TableName"], "Service for " + ConfigurationManager.AppSettings["TableName"] + " EtlTimer access error ");
            }
            finally
            { }
            return resp;
        }

        

        public int ImpSTKOrder(StkOrder ord)
        {
            Object ret = 0;
            //try
            //{
            //    System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
            //    args.Add("@Order", ord.stkOrderSummary.AccountId.ToString());
            //    args.Add("@uOption", uOption.ToString());
            //    args.Add("@impProb", importProblem.ToString());
            //    DBContext.DBAccess access = new DBContext.DBAccess();
            //    //
            //    ret = access.ExecuteScalar("MW_UpdNISOrder", DBContext.DBAccess.DBConnection.NameSys, args);
            //    //   
            //}
            //catch (Exception)
            //{
            //    writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1, ConfigurationManager.AppSettings["TableName"], "Table update error, order " + order);
            //}
            return Convert.ToInt32(ret);
        }

        public string writeSyncLog(int LogLevel, int SourceId, int LogType, string fileName, string description, string exceptionData = null)
        {
            /// *****************************************************************************  
            /// Write Process Log directly to File or Console 
            /// -----------------------------------------------------------------------------
            NameValueCollection args = new NameValueCollection();
            args.Add("@LogLevel", LogLevel.ToString());
            args.Add("@SourceId", SourceId.ToString());
            args.Add("@LogType", LogType.ToString());
            if (string.IsNullOrEmpty(fileName))
                fileName = "";
            args.Add("@FileName", fileName);
            if (string.IsNullOrEmpty(exceptionData))
                exceptionData = "";
            args.Add("@Description", description);
            args.Add("@ExceptionData", exceptionData);
            // Design Request export status was updated successfully. Log Activity as successfull           
            string responseMessage;
            bool rc = LogActivity(new ActivityLogBO
            {
                ActivityLogLevel = (ActivityLogLevels)LogLevel,
                ActivitySourceId = (ActivitySource)SourceId,   //ActivitySource.DesignRequestOrder,
                ActivityLogType = (ActivityLogTypes)LogType,            //.UpdateCatalog,
                //OrderTypeId = OrderTypes.DR,
                OrderId = "",
                ExceptionData = exceptionData,
                Description = description
            } // WE.Common.Constants.OK_MESSAGE + WE.WindowsSvc.Common.Constants.NUMBER_OF_DESIGN_REQUESTS_UPDATED_IN_CATALOG + designRequestOrdersSentToMWOKList.Count }
                      , ConfigurationManager.AppSettings["ActivityLogWSBaseUrl"], ConfigurationManager.AppSettings["ActivityLogWSSuffixUrl"], out responseMessage);
            return responseMessage;
        }

        public bool LogActivity(ActivityLogBO activity, string webServiceBaseUrl, string webServiceSuffixUrl, out string responseMessage)
        {
            return CallWebApiService<ActivityLogBO>(activity, webServiceBaseUrl, webServiceSuffixUrl, out responseMessage);
        }

        // Generic function to call different types of Web API service via POST 
        public bool CallWebApiService<T>(T inObject, string webServiceBaseUrl, string webServiceSuffixUrl, out string responseMessage)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(webServiceBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {   // HTTP POST
                    HttpResponseMessage response = client.PostAsJsonAsync(webServiceSuffixUrl, inObject).Result;
                    responseMessage = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                catch (Exception ex)
                {
                    responseMessage = ex.Message;
                    return false;
                }
            }
        }

        ///  *******************************************************************************************
        ///  <summary>
        ///  Final file processing stage - Only XML and txt Files are processed here by moving the file  
        ///  from their original folder to AppProcess (when the file was processed ok), or to AppProblem
        ///  if there was a problem with it. In both cases, the file is eliminated from AppData.  
        /// </summary>
        //  --------------------------------------------------------------------------------------------
        public void SaveProcessedFile(string path, bool ok, EtlTimer sync, string source)
        {
            //WriteLogFile wlf = new WriteLogFile();
            DAL.ImportControl.ImportControlRepository wlf = new DAL.ImportControl.ImportControlRepository();
            ///
            string fStatus = "processed";
            string fileName = Path.GetFileName(path);
            string newPath;
            if (ok)
            {
                //  Save processesd file in succeeded folder as defined in etlCtrol
                newPath = sync.ProcessedFileFolder + @"\" + fileName;
                //newPath = (ConfigurationManager.AppSettings["ProcessedFilesDirectory"].ToString()) + @"\" + fileName;
                int src = Convert.ToInt32(ConfigurationManager.AppSettings["ServiceID"]);
                ///
                /// Save processed files log - May16-2017
                ///
                int res = writeImportLog(src, source, fileName);
            }
            else
            {
                newPath = sync.ProblemFileFolder + @"\" + fileName;
                //newPath = (ConfigurationManager.AppSettings["ProblemFilesDirectory"].ToString()) + @"\" + fileName;
                fStatus = "rejected with errors.";
            }
            //  delete file, if exist 
            if (File.Exists(newPath))
                File.Delete(newPath);
            //  Move File from processed to problem directory
            File.Move(path, newPath);
            //  *************************************************************************
            //  End of file process message in service log
            //  -------------------------------------------------------------------------   
            //string tableName = ConfigurationManager.AppSettings["TableName"];
            string ret = wlf.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, "> " + sync.Description +  " File: <" + fileName + "> " + fStatus + ".");
        }

        /// <summary>
        ///     Create new row for every file processed suscessfully - duplicate files control 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fileType"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int writeImportLog(int source, string fileType, string fileName)
        {
            int res = 0;
            //DBContext.DBAccess access = new DBContext.DBAccess();
            NameValueCollection args = new NameValueCollection();
            args.Add("@Source", source.ToString());
            args.Add("@fileType", fileType);
            args.Add("@fileName", fileName);
            //
            res = access.ExecuteNonQuery("MW_NavImportLogInsert", DBContext.DBAccess.DBConnection.SqlMainNew, args);
            //
            return res;
        }

        /// <summary>
        ///     Get an specific processed file informnation - duplicate files control
        ///     this process does not allow to resend a file that has been sucessfully processed by the MW
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fileType"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int getImportLog(int source, string fileType, string fileName)
        {
            object res = 0;
            //DBContext.DBAccess access = new DBContext.DBAccess();
            NameValueCollection args = new NameValueCollection();
            args.Add("@Source", source.ToString());
            args.Add("@fileType", fileType);
            args.Add("@fileName", fileName);
            //
            res = access.ExecuteScalar("MW_NavImportLogGet", DBContext.DBAccess.DBConnection.SqlMainNew, args);
            //
            return Convert.ToInt32(res);
        }

        /// <summary>
        ///     Save information about files rejected by the Middleware app.
        ///     information include the Json frame sent to MW.
        /// </summary>
        /// <param name="prob"></param>
        /// <returns></returns>
        public int writeProblemFile(ProblemFiles prob)
        {
            int resp = 0;
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@fileType", prob.FileType.ToString());
                args.Add("@fileName", prob.FileName.ToString());
                args.Add("@fileNumber", prob.FileNumber.ToString());
                args.Add("@jsonData", prob.JsonData.ToString());
                DBContext.DBAccess access = new DBContext.DBAccess();
                //  ===== >
                resp = access.ExecuteNonQuery("[MW_NavImportProblemsInsert]", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //  < =====
            }
            catch
            {
                writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1,
                              ConfigurationManager.AppSettings["TableName"], "Service for " + ConfigurationManager.AppSettings["TableName"] + " MW_NavImportProblems access error ");
            }
            finally
            { }
            return resp;


        }

    }
}