﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TNY.BUS.Utils
{
    public class ConfigHelper
    {
        public string DBConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

        public string DBName = ConfigurationManager.AppSettings["DBName"];
    }
}
