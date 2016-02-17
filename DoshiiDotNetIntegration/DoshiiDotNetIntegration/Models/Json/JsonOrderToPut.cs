using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// DO NOT USE FROM POS
    /// This model is used when either completing a PUT or a POST to update an order
    /// This model should not instantiated by the Pos and should only be used internally. 
    /// </summary>
    [DataContract]
    [Serializable]
	internal class JsonOrderToPut : JsonSerializationBase<JsonOrderToPut>
    {
		/// <summary>
		/// Private placeholder for the items in the order.
		/// </summary>
		private List<JsonProduct> mItems;

		/// <summary>
		/// Private placeholder for the surcounts in the order.
		/// </summary>
		private List<JsonSurcount> mSurcounts;

		/// <summary>
        /// The order status
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        
        /// <summary>
        /// The version number of the order in Doshii. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
        
        /// <summary>
        /// All the items included in the order. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "items")]
		public List<JsonProduct> Items 
		{
			get
			{
				if (mItems == null)
					mItems = new List<JsonProduct>();
				return mItems;
			}
			set
			{
				mItems = value;
			}
		}

        /// <summary>
        /// A list of all surcounts applied at the order level
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "surcounts")]
		public List<JsonSurcount> Surcounts 
		{
			get
			{
				if (mSurcounts == null)
					mSurcounts = new List<JsonSurcount>();
				return mSurcounts;
			}
			set
			{
				mSurcounts = value;
			}
		}

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        /// <returns></returns>
        public string ToJsonStringForOrder()
        {
			foreach (JsonProduct pro in Items)
            {
                pro.SetSerializeSettingsForOrder();
            }
            return base.ToJsonString();
        }
    }
}
