using AutoMapper.Internal;
using AutoMapper.Mappers;
using AutoMapper;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using JWT;

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    /// <summary>
    /// This class is used internally by the SDK.
    /// This class manages the HTTP communications between the pos and the Doshii API.
    /// </summary>
    internal class DoshiiHttpCommunication 
    {
		/// <summary>
		/// The HTTP request method for a DELETE endpoint action.
		/// </summary>
		private const string DeleteMethod = "DELETE";

        /// <summary>
        /// The base URL for HTTP communication with Doshii,
        /// an example of the format for this URL is 'https://sandbox.doshii.co/pos/api/v2'
        /// </summary>
		internal string m_DoshiiUrlBase { get; private set; }

        /// <summary>
        /// Doshii Manager logic
        /// </summary>
		internal DoshiiManager m_DoshiiLogic { get; private set; }

        /// <summary>
		/// The logging callback mechanism for the POS.
		/// </summary>
		internal DoshiiLogManager mLog { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="urlBase">
        /// The base URL for HTTP communication with the Doshii API, <see cref="m_DoshiiUrlBase"/>
        /// </param>
		/// <param name="token">
		/// The Doshii pos token that will identify the pos on the Doshii API, <see cref="m_Token"/>
		/// </param>
		/// <param name="logManager">
		/// The <see cref="DoshiiLogManager"/> that is responsible for logging doshii messages, <see cref="mLog"/>
		/// </param>
        /// <param name="doshiiLogic">
        /// the <see cref="DoshiiManager"/> that controls the operation of the SDK.
        /// </param>
		internal DoshiiHttpCommunication(string urlBase, DoshiiLogManager logManager, DoshiiManager doshiiLogic)
        {
            if (doshiiLogic == null)
            {
                throw new ArgumentNullException("doshiiLogic");
            }

			if (logManager == null)
			{
				throw new ArgumentNullException("logManager");
			}

            m_DoshiiLogic = doshiiLogic;
			mLog = logManager;

            mLog.LogMessage(typeof(DoshiiHttpCommunication), Enums.DoshiiLogLevels.Debug, string.Format("Instantiating DoshiiHttpCommunication Class with; urlBase - '{0}', locationId - '{1}', vendor - '{2}', secretKey - '{3}'", urlBase, doshiiLogic.LocationToken, doshiiLogic.Vendor, doshiiLogic.SecretKey));
            if (string.IsNullOrWhiteSpace(urlBase))
            {
				mLog.LogMessage(typeof(DoshiiHttpCommunication), Enums.DoshiiLogLevels.Error, string.Format("Instantiating DoshiiHttpCommunication Class with a blank urlBase - '{0}'", urlBase));
                throw new ArgumentException("blank URL");
            
            }
            
            
            m_DoshiiUrlBase = urlBase;
        }

        #region order methods

        /// <summary>
        /// This method is used to retrieve the order from Doshii matching the provided orderId (the pos identifier for the order),
        /// </summary>
        /// <param name="orderId">
        /// The pos identifier for the order
        /// </param>
        /// <returns>
        /// If an order if found matching the orderId the order is returned,
        /// If on order matching the orderId is not found a new order is returned. 
        /// </returns>
        internal virtual Order GetOrder(string orderId)
        {
            var retreivedOrder = new Order();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Order, orderId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
						var jsonOrder = JsonOrder.deseralizeFromJson(responseMessage.Data);
						retreivedOrder = Mapper.Map<Order>(jsonOrder);
                    }
                    else
                    {
						mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                    }

                }
                else
                {
					mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                }
            }
            else
            {
				mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
            }

            return retreivedOrder;
        }

        /// <summary>
        /// This method is used to retrieve the order from Doshii matching the provided doshiiOrderId (the doshii identifier for the order),
        /// This method should only be used with trying to retreive orders from Doshii that are not currently linked to a pos order, 
        /// If the orders are currently linked on the Pos <see cref="GetOrder"/> should be used. 
        /// </summary>
        /// <param name="orderId">
        /// The pos identifier for the order
        /// </param>
        /// <returns>
        /// If an order if found matching the orderId the order is returned,
        /// If on order matching the orderId is not found a new order is returned. 
        /// </returns>
        internal virtual Order GetOrderFromDoshiiOrderId(string doshiiOrderId)
        {
            var retreivedOrder = new Order();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.UnlinkedOrders, doshiiOrderId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonOrder = JsonOrder.deseralizeFromJson(responseMessage.Data);
                        try
                        {
                            retreivedOrder = Mapper.Map<Order>(jsonOrder);
                        }
                        catch (Exception ex)
                        {
                            mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Error,
                                string.Format(
                                    "Doshii: An order received from Doshii could not be processed, A Price value in the order could not be converted into a decimal, the order will be rejected by the SDK: ",
                                    jsonOrder));
                            //reject the order. 
                            var orderWithNoPricePropertiesToReject = Mapper.Map<OrderWithNoPriceProperties>(jsonOrder);
                            var orderToReject = Mapper.Map<Order>(orderWithNoPricePropertiesToReject);
                            m_DoshiiLogic.RejectOrderAheadCreation(orderToReject);
                            retreivedOrder = null;
                        }
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning,
                            string.Format(
                                "Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response",
                                GenerateUrl(Enums.EndPointPurposes.UnlinkedOrders, doshiiOrderId)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.UnlinkedOrders, doshiiOrderId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.UnlinkedOrders, doshiiOrderId)));
            }

            return retreivedOrder;
        }

        /// <summary>
        /// Gets all the current active linked orders in Doshii.
        /// To get all order including unlinked orders you must also call <see cref="GetUnlinkedOrders"/>
        /// </summary>
        /// <returns>
        /// A list of all currently active linked orders from Doshii
        /// If there are no current active linked orders an empty list is returned.  
        /// </returns>
        internal virtual IEnumerable<Order> GetOrders()
        {
            var retreivedOrderList = new List<Order>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Order), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonOrder>>(responseMessage.Data);
                        retreivedOrderList = Mapper.Map<List<Order>>(jsonList);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Order)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Order)));
            }

            List<Order> fullOrderList = new List<Order>();
            foreach (var partOrder in retreivedOrderList)
            {
                Order newOrder = GetOrder(partOrder.Id);
                fullOrderList.Add(newOrder);
            }
            return (IEnumerable<Order>)fullOrderList;
        }

        /// <summary>
        /// Gets all the current active unlinked orders in Doshii.
        /// To get all order including linked orders you must also call <see cref="GetOrders"/>
        /// </summary>
        /// <returns>
        /// A list of all currently active unlinked orders from Doshii
        /// If there are no current active unlinked orders an empty list is returned.  
        /// </returns>
        internal virtual IEnumerable<Order> GetUnlinkedOrders()
        {
            var retreivedOrderList = new List<Order>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.UnlinkedOrders), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonOrder>>(responseMessage.Data);
                        retreivedOrderList = Mapper.Map<List<Order>>(jsonList);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Order)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Order)));
            }

            List<Order> fullOrderList = new List<Order>();
            foreach (var partOrder in retreivedOrderList)
            {
                Order newOrder = GetOrderFromDoshiiOrderId(partOrder.DoshiiId);
                if (newOrder != null)
                {
                    fullOrderList.Add(newOrder);
                }
            }
            return (IEnumerable<Order>)fullOrderList;
        }

        /// <summary>
        /// completes the Put or Post request to update an order with Doshii. 
        /// </summary>
        /// <param name="order">
        /// The order to by updated on Doshii
        /// </param>
        /// <param name="method">
        /// The HTTP verb to be used (in the current version the only acceptable verb is PUT)
        /// </param>
        /// <returns>
        /// The order returned from the request. 
        /// </returns>
        /// <exception cref="System.NotSupportedException">Currently thrown when the method is not <see cref="System.Net.WebRequestMethods.Http.Put"/>.</exception>
        private Order PutPostOrder(Order order, string method)
        {
            if (!method.Equals(WebRequestMethods.Http.Put))
            {
                throw new NotSupportedException("Method Not Supported");
            }

            var returnOrder = new Order();
            DoshiHttpResponseMessage responseMessage;

            try
            {
                var jsonOrderToPut = Mapper.Map<JsonOrderToPut>(order);
                if (String.IsNullOrEmpty(order.Id))
                {
                    responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.UnlinkedOrders, order.DoshiiId), method, jsonOrderToPut.ToJsonString());
                }
                else
                {
                    responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Order, order.Id), method, jsonOrderToPut.ToJsonString());
                }
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            var dto = new JsonOrder();
            returnOrder = HandleOrderResponse<Order, JsonOrder>(order.Id, responseMessage, out dto);

            return returnOrder;
        }

        /// <summary>
        /// This method is specifically called to confirm an order created on an orderAhead partner.
        /// </summary>
        /// <param name="order">
        /// The order to be confirmed
        /// </param>
        /// <returns>
        /// The order that was returned from the PUT request to Doshii. 
        /// </returns>
        internal Order PutOrderCreatedResult(Order order)
        {
            var returnOrder = new Order();
            DoshiHttpResponseMessage responseMessage;

            try
            {
                var jsonOrderToPut = Mapper.Map<JsonUnlinkedOrderToPut>(order);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.UnlinkedOrders, order.DoshiiId), WebRequestMethods.Http.Put, jsonOrderToPut.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            var dto = new JsonOrder();
            if (order.Status != "rejected")
            {
                returnOrder = HandleOrderResponse<Order, JsonOrder>(order.Id, responseMessage, out dto);
            }
            
            return returnOrder;
        }

        /// <summary>
        /// This function takes the supplied <paramref name="responseMessage"/> received from the RESTful Doshii API and translates it
        /// into some sort of order object. It utilises the mapping between a model object (<typeparamref name="T"/>) and its corresponding 
        /// JSON data transfer object (<typeparamref name="DTO"/>). The data transfer object type should be an extension of the 
        /// <see cref="DoshiiDotNetIntegration.Models.Json.JsonSerializationBase<TSelf>"/> class.
        /// </summary>
        /// <remarks>
        /// The purpose of this function is to provide a consistent manner of parsing the response to the <c>PUT /orders/:pos_id</c> call in the 
        /// API, regardless of the actual model object we are dealing with for the action taken.
        /// </remarks>
        /// <typeparam name="T">The type of model object to be returned by this call. This should be a member of the <c>DoshiiDotNetIntegration.Models</c>
        /// namespace that is mapped to the <typeparamref name="DTO"/> type via the <see cref="DoshiiDotNetIntegration.Helpers.AutoMapperConfigurator"/>
        /// helper class.</typeparam>
        /// <typeparam name="DTO">The corresponding data type object used by the communication with the API for the action.</typeparam>
        /// <param name="orderId">The POS identifier for the order.</param>
        /// <param name="responseMessage">The current response message to be parsed.</param>
        /// <param name="jsonDto">When this function returns, this output parameter will be the data transfer object used in communication with the API.</param>
        /// <returns>The details of the order in the Doshii API.</returns>
        private T HandleOrderResponse<T, DTO>(string orderId, DoshiHttpResponseMessage responseMessage, out DTO jsonDto)
        {
            jsonDto = default(DTO); // null since its an object
            T returnObj = default(T); // null since its an object

            mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message has been returned to the put order function"));

            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response order data was not null"));
                        jsonDto = JsonConvert.DeserializeObject<DTO>(responseMessage.Data);
                        returnObj = Mapper.Map<T>(jsonDto);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                throw new NullOrderReturnedException();
            }

            UpdateOrderVersion<T>(returnObj);
            UpdateOrderCheckin<T>(returnObj);


            return returnObj;
        }

        /// <summary>
        /// A call to this function updates the order version in the POS. The generic nature of this function is due to the fact that
        /// we might be dealing with different actual model objects. This function can be used to update the POS version of the order
        /// regardless of the actual type used.
        /// </summary>
        /// <remarks>
        /// NOTE: The SDK implementer must update this call for any new model types that make use of the order version.
        /// </remarks>
        /// <typeparam name="T">The type of model object being updated. In this case, the type should be a derivative of an
        /// <see cref="DoshiiDotNetIntegration.Models.Order"/> or a class that contains a reference to an order.</typeparam>
        /// <param name="orderDetails">The details of the order.</param>
        private void UpdateOrderVersion<T>(T orderDetails)
        {
            if (orderDetails != null)
            {
                Order order = null;
                if (orderDetails is Order)
                    order = orderDetails as Order;
                else if (orderDetails is TableOrder)
                    order = (orderDetails as TableOrder).Order;

                if (order != null && !String.IsNullOrEmpty(order.Id))
                    m_DoshiiLogic.RecordOrderVersion(order.Id, order.Version);
            }
        }

        private void UpdateOrderCheckin<T>(T orderDetails)
        {
            if (orderDetails != null)
            {
                Order order = null;
                if (orderDetails is Order)
                    order = orderDetails as Order;
                else if (orderDetails is TableOrder)
                    order = (orderDetails as TableOrder).Order;

                if (order != null && !String.IsNullOrEmpty(order.CheckinId))
                    m_DoshiiLogic.RecordOrderCheckinId(order.Id, order.CheckinId);
            }
        }

        /// <summary>
        /// This method is used to confirm or reject or update an order when the order has an OrderId
        /// </summary>
        /// <param name="order">
        /// The order to be updated
        /// </param>
        /// <returns>
        /// If the request is not successful a new order will be returned - you can check the order.Id in the returned order to confirm it is a valid response. 
        /// </returns>
        internal virtual Order PutOrder(Order order)
        {
            return PutPostOrder(order, WebRequestMethods.Http.Put);
        }

        /// <summary>
        /// This method should be used to create or update an order on Doshii that has a table allocation. 
        /// </summary>
        /// <param name="tableOrder">The details of the order and table allocation to present to Doshii.</param>
        /// <returns>The order details returned from Doshii.</returns>
        ///<exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual Order PutOrderWithTableAllocation(TableOrder tableOrder)
        {
            DoshiHttpResponseMessage responseMessage;
            string orderIdentifier = tableOrder.Order.Id;
            try
            {
                var jsonTableOrder = Mapper.Map<JsonTableOrder>(tableOrder);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Order, orderIdentifier), WebRequestMethods.Http.Put, jsonTableOrder.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            var dto = new JsonOrder();
            return HandleOrderResponse<Order, JsonOrder>(orderIdentifier, responseMessage, out dto);
        }


        /// <summary>
        /// Deletes a table allocation from doshii for the provided checkinId. 
        /// </summary>
        /// <returns>
        /// true if successful
        /// false if failed
        /// </returns>
        ///<exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual bool DeleteTableAllocation(string checkinId)
        {
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.DeleteAllocationFromCheckin, checkinId), DeleteMethod);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: A 'DELETE' request to {0} was successful. Allocations have been removed", GenerateUrl(EndPointPurposes.DeleteAllocationFromCheckin, checkinId)));
                    return true;
                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} was not successful", GenerateUrl(EndPointPurposes.DeleteAllocationFromCheckin, checkinId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'DELETE' to URL '{0}'", GenerateUrl(EndPointPurposes.DeleteAllocationFromCheckin, checkinId)));
            }

            return false;
        }
        
        #endregion

        #region transaction methods

        /// <summary>
        /// This method is used to retrieve a list of transaction related to an order with the doshiiOrderId
        /// This method will only retreive transactions for unlinked orders on Doshii - if the order is linked to a pos order there is no method to retreive the transaction in the OrderAhead implementation. 
        /// </summary>
        /// <param name="doshiiOrderId">
        /// the doshiiOrderId for the order. 
        /// </param>
        /// <returns>
        /// A list of transactions associated with the order,
        /// If there are no transaction associated with the order an empty list is returned. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual IEnumerable<Transaction> GetTransactionsFromDoshiiOrderId(string doshiiOrderId)
        {
            var retreivedTransactionList = new List<Transaction>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.TransactionFromDoshiiOrderId, doshiiOrderId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonTransaction>>(responseMessage.Data);
                        retreivedTransactionList = Mapper.Map<List<Transaction>>(jsonList);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.TransactionFromDoshiiOrderId, doshiiOrderId)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.TransactionFromDoshiiOrderId, doshiiOrderId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.TransactionFromDoshiiOrderId, doshiiOrderId)));
            }

            return retreivedTransactionList;
        }

        /// <summary>
        /// This method is used to get a transaction from Doshii with the matching transacitonId
        /// </summary>
        /// <param name="transactionId">
        /// The Id of the transaction to be retrieved.
        /// </param>
        /// <returns>
        /// The transaction with the given Id if it exists,
        /// A Blank transaction if it did not exist. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual Transaction GetTransaction(string transactionId)
        {
            var retreivedTransaction = new Transaction();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Transaction, transactionId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTransaction = JsonTransaction.deseralizeFromJson(responseMessage.Data);
                        retreivedTransaction = Mapper.Map<Transaction>(jsonTransaction);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Transaction, transactionId)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Transaction, transactionId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Transaction, transactionId)));
            }

            return retreivedTransaction;
        }

        /// <summary>
        /// Gets all the current active transactions in Doshii. 
        /// </summary>
        /// <returns>
        /// an IEnumerable of transactions 
        /// it will be empty if no transactions exist. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual IEnumerable<Transaction> GetTransactions()
        {
            var retreivedTransactionList = new List<Transaction>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Transaction), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonTransaction>>(responseMessage.Data);
                        retreivedTransactionList = Mapper.Map<List<Transaction>>(jsonList);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Transaction)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Transaction)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Transaction)));
            }

            return retreivedTransactionList;
        }

        /// <summary>
        /// completes the Post request to create a transaction on Doshii. 
        /// </summary>
        /// <param name="transaction">the transaction to be created on Doshii</param>
        /// <returns>
        /// The transaction that was created on Doshii
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual Transaction PostTransaction(Transaction transaction)
        {
            DoshiHttpResponseMessage responseMessage;
            Transaction returnedTransaction = null;
            try
            {
                var jsonTransaction = Mapper.Map<JsonTransaction>(transaction);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Transaction, transaction.Id), WebRequestMethods.Http.Post, jsonTransaction.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTransaction = JsonConvert.DeserializeObject<JsonTransaction>(responseMessage.Data);
                        returnedTransaction = Mapper.Map<Transaction>(jsonTransaction);
                        if (returnedTransaction != null)
                        {
                            m_DoshiiLogic.RecordTransactionVersion(returnedTransaction);
                        }
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Transaction, transaction.Id)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Transaction, transaction.Id)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Transaction, transaction.Id)));
                throw new NullOrderReturnedException();
            }

            return returnedTransaction;
        }

        /// <summary>
        /// completes the Put request to update a transaction that already exists on Doshii. 
        /// </summary>
        /// <param name="transaction">
        /// The transaction to be updated. 
        /// </param>
        /// <returns>
        /// The updated transaction returned by Doshii.
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual Transaction PutTransaction(Transaction transaction)
        {
            DoshiHttpResponseMessage responseMessage;
            Transaction returnedTransaction = null;
            try
            {
                var jsonTransaction = Mapper.Map<JsonTransaction>(transaction);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Transaction, transaction.Id), WebRequestMethods.Http.Put, jsonTransaction.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTransaction = JsonConvert.DeserializeObject<JsonTransaction>(responseMessage.Data);
                        returnedTransaction = Mapper.Map<Transaction>(jsonTransaction);
                        if (returnedTransaction != null)
                        {
                            m_DoshiiLogic.RecordTransactionVersion(returnedTransaction);
                        }
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Transaction, transaction.Id)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Transaction, transaction.Id)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Transaction, transaction.Id)));
                throw new NullOrderReturnedException();
            }

            return returnedTransaction;
        }
#endregion

        #region Member methods

        internal virtual Member GetMember(string memberId)
        {
            var retreivedMember = new Member();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Members, memberId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonMember = JsonMember.deseralizeFromJson(responseMessage.Data);
                        retreivedMember = Mapper.Map<Member>(jsonMember);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Members, memberId)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Members, memberId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Members, memberId)));
            }

            return retreivedMember;
        }

        internal virtual IEnumerable<Member> GetMembers()
        {
            var retreivedMemberList = new List<Member>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Members), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonMember>>(responseMessage.Data);
                        retreivedMemberList = Mapper.Map<List<Member>>(jsonList);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Members)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Members)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Members)));
            }

            List<Member> fullMemberList = new List<Member>();
            foreach (var partmember in retreivedMemberList)
            {
                Member newMember = GetMember(partmember.Id);
                fullMemberList.Add(newMember);
            }
            return (IEnumerable<Member>)fullMemberList;
        }


        internal Member PutMember(Member member)
        {
            var returnedMember = new Member();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonMember = Mapper.Map<JsonMemberToUpdate>(member);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Members, member.Id), WebRequestMethods.Http.Put, jsonMember.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonMember = JsonConvert.DeserializeObject<JsonMenuProduct>(responseMessage.Data);
                        returnedMember = Mapper.Map<Member>(jsonMember);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Members, member.Id)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(EndPointPurposes.Members, member.Id)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(EndPointPurposes.Members, member.Id)));
                throw new NullOrderReturnedException();
            }
            return returnedMember;
        }

        internal Member PostMember(Member member)
        {
            var returnedMember = new Member();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonMember = Mapper.Map<JsonMemberToUpdate>(member);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Members), WebRequestMethods.Http.Post, jsonMember.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonMember = JsonConvert.DeserializeObject<JsonMenuProduct>(responseMessage.Data);
                        returnedMember = Mapper.Map<Member>(jsonMember);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Members)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(EndPointPurposes.Members)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(EndPointPurposes.Members)));
                throw new NullOrderReturnedException();
            }
            return returnedMember;
        }

        internal bool DeleteMember(Member member)
        {
            var returnedMember = new Member();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Members, member.Id), DoshiiHttpCommunication.DeleteMethod);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                    

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} was not successful", GenerateUrl(EndPointPurposes.Members, member.Id)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'DELETE' and URL '{0}'", GenerateUrl(EndPointPurposes.Members, member.Id)));
                throw new NullOrderReturnedException();
            }
            return false;
        }

        internal virtual IEnumerable<Reward> GetRewardsForMember(string memberId, string orderId, decimal orderTotal)
        {
            var retreivedRewardList = new List<Reward>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(string.Format("{0}?orderId={1}&orderTotal={2}", GenerateUrl(EndPointPurposes.MemberRewards),orderId,orderTotal), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonReward>>(responseMessage.Data);
                        retreivedRewardList = Mapper.Map<List<Reward>>(jsonList);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.MemberRewards, memberId)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.MemberRewards, memberId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.MemberRewards, memberId)));
            }

            return retreivedRewardList;
        }

        internal bool RedeemRewardForMember(string memberId, string rewardId, Order order)
        {
            
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonOrderIdSimple = Mapper.Map<JsonOrderIdSimple>(order);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.MemberRewardsRedeem, memberId, rewardId), WebRequestMethods.Http.Post, jsonOrderIdSimple.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(EndPointPurposes.MemberRewardsRedeem, memberId, rewardId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(EndPointPurposes.MemberRewardsRedeem, memberId, rewardId)));
                throw new NullOrderReturnedException();
            }
            return false;
        }

        internal bool RedeemRewardForMemberCancel(string memberId, string rewardId, string cancelReason)
        {

            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.MemberRewardsRedeemCancel, memberId, rewardId), WebRequestMethods.Http.Put, "{ \"reason\": \""+cancelReason+"\"}");
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(EndPointPurposes.MemberRewardsRedeemCancel, memberId, rewardId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(EndPointPurposes.MemberRewardsRedeemCancel, memberId, rewardId)));
                throw new NullOrderReturnedException();
            }
            return false;
        }

        internal bool RedeemRewardForMemberConfirm(string memberId, string rewardId)
        {

            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.MemberRewardsRedeemConfirm, memberId, rewardId), WebRequestMethods.Http.Put);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(EndPointPurposes.MemberRewardsRedeemConfirm, memberId, rewardId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(EndPointPurposes.MemberRewardsRedeemConfirm, memberId, rewardId)));
                throw new NullOrderReturnedException();
            }
            return false;
        }

        internal bool RedeemPointsForMember(PointsRedeem pr, Member member)
        {
            DoshiHttpResponseMessage responseMessage;
            //create redeem points object
            try
            {
                var jsonPointsRedeem = Mapper.Map<JsonPointsRedeem>(pr);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.MemberPointsRedeem, member.Id), WebRequestMethods.Http.Post, jsonPointsRedeem.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(EndPointPurposes.MemberPointsRedeem, member.Id)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(EndPointPurposes.MemberPointsRedeem, member.Id)));
                throw new NullOrderReturnedException();
            }
            return false;
        }

        internal bool RedeemPointsForMemberConfirm(Member member)
        {
            DoshiHttpResponseMessage responseMessage;
            //create redeem points object
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.MemberPointsRedeemConfirm, member.Id), WebRequestMethods.Http.Put);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(EndPointPurposes.MemberPointsRedeemConfirm, member.Id)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(EndPointPurposes.MemberPointsRedeemConfirm, member.Id)));
                throw new NullOrderReturnedException();
            }
            return false;
        }

        internal bool RedeemPointsForMemberCancel(Member member, string cancelReason)
        {
            DoshiHttpResponseMessage responseMessage;
            //create redeem points object
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.MemberPointsRedeemCancel, member.Id), WebRequestMethods.Http.Put, "{ \"reason\": \""+cancelReason+"\"}");
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    return true;
                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(EndPointPurposes.MemberPointsRedeemCancel, member.Id)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(EndPointPurposes.MemberPointsRedeemCancel, member.Id)));
                throw new NullOrderReturnedException();
            }
            return false;
        }
        #endregion

        #region checkins / consumers
        /// <summary>
        /// This method is use to get a consumer from Doshii that corresponds with the checkinId 
        /// </summary>
        /// <param name="checkinId">
        /// The checkinId identifying the consumer. 
        /// </param>
        /// <returns>
        /// The consumer returned by doshii
        /// or a blank consumer if no consumer was returned. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal virtual Consumer GetConsumerFromCheckinId(string checkinId)
        {
            var retreivedConsumer = new Consumer();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.ConsumerFromCheckinId, checkinId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonConsumer = JsonConsumer.deseralizeFromJson(responseMessage.Data);
                        retreivedConsumer = Mapper.Map<Consumer>(jsonConsumer);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.ConsumerFromCheckinId, checkinId)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.ConsumerFromCheckinId, checkinId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.ConsumerFromCheckinId, checkinId)));
            }

            return retreivedConsumer;
        }

        internal virtual Checkin PostCheckin(Checkin checkin)
        {
            var retreivedCheckin = new Checkin();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonCheckin = Mapper.Map<JsonCheckin>(checkin);
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Checkins), WebRequestMethods.Http.Post, jsonCheckin.ToJsonString());
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonCheckin = JsonConsumer.deseralizeFromJson(responseMessage.Data);
                        retreivedCheckin = Mapper.Map<Checkin>(jsonCheckin);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Checkins)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Checkins)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Checkins)));
            }

            return retreivedCheckin;
        }

        internal virtual Checkin PutCheckin(Checkin checkin)
        {
            var retreivedCheckin = new Checkin();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonCheckin = Mapper.Map<JsonCheckin>(checkin);
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Checkins, checkin.Id), WebRequestMethods.Http.Put, jsonCheckin.ToJsonString());
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonCheckin = JsonConsumer.deseralizeFromJson(responseMessage.Data);
                        retreivedCheckin = Mapper.Map<Checkin>(jsonCheckin);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Checkins, checkin.Id)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Checkins, checkin.Id)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Checkins, checkin.Id)));
            }

            return retreivedCheckin;
        }

        internal virtual Checkin DeleteCheckin(string checkinId)
        {
            var retreivedCheckin = new Checkin();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId), DeleteMethod);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonCheckin = JsonCheckin.deseralizeFromJson(responseMessage.Data);
                        retreivedCheckin = Mapper.Map<Checkin>(jsonCheckin);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'DELETE' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId)));
            }

            return retreivedCheckin;
        }

        internal virtual Checkin GetCheckin(string checkinId)
        {
            var retreivedCheckin = new Checkin();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonCheckin = JsonConsumer.deseralizeFromJson(responseMessage.Data);
                        retreivedCheckin = Mapper.Map<Checkin>(jsonCheckin);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Checkins, checkinId)));
            }

            return retreivedCheckin;
        }

        internal virtual IEnumerable<Checkin> GetCheckins()
        {
            var retreivedCheckinList = new List<Checkin>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Checkins), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<Checkin>>(responseMessage.Data);
                        retreivedCheckinList = Mapper.Map<List<Checkin>>(jsonList);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Checkins)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Checkins)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Checkins)));
            }

            return retreivedCheckinList;
        }

#endregion

#region Menu
        /// <summary>
        /// Adds a menu to Doshii, this will overwrtie the current menu stored on Doshii 
        /// </summary>
        /// <param name="menu">
        /// The menu to be added to Doshii
        /// </param>
        /// <returns>
        /// The menu that was added to doshii. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal Menu PostMenu(Menu menu)
        {
            var returedMenu = new Menu();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonMenu = Mapper.Map<JsonMenu>(menu);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Menu), WebRequestMethods.Http.Post, jsonMenu.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonMenu = JsonConvert.DeserializeObject<JsonMenu>(responseMessage.Data);
                        returedMenu = Mapper.Map<Menu>(jsonMenu);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Menu)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Menu)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Menu)));
                throw new NullOrderReturnedException();
            }
            return returedMenu;
        }

        /// <summary>
        /// adds or updates a surcount on the pos menu in doshii. 
        /// </summary>
        /// <param name="surcount">
        /// the surcount to be added or updated on Doshii
        /// </param>
        /// <returns>
        /// The surcount that was updated on Doshii
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal Surcount PutSurcount(Surcount surcount)
        {
            var returedSurcount = new Surcount();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonSurcount = Mapper.Map<JsonMenuSurcount>(surcount);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Surcounts, surcount.Id), WebRequestMethods.Http.Put, jsonSurcount.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonSurcount = JsonConvert.DeserializeObject<JsonMenuSurcount>(responseMessage.Data);
                        returedSurcount = Mapper.Map<Surcount>(jsonSurcount);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Surcounts, surcount.Id)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Surcounts, surcount.Id)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Surcounts, surcount.Id)));
                throw new NullOrderReturnedException();
            }
            return returedSurcount;
        }

        /// <summary>
        /// Deletes a surcount from the pos menu on doshii
        /// </summary>
        /// <param name="posId">
        /// The pos Id related to the surcount on Doshii
        /// </param>
        /// <returns></returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal bool DeleteSurcount(string posId)
        {
            var returedMenu = new Menu();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Surcounts, posId), DeleteMethod);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage.Status == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// adds or updates a product on the pos menu on doshii
        /// </summary>
        /// <param name="product">
        /// The product to be added or updated
        /// </param>
        /// <returns>
        /// The product that was added or updated. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal Product PutProduct(Product product)
        {
            var returnedProduct = new Product();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                var jsonProduct = Mapper.Map<JsonMenuProduct>(product);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Products, product.PosId), WebRequestMethods.Http.Put, jsonProduct.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonProduct = JsonConvert.DeserializeObject<JsonMenuProduct>(responseMessage.Data);
                        returnedProduct = Mapper.Map<Product>(jsonProduct);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Products, product.PosId)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(EndPointPurposes.Products, product.PosId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(EndPointPurposes.Products, product.PosId)));
                throw new NullOrderReturnedException();
            }
            return returnedProduct;
        }

        /// <summary>
        /// deletes the product from the pos menu on doshii
        /// </summary>
        /// <param name="posId">
        /// The posId of the product to be deleted. 
        /// </param>
        /// <returns>
        /// true if the product was deleted,
        /// false if the product was not deleted. 
        /// </returns>
        /// <exception cref="RestfulApiErrorResponseException">Thrown when there is an error during the Request to doshii</exception>
        internal bool DeleteProduct(string posId)
        {
            var returedMenu = new Menu();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Products, posId), DeleteMethod);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage.Status == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
#endregion

#region Location
        /// <summary>
        /// This method is used to retrieve the location information for the connected pos from doshii,
        /// </summary>
        /// <returns>
        /// The location information for the connected venue in doshii
        /// </returns>
        internal virtual Location GetLocation()
        {
            var retreivedLocation = new Location();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Location), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }


            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonLocation = JsonLocation.deseralizeFromJson(responseMessage.Data);
                        retreivedLocation = Mapper.Map<Location>(jsonLocation);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Location)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Location)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Location)));
            }

            return retreivedLocation;
        }

#endregion

        #region Tables

        internal virtual Table PostTable(Table table)
        {
            DoshiHttpResponseMessage responseMessage;
            Table returnedTable = null;
            try
            {
                var jsonTable = Mapper.Map<JsonTable>(table);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Tables), WebRequestMethods.Http.Post, jsonTable.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTable = JsonConvert.DeserializeObject<JsonTable>(responseMessage.Data);
                        returnedTable = Mapper.Map<Table>(jsonTable);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Tables)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Tables)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Tables)));
                throw new NullOrderReturnedException();
            }

            return returnedTable;
        }

        internal virtual Table PutTable(Table table)
        {
            DoshiHttpResponseMessage responseMessage;
            Table returnedTable = null;
            try
            {
                var jsonTable = Mapper.Map<JsonTable>(table);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Tables, table.Name), WebRequestMethods.Http.Put, jsonTable.ToJsonString());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTable = JsonConvert.DeserializeObject<JsonTable>(responseMessage.Data);
                        returnedTable = Mapper.Map<Table>(jsonTable);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Tables, table.Name)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Tables, table.Name)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Tables, table.Name)));
                throw new NullOrderReturnedException();
            }

            return returnedTable;
        }

        internal virtual Table PutTables(List<Table> tables)
        {
            DoshiHttpResponseMessage responseMessage;
            Table returnedTable = null;
            try
            {
                List<JsonTable> jsonTableList = new List<JsonTable>();
                foreach (var t in tables)
                {
                    jsonTableList.Add(Mapper.Map<JsonTable>(t));
                }
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Tables), WebRequestMethods.Http.Put, JsonConvert.SerializeObject(jsonTableList));
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTable = JsonConvert.DeserializeObject<JsonTable>(responseMessage.Data);
                        returnedTable = Mapper.Map<Table>(jsonTable);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Tables)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Tables)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Tables)));
                throw new NullOrderReturnedException();
            }

            return returnedTable;
        }

        internal virtual Table DeleteTable(string tableName)
        {
            DoshiHttpResponseMessage responseMessage;
            Table returnedTable = null;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Tables, tableName), DeleteMethod);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTable = JsonConvert.DeserializeObject<JsonTable>(responseMessage.Data);
                        returnedTable = Mapper.Map<Table>(jsonTable);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Tables, tableName)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'DELETE' request to {0} was not successful", GenerateUrl(EndPointPurposes.Tables, tableName)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'DELETE' and URL '{0}'", GenerateUrl(EndPointPurposes.Tables, tableName)));
                throw new NullOrderReturnedException();
            }

            return returnedTable;
        }

        internal virtual Table GetTable(string tableName)
        {
            DoshiHttpResponseMessage responseMessage;
            Table returnedTable = null;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Tables, tableName), WebRequestMethods.Http.Get);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTable = JsonConvert.DeserializeObject<JsonTable>(responseMessage.Data);
                        returnedTable = Mapper.Map<Table>(jsonTable);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Tables, tableName)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(EndPointPurposes.Tables, tableName)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(EndPointPurposes.Tables, tableName)));
                throw new NullOrderReturnedException();
            }

            return returnedTable;
        }

        internal virtual IEnumerable<Table> GetTables()
        {
            var retreivedtableList = new List<Table>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Tables), WebRequestMethods.Http.Get);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonList = JsonConvert.DeserializeObject<List<JsonTable>>(responseMessage.Data);
                        retreivedtableList = Mapper.Map<List<Table>>(jsonList);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(EndPointPurposes.Tables)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(EndPointPurposes.Tables)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(EndPointPurposes.Tables)));
            }

            return retreivedtableList;
        }

#endregion

        #region comms helper methods

        /// <summary>
        /// Generates a URL based on the base URL and the purpose of the message that is being sent. 
        /// </summary>
        /// <param name="purpose">
        /// An <see cref="EndPointPurposes"/> the represents the purpose of the request
        /// </param>
        /// <param name="identification">
        /// An optional identifier used in the request 
        /// eg, the orderId for a get order request
        /// </param>
        /// <returns>
        /// The Url required to make the desiered request. 
        /// </returns>
        internal string GenerateUrl(EndPointPurposes purpose, string identification = "", string secondIdentification = "")
        {
            StringBuilder newUrlbuilder = new StringBuilder();

            if (string.IsNullOrWhiteSpace(m_DoshiiUrlBase))
            {
				mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Error, "Doshii: The DoshiiHttpCommunication class was not initialized correctly, the base URl is null or white space");
                return newUrlbuilder.ToString();
            }
            newUrlbuilder.AppendFormat("{0}", m_DoshiiUrlBase);

            switch (purpose)
            {
				case EndPointPurposes.Order:
                    newUrlbuilder.Append("/orders");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.DeleteAllocationFromCheckin:
                    newUrlbuilder.AppendFormat("/tables?checkin={0}", identification);
                    break;
                case EndPointPurposes.Transaction:
                    newUrlbuilder.Append("/transactions");
					if (!String.IsNullOrWhiteSpace(identification))
					{
						newUrlbuilder.AppendFormat("/{0}", identification);
					}
                    break;
                case EndPointPurposes.TransactionFromDoshiiOrderId:
                    newUrlbuilder.AppendFormat("/unlinked_orders/{0}/transactions", identification);
                    break;
                case EndPointPurposes.UnlinkedOrders:
                    newUrlbuilder.Append("/unlinked_orders");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.ConsumerFromCheckinId:
                    newUrlbuilder.AppendFormat("/checkins/{0}/consumer", identification);
                    break;
                case EndPointPurposes.Menu:
                    newUrlbuilder.AppendFormat("/menu");
                    break;
                case EndPointPurposes.Products:
                    newUrlbuilder.Append("/menu/products");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.Surcounts:
                    newUrlbuilder.Append("/menu/surcounts");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.Location:
                    newUrlbuilder.Append("/location");
                    break;
                case EndPointPurposes.Members:
                    newUrlbuilder.Append("/members");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.MemberRewards:
                    newUrlbuilder.AppendFormat("/members/{0}/rewards", identification);
                    break;
                case EndPointPurposes.MemberRewardsRedeem:
                    newUrlbuilder.AppendFormat("/members/{0}/rewards/{1}/redeem", identification, secondIdentification);
                    break;
                case EndPointPurposes.MemberRewardsRedeemConfirm:
                    newUrlbuilder.AppendFormat("/members/{0}/rewards/{1}/confirm", identification, secondIdentification);
                    break;
                case EndPointPurposes.MemberRewardsRedeemCancel:
                    newUrlbuilder.AppendFormat("/members/{0}/rewards/{1}/cancel", identification, secondIdentification);
                    break;
                case EndPointPurposes.MemberPointsRedeem:
                    newUrlbuilder.AppendFormat("/members/{0}/points/redeem", identification);
                    break;
                case EndPointPurposes.MemberPointsRedeemConfirm:
                    newUrlbuilder.AppendFormat("/members/{0}/points/confirm", identification);
                    break;
                case EndPointPurposes.MemberPointsRedeemCancel:
                    newUrlbuilder.AppendFormat("/members/{0}/points/cancel", identification);
                    break;
                case EndPointPurposes.Checkins:
                    newUrlbuilder.Append("/checkins");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case EndPointPurposes.Tables:
                    newUrlbuilder.Append("/tables");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                default:
                    throw new NotSupportedException(purpose.ToString());
            }

            return newUrlbuilder.ToString();
        }

        /// <summary>
        /// makes a request HTTP to doshii based on the parameters provided. 
        /// </summary>
        /// <param name="url">
        /// The URL for the request - should be generated by <see cref="GenerateUrl"/>
        /// </param>
        /// <param name="method">
        /// The HTTP verb used for the request
        /// the following four verbs can be used
        /// <Item>GET</Item>
        /// <Item>PUT</Item>
        /// <Item>POST</Item>
        /// <Item>DELETE</Item>
        /// </param>
        /// <param name="data">
        /// The data that will be sent with the request. 
        /// eg, a JSON representation of the order that should be send to Doshii with a PUT order request. 
        /// </param>
        /// <returns></returns>
        /// <exception cref="RestfulApiErrorResponseException">Is thrown when any of the following responses are received.
        /// <item> HttpStatusCode.BadRequest </item> 
        /// <item> HttpStatusCode.Unauthorized </item> 
        /// <item> HttpStatusCode.Forbidden </item>
        /// <item> HttpStatusCode.InternalServerError </item>
        /// <item> HttpStatusCode.NotFound </item> 
        /// <item> HttpStatusCode.Conflict </item>
        /// This must be handled where a conflict needs special treatment - this is especially important when orders are being updated by both the pos and the partner. 
        /// </exception>
        internal virtual DoshiHttpResponseMessage MakeRequest(string url, string method, string data = "")
        {
            if (string.IsNullOrWhiteSpace(url))
            {
				mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Error, string.Format("MakeRequest was called without a URL"));
                throw new NotSupportedException("request with blank URL");
            }

            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Headers.Add("authorization", m_DoshiiLogic.CreateToken());
            request.Headers.Add("vendor", m_DoshiiLogic.Vendor);
            request.ContentType = "application/json";


            if (method.Equals(WebRequestMethods.Http.Get) || 
				method.Equals(WebRequestMethods.Http.Put) || 
				method.Equals(WebRequestMethods.Http.Post) || 
				method.Equals(DoshiiHttpCommunication.DeleteMethod))
            {
                request.Method = method;
            }
            else
            {
				mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Error, string.Format("MakeRequest was called with a non supported HTTP request method type - '{0}", method));
                throw new NotSupportedException("Invalid HTTP request Method Type");
            }
            if (!string.IsNullOrWhiteSpace(data))
            {
                

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(data);
                    writer.Close();
                }
            }

            DoshiHttpResponseMessage responceMessage = new DoshiHttpResponseMessage();
            try
            {
				mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: generating {0} request to endpoint {1}, with data {2}", method, url, data));
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                responceMessage.Status = response.StatusCode;
                responceMessage.StatusDescription = responceMessage.StatusDescription;

                StreamReader sr = new StreamReader(response.GetResponseStream());
                responceMessage.Data = sr.ReadToEnd();

                sr.Close();
                response.Close();

                if (responceMessage.Status == HttpStatusCode.OK || responceMessage.Status == HttpStatusCode.Created)
                {
					mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Info, string.Format("Doshii: Successful response from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                }
                else if (responceMessage.Status == HttpStatusCode.BadRequest || 
                    responceMessage.Status == HttpStatusCode.Unauthorized || 
                    responceMessage.Status == HttpStatusCode.Forbidden || 
                    responceMessage.Status == HttpStatusCode.InternalServerError || 
                    responceMessage.Status == HttpStatusCode.NotFound || 
                    responceMessage.Status == HttpStatusCode.Conflict ||
                    responceMessage.Status == (HttpStatusCode)456) //Upstream rejected
                {
					mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: Failed response from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                    throw new Exceptions.RestfulApiErrorResponseException(responceMessage.Status, responceMessage.Message);
                }
                else
                {
					mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: Failed response from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                }
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (WebResponse response = wex.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse) response;
                        //Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                        string errorResponce;
                        using (Stream responceErrorData = response.GetResponseStream())
                        {
                            using (var reader = new StreamReader(responceErrorData))
                            {
                                errorResponce = reader.ReadToEnd();
                                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Error,
                                    String.Format("Error code: {0}, ErrorResponse {1}", httpResponse.StatusCode,
                                        errorResponce));
                            }
                        }
                        if (httpResponse.StatusCode == HttpStatusCode.BadRequest ||
                            httpResponse.StatusCode == HttpStatusCode.Unauthorized ||
                            httpResponse.StatusCode == HttpStatusCode.Forbidden ||
                            httpResponse.StatusCode == HttpStatusCode.InternalServerError ||
                            httpResponse.StatusCode == HttpStatusCode.NotFound ||
                            httpResponse.StatusCode == HttpStatusCode.Conflict)
                        {
                            mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Error,
                                string.Format(
                                    "Doshii: A  WebException was thrown while attempting a {0} request to endpoint {1}, with data {2}, error Response {3}, exception {4}",
                                    method, url, data, errorResponce, wex));
                            throw new Exceptions.RestfulApiErrorResponseException(httpResponse.StatusCode, errorResponce, wex);
                        }
                        else
                        {
                            mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Error,
                                string.Format(
                                    "Doshii: A  WebException was thrown while attempting a {0} request to endpoint {1}, with data {2}, error Response {3}, exception {4}",
                                    method, url, data, errorResponce, wex));
                        }
                    }
                }
                else
                {
                    throw new Exceptions.RestfulApiErrorResponseException("There was no response in the web exception while making a request.");
                }
                
            }
            catch (Exception ex)
            {
				mLog.LogMessage(typeof(DoshiiHttpCommunication), Enums.DoshiiLogLevels.Error, string.Format("Doshii: As exception was thrown while attempting a {0} request to endpoint {1}, with data {2} and status {3} : {4}", method, url, data, responceMessage.Status.ToString(), ex));
				throw new Exceptions.RestfulApiErrorResponseException(responceMessage.Status, ex);
            }

            return responceMessage;
        }

        

#endregion

    }
}
