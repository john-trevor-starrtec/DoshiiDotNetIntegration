using DoshiiDotNetIntegration.Exceptions;
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
	/// This is a sample implementation of the <see cref="DoshiiDotNetIntegration.Interfaces.IOrderingManager"/>
	/// interface.
	/// </summary>
	/// <remarks>
	/// As the POS provider, your job will be to implement the <see cref="DoshiiDotNetIntegration.Interfaces.IOrderingManager"/>
	/// interface in such a way that orders are available from the POS to the SDK with version control.
	/// This sample currently doesn't do anything except write calls back to the logging mechanism.
	/// </remarks>
	public class SampleOrderingManager : IOrderingManager, IDisposable
	{
		/// <summary>
		/// Presenter for the application.
		/// </summary>
		private SampleDotNetPOSPresenter mPresenter;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="presenter">Presenter for the application.</param>
		public SampleOrderingManager(SampleDotNetPOSPresenter presenter)
		{
			if (presenter == null)
				throw new ArgumentNullException("presenter");

			mPresenter = presenter;
		}

		#region IOrderingManager Members

		/// <summary>
		/// See <see cref="DoshiiDotNetIntegration.Interfaces.IOrderingManager.RetrieveOrder(string)"/> for details on this call.
		/// </summary>
		/// <param name="posOrderId"></param>
		/// <returns></returns>
		public Order RetrieveOrder(string posOrderId)
		{
			if (mPresenter != null)
			{
				mPresenter.LogMessage(typeof(SampleOrderingManager), String.Format("Retrieving order {0} from POS", posOrderId), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Info);

				var order = new Order();
				order.Id = posOrderId;
				return order;
			}

			throw new OrderDoesNotExistOnPosException(String.Format("Order {0} does not exist!", posOrderId));
		}

		/// <summary>
		/// See <see cref="DoshiiDotNetIntegration.Interfaces.IOrderingManager.RecordOrderVersion(string, string)"/> for details on this call.
		/// </summary>
		/// <param name="posOrderId"></param>
		/// <returns></returns>
		public void RecordOrderVersion(string posOrderId, string version)
		{
			if (mPresenter != null)
			{
				mPresenter.LogMessage(typeof(SampleOrderingManager), String.Format("Setting order {0} to version {1} in POS", posOrderId, version), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Info);

				var order = new Order();
				order.Id = posOrderId;
				order.Version = version;
			}

			throw new OrderDoesNotExistOnPosException(String.Format("Order {0} does not exist!", posOrderId));
		}

		/// <summary>
		/// See <see cref="DoshiiDotNetIntegration.Interfaces.IOrderingManager.RetrieveOrderVersion(string)"/> for details on this call.
		/// </summary>
		/// <param name="posOrderId"></param>
		/// <returns></returns>
		public string RetrieveOrderVersion(string posOrderId)
		{
			if (mPresenter != null)
			{
				mPresenter.LogMessage(typeof(SampleOrderingManager), String.Format("Retrieving version for order {0} from POS", posOrderId), DoshiiDotNetIntegration.Enums.DoshiiLogLevels.Info);

				return "XYZ1234";
			}

			throw new OrderDoesNotExistOnPosException(String.Format("Order {0} does not exist!", posOrderId));
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Cleanly disposes of the instance.
		/// </summary>
		public void Dispose()
		{
			mPresenter = null;
		}

		#endregion
	}
}
