using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDotNetPOS.POSImpl
{
	/// <summary>
	/// This is a sample implementation of the <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager"/>
	/// interface.
	/// </summary>
	/// <remarks>
	/// As the POS provider, your job will be to implement the <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager"/>
	/// interface in such a way that orders are paid for in the POS via the calls coming from the Doshii SDK.
	/// This sample doesn't actually do anything except update the order status accordingly.
	/// </remarks>
	public class SamplePaymentModuleManager : IPaymentModuleManager, IDisposable
	{
		/// <summary>
		/// Presenter for the application.
		/// </summary>
		private SampleDotNetPOSPresenter mPresenter;

		/// <summary>
		/// Attaches the presenter to the payment manager.
		/// </summary>
		/// <param name="presenter">The presenter to be attached.</param>
		public void AttachPresenter(SampleDotNetPOSPresenter presenter)
		{
			if (presenter == null)
				throw new ArgumentNullException("presenter");
			mPresenter = presenter;
		}

		/// <summary>
		/// Removes the presenter reference.
		/// </summary>
		public void RemovePresenter()
		{
			mPresenter = null;
		}

		#region IPaymentModuleManager Members

		/// <summary>
		/// See <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.ReadyToPay(string)"/> for details of this call.
		/// </summary>
		/// <param name="orderId"></param>
		/// <returns></returns>
		public Order ReadyToPay(string orderId)
		{
			return UpdateOrder(orderId, "waiting_for_payment");
		}

		/// <summary>
		/// See <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.CancelPayment(string)"/> for details of this call.
		/// </summary>
		/// <param name="orderId"></param>
		/// <returns></returns>
		public Order CancelPayment(string orderId)
		{
			return UpdateOrder(orderId, "accepted");
		}

		/// <summary>
		/// See <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.AcceptPayment(string, decimal)"/> for details of this call.
		/// </summary>
		/// <param name="orderId"></param>
		/// <returns></returns>
		public Order AcceptPayment(string orderId, decimal paymentAmount)
		{
			var order = UpdateOrder(orderId, "paid");
			var payment = new Payment();
			payment.PaymentType = "TEST PAYMENT TYPE";
			payment.PaymentAmount = paymentAmount;
			var payments = order.Payments.ToList<Payment>();
			payments.Add(payment);
			order.Payments = payments;
			return order;
		}

		#endregion

		/// <summary>
		/// Returns a blank order with the new status for testing purposes.
		/// </summary>
		/// <param name="orderId">The ID of the order.</param>
		/// <param name="status">The new status of the order.</param>
		/// <returns>The order details.</returns>
		private Order UpdateOrder(string orderId, string status)
		{
			var result = new Order();

			if (mPresenter != null)
			{
				result = mPresenter.RetrieveOrder(orderId);
			}
			else			
				result.Id = orderId;

			result.Status = status;
			return result;
		}

		#region IDisposable Members

		/// <summary>
		/// Disposes of the instance by cleaning up the memory imprint.
		/// </summary>
		public void Dispose()
		{
			RemovePresenter();
		}

		#endregion
	}
}
