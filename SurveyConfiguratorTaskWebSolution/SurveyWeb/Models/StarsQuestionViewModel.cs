using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SurveyWeb.Resources;

namespace SurveyConfiguratorTaskWeb.Models
{
    public class StarsQuestionViewModel
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

        [Range(1,10)]
        [Display(Name ="STARS_COUNT", ResourceType = typeof(Resources))]
        public int StarsCount { get; set; }
    }
}