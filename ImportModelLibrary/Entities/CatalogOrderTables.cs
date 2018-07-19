using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class CatalogOrderTables
    {
        public CatalogWebOrderHeader CatalogWebOrderSummary { get; set; }
        public List<CatalogWebOrderItem> CatalogWebOrderItems { get; set; }
    }

    public class CatalogWebOrderHeader
    {
        public string   AccountId { get; set; }
        public string   AffiliateId { get; set; }                   //  AffiliateID  
        public bool?    AllowBo { get; set; }                       //  allow_bo
        public string   Attention { get; set; }
        public string   BdIdInvoice { get; set; }                   //  bd_id_invoice
        public string   BdIdPacking { get; set; }                   //  bd_id_packing
        public string   BillShippingAccountNumber { get; set; }
        public string   BoPreference { get; set; }                  //  bo_preference
        public string   BrowserName { get; set; }
        public decimal? Bvb { get; set; }                           //  bvb
        public string   CNm { get; set; }                          //  c_nm
        public string   CatWebOrderId { get; set; }
        public string   Comment { get; set; }
        public string   CountyName { get; set; }
        public string   CountyName1 { get; set; }
        public decimal? CountyTax { get; set; }
        public string   CountyTaxId { get; set; }
        public string   CouponAccountingType { get; set; }
        public decimal? CouponAmount { get; set; }
        public string   CouponName { get; set; }
        public decimal? CouponSubtotalProducts { get; set; }
        public decimal? CouponSubtotalShipping { get; set; }
        public string   CurrencyId { get; set; }
        public string   CustomerAddress1 { get; set; }
        public string   CustomerAddress2 { get; set; }
        public string   CustomerCellular { get; set; }
        public string   CustomerCity { get; set; }
        public string   CustomerCompany { get; set; }
        public string   CustomerCountry { get; set; }
        public string   CustomerCounty { get; set; }
        public string   CustomerEmail { get; set; }
        public string   CustomerEmailCC { get; set; }
        public string   CustomerFax { get; set; }
        public string   CustomerFirstName { get; set; }
        public string   CustomerLastName { get; set; }
        public string   CustomerPager { get; set; }
        public string   CustomerPhone { get; set; }
        public string   CustomerState { get; set; }
        public string   CustomerZip { get; set; }
        public string   Data { get; set; }
        public DateTime? DateCreated { get; set; }
        public long?    EmailFlags { get; set; }
        public string   EmailSentFlag { get; set; }
        public string   ErpHoldReason { get; set; }
        public string   ErpOrderStatus { get; set; }
        public string   ExportFlag { get; set; }
        public string   ExternalOrderNumber { get; set; }
        public long?    Flags { get; set; }
        public string   FTransaction { get; set; }                      //  ftransaction
        public decimal? Fvb { get; set; }                               //  fvb
        public string   GeneralLedgerAccountId { get; set; }
        public decimal  IndexCounter { get; set; }
        public string   IPAddress { get; set; }
        public string   Legal { get; set; }
        public string   LegalResponse { get; set; }
        public int?     MwOrderMasterId { get; set; }
        public int      MwWebOrderId { get; set; }
        public bool?    NewOrderEmailSent { get; set; }
        public decimal? OBalance { get; set; }                          //  o_balance
        public bool?    OipFlagSet { get; set; }                        //  oip_flag_set
        public string   Optional1 { get; set; }
        public string   Optional2 { get; set; }
        public string   Optional3 { get; set; }
        public string   Optional4 { get; set; }
        public string   Optional5 { get; set; }
        public string   Optional6 { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? OrderGrandTotal { get; set; }
        //public List<WebOrderCatalogItem> OrderItemList { get; set; }
        public decimal? OrderNumber { get; set; }
        public string   OrdersCcRemoved { get; set; }                   //  orders_ccremoved
        public string   OrderSource { get; set; }
        public string   OrderStatus { get; set; }
        public string   OrderType { get; set; }
        public string   OrderXml { get; set; }
        public decimal? PaidByOtherAmount { get; set; }
        public string   PaymentMethod { get; set; }
        public string   PcgNm { get; set; }                             //  pcg_nm
        public string   PcsNm { get; set; }                             //  pcs_nm
        public string   PONumber { get; set; }
        public decimal? ProductTotal { get; set; }
        public decimal? ProductTotalWithShipping { get; set; }
        public string   SEm { get; set; }                               //  s_em
        public string   SNm { get; set; }                               //  s_nm
        public string   SalesRep { get; set; }
        public string   ScgId { get; set; }                             //  scg_id
        public string   ServerName { get; set; }
        public string   ShipAddress1 { get; set; }
        public string   ShipAddress2 { get; set; }
        public string   ShipAddress3 { get; set; }
        public string   ShipCellular { get; set; }
        public string   ShipCity { get; set; }
        public string   ShipCompany { get; set; }
        public string   ShipCountry { get; set; }
        public string   ShipFax { get; set; }
        public string   ShipFirstName { get; set; }
        public string   ShipLastName { get; set; }
        public int?     Shipment { get; set; }
        public string   ShipPager { get; set; }
        public string   ShipPhone { get; set; }
        public string   ShippingAddressCode { get; set; }
        public decimal? ShippingNetTotal { get; set; }
        public decimal? ShippingTotal { get; set; }
        public string   ShipState { get; set; }
        public string   ShipViaName { get; set; }
        public string   ShipViaType { get; set; }
        public string   ShipZip { get; set; }
        public decimal? SpendAllowanceAfter { get; set; }
        public decimal? SpendAllowanceBefore { get; set; }
        public decimal? SpendAllowanceUsed { get; set; }
        public decimal? StTotal { get; set; }                   //  st_total
        public string   Status { get; set; }
        public string   StatusReason { get; set; }
        public string   SubAffiliateID { get; set; }
        public string   TaxCalculationtype { get; set; }
        public string   Taxdetail { get; set; }
        public bool?    TaxExempt { get; set; }
        public string   TaxIdNumber { get; set; }
        public string   TaxMatchFrom { get; set; }
        public string   TaxProfileLog { get; set; }
        public decimal? TaxSubtotalHandling { get; set; }
        public decimal? TaxSubtotalProducts { get; set; }
        public decimal? TaxSubtotalShipping { get; set; }
        public decimal? TaxTotal { get; set; }
        public string   Temp { get; set; }                      // temp
        public string   Terms { get; set; }
        public byte[]   Time { get; set; }
        public decimal? TxTotal { get; set; }
        public string   Type { get; set; }
        public string   Username { get; set; }
        public decimal? Vol1 { get; set; }                      //  Vol1
        public decimal? Vol10 { get; set; }                     //  Vol10
        public decimal? Vol2 { get; set; }                      //  Vol2
        public decimal? Vol3 { get; set; }                      //  Vol3
        public decimal? Vol4 { get; set; }                      //  Vol4
        public decimal? Vol5 { get; set; }                      //  Vol5
        public decimal? Vol6 { get; set; }                      //  Vol6
        public decimal? Vol7 { get; set; }                      //  Vol7
        public decimal? Vol8 { get; set; }                      //  Vol8
        public decimal? Vol9 { get; set; }                      //  Vol9
        public string   WarehouseSelection { get; set; }
    }

    public class CatalogWebOrderItem
    {
        //public WebOrderCatalogItem();

        public decimal?     Bo { get; set; }                    //  bo
        public DateTime?    BoDate { get; set; }                //  bo_date
        public decimal?     Bogus { get; set; }                 //  bogus
        public decimal?     Bvb { get; set; }                   //  bvb
        public string       CartOption { get; set; }
        public string       CatWebOrderId { get; set; }
        public string       CatWebOrderItemId { get; set; }
        public decimal?     Cost { get; set; }
        public decimal?     CouponAmount { get; set; }
        public DateTime?    DateCreated { get; set; }
        public string       DecId { get; set; }                 //  dec_id
        public string       DecKeys { get; set; }               //  deckeys
        public string       DisplayType { get; set; }
        public string       EmbType { get; set; }               //  emb_type
        public decimal?     ESellerBonus { get; set; }          //  eSellerBonus
        public decimal?     ESellerPrice { get; set; }          //  eSellerPrice
        public decimal?     ESellerSetupPrice { get; set; }     //  eSellerSetupPrice
        public DateTime?    ExpectedDate { get; set; }
        public decimal?     ExtTotal { get; set; }              //  ext_total
        public long?        Flags { get; set; }
        public string       ForNm { get; set; }                 //  for_nm
        public string       Frequency { get; set; }
        public decimal?     Fvb { get; set; }                   //  fvb
        public string       GarmentPlacementId { get; set; }
        public string       GeneralLedgerAccountId { get; set; }
        public string       Instance { get; set; }
        public bool?        IsCsg { get; set; }                 //  is_csg
        public decimal?     MinimumPrice { get; set; }
        public decimal?     MinimumSetupPrice { get; set; }
        public string       Mqty { get; set; }                  //  mqty
        public int?         MwWebOrderId { get; set; }
        public int          MwWebOrderItemId { get; set; }
        public decimal?     NScId { get; set; }                 //  n_sc_id
        public string       NmAlias { get; set; }               //  nm_alias
        public string       Notes { get; set; }
        public string       OUnq { get; set; }                  //  u_ung
        public string       Optional1 { get; set; }
        public string       Optional2 { get; set; }
        public string       Optional3 { get; set; }
        public string       Optional4 { get; set; }
        public string       Optional5 { get; set; }
        public string       Optional6 { get; set; }
        public string       Optional7 { get; set; }
        public string       Optional8 { get; set; }
        public decimal      OrderDetailNumber { get; set; }
        public long?        OrderFlags { get; set; }
        public string       PRecordType { get; set; }           //  p_record_type
        public string       PcgId { get; set; }                 //  pcg_id
        public string       PONumber { get; set; }
        public string       PpbNm { get; set; }                 //  ppb_nm
        public string       PppNm { get; set; }                 //  ppp_nm
        public string       PptNm { get; set; }                 //  ppt_nm
        public decimal?     PriceBeforeMarkup { get; set; }
        public string       PriceCalculationDescription { get; set; }
        public decimal?     PricePaid { get; set; }
        public string       ProductCategoryStyleId { get; set; }
        public string       ProductId { get; set; }
        public string       ProductName { get; set; }
        public string       ProductUnit { get; set; }
        public string       PsgInstance { get; set; }           //  psg_instance
        public decimal?     QtyAvailable { get; set; }
        public decimal?     Quantity { get; set; }
        public string       RPAdded { get; set; }               //  r_p_added
        public string       RPType { get; set; }                //  r_p_type
        public decimal?     RequestedQty { get; set; }
        public string       RequestedSku { get; set; }
        public decimal?     RequestedUnitPrice { get; set; }
        public decimal?     RetailPrice { get; set; }
        public decimal?     RetailSetupPrice { get; set; }
        public decimal?     ReturnRec { get; set; }             //  return_rec
        public decimal?     ReturnReq { get; set; }             //  return_req
        public DateTime?    RowAdded { get; set; }
        public decimal?     SPrice { get; set; }                //  s_price
        public int?         SetSize { get; set; }
        public decimal?     SetupCost { get; set; }
        public decimal?     SetupRetailPrice { get; set; }
        public decimal?     Shipped { get; set; }                //  shiped
        public decimal?     ShippedPrice { get; set; }
        public string       Sku { get; set; }
        public string       SkuAlias { get; set; }
        public string       Status { get; set; }                //  status
        public bool?        SysDirtyPublic { get; set; }        //  sys_dirty_public
        public DateTime?    SysLastModifiedDatePublic { get; set; }     //  sys_last_modified_date_public
        public decimal?     TaxRatePercent { get; set; }        //  tax_rate_percent
        public decimal?     TaxableExtTotal { get; set; }       //  taxable_ext_total
        public string       TbpPaid { get; set; }               //  tbp_paid
        public string       TrackingXml { get; set; }
        public decimal?     TxAmount { get; set; }              //  tx_amount
        public string       Type { get; set; }
        public string       Unq { get; set; }                   //  ung
        public decimal?     UomConversion { get; set; }         //  uom_conversion
        public string       UomNm { get; set; }                 //  uom_nm
        public decimal?     UomStockPricePer { get; set; }      //  uom_stock_price_per
        public decimal?     UomStockQty { get; set; }           //  uom_stock_qty
        public string       UomType { get; set; }               //  uom_type
        public decimal?     Vol1 { get; set; }
        public decimal?     Vol10 { get; set; }
        public decimal?     Vol2 { get; set; }
        public decimal?     Vol3 { get; set; }
        public decimal?     Vol4 { get; set; }
        public decimal?     Vol5 { get; set; }
        public decimal?     Vol6 { get; set; }
        public decimal?     Vol7 { get; set; }
        public decimal?     Vol8 { get; set; }
        public decimal?     Vol9 { get; set; }
    }

    //
    public class CatalogConvertTable
    {
        public CatalogConvertOrderHeader CatalogConvertOrderSummary { get; set; }
    }

    public class CatalogConvertOrderHeader
    {
        //public ConvertOrderBO();

        public string       AccountNumber { get; set; }
        public string       ActionTaken { get; set; }
        public long?        AddEditFlag { get; set; }
        public string       CatConvertOrderId { get; set; }
        public string       CreateIPAddress { get; set; }
        public string       CreateUserId { get; set; }
        public DateTime?    CreationDate { get; set; }
        public string       CustomerEmailAddress { get; set; }
        public DateTime?    DateCreated { get; set; }
        public string       DesignId { get; set; }
        public int?         DirtyFlags { get; set; }
        public string       EmailAddresses { get; set; }
        public long?        EmailFlag { get; set; }
        public decimal?     FileExists { get; set; }
        //public int          MwConvertOrderId { get; set; }      //
        //public int?         MwOrderMasterId { get; set; }       //
        public string       Optional1 { get; set; }
        public string       Optional2 { get; set; }
        public string       Optional3 { get; set; }
        public string       PONumber { get; set; }
        public decimal?     Quantity { get; set; }
        public string       ReferenceId { get; set; }
        public bool?        ShipBlind { get; set; }
        public string       ShipAddressLine1 { get; set; }
        public string       ShipAddressLine2 { get; set; }
        public string       ShipCity { get; set; }
        public string       ShipCompany { get; set; }
        public string       ShipCountry { get; set; }
        public string       ShipEmailAddress { get; set; }
        public string       ShipFirstName { get; set; }
        public string       ShipId { get; set; }
        public string       ShipLastname { get; set; }
        public string       ShipName { get; set; }
        public string       ShipPhoneNumber { get; set; }
        public string       ShipState { get; set; }
        public decimal?     ShipVia { get; set; }
        public string       ShipViaCode { get; set; }
        public string       ShipZip { get; set; }
        public string       Sku { get; set; }
        public byte[]       Timestamp { get; set; }
        public string       UserInitials { get; set; }
    }
}