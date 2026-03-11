using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurveyConfiguratorTask.Models
{
    /// <summary>
    /// Represent information type of question : Stars .
    /// </summary>
    public class StarsQuestion : Question
    {
        public int StarsCount { get; private set; }

        //Create StarsQuestion object with new unique identifier.
        public StarsQuestion(string pText, int pOrder,int pCount)
            :base(pText ,  pOrder,TypeQuestionEnum.StarsQuestion )
        {
            try
            {
                SetStarsCount(pCount);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while create stars question object.");

                throw;
            }
        }
        //Initializes a StarsQuestion object from an existing data source.
        public StarsQuestion(int pId,string pText , int pOrder , int pStarsCount)
            :base(pId , pText , pOrder)
        {
            try
            {
                TypeQuestion = TypeQuestionEnum.StarsQuestion;
                SetStarsCount(pStarsCount);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while create stars question object.");

                throw;
            }
        }

        public void SetStarsCount(int pCount)
        {
            try
            {
                //StarsCount must not be less than 1 or greater than 10
                if (pCount <= 0 || pCount > 10)
                {
                    throw new ArgumentOutOfRangeException("Stars  must be between 1 and 10.");
                }
                StarsCount = pCount;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while set stars count .");

                throw;
            }
        }

        
        

    }
}
