using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class ImportServicesLog
    {
        public int ID { get; set; }
        public string Source { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public DateTime DateProcessed { get; set; }
    }
}
