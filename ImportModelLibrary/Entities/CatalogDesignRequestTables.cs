using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{

    public class CatalogDesignRequestTables
    {
        public DesignRequest CatalogDesignRequest { get; set; }
    }

    public class DesignRequest
    {
        public string       AIdCusNo { get; set; }                  //  a_id_cus_no
        public string       AccountId { get; set; }
        public long?        AddEditFlag { get; set; }
        public bool?        AnnotateAttempted { get; set; }         //  annotate_attempted
        public string       AnnotateId { get; set; }                //  annotate_id
        public string       AnnotateInfo { get; set; }              //  annotate_info
        public bool?        AnnotateSaved { get; set; }             //  annotate_saved
        public DateTime?    ApprovedDate { get; set; }
        public decimal?     ArtHeight { get; set; }
        public decimal?     ArtWidth { get; set; }
        public string       Artwork { get; set; }
        public bool?        AtcFlagged { get; set; }                //  atc_flagged
        public string       Backing2Id { get; set; }
        public string       BackingId { get; set; }
        public string       Border2Id { get; set; }
        public string       BorderId { get; set; }
        public string       CatDesignRequestOrderId { get; set; }
        public bool?        ColorChange { get; set; }
        public string       ColorInstructions { get; set; }
        public string       ColorPreference { get; set; }
        public DateTime?    CompleteDate { get; set; }
        public string       CompletedDesignNormal { get; set; }
        public string       CompletedPictureThumbnail { get; set; }
        public string       ContactFullName { get; set; }
        public string       Contacts { get; set; }
        public DateTime?    CreateDate { get; set; }
        public bool?        CreateOrder { get; set; }
        public string       CustomerId { get; set; }
        public string       CustomerType { get; set; }
        //public DateTime?    DateCreated { get; set; }
        public string       DeliveryOption { get; set; }
        public string       Description { get; set; }
        public string       DesignId { get; set; }
        public string       DiecodeId { get; set; }
        public bool?        DpNotificationSent { get; set; }
        public DateTime?    DpSent { get; set; }
        public string       DrBackingWeiId { get; set; }            //  dr_backing_wei_id
        public string       DrBordersWeiId { get; set; }            //  dr_borders_wei_id
        public string       DrDropperLayoutsWeiId { get; set; }     //  dr_dropper_layouts_wei_id
        public string       DrEditReasonsWeiId { get; set; }        //  dr_edit_reasons_wei_id
        public string       DrFabricTypesWeiId { get; set; }        //  dr_fabric_types_wei_id
        public string       DrFabricsWeiId { get; set; }            //  dr_fabrics_wei_id
        public string       DrImpacCodesWeiId { get; set; }         //  dr_impac_codes_wei_id
        public string       DrProductClassesWeiId { get; set; }     //  dr_product_classes_wei_id
        public string       DrProductTypeWeiId { get; set; }        //  dr_product_type_wei_id
        public string       DrShapesName { get; set; }              //  dr_shapes_name
        public string       DrShapesSizesWeiId { get; set; }        //  dr_shapes_sizes_wei_id
        public string       DrShapesStandard { get; set; }          //  dr_shapes_standard
        public string       DrStitchFonts1WeiId { get; set; }       //  dr_stitch_fonts1_wei_id
        public string       DrStitchFonts2WeiId { get; set; }       //  dr_stitch_fonts2_wei_id
        public string       DrStitchFonts3WeiId { get; set; }       //  dr_stitch_fonts3_wei_id
        public string       DroperLayoutId { get; set; }
        public string       DrshId { get; set; }                    //  drsh_id
        public string       DrsiId { get; set; }                    //  drsi_id
        public bool?        EditLock { get; set; }
        public string       EditReason2Id { get; set; }
        public string       EditReasonComments { get; set; }
        public string       EditReasonId { get; set; }
        public bool?        ElectronicSimulation { get; set; }
        public string       EmailConfirmation { get; set; }
        public string       EmailsElectronicSimulation { get; set; }
        public string       EmbellishmentGroupsDs { get; set; }     //  embellishment_groups_ds
        public string       EmbellishmentGroupId { get; set; }
        public string       EmbellishmentGroupRefId { get; set; }
        public string       EmployeeId { get; set; }
        public string       EmployeeUserName { get; set; }
        public string       ErrorDescription { get; set; }
        public bool?        ErrorFlag { get; set; }
        public DateTime?    ExpectedShipDate { get; set; }
        public bool?        ExportDr { get; set; }                  //  export_dr
        public string       ExportStatus { get; set; }
        public string       Fabric2Id { get; set; }
        public string       FabricId { get; set; }
        public string       FabricType2Id { get; set; }
        public string       FabricTypeId { get; set; }
        public string       FavoritesAudience { get; set; }
        public DateTime?    FavoritesCreateDate { get; set; }
        public string       FavoritesNickName { get; set; }
        public string       FormNm { get; set; }                    //  form_nm
        public string       GarmentPlacementsWeiId { get; set; }    //  garment_placements_wei_id
        public string       GarmentPlacementId { get; set; }
        public string       GarmentType { get; set; }
        public string       GarmentTypeId { get; set; }
        public string       GarmentTypeName { get; set; }
        public bool?        HasConfirmationEmailBeenSent { get; set; }
        public bool?        HasDRBeenProcessed { get; set; }
        public string       Height { get; set; }
        public string       ImpacCodeId { get; set; }
        public string       ImpacCodes { get; set; }
        public string       ItemsToDecorate { get; set; }
        public bool?        KnowSize { get; set; }
        public bool?        LatestRecord { get; set; }
        public bool?        LegalAcceptTerms { get; set; }
        public string       LegalAgreement { get; set; }
        public bool?        LocationChange { get; set; }
        public string       MaintainProportions { get; set; }
        public string       Misc { get; set; }
        public string       MobileAppDesign { get; set; }
        public int          MwDesignRequestOrderId { get; set; }
        public int?         MwOrderMasterId { get; set; }
        public string       Optional1 { get; set; }
        public string       Optional10 { get; set; }
        public string       Optional2 { get; set; }
        public string       Optional3 { get; set; }
        public string       Optional4 { get; set; }
        public string       Optional5 { get; set; }
        public string       Optional6 { get; set; }
        public string       Optional7 { get; set; }
        public string       Optional8 { get; set; }
        public string       OptionalArtFont1 { get; set; }
        public string       OptionalArtFont2 { get; set; }
        public string       OptionalArtFont3 { get; set; }
        public string       OptionalArtText1 { get; set; }
        public string       OptionalArtText2 { get; set; }
        public string       OptionalArtText3 { get; set; }
        public string       ParentDesignRequestId { get; set; }
        public string       ParentSKU { get; set; }
        public string       PlacementId { get; set; }
        public string       PONumber { get; set; }
        public string       Price { get; set; }
        public string       PricingDescription { get; set; }
        public DateTime?    PrintedDate { get; set; }
        public decimal?     Priority { get; set; }
        public string       ProductClass { get; set; }
        public string       ProductClassId { get; set; }
        public string       ProductType { get; set; }
        public string       ProductTypeId { get; set; }
        public string       RecordType { get; set; }
        public string       Remake { get; set; }
        public string       RequestFor { get; set; }
        public string       Requestor { get; set; }
        public string       SafetyStripesId { get; set; }
        public string       SalesRep { get; set; }
        public string       SalesRepEmail { get; set; }
        public bool?        SampleShippedNotificationSent { get; set; }
        public DateTime?    SampleShippedSent { get; set; }
        public string       SapCustomerName { get; set; }
        public string       SapCustomerNo { get; set; }
        public string       SearchField1 { get; set; }
        public string       SendDRConfirmationEmail { get; set; }
        public string       SessionId { get; set; }
        public string       Shapes { get; set; }
        public string       ShapeSize { get; set; }
        public string       ShipAddress1 { get; set; }
        public string       ShipAddress2 { get; set; }
        public string       ShipAddress3 { get; set; }
        public string       ShipEmailAddress1 { get; set; }
        public string       ShipEmailAddress2 { get; set; }
        public string       ShipEmailAddress3 { get; set; }
        public long?        ShippingAddressesAddress1Addedit { get; set; }  //  shipping_addresses_address_1_addedit
        public string       ShippingAddressesAddress1Code { get; set; }     //  shipping_addresses_address_1_code
        public long?        ShippingAddressesAddress2Addedit { get; set; }  //  shipping_addresses_address_2_addedit
        public string       ShippingAddressesAddress2Code { get; set; }     //  shipping_addresses_address_2_code
        public long?        ShippingAddressesAddress3Addedit { get; set; }  //  shipping_addresses_address_3_addedit
        public string       ShippingAddressesAddress3Code { get; set; }     //  shipping_addresses_address_3_code
        public string       ShippingMethodsShipVia1 { get; set; }           //  shipping_methods_ship_via_1
        public string       ShippingMethodsShipVia2 { get; set; }           //  shipping_methods_ship_via_2
        public string       ShippingMethodsShipVia3 { get; set; }           //  shipping_methods_ship_via_3
        public string       ShipQty1 { get; set; }
        public string       ShipQty2 { get; set; }
        public string       ShipQty3 { get; set; }
        public string       ShipViaMethod1 { get; set; }
        public string       ShipViaMethod2 { get; set; }
        public string       ShipViaMethod3 { get; set; }
        public string       SizeShapeId { get; set; }
        public string       Sku { get; set; }
        public string       SpecialInstructions { get; set; }
        public string       Status { get; set; }
        public string       StitchFont1Id { get; set; }
        public string       StitchFont2Id { get; set; }
        public string       StitchFont3Id { get; set; }
        public string       SubmittedByAIdCusNo { get; set; }               //  submitted_by_a_id_cus_no
        public string       SubmittedByAccountId { get; set; }
        public string       SubmittedByCustomerId { get; set; }
        public string       SyndicatedAcctName { get; set; }                //  syndicated_acct_name
        public string       SyndicatedAccountNo { get; set; }
        public string       SyndicatedSitename { get; set; }
        public string       SyndicatedUsername { get; set; }
        public string       ThreadInkColors { get; set; }
        public string       TrackingNumber1 { get; set; }
        public string       TrackingNumber2 { get; set; }
        public string       TrackingNumber3 { get; set; }
        public string       TrackingXml { get; set; }
        public string       WECustomerNo { get; set; }
        public decimal?     WeiDrId { get; set; }                           //  wei_dr_id
        public string       Width { get; set; }
    }
}