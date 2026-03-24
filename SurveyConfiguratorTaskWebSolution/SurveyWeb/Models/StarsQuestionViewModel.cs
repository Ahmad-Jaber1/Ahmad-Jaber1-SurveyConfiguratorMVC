using Models;
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

        public override AddQuestionDto MapToDto()
        {

            return new AddQuestionDto { Text = this.Text, Order = this.Order, StarsCount = this.StarsCount };
        }
    }
}