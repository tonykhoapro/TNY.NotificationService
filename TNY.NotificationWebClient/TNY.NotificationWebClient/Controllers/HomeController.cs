using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TNY.NotificationWebClient.Models;
using TNY.NotificationWebClient.Utils;

namespace TNY.NotificationWebClient.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            UserModel user = (UserModel)Session[CommonConst.USER_SESSION];
            return View(user);
        }
        

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}