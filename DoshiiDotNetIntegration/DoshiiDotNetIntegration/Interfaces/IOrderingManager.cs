using DoshiiDotNetIntegration.Models;
using System.Collections.Generic;

namespace DoshiiDotNetIntegration.Interfaces
{
	/// <summary>
	/// Implementations of this interface is required to handle orders in Doshii.
	/// <para/>The POS should implement this interface to accept new orders and updates to orders from Doshii.
    /// <para/>Version control on orders is also managed through the POS implementation of this interface.
	/// </summary>
	/// <remarks>
	/// This interface is a core Doshii interface required for implementation on the POS side. 
    /// <para/><see cref="DoshiiController"/> uses this interface as a callback mechanism 
	/// to the POS for basic order functions. 
    /// <para/>It should be noted however that this interface is not the handler for extension modules such as Order@Table which will be implemented in a separate
	/// callback interface.
	/// <para>
	/// When a partner wishes to perform an action on an order in Doshii, the SDK emits a call to 
	/// <see cref="DoshiiDotNetIntegration.Interfaces.IOrderingManager.RetrieveOrder(string)"/> to
	/// retrieve the current state of the order in the POS. 
    /// <para/>After a partner has mutated the order in Doshii
	/// in some way, the SDK emits a call to 
	/// <see cref="DoshiiDotNetIntegration.Interfaces.IOrderingManager.RecordOrderVersion(string, string)"/>
	/// to inform the POS of the update success. 
	/// </para>
	/// </remarks>
	public interface IOrderingManager
	{
		/// <summary>
		/// The <see cref="DoshiiController"/> uses this call to request the current
		/// details of an order from the POS.
		/// </summary>
		/// <remarks>
		/// POS implementations of this function are required to return the full details of the order with
        /// the corresponding <paramref name="posOrderId"/>.
		/// </remarks>
		/// <param name="posOrderId">The unique identifier of the order being queried in the POS.</param>
		/// <returns>The order details </returns>
		/// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistOnPosException">This exception 
		/// should be thrown when there is no order in the POS with the corresponding 
		/// <paramref name="posOrderId"/>.</exception>
		DoshiiDotNetIntegration.Models.Order RetrieveOrder(string posOrderId);

		/// <summary>
		/// The <see cref="DoshiiController"/> uses this call to inform the pos
		/// that an order has been updated. The <paramref name="version"/> string must be persisted in
		/// the POS and passed back when the POS updates an order. 
		/// </summary>
		/// <remarks>
		/// The current <paramref name="version"/> is used by Doshii for conflict resolution, but the POS is 
		/// the final arbiter on the state of an order.
		/// </remarks>
		/// <param name="posOrderId">The unique identifier of the order being updated in the POS.</param>
		/// <param name="version">The current version of the order in Doshii.</param>
		/// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistOnPosException">This exception 
		/// should be thrown when there is no order in the POS with the corresponding 
		/// <paramref name="posOrderId"/>.</exception>
		void RecordOrderVersion(string posOrderId, string version);

		/// <summary>
		/// The <see cref="DoshiiController"/> uses this call to request the current
		/// version of an order in the POS.
		/// </summary>
		/// <param name="posOrderId">The unique identifier of the order being queried on the POS.</param>
		/// <returns>The current version of the order in the POS.</returns>
		/// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistOnPosException">This exception 
		/// should be thrown when there is no order in the POS with the corresponding 
		/// <paramref name="posOrderId"/>.</exception>
		string RetrieveOrderVersion(string posOrderId);

        /// <summary>
        /// The <see cref="DoshiiController"/> uses this call to inform the pos the checkin 
        /// associated with an order stored on Doshii. The <paramref name="checkinId"/> string must be persisted in
        /// the POS against the order - the checkinId is the link between orders and tables and also orders and consumers, in the doshii API. 
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="posOrderId">The unique identifier of the order being updated in the POS.</param>
        /// <param name="checkinId">The current checkinId related to the order in Doshii.</param>
        /// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistOnPosException">This exception 
        /// should be thrown when there is no order in the POS with the corresponding 
        /// <paramref name="posOrderId"/>.</exception>
        void RecordCheckinForOrder(string posOrderId, string checkinId);

        /// <summary>
        /// The <see cref="DoshiiController"/> uses this call to request the current
        /// checkinId associated with an order.
        /// </summary>
        /// <param name="posOrderId">The unique identifier of the order being queried on the POS.</param>
        /// <returns>The current version of the order in the POS.</returns>
        /// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistOnPosException">This exception 
        /// should be thrown when there is no order in the POS with the corresponding 
        /// <paramref name="posOrderId"/>.</exception>
        string RetrieveCheckinIdForOrder(string posOrderId);

        /// <summary>
        /// The <see cref="DoshiiController"/> calls this method on the pos so the pos can confirm the acceptance of the order. 
        /// The pos must check that the order can be made on the pos, and that the transactions total the correct amount for payment of the order in full
        /// The pos cannot modify the <see cref="Transaction"/> objects in the transaction list, during this process as the amount has already been confirmed with the consumer.
        /// If the <see cref="Order"/> is accepted the POS must update the <see cref="Order.Id"/> property with the pos reference to the order and call <see cref="DoshiiController.AcceptOrderAheadCreation"/> with the order, and the
        /// if the response from <see cref="DoshiiController.AcceptOrderAheadCreation"/> is successful - if the response is false this could indicate that the order has been canceled or changed on doshii and the pos will receive another create notification if necessary.
        /// The transaction should not be recorded on the POS during this method.
        /// The Pos will receive a call to <see cref="ITransactionManager.RecordSuccessfulPayment"/> to record the transaction
        /// If the <see cref="Order"/> or the <see cref="Transaction"/> is rejected the pos should call <see cref="DoshiiController.RejectOrderAheadCreation"/> with the order.
        /// </summary>
        /// <param name="order">
        /// The <see cref="Order"/> to be approved
        /// </param>
        /// <param name="transactionList">
        /// A List of <see cref="Transaction"/> to be approved
        /// </param>
        /// /// <param name="consumer">
        /// The <see cref="Consumer"/> associated with the order
        /// </param>
        /// <returns></returns>
        void ConfirmNewDeliveryOrderWithFullPayment(Order order, Consumer consumer, IEnumerable<Transaction> transactionList);

        /// <summary>
        /// The <see cref="DoshiiController"/> calls this method on the pos so the pos can confirm the acceptance of the order. 
        /// The pos must check that the order can be made on the pos.
        /// if the <see cref="Order"/> is accepted the POS must update the <see cref="Order.Id"/> property with the pos reference to the order and call <see cref="DoshiiController.AcceptOrderAheadCreation"/> with the order, 
        /// if the response from <see cref="DoshiiController.AcceptOrderAheadCreation"/> is successful - if the response is false this could indicate that the order has been canceled or changed on doshii and the pos will receive another create notification if necessary.
        /// If the <see cref="Order"/> is rejected the pos should call <see cref="DoshiiController.RejectOrderAheadCreation"/> with the order.
        /// </summary>
        /// <param name="order">
        /// The <see cref="Order"/> to be approved
        /// </param>
        /// <param name="consumer">
        /// The <see cref="Consumer"/> associated with the order
        /// <para/> The consumer object contains data including;
        /// <item>The consumer address</item>
        /// <item>The consumer phone number</item>
        /// <item>The consumer name</item>
        /// <item>Special notes relating to the order</item>
        /// </param>
        /// <returns></returns>
	    void ConfirmNewDeliveryOrder(Order order, Consumer consumer);

        /// <summary>
        /// The <see cref="DoshiiController"/> calls this method on the pos so the pos can confirm the acceptance of the order. 
        /// The pos must check that the order can be made on the pos, and that the transactions total the correct amount for payment of the order in full
        /// The pos cannot modify the <see cref="Transaction"/> objects in the transaction list, during this process as the amount has already been confirmed with the consumer.
        /// If the <see cref="Order"/> is accepted the POS must update the <see cref="Order.Id"/> property with the pos reference to the order and and call <see cref="DoshiiController.AcceptOrderAheadCreation"/> with the order, and the
        /// order should be made on the pos if the response from <see cref="DoshiiController.AcceptOrderAheadCreation"/> is successful - if the response is false this could indicate that the order has been canceled or changed on doshii and the pos will receive another create notification if necessary. 
        /// The transaction should not be recorded on the POS during this method. The
        /// he Pos will receive a call to <see cref="ITransactionManager.RecordSuccessfulPayment"/> to record the transaction
        /// If the <see cref="Order"/> or the <see cref="Transaction"/> is rejected the pos should call <see cref="DoshiiController.RejectOrderAheadCreation"/> with the order.
        /// </summary>
        /// <param name="order">
        /// The <see cref="Order"/> to be approved
        /// </param>
        /// <param name="transactionList">
        /// A List of <see cref="Transaction"/> to be approved
        /// </param>
        /// <param name="consumer">
        /// The <see cref="Consumer"/> associated with the order
        /// <para/> The consumer object contains data including;
        /// <item>The consumer address</item>
        /// <item>The consumer phone number</item>
        /// <item>The consumer name</item>
        /// <item>Special notes relating to the order</item>
        /// </param>
        /// <returns></returns>
        void ConfirmNewPickupOrderWithFullPayment(Order order, Consumer consumer, IEnumerable<Transaction> transactionList);

        /// <summary>
        /// The <see cref="DoshiiController"/> calls this method on the pos so the pos can confirm the acceptance of the order. 
        /// The pos must check that the order can be made on the pos.
        /// if the <see cref="Order"/> is accepted the POS must update the <see cref="Order.Id"/> property with the pos reference to the order and call <see cref="DoshiiController.AcceptOrderAheadCreation"/> with the order, and the 
        /// order should be made on the pos if the response from <see cref="DoshiiController.AcceptOrderAheadCreation"/> is successful - if the response is false this could indicate that the order has been canceled or changed on doshii and the pos will receive another create notification if necessary.. 
        /// If the <see cref="Order"/> is rejected the pos should call <see cref="DoshiiController.RejectOrderAheadCreation"/> with the order.
        /// </summary>
        /// <param name="order">
        /// the <see cref="Order"/> to be approved
        /// </param>
        /// <param name="consumer">
        /// The <see cref="Consumer"/> associated with the order
        /// <para/> The consumer object contains data including;
        /// <item>The consumer address</item>
        /// <item>The consumer phone number</item>
        /// <item>The consumer name</item>
        /// <item>Special notes relating to the order</item>
        /// </param>
        /// <returns></returns>
        void ConfirmNewPickupOrder(Order order, Consumer consumer);


        void ConfirmNewUnknownTypeOrderWithFullPayment(Order order, Consumer consumer, IEnumerable<Transaction> transactionList);
        
        void ConfirmNewUnknownTypeOrder(Order order, Consumer consumer);
	}
}
