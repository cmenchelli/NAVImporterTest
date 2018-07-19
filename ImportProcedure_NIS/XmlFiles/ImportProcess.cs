using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ImportProcedure_NIS.XmlFiles
{
    public interface IXmlFiles
    {
        bool ProcessNisFiles(XmlDocument doc, string fName, EtlTimer syncRow);
    }

    public class ImportProcess : IXmlFiles
    {
        /// *************************************************************************************************
        /// <summary>
        ///     11/03/2015 10:23 version
        ///     NIS Files process, Xml document mapped to an object and splited into order header/detail 
        ///     sections.
        ///     Each section is send to WebServices as a JSON string if header/detail information is complete, 
        ///     when detail info. is not included in the xml document, it must have an excel file associated 
        ///     with the detail information (Header filename field).
        ///     All errors are send to a Webservice (Error) and included in the Service log. 
        /// </summary>
        /// <param name="doc">Imported file content read as an XmlDocument</param>
        /// <param name="fName">File name (excluding path section)</param>
        /// <param name="fName">EtlTimer sync control data)</param>
        /// <returns>Process status true if procees was completed ok, else false</returns>
        /// -------------------------------------------------------------------------------------------------
        bool IXmlFiles.ProcessNisFiles(XmlDocument doc, string fName, EtlTimer syncRow)
        {
            //bool rStatus = false;                    //  WebService return status (default=false)
            ServiceResponse rStatus = new ServiceResponse();
            DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();  //  Set WebService methods pointer
            DAL.DataValidationServices.DataValidation dvs = new DAL.DataValidationServices.DataValidation();

            ///  Create error message object and initialice it     
            ServiceResponse errMsg = new ServiceResponse();
            errMsg.FileType = "1";      //  Set Nis Type
            errMsg.FileName = fName + ".xml";
            errMsg.NISOrderId = "0";
            errMsg.Status = "Not Processed";
            errMsg.Message = "No items found for this order header";
            /// *********************************************************************************************
            //  Order header section
            /// ---------------------------------------------------------------------------------------------
            XmlNode header = doc.DocumentElement.SelectSingleNode("/Order/OrderHeader");
            Header head = new Header();
            ///           
            head.HeaderId           = dvs.SingleNodeValidation_Text(header, "ID", 6);
            head.HeaderUserId       = dvs.SingleNodeValidation_Text(header, "userID", 6);
            head.HeaderUserEmail    = dvs.SingleNodeValidation_Text(header, "UserEmail", 60);
            head.HeaderOrderDate    = dvs.SingleNodeValidation_Text(header, "orderDate", 30);
            head.HeaderPO           = dvs.SingleNodeValidation_Text(header, "PO", 25);
            head.HeaderDateRequested = dvs.SingleNodeValidation_Text(header, "dateReq", 30);
            head.HeaderDescription  = dvs.SingleNodeValidation_Text(header, "description", 0);
            head.HeaderShipVia1     = dvs.SingleNodeValidation_Text(header, "shippingViaID", 2);
            head.HeaderShipVia2     = dvs.SingleNodeValidation_Text(header, "shippingViaID2", 2);
            head.HeaderShipToId1    = dvs.SingleNodeValidation_Text(header, "shippingToID", 0);
            head.HeaderShipToId2    = dvs.SingleNodeValidation_Text(header, "shippingToID2", 0);
            head.HeaderComments     = dvs.SingleNodeValidation_Text(header, "comments", 0);
            head.FileName           = dvs.SingleNodeValidation_Text(header, "ItemsFileName", 0);
            head.HeaderSentDate     = dvs.SingleNodeValidation_Text(header, "sentDate", 30);
            ///
            string jsonHead = "{ \"nisOrderSummary\":" + Newtonsoft.Json.JsonConvert.SerializeObject(head);
            /// *********************************************************************************************
            ///  Order Detail Section
            ///  --------------------------------------------------------------------------------------------
            XmlNodeList orders = doc.DocumentElement.SelectNodes("/Order/OrderItems");
            ///
            List<Order> ordList = new List<Order>();
            int itm = 1;
            foreach (XmlNode order in orders)
            {
                Order ord = new Order();
                ord.NisOrderId  = itm.ToString();
                ord.Itemid      = dvs.SingleNodeValidation_Text(order, "ID", 6); ;
                ord.OrderId     = dvs.SingleNodeValidation_Text(order, "orderID", 6);
                ord.SkuId       = dvs.SingleNodeValidation_Text(order, "SKU_ID", 3);
                ord.LineId      = dvs.SingleNodeValidation_Int(order, "lineID");
                ord.Quantity1   = dvs.SingleNodeValidation_Int(order, "qty1");
                ord.Quantity2   = dvs.SingleNodeValidation_Int(order, "qty2");
                ord.Line1       = dvs.SingleNodeValidation_Text(order, "line1", 40);
                ord.Line2       = dvs.SingleNodeValidation_Text(order, "line2", 40);
                ord.Line3       = dvs.SingleNodeValidation_Text(order, "line3", 40);
                ord.Comment     = dvs.SingleNodeValidation_Text(order, "comment", 100);
                ord.Description = dvs.SingleNodeValidation_Text(order, "description", 0);
                ord.SkuClient   = dvs.SingleNodeValidation_Text(order, "SKU_client", 0);
                ord.SkuWe       = dvs.SingleNodeValidation_Text(order, "SKU_WE", 30);
                ord.AccountCode = dvs.SingleNodeValidation_Text(order, "AccountCode", 0);
                ///
                itm++;
                ordList.Add(ord);
            }
            /// ===========================================================================================            
            /// When an Order don't include items, order items must be provided through an xls / xlsx file 
            /// indicated in the Header "FileName" field.
            /// If fileName is not indicated/provided, the corresponding order must remain pending in the
            /// problem directory until it can be processed, an entry in the ServiceLog must be generated 
            /// to report this situation and a WebService error message is send.
            /// Old system ==> subImportXLSFile
            /// -------------------------------------------------------------------------------------------
            if (ordList.Count == 0)
            {
                if (String.IsNullOrEmpty(head.FileName))
                {
                    /// Incomplete order (No order items included and no xls file Name reported in orderHeader)
                    errMsg.Message = "No items in xml file and no xls file reported in Header (fileName)";
                }
                else
                {
                    /// ===================================================================================================
                    /// read xls file using Header FileName, build order items List and add it to header section.                  
                    /// ----------------------------------------------------------------------------------------------------
                    //  string path = ConfigurationManager.AppSettings["ExcelFilesDirectory"].ToString() + @"\" + head.FileName;
                    string path = syncRow.ExcelFileFolder + @"\" + head.FileName;
                    DataTable dt = ImportProcedure_NIS.ExcelFiles.ImportProcess.ReadExcelFile(path, "Items", "*", head.FileName, false);
                    ///
                    if ((dt == null) || (dt.Rows.Count == 0))
                    {   /// Incomplete order (associated xls file name was not found)
                        errMsg.Message = "Associated xls/xlsx file <" + head.FileName + "> not found";
                        /// -------------------------------------------------------------------------
                    }
                    else
                    {   /// Built items list from xls/xlsx file content    
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string it = Convert.ToString(dt.Rows[i].ItemArray[5]);
                            if (!String.IsNullOrEmpty(it))
                            {
                                /// All empty rows Qty(G) are ignored, when current row is empty the process 
                                /// continue. Associated file may have additional filled rows that must be 
                                /// included.
                                Order ord = new Order();
                                /// process all existing items 
                                ord.NisOrderId = head.HeaderId;         //  Header file name
                                ord.Itemid = string.Empty;
                                ord.OrderId = head.HeaderId;
                                ord.SkuId = (i + 1).ToString();
                                /// Quantity Qty1/Qty2 nodes
                                int q1;
                                bool n = Int32.TryParse(dt.Rows[i].ItemArray[5].ToString(), out q1);
                                ord.Quantity1 = q1;                                 //  ok
                                ord.Quantity2 = 0;                                  //  ok
                                /// Line Line1-line3 nodes 
                                ord.Line1 = dt.Rows[i].ItemArray[2].ToString();     //  ok
                                ord.Line2 = dt.Rows[i].ItemArray[3].ToString();     //  ok
                                ord.Line3 = dt.Rows[i].ItemArray[4].ToString();     //  ok
                                /// This counter must be after storing Line1 .. Line3
                                /// Conts the number of lines included in this row.
                                ord.LineId = 0;
                                if (!string.IsNullOrEmpty(ord.Line1))
                                    ord.LineId++;
                                if (!string.IsNullOrEmpty(ord.Line2))
                                    ord.LineId++;
                                if (!string.IsNullOrEmpty(ord.Line3))
                                    ord.LineId++;
                                ///
                                ord.Comment = dt.Rows[i].ItemArray[7].ToString();   //  ok
                                ord.Description = "";
                                ord.SkuClient = dt.Rows[i].ItemArray[1].ToString();
                                ord.SkuWe = dt.Rows[i].ItemArray[0].ToString();
                                /// Add row in detail list
                                ordList.Add(ord);
                            }
                        }
                    }
                }
            }
            /// ===============================================================================
            /// Send Imported files data content to webServices - information must be complete
            /// Order Header + all related Order Items.
            /// -------------------------------------------------------------------------------
            if (ordList.Count > 0)
            {
                /// Serialize Items list and complete json string for Header and Items
                string json2 = jsonHead + ", \"nisOrderItems\" : " + Newtonsoft.Json.JsonConvert.SerializeObject(ordList) + "}";
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
                rStatus = wsm.ConsumeWebService(json2, fName, syncRow.WebServiceOrders, syncRow);   //"OrderData", syncRow);
                //  <==============
            }
            else
            {   /// Incomplete order (No order items included and no xls file Name reported in orderHeader)
                jsonHead = jsonHead + " }";
                /// Send Message to WebService
                rStatus.IsOk = false;
                wsm.WriteErrorFile(errMsg, syncRow);
            }
            return rStatus.IsOk;
        }

        //    /// *************************************************************************************************
        //    /// <summary>
        //    ///     09/22/2015 16:39 version
        //    ///     Attribute retreival and validation, any null value returns as " ".
        //    /// </summary>
        //    /// <param name="nod">XmlNode where the attibute is located</param>
        //    /// <param name="attName">Attribute name</param>
        //    /// <returns>String, Int, DateTime, Decimal depending on the validation type</returns>
        //    /// -------------------------------------------------------------------------------------------------       
        //    private static string AttributeValidation_String(XmlNode nod, string attName)
        //    {
        //        string ret = string.Empty;
        //        try
        //        {
        //            ret = nod.Attributes[attName].Value;
        //        }
        //        catch
        //        {
        //            ret = "";
        //        }
        //        return ret;

        //        //return nod.Attributes[attName].Value ?? " ";
        //    }

        //    private static int AttributeValidation_Int(XmlNode nod, string attName)
        //    {
        //        int ret = 0;
        //        try
        //        {
        //            string val = nod.Attributes[attName].Value;
        //            ret = Int32.Parse(val);
        //        }
        //        catch
        //        {
        //            ret = 0;
        //        }
        //        return ret;
        //    }

        //    private static DateTime AttributeValidation_Date(XmlNode nod, string attName)
        //    {
        //        DateTime ret = new DateTime();
        //        if (nod.Attributes[attName] != null)
        //        {
        //            try
        //            {
        //                string dat = AttributeValidation_String(nod, attName);
        //                ret = Convert.ToDateTime(dat);
        //            }
        //            catch
        //            { }
        //        }
        //        return ret;
        //    }

        //    private static Decimal AttributeValidation_Dec(XmlNode nod, string attName)
        //    {
        //        Decimal ret = 0;
        //        try
        //        {
        //            string val = nod.Attributes[attName].Value;
        //            ret = Decimal.Parse(val);
        //        }
        //        catch
        //        {
        //            ret = 0;
        //        }
        //        return ret;
        //    }

        //    private static string SingleNodeValidation_Text(XmlNode nod, string attName)
        //    {
        //        string ret = " ";
        //        try
        //        {
        //            ret = nod.SelectSingleNode(attName).InnerText;
        //        }
        //        catch
        //        { }
        //        return ret;
        //    }

        //    private static int SingleNodeValidation_Int(XmlNode nod, string attName)
        //    {
        //        int ret = 0;
        //        try
        //        {
        //            ret = Int32.Parse(nod.SelectSingleNode(attName).InnerText);
        //        }
        //        catch
        //        { }
        //        return ret;
        //    }

        //    private static string FirstChildValidation_Inner(XmlNode nod)
        //    {
        //        string iText = " ";
        //        try
        //        {
        //            iText = nod.FirstChild.InnerText;
        //        }
        //        catch
        //        { }
        //        return iText;
        //    }
    }
}
