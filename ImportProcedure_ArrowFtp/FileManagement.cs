using ImportModelLibrary.Entities;
using ImportProcedure_ArrowFtp.TextFiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportProcedure_ArrowFtp
{
    /// <summary>
    ///     Version 3.0
    ///     Process Arrow Ftp txt files.
    ///     Retreive them from input folder.
    ///     create the Arrow Ftp data model.
    ///     Generate Json frame.
    ///     Sent Json frome to Middleware API.
    /// </summary>
    public class FileManagement
    {
        /// Text files Methods delegate
        public delegate bool DelTxt(string path, string tst, EtlTimer sync);
        /// 
        readonly DAL.SupportServices.SupportServices wss = new DAL.SupportServices.SupportServices();
        readonly DAL.ImportControl.ImportControlRepository wtf = new DAL.ImportControl.ImportControlRepository();

        //int tableId = Convert.ToInt16(ConfigurationManager.AppSettings["TableId"]);
        // int serviceId = 0;  // Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]);
        //string tableName = ConfigurationManager.AppSettings["TableName"];
        //int dirLen = (ConfigurationManager.AppSettings["NewFilesDirectory"].ToString()).Length + 1;
        //
        //  EtlTimer sync = new EtlTimer();
        ///  **************************************************************************************
        ///  Perform Directory File search process when task time is due
        ///  Look for existing files in the new files folder
        ///  Same method is used in the WindowsService version....
        ///  --------------------------------------------------------------------------------------
        ///public void FileSearch(object e, EtlTimer sync)
        //{
        //    if (serviceId == 0)
        //    {
        //        //serviceId = Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]); ;
        //        //sync = wtf.getImportControl(serviceId);
        //        serviceId = sync.MwEtlTimerId;
        //    }
        //    List<string> li = new List<string>();
        //    //
        //    wtf.writeSyncLog(1, serviceId, 1, sync.ServiceName, " > (FileSearch) Start Directory scanning. for " + sync.ServiceName + ".");
        //    //
        //    //li.Add(ConfigurationManager.AppSettings["NewFilesDirectory"].ToString());   //  C:\\AppData
        //    li.Add(sync.InputFileFolder);
        //    if (li.Count == 0)
        //    {
        //        wtf.writeSyncLog(1, serviceId, 1, sync.ServiceName, " > (FileSearch) No Folder to process. for " + sync.ServiceName);
        //    }
        //    foreach (string path in li)
        //    {
        //        if (File.Exists(path))
        //        {
        //            // This path is a file                    
        //            ProcessFile(path, sync);
        //        }
        //        else if (Directory.Exists(path))
        //        {
        //            // This path is a directory                    
        //            ProcessDirectory(path, sync);
        //        }
        //        else
        //        {
        //            //  Invalid File or Directory exit
        //            wtf.writeSyncLog(1, serviceId, 1, sync.ServiceName, "(FileSearch) <" + path + " > is not a valid file or directory.");
        //        }
        //    }
        //    /// *******************************************************************************
        //    /// Back to Service SchedularCallback Function
        //    /// When FileSearch is completed process flow return to Service1.SchedularCallback.      
        //    /// -------------------------------------------------------------------------------
        //}

        ///  ************************************************************************
        ///  Process all files in the directory passed in, recurse on any directories  
        ///  that are found, and process the files they contain. 
        ///  ------------------------------------------------------------------------

        /// public void ProcessDirectory(string targetDirectory, EtlTimer sync)
        //{
        //    /// 
        //    string[] fileEntries = Directory.GetFiles(targetDirectory);
        //    foreach (string fileName in fileEntries)
        //    {
        //        ProcessFile(fileName, sync);
        //    }
        //    // Recurse into subdirectories of this directory. 
        //    string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        //    foreach (string subdirectory in subdirectoryEntries)
        //        ProcessDirectory(subdirectory, sync);
        //}

        /// *****************************************************************************************
        /// General files process - Only XML/Text valid Files are processed here:      
        ///     (1) - Select files to process depending on file extension and/or file name.
        ///     (2) - Route process to the corresponding file types import process using delegates.
        ///     (3) - Unrecognized file types and Service reading errors are detected here.
        ///     (4) - Errors are logged in service log and reported through web services.
        ///         
        ///     USE THIS SECTION TO ADD ALL FILE TYPES TO BE INCLUDED IN THE IMPORT PROCESS        
        /// -----------------------------------------------------------------------------------------
        private void ProcessFile(string path, EtlTimer sync)
        {   ///
            string extension = (Path.GetExtension(path)).ToString();
            string fileName = Path.GetFileNameWithoutExtension(path);
            /// Initialize Error messages object basic information
            ServiceResponse errMsg = new ServiceResponse();
            //
            errMsg.FileType = "0";
            errMsg.FileName = fileName + extension;
            errMsg.NISOrderId = "0";
            errMsg.Status = "Not Processed";
            errMsg.Message = string.Empty;
            //
            try
            {
                /// *********************************************************************************
                /// Select files acceptable to be process, all other file types (extension) will not
                /// be processed (they stay in the newFiles folder) and an error message is generated
                /// to the WebServices.
                /// ---------------------------------------------------------------------------------   
                if (extension == "")
                {
                    /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    ///     Process all txt files here, they must belong to WE or G&K 
                    ///  ---------------------------------------------------------------------------------
                    errMsg.FileType = "x";
                    DelTxt handler = null;
                    // Declare a class instance txt:
                    ImportProcedure_ArrowFtp.TextFiles.ImportProcess txt = new ImportProcedure_ArrowFtp.TextFiles.ImportProcess();
                    /// Declare an interface instance txtFile:
                    ITextFiles txtfile = (ITextFiles)txt;

                    if (fileName.Substring(0, 5) == "OADS_")
                    {   //  WE - XML Order Files
                        handler = txt.ProcessArrowFtpFiles;
                    }
                    /// =========== >
                    ProcessTxt(path, fileName, handler, sync);
                    /// < ===========
                }
                else
                {   /// Unrecognized file type
                    errMsg.NISOrderId = fileName;
                    errMsg.Message = "(NIS ProcessFile) File <" + fileName + extension + ">, unrecognized file type.";
                    wtf.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, errMsg.Message);
                    /// Move file to problem directory
                    wtf.SaveProcessedFile(path, false, sync, "Arrow");
                }
            }
            catch (Exception fle)
            {
                int res = wtf.updImportControl(sync.MwEtlTimerId, 0);     //  set EtlTimer for this service to not Running (isRunning = false)
                errMsg.Message = "(ArrowFtp ProcessFile) txt reading error - File in " + errMsg.FileName + ". " + fle;
                wtf.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, errMsg.Message);
            }
        }

        /// <summary>
        ///     Version 3.0
        ///     Process text files, text files are routed here the corresponding import process and  
        ///     WebService depending on the Process method assigned through the delegate specified
        ///     in the handler parameter.
        ///     Does not apply to version 3.0 [After processing, the complete original file content is send to the webServices for
        ///     storage in OrderRaw table.] 
        /// </summary>
        /// <param name="path">Incoming file complete path</param>
        /// <param name="tst">file name without extnsion</param>
        /// <param name="handler">Delegate that points to the corresponding process method</param>
        private void ProcessTxt(string path, string tst, DelTxt handler, EtlTimer sync)
        {
            bool ok = false;
            /// =============== >   Process file and WebService using sync method
            //bool processResult = handler(path, tst, sync);
            ok = handler(path, tst, sync);
            /// PERFORM getTextFileContent while waiting for 'processResult' response.
            /// /// DO NOT SAVE INPUT FILE AS RAW DATA IN THE MIDDLEWARE - Version 2.0 up
            /// ****************************************************************************************
            /// Call Web Service to transfer a copy of the processed file through RawData
            /// ----------------------------------------------------------------------------------------    
            //  NO RAW DATA IS SEND TO THE MIDDLEWARE 
            /// =========================================
            //   bool r = getTextFileContent(path, sync);
            //  =========================================
            /// DON't PERFORM THE FOLLOWING STEPS UNTIL 'processResult' IS COMPLETED
            //bool ok = processResult;
            /// Exit to final file process step.   
            //if (ok)
            //{
                ///DAL.ImportControl.ImportControlRepository icr = new DAL.ImportControl.ImportControlRepository();
            wtf.SaveProcessedFile(path, ok, sync, "Arrow");
            //}
        }

        /// <summary>
        ///     NOT REQUIRED - Version 2.0 up
        ///     Retreive text file content as a string before calling the Web Service that store a
        ///     a copy of the processed file in OrderData.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        //public bool getTextFileContent(string path, EtlTimer sync)
        //{
        //    bool ret = false;
        //    //string errorTyp = "Unable to retreive file content, ";
        //    /// ------------------------------------------------------------------------------------
        //    try
        //    {
        //        /// Get complete file content as string
        //        string fileContent = File.ReadAllText(path);
        //        ///  Find and replace XML information that can't be transfer within the json data string     
        //        string cleanContent = fileContent.Replace("\"", "\\\"");
        //        string frame2 = "{\"MwOrderMasterId\" : \"" + "1" + "\", \"RawData\": \"" + cleanContent + "\"}";
        //        /// ==========> To store copy of file content WebService 
        //        DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();
        //        bool r = wsm.ConsumeWebService(frame2, "", "OrderRaw", sync);
        //        /// <==========
        //        /// Process outcome to ok.
        //        ret = true;
        //    }
        //    catch
        //    {
        //        ///  Create error message frame and initial data
        //        ServiceResponse errMsg = new ServiceResponse();
        //        errMsg.FileType = "1";      //  Set Nis Type
        //        errMsg.FileName = Path.GetFileName(path);
        //        errMsg.NISOrderId = "0";
        //        errMsg.Status = "RawData Not saved";
        //        errMsg.Message = "Unable to retreive file <" + errMsg.FileName + "> content. File moved to problem directory";
        //        /// Send Message to WebService
        //        wss.WriteErrorFile(errMsg, sync);
        //    }
        //    return ret;
        //}

        ///  *******************************************************************************************
        ///  <summary>
        ///     NOT REQUIRED - Version 2.0 up
        ///     Final file processing stage - Only XML and txt Files are processed here by moving the file  
        ///     from their original folder to AppProcess (when the file was processed ok), or to AppProblem
        ///     if there was a problem with it. In both cases, the file is eliminated from AppData.  
        /// </summary>
        //  --------------------------------------------------------------------------------------------
        //private static void SaveProcessedFile(string path, bool ok)
        //{
        //    //WriteLogFile wlf = new WriteLogFile();
        //    DAL.ImportControl.ImportControlRepository wlf = new DAL.ImportControl.ImportControlRepository();
        //    ///
        //    string fStatus = "processed";
        //    string fileName = Path.GetFileName(path);
        //    string newPath;
        //    if (ok)
        //    {
        //        newPath = (ConfigurationManager.AppSettings["ProcessedFilesDirectory"].ToString()) + @"\" + fileName;
        //    }
        //    else
        //    {
        //        newPath = (ConfigurationManager.AppSettings["ProblemFilesDirectory"].ToString()) + @"\" + fileName;
        //        fStatus = "rejected with errors.";
        //    }
        //    //  delete file, if exist 
        //    if (File.Exists(newPath))
        //        File.Delete(newPath);
        //    //  Move File from processed to problem directory
        //    File.Move(path, newPath);
        //    //  *************************************************************************
        //    //  End of file process message ijn service log
        //    //  -------------------------------------------------------------------------   
        //    string tableName = ConfigurationManager.AppSettings["TableName"];
        //    string ret = wlf.writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1, tableName, "> File: <" + fileName + "> " + fStatus + ".");
        //}
    }
}