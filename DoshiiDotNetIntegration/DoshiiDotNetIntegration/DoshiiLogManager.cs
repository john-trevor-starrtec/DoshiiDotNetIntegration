using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Interfaces;
using System;

namespace DoshiiDotNetIntegration
{
	/// <summary>
	/// This class is used internally by the SDK to manage the logging of messages back to the POS implementation.
	/// This should not need to be instantiated by the POS implementation.
	/// </summary>
	internal class DoshiiLogManager : IDisposable
	{
		/// <summary>
		/// A reference to the callback mechanism for message logging in the application.
		/// </summary>
		internal IDoshiiLogger mLog;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="logger">The callback for message logging in the application. Can optionally be <c>null</c> in
		/// which case messages logged by the SDK will not be returned to the application.</param>
		internal DoshiiLogManager(IDoshiiLogger logger)
		{
			mLog = logger;
		}

		#region IDisposable Members

		/// <summary>
		/// Cleanly disposes of the instance.
		/// </summary>
		public void Dispose()
		{
			mLog = null;
		}

		#endregion

		/// <summary>
		/// Logs a message to the application logging mechanism if available.
		/// </summary>
		/// <param name="type">The calling class type.</param>
		/// <param name="level">The logging level.</param>
		/// <param name="message">The raw string message to be logged.</param>
		/// <param name="ex">An optional exception associated with the log message.</param>
		internal void LogMessage(Type type, DoshiiLogLevels level, string message, Exception ex = null)
		{
			if (mLog != null)
				mLog.LogDoshiiMessage(type, level, message, ex);
		}
	}
}
