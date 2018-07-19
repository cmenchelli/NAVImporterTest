using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LateOrdersProcedure_NIS
{
    /// <summary>
    ///     Version 3.0
    ///     Process NIS orders tables.
    ///     create the imported NIS Orders list.
    ///     Insert late Orders in MW_LateOrders and Sent email.
    /// </summary>
    public class FileManagement
    {
        // Xml files Methods delegate
        //public delegate bool Del(NISLateOrders doc, int oNumber, EtlTimer sync);
        /// 
        readonly DAL.ImportControl.ImportControlRepository  icr = new DAL.ImportControl.ImportControlRepository();
        readonly DAL.ImportControl.ImportNISRepository      Inr = new DAL.ImportControl.ImportNISRepository();
        ///   
        int FilesRead = 0;
        string tableName = ConfigurationManager.AppSettings["TableName"];
        /// *****************************************************************************************
        /// General files process - Only NIS XML valid Files are processed here:      
        ///     (1) - Select orders to process depending on Imported conditions.
        ///     (2) - Route process to the corresponding late orders process using delegates.
        ///     (3) - NIS table reading errors are detected here.
        ///     (4) - Errors are logged in service log and reported through web services.      
        /// -----------------------------------------------------------------------------------------
        public void ProcessOrder(NISLateOrders order, EtlTimer sync)        
        {            
            /// Initialize Error messages object basic information
            ServiceResponse errMsg = new ServiceResponse();
            errMsg.FileType = "1";
            errMsg.FileName = "Late Order: " + order.ToString();
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
                if (order != null)    
                {   
                    /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    /// Route to the corresponding NIS procedure 
                    ///  ---------------------------------------------------------------------------- 
                    int res = Inr.insertNisLateOrder(order);
                    icr.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, "late order inserted: " + order.ID);
                }
                else
                {   //  update error order - set imported on and import problem on
                    int ret = Inr.updNisOrder(order.ID, 0, 1, 0);
                    icr.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, "Table reading error Late orders, order " + order);
                }
            }
            catch (Exception fle)
            {
                int res = icr.updImportControl(sync.MwEtlTimerId, 0);     //  set EtlTimer for this service to not Running (isRunning = false)
                errMsg.Message = "(NIS LateOrders) Table reading error - in order " + errMsg.FileName + ". " + fle;
                icr.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, errMsg.Message);
                int resp = Inr.updNisOrder(Convert.ToInt32(errMsg.FileName), 0, 1, 0);
            }
        }     
    }
}
