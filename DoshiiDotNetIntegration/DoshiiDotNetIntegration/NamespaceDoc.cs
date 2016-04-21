using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration
{
    /// <summary>
    /// <see cref="DoshiiDotNetIntegration"/> is an SDK developed to assist Point Of Sales software to integration with the Doshii API.
    /// <para/>The SDK targets .NET 4.5
    /// <para/>The SDK manages socket and HTTPs connections to the Doshii API
    /// <para/><see cref="DoshiiManager"/> provides methods for the pos to update orders and transactions on the Doshii API.
    /// <para/><see cref="DoshiiDotNetIntegration.Interfaces"/> provides call back mechanisms that alert the pos to new and modified orders, transactions, and logging (allowing the pos to implement SDK logging in their preferred method.
    /// <para/> and callback mechanism to update and retrieve order versions, transaction versions, and checkinIds related to orders. 
    /// <para/><see cref="DoshiiDotNetIntegration.Exceptions"/> provides a number of exceptions that are raised by the SDK and exceptions that must be raised by the pos under certain circumstances to correct operation of the SDK.
    /// <para/><see cref="DoshiiDotNetIntegration.Models"/> provides the models used by the SDK while integration with the Doshii API.
    /// <para/><see cref="DoshiiDotNetIntegration.Enums"/> provides the enums used by the SDK.
    /// <h1 class="Heading">Known issue with Visual Studio</h1>
    /// <para>If you have used the Nuget package manager to manage this SDK visual studio fails to automatically copy the 'websocketsharp.dll' into your build directories during build</para>
    /// <h2 class="heading">Solution</h2>
    /// <para>The solution to this issue is to directly reference the 'websocket-sharp.dll' in the project that is consuming the SDK.</para>
    /// <para>The 'websocket-sharp.dll' can be found in the packages folder packages\DoshiiPosDotNetSDK.~currentVersion~\lib</para>
    /// <para>The above file must be referenced and set to copy local in your project.</para>
    /// </summary>
    internal class NamespaceDoc
    {

    }
}
