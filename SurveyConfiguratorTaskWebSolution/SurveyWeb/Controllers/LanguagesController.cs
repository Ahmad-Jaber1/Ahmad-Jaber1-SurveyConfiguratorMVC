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
        const string UI_ERROR_MESSAGE = "UI_ERROR_MESSAGE";
        const string ERROR_VIEW = "Error";
        const string LANGUAGE = "lang";
        const string ENGLISH = "en";
        const string ARABIC = "ar";

        public ActionResult Arabic()
        {
            try
            {
                Session[LANGUAGE] = ARABIC;
                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while switching language to Arabic in Arabic action.");
                return View(viewName: ERROR_VIEW, model: UI_ERROR_MESSAGE);
            }
        }

        public ActionResult English()
        {
            try
            {
                Session[LANGUAGE] = ENGLISH;
                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while switching language to English in English action.");
                return View(viewName: ERROR_VIEW, model: UI_ERROR_MESSAGE);
            }
        }
    }
}