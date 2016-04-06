using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// The consumer model represents the individual customer in Doshii.
    /// </summary>
    public class Consumer
    {
        /// <summary>
		/// Constructor.
		/// </summary>
        public Consumer()
		{
			Clear();
		}
        
        /// <summary>
        /// Resets all property values to default settings.
        /// </summary>
        public void Clear()
        {
            Name = String.Empty;
            PhoneNumber = String.Empty;
            AddressLine1 = String.Empty;
            AddressLine2 = String.Empty;
            City = String.Empty;
            State = String.Empty;
            PostalCode = String.Empty;
            Country = String.Empty;
            Notes = String.Empty;
            PhotoUrl = string.Empty;
            Anonymous = false;
        }

        /// <summary>
        /// the url for the consumers photo
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// is this an anonymous user. 
        /// </summary>
        public bool Anonymous { get; set; }
        
        /// <summary>
        /// The consumers name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the consumers phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// the consumers address line 1
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// the consumers address line 1
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// the consumers address city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// the consumers address state
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// the consumers address postal code
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// the consumers address country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Notes specific to this order, 
        /// this may include:
        /// Notes about delivery location,
        /// Notes about allergies,
        /// Notes about a booking that has been made,
        /// Notes about special requests for the delivery. 
        /// </summary>
        public string Notes { get; set; }
    }
}
