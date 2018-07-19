using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class Header
    {
        public string HeaderId { get; set; }
        public string HeaderUserId { get; set; }
        public string HeaderUserEmail { get; set; }
        public string HeaderOrderDate { get; set; }
        public string HeaderPO { get; set; }
        public string HeaderDateRequested { get; set; }
        public string HeaderDescription { get; set; }
        public string HeaderShipVia1 { get; set; }
        public string HeaderShipVia2 { get; set; }
        public string HeaderShipToId1 { get; set; }
        public string HeaderShipToId2 { get; set; }
        public string HeaderComments { get; set; }
        public string FileName { get; set; }
        public string HeaderSentDate { get; set; }
    }

    public class Order
    {
        public string NisOrderId { get; set; }
        public string Itemid { get; set; }
        public string OrderId { get; set; }
        public string SkuId { get; set; }
        public int LineId { get; set; }
        public int Quantity1 { get; set; }
        public int Quantity2 { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string Comment { get; set; }
        public string Description { get; set; }
        public string SkuClient { get; set; }
        public string SkuWe { get; set; }
        public string AccountCode { get; set; }
    }
}
