using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Interfaces
{
    /// <summary>
    /// This interface enables the pos to control the menu products available through Doshii 
    /// </summary>
    public interface iDoshiiProducts
    {
        /// <summary>
        /// This method should be used to add new products to Doshii
        /// When implementing this method doshiiLodic.AddNewProducts MUST be called
        /// The above call must be in a try catch and you must catch a Exceptions.ProductNotCreatedException exception
        /// If caught it indicated that at least one of the products on the list of products provided already exists within Doshii and updateProduct should be called.
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        void AddNewProducts(DoshiiManagement doshiiLogic, List<Models.Product> productList);

        /// <summary>
        /// This method should be used to update a product that already exists on doshii
        /// When implementing this method doshiiLodic.UpdateProcuct MUST be called
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <param name="productToUpdate"></param>
        /// <returns></returns>
        void UpdateProduct(DoshiiManagement doshiiLogic, Models.Product productToUpdate);

        /// <summary>
        /// This method should be used to delete products from Doshii
        /// When implementing this method doshiiLodic.DeleteProducts MUST be called
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <param name="productIdList"></param>
        /// <returns></returns>
        void DeleteProducts(DoshiiManagement doshiiLogic, List<string> productIdList);

        /// <summary>
        /// This method should be used to delete all products from doshii
        /// When implementing this method doshiiLodic.DeleteAllProducts MUST be called
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <returns></returns>
        void DeleteAllProducts(DoshiiManagement doshiiLogic);

        /// <summary>
        /// This method should be used to retrieve all products from Doshii
        /// When implementing this method doshiiLodic.GetAllProducts MUST be returned
        /// </summary>
        /// <returns></returns>
        List<Models.Product> GetAllProducts(DoshiiManagement doshiiLogic);
    }
}
