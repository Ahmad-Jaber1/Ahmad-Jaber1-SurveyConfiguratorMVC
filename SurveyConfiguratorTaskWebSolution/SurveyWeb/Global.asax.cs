
using Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace SurveyWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ContainerConfig.Resgister();
            LogConfiguration.Configure(Server.MapPath("~"));


        }
        protected void Application_AcquireRequestState()
        {
            if (Session["lang"] == null) Session["lang"] = "en";
            var tLanguage = Session["lang"].ToString();
            var tCultureInfo = new CultureInfo(tLanguage);
            Thread.CurrentThread.CurrentUICulture = tCultureInfo;
            //Thread.CurrentThread.CurrentCulture = tCultureInfo;
        }
    }
}
