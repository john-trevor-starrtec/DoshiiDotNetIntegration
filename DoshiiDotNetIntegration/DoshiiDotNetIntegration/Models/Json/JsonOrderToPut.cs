﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// A Doshii order
    /// </summary>
    [DataContract]
    [Serializable]
    internal class JsonOrderToPut : JsonSerializationBase<JsonOrderToPut>
    {
        /// <summary>
        /// Order status
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "phase")]
        public string Phase { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "memberId")]
        public string MemberId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "checkinId")]
        public string CheckinId { get; set; }

        /// <summary>
        /// An obfuscated string representation of the version of the order in Doshii.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }


        private List<JsonOrderSurcount> _surcounts;

		/// <summary>
		/// A list of all surcounts applied at and order level
		/// Surcounts are discounts and surcharges / discounts should have a negative value. 
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "surcounts")]
		public List<JsonOrderSurcount> Surcounts
		{
			get
			{
				if (_surcounts == null)
				{
					_surcounts = new List<JsonOrderSurcount>();
				}
				return _surcounts;
			}
			set { _surcounts = value; }
		}
        
        
		private List<JsonOrderProduct> _items;
        
        /// <summary>
        /// A list of all the items included in the order. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "items")]
		public List<JsonOrderProduct> Items
		{
            get
            {
                if (_items == null)
                {
					_items = new List<JsonOrderProduct>();
                }
                return _items;
            }
			set { _items = value; } 
        }

        #region serializeMembers

        public override string ToJsonString()
        {
            string json = "";
            try
            {
                json = JsonConvert.SerializeObject(new { order = this });

            }
            catch (Exception ex)
            {

            }
            return json;

        }

        public bool ShouldSerializeVersion()
        {
            return (!string.IsNullOrEmpty(Version));
        }

        public bool ShouldSerializePhase()
        {
            return (!string.IsNullOrEmpty(Phase));
        }

        public bool ShouldSerializeMemberId()
        {
            return (!string.IsNullOrEmpty(MemberId));
        }

        public bool ShouldSerializeCheckinId()
        {
            return (!string.IsNullOrEmpty(CheckinId));
        }

        #endregion

        
    }
}
