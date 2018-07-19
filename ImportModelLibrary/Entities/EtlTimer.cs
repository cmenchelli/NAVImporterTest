using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class EtlTimer
    {
        public int MwEtlTimerId { get; set; }
        public string Description { get; set; }
        public string ServiceName { get; set; }
        public bool IsActive { get; set; }
        //
        public Nullable<long> FrequencyInSecs { get; set; }
        public bool IsRunning { get; set; }
        public TimeSpan DailyRunTime { get; set; }
        public DateTime LastRunDate { get; set; }
        //
        public bool IsApiRunning { get; set; }
        public string WebServiceRaw { get; set; }
        public string WebServiceErrors { get; set; }
        //
        public string WebServiceOrders { get; set; }
        public string WebServiceAccounts { get; set; }
        public string WebServiceDesigns { get; set; }
        //
        public string InputFileFolder { get; set; }
        public string ExcelFileFolder { get; set; }
        public string ProcessedFileFolder { get; set; }
        public string ProblemFileFolder { get; set; }
        //  used only for data transport among functions 
        //  Not coming from data set
        public int OrderType { get; set; }
    }
}