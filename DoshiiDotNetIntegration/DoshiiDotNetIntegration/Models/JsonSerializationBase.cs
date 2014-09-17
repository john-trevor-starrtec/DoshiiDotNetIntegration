using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models
{
    public abstract class JsonSerializationBase<TSelf> 
    {
        /// <summary>
        /// deseralizes a json string into the derived class
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static TSelf deseralizeFromJson(string json)
        {
            TSelf deserailzedObject = JsonConvert.DeserializeObject<TSelf>(json);
            
            return deserailzedObject;
        }
        
        /// <summary>
        /// serializes the derived class into a json string. 
        /// </summary>
        /// <returns></returns>
        internal string ToJsonString()
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
