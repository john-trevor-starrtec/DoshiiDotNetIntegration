using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "DoshiiLog4Net.config", Watch = true)]

namespace DoshiiDotNetIntegration
{
    public abstract class LoggingBase
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
