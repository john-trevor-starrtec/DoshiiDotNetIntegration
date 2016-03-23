using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// the pos menu
    /// </summary>
    [DataContract]
    [Serializable]
    internal class JsonMenu : JsonSerializationBase<JsonMenu>
    {
        private List<JsonMenuSurcount> _surcounts;

        /// <summary>
        /// A list of all order level surcounts available in the pos menu
        /// Surcounts are discounts and surcharges / discounts should have a negative value. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "surcounts")]
        public List<JsonMenuSurcount> Surcounts
        {
            get
            {
                if (_surcounts == null)
                {
                    _surcounts = new List<JsonMenuSurcount>();
                }
                return _surcounts;
            }
            set { _surcounts = value; }
        }

        private List<JsonMenuProduct> _products;

        /// <summary>
        /// A list of all products available on the pos menu
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "products")]
        public List<JsonMenuProduct> Products
        {
            get
            {
                if (_products == null)
                {
                    _products = new List<JsonMenuProduct>();
                }
                return _products;
            }
            set { _products = value; }
        }
    }
}
