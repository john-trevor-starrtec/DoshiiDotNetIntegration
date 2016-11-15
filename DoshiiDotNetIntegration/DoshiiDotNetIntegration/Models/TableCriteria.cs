using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// the criteria for a table in a venue. 
    /// </summary>
    public class TableCriteria
    {
        /// <summary>
        /// flag indicating if the table is communal. 
        /// </summary>
        public bool IsCommunal { get; set; }

        /// <summary>
        /// flag indicating if it is possible to merge the table with other tables. 
        /// </summary>
        public bool CanMerge { get; set; }

        /// <summary>
        /// flag indicating if the table allows smoking. 
        /// </summary>
        public bool IsSmoking { get; set; }

        /// <summary>
        /// flag indicating if the table is outdoors. 
        /// </summary>
        public bool IsOutdoor { get; set; }
    }
}
