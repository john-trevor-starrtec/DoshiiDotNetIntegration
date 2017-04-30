using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// represents an app that a member is registered with. 
    /// </summary>
    public class App
    {
        /// <summary>
        /// the Id of the partner app
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// the name of the partner app
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the amount of points available for the member on this app
        /// </summary>
        public decimal Points { get; set; }

        public decimal Ref { get; set; }


        protected bool Equals(App other)
        {
            return string.Equals(Id, other.Id) && string.Equals(Name, other.Name) && Ref == other.Ref;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((App) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Ref.GetHashCode();
                return hashCode;
            }
        }
    }
}
