using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class NISLateOrders
    {
        public int      ID          { get; set; }
        public string   UserId      { get; set; }
        public string   Email       { get; set; }
        public DateTime OrderDate   { get; set; }
        public DateTime DateReq     { get; set; }
        public string   PO          { get; set; }
        public DateTime SentDate    { get; set; }
        public int      Items       { get; set; }
        public string   SON         { get; set; }
        public DateTime CutOff      { get; set; }
        public DateTime Imported    { get; set; } 
    }
}
