using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class WebOrder
    {
        //  order summary
        public NBIHeader nbiOrderSummary { get; set; }
        //  Item
        public List<WebOrderItem> nbiOrderItems { get; set; }
    }

    public class NBIHeader
    {
        //  Order
        public string   OrderType           { get; set; }                   //  Apr19-17 (nbi)
        public string   OrderId             { get; set; }
        public string   OrderNumber         { get; set; }                   //  new
        //  Account
        //public string AccountNum { get; set; }
        public string   AccountId           { get; set; }
        public string   AccountNum          { get; set; }                   //  new
        //  Locale
        public string   Language            { get; set; }
        public string   Currency            { get; set; }
        public string   SiteUsed            { get; set; }
        //  Profile 
        public string   VatNum              { get; set; }                   //  new
        public string   Credit              { get; set; }                   //  new
        //  Contact (Main)
        public string   Email               { get; set; }
        public string   Title               { get; set; }
        public string   Forename            { get; set; }
        public string   Surname             { get; set; }
        public string   CompanyName         { get; set; }
        public string   Industry            { get; set; }
        public string   Source              { get; set; }                   //  new
        public string   Telephone           { get; set; }
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
        public string   AddressDeliveryTown  { get; set; }
        public string   AddressDeliveryCounty { get; set; }
        public string   AddressDeliveryPostCode { get; set; }
        public string   AddressDeliveryCountry { get; set; }
        public string   AddressDeliveryCompany { get; set; }
        //  Cart
        public Decimal  CartPrice           { get; set; }
        //  Discount
        public string   DiscountType        { get; set; }
        public string   DiscountCode        { get; set; }
        public Decimal  DiscountTotal       { get; set; }
        public Decimal  DiscountValue       { get; set; }
        //  Delivery
        public Decimal  DeliveryTotal       { get; set; }
        public string   DeliveryType        { get; set; }
        public string   DeliveryTerritory   { get; set; }
        public int      DeliveryId          { get; set; }                 //  int
        public DateTime DeliveryEtd         { get; set; }                 //  new
        public DateTime DeliveryEta         { get; set; }                 //  new
        //  Vat
        public string   VatRate             { get; set; }
        public string   VatLiveRate         { get; set; }
        public string   VatStaticRate       { get; set; }
        public string   VatCity             { get; set; }
        public string   VatCounty           { get; set; }
        public string   VatState            { get; set; }
        public string   VatStateRate        { get; set; }
        public string   VatCountyRate       { get; set; }
        public string   VatCityRate         { get; set; }
        public string   VatDistrictRate     { get; set; }
        public string   VatCountyCode       { get; set; }
        public Decimal  VatTotal            { get; set; }
        public int      VatNumber           { get; set; }                 //  int
        //  Options        
        public string   OptionsCustRef      { get; set; }
        public string   OptionsBlindPacking { get; set; }
        //  Payment
        public string   PaymentUsing        { get; set; }
        public string   PaymentCardType     { get; set; }                 //  Apr13-17
        public string   PaymentLastFour     { get; set; }                 //  Apr13-17
        public string   PaymentCurrency     { get; set; }
        public Decimal  PaymentAmount       { get; set; }
        public string   GrossCurrency       { get; set; }
        public Decimal  GrossAmount         { get; set; }
        //
        public string   Filename            { get; set; }
    }

    public class WebOrderItem
    {
        public string   ItemCode            { get; set; }
        public string   ItemVinyl           { get; set; }
        public string   ItemBar             { get; set; }
        public string   ItemFitting         { get; set; }
        public int      ItemLines           { get; set; }
        public int      ItemQuantity        { get; set; }
        public string   ItemPpu             { get; set; }
        public string   ItemDesignId        { get; set; }
        public int      ItemId              { get; set; }                 //  int
        public string   ItemStyleNum        { get; set; }
        public string   ItemSimilar         { get; set; }
        public string   ItemIdentical       { get; set; }
        public string   ItemSku             { get; set; }
        public string   ItemProof           { get; set; }
        //
        public List<ItemNames> names        { get; set; }
    }

    public class ItemNames
    {
        public string   ItemName1           { get; set; }
        public string   ItemName2           { get; set; }
        public string   ItemName3           { get; set; }
        public string   ItemName4           { get; set; }
        public string   ItemName5           { get; set; }
        public string   ItemName6           { get; set; }
        public string   ItemName7           { get; set; }
    }
}