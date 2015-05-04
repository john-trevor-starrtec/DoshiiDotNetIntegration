using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using System.Net;
using DoshiiDotNetIntegration;
using DoshiiDotNetIntegration.Exceptions;
using Newtonsoft;

namespace DoshiiDotNetSDKTests
{
    [TestFixture]
    public class HttpCommunicationTests
    {
        DoshiiOperationLogic operationLogic;
        DoshiiDotNetIntegration.Interfaces.iDoshiiOrdering orderingInterface;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication HttpComs;
        DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication MockHttpComs;

        [SetUp]
        public void Init()
        {
            orderingInterface = MockRepository.GenerateMock<DoshiiDotNetIntegration.Interfaces.iDoshiiOrdering>();
            operationLogic = MockRepository.GenerateMock<DoshiiOperationLogic>(orderingInterface);
            MockHttpComs = MockRepository.GeneratePartialMock<DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication>(GenerateObjectsAndStringHelper.TestBaseUrl, operationLogic, GenerateObjectsAndStringHelper.TestToken);
            HttpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication(GenerateObjectsAndStringHelper.TestBaseUrl, operationLogic, GenerateObjectsAndStringHelper.TestToken);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Constructor_NoUrl()
        {
            var httpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication("", operationLogic, GenerateObjectsAndStringHelper.TestToken);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Constructor_OperationLogic()
        {
            var httpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication(GenerateObjectsAndStringHelper.TestBaseUrl, null, GenerateObjectsAndStringHelper.TestToken);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Constructor_NoToken()
        {
            var httpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication(GenerateObjectsAndStringHelper.TestBaseUrl, operationLogic, "");
        }

        [Test]
        public void Constructor_AllParamatersCorrect()
        {
            var httpComs = new DoshiiDotNetIntegration.CommunicationLogic.DoshiiHttpCommunication(GenerateObjectsAndStringHelper.TestBaseUrl, operationLogic, GenerateObjectsAndStringHelper.TestToken);
            Assert.AreEqual(httpComs.m_Token, GenerateObjectsAndStringHelper.TestToken);
            Assert.AreEqual(httpComs.m_DoshiiLogic, operationLogic);
            Assert.AreEqual(httpComs.m_DoshiiUrlBase, GenerateObjectsAndStringHelper.TestBaseUrl);
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetConsumer_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();
            
            MockHttpComs.GetConsumer(GenerateObjectsAndStringHelper.TestCustomerId);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void GetConsumer_ReturnedConumerIsRetreivedConsumer()
        {
            var consumerInput = GenerateObjectsAndStringHelper.GenerateConsumer1();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageConsumerSuccess();
            responseMessage.Data = consumerInput.ToJsonString();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            
            var consumerResponse = MockHttpComs.GetConsumer(GenerateObjectsAndStringHelper.TestCustomerId);
            
            //MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(consumerResponse.Id , consumerInput.Id);
            Assert.AreEqual(consumerResponse.Name, consumerInput.Name);
            Assert.AreEqual(consumerResponse.MeerkatConsumerId, consumerInput.MeerkatConsumerId);
            Assert.AreEqual(consumerResponse.PhotoUrl, consumerInput.PhotoUrl);
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetConsumers_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.GetConsumers();

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void GetConsumers_ReturnedConumersAreRetreivedConsumer()
        {
            var consumerListInput = GenerateObjectsAndStringHelper.GenerateConsumerList();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageConsumersSuccess();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(consumerListInput);
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var consumerResponse = MockHttpComs.GetConsumers();

            //MockHttpComs.VerifyAllExpectations();
            Assert.AreEqual(consumerListInput[0].Id, consumerResponse[0].Id);
            Assert.AreEqual(consumerListInput[1].Id, consumerResponse[1].Id);
            Assert.AreEqual(consumerListInput[2].Id, consumerResponse[2].Id);
            Assert.AreEqual(consumerListInput[3].Id, consumerResponse[3].Id);
            
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetOrder_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.GetOrder(GenerateObjectsAndStringHelper.TestOrderId.ToString());

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void GetOrder_ReturnedConumersAreRetreivedConsumer()
        {
            var orderInput = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageOrderSuccess();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var orderrResponse = MockHttpComs.GetOrder(orderInput.Id.ToString());

            Assert.AreEqual(orderInput.Id, orderrResponse.Id);
        }
        
        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetOrders_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.GetOrders();

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void GetOrders_ReturnedOrdersAreRetreivedOrders()
        {
            var orderListInput = GenerateObjectsAndStringHelper.GenerateOrderList();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageOrdersSuccess();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var orderResponse = MockHttpComs.GetOrders();

            Assert.AreEqual(orderListInput[0].Id, orderResponse[0].Id);
            Assert.AreEqual(orderListInput[1].Id, orderResponse[1].Id);
            Assert.AreEqual(orderListInput[2].Id, orderResponse[2].Id);
            Assert.AreEqual(orderListInput[3].Id, orderResponse[3].Id);
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetTableAllocations_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.GetTableAllocations();

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void GetTableAllocations_ReturnedTableAllocationsAreRetreivedTableAllocations()
        {
            var orderListInput = GenerateObjectsAndStringHelper.GenerateTableAllocationList();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageTableAllocationSuccess();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var tableAllocationResponse = MockHttpComs.GetTableAllocations();

            Assert.AreEqual(orderListInput[0].Id, tableAllocationResponse[0].Id);
            Assert.AreEqual(orderListInput[1].Id, tableAllocationResponse[1].Id);
            
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void PutTableAllocaiton_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.PutTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PutTableAllocaiton_SuccessfulPut_ReturnsTrue()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationSuccess();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var success = MockHttpComs.PutTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName);

            Assert.AreEqual(success, true);
        }

        [Test]
        public void PutTableAllocaiton_FailedPut_ReturnsFalse()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var success = MockHttpComs.PutTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName);

            Assert.AreEqual(success, false);
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void PostTableAllocaiton_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.PostTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PostTableAllocaiton_SuccessfulPut_ReturnsTrue()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationSuccess();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var success = MockHttpComs.PostTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName);

            Assert.AreEqual(success, true);
        }

        [Test]
        public void PostTableAllocaiton_FailedPut_ReturnsFalse()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var success = MockHttpComs.PostTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName);

            Assert.AreEqual(success, false);
        }

        [Test]
        public void PostTableAllocaiton_GeneratesCorrectStringData()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{\"tableName\": \"");
            builder.AppendFormat("{0}", GenerateObjectsAndStringHelper.TestTableName);
            builder.Append("\"}");

            MockHttpComs.Expect(x => x.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.AddTableAllocation, GenerateObjectsAndStringHelper.TestCustomerId)).Return(string.Format("/consumers/{0}/table", GenerateObjectsAndStringHelper.TestCustomerId));
            MockHttpComs.Expect(x => x.MakeRequest(string.Format("/consumers/{0}/table", GenerateObjectsAndStringHelper.TestCustomerId), "POST", builder.ToString())).Return(GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationSuccess());

            MockHttpComs.PostTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void DeleteTableAllocationWithCheckInId_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.DeleteTableAllocationWithCheckInId(GenerateObjectsAndStringHelper.TestCheckinId, DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void DeleteTableAllocationWithCheckInId_SuccessfulDelete_ReturnsTrue()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationSuccess();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var success = MockHttpComs.DeleteTableAllocationWithCheckInId(GenerateObjectsAndStringHelper.TestCheckinId, DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos);

            Assert.AreEqual(success, true);
        }

        [Test]
        public void DeleteTableAllocationWithCheckInId_FailedDelete_ReturnsFalse()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var success = MockHttpComs.DeleteTableAllocationWithCheckInId(GenerateObjectsAndStringHelper.TestCheckinId, DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos);

            Assert.AreEqual(success, false);
        }

        [Test]
        public void DeleteTableAllocationWithCheckInId_NullResponce()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);

            var success = MockHttpComs.DeleteTableAllocationWithCheckInId(GenerateObjectsAndStringHelper.TestCheckinId, DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos);

            Assert.AreEqual(success, false);
        }
        
        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void SetSeatingAndOrderConfiguration_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.SetSeatingAndOrderConfiguration(DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation, DoshiiDotNetIntegration.Enums.OrderModes.BistroMode);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SetSeatingAndOrderConfiguration_SuccessfulPut_ReturnsTrue()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationSuccess();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var success = MockHttpComs.SetSeatingAndOrderConfiguration(DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation, DoshiiDotNetIntegration.Enums.OrderModes.BistroMode);

            Assert.AreEqual(success, true);
        }

        [Test]
        public void SetSeatingAndOrderConfiguration_FailedPut_ReturnsFalse()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var success = MockHttpComs.SetSeatingAndOrderConfiguration(DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation, DoshiiDotNetIntegration.Enums.OrderModes.BistroMode);

            Assert.AreEqual(success, false);
        }

        [Test]
        public void SetSeatingAndOrderConfiguration_NullResponce()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);

            var success = MockHttpComs.SetSeatingAndOrderConfiguration(DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation, DoshiiDotNetIntegration.Enums.OrderModes.BistroMode);

            Assert.AreEqual(success, false);
        }

        [Test]
        public void SetSeatingAndOrderConfiguration_GeneratesCorrectStringData_DoshiiAllocation_Bistro()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder configString = new StringBuilder();
            configString.Append("{ \"restaurantMode\":");
            configString.Append(" \"bistro\",");
            configString.Append(" \"tableMode\": ");
            configString.Append(" \"selection\" }");

            MockHttpComs.Expect(x => x.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.SetSeatingAndOrderConfiguration)).Return("/config");
            MockHttpComs.Expect(x => x.MakeRequest("/config", "PUT", configString.ToString())).Return(GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationSuccess());

            MockHttpComs.SetSeatingAndOrderConfiguration(DoshiiDotNetIntegration.Enums.SeatingModes.DoshiiAllocation, DoshiiDotNetIntegration.Enums.OrderModes.BistroMode);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SetSeatingAndOrderConfiguration_GeneratesCorrectStringData_PosAllocation_Resturant()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder configString = new StringBuilder();
            configString.Append("{ \"restaurantMode\":");
            configString.Append(" \"restaurant\",");
            configString.Append(" \"tableMode\": ");
            configString.Append(" \"allocation\" }");

            MockHttpComs.Expect(x => x.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.SetSeatingAndOrderConfiguration)).Return("/config");
            MockHttpComs.Expect(x => x.MakeRequest("/config", "PUT", configString.ToString())).Return(GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationSuccess());

            MockHttpComs.SetSeatingAndOrderConfiguration(DoshiiDotNetIntegration.Enums.SeatingModes.PosAllocation, DoshiiDotNetIntegration.Enums.OrderModes.RestaurantMode);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void SerializeTableDeAllocationRejectionReason_TableDoesNotExist()
        {
            string reasonCodeString = HttpComs.SerializeTableDeAllocationRejectionReason(DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.TableDoesNotExist);

            Assert.AreEqual(reasonCodeString, "{\"reasonCode\" : \"1\"}");
            
        }

        [Test]
        public void SerializeTableDeAllocationRejectionReason_TableIsOccupied()
        {
            string reasonCodeString = HttpComs.SerializeTableDeAllocationRejectionReason(DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.TableIsOccupied);

            Assert.AreEqual(reasonCodeString, "{\"reasonCode\" : \"2\"}");
            
        }

        [Test]
        public void SerializeTableDeAllocationRejectionReason_CheckinWasDeallocatedByPos()
        {
            string reasonCodeString = HttpComs.SerializeTableDeAllocationRejectionReason(DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos);

            Assert.AreEqual(reasonCodeString, "{\"reasonCode\" : \"3\"}");
            
        }

        [Test]
        public void SerializeTableDeAllocationRejectionReason_ConcurrencyIssueWithPos()
        {
            string reasonCodeString = HttpComs.SerializeTableDeAllocationRejectionReason(DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.ConcurrencyIssueWithPos);

            Assert.AreEqual(reasonCodeString, "{\"reasonCode\" : \"4\"}");
            
        }

        [Test]
        public void SerializeTableDeAllocationRejectionReason_tableDoesNotHaveATab()
        {
            string reasonCodeString = HttpComs.SerializeTableDeAllocationRejectionReason(DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.tableDoesNotHaveATab);

            Assert.AreEqual(reasonCodeString, "{\"reasonCode\" : \"5\"}");
            
        }

        [Test]
        public void SerializeTableDeAllocationRejectionReason_tableHasBeenPaid()
        {
            string reasonCodeString = HttpComs.SerializeTableDeAllocationRejectionReason(DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.tableHasBeenPaid);

            Assert.AreEqual(reasonCodeString, "{\"reasonCode\" : \"6\"}");
            
        }

        [Test]
        public void SerializeTableDeAllocationRejectionReason_unknownError()
        {
            string reasonCodeString = HttpComs.SerializeTableDeAllocationRejectionReason(DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.unknownError);

            Assert.AreEqual(reasonCodeString, "{\"reasonCode\" : \"7\"}");
            
        }

        [Test]
        public void RejectTableAllocation_MakeRequestThrowsException_NoExceptionThrown()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.RejectTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName, DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void RejectTableAllocation_SuccessfulDelete_ReturnsTrue()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationSuccess();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var success = MockHttpComs.RejectTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName, DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos);

            Assert.AreEqual(success, true);
        }

        [Test]
        public void RejectTableAllocation_FailedDelete_ReturnsFalse()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var success = MockHttpComs.RejectTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName, DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos);

            Assert.AreEqual(success, false);
        }

        [Test]
        public void RejectTableAllocation_NullResponce()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);

            var success = MockHttpComs.RejectTableAllocation(GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName, DoshiiDotNetIntegration.Enums.TableAllocationRejectionReasons.CheckinWasDeallocatedByPos);

            Assert.AreEqual(success, false);
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void PutOrder_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.PutOrder(GenerateObjectsAndStringHelper.GenerateOrderAccepted());

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PutOrder_SuccessfullResponse_ReturnsResponseOrder()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageSuccessfulOrder();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var responseOrder = MockHttpComs.PutOrder(order);

            Assert.AreEqual(responseOrder.Id, order.Id);
        }

        [Test]
        public void PutOrder_OrderToPutStatus_EqualsOrderStatus_Accepted()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageSuccessfulOrder();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Matches(data => data.Contains("accepted")))).Return(responseMessage);

            MockHttpComs.PutOrder(order);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PutOrder_OrderToPutStatus_EqualsOrderStatus_Paid()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderPaid();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageSuccessfulOrder();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Matches(data => data.Contains("paid")))).Return(responseMessage);

            MockHttpComs.PutOrder(order);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PutOrder_OrderToPutStatus_EqualsOrderStatus_WaitingForPayment()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageSuccessfulOrder();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Matches(data => data.Contains("waiting for payment")))).Return(responseMessage);

            MockHttpComs.PutOrder(order);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PutOrder_OrderToPutStatus_EqualsOrderStatus_Rejected()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderReadyToPay();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageSuccessfulOrder();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Matches(data => data.Contains("rejected")))).Return(responseMessage);

            MockHttpComs.PutOrder(order);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.NullOrderReturnedException))]
        public void PutOrder_NullOrderReturned_ExceptionThrown()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderReadyToPay();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageSuccessfulOrder();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);

            MockHttpComs.PutOrder(order);

            MockHttpComs.VerifyAllExpectations();
        }
        
        [Test]
        public void PutOrder_SuccessfulResponse_RecordOrderUpdatedAtTime()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageOrderSuccess();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            MockHttpComs.m_DoshiiLogic.m_DoshiiInterface.Expect(x => x.RecordOrderUpdatedAtTime(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == order.Status && y.Id == order.Id))).Repeat.Once();
            
            MockHttpComs.PutOrder(order);

            MockHttpComs.VerifyAllExpectations();
            MockHttpComs.m_DoshiiLogic.m_DoshiiInterface.VerifyAllExpectations();
        }

        [Test]
        public void PutOrder_SuccessfullResponse_ResponseDataIsNull_DoesNotCallRecordOrderUpdatedAtTime()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageOrderSuccess();
            responseMessage.Data = "";
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            MockHttpComs.m_DoshiiLogic.m_DoshiiInterface.Expect(x => x.RecordOrderUpdatedAtTime(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == order.Status && y.Id == order.Id))).Repeat.Never();

            MockHttpComs.PutOrder(order);

            MockHttpComs.VerifyAllExpectations();
            MockHttpComs.m_DoshiiLogic.m_DoshiiInterface.VerifyAllExpectations();
        }

        ///
        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void PostOrder_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.PostOrder(GenerateObjectsAndStringHelper.GenerateOrderAccepted());

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PostOrder_SuccessfullResponse_ReturnsResponseOrder()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageSuccessfulOrder();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var responseOrder = MockHttpComs.PostOrder(order);

            Assert.AreEqual(responseOrder.Id, order.Id);
        }

        [Test]
        public void PostOrder_OrderToPutStatus_EqualsOrderStatus_Accepted()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageSuccessfulOrder();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Matches(data => data.Contains("accepted")))).Return(responseMessage);

            MockHttpComs.PostOrder(order);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PostOrder_OrderToPutStatus_EqualsOrderStatus_Paid()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderPaid();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageSuccessfulOrder();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Matches(data => data.Contains("paid")))).Return(responseMessage);

            MockHttpComs.PostOrder(order);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PostOrder_OrderToPutStatus_EqualsOrderStatus_WaitingForPayment()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderWaitingForPayment();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageSuccessfulOrder();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Matches(data => data.Contains("waiting for payment")))).Return(responseMessage);

            MockHttpComs.PostOrder(order);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PostOrder_OrderToPutStatus_EqualsOrderStatus_Rejected()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderReadyToPay();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageSuccessfulOrder();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Matches(data => data.Contains("rejected")))).Return(responseMessage);

            MockHttpComs.PostOrder(order);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.NullOrderReturnedException))]
        public void PostOrder_NullOrderReturned_ExceptionThrown()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderReadyToPay();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageSuccessfulOrder();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);

            MockHttpComs.PostOrder(order);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PostOrder_SuccessfulResponse_RecordOrderUpdatedAtTime()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageOrderSuccess();
            responseMessage.Data = order.ToJsonString();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            MockHttpComs.m_DoshiiLogic.m_DoshiiInterface.Expect(x => x.RecordOrderUpdatedAtTime(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == order.Status && y.Id == order.Id))).Repeat.Once();

            MockHttpComs.PostOrder(order);

            MockHttpComs.VerifyAllExpectations();
            MockHttpComs.m_DoshiiLogic.m_DoshiiInterface.VerifyAllExpectations();
        }

        [Test]
        public void PostOrder_SuccessfullResponse_ResponseDataIsNull_DoesNotCallRecordOrderUpdatedAtTime()
        {
            var order = GenerateObjectsAndStringHelper.GenerateOrderAccepted();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageOrderSuccess();
            responseMessage.Data = "";
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);
            MockHttpComs.m_DoshiiLogic.m_DoshiiInterface.Expect(x => x.RecordOrderUpdatedAtTime(Arg<DoshiiDotNetIntegration.Models.Order>.Matches(y => y.Status == order.Status && y.Id == order.Id))).Repeat.Never();

            MockHttpComs.PostOrder(order);

            MockHttpComs.VerifyAllExpectations();
            MockHttpComs.m_DoshiiLogic.m_DoshiiInterface.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void GetDoshiiProducts_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.GetDoshiiProducts();

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void GetDoshiiProducts_ReturnedProductsAreRetreivedProducts()
        {
            var productListOutput = GenerateObjectsAndStringHelper.GenerateProductList();
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageTableAllocationSuccess();
            responseMessage.Data = Newtonsoft.Json.JsonConvert.SerializeObject(productListOutput);
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var productList = MockHttpComs.GetDoshiiProducts();

            Assert.AreEqual(productListOutput[0].PosId, productList[0].PosId);
            Assert.AreEqual(productListOutput[1].PosId, productList[1].PosId);
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void DeleteDoshiiProduct_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.DeleteProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions().Id);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void DeleteDoshiiProduct_SuccessfullDelete_ReturnsTrue()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessageOrderSuccess();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var success = MockHttpComs.DeleteProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions().Id);

            Assert.AreEqual(success, true);
        }

        [Test]
        public void DeleteDoshiiProduct_WebException_ReturnsFalse()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationSuccess();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new WebException()).Repeat.Once();

            var success = MockHttpComs.DeleteProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions().Id);

            Assert.AreEqual(success, false);
        }

        [Test]
        public void DeleteDoshiiProduct_Exception_ReturnsFalse()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationSuccess();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new Exception()).Repeat.Once();

            var success = MockHttpComs.DeleteProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions().Id);

            Assert.AreEqual(success, false);
        }

        [Test]
        public void DeleteDoshiiProduct_FailedDelete_ReturnsFalse()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(responseMessage);

            var success = MockHttpComs.DeleteProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions().Id);

            Assert.AreEqual(success, false);
        }

        [Test]
        public void DeleteDoshiiProduct_NullResponse_ReturnsFalse()
        {
            MockHttpComs.Stub(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(null);

            var success = MockHttpComs.DeleteProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions().Id);

            Assert.AreEqual(success, false);
        }

        ///
        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void PostProductData_MakeRequestThrowsException_NotNewItem()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.PostProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions(), false);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void PostProductData_MakeRequestThrowsException_NewItem()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.PostProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions(), true);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PostProductData_PostRequestFails_PutRequestSuccess()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Matches(str => str.Contains("PUT")), Arg<String>.Is.Anything)).Return(responseMessage).Repeat.Once();
            MockHttpComs.Expect(x => x.PutProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions())).IgnoreArguments().Return(true);

            var success = MockHttpComs.PostProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions(), false);
            Assert.AreEqual(success, true);
        }

        [Test]
        public void PostProductData_PostRequestFails_PutRequestFails()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Matches(str => str.Contains("PUT")), Arg<String>.Is.Anything)).Return(responseMessage).Repeat.Once();
            MockHttpComs.Expect(x => x.PutProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions())).IgnoreArguments().Return(false);

            var success = MockHttpComs.PostProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions(), false);
            Assert.AreEqual(success, false);
        }

        [Test]
        public void PostProductData_PostRequestFails_InternalServiceError()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            responseMessage.Status = HttpStatusCode.InternalServerError;
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Matches(str => str.Contains("POST")), Arg<String>.Is.Anything)).Return(responseMessage).Repeat.Once();
            MockHttpComs.Expect(x => x.PutProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions())).IgnoreArguments().Return(true);

            var success = MockHttpComs.PostProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions(), true);
            Assert.AreEqual(success, true);
        }

        [Test]
        public void PostProductData_PostRequestSuccess()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationSuccess();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Matches(str => str.Contains("POST")), Arg<String>.Is.Anything)).Return(responseMessage).Repeat.Once();
            
            var success = MockHttpComs.PostProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions(), true);
            Assert.AreEqual(success, true);
        }

        [Test]
        public void PostProductData_PostRequestFailure()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            responseMessage.Status = HttpStatusCode.Conflict;
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Matches(str => str.Contains("POST")), Arg<String>.Is.Anything)).Return(responseMessage).Repeat.Once();

            var success = MockHttpComs.PostProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions(), true);
            Assert.AreEqual(success, false);
        }

        [Test]
        public void PostProductData_PostRequestFailure_NullResponse()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            responseMessage.Status = HttpStatusCode.Conflict;
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Matches(str => str.Contains("POST")), Arg<String>.Is.Anything)).Return(null).Repeat.Once();

            var success = MockHttpComs.PostProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions(), true);
            Assert.AreEqual(success, false);
        }
        ///
        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void PostProductData_List_MakeRequestThrowsException()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.PostProductData(GenerateObjectsAndStringHelper.GenerateProductList(), false);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PostProductData_List_DeleteAllProducts_MakeRequestThrowsException_NewItem()
        {
            MockHttpComs.Expect(x => x.DeleteProductData()).Return(true).Repeat.Once(); 
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Return(GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationSuccess()).Repeat.Once();

            MockHttpComs.PostProductData(GenerateObjectsAndStringHelper.GenerateProductList(), true);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PostProductData_List_MakeRequest_DataStringIsJsonOfProductList()
        {
            var productList = GenerateObjectsAndStringHelper.GenerateProductList();
            MockHttpComs.Expect(x => x.MakeRequest(string.Format("{0}/products",GenerateObjectsAndStringHelper.TestBaseUrl), "POST", Newtonsoft.Json.JsonConvert.SerializeObject(productList))).Return(GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationSuccess());

            MockHttpComs.PostProductData(productList, false);

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PostProductData_List_MakeRequest_Success()
        {
            var response = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            var productList = GenerateObjectsAndStringHelper.GenerateProductList();
            response.Data = Newtonsoft.Json.JsonConvert.SerializeObject(productList);
            response.Status = HttpStatusCode.Created;
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).Return(response);

            var success = MockHttpComs.PostProductData(productList, false);

            Assert.AreEqual(true, success);
        }

        [Test]
        public void PostProductData_List_MakeRequest_Failure()
        {
            var response = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure(); 
            var productList = GenerateObjectsAndStringHelper.GenerateProductList();
            response.Data = Newtonsoft.Json.JsonConvert.SerializeObject(productList);
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).Return(response);

            var success = MockHttpComs.PostProductData(productList, false);

            Assert.AreEqual(success, false);
        }

        [Test]
        public void PostProductData_List_MakeRequest_NullResponse()
        {
            var response = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            var productList = GenerateObjectsAndStringHelper.GenerateProductList();
            response.Data = Newtonsoft.Json.JsonConvert.SerializeObject(productList);
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).Return(null);

            var success = MockHttpComs.PostProductData(productList, false);

            Assert.AreEqual(success, false);
        }
        ///
        [Test]
        [ExpectedException(typeof(DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException))]
        public void PutProductData_MakeRequestThrowsException_NewItem()
        {
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).IgnoreArguments().Throw(new DoshiiDotNetIntegration.Exceptions.RestfulApiErrorResponseException()).Repeat.Once();

            MockHttpComs.PutProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions());

            MockHttpComs.VerifyAllExpectations();
        }

        [Test]
        public void PutProductData_PostRequestFails_PutRequestSuccess()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Matches(str => str.Contains("PUT")), Arg<String>.Is.Anything)).Return(responseMessage).Repeat.Once();
            MockHttpComs.Expect(x => x.PutProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions())).IgnoreArguments().Return(true);

            var success = MockHttpComs.PutProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions());
            Assert.AreEqual(success, true);
        }

        [Test]
        public void PutProductData_PostRequestFailure()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            responseMessage.Status = HttpStatusCode.Conflict;
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).Return(responseMessage).Repeat.Once();

            var success = MockHttpComs.PutProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions());
            Assert.AreEqual(success, false);
        }

        [Test]
        public void PutProductData_PostRequestFailure_NullResponse()
        {
            var responseMessage = GenerateObjectsAndStringHelper.GenerateResponceMessagePutTableAllocationFailure();
            responseMessage.Status = HttpStatusCode.Conflict;
            MockHttpComs.Expect(x => x.MakeRequest(Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).Return(null).Repeat.Once();

            var success = MockHttpComs.PutProductData(GenerateObjectsAndStringHelper.GenerateProduct1WithOptions());
            Assert.AreEqual(success, false);
        }

        [Test]
        public void GenerateUrl_Products()
        {
            var urlString = HttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Products);

            Assert.AreEqual(string.Format("{0}/products", GenerateObjectsAndStringHelper.TestBaseUrl), urlString);
        }

        [Test]
        public void GenerateUrl_Product()
        {
            var urlString = HttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Products, GenerateObjectsAndStringHelper.TestProductId);

            Assert.AreEqual(string.Format("{0}/products/{1}", GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestProductId), urlString);
        }

        [Test]
        public void GenerateUrl_Orders()
        {
            var urlString = HttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Order);

            Assert.AreEqual(string.Format("{0}/orders", GenerateObjectsAndStringHelper.TestBaseUrl), urlString);
        }

        [Test]
        public void GenerateUrl_Order()
        {
            var urlString = HttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Order, GenerateObjectsAndStringHelper.TestOrderId.ToString());

            Assert.AreEqual(string.Format("{0}/orders/{1}", GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestOrderId.ToString()), urlString);
        }

        [Test]
        public void GenerateUrl_TableAllocations()
        {
            var urlString = HttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.GetTableAllocations);

            Assert.AreEqual(string.Format("{0}/tables", GenerateObjectsAndStringHelper.TestBaseUrl), urlString);
        }

        [Test]
        public void GenerateUrl_TableAllocation()
        {
            var urlString = HttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.GetTableAllocations, GenerateObjectsAndStringHelper.TestTableAllocationName);

            Assert.AreEqual(string.Format("{0}/tables/{1}", GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestTableAllocationName), urlString);
        }

        [Test]
        public void GenerateUrl_ConfirmTableAllocation()
        {
            var urlString = HttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.ConfirmTableAllocation, GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName);

            Assert.AreEqual(string.Format("{0}/consumers/{1}/table/{2}", GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestCustomerId, GenerateObjectsAndStringHelper.TestTableName), urlString);
        }

        [Test]
        public void GenerateUrl_Consumer()
        {
            var urlString = HttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.Consumer, GenerateObjectsAndStringHelper.TestCustomerId);

            Assert.AreEqual(string.Format("{0}/consumers/{1}", GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestCustomerId), urlString);
        }

        [Test]
        public void GenerateUrl_DeleteAllocationWithCheckInId()
        {
            var urlString = HttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.DeleteAllocationWithCheckInId, GenerateObjectsAndStringHelper.TestCheckinId);

            Assert.AreEqual(string.Format("{0}/tables?checkin={1}", GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestCheckinId), urlString);
        }

        [Test]
        public void GenerateUrl_AddTableAllocation()
        {
            var urlString = HttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.AddTableAllocation, GenerateObjectsAndStringHelper.TestCustomerId);

            Assert.AreEqual(string.Format("{0}/consumers/{1}/table", GenerateObjectsAndStringHelper.TestBaseUrl, GenerateObjectsAndStringHelper.TestCustomerId), urlString);
        }

        [Test]
        public void GenerateUrl_SetSeatingAndOrderConfiguration()
        {
            var urlString = HttpComs.GenerateUrl(DoshiiDotNetIntegration.Enums.EndPointPurposes.SetSeatingAndOrderConfiguration);

            Assert.AreEqual(string.Format("{0}/config", GenerateObjectsAndStringHelper.TestBaseUrl), urlString);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void PutProductData_MakeRequest_emptyUrl()
        {
            HttpComs.MakeRequest("", "PUT", "");

        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void PutProductData_MakeRequest_NoWebVerb()
        {
            HttpComs.MakeRequest(GenerateObjectsAndStringHelper.TestBaseUrl, "", "");

        }
    }
}
