using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class ServiceResponse
    {
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string NISOrderId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string Request { get; set; }
        public Boolean IsOk { get; set; }
    }
}