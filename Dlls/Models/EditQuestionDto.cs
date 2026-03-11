using System;
using System.Collections.Generic;
using System.Text;

namespace SurveyConfiguratorTask.Models
{
    public class EditQuestionDto
    {
        public string Text { get;  set; }
        public int Order { get; set; }
        public int StartValue { get;  set; } 
        public int EndValue { get;  set; } 
        public string StartCaption { get;  set; }
        public string EndCaption { get;  set; }

        public int StarsCount { get;  set; }

        public int SmileyCount { get;  set; }


    }
}
