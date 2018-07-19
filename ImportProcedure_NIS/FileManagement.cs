using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ImportProcedure_NIS
{
    /// <summary>
    ///     Version 3.0
    ///     Process NIS xml files.
    ///     Retreive them from input folder.
    ///     create the NIS data model.
    ///     Generate Json frame.
    ///     Sent Json frome to Middleware API.
    /// </summary>
    public class FileManagement
    {
        /// Xml files Methods delegate
        public delegate bool Del(XmlDocument doc, string tst, EtlTimer sync);
        /// 
        readonly DAL.ImportControl.ImportControlRepository wtf = new DAL.ImportControl.ImportControlRepository();
        readonly DAL.ImportControl.ImportControlRepository icr = new DAL.ImportControl.ImportControlRepository();
        //  EtlTimer sync = new EtlTimer();     //  Versson 2.0
        ///   
        //  int serviceId = 0; // Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]);   //  version 2.0
        int FilesRead = 0;
        string tableName = ConfigurationManager.AppSettings["TableName"]; 
        ///  **************************************************************************************
        ///  Version 2.0
        ///  Perform Directory File search process when task time is due
        ///  Look for existing files in the new files folder
        ///  Same method is used in the WindowsService version....
        ///  --------------------------------------------------------------------------------------
        //public void FileSearch(object e, EtlTimer sync)
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
        //        {   // This path is a file     
        //            ProcessFile(path, sync);
        //        }
        //        else if (Directory.Exists(path))
        //        {   // This path is a directory                    
        //            ProcessDirectory(path, sync);
        //        }
        //        else
        //        {   //  Invalid File or Directory exit
        //            wtf.writeSyncLog(1, serviceId, 1, sync.ServiceName, "(FileSearch) <" + path + " > is not a valid file or directory.");
        //        }
        //    }
        //    wtf.writeSyncLog(1, serviceId, 1, sync.ServiceName, " > (FileSearch) " + sync.ServiceName + " Files read: " + FilesRead);
        //    /// *******************************************************************************
        //    /// Back to Service SchedularCallback Function
        //    /// When FileSearch is completed process flow return to Service1.SchedularCallback.      
        //    /// -------------------------------------------------------------------------------
        //}

        //  ************************************************************************
        //  Process all files in the directory passed in, recurse on any directories  
        //  that are found, and process the files they contain. 
        //  ------------------------------------------------------------------------
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
        /// General files process - Only NIS XML valid Files are processed here:      
        ///     (1) - Select files to process depending on file extension and/or file name.
        ///     (2) - Route process to the corresponding file types import process using delegates.
        ///     (3) - Unrecognized file types and Service reading errors are detected here.
        ///     (4) - Errors are logged in service log and reported through web services.      
        /// -----------------------------------------------------------------------------------------
        public void ProcessFile(string path, EtlTimer sync)
        {
            string extension = (Path.GetExtension(path)).ToString();
            string fileName = Path.GetFileNameWithoutExtension(path);
            /// Initialize Error messages object basic information
            ServiceResponse errMsg = new ServiceResponse();
            errMsg.FileType = "1";
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
                /// Only NIS (1) files are allowed in this directory. 
                /// They have .xml and or sent extension.
                /// ---------------------------------------------------------------------------------   
                string prefix = fileName;
                ///
                ///  Validate if this file was already processed - May 16 - 2017
                ///
                int serviceId = Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]);
                int dupFile = wtf.getImportLog(serviceId, "NIS", fileName + extension);

                //if (((extension == ".xml") || (extension == ".sent") || (extension == ".Sent")))    //   && (prefix != "Acc" && prefix != "Dsn" && prefix != "Ord"))
                if (dupFile == 0 && ((extension == ".xml") || (extension == ".sent")))
                {
                    XmlDocument doc = new XmlDocument();
                    //  Read / Load selected file content as xml
                    doc.Load(path);
                    /// Instanciate Xml methods delegate
                    Del handler = null;
                    // Declare a class instance xml:
                    ImportProcedure_NIS.XmlFiles.ImportProcess xml = new ImportProcedure_NIS.XmlFiles.ImportProcess();
                    /// Declare an interface instance xmlFile:
                    ImportProcedure_NIS.XmlFiles.IXmlFiles xmlfile = (ImportProcedure_NIS.XmlFiles.IXmlFiles)xml;
                    /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    /// Identify NIS xml files and route them to the corresponding import procedure 
                    ///  ----------------------------------------------------------------------------     
                    //  NIS Files process
                    handler = xmlfile.ProcessNisFiles;
                    /// =========== >
                    ProcessXml(path, doc, fileName, handler, sync);
                    /// < ===========
                }
                else
                {   /// Unrecognized file type
                    errMsg.NISOrderId = fileName;
                    errMsg.Message = "(NIS ProcessFile) File <" + fileName + extension + ">, unrecognized file type.";
                    wtf.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, errMsg.Message);
                    /// Move file to problem directory
                    //DAL.ImportControl.ImportControlRepository icr = new DAL.ImportControl.ImportControlRepository();
                    //icr.SaveProcessedFile(path, false, sync);
                    /// Duplicate files contrl - May16-2017
                    if (dupFile == 0)
                        icr.SaveProcessedFile(path, false, sync, "NIS");
                    else
                        File.Delete(path);          //  delete duplicate file May16-2017
                }
            }
            catch (Exception fle)
            {
                int res = wtf.updImportControl(sync.MwEtlTimerId, 0);     //  set EtlTimer for this service to not Running (isRunning = false)
                errMsg.Message = "(NIS ProcessFile) Xml reading error - File in " + errMsg.FileName + ". " + fle;
                wtf.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, errMsg.Message);
            }
        }

        ///  *******************************************************************************************
        /// <summary>
        /// 09/22/2015 16:39 version
        ///      Common Entry to Process all NIS Xml Files.
        ///      (1) File send to the corresponding import procedure depending on the file type.        
        ///      (2) Finally process flow is transfer to final file processing stage.
        /// </summary>     
        ///  -------------------------------------------------------------------------------------------
        private void ProcessXml(string path, XmlDocument doc, string tst, Del handler, EtlTimer sync)
        {
            bool ok = false;
            /// ----------------------------------------------------------------------------------------
            /// Synchronous Process option
            /// ----------------------------------------------------------------------------------------
            ok = handler(doc, tst, sync);                 //  Process file and WebService            
            /// ========== >
            icr.SaveProcessedFile(path, ok, sync, "NIS");
            /// < ==========
        }
    }
}
