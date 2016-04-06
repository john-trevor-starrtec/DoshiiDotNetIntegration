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
        string DoshiiId { get; set; }

        /// <summary>
        /// the name of the venue
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// address of the venue
        /// </summary>
        string AddressLine1 { get; set; }

        /// <summary>
        /// address of the venue
        /// </summary>
        string AddressLine2 { get; set; }

        /// <summary>
        /// the city element of the venue address
        /// </summary>
        string City { get; set; }

        /// <summary>
        /// the state element of the venue address
        /// </summary>
        string State { get; set; }

        /// <summary>
        /// the postal code of the venue
        /// </summary>
        string PostalCode { get; set; }

        /// <summary>
        /// the country element of the venue address
        /// </summary>
        string Country { get; set; }

        /// <summary>
        /// the phone number of the venue
        /// </summary>
        string PhoneNumber { get; set; }

        /// <summary>
        /// the last time the venue was disconnected - will be null if the venue is connected. 
        /// </summary>
        DateTime? DisconnectedDate { get; set; }
    }
}
