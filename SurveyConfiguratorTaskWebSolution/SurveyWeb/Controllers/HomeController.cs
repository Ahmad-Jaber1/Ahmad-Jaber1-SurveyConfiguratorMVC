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
        const string UI_ERROR_MESSAGE ="Error"; 
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

                    return View(viewName: "Error", model: tQuestionList.Error.ToString());
                }
                return View(model: tQuestionList);
            }
            catch (Exception ex)
            {

                Log.Error(ex, "Unexpected error occurred while loading questions in Index action.");
                return View(viewName: "Error",model: nameof(UI_ERROR_MESSAGE) );
            }

        }
        [HttpGet]
        public ActionResult Add()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while loading Add action.");
                return View(viewName: "Error", model: nameof(UI_ERROR_MESSAGE));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBasicQuestoinInfo(BasicQuestionViewModel pQuestion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Add", pQuestion);
                }

                TempData["BasicQuestion"] = pQuestion;

                switch (pQuestion.QuestionType)
                {
                    case TypeQuestionEnum.SliderQuestion:
                        TempData["QuestionType"] = TypeQuestionEnum.SliderQuestion;
                        return View("~/Views/SliderQuestion/SliderView.cshtml");

                    case TypeQuestionEnum.SmileyFacesQuestion:
                        TempData["QuestionType"] = TypeQuestionEnum.SmileyFacesQuestion;
                        return View("~/Views/SmileyQuestion/SmileyView.cshtml");

                    case TypeQuestionEnum.StarsQuestion:
                        TempData["QuestionType"] = TypeQuestionEnum.StarsQuestion;
                        return View("~/Views/StarsQuestion/StarsView.cshtml");
                }

                return RedirectToAction("Index" , controllerName: "Home");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while adding basic question info in AddBasicQuestoinInfo action.");
                return View(viewName: "Error", model: nameof(UI_ERROR_MESSAGE));
            }
        }
        
       

        


        public ActionResult ConfirmDeletion(int id)
        {
            try
            {
                Result<Question> tQuestion = mService.GetQuestion(id);
                if (!tQuestion.Success)
                {
                    return View(viewName: "Error",model: tQuestion.Error.ToString());
                }

                return View(model: tQuestion.Data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Unexpected error occurred while fetching question with ID {id} in ConfirmDeletion action.");
                return View(viewName: "Error", model: nameof(UI_ERROR_MESSAGE));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Delete(int id)
        {
            try
            {
                var tResult = mService.DeleteQuestion(id);
                if (!tResult.Success)
                {
                    return View("Error", tResult.Error.ToString());
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Unexpected error occurred while deleting question with ID {id} in Delete action.");
                return View(viewName: "Error", model: nameof(UI_ERROR_MESSAGE));
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                Result<Question> tResult = mService.GetQuestion(id);
                if (!tResult.Success)
                {
                    return View("Error", model: tResult.Error.ToString());
                }

                TempData["Id"] = tResult.Data.Id;

                switch (tResult.Data.TypeQuestion)
                {
                    case TypeQuestionEnum.SliderQuestion:
                        var tSliderQuestion = (SliderQuestion)tResult.Data;
                        var tSliderViewModel = new SliderQuestionViewModel
                        {
                            Text = tSliderQuestion.Text,
                            Order = tSliderQuestion.Order,
                            StartValue = tSliderQuestion.StartValue,
                            EndValue = tSliderQuestion.EndValue,
                            StartCaption = tSliderQuestion.StartCaption,
                            EndCaption = tSliderQuestion.EndCaption
                        };
                        return View("~/Views/SliderQuestion/EditSliderQuestion.cshtml", model: tSliderViewModel);

                    case TypeQuestionEnum.SmileyFacesQuestion:
                        var tSmileyQuestion = (SmileyFacesQuestion)tResult.Data;
                        var tSmileyViewModel = new SmileyQuestionViewModel
                        {
                            Text = tSmileyQuestion.Text,
                            Order = tSmileyQuestion.Order,
                            SmileyCount = tSmileyQuestion.SmileyCount
                        };
                        return View("~/Views/SmileyQuestion/EditSmileyQuestion.cshtml", tSmileyViewModel);

                    case TypeQuestionEnum.StarsQuestion:
                        var tStarsQuestion = (StarsQuestion)tResult.Data;
                        var tStarsViewModel = new StarsQuestionViewModel
                        {
                            Text = tStarsQuestion.Text,
                            Order = tStarsQuestion.Order,
                            StarsCount = tStarsQuestion.StarsCount
                        };
                        return View("~/Views/StarsQuestion/EditStarsQuestion.cshtml", tStarsViewModel);
                }

                return View("Index");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Unexpected error occurred while editing question with ID {id} in Edit action.");
                return View(viewName: "Error", model: nameof(UI_ERROR_MESSAGE));
            }
        }

        

        

       


    }
}