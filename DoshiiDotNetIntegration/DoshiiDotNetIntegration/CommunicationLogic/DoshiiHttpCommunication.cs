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
        private string DoshiiUrlBase;

        private Doshii DoshiiLogic;

        private string Token;

        internal DoshiiHttpCommunication(string urlBase, Doshii doshiiLogic, string token)
        {
            DoshiiUrlBase = urlBase;
            Token = token;
            DoshiiLogic = doshiiLogic;
        }

        #region internal methods 

        #region checkin and allocate methods

        /// <summary>
        /// the Id is the paypal consumer id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal Models.Consumer GetConsumer(string id)
        {
            Models.Consumer retreivedConsumer = new Models.Consumer();
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.GetConsumer, id), "GET");

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
                        DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful responce but there was not data contained in the responce", GenerateUrl(Enums.EndPointPurposes.GetConsumer, id)));
                    }
                    
                }
                else
                {
                    DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.GetConsumer, id)));
                }
            }
            else
            {
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and url '{0}'", GenerateUrl(Enums.EndPointPurposes.GetConsumer, id)));
            }

            return retreivedConsumer;
        }

        internal List<Models.Consumer> GetConsumers()
        {
            List<Models.Consumer> retreivedConsumerList = new List<Models.Consumer>();
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.GetConsumer), "GET");

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
                        DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful responce but there was not data contained in the responce", GenerateUrl(Enums.EndPointPurposes.GetConsumer)));
                    }
                }
                else
                {
                    DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.GetConsumer)));
                }
            }
            else
            {
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and url '{0}'", GenerateUrl(Enums.EndPointPurposes.GetConsumer)));
            }
            
            return retreivedConsumerList;
        }
        #endregion

        #region order methods

        /// <summary>
        /// this method is used to retreive the order from doshii, prymiarly used after doshii has sent an new order or an order changed notification. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal Models.Order GetOrder(string id)
        {
            Models.Order retreivedOrder = new Models.Order();
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.GetOrder, id), "GET");

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
                        DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful responce but there was not data contained in the responce", GenerateUrl(Enums.EndPointPurposes.GetOrder, id)));
                    }

                }
                else
                {
                    DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.GetOrder, id)));
                }
            }
            else
            {
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and url '{0}'", GenerateUrl(Enums.EndPointPurposes.GetOrder, id)));
            }

            return retreivedOrder;
        }

        internal List<Models.Order> GetOrders()
        {
            List<Models.Order> retreivedOrderList = new List<Models.Order>();
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.GetOrder), "GET");

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
                        DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful responce but there was not data contained in the responce", GenerateUrl(Enums.EndPointPurposes.GetOrder)));
                    }

                }
                else
                {
                    DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.GetOrder)));
                }
            }
            else
            {
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and url '{0}'", GenerateUrl(Enums.EndPointPurposes.GetOrder)));
            }

            return retreivedOrderList;
        }

        internal List<Models.TableAllocation> GetTableAllocations()
        {
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.GetTableAllocations), "GET");

            List<Models.TableAllocation> tableAllocationList = JsonConvert.DeserializeObject<List<Models.TableAllocation>>(responseMessage.Data);
            return tableAllocationList;

        }

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

        internal bool RejectTableAllocation(string consumerId, string tableName, DoshiiDotNetIntegration.Models.TableAllocation allocation)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.ConfirmTableAllocation, consumerId, tableName), "DELETE");

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
        /// this method is used to confirm or reject an order placed by doshii
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        internal bool PutOrder(Models.Order order)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            Models.OrderToPut orderToPut = new Models.OrderToPut();
            orderToPut.UpdatedAt = DateTime.Now;
            
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

            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.GetOrder, order.Id.ToString()), "PUT", orderToPut.ToJsonStringForOrder());

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


        #region product sync methods
        
        /// <summary>
        /// gets all the products currently uploaded to doshii. 
        /// </summary>
        /// <returns></returns>
        internal List<Models.Product> GetDoshiiProducts()
        {
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.GetAllProducts), "GET");
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
                        DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} returned a successful responce but there was not data contained in the responce", GenerateUrl(Enums.EndPointPurposes.GetAllProducts)));
                    }

                }
                else
                {
                    DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: A 'GET' request to {0} was not successful", GenerateUrl(Enums.EndPointPurposes.GetAllProducts)));
                }
            }
            else
            {
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and url '{0}'", GenerateUrl(Enums.EndPointPurposes.GetAllProducts)));
            }
            
            return productList;
        }

        /// <summary>
        /// deletes product data from doshii, if the productId is empty all the products will be delete from doshi else only the products that are with the provided Id will be deleted 
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>
        /// This will return true unless there was an exception, as the doshii web service will return an error responce if we attempt to delete an item that doesn't exist, we should ignore this error. 
        /// REVIEW: (liam) we should prob return false if the responce code is not 200 or 404 this will prevent us from continuing if the error was not because the product does not exits. 
        /// </returns>
        internal bool DeleteProductData(string productId = "")
        {
            bool success = true;
            try
            {
                DoshiHttpResponceMessages responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.UploadProducts, productId),"DELETE");
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
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Error, "Doshii: There was a web exception when attempting to delete products from doshii", ex);
                success = false;
            }
            catch (Exception ex)
            {
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Error, "Doshii: There was a web exception when attempting to delete products from doshii", ex);
                success = false;
            }

            return success;
        }

        internal bool PostProductData(Models.Product productToPost, bool isNewProduct)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            if (isNewProduct)
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.UploadProducts, ""), "POST", productToPost.ToJsonStringForProductSync());
            }
            else
            {
                responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.UploadProducts, productToPost.PosId), "PUT", productToPost.ToJsonStringForProductSync());
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
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.UploadProducts), "POST", productListJsonString.ToString());
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

        internal bool PutProductData(Models.Product productToPost)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            
            StringBuilder productListJsonString = new StringBuilder();
            productListJsonString.Append(productToPost.ToJsonStringForProductSync());
            
            responseMessage = MakeRequest(GenerateUrl(Enums.EndPointPurposes.UploadProducts, productToPost.PosId), "PUT", productListJsonString.ToString());
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
            if (string.IsNullOrWhiteSpace(DoshiiUrlBase))
            {
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Error, "Doshii: The DoshiiHttpCommunication class was not initialized correctly, the base URl is null or white space");
                return newUrlbuilder.ToString();
            }
            newUrlbuilder.AppendFormat("{0}", DoshiiUrlBase);
            switch (purpose)
            {
                case Enums.EndPointPurposes.UploadProducts:
                    newUrlbuilder.Append("/products");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case Enums.EndPointPurposes.GetAllProducts:
                    newUrlbuilder.Append("/products");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}", identification);
                    }
                    break;
                case Enums.EndPointPurposes.GetOrder:
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
                case Enums.EndPointPurposes.GetConsumer:
                    newUrlbuilder.AppendFormat("/consumers/{0}", identification);
                    break;
                default:
                    throw new NotSupportedException(purpose.ToString());
            }

            return newUrlbuilder.ToString();
        }

        private DoshiHttpResponceMessages MakeRequest(string url, string method, string data = "")
        {
            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Headers.Add("authorization", Token);
            if (method.Equals("GET") || method.Equals("POST") || method.Equals("DELETE") || method.Equals("PUT"))
            {
                // Set the Method property of the request to POST.
                request.Method = method;
            }
            else
            {
                throw new Exception("Invalid Method Type");
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
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: generating {0} request to endpoint {1}, with data {2}", method, url, data));
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
                    DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Successfull responce from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                }
                else
                {
                    DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: Failed responce from {0} request to endpoint {1}, with data {2} , responceCode - {3}, responceData - {4}", method, url, data, responceMessage.Status.ToString(), responceMessage.Data));
                }
            }
            catch (Exception ex)
            {
                DoshiiLogic.LogDoshiiError(Enums.DoshiiLogLevels.Error, string.Format("Doshii: As exception was thrown while attempting a {0} request to endpoint {1}, with data {2}", method, url, data, responceMessage.Status.ToString()), ex);
                
            }
            return responceMessage;
        }


        #endregion

    }
}
