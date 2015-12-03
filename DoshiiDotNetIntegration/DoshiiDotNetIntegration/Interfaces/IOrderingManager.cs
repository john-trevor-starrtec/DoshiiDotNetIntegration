using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Interfaces
{
	/// <summary>
	/// Implementations of this interface are required to handle orders in Doshii.
	/// The POS should implement this interface to accept new orders and updates to orders from Doshii.
	/// Version control on orders is also managed through the POS implementation of this interface.
	/// </summary>
	/// <remarks>
	/// This interface is a core Doshii interface required for implementation on the POS side. 
	/// <see cref="DoshiiDotNetIntegration.DoshiiManager"/> uses this interface as a callback mechanism 
	/// to the POS for basic order functions. It should be noted however that this does interface is not
	/// the handler for extension modules such as Order@Table which will be implemented in a separate
	/// callback interface.
	/// <para>
	/// When a partner wishes to perform an action on an order in Doshii, the SDK emits a call to 
	/// <see cref="DoshiiDotNetIntegration.Interfaces.IOrderingManager.RetrieveOrder(string)"/> to
	/// retrieve the current state of the order in the POS. After a partner has mutated the order in Doshii
	/// in some way, the SDK emits a call to 
	/// <see cref="DoshiiDotNetIntegration.Interfaces.IOrderingManager.RecordOrderVersion(string, string)"/>
	/// to inform the POS of the update success. 
	/// </para>
	/// </remarks>
	public interface IOrderingManager
	{
		/// <summary>
		/// The <see cref="DoshiiDotNetIntegration.DoshiiManager"/> uses this call to request the current
		/// details of an order from the POS.
		/// </summary>
		/// <remarks>
		/// POS implementations of this function are required to return the full details of the order with
		/// the corresponding <paramref name="orderId"/>.
		/// </remarks>
		/// <param name="posOrderId">The unique identifier of the order being queried in the POS.</param>
		/// <returns>The order details </returns>
		/// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistException">This exception 
		/// should be thrown when there is no order in the POS with the corresponding 
		/// <paramref name="posOrderId"/>.</exception>
		DoshiiDotNetIntegration.Models.Order RetrieveOrder(string posOrderId);

		/// <summary>
		/// The <see cref="DoshiiDotNetIntegration.DoshiiManager"/> uses this call to inform the point of
		/// sale that an order has been updated. The <paramref name="version"/> string must be persisted in
		/// the POS and passed back when the POS updates an order. 
		/// </summary>
		/// <remarks>
		/// The current <paramref name="version"/> is used by Doshii for conflict resolution, but the POS is 
		/// the final arbiter on the state of an order.
		/// </remarks>
		/// <param name="posOrderId">The unique identifier of the order being updated in the POS.</param>
		/// <param name="version">The current version of the order in Doshii.</param>
		/// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistException">This exception 
		/// should be thrown when there is no order in the POS with the corresponding 
		/// <paramref name="posOrderId"/>.</exception>
		void RecordOrderVersion(string posOrderId, string version);

		/// <summary>
		/// The <see cref="DoshiiDotNetIntegration.DoshiiManager"/> uses this call to request the current
		/// version of an order in the POS.
		/// </summary>
		/// <param name="posOrderId">The unique identifier of the order being queried on the POS.</param>
		/// <returns>The current version of the order in the POS.</returns>
		/// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistException">This exception 
		/// should be thrown when there is no order in the POS with the corresponding 
		/// <paramref name="posOrderId"/>.</exception>
		string RetrieveOrderVersion(string posOrderId);
	}
}
