using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Text2Image;

namespace ImportProcedure_DesignRequest.DesignRequestJson
{


    public interface IDesignRequestTables
    {
        bool ProcessDesignRequestOrder(DesignRequest doc, string oNumber, EtlTimer syncRow);
    }

    public class ImportProcess : IDesignRequestTables
    {
        private string ClientArtImagesFolderSource = "";            //  ConfigurationManager.AppSettings["ClientArtImagesFolderSource"];
        private string ClientArtImagesFolderDestination = "";       //  ConfigurationManager.AppSettings["ClientArtImagesFolderDestination"];

        private string FTPServerIP = "";
        private string FTPUserID = "";
        private string FTPPassword = "";        

        readonly DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();  //  Set WebService methods pointer
        readonly DAL.ImportControl.ImportControlRepository ikr = new DAL.ImportControl.ImportControlRepository();
        readonly DAL.ImportControl.ImportDesignRequestRepository idr = new DAL.ImportControl.ImportDesignRequestRepository();

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
        bool IDesignRequestTables.ProcessDesignRequestOrder(DesignRequest doc, string oNumber, EtlTimer syncRow)
        {
            //bool rStatus = false;                    //  WebService return status (default=false)
            ServiceResponse rStatus = new ServiceResponse();

            Dictionary<string, string> configValues = new Dictionary<string, string>();

            FTPServerIP = ConfigurationManager.AppSettings["FTPServerIP"];              //  configValues[ConfigurationValues.FTP_SERVER_IP];
            FTPUserID   = ConfigurationManager.AppSettings["FTPUserID"];                //  configValues[ConfigurationValues.FTP_USER_ID];
            FTPPassword = ConfigurationManager.AppSettings["FTPPassword"];              //  configValues[ConfigurationValues.FTP_PASSWORD];

            // ClientArtImagesFolderSource = syncRow.ExcelFileFolder;
            ClientArtImagesFolderDestination = syncRow.ProcessedFileFolder;

            ///  Create error message object and initialice it     
            ServiceResponse errMsg = new ServiceResponse();
            errMsg.FileType = "1";      //  Set Nis Type
            errMsg.FileName = "Catalog Design Request Table";
            errMsg.NISOrderId = oNumber.ToString();
            errMsg.Status = "Not Processed";
            errMsg.Message = "Design request not found";
            /// *********************************************************************************************
            //  Design request order
            /// ---------------------------------------------------------------------------------------------   
            /// ===============================================================================
            /// Send Imported files data content to webServices - information must be complete
            /// Order Header + all related Order Items.
            /// -------------------------------------------------------------------------------
            if (doc != null)
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
                /// 
                /// Process design Request image
                /// 
                string errorMessage = " ";
                rStatus.IsOk = ProcessDesignRequestImage(doc, out errorMessage); // , errorMessage);
                ///
                string apiMsg = "";
                int resp = 0;               //  Order update response
                //
                string json2 = "{ \"WebOrderSummary\":" + Newtonsoft.Json.JsonConvert.SerializeObject(doc) + "}";
                if (rStatus.IsOk)
                {
                    /// ==============> save order data through the defined web service                
                    ///  rStatus = wsm.ConsumeWebService(json2, oNumber, syncRow.WebServiceOrders, syncRow);   //"OrderData", syncRow); //// TEMPORARY UNCOMMENT TO SEND JSON INFO 
                    /// ------------------------------------------------------------------------------
                    ///     UNCOMMENT IF WEB SERVICE IS USED TO SEND JSON INFO 
                    //rStatus.IsOk = true;         /// set order process to ok TEMPORARY,
                    rStatus.NISOrderId = "Nav1234567";
                    /// ------------------------------------------------------------------------------                    
                    if (rStatus.IsOk)
                    {   //  set order condition to imported
                        apiMsg = " Imported";
                        // resp = idr.updDesignRequest(oNumber, "Imported", 1);      
                        if (resp < 1)
                            apiMsg += " but Design Request status not updated due to DB access errors.";
                    }
                    else
                    {   //  Set order contion to imported with errors.
                        apiMsg = " Not Imported - API errors" + " Image errors: " + errorMessage;
                    }
                }
                string ret = ikr.writeSyncLog(1, syncRow.MwEtlTimerId, 1, syncRow.ServiceName, "> " + syncRow.Description + " Catalog Web Order: <" + doc.DesignId + "> " + apiMsg + ".");
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

        //  ****************************************************** IMAGES MANAGEMENT  ******************************************************

        /// <summary>
        ///     Get design request image information - create it from image or text depending iin the artwork column
        /// </summary>
        /// <param name="designRequest"></param>
        /// <param name="errorMessage"> Out parameter error message</param>
        /// <returns>boolean</returns>
        bool ProcessDesignRequestImage(DesignRequest designRequest, out string errorMessage)
        {
            bool bRetVal;
            string sRetVal = "";
            string font_1_art = "";
            string textbox_1_art = "";
            string font_2_art = "";
            string textbox_2_art = "";
            string font_3_art = "";
            string textbox_3_art = "";

            errorMessage = "";

            try
            {
                /*  ALL DESIGN REQUESTS VALIDATION WILL BE PERFORMED BY THE MIDDLEWARE  
                /// When shipping quantity is not 0 and shipping address is not blank, address edit can't be 0 
                if ((designRequest.ShipQty1 != "0") && (designRequest.ShipAddress1 != "")
                    && (designRequest.ShippingAddressesAddress1Addedit == 0 ))             // shipping_addresses_address_1_addedit == 0))
                {
                    errorMessage = Constants.ERROR_FOUND_PROCESSING_DESIGN_REQUEST_SHIPPING_ADDRESS_HAVE_NOT_BEEN_UPDATED + designRequest.DesignId;
                    return false;
                }

                if ((designRequest.ShipQty2 != "0") && (designRequest.ShipAddress2 != "")
                    && (designRequest.ShippingAddressesAddress2Addedit == 0 ))              //shipping_addresses_address_2_addedit == 0))
                {
                    errorMessage = Constants.ERROR_FOUND_PROCESSING_DESIGN_REQUEST_SHIPPING_ADDRESS_HAVE_NOT_BEEN_UPDATED + designRequest.DesignId;
                    return false;
                }


                if ((designRequest.ShipQty3 != "0") && (designRequest.ShipAddress3 != "")
                    && (designRequest.ShippingAddressesAddress3Addedit == 0 ))              //shipping_addresses_address_3_addedit == 0))
                {
                    errorMessage = Constants.ERROR_FOUND_PROCESSING_DESIGN_REQUEST_SHIPPING_ADDRESS_HAVE_NOT_BEEN_UPDATED + designRequest.DesignId;
                    return false;
                }

                //  Validate design id column, must be 6 characters.
                if (designRequest.DesignId.Length != 6)
                {
                    errorMessage = Constants.ERROR_FOUND_PROCESSING_DESIGN_REQUEST_DESIGN_ID_LENGTH_IS_NOT_VALID + designRequest.CatDesignRequestOrderId;
                    return false;
                }
                */
 
                if (designRequest.DrStitchFonts1WeiId != null) font_1_art = designRequest.DrStitchFonts1WeiId;
                if (designRequest.DrStitchFonts2WeiId != null) font_2_art = designRequest.DrStitchFonts2WeiId;
                if (designRequest.DrStitchFonts3WeiId != null) font_3_art = designRequest.DrStitchFonts3WeiId;
                if (designRequest.OptionalArtText1 != null) textbox_1_art = designRequest.OptionalArtText1;
                if (designRequest.OptionalArtText2 != null) textbox_2_art = designRequest.OptionalArtText2;
                if (designRequest.OptionalArtText3 != null) textbox_3_art = designRequest.OptionalArtText3;

                if (designRequest.Artwork.Equals("Upload"))
                {
                    if ((designRequest.DrProductTypeWeiId != "STP") && (textbox_2_art == ""))
                    {
                        errorMessage = Constants.ERROR_FOUND_PROCESSING_DESIGN_REQUEST_NO_ARTWORK_IMAGE_AVAILABLE + designRequest.CatDesignRequestOrderId;
                        return false;
                    }
                    if (textbox_2_art != "")
                    {
                        sRetVal = DownloadFileFromServerUsingWebClientFTP(FTPServerIP, FTPUserID, FTPPassword, textbox_2_art, ClientArtImagesFolderDestination, designRequest.CatDesignRequestOrderId);
                        if (sRetVal != "")
                        {
                            errorMessage = Constants.ERROR_FOUND_PROCESSING_DESIGN_REQUEST_FTP_PROBLEM + designRequest.DesignId + ". FTP Problem = " + sRetVal;
                            return false;
                        }
                    }
                }
                else
                {
                    bRetVal = CreateImageFileFromText(font_1_art, font_2_art, font_3_art, textbox_1_art, textbox_2_art, textbox_3_art, ClientArtImagesFolderDestination, designRequest.CatDesignRequestOrderId);

                    if (!bRetVal)
                    {
                        errorMessage = Constants.ERROR_FOUND_PROCESSING_DESIGN_REQUEST_SHIPPING_ADDRESS_HAVE_NOT_BEEN_UPDATED + designRequest.CatDesignRequestOrderId;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = Constants.ERROR_FOUND_PROCESSING_DESIGN_REQUEST_EXCEPTION_THROWN + designRequest.DesignId + ". Exception =" + ex.Message;
                return false;
            }
            return true;
        }


        //  Design request validateion and image processing 
        /// <summary>
        ///     Down load images from Image center folder - Using FTP
        /// </summary>
        /// <param name="ftpServerIP"></param>
        /// <param name="ftpUserID"></param>
        /// <param name="ftpPassword"></param>
        /// <param name="sourcefileName"></param>
        /// <param name="destinationfilePath"></param>
        /// <param name="destinationfileName"></param>
        /// <returns></returns>
        private string DownloadFileFromServerUsingWebClientFTP(string ftpServerIP, string ftpUserID, string ftpPassword, string sourcefileName, string destinationfilePath, string destinationfileName)
        {
            try
            {
                //Uri serverUri = new Uri(ftpServerIP + sourcefileName);
                WebClient request = new WebClient();
                request.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                //sourcefileName format: 201103/abinbev.jpg_3_23_2011_1_55_00_PM_wa09.JPG
                // incorrect: http://media.webstorepackage.com/wei/201103/abinbev.jpg_3_23_2011_1_55_00_PM_wa09.JPG
                request.DownloadFile(ftpServerIP + sourcefileName, destinationfilePath + destinationfileName + "." + sourcefileName.Substring(sourcefileName.Length - 3));
                return "";
            }
            catch (WebException ex)
            {
                return ex.ToString();
            }
        }


        /// <summary>
        ///     Create Image File from image Text 
        /// </summary>
        /// <param name="font_1"></param>
        /// <param name="font_2"></param>
        /// <param name="font_3"></param>
        /// <param name="line_1"></param>
        /// <param name="line_2"></param>
        /// <param name="line_3"></param>
        /// <param name="destinationfilePath"></param>
        /// <param name="destinationfileName"></param>
        /// <returns></returns>
        private bool CreateImageFileFromText(string font_1, string font_2, string font_3, string line_1, string line_2, string line_3, string destinationfilePath, string destinationfileName)
        {
            try
            {
                bool bRetVal = false;
                Text2Image.ComClassText2Image _Text2Image = new ComClassText2Image();
                bRetVal = _Text2Image.GenerateImageFromText(line_1, line_2, line_3, font_1, font_2, font_3, destinationfilePath + destinationfileName + ".jpg", true);
                return bRetVal;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
