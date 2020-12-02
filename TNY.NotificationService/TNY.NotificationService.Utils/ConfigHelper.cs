using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TNY.NotificationService.Utils
{
    public class ConfigHelper
    {
        public static readonly string DBConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

        public static readonly string DBName = ConfigurationManager.AppSettings["DBName"];
        public static readonly string LogDirectory = ConfigurationManager.AppSettings["LogDirectory"];
    }
}
