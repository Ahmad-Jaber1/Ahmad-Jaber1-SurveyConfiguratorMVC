using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Models;
using Serilog;
using Shared;
using SurveyConfiguratorTask.Models;
using SurveyConfiguratorTask.Repo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using System.Text;
using System.Threading;

namespace Services
{
    public class QuestionService : IQuestionService
    {
        QuestionRepo mRepo = new();
        List<Question> mQuestions;
        private bool mConnectionValid;
        public event Action CheckUpdateEvent;
        private DateTime mDateTime = DateTime.Now;
        Thread mCheckForUpdate;
        bool mIsRunning = true;




        public QuestionService()
        {
            try
            {
                mQuestions = new List<Question>();
                CreateCheckThread();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while create Question Service object.");
                throw;

            }

        }

        public Result<List<Question>> QuestionsLoad()
        {
            try
            {
                Result<List<Question>> tResult = new Result<List<Question>> { Success = true, Error = ErrorTypeEnum.None };

                tResult = mRepo.QuestionsLoad();
                if (!tResult.Success)
                {

                    return tResult;

                }
                mQuestions = mRepo.QuestionsLoad().Data;


                if (mQuestions is not null & mQuestions.Count > 0)
                    mQuestions.Sort();
                tResult.Data = mQuestions;
                return tResult;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while loading the question list.");

                return new Result<List<Question>>()
                {
                    Success = false,
                    Error = ErrorTypeEnum.UnknownErrorQuestionsLoad,
                    Message = "Unexpected error occurred while loading questions."
                };
            }

        }
        public Result<int> AddQuestion(TypeQuestionEnum pType, AddQuestionDto pQuestionDto)
        {
            try
            {
                Result<int> tResult = new Result<int> { Error = ErrorTypeEnum.None, Success = true, Data = 0 };

                if (pQuestionDto is null)
                    throw new ArgumentNullException(null, "Question data cannot be empty. Please provide a valid question.");

                int tQuestionCount = mRepo.GetCount().Data;


                Question tQuestion = null;

                switch (pType)
                {

                    case TypeQuestionEnum.SliderQuestion:
                        tQuestion = new SliderQuestion(pQuestionDto.Text, pQuestionDto.Order, pQuestionDto.StartValue
                            , pQuestionDto.EndValue, pQuestionDto.StartCaption, pQuestionDto.EndCaption);
                        break;
                    case TypeQuestionEnum.SmileyFacesQuestion:
                        tQuestion = new SmileyFacesQuestion(pQuestionDto.Text, pQuestionDto.Order, pQuestionDto.SmileyCount);
                        break;
                    case TypeQuestionEnum.StarsQuestion:
                        tQuestion = new StarsQuestion(pQuestionDto.Text, pQuestionDto.Order, pQuestionDto.StarsCount);
                        break;
                    _:
                        throw new ArgumentOutOfRangeException(null, "The selected question type is not valid. Please choose a supported type.");
                        break;

                }
                // Validate that the user-selected order does not exceed the total number of mQuestions
                var tCountResult = GetCount();
                if (!tCountResult.Success)
                    return tCountResult;


                if (pQuestionDto.Order <= 0)
                {
                    throw new ArgumentOutOfRangeException(null, $"Order value must be non negative number. Please enter a valid number.");
                }


                mQuestions.Add(tQuestion);

                //Add new question to database.

                tResult = mRepo.AddQuestion(tQuestion);
                if (!tResult.Success)
                    return tResult;



                mRepo.UpdateLastModified();
                return tResult;


            }

            catch (ArgumentNullException ex)
            {
                Log.Error(ex, "Validation failed: null value.");

                return new Result<int>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.Validation_QuestionDataNull
                };
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Log.Error(ex, "Validation failed: value out of range.");

                return new Result<int>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.Validation_OrderOutOfRangeAdd
                };
            }
            catch (ArgumentException ex)
            {
                Log.Error(ex, "Validation failed: invalid argument.");

                return new Result<int>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.Validation_QuestionTypeInvalid
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while adding a question of type {Type}", (TypeQuestionEnum)pType);

                return new Result<int>()
                {
                    Success = false,
                    Error = ErrorTypeEnum.UnknownErrorAddQuestion,
                    Message = "An unexpected error occurred while adding your question. Please try again or contact support."
                };
            }
        }

        public Result<int> DeleteQuestion(int pId)
        {

            try
            {
                Result<int> tResult = new Result<int>();
                tResult.Success = true;
                tResult.Error = ErrorTypeEnum.None;
                tResult.Data = 0;
                Question deletedQuestion = null;
                foreach (var tQuestion in mQuestions)
                {
                    if (tQuestion.Id == pId)
                    {

                        mRepo.DeleteQuestion(pId);
                        if (!tResult.Success)
                            return tResult;
                        mQuestions.Remove(tQuestion);


                        // Reorder mQuestions to maintain consistent ordering after deletion
                        //tResult = EditOrder();
                        //if (!tResult.Success)
                        //    return tResult; 

                        deletedQuestion = tQuestion;
                        mRepo.UpdateLastModified();

                        break;
                    }

                }
                if (deletedQuestion is null)
                    throw new KeyNotFoundException("Question not found.");
                return tResult;


            }
            catch (KeyNotFoundException ex)
            {
                Log.Error(ex, "Attempted to delete a question that does not exist. Id: {Id}", pId);

                return new Result<int>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.NotFound_DeleteQuestion
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while deleting a question with Id {Id}", pId);

                return new Result<int>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.UnknownErrorDeleteQuestion
                };
            }
        }
        public Result<int> EditQuestion(int pId, EditQuestionDto pEditQuestionDto)
        {
            try
            {
                Result<int> tResult = new Result<int> { Success = true, Error = ErrorTypeEnum.None, Data = 0 };

                if (pEditQuestionDto is null)
                    throw new ArgumentNullException(null, "Please provide a question. This field cannot be empty.");


                Question question = null;
                //var getQuestionResult  = mRepo.GetQuestion(id);
                //if (!getQuestionResult.Success)
                //{
                //    tResult.Error = ErrorTypeEnum.UnknownError;
                //    tResult.Message = getQuestionResult.Message;
                //    tResult.Success = false; 
                //    return tResult;
                //}

                foreach (var tItem in mQuestions)
                {
                    if (tItem.Id == pId)
                    {
                        question = tItem;
                        break;
                    }
                }
                if (question is null)
                    throw new KeyNotFoundException("The specified question was not found. Please check your selection.");


                Question questionEdit = null;
                switch (question.TypeQuestion)
                {

                    case TypeQuestionEnum.SliderQuestion:

                        questionEdit = new SliderQuestion(question.Id, pEditQuestionDto.Text, pEditQuestionDto.Order, pEditQuestionDto.StartValue
                            , pEditQuestionDto.EndValue, pEditQuestionDto.StartCaption, pEditQuestionDto.EndCaption);
                        break;
                    case TypeQuestionEnum.SmileyFacesQuestion:
                        questionEdit = new SmileyFacesQuestion(question.Id, pEditQuestionDto.Text, pEditQuestionDto.Order, pEditQuestionDto.SmileyCount);
                        break;
                    case TypeQuestionEnum.StarsQuestion:
                        questionEdit = new StarsQuestion(question.Id, pEditQuestionDto.Text, pEditQuestionDto.Order, pEditQuestionDto.StarsCount);
                        break;


                }
                //tResult = GetCount();
                //if (!tResult.Success)
                //    return tResult; 
                // Validate that the user-selected order does not exceed the total number of mQuestions

                if (questionEdit.Order <= 0)
                {
                    throw new ArgumentOutOfRangeException(null, $"Order value must be non negative number . Please enter a valid number.");

                }


                tResult = mRepo.EditQuestion(questionEdit);
                if (!tResult.Success)
                    return tResult;

                mQuestions.Remove(question);
                mQuestions.Add(questionEdit);

                // Reorder all mQuestions only when the question's order has been changed
                //if (pEditQuestionDto.Order != 0)
                //{
                //    tResult = EditOrder();
                //    if(!tResult.Success)
                //            return tResult;
                //}
                mRepo.UpdateLastModified();

                return tResult;

            }
            catch (KeyNotFoundException ex)
            {
                Log.Error(ex, "Attempted to edit a question a question with Id {Id}", pId);

                return new Result<int>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.NotFound_EditQuestion
                };
            }
            catch (ArgumentNullException ex)
            {
                Log.Error(ex, "Validation failed: null value.");

                return new Result<int>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.Validation_EditQuestionDataNull
                };
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Log.Error(ex, "Validation failed: value out of range.");

                return new Result<int>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.Validation_OrderOutOfRangeEdit
                };
            }
            catch (ArgumentException ex)
            {
                Log.Error(ex, "Validation failed: invalid argument.");

                return new Result<int>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.Validation_QuestionTypeInvalid
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while edit a question a question with Id {Id}", pId);

                return new Result<int>()
                {
                    Success = false,
                    Error = ErrorTypeEnum.UnknownErrorEditQuestion,
                    Message = "An unexpected error occurred while edit your question. Please try again or contact support."
                };
            }


        }
        public Result<int> EditOrder()
        {

            try
            {
                Result<int> tResult = new Result<int>();
                tResult.Success = true;
                tResult.Error = ErrorTypeEnum.None;
                mQuestions.Sort();
                var tIds = new List<int>();
                var tOrders = new List<int>();

                for (int i = 0; i < mQuestions.Count; i++)
                {
                    tIds.Add(mQuestions[i].Id);
                    tOrders.Add(i + 1);
                    mQuestions[i].Order = i + 1;
                }
                if (mRepo.GetCount().Data > 0)
                {
                    tResult = mRepo.EditOrder(tIds, tOrders);
                    if (!tResult.Success)
                        return tResult;
                    mRepo.UpdateLastModified();

                }
                return tResult;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while editing the order of mQuestions.");

                return new Result<int>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.UnknownErrorEditOrder
                };
            }
        }
        public List<Question> GetQuestionsList()
        {

            try
            {
                return mQuestions;
            }
            catch (Exception ex)
            {

                Log.Error(ex, "Unexpected error occurred while get questions list.");
                throw;
            }
        }

        public Result<Question> GetQuestion(int pId)
        {
            try
            {
                Result<Question> tResult = new Result<Question> { Success = true, Error = ErrorTypeEnum.None };

                foreach (var tQuestion in mQuestions)
                {
                    if (tQuestion.Id == pId)
                    {
                        tResult.Data = tQuestion;
                        return tResult;

                    }
                }
                throw new KeyNotFoundException("The specified question was not found. Please check your selection.");


            }
            catch (KeyNotFoundException ex)
            {
                return new Result<Question>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.NotFound_GetQuestion
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving the question with Id {Id}", pId);

                return new Result<Question>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.UnknownErrorGetQuestion
                };
            }
        }

        public Result<int> GetCount()
        {
            try
            {
                Result<int> tResult = new Result<int> { Success = true, Error = ErrorTypeEnum.None, Data = 0 };

                tResult = mRepo.GetCount();
                return tResult;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving the question count from the repository.");

                return new Result<int>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.UnknownErrorGetCount
                };
            }
        }

        public Result<DateTime> GetLastModified()
        {

            try
            {
                Result<DateTime> tResult = new Result<DateTime>
                {
                    Success = true,
                    Error = ErrorTypeEnum.None,

                };
                tResult = mRepo.GetLastModified();
                return tResult;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving the last modified .");

                return new Result<DateTime>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.UnknownErrorGetLastModified
                };
            }

        }

        public Result<bool> ChangeConnectionString(string pConnectionString)
        {


            try
            {
                var tResult = new Result<bool>()
                {
                    Success = true,
                };
                tResult = mRepo.ChangeConnectionString(pConnectionString);
                mConnectionValid = tResult.Success;
                return tResult;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while Change Connection String  .");

                return new Result<bool>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.UnknownErrorChangeConnectionString
                };
            }

        }
        public Result<bool> CheckConnection()
        {
            try
            {
                var tResult = new Result<bool>()
                {
                    Success = true,
                };
                var tSavedConnection = ConfigurationManager.ConnectionStrings["DbConnectionString"]?.ConnectionString;

                if (!string.IsNullOrWhiteSpace(tSavedConnection))
                {
                    ChangeConnectionString(tSavedConnection);
                }
                tResult = new Result<bool>();
                if (mConnectionValid)
                {
                    tResult.Success = true;
                    tResult.Error = ErrorTypeEnum.None;
                }
                else if (!string.IsNullOrEmpty(tSavedConnection))
                {
                    tResult.Success = false;
                    tResult.Error = ErrorTypeEnum.DatabaseUnavailable;
                }
                else
                {
                    tResult.Success = false;
                    tResult.Message = "Database connection is not set or invalid.\n Go to Settings → Database Connection to set it up.";
                    tResult.Error = ErrorTypeEnum.ConnectionStringNotSet;
                }
                return tResult;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while Check Connection  .");

                return new Result<bool>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.UnknownErrorConnectionTest
                };
            }
        }

        public void CheckForUpdates()
        {

            try
            {
                while (mIsRunning)
                {
                    var tResult = GetLastModified();
                    if (tResult.Success && tResult.Data != mDateTime)
                    {
                        mDateTime = tResult.Data;
                        //Invoke(ReloadMainForm);
                        CheckUpdateEvent?.Invoke();
                    }

                    Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {

                //Log.Error(ex, "Unexpected error occurred while Check updates");
                throw;
            }

        }
        private void CreateCheckThread()
        {

            try
            {
                mCheckForUpdate = new Thread(CheckForUpdates)
                {
                    IsBackground = true
                };
                mCheckForUpdate.Start();
            }
            catch (Exception ex)
            {

                Log.Error(ex, "Unexpected error occurred while create check thread .");
                throw;
            }

        }
        public void FormClosing()
        {
            try
            {
                mIsRunning = false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while check if form closing");

            }
        }
        public Result<bool> ConnectionTest(string pConnectionString)
        {


            try
            {
                var tResult = new Result<bool>()
                {
                    Success = true,
                    Error = ErrorTypeEnum.None
                };
                tResult = mRepo.ConnectoinTest(pConnectionString);
                return tResult;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while testing connection.");

                return new Result<bool>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.UnknownErrorConnectionTest
                };
            }

        }

        public Result<SqlConnectionStringBuilder> GetConnectionString()
        {


            try
            {
                var tResult = new Result<SqlConnectionStringBuilder>()
                {
                    Success = true,
                    Error = ErrorTypeEnum.None
                };
                var tConnectionString = mRepo.GetConnectionString();

                if (!tConnectionString.Success || string.IsNullOrEmpty(tConnectionString.Data))
                {
                    tResult.Success = false;
                    tResult.Message = tConnectionString.Message;
                    tResult.Error = tConnectionString.Error;
                    return tResult;
                }

                var tSqlConnectionStringBuilder = new SqlConnectionStringBuilder(tConnectionString.Data);

                tResult.Data = tSqlConnectionStringBuilder;
                return tResult;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving the connection string.");

                return new Result<SqlConnectionStringBuilder>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.UnknownErrorGetConnectionString
                };
            }

        }

        public Result<bool> SetConnectionString(string pConnectionString)
        {
            try
            {
                var tResult = new Result<bool>() { Success = true, Error = ErrorTypeEnum.None };
                mRepo.SetConnectionString(pConnectionString);
                return tResult;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while set the connection string.");

                return new Result<bool>()
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ErrorTypeEnum.UnknownErrorChangeConnectionString
                };
            }
        }



    }
}
