using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurveyConfiguratorTask.Models
{
    /// <summary>
    /// Represent information type of question : Smiley Faces .
    /// </summary>
    public class SmileyFacesQuestion : Question
    {
        public int SmileyCount { get; private set; }

        //Create SmileyFacesQuestion object with new unique identifier.
        public SmileyFacesQuestion(string pText,int pOrder,int pCount) 
            : base(pText, pOrder,TypeQuestionEnum.SmileyFacesQuestion )
        {
            try
            {
                SetSmileyCount(pCount);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while create smiley question object.");

                throw;
            }
        }
        //Initializes a SmileyFacesQuestion object from an existing data source.
        public SmileyFacesQuestion(int pId, string pText , int pOrder , int pCount) 
            : base(pId, pText , pOrder )
        {

            try
            {
                SetSmileyCount(pCount);

                TypeQuestion = TypeQuestionEnum.SmileyFacesQuestion;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while create smiley question object.");

                throw;
            }
        }

        public void SetSmileyCount(int pCount)
        {
            try
            {
                //SmileyCount must not be less than 2 or greater than 5 
                if (pCount < 2 || pCount > 5)
                {
                    throw new ArgumentOutOfRangeException("Smiley faces  must be between 2 and 5.");

                }
                SmileyCount = pCount;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while set smiley count .");

                throw;
            }

        }

    }
}
