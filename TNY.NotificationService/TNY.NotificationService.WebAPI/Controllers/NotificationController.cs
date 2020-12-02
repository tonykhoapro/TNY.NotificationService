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
        //SignalRService service_signalr = new SignalRService();
        NotificationHub notificationHub = new NotificationHub();
        public string GetAllAppInfo()
        {
            List<AppInfo> lstAppInfo = bus_AppInfo.GetAll();
            return JsonConvert.SerializeObject(lstAppInfo);
        }

        public void Post([FromBody]string value)
        {

        }

        [HttpPost]
        public HttpResponseMessage SendAll([FromBody]NotificationModel value)
        {
            try
            {
                if (string.IsNullOrEmpty(value.Content))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                IEnumerable<string> _sessionGuid;
                User _usr = new User();
                if (Request.Headers.TryGetValues("SessionGuid", out _sessionGuid))
                {
                    string _sessID = _sessionGuid.First();
                    string _userID = MemoryCacher.GetValue(_sessID).ToString();
                    _usr = bus_user.Get_ById(_userID);
                    if (_usr == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                string _webappid = value.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).FirstOrDefault();
                if (_webappid != null)
                {
                    bus_NotificationActivity.Create(new NotificationActivity
                    {
                        Content = value.Content,
                        UserID = _usr.Id,
                        RecipientIDs = new List<string> { "all" },
                        AppID = new List<string> { _webappid },
                        SendTime = DateTime.Now
                    });
                    notificationHub.SendAll(_webappid, value.Content);
                    return Request.CreateResponse(HttpStatusCode.OK);
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
                    string _userID = MemoryCacher.GetValue(_sessID).ToString();
                    _usr = bus_user.Get_ById(_userID);
                    if (_usr == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                string _webappid = value.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).FirstOrDefault();
                if (_webappid != null)
                {
                    bus_NotificationActivity.Create(new NotificationActivity
                    {
                        Content = value.Content,
                        UserID = _usr.Id,
                        RecipientIDs = value.RecipientIDs,
                        AppID = new List<string> { _webappid },
                        SendTime = DateTime.Now
                    });
                    notificationHub.SendSegment(value.RecipientIDs, _webappid, value.Content);
                    return Request.CreateResponse(HttpStatusCode.OK);
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

        [HttpGet]
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

        [HttpGet]
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

        [HttpGet]
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

        [HttpPost]
        public HttpResponseMessage SendAll_Schedule([FromBody]NotificationModel value)
        {
            try
            {
                if (string.IsNullOrEmpty(value.Content) || value.ScheduleTime == DateTime.MinValue)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                IEnumerable<string> _sessionGuid;
                User _usr = new User();
                if (Request.Headers.TryGetValues("SessionGuid", out _sessionGuid))
                {
                    string _sessID = _sessionGuid.First();
                    string _userID = MemoryCacher.GetValue(_sessID).ToString();
                    _usr = bus_user.Get_ById(_userID);
                    if (_usr == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                string _webappid = value.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).FirstOrDefault();
                if (_webappid != null)
                {
                    bus_NotificationActivity.Create(new NotificationActivity
                    {
                        Content = value.Content,
                        UserID = _usr.Id,
                        RecipientIDs = new List<string> { "all" },
                        AppID = new List<string> { _webappid },
                        ScheduleTime = value.ScheduleTime
                    });
                    return Request.CreateResponse(HttpStatusCode.OK);
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
                    string _userID = MemoryCacher.GetValue(_sessID).ToString();
                    _usr = bus_user.Get_ById(_userID);
                    if (_usr == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                string _webappid = value.AppIDs.Where(x => bus_AppInfo.Get_ById(x).Type == AppTypeConst.WEBAPP).FirstOrDefault();
                if (_webappid != null)
                {
                    bus_NotificationActivity.Create(new NotificationActivity
                    {
                        Content = value.Content,
                        UserID = _usr.Id,
                        RecipientIDs = value.RecipientIDs,
                        AppID = new List<string> { _webappid },
                        ScheduleTime = value.ScheduleTime
                    });
                    return Request.CreateResponse(HttpStatusCode.OK);
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
    }
}
