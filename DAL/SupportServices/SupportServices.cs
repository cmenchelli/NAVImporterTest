using ImportModelLibrary.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DAL.SupportServices
{
    //public class SupportServices
    //{
    //    readonly DAL.ImportControl.ImportControlRepository wtf = new DAL.ImportControl.ImportControlRepository();


    //    /// ***************************************************************************************
    //    /// <summary>
    //    /// Version 10/15/2015 - 14:48 
    //    ///     Consume web services, in case of error, an error description entry is included 
    //    ///     in the log file. 
    //    ///     WeServiceTimer (method performance measure) and WebServiceOption (Production 
    //    ///     or test) options are controlled through AppConfig.File.
    //    /// </summary>
    //    /// <param name="frame">Json Data string</param>
    //    /// <param name="fName">Processed File name</param>
    //    /// <param name="fType">File type (Raw Data or File data) </param>
    //    /// <param name="fData">WebService url </param>
    //    /// <returns>Bool value to true if procces was OK</returns>
    //    /// --------------------------------------------------------------------------------------
    //    public bool ConsumeWebService(string frame, string fName, string fData, EtlTimer syncRow)   //string fType, string fData)
    //    {
    //        /// Process measurement option, only apply when WebServicesTimer has been set to Yes
    //        /// in the AppConfig.file Will measure the process time from starting this method, until 
    //        /// the response from the WebService is received. 
    //        Stopwatch stopWatch = new Stopwatch();
    //        if ((ConfigurationManager.AppSettings["WebServicesTimer"].ToString()) == "Yes")
    //            stopWatch.Start();
    //        /// ----------------------------------------------------------------------------------
    //        //WriteLogFile wlf = new WriteLogFile();
    //        //WriteErrorFile wef = new WriteErrorFile();
    //        ServiceResponse err = new ServiceResponse();    /// process errors description object
    //        ServiceResponse resp = new ServiceResponse();    /// Response from web service
    //        ///
    //        err.FileName = fName;
    //        err.Request = frame;
    //        ///
    //        bool res = true;
    //        var result = string.Empty;
    //        try
    //        {
    //            /// ******************************************************************************
    //            /// Call Web Service
    //            /// Web service return string message with operation result.
    //            /// Access to Middleware API is controlled through etlTimer Table (IsApiRunning)
    //            /// ------------------------------------------------------------------------------            
    //            if (syncRow.IsApiRunning)
    //            {
    //                string uri = fData;
    //                var request = (HttpWebRequest)WebRequest.Create(uri);
    //                request.ContentType = "text/json";
    //                request.Method = "POST";

    //                using (var client = new WebClient())
    //                {
    //                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
    //                    // Optionally specify an encoding for uploading and downloading strings.
    //                    client.Encoding = System.Text.Encoding.UTF8;
    //                    // Upload the data.
    //                    result = client.UploadString(uri, "POST", frame);
    //                }
    //                resp = JsonConvert.DeserializeObject<ServiceResponse>(result);
    //            }
    //            else
    //            {
    //                /// ----------------------------------------------------------------------------
    //                /// Testing webService (using proxy) option, apply only when WebServices Option
    //                /// is not set to IsApiRunning in the etlTimer table.
    //                /// To use the ApiTest option, uncomment the following code
    //                /// ----------------------------------------------------------------------------
    //                //ServiceOrdersClient proxy = new ServiceOrdersClient();
    //                ////  route webservice based on file type
    //                //result = proxy.AddOrder(frame, fName, fType);
    //                resp.Status = "OK";
    //            }
    //            /// --------------------------------------------------------------------------------
    //            ///                 
    //            /// --------------------------------------------------------------------------------
    //            if (!String.IsNullOrEmpty(resp.Status) && (resp.Status != "OK"))
    //            {
    //                res = false;
    //                err.NISOrderId = resp.NISOrderId;
    //                err.Status = resp.Status;
    //                err.Message = "Order id.: " + resp.NISOrderId + " Message: " + resp.Message;
    //                ///
    //                WriteErrorFile(err, syncRow);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            /// Replace service log error by WebService call         
    //            err.NISOrderId = resp.NISOrderId;
    //            err.Status = "API Call not processed";
    //            err.Message = "An error occured while running " + fData + " Web Service, : " + ex.Message;
    //            ///
    //            WriteErrorFile(err, syncRow);
    //            res = false;
    //        }
    //        if ((ConfigurationManager.AppSettings["WebServicesTimer"].ToString()) == "Yes")
    //        {
    //            stopWatch.Stop();
    //            // Get the elapsed time as a TimeSpan value.
    //            TimeSpan ts = stopWatch.Elapsed;
    //            // Format and display the TimeSpan value.
    //            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
    //                ts.Hours, ts.Minutes, ts.Seconds,
    //                ts.Milliseconds / 10);
    //            Console.WriteLine("File: " + fName + " WebService RunTime " + elapsedTime);
    //        }
    //        return (res);
    //    }

    //    /// *****************************************************************************  
    //    /// <summary>
    //    /// Write error messages to the WindowsService serviceLog and send same message to
    //    /// the WebService (Errors) 
    //    /// </summary>  
    //    /// 
    //    /// "MwOrderMasterId": "1",
    //    /// "OrderTypeId"    : "1",    () from MW_OrderType 
    //    ///                                 (WebAccount / WebDesign / WebOrder = 2)
    //    ///                                 (Nis - Name System = 1 )
    //    ///                                 (Text file - G&K  system = 5)
    //    /// "Filename"       : "test.txt",
    //    /// "Message"        : "test error"       
    //    /// -----------------------------------------------------------------------------   
    //    //public async Task WriteErrorFile(ServiceResponse source, EtlTimer syncRow)
    //    public void WriteErrorFile(ServiceResponse source, EtlTimer syncRow)
    //    {
    //        /// ******************************************************************************
    //        /// Call Web Service to transfer Error Message
    //        /// ------------------------------------------------------------------------------
    //        XDocument doc = new XDocument();
    //        string cleanContent = string.Empty;
    //        try
    //        {
    //            doc = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(
    //                    new MemoryStream(Encoding.ASCII.GetBytes(source.Request)),
    //                    new XmlDictionaryReaderQuotas()));
    //            string fileContent = doc.ToString();
    //            cleanContent = fileContent.Replace("\"", "\\\"");
    //        }
    //        catch (Exception e)
    //        {
    //            cleanContent = e.Message;
    //        }
    //        /// Build the error message json string              
    //        string frame2 = "{\"MwOrderMasterId\" : \"" + "1" +
    //                        "\", \"OrderTypeId\": " + "\"" + source.FileType +
    //                        "\"" + ", \"FileName\" : \"" + source.FileName +
    //                        "\", \"Error\": \"" + source.Message +
    //                        "\", \"Request\": \"" + cleanContent + "\" }";
    //        /// ==========> 
    //        //WebServiceManager wsm = new WebServiceManager();  //  Set WebService methods pointer  

    //        //string errors = ConfigurationManager.AppSettings["ExcelFilesDirectory"].ToString();
    //        bool r = ConsumeWebService(frame2, source.FileName, syncRow.WebServiceErrors, syncRow);
    //        /// <==========
    //        /// ******************************************************************************
    //        /// Write ServiceLog 
    //        /// ------------------------------------------------------------------------------
    //        //string logPath = (ConfigurationManager.AppSettings["WindowsServiceLog"].ToString());

    //        string message = " > File: <";       /// Set message prefix
    //        message = message + source.FileName + ">, Status: <" + source.Status + "> message: <" + source.Message + ">.";

    //        wtf.writeSyncLog(1, syncRow.MwEtlTimerId, 1, syncRow.ServiceName, message);

    //        //using (StreamWriter writer = new StreamWriter(logPath, true))
    //        //{
    //        //    await writer.WriteLineAsync(string.Format(message, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
    //        //    writer.Close();
    //        //}

    //    }

    //    /// *****************************************************************************  
    //    /// Write Process Log directly to File or Console 
    //    /// -----------------------------------------------------------------------------
    //    //public async Task WriteLogFile(string source)
    //    //{
    //    //    //Console.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));

    //    //    string logPath = (ConfigurationManager.AppSettings["WindowsServiceLog"].ToString());
    //    //    using (StreamWriter writer = new StreamWriter(logPath, true))
    //    //    {
    //    //        //writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
    //    //        await writer.WriteLineAsync(string.Format(source, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
    //    //        writer.Close();
    //    //    }
    //    //}        
    //}

    public class SupportServices
    {
        readonly DAL.ImportControl.ImportControlRepository wtf = new DAL.ImportControl.ImportControlRepository();


        /// ***************************************************************************************
        /// <summary>
        /// Version 10/15/2015 - 14:48 
        ///     Consume web services, in case of error, an error description entry is included 
        ///     in the log file. 
        ///     WeServiceTimer (method performance measure) and WebServiceOption (Production 
        ///     or test) options are controlled through AppConfig.File.
        /// </summary>
        /// <param name="frame">Json Data string</param>
        /// <param name="fName">Processed File name</param>
        /// <param name="fType">File type (Raw Data or File data) </param>  unused
        /// <param name="fData">WebService url </param>
        /// <returns>Bool value to true if procces was OK</returns>
        /// --------------------------------------------------------------------------------------
        public ServiceResponse ConsumeWebService(string frame, string fName, string fData, EtlTimer syncRow)   //string fType, string fData)
        {
            /// Process measurement option, only apply when WebServicesTimer has been set to Yes
            /// in the AppConfig.file Will measure the process time from starting this method, until 
            /// the response from the WebService is received. 
            Stopwatch stopWatch = new Stopwatch();
            if ((ConfigurationManager.AppSettings["WebServicesTimer"].ToString()) == "Yes")
                stopWatch.Start();
            /// ----------------------------------------------------------------------------------
            //WriteLogFile wlf = new WriteLogFile();
            //WriteErrorFile wef = new WriteErrorFile();
            ServiceResponse err = new ServiceResponse();    /// process errors description object
            ServiceResponse resp = new ServiceResponse();    /// Response from web service
            ///
            err.FileName = fName;
            err.Request = frame;
            ///
            //bool res = true;
            var result = string.Empty;
            try
            {
                /// ******************************************************************************
                /// Call Web Service
                /// Web service return string message with operation result.
                /// Access to Middleware API is controlled through etlTimer Table (IsApiRunning)
                /// ------------------------------------------------------------------------------           
                if (syncRow.IsApiRunning)
                {
                    string uri = fData;
                    var request = (HttpWebRequest)WebRequest.Create(uri);
                    request.ContentType = "text/json";
                    request.Method = "POST";

                    using (var client = new WebClient())
                    {
                        /// Send json data to api supporting special characters within the json data
                        client.Headers[HttpRequestHeader.ContentType] = "application/json";
                        // Optionally specify an encoding for uploading and downloading strings.
                        client.Encoding = System.Text.Encoding.UTF8;
                        // Upload the data.
                        result = client.UploadString(uri, "POST", frame);
                        //resp.Status = "Error";        //  force error response only for testing purposes
                    }
                    resp = JsonConvert.DeserializeObject<ServiceResponse>(result);
                    //  -----------------------------------------------------------------------------------------
                    //  When api service return an error response (status not 'OK'), 
                    //  save the data file information in MW_NavImportProblems table
                    //  -----------------------------------------------------------------------------------------
                    if (resp.Status != "OK")
                    {
                        resp.IsOk = false;                          //  Set response flag to no ok  
                        int position = fName.LastIndexOf('-');
                        ProblemFiles prob = new ProblemFiles();
                        prob.FileName = fName;
                        if (position == -1)
                        { prob.FileNumber = "n/a"; }
                        else
                        { prob.FileNumber = fName.Substring(position + 1); }
                        prob.JsonData = frame;
                        prob.FileType = ConfigurationManager.AppSettings["FileType"];
                        //  ================ >
                        int write = wtf.writeProblemFile(prob);
                        //  < ================
                    }
                }
                else
                {
                    /// ----------------------------------------------------------------------------
                    /// Testing webService (using proxy) option, apply only when WebServices Option
                    /// is not set to IsApiRunning in the etlTimer table.
                    /// To use the ApiTest option, uncomment the following code
                    /// ----------------------------------------------------------------------------
                    //ServiceOrdersClient proxy = new ServiceOrdersClient();
                    ////  route webservice based on file type
                    //result = proxy.AddOrder(frame, fName, fType);
                    resp.Status = "OK";
                    resp.IsOk = true;
                }
                /// --------------------------------------------------------------------------------
                ///                 
                /// --------------------------------------------------------------------------------
                if (!String.IsNullOrEmpty(resp.Status) && (resp.Status != "OK"))
                {
                    // res = false;
                    resp.IsOk = false;
                    err.NISOrderId = resp.NISOrderId;
                    err.Status = resp.Status;
                    err.Message = "Order id.: " + resp.NISOrderId + " Message: " + resp.Message;
                    ///
                    WriteErrorFile(err, syncRow);
                }
            }
            catch (Exception ex)
            {
                int position = fName.LastIndexOf('-');
                ProblemFiles prob = new ProblemFiles();
                prob.FileName = fName;
                if (position == -1)
                { prob.FileNumber = "n/a"; }
                else
                { prob.FileNumber = fName.Substring(position + 1); }
                prob.JsonData = frame;
                prob.FileType = ConfigurationManager.AppSettings["FileType"];
                //  ================ >
                int write = wtf.writeProblemFile(prob);
                //  < ================
                /// Replace service log error by WebService call         
                err.NISOrderId = resp.NISOrderId;
                err.Status = "Not processed";
                err.Message = "An error occured while running " + fData + " Web Service, : " + ex.Message;
                ///
                WriteErrorFile(err, syncRow);
                // res = false;
                resp.IsOk = false;
            }
            if ((ConfigurationManager.AppSettings["WebServicesTimer"].ToString()) == "Yes")
            {
                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;
                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Console.WriteLine("File: " + fName + " WebService RunTime " + elapsedTime);
            }
            if (resp.Status == "OK")
                resp.IsOk = true;
            return (resp);
        }

        /// *****************************************************************************  
        /// <summary>
        /// Write error messages to the WindowsService serviceLog and send same message to
        /// the WebService (Errors) 
        /// </summary>  
        /// 
        /// "MwOrderMasterId": "1",
        /// "OrderTypeId"    : "1",    () from MW_OrderType 
        ///                                 (WebAccount / WebDesign / WebOrder = 2)
        ///                                 (Nis - Name System = 1 )
        ///                                 (Text file - G&K  system = 5)
        /// "Filename"       : "test.txt",
        /// "Message"        : "test error"       
        /// -----------------------------------------------------------------------------   
        //public async Task WriteErrorFile(ServiceResponse source, EtlTimer syncRow)
        public void WriteErrorFile(ServiceResponse source, EtlTimer syncRow)
        {
            /// ******************************************************************************
            /// Call Web Service to transfer Error Message
            /// ------------------------------------------------------------------------------
            XDocument doc = new XDocument();
            string cleanContent = string.Empty;
            try
            {
                doc = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(
                        new MemoryStream(Encoding.ASCII.GetBytes(source.Request)),
                        new XmlDictionaryReaderQuotas()));
                string fileContent = doc.ToString();
                cleanContent = fileContent.Replace("\"", "\\\"");
            }
            catch (Exception e)
            {
                cleanContent = e.Message;
            }
            /// Build the error message json string              
            string frame2 = "{\"MwOrderMasterId\" : \"" + "1" +
                            "\", \"OrderTypeId\": " + "\"" + source.FileType +
                            "\"" + ", \"FileName\" : \"" + source.FileName +
                            "\", \"Error\": \"" + source.Message +
                            "\", \"Request\": \"" + cleanContent + "\" }";
            /// ==========> 
            /// 
            ServiceResponse resp = ConsumeWebService(frame2, source.FileName, syncRow.WebServiceErrors, syncRow);
            /// <==========
            /// ******************************************************************************
            /// Write ServiceLog 
            /// ------------------------------------------------------------------------------
            //string logPath = (ConfigurationManager.AppSettings["WindowsServiceLog"].ToString());

            string message = " > File: <";       /// Set message prefix
            message = message + source.FileName + ">, Status: <" + source.Status + "> message: <" + source.Message + ">.";

            wtf.writeSyncLog(1, syncRow.MwEtlTimerId, 1, syncRow.ServiceName, message);
        }

        /// *****************************************************************************  
        /// Write Process Log directly to File or Console 
        /// -----------------------------------------------------------------------------
        //public async Task WriteLogFile(string source)
        //{
        //    //Console.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));

        //    string logPath = (ConfigurationManager.AppSettings["WindowsServiceLog"].ToString());
        //    using (StreamWriter writer = new StreamWriter(logPath, true))
        //    {
        //        //writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
        //        await writer.WriteLineAsync(string.Format(source, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
        //        writer.Close();
        //    }
        //}        
                        
    }
}