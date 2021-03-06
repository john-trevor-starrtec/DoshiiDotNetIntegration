﻿using DoshiiDotNetIntegration.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDotNetPOS.POSImpl
{
    public class SampleConfigurationManager : IConfigurationManager
    {
        private String mBaseUrl;
        private String mVendor;
        private String mSecretKey;
        private String mLocationToken;

        public void Initialise(string baseUrl, string vendor, string secretKey, string locationToken)
        {
            this.mBaseUrl = baseUrl;
            this.mVendor = vendor;
            this.mSecretKey = secretKey;
            this.mLocationToken = locationToken;
        }

        public string GetBaseUrlFromPos()
        {
            return mBaseUrl;
        }

        public string GetLocationTokenFromPos()
        {
            return mLocationToken;
        }

        public string GetSecretKeyFromPos()
        {
            return mSecretKey;
        }

        public int GetSocketTimeOutFromPos()
        {
            return 30;
        }

        public string GetSocketUrlFromPos()
        {
            return "wss://sandbox-socket.doshii.co/pos/socket";
        }

        public string GetVendorFromPos()
        {
            return mVendor;
        }

    public IOrderingManager GetOrderingManagerFromPos()
    {
      return OrderingManager;
    }

    public ITransactionManager GetTransactionManagerFromPos()
    {
      return TransactionManager;
    }

    public IRewardManager GetRewardManagerFromPos()
    {
      return RewardManager;
    }

    public IReservationManager GetReservationManagerFromPos()
    {
      return ReservationManager;
    }

    public ILoggingManager GetLoggingManagerFromPos()
    {
      return LoggingManager;
    }

    public IOrderingManager OrderingManager { get; set; }
    public ITransactionManager TransactionManager { get; set; }
    public IRewardManager RewardManager { get; set; }
    public ILoggingManager LoggingManager { get; set; }
    public IReservationManager ReservationManager { get; set; }
  }
}
