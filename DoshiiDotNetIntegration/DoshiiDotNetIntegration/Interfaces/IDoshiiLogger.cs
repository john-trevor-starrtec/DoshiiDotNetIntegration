using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoshiiDotNetIntegration.Enums;

namespace DoshiiDotNetIntegration.Interfaces
{
	/// <summary>
	/// Implementations of the <c>IDoshiiLogger</c> interface are responsible for logging messages to the POS logging mechanism.
	/// </summary>
	/// <remarks>
	/// This interface must be implemented by the POS for trace logging to be enabled in the SDK.
	/// </remarks>
	public interface IDoshiiLogger
	{
		/// <summary>
		/// This method should log Doshii log messages in the POS logger.
		/// <para/>This is the method that records all doshii logs. 
        /// <para/>There is no separate file created for Doshii logs so they should be logged by the POS implementing the integration. 
        /// <para/>Please check <see cref="DoshiiDotNetIntegration.Enums.DoshiiLogLevels"/> for the different log levels implemented by doshii. 
		/// </summary>
		/// <param name="callingClass">The calling class for the logging mechanism.</param>
		/// <param name="logLevel">The level of the log message to be applied.</param>
		/// <param name="message">The message to be logged.</param>
		/// <param name="ex">An optional exception to be included in the log message.</param>
		/// <example>
		/// This sample shows how a POS might use log4net to implement the logging mechanism for calls made from the Doshii SDK
		/// via the <see cref="DoshiiDotNetIntegration.Interfaces.IDoshiiLogger.LogDoshiiMessage(Type, DoshiiLogLevels, string, Exception)"/> call.
		/// <code lang="C#">
		/// public void LogDoshiiMessage(Type callingClass, DoshiiLogLevels logLevel, string message, Exception ex = null)
		/// {
		///		var log = LogManager.GetLogger(callingClass);
		///		if (logLevel == DoshiiLogLevels.Debug)
		///			ex == null ? log.Debug(message) : log.Debug(message, ex);
		///		else if (logLevel == DoshiiLogLevels.Info)
		///			ex == null ? log.Info(message) : log.Info(message, ex);
		///		else if (logLevel == DoshiiLogLevels.Warning)
		///			ex == null ? log.Warning(message) : log.Warning(message, ex);
		///		else if (logLevel == DoshiiLogLevels.Error)
		///			ex == null ? log.Error(message) : log.Error(message, ex);
		///		else if (logLevel == DoshiiLogLevels.Fatal)
		///			ex == null ? log.Fatal(message) : log.Fatal(message, ex);
		///	}
		/// </code>
		/// </example>
		void LogDoshiiMessage(Type callingClass, Enums.DoshiiLogLevels logLevel, string message, Exception ex = null);
	}
}
