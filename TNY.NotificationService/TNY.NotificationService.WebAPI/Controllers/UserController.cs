﻿using System;
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
    public class UserController : ApiController
    {
        Bus_User bus_User = new Bus_User();

        [HttpGet]
        public HttpResponseMessage GetAll()
        {
            try
            {
                List<User> lstUser = bus_User.GetAll();
                lstUser.ForEach(_ => _.Password = "********");
                return Request.CreateResponse(HttpStatusCode.OK, lstUser);
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage Add([FromBody]User model)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                User _usr = bus_User.Create(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { UserID = _usr.Id });
            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage Update([FromBody]User model)
        {
            if (model.Id == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                User _usr = bus_User.Get_ById(model.Id);
                if (_usr == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, bus_User.Update(model));
                }

            }
            catch (Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage Remove([FromBody]User model)
        {
            if (model.Id == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                User _usr = bus_User.Get_ById(model.Id);
                if(_usr == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    bus_User.Remove(model.Id);
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
