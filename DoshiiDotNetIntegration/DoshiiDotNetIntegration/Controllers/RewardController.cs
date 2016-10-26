using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Controllers
{
    internal class RewardController
    {
        internal Models.Controllers _controllers;
        internal HttpController _httpComs;

        internal RewardController(Models.Controllers controller, HttpController httpComs)
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
            if (_controllers.OrderingController == null)
            {
                _controllers.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - IOrderingManager cannot be null");
                throw new NullReferenceException("orderingManager cannot be null");
            }
            if (_controllers.RewardManager == null)
            {
                _controllers.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - rewardManager cannot be null");
                throw new NullReferenceException("rewardManager cannot be null");
            }
            if (httpComs == null)
            {
                _controllers.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;
            
        }

        internal virtual Member GetMember(string memberId)
        {
            try
            {
                return _httpComs.GetMember(memberId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual IEnumerable<Member> GetMembers()
        {
            try
            {
                return _httpComs.GetMembers();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual bool DeleteMember(Member member)
        {
            try
            {
                return _httpComs.DeleteMember(member);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual Member UpdateMember(Member member)
        {
            if (string.IsNullOrEmpty(member.Name))
            {
                throw new MemberIncompleteException("member name is blank");
            }
            try
            {
                if (string.IsNullOrEmpty(member.Id))
                {
                    return _httpComs.PostMember(member);
                }
                else
                {
                    return _httpComs.PutMember(member);
                }

            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual bool SyncDoshiiMembersWithPosMembers()
        {
            try
            {
                List<Member> DoshiiMembersList = GetMembers().ToList();
                List<Member> PosMembersList = _controllers.RewardManager.GetMembersFromPos().ToList();

                var doshiiMembersHashSet = new HashSet<string>(DoshiiMembersList.Select(p => p.Id));
                var posMembersHashSet = new HashSet<string>(PosMembersList.Select(p => p.Id));

                var membersNotInDoshii = PosMembersList.Where(p => !doshiiMembersHashSet.Contains(p.Id));
                foreach (var mem in membersNotInDoshii)
                {
                    _controllers.RewardManager.DeleteMemberOnPos(mem);
                }

                var membersInPos = DoshiiMembersList.Where(p => posMembersHashSet.Contains(p.Id));
                foreach (var mem in membersInPos)
                {
                    Member posMember = PosMembersList.FirstOrDefault(p => p.Id == mem.Id);
                    if (!mem.Equals(posMember))
                    {
                        _controllers.RewardManager.UpdateMemberOnPos(mem);
                    }
                }

                var membersNotInPos = DoshiiMembersList.Where(p => !posMembersHashSet.Contains(p.Id));
                foreach (var mem in membersNotInPos)
                {
                    _controllers.RewardManager.CreateMemberOnPos(mem);
                }


                return true;
            }
            catch (Exception ex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: There was an exception while attempting to sync Doshii members with the pos"), ex);
                return false;
            }
        }

        internal virtual IEnumerable<Reward> GetRewardsForMember(string memberId, string orderId, decimal orderTotal)
        {
            try
            {
                return _httpComs.GetRewardsForMember(memberId, orderId, orderTotal);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual bool RedeemRewardForMember(Member member, Reward reward, Order order)
        {
            try
            {
                var returnedOrder = _controllers.OrderingController.UpdateOrder(order);
                if (returnedOrder == null)
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: The order was not successfully sent to Doshii so the reward could not be redeemed."));
                    return false;
                }
            }
            catch (Exception ex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: There was an exception putting and order to Doshii for a rewards redeem"), ex);
                return false;
            }
            try
            {
                return _httpComs.RedeemRewardForMember(member.Id, reward.Id, order);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual bool RedeemRewardForMemberCancel(string memberId, string rewardId, string cancelReason)
        {
            try
            {
                return _httpComs.RedeemRewardForMemberCancel(memberId, rewardId, cancelReason);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual bool RedeemRewardForMemberConfirm(string memberId, string rewardId)
        {
            try
            {
                return _httpComs.RedeemRewardForMemberConfirm(memberId, rewardId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual bool RedeemPointsForMember(Member member, App app, Order order, int points)
        {
            try
            {
                order = _controllers.OrderingController.UpdateOrder(order);
                if (order == null)
                {
                    _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: There was a problem updating the order on Doshii, so the points can't re redeemed."));
                    return false;
                }
            }
            catch (Exception ex)
            {
                _controllers.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: There was an exception putting and order to Doshii for a rewards redeem"), ex);
                return false;
            }
            PointsRedeem pr = new PointsRedeem()
            {
                AppId = app.Id,
                OrderId = order.Id,
                Points = points
            };
            try
            {
                return _httpComs.RedeemPointsForMember(pr, member);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        internal virtual bool RedeemPointsForMemberConfirm(string memberId)
        {
            try
            {
                return _httpComs.RedeemPointsForMemberConfirm(memberId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        public virtual bool RedeemPointsForMemberCancel(string memberId, string cancelReason)
        {
            try
            {
                return _httpComs.RedeemPointsForMemberCancel(memberId, cancelReason);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }
    }
}
