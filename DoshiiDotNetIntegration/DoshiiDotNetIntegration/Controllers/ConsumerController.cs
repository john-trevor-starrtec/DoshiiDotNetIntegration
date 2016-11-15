using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Controllers
{
    internal class ConsumerController
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
        internal ConsumerController(Models.Controllers controller, HttpController httpComs)
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

        internal virtual Consumer GetConsumerFromCheckinId(string checkinId)
        {
            try
            {
                return _httpComs.GetConsumerFromCheckinId(checkinId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /*/// <summary>
        /// Gets the consumer related to the order,
        /// If there is a problem getting the consumer from Doshii the order is rejected by the SDK
        /// </summary>
        /// <param name="order">
        /// The order the consumer is needed for
        /// </param>
        /// <param name="transactionList">
        /// The transaction list for the pending order
        /// </param>
        /// <returns>
        /// The consumer related to the order. 
        /// </returns>
        internal virtual Consumer GetConsumerForOrderCreated(Order order, List<Transaction> transactionList)
        {
            try
            {
                return GetConsumerFromCheckinId(order.CheckinId);
            }
            catch (Exception ex)
            {
                _controllers.LoggingController.LogMessage(this.GetType(), DoshiiLogLevels.Error, string.Format("Doshii: There was an exception when retreiving the consumer for a pending order doshiiOrderId - {0}. The order will be rejected", order.Id), ex);
                _controllers.OrderingController.RejectOrderFromOrderCreateMessage(order, transactionList);
                return null;
            }
        }*/
    }
}
