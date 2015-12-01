using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Interfaces
{
	/// <summary>
	/// Implementations of this interface are required to accept a payment via Doshii.
	/// The POS should implement this interface in order to accept payment requests from the Doshii workflow.
	/// </summary>
	/// <remarks>
	/// The <c>IPaymentModuleManager</c> is the core interface of the Doshii API.
	/// When a partner wishes to make a payment via Doshii, the API emits a 
	/// <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.ReadyToPay(string)"/> call.
	/// At this point, the POS should place the corresponding order into a "locked" state and send back the final
	/// details of the order to ensure that the order is current in Doshii. Once the partner captures the funds
	/// to pay off the order, the Doshii API emits a 
	/// <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.ReadyToPay(string, DoshiiDotNetIntegration.Models.Payment)"/>
	/// call to finalise the payment.
	/// </remarks>
	public interface IPaymentModuleManager
	{
		/// <summary>
		/// The Doshii SDK will call this function to indicate that the partner is ready to accept a payment against the order
		/// with the supplied <paramref name="orderId"/>. The POS is required to respond with the current details of the order
		/// and it is recommended that the POS places this order into a state that cannot be edited from the POS.
		/// </summary>
		/// <param name="orderId">The identifier for the order being paid.</param>
		/// <returns>The current POS-side state of the order with the corresponding <paramref name="orderId"/>; 
		/// or <c>null</c> if not found.</returns>
		DoshiiDotNetIntegration.Models.Order ReadyToPay(string orderId);

		/// <summary>
		/// The Doshii SDK will call this function to indicate that the partner has failed to claim payment for an order that
		/// was previously locked using a call to <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.ReadyToPay(string)"/>.
		/// The POS should return the order to its previous state prior to the 
		/// <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.ReadyToPay(string)"/> call, allowing it to be edited
		/// once more from the POS.
		/// </summary>
		/// <param name="orderId">The identifier for the order previously being paid.</param>
		/// <returns>The current POS-side state of the order with the corresponding <paramref name="orderId"/>.</returns>
		DoshiiDotNetIntegration.Models.Order CancelPayment(string orderId);

		/// <summary>
		/// The Doshii SDK will call this function after payment has been captured for an order with the supplied <paramref name="orderId"/>.
		/// The POS is required to respond with the current details of the order after the payment has been applied.
		/// </summary>
		/// <param name="orderId">The identifier for the order being paid.</param>
		/// <param name="paymentAmount">The amount paid.</param>
		/// <returns>The current POS-side state of the order with the corresponding <paramref name="orderId"/>.</returns>
		DoshiiDotNetIntegration.Models.Order AcceptPayment(string orderId, decimal paymentAmount);
	}
}
