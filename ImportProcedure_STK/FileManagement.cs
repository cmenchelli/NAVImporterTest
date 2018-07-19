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

namespace ImportProcedure_STK
{
    /// <summary>
    ///     Version 3.0
    ///     Process STK xml files.
    ///     Retreive them from input folder.
    ///     create the STK data model.
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
        int serviceId = Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]); // Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]);        
        /// *****************************************************************************************
        /// Version 3.0 (Feb20-17)
        /// General files process - Only STK XML valid Files are processed here:      
        ///     (1) - Select files to process depending on file extension and/or file name.
        ///     (2) - Route process to the corresponding file types import process using delegates.
        ///     (3) - Unrecognized file types and Service reading errors are detected here.
        ///     (4) - Errors are logged in service log and reported through web services.   
        /// -----------------------------------------------------------------------------------------
        public void ProcessFile(string path, EtlTimer sync)
        {
            ServiceResponse errMsg = new ServiceResponse();
            errMsg.FileType = "15";                      //  STK file type
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
                int dupFile = wtf.getImportLog(serviceId, "STK", fileName + extension);

                if (dupFile == 0 && (((extension == ".xml") || (extension == ".sent")) && ((prefix == "Acc") || (prefix == "Dsn") || (prefix == "Ord"))))
                //if (((extension == ".xml") || (extension == ".sent")) && ((prefix == "Acc") || (prefix == "Dsn") || (prefix == "Ord")))
                {
                    XmlDocument doc = new XmlDocument();
                    //  Read / Load selected file content as xml
                    doc.Load(path);
                    /// Instanciate Xml methods delegate
                    Del handler = null;
                    // Declare a class instance xml:
                    ImportProcedure_STK.XmlFiles.ImportProcess xml = new ImportProcedure_STK.XmlFiles.ImportProcess();
                    /// Declare an interface instance xmlFile:
                    ImportProcedure_STK.XmlFiles.IXmlFiles xmlfile = xml;       //  (ImportProcedure_NBI.XmlFiles.IXmlFiles)xml
                    /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    /// Identify all xml files and route them to the corresponding import procedure 
                    /// Replace FileName with prefix's to simplify file type identification.
                    ///  ----------------------------------------------------------------------------          
                    if (prefix == "Acc")
                    {   //  NBI - Website-Accounts Files
                        handler = xmlfile.ProcessStkAccountFiles;
                    }
                    else if (prefix == "Dsn")
                    {   //  NBI - Website-Designs Files
                        handler = xmlfile.ProcessStkDesignFiles;
                    }
                    else if (prefix == "Ord")
                    {   //  NBI - Website-Order Files process
                        handler = xmlfile.ProcessStkOrderFiles;
                    }
                    /// =========== >
                    bool ok = handler(doc, fileName, sync);                 //  Process file and WebService     
                    icr.SaveProcessedFile(path, ok, sync, "STK");
                    /// ===========>  
                    //ProcessXml(path, doc, fileName, handler, sync);
                    /// < ===========
                }
                else
                {   /// Unrecognized file type or duplicate
                    /// Initialize Error messages object basic information  
                    errMsg.Status = "Not Processed";
                    errMsg.NISOrderId = fileName;
                    if (dupFile > 0)
                    {
                        errMsg.Message = "(STK ProcessFile) File <" + fileName + extension + ">, duplicate file - already processed.";
                    }
                    else
                        errMsg.Message = "(STK ProcessFile) File <" + fileName + extension + ">, unrecognized file type.";
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
                errMsg.Message = "(STK ProcessFile) Xml reading error - File in " + errMsg.FileName + ". " + fle;
                wtf.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, errMsg.Message);
            }
        }

        ///  *******************************************************************************************
        /// <summary>
        ///     Version 3.0 Feb17-17  
        ///     Common Entry to Process all NIB Xml Files.
        ///     (1) File send to the corresponding import procedure depending on the file type.
        ///     (2) Finally process flw is transfer to final file processing stage.
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
        //    /// ****************************************************************************************
        //    ///     Synchronous Process option
        //    /// ----------------------------------------------------------------------------------------
        //    bool ok = handler(doc, tst, sync);                 //  Process file and WebService            
        //    /// ----------------------------------------------------------------------------------------                
        //    /// ===========>     
        //    icr.SaveProcessedFile(path, ok, sync, "STK");
        //    /// <==========
        //}
    }
}