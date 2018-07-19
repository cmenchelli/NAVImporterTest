using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportProcedure_NIS.ExcelFiles
{
    public class ImportProcess
    {
        /// <summary>
        ///     Read any excel file with xls and/or xlsx extension, it can read all columns or select 
        ///     specific columns detailed in 'selectedFields' parameter.   
        ///     First 6 Rows of the original file are ignored, first valid data row in returning table 
        ///     is 7.
        /// </summary>
        /// <param name="filePath">The complete file path to input file.</param>
        /// <param name="sheetName">Excel file sheet name.</param>
        /// <param name="selectFields">Select fields like * or fields with comma separated.</param>
        /// <param name="tableName">Name of the return DataTable.</param>
        /// <param name="fileIncludesHeaders">Indicates whether file includes headers.</param>
        /// <returns>DataTable with the excel file content</returns>
        public static DataTable ReadExcelFile(String filePath, String sheetName, String selectFields, String tableName, Boolean fileIncludesHeaders)
        {
            DataSet dataSet = null;
            DataTable dtReturn = null;
            string connectionString = string.Empty;
            string commandText = string.Empty;

            /// Indicates the Excel file with header or not.
            string headerYesNo = string.Empty;
            string fileExtension = string.Empty;
            try
            {
                if (fileIncludesHeaders == true)
                {   /// Set YES if excel WithHeader is TRUE.
                    headerYesNo = "YES";
                }
                else
                {   /// Set NO if excel WithHeader is FALSE.
                    headerYesNo = "NO";
                }
                /// Gets file extension to select which connection type to use with this excel version file.

                string currentFile = filePath + "-sent";
                //File.Move(currentFile, filePath);
                //fileExtension = Path.GetExtension(filePath);
                fileExtension = Path.GetExtension(currentFile);
                switch (fileExtension.ToUpper())
                {
                    case ".XLS":
                        /// Take Connection For Microsoft Excel 97-2003 Worksheet.
                        connectionString =
                          string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;IMEX=2.0;HDR={1}""",
                                        filePath, headerYesNo);
                        break;

                    case ".XLSX":
                        /// Take Connection For Microsoft Excel Worksheet.
                        connectionString =
                          string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0;IMEX=2.0;HDR={1}""",
                                        currentFile, headerYesNo);
                        break;

                    case ".XLS-SENT":
                        /// Take Connection For Microsoft Excel Worksheet.
                        connectionString =
                          string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0;IMEX=2.0;HDR={1}""",
                                        currentFile, headerYesNo);
                        break;

                    default:
                        throw new Exception("File is invalid.");
                }

                commandText = string.Format("SELECT {0} FROM [{1}$]", selectFields, sheetName);

                dataSet = new DataSet();
                using (OleDbConnection dbConnection = new OleDbConnection(connectionString))
                {
                    OleDbCommand dbCommand = new OleDbCommand(commandText, dbConnection);
                    dbCommand.CommandType = CommandType.Text;
                    OleDbDataAdapter dbDataAdapter = new OleDbDataAdapter(dbCommand);
                    dbDataAdapter.Fill(dataSet);
                }

                if (dataSet != null &&
                    dataSet.Tables.Count > 0)
                {
                    dataSet.Tables[0].TableName = tableName;
                    /// Sets reference of data table.
                    dtReturn = dataSet.Tables[tableName];
                    /// Get Rid of first 6 rows (Form title + Header) 
                    for (int i = 0; i < 6; i++)
                    {
                        dtReturn.Rows.RemoveAt(0);
                    }
                    //string resXml = GetXML(dataSet);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dtReturn;
        }

        /// <summary>
        ///     Convert DataTable to Xml format
        /// </summary>
        /// <param name="ds">DataSet with the file info</param>
        /// <returns>Xml format string</returns>
        private static string GetXML(DataSet ds)
        {
            return ds.GetXml();
        }
    }
}