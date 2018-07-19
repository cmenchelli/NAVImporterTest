using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportProcedure_DesignRequest
{
    public class Constants
    {
        public const int CONVERT_ORDER_STATUS_PROCESSED = 1;
        public const string DESIGN_REQUEST_EXPORT_STATUS_IMPORTED = "Imported";
        public const string DESIGN_REQUEST_EXPORT_STATUS_READY = "ready";
        public const string DESIGN_REQUEST_STATUS_PROCESSING = "Processing";
        public const string ERROR_FOUND_PROCESSING_DESIGN_REQUEST_DESIGN_ID_LENGTH_IS_NOT_VALID = "Error encountered while processing design request: design_id length is not 6 digits. CatDesignRequestOrderId=";
        public const string ERROR_FOUND_PROCESSING_DESIGN_REQUEST_EXCEPTION_THROWN = "Error encountered while processing design request. Exception thrown. CatDesignRequestOrderId=";
        public const string ERROR_FOUND_PROCESSING_DESIGN_REQUEST_FTP_PROBLEM = "Error encountered while processing design request: Unable to download artwork image. CatDesignRequestOrderId=";
        public const string ERROR_FOUND_PROCESSING_DESIGN_REQUEST_NO_ARTWORK_IMAGE_AVAILABLE = "Error encountered while processing design request: No artwork image info available. textbox_2_art empty. CatDesignRequestOrderId=";
        public const string ERROR_FOUND_PROCESSING_DESIGN_REQUEST_SHIPPING_ADDRESS_HAVE_NOT_BEEN_UPDATED = "Error encountered while processing design request: Waiting on Shipping Address(es) to be updated. CatDesignRequestOrderId=";
        public const string ERROR_FOUND_PROCESSING_DESIGN_REQUEST_UNABLE_TO_CREATE_TEXT_IMAGE = "Error encountered while processing design request: Unable to create the text artwork image. CatDesignRequestOrderId=";
        public const string ERROR_FOUND_UPDATING_CONVERT_ORDERS_ADD_EDIT_IN_CATALOG = "Error encountered while updating convert orders add_edit column in Catalog";
        public const string ERROR_FOUND_UPDATING_DESIGN_REQUEST_EXPORT_STATUS_IN_CATALOG = "Error encountered while updating design request export_status in Catalog";
        public const string ERROR_FOUND_UPDATING_WEB_ORDER_EXTERNAL_ORDER_NO_AND_EXPORT_STATUS_IN_CATALOG = "Error encountered while updating web order external order number and exported status columns in Catalog";
        public const string ERROR_FOUND_WHEN_DESIGN_REQUEST_SENT_TO_MIDDLEWARE = "Error encountered when design request was sent to Middleware with CatDesignRequestOrderId=";
        public const string ERROR_FOUND_WHEN_WEB_ORDER_DOES_NOT_HAVE_ANY_LINE_ITEMS = "Error encountered. Web order has no associated line items. o_key=";
        public const string ERROR_FOUND_WHEN_WEB_ORDER_WAS_SENT_TO_MIDDLEWARE = "Error encountered when web order was sent to Middleware with o_key=";
        public const string NUMBER_OF_CONVERTS_RETRIEVED_FROM_CATALOG = ". # of Converts Retrieved from Catalog = ";
        public const string NUMBER_OF_CONVERTS_UPDATED_IN_CATALOG = ". # of Converts Updated in Catalog = ";
        public const string NUMBER_OF_DESIGN_REQUESTS_RETRIEVED_FROM_CATALOG = ". # of Design Requests Retrieved from Catalog = ";
        public const string NUMBER_OF_DESIGN_REQUESTS_UPDATED_IN_CATALOG = ". # of Design Requests Updated in Catalog = ";
        public const string NUMBER_OF_WEB_ORDERS_RETRIEVED_FROM_CATALOG = ". # of Web Orders Retrieved from Catalog = ";
        public const string NUMBER_OF_WEB_ORDERS_UPDATED_IN_CATALOG = ". # of Web Orders Updated in Catalog = ";
        public const string ORDER_EXPORTED_STATUS = "1";
        public const int TASK_IS_NOT_RUNNING = 0;
        public const int TASK_IS_RUNNING = 1;
    }
}
