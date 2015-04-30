using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// The doshii representation of a product 
    /// A product is the highest level line item on orders.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Product : JsonSerializationBase<Product>
    {
        /// <summary>
        /// The Doshii Id of the prduct.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The POS Id of the product
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "pos_id")]
        public string PosId { get; set; }

        /// <summary>
        /// The name of the product.
        /// This name will be displayed to Doshii users on the mobile app.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }


        private List<string> _Tags;
        
        /// <summary>
        /// A list of the groups the product should be displayed under in the doshii mobile app
        /// This field is optional,
        /// Products can be added manualy to groups in the doshii dashboard,
        /// If this list is populated the product will be automoatically added to the groups included, this will reduce setup time. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags 
        { 
            get
            { 
                if (_Tags == null)
                {
                    _Tags = new List<string>();
                }
                return _Tags;
            }
            set
            {
                _Tags = value;
            }
        }

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

        private List<ProductOptions> _ProductOptions;
        
        /// <summary>
        /// A list of ProductOptions the customer can choose from to modify the product they are ordering.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "product_options")]
        public List<ProductOptions> ProductOptions {
            get
            {
                if (_ProductOptions == null)
                {
                    _ProductOptions = new List<ProductOptions>();
                }
                return _ProductOptions;
            } 
            set
            {
                _ProductOptions = value;
            }
        }

        /// <summary>
        /// Additional instructions added by the customer
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "additional_instructions")]
        public string AdditionalInstructions { get; set; }

        /// <summary>
        /// The reason the product was rejected by the pos
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "rejection_reason")]
        public string RejectionReason { get; set; }

        /// <summary>
        /// The status of the item that is being ordered. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Product(){}


        #region conditional json serialization
        
        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeAdditionalInstructions()
        {
            return false;
        }

        bool SerializeStatus = true;

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeStatus()
        {
            return SerializeStatus;
        }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeId()
        {
            return false;
        }

        public bool SerializeRejectionReason = false;

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        /// <returns></returns>
        public string ToJsonStringForOrderConfirmation()
        {
            SerializeRejectionReason = true;
            SerializeStatus = true;
            SetSerializeForOrderForProductOptions(true);
            return base.ToJsonString();
        }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        /// <returns></returns>
        public string ToJsonStringForOrder()
        {
            SerializeRejectionReason = false;
            SerializeStatus = true;
            SetSerializeForOrderForProductOptions(true);
            return base.ToJsonString();
        }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        /// <param name="isForOrder"></param>
        public void SetSerializeForOrderForProductOptions(bool isForOrder)
        {
            foreach (ProductOptions po in ProductOptions)
            {
                po.SetSerializeForOrder(isForOrder);
            }
        }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        public void SetSerializeSettingsForOrder()
        {
            SerializeRejectionReason = false;
            SerializeStatus = true;
            SetSerializeForOrderForProductOptions(true);
        }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        /// <returns></returns>
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
