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
        /// Gets all the current active table allocations in doshii, if there are no current active table allocations an empty list is returned. 
        /// </summary>
        /// <returns></returns>
        internal virtual List<TableAllocation> GetTableAllocations()
        {
            var tableAllocationList = new List<TableAllocation>();
            DoshiHttpResponseMessage responseMessage;

            try
            {
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.GetTableAllocations), WebRequestMethods.Http.Get);
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (responseMessage.Data != null)
                    {
						var jsonList = JsonConvert.DeserializeObject<List<JsonTableAllocation>>(responseMessage.Data);
						tableAllocationList = Mapper.Map<List<TableAllocation>>(jsonList);
                    }
                    else
                    {
						mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.GetTableAllocations)));
                    }

                }
                else
                {
					mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.GetTableAllocations)));
                }
            }
            else
            {
				mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.GetTableAllocations)));
            }

            return tableAllocationList;

        }
        
        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Rejects a table allocation doshii has sent for approval. 
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="tableName"></param>
        /// <param name="tableAllocation"></param>
        /// <returns></returns>
        internal virtual bool DeleteTableAllocationWithCheckInId(string checkInId, TableAllocationRejectionReasons rejectionReasons)
        {

            bool success = false;
            
            DoshiHttpResponseMessage responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.DeleteAllocationWithCheckInId, checkInId), 
					DoshiiHttpCommunication.DeleteMethod, SerializeTableDeAllocationRejectionReason(rejectionReasons));
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    success = true;
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Serializes the table allocation rejection reason into the required string to pass to Doshii. 
        /// </summary>
        /// <param name="rejectionReason"></param>
        /// <returns></returns>
        internal virtual string SerializeTableDeAllocationRejectionReason(Enums.TableAllocationRejectionReasons rejectionReason)
        {
            string reasonCodeString = "";
            switch (rejectionReason)
            {
                case Enums.TableAllocationRejectionReasons.TableDoesNotExist:
                    reasonCodeString = "{\"reasonCode\" : \"1\"}";
                    break;
                case Enums.TableAllocationRejectionReasons.TableIsOccupied:
                    reasonCodeString = "{\"reasonCode\" : \"2\"}";
                    break;
                case Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos:
                    reasonCodeString = "{\"reasonCode\" : \"3\"}";
                    break;
                case Enums.TableAllocationRejectionReasons.ConcurrencyIssueWithPos:
                    reasonCodeString = "{\"reasonCode\" : \"4\"}";
                    break;
                case Enums.TableAllocationRejectionReasons.tableDoesNotHaveATab:
                    reasonCodeString = "{\"reasonCode\" : \"5\"}";
                    break;
                case Enums.TableAllocationRejectionReasons.tableHasBeenPaid:
                    reasonCodeString = "{\"reasonCode\" : \"6\"}";
                    break;
                case Enums.TableAllocationRejectionReasons.unknownError:
                    reasonCodeString = "{\"reasonCode\" : \"7\"}";
                    break;
            }
            return reasonCodeString;
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

            string orderIdentifier = order.Id;
            var returnOrder = new Order();
            DoshiHttpResponseMessage responseMessage;
            OrderToPut orderToPut = new OrderToPut();

            if (order.Status == "accepted")
            {
                orderToPut.Status = "accepted";
            }
            else if (order.Status == "paid")
            {
                orderToPut.Status = "paid";
            }
            else if (order.Status == "waiting_for_payment")
            {
                orderToPut.Status = "waiting_for_payment";
            }
            else
            {
                orderToPut.Status = "rejected";
            }

            orderToPut.Items = order.Items;
            orderToPut.Surcounts = order.Surcounts;
            orderToPut.Payments = order.Payments;
            try
            {
                var jsonOrderToPut = Mapper.Map<JsonOrderToPut>(orderToPut);
                responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Order, orderIdentifier), method, jsonOrderToPut.ToJsonStringForOrder());
            }
            catch (RestfulApiErrorResponseException rex)
            {
                throw rex;
            }

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
                        var jsonOrder = JsonConvert.DeserializeObject<JsonOrder>(responseMessage.Data);
                        returnOrder = Mapper.Map<Order>(jsonOrder);
                        m_DoshiiLogic.RecordOrderVersion(returnOrder.Id, returnOrder.Version);
                    }
                    else
                    {
                        mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Order, order.Id.ToString())));
                    }

                }
                else
                {
                    mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order, order.Id.ToString())));
                }
            }
            else
            {
                mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Order, order.Id.ToString())));
                throw new NullOrderReturnedException();
            }

            return returnOrder;
        }


        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// completes the Put or Post request to update an order with Doshii. 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">Currently thrown when the method is not <see cref="System.Net.WebRequestMethods.Http.Put"/>.</exception>
        internal Transaction PostTransaction(Transaction transaction)
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
		internal Configuration GetConfig()
		{
			DoshiHttpResponseMessage responseMessage;

			try
			{
				responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Configuration), WebRequestMethods.Http.Get);
			}
			catch (RestfulApiErrorResponseException rex)
			{
				throw rex;
			}

			if (responseMessage != null)
			{
				if (responseMessage.Status == HttpStatusCode.OK)
				{
					if (responseMessage.Data != null)
					{
						var jsonConfig = JsonConvert.DeserializeObject<JsonConfiguration>(responseMessage.Data);
						return Mapper.Map<Configuration>(jsonConfig);
					}
					else
					{
						mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.GetTableAllocations)));
					}

				}
				else
				{
					mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.GetTableAllocations)));
				}
			}
			else
			{
				mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.GetTableAllocations)));
			}

			return null;
		}

		/// <summary>
		/// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
		/// This method uploads the supplied <paramref name="config"/> to Doshii.
		/// </summary>
		/// <param name="config">The configuration to be uploaded to Doshii.</param>
		/// <returns>True on successful upload; false otherwise.</returns>
		internal bool PutConfiguration(Configuration config)
		{
			bool result = false;
			DoshiHttpResponseMessage responseMessage;

			try
			{
				var jsonConfig = Mapper.Map<JsonConfiguration>(config);
				responseMessage = MakeRequest(GenerateUrl(EndPointPurposes.Configuration), WebRequestMethods.Http.Put, jsonConfig.ToJsonString());
			}
			catch (RestfulApiErrorResponseException rex)
			{
				throw rex;
			}

			mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message has been returned to the put config function"));

			if (responseMessage != null)
			{
				mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

				if (responseMessage.Status == HttpStatusCode.OK)
				{
					result = true;
				}
				else
				{
					mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, string.Format("Doshii: The Response status code for PUT /config was {0}", responseMessage.Status));
				}
			}
			else
			{
				mLog.LogMessage(typeof(DoshiiHttpCommunication), DoshiiLogLevels.Warning, "Doshii: The Response for PUT /config was null");
			}

			return result;
		}

        #endregion

        #endregion

        #region comms helper methods

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Generates a URL based on the base URL and the purpose of the message that is being sent. 
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="identification"></param>
        /// <returns></returns>
        private string GenerateUrl(EndPointPurposes purpose, string identification = "", string tableName = "")
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
				case EndPointPurposes.Configuration:
					newUrlbuilder.Append("/config");
					break;
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
                case EndPointPurposes.DeleteAllocationWithCheckInId:
                    newUrlbuilder.AppendFormat("/tables?checkin={0}", identification);
                    break;
                case EndPointPurposes.Transaction:
                    newUrlbuilder.AppendFormat("/transactions/:{0}", identification);
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

            if (method.Equals(WebRequestMethods.Http.Get) || method.Equals(WebRequestMethods.Http.Put) || method.Equals(DoshiiHttpCommunication.DeleteMethod))
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
