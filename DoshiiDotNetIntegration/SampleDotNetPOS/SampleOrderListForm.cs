using DoshiiDotNetIntegration.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleDotNetPOS
{
	/// <summary>
	/// This form displays the list of current orders. 
	/// </summary>
	public partial class SampleOrderListForm : Form
	{
		/// <summary>
		/// The presenter controlling the view.
		/// </summary>
		private SampleDotNetPOSPresenter mPresenter;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SampleOrderListForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Attaches the supplied <paramref name="presenter"/> to the view.
		/// </summary>
		/// <param name="presenter">The presenter to control the view.</param>
		/// <exception cref="System.ArgumentNullException">Thrown when the supplied <paramref name="presenter"/> is <c>null</c> -
		/// use a call to <see cref="RemovePresenter()"/> to remove the reference to the presenter instead.</exception>
		public void AttachPresenter(SampleDotNetPOSPresenter presenter)
		{
			if (presenter == null)
				throw new ArgumentNullException("presenter");

			mPresenter = presenter;
		}

		/// <summary>
		/// Removes the presenter from the view.
		/// </summary>
		public void RemovePresenter()
		{
			mPresenter = null;
		}

		/// <summary>
		/// Adds an order to the list.
		/// </summary>
		/// <param name="order">The order to be added to the view.</param>
		public void AddOrder(Order order)
		{
			int index = dgvOrders.Rows.Add(
				order.Id, 
				order.DoshiiId, 
				order.Status, 
				order.InvoiceId, 
				order.LocationId, 
				order.Version, 
				order.Uri, 
				order.Items.Count, 
				order.Items.Sum(o => o.UnitPrice).ToString("c"), 
				order.Surcounts.Count,
				order.Surcounts.Sum(o => o.Amount).ToString("c")
			);
			dgvOrders.Rows[index].Tag = order;
		}

		/// <summary>
		/// Adds an item to the view.
		/// </summary>
		/// <param name="item">Item to be added.</param>
		public void AddItem(Product item)
		{
			int index = dgvOrderItems.Rows.Add(
				item.Id,
				item.Name,
				item.Description,
				item.UnitPrice.ToString("c"),
				String.Join(";", item.Tags),
				item.Version,
				item.PosId,
				item.Image
			);
			dgvOrderItems.Rows[index].Tag = item;
		}

		/// <summary>
		/// Adds a surcount to the view.
		/// </summary>
		/// <param name="surcount">Surcount to be added.</param>
		public void AddSurcount(Surcount surcount)
		{
			int index = dgvSurcounts.Rows.Add(
				surcount.Name,
				surcount.Amount.ToString("c")
			);
			dgvSurcounts.Rows[index].Tag = surcount;
		}

		/// <summary>
		/// Adds a payment to the view.
		/// </summary>
		/// <param name="payment">Payment to be added.</param>
		public void AddPayment(Transaction payment)
		{
			int index = dgvOrderPayments.Rows.Add(
				payment.Id,
				payment.Reference,
				payment.Invoice,
				payment.PaymentAmount.ToString("c"),
				payment.AcceptLess,
				payment.PartnerInitiated,
				payment.Partner,
				payment.Status,
				payment.Version,
				payment.Uri
			);
			dgvOrderPayments.Rows[index].Tag = payment;
		}

		/// <summary>
		/// Callback for when the icon link is clicked.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void lblIconLink_Click(object sender, EventArgs e)
		{
			// this link is here so we can use the icons in this form
			string url = "http://www.awicons.com/stock-icons/";
			System.Diagnostics.Process.Start(url);
		}

		/// <summary>
		/// Callback for when the selected order changes.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void dgvOrders_SelectionChanged(object sender, EventArgs e)
		{
			ClearOrderChildViews();

			if (dgvOrders.SelectedRows.Count == 1)
			{
				var row = dgvOrders.SelectedRows[0];
				var order = row.Tag as Order;
				if (order != null)
				{
					UpdateOrderChildViews(order);
				}
			}
		}

		/// <summary>
		/// Clears the items, surcharges and payments from the view.
		/// </summary>
		private void ClearOrderChildViews()
		{
			dgvOrderItems.Rows.Clear();
			dgvSurcounts.Rows.Clear();
			dgvOrderPayments.Rows.Clear();
		}

		/// <summary>
		/// Updates the current items, surcharges and payments in the view.
		/// </summary>
		/// <param name="order">The current order to be displayed.</param>
		private void UpdateOrderChildViews(Order order)
		{
			foreach (var item in order.Items)
			{
				AddItem(item);
			}

			foreach (var surcount in order.Surcounts)
			{
				AddSurcount(surcount);
			}

			if (mPresenter != null)
			{
				var payments = mPresenter.RetrieveOrderTransactions(order.Id);
				foreach (var payment in payments)
				{
					AddPayment(payment);
				}
			}
		}
	}
}
