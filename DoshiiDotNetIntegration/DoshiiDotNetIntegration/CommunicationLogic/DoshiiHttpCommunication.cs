using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    internal class DoshiiHttpCommunication 
    {
        private string m_DoshiiUrlBase;

        private DoshiiOperationLogic m_DoshiiLogic;

        private string m_Token;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="urlBase"></param>
        /// <param name="doshiiLogic"></param>
        /// <param name="token"></param>
        internal DoshiiHttpCommunication(string urlBase, DoshiiOperationLogic doshiiLogic, string token)
        {
            if (doshiiLogic == null)
            {
                throw new NotSupportedException("doshiiLogic");
            }

            m_DoshiiLogic = doshiiLogic;
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Instanciating DoshiiHttpCommunication Class with; urlBase - '{0}', token - '{1}'", urlBase, token));
            //REVIEW: (LIAM) this should use regex to test the url form
            if (string.IsNullOrWhiteSpace(urlBase))
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Instanciating DoshiiHttpCommunication Class with a blank urlBase - '{0}'", urlBase));
                throw new NotSupportedException("blank url");
            
            }
            if (string.IsNullOrWhiteSpace(token))
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Instanciating DoshiiHttpCommunication Class with a blank token - '{0}'", token));
                throw new NotSupportedException("blank token");
            }
            
            m_DoshiiUrlBase = urlBase;
            m_Token = token;
            
        }

        #region internal methods 

        #region checkin and allocate methods

        /// <summary>
        /// gets a consumer from doshii with the paypalCustomerId provided, if no consumer exists a new consumer object is returned. 
        /// </summary>
        /// <param name="customerPayPalId"></param>
        /// <returns></returns>
        internal Models.Consumer GetConsumer(string customerPayPalId)
        {
                
            Models.Consumer retreivedConsumer = new Models.Consumer();
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Consumer, customerPayPalId), "GET");

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (responseMessage.Data != null)
                    {
                        retreivedConsumer = Models.Consumer.deseralizeFromJson(responseMessage.Data);
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful responce but there was not data contained in the responce", GenerateUrl(Enums.EndPointPurposes.Consumer, customerPayPalId)));
                    }
                    
                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Consumer, customerPayPalId)));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and url '{0}'", GenerateUrl(Enums.EndPointPurposes.Consumer, customerPayPalId)));
            }

            return retreivedConsumer;
        }

        /// <summary>
        /// get all the customers currently checkedIn with doshii, if no customers are returned en empty list is returned. 
        /// </summary>
        /// <returns></returns>
        internal List<Models.Consumer> GetConsumers()
        {

            List<Models.Consumer> retreivedConsumerList = new List<Models.Consumer>();
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Consumer), "GET");

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (responseMessage.Data != null)
                    {
                        retreivedConsumerList = JsonConvert.DeserializeObject<List<Models.Consumer>>(responseMessage.Data);
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful responce but there was not data contained in the responce", GenerateUrl(Enums.EndPointPurposes.Consumer)));
                    }
                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Consumer)));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and url '{0}'", GenerateUrl(Enums.EndPointPurposes.Consumer)));
            }
            
            return retreivedConsumerList;
        }
        #endregion

        #region order methods

        /// <summary>
        /// this method is used to retreive the order from doshii matching the provided orderId, if no order matches the provied orderId a new order is returned. 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        internal Models.Order GetOrder(string orderId)
        {
            Models.Order retreivedOrder = new Models.Order();
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Order, orderId), "GET");

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (responseMessage.Data != null)
                    {
                        retreivedOrder = Models.Order.deseralizeFromJson(responseMessage.Data);
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful responce but there was not data contained in the responce", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                    }

                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and url '{0}'", GenerateUrl(Enums.EndPointPurposes.Order, orderId)));
            }

            return retreivedOrder;
        }

        /// <summary>
        /// gets all the current active orders in doshii, if there are no active orders an empty list is returned. 
        /// </summary>
        /// <returns></returns>
        internal List<Models.Order> GetOrders()
        {
            List<Models.Order> retreivedOrderList = new List<Models.Order>();
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Order), "GET");

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (responseMessage.Data != null)
                    {
                        retreivedOrderList = JsonConvert.DeserializeObject<List<Models.Order>>(responseMessage.Data);
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful responce but there was not data contained in the responce", GenerateUrl(Enums.EndPointPurposes.Order)));
                    }

                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order)));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and url '{0}'", GenerateUrl(Enums.EndPointPurposes.Order)));
            }

            return retreivedOrderList;
        }

        /// <summary>
        /// gets all the current active table allocations in doshii, if there are no current active table allocations an enpty list is returned. 
        /// </summary>
        /// <returns></returns>
        internal List<Models.TableAllocation> GetTableAllocations()
        {
            List<Models.TableAllocation> tableAllocationList = new List<Models.TableAllocation>();
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.GetTableAllocations), "GET");

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
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: A 'GET' request to {0} returned a successful responce but there was not data contained in the responce", GenerateUrl(Enums.EndPointPurposes.GetTableAllocations)));
                    }

                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.GetTableAllocations)));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and url '{0}'", GenerateUrl(Enums.EndPointPurposes.GetTableAllocations)));
            }

            return tableAllocationList;

        }

        /// <summary>
        /// attempts to put a table allocaiton to doshii, if successful returns true, else returns false. 
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal bool PutTableAllocation(string consumerId, string tableName)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.ConfirmTableAllocation, consumerId, tableName), "PUT", "{\"status\": \"confirmed\"}");

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
        /// attempts to post a table allocaiton to doshii, if successful returns true, else returns false. 
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal bool PostTableAllocation(string consumerId, string tableName)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            StringBuilder builder = new StringBuilder();
            builder.Append("{\"tableName\": \"");
            builder.AppendFormat("{0}", tableName);
            builder.Append("\"}");
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.AddTableAllocation, consumerId), "POST", builder.ToString());

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
        /// removes a table allocation from doshii
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="tableName"></param>
        /// <param name="rejectionReason"></param>
        /// <returns>
        /// true = successfully removed
        /// false = not removed. 
        /// </returns>
        internal bool RemoveTableAllocation(string consumerId, string tableName, string rejectionReason)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.GetTableAllocations, consumerId, tableName), "DELETE", rejectionReason);

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
        /// rejects a table allocation doshii has sent for approval. 
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="tableName"></param>
        /// <param name="tableAllocation"></param>
        /// <returns></returns>
        internal bool DeleteTableAllocationWithCheckInId(string checkInId)
        {

            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.DeleteAllocationWithCheckInId, checkInId), "DELETE");

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
        /// rejects a table allocation doshii has sent for approval. 
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="tableName"></param>
        /// <param name="tableAllocation"></param>
        /// <returns></returns>
        internal bool SetSeatingAndOrderConfiguration(Enums.SeatingModes seatingMode, Enums.OrderModes orderMode)
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
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Setting Order and Seating Configuration for Dohsii. SeatingMode = {0}, OrderMode = {1}", seatingMode.ToString(), orderMode.ToString()));
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.SetSeatingAndOrderConfiguration), "PUT", configString.ToString());

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
        /// rejects a table allocation doshii has sent for approval. 
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="tableName"></param>
        /// <param name="tableAllocation"></param>
        /// <returns></returns>
        internal bool RejectTableAllocation(string consumerId, string tableName, Enums.TableAllocationRejectionReasons rejectionReason)
        {
            
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
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
            }
            
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

            return success;
        }

        /// <summary>
        /// this method is used to confirm or reject or update an order when the order has an Id order placed by doshii
        /// </summary>
        /// <param name="order"></param>
        /// <returns>
        /// if the request is not successfull a new order will be returned - you can check the order.Id in the returned order to confirm it is a valid responce. 
        /// </returns>
        internal Models.Order PutOrder(Models.Order order)
        {
            Models.Order returnOrder = new Models.Order();
            DoshiHttpResponceMessages responseMessage;
            Models.OrderToPut orderToPut = new Models.OrderToPut();
            orderToPut.UpdatedAt = m_DoshiiLogic.m_DoshiiInterface.GetOrderUpdatedAtTime(order); 
            
            if (order.Status == "accepted")
            {
                orderToPut.Status = "accepted";
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
            //REVIEW(Surcounts)
            orderToPut.Surcounts = order.Surcounts;

            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Order, order.Id.ToString()), "PUT", orderToPut.ToJsonStringForOrder());
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: The Responce message has been returned to the put order function"));
                    
            if (responseMessage != null)
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: The Responce message was not null"));
            
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: The Responce message was OK"));
                    if (responseMessage.Data != null)
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: The Responce order data was not null"));
                        returnOrder = JsonConvert.DeserializeObject<Models.Order>(responseMessage.Data);
                        m_DoshiiLogic.m_DoshiiInterface.RecordOrderUpdatedAtTime(returnOrder); 
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} returned a successful responce but there was not data contained in the responce", GenerateUrl(Enums.EndPointPurposes.Order, order.Id.ToString())));
                    }

                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'PUT' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order, order.Id.ToString())));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'PUT' and url '{0}'", GenerateUrl(Enums.EndPointPurposes.Order, order.Id.ToString())));
            }
            m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: the order is now bein returned - end of putorder method"));
            return returnOrder;
        }

        /// <summary>
        /// This method is used to create an order when no order has been previously created createed on doshii for the checkinId
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        internal Models.Order PostOrder(Models.Order order)
        {
            Models.Order returnOrder = new Models.Order();
            DoshiHttpResponceMessages responseMessage;
            Models.OrderToPut orderToPut = new Models.OrderToPut();
            orderToPut.UpdatedAt = DateTime.Now.ToString();

            if (order.Status == "accepted")
            {
                orderToPut.Status = "accepted";
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
            //REVIEW(Surcounts)
            orderToPut.Surcounts = order.Surcounts;

            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Order, order.CheckinId.ToString()), "POST", orderToPut.ToJsonStringForOrder());

            if (responseMessage != null)
            {
                if (responseMessage.Status == HttpStatusCode.OK)
                {
                    if (responseMessage.Data != null)
                    {
                        returnOrder = JsonConvert.DeserializeObject<Models.Order>(responseMessage.Data);
                        m_DoshiiLogic.m_DoshiiInterface.RecordOrderUpdatedAtTime(returnOrder); 
                    }
                    else
                    {
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} returned a successful responce but there was not data contained in the responce", GenerateUrl(Enums.EndPointPurposes.Order, order.CheckinId.ToString())));
                    }
                    
                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'POST' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Order, order.CheckinId.ToString())));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'POST' and url '{0}'", GenerateUrl(Enums.EndPointPurposes.Order, order.CheckinId.ToString())));
            }
            return returnOrder;
        }

        #endregion


        #region product sync methods
        
        /// <summary>
        /// gets all the products currently uploaded to doshii. 
        /// </summary>
        /// <returns></returns>
        internal List<Models.Product> GetDoshiiProducts()
        {
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Products), "GET");
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
                        m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful responce but there was not data contained in the responce", GenerateUrl(Enums.EndPointPurposes.Products)));
                    }

                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.Products)));
                }
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and url '{0}'", GenerateUrl(Enums.EndPointPurposes.Products)));
            }
            
            return productList;
        }

        /// <summary>
        /// deletes product data from doshii, if the productId is empty all the products will be delete from doshi else only the products that are with the provided Id will be deleted 
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>
        /// This will return true unless there was an exception, as the doshii web service will return an error responce if we attempt to delete an item that doesn't exist, we should ignore this error. 
        /// </returns>
        internal bool DeleteProductData(string productId = "")
        {
            bool success = true;
            try
            {
                DoshiHttpResponceMessages responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Products, productId),"DELETE");
                //if the responce is fail might need to do something but should be able to continue
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
        /// posts a product to doshii to add to the menu. 
        /// </summary>
        /// <param name="productToPost"></param>
        /// <param name="isNewProduct"></param>
        /// <returns></returns>
        internal bool PostProductData(Models.Product productToPost, bool isNewProduct)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            if (isNewProduct)
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Products, ""), "POST", productToPost.ToJsonStringForProductSync());
            }
            else
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Products, productToPost.PosId), "PUT", productToPost.ToJsonStringForProductSync());
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
        /// post a product list to doshii to add to the menu.
        /// </summary>
        /// <param name="productListToPost"></param>
        /// <param name="clearCurrentMenu"></param>
        /// <returns></returns>
        internal bool PostProductData(List<Models.Product> productListToPost, bool clearCurrentMenu)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            if (clearCurrentMenu)
            {
                DeleteProductData("");
            }

            StringBuilder productListJsonString = new StringBuilder();
            productListJsonString.Append("[");
            int count = 0;
            foreach (Models.Product pro in productListToPost)
            {
                if (count > 0)
                {
                    productListJsonString.Append(",");
                }
                productListJsonString.Append(pro.ToJsonStringForProductSync());
                count++;
            }
            productListJsonString.Append("]");
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Products), "POST", productListJsonString.ToString());
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
        /// puts product data to doshii - this method should only be used to update products already existing on doshii
        /// </summary>
        /// <param name="productToPost"></param>
        /// <returns></returns>
        internal bool PutProductData(Models.Product productToPost)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            
            StringBuilder productListJsonString = new StringBuilder();
            productListJsonString.Append(productToPost.ToJsonStringForProductSync());
            
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.Products, productToPost.PosId), "PUT", productListJsonString.ToString());
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
        /// generates a url based on the base url and the purpose of the message that is being sent. 
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="identification"></param>
        /// <returns></returns>
        private string GenerateUrl(Enums.EndPointPurposes purpose, string identification = "", string tableName = "")
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
                    newUrlbuilder.AppendFormat("/consumers/{0}/table", identification, tableName);
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
        /// makes a request to doshii based on the paramaters provided. 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private DoshiHttpResponceMessages MakeRequest(string url, string method, string data = "")
        {
            //REVIEW: (LIAM) this should use regex to test if it is a correclty formed url
            if (string.IsNullOrWhiteSpace(url))
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("MakeRequest was called without a url"));
                 throw new NotSupportedException("request with blank url");
            }
            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Headers.Add("authorization", m_Token);
            if (method.Equals("GET") || method.Equals("POST") || method.Equals("DELETE") || method.Equals("PUT"))
            {
                // Set the Method property of the request to POST.
                request.Method = method;
            }
            else
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("MakeRequest was called with a non suppoerted Http request method type - '{0}", method));
                throw new Exception("Invalid Http request Method Type");
            }
            if (!string.IsNullOrWhiteSpace(data))
            {
                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/json";

                //set request data
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(data);
                    writer.Close();
                }
            }

            DoshiHttpResponceMessages responceMessage = new DoshiHttpResponceMessages();
            try
            {
                // Get the original response.
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: generating {0} request to endpoint {1}, with data {2}", method, url, data));
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                responceMessage.Status = response.StatusCode;
                responceMessage.StatusDescription = responceMessage.StatusDescription;

                StreamReader sr = new StreamReader(response.GetResponseStream());
                responceMessage.Data = sr.ReadToEnd();

                // Clean up the streams.
                sr.Close();
                response.Close();

                if (responceMessage.Status == HttpStatusCode.OK || responceMessage.Status == HttpStatusCode.Created)
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Successfull responce from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                }
                else
                {
                    m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: Failed responce from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                }
                
            }
            catch (Exception ex)
            {
                m_DoshiiLogic.m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: As exception was thrown while attempting a {0} request to endpoint {1}, with data {2} : {4}", method, url, data, responceMessage.Status.ToString(), ex), ex);
                
            }
            return responceMessage;
        }


        #endregion

    }
}
