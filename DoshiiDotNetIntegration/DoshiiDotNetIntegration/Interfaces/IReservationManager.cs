using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Interfaces
{
	   /// <summary>
    /// Implementations of this interface is required to handle reservation functionality in Doshii.
    /// <para/>The POS should implement this interface to enable reservations and table bookings.
    /// </summary>
    /// <remarks>
    /// <para/><see cref="DoshiiDotNetIntegration.DoshiiManager"/> uses this interface as a callback mechanism 
    /// to the POS for reservation functions. 
    /// <para>
    /// </para>
    /// </remarks>
	public interface IReservationManager
	{
		/// <summary>
		/// This method should create a doshii booking on the Pos.
		/// </summary>
		/// <param name="booking"></param>
		/// <returns></returns>
		bool CreateBookingOnPos(Booking booking);

		/// <summary>
		/// This method should update a doshii booking on the Pos.
		/// </summary>
		/// <param name="booking"></param>
		/// <returns></returns>
		bool UpdateBookingOnPos(Booking booking);

		/// <summary>
		/// This method should delete a doshii booking on the Pos.
		/// </summary>
		/// <param name="booking"></param>
		/// <returns></returns>
		bool DeleteBookingOnPos(Booking booking);

        /// <summary>
        /// The <see cref="DoshiiDotNetIntegration.DoshiiManager"/> uses this call to inform the pos the checkin 
        /// associated with an booking stored on Doshii. The <paramref name="checkinId"/> string must be persisted in
        /// the POS against the booking - the checkinId is the link between booking and orders and tables and also consumers, in the doshii API. 
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="checkinId">The current checkinId related to the booking in Doshii.</param>
        /// <exception cref="DoshiiDotNetIntegration.Exceptions.BookingDoesNotExistOnPosException">This exception 
        /// should be thrown when there is no booking in the POS with the corresponding 
        /// <paramref name="bookingId"/>.</exception>
        void RecordCheckinForBooking(string bookingId, string checkinId);
    }
}
