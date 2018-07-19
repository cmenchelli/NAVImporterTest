using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class WEFiles
    {
        public string MacolaUserID { get; set; }
        public string UserEmail { get; set; }
        public string OrderDate { get; set; }
        public string PO { get; set; }
        public string ImportType { get; set; }
        public int TotalItemsNo { get; set; }
        public List<WEItems> OrderDetail { get; set; }
    }

    public class WEItems
    {
        public string Qty { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string ItemClientCode { get; set; }
        public string ItemClientCodeDescription { get; set; }
        public string ItemWECode { get; set; }
        public string PriorityID { get; set; }
        public string Type { get; set; }
        public string Instructions1 { get; set; }
        public string Instructions2 { get; set; }
        public string Instructions3 { get; set; }
        public string Instructions4 { get; set; }
    }
}