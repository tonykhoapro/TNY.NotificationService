using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using TNY.NotificationService.BUS;
using TNY.NotificationService.BUS.Models;
using TNY.NotificationService.Utils;
using TNY.NotificationService.WebAPI.signalr.models;
using TNY.NotificationService.WebAPI.Utils;

namespace TNY.NotificationService.WebAPI.signalr.hubs
{
    public class NotificationHub : Hub
    {
        Bus_Recipients bus_Recipients = new Bus_Recipients();
        Bus_NotificationActivity bus_NotificationActivity = new Bus_NotificationActivity();
        private static readonly ConcurrentDictionary<Tuple<string, string>, HashSet<string>> Users =
            new ConcurrentDictionary<Tuple<string, string>, HashSet<string>>(new TupleEqualityComparer());
        //public void SendAll(string appid, string message)
        //{
        //    try
        //    {
        //        List<HashSet<string>> lstSel = Users.Where(x => x.Key.Item2 == appid).Select(x => x.Value).ToList();
        //        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        //        foreach (var supItem in lstSel)
        //        {
        //            foreach (var cid in supItem)
        //            {
        //                context.Clients.Client(cid).receiveNotification(message);
        //            }
        //        }
        //        context.Clients.All.receiveNotification(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public void SendSpecific(string userID, string message)
        //{
        //    try
        //    {
        //        //Send To  
        //        UserHubModel receiver;
        //        var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        //        if (Users.TryGetValue(userID, out receiver))
        //        {
        //            var cid = receiver.ConnectionIds.FirstOrDefault();
        //            context.Clients.Client(cid).receiveNotification(message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void SendSegment(string notifid, List<string> userIDs, string appid, string message)
        {
            try
            {
                //Send To  
                //UserHubModel receiver;
                HashSet<string> receiver;
                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                foreach (string id in userIDs)
                {
                    if (Users.TryGetValue(new Tuple<string, string>(id, appid), out receiver))
                    {
                        foreach (var cid in receiver)
                        {
                            LogGenerationHelper.WriteToFile("Sending notif to: " + id + " - cid: " + cid);
                            context.Clients.Client(cid).receiveNotification(message);
                        }
                    }
                    else
                    {
                        //cap nhat vao bang Recipients
                        LogGenerationHelper.WriteToFile("Not found: " + id + " - Updating to Recipients");
                        Recipients _rep = bus_Recipients.Get_ByIdAppID(id, appid);
                        if(_rep == null)
                        {
                            bus_Recipients.Create(new Recipients {
                                Id = id,
                                AppID = appid,
                                NotifIDs = new List<string> { notifid }
                            });
                        }
                        else
                        {
                            _rep.NotifIDs.Add(notifid);
                            bus_Recipients.Replace(_rep);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendMisspush(string userid, string appid)
        {
            try
            {
                Recipients _rep = bus_Recipients.Get_ByIdAppID(userid, appid);
                if (_rep != null)
                {
                    LogGenerationHelper.WriteToFile("Pushing leftover notif");
                    HashSet<string> receiver;
                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    if (Users.TryGetValue(new Tuple<string, string>(userid, appid), out receiver))
                    {
                        foreach (var cid in receiver)
                        {
                            foreach (string notif in _rep.NotifIDs)
                            {
                                NotificationActivity _not = bus_NotificationActivity.Get_ById(notif);
                                context.Clients.Client(cid).receiveNotification(_not.Content);
                            }
                        }
                    }
                    bus_Recipients.Update_ReceiveTime(_rep);
                }
                else
                {
                    LogGenerationHelper.WriteToFile("No leftover notif");
                }
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
            }
        }

        public override Task OnConnected()
        {
            //string userID = Context.User.Identity.Name;
            LogGenerationHelper.WriteToFile("Connected");
            string userID = Context.QueryString["userid"] == null ? string.Empty : Context.QueryString["userid"];
            string appID = Context.QueryString["appid"] == null ? string.Empty : Context.QueryString["appid"];
            string connectionId = Context.ConnectionId;

            var user = Users.GetOrAdd(new Tuple<string, string>(userID, appID), _ => new HashSet<string>());

            lock (user)
            {
                user.Add(connectionId);
                //if (user.ConnectionIds.Count == 1)
                //{
                //    Clients.Others.userConnected(userID);
                //}
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            LogGenerationHelper.WriteToFile("Disconnected");
            string userID = Context.QueryString["userid"] == null ? string.Empty : Context.QueryString["userid"];
            string appID = Context.QueryString["appid"] == null ? string.Empty : Context.QueryString["appid"];
            string connectionId = Context.ConnectionId;

            HashSet<string> user;
            Users.TryGetValue(new Tuple<string, string>(userID, appID), out user);

            if (user != null)
            {
                lock (user)
                {
                    user.RemoveWhere(cid => cid.Equals(connectionId));
                    if (!user.Any())
                    {
                        HashSet<string> removedUser;
                        Users.TryRemove(new Tuple<string, string>(userID, appID), out removedUser);
                        //Clients.Others.userDisconnected(userName);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}