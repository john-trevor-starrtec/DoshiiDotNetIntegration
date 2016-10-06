using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Json;

namespace DoshiiDotNetIntegration.Models
{
    public class Checkin 
    {
        public string Id { get; set; }
        
        /// <summary>
        /// checkin id in the POS system
        /// </summary>
        public string Ref { get; set; }

        public List<string> TableNames { get; set; }
        public int Covers { get; set; }

        public Consumer Consumer { get; set; }

        public DateTime CompletedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public Uri Uri { get; set; }
        
    }
}
