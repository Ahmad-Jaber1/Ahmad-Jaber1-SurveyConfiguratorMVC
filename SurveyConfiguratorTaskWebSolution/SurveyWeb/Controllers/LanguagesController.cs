using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SurveyConfiguratorTaskWeb.Controllers
{
    public class LanguagesController : Controller
    {
        const string UI_ERROR_MESSAGE = "Error";

        public ActionResult Arabic()
        {
            try
            {
                Session["lang"] = "ar";
                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while switching language to Arabic in Arabic action.");
                return View(viewName: "Error", model: nameof(UI_ERROR_MESSAGE));
            }
        }

        public ActionResult English()
        {
            try
            {
                Session["lang"] = "en";
                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while switching language to English in English action.");
                return View(viewName: "Error", model: nameof(UI_ERROR_MESSAGE));
            }
        }
    }
}