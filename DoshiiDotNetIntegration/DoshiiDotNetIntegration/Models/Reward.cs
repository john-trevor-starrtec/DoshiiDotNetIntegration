using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Json;

namespace DoshiiDotNetIntegration.Models
{
    public class Reward 
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string SurcountType { get; set; }
        public decimal SurcountAmount { get; set; }
        
        public string AppName { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public DateTime? CreatedAt { get; set; }

        public Uri Uri { get; set; }
        
    }
}
