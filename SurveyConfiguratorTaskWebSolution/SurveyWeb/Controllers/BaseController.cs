using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;

namespace SurveyConfiguratorTaskWeb.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["lang"] == null) Session["lang"] = "en";
            var tLanguage = Session["lang"].ToString();
            var tCultureInfo = new CultureInfo(tLanguage);
            Thread.CurrentThread.CurrentUICulture = tCultureInfo;
            Thread.CurrentThread.CurrentCulture = tCultureInfo;
        }
    }
}