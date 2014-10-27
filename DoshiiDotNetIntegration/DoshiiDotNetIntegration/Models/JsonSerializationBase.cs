using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// a base class to make available json.net serialization functions. 
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
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

        /// <summary>
        /// Object for thread safety when initialising <see cref="ms_XmlSerializerInstance"/>
        /// </summary>
        private static object ms_Sync = new object();

        private static System.Xml.Serialization.XmlSerializer ms_XmlSerializerInstance = null;

        public string ToStringXml()
        {
            System.Xml.Serialization.XmlSerializer serializer = XmlSerializerInstance;
            System.IO.StringWriter writer = new System.IO.StringWriter();
            serializer.Serialize(writer, this);
            string instanceAsXml = writer.ToString();
            writer = null;
            serializer = null;
            return instanceAsXml;
        }

        /// <summary>
        /// Gets the only instance of the <see cref="System.Xml.Serialization.XmlSerializer"/> for the derived non-generic type.
        /// </summary>
        /// <remarks>
        /// If there are problems with the incorrect serializer being used, this needs to be changed to the dictionary which is commented out above.
        /// </remarks>
        private static System.Xml.Serialization.XmlSerializer XmlSerializerInstance
        {
            get
            {
                System.Xml.Serialization.XmlSerializer serializer = ms_XmlSerializerInstance;
                if (serializer == null)
                {
                    lock (ms_Sync)
                    {
                        serializer = ms_XmlSerializerInstance;
                        if (ms_XmlSerializerInstance == null)
                        {
                            serializer = new System.Xml.Serialization.XmlSerializer(typeof(TSelf));
                            ms_XmlSerializerInstance = serializer;
                        }
                    }
                }
                return serializer;
            }
        }
    }
}
