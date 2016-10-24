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
                && consumer.Address.Line1 == other.Address.Line1
                && consumer.Address.Line2 == other.Address.Line2
                && consumer.Address.City == other.Address.City
                && consumer.Address.Country == other.Address.Country
                && consumer.Name == other.Name
                && consumer.Phone == other.Phone
                && consumer.Address.PostalCode == other.Address.PostalCode
                && consumer.Address.State == other.Address.State
                ? 0 : 1;
        }
    }
}
