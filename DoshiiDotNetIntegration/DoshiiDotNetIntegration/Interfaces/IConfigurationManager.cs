using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoshiiDotNetIntegration.Enums;

namespace DoshiiDotNetIntegration.Interfaces
{
    /// <summary>
    /// Implementations of the <see cref="IConfigurationManager"/> is mandatory, it provides the only method for injecting basic configurations options and interface implementations into the SDK. 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public interface IConfigurationManager
    {
        /// <summary>
        /// This method must return the socket url used for socket connections to Doshii. 
        /// </summary>
        /// <returns></returns>
        string GetSocketUrlFromPos();

        /// <summary>
        /// The method must return the baseUrl use for http communications with Doshii
        /// </summary>
        /// <returns></returns>
        string GetBaseUrlFromPos();

        /// <summary>
        /// the method must return the location token for the specific venue using Doshii
        /// </summary>
        /// <returns></returns>
        string GetLocationTokenFromPos();

        /// <summary>
        /// this method must return the pos vendor secret key used for authentication with Doshii
        /// </summary>
        /// <returns></returns>
        string GetSecretKeyFromPos();

        /// <summary>
        /// this method must return the time out interval for a socket connection that has become disconnected. 
        /// If the socket connection goes down the sdk will wait the provided amount of seconds before it considers the socket connection has gone down. 
        /// If 0 is provided the default 600 will be used. 
        /// </summary>
        /// <returns></returns>
        int GetSocketTimeOutFromPos();

        /// <summary>
        /// this method must reutrn the vendor name string used for authentication into Doshii, the exact string can be provided by Doshii. 
        /// </summary>
        /// <returns></returns>
        string GetVendorFromPos();

        /// <summary>
        /// this method must return an IOrderingManager implementation, this is a mandatory interface and SDK will not operate if this is not implemented. 
        /// </summary>
        /// <returns></returns>
        IOrderingManager GetOrderingManagerFromPos();

        /// <summary>
        /// the method must return an ITransactionManager implementation, this is a mandatory interface and the SDK will not operate if this is not implemented. 
        /// </summary>
        /// <returns></returns>
        ITransactionManager GetTransactionManagerFromPos();

        /// <summary>
        /// this method can reutrn a IRewardManager implementation if the pos wishes to implement the rewards and membership functionality offered by Doshii, if the pos does not wish to implement the 
        /// rewards and membership functionality the pos can reutrn null for this methods. 
        /// </summary>
        /// <returns></returns>
        IRewardManager GetRewardManagerFromPos();

        /// <summary>
        /// this method can return an IReservationManager implementation if the pos wishes to implement the reservations functionality offered by Doshii,
        /// if the pos does not wish to use the reservation functionality you my return Null for this method. 
        /// </summary>
        /// <returns></returns>
        IReservationManager GetReservationManagerFromPos();

        /// <summary>
        /// this method must return an ILoggingManager implementation, this is manatory interface and the SDK will not operate if this is not implemented. 
        /// </summary>
        /// <returns></returns>
        ILoggingManager GetLoggingManagerFromPos();
    }
}
