using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Helpers
{
    internal static class ConsumerExtension
    {
        internal static int CompareTo(this Consumer consumer, Consumer other)
        {
            return
                consumer.Anonymous == other.Anonymous
                && consumer.AddressLine1 == other.AddressLine1
                && consumer.AddressLine2 == other.AddressLine2
                && consumer.City == other.City
                && consumer.Country == other.Country
                && consumer.Name == other.Name
                && consumer.PhoneNumber == other.PhoneNumber
                && consumer.PostalCode == other.PostalCode
                && consumer.State == other.State
                ? 0 : 1;
        }
    }
}
