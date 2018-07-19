using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class ActivityLogBO
    {
        public int MwActivityLogId { get; set; }                    //  ID
        //public string service { get; set; }                         //  prefix
        public ActivityLogLevels ActivityLogLevel { get; set; }
        public ActivitySource ActivitySourceId { get; set; }
        public ActivityLogTypes ActivityLogType { get; set; }       //  type
        public OrderTypes OrderTypeId { get; set; }
        public string Filename { get; set; }                        //  table
        public string OrderId { get; set; }
        public string Description { get; set; }                     //  comment
        public string ExceptionData { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }  //  eventDate
    }
    ///
    /// IMPORTANT - Note that this table of enums must be synched up with the ActivityLogType table
    ///
    public enum ActivityLogLevels
    {
        Info = 1,
        Error = 2
    }

    public enum ActivityLogTypes
    {
        GetData = 1,
        InsertIntoMiddleware = 2,
        UpdateCatalog = 3
    }

    public enum ActivitySource
    {
        DesignRequestOrder = 1,
        WebOrder = 2,
        CatalogAccountSync = 3,
        CatalogCustomerSync = 4,
        CatalogShippingSync = 5,
        CatalogShippingMethodSync = 6,
        CatalogProductsSync = 7,
        DFS = 8,
        ArrowFTPOrderImport = 9,
        ImportProcessNis = 10,
        ImportProcessGK = 11,
        ImportProcessArrowFtp = 12,
        ImportProcessNBI = 13,
        NBIShippingTracking = 14,
        NavImportNBITest = 15,
        NavImportStickersTest = 16,
        NisNAVTest = 17,
        NisLateOrders = 18
    }


    public enum OrderTypes
    {
        NameSystem = 1,
        NBI = 2,
        WeiCatalog = 3,
        DR = 4,
        GKFTP = 5,
        GKEmail = 6,
        ArrowFTP = 7,
        ArrowEmail = 8,
        NixonFTP = 9,
        Fax = 10,
        Phone = 11,
        Email = 12,
        MagentoWeb = 13,
        Convert = 14,
        Stickers=15
    }

}
