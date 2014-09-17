using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// a doshii order
    /// </summary>
    public class Order : JsonSerializationBase<Order>
    {
        /// <summary>
        /// order id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        
        /// <summary>
        /// order status
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        
        /// <summary>
        /// paypal invoice Id
        /// </summary>
        [JsonProperty(PropertyName = "invoiceId")]
        public string InvoiceId{ get; set; }
        
        /// <summary>
        /// paypal transaciton id
        /// </summary>
        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }
        
        /// <summary>
        /// the checkinId the order is associated with
        /// </summary>
        [JsonProperty(PropertyName = "checkinId")]
        public string CheckinId { get; set; }
        
        /// <summary>
        /// any tips associated with the order
        /// </summary>
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
        [JsonProperty(PropertyName = "splitWays")]
        public string SplitWays { get; set; }

        /// <summary>
        /// the amount that is being paid REVIEW: (LIAM) - how will this work with multi payments on paypal. 
        /// </summary>
        [JsonProperty(PropertyName = "payTotal")]
        public string PayTotal { get; set; }

        /// <summary>
        /// the amount that has not been paid REVIEW: (LIAM) - how will this work with multi payments on paypal.
        /// </summary>
        [JsonProperty(PropertyName = "notPayingTotal")]
        public string NotPayingTotal { get; set; }
        
        /// <summary>
        /// a list of all the items included in the order. 
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public List<Product> Items { get; set; }
    }
}
