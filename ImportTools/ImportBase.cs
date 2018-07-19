using DAL.ImportControl;
using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace ImportTools
{
    /// <summary>
    ///     Version 2.0 Feb13-17
    ///     Must replace serviceBase from WEMWImportSrvices
    ///     This new version use only the etlTimer table content to control service process intervals
    ///     
    /// </summary>
    public partial class ImportBase : ServiceBase
    {
        //private bool newEntry = false;
        private bool testFlag = false;
        // readonly ImportControlRepository accCtrl = new ImportControlRepository();
        DAL.ImportControl.ImportControlRepository wtf = new DAL.ImportControl.ImportControlRepository();
        DAL.ImportControl.ImportNISRepository inr = new DAL.ImportControl.ImportNISRepository();
        DAL.ImportControl.ImportCatalogRepository icr = new DAL.ImportControl.ImportCatalogRepository();
        DAL.ImportControl.ImportDesignRequestRepository idr = new DAL.ImportControl.ImportDesignRequestRepository();

        //  Retreive service id for this service instance.
        int serviceId = Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]);
        //  int iter = 0;
        EtlTimer syncRow = new EtlTimer();

        public ImportBase()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //  this.WriteToFile("NAV Service Test started {0}");
            if (syncRow.MwEtlTimerId == 0)
            {
                syncRow = wtf.getImportControl(serviceId);     
            }
            //string ret = wtf.writeSyncLog(1, serviceId, 1, syncRow.ServiceName, "==> Window service for: " + syncRow.ServiceName + " started ");
            string ret = wtf.writeSyncLog(1, syncRow.MwEtlTimerId, 1, syncRow.ServiceName, "==> Window service for: " + syncRow.ServiceName + " started ");
            this.ScheduleService();
        }

        protected override void OnStop()
        {
            string ret = wtf.writeSyncLog(1, serviceId, 1, syncRow.ServiceName, "<== Windows service for: " + syncRow.ServiceName + " stopped at ");
            //  this.WriteToFile("NAV Service Test stopped {0}");
            this.Schedular.Dispose();
        }

        /// <summary>
        ///     THis entry only apply when the project is defined as console application,
        ///    thie is used to test the application.       
        /// </summary>
        /// <param name="args"></param>
        public void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.WriteLine("NAV Service Test started");
            Console.WriteLine("NAV Service Test: type s to stop:");
            string resp = Console.ReadLine();
            if (resp == "s")
                this.OnStop();
        }

        private Timer Schedular;

        /// <summary>
        ///     Version 2.0 Feb13-17
        ///     This version ignore interval modes and times from app config, instead retreive that information from the etlTimer table.
        ///     etlTimer information is retreived the first time in the OnStrat method, from them on, the etlTimer information is retreived
        ///     
        /// </summary>
        public void ScheduleService()
        {
            string ret = string.Empty;      //  Logging request response
            ret = wtf.writeSyncLog(1, syncRow.MwEtlTimerId, 1, syncRow.ServiceName, "<== Schedule service active for: " + syncRow.ServiceName);
            try
            {
                Schedular = new Timer(new TimerCallback(SchedularCallback));
                //Set the Default Time.
                DateTime scheduledTime = DateTime.MinValue;
                //
                if (syncRow.FrequencyInSecs != null)
                {   //Get the Interval in seconds from etlTimer Table.
                    UInt32 intervalSeconds = Convert.ToUInt32(syncRow.FrequencyInSecs);
                    ret = wtf.writeSyncLog(1, syncRow.MwEtlTimerId, 1, syncRow.ServiceName, "<== Windows service (interval mode time) for" + syncRow.ServiceName + ": " + intervalSeconds.ToString());
                    //  this.WriteToFile("NAV Service Test (interval mode) " + intervalSeconds.ToString() + " at {0}");
                    //  Set the Scheduled Time by adding the Interval to Current Time.
                    scheduledTime = DateTime.Now.AddSeconds(intervalSeconds);
                    if (DateTime.Now > scheduledTime)
                    {   //If Scheduled Time is passed set Schedule for the next Interval.
                        scheduledTime = scheduledTime.AddSeconds(intervalSeconds);
                    }
                }
                else if (syncRow.DailyRunTime != null)
                {   //Get the Scheduled Time from etlTimer.
                    scheduledTime = DateTime.Parse(syncRow.DailyRunTime.ToString());
                    if (DateTime.Now > scheduledTime)
                    {   //If Scheduled Time is passed set Schedule for the next day.
                        scheduledTime = scheduledTime.AddDays(1);
                    }
                    ret = wtf.writeSyncLog(1, serviceId, 1, syncRow.ServiceName, "<== Windows service (Daily mode time) for" + syncRow.ServiceName + ": " + scheduledTime.ToString().ToString());
                    //  this.WriteToFile("NAV Service Test (Daily mode) " + scheduledTime.ToString() + " at {0}");
                }
                //
                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                string schedule = string.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                //  this.WriteToFile("NAV Service Test scheduled to run after: " + schedule + " {0}");

                //Get the difference in milliseconds between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);
                //  Change the Timer's Due Time.
                Schedular.Change(dueTime, Timeout.Infinite);
                /// Console testing procedure - override time interval to allow test.
                if (Environment.UserInteractive && !testFlag)
                {
                    testFlag = true;
                    object e = new object();
                    SchedularCallback(e);
                }
            }
            catch (Exception ex)
            {
                int res = wtf.updImportControl(serviceId, 0);     //  set EtlTimer for this service to not Running (isRunning = false)
                ret = wtf.writeSyncLog(1, serviceId, 1, syncRow.ServiceName, "<== Windows service Error for" + syncRow.ServiceName + ": " + ex.Message + ex.StackTrace);
                //  WriteToFile("NAV Service Test Error on: {0} " + ex.Message + ex.StackTrace);                
                using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController(syncRow.ServiceName))
                {   //Stop the Windows Service.
                    //serviceController.Stop();
                }
            }
        }

        /// <summary>
        ///     Version Feb13-17
        ///     Execute method when timer is due 
        ///     Must retreive etlTimer information to validate if service is active / running (busy)
        ///     Validate the following for each of the services:       
        ///     1 - The service exist in the EtlTimer table.
        ///     2 - The service is active.
        ///     3 - The service is not currently busy running another thread.
        /// </summary>
        /// <param Object name="e"></param>False
        private void SchedularCallback(object e)
        {   //  Get etlTimer info. for this service
            syncRow = wtf.getImportControl(serviceId);
            //
            
            if (syncRow.IsActive)
            {
                if (syncRow.IsRunning)
                {   //  Ignore process if service is busy performing another thread
                    wtf.writeSyncLog(9, syncRow.MwEtlTimerId, 1, syncRow.ServiceName, "Sync service for " + syncRow.ServiceName + " not processed, is busy in other thread.");
                    //  this.WriteToFile("NAV Service Log - not processed, service is busy in other thread: {0}");
                }
                else
                {   //  Input Files services is performed here.
                    //  SyncControl();      /// Version 2.0
                    //  SyncControl(syncRow);   /// Version 3.0   
                    if (syncRow.MwEtlTimerId == 17 || syncRow.MwEtlTimerId == 1 || syncRow.MwEtlTimerId == 2 || syncRow.MwEtlTimerId == 18)
                    {   //  Input files comming from db tables
                        SyncControl_Disk(syncRow);   /// Version 3.1   
                    }      
                    else
                    {   //  Input files comming from windows folders
                        SyncControl(syncRow);   /// Version 3.1   
                    }
                }
            }
            else
            {   //  Ignore process when service is not active.
                wtf.writeSyncLog(9, syncRow.MwEtlTimerId, 1, syncRow.ServiceName, "Sync service for " + syncRow.ServiceName + " is not active");
                //  this.WriteToFile("NAV Service Log - not processed, service is inactive: {0}");
            }
            this.ScheduleService();
        }

        //private void WriteToFile(string text)
        //{
        //    string path = "C:\\ServiceLogNAVTest.txt";
        //    using (StreamWriter writer = new StreamWriter(path, true))
        //    {
        //        writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
        //        writer.Close();
        //    }
        //}

        
        /// <summary>   Version 2.0
        ///     Controls all windows services related with the import function.    
        ///     Route the process flow to the corresponding service depending in the process id.
        /// </summary>
        //public void SyncControl()
        //{   //
        //    int res = wtf.updImportControl(serviceId, 1);     //  set EtlTimer for this service to InUse
        //    object e = new object();
        //    //  =================================================================================
        //    if (serviceId == 10)
        //    {
        //        ImportProcedure_NIS.FileManagement fileManagement = new ImportProcedure_NIS.FileManagement();
        //        fileManagement.FileSearch(e, syncRow);
        //    }
        //    else if (serviceId == 11)
        //    {
        //        ImportProcedure_GK_PO.FileManagement fileManagement = new ImportProcedure_GK_PO.FileManagement();
        //        fileManagement.FileSearch(e, syncRow);
        //    }
        //    else if (serviceId == 12)
        //    {
        //        ImportProcedure_ArrowFtp.FileManagement fileManagement = new ImportProcedure_ArrowFtp.FileManagement();
        //        fileManagement.FileSearch(e, syncRow);
        //    }
        //    else if (serviceId == 13)
        //    {
        //        ImportProcedure_NBI.FileManagement fileManagement = new ImportProcedure_NBI.FileManagement();
        //        fileManagement.FileSearch(e, syncRow);
        //    }  
        //    else if (serviceId == 15)
        //    {   /// Nav NBI Import test option
        //        ImportProcedure_NBI.FileManagement fileManagement = new ImportProcedure_NBI.FileManagement();
        //        fileManagement.FileSearch(e, syncRow);
        //    }
        //    res = wtf.updImportControl(serviceId, 0);     //  set EtlTimer for this service to Inactive
            //  ================================================================================
        ///}

        /// ************************************************************************************
        /// Version 3.0 methods start here, they replace the SyncControl Method
        /// All import services share the same input (directory/files) access methods
        /// ------------------------------------------------------------------------------------
        public void SyncControl(EtlTimer sync)
        {            
            List<string> li = new List<string>();
            //
            wtf.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, " > (FileSearch) Start Directory scanning. for " + sync.ServiceName + ".");
            //       
            li.Add(sync.InputFileFolder);
            if (li.Count == 0)
            {
                wtf.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, " > (FileSearch) No Folder to process. for " + sync.ServiceName);
            }
            else 
            {   //  Input files folder found
                foreach (string path in li)
                {
                    if (File.Exists(path))
                    {   // This path is a file     
                        ProcessFile(path, sync);                        
                    }
                    else if (Directory.Exists(path))
                    {   // This path is a directory                    
                        ProcessDirectory(path, sync);
                    }
                    else
                    {   //  Invalid File or Directory exit
                        wtf.writeSyncLog(2, sync.MwEtlTimerId, 1, sync.ServiceName, "(FileSearch) <" + path + " > is not a valid file or directory.");
                    }                
                }               
            }
        }

        //  ************************************************************************
        //  Process all files in the directory passed in, recurse on any directories  
        //  that are found, and process the files they contain. 
        //  ------------------------------------------------------------------------
        public void ProcessDirectory(string targetDirectory, EtlTimer sync)
        {
            int read = 0;
            /// 
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName, sync);
                read = read + 1;
            }
            wtf.writeSyncLog(1, serviceId, 1, sync.ServiceName, " > (FileSearch) " + sync.ServiceName + " Files read: " + read.ToString());   
            // Recurse into subdirectories of this directory. 
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory, sync);
        }

        /// <summary>
        ///     Version 3.0 
        ///     Route process to the corresponding service ProcessFile Method depending on the 
        ///     service id.
        ///     Control the start/end of each service,  
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sync"></param>
        private void ProcessFile(string path, EtlTimer sync)
        {
            int res = wtf.updImportControl(serviceId, 1);     //  set EtlTimer for this service to InUse
            object e = new object();
            //  =================================================================================    
            switch (serviceId)
            {
                case 10:
                    ImportProcedure_NIS.FileManagement FileNis = new ImportProcedure_NIS.FileManagement();
                    FileNis.ProcessFile(path, sync);
                    break;
                case 15:
                    ImportProcedure_NBI.FileManagement FileNbiT = new ImportProcedure_NBI.FileManagement();
                    FileNbiT.ProcessFile(path, sync);
                    break;
                case 16:
                    ImportProcedure_STK.FileManagement FileStkT = new ImportProcedure_STK.FileManagement();
                    FileStkT.ProcessFile(path, sync);
                    break;
            }
            res = wtf.updImportControl(serviceId, 0);     //  set EtlTimer for this service to Inactive
        }
        /// ------------------------------------------------------------------------------------
        /// END OF Version 3.0 methods
        /// ************************************************************************************


        /// ************************************************************************************
        /// Version 3.1 methods start here, they replace the SyncControl Method
        /// All import services share the same input (Db Tables) access methods
        /// ------------------------------------------------------------------------------------
        public void SyncControl_Disk(EtlTimer sync)
        {   //
            wtf.writeSyncLog(1, sync.MwEtlTimerId, 1, sync.ServiceName, " > (TableSearch) Start Directory scanning. for " + sync.ServiceName + ".");
            //
            int testn = 0;
            int order = 1;
            string webOrder = "";
            //
            if (syncRow.MwEtlTimerId == 17)
            {
                /// Search for unimported NIS order (one at the time)
                while (order > 0 ) //  && testn < 5)
                {   //  Get first unimported order from orders table
                    order = inr.getNisOrdersToImport();
                    if (order > 0)
                    {   // a row was found      
                        ProcessRow(order, sync);
                        // testn++;
                    }
                }
            }
            else if (syncRow.MwEtlTimerId == 1)
            {
                /// Get unimported Catalog web orders list (order Id.)
                /// Processs Convert Orders
                var List = idr.getDesignRequest_NotImportedList();
                sync.OrderType = 0;         //  Set order type to convert
                foreach (var item in List)
                {   //  Get first unimported order from orders table    
                    ProcessRow(order, sync, item);
                    testn++;
                }                
            }
            else if (syncRow.MwEtlTimerId == 2)
            {
                /// Get unimported Catalog web orders list (order Id.)
                /// Processs Convert Orders
                var List = icr.getCatalogConvertOrder_NotImportedList();
                sync.OrderType = 1;         //  Set order type to convert
                foreach (var item in List)
                {   //  Get first unimported order from orders table    
                    ProcessRow(order, sync, item);
                    testn++;                    
                }
                //  Process Web Orders
                sync.OrderType = 2;         //  Set order type to Web Orders
                List = icr.getCatalogOrder_NotImportedList();
                foreach (var item in List)	
                {   //  Get first unimported order from orders table  
                    ProcessRow(order, sync, item);
                    testn++;
                }
            }
            else if (syncRow.MwEtlTimerId == 18)
            {
                List<NISLateOrders> orders = new List<NISLateOrders>();
                orders = inr.getNisLateOrders();
                int res = wtf.updImportControl(serviceId, 1);     //  set EtlTimer for this service to InUse
                /// Search for imported NIS order that are late orders (one at the time)
                foreach (var item in orders)
                {   //  Get first imported order from orders table
                    LateOrdersProcedure_NIS.FileManagement LateOrdersRequest = new LateOrdersProcedure_NIS.FileManagement();
                    LateOrdersRequest.ProcessOrder(item, sync);
                    testn++;
                }
                res = wtf.updImportControl(serviceId, 0);     //  set EtlTimer for this service to Inactive
            }
        }

        /// <summary>
        ///     Process orders to import retreived in SyncControl_Disk, earch order will be process individually. 
        ///     Will handle:
        ///     -   Catalog Web Orders and Convert orders (Service Id 2)
        ///     -   Name System Orders (Service Id 17)
        /// </summary>
        /// <param name="order">Order Number</param>
        /// <param name="sync">Import control parameters</param>
        /// <param name="item">Order type (1 - Convert. 2 - web orders), apply only for catalog Web Orders</param>
        private void ProcessRow(int order, EtlTimer sync, string item = "")
        {
            int res = wtf.updImportControl(serviceId, 1);     //  set EtlTimer for this service to InUse
            object e = new object();
            //  =================================================================================       
            if (serviceId == 17)
            {
                ImportProcedure_NIS_V2.FileManagement TableNis = new ImportProcedure_NIS_V2.FileManagement();
                TableNis.ProcessOrder(order, sync);
            }
            else if (serviceId == 1)
            {
                ImportProcedure_DesignRequest.FileManagement TableCatalogDesignRequest = new ImportProcedure_DesignRequest.FileManagement();
                TableCatalogDesignRequest.ProcessOrder(item, sync);
            }
            else if(serviceId == 2)
            {                
                ImportProcedure_Catalog.FileManagement TableCatalogWorkOrder = new ImportProcedure_Catalog.FileManagement();
                TableCatalogWorkOrder.ProcessOrder(item, sync);
            }

            res = wtf.updImportControl(serviceId, 0);     //  set EtlTimer for this service to Inactive
        }
    }
}