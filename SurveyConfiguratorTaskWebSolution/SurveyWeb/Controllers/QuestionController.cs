using Helper;
using Models;
using Newtonsoft.Json;
using Serilog;
using Services;
using Shared;
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
        const string ADD_VIEW = "Add";
        const string HOME_CONTROLLER ="Home";
        const string INDEX_ACTION = "Index";
        const string ERROR_VIEW = "Error";
        const string SLIDER_PARTIAL = "_SliderPartial";
        const string SMILEY_PARTIAL = "_SmileyPartial";
        const string STARS_PARTIAL = "_StarsPartial";
        const string SLIDER_QUESTION = "SliderQuestion";
        const string SMILEY_QUESTION = "SmileyFacesQuestion";
        const string STARS_QUESTION = "StarsQuestion";



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
            return View(new QuestionFormViewModel());
        }
        [HttpPost]
        public ActionResult CreateQuestion(QuestionFormViewModel pQuestion)
        {
            try
            {
                if (!ModelState.IsValid)
                {

                    return View(viewName: ADD_VIEW, pQuestion);
                }

                BasicQuestionViewModel tSpecificVM = QuestionHelper.Deserialize(pQuestion);

                bool isValid = QuestionHelper.Validate(tSpecificVM, out var pValidationResults);



                if (!isValid)
                {

                    foreach (var vr in pValidationResults)
                    {

                        ModelState.AddModelError("", vr.ErrorMessage);
                    }

                    return View(viewName: ADD_VIEW, pQuestion);
                }


                var tDto = tSpecificVM.MapToAddDto();


                mService.AddQuestion(pQuestion.QuestionType, tDto);
                QuestionHelper.NotifyQuestionsUpdated();

                return RedirectToAction(INDEX_ACTION, controllerName: HOME_CONTROLLER);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Unexpected error occurred while creating question .");
                return View(ERROR_VIEW, UI_ERROR_MESSAGE);
            }
        }


        public ActionResult ConfirmDeletion([Bind(Prefix = "id")] int pId)
        {
            try
            {
                Result<Question> tQuestion = mService.GetQuestion(pId);
                if (!tQuestion.Success)
                {
                    return View(viewName: ERROR_VIEW, model: tQuestion.Error.ToString());
                }

                return View(model: tQuestion.Data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Unexpected error occurred while fetching question with ID {pId} in ConfirmDeletion action.");
                return View(viewName: ERROR_VIEW, model: UI_ERROR_MESSAGE);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Delete([Bind(Prefix = "id")] int pId)
        {
            try
            {
                Result<Question> tQuestion = mService.GetQuestion(pId);
                if (!tQuestion.Success)
                {
                    return View(viewName: ERROR_VIEW, model: tQuestion.Error.ToString());
                }

                var tResult = mService.DeleteQuestion(pId);
                if (!tResult.Success)
                {
                    return View(ERROR_VIEW, tResult.Error.ToString());
                }
                QuestionHelper.NotifyQuestionsUpdated();


                return RedirectToAction(INDEX_ACTION, controllerName: HOME_CONTROLLER);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Unexpected error occurred while deleting question with ID {pId} in Delete action.");
                return View(viewName: ERROR_VIEW, model: UI_ERROR_MESSAGE);
            }
        }


        public ActionResult Edit([Bind(Prefix = "id")] int pId)
        {
            try
            {
                var tResult = mService.GetQuestion(pId);

                if (!tResult.Success)
                    return View(ERROR_VIEW, tResult.Error.ToString());

                var tQuestion = tResult.Data;

                var tViewModel = new QuestionFormViewModel
                {
                    Id = tQuestion.Id,
                    Text = tQuestion.Text,
                    Order = tQuestion.Order,
                    QuestionType = tQuestion.TypeQuestion,
                    RawData = QuestionHelper.SerializeToRawData(tQuestion) 
                };

                return View(ADD_VIEW, tViewModel); 
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error editing question {pId}");
                return View(ERROR_VIEW, UI_ERROR_MESSAGE);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(QuestionFormViewModel pQuestion)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(ADD_VIEW, pQuestion);

                var tViewModel = QuestionHelper.Deserialize(pQuestion);

                bool isValid = QuestionHelper.Validate(tViewModel, out var results);

                if (!isValid)
                {
                    foreach (var r in results)
                        ModelState.AddModelError("", r.ErrorMessage);

                    return View(ADD_VIEW, pQuestion);
                }

                var tDto = tViewModel.MapToEditDto();

                var tResult = mService.EditQuestion(pQuestion.Id, tDto);
                if (!tResult.Success)
                {
                    return View(ERROR_VIEW, tResult.Error.ToString());
                }
                QuestionHelper.NotifyQuestionsUpdated();

                return RedirectToAction(INDEX_ACTION, HOME_CONTROLLER);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Unexpected error occurred while editing question with ID {pQuestion?.Id }.");
                return View(ERROR_VIEW, UI_ERROR_MESSAGE);
            }
        }


        public ActionResult GetPartial([Bind(Prefix = "type")] string pType)
        {
            try
            {
                switch (pType)
                {
                    case SLIDER_QUESTION:
                        return PartialView(SLIDER_PARTIAL, new SliderQuestionViewModel());
                    case STARS_QUESTION:
                        return PartialView(STARS_PARTIAL, new StarsQuestionViewModel());
                    case SMILEY_QUESTION:
                        return PartialView(SMILEY_PARTIAL, new SmileyQuestionViewModel());
                    default:
                        return Content(""); 
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Unexpected error occurred while fetching partial view .");
                return Content(""); 
            }
        }

        public ActionResult GetQuestionsPartial()
        {
            Result<List<Question>> tQuestions = mService.QuestionsLoad(); 
            return PartialView("_QuestionsList", tQuestions );
        }
    }
}