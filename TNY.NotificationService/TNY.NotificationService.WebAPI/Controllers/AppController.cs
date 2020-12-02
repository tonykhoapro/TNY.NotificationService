using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TNY.NotificationService.BUS;
using TNY.NotificationService.BUS.Models;
using TNY.NotificationService.Utils;

namespace TNY.NotificationService.WebAPI.Controllers
{
    public class AppController : ApiController
    {
        Bus_AppInfo bus_AppInfo = new Bus_AppInfo();

        [HttpGet]
        public HttpResponseMessage GetAll()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, bus_AppInfo.GetAll());
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage Add([FromBody]AppInfo model)
        {
            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Type))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                bus_AppInfo.Create(model);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage Remove([FromBody]AppInfo model)
        {
            if (model.Id == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                bus_AppInfo.Remove(model.Id);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
