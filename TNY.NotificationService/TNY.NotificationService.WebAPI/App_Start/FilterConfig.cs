using System.Web;
using System.Web.Mvc;

namespace TNY.NotificationService.WebAPI.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
