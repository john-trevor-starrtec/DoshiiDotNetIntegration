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
            Doshii.log.Debug(string.Format("Deserializing JSON {0} {1} into {1}", json, Environment.NewLine, typeof(TSelf).FullName));
            TSelf deserailzedObject = JsonConvert.DeserializeObject<TSelf>(json);
            Doshii.log.Debug(string.Format("successfull Deserialized {0} from JSON", typeof(TSelf).FullName));

            return deserailzedObject;
        }
        
        internal string ToJsonString()
        {
            string json = "";
            try
            {
                Doshii.log.Debug(string.Format("serializing JSON of {0}", typeof(TSelf).FullName));
                json = JsonConvert.SerializeObject(this);
                Doshii.log.Debug(string.Format("{0} successfully serailzed into {2}{1}", typeof(TSelf).FullName, json, Environment.NewLine));
            }
            catch (Exception ex)
            {
                Doshii.log.Error(string.Format("There was an exception serializing {0}", typeof(TSelf).FullName), ex);
            }
            return json;
            
        }
    }
}
