using ImportModelLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ImportControl
{
    public class ImportNISRepository
    {
        readonly ImportControlRepository wsl = new ImportControlRepository(); 

        /// <summary>
        ///     Get the next order number to be imported
        ///     Verify that the order was not imported before and has no sku problems.        
        /// </summary>
        /// <returns>Next order number</returns>
        public int getNisOrdersToImport()
        {
            object orderId = 0;
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                orderId = access.ExecuteScalar("MW_OrdersImport", DBContext.DBAccess.DBConnection.NameSys, args);
            }
            catch (Exception)
            {

            }
            return Convert.ToInt32(orderId);
        }

        /// <summary>
        ///     Retreive the complete order information (header and all depending items) for an specific order.    
        /// </summary>
        /// <param name="order"></param>
        /// <returns>Nis tables class</returns>
        public NisTables getNisOrder(int order)
        {
            SqlDataReader dr = null;
            NisTables ord = new NisTables();
            List<NisItems> list = new List<NisItems>();
            int items = 0;
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@Order", order.ToString());
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("[MW_GetNisOrder]", DBContext.DBAccess.DBConnection.NameSys, args);
                //
                while (dr != null && dr.Read())
                {
                    NisHeader head = new NisHeader();
                    head.HeaderId = (Int32)(dr["ID"].ToString().Length > 0 ? Int32.Parse(dr["ID"].ToString()) : 0);
                    head.HeaderUserId = (Int32)(dr["userID"].ToString().Length > 0 ? Int32.Parse(dr["userID"].ToString()) : 0);
                    head.HeaderUserEmail = dr["email"].ToString().Length > 0 ? dr["email"].ToString() : "";
                    head.HeaderOrderDate = dr["orderDate"].ToString().Length > 0 ? DateTime.Parse(dr["orderDate"].ToString()) : DateTime.Now;
                    head.HeaderDateRequested = dr["dateReq"].ToString().Length > 0 ? DateTime.Parse(dr["dateReq"].ToString()) : DateTime.Now;
                    head.HeaderPO = dr["PO"].ToString().Length > 0 ? dr["PO"].ToString() : "";
                    head.HeaderDescription = dr["description"].ToString().Length > 0 ? dr["description"].ToString() : "";
                    head.HeaderShipVia1 = dr["shippingViaID"].ToString().Length > 0 ? dr["shippingViaID"].ToString() : "";
                    head.HeaderShipVia2 = dr["shippingViaID2"].ToString().Length > 0 ? dr["shippingViaID2"].ToString() : "";
                    head.HeaderShipToId1 = dr["shippingToID"].ToString().Length > 0 ? dr["shippingToID"].ToString() : "";
                    head.HeaderShipToId2 = dr["shippingToID2"].ToString().Length > 0 ? dr["shippingToID2"].ToString() : "";
                    head.HeaderComments = dr["comments"].ToString().Length > 0 ? dr["comments"].ToString() : "";
                    head.FileName = "Db Source";
                    head.HeaderSentDate = dr["sentDate"].ToString().Length > 0 ? DateTime.Parse(dr["sentDate"].ToString()) : DateTime.Now;
                    ord.header = head;      //  Store order header information
                    //
                    dr.NextResult();        //  Move to the second select result set
                    //
                    while (dr != null && dr.Read())
                    {   //  Get all order items (From second select result set)                        
                        NisItems itm = new NisItems();
                        items++;
                        itm.NisOrderId = items;             //  set item sequence 
                        itm.Itemid = (int)(dr["Id"].ToString().Length > 0 ? int.Parse(dr["Id"].ToString()) : 0);
                        itm.OrderId = (int)(dr["orderID"].ToString().Length > 0 ? int.Parse(dr["orderID"].ToString()) : 0);
                        itm.SkuId = (int)(dr["SKU_ID"].ToString().Length > 0 ? int.Parse(dr["SKU_ID"].ToString()) : 0);
                        itm.LineId = (int)(dr["lineID"].ToString().Length > 0 ? int.Parse(dr["lineID"].ToString()) : 0);
                        itm.Quantity1 = (int)(dr["Qty1"].ToString().Length > 0 ? int.Parse(dr["Qty1"].ToString()) : 0);
                        itm.Quantity2 = (int)(dr["Qty2"].ToString().Length > 0 ? int.Parse(dr["Qty2"].ToString()) : 0);
                        itm.Line1 = dr["line1"].ToString().Length > 0 ? dr["line1"].ToString() : "";
                        itm.Line2 = dr["line2"].ToString().Length > 0 ? dr["line2"].ToString() : "";
                        itm.Line3 = dr["line3"].ToString().Length > 0 ? dr["line3"].ToString() : "";
                        itm.Description = dr["description"].ToString().Length > 0 ? dr["description"].ToString() : "";
                        itm.SkuClient = dr["SKU_client"].ToString().Length > 0 ? dr["SKU_client"].ToString() : "";
                        itm.SkuWe = dr["SKU_WE"].ToString().Length > 0 ? dr["SKU_WE"].ToString() : "";
                        itm.AccountCode = dr["accountID"].ToString().Length > 0 ? dr["accountID"].ToString() : "";
                        list.Add(itm);
                    }
                    ord.items = list;   //  Store order Items information 
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return ord;
        }

        /// <summary>
        ///     Update an specific order haeder information with the import results atatus codes.
        ///     The order parameter is mandatory
        ///     The option parameter update the WasImported columnn of the order header
        ///     The option importProblem update the hasImportProblem
        /// </summary>
        /// <param name="order">specified the order number to be updated</param>
        /// <param name="uOption">specified the import option - yes = 1 / no = 0</param>
        /// <param name="importProblem"></param>
        /// <returns></returns>
        public int updNisOrder(int order, int uOption, int importProblem, int NavOrder)
        {
            Object ret = 0;
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@Order",order.ToString());
                args.Add("@uOption", uOption.ToString());
                args.Add("@impProb", importProblem.ToString());
                args.Add("@NavOrder", "NAV-" + NavOrder.ToString());
                args.Add("@isNav", "1");        //  SET IS A NAV ORDER FLAG ON
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                ret = access.ExecuteScalar("MW_UpdNISOrder", DBContext.DBAccess.DBConnection.NameSys, args);
                //   
            }
            catch (Exception)
            {
                wsl.writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1, ConfigurationManager.AppSettings["TableName"], "Table update error, order " + order);
            }
            return Convert.ToInt32(ret);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int insertNisLateOrder(NISLateOrders order)
        {
            object orderId = 0;
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@ID", order.ID.ToString());
                args.Add("@userID", order.UserId);
                args.Add("@email", order.Email);
                args.Add("@orderDate", order.OrderDate.ToString());
                args.Add("@dateReq", order.DateReq.ToString());
                args.Add("@PO", order.PO);
                args.Add("@sentDate", order.SentDate.ToString());
                args.Add("@items", order.Items.ToString());
                args.Add("@SON", order.SON);
                args.Add("@cutOff", order.CutOff.ToString());
                args.Add("@imported", order.Imported.ToString());
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                orderId = access.ExecuteScalar("MW_NISLateOrderInsert", DBContext.DBAccess.DBConnection.NameSys, args);
            }
            catch (Exception)
            {

            }
            return Convert.ToInt32(orderId);
        }

        /// <summary>
        /// Create a list of all NIS sent orders that have been imported after cutoff time
        /// No parameters requiered.
        /// </summary>
        /// <returns>Late orders list</returns>
        public List<NISLateOrders> getNisLateOrders()
        {
            SqlDataReader dr = null;
            List<NISLateOrders> list = new List<NISLateOrders>();
            int items = 0;
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("[MW_NISLateOrdersControl]", DBContext.DBAccess.DBConnection.NameSys, args);
                //
                while (dr != null && dr.Read())
                {
                    NISLateOrders ord = new NISLateOrders();
                    ord.ID = (Int32)(dr["ID"].ToString().Length > 0 ? Int32.Parse(dr["ID"].ToString()) : 0);
                    ord.UserId = (dr["userID"].ToString().Length > 0 ? (dr["userID"].ToString()) : "");
                    ord.Email = dr["email"].ToString().Length > 0 ? dr["email"].ToString() : "";
                    ord.OrderDate = dr["orderDate"].ToString().Length > 0 ? DateTime.Parse(dr["orderDate"].ToString()) : DateTime.Now;
                    ord.DateReq = dr["dateReq"].ToString().Length > 0 ? DateTime.Parse(dr["dateReq"].ToString()) : DateTime.Now;
                    ord.PO = dr["PO"].ToString().Length > 0 ? dr["PO"].ToString() : "";
                    ord.SentDate = dr["sentDate"].ToString().Length > 0 ? DateTime.Parse(dr["sentDate"].ToString()) : DateTime.Now;
                    ord.Items = (Int32)(dr["items"].ToString().Length > 0 ? Int32.Parse(dr["items"].ToString()) : 0);
                    ord.SON = dr["SON"].ToString().Length > 0 ? dr["SON"].ToString() : "";
                    ord.CutOff = dr["cutoff"].ToString().Length > 0 ? DateTime.Parse(dr["cutoff"].ToString()) : DateTime.Now;
                    ord.Imported = dr["Imported"].ToString().Length > 0 ? DateTime.Parse(dr["Imported"].ToString()) : DateTime.Now;
                    //
                    list.Add(ord);   //  Store order Items information 
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return list;
        }
    }
}
