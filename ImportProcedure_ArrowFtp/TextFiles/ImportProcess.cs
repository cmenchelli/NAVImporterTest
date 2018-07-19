using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportProcedure_ArrowFtp.TextFiles
{
    public interface ITextFiles
    {
        bool ProcessArrowFtpFiles(string path, string fileName, EtlTimer sync);
    }

    public class ImportProcess : ITextFiles
    {
        //private int LineNo = 0;
        private string SeqNo = string.Empty;
        //private int Qty = 0;
        //private int EmblemPrice = 0;
        /// 
        //readonly WebServiceManager wsm = new WebServiceManager();   //  Set WebService methods pointer   
        DAL.SupportServices.SupportServices wsm = new DAL.SupportServices.SupportServices();  //  Set WebService methods pointer

        // <summary>
        ///     Process WE text files, built an object, generate a Json string and submit it to the 
        ///     webServices.
        ///     If any processing errors are detected, the corresponding message is send to WebServices 
        /// </summary>
        /// <param name="path">The complete path to the file in process</param>
        /// <param name="fileName">File name without the extension</param>
        /// <returns>Process result in a boolean var. if OK (true) else (false) </returns>
        public bool ProcessArrowFtpFiles(string path, string fileName, EtlTimer sync)
        {
            bool head = false;    /// Header info detection flag
            bool newIsa = false;    /// Detail info detection flag
            bool item1 = false;    /// Items 1 / 2 sequence control
            //  bool resp = false;    /// Process final status return flag
            ServiceResponse resp = new ServiceResponse();
            bool process = true;     /// control that process was completed ok
            ///
            int itemsCounter = 0;        /// Detail items counter
            int lineCounter = 0;        /// Count the number of rows(records) in file
            ///
            string line;
            string[] separators = { "^" };
            ///
            WEFiles weFiles = new WEFiles();
            List<WEItems> items = new List<WEItems>();
            //WriteErrorFile wef = new WriteErrorFile();
            /// ---------------------------------------------------------------------------
            /// Read the file line by line. OADS_F00001 File type (Arrow FTP)
            /// ---------------------------------------------------------------------------
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            try
            {
                while ((line = file.ReadLine()) != null)
                {
                    line = line.Replace("~", "");
                    ///// Count lines in file
                    lineCounter++;
                    /// Split input record into string array elements
                    string[] words = line.Split(separators, StringSplitOptions.None);
                    /// ------------------------------------------------------------------- 
                    /// Start file Identifier processing (Only one per file)
                    /// -------------------------------------------------------------------
                    if (words[0] == "ISA" && !newIsa)
                    {
                        if (words.Count() < 8)
                            throw new Exception("ISA record without complete info.");
                        newIsa = true;
                        string isaDate = words[3];
                        string isaTime = words[4];
                        string isaLan = words[5];
                        string isaCurr = words[6];
                        string isaNumb = words[7];
                        string isaDatp = words[8];
                    }
                    /// -------------------------------------------------------------------
                    /// Header entry processing
                    /// -------------------------------------------------------------------
                    else if (words[0] == "HDR" && newIsa)
                    {
                        head = true;
                        string hdrCust = words[2];
                        string hdrDat1 = words[3];
                        string hdrUsr1 = words[5];
                        string hdrUsr2 = words[6];
                        string hdrDate = words[8];
                        string hdrTime = words[9];
                        string hdrDat3 = words[18];
                    }
                    /// -------------------------------------------------------------------
                    /// Address 1 entry processing
                    /// -------------------------------------------------------------------
                    else if (words[0] == "N1" && head)
                    {
                        string n1Company = words[3];
                        string n1Address1 = words[4];
                        string n1Address2 = words[5];
                        string n1Contact = words[6];
                        string n1State = words[7];
                        string n1PostalCode = words[8];
                        string n1Country = words[9];
                        string n1Dat1 = words[10];
                        string n1Dat2 = words[11];
                        string n1Dat3 = words[20];
                    }
                    /// -------------------------------------------------------------------
                    /// Address 2 entry processing
                    /// -------------------------------------------------------------------
                    else if (words[0] == "N2" && head)
                    {
                        string n2Company = words[3];
                        string n2Address1 = words[4];
                        string n2Address2 = words[5];
                        string n2Contact = words[6];
                        string n2State = words[7];
                        string n2PostalCode = words[8];
                        string n2Country = words[9];
                    }
                    /// -------------------------------------------------------------------
                    /// Item 1 entry processing
                    /// -------------------------------------------------------------------
                    else if (words[0] == "IT1" && head)
                    {
                        string it1Sequence = words[1];
                        string it1Dat1 = words[2];
                        string it1Sku = words[5];
                        string it1Description = words[11];
                        string it1Dat2 = words[12];
                        string it1Dat3 = words[13];
                        string it1Date = words[14];
                        string it1Dat4 = words[15];
                        string it1Dat5 = words[16];
                        string it1Comments = words[19];
                        item1 = true;
                    }
                    /// -------------------------------------------------------------------
                    /// Item 2 entry processing
                    /// -------------------------------------------------------------------
                    else if (words[0] == "IT2" && item1)
                    {
                        item1 = false;
                        string it2Sequence = words[1];
                        string it2Dat1 = words[2];
                        string it2Dat2 = words[3];
                        string it2Dat3 = words[4];
                    }
                    /// -------------------------------------------------------------------
                    /// GRM entry processing
                    /// -------------------------------------------------------------------
                    else if (words[0] == "GRM" && head)
                    {
                        item1 = false;
                        string grmSequence = words[1];
                        string grmClientId = words[2];
                        string grmDescription = words[3];
                        string grmColor = words[4];
                        string grmText = words[5];
                        string grmDat1 = words[6];
                    }
                    /// -------------------------------------------------------------------
                    /// EOF entry processing
                    /// -------------------------------------------------------------------
                    else if (words[0] == "EOF" && newIsa)
                    {
                        newIsa = false;
                    }
                    else
                    {
                        /// throw error - unrecognized data line (lineCounter)
                        throw new Exception("Invalid data <" + words[0] + "> in row: " + lineCounter);
                    }
                }
            }
            catch (Exception e)
            {
                /// error while reading the file
                process = false;
                resp.IsOk = false;
                ServiceResponse errMsg = new ServiceResponse();
                errMsg.IsOk= false;
                errMsg.FileType = "x";      //  Set Nis Type
                errMsg.FileName = fileName;
                errMsg.NISOrderId = "0";
                errMsg.Status = "Not Processed";
                errMsg.Message = e.Message;
                /// Send Message to WebService
                wsm.WriteErrorFile(errMsg, sync);
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
                resp = wsm.ConsumeWebService(json, fileName, "ArrowFTP", sync);
                //  <==============  
            }
            return resp.IsOk;
        }
    }
}
