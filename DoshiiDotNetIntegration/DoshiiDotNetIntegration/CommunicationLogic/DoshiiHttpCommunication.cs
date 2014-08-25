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

        internal DoshiiHttpCommunication(string urlBase)
        {
            DoshiiUrlBase = urlBase;
        }

        #region internal methods 

        #region checkin and allocate methods

        /// <summary>
        /// the Id is the paypal consumer id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal Modles.Consumer GetConsumer(string id)
        {
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(generateUrl(Enums.EndPointPurposes.GetConsumer, id), "GET");

            Modles.Consumer retreivedConsumer = Modles.Consumer.deseralizeFromJson(responseMessage.data);
            return retreivedConsumer;
        }
        #endregion

        #region order methods

        /// <summary>
        /// this method is used to retreive the order from doshii, prymiarly used after doshii has sent an new order or an order changed notification. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal Modles.order GetOrder(string id)
        {
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(generateUrl(Enums.EndPointPurposes.GetOrder, id), "GET");

            Modles.order retreivedOrder = Modles.order.deseralizeFromJson(responseMessage.data);
            return retreivedOrder;
        }

        internal List<Modles.order> GetOrders()
        {
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(generateUrl(Enums.EndPointPurposes.GetOrder), "GET");

            List<Modles.order> retreivedOrderList = JsonConvert.DeserializeObject<List<Modles.order>>(responseMessage.data);
            return retreivedOrderList;
        }

        internal List<Modles.table_allocation> GetTableAllocations()
        {
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(generateUrl(Enums.EndPointPurposes.GetTableAllocations), "GET");

            List<Modles.table_allocation> retreivedOrderList = JsonConvert.DeserializeObject<List<Modles.table_allocation>>(responseMessage.data);
            return retreivedOrderList;

        }

        internal bool PutTableAllocation(string consumerId, string tableName)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(generateUrl(Enums.EndPointPurposes.GetTableAllocations, consumerId, tableName), "PUT");

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

        internal bool RejectTableAllocation(string consumerId, string tableName, Modles.table_allocation tableAllocation)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(generateUrl(Enums.EndPointPurposes.GetTableAllocations, consumerId, tableName), "DELETE", tableAllocation.ToJsonString());

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
        /// this method is used to confirm or reject an order placed by doshii
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        internal bool PutOrder(Modles.order order)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(generateUrl(Enums.EndPointPurposes.GetOrder, order.id.ToString()), "PUT");

            if (responseMessage.Status == (HttpStatusCode)200)
            {
                success = true;
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
        internal List<Modles.product> GetDoshiiProducts()
        {
            DoshiHttpResponceMessages responseMessage;
            responseMessage = MakeRequest(generateUrl(Enums.EndPointPurposes.GetAllProducts), "GET");

            List<Modles.product> productList = JsonConvert.DeserializeObject<List<Modles.product>>(responseMessage.data);
            return productList;
        }

        /// <summary>
        /// gets all the products currently uploaded to doshii. 
        /// </summary>
        /// <returns></returns>
        internal string GetWebSocketsAddress(string url)
        {
            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Method = "GET";
            return GetResponse(request).data;
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
                DoshiHttpResponceMessages responseMessage = MakeRequest(generateUrl(Enums.EndPointPurposes.UploadProducts, productId),"DELETE");
                //if the responce is fail might need to do something but should be able to continue
            }
            catch (WebException ex)
            {
                Doshii.log.Error("There was a web exception when attempting to delete products from doshii", ex);
                success = false;
            }
            catch (Exception ex)
            {
                Doshii.log.Error("There was a web exception when attempting to delete products from doshii", ex);
                success = false;
            }

            return success;
        }

        internal bool PostProductData(Modles.product productToPost, bool isNewProduct)
        {
            bool success = false;
            DoshiHttpResponceMessages responseMessage;
            if (isNewProduct)
            {
                responseMessage = MakeRequest(generateUrl(Enums.EndPointPurposes.UploadProducts, ""), "POST", productToPost.ToJsonStringForProductSync());
            }
            else
            {
                responseMessage = MakeRequest(generateUrl(Enums.EndPointPurposes.UploadProducts, productToPost.pos_Id), "PUT", productToPost.ToJsonStringForProductSync());
            }
            if (responseMessage.Status == (HttpStatusCode)200)
            {
                success = true;
            }
            else
            {
                success = false;
            }
            return success;
        }

        internal bool PostProductData(List<Modles.product> productListToPost, bool clearCurrentMenu)
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
            foreach (Modles.product pro in productListToPost)
            {
                if (count > 0)
                {
                    productListJsonString.Append(",");
                }
                productListJsonString.Append(pro.ToJsonStringForProductSync());
                count++;
            }
            responseMessage = MakeRequest(generateUrl(Enums.EndPointPurposes.UploadProducts), "POST", productListJsonString.ToString());
            if (responseMessage.Status == (HttpStatusCode)200)
            {
                success = true;
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
        private string generateUrl(Enums.EndPointPurposes purpose, string identification = "", string tableName = "")
        {
            StringBuilder newUrlbuilder = new StringBuilder();
            if (string.IsNullOrWhiteSpace(DoshiiUrlBase))
            {
                Doshii.log.Error("the DoshiiHttpCommunication class was not initialized correctly, the base URl is null or white space");
                return newUrlbuilder.ToString();
            }
            newUrlbuilder.AppendFormat("{0}", DoshiiUrlBase);
            switch (purpose)
            {
                case Enums.EndPointPurposes.UploadProducts:
                    newUrlbuilder.Append("/products");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}");
                    }
                    break;
                case Enums.EndPointPurposes.GetAllProducts:
                    newUrlbuilder.Append("/products");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}");
                    }
                    break;
                case Enums.EndPointPurposes.GetOrder:
                    newUrlbuilder.Append("/orders");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}");
                    }
                    break;
                case Enums.EndPointPurposes.GetTableAllocations:
                    newUrlbuilder.Append("/tables");
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}");
                    }
                    break;
                case Enums.EndPointPurposes.ConfirmTableAllocation:
                    newUrlbuilder.AppendFormat("/consumers/{0}/tables/{1}",identification,tableName);
                    if (!string.IsNullOrWhiteSpace(identification))
                    {
                        newUrlbuilder.AppendFormat("/{0}");
                    }
                    break;
                default:
                    throw new NotSupportedException(purpose.ToString());
            }

            return newUrlbuilder.ToString();
        }

        private DoshiHttpResponceMessages MakeRequest(string url, string method)
        {
            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            if (method.Equals("GET") || method.Equals("POST") || method.Equals("DELETE") || method.Equals("PUT"))
            {
                // Set the Method property of the request to POST.
                request.Method = method;
            }
            else
            {
                throw new Exception("Invalid Method Type");
            }
            return GetResponse(request);
        }

        private DoshiHttpResponceMessages MakeRequest(string url, string method, string data)
        {
            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
                       
            // Set the ContentType property of the WebRequest.
            request.ContentType = "text/json";
            
            //set request data
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                if (data != null)
                {
                    writer.Write(data);
                }
                writer.Close();
            }
            return GetResponse(request);
        }

        private DoshiHttpResponceMessages GetResponse(WebRequest request)
        {
            DoshiHttpResponceMessages responceMessage = new DoshiHttpResponceMessages();
            
            // Get the original response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            responceMessage.Status = response.StatusCode;
            responceMessage.statusDescription = responceMessage.statusDescription;

            StreamReader sr = new StreamReader(response.GetResponseStream());
            responceMessage.data = sr.ReadToEnd();
            
            // Clean up the streams.
            sr.Close();
            response.Close();

            return responceMessage;
        }

        #endregion

    }
}
