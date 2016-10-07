using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    public class TableCriteria
    {
        public bool IsCommunal { get; set; }
        public bool CanMerge { get; set; }
        public bool IsSmoking { get; set; }
        public bool IsOutdoor { get; set; }
    }
}
