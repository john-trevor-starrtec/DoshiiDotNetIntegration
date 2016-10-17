using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    internal class PointsRedeem
    {
        public string OrderId { get; set; }
        public int Points { get; set; }
        public string AppId { get; set; }
    }
}
