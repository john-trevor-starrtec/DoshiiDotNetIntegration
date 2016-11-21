using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDotNetPOS.POSImpl
{
	/// <summary>
	/// This is a sample implementation of the <see cref="DoshiiDotNetIntegration.Interfaces.IDoshiiLogger"/>
	/// interface.
	/// </summary>
	/// <remarks>
	/// As the POS provider, your job will be to implement the <see cref="DoshiiDotNetIntegration.Interfaces.IDoshiiLogger"/>
	/// interface in such a way as is consistent with trace logging in your point of sale application.
	/// This sample simply passes the messages back to the main form of the sample to be displayed on screen.
	/// </remarks>
	public class SampleDoshiiLogger : IDoshiiLogger, IDisposable
	{
		/// <summary>
		/// The presenter that is used during callback.
		/// </summary>
		private SampleDotNetPOSPresenter mPresenter;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="presenter">The presenter that is used during callback.</param>
		public SampleDoshiiLogger(SampleDotNetPOSPresenter presenter)
		{
			if (presenter == null)
				throw new ArgumentNullException("presenter");

			mPresenter = presenter;
		}

		#region IDoshiiLogger Members

		/// <summary>
		/// See <see cref="DoshiiDotNetIntegration.Interfaces.IDoshiiLogger.LogDoshiiMessage(Type, DoshiiLogLevels, string, Exception)"/>
		/// for details of this call.
		/// </summary>
		/// <param name="callingClass"></param>
		/// <param name="logLevel"></param>
		/// <param name="message"></param>
		/// <param name="ex"></param>
		public void LogDoshiiMessage(Type callingClass, DoshiiLogLevels logLevel, string message, Exception ex = null)
		{
			if (mPresenter != null)
			{
				string messageToSend = BuildMessage(callingClass, message, ex);
				mPresenter.SendMessage(messageToSend, logLevel);
			}
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Cleans up the instance by removing two-way references, thus avoiding memory leaks.
		/// </summary>
		public void Dispose()
		{
			mPresenter = null;
		}

		#endregion

		/// <summary>
		/// Builds the string message to send to the logger.
		/// </summary>
		/// <param name="type">The calling type.</param>
		/// <param name="baseMessage">The base message.</param>
		/// <param name="e">An optional exception that has been thrown.</param>
		/// <returns>The message string to send to the logger.</returns>
		private string BuildMessage(Type type, string baseMessage, Exception e)
		{
			var sb = new StringBuilder();

			sb.AppendFormat("{0:dd/MM/yyyy HH:mm:ss.fff} ", DateTime.Now);
			sb.Append(TypeName(type));
			while (sb.Length < 40)
				sb.Append(" ");
			sb.AppendFormat(" : {0}", baseMessage);
			if (e != null)
			{
				sb.AppendLine();
				sb.AppendFormat("\t{0}:", e.Message);
				sb.AppendLine();
				sb.AppendLine("\t[Stack Trace]:");
				sb.AppendFormat("\t{0}", e.StackTrace);
			}
			sb.AppendLine();

			return sb.ToString();
		}

		/// <summary>
		/// This function returns a human-readable string name for a <paramref name="type"/> without its 
		/// namespace qualifiers but inclusive of generics.
		/// </summary>
		/// <remarks>
		/// <c>TypeName(SampleDoshiiLogger)</c> returns the string <c>SampleDoshiiLogger</c>.
		/// <c>TypeName(Dictionary&lt;int, string&gt;)</c> returns the string <c>Dictionary&lt;int, string&gt;</c>.
		/// </remarks>
		/// <param name="type">The type to be converted to a string.</param>
		/// <returns>A human-readable string for the name of the supplied <paramref name="type"/>.</returns>
		private static string TypeName(Type type)
		{
			var sb = new StringBuilder();
			var name = type.Name;
			if (!type.IsGenericType) 
				return name;

			sb.Append(name.Substring(0, name.IndexOf('`')));
			sb.Append("<");
			sb.Append(string.Join(", ", type.GetGenericArguments().Select(t => TypeName(t))));
			sb.Append(">");
			return sb.ToString();
		}
	}
}
