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
    public partial class SampleArriveForm : Form
    {
        /// <summary>
        /// The presenter controlling the view.
        /// </summary>
        private SampleDotNetPOSPresenter mPresenter;

        public SampleArriveForm()
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
        /// Updates the view with the booking details.
        /// </summary>
        /// <param name="booking">The booking details to be displayed.</param>
        public void UpdateView(Booking booking)
        {
            tbxTableNames.Text = String.Join(";", booking.TableNames.ToArray());
            tbxCovers.Text = booking.Covers.ToString();
            tbxId.Text = booking.Id;
        }

        /// <summary>
        /// Reads back the details of the booking as entered in the view.
        /// </summary>
        /// <returns>The booking details currently displayed in the view.</returns>
        public Checkin ReadForm()
        {
            var checkin = new Checkin();
            checkin.Ref = tbxPosRef.Text;
            checkin.TableNames = new List<String>() { tbxTableNames.Text };
            checkin.Covers = int.Parse(tbxCovers.Text);
            return checkin;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            var booking = mPresenter.GetBooking(tbxId.Text);
            if (booking != null)
            {
            }
        }
    }
}