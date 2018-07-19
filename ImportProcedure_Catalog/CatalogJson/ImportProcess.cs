using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportProcedure_Catalog.CatalogJson
{    


    public interface ICatalogTables
    {
        bool ProcessCatalogOrder(CatalogOrderTables doc, string oNumber, EtlTimer syncRow);
        bool ProcessConvertOrder(CatalogConvertTable doc, string oNumber, EtlTimer syncRow);
        //bool ProcessCatalogDesignRequest(CatalogDesignRequestTables doc, int oNumber, EtlTimer syncRow);
    }

    public class ImportProcess : ICatalogTables
    {
        /// *************************************************************************************************
        /// <summary>
        ///     07/06/2017 10:23 version
        ///     Catalog Web Orders process, un-imported web orders are retreived and  mapped to an object (header/detail 
        ///     sections).
        ///     Each section is send to WebServices as a JSON string if header/detail information is complete, 
        ///     when detail info. 
        ///     All errors are send to a Webservice (Error) and included in the Service log. 
        /// </summary>
        /// <param name="doc">Imported Web orders</param>
        /// <param name="oNumber">Web Order number</param>
        /// <param name="syncRow">EtlTimer sync control data)</param>
        /// <returns>Process status true if procees was completed ok, else false</returns>
        /// -------------------------------------------------------------------------------------------------
        bool ICatalogTables.ProcessCatalogOrder(CatalogOrderTables doc, string oNumber, EtlTimer syncRow)
        {
            //bool rStatus = false;                    //  WebService return status (default=false)
            ServiceResponse rStatus = new ServiceResponse();

            DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();  //  Set WebService methods pointer

            ///  Create error message object and initialice it     
            ServiceResponse errMsg = new ServiceResponse();
            errMsg.FileType = "1";      //  Set Nis Type
            errMsg.FileName = "Catalog Web orders Table";
            errMsg.NISOrderId = oNumber.ToString();
            errMsg.Status = "Not Processed";
            errMsg.Message = "No items found for this order header";
            /// *********************************************************************************************
            //  Order header section
            /// ---------------------------------------------------------------------------------------------
            string jsonHead = "{ \"WebOrderSummary\":" + Newtonsoft.Json.JsonConvert.SerializeObject(doc.CatalogWebOrderSummary);
            /// *********************************************************************************************
            ///  Order Detail Section
            ///  --------------------------------------------------------------------------------------------           
            /// ===============================================================================
            /// Send Imported files data content to webServices - information must be complete
            /// Order Header + all related Order Items.
            /// -------------------------------------------------------------------------------
            if (doc.CatalogWebOrderItems.Count > 0)
            {
                /// Serialize Items list and complete json string for Header and Items
                string json2 = jsonHead + ", \"WebOrderItems\" : " + Newtonsoft.Json.JsonConvert.SerializeObject(doc.CatalogWebOrderItems) + "}";
                /// ******************************************************************************
                /// Call Web Service
                /// <param name="json2">        Header/detail object Json serialized string.</param> 
                /// <param name="fname">        Xml file name </param>
                /// <param name="syncRow.WebServiceOrders">File Type, route to web service(url)</param>
                /// <param name="syncRow">      EtlTimer control data.
                /// <returns>"rStatus.IsOk" true if WebService was processed, else false</returns>
                /// *** Web service url is defined in the App.Config file key 'NisOrder'.
                /// ------------------------------------------------------------------------------
                /// ==============> save order data through the defined web service                
                rStatus = wsm.ConsumeWebService(json2, oNumber, syncRow.WebServiceOrders, syncRow);   //"OrderData", syncRow); //// TEMPORARY UNCOMMENT TO SEND JSON INFO 
                /// ------------------------------------------------------------------------------
                ///     UNCOMMENT IF WEB SERVICE IS USED TO SEND JSON INFO 
                //      rStatus.IsOK = true;         /// set order process to ok TEMPORARY,
                //      rStatus.NISOrderId = "Nav1234567";
                /// ------------------------------------------------------------------------------
                string apiMsg = "";
                int resp = 0;               //  Order update response
                DAL.ImportControl.ImportControlRepository ikr = new DAL.ImportControl.ImportControlRepository();
                DAL.ImportControl.ImportCatalogRepository icr = new DAL.ImportControl.ImportCatalogRepository();
                //
                if (rStatus.IsOk)
                {   //  set order condition to imported
                    apiMsg = " Imported";
                    resp = icr.updCatalogOrder(oNumber, rStatus.NISOrderId, 1);
                }
                else
                {   //  Set order contion to imported with errors.
                    apiMsg = " Not Imported - API errors";
                }
                string ret = ikr.writeSyncLog(1, syncRow.MwEtlTimerId, 1, syncRow.ServiceName, "> " + syncRow.Description + " Catalog Web Order: <" + doc.CatalogWebOrderSummary.OrderNumber + "> " + apiMsg + ".");
                //  <==============
            }
            else
            {   /// Incomplete order (No order items included and no xls file Name reported in orderHeader)
                jsonHead = jsonHead + " }";
                /// Send Message to WebService
                wsm.WriteErrorFile(errMsg, syncRow);    //// TEMPORARY UNCOMMENT TO SEND JSON INFO 
            }
            return rStatus.IsOk;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="oNumber"></param>
        /// <param name="syncRow"></param>
        /// <returns></returns>
        bool ICatalogTables.ProcessConvertOrder(CatalogConvertTable doc, string oNumber, EtlTimer syncRow)
        {
            ServiceResponse rStatus = new ServiceResponse();
            DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();  //  Set WebService methods pointer

            ///  Create error message object and initialice it     
            ServiceResponse errMsg = new ServiceResponse();
            errMsg.FileType = "2";      //  Set Catalog order Type
            errMsg.FileName = "Catalog Convert orders Table";
            errMsg.NISOrderId = oNumber.ToString();
            errMsg.Status = "Not Processed";
            errMsg.Message = "No items found for this order header";
            /// *********************************************************************************************
            //  Order header section
            /// ---------------------------------------------------------------------------------------------
            string json2 = "{ \"WebOrderSummary\":" + Newtonsoft.Json.JsonConvert.SerializeObject(doc.CatalogConvertOrderSummary) + "}";
            /// *********************************************************************************************
            ///  Order Detail Section
            ///  --------------------------------------------------------------------------------------------           
            /// ===============================================================================
            /// Send Imported files data content to webServices - information must be complete
            /// Order Header + all related Order Items.
            /// -------------------------------------------------------------------------------
            if (doc.CatalogConvertOrderSummary != null)
            {                
                /// ******************************************************************************
                /// Call Web Service
                /// <param name="json2">        Header/detail object Json serialized string.</param> 
                /// <param name="fname">        Xml file name </param>
                /// <param name="syncRow.WebServiceOrders">File Type, route to web service(url)</param>
                /// <param name="syncRow">      EtlTimer control data.
                /// <returns>"rStatus.IsOk" true if WebService was processed, else false</returns>
                /// *** Web service url is defined in the App.Config file key 'NisOrder'.
                /// ------------------------------------------------------------------------------
                /// ==============> save order data through the defined web service                
                rStatus = wsm.ConsumeWebService(json2, oNumber, syncRow.WebServiceOrders, syncRow);   //"OrderData", syncRow); //// TEMPORARY UNCOMMENT TO SEND JSON INFO 
                /// ------------------------------------------------------------------------------
                ///     UNCOMMENT IF WEB SERVICE IS USED TO SEND JSON INFO 
                //      rStatus.IsOK = true;         /// set order process to ok TEMPORARY,
                //      rStatus.NISOrderId = "Nav8912345";
                /// ------------------------------------------------------------------------------
                string apiMsg = "";
                int resp = 0;               //  Order update response
                DAL.ImportControl.ImportControlRepository ikr = new DAL.ImportControl.ImportControlRepository();
                DAL.ImportControl.ImportCatalogRepository icr = new DAL.ImportControl.ImportCatalogRepository();
                //
                if (rStatus.IsOk)
                {   //  set order condition to imported
                    apiMsg = " Imported";
                    resp = icr.updCatalogOrder(oNumber, rStatus.NISOrderId, 1);
                }
                else
                {   //  Set order contion to imported with errors.
                    apiMsg = " Not Imported - API errors";
                }
                string ret = ikr.writeSyncLog(1, syncRow.MwEtlTimerId, 1, syncRow.ServiceName, "> " + syncRow.Description + " Catalog Convert Order: <" + doc.CatalogConvertOrderSummary.ReferenceId + "> " + apiMsg + ".");
                //  <==============
            }
            else
            {   /// Incomplete order (No order items included and no xls file Name reported in orderHeader)
                //json2 = json2 + " }";
                /// Send Message to WebService
                wsm.WriteErrorFile(errMsg, syncRow);    //// TEMPORARY UNCOMMENT TO SEND JSON INFO 
            }
            return rStatus.IsOk;
        }


        //bool ICatalogTables.ProcessCatalogDesignRequest(CatalogDesignRequestTables doc, int oNumber, EtlTimer syncRow)
        //{
        //    // bool rStatus = false;                    //  WebService return status (default=false)
        //    ServiceResponse rStatus = new ServiceResponse();
        //    DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();  //  Set WebService methods pointer

        //    ///  Create error message object and initialice it     
        //    ServiceResponse errMsg = new ServiceResponse();
        //    errMsg.FileType = "1";      //  Set Nis Type
        //    errMsg.FileName = "Nis Table";
        //    errMsg.NISOrderId = oNumber.ToString();
        //    errMsg.Status = "Not Processed";
        //    errMsg.Message = "No items found for this order header";
        //    /// *********************************************************************************************
        //    //  Order header section
        //    /// ---------------------------------------------------------------------------------------------
        //    string json2 = "{ \"WebOrderSummary\":" + Newtonsoft.Json.JsonConvert.SerializeObject(doc.CatalogDesignRequest);
        //    /// *********************************************************************************************
        //    ///  Order Detail Section
        //    ///  --------------------------------------------------------------------------------------------           
        //    /// ===============================================================================
        //    /// Send Imported files data content to webServices - information must be complete
        //    /// Order Header + all related Order Items.
        //    /// -------------------------------------------------------------------------------
        //    //if (doc.CatalogWebOrderItems.Count > 0)
        //    //{
        //        /// Serialize Items list and complete json string for Header and Items
        //        //string json2 = jsonHead + ", \"WebOrderItems\" : " + Newtonsoft.Json.JsonConvert.SerializeObject(doc.CatalogWebOrderItems) + "}";
        //        /// ******************************************************************************
        //        /// Call Web Service
        //        /// <param name="json2">        Header/detail object Json serialized string.</param> 
        //        /// <param name="fname">        Xml file name </param>
        //        /// <param name="syncRow.WebServiceOrders">File Type, route to web service(url)</param>
        //        /// <param name="syncRow">      EtlTimer control data.
        //        /// <returns>"rStatus" true if WebService was processed, else false</returns>
        //        /// *** Web service url is defined in the App.Config file key 'NisOrder'.
        //        /// ------------------------------------------------------------------------------
        //        /// ==============> save order data through the defined web service                
        //        rStatus = wsm.ConsumeWebService(json2, oNumber.ToString(), syncRow.WebServiceDesigns, syncRow);   //"OrderData", syncRow); //// TEMPORARY UNCOMMENT TO SEND JSON INFO 
        //        //rStatus = true;         /// set order process to ok TEMPORARY, UNCOMMENT IF WEB SERVICE IS USED TO SEND JSON INFO 
        //        //  <==============
        //    //}
        //    //else
        //    //{   /// Incomplete order (No order items included and no xls file Name reported in orderHeader)
        //    //    jsonHead = jsonHead + " }";
        //    //    /// Send Message to WebService
        //    //    wsm.WriteErrorFile(errMsg, syncRow);    //// TEMPORARY UNCOMMENT TO SEND JSON INFO 
        //    //}
        //    return rStatus.IsOk;
        //}
    }
}