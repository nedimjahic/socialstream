using SocialStream.DAL;
using SocialStream.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SocialStream
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
            {
                Response.Flush();
            }
        }



        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            using (DC data = new DC())
            {
                Log log = new Log();
                log.ErrorMessage = exception.Message;
                log.ErrorStack = exception.StackTrace;

                data.Log.Add(log);

                data.SaveChanges();
            }
        }
    }
}
