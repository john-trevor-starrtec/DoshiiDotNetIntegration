using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// A base class to make available json.net serialization functions. 
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    internal abstract class JsonSerializationBase<TSelf> 
    {
        /// <summary>
        /// Deserializes a json string into the derived class
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static TSelf deseralizeFromJson(string json)
        {
            TSelf deserailzedObject = JsonConvert.DeserializeObject<TSelf>(json);
            
            return deserailzedObject;
        }
        
        /// <summary>
        /// Serializes the derived class into a json string. 
        /// </summary>
        /// <returns></returns>
        public virtual string ToJsonString()
        {
            string json = "";
            try
            {
                json = JsonConvert.SerializeObject(this);
                
            }
            catch (Exception ex)
            {
                
            }
            return json;
            
        }

        
    }
}
