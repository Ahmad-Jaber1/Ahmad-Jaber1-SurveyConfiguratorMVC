using Models;
using SurveyConfiguratorTask.Models;
using SurveyWeb.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SurveyConfiguratorTaskWeb.Models
{
    public class SmileyQuestionViewModel : BasicQuestionViewModel
    {
        

        [Required]
        [Range(2,5 , ErrorMessageResourceName ="ERROR_RANGE_SMILEY" , ErrorMessageResourceType =typeof(Resources)) ]
        [Display(Name ="SMILEY_COUNT", ResourceType = typeof(Resources)) ]
        
        public int SmileyCount { get; set; }

        public override AddQuestionDto MapToAddDto()
        {

            return new AddQuestionDto { Text = this.Text, Order = this.Order, SmileyCount = this.SmileyCount };
        }
        public override EditQuestionDto MapToEditDto()
        {

            return new EditQuestionDto { Text = this.Text, Order = this.Order, SmileyCount = this.SmileyCount };
        }

        public override BasicQuestionViewModel MapToQuestionViewModel(Question pQuesiton)
        {
            var tSmiley = (SmileyFacesQuestion)pQuesiton;
            return new SmileyQuestionViewModel
            {
                Text = tSmiley.Text,
                Order = tSmiley.Order,
                QuestionType = tSmiley.TypeQuestion,
                SmileyCount = tSmiley.SmileyCount
            };
        }
    }
}