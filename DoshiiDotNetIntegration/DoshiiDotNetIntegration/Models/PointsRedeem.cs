using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// an internal class used in the process of redeeming points. 
    /// </summary>
    internal class PointsRedeem
    {
        /// <summary>
        /// the orderId the points will be redeemed against. 
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// the amount of points to be redeemed. 
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// that <see cref="App.Id"/> representing the App the points will be redeemed against. 
        /// </summary>
        public string AppId { get; set; }
    }
}
