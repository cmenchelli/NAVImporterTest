using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportProcedure_GK_PO.TextFiles
{
    public interface ITextFiles
    {
        bool ProcessWEFiles(string path, string fileName, EtlTimer sync);
        bool ProcessGKFiles(string path, string fileName, EtlTimer sync);
    }

    public class ImportProcess : ITextFiles
    {
        private int LineNo = 0;
        private string SeqNo = string.Empty;
        private int Qty = 0;
        private int EmblemPrice = 0;
        /// 
        //readonly WebServiceManager wsm = new WebServiceManager();   //  Set WebService methods pointer   
        DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();  //  Set WebService methods pointer

        /// <summary>
        ///     Process WE text files, built an object, generate a Json string and submit it to the 
        ///     webServices. [ WE XML Order.txt ]
        ///     If any processing errors are detected, the corresponding message is send to WebServices 
        /// </summary>
        /// <param name="path">The complete path to the file in process</param>
        /// <param name="fileName">File name without the extension</param>
        /// <returns>Process result in a boolean var. if OK (true) else (false) </returns>
        public bool ProcessWEFiles(string path, string fileName, EtlTimer sync)
        {
            bool head = false;        /// Header info detection flag
            bool det = false;        /// Detail info detection flag
            //bool resp = false;        /// Process final status return flag
            ServiceResponse resp = new ServiceResponse();
            bool process = true;         /// control that process was completed ok
            ///
            int itemsCounter = 0;       /// Detail items counter
            int lineCounter = 0;        /// Count the number of rows(records) in file
            ///
            string line;
            string[] separators = { "=", "_" };
            ///
            WEFiles weFiles = new WEFiles();
            List<WEItems> items = new List<WEItems>();
            //WriteErrorFile wef = new WriteErrorFile();
            DAL.SupportServices.SupportServices wef = new DAL.SupportServices.SupportServices();
            /// ---------------------------------------------------------------------------
            /// Read the file line by line.
            /// ---------------------------------------------------------------------------
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            try
            {
                while ((line = file.ReadLine()) != null)
                {
                    /// Count lines in file
                    lineCounter++;
                    /// Split input record into string array elements
                    string[] words = line.Split(separators, StringSplitOptions.None);
                    /// -------------------------------------------------------------------
                    /// Only one element found in record <element 0>
                    /// Identify is Header or detail tags come after this element
                    /// -------------------------------------------------------------------
                    if (words.Count() == 1)
                    {
                        if (words[0] == "[Order Header]")
                        {
                            head = true;        /// Set header section detected flag on
                            continue;
                        }
                        else if (words[0] == "[Order Detail]")
                        {
                            det = true;         /// set Deatil section detected flag on
                            head = false;       /// Header detected off
                            continue;
                        }
                        else if (words[0] == "")
                        {
                            continue;           /// Ignore blank records
                        }
                        throw new Exception("Unrecognized content");
                    }
                    /// -------------------------------------------------------------------
                    /// Two elements found in record <tag name> = <tag value>
                    /// Header flag on. validate against Header tags
                    /// Is not a detail record
                    /// -------------------------------------------------------------------
                    if (head && !det && words.Count() == 2)
                    {
                        /// Build Header object from data file 
                        switch (words[0])
                        {
                            case "MacolaUserID":
                                weFiles.MacolaUserID = words[1];
                                break;

                            case "UserEmail":
                                weFiles.UserEmail = words[1].TrimStart();
                                break;

                            case "OrderDate":
                                weFiles.OrderDate = words[1].TrimStart();
                                break;

                            case "PO":
                                weFiles.PO = words[1].TrimStart();
                                break;

                            case "ImportType":
                                weFiles.ImportType = words[1].TrimStart();
                                break;

                            case "TotalItemsNo":
                                if (!string.IsNullOrEmpty(words[1]))
                                    weFiles.TotalItemsNo = Int32.Parse(words[1]);
                                break;

                            default:
                                //  throw error
                                throw new Exception("Unrecognized header tag name");
                        }
                    }
                    /// -------------------------------------------------------------------
                    /// Three elements found in record:
                    ///     <tag name> = <detail record count> = <tag value>
                    /// Detail flag on. validate against detail tags
                    /// -------------------------------------------------------------------
                    else if (det && words.Count() == 3)
                    {
                        int itm = Int32.Parse(words[1]);
                        if (itm > itemsCounter)
                        {
                            /// Add new empty() line object into Items list
                            /// All new data entry values will be stored in the empty line.
                            /// ============= >
                            items.Add(AddWEItemToList());
                            /// < =============
                            /// Count items in List
                            itemsCounter++;
                        }
                        switch (words[0])
                        {
                            case "Qty":
                                items[itemsCounter - 1].Qty = words[2];
                                break;
                            case "Line1":
                                items[itemsCounter - 1].Line1 = words[2];
                                break;
                            case "Line2":
                                items[itemsCounter - 1].Line2 = words[2];
                                break;
                            case "Line3":
                                items[itemsCounter - 1].Line3 = words[2];
                                break;
                            case "ItemClientCode":
                                items[itemsCounter - 1].ItemClientCode = words[2];
                                break;
                            case "ItemClientCodeDescription":
                                items[itemsCounter - 1].ItemClientCodeDescription = words[2];
                                break;
                            case "ItemWECode":
                                items[itemsCounter - 1].ItemWECode = words[2];
                                break;
                            case "PriorityID":
                                items[itemsCounter - 1].PriorityID = words[2];
                                break;
                            case "Type":
                                items[itemsCounter - 1].Type = words[2];
                                break;
                            case "Instructions1":
                                items[itemsCounter - 1].Instructions1 = words[2];
                                break;
                            case "Instructions2":
                                items[itemsCounter - 1].Instructions2 = words[2];
                                break;
                            case "Instructions3":
                                items[itemsCounter - 1].Instructions3 = words[2];
                                break;
                            case "Instructions4":
                                items[itemsCounter - 1].Instructions4 = words[2];
                                break;
                            default:
                                //  throw error
                                throw new Exception("Unrecognized detail tag name");
                        }
                    }
                    else
                    {
                        /// throw error - unrecognized data line (lineCounter)
                        throw new Exception("Invalid data format");
                    }
                }
            }
            catch (Exception e)
            {
                /// error while reading the file
                process = false;
                resp.IsOk = false;
                ServiceResponse errMsg = new ServiceResponse();
                errMsg.FileType = "1";      //  Set Nis Type
                errMsg.FileName = fileName + ".txt";
                errMsg.NISOrderId = "0";
                errMsg.Status = "Not Processed";
                errMsg.Message = e.Message + " in line " + lineCounter;
                /// Send Message to WebService
                wef.WriteErrorFile(errMsg, sync);
            }
            file.Close();
            /// Only if process is completed ok and the number of detail records expected 
            /// match the number of records read the object is serialized and send to the
            /// corresponding Web Service
            if (process)
            {
                if (items.Count() == itemsCounter)
                {
                    weFiles.OrderDetail = items;
                    resp.IsOk = true;
                }
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(weFiles);
                /// ******************************************************************************
                /// Call Web Service
                /// <param name="json2">Include Header serialized + items seialized info.</param> 
                /// <param name="fname">File to process name </param>
                /// <param name="type">File Type, determine what service to use (url)</param>
                /// <returns>"rStatus" true if WebService was processed, else false</returns>
                /// Web service url is defined in the App.Config file
                /// ------------------------------------------------------------------------------
                ///                 
                resp = wsm.ConsumeWebService(json, fileName, "WEOrders", sync);
                //  <==============  
            }
            return resp.IsOk;
        }

        /// <summary>
        ///     Add additional new item lines to the Order Detail list, 
        /// </summary>
        private static WEItems AddWEItemToList()
        {
            WEItems newItem = new WEItems();
            return newItem;
        }

        /// <summary>
        ///     G&K Text files provide orders header and items information, each data line distribution [ PO_GK00.....txt ]
        ///     (data type and positioning) will depend in the type of record which is identified by tags
        ///     in the first position of each readed line.
        ///     Data is separated with , * or "," characters.
        ///     This process split data line into an string array and use the elements within the array 
        ///     to retreive the corresponding fields. (they will depend in the line type in process)
        /// </summary>
        /// Old system ==> subImportTXTFile / subImportGKOrders
        public bool ProcessGKFiles(string path, string fileName, EtlTimer sync)
        {
            DAL.SupportServices.SupportServices wef = new DAL.SupportServices.SupportServices();

            //  bool resp = false;
            ServiceResponse resp = new ServiceResponse();
            string[] separators = { " , ", "*", "", "" };
            ///
            string line;
            bool newFile = false;
            //bool newLine = true;
            GKHeader order = new GKHeader();
            List<OrderItems> list = new List<OrderItems>();
            /// ------------------------------------------------------------------------------         
            /// Read the file and display it line by line.
            /// ------------------------------------------------------------------------------
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(path);
                while ((line = file.ReadLine()) != null)
                {
                    line = line.Replace('"', ' ').Replace('-', ' ');
                    ///
                    string[] words = line.Split(separators, StringSplitOptions.None);
                    if (words[0] == "BG")
                    {
                        newFile = true;
                    }
                    string tag = words[0].TrimStart();
                    /// ------------------------------------------------------------------------------- 
                    /// Process header information
                    /// -------------------------------------------------------------------------------
                    if ((tag == "AAA0") || (tag == "POH0") || (tag == "POH1") && (newFile))
                    {
                        switch (tag)
                        {
                            case "AAA0":
                                string date = words[2].Substring(0, 4) + "-" + words[2].Substring(4, 2) + "-" + words[2].Substring(6, 2) + " " +
                                              words[3].Substring(0, 2) + ":" + words[3].Substring(2, 2) + ":" + words[3].Substring(4, 2); // ok
                                order.OrderDate = DateTime.ParseExact(date, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
                                break;

                            case "POH0":
                                order.PO = words[1];            //  ok
                                order.UserID = words[2];        //  ok
                                order.DateReq = words[3].Substring(0, 4) + "-" + words[3].Substring(4, 2) + "-" + words[3].Substring(6, 2); //  ok
                                break;

                            case "POH1":
                                order.ID = order.PO;            //  ok
                                order.ShippingToID = "";
                                order.Description = words[1];   //  ok
                                order.UserEmail = words[2];     //  ok
                                order.UserID = "";
                                if (order.UserID == "0096")
                                {
                                    order.UserID = "0596";
                                }
                                if ((order.UserID == "000000302871") || (order.UserID == "000000302863") || (order.UserID == "000000306101")
                                 || (order.UserID == "000000316854") || (order.UserID == "000000316891"))
                                {
                                    order.ShippingViaID = "PL";
                                }
                                else
                                {
                                    order.ShippingViaID = "10";
                                }
                                //
                                break;

                            default:
                                break;
                        }
                    }
                    /// -------------------------------------------------------------------------------
                    /// Process order items tags
                    /// -------------------------------------------------------------------------------
                    else if ((tag == "POD0") && (newFile))
                    {
                        //newLine = true;
                        ///
                        LineNo = Int32.Parse(words[1]);
                        SeqNo = words[3];
                        Qty = Int32.Parse(words[6]);
                        EmblemPrice = Int32.Parse(words[7]);
                    }
                    else if ((tag == "POD1") && (newFile))
                    {
                        //complete = true;
                        //newLine = true;
                        OrderItems ordItms = BuildPod0(words, order.UserID);
                        list.Add(ordItms);
                    }
                    /// If this is not a valid file - terminate the process
                    if (!newFile)
                    {
                        throw new Exception();
                    }
                }
                order.items = list;

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(order);
                //Console.WriteLine("Text process ok");
                //Console.ReadLine();
                file.Close();
                /// ******************************************************************************
                /// Call Web Service
                /// <param name="json2">Include Header serialized + items seialized info.</param> 
                /// <param name="fname">File to process name </param>
                /// <param name="type">File Type, determine what service to use (url)</param>
                /// <param name="pStatus">Data status</param> Ignore
                /// <returns>"rStatus" true if WebService was processed, else false</returns>
                /// Web service url is defined in the App.Config file
                /// ------------------------------------------------------------------------------
                ///                 
                resp = wsm.ConsumeWebService(json, fileName, "GKOrders", sync);
                //  <==============  
            }
            catch (Exception e)
            {
                /// Process errors
                //Console.WriteLine("Text process error");
                //Console.ReadLine();

                /// error while reading the file
                //process = false;
                resp.IsOk = false;
                ServiceResponse errMsg = new ServiceResponse();
                errMsg.FileType = "1";      //  Set Nis Type
                errMsg.FileName = fileName + ".txt";
                errMsg.NISOrderId = "0";
                errMsg.Status = "Not Processed";
                errMsg.Message = e.Message + " in line "; // +lineCounter;
                /// Send Message to WebService
                wef.WriteErrorFile(errMsg, sync);
            }

            return resp.IsOk;
        }

        /// <summary>
        ///     Build order items from text file - each entry under POD0 will generate a new order item
        ///     entry. Data positioning within the order item line (POD0) depend on Specific users id's 
        ///     or product types.
        /// </summary>
        /// <param name="words">The complete input line</param>
        /// <param name="UserId">User id, comming from the header</param>
        /// <returns>OrderItem line</returns>
        private OrderItems BuildPod0(string[] words, string UserId)
        {
            OrderItems ordItems = new OrderItems();
            ///
            ordItems.DesignNo = words[1];
            /// The default product type (when data field is empty or null) is 05
            if (string.IsNullOrEmpty(words[4]))
                words[4] = "05";
            ordItems.ProdType = words[4];

            if (UserId == "000000302863" || UserId == "000000306101" || UserId == "000000316854" || UserId == "000000316891")
            {
                /// Select data sources (position) for specific user id's.
                ordItems.DesignFont = words[5];
                ordItems.ImpacCode = words[6];
                ordItems.ThreadColor2 = words[7];
                ordItems.Txt1 = words[8];
                ordItems.Txt2 = words[9];
                ordItems.Txt3 = words[10];
                ordItems.Placement = words[12];
                ordItems.GarmentDesc = words[14];
                if (ordItems.ThreadColor2 == " ")
                    ordItems.ThreadColor2 = words[16];
                ordItems.DesignSize = words[20];
            }
            else
            {
                /// Data sources (position) for all other user id's, addditional source
                /// position selection apply depending on product type 
                if (ordItems.ProdType == "05")
                {
                    /// Data distribution that apply for products type 05
                    ordItems.DesignFont = words[11];
                    ordItems.Placement = words[12];
                    ordItems.DesignNo = words[13];
                    ordItems.GarmentDesc = words[14];
                    ordItems.ThreadColor2 = words[16];
                    ordItems.Txt1 = words[17];
                    ordItems.Txt2 = words[18];
                    ordItems.Txt3 = words[19];
                }
                else
                {
                    /// Data distribution that apply for all other products types
                    ordItems.DesignFont = words[5];
                    ordItems.ImpacCode = words[6];
                    ordItems.ThreadColor2 = words[7];
                    ordItems.Txt1 = words[8];
                    ordItems.Txt2 = words[9];
                    ordItems.Txt3 = words[10];
                    ordItems.Font2 = words[11];
                    ordItems.Placement = words[12];
                    ordItems.DesignSize = words[20];
                }
            }
            ordItems.Gkson = words[21];
            /// Common line fields
            ordItems.LineNo = LineNo;
            ordItems.SeqNo = SeqNo;
            ordItems.Qty = Qty;
            ordItems.EmblemPrice = EmblemPrice;

            return ordItems;
        }
    }
}
