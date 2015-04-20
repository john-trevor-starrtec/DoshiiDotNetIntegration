using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// a doshii order
    /// </summary>
    [DataContract]
    [Serializable]
    public class Order : JsonSerializationBase<Order>
    {
        /// <summary>
        /// order id
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        
        /// <summary>
        /// order status
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        
        /// <summary>
        /// paypal invoice Id
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "invoiceId")]
        public string InvoiceId{ get; set; }
        
        /// <summary>
        /// paypal transaciton id
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }
        
        /// <summary>
        /// the checkinId the order is associated with
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "checkinId")]
        public string CheckinId { get; set; }
        
        /// <summary>
        /// any tips associated with the order
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "tip")]
        public string Tip { get; set; }

        /// <summary>
        /// this is used by doahii when splitting the bill
        /// </summary>
        [JsonProperty(PropertyName = "paySplits")]
        public string PaySplits { get; set; }

        /// <summary>
        /// this is used by doshii when splitting the bill
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "splitWays")]
        public string SplitWays { get; set; }

        /// <summary>
        /// the amount that is being paid REVIEW: (LIAM) - how will this work with multi payments on paypal. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "payTotal")]
        public string PayTotal { get; set; }

        /// <summary>
        /// the amount that has not been paid REVIEW: (LIAM) - how will this work with multi payments on paypal.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "notPayingTotal")]
        public string NotPayingTotal { get; set; }

        public  List<Product> _items;
        
        /// <summary>
        /// a list of all the items included in the order. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "items")]
        public List<Product> Items {
            get
            {
                if (_items == null)
                {
                    _items = new List<Product>();
                }
                return _items;
            }
            set { _items = value; } 
        }

        public  List<Surcount> _surcounts;

        /// <summary>
        /// a list of all surcounts applied at and order level
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "surcounts")]
        public List<Surcount> Surcounts
        {
            get
            {
                if (_surcounts == null)
                {
                    _surcounts = new List<Surcount>();
                }
                return _surcounts;
            }
            set { _surcounts = value; }
        }

        public  List<Payment> _payments;
        /// <summary>
        /// a list of all payments applied at and order level
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "payments")]
        public List<Payment> Payments {
            get
            {
                if (_payments == null)
                {
                    _payments = new List<Payment>();
                }
                return _payments;
            }
            set { _payments = value; }
        }

        /// <summary>
        /// the last time the order was updated on doshii
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "updatedAt")]
        public string UpdatedAt { get; set; }
    }
}
