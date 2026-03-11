using Serilog;
using System;
using System.Collections.Generic;
using System.Text;


namespace SurveyConfiguratorTask.Models
{
    /// <summary>
    /// Represent general info about question object .
    /// </summary>

    public   class Question : IComparable<Question>
    {
        public int Id { get; set; }
        public string Text { get;  set; }
        public int Order { get;  set;  }
        public TypeQuestionEnum TypeQuestion { get;  set; }

        public Question() { }

        //Create question object with new unique identifier.
        public Question(string pText , int pOrder,TypeQuestionEnum pType)
        {

            try
            {
                //Id = Guid.NewGuid();
                SetText(pText);
                Order = pOrder;
                TypeQuestion = pType;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while create Question object.");
                throw;
            }
        }
        //Initializes a Question object from an existing data source.
        public Question(int pId, string pText, int pOrder)
        {
            try
            {
                Id = pId;
                SetText(pText);
                Order = pOrder;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while create Question object.");

                throw;
            }
           
        }
        public void SetText(string pText)
        {
            try
            {
                //Text must not be null or empty.
                if (pText is null)
                    throw new ArgumentNullException("The text cannot be empty. Please enter a valid value.");
                if (pText == string.Empty)
                    throw new ArgumentException("The text cannot be empty. Please enter a valid value.");

                Text = pText;

            }
            catch(ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }

            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while set Text .");

                throw;
            }
            
        }
        protected void SetOrder(int pOrder)
        {
            try
            {
                Order = pOrder;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while set Order .");

                throw;
            }
        }
        
        
        

        //For sort question list based on order.
        int IComparable<Question>.CompareTo(Question other)
        {
            try
            {
                return Order.CompareTo(other.Order);
            }
            catch (Exception ex)
            {

                Log.Error(ex, "Unexpected error occurred while comper questions based on order");
                throw;
            }
        }
    }
}
