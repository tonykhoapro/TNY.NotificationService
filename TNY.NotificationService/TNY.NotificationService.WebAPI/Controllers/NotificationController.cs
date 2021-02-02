using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Newtonsoft.Json;
using TNY.NotificationService.BUS;
using TNY.NotificationService.BUS.Models;
using TNY.NotificationService.Utils;
using TNY.NotificationService.WebAPI.Definitions;
using TNY.NotificationService.WebAPI.Models;
using TNY.NotificationService.WebAPI.signalr.hubs;
using TNY.NotificationService.WebAPI.Utils;

namespace TNY.NotificationService.WebAPI.Controllers
{
    public class NotificationController : ApiController
    {
        Bus_AppInfo bus_AppInfo = new Bus_AppInfo();
        Bus_NotificationActivity bus_NotificationActivity = new Bus_NotificationActivity();
        Bus_User bus_user = new Bus_User();
        NotificationHub notificationHub = new NotificationHub();

        //public void Post([FromBody]string value)
        //{

        //}

        //[HttpPost]
        //public HttpResponseMessage SendAll([FromBody]NotificationModel value)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(value.Content))
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest);
        //        }
        //        IEnumerable<string> _sessionGuid;
        //        User _usr = new User();
        //        if (Request.Headers.TryGetValues("SessionGuid", out _sessionGuid))
        //        {
        //            string _sessID = _sessionGuid.First();
        //            string _userID = MemoryCacher.GetValue(_sessID).ToString();
        //            _usr = bus_user.Get_ById(_userID);
        //            if (_usr == null)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.Forbidden);
        //            }
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest);
        //        }
        //        string _webappid = value.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).FirstOrDefault();
        //        if (_webappid != null)
        //        {
        //            bus_NotificationActivity.Create(new NotificationActivity
        //            {
        //                Content = value.Content,
        //                UserID = _usr.Id,
        //                RecipientIDs = new List<string> { "all" },
        //                AppIDs = new List<string> { _webappid },
        //                SendTime = DateTime.Now
        //            });
        //            notificationHub.SendAll(_webappid, value.Content);
        //            return Request.CreateResponse(HttpStatusCode.OK);
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogGenerationHelper.WriteToFile(ex.Message);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    }
        //}

        [HttpPost]
        public HttpResponseMessage SendToList([FromBody]NotificationModel value)
        {
            try
            {
                //NotificationActivity _request = JsonConvert.DeserializeObject<NotificationActivity>(value);
                if (value.RecipientIDs == null || string.IsNullOrEmpty(value.Content))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                IEnumerable<string> _sessionGuid;
                User _usr = new User();
                if (Request.Headers.TryGetValues("SessionGuid", out _sessionGuid))
                {
                    string _sessID = _sessionGuid.First();
                    if (MemoryCacher.GetValue(_sessID) == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden);
                    }
                    string _userID = MemoryCacher.GetValue(_sessID).ToString();
                    _usr = bus_user.Get_ById(_userID);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                List<string> _webappid = value.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).ToList();
                if (_webappid != null && _webappid.Count > 0)
                {
                    NotificationActivity _notif = bus_NotificationActivity.Create(new NotificationActivity
                    {
                        Content = value.Content,
                        UserID = _usr.Id,
                        RecipientIDs = value.RecipientIDs,
                        AppIDs = _webappid,
                        SendTime = DateTime.Now
                    });
                    foreach (string _id in _webappid)
                    {

                        notificationHub.SendSegment(_notif.Id, value.RecipientIDs, _id, value.Content);

                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { NotifID = _notif.Id });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetNotif_ByTime([FromBody]NotifHistoryModel model)
        {
            //NotificationActivity _request = JsonConvert.DeserializeObject<NotificationActivity>(value);
            if (model.FromTime == DateTime.MinValue || model.ToTime == DateTime.MinValue)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                List<NotificationActivity> lstNotif = bus_NotificationActivity.Get_ByTime(model.FromTime, model.ToTime);
                return Request.CreateResponse(HttpStatusCode.OK, lstNotif);
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetNotif_ById([FromBody]NotifHistoryModel model)
        {
            if (model.Id == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                NotificationActivity notif = bus_NotificationActivity.Get_ById(model.Id);
                return Request.CreateResponse(HttpStatusCode.OK, notif);
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetNotif_ByUserID([FromBody]NotifHistoryModel model)
        {
            if (model.UserID == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                List<NotificationActivity> lstNotif = bus_NotificationActivity.Get_ByUserID(model.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, lstNotif);
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //[HttpPost]
        //public HttpResponseMessage SendAll_Schedule([FromBody]NotificationModel value)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(value.Content) || value.ScheduleTime == DateTime.MinValue)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest);
        //        }
        //        IEnumerable<string> _sessionGuid;
        //        User _usr = new User();
        //        if (Request.Headers.TryGetValues("SessionGuid", out _sessionGuid))
        //        {
        //            string _sessID = _sessionGuid.First();
        //            string _userID = MemoryCacher.GetValue(_sessID).ToString();
        //            _usr = bus_user.Get_ById(_userID);
        //            if (_usr == null)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.Forbidden);
        //            }
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest);
        //        }
        //        string _webappid = value.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).FirstOrDefault();
        //        if (_webappid != null)
        //        {
        //            bus_NotificationActivity.Create(new NotificationActivity
        //            {
        //                Content = value.Content,
        //                UserID = _usr.Id,
        //                RecipientIDs = new List<string> { "all" },
        //                AppIDs = new List<string> { _webappid },
        //                ScheduleTime = value.ScheduleTime
        //            });
        //            return Request.CreateResponse(HttpStatusCode.OK);
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogGenerationHelper.WriteToFile(ex.Message);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    }
        //}

        [HttpPost]
        public HttpResponseMessage SendToList_Schedule([FromBody]NotificationModel value)
        {
            try
            {
                if (value.RecipientIDs == null || string.IsNullOrEmpty(value.Content) || value.ScheduleTime == DateTime.MinValue)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                IEnumerable<string> _sessionGuid;
                User _usr = new User();
                if (Request.Headers.TryGetValues("SessionGuid", out _sessionGuid))
                {
                    string _sessID = _sessionGuid.First();
                    if (MemoryCacher.GetValue(_sessID) == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden);
                    }
                    string _userID = MemoryCacher.GetValue(_sessID).ToString();
                    _usr = bus_user.Get_ById(_userID);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                string _webappid = value.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).FirstOrDefault();
                if (_webappid != null)
                {
                    NotificationActivity _notif = bus_NotificationActivity.Create(new NotificationActivity
                    {
                        Content = value.Content,
                        UserID = _usr.Id,
                        RecipientIDs = value.RecipientIDs,
                        AppIDs = new List<string> { _webappid },
                        ScheduleTime = value.ScheduleTime
                    });
                    return Request.CreateResponse(HttpStatusCode.OK, new { NotifID = _notif.Id });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage SendToList_Routine([FromBody]NotificationModel value)
        {
            try
            {
                if (value.RecipientIDs == null || string.IsNullOrEmpty(value.Content) || value.Routine == null || value.Routine.Time == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                IEnumerable<string> _sessionGuid;
                User _usr = new User();
                if (Request.Headers.TryGetValues("SessionGuid", out _sessionGuid))
                {
                    string _sessID = _sessionGuid.First();
                    if (MemoryCacher.GetValue(_sessID) == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden);
                    }
                    string _userID = MemoryCacher.GetValue(_sessID).ToString();
                    _usr = bus_user.Get_ById(_userID);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                string _webappid = value.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).FirstOrDefault();
                if (_webappid != null)
                {
                    NotificationActivity _notif = bus_NotificationActivity.Create(new NotificationActivity
                    {
                        Content = value.Content,
                        UserID = _usr.Id,
                        RecipientIDs = value.RecipientIDs,
                        AppIDs = new List<string> { _webappid },
                        IsRoutine = true,
                        Routine = value.Routine
                    });
                    return Request.CreateResponse(HttpStatusCode.OK, new { NotifID = _notif.Id });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetAllRoutineNotif()
        {
            try
            {
                List<NotificationActivity> notifications = bus_NotificationActivity.Get_Routine();
                return Request.CreateResponse(HttpStatusCode.OK, notifications);
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        public HttpResponseMessage GetAllUnpushNotif()
        {
            try
            {
                List<NotificationActivity> notifications = bus_NotificationActivity.Get_Unpush();
                return Request.CreateResponse(HttpStatusCode.OK, notifications);
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage CancelScheduledNotif([FromBody]NotificationModel value)
        {
            List<NotificationActivity> notifications = bus_NotificationActivity.Get_Unpush();
            NotificationActivity _notif = notifications.Where(x => x.Id == value.Id).FirstOrDefault();
            if (_notif == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                bus_NotificationActivity.Update_Cancel(_notif);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateUnpushNotif([FromBody]NotificationModel value)
        {
            List<NotificationActivity> notifications = bus_NotificationActivity.Get_Unpush();
            notifications.AddRange(bus_NotificationActivity.Get_Routine());
            NotificationActivity _notif = notifications.Where(x => x.Id == value.Id).FirstOrDefault();
            if (_notif == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                _notif.Content = value.Content;
                _notif.RecipientIDs = value.RecipientIDs;
                _notif.AppIDs = value.AppIDs;
                _notif.ScheduleTime = value.ScheduleTime;
                _notif.IsRoutine = value.IsRoutine;
                _notif.Routine = value.Routine;
                bus_NotificationActivity.Replace(_notif);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }
    }
}
