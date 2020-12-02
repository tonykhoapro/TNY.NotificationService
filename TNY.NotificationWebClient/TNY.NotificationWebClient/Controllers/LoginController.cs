using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TNY.NotificationWebClient.Models;
using TNY.NotificationWebClient.Utils;

namespace TNY.NotificationWebClient.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserModel model)
        {
            if (model.UserID == null)
            {
                return View();
            }
            Session.Add(CommonConst.USER_SESSION, model);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            return RedirectToAction("Index");
        }
    }
}