using DoshiiDotNetIntegration.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models;

namespace SampleDotNetPOS.POSImpl
{
    public class SampleReservationManager : IReservationManager, IDisposable
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

        public bool CreateBookingOnPos(Booking booking)
        {
            if (mPresenter != null)
            {
                var fBooking = mPresenter.RetrieveBooking(booking.Id);
                if (fBooking == null)
                {
                    mPresenter.UpdateBooking(booking);
                    return true;
                }
            }
            throw new DoshiiDotNetIntegration.Exceptions.BookingExistOnPosException();
        }

        public bool DeleteBookingOnPos(Booking booking)
        {
            if (mPresenter != null)
            {
                var fBooking = mPresenter.RetrieveBooking(booking.Id);
                if (fBooking != null)
                {
                    mPresenter.DeleteBooking(booking.Id);
                    return true;
                }
            }

            throw new DoshiiDotNetIntegration.Exceptions.BookingDoesNotExistOnPosException();
        }

        public void RecordCheckinForBooking(string bookingId, string checkinId)
        {
            if (mPresenter != null)
            {
                var booking = mPresenter.RetrieveBooking(bookingId);
                if (booking != null)
                {
                    booking.CheckinId = checkinId;
                    mPresenter.UpdateBooking(booking);
                    return;
                }
            }

            throw new DoshiiDotNetIntegration.Exceptions.BookingDoesNotExistOnPosException();
        }

        public bool UpdateBookingOnPos(Booking booking)
        {
            if (mPresenter != null)
            {
                var fBooking = mPresenter.RetrieveBooking(booking.Id);
                if (fBooking != null)
                {
                    mPresenter.UpdateBooking(booking);
                    return true;
                }
            }

            throw new DoshiiDotNetIntegration.Exceptions.BookingDoesNotExistOnPosException();
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
