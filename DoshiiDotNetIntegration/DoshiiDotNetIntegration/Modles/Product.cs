using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Modles
{
    public class product : JsonSerializationBase<product>
    {
        /// <summary>
        /// The Doshii Id of the prduct this will be provided by Doshii
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// the time the product was updated
        /// REVIEW: (liam): check what this updated at time is used for. 
        /// </summary>
        //public DateTime updatedAt { get; set; }

        /// <summary>
        /// The internal Id of the product
        /// </summary>
        public string pos_id { get; set; }

        /// <summary>
        /// The name of the product that will be displayed to the user on the Doshii application
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// A list of the tags the product should be displayed under in the mobile menu
        /// </summary>
        List<string> tags { get; set; }

        /// <summary>
        /// The price the product will be sold for through the mobile app, 
        /// This price is to be represented in cents. 
        /// </summary>
        public string price { get; set; }

        /// <summary>
        /// A description of the product that will be displayed on the mobile app
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// a list of varient lists the customer can choose from to modify their product.
        /// </summary>
        public List<product_options> product_options { get; set; }

        /// <summary>
        /// additional instructions added by the customer
        /// </summary>
        public string additional_instructions { get; set; }

        /// <summary>
        /// the reason the product was rejected by the pos
        /// </summary>
        public string rejection_reason { get; set; }

        /// <summary>
        /// the status of the item that is being ordered. 
        /// </summary>
        public Enums.OrderStates status { get; set; }


        public product()
        {
            tags = new List<string>();
            product_options = new List<product_options>();
        }


        #region conditional json serialization



        /// <summary>
        /// determians if the 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeadditional_instructions()
        {
            return false;
        }

        bool SerializeStatus = true;

        public bool ShouldSerializestatus()
        {
            return SerializeStatus;
        }

        /// <summary>
        /// this is no longer required to be stored by the pos. 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeid()
        {
            return false;
        }

        private bool serializeRejectionReason = false;

        public bool ShouldSerializerejection_reason()
        {
            if (serializeRejectionReason)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string ToJsonStringForOrderConfirmation()
        {
            serializeRejectionReason = true;
            SerializeStatus = true;
            SetSerializeForOrderForProductOptions(true);
            return base.ToJsonString();
        }

        public string ToJsonSTringForOrder()
        {
            serializeRejectionReason = false;
            SerializeStatus = true;
            SetSerializeForOrderForProductOptions(true);
            return base.ToJsonString();
        }

        private void SetSerializeForOrderForProductOptions(bool isForOrder)
        {
            foreach (product_options po in product_options)
            {
                po.SetSerializeForOrder(isForOrder);
            }
        }

        public string ToJsonStringForProductSync()
        {
            serializeRejectionReason = false;
            SerializeStatus = false;
            SetSerializeForOrderForProductOptions(false);
            return base.ToJsonString();
        }
        #endregion
    }
}
