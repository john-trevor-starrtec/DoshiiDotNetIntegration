using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Json;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// an object representing a checkin
    /// </summary>
    public class Checkin 
    {
        /// <summary>
        /// the Id of the checkin. 
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// checkin id in the POS system
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// the table names associated with the checkin
        /// </summary>
        public List<string> TableNames { get; set; }

        /// <summary>
        /// the covers associated with the checkin
        /// </summary>
        public int Covers { get; set; }

        /// <summary>
        /// the <see cref="Consumer"/> associated with the checkin
        /// </summary>
        public Consumer Consumer { get; set; }

        /// <summary>
        /// the time the checkin was completed. 
        /// </summary>
        public DateTime CompletedAt { get; set; }

        /// <summary>
        /// the last time the checkin was updated on Doshii
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// the time the checkin was created. 
        /// </summary>
        public DateTime CreatedAt { get; set; }

        public Uri Uri { get; set; }
        
    }
}
