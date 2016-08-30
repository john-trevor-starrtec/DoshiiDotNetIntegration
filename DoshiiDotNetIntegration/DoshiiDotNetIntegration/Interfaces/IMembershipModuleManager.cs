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
        
        bool CreateMember(Member newMember);

        bool UpdateMember(Member updatedMember);


    }
}
