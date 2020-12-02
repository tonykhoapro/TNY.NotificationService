using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNY.NotificationService.WebAPI.signalr.models
{
    public class UserHubModel
    {
        public string UserID { get; set; }
        public string AppID { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
}
