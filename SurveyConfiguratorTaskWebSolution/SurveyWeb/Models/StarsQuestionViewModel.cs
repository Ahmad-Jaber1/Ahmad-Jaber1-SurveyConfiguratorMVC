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
    public class StarsQuestionViewModel : BasicQuestionViewModel
    {
        
        [Required]

        [Range(1,10)]
        [Display(Name ="STARS_COUNT", ResourceType = typeof(Resources))]
        public int StarsCount { get; set; }

        public override AddQuestionDto MapToAddDto()
        {

            return new AddQuestionDto { Text = this.Text, Order = this.Order, StarsCount = this.StarsCount };
        }
        public override EditQuestionDto MapToEditDto()
        {

            return new EditQuestionDto { Text = this.Text, Order = this.Order, StarsCount = this.StarsCount };
        }

        public override BasicQuestionViewModel MapToQuestionViewModel(Question pQuesiton)
        {
            var tStars = (StarsQuestion)pQuesiton;
            return new StarsQuestionViewModel
            {
                Text = tStars.Text,
                Order = tStars.Order,
                QuestionType = tStars.TypeQuestion,
                StarsCount = tStars.StarsCount
            };
        }
    }
}