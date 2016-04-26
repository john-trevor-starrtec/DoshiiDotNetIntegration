using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Exceptions
{
    /// <summary>
    /// this exception will be thrown when the string received from the Doshii API representing a currency amount cannot be converted into 
    /// a valid currency amount. 
    /// </summary>
    internal class NotValidCurrencyAmountException : Exception
    {
        internal NotValidCurrencyAmountException() : base() { }
        internal NotValidCurrencyAmountException(string message) : base(message) { }
        internal NotValidCurrencyAmountException(string message, Exception ex) : base(message, ex) { }
    }
}
