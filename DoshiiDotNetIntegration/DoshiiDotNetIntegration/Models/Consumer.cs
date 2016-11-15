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
            Phone = String.Empty;
            Address = null;
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
        public string Phone { get; set; }

        /// <summary>
        /// the email of the consumer
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// the <see cref="Address"/> associated with the consumer
        /// </summary>
        public Address Address { get; set; }
        
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
