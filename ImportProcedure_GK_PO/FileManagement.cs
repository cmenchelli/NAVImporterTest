using ImportModelLibrary.Entities;
using ImportProcedure_GK_PO.TextFiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportProcedure_GK_PO
{
    /// <summary>
    ///     Version 3.0
    ///     Process GK_PO txt files.
    ///     Retreive them from input folder.
    ///     create the GK data model.
    ///     Generate Json frame.
    ///     Sent Json frome to Middleware API.
    /// </summary>
    public class FileManagement
    {
        /// Text files Methods delegate
        public delegate bool DelTxt(string path, string tst, EtlTimer sync);
        /// 
        //  readonly DAL.SupportServices.SupportServices wss = new DAL.SupportServices.SupportServices();
        readonly DAL.ImportControl.ImportControlRepository wtf = new DAL.ImportControl.ImportControlRepository();
        readonly DAL.ImportControl.ImportControlRepository icr = new DAL.ImportControl.ImportControlRepository();
              
        //int serviceId = Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]);
        int serviceId = 0; // Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]);
        /// 
        //  EtlTimer sync = new EtlTimer();
        ///  **************************************************************************************
        ///  Replaced in Version 3.0
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
        //    wtf.writeSyncLog(1, serviceId, 1, sync.ServiceName, " > (GK FileSearch) Start Directory scanning. for " + sync.ServiceName + ".");
        //    //
        //    //li.Add(ConfigurationManager.AppSettings["NewFilesDirectory"].ToString());   //  C:\\AppData
        //    li.Add(sync.InputFileFolder);
        //    if (li.Count == 0)
        //    {
        //        wtf.writeSyncLog(1, serviceId, 1, sync.ServiceName, " > (GK FileSearch) No Folder to process. for " + sync.ServiceName);
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
        //            wtf.writeSyncLog(1, serviceId, 1, sync.ServiceName, "(GK FileSearch) <" + path + " > is not a valid file or directory.");
        //        }
        //    }
        //    /// *******************************************************************************
        //    /// Back to Service SchedularCallback Function
        //    /// When FileSearch is completed process flow return to Service1.SchedularCallback.      
        //    /// -------------------------------------------------------------------------------
        //}

        ////  ************************************************************************
        ////  Process all files in the directory passed in, recurse on any directories  
        ////  that are found, and process the files they contain. 
        ////  ------------------------------------------------------------------------
        //public void ProcessDirectory(string targetDirectory, EtlTimer sync)
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
        /// Version 3.0
        /// General files process - Only Text valid Files are processed here:      
        ///     (1) - Select files to process depending on file extension and/or file name.
        ///     (2) - Route process to the corresponding file types import process using delegates.
        ///     (3) - Unrecognized file types and Service reading errors are detected here.
        ///     (4) - Errors are logged in service log and reported through web services.
        ///         
        ///     USE THIS SECTION TO ADD ALL FILE TYPES TO BE INCLUDED IN THE IMPORT PROCESS        
        /// -----------------------------------------------------------------------------------------
        private void ProcessFile(string path, EtlTimer sync)
        {
            int dirLen = (ConfigurationManager.AppSettings["NewFilesDirectory"].ToString()).Length + 1;
            ///
            string extension = (Path.GetExtension(path)).ToString();
            string fileName = Path.GetFileNameWithoutExtension(path);
            /// Initialize Error messages object basic information
            ServiceResponse errMsg = new ServiceResponse();
            errMsg.FileType = "5";
            errMsg.FileName = fileName + extension;
            errMsg.NISOrderId = "0";
            errMsg.Status = "Not Processed";
            errMsg.Message = string.Empty;
            try
            {
                /// *********************************************************************************
                /// Select files acceptable to be process, all other file types (extension) will not
                /// be processed (they stay in the newFiles folder) and an error message is generated
                /// to the WebServices.
                /// ---------------------------------------------------------------------------------   
                if (extension == ".txt")
                {
                    /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    ///     Process all txt files here, they must belong to WE or G&K 
                    ///  ---------------------------------------------------------------------------------                    
                    DelTxt handler = null;
                    // Declare a class instance txt:
                    ImportProcedure_GK_PO.TextFiles.ImportProcess txt = new ImportProcedure_GK_PO.TextFiles.ImportProcess();
                    /// Declare an interface instance txtFile:
                    ITextFiles txtfile = (ITextFiles)txt;

                    if (fileName.Substring(0, 6) == "WE XML")
                    {   //  WE - XML Order Files
                        handler = txtfile.ProcessWEFiles;
                    }
                    else
                    {   // G&K Text files
                        handler = txtfile.ProcessGKFiles;
                    }
                    /// =========== >
                    ProcessTxt(path, fileName, handler, sync);
                    /// < ===========
                }
                else
                {   /// Unrecognized file type
                    errMsg.NISOrderId = fileName;
                    errMsg.Message = "GK ProcessFile) File <" + fileName + extension + ">, unrecognized file type.";
                    wtf.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, errMsg.Message);
                    /// Move file to problem directory
                    icr.SaveProcessedFile(path, false, sync, "GK");
                }
            }
            catch (Exception fle)   
            {
                int res = wtf.updImportControl(sync.MwEtlTimerId, 0);     //  set EtlTimer for this service to not Running (isRunning = false)
                errMsg.Message = "((GK_PO ProcessFile) txt reading error - File in " + errMsg.FileName + ". " + fle;
                wtf.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, errMsg.Message);
            }
        }

        /// <summary>
        ///     Process text files, text files are routed here the corresponding import process and  
        ///     WebService depending on the Process method assigned through the delegate specified
        ///     in the handler parameter.
        ///     After processing, the complete original file content is send to the webServices for
        ///     storage in OrderRaw table.
        /// </summary>
        /// <param name="path">Incoming file complete path</param>
        /// <param name="tst">file name without extnsion</param>
        /// <param name="handler">Delegate that points to the corresponding process method</param>
        private void ProcessTxt(string path, string tst, DelTxt handler, EtlTimer sync)
        {
            bool ok = false;
            /// =============== >   Process file and WebService using sync method
            ok = handler(path, tst, sync);
            // DO NOT SAVE INPUT FILE AS RAW DATA IN THE MIDDLEWARE - Version 2.0 up
            /// ****************************************************************************************
            /// Call Web Service to transfer a copy of the processed file through RawData
            /// ----------------------------------------------------------------------------------------   
            /// PERFORM getTextFileContent while waiting for 'processResult' response.
            // 
            //  bool r = getTextFileContent(path, sync);
            //
            /// WAIT TO PERFORM THE FOLLOWING STEPS UNTIL 'processResult' IS COMPLETED
            //  bool ok = processResult;
            /// Exit to final file process step.   
            //if (ok)
            //{
                //DAL.ImportControl.ImportControlRepository icr = new DAL.ImportControl.ImportControlRepository();
                icr.SaveProcessedFile(path, ok, sync, "GK");
            //}
        }

        /// <summary>
        ///     UNUSED IN VERSION 3.0
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
    }
}