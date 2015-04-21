using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Tests
{
    public static class GenerateObjectsAndStringHelper
    {
        #region Generate TestObjects and TestValues

        public static string TestBaseUrl = "TestUrl";
        public static string TestCustomerId = "TestCustomerId";
        public static string TestPayPalCustomerId = "TestPaypalCustomerId";
        public static string TestTableNumber = "TestTableNumber";
        public static string TestToken = "TestToken";
        public static string TestCheckinId = "TestCheckinId";
        public static string TestTableName = "TestName";
        public static string TestTableAllocationName = "TestAllocationName";
        public static string TestTableAllocationStatus = "TableAllocationStatus";
        public static string TestPaypalTabId = "TestPaypalTabId";
        public static string TestLocationId = "TestLocationId";
        public static string TestCheckinStatus = "TestCheckinStatus";
        public static string TestGratuity = "TestGratuity";
        public static int TestOrderId = 123;


        public static Models.Order GenerateOrder()
        {
            var order = new Models.Order()
            {
                Id = TestOrderId,
                CheckinId = TestCheckinId
            };
            return order;
        }

        public static CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs GenerateTableAllocationEventArgs()
        {
            var allocationEventArgs = new CommunicationLogic.CommunicationEventArgs.TableAllocationEventArgs()
            {
                TableAllocation = GenerateTableAllocation()
            };
            return allocationEventArgs;
        }

        public static Models.TableAllocation GenerateTableAllocation()
        {
            var tableAllocation = new Models.TableAllocation()
            {
                Id = TestTableName,
                Name = TestTableAllocationName,
                CustomerId = TestCustomerId,
                PaypalCustomerId = TestCustomerId,
                Status = TestTableAllocationStatus,
                Checkin = GenerateCheckin(),
                rejectionReason = Enums.TableAllocationRejectionReasons.TableDoesNotExist
            };
            return tableAllocation;
        }

        public static Models.Checkin GenerateCheckin()
        {
            var checkin = new Models.Checkin()
            {
                Id = TestCheckinId,
                PaypalTabId = TestPaypalTabId,
                ConsumerId = TestCustomerId,
                LocationId = TestLocationId,
                Status = TestCheckinStatus,
                ExpirationDate = DateTime.Now,
                Gratuity = TestGratuity,
                UpdatedAt = DateTime.Now,
                PaypalCustomerId = TestPayPalCustomerId
            };
            return checkin;
        }

        public static Models.Consumer GenerateConsumer1()
        {
            Models.Consumer consumer = new Models.Consumer();
            consumer.CheckInId = "123";
            consumer.Id = 1;
            consumer.Name = "John Doe";
            consumer.PaypalCustomerId = "NC123NV";
            consumer.PhotoUrl = new Uri("http://www.google.com");
            return consumer;
        }

        public static Models.Consumer GenerateConsumer2()
        {
            Models.Consumer consumer = new Models.Consumer();
            consumer.CheckInId = "456";
            consumer.Id = 2;
            consumer.Name = "Jayne Doe";
            consumer.PaypalCustomerId = "ACB123AB";
            consumer.PhotoUrl = new Uri("http://www.google.com");
            return consumer;
        }

        public static Models.Consumer GenerateConsumer3()
        {
            Models.Consumer consumer = new Models.Consumer();
            consumer.CheckInId = "789";
            consumer.Id = 3;
            consumer.Name = "Bulkan e";
            consumer.PaypalCustomerId = "axy765xa";
            consumer.PhotoUrl = new Uri("http://www.google.com");
            return consumer;
        }

        public static Models.Consumer GenerateConsumer4()
        {
            Models.Consumer consumer = new Models.Consumer();
            consumer.CheckInId = "101";
            consumer.Id = 4;
            consumer.Name = "Mary Jane";
            consumer.PaypalCustomerId = "bgr531gb";
            consumer.PhotoUrl = new Uri("http://www.google.com");
            return consumer;
        }
        #endregion
    }
}
