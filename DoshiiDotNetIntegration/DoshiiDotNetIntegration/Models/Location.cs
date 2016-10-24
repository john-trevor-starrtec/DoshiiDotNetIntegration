using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// This class represents a location / venue in doshii
    /// </summary>
    public class Location
    {
        /// <summary>
        /// the DoshiiId for the venue - give this value to partners to allow them to send orders and payments to your venue. 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// the name of the venue
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// address of the venue
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// address of the venue
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// the city element of the venue address
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// the state element of the venue address
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// the postal code of the venue
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// the country element of the venue address
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// the phone number of the venue
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// the last time the venue was disconnected - will be null if the venue is connected. 
        /// </summary>
        public DateTime? DisconnectedDate { get; set; }

        /// <summary>
        /// Resets all property values to default settings.
        /// </summary>
        public void Clear()
        {
            Id = String.Empty;
            Name = String.Empty;
            AddressLine1 = String.Empty;
            AddressLine2 = String.Empty;
            City = String.Empty;
            State = String.Empty;
            PostalCode = String.Empty;
            Country = String.Empty;
            PhoneNumber = String.Empty;
            DisconnectedDate = null;
        }
    }
}
