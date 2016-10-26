using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Helpers;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Controllers
{
    internal class ReservationController
    {
        internal Models.Controllers _controllers;
        internal HttpController _httpComs;

        internal ReservationController(Models.Controllers controller, HttpController httpComs)
        {
            if (controller == null)
            {
                throw new NullReferenceException("controller cannot be null");
            }
            _controllers = controller;
            if (_controllers.LoggingController == null)
            {
                throw new NullReferenceException("doshiiLogger cannot be null");
            }

            if (_controllers.ReservationManager == null)
            {
                _controllers.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - rewardManager cannot be null");
                throw new NullReferenceException("rewardManager cannot be null");
            }
            if (_controllers.OrderingManager == null)
            {
                _controllers.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - orderingManager cannot be null");
                throw new NullReferenceException("orderingManager cannot be null");
            }
            if (_controllers.OrderingController == null)
            {
                _controllers.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - orderingController cannot be null");
                throw new NullReferenceException("orderingController cannot be null");
            }
            if (httpComs == null)
            {
                _controllers.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;
        }

        internal virtual Booking GetBooking(String bookingId)
        {
            try
            {
                return _httpComs.GetBooking(bookingId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual List<Booking> GetBookings(DateTime from, DateTime to)
        {
            try
            {
                return _httpComs.GetBookings(from, to).ToList();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal bool SeatBooking(String bookingId, Checkin checkin, String posOrderId = null)
        {
            _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: pos Seating Booking '{0}'", bookingId));

            Order order = null;
            if (posOrderId != null)
            {
                try
                {
                    order = _controllers.OrderingManager.RetrieveOrder(posOrderId);
                    order.Version = _controllers.OrderingManager.RetrieveOrderVersion(posOrderId);
                    order.CheckinId = _controllers.OrderingManager.RetrieveCheckinIdForOrder(posOrderId);
                    order.Status = "accepted";
                }
                catch (OrderDoesNotExistOnPosException dne)
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, "Doshii: Order does not exist on POS during seating");
                    throw dne;
                }

                if (order == null)
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, "Doshii: NULL Order returned from POS during seating");
                    throw new OrderDoesNotExistOnPosException("Doshii: The pos returned a null order during seating", new OrderUpdateException());
                }

                if (!String.IsNullOrEmpty(order.CheckinId))
                {
                    try
                    {
                        Checkin orderCheckin = _httpComs.GetCheckin(order.CheckinId);
                        if (orderCheckin != null)
                        {
                            if (orderCheckin.Id.CompareTo(checkin.Id) != 0)
                            {
                                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, "Doshii: Order checkin id not equal to booking checkin id");
                                throw new BookingCheckinException("Doshii: Order checkin id != booking checkin id");
                            }
                            if (orderCheckin.Covers != checkin.Covers)
                            {
                                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, "Doshii: Order covers not equal to booking covers");
                                throw new BookingCheckinException("Doshii: Order covers != booking covers");
                            }
                            if (orderCheckin.Consumer.CompareTo(checkin.Consumer) != 0)
                            {
                                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, "Doshii: Order consumer not equal to booking consumer");
                                throw new BookingCheckinException("Doshii: Order consumer != booking consumer");
                            }
                            if (orderCheckin.TableNames.All(o => checkin.TableNames.Contains(o)) && orderCheckin.TableNames.Count == checkin.TableNames.Count)
                            {
                                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, "Doshii: Order Tables not equal to booking tables");
                                throw new BookingCheckinException("Doshii: Order tables != booking tables");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Fatal, "Doshii: an exception was thrown getting the checking", ex);
                        throw ex;
                    }
                }
            }

            Checkin bookingCheckin = null;
            try
            {
                bookingCheckin = _httpComs.SeatBooking(bookingId, checkin);
                if (bookingCheckin == null)
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: There was an error generating a new checkin through Doshii, the seat booking could not be completed."));
                    return false;
                }
            }
            catch (Exception ex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown while attempting a seat booking Id{0} : {1}", bookingId, ex));
                throw new BookingUpdateException(string.Format("Doshii: a exception was thrown during an attempt to seat a booking. Id{0}", bookingId), ex);
            }

            _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: Booking Seated."));

            _controllers.ReservationManager.RecordCheckinForBooking(bookingId, bookingCheckin.Id);

            if (order != null)
            {
                order.CheckinId = bookingCheckin.Id;
                Order returnedOrder = _controllers.OrderingController.UpdateOrder(order);
                if (returnedOrder == null)
                    return false;
            }

            return true;
        }
    }
}
