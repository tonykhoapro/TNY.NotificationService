using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNY.NotificationService.BUS.Models;
using TNY.NotificationService.SignalR.Hubs;

namespace TNY.NotificationService.SignalR
{
    public class SignalRService
    {
        public void SendNotification(NotificationActivity obj)
        {
            SubNotificationHub objNotifHub = new SubNotificationHub();
            //Notification objNotif = new Notification();

            //context.Configuration.ProxyCreationEnabled = false;
            //context.Notifications.Add(objNotif);
            //context.SaveChanges();

            objNotifHub.SendNotification(obj.Content);
        }
    }
}
