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
		/// See <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.ReadyToPay(Transaction)"/> for details of this call.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public Transaction ReadyToPay(Transaction transaction)
		{
			transaction.Status = "waiting";
			transaction.AcceptLess = true;

			if (String.IsNullOrWhiteSpace(transaction.OrderId))
			{
				transaction.PaymentAmount = 240M;
			}
			else
			{
				var order = mPresenter.RetrieveOrder(transaction.OrderId);
				if (order == null)
				{
					transaction.PaymentAmount = 240M;
				}
				else
				{
				    transaction.PaymentAmount = 240M;
				}
			}

			return transaction;
		}

		/// <summary>
		/// See <see cref="DoshiiDotNetIntegration.Interfaces.IPaymentModuleManager.CancelPayment(Transaction)"/> for details of this call.
		/// </summary>
		/// <param name="transaction"></param>
		public void CancelPayment(Transaction transaction)
		{
			//cancel the payment on the pos
			if (mPresenter != null)
				mPresenter.RemoveTransaction(transaction.Id);
		}

		/// <summary>
		/// See <see cref="IPaymentModuleManager.RecordSuccessfulPayment"/> for details of this call.
		/// </summary>
		/// <param name="transaction"></param>
		public void RecordSuccessfulPayment(Transaction transaction)
		{
			if (mPresenter != null)
			{
				mPresenter.AddOrUpdateTransaction(transaction);
				var order = UpdateOrder(transaction.OrderId, "paid");
				
			}
		}

		/// <summary>
		/// See <see cref="IPaymentModuleManager.RecordTransactionVersion"/> for details of this call.
		/// </summary>
		/// <param name="transactionId"></param>
		/// <param name="version"></param>
	    public void RecordTransactionVersion(string transactionId, string version)
	    {
	        if (mPresenter != null)
			{
				var transaction = mPresenter.RetrieveTransaction(transactionId);
				if (transaction != null)
				{
					transaction.Version = version;
					return;
				}
			}

			throw new DoshiiDotNetIntegration.Exceptions.TransactionDoesNotExistOnPosException();
	    }

		/// <summary>
		/// See <see cref="IPaymentModuleManager.RetrieveTransactionVersion"/> for details of this call.
		/// </summary>
		/// <param name="transactionId"></param>
		/// <param name="version"></param>
	    public string RetrieveTransactionVersion(string transactionId)
	    {
	        if (mPresenter != null)
			{
				var transaction = mPresenter.RetrieveTransaction(transactionId);
				if (transaction != null)
				{
					return transaction.Version;
				}
			}

			throw new DoshiiDotNetIntegration.Exceptions.TransactionDoesNotExistOnPosException();
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
