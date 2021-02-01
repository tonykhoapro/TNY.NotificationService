using System;
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
                            LogGenerationHelper.WriteToFile("Scheduled Sender running");
                            List<NotificationActivity> _lstSel = bus_NotificationActivity.Get_Unpush();
                            foreach (NotificationActivity notif in _lstSel)
                            {
                                try
                                {
                                    LogGenerationHelper.WriteToFile("Scheduled Sender sending");
                                    string _webappid = notif.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).FirstOrDefault();
                                    notificationHub.SendSegment(notif.Id, notif.RecipientIDs, _webappid, notif.Content);
                                    bus_NotificationActivity.Update_SendTime(notif);
                                }
                                catch (Exception ex)
                                {
                                    LogGenerationHelper.WriteToFile($"{notif.Id} -> {ex.Message}");
                                }
                            }
                            List<NotificationActivity> _lstRoutine = bus_NotificationActivity.Get_Routine();
                            foreach (NotificationActivity routine in _lstRoutine)
                            {
                                try
                                {
                                    LogGenerationHelper.WriteToFile("Routine Sender sending");
                                    if (routine.Routine.Type == RoutineType.DAILY)
                                    {
                                        if (((DateTime)routine.Routine.Time).TimeOfDay >= DateTime.Now.TimeOfDay)
                                        {
                                            if(routine.SendTime != null && ((DateTime)routine.SendTime).Date != DateTime.Today)
                                            {
                                                string _webappid = routine.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).FirstOrDefault();
                                                notificationHub.SendSegment(routine.Id, routine.RecipientIDs, _webappid, routine.Content);
                                                bus_NotificationActivity.Update_SendTime(routine);
                                            }
                                        }
                                    }
                                    else if(routine.Routine.Type == RoutineType.WEEKLY)
                                    {
                                        if ( routine.Routine.DayOfWeek == DateTime.Now.DayOfWeek && ((DateTime)routine.Routine.Time).TimeOfDay >= DateTime.Now.TimeOfDay)
                                        {
                                            if (routine.SendTime != null && ((DateTime)routine.SendTime).Date != DateTime.Today)
                                            {
                                                string _webappid = routine.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).FirstOrDefault();
                                                notificationHub.SendSegment(routine.Id, routine.RecipientIDs, _webappid, routine.Content);
                                                bus_NotificationActivity.Update_SendTime(routine);
                                            }
                                        }
                                    }
                                    else if (routine.Routine.Type == RoutineType.MONTHLY)
                                    {
                                        if (((DateTime)routine.Routine.Time).Day == DateTime.Now.Day && ((DateTime)routine.Routine.Time).TimeOfDay >= DateTime.Now.TimeOfDay)
                                        {
                                            if (routine.SendTime != null && ((DateTime)routine.SendTime).Date != DateTime.Today)
                                            {
                                                string _webappid = routine.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).FirstOrDefault();
                                                notificationHub.SendSegment(routine.Id, routine.RecipientIDs, _webappid, routine.Content);
                                                bus_NotificationActivity.Update_SendTime(routine);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogGenerationHelper.WriteToFile($"{routine.Id} -> {ex.Message}");
                                }
                            }
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
