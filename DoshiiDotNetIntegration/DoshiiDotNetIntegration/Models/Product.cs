using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// the doshii representation of a product - (an item that can be sold)
    /// </summary>
    [DataContract]
    [Serializable]
    public class Product : JsonSerializationBase<Product>
    {
        /// <summary>
        /// The Doshii Id of the prduct this will be provided by Doshii
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The internal Id of the product
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "pos_id")]
        public string PosId { get; set; }

        /// <summary>
        /// The name of the product that will be displayed to the user on the Doshii application
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// A list of the tags the product should be displayed under in the mobile menu
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; }

        /// <summary>
        /// The price the product will be sold for through the mobile app, 
        /// This price is to be represented in cents. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "price")]
        public string Price { get; set; }

        /// <summary>
        /// A description of the product that will be displayed on the mobile app
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// a list of varient lists the customer can choose from to modify their product.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "product_options")]
        public List<ProductOptions> ProductOptions { get; set; }

        /// <summary>
        /// additional instructions added by the customer
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "additional_instructions")]
        public string AdditionalInstructions { get; set; }

        /// <summary>
        /// the reason the product was rejected by the pos
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "rejection_reason")]
        public string RejectionReason { get; set; }

        /// <summary>
        /// the status of the item that is being ordered. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        public Product()
        {
            Tags = new List<string>();
            ProductOptions = new List<ProductOptions>();
        }


        #region conditional json serialization



        /// <summary>
        /// determians if the 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeAdditionalInstructions()
        {
            return false;
        }

        bool SerializeStatus = true;

        public bool ShouldSerializeStatus()
        {
            return SerializeStatus;
        }

        /// <summary>
        /// this is no longer required to be stored by the pos. 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeId()
        {
            return false;
        }

        private bool SerializeRejectionReason = false;

        public bool ShouldSerializeRejectionReason()
        {
            if (SerializeRejectionReason)
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
            SerializeRejectionReason = true;
            SerializeStatus = true;
            SetSerializeForOrderForProductOptions(true);
            return base.ToJsonString();
        }

        public string ToJsonSTringForOrder()
        {
            SerializeRejectionReason = false;
            SerializeStatus = true;
            SetSerializeForOrderForProductOptions(true);
            return base.ToJsonString();
        }

        private void SetSerializeForOrderForProductOptions(bool isForOrder)
        {
            foreach (ProductOptions po in ProductOptions)
            {
                po.SetSerializeForOrder(isForOrder);
            }
        }

        public void SetSerializeSettingsForOrder()
        {
            SerializeRejectionReason = false;
            SerializeStatus = true;
            SetSerializeForOrderForProductOptions(true);
        }

        public string ToJsonStringForProductSync()
        {
            SerializeRejectionReason = false;
            SerializeStatus = false;
            SetSerializeForOrderForProductOptions(false);
            return base.ToJsonString();
        }
        #endregion
    }
}
