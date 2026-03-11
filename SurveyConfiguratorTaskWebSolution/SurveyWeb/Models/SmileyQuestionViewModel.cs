using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SurveyWeb.Resources;

namespace SurveyConfiguratorTaskWeb.Models
{
    public class SmileyQuestionViewModel
    {
        [Required]
        [MaxLength(60)]
        [Display(Name = "QUESTION_TEXT", ResourceType = typeof(Resources))]

        public string Text { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "QUESTION_ORDER", ResourceType = typeof(Resources))]

        public int Order { get; set; }

        [Required]
        [Range(2,5 , ErrorMessageResourceName ="ERROR_RANGE_SMILEY" , ErrorMessageResourceType =typeof(Resources)) ]
        [Display(Name ="SMILEY_COUNT", ResourceType = typeof(Resources)) ]
        
        public int SmileyCount { get; set; }
    }
}