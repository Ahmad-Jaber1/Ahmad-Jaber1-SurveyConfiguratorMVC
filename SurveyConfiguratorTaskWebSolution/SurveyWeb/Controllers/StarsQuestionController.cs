using Models;
using Serilog;
using Services;
using SurveyConfiguratorTask.Models;
using SurveyConfiguratorTaskWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace testTaskConfigurator.Controllers
{
    
    public class StarsQuestionController : Controller
    {
        const string UI_ERROR_MESSAGE = "Error";
        private readonly IQuestionService mService;

        public StarsQuestionController(IQuestionService pService)
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
                Log.Error(ex, "Unexpected error occurred while create StarsController instance.");

            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStarsQuestion(StarsQuestionViewModel pStarsQuestion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("StarsView");
                }

                if (TempData["BasicQuestion"] == null)
                {
                    return View("Add");
                }

                var tBasicQuestion = (BasicQuestionViewModel)TempData["BasicQuestion"];
                AddQuestionDto tAddQuestionDto = new AddQuestionDto
                {
                    StarsCount = pStarsQuestion.StarsCount,
                    Text = tBasicQuestion.Text,
                    Order = tBasicQuestion.Order
                };

                var tResult = mService.AddQuestion(TypeQuestionEnum.StarsQuestion, tAddQuestionDto);
                if (!tResult.Success)
                {
                    return View("Error", model: tResult.Error.ToString());
                }

                return RedirectToAction("Index" ,controllerName:"Home");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while adding stars question in AddStarsQuestion action.");
                return View(viewName: "Error", model: nameof(UI_ERROR_MESSAGE));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStars(StarsQuestionViewModel pStarsQuestion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("EditStarsQuestion");
                }

                var tEditQuestionDto = new EditQuestionDto
                {
                    StarsCount = pStarsQuestion.StarsCount,
                    Text = pStarsQuestion.Text,
                    Order = pStarsQuestion.Order
                };

                var tResult = mService.EditQuestion((int)TempData["Id"], tEditQuestionDto);
                if (!tResult.Success)
                {
                    return View("Error", model: tResult.Error.ToString());
                }

                return RedirectToAction("Index", controllerName: "Home");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while editing stars question in EditStars action.");
                return View(viewName: "Error", model: nameof(UI_ERROR_MESSAGE));
            }
        }
    }
}