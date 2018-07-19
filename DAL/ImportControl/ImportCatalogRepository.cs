using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ImportControl
{
    public class ImportCatalogRepository
    {
        readonly ImportControlRepository ikr = new ImportControlRepository(); 


         public List<string> getCatalogOrder_NotImportedList()
        {
            SqlDataReader dr = null;
            var orderList = new List<string>();
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                //args.Add("@Order", order.ToString());
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("MW_WSPGetWebOrdersList_notImported", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //
                while (dr != null && dr.Read())
                {
                    string orderId = null;
                    orderId = dr["CatWebOrderId"].ToString().Length > 0 ? (dr["CatWebOrderId"].ToString()) : "0";
                    orderList.Add(orderId);
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return orderList;
        }


        /// <summary>
        ///     Retreive the complete order information (header and all depending items) for an specific order.    
        /// </summary>
        /// <param name="order"></param>
        /// <returns>Nis tables class</returns>
        public CatalogOrderTables getCatalogOrder(string order)
        {
            SqlDataReader dr = null;
            CatalogOrderTables ord = new CatalogOrderTables();
            List<CatalogWebOrderItem> list = new List<CatalogWebOrderItem>();
            int items = 0;
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@Order", order);
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("[MW_WSPGetWebOrdersAndItems]", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //
                while (dr != null && dr.Read())
                {
                    CatalogWebOrderHeader head = new CatalogWebOrderHeader();
                    head.CatWebOrderId              = dr["CatWebOrderId"].ToString().Length > 0 ? (dr["CatWebOrderId"].ToString()) : "0";
                    head.OrderGrandTotal            = (Decimal)(dr["OrderGrandTotal"].ToString().Length > 0 ? Decimal.Parse(dr["OrderGrandTotal"].ToString()) : 0);
                    head.ProductTotal               = (Decimal)(dr["ProductTotal"].ToString().Length > 0 ? Decimal.Parse(dr["ProductTotal"].ToString()) : 0);
                    head.ShippingTotal              = (Decimal)(dr["ShippingTotal"].ToString().Length > 0 ? Decimal.Parse(dr["ShippingTotal"].ToString()) : 0);
                    head.TaxTotal                   = (Decimal)(dr["TaxTotal"].ToString().Length > 0 ? Decimal.Parse(dr["TaxTotal"].ToString()) : 0);
                    head.ProductTotalWithShipping   = (Decimal)(dr["ProductTotalWithShipping"].ToString().Length > 0 ? Decimal.Parse(dr["ProductTotalWithShipping"].ToString()) : 0);
                    head.OrderDate                  = dr["OrderDate"].ToString().Length > 0 ? DateTime.Parse(dr["OrderDate"].ToString()) : DateTime.Now;
                    head.Comment                    = dr["Comment"].ToString().Length > 0 ? dr["Comment"].ToString() : "";
                    head.Time                       = (Byte[])(dr["Time"]);
                    head.StTotal                    = (Decimal)(dr["st_total"].ToString().Length > 0 ? Decimal.Parse(dr["st_total"].ToString()) : 0);
                    head.IndexCounter               = (Decimal)(dr["IndexCounter"].ToString().Length > 0 ? Decimal.Parse(dr["IndexCounter"].ToString()) : 0);
                    head.OrderNumber                = (Decimal)(dr["OrderNumber"].ToString().Length > 0 ? Decimal.Parse(dr["OrderNumber"].ToString()) : 0);
                    head.Legal                      = dr["Legal"].ToString().Length > 0 ? dr["Legal"].ToString() : "";
                    head.ShipViaName                = dr["ShipViaName"].ToString().Length > 0 ? dr["ShipViaName"].ToString() : "";
                    head.Data                       = dr["Data"].ToString().Length > 0 ? dr["Data"].ToString() : "";
                    head.Optional1                  = dr["Optional1"].ToString().Length > 0 ? dr["Optional1"].ToString() : "";
                    head.Optional2                  = dr["Optional2"].ToString().Length > 0 ? dr["Optional2"].ToString() : "";
                    head.Optional3                  = dr["Optional3"].ToString().Length > 0 ? dr["Optional3"].ToString() : "";
                    head.ExternalOrderNumber        = dr["ExternalOrderNumber"].ToString().Length > 0 ? dr["ExternalOrderNumber"].ToString() : "";
                    head.PONumber                   = dr["PONumber"].ToString().Length > 0 ? dr["PONumber"].ToString() : "";
                    head.OrderStatus                = dr["OrderStatus"].ToString().Length > 0 ? dr["OrderStatus"].ToString() : "";
                    head.OrderType                  = dr["OrderType"].ToString().Length > 0 ? dr["OrderType"].ToString() : "";
                    head.CouponName                 = dr["CouponName"].ToString().Length > 0 ? dr["CouponName"].ToString() : "";
                    head.CouponAmount               = (Decimal)(dr["CouponAmount"].ToString().Length > 0 ? Decimal.Parse(dr["CouponAmount"].ToString()) : 0); 
                    head.ExportFlag                 = dr["ExportFlag"].ToString().Length > 0 ? dr["ExportFlag"].ToString() : "";
                    head.CountyTax                  = (Decimal)(dr["CountyTax"].ToString().Length > 0 ? Decimal.Parse(dr["CountyTax"].ToString()) : 0);
                    head.CountyName                 = dr["CountyName"].ToString().Length > 0 ? dr["CountyName"].ToString() : "";
                    head.Vol1                       = (Decimal)(dr["vol1"].ToString().Length > 0 ? Decimal.Parse(dr["vol1"].ToString()) : 0);
                    head.Vol2                       = (Decimal)(dr["vol2"].ToString().Length > 0 ? Decimal.Parse(dr["vol2"].ToString()) : 0);
                    head.Vol3                       = (Decimal)(dr["vol3"].ToString().Length > 0 ? Decimal.Parse(dr["vol3"].ToString()) : 0);
                    head.Vol4                       = (Decimal)(dr["vol4"].ToString().Length > 0 ? Decimal.Parse(dr["vol4"].ToString()) : 0);
                    head.Vol5                       = (Decimal)(dr["vol5"].ToString().Length > 0 ? Decimal.Parse(dr["vol5"].ToString()) : 0);
                    head.Vol6                       = (Decimal)(dr["vol6"].ToString().Length > 0 ? Decimal.Parse(dr["vol6"].ToString()) : 0);
                    head.Vol7                       = (Decimal)(dr["vol7"].ToString().Length > 0 ? Decimal.Parse(dr["vol7"].ToString()) : 0);
                    head.Vol8                       = (Decimal)(dr["vol8"].ToString().Length > 0 ? Decimal.Parse(dr["vol8"].ToString()) : 0);
                    head.Vol9                       = (Decimal)(dr["vol9"].ToString().Length > 0 ? Decimal.Parse(dr["vol9"].ToString()) : 0);
                    head.Vol10                      = (Decimal)(dr["vol10"].ToString().Length > 0 ? Decimal.Parse(dr["vol10"].ToString()) : 0);
                    head.Fvb                        = (Decimal)(dr["fvb"].ToString().Length > 0 ? Decimal.Parse(dr["fvb"].ToString()) : 0);                  
                    head.Bvb                        = (Decimal)(dr["bvb"].ToString().Length > 0 ? Decimal.Parse(dr["bvb"].ToString()) : 0);                  
                    head.Type                       =  dr["Type"].ToString().Length > 0 ? dr["Type"].ToString() : "";                 
                    head.CustomerFirstName          =  dr["CustomerFirstName"].ToString().Length > 0 ? dr["CustomerFirstName"].ToString() : "";         
                    head.CustomerLastName           =  dr["CustomerLastName"].ToString().Length > 0 ? dr["CustomerLastName"].ToString() : "";         
                    head.CustomerAddress1           =  dr["CustomerAddress1"].ToString().Length > 0 ? dr["CustomerAddress1"].ToString() : "";         
                    head.CustomerAddress2           =  dr["CustomerAddress2"].ToString().Length > 0 ? dr["CustomerAddress2"].ToString() : "";         
                    head.CustomerCity               =  dr["CustomerCity"].ToString().Length > 0 ? dr["CustomerCity"].ToString() : "";           
                    head.CustomerState              =  dr["CustomerState"].ToString().Length > 0 ? dr["CustomerState"].ToString() : "";     
                    head.CustomerCounty             =  dr["CustomerCounty"].ToString().Length > 0 ? dr["CustomerCounty"].ToString() : "";     
                    head.CustomerZip                =  dr["CustomerZip"].ToString().Length > 0 ? dr["CustomerZip"].ToString() : "";    
                    head.CustomerCountry            =  dr["CustomerCountry"].ToString().Length > 0 ? dr["CustomerCountry"].ToString() : "";    
                    head.CustomerPhone              =  dr["CustomerPhone"].ToString().Length > 0 ? dr["CustomerPhone"].ToString() : "";    
                    head.CustomerFax                =  dr["CustomerFax"].ToString().Length > 0 ? dr["CustomerFax"].ToString() : "";    
                    head.CustomerCompany            =  dr["CustomerCompany"].ToString().Length > 0 ? dr["CustomerCompany"].ToString() : "";    
                    head.CustomerPager              =  dr["CustomerPager"].ToString().Length > 0 ? dr["CustomerPager"].ToString() : "";    
                    head.CustomerCellular           =  dr["CustomerCellular"].ToString().Length > 0 ? dr["CustomerCellular"].ToString() : "";    
                    head.CustomerEmail              =  dr["CustomerEmail"].ToString().Length > 0 ? dr["CustomerEmail"].ToString() : "";    
                    head.CustomerEmailCC            =  dr["CustomerEmailCC"].ToString().Length > 0 ? dr["CustomerEmailCC"].ToString() : "";    
                    head.ShipFirstName              =  dr["ShipFirstName"].ToString().Length > 0 ? dr["ShipFirstName"].ToString() : "";    
                    head.ShipLastName               =  dr["ShipLastName"].ToString().Length > 0 ? dr["ShipLastName"].ToString() : "";    
                    head.ShipAddress1               =  dr["ShipAddress1"].ToString().Length > 0 ? dr["ShipAddress1"].ToString() : "";    
                    head.ShipAddress2               =  dr["ShipAddress2"].ToString().Length > 0 ? dr["ShipAddress2"].ToString() : "";    
                    head.ShipCity                   =  dr["ShipCity"].ToString().Length > 0 ? dr["ShipCity"].ToString() : "";    
                    head.ShipState                  =  dr["ShipState"].ToString().Length > 0 ? dr["ShipState"].ToString() : "";    
                    head.CountyName1                =  dr["CountyName1"].ToString().Length > 0 ? dr["CountyName1"].ToString() : "";    
                    head.ShipZip                    =  dr["ShipZip"].ToString().Length > 0 ? dr["ShipZip"].ToString() : "";    
                    head.ShipCountry                =  dr["ShipCountry"].ToString().Length > 0 ? dr["ShipCountry"].ToString() : "";    
                    head.ShipPhone                  =  dr["ShipPhone"].ToString().Length > 0 ? dr["ShipPhone"].ToString() : "";    
                    head.ShipFax                    =  dr["ShipFax"].ToString().Length > 0 ? dr["ShipFax"].ToString() : "";    
                    head.ShipCompany                =  dr["ShipCompany"].ToString().Length > 0 ? dr["ShipCompany"].ToString() : "";    
                    head.ShipPager                  =  dr["ShipPager"].ToString().Length > 0 ? dr["ShipPager"].ToString() : "";    
                    head.ShipCellular               =  dr["ShipCellular"].ToString().Length > 0 ? dr["ShipCellular"].ToString() : "";     
                    head.Username                   =  dr["Username"].ToString().Length > 0 ? dr["Username"].ToString() : "";    
                    head.CountyTaxId                =  dr["CountyTaxId"].ToString().Length > 0 ? dr["CountyTaxId"].ToString() : "";    
                    head.ScgId                      =  dr["scg_id"].ToString().Length > 0 ? dr["scg_id"].ToString() : "";    
                    head.Optional4                  =  dr["Optional4"].ToString().Length > 0 ? dr["Optional4"].ToString() : "";    
                    head.Optional5                  =  dr["Optional5"].ToString().Length > 0 ? dr["Optional5"].ToString() : "";    
                    head.Optional6                  =  dr["Optional6"].ToString().Length > 0 ? dr["Optional6"].ToString() : "";    
                    head.OrderXml                   =  dr["OrderXml"].ToString().Length > 0 ? dr["OrderXml"].ToString() : "";    
                    head.Temp                       =  dr["Temp"].ToString().Length > 0 ? dr["Temp"].ToString() : "";    
                    head.FTransaction               =  dr["ftransaction"].ToString().Length > 0 ? dr["ftransaction"].ToString() : "";    
                    head.AffiliateId                =  dr["AffiliateID"].ToString().Length > 0 ? dr["AffiliateID"].ToString() : "";
                    head.SubAffiliateID             = dr["SubAffiliateID"].ToString().Length > 0 ? dr["SubAffiliateID"].ToString() : "";
                    head.LegalResponse              = dr["LegalResponse"].ToString().Length > 0 ? dr["LegalResponse"].ToString() : "";
                    head.PcsNm                      = dr["pcs_nm"].ToString().Length > 0 ? dr["pcs_nm"].ToString() : "";
                    head.PcgNm                      = dr["pcg_nm"].ToString().Length > 0 ? dr["pcg_nm"].ToString() : "";
                    head.TaxProfileLog              = dr["TaxProfileLog"].ToString().Length > 0 ? dr["TaxProfileLog"].ToString() : "";
                    head.Shipment                   = (Int32)(dr["Shipment"].ToString().Length > 0 ? Int32.Parse(dr["Shipment"].ToString()) : 0);
                    head.TaxIdNumber                = dr["TaxIdNumber"].ToString().Length > 0 ? dr["TaxIdNumber"].ToString() : "";
                    head.Status                     = dr["Status"].ToString().Length > 0 ? dr["Status"].ToString() : "";
                    head.ServerName                 = dr["ServerName"].ToString().Length > 0 ? dr["ServerName"].ToString() : "";
                    head.BillShippingAccountNumber  = dr["BillShippingAccountNumber"].ToString().Length > 0 ? dr["BillShippingAccountNumber"].ToString() : "";
                    head.EmailSentFlag              = dr["EmailSentFlag"].ToString().Length > 0 ? dr["EmailSentFlag"].ToString() : "";
                    head.Flags                      = (Int64)(dr["Flags"].ToString().Length > 0 ? Int64.Parse(dr["Flags"].ToString()) : 0);
                    head.CurrencyId                 = dr["CurrencyId"].ToString().Length > 0 ? dr["CurrencyId"].ToString() : "";
                    head.EmailFlags                 = (Int64)(dr["EmailFlags"].ToString().Length > 0 ? Int64.Parse(dr["EmailFlags"].ToString()) : 0);
                    head.AccountId                  = dr["AccountId"].ToString().Length > 0 ? dr["AccountId"].ToString() : "";
                    head.GeneralLedgerAccountId     = dr["GeneralLedgerAccountId"].ToString().Length > 0 ? dr["GeneralLedgerAccountId"].ToString() : "";
                    head.OrderSource                = dr["OrderSource"].ToString().Length > 0 ? dr["OrderSource"].ToString() : "";
                    head.NewOrderEmailSent          = Convert.ToBoolean(dr["NewOrderEmailSent"].ToString().Length > 0 ? Convert.ToBoolean(dr["NewOrderEmailSent"].ToString()) : false);
                    head.ShipViaType                = dr["ShipViaType"].ToString().Length > 0 ? dr["ShipViaType"].ToString() : "";
                    head.PaymentMethod              = dr["PaymentMethod"].ToString().Length > 0 ? dr["PaymentMethod"].ToString() : "";
                    head.BdIdInvoice                = dr["bd_id_invoice"].ToString().Length > 0 ? dr["bd_id_invoice"].ToString() : "";
                    head.BdIdPacking                = dr["bd_id_packing"].ToString().Length > 0 ? dr["bd_id_packing"].ToString() : "";
                    head.OrdersCcRemoved            = dr["orders_ccremoved"].ToString().Length > 0 ? dr["orders_ccremoved"].ToString() : "";
                    head.Attention                  = dr["Attention"].ToString().Length > 0 ? dr["Attention"].ToString() : "";
                    head.ShipAddress3               = dr["ShipAddress3"].ToString().Length > 0 ? dr["ShipAddress3"].ToString() : "";
                    head.ShippingNetTotal           = (Decimal)(dr["ShippingNetTotal"].ToString().Length > 0 ? Decimal.Parse(dr["ShippingNetTotal"].ToString()) : 0);
                    head.CouponAccountingType       = dr["CouponAccountingType"].ToString().Length > 0 ? dr["CouponAccountingType"].ToString() : "";
                    head.CouponSubtotalProducts     = (Decimal)(dr["CouponSubtotalProducts"].ToString().Length > 0 ? Decimal.Parse(dr["CouponSubtotalProducts"].ToString()) : 0);
                    head.CouponSubtotalShipping     = (Decimal)(dr["CouponSubtotalShipping"].ToString().Length > 0 ? Decimal.Parse(dr["CouponSubtotalShipping"].ToString()) : 0);
                    head.TaxSubtotalProducts        = (Decimal)(dr["TaxSubtotalProducts"].ToString().Length > 0 ? Decimal.Parse(dr["TaxSubtotalProducts"].ToString()) : 0);
                    head.TaxSubtotalShipping        = (Decimal)(dr["TaxSubtotalShipping"].ToString().Length > 0 ? Decimal.Parse(dr["TaxSubtotalShipping"].ToString()) : 0);
                    head.TaxSubtotalHandling        = (Decimal)(dr["TaxSubtotalHandling"].ToString().Length > 0 ? Decimal.Parse(dr["TaxSubtotalHandling"].ToString()) : 0);
                    head.SalesRep                   = dr["SalesRep"].ToString().Length > 0 ? dr["SalesRep"].ToString() : "";
                    head.Terms                      = dr["Terms"].ToString().Length > 0 ? dr["Terms"].ToString() : "";
                    head.AllowBo                    = Convert.ToBoolean(dr["allow_bo"].ToString().Length > 0 ? Convert.ToBoolean(dr["allow_bo"].ToString()) : false);
                    head.BoPreference               = dr["bo_preference"].ToString().Length > 0 ? dr["bo_preference"].ToString() : "";
                    head.ErpOrderStatus             = dr["ErpOrderStatus"].ToString().Length > 0 ? dr["ErpOrderStatus"].ToString() : "";
                    head.ErpHoldReason              = dr["ErpHoldReason"].ToString().Length > 0 ? dr["ErpHoldReason"].ToString() : "";
                    head.TaxCalculationtype         = dr["TaxCalculationtype"].ToString().Length > 0 ? dr["TaxCalculationtype"].ToString() : "";
                    head.TaxMatchFrom               = dr["TaxMatchFrom"].ToString().Length > 0 ? dr["TaxMatchFrom"].ToString() : "";
                    head.TxTotal                    = (Decimal)(dr["TxTotal"].ToString().Length > 0 ? Decimal.Parse(dr["TxTotal"].ToString()) : 0);
                    head.Taxdetail                  = dr["Taxdetail"].ToString().Length > 0 ? dr["Taxdetail"].ToString() : "";
                    head.StatusReason               = dr["StatusReason"].ToString().Length > 0 ? dr["StatusReason"].ToString() : "";
                    head.SpendAllowanceBefore       = (Decimal)(dr["SpendAllowanceBefore"].ToString().Length > 0 ? Decimal.Parse(dr["SpendAllowanceBefore"].ToString()) : 0);
                    head.SpendAllowanceUsed         = (Decimal)(dr["SpendAllowanceUsed"].ToString().Length > 0 ? Decimal.Parse(dr["SpendAllowanceUsed"].ToString()) : 0);
                    head.SpendAllowanceAfter        = (Decimal)(dr["SpendAllowanceAfter"].ToString().Length > 0 ? Decimal.Parse(dr["SpendAllowanceAfter"].ToString()) : 0);
                    head.OBalance                   = (Decimal)(dr["o_balance"].ToString().Length > 0 ? Decimal.Parse(dr["o_balance"].ToString()) : 0);
                    head.PaidByOtherAmount          = (Decimal)(dr["PaidByOtherAmount"].ToString().Length > 0 ? Decimal.Parse(dr["PaidByOtherAmount"].ToString()) : 0);
                    head.WarehouseSelection         = dr["WarehouseSelection"].ToString().Length > 0 ? dr["WarehouseSelection"].ToString() : "";
                    head.ShippingAddressCode        = dr["ShippingAddressCode"].ToString().Length > 0 ? dr["ShippingAddressCode"].ToString() : "";
                    head.SEm                        = dr["s_em"].ToString().Length > 0 ? dr["s_em"].ToString() : "";
                    head.SNm                        = dr["s_nm"].ToString().Length > 0 ? dr["s_nm"].ToString() : "";
                    head.CNm                        = dr["c_nm"].ToString().Length > 0 ? dr["c_nm"].ToString() : "";
                    head.TaxExempt                  = Convert.ToBoolean(dr["TaxExempt"].ToString().Length > 0 ? Convert.ToBoolean(dr["TaxExempt"].ToString()) : false);
                    head.OipFlagSet                 = Convert.ToBoolean(dr["oip_flag_set"].ToString().Length > 0 ? Convert.ToBoolean(dr["oip_flag_set"].ToString()) : false);
                    head.IPAddress                  = dr["IPAddress"].ToString().Length > 0 ? dr["IPAddress"].ToString() : "";
                    head.BrowserName                = dr["BrowserName"].ToString().Length > 0 ? dr["BrowserName"].ToString() : "";                    
                    ord.CatalogWebOrderSummary = head;      //  Store order header information
                    //
                    dr.NextResult();        //  Move to the second select result set
                    //
                    while (dr != null && dr.Read())
                    {   //  Get all order items (From second select result set)                        
                        CatalogWebOrderItem itm = new CatalogWebOrderItem();
                        items++;
                        itm.CatWebOrderItemId   = dr["CatWebOrderItemId"].ToString().Length > 0 ? dr["CatWebOrderItemId"].ToString() : "";
                        itm.CatWebOrderId       = dr["CatWebOrderId"].ToString().Length > 0 ? dr["CatWebOrderId"].ToString() : "";
                        itm.ProductId           = dr["ProductId"].ToString().Length > 0 ? dr["ProductId"].ToString() : "";
                        itm.ProductName         = dr["ProductName"].ToString().Length > 0 ? dr["ProductName"].ToString() : "";
                        itm.RetailPrice         = (Decimal)(dr["RetailPrice"].ToString().Length > 0 ? Decimal.Parse(dr["RetailPrice"].ToString()) : 0);
                        itm.RetailSetupPrice    = (Decimal)(dr["RetailSetupPrice"].ToString().Length > 0 ? Decimal.Parse(dr["RetailSetupPrice"].ToString()) : 0);
                        itm.Cost                = (Decimal)(dr["Cost"].ToString().Length > 0 ? Decimal.Parse(dr["Cost"].ToString()) : 0);
                        itm.SetupCost           = (Decimal)(dr["SetupCost"].ToString().Length > 0 ? Decimal.Parse(dr["SetupCost"].ToString()) : 0);
                        itm.MinimumPrice        = (Decimal)(dr["MinimumPrice"].ToString().Length > 0 ? Decimal.Parse(dr["MinimumPrice"].ToString()) : 0);
                        itm.MinimumSetupPrice   = (Decimal)(dr["MinimumSetupPrice"].ToString().Length > 0 ? Decimal.Parse(dr["MinimumSetupPrice"].ToString()) : 0);
                        itm.Frequency           = dr["Frequency"].ToString().Length > 0 ? dr["Frequency"].ToString() : "";
                        itm.ESellerSetupPrice   = (Decimal)(dr["eSellerSetupPrice"].ToString().Length > 0 ? Decimal.Parse(dr["eSellerSetupPrice"].ToString()) : 0);
                        itm.ESellerPrice        = (Decimal)(dr["eSellerPrice"].ToString().Length > 0 ? Decimal.Parse(dr["eSellerPrice"].ToString()) : 0);  
                        itm.ESellerBonus        = (Decimal)(dr["eSellerBonus"].ToString().Length > 0 ? Decimal.Parse(dr["eSellerBonus"].ToString()) : 0);
                        itm.Quantity            = (Decimal)(dr["Quantity"].ToString().Length > 0 ? Decimal.Parse(dr["Quantity"].ToString()) : 0);
                        itm.Sku                 = dr["Sku"].ToString().Length > 0 ? dr["Sku"].ToString() : "";
                        itm.PricePaid           = (Decimal)(dr["PricePaid"].ToString().Length > 0 ? Decimal.Parse(dr["PricePaid"].ToString()) : 0);
                        itm.SPrice              = (Decimal)(dr["s_price"].ToString().Length > 0 ? Decimal.Parse(dr["s_price"].ToString()) : 0);
                        itm.Shipped             = (Decimal)(dr["shipped"].ToString().Length > 0 ? Decimal.Parse(dr["shipped"].ToString()) : 0);
                        itm.Bo                  = (Decimal)(dr["bo"].ToString().Length > 0 ? Decimal.Parse(dr["bo"].ToString()) : 0);
                        itm.Bogus               = (Decimal)(dr["bogus"].ToString().Length > 0 ? Decimal.Parse(dr["bogus"].ToString()) : 0);
                        itm.ReturnReq           = (Decimal)(dr["return_req"].ToString().Length > 0 ? Decimal.Parse(dr["return_req"].ToString()) : 0);  
                        itm.ReturnRec           = (Decimal)(dr["return_rec"].ToString().Length > 0 ? Decimal.Parse(dr["return_rec"].ToString()) : 0);  
                        itm.BoDate              = dr["bo_date"].ToString().Length > 0 ? DateTime.Parse(dr["bo_date"].ToString()) : DateTime.Now;
                        itm.Optional1           = dr["Optional1"].ToString().Length > 0 ? dr["Optional1"].ToString() : "";
                        itm.Optional2           = dr["Optional2"].ToString().Length > 0 ? dr["Optional2"].ToString() : "";
                        itm.Optional3           = dr["Optional3"].ToString().Length > 0 ? dr["Optional3"].ToString() : "";
                        itm.Optional4           = dr["Optional4"].ToString().Length > 0 ? dr["Optional4"].ToString() : "";
                        itm.Optional5           = dr["Optional5"].ToString().Length > 0 ? dr["Optional5"].ToString() : "";
                        itm.Optional6           = dr["Optional6"].ToString().Length > 0 ? dr["Optional6"].ToString() : "";
                        itm.Optional7           = dr["Optional7"].ToString().Length > 0 ? dr["Optional7"].ToString() : "";
                        itm.Optional8           = dr["Optional8"].ToString().Length > 0 ? dr["Optional8"].ToString() : "";
                        itm.ShippedPrice        = (Decimal)(dr["ShippedPrice"].ToString().Length > 0 ? Decimal.Parse(dr["ShippedPrice"].ToString()) : 0);  
                        itm.CartOption          = dr["CartOption"].ToString().Length > 0 ? dr["CartOption"].ToString() : "";
                        itm.ProductUnit         = dr["ProductUnit"].ToString().Length > 0 ? dr["ProductUnit"].ToString() : "";
                        itm.Vol1                = (Decimal)(dr["Vol1"].ToString().Length > 0 ? Decimal.Parse(dr["Vol1"].ToString()) : 0); 
                        itm.Vol2                = (Decimal)(dr["Vol2"].ToString().Length > 0 ? Decimal.Parse(dr["Vol2"].ToString()) : 0); 
                        itm.Vol3                = (Decimal)(dr["Vol3"].ToString().Length > 0 ? Decimal.Parse(dr["Vol3"].ToString()) : 0); 
                        itm.Vol4                = (Decimal)(dr["Vol4"].ToString().Length > 0 ? Decimal.Parse(dr["Vol4"].ToString()) : 0); 
                        itm.Vol5                = (Decimal)(dr["Vol5"].ToString().Length > 0 ? Decimal.Parse(dr["Vol5"].ToString()) : 0); 
                        itm.Vol6                = (Decimal)(dr["Vol6"].ToString().Length > 0 ? Decimal.Parse(dr["Vol6"].ToString()) : 0); 
                        itm.Vol7                = (Decimal)(dr["Vol7"].ToString().Length > 0 ? Decimal.Parse(dr["Vol7"].ToString()) : 0); 
                        itm.Vol8                = (Decimal)(dr["Vol8"].ToString().Length > 0 ? Decimal.Parse(dr["Vol8"].ToString()) : 0); 
                        itm.Vol9                = (Decimal)(dr["Vol9"].ToString().Length > 0 ? Decimal.Parse(dr["Vol9"].ToString()) : 0); 
                        itm.Vol10               = (Decimal)(dr["Vol10"].ToString().Length > 0 ? Decimal.Parse(dr["Vol10"].ToString()) : 0); 
                        itm.Fvb                 = (Decimal)(dr["fvb"].ToString().Length > 0 ? Decimal.Parse(dr["fvb"].ToString()) : 0); 
                        itm.Bvb                 = (Decimal)(dr["bvb"].ToString().Length > 0 ? Decimal.Parse(dr["bvb"].ToString()) : 0); 
                        itm.Type                = dr["Type"].ToString().Length > 0 ? dr["Type"].ToString() : "";
                        itm.TbpPaid             = dr["tbp_paid"].ToString().Length > 0 ? dr["tbp_paid"].ToString() : "";
                        itm.DecKeys             = dr["deckeys"].ToString().Length > 0 ? dr["deckeys"].ToString() : "";
                        itm.Notes               = dr["Notes"].ToString().Length > 0 ? dr["Notes"].ToString() : "";
                        itm.ProductCategoryStyleId = dr["ProductCategoryStyleId"].ToString().Length > 0 ? dr["ProductCategoryStyleId"].ToString() : "";
                        itm.PcgId               = dr["pcg_id"].ToString().Length > 0 ? dr["pcg_id"].ToString() : "";
                        itm.PptNm               = dr["ppt_nm"].ToString().Length > 0 ? dr["ppt_nm"].ToString() : "";
                        itm.PpbNm               = dr["ppb_nm"].ToString().Length > 0 ? dr["ppb_nm"].ToString() : "";
                        itm.DecId               = dr["dec_id"].ToString().Length > 0 ? dr["dec_id"].ToString() : "";
                        itm.PppNm               = dr["ppp_nm"].ToString().Length > 0 ? dr["ppp_nm"].ToString() : "";
                        itm.RowAdded            = dr["RowAdded"].ToString().Length > 0 ? DateTime.Parse(dr["RowAdded"].ToString()) : DateTime.Now;
                        itm.SetSize             = (Int32)(dr["SetSize"].ToString().Length > 0 ? Int32.Parse(dr["SetSize"].ToString()) : 0);
                        itm.Instance            = dr["Instance"].ToString().Length > 0 ? dr["Instance"].ToString() : "";
                        itm.Mqty                = dr["mqty"].ToString().Length > 0 ? dr["mqty"].ToString() : "";
                        itm.Unq                 = dr["unq"].ToString().Length > 0 ? dr["unq"].ToString() : "";
                        itm.RPAdded             = dr["r_p_added"].ToString().Length > 0 ? dr["r_p_added"].ToString() : "";
                        itm.RPType              = dr["r_p_type"].ToString().Length > 0 ? dr["r_p_type"].ToString() : "";
                        itm.PsgInstance         = dr["psg_instance"].ToString().Length > 0 ? dr["psg_instance"].ToString() : "";
                        itm.OUnq                = dr["o_unq"].ToString().Length > 0 ? dr["o_unq"].ToString() : "";
                        itm.Status              = dr["status"].ToString().Length > 0 ? dr["status"].ToString() : "";
                        itm.PONumber            = dr["PONumber"].ToString().Length > 0 ? dr["PONumber"].ToString() : "";
                        itm.SetupRetailPrice    = (Decimal)(dr["SetupRetailPrice"].ToString().Length > 0 ? Decimal.Parse(dr["SetupRetailPrice"].ToString()) : 0); 
                        itm.PriceBeforeMarkup   = (Decimal)(dr["PriceBeforeMarkup"].ToString().Length > 0 ? Decimal.Parse(dr["PriceBeforeMarkup"].ToString()) : 0); 
                        itm.Flags               = (Int64)(dr["Flags"].ToString().Length > 0 ? Int64.Parse(dr["Flags"].ToString()) : 0);
                        itm.OrderFlags          = (Int64)(dr["OrderFlags"].ToString().Length > 0 ? Int64.Parse(dr["OrderFlags"].ToString()) : 0);
                        itm.TrackingXml         =  dr["TrackingXml"].ToString().Length > 0 ? dr["TrackingXml"].ToString() : "";
                        itm.ExpectedDate        = dr["ExpectedDate"].ToString().Length > 0 ? DateTime.Parse(dr["ExpectedDate"].ToString()) : DateTime.Now;
                        itm.OrderDetailNumber   = (Decimal)(dr["OrderDetailNumber"].ToString().Length > 0 ? Decimal.Parse(dr["OrderDetailNumber"].ToString()) : 0); 
                        itm.GeneralLedgerAccountId      = dr["GeneralLedgerAccountId"].ToString().Length > 0 ? dr["GeneralLedgerAccountId"].ToString() : "";
                        itm.PriceCalculationDescription = dr["PriceCalculationDescription"].ToString().Length > 0 ? dr["PriceCalculationDescription"].ToString() : "";
                        itm.UomConversion       = (Decimal)(dr["uom_conversion"].ToString().Length > 0 ? Decimal.Parse(dr["uom_conversion"].ToString()) : 0); 
                        itm.UomStockQty         = (Decimal)(dr["uom_stock_qty"].ToString().Length > 0 ? Decimal.Parse(dr["uom_stock_qty"].ToString()) : 0);  
                        itm.UomStockPricePer    = (Decimal)(dr["uom_stock_price_per"].ToString().Length > 0 ? Decimal.Parse(dr["uom_stock_price_per"].ToString()) : 0);  
                        itm.UomNm               = dr["uom_nm"].ToString().Length > 0 ? dr["uom_nm"].ToString() : "";
                        itm.NScId               = (Decimal)(dr["n_sc_id"].ToString().Length > 0 ? Decimal.Parse(dr["n_sc_id"].ToString()) : 0);  
                        itm.DisplayType         = dr["DisplayType"].ToString().Length > 0 ? dr["DisplayType"].ToString() : "";
                        itm.IsCsg               = Convert.ToBoolean(dr["is_csg"].ToString().Length > 0 ? Convert.ToBoolean(dr["is_csg"].ToString()) : false);
                        itm.GarmentPlacementId  = dr["GarmentPlacementId"].ToString().Length > 0 ? dr["GarmentPlacementId"].ToString() : "";
                        itm.SysDirtyPublic      = Convert.ToBoolean(dr["sys_dirty_public"].ToString().Length > 0 ? Convert.ToBoolean(dr["sys_dirty_public"].ToString()) : false);
                        itm.SysLastModifiedDatePublic = dr["sys_last_modified_date_public"].ToString().Length > 0 ? DateTime.Parse(dr["sys_last_modified_date_public"].ToString()) : DateTime.Now;
                        itm.RequestedQty        = (Decimal)(dr["RequestedQty"].ToString().Length > 0 ? Decimal.Parse(dr["RequestedQty"].ToString()) : 0);
                        itm.RequestedSku        = dr["RequestedSku"].ToString().Length > 0 ? dr["RequestedSku"].ToString() : "";
                        itm.RequestedUnitPrice  = (Decimal)(dr["RequestedUnitPrice"].ToString().Length > 0 ? Decimal.Parse(dr["RequestedUnitPrice"].ToString()) : 0);
                        itm.QtyAvailable        = (Decimal)(dr["QtyAvailable"].ToString().Length > 0 ? Decimal.Parse(dr["QtyAvailable"].ToString()) : 0); 
                        list.Add(itm);
                    }
                    ord.CatalogWebOrderItems = list;   //  Store order Items information 
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return ord;
        }

        /// <summary>
        ///     Update an specific order header information with the import results status codes.
        ///     The order parameter is mandatory
        ///     The option parameter update the WasImported columnn of the order header
        ///     The option importProblem update the hasImportProblem
        /// </summary>
        /// <param name="orderId">specified the order number to be updated</param>
        /// <param name="externalOrderNo">specified the NAV order number</param>
        /// <param name="exportStatus">Exported order status (0- no/1- yes)</param>
        /// <returns></returns>
        public int updCatalogOrder(string orderId, string externalOrderNo, int exportStatus)
        {
            object ret = null;
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@OrderKey", orderId.ToString());
                args.Add("@NavOrderNo", externalOrderNo.ToString());
                args.Add("@Status", exportStatus.ToString());

                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                ret = access.ExecuteScalar("dbo.MW_WSPUpdateImportedWebOrder", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //   
            }
            catch (Exception)
            {
                ikr.writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1, ConfigurationManager.AppSettings["TableName"], "Table update error, order " + orderId);
            }
            return Convert.ToInt32(ret);
        }

        /// <summary>
        ///     Get Catalog Convert orders List - Not imported orders From [designs_approve_request]
        /// </summary>
        /// <returns>List of CatConvertedOrderId's</returns>
        public List<string> getCatalogConvertOrder_NotImportedList()
        {
            SqlDataReader dr = null;
            var orderList = new List<string>();
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                //args.Add("@Order", order.ToString());
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("MW_WSPGetConvertOrdersList_notImported", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //
                while (dr != null && dr.Read())
                {
                    string orderId = null;
                    orderId = dr["CatConvertOrderId"].ToString().Length > 0 ? (dr["CatConvertOrderId"].ToString()) : "0";
                    orderList.Add(orderId);
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return orderList;
        }

        public CatalogConvertTable getCatalogConvertOrder(string order)
        {
            SqlDataReader dr = null;
            CatalogConvertTable ord = new CatalogConvertTable();
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@Order", order);
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("[MW_WSPGetConvertOrder_V01]", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //
                while (dr != null && dr.Read())
                {   
                    CatalogConvertOrderHeader head = new CatalogConvertOrderHeader();
                    head.CatConvertOrderId  = dr["CatConvertOrderId"].ToString().Length > 0 ? (dr["CatConvertOrderId"].ToString()) : "0";
                    head.AccountNumber      = dr["AccountNumber"].ToString().Length > 0 ? dr["AccountNumber"].ToString() : "";
                    head.ActionTaken        = dr["ActionTaken"].ToString().Length > 0 ? dr["ActionTaken"].ToString() : "";
                    head.AddEditFlag        = (Int64)(dr["AddEditFlag"].ToString().Length > 0 ? Int64.Parse(dr["AddEditFlag"].ToString()) : 0);
                    head.CatConvertOrderId  = dr["CatConvertOrderId"].ToString().Length > 0 ? dr["CatConvertOrderId"].ToString() : "";
                    head.CreateIPAddress    = dr["CreateIPAddress"].ToString().Length > 0 ? dr["CreateIPAddress"].ToString() : "";
                    head.CreateUserId       = dr["CreateUserId"].ToString().Length > 0 ? dr["CreateUserId"].ToString() : "";
                    head.CreationDate       = dr["CreationDate"].ToString().Length > 0 ? DateTime.Parse(dr["CreationDate"].ToString()) : DateTime.Now;
                    head.CustomerEmailAddress = dr["CustomerEmailAddress"].ToString().Length > 0 ? dr["CustomerEmailAddress"].ToString() : "";
                    head.DateCreated        = dr["CreationDate"].ToString().Length > 0 ? DateTime.Parse(dr["CreationDate"].ToString()) : DateTime.Now;
                    head.DesignId           = dr["DesignId"].ToString().Length > 0 ? dr["DesignId"].ToString() : "";
                    head.DirtyFlags         = (Int32)(dr["DirtyFlags"].ToString().Length > 0 ? Int32.Parse(dr["DirtyFlags"].ToString()) : 0);
                    head.EmailAddresses     = dr["EmailAddresses"].ToString().Length > 0 ? dr["EmailAddresses"].ToString() : "";
                    head.EmailFlag          = (Int64)(dr["EmailFlag"].ToString().Length > 0 ? Int64.Parse(dr["EmailFlag"].ToString()) : 0);
                    head.FileExists         = (Decimal)(dr["FileExists"].ToString().Length > 0 ? Decimal.Parse(dr["FileExists"].ToString()) : 0);
                    //head.MwConvertOrderId   = (Int32)(dr["MwConvertOrderId"].ToString().Length > 0 ? Int32.Parse(dr["MwConvertOrderId"].ToString()) : 0);
                    //head.MwOrderMasterId    = (Int32)(dr["MwOrderMasterId"].ToString().Length > 0 ? Int32.Parse(dr["MwOrderMasterId"].ToString()) : 0);
                    head.Optional1          = dr["Optional1"].ToString().Length > 0 ? dr["Optional1"].ToString() : "";
                    head.Optional2          = dr["Optional2"].ToString().Length > 0 ? dr["Optional2"].ToString() : "";
                    head.Optional3          = dr["Optional3"].ToString().Length > 0 ? dr["Optional3"].ToString() : "";
                    head.Quantity           = (Decimal)(dr["Quantity"].ToString().Length > 0 ? Decimal.Parse(dr["Quantity"].ToString()) : 0);
                    head.ReferenceId        = dr["ReferenceId"].ToString().Length > 0 ? dr["ReferenceId"].ToString() : "";
                    head.ShipBlind          = Convert.ToBoolean(dr["ShipBlind"].ToString().Length > 0 ? Convert.ToBoolean(dr["ShipBlind"].ToString()) : false);
                    head.ShipAddressLine1   = dr["ShipAddressLine1"].ToString().Length > 0 ? dr["ShipAddressLine1"].ToString() : "";
                    head.ShipAddressLine2   = dr["ShipAddressLine2"].ToString().Length > 0 ? dr["ShipAddressLine2"].ToString() : "";
                    head.ShipCity           = dr["ShipCity"].ToString().Length > 0 ? dr["ShipCity"].ToString() : "";
                    head.ShipCompany        = dr["ShipCompany"].ToString().Length > 0 ? dr["ShipCompany"].ToString() : "";
                    head.ShipCountry        = dr["ShipCountry"].ToString().Length > 0 ? dr["ShipCountry"].ToString() : "";
                    head.ShipEmailAddress   = dr["ShipEmailAddress"].ToString().Length > 0 ? dr["ShipEmailAddress"].ToString() : "";
                    head.ShipFirstName      = dr["ShipFirstName"].ToString().Length > 0 ? dr["ShipFirstName"].ToString() : "";
                    head.ShipId             = dr["ShipId"].ToString().Length > 0 ? dr["Optional3"].ToString() : "";
                    head.ShipLastname       = dr["ShipLastname"].ToString().Length > 0 ? dr["ShipLastname"].ToString() : "";
                    head.ShipName           = dr["ShipName"].ToString().Length > 0 ? dr["ShipName"].ToString() : "";
                    head.ShipPhoneNumber    = dr["ShipPhoneNumber"].ToString().Length > 0 ? dr["ShipPhoneNumber"].ToString() : "";
                    head.ShipState          = dr["ShipState"].ToString().Length > 0 ? dr["ShipState"].ToString() : "";
                    head.ShipVia            = (Decimal)(dr["ShipVia"].ToString().Length > 0 ? Decimal.Parse(dr["ShipVia"].ToString()) : 0);
                    head.ShipViaCode        = dr["ShipViaCode"].ToString().Length > 0 ? dr["ShipViaCode"].ToString() : "";
                    head.ShipZip            = dr["ShipZip"].ToString().Length > 0 ? dr["ShipZip"].ToString() : "";
                    head.Sku                = dr["Sku"].ToString().Length > 0 ? dr["Sku"].ToString() : "";
                    head.Timestamp          = (Byte[])(dr["Timestamp"]);
                    head.UserInitials       = dr["UserInitials"].ToString().Length > 0 ? dr["UserInitials"].ToString() : "";                    
                    //
                    ord.CatalogConvertOrderSummary = head;
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return ord;
        }

        public List<string> getCatalogDesignRequests_NotImportedList(string orderStatus, string status)
        {
            SqlDataReader dr = null;
            var orderList = new List<string>();
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@export_status", orderStatus.ToString());
                args.Add("@status", status.ToString());
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("MW_WSPGetDesignRequestsList_notImported", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //
                while (dr != null && dr.Read())
                {
                    string orderId = null;
                    orderId = dr["CatDesignRequestOrderId"].ToString().Length > 0 ? (dr["CatDesignRequestOrderId"].ToString()) : "0";
                    orderList.Add(orderId);
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return orderList;
        }

        public CatalogDesignRequestTables getCatalogDesignRequestOrder(string order)
        {
            SqlDataReader dr = null;
            CatalogDesignRequestTables ord = new CatalogDesignRequestTables();
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@Order", order);
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("[MW_WSPGetConvertOrder_V01]", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //
                while (dr != null && dr.Read())
                {
                    DesignRequest head = new DesignRequest();
                    head.AIdCusNo               = dr["a_id_cus_no"].ToString().Length > 0 ? (dr["a_id_cus_no"].ToString()) : "0";
                    head.AccountId              = dr["AccountId"].ToString().Length > 0 ? dr["AccountId"].ToString() : "";
                    head.AddEditFlag            = (Int64)(dr["AddEditFlag"].ToString().Length > 0 ? Int64.Parse(dr["AddEditFlag"].ToString()) : 0);
                    head.AnnotateAttempted      = Convert.ToBoolean(dr["annotate_attempted"].ToString().Length > 0 ? Convert.ToBoolean(dr["annotate_attempted"].ToString()) : false);
                    head.AnnotateId             = dr["annotate_id"].ToString().Length > 0 ? dr["annotate_id"].ToString() : "";
                    head.AnnotateInfo           = dr["annotate_info"].ToString().Length > 0 ? dr["annotate_info"].ToString() : "";
                    head.AnnotateSaved          = Convert.ToBoolean(dr["annotate_saved"].ToString().Length > 0 ? Convert.ToBoolean(dr["annotate_saved"].ToString()) : false);
                    head.ApprovedDate           = dr["ApprovedDate"].ToString().Length > 0 ? DateTime.Parse(dr["ApprovedDate"].ToString()) : DateTime.Now;
                    head.ArtHeight              = (Decimal)(dr["ArtHeight"].ToString().Length > 0 ? Decimal.Parse(dr["ArtHeight"].ToString()) : 0);
                    head.ArtWidth               = (Decimal)(dr["ArtWidth"].ToString().Length > 0 ? Decimal.Parse(dr["ArtWidth"].ToString()) : 0);
                    head.Artwork                = dr["Artwork"].ToString().Length > 0 ? dr["Artwork"].ToString() : "";
                    head.AtcFlagged             = Convert.ToBoolean(dr["atc_flagged"].ToString().Length > 0 ? Convert.ToBoolean(dr["atc_flagged"].ToString()) : false);
                    head.Backing2Id             = dr["Backing2Id"].ToString().Length > 0 ? dr["Backing2Id"].ToString() : "";
                    head.BackingId              = dr["BackingId"].ToString().Length > 0 ? dr["BackingId"].ToString() : "";
                    head.Border2Id              = dr["Border2Id"].ToString().Length > 0 ? dr["Border2Id"].ToString() : "";
                    head.BorderId               = dr["BorderId"].ToString().Length > 0 ? dr["BorderId"].ToString() : "";
                    head.CatDesignRequestOrderId = dr["CatDesignRequestOrderId"].ToString().Length > 0 ? dr["CatDesignRequestOrderId"].ToString() : "";
                    head.ColorChange            = Convert.ToBoolean(dr["ColorChange"].ToString().Length > 0 ? Convert.ToBoolean(dr["ColorChange"].ToString()) : false);
                    head.ColorInstructions      = dr["ColorInstructions"].ToString().Length > 0 ? dr["ColorInstructions"].ToString() : "";
                    head.ColorPreference        = dr["ColorPreference"].ToString().Length > 0 ? dr["ColorPreference"].ToString() : "";
                    head.CompleteDate           = dr["CompleteDate"].ToString().Length > 0 ? DateTime.Parse(dr["CompleteDate"].ToString()) : DateTime.Now;
                    head.CompletedDesignNormal  = dr["CompletedDesignNormal"].ToString().Length > 0 ? dr["CompletedDesignNormal"].ToString() : "";
                    head.CompletedPictureThumbnail = dr["CompletedPictureThumbnail"].ToString().Length > 0 ? dr["CompletedPictureThumbnail"].ToString() : "";
                    head.ContactFullName        = dr["ContactFullName"].ToString().Length > 0 ? dr["ContactFullName"].ToString() : "";
                    head.Contacts               = dr["Contacts"].ToString().Length > 0 ? dr["Contacts"].ToString() : "";
                    head.CreateDate             = dr["CreateDate"].ToString().Length > 0 ? DateTime.Parse(dr["CreateDate"].ToString()) : DateTime.Now;
                    head.CustomerId             = dr["CustomerId"].ToString().Length > 0 ? dr["CustomerId"].ToString() : "";
                    head.CustomerType           = dr["CustomerType"].ToString().Length > 0 ? dr["CustomerType"].ToString() : "";
                    head.DeliveryOption         = dr["DeliveryOption"].ToString().Length > 0 ? dr["DeliveryOption"].ToString() : "";
                    head.Description            = dr["Description"].ToString().Length > 0 ? dr["Description"].ToString() : "";
                    head.DesignId               = dr["DesignId"].ToString().Length > 0 ? dr["DesignId"].ToString() : "";
                    head.DiecodeId              = dr["DiecodeId"].ToString().Length > 0 ? dr["DiecodeId"].ToString() : "";
                    head.DpNotificationSent     = Convert.ToBoolean(dr["DpNotificationSent"].ToString().Length > 0 ? Convert.ToBoolean(dr["DpNotificationSent"].ToString()) : false);
                    head.DpSent                 = dr["DpSent"].ToString().Length > 0 ? DateTime.Parse(dr["DpSent"].ToString()) : DateTime.Now;
                    head.DrBackingWeiId         = dr["dr_backing_wei_id"].ToString().Length > 0 ? dr["dr_backing_wei_id"].ToString() : "";
                    head.DrBordersWeiId         = dr["dr_borders_wei_id"].ToString().Length > 0 ? dr["dr_borders_wei_id"].ToString() : "";
                    head.DrDropperLayoutsWeiId  = dr["dr_dropper_layouts_wei_id"].ToString().Length > 0 ? dr["dr_dropper_layouts_wei_id"].ToString() : "";
                    head.DrEditReasonsWeiId     = dr["dr_edit_reasons_wei_id"].ToString().Length > 0 ? dr["dr_edit_reasons_wei_id"].ToString() : "";
                    head.DrFabricTypesWeiId     = dr["dr_fabric_types_wei_id"].ToString().Length > 0 ? dr["dr_fabric_types_wei_id"].ToString() : "";
                    head.DrFabricsWeiId         = dr["dr_fabrics_wei_id"].ToString().Length > 0 ? dr["dr_fabrics_wei_id"].ToString() : "";
                    head.DrImpacCodesWeiId      = dr["dr_impac_codes_wei_id"].ToString().Length > 0 ? dr["dr_impac_codes_wei_id"].ToString() : "";
                    head.DrProductClassesWeiId  = dr["dr_product_classes_wei_id"].ToString().Length > 0 ? dr["dr_product_classes_wei_id"].ToString() : "";
                    head.DrProductTypeWeiId     = dr["dr_product_type_wei_id"].ToString().Length > 0 ? dr["dr_product_type_wei_id"].ToString() : "";
                    head.DrShapesName           = dr["dr_shapes_name"].ToString().Length > 0 ? dr["dr_shapes_name"].ToString() : "";
                    head.DrShapesSizesWeiId     = dr["dr_shapes_sizes_wei_id"].ToString().Length > 0 ? dr["dr_shapes_sizes_wei_id"].ToString() : "";
                    head.DrShapesStandard       = dr["dr_shapes_standard"].ToString().Length > 0 ? dr["dr_shapes_standard"].ToString() : "";
                    head.DrStitchFonts1WeiId    = dr["dr_stitch_fonts1_wei_id"].ToString().Length > 0 ? dr["dr_stitch_fonts1_wei_id"].ToString() : "";
                    head.DrStitchFonts2WeiId    = dr["dr_stitch_fonts2_wei_id"].ToString().Length > 0 ? dr["dr_stitch_fonts2_wei_id"].ToString() : "";
                    head.DrStitchFonts3WeiId    = dr["dr_stitch_fonts3_wei_id"].ToString().Length > 0 ? dr["dr_stitch_fonts3_wei_id"].ToString() : "";
                    head.DroperLayoutId         = dr["DroperLayoutId"].ToString().Length > 0 ? dr["DroperLayoutId"].ToString() : "";
                    head.DrshId                 = dr["drsh_id"].ToString().Length > 0 ? dr["drsh_id"].ToString() : "";
                    head.DrsiId                 = dr["drsi_id"].ToString().Length > 0 ? dr["drsi_id"].ToString() : "";
                    head.EditLock               = Convert.ToBoolean(dr["EditLock"].ToString().Length > 0 ? Convert.ToBoolean(dr["EditLock"].ToString()) : false);
                    head.EditReason2Id          = dr["EditReason2Id"].ToString().Length > 0 ? dr["EditReason2Id"].ToString() : "";
                    head.EditReasonComments     = dr["EditReasonComments"].ToString().Length > 0 ? dr["EditReasonComments"].ToString() : "";
                    head.EditReasonId           = dr["EditReasonId"].ToString().Length > 0 ? dr["EditReasonId"].ToString() : "";
                    head.ElectronicSimulation   = Convert.ToBoolean(dr["ElectronicSimulation"].ToString().Length > 0 ? Convert.ToBoolean(dr["ElectronicSimulation"].ToString()) : false);
                    head.EmailConfirmation      = dr["EmailConfirmation"].ToString().Length > 0 ? dr["EmailConfirmation"].ToString() : "";

                    head.EmailsElectronicSimulation = dr["EmailsElectronicSimulation"].ToString().Length > 0 ? dr["EmailsElectronicSimulation"].ToString() : "";

                    head.EmbellishmentGroupsDs  = dr["embellishment_groups_ds"].ToString().Length > 0 ? dr["embellishment_groups_ds"].ToString() : "";
                    head.EmbellishmentGroupId   = dr["EmbellishmentGroupId"].ToString().Length > 0 ? dr["EmbellishmentGroupId"].ToString() : "";

                    head.EmbellishmentGroupRefId = dr["EmbellishmentGroupRefId"].ToString().Length > 0 ? dr["EmbellishmentGroupRefId"].ToString() : "";

                    head.EmployeeId             = dr["EmployeeId"].ToString().Length > 0 ? dr["EmployeeId"].ToString() : "";
                    head.EmployeeUserName       = dr["EmployeeUserName"].ToString().Length > 0 ? dr["EmployeeUserName"].ToString() : "";
                    head.ErrorDescription       = dr["ErrorDescription"].ToString().Length > 0 ? dr["ErrorDescription"].ToString() : "";
                    head.ErrorFlag              = Convert.ToBoolean(dr["ErrorFlag"].ToString().Length > 0 ? Convert.ToBoolean(dr["ErrorFlag"].ToString()) : false);
                    head.ExpectedShipDate       = dr["ExpectedShipDate"].ToString().Length > 0 ? DateTime.Parse(dr["ExpectedShipDate"].ToString()) : DateTime.Now;
                    head.ExportDr               = Convert.ToBoolean(dr["export_dr"].ToString().Length > 0 ? Convert.ToBoolean(dr["export_dr"].ToString()) : false);
                    head.ExportStatus           = dr["ExportStatus"].ToString().Length > 0 ? dr["ExportStatus"].ToString() : "";
                    head.Fabric2Id              = dr["Fabric2Id"].ToString().Length > 0 ? dr["Fabric2Id"].ToString() : "";
                    head.FabricId               = dr["FabricId"].ToString().Length > 0 ? dr["FabricId"].ToString() : "";
                    head.FabricType2Id          = dr["FabricType2Id"].ToString().Length > 0 ? dr["FabricType2Id"].ToString() : "";
                    head.FabricTypeId           = dr["FabricTypeId"].ToString().Length > 0 ? dr["FabricTypeId"].ToString() : "";
                    head.FavoritesAudience      = dr["FavoritesAudience"].ToString().Length > 0 ? dr["FavoritesAudience"].ToString() : "";
                    head.FavoritesCreateDate    = dr["FavoritesCreateDate"].ToString().Length > 0 ? DateTime.Parse(dr["FavoritesCreateDate"].ToString()) : DateTime.Now;
                    head.FavoritesNickName      = dr["FavoritesNickName"].ToString().Length > 0 ? dr["FavoritesNickName"].ToString() : "";
                    head.FormNm                 = dr["form_nm"].ToString().Length > 0 ? dr["form_nm"].ToString() : "";
                    head.GarmentPlacementsWeiId = dr["garment_placements_wei_id"].ToString().Length > 0 ? dr["garment_placements_wei_id"].ToString() : "";
                    head.GarmentPlacementId     = dr["GarmentPlacementId"].ToString().Length > 0 ? dr["GarmentPlacementId"].ToString() : "";
                    head.GarmentType            = dr["GarmentType"].ToString().Length > 0 ? dr["GarmentType"].ToString() : "";
                    head.GarmentTypeId          = dr["GarmentTypeId"].ToString().Length > 0 ? dr["GarmentTypeId"].ToString() : "";
                    head.GarmentTypeName        = dr["GarmentTypeName"].ToString().Length > 0 ? dr["GarmentTypeName"].ToString() : "";

                    head.HasConfirmationEmailBeenSent = Convert.ToBoolean(dr["HasConfirmationEmailBeenSent"].ToString().Length > 0 ? Convert.ToBoolean(dr["HasConfirmationEmailBeenSent"].ToString()) : false);

                    head.HasDRBeenProcessed     = Convert.ToBoolean(dr["HasDRBeenProcessed"].ToString().Length > 0 ? Convert.ToBoolean(dr["HasDRBeenProcessed"].ToString()) : false);
                    head.Height                 = dr["Height"].ToString().Length > 0 ? dr["Height"].ToString() : "";
                    head.ImpacCodeId            = dr["ImpacCodeId"].ToString().Length > 0 ? dr["ImpacCodeId"].ToString() : "";
                    head.ImpacCodes             = dr["ImpacCodes"].ToString().Length > 0 ? dr["ImpacCodes"].ToString() : "";
                    head.ItemsToDecorate        = dr["ItemsToDecorate"].ToString().Length > 0 ? dr["ItemsToDecorate"].ToString() : "";
                    head.KnowSize               = Convert.ToBoolean(dr["KnowSize"].ToString().Length > 0 ? Convert.ToBoolean(dr["KnowSize"].ToString()) : false);
                    head.LatestRecord           = Convert.ToBoolean(dr["LatestRecord"].ToString().Length > 0 ? Convert.ToBoolean(dr["LatestRecord"].ToString()) : false);
                    head.LegalAcceptTerms       = Convert.ToBoolean(dr["LegalAcceptTerms"].ToString().Length > 0 ? Convert.ToBoolean(dr["LegalAcceptTerms"].ToString()) : false);
                    head.LegalAgreement         = dr["LegalAgreement"].ToString().Length > 0 ? dr["LegalAgreement"].ToString() : "";
                    head.LocationChange         = Convert.ToBoolean(dr["LocationChange"].ToString().Length > 0 ? Convert.ToBoolean(dr["LocationChange"].ToString()) : false);
                    head.MaintainProportions    = dr["MaintainProportions"].ToString().Length > 0 ? dr["MaintainProportions"].ToString() : "";
                    head.Misc                   = dr["Misc"].ToString().Length > 0 ? dr["Misc"].ToString() : "";
                    head.MobileAppDesign        = dr["MobileAppDesign"].ToString().Length > 0 ? dr["MobileAppDesign"].ToString() : "";
                    // head.MwDesignRequestOrderId = (Int32)(dr["MwDesignRequestOrderId"].ToString().Length > 0 ? Int32.Parse(dr["MwDesignRequestOrderId"].ToString()) : 0);
                    //head.MwOrderMasterId = (Int32)(dr["MwOrderMasterId"].ToString().Length > 0 ? Int32.Parse(dr["MwOrderMasterId"].ToString()) : 0);
                    head.Optional1              = dr["Optional1"].ToString().Length > 0 ? dr["Optional1"].ToString() : "";
                    head.Optional2              = dr["Optional2"].ToString().Length > 0 ? dr["Optional2"].ToString() : "";
                    head.Optional3              = dr["Optional3"].ToString().Length > 0 ? dr["Optional3"].ToString() : "";
                    head.Optional4              = dr["Optional4"].ToString().Length > 0 ? dr["Optional4"].ToString() : "";
                    head.Optional5              = dr["Optional5"].ToString().Length > 0 ? dr["Optional5"].ToString() : "";
                    head.Optional6              = dr["Optional6"].ToString().Length > 0 ? dr["Optional6"].ToString() : "";
                    head.Optional7              = dr["Optional7"].ToString().Length > 0 ? dr["Optional7"].ToString() : "";
                    head.Optional8              = dr["Optional8"].ToString().Length > 0 ? dr["Optional8"].ToString() : "";
                    head.Optional10             = dr["Optional10"].ToString().Length > 0 ? dr["Optional10"].ToString() : "";
                    head.OptionalArtFont1       = dr["OptionalArtFont1"].ToString().Length > 0 ? dr["OptionalArtFont1"].ToString() : "";
                    head.OptionalArtFont2       = dr["OptionalArtFont2"].ToString().Length > 0 ? dr["OptionalArtFont2"].ToString() : "";
                    head.OptionalArtFont3       = dr["OptionalArtFont3"].ToString().Length > 0 ? dr["OptionalArtFont3"].ToString() : "";
                    head.OptionalArtText1       = dr["OptionalArtText1"].ToString().Length > 0 ? dr["OptionalArtText1"].ToString() : "";
                    head.OptionalArtText2       = dr["OptionalArtText2"].ToString().Length > 0 ? dr["OptionalArtText2"].ToString() : "";
                    head.OptionalArtText3       = dr["OptionalArtText3"].ToString().Length > 0 ? dr["OptionalArtText3"].ToString() : "";
                    head.ParentDesignRequestId  = dr["ParentDesignRequestId"].ToString().Length > 0 ? dr["ParentDesignRequestId"].ToString() : "";
                    head.ParentSKU              = dr["ParentSKU"].ToString().Length > 0 ? dr["ParentSKU"].ToString() : "";
                    head.PlacementId            = dr["PlacementId"].ToString().Length > 0 ? dr["PlacementId"].ToString() : "";
                    head.PONumber               = dr["PONumber"].ToString().Length > 0 ? dr["PONumber"].ToString() : "";
                    head.Price                  = dr["Price"].ToString().Length > 0 ? dr["Price"].ToString() : "";
                    head.PricingDescription     = dr["PricingDescription"].ToString().Length > 0 ? dr["PricingDescription"].ToString() : "";
                    head.PrintedDate            = dr["PrintedDate"].ToString().Length > 0 ? DateTime.Parse(dr["PrintedDate"].ToString()) : DateTime.Now;
                    head.Priority               = (Decimal)(dr["Priority"].ToString().Length > 0 ? Decimal.Parse(dr["Priority"].ToString()) : 0);
                    head.ProductClass           = dr["ProductClass"].ToString().Length > 0 ? dr["ProductClass"].ToString() : "";
                    head.ProductClassId         = dr["ProductClassId"].ToString().Length > 0 ? dr["ProductClassId"].ToString() : "";
                    head.ProductType            = dr["ProductType"].ToString().Length > 0 ? dr["ProductType"].ToString() : "";
                    head.ProductTypeId          = dr["ProductTypeId"].ToString().Length > 0 ? dr["ProductTypeId"].ToString() : "";
                    head.RecordType             = dr["RecordType"].ToString().Length > 0 ? dr["RecordType"].ToString() : "";
                    head.Remake                 = dr["Remake"].ToString().Length > 0 ? dr["Remake"].ToString() : "";
                    head.RequestFor             = dr["RequestFor"].ToString().Length > 0 ? dr["RequestFor"].ToString() : "";
                    head.Requestor              = dr["Requestor"].ToString().Length > 0 ? dr["Requestor"].ToString() : "";
                    head.SafetyStripesId        = dr["SafetyStripesId"].ToString().Length > 0 ? dr["SafetyStripesId"].ToString() : "";
                    
                    head.SalesRepEmail          = dr["SalesRepEmail"].ToString().Length > 0 ? dr["SalesRepEmail"].ToString() : "";

                    head.SampleShippedNotificationSent = Convert.ToBoolean(dr["SampleShippedNotificationSent"].ToString().Length > 0 ? Convert.ToBoolean(dr["SampleShippedNotificationSent"].ToString()) : false);

                    head.SampleShippedSent      = dr["SampleShippedSent"].ToString().Length > 0 ? DateTime.Parse(dr["SampleShippedSent"].ToString()) : DateTime.Now;
                    head.SapCustomerName        = dr["SapCustomerName"].ToString().Length > 0 ? dr["SapCustomerName"].ToString() : "";
                    head.SapCustomerNo          = dr["SapCustomerNo"].ToString().Length > 0 ? dr["SapCustomerNo"].ToString() : "";

                    head.SendDRConfirmationEmail = dr["SendDRConfirmationEmail"].ToString().Length > 0 ? dr["SendDRConfirmationEmail"].ToString() : "";

                    head.SessionId              = dr["SessionId"].ToString().Length > 0 ? dr["SessionId"].ToString() : "";
                    head.Shapes                 = dr["Shapes"].ToString().Length > 0 ? dr["Shapes"].ToString() : "";
                    head.ShapeSize              = dr["ShapeSize"].ToString().Length > 0 ? dr["ShapeSize"].ToString() : "";
                    head.ShipAddress1           = dr["ShipAddress1"].ToString().Length > 0 ? dr["ShipAddress1"].ToString() : "";
                    head.ShipAddress2           = dr["ShipAddress2"].ToString().Length > 0 ? dr["ShipAddress2"].ToString() : "";
                    head.ShipAddress3           = dr["ShipAddress3"].ToString().Length > 0 ? dr["ShipAddress3"].ToString() : "";
                    head.ShipEmailAddress1      = dr["ShipEmailAddress1"].ToString().Length > 0 ? dr["ShipEmailAddress1"].ToString() : "";
                    head.ShipEmailAddress2      = dr["ShipEmailAddress2"].ToString().Length > 0 ? dr["ShipEmailAddress2"].ToString() : "";
                    head.ShipEmailAddress3      = dr["ShipEmailAddress3"].ToString().Length > 0 ? dr["ShipEmailAddress3"].ToString() : "";

                    head.ShippingAddressesAddress1Addedit   = (Int64)(dr["shipping_addresses_address_1_addedit"].ToString().Length > 0 ? Int64.Parse(dr["shipping_addresses_address_1_addedit"].ToString()) : 0);
                    head.ShippingAddressesAddress1Code      = dr["shipping_addresses_address_1_code"].ToString().Length > 0 ? dr["shipping_addresses_address_1_code"].ToString() : "";
                    head.ShippingAddressesAddress2Addedit   = (Int64)(dr["shipping_addresses_address_2_addedit"].ToString().Length > 0 ? Int64.Parse(dr["shipping_addresses_address_2_addedit"].ToString()) : 0);
                    head.ShippingAddressesAddress2Code      = dr["shipping_addresses_address_2_code"].ToString().Length > 0 ? dr["shipping_addresses_address_2_code"].ToString() : "";
                    head.ShippingAddressesAddress3Addedit   = (Int64)(dr["shipping_addresses_address_3_addedit"].ToString().Length > 0 ? Int64.Parse(dr["shipping_addresses_address_3_addedit"].ToString()) : 0);
                    head.ShippingAddressesAddress3Code      = dr["shipping_addresses_address_3_code"].ToString().Length > 0 ? dr["shipping_addresses_address_3_code"].ToString() : "";
                    head.ShippingMethodsShipVia1            = dr["shipping_methods_ship_via_1"].ToString().Length > 0 ? dr["shipping_methods_ship_via_1"].ToString() : "";
                    head.ShippingMethodsShipVia2            = dr["shipping_methods_ship_via_2"].ToString().Length > 0 ? dr["shipping_methods_ship_via_2"].ToString() : "";
                    head.ShippingMethodsShipVia3            = dr["shipping_methods_ship_via_3"].ToString().Length > 0 ? dr["shipping_methods_ship_via_3"].ToString() : "";

                    head.ShipQty1               = dr["ShipQty1"].ToString().Length > 0 ? dr["ShipQty1"].ToString() : "";
                    head.ShipQty2               = dr["ShipQty2"].ToString().Length > 0 ? dr["ShipQty2"].ToString() : "";
                    head.ShipQty3               = dr["ShipQty3"].ToString().Length > 0 ? dr["ShipQty3"].ToString() : "";
                    head.ShipViaMethod1         = dr["ShipViaMethod1"].ToString().Length > 0 ? dr["ShipViaMethod1"].ToString() : "";
                    head.ShipViaMethod2         = dr["ShipViaMethod2"].ToString().Length > 0 ? dr["ShipViaMethod2"].ToString() : "";
                    head.ShipViaMethod3         = dr["ShipViaMethod3"].ToString().Length > 0 ? dr["ShipViaMethod3"].ToString() : "";
                    head.SizeShapeId            = dr["SizeShapeId"].ToString().Length > 0 ? dr["SizeShapeId"].ToString() : "";
                    head.Sku                    = dr["Sku"].ToString().Length > 0 ? dr["Sku"].ToString() : "";
                    head.SpecialInstructions    = dr["SpecialInstructions"].ToString().Length > 0 ? dr["SpecialInstructions"].ToString() : "";
                    head.Status                 = dr["Status"].ToString().Length > 0 ? dr["Status"].ToString() : "";
                    head.StitchFont1Id          = dr["StitchFont1Id"].ToString().Length > 0 ? dr["StitchFont1Id"].ToString() : "";
                    head.StitchFont2Id          = dr["StitchFont2Id"].ToString().Length > 0 ? dr["StitchFont2Id"].ToString() : "";
                    head.StitchFont3Id          = dr["StitchFont3Id"].ToString().Length > 0 ? dr["StitchFont3Id"].ToString() : "";
                    head.SubmittedByAIdCusNo    = dr["submitted_by_a_id_cus_no"].ToString().Length > 0 ? dr["submitted_by_a_id_cus_no"].ToString() : "";
                    head.SubmittedByAccountId   = dr["SubmittedByAccountId"].ToString().Length > 0 ? dr["SubmittedByAccountId"].ToString() : "";
                    head.SubmittedByCustomerId  = dr["SubmittedByCustomerId"].ToString().Length > 0 ? dr["SubmittedByCustomerId"].ToString() : "";
                    head.SyndicatedAcctName     = dr["syndicated_acct_name"].ToString().Length > 0 ? dr["syndicated_acct_name"].ToString() : "";
                    head.SyndicatedAccountNo    = dr["SyndicatedAccountNo"].ToString().Length > 0 ? dr["SyndicatedAccountNo"].ToString() : "";
                    head.SyndicatedSitename     = dr["SyndicatedSitename"].ToString().Length > 0 ? dr["SyndicatedSitename"].ToString() : "";
                    head.SyndicatedUsername     = dr["SyndicatedUsername"].ToString().Length > 0 ? dr["SyndicatedUsername"].ToString() : "";
                    head.ThreadInkColors        = dr["ThreadInkColors"].ToString().Length > 0 ? dr["ThreadInkColors"].ToString() : "";
                    head.TrackingNumber1        = dr["TrackingNumber1"].ToString().Length > 0 ? dr["TrackingNumber1"].ToString() : "";
                    head.TrackingNumber2        = dr["TrackingNumber2"].ToString().Length > 0 ? dr["TrackingNumber2"].ToString() : "";
                    head.TrackingNumber3        = dr["TrackingNumber3"].ToString().Length > 0 ? dr["TrackingNumber3"].ToString() : "";
                    head.TrackingXml            = dr["TrackingXml"].ToString().Length > 0 ? dr["TrackingXml"].ToString() : "";
                    head.WECustomerNo           = dr["WECustomerNo"].ToString().Length > 0 ? dr["WECustomerNo"].ToString() : "";
                    head.WeiDrId                = (Decimal)(dr["wei_dr_id"].ToString().Length > 0 ? Decimal.Parse(dr["wei_dr_id"].ToString()) : 0);
                    head.Width                  = dr["Width"].ToString().Length > 0 ? dr["Width"].ToString() : "";
                    //
                    ord.CatalogDesignRequest = head;
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return ord;
        }
        
    }
}
