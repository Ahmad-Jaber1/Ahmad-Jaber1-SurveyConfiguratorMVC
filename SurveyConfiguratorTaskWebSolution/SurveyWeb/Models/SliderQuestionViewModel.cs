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
    public class SliderQuestionViewModel : BasicQuestionViewModel
    {


        
        [Required]

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

        public override AddQuestionDto MapToDto()
        {

            return new AddQuestionDto { Text = this.Text, Order = this.Order, StartValue = this.StartValue ,
                StartCaption = this.StartCaption , EndCaption = this.EndCaption , EndValue = this.EndValue };
        }


    }
}