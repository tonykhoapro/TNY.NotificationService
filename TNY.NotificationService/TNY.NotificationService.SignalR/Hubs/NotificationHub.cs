using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNY.NotificationService.BUS;
using TNY.NotificationService.SignalR.Models;
using TNY.NotificationService.Utils;

namespace TNY.NotificationService.SignalR.Hubs
{
    public class SubNotificationHub : Hub
    {
        private Bus_NotificationActivity bus_NotificationActivity = new Bus_NotificationActivity();

        private static readonly ConcurrentDictionary<string, UserHubModel> Users =
        new ConcurrentDictionary<string, UserHubModel>(StringComparer.InvariantCultureIgnoreCase);

        //private NotifEntities context = new NotifEntities();

        //Logged Use Call  
        //public void GetNotification()
        //{
        //    try
        //    {
        //        //string loggedUser = Context.User.Identity.Name;

        //        //Get TotalNotification  
        //        string totalNotif = "My notification";

        //        //Send To  
        //        UserHubModel receiver;
        //        if (Users.TryGetValue(loggedUser, out receiver))
        //        {
        //            var cid = receiver.ConnectionIds.FirstOrDefault();
        //            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        //            context.Clients.Client(cid).broadcastNotif(totalNotif);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logHelper.WriteToFile(ex.Message);
        //    }
        //}

        //Specific User Call  
        public void SendNotification(string message)
        {
            try
            {
                //Get TotalNotification  
                //string totalNotif = LoadNotifData(SentTo);

                //Send To  
                //UserHubModel receiver;
                //if (Users.TryGetValue(SentTo, out receiver))
                //{
                //    var cid = receiver.ConnectionIds.FirstOrDefault();
                //    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                //    context.Clients.All.broadcaastNotif(totalNotif);
                //}
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SubNotificationHub>();
                context.Clients.All.receiveNotification($"{message}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            var user = Users.GetOrAdd(userName, _ => new UserHubModel
            {
                UserName = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);
                if (user.ConnectionIds.Count == 1)
                {
                    Clients.Others.userConnected(userName);
                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            UserHubModel user;
            Users.TryGetValue(userName, out user);

            if (user != null)
            {
                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                    if (!user.ConnectionIds.Any())
                    {
                        UserHubModel removedUser;
                        Users.TryRemove(userName, out removedUser);
                        Clients.Others.userDisconnected(userName);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}
