using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// This class represents the pos menu as it is stored on Doshii
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public Menu()
        {
            _products = new List<Product>();
            _surcounts = new List<Surcount>();
            Clear();
        }

        /// <summary>
        /// Resets all property values to default settings.
        /// </summary>
        public void Clear()
        {
            _surcounts.Clear();
            _products.Clear();
        }
        
        private List<Surcount> _surcounts;

        /// <summary>
        /// a list of all the order level surcounts on the pos menu
        /// </summary>
        public IEnumerable<Surcount> Surcounts
        {
            get
            {
                if (_surcounts == null)
                {
                    _surcounts = new List<Surcount>();
                }
                return _surcounts;
            }
            set { _surcounts = value.ToList<Surcount>(); }
        }

        
        private List<Product> _products;

        /// <summary>
        /// a list of all the products on the pos menu
        /// </summary>
        public IEnumerable<Product> Products
        {
            get
            {
                if (_products == null)
                {
                    _products = new List<Product>();
                }
                return _products;
            }
            set { _products = value.ToList<Product>(); }
        } 
    }
}
