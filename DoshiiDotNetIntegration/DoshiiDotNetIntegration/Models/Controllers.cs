using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Controllers;
using DoshiiDotNetIntegration.Interfaces;

namespace DoshiiDotNetIntegration.Models
{
    internal class Controllers
    {
        internal IOrderingManager OrderingManager;
        internal ITransactionManager TransactionManager;
        internal IRewardManager RewardManager;
        internal IReservationManager ReservationManager;
        internal IConfigurationManager ConfigurationManager;
        
        internal OrderingController OrderingController;
        internal TransactionController TransactionController;
        internal ReservationController ReservationController;
        internal RewardController RewardController;
        internal LoggingController LoggingController;
    }
}
