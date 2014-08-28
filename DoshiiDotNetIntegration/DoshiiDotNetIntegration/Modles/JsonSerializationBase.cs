using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Modles
{
    public abstract class JsonSerializationBase<TSelf> 
    {
        
        public static TSelf deseralizeFromJson(string json)
        {
            TSelf deserailzedObject = JsonConvert.DeserializeObject<TSelf>(json);
            
            return deserailzedObject;
        }
        
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
