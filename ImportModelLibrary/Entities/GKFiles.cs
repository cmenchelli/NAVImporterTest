using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class GKHeader
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public string UserEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public string DateReq { get; set; }
        public string PO { get; set; }
        public string Description { get; set; }
        public string ShippingViaID { get; set; }
        public string ShippingViaID2 { get; set; }
        public string ShippingToID { get; set; }
        public string ShippingToID2 { get; set; }
        public string Comments { get; set; }
        public string SentDate { get; set; }
        public string ItemsFileName { get; set; }
        public List<OrderItems> items { get; set; }
        public string AccountCode { get; set; }
    }

    public class OrderItems
    {
        public string ID { get; set; }
        public int LineNo { get; set; }
        public string SeqNo { get; set; }
        public int Qty { get; set; }
        public int EmblemPrice { get; set; }
        public string DesignNo { get; set; }
        public string DesignFont { get; set; }
        public string ImpacCode { get; set; }
        public string Txt1 { get; set; }
        public string Txt2 { get; set; }
        public string Txt3 { get; set; }
        public string DesignSize { get; set; }
        public string Placement { get; set; }
        public string ProdType { get; set; }
        public string Gkson { get; set; }
        public string ThreadColor2 { get; set; }
        public string Font2 { get; set; }
        public string WeSku { get; set; }
        public bool Import_Fg { get; set; }
        public int EmblemPriceWE { get; set; }
        public string Cmt { get; set; }
        public string Dpc { get; set; }
        public string Cpon { get; set; }
        public string Ccon { get; set; }
        public int Priority { get; set; }
        public string GarmentDesc { get; set; }
    }
}
