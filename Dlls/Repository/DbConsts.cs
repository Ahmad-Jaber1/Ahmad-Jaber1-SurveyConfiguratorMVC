using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public static class DbConsts
    {
        
        public const string SLIDER_TABLE_NAME = "SliderQuestion";
        public const string SLIDER_ID = "Id";
        public const string SLIDER_START_VALUE = "StartValue";
        public const string SLIDER_END_VALUE = "EndValue";
        public const string SLIDER_START_CAPTION = "StartCaption";
        public const string SLIDER_END_CAPTION = "EndCaption";
        public const string SLIDER_SELECT_COLUMNS = $"{SLIDER_TABLE_NAME}.{SLIDER_START_VALUE}, " +
            $"{SLIDER_TABLE_NAME}.{SLIDER_END_VALUE}, " +
            $"{SLIDER_TABLE_NAME}.{SLIDER_START_CAPTION}, " +
            $"{SLIDER_TABLE_NAME}.{SLIDER_END_CAPTION} ";
        public const string SMILEY_TABLE_NAME = "SmileyFacesQuestion";
        public const string SMILEY_ID = "Id";
        public const string SMILEY_SMILEY_COUNT = "SmileyCount";
        public const string SMILEY_SELECT_COLUMNS = $"{SMILEY_TABLE_NAME}.{SMILEY_SMILEY_COUNT}";

        public const string STARS_TABLE_NAME = "StarsQuestion";
        public const string STARS_ID = "Id";
        public const string STARS_STARS_COUNT = "StarsCount";
        public const string STARS_SELECT_COLUMNS = $"{STARS_TABLE_NAME}.{STARS_STARS_COUNT}";
        
        public const string DATABASE_CHANGE_TABLE_NAME = "DatabaseChangeTracker";
        public const string DATABASE_CHANGE_LAST_MODIFIED = "LastModified";

        public const string QUESTIONS_TABLE_NAME = "Questions";
        public const string QUESTIONS_ID = "Id";
        public const string QUESTIONS_QUESTIONS_TEXT = "QuestionText";
        public const string QUESTIONS_QUESTIONS_ORDER = "QuestionOrder";
        public const string QUESTIONS_QUESTIONS_TYPE = "QuestionType";
        public const string QUESTIONS_SELECT_COLUMNS = $"{QUESTIONS_TABLE_NAME}.{QUESTIONS_ID}," +
            $"{QUESTIONS_TABLE_NAME}.{QUESTIONS_QUESTIONS_TEXT}," +
            $"{QUESTIONS_TABLE_NAME}.{QUESTIONS_QUESTIONS_ORDER} ";
    }
}
