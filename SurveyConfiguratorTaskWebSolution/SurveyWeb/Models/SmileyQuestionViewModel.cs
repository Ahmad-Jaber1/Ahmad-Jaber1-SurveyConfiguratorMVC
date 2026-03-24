using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Models;
using SurveyWeb.Resources;

namespace SurveyConfiguratorTaskWeb.Models
{
    public class SmileyQuestionViewModel : BasicQuestionViewModel
    {
        

        [Required]
        [Range(2,5 , ErrorMessageResourceName ="ERROR_RANGE_SMILEY" , ErrorMessageResourceType =typeof(Resources)) ]
        [Display(Name ="SMILEY_COUNT", ResourceType = typeof(Resources)) ]
        
        public int SmileyCount { get; set; }

        public override AddQuestionDto MapToDto()
        {

            return new AddQuestionDto { Text = this.Text, Order = this.Order, SmileyCount = this.SmileyCount };
        }
    }
}