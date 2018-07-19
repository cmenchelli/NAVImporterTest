using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ImportProcedure_NBI.XmlFiles
{
    public interface IXmlFiles
    {
        bool ProcessWebsiteAccountFiles(XmlDocument doc, string fName, EtlTimer syncRow);
        bool ProcessWebsiteDesignFiles(XmlDocument doc, string fName, EtlTimer syncRow);
        bool ProcessWebsiteOrderFiles(XmlDocument doc, string fName, EtlTimer syncRow);
    }

    public class ImportProcess : IXmlFiles
    {
        readonly DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();
        readonly DAL.DataValidationServices.DataValidation dat = new DAL.DataValidationServices.DataValidation();

        /// *************************************************************************************************
        /// <summary>
        ///     09/22/2015 16:39 version        
        ///     Website-Account Xml document is mapped to Object, then passed to WebServices as a JSON string.
        /// </summary>
        /// <param name="doc">Imported file content read as an XmlDocument</param>
        /// <param name="fName">File name (excluding path section)</param>
        /// <returns>Process status true if procees was completed ok, else false</returns>
        /// -------------------------------------------------------------------------------------------------
        bool IXmlFiles.ProcessWebsiteAccountFiles(XmlDocument doc, string fName, EtlTimer syncRow)
        {
            ServiceResponse rStatus = new ServiceResponse();
            //WebServiceManager wsm = new WebServiceManager();
            ///  ----------------------------------------------------------------------
            //  Account section
            ///  ----------------------------------------------------------------------            ///  
            XmlNode account = doc.DocumentElement.SelectSingleNode("/account");
            WebAccount ord = new WebAccount();
            ord.OrderType = "nbi";
            ord.AccountId = dat.AttributeValidation_String(account, "accountid", 20);                   //  20
            ///  ----------------------------------------------------------------------
            //  Locale Section
            ///  ----------------------------------------------------------------------
            XmlNode locale  = doc.DocumentElement.SelectSingleNode("/account/locale");
            ord.Language    = dat.AttributeValidation_String(locale, "lang", 5);                        //  5
            ord.SiteUsed    = dat.AttributeValidation_String(locale, "siteused", 10);                   //  10
            ord.Currency    = dat.AttributeValidation_String(locale, "currency", 10);                   //  10
            ///  ----------------------------------------------------------------------
            //  Contact (account) section 
            ///  ----------------------------------------------------------------------
            XmlNode contact = doc.DocumentElement.SelectSingleNode("/account/contact");
            ord.Email       = dat.SingleNodeValidation_Text(contact, "email", 60);                      //  60
            ord.Title       = dat.SingleNodeValidation_Text(contact, "title", 20);                      //  20     
            ord.Forename    = dat.SingleNodeValidation_Text(contact, "forename", 70);                   //  70
            ord.Surname     = dat.SingleNodeValidation_Text(contact, "surname", 70);                    //  70
            ord.CompanyName = dat.SingleNodeValidation_Text(contact, "company", 300);                   //  300
            ord.Industry    = dat.SingleNodeValidation_Text(contact, "industry", 200);                  //  200
            ord.Telephone   = dat.SingleNodeValidation_Text(contact, "telephone", 30);                  //  30
            ///  ----------------------------------------------------------------------
            //  Address Section Process - (Cardholder)
            ///  ----------------------------------------------------------------------
            XmlNode cardHolder = doc.DocumentElement.SelectSingleNode("/account/address[@type='cardholder']");
            ord.AddressCardHolderTown       = dat.SingleNodeValidation_Text(cardHolder, "town", 150);    //  150
            ord.AddressCardHolderCounty     = dat.SingleNodeValidation_Text(cardHolder, "county", 150);  //  150
            ord.AddressCardHolderPostCode   = dat.SingleNodeValidation_Text(cardHolder, "postcode", 30); //  30
            ord.AddressCardHolderCountry    = dat.SingleNodeValidation_Text(cardHolder, "country", 200); //  200
            //  Line section within Cardholder address section
            XmlNodeList xLt = cardHolder.SelectNodes("/account/address[@type='cardholder']/line");
            foreach (XmlNode xc in xLt)
            {
                int index = dat.AttributeValidation_Int(xc, "index");
                switch (index)
                {
                    case 1:
                        ord.AddressCardHolderLine1 = xc.InnerText;          //  300
                        if (ord.AddressCardHolderLine1.Length > 300)
                        {
                            string txt = ord.AddressCardHolderLine1.Substring(0, 300);
                            ord.AddressCardHolderLine1 = txt;
                        }
                        break;
                    case 2:
                        ord.AddressCardHolderLine2 = xc.InnerText;          //  300
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
            ///  ----------------------------------------------------------------------
            //  Address Section - (Delivery)
            ///  ----------------------------------------------------------------------
            XmlNodeList delivery = doc.DocumentElement.SelectNodes("/account/address[@type='delivery']");
            foreach (XmlNode del in delivery)
            {
                /// Contact information within delivery address
                XmlNode delLine = del.SelectSingleNode("/account/address[@type='delivery']/contact");
                ord.AddressDeliveryForename = dat.SingleNodeValidation_Text(delLine, "forename", 70);       //  70
                ord.AddressDeliverySurname = dat.SingleNodeValidation_Text(delLine, "surname", 70);         //  70
                ord.AddressDeliveryTelephone = dat.SingleNodeValidation_Text(delLine, "telephone", 30);     //  70
                ///
                ord.AddressDeliveryCompany = dat.SingleNodeValidation_Text(del, "company", 300);            //  300
                /// Line section within Delivery address section
                XmlNodeList xnLine = del.SelectNodes("/account/address[@type='delivery']/line");
                foreach (XmlNode xd in xnLine)
                {
                    int index = dat.AttributeValidation_Int(xd, "index");
                    switch (index)
                    {
                        case 1:
                            ord.AddressDeliveryLine1 = xd.InnerText;            //  300
                            if (ord.AddressDeliveryLine1.Length > 300)
                            {
                                string txt = ord.AddressDeliveryLine1.Substring(0, 300);
                                ord.AddressDeliveryLine1 = txt;
                            }
                            break;
                        case 2:
                            ord.AddressDeliveryLine2 = xd.InnerText;            //  300
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
                ord.AddressDeliveryTown = dat.SingleNodeValidation_Text(del, "town", 150);                  //  150
                ord.AddressDeliveryCounty = dat.SingleNodeValidation_Text(del, "county", 150);              //  150
                ord.AddressDeliveryPostCode = dat.SingleNodeValidation_Text(del, "postcode", 30);           //  30
                ord.AddressDeliveryCountry = dat.SingleNodeValidation_Text(del, "country", 200);            //  200
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
        ///     09/22/2015 16:39 version      
        ///     Website-Design Xml File is mapped to Object, then passed to WebServices as a JSON string.
        /// </summary>
        /// <param name="doc">Imported file content read as an XmlDocument</param>
        /// <param name="fName">File name (excluding path section)</param>
        /// <returns>Process status true if procees was completed ok, else false</returns>
        /// -------------------------------------------------------------------------------------------------
        bool IXmlFiles.ProcessWebsiteDesignFiles(XmlDocument doc, string fName, EtlTimer syncRow)
        {
            //WebServiceManager wsm = new WebServiceManager();
            DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();
            WebDesign des = new WebDesign();
            ServiceResponse rStatus = new ServiceResponse();
            /// ----------------------------------------------------------------------
            //  Design section
            /// ----------------------------------------------------------------------
            des.OrderType = "nbi";
            XmlNode design  = doc.DocumentElement.SelectSingleNode("/design");
            des.StyleNum    = dat.AttributeValidation_String(design, "stylenum", 0);
            des.DesignID    = dat.AttributeValidation_String(design, "designid", 0);
            des.TimeStamp   = dat.AttributeValidation_Date(design, "timestamp");
            /// ----------------------------------------------------------------------
            //  Specs Section
            /// ----------------------------------------------------------------------
            XmlNode specs = doc.DocumentElement.SelectSingleNode("/design/specs");
            des.Proof   = dat.AttributeValidation_String(specs, "proof", 0);
            des.lines   = dat.AttributeValidation_String(specs, "lines", 0);
            des.Fitting = dat.AttributeValidation_String(specs, "fitting", 0);
            des.Bar     = dat.AttributeValidation_String(specs, "bar", 0);
            des.Vinyl   = dat.AttributeValidation_String(specs, "vinyl", 0);
            des.Code    = dat.AttributeValidation_String(specs, "code", 0);
            /// ----------------------------------------------------------------------
            //  Background Section
            /// ----------------------------------------------------------------------
            XmlNode background = doc.DocumentElement.SelectSingleNode("/design/background");
            des.Src = dat.AttributeValidation_String(background, "src", 0);
            /// ------dat.----------------------------------------------------------------
            //  Contacdat.t (account) section process
            /// ----------------------------------------------------------------------
            XmlNodeList frag = doc.DocumentElement.SelectNodes("/design/fragments/logo");
            WebDesignFragments fragments = new WebDesignFragments();
            foreach (XmlNode logo in frag)
            {
                fragments.Src   = dat.AttributeValidation_String(logo, "src", 0);
                fragments.Top   = dat.AttributeValidation_String(logo, "top", 0);
                fragments.Left  = dat.AttributeValidation_String(logo, "left", 0);
                fragments.Height= dat.AttributeValidation_String(logo, "height", 0);
                fragments.Width = dat.AttributeValidation_String(logo, "width", 0);
                fragments.Index = dat.AttributeValidation_Int(logo, "index");
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
                    cnt1.Index  = dat.AttributeValidation_Int(tNode, "index");
                    cnt1.Color  = dat.AttributeValidation_String(tNode, "color", 0);
                    cnt1.Size   = dat.AttributeValidation_String(tNode, "size", 0);
                    cnt1.Style  = dat.AttributeValidation_String(tNode, "style", 0);
                    cnt1.Font   = dat.AttributeValidation_String(tNode, "font", 0);
                    cnt1.Y      = dat.AttributeValidation_String(tNode, "y", 0);
                    cnt1.X      = dat.AttributeValidation_String(tNode, "x", 0);
                    cnt1.Content= dat.AttributeValidation_String(tNode, "content", 0);
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
            return rStatus.IsOk;
        }

        /// *************************************************************************************************
        /// <summary>
        ///     10/29/2015 08:59 version   
        ///     Website-Account Xml File is mapped to Object, then passed to WebServices as a JSON string.
        /// </summary>
        /// <param name="doc">Imported file content read as an XmlDocument</param>
        /// <param name="fName">File name (excluding path section)</param>
        /// <returns>Process status true if procees was completed ok, else false</returns>
        /// -------------------------------------------------------------------------------------------------
        bool IXmlFiles.ProcessWebsiteOrderFiles(XmlDocument doc, string fName, EtlTimer syncRow)
        {
            ServiceResponse rStatus = new ServiceResponse();
            WebOrder summary = new WebOrder();
            NBIHeader ord = new NBIHeader();
            /// ---------------------------------------------------------------------
            //  Order section
            /// ---------------------------------------------------------------------
            ord.OrderType = "nbi";
            XmlNode order   = doc.DocumentElement.SelectSingleNode("/order");
            ord.OrderId     = dat.AttributeValidation_String(order, "orderid", 15);             //  int 15
            ord.OrderNumber = dat.AttributeValidation_String(order, "ordernum", 15);            //  int 15
            /// ---------------------------------------------------------------------
            //  Account section
            /// ---------------------------------------------------------------------
            XmlNode account = doc.DocumentElement.SelectSingleNode("/order/account");
            ord.AccountId   = dat.AttributeValidation_String(account, "accountid", 15);         //  int
            ord.AccountNum  = dat.AttributeValidation_String(account, "accountnum", 15);        //  int
            /// ---------------------------------------------------------------------
            //  Locale Section
            /// ---------------------------------------------------------------------
            XmlNode locale  = doc.DocumentElement.SelectSingleNode("/order/locale");
            ord.Language    = dat.AttributeValidation_String(locale, "lang", 5);                //  5
            ord.SiteUsed    = dat.AttributeValidation_String(locale, "siteused", 10);           //  10
            ord.Currency    = dat.AttributeValidation_String(locale, "currency", 10);           //  10
            /// ---------------------------------------------------------------------
            //  profile section
            /// ---------------------------------------------------------------------
            XmlNode profile = doc.DocumentElement.SelectSingleNode("/order/profile");
            ord.VatNum      = dat.AttributeValidation_String(profile, "vatnum", 15);            //  int
            ord.Credit      = dat.AttributeValidation_String(profile, "credit", 20);            //  dec  
            /// ---------------------------------------------------------------------
            //  Contact (Order) section
            /// ---------------------------------------------------------------------
            XmlNode contact = doc.DocumentElement.SelectSingleNode("/order/contact");
            ord.Email       = dat.SingleNodeValidation_Text(contact, "email", 60);              //  60
            ord.Title       = dat.SingleNodeValidation_Text(contact, "title", 20);              //  20
            ord.Forename    = dat.SingleNodeValidation_Text(contact, "forename", 70);           //  70
            ord.Surname     = dat.SingleNodeValidation_Text(contact, "surname", 70);            //  70
            ord.CompanyName = dat.SingleNodeValidation_Text(contact, "company", 300);           //  300
            ord.Industry    = dat.SingleNodeValidation_Text(contact, "industry", 200);          //  200
            ord.Source      = dat.SingleNodeValidation_Text(contact, "source", 50);             //  50
            ord.Telephone   = dat.SingleNodeValidation_Text(contact, "telephone", 30);          //  30
            /// ---------------------------------------------------------------------
            //  Address Section - (Cardholder)
            /// ---------------------------------------------------------------------
            XmlNode cardHolder = doc.DocumentElement.SelectSingleNode("/order/address[@type='cardholder']");
            //  ------------------------------------------------------------------
            ord.AddressCardHolderTown       = dat.SingleNodeValidation_Text(cardHolder, "town", 150);           //  150
            ord.AddressCardHolderCounty     = dat.SingleNodeValidation_Text(cardHolder, "county", 150);         //  150
            ord.AddressCardHolderPostCode   = dat.SingleNodeValidation_Text(cardHolder, "postcode", 30);        //  30
            ord.AddressCardHolderCountry    = dat.SingleNodeValidation_Text(cardHolder, "country", 200);        //  200
            /// Cardholder Line section within 
            XmlNodeList xLt = doc.DocumentElement.SelectNodes("/order/address[@type='cardholder']/line");
            foreach (XmlNode xn in xLt)
            {
                int index = dat.AttributeValidation_Int(xn, "index");
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
            ord.AddressDeliveryCompany  = dat.SingleNodeValidation_Text(delivery, "company", 300);  //  300
            ord.AddressDeliveryTown     = dat.SingleNodeValidation_Text(delivery, "town", 150);     //  150
            ord.AddressDeliveryCounty   = dat.SingleNodeValidation_Text(delivery, "county", 150);   //  150
            ord.AddressDeliveryPostCode = dat.SingleNodeValidation_Text(delivery, "postcode", 30);  //  30
            ord.AddressDeliveryCountry  = dat.SingleNodeValidation_Text(delivery, "country", 200);  //  200
            //  Contact Section within Address section (Delivery type)      
            XmlNode delContact          = doc.DocumentElement.SelectSingleNode("/order/address[@type='delivery']/contact");
            ord.AddressDeliveryForename = delContact.SelectSingleNode("forename").InnerXml;     //  70
            //  Validate AddressDeliveryForename to max 70
            if (ord.AddressDeliveryForename.Length > 70)
            {
                string fname = ord.AddressDeliveryForename.Substring(0, 70);
                ord.AddressDeliveryForename = fname;
            }
            ord.AddressDeliverySurname  = delContact.SelectSingleNode("surname").InnerXml;      //  70
            //  Validate AddressDeliverySurename to max 70
            if (ord.AddressDeliverySurname.Length > 70)
            {
                string sname = ord.AddressDeliverySurname.Substring(0, 70);
                ord.AddressDeliverySurname = sname;
            }
            ord.AddressDeliveryTelephone = delContact.SelectSingleNode("telephone").InnerXml;   //  30
            //  Validate Phone number max length is 30
            if (ord.AddressDeliveryTelephone.Length > 30)
            {
                string phone = ord.AddressDeliveryTelephone.Substring(0, 30);
                ord.AddressDeliveryTelephone = phone;
            }
            //  Line section within Address section (Delivery type)
            XmlNodeList xnLine = doc.DocumentElement.SelectNodes("/order/address[@type='delivery']/line");
            foreach (XmlNode xi in xnLine)
            {
                int index = dat.AttributeValidation_Int(xi, "index");
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
            ord.CartPrice = dat.AttributeValidation_Dec(cart, "price");
            /// ------------------------------------------------------------------
            //  Item Section - (n) Items Within Cart
            //  ------------------------------------------------------------------
            XmlNodeList item = doc.DocumentElement.SelectNodes("/order/cart/item");
            /// Order items work area array
            List<WebOrderItem> nItems = new List<WebOrderItem>();
            ///
            foreach (XmlNode itm in item)
            {
                WebOrderItem xItem = new WebOrderItem();
                xItem.ItemProof     = dat.AttributeValidation_String(itm, "proof", 100);        //  100
                xItem.ItemSku       = dat.AttributeValidation_String(itm, "sku", 30);           //  30
                xItem.ItemIdentical = dat.AttributeValidation_String(itm, "identical", 30);     //  30
                xItem.ItemSimilar   = dat.AttributeValidation_String(itm, "similar", 30);       //  30
                xItem.ItemStyleNum  = dat.AttributeValidation_String(itm, "stylenum", 30);      //  30
                xItem.ItemId        = dat.AttributeValidation_Int(itm, "itemid");
                xItem.ItemDesignId  = dat.AttributeValidation_String(itm, "designid", 30);      //  30
                xItem.ItemPpu       = dat.AttributeValidation_String(itm, "ppu", 20);           //  20
                xItem.ItemQuantity  = dat.AttributeValidation_Int(itm, "quantity");
                xItem.ItemLines     = dat.AttributeValidation_Int(itm, "lines");
                xItem.ItemFitting   = dat.AttributeValidation_String(itm, "fitting", 50);       //  50
                xItem.ItemBar       = dat.AttributeValidation_String(itm, "bar", 50);           //  50
                xItem.ItemVinyl     = dat.AttributeValidation_String(itm, "vinyl", 50);         //  50
                xItem.ItemCode      = dat.AttributeValidation_String(itm, "code", 10);          //  10
                /// --------------------------------------------------------------
                //  Names Section - (n) names within Item
                //  --------------------------------------------------------------
                List<ItemNames> lNames = new List<ItemNames>();
                int qty = xItem.ItemQuantity;       //  Set the names search limit
                foreach (XmlNode nme in itm)
                {
                    /// ----------------------------------------------------------
                    //  Line Section - (Maximun 7 lines) within name
                    /// ----------------------------------------------------------
                    //XmlNodeList line = doc.DocumentElement.SelectNodes("/order/cart/item/name/line");
                    ItemNames wrkName = new ItemNames();                    
                    foreach (XmlNode xn in nme)
                    {
                        int index = dat.AttributeValidation_Int(xn, "index");
                        switch (index)
                        {
                            case 1:
                                wrkName.ItemName1 = dat.FirstChildValidation_TextValue(xn);
                                if (wrkName.ItemName1.Length > 100)
                                {
                                    string txt = wrkName.ItemName1.Substring(0, 300);
                                    wrkName.ItemName1 = txt;
                                }
                                break;
                            case 2:
                                wrkName.ItemName2 = dat.FirstChildValidation_TextValue(xn);
                                if (wrkName.ItemName2.Length > 100)
                                {
                                    string txt = wrkName.ItemName2.Substring(0, 300);
                                    wrkName.ItemName2 = txt;
                                }
                                break;
                            case 3:
                                wrkName.ItemName3 = dat.FirstChildValidation_TextValue(xn);
                                if (wrkName.ItemName3.Length > 100)
                                {
                                    string txt = wrkName.ItemName3.Substring(0, 300);
                                    wrkName.ItemName3 = txt;
                                }
                                break;
                            case 4:
                                wrkName.ItemName4 = dat.FirstChildValidation_TextValue(xn);
                                if (wrkName.ItemName4.Length > 100)
                                {
                                    string txt = wrkName.ItemName4.Substring(0, 300);
                                    wrkName.ItemName4 = txt;
                                }
                                break;
                            case 5:
                                wrkName.ItemName5 = dat.FirstChildValidation_TextValue(xn);
                                if (wrkName.ItemName5.Length > 100)
                                {
                                    string txt = wrkName.ItemName5.Substring(0, 300);
                                    wrkName.ItemName5 = txt;
                                }
                                break;
                            case 6:
                                wrkName.ItemName6 = dat.FirstChildValidation_TextValue(xn);
                                if (wrkName.ItemName6.Length > 100)
                                {
                                    string txt = wrkName.ItemName6.Substring(0, 300);
                                    wrkName.ItemName6 = txt;
                                }
                                break;
                            case 7:
                                wrkName.ItemName7 = dat.FirstChildValidation_TextValue(xn);
                                if (wrkName.ItemName7.Length > 100)
                                {
                                    string txt = wrkName.ItemName7.Substring(0, 300);
                                    wrkName.ItemName7 = txt;
                                }
                                break;
                            default:
                                break;
                        }
                    }                    
                    /// Add max. lines (index texts) by name to Names List
                    lNames.Add(wrkName);
                    qty = qty - 1;
                    if (qty == 0)
                    {
                        break;
                    }
                }
                /// Save Names List in Item row
                xItem.names = lNames;
                /// Add Item row to Items List
                nItems.Add(xItem);
            }            
            /// ----------------------------------------------------------------------
            /// Discount Section
            /// ----------------------------------------------------------------------
            XmlNode discount    = doc.DocumentElement.SelectSingleNode("/order/discount");
            ord.DiscountType    = dat.AttributeValidation_String(discount, "type", 20);                //  20
            ord.DiscountCode    = dat.AttributeValidation_String(discount, "code", 20);                //  20
            ord.DiscountTotal   = dat.AttributeValidation_Dec(discount, "total");
            ord.DiscountValue   = dat.AttributeValidation_Dec(discount, "value");
            /// ----------------------------------------------------------------------
            //  Delivery section
            //  ----------------------------------------------------------------------
            XmlNode xDelivery = doc.DocumentElement.SelectSingleNode("/order/delivery");
            ord.DeliveryTotal       = dat.AttributeValidation_Dec(xDelivery, "total");
            ord.DeliveryType        = dat.AttributeValidation_String(xDelivery, "type", 10);        //  10
            ord.DeliveryTerritory   = dat.AttributeValidation_String(xDelivery, "territory", 5);    //  5
            ord.DeliveryId          = dat.AttributeValidation_Int(xDelivery, "deliveryid");         //  int
            ord.DeliveryEta         = dat.AttributeValidation_Date(xDelivery, "etd");
            ord.DeliveryEtd         = dat.AttributeValidation_Date(xDelivery, "eta");
            /// ----------------------------------------------------------------------
            //  Vat Section
            //  ----------------------------------------------------------------------
            XmlNode vat = doc.DocumentElement.SelectSingleNode("/order/vat");
            ord.VatTotal            = dat.AttributeValidation_Dec(vat, "total");                    //  dec  
            ord.VatNumber           = dat.AttributeValidation_Int(vat, "number");                   //  int
            ord.VatCountyCode       = dat.AttributeValidation_String(vat, "countycode", 5);         //  5
            ord.VatDistrictRate     = dat.AttributeValidation_String(vat, "districtrate", 20);      //  20
            ord.VatCityRate         = dat.AttributeValidation_String(vat, "cityrate", 20);          //  20
            ord.VatCountyRate       = dat.AttributeValidation_String(vat, "countyrate", 20);        //  20
            ord.VatStateRate        = dat.AttributeValidation_String(vat, "staterate", 20);         //  20
            ord.VatState            = dat.AttributeValidation_String(vat, "state", 2);              //  2
            ord.VatCounty           = dat.AttributeValidation_String(vat, "county", 150);           //  150
            ord.VatCity             = dat.AttributeValidation_String(vat, "city", 150);             //  150
            ord.VatStaticRate       = dat.AttributeValidation_String(vat, "staticrate", 20);        //  20
            ord.VatLiveRate         = dat.AttributeValidation_String(vat, "liverate", 20);          //  20
            ord.VatRate             = dat.AttributeValidation_String(vat, "rate", 20);              //  20
            /// ----------------------------------------------------------------------
            //  Options Section
            //  ----------------------------------------------------------------------
            XmlNode options = doc.DocumentElement.SelectSingleNode("/order/options");
            ord.OptionsBlindPacking = dat.AttributeValidation_String(options, "blindpacking", 15);  //  int
            ord.OptionsCustRef = dat.AttributeValidation_String(options, "custref", 50);            //  50
            /// ----------------------------------------------------------------------
            //  Payment Section
            //  ----------------------------------------------------------------------
            XmlNode payment         = doc.DocumentElement.SelectSingleNode("/order/payment");
            ord.PaymentUsing        = dat.AttributeValidation_String(payment, "using", 50);         //  50
            ord.PaymentCardType     = dat.SingleNodeValidation_Text(payment, "cardtype", 20);       //  Apr13-17    20
            ord.PaymentLastFour     = dat.SingleNodeValidation_Text(payment, "lastfour", 4);        //  Apr13-17    4
            //  net Section
            XmlNode net = doc.DocumentElement.SelectSingleNode("/order/payment/net");
            ord.PaymentCurrency = dat.AttributeValidation_String(net, "currency", 5);               //  5
            string amt = dat.FirstChildValidation_Inner(net);
            ord.PaymentAmount = Decimal.Parse(amt);                                         
            //  gross Section
            XmlNode gross = doc.DocumentElement.SelectSingleNode("/order/payment/gross");
            ord.GrossCurrency = gross.Attributes["currency"].Value;                             //  5
            string grossAmt = dat.FirstChildValidation_Inner(gross);
            ord.GrossAmount = Decimal.Parse(grossAmt);
            /// ----------------------------------------------------------------------
            /// Serialize object (WebOrder) to Json string
            /// Save Items List in Order Items List (include items / names within / name lines within)
            summary.nbiOrderSummary = ord; 
            summary.nbiOrderItems = nItems;
            //
            ord.Filename = fName + ".xml";
            //
            string test = Newtonsoft.Json.JsonConvert.SerializeObject(summary);
            //  =========== >
            DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();
            /// ******************************************************************************
            /// Call Web Service - "WebOrder" 
            /// <param name="json2">Include Header serialized + items seialized info.</param> 
            /// <param name="fname">File to process name </param>
            /// <param name="type">File Type, determine what service to use (url)</param>
            /// <returns>"rStatus" true if WebService was processed, else false</returns>
            /// Web service url is defined in the App.Config file
            /// ------------------------------------------------------------------------------
            rStatus = wsm.ConsumeWebService(test, fName, syncRow.WebServiceOrders, syncRow);
            //rStatus = true;       // use only for testing purposes
            //  < ===========
            return rStatus.IsOk;
        }

        /// *************************************************************************************************
        /// <summary>
        ///     09/22/2015 16:39 version
        ///     Attribute retreival and validation, any null value returns as " ".
        /// </summary>
        /// <param name="nod">XmlNode where the attibute is located</param>
        /// <param name="attName">Attribute name</param>
        /// <returns>String, Int, DateTime, Decimal depending on the validation type</returns>
        /// -------------------------------------------------------------------------------------------------   
        /// //  Mod Oct12 2017 max.length control
        //private static string AttributeValidation_String(XmlNode nod, string attName, int len)
        //{
        //    string ret = string.Empty;
        //    try
        //    {
        //        //ret = nod.Attributes[attName].Value;
        //        string txt = nod.Attributes[attName].Value;
        //        if (len == 0)
        //            ret = txt;
        //        else
        //        {
        //            if (txt.Length < len)
        //                len = txt.Length;
        //            ret = txt.Substring(0, len);
        //        }
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
        //            string dat = AttributeValidation_String(nod, attName, 0);
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

        ////  Mod Oct12 2017 max.length control
        //private static string SingleNodeValidation_Text(XmlNode nod, string attName, int len)
        //{
        //    string ret = " ";
        //    try
        //    {
        //        string txt = nod.SelectSingleNode(attName).InnerText;
        //        if (txt.Length < len)
        //            len = txt.Length;
        //        ret = txt.Substring(0, len);
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

        //private static string FirstChildValidation_TextValue(XmlNode nod)
        //{
        //    string iText = " ";
        //    try
        //    {
        //        iText = nod.FirstChild.Value;
        //    }
        //    catch
        //    { }
        //    return iText;
        //}
    }
}