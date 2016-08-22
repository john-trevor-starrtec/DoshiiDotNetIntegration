using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Interfaces
{
    /// <summary>
    /// Implementations of this interface is required to handle membership functionality in Doshii.
    /// <para/>The POS should implement this interface to enable member sales and rewards tracking.
    /// <para/>Version control on members is also managed through the POS implementation of this interface.
    /// </summary>
    /// <remarks>
    /// <para/><see cref="DoshiiDotNetIntegration.DoshiiManager"/> uses this interface as a callback mechanism 
    /// to the POS for membership functions. 
    /// <para>
    /// </para>
    /// </remarks>
    public interface IMembershipModuleManager
    {
        DoshiiDotNetIntegration.Models.Member RetrieveMember(string DoshiiMemberId);
        
        /// <summary>
        /// The <see cref="DoshiiDotNetIntegration.DoshiiManager"/> uses this call to inform the pos
        /// that a member has been updated. The <paramref name="version"/> string must be persisted in
        /// the POS and passed back when the POS updates a member. 
        /// </summary>
        /// <remarks>
        /// The current <paramref name="version"/> is used by Doshii for conflict resolution, but the POS is 
        /// the final arbiter on the state of an member.
        /// </remarks>
        /// <param name="DoshiiMemberId">The unique identifier of the order being updated in the POS.</param>
        /// <param name="version">The current version of the member in Doshii.</param>
        /// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistOnPosException">This exception 
        /// should be thrown when there is no member in the POS with the corresponding 
        /// <paramref name="DoshiiMemberId"/>.</exception>
        void RecordMemberVersion(string DoshiiMemberId, string version);

        /// <summary>
        /// The <see cref="DoshiiDotNetIntegration.DoshiiManager"/> uses this call to request the current
        /// version of a member in the POS.
        /// </summary>
        /// <param name="DoshiiMemberId">The unique identifier of the member being queried on the POS.</param>
        /// <returns>The current version of the member in the POS.</returns>
        /// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistOnPosException">This exception 
        /// should be thrown when there is no order in the POS with the corresponding 
        /// <paramref name="posOrderId"/>.</exception>
        string RetrieveMemberVersion(string DoshiiMemberId);


        bool CreateMember(Member newMember);

        bool UpdateMember(Member updatedMember);


    }
}
