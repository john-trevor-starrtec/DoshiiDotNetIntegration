using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    /// <summary>
    /// DO NOT USE, This class is used internally by the SDK and should not be instantiated by the pos.
    /// </summary>
    internal class DoshiiHttpCommunication 
    {
        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// The base URL for HTTP communication with Doshii
        /// </summary>
        internal  string m_DoshiiUrlBase;

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Doshii operation logic
        /// </summary>
        internal  DoshiiManager m_DoshiiLogic;

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// The token used for authentication with doshii
        /// </summary>
        internal  string m_Token;

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// constructor
        /// </summary>
        /// <param name="urlBase"></param>
        /// <param name="doshiiLogic"></param>
        /// <param name="token"></param>
        internal DoshiiHttpCommunication(string urlBase, DoshiiManager doshiiLogic, string token)
        {
            if (doshiiLogic == null)
            {
                throw new NotSupportedException("doshiiLogic");
            }

            m_DoshiiLogic = doshiiLogic;
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Instantiating DoshiiHttpCommunication Class with; urlBase - '{0}', token - '{1}'", urlBase, token));
            if (string.IsNullOrWhiteSpace(urlBase))
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Instantiating DoshiiHttpCommunication Class with a blank urlBase - '{0}'", urlBase));
                throw new NotSupportedException("blank URL");
            
            }
            if (string.IsNullOrWhiteSpace(token))
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Instantiating DoshiiHttpCommunication Class with a blank token - '{0}'", token));
                throw new NotSupportedException("blank token");
            }
            
            m_DoshiiUrlBase = urlBase;
            m_Token = token;
        }

        #region internal  methods 

        #region CheckIn and allocate methods

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Gets a consumer from Doshii with the meerkatCustomerId provided, if no consumer exists a new consumer object is returned. 
        /// </summary>
        /// <param name="meerkatCustomerId"></param>
        /// <returns></returns>
        internal virtual Models.Consumer GetConsumer(string meerkatCustomerId)
        {
                
            Models.Consumer retreivedConsumer = new Models.Consumer();
            DoshiHttpResponceMessages responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Consumer, meerkatCustomerId), "GET");

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
                        retreivedConsumer = Models.Consumer.deseralizeFromJson(responseMessage.Data);
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Consumer, meerkatCustomerId)));
                    }
                    
                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Consumer, meerkatCustomerId)));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Consumer, meerkatCustomerId)));
            }

            return retreivedConsumer;
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Gets all the consumers currently checkedIn with Doshii, if no customers are found en empty list is returned. 
        /// </summary>
        /// <returns></returns>
        internal virtual List<Models.Consumer> GetConsumers()
        {

            List<Models.Consumer> retreivedConsumerList = new List<Models.Consumer>();
            DoshiHttpResponceMessages responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Consumer), "GET");
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
                        retreivedConsumerList = JsonConvert.DeserializeObject<List<Models.Consumer>>(responseMessage.Data);
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Consumer)));
                    }
                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Consumer)));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Consumer)));
            }
            
            return retreivedConsumerList;
        }
        #endregion

        #region order methods

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// This method is used to retrieve the order from Doshii matching the provided orderId, if no order matches the provided orderId a new order is returned. 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        internal virtual Models.Order GetOrder(string orderId)
        {
            Models.Order retreivedOrder = new Models.Order();
            DoshiHttpResponceMessages responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Order, orderId), "GET");
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
                        retreivedOrder = Models.Order.deseralizeFromJson(responseMessage.Data);
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                    }

                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
            }

            return retreivedOrder;
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Gets all the current active orders in Doshii, if there are no active orders an empty list is returned. 
        /// </summary>
        /// <returns></returns>
        internal virtual List<Models.Order> GetOrders()
        {
            List<Models.Order> retreivedOrderList = new List<Models.Order>();
            DoshiHttpResponceMessages responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Order), "GET");
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
                        retreivedOrderList = JsonConvert.DeserializeObject<List<Models.Order>>(responseMessage.Data);
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Order)));
                    }

                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order)));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Order)));
            }

            return retreivedOrderList;
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Gets all the current active table allocations in doshii, if there are no current active table allocations an empty list is returned. 
        /// </summary>
        /// <returns></returns>
        internal virtual List<Models.TableAllocation> GetTableAllocations()
        {
            List<Models.TableAllocation> tableAllocationList = new List<Models.TableAllocation>();
            DoshiHttpResponceMessages responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.GetTableAllocations), "GET");
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (responseMessage.Data != null)
                    {
                        tableAllocationList = JsonConvert.DeserializeObject<List<Models.TableAllocation>>(responseMessage.Data);
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.GetTableAllocations)));
                    }

                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.GetTableAllocations)));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.GetTableAllocations)));
            }

            return tableAllocationList;

        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Attempts to put a table allocation to doshii, if successful returns true, else returns false. 
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal virtual bool PutTableAllocation(string consumerId, string tableName)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.ConfirmTableAllocation, consumerId, tableName), "PUT", "{\"status\": \"confirmed\"}");

            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
            if (responseMessage.Status == HttpStatusCode.OK)
            {
                success = true;
            }
            else
            {
                success = false;
                
            }

            return success;
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Attempts to post a table allocation to doshii, if successful returns true, else returns false. 
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal virtual bool PostTableAllocation(string consumerId, string tableName)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            StringBuilder builder = new StringBuilder();
            builder.Append("{\"tableName\": \"");
            builder.AppendFormat("{0}", tableName);
            builder.Append("\"}");
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.AddTableAllocation, consumerId), "POST", builder.ToString());
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
            if (responseMessage.Status == HttpStatusCode.OK)
            {
                success = true;
            }
            else
            {
                success = false;
            }

            return success;
        }
        
        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Rejects a table allocation doshii has sent for approval. 
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="tableName"></param>
        /// <param name="tableAllocation"></param>
        /// <returns></returns>
        internal virtual bool DeleteTableAllocationWithCheckInId(string checkInId, Enums.TableAllocationRejectionReasons rejectionReasons)
        {

            bool success = false;
            
            DoshiHttpResponceMessages responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.DeleteAllocationWithCheckInId, checkInId), "DELETE", SerializeTableDeAllocationRejectionReason(rejectionReasons));
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
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
        /// Sets the seating and order configuration. 
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="tableName"></param>
        /// <param name="tableAllocation"></param>
        /// <returns></returns>
        internal virtual bool SetSeatingAndOrderConfiguration(Enums.SeatingModes seatingMode, Enums.OrderModes orderMode)
        {

            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            //create the configuration message
            StringBuilder configString = new StringBuilder();
            configString.Append("{ \"restaurantMode\":");
            if (orderMode == Enums.OrderModes.BistroMode)
            {
                configString.Append(" \"bistro\",");
            }
            else
            {
                configString.Append(" \"restaurant\",");
            }

            configString.Append(" \"tableMode\": ");

            if (seatingMode == Enums.SeatingModes.PosAllocation)
            {
                configString.Append(" \"allocation\" }");
            }
            else
            {
                configString.Append(" \"selection\" }");
            }
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Setting Order and Seating Configuration for Doshii. SeatingMode = {0}, OrderMode = {1}", seatingMode.ToString(), orderMode.ToString()));
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.SetSeatingAndOrderConfiguration), "PUT", configString.ToString());
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
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
        /// Rejects a table allocation doshii has sent for approval. 
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="tableName"></param>
        /// <param name="tableAllocation"></param>
        /// <returns></returns>
        internal virtual bool RejectTableAllocation(string consumerId, string tableName, Enums.TableAllocationRejectionReasons rejectionReason)
        {
            
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            string reasonCodeString = SerializeTableDeAllocationRejectionReason(rejectionReason);
            
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.ConfirmTableAllocation, consumerId, tableName), "DELETE", reasonCodeString);
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
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: there was an exception rejecting a table allocation with doshii", rex));
            }
           
            return success;
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// completes the Put or Post request to update an order with Doshii. 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        internal Models.Order PutPostOrder(Models.Order order, string method)
        {
            if (!(method.Equals("POST") || method.Equals("PUT")))
            {
                throw new NotSupportedException("Method Not Supported");
            }
            string orderIdentifier;
            Models.Order returnOrder = new Models.Order();
            DoshiHttpResponceMessages responseMessage;
            Models.OrderToPut orderToPut = new Models.OrderToPut();
            if (method.Equals("POST"))
            {
                orderIdentifier = order.CheckinId.ToString();
                orderToPut.UpdatedAt = DateTime.Now.ToString();
            }
            else
            {
                orderIdentifier = order.Id.ToString();
                orderToPut.UpdatedAt = m_DoshiiLogic.m_DoshiiInterface.GetOrderUpdatedAtTime(order);
            }
            
            if (order.Status == "accepted")
            {
                orderToPut.Status = "accepted";
            }
            else if (order.Status == "paid")
            {
                orderToPut.Status = "paid";
            }
            else if (order.Status == "waiting for payment")
            {
                orderToPut.Status = "waiting for payment";
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
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Order, orderIdentifier), method, orderToPut.ToJsonStringForOrder());

            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: The Response message has been returned to the put order function"));

            if (responseMessage != null)
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was not null"));

                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: The Response message was OK"));
                    if (!string.IsNullOrWhiteSpace(responseMessage.Data))
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: The Response order data was not null"));
                        returnOrder = JsonConvert.DeserializeObject<Models.Order>(responseMessage.Data);
                        m_DoshiiLogic.m_DoshiiInterface.RecordOrderUpdatedAtTime(returnOrder);
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Order, order.Id.ToString())));
                    }

                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order, order.Id.ToString())));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Order, order.Id.ToString())));
                throw new Exceptions.NullOrderReturnedException();
            }
            return returnOrder;
        }
        
        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// This method is used to confirm or reject or update an order when the order has an OrderId
        /// </summary>
        /// <param name="order"></param>
        /// <returns>
        /// If the request is not successful a new order will be returned - you can check the order.Id in the returned order to confirm it is a valid response. 
        /// </returns>
        internal virtual Models.Order PutOrder(Models.Order order)
        {
            return PutPostOrder(order, "PUT");
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// This method is used to create an order when no order has been previously created on doshii for the checkinId
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        internal virtual Models.Order PostOrder(Models.Order order)
        {
            return PutPostOrder(order, "POST");
        }

        #endregion
        
        #region product sync methods
        
        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Gets all the products currently uploaded to Doshii. 
        /// </summary>
        /// <returns></returns>
        internal virtual List<Models.Product> GetDoshiiProducts()
        {
            DoshiHttpResponceMessages responseMessage;
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Products), "GET");
            
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            List<Models.Product> productList = new List<Models.Product>();
            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (responseMessage.Data != null)
                    {
                        productList = JsonConvert.DeserializeObject<List<Models.Product>>(responseMessage.Data);
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful response but there was not data contained in the response", GenerateUrl(Enums.EndPointPurposes.Products)));
                    }

                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Products)));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'", GenerateUrl(Enums.EndPointPurposes.Products)));
            }
            
            return productList;
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Deletes product data from doshii, 
        /// if the productId is empty all the products will be delete from Doshii else only the products with the provided Id will be deleted 
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>
        /// This will return true unless there was an exception, 
        /// as the doshii web service will return an error response if we attempt to delete an item that doesn't exist, we should ignore this error. 
        /// </returns>
        internal virtual bool DeleteProductData(string productId = "")
        {
            bool success = true;
            try
            {
                DoshiHttpResponceMessages responseMessage;
                try
                {
                    responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Products, productId), "DELETE");
                
                }
                catch (Exceptions.RestfulApiErrorResponseException rex)
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
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            catch (WebException ex)
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: There was a web exception when attempting to delete products from doshii", ex);
                success = false;
            }
            catch (Exception ex)
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: There was a web exception when attempting to delete products from doshii", ex);
                success = false;
            }

            return success;
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Posts a product to doshii to add to the menu. 
        /// </summary>
        /// <param name="productToPost"></param>
        /// <param name="isNewProduct"></param>
        /// <returns></returns>
        internal virtual bool PostProductData(Models.Product productToPost, bool isNewProduct)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            if (isNewProduct)
            {
                try
                {
                    responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Products, ""), "POST", productToPost.ToJsonStringForProductSync());
                }
                catch (Exceptions.RestfulApiErrorResponseException rex)
                {
                    throw rex;
                }
                
            }
            else
            {
                try
                {
                    responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Products, productToPost.PosId), "PUT", productToPost.ToJsonStringForProductSync());
                }
                catch (Exceptions.RestfulApiErrorResponseException rex)
                {
                    throw rex;
                }
                
            }
            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    success = true;
                }
                else if (responseMessage.Status == HttpStatusCode.InternalServerError)
                {
                    success = PutProductData(productToPost);
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
        /// Post a product list to doshii to add to the menu.
        /// </summary>
        /// <param name="productListToPost"></param>
        /// <param name="clearCurrentMenu"></param>
        /// <returns></returns>
        internal virtual bool PostProductData(List<Models.Product> productListToPost, bool clearCurrentMenu)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            if (clearCurrentMenu)
            {
                DeleteProductData("");
            }

            string productListJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(productListToPost);
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Products), "POST", productListJsonString);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.Created)
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
        /// Puts product data to doshii - this method should only be used to update products already existing on doshii
        /// </summary>
        /// <param name="productToPost"></param>
        /// <returns></returns>
        internal virtual bool PutProductData(Models.Product productToPost)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            
            StringBuilder productListJsonString = new StringBuilder();
            productListJsonString.Append(productToPost.ToJsonStringForProductSync());
            try
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Products, productToPost.PosId), "PUT", productListJsonString.ToString());
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
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
        internal virtual string GenerateUrl(Enums.EndPointPurposes purpose, string identification = "", string tableName = "")
        {
            StringBuilder newUrlbuilder = new StringBuilder();
            if (string.IsNullOrWhiteSpace(m_DoshiiUrlBase))
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: The DoshiiHttpCommunication class was not initialized correctly, the base URl is null or white space");
                return newUrlbuilder.ToString();
            }
            newUrlbuilder.AppendFormat("{0}", m_DoshiiUrlBase);
            switch (purpose)
            {
                case Enums.EndPointPurposes.Products:
                    newUrlbuilder.Append("/products");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case Enums.EndPointPurposes.Order:
                    newUrlbuilder.Append("/orders");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case Enums.EndPointPurposes.GetTableAllocations:
                    newUrlbuilder.Append("/tables");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case Enums.EndPointPurposes.ConfirmTableAllocation:
                    newUrlbuilder.AppendFormat("/consumers/{0}/table/{1}", identification, tableName);
                    break;
                case Enums.EndPointPurposes.Consumer:
                    newUrlbuilder.AppendFormat("/consumers/{0}", identification);
                    break;
                case Enums.EndPointPurposes.DeleteAllocationWithCheckInId:
                    newUrlbuilder.AppendFormat("/tables?checkin={0}", identification);
                    break;
                case Enums.EndPointPurposes.AddTableAllocation:
                    newUrlbuilder.AppendFormat("/consumers/{0}/table", identification);
                    break;
                case Enums.EndPointPurposes.SetSeatingAndOrderConfiguration:
                    newUrlbuilder.AppendFormat("/config");
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
        internal virtual DoshiHttpResponceMessages MakeRequest(string url, string method, string data = "")
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("MakeRequest was called without a URL"));
                 throw new NotSupportedException("request with blank URL");
            }
            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Headers.Add("authorization", m_Token);
            if (method.Equals("GET") || method.Equals("POST") || method.Equals("DELETE") || method.Equals("PUT"))
            {
                request.Method = method;
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("MakeRequest was called with a non supported HTTP request method type - '{0}", method));
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

            DoshiHttpResponceMessages responceMessage = new DoshiHttpResponceMessages();
            try
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: generating {0} request to endpoint {1}, with data {2}", method, url, data));
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                responceMessage.Status = response.StatusCode;
                responceMessage.StatusDescription = responceMessage.StatusDescription;

                StreamReader sr = new StreamReader(response.GetResponseStream());
                responceMessage.Data = sr.ReadToEnd();

                sr.Close();
                response.Close();

                if (responceMessage.Status == HttpStatusCode.OK || responceMessage.Status == HttpStatusCode.Created)
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Successful response from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                }
                else if (responceMessage.Status == HttpStatusCode.BadRequest || 
                    responceMessage.Status == HttpStatusCode.Unauthorized || 
                    responceMessage.Status == HttpStatusCode.Forbidden || 
                    responceMessage.Status == HttpStatusCode.InternalServerError || 
                    responceMessage.Status == HttpStatusCode.NotFound || 
                    responceMessage.Status == HttpStatusCode.Conflict)
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: Failed response from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                    throw new Exceptions.RestfulApiErrorResponseException(responceMessage.Status);
                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: Failed response from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
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
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: A  WebException was thrown while attempting a {0} request to endpoint {1}, with data {2}, error Response {3}, exception {4}", method, url, data, errorResponce, wex));
                        throw new Exceptions.RestfulApiErrorResponseException(httpResponse.StatusCode);
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: A  WebException was thrown while attempting a {0} request to endpoint {1}, with data {2}, error Response {3}, exception {4}", method, url, data, errorResponce, wex));
                    }
                }
            }
            catch (Exception ex)
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: As exception was thrown while attempting a {0} request to endpoint {1}, with data {2} : {4}", method, url, data, responceMessage.Status.ToString(), ex));
            }
            return responceMessage;
        }
        #endregion

    }
}
