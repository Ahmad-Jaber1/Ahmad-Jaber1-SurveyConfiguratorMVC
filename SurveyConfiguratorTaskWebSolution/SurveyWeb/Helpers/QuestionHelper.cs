using SurveyConfiguratorTask.Models;
using SurveyConfiguratorTaskWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SurveyWeb.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;

namespace Helper
{
    public  class QuestionHelper
    {
        public static BasicQuestionViewModel Deserialize(CreateQuestionViewModel pCreateQuestionViewModel )
        {
            BasicQuestionViewModel tQuestion = null; 
            switch (pCreateQuestionViewModel.QuestionType)
            {
                case TypeQuestionEnum.SliderQuestion:
                    tQuestion = JsonConvert.DeserializeObject<SliderQuestionViewModel>(pCreateQuestionViewModel.RawData);
                    break;
                case TypeQuestionEnum.StarsQuestion:
                    tQuestion = JsonConvert.DeserializeObject<StarsQuestionViewModel>(pCreateQuestionViewModel.RawData);
                    break;
                case TypeQuestionEnum.SmileyFacesQuestion:
                    tQuestion = JsonConvert.DeserializeObject<SmileyQuestionViewModel>(pCreateQuestionViewModel.RawData);
                    break;
            }
            tQuestion.Text = pCreateQuestionViewModel.Text; 
            tQuestion.Order = pCreateQuestionViewModel.Order;
            tQuestion.QuestionType = pCreateQuestionViewModel.QuestionType;
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
    }
}
