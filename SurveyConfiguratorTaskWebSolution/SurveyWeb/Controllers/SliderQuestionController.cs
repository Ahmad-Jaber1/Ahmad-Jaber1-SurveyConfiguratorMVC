using Models;
using Serilog;
using Services;
using SurveyConfiguratorTask.Models;
using SurveyConfiguratorTaskWeb.Models;
using SurveyWeb.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace testTaskConfigurator.Controllers
{
    public class SliderQuestionController : Controller
    {
        const string UI_ERROR_MESSAGE = "Error";
        private readonly IQuestionService mService;

        public SliderQuestionController(IQuestionService pService)
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
                Log.Error(ex, "Unexpected error occurred while create SliderQuestionController instance.");

            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSliderQuestion(SliderQuestionViewModel pSliderQuestion)
        {
            try
            {
                if (pSliderQuestion.StartValue >= pSliderQuestion.EndValue)
                {
                    ModelState.AddModelError("StartValue", Resources.ERROR_START_VALUE_GREATER);
                    ModelState.AddModelError("EndValue", Resources.ERROR_END_VALUE_LESS);
                }

                if (!ModelState.IsValid)
                {
                    return View("SliderView");
                }

                if (TempData["BasicQuestion"] == null)
                {
                    return RedirectToAction("Index", controllerName: "Home");
                }

                var tBasicQuestion = (BasicQuestionViewModel)TempData["BasicQuestion"];
                AddQuestionDto tAddQuestionDto = new AddQuestionDto
                {
                    StartValue = pSliderQuestion.StartValue,
                    EndValue = pSliderQuestion.EndValue,
                    StartCaption = pSliderQuestion.StartCaption,
                    EndCaption = pSliderQuestion.EndCaption,
                    Text = tBasicQuestion.Text,
                    Order = tBasicQuestion.Order
                };

                var tResult = mService.AddQuestion(TypeQuestionEnum.SliderQuestion, tAddQuestionDto);
                if (!tResult.Success)
                {
                    return View("Error", model: tResult.Error.ToString());
                }

                return RedirectToAction("Index", controllerName: "Home");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while adding slider question in AddSliderQuestion action.");
                return View(viewName: "Error", model: nameof(UI_ERROR_MESSAGE));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult EditSlider(SliderQuestionViewModel pSliderQuestion)
        {
            try
            {
                if (pSliderQuestion.StartValue >= pSliderQuestion.EndValue)
                {
                    ModelState.AddModelError("StartValue", Resources.ERROR_START_VALUE_GREATER);
                    ModelState.AddModelError("EndValue", Resources.ERROR_END_VALUE_LESS);
                }

                if (!ModelState.IsValid)
                {
                    return View("EditSliderQuestion");
                }

                var tEditQuestionDto = new EditQuestionDto
                {
                    StartValue = pSliderQuestion.StartValue,
                    EndValue = pSliderQuestion.EndValue,
                    StartCaption = pSliderQuestion.StartCaption,
                    EndCaption = pSliderQuestion.EndCaption,
                    Text = pSliderQuestion.Text,
                    Order = pSliderQuestion.Order
                };

                var tResult = mService.EditQuestion((int)TempData["Id"], tEditQuestionDto);
                if (!tResult.Success)
                {
                    return View("Error", model: tResult.Error.ToString());
                }

                return RedirectToAction("Index",controllerName:"Home");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while editing slider question in EditSlider action.");
                return View(viewName: "Error", model: nameof(UI_ERROR_MESSAGE));
            }
        }
    }
}