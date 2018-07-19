using DAL.ImportControl;
using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ImportProcedure_NBI
{
    /// <summary>
    ///     Version 3.0
    ///     Process NBI xml files.
    ///     Retreive them from input folder.
    ///     create the NBI data model.
    ///     Generate Json frame.
    ///     Sent Json frome to Middleware API.
    /// </summary>
    public class FileManagement
    {
        /// Xml files Methods delegate
        public delegate bool Del(XmlDocument doc, string tst, EtlTimer sync);
        /// 
        readonly ImportControlRepository wtf = new ImportControlRepository();
        readonly DAL.ImportControl.ImportControlRepository icr = new DAL.ImportControl.ImportControlRepository();
        ///    
        int serviceId = 0; // Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]);
        //  EtlTimer sync = new EtlTimer();     //  Version 2.0
        ///  **************************************************************************************
        ///  Version 2.0 Feb13-17
        ///  Perform Directory File search process when task time is due
        ///  Look for existing files in the NIB new files folder
        ///  Same method is used in the WindowsService version....
        ///  --------------------------------------------------------------------------------------
        //public void FileSearch(object e, EtlTimer sync)       /// NOT USED IN Version 3.0
        //{
        //    if (serviceId == 0)
        //    {
        //        //serviceId = Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]); ;
        //        //sync = wtf.getImportControl(serviceId);
        //        serviceId = sync.MwEtlTimerId;
        //    }
        //    List<string> li = new List<string>();
        //    //
        //    wtf.writeSyncLog(1, serviceId, 1, sync.ServiceName, " > (NBI FileSearch) Start Directory scanning. for " + sync.ServiceName + ".");
        //    //
        //    //li.Add(ConfigurationManager.AppSettings["NewFilesDirectory"].ToString());   //  C:\\ImportNIBData_xml
        //    li.Add(sync.InputFileFolder);
        //    if (li.Count == 0)
        //    {
        //        wtf.writeSyncLog(1, serviceId, 1, sync.ServiceName, " > (NBI FileSearch) No Folder to process. for " + sync.ServiceName);
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
        //            wtf.writeSyncLog(1, serviceId, 1, sync.ServiceName, "(NBI FileSearch) <" + path + " > is not a valid file or directory.");
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
        //public void ProcessDirectory(string targetDirectory, EtlTimer sync)           /// NOT USED IN Version 3.0
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
        /// Version 3.0 (Feb17-17)
        /// General files process - Only NIB XML valid Files are processed here:      
        ///     (1) - Select files to process depending on file extension and/or file name.
        ///     (2) - Route process to the corresponding file types import process using delegates.
        ///     (3) - Unrecognized file types and Service reading errors are detected here.
        ///     (4) - Errors are logged in service log and reported through web services.   
        /// -----------------------------------------------------------------------------------------
        public void ProcessFile(string path, EtlTimer sync)
        {
            ServiceResponse errMsg = new ServiceResponse();
            errMsg.FileType = "2";                      //  NIB file type
            errMsg.FileName = Path.GetFileName(path);  //fileName + extension;
            errMsg.Status = "Not Processed";
            errMsg.NISOrderId = Path.GetFileNameWithoutExtension(path); ;
            ///
            string extension = (Path.GetExtension(path)).ToString();
            string fileName = Path.GetFileNameWithoutExtension(path);
            try
            {   /// *********************************************************************************
                /// Select files acceptable to be process, all other file types (extension) will not
                /// be processed (they stay in the newFiles folder) and an error message is generated
                /// to the WebServices.
                /// NIB (2) files process
                /// All NIB files have .xml and or sent extension.
                /// ---------------------------------------------------------------------------------   
                string prefix = fileName;
                prefix = prefix
                       .Replace("website-account-", "Acc").Replace("website-design-", "Dsn").Replace("website-order-", "Ord");
                prefix = prefix.Substring(0, 3);
                ///
                ///  Validate if this file was already processed    May16-2017
                ///  
                int dupFile = wtf.getImportLog(serviceId, "NBI", fileName + extension);

                //if (((extension == ".xml") || (extension == ".sent")) && ((prefix == "Acc") || (prefix == "Dsn") || (prefix == "Ord")))
                if (dupFile == 0 && (((extension == ".xml") || (extension == ".sent")) && ((prefix == "Acc") || (prefix == "Dsn") || (prefix == "Ord"))))
                {
                    XmlDocument doc = new XmlDocument();
                    //  Read / Load selected file content as xml
                    doc.Load(path);
                    /// Instanciate Xml methods delegate
                    Del handler = null;
                    // Declare a class instance xml:
                    ImportProcedure_NBI.XmlFiles.ImportProcess xml = new ImportProcedure_NBI.XmlFiles.ImportProcess();
                    /// Declare an interface instance xmlFile:
                    ImportProcedure_NBI.XmlFiles.IXmlFiles xmlfile = xml;       //  (ImportProcedure_NBI.XmlFiles.IXmlFiles)xml
                    /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    /// Identify all xml files and route them to the corresponding import procedure 
                    /// Replace FileName with prefix's to simplify file type identification.
                    ///  ----------------------------------------------------------------------------          
                    if (prefix == "Acc")
                    {   //  NBI - Website-Accounts Files
                        handler = xmlfile.ProcessWebsiteAccountFiles;
                    }
                    else if (prefix == "Dsn")
                    {   //  NBI - Website-Designs Files
                        handler = xmlfile.ProcessWebsiteDesignFiles;
                    }
                    else if (prefix == "Ord")
                    {   //  NBI - Website-Order Files process
                        handler = xmlfile.ProcessWebsiteOrderFiles;
                    }
                    /// =========== >
                    bool ok = handler(doc, fileName, sync);                 //  Process file and WebService     
                    icr.SaveProcessedFile(path, ok, sync, "NBI");
                    /// ===========>  
                    //ProcessXml(path, doc, fileName, handler, sync);
                    /// < ===========
                }
                else
                {   /// Unrecognized file type
                    /// Initialize Error messages object basic information  
                    /// 
                    errMsg.Status = "Not Processed";
                    errMsg.NISOrderId = fileName;
                    if (dupFile > 0)
                    {
                        errMsg.Message = "(NBI ProcessFile) File <" + fileName + extension + ">, duplicate file - already processed.";
                    }
                    else
                        errMsg.Message = "(NBI ProcessFile) File <" + fileName + extension + ">, unrecognized file type.";
                    wtf.writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1, sync.ServiceName, errMsg.Message);
                    /// Duplicate file control May16-2017
                    ///
                    if (dupFile == 0)
                        icr.SaveProcessedFile(path, false, sync, "STK");
                    else
                        File.Delete(path);
                    /// < ===========
                }
            }
            catch (Exception fle)
            {
                int res = wtf.updImportControl(serviceId, 0);     //  set EtlTimer for this service to not Running (isRunning = false)
                errMsg.Message = "(NBI ProcessFile) Xml reading error - File in " + errMsg.FileName + ". " + fle;
                wtf.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, errMsg.Message);
            }
        }

        ///  *******************************************************************************************
        /// <summary>
        ///     Version 3.0 Feb17-17  
        ///     Common Entry to Process all NIB Xml Files.
        ///     (1) File send to the corresponding import procedure depending on the file type.
        ///     (2) A copy of the processed file is send (webService) as raw data for archiving purposes.
        ///     (3) Finally process flw is transfer to final file processing stage.
        /// </summary>     
        /// <param name="path">inbox folder path</param>
        /// <param name="doc">copy of the xml file content</param>
        /// <param name="tst">in process xml file name</param>
        /// <param name="Del">Delegate pointing to the corresponding xml editing process</param>
        /// <param name="sync">EtlTimer control parameters</param>
        /// <returns>void</returns>
        ///  -------------------------------------------------------------------------------------------
        //private void ProcessXml(string path, XmlDocument doc, string tst, Del handler, EtlTimer sync)
        //{
        //    //bool ok = false;
        //    /// ****************************************************************************************
        //    ///     Synchronous Process option
        //    /// ----------------------------------------------------------------------------------------
        //    bool ok = handler(doc, tst, sync);                 //  Process file and WebService
        //    /// ****************************************************************************************
        //    ///     Call Web Service to transfer a copy of the processed file through RawData web service                     
        //    ///     Find and replace XML information that can't be transfer within the json data string
        //    /// ----------------------------------------------------------------------------------------    
        //    //string fileContent = doc.OuterXml;
        //    //string cleanContent = fileContent.Replace("\"", "\\\"");
        //    //string jsonFrame = "{\"MwOrderMasterId\" : \"" + "1" + "\", \"RawData\": \"" + cleanContent + "\"}";
        //    /// ----------------------------------------------------------------------------------------
        //    ///     Select which web service to use for NBI orders depending on the Order Type
        //    ///     NOT USED IN VERSION 3.0 - NO RAW DATA TRANSFER TO THE MIDDLEWARE
        //    /// ----------------------------------------------------------------------------------------
        //    //string prefix = tst;
        //    //prefix = prefix
        //    //            .Replace("website-account-", "Acc")
        //    //            .Replace("website-design-", "Dsn")
        //    //            .Replace("website-order-", "Ord");
        //    //prefix = prefix.Substring(0, 3);
        //    ////
        //    //string fType = sync.WebServiceOrders;           //  set default web service
        //    //if (prefix == "Acc")
        //    //    fType = sync.WebServiceAccounts;            //  Set Web Accounts service
        //    //else if (prefix == "Dsn")
        //    //    fType = sync.WebServiceDesigns;             //  Set Web Designs service
        //    /// ==========> 
        //    /// DO NOT SAVE INPUT FILE AS RAW DATA IN THE MIDDLEWARE - 
        //    //DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices(); 
        //    //bool r = wsm.ConsumeWebService(jsonFrame, "", fType, sync);

        //    /// <==========
        //    /// DELETE PROCESSED XML FLE FROM INBOX AND MOVE IT TO PROCESSED (succeeded)
        //    /// ==========>
        //    //  DAL.ImportControl.ImportControlRepository icr = new DAL.ImportControl.ImportControlRepository();
        //    icr.SaveProcessedFile(path, ok, sync, "NBI");
        //    /// <==========
        //}
    }
}