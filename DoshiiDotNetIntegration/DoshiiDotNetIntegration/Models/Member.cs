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

        public override bool Equals(System.Object obj)
        {
            // Check if the object is a RecommendationDTO.
            // The initial null check is unnecessary as the cast will result in null
            // if obj is null to start with.
            var memberToTest = obj as Member;

            if (memberToTest == null)
            {
                // If it is null then it is not equal to this instance.
                return false;
            }

             
            if (this.Id != memberToTest.Id)
            {
                return false;
            }
             
            if (this.Name != memberToTest.Name)
            {
                return false;
            }
             
            if (this.Email != memberToTest.Email)
            {
                return false;
            }
             
            if (this.Phone != memberToTest.Phone)
            {
                return false;
            }
             
            if (!this.Address.Equals(Address))
            {
                return false;
            }
             
            if (this.Uri != memberToTest.Uri)
            {
                return false;
            }

            if (this.Ref != memberToTest.Ref)
            {
                return false;
            }

            return true;
        }
    }
}
