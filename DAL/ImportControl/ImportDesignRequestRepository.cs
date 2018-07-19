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
    public class ImportDesignRequestRepository
    {
        readonly ImportControlRepository ikr = new ImportControlRepository();

        /// <summary>
        ///     Get the list of all resign requets Id that are not imported.
        /// </summary>
        /// <returns></returns>
        public List<string> getDesignRequest_NotImportedList()
        {
            SqlDataReader dr = null;
            var orderList = new List<string>();
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                //args.Add("@Order", order.ToString());
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("MW_WSPGetDesignRequestsList_notImported", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //
                while (dr != null && dr.Read())
                {
                    string orderId = null;
                    orderId = dr["CatDesignRequestOrderId"].ToString().Length > 0 ? (dr["CatDesignRequestOrderId"].ToString()) : "0";
                    orderList.Add(orderId);
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return orderList;
        }
        
        /// <summary>
        ///     Retreive the complete order information (header and all depending items) for an specific order.    
        /// </summary>
        /// <param name="order"></param>
        /// <returns>Nis tables class</returns>
        public DesignRequest getDesignRequest(string order)
        {
            SqlDataReader dr = null;
            DesignRequest dreq = new DesignRequest();
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@Order", order);
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("[MW_WSPGetDesignRequestOrders_V01]", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //
                while (dr != null && dr.Read())
                {
                    dreq.CatDesignRequestOrderId    = dr["CatDesignRequestOrderId"].ToString().Length > 0 ? (dr["CatDesignRequestOrderId"].ToString()) : "0";
                    dreq.DesignId                   = dr["DesignId"].ToString().Length > 0 ? (dr["DesignId"].ToString()) : "0";
                    dreq.CreateDate                 = dr["CreateDate"].ToString().Length > 0 ? DateTime.Parse(dr["CreateDate"].ToString()) : DateTime.Now;
                    dreq.ParentDesignRequestId      = dr["ParentDesignRequestId"].ToString().Length > 0 ? (dr["ParentDesignRequestId"].ToString()) : "0";
                    dreq.CustomerId                 = dr["CustomerId"].ToString().Length > 0 ? (dr["CustomerId"].ToString()) : "0";
                    dreq.Optional1                  = dr["Optional1"].ToString().Length > 0 ? dr["Optional1"].ToString() : "";                    
                    dreq.Optional2                  = dr["Optional2"].ToString().Length > 0 ? dr["Optional2"].ToString() : "";
                    dreq.Optional3                  = dr["Optional3"].ToString().Length > 0 ? dr["Optional3"].ToString() : "";
                    dreq.Optional4                  = dr["Optional4"].ToString().Length > 0 ? dr["Optional4"].ToString() : "";
                    dreq.Optional5                  = dr["Optional5"].ToString().Length > 0 ? dr["Optional5"].ToString() : "";
                    dreq.Optional6                  = dr["Optional6"].ToString().Length > 0 ? dr["Optional6"].ToString() : "";
                    dreq.Optional7                  = dr["Optional7"].ToString().Length > 0 ? dr["Optional7"].ToString() : "";
                    dreq.Optional8                  = dr["Optional8"].ToString().Length > 0 ? dr["Optional8"].ToString() : "";
                    dreq.Optional10                 = dr["Optional10"].ToString().Length > 0 ? dr["Optional10"].ToString() : "";
                    dreq.Requestor                  = dr["Requestor"].ToString().Length > 0 ? dr["Requestor"].ToString() : "";
                    dreq.WECustomerNo               = dr["WECustomerNo"].ToString().Length > 0 ? dr["WECustomerNo"].ToString() : "";
                    dreq.SapCustomerName            = dr["SapCustomerName"].ToString().Length > 0 ? dr["SapCustomerName"].ToString() : "";
                    dreq.SapCustomerNo              = dr["SapCustomerNo"].ToString().Length > 0 ? dr["SapCustomerNo"].ToString() : "";
                    dreq.PONumber                   = dr["PONumber"].ToString().Length > 0 ? dr["PONumber"].ToString() : "";
                    dreq.Description                = dr["Description"].ToString().Length > 0 ? dr["Description"].ToString() : "";
                    dreq.SpecialInstructions        = dr["SpecialInstructions"].ToString().Length > 0 ? dr["SpecialInstructions"].ToString() : "";
                    dreq.ProductClass               = dr["ProductClass"].ToString().Length > 0 ? dr["ProductClass"].ToString() : "";
                    dreq.ProductType                = dr["ProductType"].ToString().Length > 0 ? dr["ProductType"].ToString() : "";
                    dreq.EmailsElectronicSimulation = dr["EmailsElectronicSimulation"].ToString().Length > 0 ? dr["EmailsElectronicSimulation"].ToString() : "";
                    dreq.GarmentType                = dr["GarmentType"].ToString().Length > 0 ? dr["GarmentType"].ToString() : "";
                    dreq.ImpacCodes                 = dr["ImpacCodes"].ToString().Length > 0 ? dr["ImpacCodes"].ToString() : "";
                    dreq.ShapeSize                  = dr["ShapeSize"].ToString().Length > 0 ? dr["ShapeSize"].ToString() : "";
                    dreq.Shapes                     = dr["Shapes"].ToString().Length > 0 ? dr["Shapes"].ToString() : "";
                    dreq.Width                      = dr["Width"].ToString().Length > 0 ? dr["Width"].ToString() : "";
                    dreq.Height                     = dr["Height"].ToString().Length > 0 ? dr["Height"].ToString() : "";
                    dreq.PlacementId                = dr["PlacementId"].ToString().Length > 0 ? dr["PlacementId"].ToString() : "";
                    dreq.FabricTypeId               = dr["FabricTypeId"].ToString().Length > 0 ? dr["FabricTypeId"].ToString() : "";
                    dreq.FabricId                   = dr["FabricId"].ToString().Length > 0 ? dr["FabricId"].ToString() : "";
                    dreq.BackingId                  = dr["BackingId"].ToString().Length > 0 ? dr["BackingId"].ToString() : "";
                    dreq.BorderId                   = dr["BorderId"].ToString().Length > 0 ? dr["BorderId"].ToString() : "";
                    dreq.Artwork                    = dr["Artwork"].ToString().Length > 0 ? dr["Artwork"].ToString() : "";
                    dreq.OptionalArtText1           = dr["OptionalArtText1"].ToString().Length > 0 ? dr["OptionalArtText1"].ToString() : "";
                    dreq.OptionalArtText2           = dr["OptionalArtText2"].ToString().Length > 0 ? dr["OptionalArtText2"].ToString() : "";
                    dreq.OptionalArtText3           = dr["OptionalArtText3"].ToString().Length > 0 ? dr["OptionalArtText3"].ToString() : "";
                    dreq.ThreadInkColors            = dr["ThreadInkColors"].ToString().Length > 0 ? dr["ThreadInkColors"].ToString() : "";
                    dreq.Contacts                   = dr["Contacts"].ToString().Length > 0 ? dr["Contacts"].ToString() : "";
                    dreq.SalesRep                   = dr["SalesRep"].ToString().Length > 0 ? dr["SalesRep"].ToString() : "";
                    dreq.ShipViaMethod1             = dr["ShipViaMethod1"].ToString().Length > 0 ? dr["ShipViaMethod1"].ToString() : "";
                    dreq.ShipQty1                   = dr["ShipQty1"].ToString().Length > 0 ? dr["ShipQty1"].ToString() : "";
                    dreq.ShipAddress1               = dr["ShipAddress1"].ToString().Length > 0 ? dr["ShipAddress1"].ToString() : "";
                    dreq.ShipEmailAddress1          = dr["ShipEmailAddress1"].ToString().Length > 0 ? dr["ShipEmailAddress1"].ToString() : "";
                    dreq.ShipViaMethod2             = dr["ShipViaMethod2"].ToString().Length > 0 ? dr["ShipViaMethod2"].ToString() : "";
                    dreq.ShipQty2                   = dr["ShipQty2"].ToString().Length > 0 ? dr["ShipQty2"].ToString() : "";
                    dreq.ShipAddress2               = dr["ShipAddress2"].ToString().Length > 0 ? dr["ShipAddress2"].ToString() : "";
                    dreq.ShipEmailAddress2          = dr["ShipEmailAddress2"].ToString().Length > 0 ? dr["ShipEmailAddress2"].ToString() : "";
                    dreq.ShipViaMethod3             = dr["ShipViaMethod3"].ToString().Length > 0 ? dr["ShipViaMethod3"].ToString() : "";
                    dreq.ShipQty3                   = dr["ShipQty3"].ToString().Length > 0 ? dr["ShipQty3"].ToString() : "";
                    dreq.ShipAddress3               = dr["ShipAddress3"].ToString().Length > 0 ? dr["ShipAddress3"].ToString() : "";
                    dreq.ShipEmailAddress3          = dr["ShipEmailAddress3"].ToString().Length > 0 ? dr["ShipEmailAddress3"].ToString() : "";
                    dreq.HasDRBeenProcessed         = Convert.ToBoolean(dr["HasDRBeenProcessed"].ToString().Length > 0 ? Convert.ToBoolean(dr["HasDRBeenProcessed"].ToString()) : false);
                    dreq.Status                     = dr["Status"].ToString().Length > 0 ? dr["Status"].ToString() : "";
                    dreq.DiecodeId                  = dr["DiecodeId"].ToString().Length > 0 ? dr["DiecodeId"].ToString() : "";
                    dreq.Misc                       = dr["Misc"].ToString().Length > 0 ? dr["Misc"].ToString() : "";
                    dreq.Price                      = dr["Price"].ToString().Length > 0 ? dr["Price"].ToString() : "";
                    dreq.EditReasonId               = dr["EditReasonId"].ToString().Length > 0 ? dr["EditReasonId"].ToString() : "";
                    dreq.EditReasonComments         = dr["EditReasonComments"].ToString().Length > 0 ? dr["EditReasonComments"].ToString() : "";
                    dreq.HasConfirmationEmailBeenSent = Convert.ToBoolean(dr["HasConfirmationEmailBeenSent"].ToString().Length > 0 ? Convert.ToBoolean(dr["HasConfirmationEmailBeenSent"].ToString()) : false);
                    dreq.FormNm                     = dr["form_nm"].ToString().Length > 0 ? dr["form_nm"].ToString() : "";
                    dreq.OptionalArtFont1           = dr["OptionalArtFont1"].ToString().Length > 0 ? dr["OptionalArtFont1"].ToString() : "";
                    dreq.OptionalArtFont2           = dr["OptionalArtFont2"].ToString().Length > 0 ? dr["OptionalArtFont2"].ToString() : "";
                    dreq.OptionalArtFont3           = dr["OptionalArtFont3"].ToString().Length > 0 ? dr["OptionalArtFont3"].ToString() : "";
                    dreq.Sku                        = dr["Sku"].ToString().Length > 0 ? dr["Sku"].ToString() : "";
                    dreq.PricingDescription         = dr["PricingDescription"].ToString().Length > 0 ? dr["PricingDescription"].ToString() : "";
                    dreq.ApprovedDate               = dr["ApprovedDate"].ToString().Length > 0 ? DateTime.Parse(dr["ApprovedDate"].ToString()) : DateTime.Now;
                    dreq.PrintedDate                = dr["PrintedDate"].ToString().Length > 0 ? DateTime.Parse(dr["PrintedDate"].ToString()) : DateTime.Now;
                    dreq.CustomerType               = dr["CustomerType"].ToString().Length > 0 ? dr["CustomerType"].ToString() : "";
                    dreq.WeiDrId                    = (Decimal)(dr["wei_dr_id"].ToString().Length > 0 ? Decimal.Parse(dr["wei_dr_id"].ToString()) : 0);
                    dreq.ErrorFlag                  = Convert.ToBoolean(dr["ErrorFlag"].ToString().Length > 0 ? Convert.ToBoolean(dr["ErrorFlag"].ToString()) : false);
                    dreq.ErrorDescription           = dr["ErrorDescription"].ToString().Length > 0 ? dr["ErrorDescription"].ToString() : "";
                    dreq.EmployeeId                 = dr["EmployeeId"].ToString().Length > 0 ? dr["EmployeeId"].ToString() : "";
                    dreq.EmployeeUserName           = dr["EmployeeUserName"].ToString().Length > 0 ? dr["EmployeeUserName"].ToString() : "";
                    dreq.AccountId                  = dr["AccountId"].ToString().Length > 0 ? dr["AccountId"].ToString() : "";
                    dreq.AddEditFlag                = (Int64)(dr["AddEditFlag"].ToString().Length > 0 ? Int64.Parse(dr["AddEditFlag"].ToString()) : 0);
                    dreq.EditLock                   = Convert.ToBoolean(dr["EditLock"].ToString().Length > 0 ? Convert.ToBoolean(dr["EditLock"].ToString()) : false);
                    dreq.TrackingXml                = dr["TrackingXml"].ToString().Length > 0 ? dr["TrackingXml"].ToString() : "";
                    dreq.AtcFlagged                 = Convert.ToBoolean(dr["atc_flagged"].ToString().Length > 0 ? Convert.ToBoolean(dr["atc_flagged"].ToString()) : false);
                    dreq.Priority                   = (Decimal)(dr["Priority"].ToString().Length > 0 ? Decimal.Parse(dr["Priority"].ToString()) : 0);
                    dreq.ProductTypeId              = dr["ProductTypeId"].ToString().Length > 0 ? dr["ProductTypeId"].ToString() : "";
                    dreq.ImpacCodeId                = dr["ImpacCodeId"].ToString().Length > 0 ? dr["ImpacCodeId"].ToString() : "";
                    dreq.FabricType2Id              = dr["FabricType2Id"].ToString().Length > 0 ? dr["FabricType2Id"].ToString() : "";
                    dreq.Fabric2Id                  = dr["Fabric2Id"].ToString().Length > 0 ? dr["Fabric2Id"].ToString() : "";
                    dreq.Backing2Id                 = dr["Backing2Id"].ToString().Length > 0 ? dr["Backing2Id"].ToString() : "";
                    dreq.Border2Id                  = dr["Border2Id"].ToString().Length > 0 ? dr["Border2Id"].ToString() : "";
                    dreq.GarmentPlacementId         = dr["GarmentPlacementId"].ToString().Length > 0 ? dr["GarmentPlacementId"].ToString() : "";
                    dreq.DroperLayoutId             = dr["DroperLayoutId"].ToString().Length > 0 ? dr["DroperLayoutId"].ToString() : "";
                    dreq.SizeShapeId                = dr["SizeShapeId"].ToString().Length > 0 ? dr["SizeShapeId"].ToString() : "";
                    dreq.GarmentTypeId              = dr["GarmentTypeId"].ToString().Length > 0 ? dr["GarmentTypeId"].ToString() : "";
                    dreq.ProductClassId             = dr["ProductClassId"].ToString().Length > 0 ? dr["ProductClassId"].ToString() : "";
                    dreq.EditReason2Id              = dr["EditReason2Id"].ToString().Length > 0 ? dr["EditReason2Id"].ToString() : "";
                    dreq.StitchFont1Id              = dr["StitchFont1Id"].ToString().Length > 0 ? dr["StitchFont1Id"].ToString() : "";
                    dreq.StitchFont2Id              = dr["StitchFont2Id"].ToString().Length > 0 ? dr["StitchFont2Id"].ToString() : "";
                    dreq.StitchFont3Id              = dr["StitchFont3Id"].ToString().Length > 0 ? dr["StitchFont3Id"].ToString() : "";
                    dreq.SearchField1               = dr["SearchField1"].ToString().Length > 0 ? dr["SearchField1"].ToString() : "";
                    dreq.RequestFor                 = dr["RequestFor"].ToString().Length > 0 ? dr["RequestFor"].ToString() : "";
                    dreq.SyndicatedAccountNo        = dr["SyndicatedAccountNo"].ToString().Length > 0 ? dr["SyndicatedAccountNo"].ToString() : "";
                    dreq.CreateOrder                = Convert.ToBoolean(dr["CreateOrder"].ToString().Length > 0 ? Convert.ToBoolean(dr["CreateOrder"].ToString()) : false);
                    dreq.DrsiId                     = dr["drsi_id"].ToString().Length > 0 ? dr["drsi_id"].ToString() : "";
                    dreq.DrshId                     = dr["drsh_id"].ToString().Length > 0 ? dr["drsh_id"].ToString() : "";
                    dreq.KnowSize                   = Convert.ToBoolean(dr["KnowSize"].ToString().Length > 0 ? Convert.ToBoolean(dr["KnowSize"].ToString()) : false);
                    dreq.EmbellishmentGroupId       = dr["EmbellishmentGroupId"].ToString().Length > 0 ? dr["EmbellishmentGroupId"].ToString() : "";
                    dreq.EmbellishmentGroupRefId    = dr["EmbellishmentGroupRefId"].ToString().Length > 0 ? dr["EmbellishmentGroupRefId"].ToString() : "";
                    dreq.ColorInstructions          = dr["ColorInstructions"].ToString().Length > 0 ? dr["ColorInstructions"].ToString() : "";
                    dreq.LegalAcceptTerms           = Convert.ToBoolean(dr["LegalAcceptTerms"].ToString().Length > 0 ? Convert.ToBoolean(dr["LegalAcceptTerms"].ToString()) : false);
                    dreq.LegalAgreement             = dr["LegalAgreement"].ToString().Length > 0 ? dr["LegalAgreement"].ToString() : "";
                    dreq.SendDRConfirmationEmail    = dr["SendDRConfirmationEmail"].ToString().Length > 0 ? dr["SendDRConfirmationEmail"].ToString() : "";
                    dreq.ExportDr                   = Convert.ToBoolean(dr["export_dr"].ToString().Length > 0 ? Convert.ToBoolean(dr["export_dr"].ToString()) : false);
                    dreq.SessionId                  = dr["SessionId"].ToString().Length > 0 ? dr["SessionId"].ToString() : "";
                    dreq.SyndicatedSitename         = dr["SyndicatedSitename"].ToString().Length > 0 ? dr["SyndicatedSitename"].ToString() : "";
                    dreq.SyndicatedUsername         = dr["SyndicatedUsername"].ToString().Length > 0 ? dr["SyndicatedUsername"].ToString() : "";
                    dreq.ItemsToDecorate            = dr["ItemsToDecorate"].ToString().Length > 0 ? dr["ItemsToDecorate"].ToString() : "";
                    dreq.LatestRecord               = Convert.ToBoolean(dr["LatestRecord"].ToString().Length > 0 ? Convert.ToBoolean(dr["LatestRecord"].ToString()) : false);
                    dreq.SyndicatedAcctName         = dr["syndicated_acct_name"].ToString().Length > 0 ? dr["syndicated_acct_name"].ToString() : "";
                    dreq.ArtHeight                  = (Decimal)(dr["ArtHeight"].ToString().Length > 0 ? Decimal.Parse(dr["ArtHeight"].ToString()) : 0);
                    dreq.ArtWidth                   = (Decimal)(dr["ArtWidth"].ToString().Length > 0 ? Decimal.Parse(dr["ArtWidth"].ToString()) : 0);
                    dreq.DeliveryOption             = dr["DeliveryOption"].ToString().Length > 0 ? dr["DeliveryOption"].ToString() : "";
                    dreq.ElectronicSimulation       = Convert.ToBoolean(dr["ElectronicSimulation"].ToString().Length > 0 ? Convert.ToBoolean(dr["ElectronicSimulation"].ToString()) : false);
                    dreq.ColorChange                = Convert.ToBoolean(dr["ColorChange"].ToString().Length > 0 ? Convert.ToBoolean(dr["ColorChange"].ToString()) : false);
                    dreq.LocationChange             = Convert.ToBoolean(dr["LocationChange"].ToString().Length > 0 ? Convert.ToBoolean(dr["LocationChange"].ToString()) : false);
                    dreq.FavoritesAudience          = dr["FavoritesAudience"].ToString().Length > 0 ? dr["FavoritesAudience"].ToString() : "";
                    dreq.FavoritesCreateDate        = dr["FavoritesCreateDate"].ToString().Length > 0 ? DateTime.Parse(dr["FavoritesCreateDate"].ToString()) : DateTime.Now;
                    dreq.FavoritesNickName          = dr["FavoritesNickName"].ToString().Length > 0 ? dr["FavoritesNickName"].ToString() : "";
                    dreq.RecordType                 = dr["RecordType"].ToString().Length > 0 ? dr["RecordType"].ToString() : "";
                    dreq.SubmittedByAccountId       = dr["SubmittedByAccountId"].ToString().Length > 0 ? dr["SubmittedByAccountId"].ToString() : "";
                    dreq.SubmittedByCustomerId      = dr["SubmittedByCustomerId"].ToString().Length > 0 ? dr["SubmittedByCustomerId"].ToString() : "";
                    dreq.CompleteDate               = dr["CompleteDate"].ToString().Length > 0 ? DateTime.Parse(dr["CompleteDate"].ToString()) : DateTime.Now;
                    dreq.AnnotateAttempted          = Convert.ToBoolean(dr["annotate_attempted"].ToString().Length > 0 ? Convert.ToBoolean(dr["annotate_attempted"].ToString()) : false);
                    dreq.AnnotateSaved              = Convert.ToBoolean(dr["annotate_saved"].ToString().Length > 0 ? Convert.ToBoolean(dr["annotate_saved"].ToString()) : false);
                    dreq.AnnotateInfo               = dr["annotate_info"].ToString().Length > 0 ? dr["annotate_info"].ToString() : "";
                    dreq.CompletedDesignNormal      = dr["CompletedDesignNormal"].ToString().Length > 0 ? dr["CompletedDesignNormal"].ToString() : "";
                    dreq.CompletedPictureThumbnail  = dr["CompletedPictureThumbnail"].ToString().Length > 0 ? dr["CompletedPictureThumbnail"].ToString() : "";
                    dreq.MaintainProportions        = dr["MaintainProportions"].ToString().Length > 0 ? dr["MaintainProportions"].ToString() : "";
                    dreq.ExportStatus               = dr["ExportStatus"].ToString().Length > 0 ? dr["ExportStatus"].ToString() : "";
                    dreq.ExpectedShipDate           = dr["ExpectedShipDate"].ToString().Length > 0 ? DateTime.Parse(dr["ExpectedShipDate"].ToString()) : DateTime.Now;
                    dreq.ColorPreference            = dr["ColorPreference"].ToString().Length > 0 ? dr["ColorPreference"].ToString() : "";
                    dreq.SafetyStripesId            = dr["SafetyStripesId"].ToString().Length > 0 ? dr["SafetyStripesId"].ToString() : "";
                    dreq.DpSent                     = dr["DpSent"].ToString().Length > 0 ? DateTime.Parse(dr["DpSent"].ToString()) : DateTime.Now;
                    dreq.SampleShippedSent          = dr["SampleShippedSent"].ToString().Length > 0 ? DateTime.Parse(dr["SampleShippedSent"].ToString()) : DateTime.Now;
                    dreq.DpNotificationSent         = Convert.ToBoolean(dr["DpNotificationSent"].ToString().Length > 0 ? Convert.ToBoolean(dr["DpNotificationSent"].ToString()) : false);
                    dreq.SampleShippedNotificationSent = Convert.ToBoolean(dr["SampleShippedNotificationSent"].ToString().Length > 0 ? Convert.ToBoolean(dr["SampleShippedNotificationSent"].ToString()) : false);
                    dreq.MobileAppDesign            = dr["MobileAppDesign"].ToString().Length > 0 ? dr["MobileAppDesign"].ToString() : "";
                    dreq.TrackingNumber1            = dr["TrackingNumber1"].ToString().Length > 0 ? dr["TrackingNumber1"].ToString() : "";
                    dreq.TrackingNumber2            = dr["TrackingNumber2"].ToString().Length > 0 ? dr["TrackingNumber2"].ToString() : "";
                    dreq.TrackingNumber3            = dr["TrackingNumber3"].ToString().Length > 0 ? dr["TrackingNumber3"].ToString() : "";
                    //
                    dreq.AIdCusNo                   = dr["a_id_cus_no"].ToString().Length > 0 ? dr["a_id_cus_no"].ToString() : "";
                    dreq.SubmittedByAIdCusNo        = dr["submitted_by_a_id_cus_no"].ToString().Length > 0 ? dr["submitted_by_a_id_cus_no"].ToString() : "";
                    dreq.EmailConfirmation          = dr["EmailConfirmation"].ToString().Length > 0 ? dr["EmailConfirmation"].ToString() : "";
                    dreq.DrImpacCodesWeiId          = dr["dr_impac_codes_wei_id"].ToString().Length > 0 ? dr["dr_impac_codes_wei_id"].ToString() : "";
                    dreq.DrFabricTypesWeiId         = dr["dr_fabric_types_wei_id"].ToString().Length > 0 ? dr["dr_fabric_types_wei_id"].ToString() : "";
                    dreq.DrBackingWeiId             = dr["dr_backing_wei_id"].ToString().Length > 0 ? dr["dr_backing_wei_id"].ToString() : "";
                    dreq.DrBordersWeiId             = dr["dr_borders_wei_id"].ToString().Length > 0 ? dr["dr_borders_wei_id"].ToString() : "";
                    dreq.DrProductTypeWeiId         = dr["dr_product_type_wei_id"].ToString().Length > 0 ? dr["dr_product_type_wei_id"].ToString() : "";
                    dreq.DrFabricsWeiId             = dr["dr_fabrics_wei_id"].ToString().Length > 0 ? dr["dr_fabrics_wei_id"].ToString() : "";
                    dreq.DrDropperLayoutsWeiId      = dr["dr_dropper_layouts_wei_id"].ToString().Length > 0 ? dr["dr_dropper_layouts_wei_id"].ToString() : "";
                    dreq.DrProductClassesWeiId      = dr["dr_product_classes_wei_id"].ToString().Length > 0 ? dr["dr_product_classes_wei_id"].ToString() : "";
                    dreq.DrEditReasonsWeiId         = dr["dr_edit_reasons_wei_id"].ToString().Length > 0 ? dr["dr_edit_reasons_wei_id"].ToString() : "";
                    dreq.DrShapesSizesWeiId         = dr["dr_shapes_sizes_wei_id"].ToString().Length > 0 ? dr["dr_shapes_sizes_wei_id"].ToString() : "";
                    dreq.DrShapesName               = dr["dr_shapes_name"].ToString().Length > 0 ? dr["dr_shapes_name"].ToString() : "";
                    dreq.DrShapesStandard           = dr["dr_shapes_standard"].ToString().Length > 0 ? dr["dr_shapes_standard"].ToString() : "";
                    dreq.DrStitchFonts1WeiId        = dr["dr_stitch_fonts1_wei_id"].ToString().Length > 0 ? dr["dr_stitch_fonts1_wei_id"].ToString() : "";
                    dreq.DrStitchFonts2WeiId        = dr["dr_stitch_fonts2_wei_id"].ToString().Length > 0 ? dr["dr_stitch_fonts2_wei_id"].ToString() : "";
                    dreq.DrStitchFonts3WeiId        = dr["dr_stitch_fonts3_wei_id"].ToString().Length > 0 ? dr["dr_stitch_fonts3_wei_id"].ToString() : "";
                    dreq.GarmentPlacementsWeiId     = dr["garment_placements_wei_id"].ToString().Length > 0 ? dr["garment_placements_wei_id"].ToString() : "";
                    dreq.EmbellishmentGroupsDs      = dr["embellishment_groups_ds"].ToString().Length > 0 ? dr["embellishment_groups_ds"].ToString() : "";
                    dreq.ShippingAddressesAddress1Addedit   = (Int64)(dr["shipping_addresses_address_1_addedit"].ToString().Length > 0 ? Int64.Parse(dr["shipping_addresses_address_1_addedit"].ToString()) : 0);
                    dreq.ShippingAddressesAddress2Addedit   = (Int64)(dr["shipping_addresses_address_2_addedit"].ToString().Length > 0 ? Int64.Parse(dr["shipping_addresses_address_2_addedit"].ToString()) : 0);
                    dreq.ShippingAddressesAddress3Addedit   = (Int64)(dr["shipping_addresses_address_3_addedit"].ToString().Length > 0 ? Int64.Parse(dr["shipping_addresses_address_3_addedit"].ToString()) : 0);
                    dreq.ShippingAddressesAddress1Code      = dr["shipping_addresses_address_1_code"].ToString().Length > 0 ? dr["shipping_addresses_address_1_code"].ToString() : "";
                    dreq.ShippingAddressesAddress2Code      = dr["shipping_addresses_address_2_code"].ToString().Length > 0 ? dr["shipping_addresses_address_2_code"].ToString() : "";
                    dreq.ShippingAddressesAddress3Code      = dr["shipping_addresses_address_3_code"].ToString().Length > 0 ? dr["shipping_addresses_address_3_code"].ToString() : "";
                    dreq.ShippingMethodsShipVia1    = dr["shipping_methods_ship_via_1"].ToString().Length > 0 ? dr["shipping_methods_ship_via_1"].ToString() : "";
                    dreq.ShippingMethodsShipVia2    = dr["shipping_methods_ship_via_2"].ToString().Length > 0 ? dr["shipping_methods_ship_via_2"].ToString() : "";
                    dreq.ShippingMethodsShipVia3    = dr["shipping_methods_ship_via_3"].ToString().Length > 0 ? dr["shipping_methods_ship_via_3"].ToString() : "";
                    dreq.ContactFullName            = dr["ContactFullName"].ToString().Length > 0 ? dr["ContactFullName"].ToString() : "";
                    dreq.ParentSKU                  = dr["ParentSKU"].ToString().Length > 0 ? dr["ParentSKU"].ToString() : "";
                    dreq.SalesRepEmail              = dr["SalesRepEmail"].ToString().Length > 0 ? dr["SalesRepEmail"].ToString() : "";
                    dreq.GarmentTypeName            = dr["GarmentTypeName"].ToString().Length > 0 ? dr["GarmentTypeName"].ToString() : "";
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return dreq;
        }

        
        /// <summary>
        ///     Update an specific order header information with the import results status codes.
        ///     The order parameter is mandatory
        ///     The option parameter update the WasImported columnn of the order header
        ///     The option importProblem update the hasImportProblem
        /// </summary>
        /// <param name="orderId">specified the order number to be updated</param>
        /// <param name="externalOrderNo">specified the NAV order number</param>
        /// <param name="exportStatus">Exported order status (0- no/1- yes)</param>
        /// <returns></returns>
        public int updDesignRequest(string orderId, string externalOrderNo, int exportStatus)
        {
            object ret = null;
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@OrderKey", orderId.ToString());
                args.Add("@NavOrderNo", externalOrderNo.ToString());
                args.Add("@Status", exportStatus.ToString());

                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                ret = access.ExecuteScalar("dbo.MW_WSPUpdateDesignRequestExportStatus_V01", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //   
            }
            catch (Exception)
            {
                ikr.writeSyncLog(1, Convert.ToInt16(ConfigurationManager.AppSettings["ServiceID"]), 1, ConfigurationManager.AppSettings["TableName"], "Table update error, order " + orderId);
            }
            return Convert.ToInt32(ret);
        }

        /*
        /// <summary>
        ///     Get Catalog Convert orders List - Not imported orders From [designs_approve_request]
        /// </summary>
        /// <returns>List of CatConvertedOrderId's</returns>
        public List<string> getCatalogConvertOrder_NotImportedList()
        {
            SqlDataReader dr = null;
            var orderList = new List<string>();
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                //args.Add("@Order", order.ToString());
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("MW_WSPGetConvertOrdersList_notImported", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //
                while (dr != null && dr.Read())
                {
                    string orderId = null;
                    orderId = dr["CatConvertOrderId"].ToString().Length > 0 ? (dr["CatConvertOrderId"].ToString()) : "0";
                    orderList.Add(orderId);
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return orderList;
        }

        public CatalogConvertTable getCatalogConvertOrder(string order)
        {
            SqlDataReader dr = null;
            CatalogConvertTable ord = new CatalogConvertTable();
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@Order", order);
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("[MW_WSPGetConvertOrder_V01]", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //
                while (dr != null && dr.Read())
                {
                    CatalogConvertOrderHeader head = new CatalogConvertOrderHeader();
                    head.CatConvertOrderId = dr["CatConvertOrderId"].ToString().Length > 0 ? (dr["CatConvertOrderId"].ToString()) : "0";
                    head.AccountNumber = dr["AccountNumber"].ToString().Length > 0 ? dr["AccountNumber"].ToString() : "";
                    head.ActionTaken = dr["ActionTaken"].ToString().Length > 0 ? dr["ActionTaken"].ToString() : "";
                    head.AddEditFlag = (Int64)(dr["AddEditFlag"].ToString().Length > 0 ? Int64.Parse(dr["AddEditFlag"].ToString()) : 0);
                    head.CatConvertOrderId = dr["CatConvertOrderId"].ToString().Length > 0 ? dr["CatConvertOrderId"].ToString() : "";
                    head.CreateIPAddress = dr["CreateIPAddress"].ToString().Length > 0 ? dr["CreateIPAddress"].ToString() : "";
                    head.CreateUserId = dr["CreateUserId"].ToString().Length > 0 ? dr["CreateUserId"].ToString() : "";
                    head.CreationDate = dr["CreationDate"].ToString().Length > 0 ? DateTime.Parse(dr["CreationDate"].ToString()) : DateTime.Now;
                    head.CustomerEmailAddress = dr["CustomerEmailAddress"].ToString().Length > 0 ? dr["CustomerEmailAddress"].ToString() : "";
                    head.DateCreated = dr["CreationDate"].ToString().Length > 0 ? DateTime.Parse(dr["CreationDate"].ToString()) : DateTime.Now;
                    head.DesignId = dr["DesignId"].ToString().Length > 0 ? dr["DesignId"].ToString() : "";
                    head.DirtyFlags = (Int32)(dr["DirtyFlags"].ToString().Length > 0 ? Int32.Parse(dr["DirtyFlags"].ToString()) : 0);
                    head.EmailAddresses = dr["EmailAddresses"].ToString().Length > 0 ? dr["EmailAddresses"].ToString() : "";
                    head.EmailFlag = (Int64)(dr["EmailFlag"].ToString().Length > 0 ? Int64.Parse(dr["EmailFlag"].ToString()) : 0);
                    head.FileExists = (Decimal)(dr["FileExists"].ToString().Length > 0 ? Decimal.Parse(dr["FileExists"].ToString()) : 0);
                    //head.MwConvertOrderId   = (Int32)(dr["MwConvertOrderId"].ToString().Length > 0 ? Int32.Parse(dr["MwConvertOrderId"].ToString()) : 0);
                    //head.MwOrderMasterId    = (Int32)(dr["MwOrderMasterId"].ToString().Length > 0 ? Int32.Parse(dr["MwOrderMasterId"].ToString()) : 0);
                    head.Optional1 = dr["Optional1"].ToString().Length > 0 ? dr["Optional1"].ToString() : "";
                    head.Optional2 = dr["Optional2"].ToString().Length > 0 ? dr["Optional2"].ToString() : "";
                    head.Optional3 = dr["Optional3"].ToString().Length > 0 ? dr["Optional3"].ToString() : "";
                    head.Quantity = (Decimal)(dr["Quantity"].ToString().Length > 0 ? Decimal.Parse(dr["Quantity"].ToString()) : 0);
                    head.ReferenceId = dr["ReferenceId"].ToString().Length > 0 ? dr["ReferenceId"].ToString() : "";
                    head.ShipBlind = Convert.ToBoolean(dr["ShipBlind"].ToString().Length > 0 ? Convert.ToBoolean(dr["ShipBlind"].ToString()) : false);
                    head.ShipAddressLine1 = dr["ShipAddressLine1"].ToString().Length > 0 ? dr["ShipAddressLine1"].ToString() : "";
                    head.ShipAddressLine2 = dr["ShipAddressLine2"].ToString().Length > 0 ? dr["ShipAddressLine2"].ToString() : "";
                    head.ShipCity = dr["ShipCity"].ToString().Length > 0 ? dr["ShipCity"].ToString() : "";
                    head.ShipCompany = dr["ShipCompany"].ToString().Length > 0 ? dr["ShipCompany"].ToString() : "";
                    head.ShipCountry = dr["ShipCountry"].ToString().Length > 0 ? dr["ShipCountry"].ToString() : "";
                    head.ShipEmailAddress = dr["ShipEmailAddress"].ToString().Length > 0 ? dr["ShipEmailAddress"].ToString() : "";
                    head.ShipFirstName = dr["ShipFirstName"].ToString().Length > 0 ? dr["ShipFirstName"].ToString() : "";
                    head.ShipId = dr["ShipId"].ToString().Length > 0 ? dr["Optional3"].ToString() : "";
                    head.ShipLastname = dr["ShipLastname"].ToString().Length > 0 ? dr["ShipLastname"].ToString() : "";
                    head.ShipName = dr["ShipName"].ToString().Length > 0 ? dr["ShipName"].ToString() : "";
                    head.ShipPhoneNumber = dr["ShipPhoneNumber"].ToString().Length > 0 ? dr["ShipPhoneNumber"].ToString() : "";
                    head.ShipState = dr["ShipState"].ToString().Length > 0 ? dr["ShipState"].ToString() : "";
                    head.ShipVia = (Decimal)(dr["ShipVia"].ToString().Length > 0 ? Decimal.Parse(dr["ShipVia"].ToString()) : 0);
                    head.ShipViaCode = dr["ShipViaCode"].ToString().Length > 0 ? dr["ShipViaCode"].ToString() : "";
                    head.ShipZip = dr["ShipZip"].ToString().Length > 0 ? dr["ShipZip"].ToString() : "";
                    head.Sku = dr["Sku"].ToString().Length > 0 ? dr["Sku"].ToString() : "";
                    head.Timestamp = (Byte[])(dr["Timestamp"]);
                    head.UserInitials = dr["UserInitials"].ToString().Length > 0 ? dr["UserInitials"].ToString() : "";
                    //
                    ord.CatalogConvertOrderSummary = head;
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return ord;
        }

        public List<string> getCatalogDesignRequests_NotImportedList(string orderStatus, string status)
        {
            SqlDataReader dr = null;
            var orderList = new List<string>();
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@export_status", orderStatus.ToString());
                args.Add("@status", status.ToString());
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("MW_WSPGetDesignRequestsList_notImported", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //
                while (dr != null && dr.Read())
                {
                    string orderId = null;
                    orderId = dr["CatDesignRequestOrderId"].ToString().Length > 0 ? (dr["CatDesignRequestOrderId"].ToString()) : "0";
                    orderList.Add(orderId);
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return orderList;
        }

        public CatalogDesignRequestTables getCatalogDesignRequestOrder(string order)
        {
            SqlDataReader dr = null;
            CatalogDesignRequestTables ord = new CatalogDesignRequestTables();
            try
            {
                System.Collections.Specialized.NameValueCollection args = new NameValueCollection();
                args.Add("@Order", order);
                DBContext.DBAccess access = new DBContext.DBAccess();
                //
                dr = access.ExecuteReader("[MW_WSPGetConvertOrder_V01]", DBContext.DBAccess.DBConnection.SqlMainNew, args);
                //
                while (dr != null && dr.Read())
                {
                    DesignRequest head = new DesignRequest();
                    head.AIdCusNo = dr["a_id_cus_no"].ToString().Length > 0 ? (dr["a_id_cus_no"].ToString()) : "0";
                    head.AccountId = dr["AccountId"].ToString().Length > 0 ? dr["AccountId"].ToString() : "";
                    head.AddEditFlag = (Int64)(dr["AddEditFlag"].ToString().Length > 0 ? Int64.Parse(dr["AddEditFlag"].ToString()) : 0);
                    head.AnnotateAttempted = Convert.ToBoolean(dr["annotate_attempted"].ToString().Length > 0 ? Convert.ToBoolean(dr["annotate_attempted"].ToString()) : false);
                    head.AnnotateId = dr["annotate_id"].ToString().Length > 0 ? dr["annotate_id"].ToString() : "";
                    head.AnnotateInfo = dr["annotate_info"].ToString().Length > 0 ? dr["annotate_info"].ToString() : "";
                    head.AnnotateSaved = Convert.ToBoolean(dr["annotate_saved"].ToString().Length > 0 ? Convert.ToBoolean(dr["annotate_saved"].ToString()) : false);
                    head.ApprovedDate = dr["ApprovedDate"].ToString().Length > 0 ? DateTime.Parse(dr["ApprovedDate"].ToString()) : DateTime.Now;
                    head.ArtHeight = (Decimal)(dr["ArtHeight"].ToString().Length > 0 ? Decimal.Parse(dr["ArtHeight"].ToString()) : 0);
                    head.ArtWidth = (Decimal)(dr["ArtWidth"].ToString().Length > 0 ? Decimal.Parse(dr["ArtWidth"].ToString()) : 0);
                    head.Artwork = dr["Artwork"].ToString().Length > 0 ? dr["Artwork"].ToString() : "";
                    head.AtcFlagged = Convert.ToBoolean(dr["atc_flagged"].ToString().Length > 0 ? Convert.ToBoolean(dr["atc_flagged"].ToString()) : false);
                    head.Backing2Id = dr["Backing2Id"].ToString().Length > 0 ? dr["Backing2Id"].ToString() : "";
                    head.BackingId = dr["BackingId"].ToString().Length > 0 ? dr["BackingId"].ToString() : "";
                    head.Border2Id = dr["Border2Id"].ToString().Length > 0 ? dr["Border2Id"].ToString() : "";
                    head.BorderId = dr["BorderId"].ToString().Length > 0 ? dr["BorderId"].ToString() : "";
                    head.CatDesignRequestOrderId = dr["CatDesignRequestOrderId"].ToString().Length > 0 ? dr["CatDesignRequestOrderId"].ToString() : "";
                    head.ColorChange = Convert.ToBoolean(dr["ColorChange"].ToString().Length > 0 ? Convert.ToBoolean(dr["ColorChange"].ToString()) : false);
                    head.ColorInstructions = dr["ColorInstructions"].ToString().Length > 0 ? dr["ColorInstructions"].ToString() : "";
                    head.ColorPreference = dr["ColorPreference"].ToString().Length > 0 ? dr["ColorPreference"].ToString() : "";
                    head.CompleteDate = dr["CompleteDate"].ToString().Length > 0 ? DateTime.Parse(dr["CompleteDate"].ToString()) : DateTime.Now;
                    head.CompletedDesignNormal = dr["CompletedDesignNormal"].ToString().Length > 0 ? dr["CompletedDesignNormal"].ToString() : "";
                    head.CompletedPictureThumbnail = dr["CompletedPictureThumbnail"].ToString().Length > 0 ? dr["CompletedPictureThumbnail"].ToString() : "";
                    head.ContactFullName = dr["ContactFullName"].ToString().Length > 0 ? dr["ContactFullName"].ToString() : "";
                    head.Contacts = dr["Contacts"].ToString().Length > 0 ? dr["Contacts"].ToString() : "";
                    head.CreateDate = dr["CreateDate"].ToString().Length > 0 ? DateTime.Parse(dr["CreateDate"].ToString()) : DateTime.Now;
                    head.CustomerId = dr["CustomerId"].ToString().Length > 0 ? dr["CustomerId"].ToString() : "";
                    head.CustomerType = dr["CustomerType"].ToString().Length > 0 ? dr["CustomerType"].ToString() : "";
                    head.DeliveryOption = dr["DeliveryOption"].ToString().Length > 0 ? dr["DeliveryOption"].ToString() : "";
                    head.Description = dr["Description"].ToString().Length > 0 ? dr["Description"].ToString() : "";
                    head.DesignId = dr["DesignId"].ToString().Length > 0 ? dr["DesignId"].ToString() : "";
                    head.DiecodeId = dr["DiecodeId"].ToString().Length > 0 ? dr["DiecodeId"].ToString() : "";
                    head.DpNotificationSent = Convert.ToBoolean(dr["DpNotificationSent"].ToString().Length > 0 ? Convert.ToBoolean(dr["DpNotificationSent"].ToString()) : false);
                    head.DpSent = dr["DpSent"].ToString().Length > 0 ? DateTime.Parse(dr["DpSent"].ToString()) : DateTime.Now;
                    head.DrBackingWeiId = dr["dr_backing_wei_id"].ToString().Length > 0 ? dr["dr_backing_wei_id"].ToString() : "";
                    head.DrBordersWeiId = dr["dr_borders_wei_id"].ToString().Length > 0 ? dr["dr_borders_wei_id"].ToString() : "";
                    head.DrDropperLayoutsWeiId = dr["dr_dropper_layouts_wei_id"].ToString().Length > 0 ? dr["dr_dropper_layouts_wei_id"].ToString() : "";
                    head.DrEditReasonsWeiId = dr["dr_edit_reasons_wei_id"].ToString().Length > 0 ? dr["dr_edit_reasons_wei_id"].ToString() : "";
                    head.DrFabricTypesWeiId = dr["dr_fabric_types_wei_id"].ToString().Length > 0 ? dr["dr_fabric_types_wei_id"].ToString() : "";
                    head.DrFabricsWeiId = dr["dr_fabrics_wei_id"].ToString().Length > 0 ? dr["dr_fabrics_wei_id"].ToString() : "";
                    head.DrImpacCodesWeiId = dr["dr_impac_codes_wei_id"].ToString().Length > 0 ? dr["dr_impac_codes_wei_id"].ToString() : "";
                    head.DrProductClassesWeiId = dr["dr_product_classes_wei_id"].ToString().Length > 0 ? dr["dr_product_classes_wei_id"].ToString() : "";
                    head.DrProductTypeWeiId = dr["dr_product_type_wei_id"].ToString().Length > 0 ? dr["dr_product_type_wei_id"].ToString() : "";
                    head.DrShapesName = dr["dr_shapes_name"].ToString().Length > 0 ? dr["dr_shapes_name"].ToString() : "";
                    head.DrShapesSizesWeiId = dr["dr_shapes_sizes_wei_id"].ToString().Length > 0 ? dr["dr_shapes_sizes_wei_id"].ToString() : "";
                    head.DrShapesStandard = dr["dr_shapes_standard"].ToString().Length > 0 ? dr["dr_shapes_standard"].ToString() : "";
                    head.DrStitchFonts1WeiId = dr["dr_stitch_fonts1_wei_id"].ToString().Length > 0 ? dr["dr_stitch_fonts1_wei_id"].ToString() : "";
                    head.DrStitchFonts2WeiId = dr["dr_stitch_fonts2_wei_id"].ToString().Length > 0 ? dr["dr_stitch_fonts2_wei_id"].ToString() : "";
                    head.DrStitchFonts3WeiId = dr["dr_stitch_fonts3_wei_id"].ToString().Length > 0 ? dr["dr_stitch_fonts3_wei_id"].ToString() : "";
                    head.DroperLayoutId = dr["DroperLayoutId"].ToString().Length > 0 ? dr["DroperLayoutId"].ToString() : "";
                    head.DrshId = dr["drsh_id"].ToString().Length > 0 ? dr["drsh_id"].ToString() : "";
                    head.DrsiId = dr["drsi_id"].ToString().Length > 0 ? dr["drsi_id"].ToString() : "";
                    head.EditLock = Convert.ToBoolean(dr["EditLock"].ToString().Length > 0 ? Convert.ToBoolean(dr["EditLock"].ToString()) : false);
                    head.EditReason2Id = dr["EditReason2Id"].ToString().Length > 0 ? dr["EditReason2Id"].ToString() : "";
                    head.EditReasonComments = dr["EditReasonComments"].ToString().Length > 0 ? dr["EditReasonComments"].ToString() : "";
                    head.EditReasonId = dr["EditReasonId"].ToString().Length > 0 ? dr["EditReasonId"].ToString() : "";
                    head.ElectronicSimulation = Convert.ToBoolean(dr["ElectronicSimulation"].ToString().Length > 0 ? Convert.ToBoolean(dr["ElectronicSimulation"].ToString()) : false);
                    head.EmailConfirmation = dr["EmailConfirmation"].ToString().Length > 0 ? dr["EmailConfirmation"].ToString() : "";

                    head.EmailsElectronicSimulation = dr["EmailsElectronicSimulation"].ToString().Length > 0 ? dr["EmailsElectronicSimulation"].ToString() : "";

                    head.EmbellishmentGroupsDs = dr["embellishment_groups_ds"].ToString().Length > 0 ? dr["embellishment_groups_ds"].ToString() : "";
                    head.EmbellishmentGroupId = dr["EmbellishmentGroupId"].ToString().Length > 0 ? dr["EmbellishmentGroupId"].ToString() : "";

                    head.EmbellishmentGroupRefId = dr["EmbellishmentGroupRefId"].ToString().Length > 0 ? dr["EmbellishmentGroupRefId"].ToString() : "";

                    head.EmployeeId = dr["EmployeeId"].ToString().Length > 0 ? dr["EmployeeId"].ToString() : "";
                    head.EmployeeUserName = dr["EmployeeUserName"].ToString().Length > 0 ? dr["EmployeeUserName"].ToString() : "";
                    head.ErrorDescription = dr["ErrorDescription"].ToString().Length > 0 ? dr["ErrorDescription"].ToString() : "";
                    head.ErrorFlag = Convert.ToBoolean(dr["ErrorFlag"].ToString().Length > 0 ? Convert.ToBoolean(dr["ErrorFlag"].ToString()) : false);
                    head.ExpectedShipDate = dr["ExpectedShipDate"].ToString().Length > 0 ? DateTime.Parse(dr["ExpectedShipDate"].ToString()) : DateTime.Now;
                    head.ExportDr = Convert.ToBoolean(dr["export_dr"].ToString().Length > 0 ? Convert.ToBoolean(dr["export_dr"].ToString()) : false);
                    head.ExportStatus = dr["ExportStatus"].ToString().Length > 0 ? dr["ExportStatus"].ToString() : "";
                    head.Fabric2Id = dr["Fabric2Id"].ToString().Length > 0 ? dr["Fabric2Id"].ToString() : "";
                    head.FabricId = dr["FabricId"].ToString().Length > 0 ? dr["FabricId"].ToString() : "";
                    head.FabricType2Id = dr["FabricType2Id"].ToString().Length > 0 ? dr["FabricType2Id"].ToString() : "";
                    head.FabricTypeId = dr["FabricTypeId"].ToString().Length > 0 ? dr["FabricTypeId"].ToString() : "";
                    head.FavoritesAudience = dr["FavoritesAudience"].ToString().Length > 0 ? dr["FavoritesAudience"].ToString() : "";
                    head.FavoritesCreateDate = dr["FavoritesCreateDate"].ToString().Length > 0 ? DateTime.Parse(dr["FavoritesCreateDate"].ToString()) : DateTime.Now;
                    head.FavoritesNickName = dr["FavoritesNickName"].ToString().Length > 0 ? dr["FavoritesNickName"].ToString() : "";
                    head.FormNm = dr["form_nm"].ToString().Length > 0 ? dr["form_nm"].ToString() : "";
                    head.GarmentPlacementsWeiId = dr["garment_placements_wei_id"].ToString().Length > 0 ? dr["garment_placements_wei_id"].ToString() : "";
                    head.GarmentPlacementId = dr["GarmentPlacementId"].ToString().Length > 0 ? dr["GarmentPlacementId"].ToString() : "";
                    head.GarmentType = dr["GarmentType"].ToString().Length > 0 ? dr["GarmentType"].ToString() : "";
                    head.GarmentTypeId = dr["GarmentTypeId"].ToString().Length > 0 ? dr["GarmentTypeId"].ToString() : "";
                    head.GarmentTypeName = dr["GarmentTypeName"].ToString().Length > 0 ? dr["GarmentTypeName"].ToString() : "";

                    head.HasConfirmationEmailBeenSent = Convert.ToBoolean(dr["HasConfirmationEmailBeenSent"].ToString().Length > 0 ? Convert.ToBoolean(dr["HasConfirmationEmailBeenSent"].ToString()) : false);

                    head.HasDRBeenProcessed = Convert.ToBoolean(dr["HasDRBeenProcessed"].ToString().Length > 0 ? Convert.ToBoolean(dr["HasDRBeenProcessed"].ToString()) : false);
                    head.Height = dr["Height"].ToString().Length > 0 ? dr["Height"].ToString() : "";
                    head.ImpacCodeId = dr["ImpacCodeId"].ToString().Length > 0 ? dr["ImpacCodeId"].ToString() : "";
                    head.ImpacCodes = dr["ImpacCodes"].ToString().Length > 0 ? dr["ImpacCodes"].ToString() : "";
                    head.ItemsToDecorate = dr["ItemsToDecorate"].ToString().Length > 0 ? dr["ItemsToDecorate"].ToString() : "";
                    head.KnowSize = Convert.ToBoolean(dr["KnowSize"].ToString().Length > 0 ? Convert.ToBoolean(dr["KnowSize"].ToString()) : false);
                    head.LatestRecord = Convert.ToBoolean(dr["LatestRecord"].ToString().Length > 0 ? Convert.ToBoolean(dr["LatestRecord"].ToString()) : false);
                    head.LegalAcceptTerms = Convert.ToBoolean(dr["LegalAcceptTerms"].ToString().Length > 0 ? Convert.ToBoolean(dr["LegalAcceptTerms"].ToString()) : false);
                    head.LegalAgreement = dr["LegalAgreement"].ToString().Length > 0 ? dr["LegalAgreement"].ToString() : "";
                    head.LocationChange = Convert.ToBoolean(dr["LocationChange"].ToString().Length > 0 ? Convert.ToBoolean(dr["LocationChange"].ToString()) : false);
                    head.MaintainProportions = dr["MaintainProportions"].ToString().Length > 0 ? dr["MaintainProportions"].ToString() : "";
                    head.Misc = dr["Misc"].ToString().Length > 0 ? dr["Misc"].ToString() : "";
                    head.MobileAppDesign = dr["MobileAppDesign"].ToString().Length > 0 ? dr["MobileAppDesign"].ToString() : "";
                    // head.MwDesignRequestOrderId = (Int32)(dr["MwDesignRequestOrderId"].ToString().Length > 0 ? Int32.Parse(dr["MwDesignRequestOrderId"].ToString()) : 0);
                    //head.MwOrderMasterId = (Int32)(dr["MwOrderMasterId"].ToString().Length > 0 ? Int32.Parse(dr["MwOrderMasterId"].ToString()) : 0);
                    head.Optional1 = dr["Optional1"].ToString().Length > 0 ? dr["Optional1"].ToString() : "";
                    head.Optional2 = dr["Optional2"].ToString().Length > 0 ? dr["Optional2"].ToString() : "";
                    head.Optional3 = dr["Optional3"].ToString().Length > 0 ? dr["Optional3"].ToString() : "";
                    head.Optional4 = dr["Optional4"].ToString().Length > 0 ? dr["Optional4"].ToString() : "";
                    head.Optional5 = dr["Optional5"].ToString().Length > 0 ? dr["Optional5"].ToString() : "";
                    head.Optional6 = dr["Optional6"].ToString().Length > 0 ? dr["Optional6"].ToString() : "";
                    head.Optional7 = dr["Optional7"].ToString().Length > 0 ? dr["Optional7"].ToString() : "";
                    head.Optional8 = dr["Optional8"].ToString().Length > 0 ? dr["Optional8"].ToString() : "";
                    head.Optional10 = dr["Optional10"].ToString().Length > 0 ? dr["Optional10"].ToString() : "";
                    head.OptionalArtFont1 = dr["OptionalArtFont1"].ToString().Length > 0 ? dr["OptionalArtFont1"].ToString() : "";
                    head.OptionalArtFont2 = dr["OptionalArtFont2"].ToString().Length > 0 ? dr["OptionalArtFont2"].ToString() : "";
                    head.OptionalArtFont3 = dr["OptionalArtFont3"].ToString().Length > 0 ? dr["OptionalArtFont3"].ToString() : "";
                    head.OptionalArtText1 = dr["OptionalArtText1"].ToString().Length > 0 ? dr["OptionalArtText1"].ToString() : "";
                    head.OptionalArtText2 = dr["OptionalArtText2"].ToString().Length > 0 ? dr["OptionalArtText2"].ToString() : "";
                    head.OptionalArtText3 = dr["OptionalArtText3"].ToString().Length > 0 ? dr["OptionalArtText3"].ToString() : "";
                    head.ParentDesignRequestId = dr["ParentDesignRequestId"].ToString().Length > 0 ? dr["ParentDesignRequestId"].ToString() : "";
                    head.ParentSKU = dr["ParentSKU"].ToString().Length > 0 ? dr["ParentSKU"].ToString() : "";
                    head.PlacementId = dr["PlacementId"].ToString().Length > 0 ? dr["PlacementId"].ToString() : "";
                    head.PONumber = dr["PONumber"].ToString().Length > 0 ? dr["PONumber"].ToString() : "";
                    head.Price = dr["Price"].ToString().Length > 0 ? dr["Price"].ToString() : "";
                    head.PricingDescription = dr["PricingDescription"].ToString().Length > 0 ? dr["PricingDescription"].ToString() : "";
                    head.PrintedDate = dr["PrintedDate"].ToString().Length > 0 ? DateTime.Parse(dr["PrintedDate"].ToString()) : DateTime.Now;
                    head.Priority = (Decimal)(dr["Priority"].ToString().Length > 0 ? Decimal.Parse(dr["Priority"].ToString()) : 0);
                    head.ProductClass = dr["ProductClass"].ToString().Length > 0 ? dr["ProductClass"].ToString() : "";
                    head.ProductClassId = dr["ProductClassId"].ToString().Length > 0 ? dr["ProductClassId"].ToString() : "";
                    head.ProductType = dr["ProductType"].ToString().Length > 0 ? dr["ProductType"].ToString() : "";
                    head.ProductTypeId = dr["ProductTypeId"].ToString().Length > 0 ? dr["ProductTypeId"].ToString() : "";
                    head.RecordType = dr["RecordType"].ToString().Length > 0 ? dr["RecordType"].ToString() : "";
                    head.Remake = dr["Remake"].ToString().Length > 0 ? dr["Remake"].ToString() : "";
                    head.RequestFor = dr["RequestFor"].ToString().Length > 0 ? dr["RequestFor"].ToString() : "";
                    head.Requestor = dr["Requestor"].ToString().Length > 0 ? dr["Requestor"].ToString() : "";
                    head.SafetyStripesId = dr["SafetyStripesId"].ToString().Length > 0 ? dr["SafetyStripesId"].ToString() : "";

                    head.SalesRepEmail = dr["SalesRepEmail"].ToString().Length > 0 ? dr["SalesRepEmail"].ToString() : "";

                    head.SampleShippedNotificationSent = Convert.ToBoolean(dr["SampleShippedNotificationSent"].ToString().Length > 0 ? Convert.ToBoolean(dr["SampleShippedNotificationSent"].ToString()) : false);

                    head.SampleShippedSent = dr["SampleShippedSent"].ToString().Length > 0 ? DateTime.Parse(dr["SampleShippedSent"].ToString()) : DateTime.Now;
                    head.SapCustomerName = dr["SapCustomerName"].ToString().Length > 0 ? dr["SapCustomerName"].ToString() : "";
                    head.SapCustomerNo = dr["SapCustomerNo"].ToString().Length > 0 ? dr["SapCustomerNo"].ToString() : "";

                    head.SendDRConfirmationEmail = dr["SendDRConfirmationEmail"].ToString().Length > 0 ? dr["SendDRConfirmationEmail"].ToString() : "";

                    head.SessionId = dr["SessionId"].ToString().Length > 0 ? dr["SessionId"].ToString() : "";
                    head.Shapes = dr["Shapes"].ToString().Length > 0 ? dr["Shapes"].ToString() : "";
                    head.ShapeSize = dr["ShapeSize"].ToString().Length > 0 ? dr["ShapeSize"].ToString() : "";
                    head.ShipAddress1 = dr["ShipAddress1"].ToString().Length > 0 ? dr["ShipAddress1"].ToString() : "";
                    head.ShipAddress2 = dr["ShipAddress2"].ToString().Length > 0 ? dr["ShipAddress2"].ToString() : "";
                    head.ShipAddress3 = dr["ShipAddress3"].ToString().Length > 0 ? dr["ShipAddress3"].ToString() : "";
                    head.ShipEmailAddress1 = dr["ShipEmailAddress1"].ToString().Length > 0 ? dr["ShipEmailAddress1"].ToString() : "";
                    head.ShipEmailAddress2 = dr["ShipEmailAddress2"].ToString().Length > 0 ? dr["ShipEmailAddress2"].ToString() : "";
                    head.ShipEmailAddress3 = dr["ShipEmailAddress3"].ToString().Length > 0 ? dr["ShipEmailAddress3"].ToString() : "";

                    head.ShippingAddressesAddress1Addedit = (Int64)(dr["shipping_addresses_address_1_addedit"].ToString().Length > 0 ? Int64.Parse(dr["shipping_addresses_address_1_addedit"].ToString()) : 0);
                    head.ShippingAddressesAddress1Code = dr["shipping_addresses_address_1_code"].ToString().Length > 0 ? dr["shipping_addresses_address_1_code"].ToString() : "";
                    head.ShippingAddressesAddress2Addedit = (Int64)(dr["shipping_addresses_address_2_addedit"].ToString().Length > 0 ? Int64.Parse(dr["shipping_addresses_address_2_addedit"].ToString()) : 0);
                    head.ShippingAddressesAddress2Code = dr["shipping_addresses_address_2_code"].ToString().Length > 0 ? dr["shipping_addresses_address_2_code"].ToString() : "";
                    head.ShippingAddressesAddress3Addedit = (Int64)(dr["shipping_addresses_address_3_addedit"].ToString().Length > 0 ? Int64.Parse(dr["shipping_addresses_address_3_addedit"].ToString()) : 0);
                    head.ShippingAddressesAddress3Code = dr["shipping_addresses_address_3_code"].ToString().Length > 0 ? dr["shipping_addresses_address_3_code"].ToString() : "";
                    head.ShippingMethodsShipVia1 = dr["shipping_methods_ship_via_1"].ToString().Length > 0 ? dr["shipping_methods_ship_via_1"].ToString() : "";
                    head.ShippingMethodsShipVia2 = dr["shipping_methods_ship_via_2"].ToString().Length > 0 ? dr["shipping_methods_ship_via_2"].ToString() : "";
                    head.ShippingMethodsShipVia3 = dr["shipping_methods_ship_via_3"].ToString().Length > 0 ? dr["shipping_methods_ship_via_3"].ToString() : "";

                    head.ShipQty1 = dr["ShipQty1"].ToString().Length > 0 ? dr["ShipQty1"].ToString() : "";
                    head.ShipQty2 = dr["ShipQty2"].ToString().Length > 0 ? dr["ShipQty2"].ToString() : "";
                    head.ShipQty3 = dr["ShipQty3"].ToString().Length > 0 ? dr["ShipQty3"].ToString() : "";
                    head.ShipViaMethod1 = dr["ShipViaMethod1"].ToString().Length > 0 ? dr["ShipViaMethod1"].ToString() : "";
                    head.ShipViaMethod2 = dr["ShipViaMethod2"].ToString().Length > 0 ? dr["ShipViaMethod2"].ToString() : "";
                    head.ShipViaMethod3 = dr["ShipViaMethod3"].ToString().Length > 0 ? dr["ShipViaMethod3"].ToString() : "";
                    head.SizeShapeId = dr["SizeShapeId"].ToString().Length > 0 ? dr["SizeShapeId"].ToString() : "";
                    head.Sku = dr["Sku"].ToString().Length > 0 ? dr["Sku"].ToString() : "";
                    head.SpecialInstructions = dr["SpecialInstructions"].ToString().Length > 0 ? dr["SpecialInstructions"].ToString() : "";
                    head.Status = dr["Status"].ToString().Length > 0 ? dr["Status"].ToString() : "";
                    head.StitchFont1Id = dr["StitchFont1Id"].ToString().Length > 0 ? dr["StitchFont1Id"].ToString() : "";
                    head.StitchFont2Id = dr["StitchFont2Id"].ToString().Length > 0 ? dr["StitchFont2Id"].ToString() : "";
                    head.StitchFont3Id = dr["StitchFont3Id"].ToString().Length > 0 ? dr["StitchFont3Id"].ToString() : "";
                    head.SubmittedByAIdCusNo = dr["submitted_by_a_id_cus_no"].ToString().Length > 0 ? dr["submitted_by_a_id_cus_no"].ToString() : "";
                    head.SubmittedByAccountId = dr["SubmittedByAccountId"].ToString().Length > 0 ? dr["SubmittedByAccountId"].ToString() : "";
                    head.SubmittedByCustomerId = dr["SubmittedByCustomerId"].ToString().Length > 0 ? dr["SubmittedByCustomerId"].ToString() : "";
                    head.SyndicatedAcctName = dr["syndicated_acct_name"].ToString().Length > 0 ? dr["syndicated_acct_name"].ToString() : "";
                    head.SyndicatedAccountNo = dr["SyndicatedAccountNo"].ToString().Length > 0 ? dr["SyndicatedAccountNo"].ToString() : "";
                    head.SyndicatedSitename = dr["SyndicatedSitename"].ToString().Length > 0 ? dr["SyndicatedSitename"].ToString() : "";
                    head.SyndicatedUsername = dr["SyndicatedUsername"].ToString().Length > 0 ? dr["SyndicatedUsername"].ToString() : "";
                    head.ThreadInkColors = dr["ThreadInkColors"].ToString().Length > 0 ? dr["ThreadInkColors"].ToString() : "";
                    head.TrackingNumber1 = dr["TrackingNumber1"].ToString().Length > 0 ? dr["TrackingNumber1"].ToString() : "";
                    head.TrackingNumber2 = dr["TrackingNumber2"].ToString().Length > 0 ? dr["TrackingNumber2"].ToString() : "";
                    head.TrackingNumber3 = dr["TrackingNumber3"].ToString().Length > 0 ? dr["TrackingNumber3"].ToString() : "";
                    head.TrackingXml = dr["TrackingXml"].ToString().Length > 0 ? dr["TrackingXml"].ToString() : "";
                    head.WECustomerNo = dr["WECustomerNo"].ToString().Length > 0 ? dr["WECustomerNo"].ToString() : "";
                    head.WeiDrId = (Decimal)(dr["wei_dr_id"].ToString().Length > 0 ? Decimal.Parse(dr["wei_dr_id"].ToString()) : 0);
                    head.Width = dr["Width"].ToString().Length > 0 ? dr["Width"].ToString() : "";
                    //
                    ord.CatalogDesignRequest = head;
                }
            }
            finally
            {
                if ((dr != null) && (!dr.IsClosed))
                    dr.Close();
            }
            return ord;
        }  
        */
    }
}
