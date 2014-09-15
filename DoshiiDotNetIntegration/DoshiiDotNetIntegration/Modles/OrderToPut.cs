using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Modles
{
    public class OrderToPut : JsonSerializationBase<Checkin>
    {
        public string status { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
