using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class ProblemFiles
    {
        public int ID { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string FileNumber { get; set; }
        public string JsonData { get; set; }
        public DateTime CreatedDate { get; set; }
        public Boolean toReimport { get; set; }
        public DateTime ReimportDate { get; set; }
        public string ReimportFileNumber { get; set; }
    }
}
