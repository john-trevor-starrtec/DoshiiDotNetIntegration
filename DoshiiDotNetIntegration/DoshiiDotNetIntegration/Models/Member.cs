using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    public class Member
    {
        public Member()
        {
            Address = new Address();
        }
        
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Uri Uri { get; set; }
        public string Ref { get; set; }

        private List<App> _Apps;

        public IEnumerable<App> Apps
        {
            get
            {
                if (_Apps == null)
                {
                    _Apps = new List<App>();
                }
                return _Apps;
            }
            set
            {
                _Apps = value.ToList<App>();
            }
        }
    }
}
