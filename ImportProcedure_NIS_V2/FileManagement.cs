using ImportModelLibrary.Entities;
using System;
using System.Configuration;
using System.Linq;

namespace ImportProcedure_NIS_V2
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
        // Xml files Methods delegate
        public delegate bool Del(NisTables doc, int oNumber, EtlTimer sync);
        /// 
        readonly DAL.ImportControl.ImportControlRepository  icr = new DAL.ImportControl.ImportControlRepository();
        readonly DAL.ImportControl.ImportNISRepository      Inr = new DAL.ImportControl.ImportNISRepository();
        ///   
        int FilesRead = 0;
        string tableName = ConfigurationManager.AppSettings["TableName"];
        /// *****************************************************************************************
        /// General files process - Only NIS XML valid Files are processed here:      
        ///     (1) - Select orders to process depending on WasImported status.
        ///     (2) - Route process to the corresponding import process using delegates.
        ///     (3) - NIS table reading errors are detected here.
        ///     (4) - Errors are logged in service log and reported through web services.      
        /// -----------------------------------------------------------------------------------------
        public void ProcessOrder(int order, EtlTimer sync)
        {            
            /// Initialize Error messages object basic information
            ServiceResponse errMsg = new ServiceResponse();
            errMsg.FileType = "1";
            errMsg.FileName = "Order: " + order.ToString();
            errMsg.NISOrderId = "0";
            errMsg.Status = "Not Processed";
            errMsg.Message = string.Empty;
            try
            {
                /// *********************************************************************************
                /// Select files acceptable to be process, all other file types (extension) will not
                /// be processed (they stay in the newFiles folder) and an error message is generated
                /// to the WebServices.
                /// Only NIS (1) row are processed by entry. 
                /// ---------------------------------------------------------------------------------   
                NisTables NisOrder = new NisTables();
                NisOrder = Inr.getNisOrder(order);

                if (NisOrder != null && NisOrder.items != null)    
                {                    
                    /// Instanciate NBI Tables methods delegate
                    Del handler = null;
                    // Declare a class instance xml:
                    ImportProcedure_NIS_V2.BuildJason.ImportProcess table = new ImportProcedure_NIS_V2.BuildJason.ImportProcess();
                    /// Declare an interface instance Table:
                    ImportProcedure_NIS_V2.BuildJason.INISTables orderClass = (ImportProcedure_NIS_V2.BuildJason.INISTables)table;
                    /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    /// Route to the corresponding NIS import procedure 
                    ///  ---------------------------------------------------------------------------- 
                    handler = orderClass.ProcessNisOrder;
                    bool ok = false;
                    ok = handler(NisOrder, order,  sync);                 //  Process file and WebService
                }
                else
                {   //  update error order - set imported on and import problem on
                    int ret = Inr.updNisOrder(order, 0, 1, 0);
                    icr.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, "Table reading error order header or no items found, order " + order);
                }
            }
            catch (Exception fle)
            {
                int res = icr.updImportControl(sync.MwEtlTimerId, 0);     //  set EtlTimer for this service to not Running (isRunning = false)
                errMsg.Message = "(NIS ProcessFile) Table reading error - in order " + errMsg.FileName + ". " + fle;
                icr.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, errMsg.Message);
                int resp = Inr.updNisOrder(Convert.ToInt32(errMsg.FileName), 0, 1, 0);
            }
        }     
    }
}
