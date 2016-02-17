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

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    /// <summary>
    /// DO NOT USE, This class is used internally by the SDK and should not be instantiated by the pos.
    /// </summary>
    internal class DoshiiHttpCommunication 
    {
		/// <summary>
		/// The HTTP request method for a DELETE endpoint action.
		/// </summary>
		private const string DeleteMethod = "DELETE";

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// The base URL for HTTP communication with Doshii
        /// </summary>
		internal string m_DoshiiUrlBase { get; private set; }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Doshii operation logic
        /// </summary>
		internal DoshiiManager m_DoshiiLogic { get; private set; }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// The token used for authentication with doshii
        /// </summary>
		internal string m_Token { get; private set; }

		/// <summary>
		/// The logging callback mechanism for the POS.
		/// </summary>
		internal DoshiiLogManager mLog { get; private set; }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// constructor
        /// </summary>
        /// <param name="urlBase"></param>
		/// <param name="token"></param>
		/// <param name="logManager"></param>
        /// <param name="doshiiLogic"></param>
		internal DoshiiHttpCommunication(string urlBase, string token, DoshiiLogManager logManager, DoshiiManager doshiiLogic)
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

            mLog.LogMessage(typeof(DoshiiHttpCommunication), Enums.DoshiiLogLevels.Debug, string.Format("Instantiating DoshiiHttpCommunication Class with; urlBase - '{0}', token - '{1}'", urlBase, token));
            if (string.IsNullOrWhiteSpace(urlBase))
            {
				mLog.LogMessage(typeof(DoshiiHttpCommunication), Enums.DoshiiLogLevels.Error, string.Format("Instantiating DoshiiHttpCommunication Class with a blank urlBase - '{0}'", urlBase));
                throw new ArgumentException("blank URL");
            
            }
            if (string.IsNullOrWhiteSpace(token))
            {
				mLog.LogMessage(typeof(DoshiiHttpCommunication), Enums.DoshiiLogLevels.Error, string.Format("Instantiating DoshiiHttpCommunication Class with a blank token - '{0}'", token));
                throw new ArgumentException("blank token");
            }
            
            m_DoshiiUrlBase = urlBase;
            m_Token = token;
        }

        #region internal  methods 

        #region order methods

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// This method is used to retrieve the order from Doshii matching the provided orderId, if no order matches the provided orderId a new order is returned. 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
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
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// This method is used to retrieve the order from Doshii matching the provided orderId, if no order matches the provided orderId a new order is returned. 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        internal virtual Order GetOrderromDoshiiOrderId(string orderId)
        {
            var retreivedOrder = new Order();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.UnlinkedOrders, orderId), WebRequestMethods.Http.Get);
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
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// This method is used to retrieve the order from Doshii matching the provided orderId, if no order matches the provided orderId a new order is returned. 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        internal virtual IEnumerable<Transaction> GetTransactionFromDoshiiOrderId(string orderId)
        {
            var retreivedTransactionList = new List<Transaction>();
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.TransactionFromDoshiiOrderId, orderId), WebRequestMethods.Http.Get);
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
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.TransactionFromDoshiiOrderId, orderId)));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.TransactionFromDoshiiOrderId, orderId)));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.TransactionFromDoshiiOrderId, orderId)));
            }

            return retreivedTransactionList;
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// This method is used to retrieve the order from Doshii matching the provided orderId, if no order matches the provided orderId a new order is returned. 
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
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
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Gets all the current active orders in Doshii, if there are no active orders an empty list is returned. 
        /// </summary>
        /// <returns></returns>
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

            return retreivedOrderList;
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Gets all the current active orders in Doshii, if there are no active orders an empty list is returned. 
        /// </summary>
        /// <returns></returns>
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

            return retreivedOrderList;
        }

		/// <summary>
		/// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
		/// Gets all the current active transactions in Doshii, if there are no active transactions an empty list is returned. 
		/// </summary>
		/// <returns></returns>
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
		/// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
		/// Creates an order in Doshii including the allocation of the table.
		/// </summary>
		/// <param name="tableOrder">The details of the order and table allocation to present to Doshii.</param>
		/// <returns>The table order details as uploaded to Doshii API.</returns>
		internal virtual Order PutOrderWithTableAllocation(TableOrder tableOrder)
		{
			var returnedTableOrder = new Order();
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
            //this call need to exist to record the Order.version
			return HandleOrderResponse<Order, JsonOrder>(orderIdentifier, responseMessage, out dto);
		}

        
        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Deletes a table allocaiton from doshii for the provided order Id. 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// completes the Put or Post request to update an order with Doshii. 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="method"></param>
        /// <returns></returns>
		/// <exception cref="System.NotSupportedException">Currently thrown when the method is not <see cref="System.Net.WebRequestMethods.Http.Put"/>.</exception>
        internal Order PutPostOrder(Order order, string method)
        {
            if (!method.Equals(WebRequestMethods.Http.Put))
            {
                throw new NotSupportedException("Method Not Supported");
            }

            var returnOrder = new Order();
            DoshiHttpResponseMessage responseMessage;
            OrderToPut orderToPut = new OrderToPut();

            if (order.Status == "accepted")
            {
                orderToPut.Status = "accepted";
            }
            else if (order.Status == "complete")
            {
                orderToPut.Status = "complete";
            }
            else
            {
                orderToPut.Status = "rejected";
            }

            orderToPut.Items = order.Items;
            orderToPut.Surcounts = order.Surcounts;
            try
            {
                var jsonOrderToPut = Mapper.Map<JsonOrderToPut>(orderToPut);
                if (String.IsNullOrEmpty(order.Id))
                {
                    responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.UnlinkedOrders, order.DoshiiId), method, jsonOrderToPut.ToJsonStringForOrder());
                }
                else
                {
                    responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Order, order.Id), method, jsonOrderToPut.ToJsonStringForOrder());
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
					mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was OK"));
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

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// completes the Put or Post request to update an order with Doshii. 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">Currently thrown when the method is not <see cref="System.Net.WebRequestMethods.Http.Put"/>.</exception>
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
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTransaction = JsonConvert.DeserializeObject<JsonTransaction>(responseMessage.Data);
                        returnedTransaction = Mapper.Map<Transaction>(jsonTransaction);
                        if (returnedTransaction != null)
                        {
                            m_DoshiiLogic.RecordTransactionVersion(returnedTransaction.Id, returnedTransaction.Version);
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
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// completes the Put request to update a transaction, the transaction but be existing 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">Currently thrown when the method is not <see cref="System.Net.WebRequestMethods.Http.Put"/>.</exception>
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
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        var jsonTransaction = JsonConvert.DeserializeObject<JsonTransaction>(responseMessage.Data);
                        returnedTransaction = Mapper.Map<Transaction>(jsonTransaction);
                        if (returnedTransaction != null)
                        {
                            m_DoshiiLogic.RecordTransactionVersion(returnedTransaction.Id, returnedTransaction.Version);
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
        
        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// This method is used to confirm or reject or update an order when the order has an OrderId
        /// </summary>
        /// <param name="order"></param>
        /// <returns>
        /// If the request is not successful a new order will be returned - you can check the order.Id in the returned order to confirm it is a valid response. 
        /// </returns>
        internal virtual Order PutOrder(Order order)
        {
            return PutPostOrder(order, WebRequestMethods.Http.Put);
        }

        /// <summary>
		/// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
		/// This method is used to retrieve the Doshii Configuration
		/// </summary>
		/// <returns>The current configuration in Doshii.</returns>
		internal virtual Configuration GetConfig()
		{
			DoshiHttpResponseMessage responseMessage;

        #endregion

        #region comms helper methods

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Generates a URL based on the base URL and the purpose of the message that is being sent. 
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="identification"></param>
        /// <returns></returns>
        internal string GenerateUrl(EndPointPurposes purpose, string identification = "")
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
                case EndPointPurposes.GetTableAllocations:
                    newUrlbuilder.Append("/tables");
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
                    newUrlbuilder.AppendFormat("unlinked_orders/{0}/transactions", identification);
                    break;
                case EndPointPurposes.UnlinkedOrders:
                    newUrlbuilder.Append("/unlinked_orders");
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
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// makes a request to doshii based on the parameters provided. 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="RestfulApiErrorResponseException">Is thrown when any of the following responses are received.
        /// <item> HttpStatusCode.BadRequest </item> 
        /// <item> HttpStatusCode.Unauthorized </item> 
        /// <item> HttpStatusCode.Forbidden </item>
        /// <item> HttpStatusCode.InternalServerError </item>
        /// <item> HttpStatusCode.NotFound </item> 
        /// <item> HttpStatusCode.Conflict </item>
        /// This must be handled where a conflict needs special treatment - this is especially important when order are being updated by both the pos and the partner. 
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
            request.Headers.Add("authorization", m_Token);

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
                request.ContentType = "application/json";

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(data);
                    writer.Close();
                }
            }

            DoshiHttpResponseMessage responceMessage = new DoshiHttpResponseMessage();
            try
            {
				mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: generating {0} request to endpoint {1}, with data {2}", method, url, data));
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                responceMessage.Status = response.StatusCode;
                responceMessage.StatusDescription = responceMessage.StatusDescription;

                StreamReader sr = new StreamReader(response.GetResponseStream());
                responceMessage.Data = sr.ReadToEnd();

                sr.Close();
                response.Close();

                if (responceMessage.Status == HttpStatusCode.OK || responceMessage.Status == HttpStatusCode.Created)
                {
					mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: Successful response from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                }
                else if (responceMessage.Status == HttpStatusCode.BadRequest || 
                    responceMessage.Status == HttpStatusCode.Unauthorized || 
                    responceMessage.Status == HttpStatusCode.Forbidden || 
                    responceMessage.Status == HttpStatusCode.InternalServerError || 
                    responceMessage.Status == HttpStatusCode.NotFound || 
                    responceMessage.Status == HttpStatusCode.Conflict)
                {
					mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: Failed response from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                    throw new Exceptions.RestfulApiErrorResponseException(responceMessage.Status);
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
                using (WebResponse response = wex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    string errorResponce;
                    using (Stream responceErrorData = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(responceErrorData))
                        {
                            errorResponce = reader.ReadToEnd();
                        }
                    }
                    if (httpResponse.StatusCode == HttpStatusCode.BadRequest || 
                        httpResponse.StatusCode == HttpStatusCode.Unauthorized || 
                        httpResponse.StatusCode == HttpStatusCode.Forbidden ||
                        httpResponse.StatusCode == HttpStatusCode.InternalServerError || 
                        httpResponse.StatusCode == HttpStatusCode.NotFound || 
                        httpResponse.StatusCode == HttpStatusCode.Conflict)
                    {
						mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Error, string.Format("Doshii: A  WebException was thrown while attempting a {0} request to endpoint {1}, with data {2}, error Response {3}, exception {4}", method, url, data, errorResponce, wex));
                        throw new Exceptions.RestfulApiErrorResponseException(httpResponse.StatusCode);
                    }
                    else
                    {
						mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Error, string.Format("Doshii: A  WebException was thrown while attempting a {0} request to endpoint {1}, with data {2}, error Response {3}, exception {4}", method, url, data, errorResponce, wex));
                    }
                }
            }
            catch (Exception ex)
            {
				mLog.LogMessage(typeof(DoshiiHttpCommunication), Enums.DoshiiLogLevels.Error, string.Format("Doshii: As exception was thrown while attempting a {0} request to endpoint {1}, with data {2} : {4}", method, url, data, responceMessage.Status.ToString(), ex));
            }

            return responceMessage;
        }

        #endregion

    }
}
