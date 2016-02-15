using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// list of Payments that are made on the Doshii check
    /// </summary>
    [DataContract]
    [Serializable]
    internal class JsonTransactionList : JsonSerializationBase<JsonTransactionList>
    {
        /// <summary>
        /// Unique number identifying this resource
        /// </summary>
        [DataMember]
        public List<JsonTransaction> transactionList { get; set; }
    }
}
