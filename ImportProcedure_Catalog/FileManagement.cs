using ImportModelLibrary.Entities;
using System;
using System.Configuration;
using System.Linq;

namespace ImportProcedure_Catalog
{
    public class FileManagement
    {
        // Xml files Methods delegate
        public delegate bool Del(CatalogOrderTables doc, string oNumber, EtlTimer sync);
        public delegate bool DelConvert(CatalogConvertTable doc, string oNumber, EtlTimer sync);
        /// 
        readonly DAL.ImportControl.ImportControlRepository ikr = new DAL.ImportControl.ImportControlRepository();
        readonly DAL.ImportControl.ImportCatalogRepository icr = new DAL.ImportControl.ImportCatalogRepository();
        //  EtlTimer sync = new EtlTimer();     //  Versson 2.0
        ///   
        //  int serviceId = 0; // Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]);   //  version 2.0
        int FilesRead = 0;
        string tableName = ConfigurationManager.AppSettings["TableName"];

        /// *****************************************************************************************
        /// General Tables process - Only Web/convert orders rows are processed here:      
        ///     (1) - Select order type process depending on order type.
        ///     (2) - Route process to the corresponding order types import process using delegates.
        ///     (3) - Unrecognized data types and Service reading errors are detected here.
        ///     (4) - Errors are logged in service log and reported through web services.      
        /// -----------------------------------------------------------------------------------------
        public void ProcessOrder(string order, EtlTimer sync)
        {
            /// Initialize Error messages object basic information
            string OrdNumber = "";
            ServiceResponse errMsg = new ServiceResponse();
            errMsg.FileType = "1";
            errMsg.FileName = "Catalog web Order: " + order.ToString();
            errMsg.NISOrderId = "0";
            errMsg.Status = "Not Processed";
            errMsg.Message = string.Empty;
            try
            {
                /// *********************************************************************************
                /// Select files acceptable to be process, all other file types (extension) will not
                /// be processed (they stay in the newFiles folder) and an error message is generated
                /// to the WebServices.
                /// Only Catalog Web Order (1) row are processed by entry. 
                /// ---------------------------------------------------------------------------------                   
                //  access catalog convert orders list
                if (sync.OrderType == 1)
                {   /// ======== >        
                    CatalogConvertTable convertOrder = new CatalogConvertTable(); 
                    convertOrder = icr.getCatalogConvertOrder(order);
                    /// <=========
                    OrdNumber = convertOrder.CatalogConvertOrderSummary.ReferenceId;
                    //
                    /// Instanciate NBI Tables methods delegate
                    DelConvert handler = null;
                    // Declare a class instance xml:
                    ImportProcedure_Catalog.CatalogJson.ImportProcess table = new ImportProcedure_Catalog.CatalogJson.ImportProcess();
                    /// Declare an interface instance Table:
                    ImportProcedure_Catalog.CatalogJson.ICatalogTables orderClass = (ImportProcedure_Catalog.CatalogJson.ICatalogTables)table;
                    /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    /// Identify Catalog Web order table and route them to the corresponding import procedure 
                    ///  ----------------------------------------------------------------------------     
                    //  Catalog Web Order table process
                    handler = orderClass.ProcessConvertOrder;
                    //    /// =========== >
                    bool ok = false;
                    ok = handler(convertOrder, order, sync);                 //  Process file and WebService
                    //       
                    OrdNumber = (convertOrder.CatalogConvertOrderSummary.ReferenceId).ToString();
                    //
                }
                else if (sync.OrderType == 2)
                {
                    CatalogOrderTables catalogOrder = new CatalogOrderTables();
                    catalogOrder = icr.getCatalogOrder(order);                
                    //if (catalogOrder != null && catalogOrder.CatalogWebOrderItems != null)
                    //{
                        /// Instanciate NBI Tables methods delegate
                        Del handler = null;
                        // Declare a class instance xml:
                        ImportProcedure_Catalog.CatalogJson.ImportProcess table = new ImportProcedure_Catalog.CatalogJson.ImportProcess();
                        /// Declare an interface instance Table:
                        ImportProcedure_Catalog.CatalogJson.ICatalogTables orderClass = (ImportProcedure_Catalog.CatalogJson.ICatalogTables)table;
                        /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        /// Identify Catalog Web order table and route them to the corresponding import procedure 
                        ///  ----------------------------------------------------------------------------     
                        //  Catalog Web Order table process
                        handler = orderClass.ProcessCatalogOrder;
                        //    /// =========== >
                        bool ok = false;
                        ok = handler(catalogOrder, order, sync);                 //  Process file and WebService
                        //       
                        OrdNumber = (catalogOrder.CatalogWebOrderSummary.OrderNumber).ToString();
                    //}                
                    //else
                    //{   //  update error order - set imported on and import problem on
                    //    ikr.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, "Table reading error order header not found or no items in file, order " + catalogOrder.CatalogWebOrderSummary.OrderNumber);
                    //}
                }
                else
                {   //  update error order - set imported on and import problem on
                    ikr.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, "Table reading error order header not found or no items in file, order " + OrdNumber);
                }
            }
            catch (Exception fle)
            {
                int res = ikr.updImportControl(sync.MwEtlTimerId, 0);     //  set EtlTimer for this service to not Running (isRunning = false)
                errMsg.Message = "(Catalog Web order Process) Table reading error - in order " + errMsg.FileName + ". " + fle;
                ikr.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, errMsg.Message);      
            }
        }        
    }
}

