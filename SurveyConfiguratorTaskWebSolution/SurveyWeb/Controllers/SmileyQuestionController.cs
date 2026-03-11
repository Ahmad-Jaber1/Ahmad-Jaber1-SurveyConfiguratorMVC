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
    
    public class SmileyQuestionController : Controller
    {
        const string UI_ERROR_MESSAGE = "Error";
        private readonly IQuestionService mService;

        public SmileyQuestionController(IQuestionService pService)
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
                Log.Error(ex, "Unexpected error occurred while create SmileyQuestionController instance.");

            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSmileyQuestion(SmileyQuestionViewModel pSmileyQuestion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("SmileyView");
                }

                if (TempData["BasicQuestion"] == null)
                {
                    return View("Add");
                }

                var tBasicQuestion = (BasicQuestionViewModel)TempData["BasicQuestion"];
                AddQuestionDto tAddQuestionDto = new AddQuestionDto
                {
                    SmileyCount = pSmileyQuestion.SmileyCount,
                    Text = tBasicQuestion.Text,
                    Order = tBasicQuestion.Order
                };

                var tResult = mService.AddQuestion(TypeQuestionEnum.SmileyFacesQuestion, tAddQuestionDto);
                if (!tResult.Success)
                {
                    return View("Error",model: tResult.Error.ToString());
                }

                return RedirectToAction("Index" , controllerName: "Home");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while adding smiley question in AddSmileyQuestion action.");
                return View(viewName: "Error", model: nameof(UI_ERROR_MESSAGE));
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSmiley(SmileyQuestionViewModel pSmileyQuestion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("EditSmileyQuestion");
                }

                var tEditQuestionDto = new EditQuestionDto
                {
                    SmileyCount = pSmileyQuestion.SmileyCount,
                    Text = pSmileyQuestion.Text,
                    Order = pSmileyQuestion.Order
                };

                var tResult = mService.EditQuestion((int)TempData["Id"], tEditQuestionDto);
                if (!tResult.Success)
                {
                    return View("Error", model: tResult.Error.ToString());
                }

                return RedirectToAction("Index" , controllerName: "Home");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while editing smiley question in EditSmiley action.");
                return View(viewName: "Error", model: nameof(UI_ERROR_MESSAGE));
            }
        }
    }
}