using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration
{

	internal class DoshiiLogManager : IDisposable
	{

		private IDoshiiLogger mLog;

		internal DoshiiLogManager(IDoshiiLogger logger)
		{
			mLog = logger;
		}

		#region IDisposable Members

		public void Dispose()
		{
			mLog = null;
		}

		#endregion


		internal void LogMessage(Type type, DoshiiLogLevels level, string message, Exception ex = null)
		{
			if (mLog != null)
				mLog.LogDoshiiMessage(type, level, message, ex);
		}
	}
}
