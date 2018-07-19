using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportProcedure_NIS_V2.BuildJason
{
    public interface INISTables
    {
        bool ProcessNisOrder(NisTables doc, int oNumber, EtlTimer syncRow);
    }

    public class ImportProcess : INISTables
    {
        /// *************************************************************************************************
        /// <summary>
        ///     07/27/2017 11:27 version
        ///     NIS Orders process, Order data come as an object splited into order header/detail sections.
        ///     Each section is send to WebServices as a JSON string if header/detail information is complete, 
        ///     when detail info. 
        ///     All errors are send to a Webservice (Error) and included in the Service log. 
        /// </summary>
        /// <param name="doc">Imported order content as a NisTables class</param>
        /// <param name="oNumber">Order number</param>
        /// <param name="fName">EtlTimer sync control data)</param>
        /// <returns>Process status true if procees was completed ok, else false</returns>
        /// -------------------------------------------------------------------------------------------------
        bool INISTables.ProcessNisOrder(NisTables doc, int oNumber, EtlTimer syncRow)
        {
            /// Response from MW api
            ServiceResponse rStatus = new ServiceResponse();
            DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();  //  Set WebService methods pointer
            DAL.ImportControl.ImportControlRepository icr = new DAL.ImportControl.ImportControlRepository();
            ///  Create error message object and initialice it     
            ServiceResponse errMsg = new ServiceResponse();
            errMsg.FileType = "1";      //  Set Nis Type
            errMsg.FileName = "NIS Table";
            errMsg.NISOrderId = oNumber.ToString();
            errMsg.Status = "Not Processed";
            errMsg.Message = "No items found for this order header";
            /// *********************************************************************************************
            //  Order header section
            /// ---------------------------------------------------------------------------------------------
            string jsonHead = "{ \"nisOrderSummary\":" + Newtonsoft.Json.JsonConvert.SerializeObject(doc.header);
            /// *********************************************************************************************
            ///  Order Detail Section
            ///  --------------------------------------------------------------------------------------------           
            /// ===============================================================================
            /// Send Imported files data content to webServices - information must be complete
            /// Order Header + all related Order Items.
            /// -------------------------------------------------------------------------------
            if (doc.items.Count > 0)
            {
                /// Serialize Items list and complete json string for Header and Items
                string json2 = jsonHead + ", \"nisOrderItems\" : " + Newtonsoft.Json.JsonConvert.SerializeObject(doc.items) + "}";
                /// ******************************************************************************
                /// Call Web Service
                /// <param name="json2">        Header/detail object Json serialized string.</param> 
                /// <param name="fname">        Xml file name </param>
                /// <param name="syncRow.WebServiceOrders">File Type, route to web service(url)</param>
                /// <param name="syncRow">      EtlTimer control data.
                /// <returns>"rStatus" true if WebService was processed, else false</returns>
                /// *** Web service url is defined in the App.Config file key 'NisOrder'.
                /// ------------------------------------------------------------------------------
                /// ==============> save order data through the defined web service                
                rStatus = wsm.ConsumeWebService(json2, oNumber.ToString(), syncRow.WebServiceOrders, syncRow);   //"OrderData", syncRow); //// TEMPORARY UNCOMMENT TO SEND JSON INFO 
                ///
                //rStatus.IsOk = true;          /// set order process to ok TEMPORARY, UNCOMMENT IF WEB SERVICE IS USED TO SEND JSON INFO 
                /// 
                string apiMsg = "";
                int resp = 0;                   ///  Order update response
                int uOption = 0;
                int impProb = 0;
                int navOrder = 0;
                DAL.ImportControl.ImportControlRepository ikr = new DAL.ImportControl.ImportControlRepository();
                DAL.ImportControl.ImportNISRepository inr = new DAL.ImportControl.ImportNISRepository();
                //
                if (rStatus.Status == "OK")
                {   //  set order condition to imported
                    apiMsg = " Imported";
                    uOption = 1;
                    rStatus.IsOk = true;
                    navOrder = Convert.ToInt32(rStatus.NISOrderId);                    
                    ///
                    /// Save processed files log - May16-2017
                    ///
                    int res = ikr.writeImportLog(syncRow.MwEtlTimerId, "NIS", doc.header.HeaderId.ToString());
                }
                else
                {   //  Set order contion to imported with errors.
                    impProb = 1;
                    apiMsg = " Not Imported - API errors";
                    rStatus.IsOk = false;
                }
                resp = inr.updNisOrder(oNumber, uOption, impProb, navOrder);   // ************  MUST UPDATE NIS ORDER    *******************
                //
                string ret = ikr.writeSyncLog(1, syncRow.MwEtlTimerId, 1, syncRow.ServiceName, "> " + syncRow.Description + " NIS Order: <" + doc.header.HeaderId + "> " + apiMsg + ".");
                //  <==============
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
    }
}
