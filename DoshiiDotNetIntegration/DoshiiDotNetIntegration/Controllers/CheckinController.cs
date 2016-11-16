using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Controllers
{
    /// <summary>
    /// This class is used internally by the SDK to handle bl arround checkins
    /// NOTE: there are a number of operations around checkins that are handled in the <see cref="TableController"/> and the <see cref="ReservationController"/>
    /// this controller is used for all functions that are not directly related to reservations or tables.
    /// </summary>
    internal class CheckinController
    {
        /// <summary>
        /// prop for the local <see cref="DoshiiDotNetIntegration.Controllers"/> instance. 
        /// </summary>
        internal Models.Controllers _controllers;

        /// <summary>
        /// prop for the local <see cref="HttpController"/> instance.
        /// </summary>
        internal HttpController _httpComs;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="httpComs"></param>
        internal CheckinController(Models.Controllers controller, HttpController httpComs)
        {
            if (controller == null)
            {
                throw new NullReferenceException("controller cannot be null");
            }
            _controllers = controller;
            if (_controllers.LoggingController == null)
            {
                throw new NullReferenceException("doshiiLogger cannot be null");
            }
            if (httpComs == null)
            {
                _controllers.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;

        }

        /// <summary>
        /// This method is used to close a checkin. 
        /// </summary>
        /// <param name="checkinId">the Id of the checkin that need to be closed.</param>
        /// <returns>
        /// True if the close was successful
        /// False if the close was not successful. 
        /// </returns>
        internal virtual bool CloseCheckin(string checkinId)
        {
            _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: pos closing checkin '{0}'", checkinId));

            Checkin checkinCreateResult = null;
            try
            {
                checkinCreateResult = _httpComs.DeleteCheckin(checkinId);
                if (checkinCreateResult == null)
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: There was an error attempting to close a checkin."));
                    return false;
                }
            }
            catch (Exception ex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown while attempting to close checkin {0} - {1}", checkinId, ex));
                throw new OrderUpdateException(string.Format("Doshii: a exception was thrown while attempting to close a checkin {0}", checkinId), ex);
            }
            return true;
        }
    }
}
