using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;

namespace Megatech.FMS.WebAPI.App_Start
{
    public class Logger
    {
        public static void AppendLog(string tag, string log) {
            AppendLog(tag, log, "post");
        }
        public static void AppendLog(string tag, string log, string fileName)
        {
            Logging.Logger.SetPath( HostingEnvironment.MapPath("~/logs/" ));
            Logging.Logger.AppendLog(tag, log, fileName);
        }
    }
}