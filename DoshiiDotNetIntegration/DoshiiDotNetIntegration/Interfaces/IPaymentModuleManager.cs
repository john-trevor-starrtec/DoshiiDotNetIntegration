using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Interfaces
{
	/// <summary>
	/// Implementations of this interface are required to accept a payment via Doshii.
	/// The POS should implement this interface in order to accept payment requests from the Doshii workflow.
	/// </summary>
	/// <remarks>
	/// The <c>IPaymentModuleManager</c> is the core interface of the Doshii SDK.
	/// When a partner wishes to make a payment via Doshii, the SDK emits a 
	/// <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.ReadyToPay"/> call.
	/// At this point, the POS should place the corresponding order into a "locked" state and send back the final
	/// details of the order to ensure that the order is current in Doshii. Once the partner captures the funds
	/// to pay off the order, the Doshii SDK emits a 
	/// <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.RecordSuccessfulPayment(Transaction)"/>
	/// call to finalize the payment. A payment canceled by the partner will cause the SDK to emit a call to
	/// <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.CancelPayment(Transaction)"/>, at which
	/// point the POS should unlock the order without accepting payment.
	/// </remarks>
	public interface IPaymentModuleManager
	{
		/// <summary>
		/// The Doshii SDK will call this function to indicate that the partner is ready to accept a payment against the order
		/// with the supplied <paramref name="transaction"/><c>.OrderId</c>. The POS is required to respond with a transaction containing the current amount owing for the 
		/// order in the <paramref name="transaction"/><c>.PaymentAmount</c> property. If the POS will accept a payment amount below the total amount owing, the 
		/// <paramref name="transaction"/><c>.AcceptLess</c> should be set to <c>true</c>. It is recommended that the POS places this order into a state that cannot 
		/// be edited from the POS after receiving the <c>ReadyToPay</c> call from the Doshii SDK.
		/// </summary>
        /// <param name="transaction">The payment details that has been initiated by the partner.</param>
		/// <returns>A payment transaction detailing the current amount owing on the order; 
		/// or <c>null</c> if the pos does not want the transaction from the partner to be processed for any reason.</returns>
		/// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistOnPosException">Thrown when the referenced order does not exist.</exception>
		Transaction ReadyToPay(Transaction transaction);

		/// <summary>
		/// The Doshii SDK will call this function to indicate that the partner has failed to claim payment for an order that
		/// was previously locked using a call to <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.ReadyToPay"/>.
		/// The POS should return the order to its previous state prior to the 
		/// <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.ReadyToPay"/> call, allowing it to be edited
		/// once more from the POS.
		/// </summary>
		/// <param name="transaction">The details of the payment being canceled.</param>
		void CancelPayment(Transaction transaction);

		/// <summary>
		/// The Doshii SDK will call this function after payment has been successfully captured for an order. 
		/// At this point the POS cannot reject the payment and must record the payment received. If the POS did not want to receive the payment the POS needed to reject the 
		/// <see cref="ReadyToPay"/> message, or reject the order when the order was received with a full payment if <see cref="IOrderingManager"/> has been implemented. 
		/// </summary>
		/// <param name="transaction">The details of the payment to be applied.</param>
		void RecordSuccessfulPayment(Transaction transaction);

        /// <summary>
        /// The <see cref="DoshiiDotNetIntegration.DoshiiManager"/> uses this call to inform the point of
        /// sale that an transaction version has been updated. The <paramref name="version"/> string must be persisted in
        /// the POS and passed back when the POS updates a transaction. 
        /// </summary>
        /// <remarks>
        /// The current <paramref name="version"/> is used by Doshii for conflict resolution, but the POS is 
        /// the final arbiter on the state of an order.
        /// </remarks>
        /// <param name="transactionId">The unique doshii identifier of the transaction being updated in the POS.</param>
        /// <param name="version">The current version of the transaction in Doshii.</param>
        /// <exception cref="DoshiiDotNetIntegration.Exceptions.TransactionDoesNotExistOnPosException">This exception 
        /// should be thrown when there is no transaction in the POS with the corresponding <paramref name="transactionId"/></exception>.
        void RecordTransactionVersion(string transactionId, string version);

        /// <summary>
        /// The <see cref="DoshiiDotNetIntegration.DoshiiManager"/> uses this call to request the current
        /// version of a transaction in the POS.
        /// </summary>
        /// <param name="transactionId">The unique doshii identifier of the transaction being queried on the POS.</param>
        /// <returns>The current version of the transaction in the POS.</returns>
        /// <exception cref="DoshiiDotNetIntegration.Exceptions.TransactionDoesNotExistOnPosException">This exception 
        /// should be thrown when there is no transaction in the POS with the corresponding 
        /// <paramref name="transactionId"/>.</exception>
        string RetrieveTransactionVersion(string transactionId);
	}
}
