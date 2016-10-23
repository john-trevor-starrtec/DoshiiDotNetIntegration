using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models.Json
{

    /// <summary>
    /// Payments that are made on the Doshii check
    /// </summary>
    [DataContract]
    [Serializable]
    internal class JsonTransaction : JsonSerializationBase<JsonTransaction>
    {
        /// <summary>
        /// Unique number identifying this resource
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// identify the order this transaction relates to
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        /// <summary>
        /// info about the payment
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "reference")]
        public string Reference { get; set; }

        /// <summary>
        /// partner identifier for the transaction
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "invoice")]
        public string Invoice { get; set; }

        /// <summary>
        /// The amount that has been paid in cents. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "amount")]
        public string PaymentAmount { get; set; }

        /// <summary>
        /// flag indicating if the pos will accept less than the total amount as a payment from the partner
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "acceptLess")]
        public bool AcceptLess { get; set; }

        /// <summary>
        /// flag indicating if the transaction was initiated by the partner
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "partnerInitiated")]
        public bool PartnerInitiated { get; set; }

        /// <summary>
        /// identifier for the partner that completed the transaction
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "partner")]
        public string Partner { get; set; }

        /// <summary>
        /// The current transaction status, pending, waiting, complete
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// An obfuscated string representation of the version of the order in Doshii.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        /// <summary>
        /// The URI of the order
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "tip")]
        public string Tip { get; set; }

        #region serialize methods

        public bool ShouldSerializePartner()
        {
            return false;
        }

        public bool ShouldSerializeVersion()
        {
            return (!string.IsNullOrEmpty(Version));
        }

        public bool ShouldSerializeOrderId()
        {
            return (!string.IsNullOrEmpty(OrderId));
        }

        public bool ShouldSerializeReference()
        {
            return (!string.IsNullOrEmpty(Reference));
        }

        public bool ShouldSerializeUri()
        {
            return false;
        }

        public bool ShouldSerializePartnerInitiated()
        {
            return false;
        }

        public bool ShouldSerializeId()
        {
            return false;
            
        }

        /*public bool ShouldSerializeAcceptLess()
        {
            return false;
        }*/
        
        public bool ShouldSerializeInvoice()
        {
            return (!string.IsNullOrEmpty(Invoice));
        }
        #endregion

        


    }
}
