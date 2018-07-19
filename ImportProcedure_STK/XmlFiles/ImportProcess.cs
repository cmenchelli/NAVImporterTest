using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ImportProcedure_STK.XmlFiles
{
    public interface IXmlFiles
    {
        bool ProcessStkAccountFiles(XmlDocument doc, string fName, EtlTimer syncRow);
        bool ProcessStkDesignFiles(XmlDocument doc, string fName, EtlTimer syncRow);
        bool ProcessStkOrderFiles(XmlDocument doc, string fName, EtlTimer syncRow);
    }

    public class ImportProcess : IXmlFiles
    {
        readonly DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();
        readonly DAL.DataValidationServices.DataValidation dvs = new DAL.DataValidationServices.DataValidation();

        /// *************************************************************************************************
        /// <summary>
        ///     Version 3.0 Feb20-17 
        ///     Website-Account Xml document is mapped to Object, then passed to WebServices as a JSON string.
        /// </summary>
        /// <param name="doc">Imported file content read as an XmlDocument</param>
        /// <param name="fName">File name (excluding path section)</param>
        /// <returns>Process status true if procees was completed ok, else false</returns>
        /// -------------------------------------------------------------------------------------------------
        bool IXmlFiles.ProcessStkAccountFiles(XmlDocument doc, string fName, EtlTimer syncRow)
        {
            //bool rStatus = false;
            ServiceResponse rStatus = new ServiceResponse();            
            ///  ----------------------------------------------------------------------
            //  Account section
            ///  ----------------------------------------------------------------------
            XmlNode account = doc.DocumentElement.SelectSingleNode("/account");
            StkAccount ord  = new StkAccount();
            ord.AccountId   = dvs.AttributeValidation_String(account, "accountid", 20);
            ///  ----------------------------------------------------------------------
            //  Locale Section
            ///  ----------------------------------------------------------------------
            XmlNode locale  = doc.DocumentElement.SelectSingleNode("/account/locale");
            ord.Language    = dvs.AttributeValidation_String(locale, "lang", 5);
            ord.SiteUsed    = dvs.AttributeValidation_String(locale, "siteused", 10);
            ord.Currency    = dvs.AttributeValidation_String(locale, "currency", 10);
            ///  ----------------------------------------------------------------------
            //  Contact (account) section 
            ///  ----------------------------------------------------------------------
            XmlNode contact = doc.DocumentElement.SelectSingleNode("/account/contact");
            ord.Email       = dvs.SingleNodeValidation_Text(contact, "email", 60);
            ord.Title       = dvs.SingleNodeValidation_Text(contact, "title", 20);
            ord.Forename    = dvs.SingleNodeValidation_Text(contact, "forename", 70);
            ord.Surname     = dvs.SingleNodeValidation_Text(contact, "surname", 70);
            ord.CompanyName = dvs.SingleNodeValidation_Text(contact, "company", 300);
            ord.Industry    = dvs.SingleNodeValidation_Text(contact, "industry", 200);
            ord.Telephone   = dvs.SingleNodeValidation_Text(contact, "telephone", 30);
            ///  ----------------------------------------------------------------------
            //  Address Section Process - (Cardholder)
            ///  ----------------------------------------------------------------------
            XmlNode cardHolder = doc.DocumentElement.SelectSingleNode("/account/address[@type='cardholder']");
            ord.AddressCardHolderTown       = dvs.SingleNodeValidation_Text(cardHolder, "town", 150);
            ord.AddressCardHolderCounty     = dvs.SingleNodeValidation_Text(cardHolder, "county", 150);
            ord.AddressCardHolderPostCode   = dvs.SingleNodeValidation_Text(cardHolder, "postcode", 30);
            ord.AddressCardHolderCountry    = dvs.SingleNodeValidation_Text(cardHolder, "country", 200);
            //  Line section within Cardholder address section
            XmlNodeList xLt = cardHolder.SelectNodes("/account/address[@type='cardholder']/line");
            foreach (XmlNode xc in xLt)
            {
                int index = dvs.AttributeValidation_Int(xc, "index");
                switch (index)
                {
                    case 1:
                        ord.AddressCardHolderLine1 = xc.InnerText;
                        break;
                    case 2:
                        ord.AddressCardHolderLine2 = xc.InnerText;
                        break;
                    default:
                        break;
                }
            }
            ///  ----------------------------------------------------------------------
            //  Address Section - (Delivery)
            ///  ----------------------------------------------------------------------
            XmlNodeList delivery = doc.DocumentElement.SelectNodes("/account/address[@type='delivery']");
            foreach (XmlNode del in delivery)
            {
                /// Contact information within delivery address
                XmlNode delLine = del.SelectSingleNode("/account/address[@type='delivery']/contact");
                ord.AddressDeliveryForename = dvs.SingleNodeValidation_Text(delLine, "forename", 70);
                ord.AddressDeliverySurname  = dvs.SingleNodeValidation_Text(delLine, "surname", 70);
                ord.AddressDeliveryTelephone = dvs.SingleNodeValidation_Text(delLine, "telephone", 30);
                ///
                ord.AddressDeliveryCompany = dvs.SingleNodeValidation_Text(del, "company", 300);
                /// Line section within Delivery address section
                XmlNodeList xnLine = del.SelectNodes("/account/address[@type='delivery']/line");
                foreach (XmlNode xd in xnLine)
                {
                    int index = dvs.AttributeValidation_Int(xd, "index");
                    switch (index)
                    {
                        case 1:
                            ord.AddressDeliveryLine1 = xd.InnerText;
                            break;
                        case 2:
                            ord.AddressDeliveryLine2 = xd.InnerText;
                            break;
                        default:
                            break;
                    }
                }
                ord.AddressDeliveryTown     = dvs.SingleNodeValidation_Text(del, "town", 150);
                ord.AddressDeliveryCounty   = dvs.SingleNodeValidation_Text(del, "county", 150);
                ord.AddressDeliveryPostCode = dvs.SingleNodeValidation_Text(del, "postcode", 30);
                ord.AddressDeliveryCountry  = dvs.SingleNodeValidation_Text(del, "country", 200);
            }
            ord.FileName = fName + ".xml";

            string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(ord);
            /// ******************************************************************************
            /// Call Web Service    - "WebAccount"
            /// <param name="json2">Object serialized Json string.</param> 
            /// <param name="fname">Xml file name </param>
            /// <param name="type">File Type (WebAccounts) </param>
            /// <returns>"rStatus" true if WebService was processed, else false</returns>
            /// Web service url is defined in the App.Config file
            /// ------------------------------------------------------------------------------
            rStatus = wsm.ConsumeWebService(json2, fName, syncRow.WebServiceAccounts, syncRow);
            //  <==============
            return rStatus.IsOk;
        }

        /// *************************************************************************************************
        /// <summary>
        ///     Version 3.0 Feb20-17 
        ///     Website-Design Xml File is mapped to Object, then passed to WebServices as a JSON string.
        /// </summary>
        /// <param name="doc">Imported file content read as an XmlDocument</param>
        /// <param name="fName">File name (excluding path section)</param>
        /// <returns>Process status true if procees was completed ok, else false</returns>
        /// -------------------------------------------------------------------------------------------------
        bool IXmlFiles.ProcessStkDesignFiles(XmlDocument doc, string fName, EtlTimer syncRow)
        {
            StkDesign des = new StkDesign();
            //bool rStatus = false;
            ServiceResponse rStatus = new ServiceResponse();
            /// ----------------------------------------------------------------------
            //  Design section
            /// ----------------------------------------------------------------------
            XmlNode design = doc.DocumentElement.SelectSingleNode("/design");
            des.StyleNum    = dvs.AttributeValidation_String(design, "stylenum", 0);
            des.DesignID    = dvs.AttributeValidation_String(design, "designid", 0);
            des.TimeStamp   = dvs.AttributeValidation_Date(design, "timestamp");
            /// ----------------------------------------------------------------------
            //  Specs Section
            /// ----------------------------------------------------------------------
            XmlNode specs = doc.DocumentElement.SelectSingleNode("/design/specs");
            des.Proof       = dvs.AttributeValidation_String(specs, "proof", 0);
            des.lines       = dvs.AttributeValidation_String(specs, "lines", 0);
            des.Fitting     = dvs.AttributeValidation_String(specs, "fitting", 0);
            des.Bar         = dvs.AttributeValidation_String(specs, "bar", 0);
            des.Vinyl       = dvs.AttributeValidation_String(specs, "vinyl", 0);
            des.Code        = dvs.AttributeValidation_String(specs, "code", 0);
            /// ----------------------------------------------------------------------
            //  Background Section
            /// ----------------------------------------------------------------------
            XmlNode background = doc.DocumentElement.SelectSingleNode("/design/background");
            des.Src         = dvs.AttributeValidation_String(background, "src", 0);
            /// ----------------------------------------------------------------------
            //  Contact (account) section process
            /// ----------------------------------------------------------------------
            XmlNodeList frag = doc.DocumentElement.SelectNodes("/design/fragments/logo");
            WebDesignFragments fragments = new WebDesignFragments();
            foreach (XmlNode logo in frag)
            {
                fragments.Src   = dvs.AttributeValidation_String(logo, "src", 0);
                fragments.Top   = dvs.AttributeValidation_String(logo, "top", 0);
                fragments.Left  = dvs.AttributeValidation_String(logo, "left", 0);
                fragments.Height = dvs.AttributeValidation_String(logo, "height", 0);
                fragments.Width = dvs.AttributeValidation_String(logo, "width", 0);
                fragments.Index = dvs.AttributeValidation_Int(logo, "index");
            }
            des.logo = fragments;
            /// ----------------------------------------------------------------------
            //  Address Section Process - (Delivery)
            /// ----------------------------------------------------------------------
            XmlNodeList Text = doc.DocumentElement.SelectNodes("/design/fragments");

            List<WebDesignText> texts = new List<WebDesignText>();
            foreach (XmlNode txt in Text)
            {
                XmlNodeList tNodes = doc.SelectNodes("/design/fragments/text");
                foreach (XmlNode tNode in tNodes)
                {
                    WebDesignText cnt1 = new WebDesignText();
                    //
                    cnt1.Index  = dvs.AttributeValidation_Int(tNode, "index");
                    cnt1.Color  = dvs.AttributeValidation_String(tNode, "color", 0);
                    cnt1.Size   = dvs.AttributeValidation_String(tNode, "size", 0);
                    cnt1.Style  = dvs.AttributeValidation_String(tNode, "style", 0);
                    cnt1.Font   = dvs.AttributeValidation_String(tNode, "font", 0);
                    cnt1.Y      = dvs.AttributeValidation_String(tNode, "y", 0);
                    cnt1.X      = dvs.AttributeValidation_String(tNode, "x", 0);
                    cnt1.Content = dvs.AttributeValidation_String(tNode, "content", 0);
                    //
                    texts.Add(cnt1);
                }
            }
            des.texts = texts;
            des.Filename = fName + ".xml";
            string test = Newtonsoft.Json.JsonConvert.SerializeObject(des);
            /// ******************************************************************************
            /// Call Web Service - "WebDesign" 
            /// <param name="json2">Include Header serialized + items seialized info.</param> 
            /// <param name="fname">File to process name </param>
            /// <param name="type">File Type, determine what service to use (url)</param>
            /// <returns>"rStatus" true if WebService was processed, else false</returns>
            /// Web service url is defined in the App.Config file
            /// ------------------------------------------------------------------------------
            rStatus = wsm.ConsumeWebService(test, fName, syncRow.WebServiceDesigns, syncRow);
            //  <==============
            return rStatus.IsOk;  //  MW MUST RETURN OK CONDITION ON DESIGN PROCESSING
            //return true;
        }

        /// *************************************************************************************************
        /// <summary>
        ///     Version 3.0 Feb20-17 
        ///     Website-Account Xml File is mapped to Object, then passed to WebServices as a JSON string.
        /// </summary>
        /// <param name="doc">Imported file content read as an XmlDocument</param>
        /// <param name="fName">File name (excluding path section)</param>
        /// <returns>Process status true if procees was completed ok, else false</returns>
        /// -------------------------------------------------------------------------------------------------
        bool IXmlFiles.ProcessStkOrderFiles(XmlDocument doc, string fName, EtlTimer syncRow)
        {
            //bool rStatus = false;
            ServiceResponse rStatus = new ServiceResponse();
            StkOrder summary = new StkOrder();
            StkHeader ord = new StkHeader();
            /// ---------------------------------------------------------------------
            //  Order section
            /// ---------------------------------------------------------------------
            ord.OrderType = "stickers";
            XmlNode order   = doc.DocumentElement.SelectSingleNode("/order");
            ord.OrderId     = dvs.AttributeValidation_String(order, "orderid", 15);
            ord.OrderNumber = dvs.AttributeValidation_String(order, "ordernum", 15);
            /// ---------------------------------------------------------------------
            //  Account section
            /// ---------------------------------------------------------------------
            XmlNode account = doc.DocumentElement.SelectSingleNode("/order/account");
            ord.AccountId   = dvs.AttributeValidation_String(account, "accountid", 15);
            ord.AccountNum  = dvs.AttributeValidation_String(account, "accountnum", 15);
            /// ---------------------------------------------------------------------
            //  Locale Section
            /// ---------------------------------------------------------------------
            XmlNode locale = doc.DocumentElement.SelectSingleNode("/order/locale");
            ord.Language = dvs.AttributeValidation_String(locale, "lang", 5);
            ord.SiteUsed = dvs.AttributeValidation_String(locale, "siteused", 10);
            ord.Currency = dvs.AttributeValidation_String(locale, "currency", 10);
            /// ---------------------------------------------------------------------
            //  profile section
            /// ---------------------------------------------------------------------
            XmlNode profile = doc.DocumentElement.SelectSingleNode("/order/profile");
            ord.VatNum = dvs.AttributeValidation_String(profile, "vatnum", 15);
            ord.Credit = dvs.AttributeValidation_String(profile, "credit", 20);
            /// ---------------------------------------------------------------------
            //  Contact (Order) section
            /// ---------------------------------------------------------------------
            XmlNode contact = doc.DocumentElement.SelectSingleNode("/order/contact");
            ord.Email       = dvs.SingleNodeValidation_Text(contact, "email", 60);
            ord.Title       = dvs.SingleNodeValidation_Text(contact, "title", 20);
            ord.Forename    = dvs.SingleNodeValidation_Text(contact, "forename", 70);
            ord.Surname     = dvs.SingleNodeValidation_Text(contact, "surname", 70);
            ord.CompanyName = dvs.SingleNodeValidation_Text(contact, "company", 300);
            ord.Industry    = dvs.SingleNodeValidation_Text(contact, "industry", 200);
            ord.Source      = dvs.SingleNodeValidation_Text(contact, "source", 50);
            ord.Telephone   = dvs.SingleNodeValidation_Text(contact, "telephone", 30);
            /// ---------------------------------------------------------------------
            //  Address Section - (Cardholder)
            /// ---------------------------------------------------------------------
            XmlNode cardHolder = doc.DocumentElement.SelectSingleNode("/order/address[@type='cardholder']");
            //  ------------------------------------------------------------------
            ord.AddressCardHolderTown       = dvs.SingleNodeValidation_Text(cardHolder, "town", 150);
            ord.AddressCardHolderCounty     = dvs.SingleNodeValidation_Text(cardHolder, "county", 150);
            ord.AddressCardHolderPostCode   = dvs.SingleNodeValidation_Text(cardHolder, "postcode", 30);
            ord.AddressCardHolderCountry    = dvs.SingleNodeValidation_Text(cardHolder, "country", 200);
            /// Cardholder Line section within 
            XmlNodeList xLt = doc.DocumentElement.SelectNodes("/order/address[@type='cardholder']/line");
            foreach (XmlNode xn in xLt)
            {
                int index = dvs.AttributeValidation_Int(xn, "index");
                switch (index)
                {
                    case 1:
                        ord.AddressCardHolderLine1 = xn.InnerText;              //  300
                        if (ord.AddressCardHolderLine1.Length > 300)
                        {
                            string txt = ord.AddressCardHolderLine1.Substring(0, 300);
                            ord.AddressCardHolderLine1 = txt;
                        }
                        break;
                    case 2:
                        ord.AddressCardHolderLine2 = xn.InnerText;              //  300
                        if (ord.AddressCardHolderLine2.Length > 300)
                        {
                            string txt = ord.AddressCardHolderLine2.Substring(0, 300);
                            ord.AddressCardHolderLine2 = txt;
                        }
                        break;
                    default:
                        break;
                }
            }
            /// ----------------------------------------------------------------------
            //  Address Section Process - (Delivery)
            //  ----------------------------------------------------------------------
            XmlNode delivery = doc.DocumentElement.SelectSingleNode("/order/address[@type='delivery']");
            /// -------------------------------------------------------------------
            //ord.AddressDeliveryCompany = SingleNodeValidation_Text(delivery, "company");
            ord.AddressDeliveryTown     = dvs.SingleNodeValidation_Text(delivery, "town", 150);
            ord.AddressDeliveryCounty   = dvs.SingleNodeValidation_Text(delivery, "county", 150);
            ord.AddressDeliveryPostCode = dvs.SingleNodeValidation_Text(delivery, "postcode", 30);
            ord.AddressDeliveryCountry  = dvs.SingleNodeValidation_Text(delivery, "country", 200);
            //  Contact Section within Address section (Delivery type)      
            XmlNode delContact = doc.DocumentElement.SelectSingleNode("/order/address[@type='delivery']/contact");
            ord.AddressDeliveryForename = delContact.SelectSingleNode("forename").InnerXml;
            //  Validate AddressDeliveryForename to max 70
            if (ord.AddressDeliveryForename.Length > 70)
            {
                string fname = ord.AddressDeliveryForename.Substring(0, 70);
                ord.AddressDeliveryForename = fname;
            }
            ord.AddressDeliverySurname = delContact.SelectSingleNode("surname").InnerXml;
            //  Validate AddressDeliverySurename to max 70
            if (ord.AddressDeliverySurname.Length > 70)
            {
                string sname = ord.AddressDeliverySurname.Substring(0, 70);
                ord.AddressDeliverySurname = sname;
            }
            ord.AddressDeliveryTelephone = delContact.SelectSingleNode("telephone").InnerXml;
            //  Validate telephone to max 30
            if (ord.AddressDeliveryTelephone.Length > 30)
            {
                string phone = ord.AddressDeliveryTelephone.Substring(0, 30);
                ord.AddressDeliveryTelephone = phone;
            }
            //  Line section within Address section (Delivery type)
            XmlNodeList xnLine = doc.DocumentElement.SelectNodes("/order/address[@type='delivery']/line");
            foreach (XmlNode xi in xnLine)
            {
                int index = dvs.AttributeValidation_Int(xi, "index");
                switch (index)
                {
                    case 1:
                        ord.AddressDeliveryLine1 = xi.InnerText;            //  300
                        if (ord.AddressDeliveryLine1.Length > 300)
                        {
                            string txt = ord.AddressDeliveryLine1.Substring(0, 300);
                            ord.AddressDeliveryLine1 = txt;
                        }
                        break;
                    case 2:
                        ord.AddressDeliveryLine2 = xi.InnerText;            //  300
                        if (ord.AddressDeliveryLine2.Length > 300)
                        {
                            string txt = ord.AddressDeliveryLine2.Substring(0, 300);
                            ord.AddressDeliveryLine2 = txt;
                        }
                        break;
                    default:
                        break;
                }
            }
            /// ----------------------------------------------------------------------    
            //  Cart Section
            //  ----------------------------------------------------------------------
            XmlNode cart = doc.DocumentElement.SelectSingleNode("/order/cart");
            ord.CartPrice = dvs.AttributeValidation_Dec(cart, "price");
            /// ------------------------------------------------------------------
            //  Item Section - (n) Items Within Cart
            //  ------------------------------------------------------------------
            XmlNodeList item = doc.DocumentElement.SelectNodes("/order/cart/item");
            /// Order items work area array
            List<StkOrderItem> nItems = new List<StkOrderItem>();
            ///          
            foreach (XmlNode itm in item)
            {
                StkOrderItem xItem = new StkOrderItem();
                xItem.ItemProof         = dvs.AttributeValidation_String(itm, "proof", 100);
                xItem.ItemSku           = dvs.AttributeValidation_String(itm, "sku", 30);
                xItem.ItemWhiteBehind   = dvs.AttributeValidation_String(itm, "whitebehind", 30);   //  for clear stickers, whether the vinyl is pre-printed with a white underlay
                xItem.ItemVsize         = dvs.AttributeValidation_Dec(itm, "vsize");                //  vertical size in mm
                xItem.ItemStyleNum      = dvs.AttributeValidation_String(itm, "stylenum", 30);
                xItem.ItemSimilar       = dvs.AttributeValidation_String(itm, "similar", 30);       //
                xItem.ItemShape         = dvs.AttributeValidation_String(itm, "shape", 30);         //  the specified shape
                xItem.ItemReversePrint  = dvs.AttributeValidation_String(itm, "reverseprint", 30);  //  for window stickers, whether the print is on the inside or outside of the sticker 
                xItem.ItemQuantity      = dvs.AttributeValidation_Int(itm, "quantity");             //
                xItem.ItemPrintedWhite  = dvs.AttributeValidation_String(itm, "printedwhite", 30);  //  for window stickers, whether the vinyl has a printed white background
                xItem.ItemPpu           = dvs.AttributeValidation_String(itm, "ppu", 20);
                xItem.ItemNumHoles      = dvs.AttributeValidation_Int(itm, "numholes");             //  number of holes inside a custom-specified shape
                xItem.ItemMaterialOption = dvs.AttributeValidation_String(itm, "materialoption", 30);  //  not sure (unused J&A internal field?)     
                xItem.ItemMaterial      = dvs.AttributeValidation_String(itm, "material", 30);      //  vinyl or paper, depending on range
                xItem.ItemLines         = dvs.AttributeValidation_Int(itm, "lines");                //  
                xItem.ItemId            = dvs.AttributeValidation_Int(itm, "itemid");
                xItem.ItemIdentical     = dvs.AttributeValidation_String(itm, "identical", 30);
                xItem.ItemHsize         = dvs.AttributeValidation_Dec(itm, "hsize");                //  horizontal size in mm     
                xItem.ItemDesignId      = dvs.AttributeValidation_String(itm, "designid", 30);
                xItem.ItemCornerRad     = dvs.AttributeValidation_String(itm, "cornerrad", 30);     //  corner radius in mm
                xItem.ItemColour        = dvs.AttributeValidation_String(itm, "colour", 30);
                xItem.ItemCode          = dvs.AttributeValidation_String(itm, "code", 10);
                xItem.ItemSheets        = dvs.AttributeValidation_Int(itm, "sheets");               //  Added on Jun 26   
                xItem.ItemsPerSheet     = dvs.AttributeValidation_Int(itm, "persheet");             //  Added on Jun 29   
                /// --------------------------------------------------------------
                //  Names Section - (n) names within Item
                //  --------------------------------------------------------------
                XmlNodeList name = doc.DocumentElement.SelectNodes("/order/cart/item/name");

                //List<ItemNames> lNames = new List<ItemNames>();
                //foreach (XmlNode nme in name)
                //{
                //    /// ----------------------------------------------------------
                //    //  Line Section - (Maximun 7 lines) within name
                //    /// ----------------------------------------------------------
                //    //XmlNodeList line = doc.DocumentElement.SelectNodes("/order/cart/item/name/line");
                //    ItemNames wrkName = new ItemNames();
                //    foreach (XmlNode xn in nme)
                //    {
                //        int index = AttributeValidation_Int(xn, "index");
                //        switch (index)
                //        {
                //            case 1:
                //                wrkName.ItemName1 = FirstChildValidation_Inner(xn);
                //                break;
                //            case 2:
                //                wrkName.ItemName2 = FirstChildValidation_Inner(xn);
                //                break;
                //            case 3:
                //                wrkName.ItemName3 = FirstChildValidation_Inner(xn);
                //                break;
                //            case 4:
                //                wrkName.ItemName4 = FirstChildValidation_Inner(xn);
                //                break;
                //            case 5:
                //                wrkName.ItemName5 = FirstChildValidation_Inner(xn);
                //                break;
                //            case 6:
                //                wrkName.ItemName6 = FirstChildValidation_Inner(xn);
                //                break;
                //            case 7:
                //                wrkName.ItemName7 = FirstChildValidation_Inner(xn);
                //                break;
                //            default:
                //                break;
                //        }
                //    }
                //    /// Add max. lines (index texts) by name to Names List
                //    lNames.Add(wrkName);
                //}
                ///// Save Names List in Item row
                //xItem.names = lNames;
                /// Add Item row to Items List
                nItems.Add(xItem);
            }
            /// ----------------------------------------------------------------------
            /// Discount Section
            /// ----------------------------------------------------------------------
            XmlNode discount    = doc.DocumentElement.SelectSingleNode("/order/discount");
            ord.DiscountType    = dvs.AttributeValidation_String(discount, "type", 20);
            ord.DiscountCode    = dvs.AttributeValidation_String(discount, "code", 20);
            ord.DiscountTotal   = dvs.AttributeValidation_Dec(discount, "total");
            ord.DiscountValue   = dvs.AttributeValidation_Dec(discount, "value");
            /// ----------------------------------------------------------------------
            //  Delivery section
            //  ----------------------------------------------------------------------
            XmlNode xDelivery = doc.DocumentElement.SelectSingleNode("/order/delivery");
            ord.DeliveryTotal   = dvs.AttributeValidation_Dec(xDelivery, "total");
            ord.DeliveryType    = dvs.AttributeValidation_String(xDelivery, "type", 10);
            ord.DeliveryTerritory = dvs.AttributeValidation_String(xDelivery, "territory", 5);
            ord.DeliveryId      = dvs.AttributeValidation_Int(xDelivery, "deliveryid");
            ord.DeliveryEta     = dvs.AttributeValidation_Date(xDelivery, "etd");
            ord.DeliveryEtd     = dvs.AttributeValidation_Date(xDelivery, "eta");
            /// ----------------------------------------------------------------------
            //  Vat Section
            //  ----------------------------------------------------------------------
            XmlNode vat = doc.DocumentElement.SelectSingleNode("/order/vat");
            ord.VatTotal        = dvs.AttributeValidation_Dec(vat, "total");
            ord.VatNumber       = dvs.AttributeValidation_Int(vat, "number");
            ord.VatCountyCode   = dvs.AttributeValidation_String(vat, "countycode", 5);
            ord.VatDistrictRate = dvs.AttributeValidation_Dec(vat, "districtrate");
            ord.VatCityRate     = dvs.AttributeValidation_Dec(vat, "cityrate");
            ord.VatCountyRate   = dvs.AttributeValidation_Dec(vat, "countyrate");
            ord.VatStateRate    = dvs.AttributeValidation_Dec(vat, "staterate");
            ord.VatState        = dvs.AttributeValidation_String(vat, "state", 2);
            ord.VatCounty       = dvs.AttributeValidation_String(vat, "county", 150);
            ord.VatCity         = dvs.AttributeValidation_String(vat, "city", 150);
            ord.VatStaticRate   = dvs.AttributeValidation_Dec(vat, "staticrate");
            ord.VatLiveRate     = dvs.AttributeValidation_Dec(vat, "liverate");
            ord.VatRate         = dvs.AttributeValidation_Dec(vat, "rate");
            /// ----------------------------------------------------------------------
            //  Options Section
            //  ----------------------------------------------------------------------
            XmlNode options = doc.DocumentElement.SelectSingleNode("/order/options");
            ord.OptionsBlindPacking = dvs.AttributeValidation_String(options, "blindpacking", 15);
            ord.OptionsCustRef = dvs.AttributeValidation_String(options, "custref", 50);
            /// ----------------------------------------------------------------------
            //  Payment Section
            //  ----------------------------------------------------------------------
            XmlNode payment = doc.DocumentElement.SelectSingleNode("/order/payment");
            ord.PaymentUsing = dvs.AttributeValidation_String(payment, "using", 50);
            //  net Section
            XmlNode net = doc.DocumentElement.SelectSingleNode("/order/payment/net");
            ord.PaymentCurrency = dvs.AttributeValidation_String(net, "currency", 5);
            string amt = dvs.FirstChildValidation_Inner(net);
            ord.PaymentAmount = Decimal.Parse(amt);
            //  gross Section
            XmlNode gross = doc.DocumentElement.SelectSingleNode("/order/payment/gross");
            ord.GrossCurrency = gross.Attributes["currency"].Value;
            string grossAmt = dvs.FirstChildValidation_Inner(gross);
            ord.GrossAmount = Decimal.Parse(grossAmt);
            /// ----------------------------------------------------------------------
            /// Serialize object (WebOrder) to Json string
            /// Save Items List in Order Items List (include items / names within / name lines within)
            summary.stkOrderSummary = ord;
            summary.stkOrderItems = nItems;
            //
            ord.Filename = fName + ".xml";
            string test = Newtonsoft.Json.JsonConvert.SerializeObject(summary);
            //  =========== >
            //DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();
            /// ******************************************************************************
            /// Call Web Service - "WebOrder" 
            /// <param name="json2">Include Header serialized + items seialized info.</param> 
            /// <param name="fname">File to process name </param>
            /// <param name="type">File Type, determine what service to use (url)</param>
            /// <returns>"rStatus" true if WebService was processed, else false</returns>
            /// Web service url is defined in the App.Config file
            /// ------------------------------------------------------------------------------
            rStatus = wsm.ConsumeWebService(test, fName, syncRow.WebServiceOrders, syncRow);
            //  < ===========
            return rStatus.IsOk;
        }

        /// *************************************************************************************************
        /// <summary>
        ///     Version 3.0 Feb20-17 
        ///     Attribute retreival and validation, any null value returns as " ".
        /// </summary>
        /// <param name="nod">XmlNode where the attibute is located</param>
        /// <param name="attName">Attribute name</param>
        /// <returns>String, Int, DateTime, Decimal depending on the validation type</returns>
        /// -------------------------------------------------------------------------------------------------       
        //private static string AttributeValidation_String(XmlNode nod, string attName)
        //{
        //    string ret = string.Empty;
        //    try
        //    {
        //        ret = nod.Attributes[attName].Value;
        //    }
        //    catch
        //    {
        //        ret = "";
        //    }
        //    return ret;

        //    //return nod.Attributes[attName].Value ?? " ";
        //}

        //private static int AttributeValidation_Int(XmlNode nod, string attName)
        //{
        //    int ret = 0;
        //    try
        //    {
        //        string val = nod.Attributes[attName].Value;
        //        ret = Int32.Parse(val);
        //    }
        //    catch
        //    {
        //        ret = 0;
        //    }
        //    return ret;
        //}

        //private static DateTime AttributeValidation_Date(XmlNode nod, string attName)
        //{
        //    DateTime ret = new DateTime();
        //    if (nod.Attributes[attName] != null)
        //    {
        //        try
        //        {
        //            string dat = AttributeValidation_String(nod, attName);
        //            ret = Convert.ToDateTime(dat);
        //        }
        //        catch
        //        { }
        //    }
        //    return ret;
        //}

        //private static Decimal AttributeValidation_Dec(XmlNode nod, string attName)
        //{
        //    Decimal ret = 0;
        //    try
        //    {
        //        string val = nod.Attributes[attName].Value;
        //        ret = Decimal.Parse(val);
        //    }
        //    catch
        //    {
        //        ret = 0;
        //    }
        //    return ret;
        //}

        //private static string SingleNodeValidation_Text(XmlNode nod, string attName)
        //{
        //    string ret = " ";
        //    try
        //    {
        //        ret = nod.SelectSingleNode(attName).InnerText;
        //    }
        //    catch
        //    { }
        //    return ret;
        //}

        //private static int SingleNodeValidation_Int(XmlNode nod, string attName)
        //{
        //    int ret = 0;
        //    try
        //    {
        //        ret = Int32.Parse(nod.SelectSingleNode(attName).InnerText);
        //    }
        //    catch
        //    { }
        //    return ret;
        //}

        //private static string FirstChildValidation_Inner(XmlNode nod)
        //{
        //    string iText = " ";
        //    try
        //    {
        //        iText = nod.FirstChild.InnerText;
        //    }
        //    catch
        //    { }
        //    return iText;
        //}
    }
}