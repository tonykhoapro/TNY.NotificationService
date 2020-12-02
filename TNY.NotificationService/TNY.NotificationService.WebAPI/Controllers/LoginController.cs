using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TNY.NotificationService.BUS;
using TNY.NotificationService.BUS.Models;
using TNY.NotificationService.Utils;
using TNY.NotificationService.WebAPI.Models;
using TNY.NotificationService.WebAPI.Utils;

namespace TNY.NotificationService.WebAPI.Controllers
{
    public class LoginController : ApiController
    {
        Bus_User bus_user = new Bus_User();

        [HttpPost]
        [Route("api/login")]
        public HttpResponseMessage Post([FromBody]UserModel value)
        {
            //Request.Headers.GetValues("SessionID");
            User _user = bus_user.Get_ByLogin(value.UserName, value.Password);
            if(_user != null)
            {
                string _sessionGuid = Guid.NewGuid().ToString();
                MemoryCacher.Add(_sessionGuid, _user.Id, DateTimeOffset.Now.AddHours(1));
                return Request.CreateResponse(HttpStatusCode.OK, new { SessionGuid = _sessionGuid});
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("api/logout")]
        public HttpResponseMessage Logout()
        {
            try
            {
                IEnumerable<string> _sessionGuid;
                if (Request.Headers.TryGetValues("SessionGuid", out _sessionGuid))
                {
                    MemoryCacher.Delete(_sessionGuid.First());
                    return Request.CreateResponse(HttpStatusCode.OK, new { SessionGuid = _sessionGuid.First() });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
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
