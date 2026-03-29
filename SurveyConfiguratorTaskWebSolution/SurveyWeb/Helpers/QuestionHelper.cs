using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using SurveyConfiguratorTask.Models;
using SurveyConfiguratorTaskWeb.Models;
using SurveyWeb.Hubs;
using SurveyWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Helper
{
    public  class QuestionHelper
    {
        public static string SerializeToRawData(Question pQuestion)
        {
            object obj = null;

            switch (pQuestion.TypeQuestion)
            {
                case TypeQuestionEnum.SliderQuestion:
                    var tSlider = (SliderQuestion)pQuestion;
                    obj = new
                    {
                        StartValue = tSlider.StartValue,
                        EndValue = tSlider.EndValue,
                        StartCaption = tSlider.StartCaption,
                        EndCaption = tSlider.EndCaption
                    };
                    break;

                case TypeQuestionEnum.StarsQuestion:
                    var tStars = (StarsQuestion)pQuestion;
                    obj = new
                    {
                        StarsCount = tStars.StarsCount
                    };
                    break;

                case TypeQuestionEnum.SmileyFacesQuestion:
                    var tSmiley = (SmileyFacesQuestion)pQuestion;
                    obj = new
                    {
                        SmileyCount = tSmiley.SmileyCount
                    };
                    break;
            }

            return JsonConvert.SerializeObject(obj);
        }
        public static BasicQuestionViewModel Deserialize(QuestionFormViewModel pQuestionFormViewModel )
        {
            BasicQuestionViewModel tQuestion = null; 
            switch (pQuestionFormViewModel.QuestionType)
            {
                case TypeQuestionEnum.SliderQuestion:
                    tQuestion = JsonConvert.DeserializeObject<SliderQuestionViewModel>(pQuestionFormViewModel.RawData);
                    break;
                case TypeQuestionEnum.StarsQuestion:
                    tQuestion = JsonConvert.DeserializeObject<StarsQuestionViewModel>(pQuestionFormViewModel.RawData);
                    break;
                case TypeQuestionEnum.SmileyFacesQuestion:
                    tQuestion = JsonConvert.DeserializeObject<SmileyQuestionViewModel>(pQuestionFormViewModel.RawData);
                    break;
            }
            tQuestion.Text = pQuestionFormViewModel.Text; 
            tQuestion.Order = pQuestionFormViewModel.Order;
            tQuestion.QuestionType = pQuestionFormViewModel.QuestionType;
            return tQuestion;
        }
        public static bool Validate(BasicQuestionViewModel pQuestion , out List<ValidationResult> pValidationResults)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(pQuestion, serviceProvider: null, items: null);
            bool isValid = Validator.TryValidateObject(pQuestion, context, validationResults, true);
            pValidationResults = validationResults;
            return isValid;
        }
        public static void NotifyQuestionsUpdated()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<QuestionHub>();
            context.Clients.All.refreshQuestions();
        }

    }
}
