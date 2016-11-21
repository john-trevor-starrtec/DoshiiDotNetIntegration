using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoshiiDotNetIntegration.Enums;

namespace DoshiiDotNetIntegration.Interfaces
{
    /// <summary>
    /// Implementations of the <c>IDoshiiLogger</c> interface allow the pos to provide the socketUrl to the doshii SDK, if this interface is not implemented the doshii SDK will generate the socket
    /// url from the baseUrl, this should not cause any problems in the live enviornemtn but may cause problems in the sandbox env as the sandbox socket connection is subject to change at short notice.
    /// It is recommended that this interface is implemented by all pos integrators. 
    /// </summary>
    /// <remarks>
    /// This interface must be implemented by the POS for trace logging to be enabled in the SDK.
    /// </remarks>
    public interface IDoshiiConfiguration
    {
        /// <summary>
        /// This method should return the socket url used for socket connections to Doshii, If this method returns an empty string the sdk will generate the socket message from the BaseUrl provided by the pos, please see interface comments. 
        /// </summary>
        /// <returns></returns>
        string GetSocketUrlFromPos();

        string GetBaseUrlFromPos();

        string GetLocationTokenFromPos();

        string GetSecretKeyFromPos();

        int GetSocketTimeOutFromPos();

        string GetVendorFromPos();
    }
}
