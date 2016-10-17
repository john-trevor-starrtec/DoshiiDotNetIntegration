using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// The address represents an address of a member in Doshii
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Line 1 of the address
        /// </summary>
        public string Line1 { get; set; }

        /// <summary>
        /// Line 2 of the address
        /// </summary>
        public string Line2 { get; set; }

        /// <summary>
        /// the Address city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// the address state
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// the address post/zip code
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// the address country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public Address()
        {
            Clear();
        }
        
        /// <summary>
        /// Resets all property values to default settings.
        /// </summary>
        public void Clear()
        {
            Line1 = string.Empty;
            Line2 = string.Empty;
            City = string.Empty;
            State = string.Empty;
            PostalCode = string.Empty;
            Country = string.Empty;
        }

        public override bool Equals(System.Object obj)
        {
            // Check if the object is a RecommendationDTO.
            // The initial null check is unnecessary as the cast will result in null
            // if obj is null to start with.
            var addressToTest = obj as Address;

            if (addressToTest == null)
            {
                // If it is null then it is not equal to this instance.
                return false;
            }

            if (this.Line1 != addressToTest.Line1)
            {
                return false;
            }

            if (this.Line2 != addressToTest.Line2)
            {
                return false;
            }
            
            if (this.City != addressToTest.City)
            {
                return false;
            }
            
            if (this.State != addressToTest.State)
            {
                return false;
            }
            
            if (this.PostalCode != addressToTest.PostalCode)
            {
                return false;
            }

            if (this.Country != addressToTest.Country)
            {
                return false;
            }
            return true;

        }
    }
}
