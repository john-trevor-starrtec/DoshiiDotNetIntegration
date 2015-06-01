using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;


namespace DoshiiDotNetIntegration
{
    /// <summary>
    /// This class facilitates the operation of the Doshii integration with POS software.
    /// </summary>
    public class DoshiiOperationLogic 
    {

        #region properties, constructors, Initialize, versionCheck

        /// <summary>
        /// Holds an instance of CommunicationLogic.DoshiiWebSocketsCommunication class for interacting with the Doshii webSocket connection
        /// </summary>
        public CommunicationLogic.DoshiiWebSocketsCommunication m_SocketComs = null;

        /// <summary>
        /// Holds an instance of CommunicationLogic.DoshiiHttpCommunication class for interacting with the Doshii HTTP restful API
        /// </summary>
        public CommunicationLogic.DoshiiHttpCommunication m_HttpComs = null;

        /// <summary>
        /// holds an implementation of Interfaces.iDoshiiOrdering used to facilitate the ordering functionality offered by Doshii. 
        /// </summary>
        public  Interfaces.iDoshiiOrdering m_DoshiiInterface = null;

        /// <summary>
        /// The order mode for the venue
        /// </summary>
        public  Enums.OrderModes OrderMode{ get; set; }

        /// <summary>
        /// The seating mode for the venue
        /// </summary>
        public  Enums.SeatingModes SeatingMode { get; set; }
        
        /// <summary>
        /// The authentication token for the venue 
        /// </summary>
        public  string AuthorizeToken { get; set; }

        /// <summary>
        /// Gets the current Doshii version information.
        /// This method is automatically called and the results logged when this class in instantiated. 
        /// </summary>
        /// <returns></returns>
        protected static string CurrnetVersion()
        {
            var versionStringBuilder = new StringBuilder();
            versionStringBuilder.Append("Doshii Integration Version: ");
            versionStringBuilder.Append(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            versionStringBuilder.Append(Environment.NewLine);
            
            return versionStringBuilder.ToString();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="doshiiInterface">
        /// An implementation of Interfaces.iDoshiiOrdering
        /// </param>
        public DoshiiOperationLogic(Interfaces.iDoshiiOrdering doshiiInterface)
        {
            
            if (doshiiInterface == null)
            {
                throw new NullReferenceException("doshiiInterface is Null");
            }
            m_DoshiiInterface = doshiiInterface;
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: Instantiating Doshii Class");
        }
        
        /// This method MUST be called immediately after this class is instantiated to initialize communication with doshii.
        /// Initializes the WebSockets communications with Doshii,
        /// Initializes the HTTP communications with Doshii,
        /// </summary>
        /// <param name="socketUrl">
        /// The socket URL for communication with doshii 
        /// The socket URL should not end in a '/'
        /// The socket URL must start with either 'ws' or 'wss' - The current Doshii integration uses 'wss'
        /// </param>
        /// <param name="token">
        /// The unique venue authentication token - This can be retrieved from the Doshii web site
        /// </param>
        /// <param name="orderMode">
        /// 1 = Restaurant mode, 
        /// 2 = Bistro mode
        /// </param>
        /// <param name="seatingMode">
        /// 1 = POS Allocation, 
        /// 2 = Doshii Allocation
        /// </param>
        /// <param name="urlBase">
        /// The base URL for communication with the Doshii restful API 
        /// The address should not end in a '/'
        /// Doshii currently uses HTTPS
        /// </param>
        /// <param name="startWebSocketConnection">
        /// Should this instance of the class start the webSocket connection with doshii
        /// There should only be one webSockets connection to Doshii per venue
        /// The webSocket connection is only necessary for the ordering functionality of the Doshii integration and is not necessary for updating the Doshii menu. 
        /// </param>
        public virtual void Initialize(string socketUrl, string token, Enums.OrderModes orderMode, Enums.SeatingModes seatingMode, string urlBase, bool startWebSocketConnection, int timeOutValueSecs)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Info, string.Format("Doshii: Version {5} with; {6} sourceUrl: {0}, {6}token {1}, {6}orderMode {2}, {6}seatingMode: {3},{6}BaseUrl: {4}{6}", socketUrl, token, orderMode.ToString(), seatingMode.ToString(), urlBase, CurrnetVersion(), Environment.NewLine));
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Info, string.Format("Doshii: Version Info: {0}, token {1}, orderMode {2}, seatingMode: {3}, BaseUrl: {4}", socketUrl, token, orderMode.ToString(), seatingMode.ToString(), urlBase));

            if (string.IsNullOrWhiteSpace(socketUrl))
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: Initialization failed - required sockerUrl");
                throw new NotSupportedException("empty socketUrl");
            }

            if (string.IsNullOrWhiteSpace(urlBase))
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: Initialization failed - required urlBase");
                throw new NotSupportedException("empty socketUrl");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: Initialization failed - required token");
                throw new NotSupportedException("token");
            }

            if (timeOutValueSecs <= 0)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: Initialization failed - timeoutvaluesecs must be larger than 0");
                throw new NotSupportedException("timeoutvaluesecs");
            }

            AuthorizeToken = token;
            string socketUrlWithToken = string.Format("{0}?token={1}", socketUrl, token);
            InitializeProcess(socketUrlWithToken, orderMode, seatingMode, urlBase, startWebSocketConnection, timeOutValueSecs);
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// </summary>
        /// <param name="socketUrl"></param>
        /// <param name="orderMode"></param>
        /// <param name="seatingMode"></param>
        /// <param name="UrlBase"></param>
        /// <param name="StartWebSocketConnection"></param>
        /// <returns></returns>
        private bool InitializeProcess(string socketUrl, Enums.OrderModes orderMode, Enums.SeatingModes seatingMode, string UrlBase, bool StartWebSocketConnection, int timeOutValueSecs)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: Initializing Doshii");

            bool result = true;

            OrderMode = orderMode;
            SeatingMode = seatingMode;

            m_HttpComs = new CommunicationLogic.DoshiiHttpCommunication(UrlBase, this, AuthorizeToken);

            if (StartWebSocketConnection)
            {
                try
                {
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format(string.Format("socketUrl = {0}, timeOutValueSecs = {1}", socketUrl, timeOutValueSecs)));
                    m_SocketComs = new CommunicationLogic.DoshiiWebSocketsCommunication(socketUrl, this, timeOutValueSecs);
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format(string.Format("socket Comms are set")));
                    
                    SubscribeToSocketEvents();
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format(string.Format("socket events are subscribed to")));
                    
                    m_SocketComs.Initialize();
                }
                catch (Exception ex)
                {
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format(string.Format("Initializing Doshii failed, there was an exception that was {0}", ex.ToString())));
                }
                
            }
            return result;
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Refreshes the current consumer CheckIns, Allocations and Orders. 
        /// </summary>
        public virtual void RefreshConsumerData()
        {
            if (m_HttpComs.SetSeatingAndOrderConfiguration(SeatingMode, OrderMode))
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: Refreshing consumers, allocation, and orders");
                List<Models.Consumer> currentlyCheckInConsumers = m_DoshiiInterface.GetCheckedInCustomersFromPos();
                
                if (OrderMode == Enums.OrderModes.BistroMode)
                {
                    foreach (Models.Consumer currentConsumer in currentlyCheckInConsumers)
                    {
                        Models.Order consumerOrder = m_DoshiiInterface.GetOrderForCheckinId(currentConsumer.CheckInId);
                        if (consumerOrder != null)
                        {
                            RequestPaymentForOrder(consumerOrder);
                        }
                    }
                }
                currentlyCheckInConsumers = m_DoshiiInterface.GetCheckedInCustomersFromPos();
                List<Models.Consumer> currentlyCheckedInDoshiiConsumers = m_HttpComs.GetConsumers();
                foreach(Models.Consumer doshiiCon in currentlyCheckedInDoshiiConsumers)
                {
                    if (!findCurrentConsumer(currentlyCheckInConsumers, doshiiCon))
                    {
                        CommunicationLogic.CommunicationEventArgs.CheckInEventArgs newCheckinEventArgs = new CommunicationLogic.CommunicationEventArgs.CheckInEventArgs();

                        newCheckinEventArgs.Consumer = m_HttpComs.GetConsumer(doshiiCon.MeerkatConsumerId);

                        newCheckinEventArgs.CheckIn = newCheckinEventArgs.Consumer.CheckInId;
                        newCheckinEventArgs.MeerkatCustomerId = doshiiCon.MeerkatConsumerId;
                        newCheckinEventArgs.Uri = newCheckinEventArgs.Consumer.PhotoUrl;
                        SocketComsConsumerCheckinEventHandler(this, newCheckinEventArgs);
                    }
                }
                List<Models.TableAllocation> initialTableAllocationList = m_HttpComs.GetTableAllocations();
                foreach (Models.TableAllocation ta in initialTableAllocationList)
                {
                    CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs args = new CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs();
                    args.TableAllocation = new Models.TableAllocation();
                    args.TableAllocation.CustomerId = ta.CustomerId;
                    args.TableAllocation.Id = ta.Id;
                    args.TableAllocation.Name = ta.Name;
                    args.TableAllocation.Status = ta.Status;
                    args.TableAllocation.MeerkatConsumerId = ta.MeerkatConsumerId;
                    args.TableAllocation.Checkin = ta.Checkin;

                    SocketComsTableAllocationEventHandler(this, args);
                }
                //remove consumers that are not checked in. 
                foreach (Models.Consumer localCon in currentlyCheckInConsumers)
                {
                    if (!findCurrentConsumer(currentlyCheckedInDoshiiConsumers, localCon))
                    {
                        CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs checkOutEventArgs = new CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs();

                        checkOutEventArgs.MeerkatConsumerId = localCon.MeerkatConsumerId;

                        SocketComsCheckOutEventHandler(this, checkOutEventArgs);
                    }
                }
                // get any pending orders
                List<Models.Order> initialOrderList = m_HttpComs.GetOrders();
                foreach (Models.Order order in initialOrderList)
                {
                    if (order.Status == "pending" || order.Status == "ready to pay" || order.Status == "cancelled")
                    {
                        m_DoshiiInterface.RecordOrderUpdatedAtTime(order);
                        Models.Order orderToConfirm = m_HttpComs.GetOrder(order.Id.ToString());
                        CommunicationLogic.CommunicationEventArgs.OrderEventArgs args = new CommunicationLogic.CommunicationEventArgs.OrderEventArgs();
                        args.Order = orderToConfirm;
                        args.OrderId = orderToConfirm.Id.ToString();
                        args.Status = orderToConfirm.Status;
                        SocketComsOrderStatusEventHandler(this, args);
                    }
                }
            }
            else
            {
                m_SocketComs.CloseSocketConnection();
            }
        }

        /// <summary>
        /// Test if the consumerToFind is in the consumersList
        /// </summary>
        /// <param name="consumersList">
        /// A list of consumers to test for the existence of consumerToFind
        /// </param>
        /// <param name="consumerToFind">
        /// The consumer to find in the list</param>
        /// <returns></returns>
        public virtual bool findCurrentConsumer(List<Models.Consumer> consumersList, Models.Consumer consumerToFind)
        {
            bool consumerFound = false;
            foreach (Models.Consumer localCon in consumersList)
            {
                if (consumerToFind.MeerkatConsumerId == localCon.MeerkatConsumerId)
                {
                    consumerFound = true;
                    break;
                }
            }
            return consumerFound;
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Subscribes to the socket communication events 
        /// </summary>
        private void SubscribeToSocketEvents()
        {
            if (m_SocketComs == null)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: The socketComs has not been initialized");
                throw new NotSupportedException("m_SocketComms is null");
            }
            else
            {
                UnsubscribeFromSocketEvents();
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: Subscribing to socket events");
                m_SocketComs.ConsumerCheckinEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.ConsumerCheckInEventHandler(SocketComsConsumerCheckinEventHandler);
                m_SocketComs.CreateOrderEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.CreatedOrderEventHandler(SocketComsOrderStatusEventHandler);
                m_SocketComs.OrderStatusEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.OrderStatusEventHandler(SocketComsOrderStatusEventHandler);
                m_SocketComs.TableAllocationEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.TableAllocationEventHandler(SocketComsTableAllocationEventHandler);
                m_SocketComs.CheckOutEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.CheckOutEventHandler(SocketComsCheckOutEventHandler);
                m_SocketComs.SocketCommunicationEstablishedEvent += new CommunicationLogic.DoshiiWebSocketsCommunication.SocketCommunicationEstablishedEventHandler(SocketComsConnectionEventHandler);
                m_SocketComs.SocketCommunicationTimeoutReached += new CommunicationLogic.DoshiiWebSocketsCommunication.SocketCommunicationTimeoutReachedEventHandler(ScoketComsTimeOutValueReached);
            }
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Unsubscribes to the socket communication events 
        /// </summary>
        private void UnsubscribeFromSocketEvents()
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: Unsubscribing from socket events");
            m_SocketComs.ConsumerCheckinEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.ConsumerCheckInEventHandler(SocketComsConsumerCheckinEventHandler);
            m_SocketComs.CreateOrderEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.CreatedOrderEventHandler(SocketComsOrderStatusEventHandler);
            m_SocketComs.OrderStatusEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.OrderStatusEventHandler(SocketComsOrderStatusEventHandler);
            m_SocketComs.TableAllocationEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.TableAllocationEventHandler(SocketComsTableAllocationEventHandler);
            m_SocketComs.CheckOutEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.CheckOutEventHandler(SocketComsCheckOutEventHandler);
            m_SocketComs.SocketCommunicationEstablishedEvent -= new CommunicationLogic.DoshiiWebSocketsCommunication.SocketCommunicationEstablishedEventHandler(SocketComsConnectionEventHandler);
            m_SocketComs.SocketCommunicationTimeoutReached -= new CommunicationLogic.DoshiiWebSocketsCommunication.SocketCommunicationTimeoutReachedEventHandler(ScoketComsTimeOutValueReached);
        }
        #endregion

        #region socket communication event handlers

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Handles the consumer checked out message by raising a consumer checkOut event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void SocketComsCheckOutEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.CheckOutEventArgs e)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: received consumer checkout event for consumerId '{0}'", e.MeerkatConsumerId));
            if (!string.IsNullOrWhiteSpace(e.MeerkatConsumerId))
            {
                m_DoshiiInterface.CheckOutConsumer(e.MeerkatConsumerId);
            }
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Handles a socket communication established event and calls refreshComsumerData. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void SocketComsConnectionEventHandler(object sender, EventArgs e)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, "Doshii: received Socket connection event");
            RefreshConsumerData();
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Handles a socket communication timeOut event - this is when there has not been successful communication with doshii within the specified timeout period. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void ScoketComsTimeOutValueReached(object sender, EventArgs e)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, "Doshii: The return property from DoshiiHttpCommuication.MakeRequest was null for method - 'GET' and URL '{0}'");
            m_DoshiiInterface.DissociateDoshiiChecks();
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Handles a SocketComs_TableAllocationEvent established event;
        /// Calls m_DoshiiInterface.ConfirmTableAllocation and accepts or rejects the TableAllocation with Doshii depending on the result 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void SocketComsTableAllocationEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs e)
        {
            Models.TableAllocation tableAllocation = e.TableAllocation;
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: received table allocation event for consumer '{0}' and table '{1}' checkInId '{2}'", e.TableAllocation.MeerkatConsumerId, e.TableAllocation.Name, e.TableAllocation.Id));
            if (m_DoshiiInterface.ConfirmTableAllocation(ref tableAllocation))
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: confirming table allocation for consumer '{0}' and table '{1}' checkInId '{2}'", e.TableAllocation.MeerkatConsumerId, e.TableAllocation.Name, e.TableAllocation.Id));
                try
                {
                    m_HttpComs.PutTableAllocation(tableAllocation.MeerkatConsumerId, tableAllocation.Id);
                }
                catch(Exception ex)
                {
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: There was an exception putting the table allocation to Doshii, {0}", ex));
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: rejecting table allocation for consumer '{0}' and table '{1}' checkInId '{2}' Table is occupied", e.TableAllocation.MeerkatConsumerId, e.TableAllocation.Name, e.TableAllocation.Id));
                    m_HttpComs.RejectTableAllocation(tableAllocation.MeerkatConsumerId, tableAllocation.Id, Enums.TableAllocationRejectionReasons.unknownError);
                }
                
            }
            else if (tableAllocation.rejectionReason == Enums.TableAllocationRejectionReasons.TableDoesNotExist)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: rejecting table allocation for consumer '{0}' and table '{1}' checkInId '{2}' Table dose not exist", e.TableAllocation.MeerkatConsumerId, e.TableAllocation.Name, e.TableAllocation.Id));
                m_HttpComs.RejectTableAllocation(tableAllocation.MeerkatConsumerId, tableAllocation.Id, tableAllocation.rejectionReason);
            }
            else
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: rejecting table allocation for consumer '{0}' and table '{1}' checkInId '{2}' Table is occupied", e.TableAllocation.MeerkatConsumerId, e.TableAllocation.Name, e.TableAllocation.Id));
                m_HttpComs.RejectTableAllocation(tableAllocation.MeerkatConsumerId, tableAllocation.Id, tableAllocation.rejectionReason);
            }
        }


        private void RecordFullCheckPaymentBistroMode(ref Models.Order order)
        {
            if (m_DoshiiInterface.RecordFullCheckPaymentBistroMode(ref order))
            {
                m_DoshiiInterface.CheckOutConsumerWithCheckInId(order.CheckinId);
            }
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Handles a SocketComs_OrderStatusEvent, 
        /// Records the Order.UpdatedAt value and calls the appropriate method on the OrderingInterface to act on the check. 
        /// <exception cref="NotSupportedException">When a partial payment is attempted during Bistro Mode.</exception>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void SocketComsOrderStatusEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.OrderEventArgs e)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: received order status event with status '{0}', for order '{1}'", e.Order.Status, e.Order.ToJsonString()));
            Models.Order returnedOrder = new Models.Order();
            switch (e.Order.Status)
            {
                case "cancelled":
                    m_DoshiiInterface.RecordOrderUpdatedAtTime(e.Order);
                    m_DoshiiInterface.OrderCancled(ref e.Order);
                    break;
                
                case "ready to pay":
                    m_DoshiiInterface.RecordOrderUpdatedAtTime(e.Order);
                    try
                    {
                        m_DoshiiInterface.ConfirmOrderTotalsBeforePaymentRestaurantMode(ref e.Order);
                    }
                    catch(Exceptions.OrderDoesNotExistOnPosException ex)
                    {
                        m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: The order with orderId = {0} does not exist on the pos", e.Order.Id));
                        break;
                    }
                    RequestPaymentForOrder(e.Order);
                    break;
                case "new":
                case "pending":
                    m_DoshiiInterface.RecordOrderUpdatedAtTime(e.Order);    
                    try
                    {
                        if (OrderMode == Enums.OrderModes.BistroMode)
                        {
                            if (m_DoshiiInterface.ConfirmOrderAvailabilityBistroMode(ref e.Order))
                            {
                                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: order availability confirmed for bistroMode - '{0}'", e.Order.ToJsonString()));
                             
                                e.Order.Status = "accepted";
                            
                                    returnedOrder = m_HttpComs.PutOrder(e.Order);
                                    if (returnedOrder.Id == e.Order.Id)
                                    {
                                        returnedOrder.Status = "waiting for payment";
                                        returnedOrder = m_HttpComs.PutOrder(returnedOrder);
                                        if (returnedOrder.Id == e.Order.Id)
                                        {
                                            int nonPayingAmount = 0;
                                            int.TryParse(returnedOrder.NotPayingTotal, out nonPayingAmount);

                                            if (nonPayingAmount > 0)
                                            {
                                                throw new NotSupportedException("Doshii: partial payment in bistro mode is not allowed");
                                            }
                                            else
                                            {
                                                RecordFullCheckPaymentBistroMode(ref returnedOrder);
                                            }
                                        }
                                    }
                            
                            }
                            else
                            {
                                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: order rejected for bistro mode - '{0}'", e.Order.ToJsonString())); 
                                e.Order.Status = "rejected";
                            
                                    m_HttpComs.PutOrder(e.Order); 
                            
                            }
                        }
                        else
                        {
                            if (m_DoshiiInterface.ConfirmOrderForRestaurantMode(ref e.Order))
                            {
                                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: order confirmed for restaurant mode - '{0}'", e.Order.ToJsonString())); 
                                e.Order.Status = "accepted";
                                m_HttpComs.PutOrder(e.Order);
                            
                            }
                            else
                            {
                                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: order rejected for restaurant mode - '{0}'", e.Order.ToJsonString())); 
                                e.Order.Status = "rejected";
                                m_HttpComs.PutOrder(e.Order);
                            }
                        }
                    }
                    catch (Exceptions.NullOrderReturnedException nex)
                    {
                        m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: a Null response was returned during a putOrder for order.Id {0} - {1}", e.Order.Id, nex));
                    }
                    catch (Exceptions.RestfulApiErrorResponseException rex)
                    {
                        m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: a RestfulApiErrorResponseException response was returned during a putOrder for order.Id {0} - {1}", e.Order.Id, rex));
                    }
                    
                    break;
                default:
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: unknown order status - '{0}'", e.Order.ToJsonString())); 
                    throw new NotSupportedException(e.Order.Status.ToString());

            }
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: SocketComsOrderStatusEventHandler has returned"));
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// This method requests a payment from Doshii
        /// calls <see cref="m_DoshiiInterface.CheckOutConsumerWithCheckInId"/> when order update was reject by doshii for a reason that means it should not be retired. 
        /// calls <see cref="m_DoshiiInterface.RecordFullCheckPaymentBistroMode(ref order) "/> 
        /// or <see cref="m_DoshiiInterface.RecordPartialCheckPayment(ref order) "/> 
        /// or <see cref="m_DoshiiInterface.RecordFullCheckPayment(ref order)"/>
        /// to record the payment in the pos. 
        /// It is currently not supported to request a payment from doshii from the pos without first receiving an order with a 'ready to pay' status so this method should not be called directly from the POS
        /// </summary>
        /// <param name="order">
        /// The order that should be paid
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public virtual bool RequestPaymentForOrder(Models.Order order)
        {
            Models.Order returnedOrder = new Models.Order();
            order.Status = "waiting for payment";
            try
            {
                returnedOrder = m_HttpComs.PutOrder(order);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                if (rex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    m_DoshiiInterface.CheckOutConsumerWithCheckInId(order.CheckinId);
                }
            }
            catch (Exceptions.NullOrderReturnedException nex)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: a Null response was returned during a putOrder for order.Id{0}", order.Id));
            }
            catch (Exception ex)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown during a putOrder for order.Id{0} : {1}", order.Id, ex));
            }
            if (returnedOrder.Id == order.Id)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: order put for payment - '{0}'", order.ToJsonString()));
                int nonPayingAmount = 0;
                int.TryParse(order.NotPayingTotal, out nonPayingAmount);

                if (nonPayingAmount > 0)
                {
                    if (OrderMode == Enums.OrderModes.BistroMode)
                    {
                        //this should only happen when the mode has changed to bistro mode and there was an amount already paid on the order because it was previously in restaurant mode. 
                        RecordFullCheckPaymentBistroMode(ref order);
                    }
                    else
                    {
                        if (m_DoshiiInterface.RecordPartialCheckPayment(ref order))
                        {
                            m_DoshiiInterface.CheckOutConsumerWithCheckInId(order.CheckinId);
                        }
                    }
                }
                else
                {
                    if (OrderMode == Enums.OrderModes.BistroMode)
                    {
                        RecordFullCheckPaymentBistroMode(ref order);
                    }
                    else
                    {
                        if (m_DoshiiInterface.RecordFullCheckPayment(ref order))
                        {
                            m_DoshiiInterface.CheckOutConsumerWithCheckInId(order.CheckinId);
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Handles the SocketComs_ConsumerCheckinEvent and records the checked in user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void SocketComsConsumerCheckinEventHandler(object sender, CommunicationLogic.CommunicationEventArgs.CheckInEventArgs e)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: checkIn event received for consumer - '{0}' with id '{1}'", e.Consumer.Name, e.Consumer.MeerkatConsumerId));
            m_DoshiiInterface.RecordCheckedInUser(ref e.Consumer);
        }

        #endregion
        
        #region product sync methods

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// This method will return all the products currently in Doshii, if the request fails the failure message will be logged and the products list will be empty. 
        /// </summary>
        /// <returns></returns>
        public virtual List<Models.Product> GetAllProducts()
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos requesting all doshii products")); 
            return m_HttpComs.GetDoshiiProducts();
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// Adds a list of products to Doshii
        /// </summary>
        /// <param name="productList"></param>
        /// <returns></returns>
        public virtual void AddNewProducts(List<Models.Product> productList)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos adding new product list- '{0}'", JsonConvert.SerializeObject(productList))); 
            try
            {
                m_HttpComs.PostProductData(productList, false);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                if (rex.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw new Exceptions.ProductNotCreatedException();
                }
                else
                {
                    throw rex;
                }
            }
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// updates a product in doshii.
        /// </summary>
        /// <param name="productToUpdate"></param>
        /// <returns></returns>
        public virtual void UpdateProcuct(Models.Product productToUpdate)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos updating product - '{0}'", productToUpdate.ToJsonStringForProductSync()));
            try
            {
                m_HttpComs.PutProductData(productToUpdate);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }
        
        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// deletes the provided list of products from doshii. 
        /// </summary>
        /// <param name="productList"></param>
        /// <returns></returns>
        public virtual void DeleteProducts(List<string> productIdList)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos deleting product list- '{0}'", JsonConvert.SerializeObject(productIdList)));
            try
            {
                foreach (string pro in productIdList)
                {
                    DeleteProduct(pro);
                }
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// delete a product from doshii
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public virtual void DeleteProduct(string productId)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos deleting product from doshii - '{0}'", productId));
            try
            {
                m_HttpComs.DeleteProductData(productId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
        }

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// deletes all the products from doshii
        /// </summary>
        /// <returns></returns>
        public virtual void DeleteAllProducts()
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos deleting all products from doshii"));
            try
            {
                m_HttpComs.DeleteProductData();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        #endregion

        #region ordering And Payment

        /// <summary>
        /// DO NOT USE, this method is for internal use only
        /// gets an order from doshii with the provided orderId, if there no order is no order matching the provided orderId on doshii a new order is returned. 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public virtual Models.Order GetOrder(string orderId)
        {
            try
            {
                return m_HttpComs.GetOrder(orderId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
        }


        /// <summary>
        /// adds items to a doshii order
        /// </summary>
        /// <param name="order">
        /// The order must contain all the products included in the check as this method overwrites all the items recorded on doshii for this check. 
        /// not tested
        /// </param>
        /// <returns></returns>
        public virtual Models.Order UpdateOrder(Models.Order order)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos updating order - '{0}'", order.ToJsonString()));
            if (OrderMode == Enums.OrderModes.BistroMode)
            {
                m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: cannot add products to order in bistro mode"));
                throw new NotSupportedException("Doshii: cannot add products to order in bistro mode");
            }
            if (order.Status != "paid")
            {
                order.Status = "accepted";
            }
            Models.Order returnedOrder = new Models.Order();
            if (order.Id == 0)
            {
                order.UpdatedAt = DateTime.Now.ToString();
                try
                {
                    returnedOrder = m_HttpComs.PostOrder(order);
                    if (returnedOrder.Id == 0)
                    {
                        m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: order was returned from doshii without an orderId"));
                    }
                }
                catch (Exceptions.RestfulApiErrorResponseException rex)
                {
                    if (rex.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        throw new Exceptions.ConflictWithOrderUpdateException(string.Format("There was a conflict updating order.id {0}", order.Id.ToString()));
                    }
                    if (rex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        m_DoshiiInterface.CheckOutConsumerWithCheckInId(order.CheckinId);
                    }
                    throw rex;
                }
                catch (Exceptions.NullOrderReturnedException nex)
                {
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: a Null response was returned during a PostOrder for order.CheckinId {0}", order.CheckinId));
                }
                catch (Exception ex)
                {
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown during a putOrder for order.CheckinId {0} : {1}", order.CheckinId, ex));
                }
                
                // record the order.Id in the pos so the post can put another order if more items are added from the pos. 
                m_DoshiiInterface.RecordOrderId(returnedOrder);
            }
            else
            {
                try
                {
                    returnedOrder = m_HttpComs.PutOrder(order);
                    if (returnedOrder.Id == 0)
                    {
                        m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Warning, string.Format("Doshii: order was returned from doshii without an orderId"));
                    }
                }
                catch (Exceptions.RestfulApiErrorResponseException rex)
                {
                    if (rex.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        throw new Exceptions.ConflictWithOrderUpdateException(string.Format("There was a conflict updating order.id {0}", order.Id.ToString()));
                    }
                    if (rex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        m_DoshiiInterface.CheckOutConsumerWithCheckInId(order.CheckinId);
                    }
                    throw rex;
                }
                catch (Exceptions.NullOrderReturnedException nex)
                {
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: a Null response was returned during a putOrder for order.Id{0}", order.Id));
                }
                catch (Exception ex)
                {
                    m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown during a putOrder for order.Id{0} : {1}", order.Id, ex));
                }
            }

            return returnedOrder;
        }

        #endregion

        #region tableAllocation and consumers

        /// <summary>
        /// gets the consumer from doshii with the provided meerkatCustomerId
        /// </summary>
        /// <param name="meerkatCustomerId"></param>
        /// <returns></returns>
        public virtual Models.Consumer GetConsumer(string meerkatCustomerId)
        {
            try
            {
                return m_HttpComs.GetConsumer(meerkatCustomerId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// attempts to add a table allocation to doshii
        /// </summary>
        /// <param name="customerId">
        /// the payPayCustomerId for the customer to be allocated. 
        /// </param>
        /// <param name="tableName">
        /// the name of the table to be allocated.
        /// </param>
        /// <returns></returns>
        public virtual void SetTableAllocation(string customerId, string tableName)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos allocating table for customerId - '{0}', table '{1}'", customerId, tableName));
            try
            {
                m_HttpComs.PostTableAllocation(customerId, tableName);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
        }

        /// <summary>
        /// Deletes a table Allocation from Doshii
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="tableName"></param>
        /// <param name="deleteReason"></param>
        public virtual void DeleteTableAllocation(string customerId, string tableName, Enums.TableAllocationRejectionReasons deleteReason)
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos DeAllocating table for customerId - '{0}', table '{1}'", customerId, tableName));
            try
            {
                m_HttpComs.RejectTableAllocation(customerId, tableName, deleteReason);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        #endregion

        /// <summary>
        /// returns a list of all the consumers currently in doshii. 
        /// </summary>
        /// <returns></returns>
        public virtual List<Models.Consumer> GetCheckedInConsumersFromDoshii()
        {
            m_DoshiiInterface.LogDoshiiMessage(Enums.DoshiiLogLevels.Debug, string.Format("Doshii: pos requesting all checked in users"));
            try
            {
                return m_HttpComs.GetConsumers();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
            
        }

    }
}
