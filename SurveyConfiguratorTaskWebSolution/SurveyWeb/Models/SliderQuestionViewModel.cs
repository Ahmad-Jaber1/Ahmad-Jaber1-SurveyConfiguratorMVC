using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SurveyWeb.Resources;

namespace SurveyConfiguratorTaskWeb.Models
{
    public class SliderQuestionViewModel
    {
        [Required]
        [MaxLength(60)]
        [Display(Name = "QUESTION_TEXT", ResourceType = typeof(Resources))]

        public string Text { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "QUESTION_ORDER", ResourceType = typeof(Resources))]

        public int Order { get; set; }
        [Required   ]

        [Display(Name = "START_VALUE" , ResourceType = typeof(Resources)) ]
        [Range(0,99)]
        public int StartValue { get; set; }
        [Required]

        [Display(Name = "END_VALUE", ResourceType = typeof(Resources))]
        [Range(1, 100)]
        public int EndValue { get; set; }
        [Required]

        [Display(Name = "START_CAPTION" , ResourceType = typeof(Resources))]
        public string StartCaption { get; set; }
        [Required]

        [Display(Name = "END_CAPTION", ResourceType = typeof(Resources))]
        public string EndCaption { get; set; }

        
    }
}