using SurveyConfiguratorTask.Models;
using SurveyConfiguratorTaskWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SurveyWeb.Resources;

namespace SurveyConfiguratorTaskWeb.Models
{
    public class BasicQuestionViewModel
    {
        [Required]
        [MaxLength(60)]
        [Display(Name = "QUESTION_TEXT", ResourceType = typeof(Resources))]
        public string Text { get; set; }
        [Required]
        [Range(1,int.MaxValue)]
        [Display(Name = "QUESTION_ORDER", ResourceType = typeof(Resources))]

        public int Order { get; set; }
        [Required ]
        [Display(Name = "QUESTION_TYPE", ResourceType = typeof(Resources))]

        public TypeQuestionEnum QuestionType { get; set; }
    }
}