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
    public class TemplatesController : ApiController
    {
        Bus_Templates bus_Templates = new Bus_Templates();

        [HttpGet]
        public HttpResponseMessage GetAll()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, bus_Templates.GetAll());
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage Add([FromBody]Templates model)
        {
            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Type))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                model.CreateTime = DateTime.Now;
                bus_Templates.Create(model);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage Update([FromBody]Templates model)
        {
            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Type))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                bool _rs = bus_Templates.Update(model);
                return Request.CreateResponse(HttpStatusCode.OK, _rs);
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage Remove([FromBody]Templates model)
        {
            if (model.Id == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                Templates _Templates = bus_Templates.Get_ById(model.Id);
                if (_Templates == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    bus_Templates.Remove(model.Id);
                    return Request.CreateResponse(HttpStatusCode.OK);
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
