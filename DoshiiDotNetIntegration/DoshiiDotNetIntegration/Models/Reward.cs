using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models.Json;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// a Doshii reward
    /// </summary>
    public class Reward 
    {
        /// <summary>
        /// the Id of the reward
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// the name of the reward
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the description of the reward
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// the Type of the reward 'absolute' or 'percentage'
        /// </summary>
        public string SurcountType { get; set; }

        /// <summary>
        /// the amount the reward is worth, 
        /// if the <see cref="SurcountType"/> = 'absolute' the SurcountAmount is the value of the reward. 
        /// if the <see cref="SurcountType"/> = 'percentage' the SurcountAmount is the percentage value of the reward.  
        /// </summary>
        public decimal SurcountAmount { get; set; }
        
        /// <summary>
        /// the name of the <see cref="App"/> associated with the reward. 
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// the last time the reward was updated. 
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// the time the reward was created. 
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// the URI to retreive details about the reward from Doshii. 
        /// </summary>
        public Uri Uri { get; set; }
        
    }
}
