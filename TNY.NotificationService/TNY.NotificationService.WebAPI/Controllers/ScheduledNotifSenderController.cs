﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using TNY.NotificationService.BUS;
using TNY.NotificationService.BUS.Models;
using TNY.NotificationService.Utils;
using TNY.NotificationService.WebAPI.Definitions;
using TNY.NotificationService.WebAPI.signalr.hubs;
using TNY.NotificationService.WebAPI.Utils;

namespace TNY.NotificationService.WebAPI.Controllers
{
    public class ScheduledNotifSenderController : ApiController
    {
        Bus_NotificationActivity bus_NotificationActivity = new Bus_NotificationActivity();
        Bus_AppInfo bus_AppInfo = new Bus_AppInfo();
        NotificationHub notificationHub = new NotificationHub();
        [HttpPost]
        public HttpResponseMessage Start()
        {
            try
            {
                Thread _thd = (Thread)MemoryCacher.GetValue(SessionConst.SESSION_THREAD);
                if (_thd == null)
                {
                    Thread t1 = new Thread(() =>
                    {
                        while (true)
                        {
                            List<NotificationActivity> _lstSel = bus_NotificationActivity.Get_Unpush();
                            foreach (NotificationActivity notif in _lstSel)
                            {
                                string _webappid = notif.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).FirstOrDefault();
                                notificationHub.SendSegment(notif.Id, notif.RecipientIDs, _webappid, notif.Content);
                                bus_NotificationActivity.Update_SendTime(notif);
                            }
                            LogGenerationHelper.WriteToFile("Scheduled Sender running");
                            Thread.Sleep(60_000);
                        }
                    });
                    MemoryCacher.Add(SessionConst.SESSION_THREAD, t1, DateTimeOffset.Now.AddYears(1));
                    t1.Start();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Error = "Thread already started." });
                }
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage Stop()
        {
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}