using Models;
using Serilog;
using Services;
using Shared;
using SurveyConfiguratorTask.Models;
using SurveyConfiguratorTaskWeb.Models;
using SurveyWeb.Resources;
using System;
using System.Configuration;
using System.Web.Mvc;


namespace SurveyConfiguratorTaskWeb.Controllers
{
    public class HomeController : Controller
    {
        IQuestionService mService;
        const string UI_ERROR_MESSAGE = "UI_ERROR_MESSAGE";
        const string ERROR_VIEW = "Error";
        public HomeController(IQuestionService pService )
        {
            
            try
            {
                mService = pService;
                string tConnString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
                var tResult = mService.SetConnectionString(tConnString);
                mService.QuestionsLoad();

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while create HomeController instance.");

            }

        }
        public ActionResult Index()
        {

            try
            {
                
                
                var tQuestionList = mService.QuestionsLoad();
                if (!tQuestionList.Success)
                {

                    return View(viewName: ERROR_VIEW, model: tQuestionList.Error.ToString());
                }
                return View(model: tQuestionList);
            }
            catch (Exception ex)
            {

                Log.Error(ex, "Unexpected error occurred while loading questions in Index action.");
                return View(viewName: ERROR_VIEW, model: UI_ERROR_MESSAGE );
            }

        }









    }
}