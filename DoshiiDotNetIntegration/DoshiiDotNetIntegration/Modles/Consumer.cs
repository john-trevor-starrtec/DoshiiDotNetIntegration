using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Modles
{
    public class Consumer : JsonSerializationBase<Consumer>
    {
        /// <summary>
        /// the doshii id for the cusomter
        /// </summary>
        public int id { get; set; }

        public string paypalCustomerId { get; set; }

        public string name { get; set; }

        public Uri PhotoUrl { get; set; }


    }
}
