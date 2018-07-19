using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class StkAccount
    {
        public string OrderType { get; set; }                       //   Apr19-17 (stickers)   
        //public string AccountNum { get; set; }                                      
        public string AccountId { get; set; }
        //public DateTime TimeStamp { get; set; }
        public string Language { get; set; }                        //  LocaleLang
        public string Currency { get; set; }
        public string SiteUsed { get; set; }
        //public string ProfileCredit { get; set; }                 //  profile credit
        //public string VatNum { get; set; }                        //  vatnum
        //  Contact - Account info
        public string Email { get; set; }
        public string Title { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string Telephone { get; set; }
        //  Address - Cardholder type
        public string AddressCardHolderLine1 { get; set; }
        public string AddressCardHolderLine2 { get; set; }
        public string AddressCardHolderTown { get; set; }
        public string AddressCardHolderCounty { get; set; }
        public string AddressCardHolderPostCode { get; set; }
        public string AddressCardHolderCountry { get; set; }
        //  Address - Delivery
        public string AddressDeliveryForename { get; set; }
        public string AddressDeliverySurname { get; set; }
        public string AddressDeliveryTelephone { get; set; }
        public string AddressDeliveryLine1 { get; set; }
        public string AddressDeliveryLine2 { get; set; }
        public string AddressDeliveryTown { get; set; }
        public string AddressDeliveryCounty { get; set; }
        public string AddressDeliveryPostCode { get; set; }
        public string AddressDeliveryCountry { get; set; }
        public string AddressDeliveryCompany { get; set; }
        public string FileName { get; set; }
    }
}