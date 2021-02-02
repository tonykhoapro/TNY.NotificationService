using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TNY.NotificationService.WebAPI.Controllers
{
    public class DemoController : Controller
    {
        // GET: Demo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SendNotif(string SessionGuid)
        {
            return View("SendNotif", null, SessionGuid);
        }

        public ActionResult GetNotifInfo()
        {
            return View();
        }

        public ActionResult Applications()
        {
            return View();
        }

        public ActionResult Users()
        {
            return View();
        }
        public ActionResult Templates()
        {
            return View();
        }

        public ActionResult ScheduledNotifSender()
        {
            return View();
        }
    }
}