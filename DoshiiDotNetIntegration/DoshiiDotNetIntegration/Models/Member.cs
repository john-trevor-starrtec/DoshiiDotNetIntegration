using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// the class representing a member in Doshii
    /// </summary>
    public class Member
    {
        /// <summary>
        /// constructor. 
        /// </summary>
        public Member()
        {
            Address = new Address();
        }
        
        /// <summary>
        /// the Id of the member
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// the name of the member
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the email of the member
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// the phone number of the member
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// the <see cref="Address"/> associated with the member
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// the last time the member was updated on Doshii
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// the time the member was created on Doshii
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// the URI to retreive the member details from Doshii
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// the Pos identifier for the member. 
        /// </summary>
        public string Ref { get; set; }

        private List<App> _Apps;

        /// <summary>
        /// a list of <see cref="App"/> associated with the member
        /// </summary>
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
