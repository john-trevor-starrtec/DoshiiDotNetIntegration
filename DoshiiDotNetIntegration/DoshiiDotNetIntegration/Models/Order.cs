using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models
{
    public class Order : JsonSerializationBase<Order>
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "invoiceId")]
        public string InvoiceId{ get; set; }
        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }
        [JsonProperty(PropertyName = "checkinId")]
        public string CheckinId { get; set; }
        [JsonProperty(PropertyName = "tip")]
        public string Tip { get; set; }
        [JsonProperty(PropertyName = "paySplits")]
        public string PaySplits { get; set; }
        [JsonProperty(PropertyName = "splitWays")]
        public string SplitWays { get; set; }
        [JsonProperty(PropertyName = "payTotal")]
        public string PayTotal { get; set; }
        [JsonProperty(PropertyName = "notPayingTotal")]
        public string NotPayingTotal { get; set; }
        [JsonProperty(PropertyName = "items")]
        public List<Product> Items { get; set; }
    }
}
