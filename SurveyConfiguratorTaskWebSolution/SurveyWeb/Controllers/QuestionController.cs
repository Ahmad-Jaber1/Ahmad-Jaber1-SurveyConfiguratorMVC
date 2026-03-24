using Helper;
using Models;
using Newtonsoft.Json;
using Serilog;
using Services;
using SurveyConfiguratorTask.Models;
using SurveyConfiguratorTaskWeb.Models;
using SurveyWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SurveyWeb.Controllers
{
    public class QuestionController : Controller
    {


        IQuestionService mService;
        const string UI_ERROR_MESSAGE = "Error";
        public QuestionController(IQuestionService pService)
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
        public ActionResult Add()
        {
            return View(new CreateQuestionViewModel());
        }
        [HttpPost]
        public ActionResult CreateQuestion(CreateQuestionViewModel pQuestion)
        {
            if (!ModelState.IsValid)
            {
                
                return View(viewName:"Add",pQuestion);
            }

            BasicQuestionViewModel tSpecificVM = QuestionHelper.Deserialize(pQuestion);

            bool isValid = QuestionHelper.Validate(tSpecificVM , out var pValidationResults);
           
           

            if (!isValid)
            {
                
                foreach (var vr in pValidationResults)
                {
                    
                    ModelState.AddModelError("", vr.ErrorMessage);
                }

                return View(viewName:"Add",pQuestion ); 
            }

            
            var tDto = tSpecificVM.MapToDto();

          
            mService.AddQuestion(pQuestion.QuestionType, tDto);

            return RedirectToAction("Index",controllerName:"Home");
        }




        public ActionResult GetPartial(string type)
        {
            switch (type)
            {
                case "SliderQuestion":
                    return PartialView("_SliderPartial", new SliderQuestionViewModel());
                case "StarsQuestion":
                    return PartialView("_StarsPartial", new StarsQuestionViewModel());
                case "SmileyFacesQuestion":
                    return PartialView("_SmileyPartial", new SmileyQuestionViewModel());
                default:
                    return Content(""); // empty
            }
        }
    }
}