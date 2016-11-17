using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoshiiDotNetIntegration.Models;

namespace SampleDotNetPOS
{
    /// <summary>
    /// This form displays the list of current bookings. 
    /// </summary>
    public partial class SampleBookingListForm : Form
    {
        /// <summary>
        /// The presenter controlling the view.
        /// </summary>
        private SampleDotNetPOSPresenter mPresenter;

        public SampleBookingListForm()
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
        /// Adds a booking to the list.
        /// </summary>
        /// <param name="booking"></param>
        public void AddBooking(Booking booking)
        {
            int index = dgvBookings.Rows.Add(
                    booking.Id,
                    String.Join(",", booking.TableNames.ToArray()),
                    booking.Date.ToLocalTime().ToString(),
                    booking.Covers.ToString(),
                    booking.Consumer.Name,
                    booking.Consumer.Phone,
                    booking.CheckinId
                );
            dgvBookings.Rows[index].Tag = booking;
        }

        private void btnArrive_Click(object sender, EventArgs e)
        {
            if (dgvBookings.CurrentRow != null)
            {
                mPresenter.SendBookingCheckin(dgvBookings.CurrentRow.Tag as Booking);
            }
        }

        private void dgvBookings_SelectionChanged(object sender, EventArgs e)
        {
            btnArrive.Enabled = dgvBookings.CurrentRow != null;
        }
    }
}
