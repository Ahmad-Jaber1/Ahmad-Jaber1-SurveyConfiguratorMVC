using SurveyConfiguratorTask.Models;
using SurveyConfiguratorTaskWeb.Models;
using SurveyWeb.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SurveyWeb.Models
{
    public class QuestionFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(60)]
        [Display(Name = "QUESTION_TEXT", ResourceType = typeof(Resources.Resources))]
        public string Text { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "QUESTION_ORDER", ResourceType = typeof(Resources.Resources))]

        public int Order { get; set; }
        [Required]
        [Display(Name = "QUESTION_TYPE", ResourceType = typeof(Resources.Resources))]

        public TypeQuestionEnum QuestionType { get; set; }

        public string RawData { get; set; }


    }
}