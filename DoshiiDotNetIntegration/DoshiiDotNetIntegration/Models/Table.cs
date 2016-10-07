using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Json;

namespace DoshiiDotNetIntegration.Models
{
    public class Table 
    {
        public string Name { get; set; }

        public int Covers { get; set; }
        public bool IsActive { get; set; }

        public TableCriteria Criteria { get; set; }
    }
}
