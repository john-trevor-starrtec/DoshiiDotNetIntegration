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
	/// This view allows entry or retrieval of an order in Doshii.
	/// This is provided as a sample only; the POS implementation in a live environment would handle this as
	/// part of their point of sale application.
	/// </summary>
	public partial class SampleOrderForm : Form
	{
		/// <summary>
		/// The current application presenter.
		/// </summary>
		private SampleDotNetPOSPresenter mPresenter;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SampleOrderForm()
		{
			InitializeComponent();
			lblUpdatedAt.Text = String.Empty;
			lblUri.Text = String.Empty;
		}

		/// <summary>
		/// Attaches the presenter to the view.
		/// </summary>
		/// <param name="presenter">The presenter to be attached.</param>
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
		/// Updates the view with the order details.
		/// </summary>
		/// <param name="order">The order details to be displayed.</param>
		public void UpdateView(Order order)
		{
			tbxOrderId.Text = order.Id;

		}

		/// <summary>
		/// Reads back the details of the order as entered in the view.
		/// </summary>
		/// <returns>The order details currently displayed in the view.</returns>
		public Order ReadForm()
		{
			var order = new Order();

			order.Id = tbxOrderId.Text;


			return order;
		}

		/// <summary>
		/// Callback for when the Retrieve button is pressed.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void btnRetrieve_Click(object sender, EventArgs e)
		{
			if (mPresenter != null)
			{
				if (String.IsNullOrWhiteSpace(tbxOrderId.Text))
				{
					MessageBox.Show("Please enter the Order ID to retrieve.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					tbxOrderId.Focus();
					return;
				}

				var order = mPresenter.RetrieveOrder(tbxOrderId.Text);
				UpdateView(order);
			}
		}
	}
}
