using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class StkOrder
    {
        public StkHeader stkOrderSummary { get; set; }              //  order summary
        public List<StkOrderItem> stkOrderItems { get; set; }       //  Item
    }

    public class StkHeader
    {
        public string OrderType { get; set; }
        //  Order
        public string   OrderNumber { get; set; }      
        public string   OrderId { get; set; }                        
        //  Account
        public string   AccountNum { get; set; }        
        public string   AccountId { get; set; }                       
        //  Locale
        public string   Language { get; set; }
        public string   SiteUsed { get; set; }
        public string   Currency { get; set; }        
        //  Profile 
        public string   VatNum { get; set; }                        
        public string   Credit { get; set; }                       
        //  Contact (Main)
        public string   Email { get; set; }
        public string   Title { get; set; }
        public string   Forename { get; set; }
        public string   Surname { get; set; }
        public string   CompanyName { get; set; }
        public string   Industry { get; set; }
        public string   Source { get; set; }                                  
        public string   Telephone { get; set; }
        //  Address (Card holder / Delivery)
        public string   AddressCardHolderLine1 { get; set; }
        public string   AddressCardHolderLine2 { get; set; }
        public string   AddressCardHolderTown { get; set; }
        public string   AddressCardHolderCounty { get; set; }
        public string   AddressCardHolderPostCode { get; set; }
        public string   AddressCardHolderCountry { get; set; }
        //public Address AddressDelivery { get; set; }
        public string   AddressDeliveryForename { get; set; }
        public string   AddressDeliverySurname { get; set; }
        public string   AddressDeliveryTelephone { get; set; }
        public string   AddressDeliveryLine1 { get; set; }
        public string   AddressDeliveryLine2 { get; set; }
        public string   AddressDeliveryTown { get; set; }
        public string   AddressDeliveryCounty { get; set; }
        public string   AddressDeliveryPostCode { get; set; }
        public string   AddressDeliveryCountry { get; set; }
        //  Cart
        public Decimal  CartPrice { get; set; }
        //  Discount
        public string DiscountType { get; set; }
        public string DiscountCode { get; set; }
        public Decimal DiscountTotal { get; set; }
        public Decimal DiscountValue { get; set; }
        //  Delivery
        public string   DeliveryType { get; set; }
        public Decimal  DeliveryTotal { get; set; }        
        public string   DeliveryTerritory { get; set; }        
        public DateTime DeliveryEtd { get; set; }                 
        public DateTime DeliveryEta { get; set; }                  
        public int      DeliveryId { get; set; }                  
        //  Vat
        public Decimal  VatTotal { get; set; }
        public Decimal  VatStaticRate { get; set; }
        public Decimal  VatStateRate { get; set; }
        public string   VatState { get; set; }    
        public Decimal  VatRate { get; set; }
        public int      VatNumber { get; set; }
        public Decimal  VatLiveRate { get; set; }
        public Decimal  VatDistrictRate { get; set; }
        public Decimal  VatCountyRate { get; set; }
        public string   VatCountyCode { get; set; }
        public string   VatCounty { get; set; }
        public Decimal  VatCityRate { get; set; }
        public string   VatCity { get; set; }
        //  Options        
        public string   OptionsCustRef { get; set; }
        public string   OptionsBlindPacking { get; set; }
        //  Payment
        public string   PaymentUsing { get; set; }
        public string   PaymentCurrency { get; set; }
        public Decimal  PaymentAmount { get; set; }
        public string   GrossCurrency { get; set; }
        public Decimal  GrossAmount { get; set; }
        //
        public string Filename { get; set; }
    }

    public class StkOrderItem
    {
        //  item
        public string   ItemProof { get; set; }             //
        public string   ItemWhiteBehind { get; set; }       //
        public Decimal  ItemVsize { get; set; }             //
        public string   ItemStyleNum { get; set; }          //
        public string   ItemSimilar { get; set; }           //
        public string   ItemShape { get; set; }             //
        public string   ItemReversePrint { get; set; }      //
        public int      ItemQuantity { get; set; }          //
        public string   ItemPrintedWhite { get; set; }      //
        public string   ItemPpu { get; set; }               //
        public int      ItemNumHoles { get; set; }          //
        public string   ItemMaterialOption { get; set; }    //
        public string   ItemMaterial { get; set; }          //
        public int      ItemLines { get; set; }             //
        public int      ItemId { get; set; }                //
        public string   ItemIdentical { get; set; }         //
        public string   ItemSku { get; set; }               //  Jun23
        public Decimal  ItemHsize { get; set; }             //
        public string   ItemDesignId { get; set; }          //
        public string   ItemCornerRad { get; set; }         //
        public string   ItemColour { get; set; }            //
        public string   ItemCode { get; set; }              //
        public int      ItemSheets { get; set; }            //  Jun 26
        public int      ItemsPerSheet { get; set; }         //  Jun 29
    }   
}