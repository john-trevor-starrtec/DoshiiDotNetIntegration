using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// represents an app that a member is registered with. 
    /// </summary>
    public class App
    {
        /// <summary>
        /// the Id of the partner app
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// the name of the partner app
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the amount of points available for the member on this app
        /// </summary>
        public decimal Points { get; set; }

        public decimal Ref { get; set; }
    }
}
