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
    /// a doshii table
    /// </summary>
    public class Table 
    {
        /// <summary>
        /// the name of the table. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the Max amount of covers that can be seated at the table. 
        /// </summary>
        public int Covers { get; set; }

        /// <summary>
        /// a flag indicating if the table is active in the pos
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// the <see cref="Criteria"/> associated with the table. 
        /// </summary>
        public TableCriteria Criteria { get; set; }
    }
}
