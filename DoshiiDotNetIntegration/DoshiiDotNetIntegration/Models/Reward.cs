using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    public class Reward
    {
        public string Id { get; set; }

        /// <summary>
        /// the reward system Id
        /// </summary>
        public string Ref { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        private List<string> _EligibleItems;

        public List<string> EligibleItems
        {
            get
            {
                if (_EligibleItems == null)
                {
                    _EligibleItems = new List<string>();
                }
                return _EligibleItems;
            }
            set { _EligibleItems = value.ToList<string>(); }
        }

        private List<string> _EligibleTags;

        public List<string> EligibleTags
        {
            get
            {
                if (_EligibleTags == null)
                {
                    _EligibleTags = new List<string>();
                }
                return _EligibleTags;
            }
            set { _EligibleTags = value.ToList<string>(); }
        }

        public decimal MinQty { get; set; }
        public decimal MinOrderValue { get; set; }

        /// <summary>
        /// this value must be either 'item' or 'order'
        /// </summary>
        public string AppliedTo { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public DateTime? CreatedAt { get; set; }

        public Uri uri { get; set; }
        
    }
}
