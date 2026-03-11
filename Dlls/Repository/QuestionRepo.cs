using Microsoft.Data.SqlClient;
using Repository;
using Serilog;
using Shared;
using SurveyConfiguratorTask.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;

using System.Text;


namespace SurveyConfiguratorTask.Repo
{
    public class QuestionRepo
    {
        private  string? mConnectionString;
        public List<Question> Questions { get; set; }
        public QuestionRepo()
        {
            try
            {
                Questions = new List<Question>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while create Question Repository object.");
                throw;
            }
        }
        public Result<List<Question>> QuestionsLoad()
        {
            try
            {
                var tResult = new Result<List<Question>>() { Success = true, Error = ErrorTypeEnum.None, Data = Questions };

                if (string.IsNullOrWhiteSpace(mConnectionString))
                {
                    Log.Error("Attempted to load questions but the database connection string is not set. ");
                    return new Result<List<Question>>
                    {
                        Success = false,
                        Error = ErrorTypeEnum.ConnectionStringNotSet,

                    };
                }
                Questions.Clear();

                // Load Slider Questions
                var tSliderResult = SliderQuestionsLoad(mConnectionString);
                if (!tSliderResult.Success)
                {
                    Log.Error("Failed to load slider questions: {Message}", tSliderResult.Message);
                    return new Result<List<Question>>
                    {
                        Success = false,
                        Error = tSliderResult.Error,
                        Message = tSliderResult.Message
                    };
                }
                Questions.AddRange(tSliderResult.Data);
                

                // Load Smiley Questions
                var tSmileyResult = SmileyQuestionsLoad(mConnectionString);
                if (!tSmileyResult.Success)
                {
                    Log.Error("Failed to load smiley questions: {Message}", tSmileyResult.Message);
                    return new Result<List<Question>>
                    {
                        Success = false,
                        Error = tSmileyResult.Error,
                        Message = tSmileyResult.Message
                    };
                }
                Questions.AddRange(tSmileyResult.Data);


                // Load Stars Questions
                var tStarsResult = StarsQuestionsLoad(mConnectionString);
                if (!tStarsResult.Success)
                {
                    Log.Error("Failed to load star questions: {Message}", tStarsResult.Message);
                    return new Result<List<Question>>
                    {
                        Success = false,
                        Error = tStarsResult.Error,
                        Message = tStarsResult.Message
                    };
                }
                Questions.AddRange(tStarsResult.Data);
                return tResult;
            }
            catch (Exception ex)
            {
                return new Result<List<Question>>
                {
                    Success = false,
                    Error = ErrorTypeEnum.UnknownErrorQuestionsLoad,
                    Message = "Unexpected error occurred while loading questions."
                };
            }
        }
        public Result<List<Question>> SliderQuestionsLoad(string pConnectionString)
        {
            var tQuestions = new List<Question>();

            try
            {
                var tResult = new Result<List<Question>> { Success = true, Error = ErrorTypeEnum.None };

                using (var tConnection = new SqlConnection(pConnectionString))
                using (var tCommand = tConnection.CreateCommand())
                {
                    tCommand.CommandText = $"SELECT {DbConsts.QUESTIONS_SELECT_COLUMNS}, {DbConsts.SLIDER_SELECT_COLUMNS} " +
                                      $"FROM {DbConsts.QUESTIONS_TABLE_NAME} " +
                                      $"INNER JOIN {DbConsts.SLIDER_TABLE_NAME} " +
                                      $"ON {DbConsts.QUESTIONS_TABLE_NAME}.{DbConsts.QUESTIONS_ID} = {DbConsts.SLIDER_TABLE_NAME}.{DbConsts.SLIDER_ID}";

                    tConnection.Open();
                    using (var tReader = tCommand.ExecuteReader())
                    {
                        while (tReader.Read())
                        {
                            tQuestions.Add(new SliderQuestion(
                                Convert.ToInt32(tReader[DbConsts.QUESTIONS_ID]),
                                tReader[DbConsts.QUESTIONS_QUESTIONS_TEXT].ToString(),
                                Convert.ToInt32(tReader[DbConsts.QUESTIONS_QUESTIONS_ORDER]),
                                Convert.ToInt32(tReader[DbConsts.SLIDER_START_VALUE]),
                                Convert.ToInt32(tReader[DbConsts.SLIDER_END_VALUE]),
                                tReader[DbConsts.SLIDER_START_CAPTION].ToString(),
                                tReader[DbConsts.SLIDER_END_CAPTION].ToString()
                            ));
                        }
                    }
                    tConnection.Close();
                }

                tResult.Data = tQuestions;
                return tResult;

            }
            catch (SqlException sqlEx)
            {
                
                Log.Error(sqlEx, "SQL error while loading slider questions.");
                return new Result<List<Question>>
                {
                    Success = false,
                    Message = "SQL error occurred while loading slider questions.",
                    Error = ErrorTypeEnum.SqlErrorSliderLoad

                };

                
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while loading slider questions.");
                return new Result<List<Question>>
                {
                    Success = false,
                    Message = "Unexpected error occurred while loading slider questions.",
                    Error = ErrorTypeEnum.UnknownErrorSliderLoad
                };
                
            }
        }
        public Result<List<Question>> SmileyQuestionsLoad(string pConnectionString)
        {

            try
            {
                var tResult = new Result<List<Question>> { Success = true, Error = ErrorTypeEnum.None };

                var tQuestions = new List<Question>();

                using (var tConnection = new SqlConnection(pConnectionString))
                using (var tCommand = tConnection.CreateCommand())
                {
                    tCommand.CommandText = $"SELECT {DbConsts.QUESTIONS_SELECT_COLUMNS}, {DbConsts.SMILEY_SELECT_COLUMNS} " +
                                      $"FROM {DbConsts.QUESTIONS_TABLE_NAME} " +
                                      $"INNER JOIN {DbConsts.SMILEY_TABLE_NAME} " +
                                      $"ON {DbConsts.QUESTIONS_TABLE_NAME}.{DbConsts.QUESTIONS_ID} = {DbConsts.SMILEY_TABLE_NAME}.{DbConsts.SMILEY_ID}";

                    tConnection.Open();
                    using (var tReader = tCommand.ExecuteReader())
                    {
                        while (tReader.Read())
                        {
                            tQuestions.Add(new SmileyFacesQuestion(
                                Convert.ToInt32(tReader[DbConsts.QUESTIONS_ID]),
                                tReader[DbConsts.QUESTIONS_QUESTIONS_TEXT].ToString(),
                                Convert.ToInt32(tReader[DbConsts.QUESTIONS_QUESTIONS_ORDER]),
                                Convert.ToInt32(tReader[DbConsts.SMILEY_SMILEY_COUNT])
                            ));
                        }
                    }
                    tConnection.Close();
                }

                tResult.Data = tQuestions;
                return tResult;

            }
            
            catch (SqlException sqlEx)
            {
                Log.Error(sqlEx, "SQL error while loading smiley questions.");
                return new Result<List<Question>>
                {
                    Success = false,
                    Message = "SQL error occurred while loading smiley questions.",
                    Error = ErrorTypeEnum.SqlErrorSmileyLoad
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while loading smiley questions.");
                return new Result<List<Question>>
                {
                    Success = false,
                    Message = "Unexpected error occurred while loading smiley questions.",
                    Error = ErrorTypeEnum.UnknownErrorSmileyLoad
                };
            }


        }
        public Result<List<Question>> StarsQuestionsLoad(string pConnectionString)
        {
            

            try
            {
                var tResult = new Result<List<Question>> { Success = true, Error = ErrorTypeEnum.None };
                var tQuestions = new List<Question>();
                using (var tConnection = new SqlConnection(pConnectionString))
                using (var tCommand = tConnection.CreateCommand())
                {
                    tCommand.CommandText = $"SELECT {DbConsts.QUESTIONS_SELECT_COLUMNS}, {DbConsts.STARS_SELECT_COLUMNS} " +
                                      $"FROM {DbConsts.QUESTIONS_TABLE_NAME} " +
                                      $"INNER JOIN {DbConsts.STARS_TABLE_NAME} " +
                                      $"ON {DbConsts.QUESTIONS_TABLE_NAME}.{DbConsts.QUESTIONS_ID} = {DbConsts.STARS_TABLE_NAME}.{DbConsts.STARS_ID}";

                    tConnection.Open();
                    using (var tReader = tCommand.ExecuteReader())
                    {
                        while (tReader.Read())
                        {
                            tQuestions.Add(new StarsQuestion(
                                Convert.ToInt32(tReader[DbConsts.QUESTIONS_ID]),
                                tReader[DbConsts.QUESTIONS_QUESTIONS_TEXT].ToString(),
                                Convert.ToInt32(tReader[DbConsts.QUESTIONS_QUESTIONS_ORDER]),
                                Convert.ToInt32(tReader[DbConsts.STARS_STARS_COUNT])
                            ));
                        }
                    }
                    tConnection.Close();
                }

                tResult.Data = tQuestions;
                return tResult;

            }
            catch (SqlException sqlEx)
            {
                Log.Error(sqlEx, "SQL error while loading star questions.");
                return new Result<List<Question>>
                {
                    Success = false,
                    Message = "SQL error occurred while loading star questions.",
                    Error = ErrorTypeEnum.SqlErrorStarsLoad
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while loading star questions.");
                return new Result<List<Question>>
                {
                    Success = false,
                    Message = "Unexpected error occurred while loading star questions.",
                    Error = ErrorTypeEnum.UnknownErrorStarsLoad
                };
            }


        }
        public Result<int> AddQuestion(Question pQuestion)
        {
            try
            {
                Result<int> tResult = new Result<int>
                {
                    Success = true,
                    Error = ErrorTypeEnum.None
                };
                using (var tConnection = new SqlConnection(mConnectionString))
                {
                    tConnection.Open();

                    using (var tTransaction = tConnection.BeginTransaction())
                    {
                        try
                        {
                            using (var tCommand = tConnection.CreateCommand())
                            {
                                tCommand.Transaction = tTransaction;

                                // 1. Insert into Questions (Id is auto-generated)
                                tCommand.CommandText = $"INSERT INTO {DbConsts.QUESTIONS_TABLE_NAME} " +
                                                  $"({DbConsts.QUESTIONS_QUESTIONS_TEXT}, {DbConsts.QUESTIONS_QUESTIONS_ORDER}, {DbConsts.QUESTIONS_QUESTIONS_TYPE}) " +
                                                  $"VALUES (@{DbConsts.QUESTIONS_QUESTIONS_TEXT}, @{DbConsts.QUESTIONS_QUESTIONS_ORDER}, @{DbConsts.QUESTIONS_QUESTIONS_TYPE}); " +
                                                  "SELECT CAST(SCOPE_IDENTITY() AS INT);";

                                tCommand.Parameters.Add($"@{DbConsts.QUESTIONS_QUESTIONS_TEXT}", SqlDbType.NVarChar,60).Value = pQuestion.Text;
                                tCommand.Parameters.Add($"@{DbConsts.QUESTIONS_QUESTIONS_ORDER}", SqlDbType.Int).Value = pQuestion.Order;
                                tCommand.Parameters.Add($"@{DbConsts.QUESTIONS_QUESTIONS_TYPE}", SqlDbType.Int).Value = (int)pQuestion.TypeQuestion;

                                // Get the generated Question Id
                                int tQuestionId = (int)tCommand.ExecuteScalar();

                                // 2. Insert into the specific question type table
                                tCommand.Parameters.Clear();

                                switch (pQuestion.TypeQuestion)
                                {
                                    case TypeQuestionEnum.SliderQuestion:
                                        var tSliderQuestion = (SliderQuestion)pQuestion;
                                        tCommand.CommandText = $"INSERT INTO {DbConsts.SLIDER_TABLE_NAME} " +
                                                          $"({DbConsts.SLIDER_ID}, {DbConsts.SLIDER_START_VALUE}, {DbConsts.SLIDER_END_VALUE}, {DbConsts.SLIDER_START_CAPTION}, {DbConsts.SLIDER_END_CAPTION}) " +
                                                          $"VALUES (@{DbConsts.QUESTIONS_ID}, @{DbConsts.SLIDER_START_VALUE}, @{DbConsts.SLIDER_END_VALUE}, @{DbConsts.SLIDER_START_CAPTION}, @{DbConsts.SLIDER_END_CAPTION})";
                                        tCommand.Parameters.Add($"@{DbConsts.QUESTIONS_ID}", SqlDbType.Int).Value = tQuestionId;
                                        tCommand.Parameters.Add($"@{DbConsts.SLIDER_START_VALUE}", SqlDbType.Int).Value = tSliderQuestion.StartValue;
                                        tCommand.Parameters.Add($"@{DbConsts.SLIDER_END_VALUE}", SqlDbType.Int).Value = tSliderQuestion.EndValue;
                                        tCommand.Parameters.Add($"@{DbConsts.SLIDER_START_CAPTION}", SqlDbType.NVarChar, 30).Value = tSliderQuestion.StartCaption;
                                        tCommand.Parameters.Add($"@{DbConsts.SLIDER_END_CAPTION}", SqlDbType.NVarChar, 30).Value = tSliderQuestion.EndCaption;
                                        break;

                                    case TypeQuestionEnum.SmileyFacesQuestion:
                                        var tSmileyFaceQuestion = (SmileyFacesQuestion)pQuestion;
                                        tCommand.CommandText = $"INSERT INTO {DbConsts.SMILEY_TABLE_NAME} " +
                                                          $"({DbConsts.SMILEY_ID}, {DbConsts.SMILEY_SMILEY_COUNT}) " +
                                                          $"VALUES (@{DbConsts.QUESTIONS_ID}, @{DbConsts.SMILEY_SMILEY_COUNT})";
                                        tCommand.Parameters.Add($"@{DbConsts.QUESTIONS_ID}", SqlDbType.Int).Value = tQuestionId;
                                        tCommand.Parameters.Add($"@{DbConsts.SMILEY_SMILEY_COUNT}", SqlDbType.Int).Value = tSmileyFaceQuestion.SmileyCount;
                                        break;

                                    case TypeQuestionEnum.StarsQuestion:
                                        var tStarsQuestion = (StarsQuestion)pQuestion;
                                        tCommand.CommandText = $"INSERT INTO {DbConsts.STARS_TABLE_NAME} " +
                                                          $"({DbConsts.STARS_ID}, {DbConsts.STARS_STARS_COUNT}) " +
                                                          $"VALUES (@{DbConsts.QUESTIONS_ID}, @{DbConsts.STARS_STARS_COUNT})";
                                        tCommand.Parameters.Add($"@{DbConsts.QUESTIONS_ID}", SqlDbType.Int).Value = tQuestionId;
                                        tCommand.Parameters.Add($"@{DbConsts.STARS_STARS_COUNT}", SqlDbType.Int).Value = tStarsQuestion.StarsCount;
                                        break;

                                    default:
                                        throw new Exception("Unsupported question type.");
                                }

                                // Execute the insert for the child table
                                tCommand.ExecuteNonQuery();

                                // Commit transaction
                                tTransaction.Commit();

                                // Return the generated question Id
                                tResult.Data = tQuestionId;
                                return tResult;

                            }
                        }
                        catch (SqlException sqlEx)
                        {
                            tTransaction.Rollback();
                            Log.Error(sqlEx, "SQL error occurred while inserting question of type {Type}", pQuestion.TypeQuestion);
                            return new Result<int>
                            {
                                Success = false,
                                Message = "We couldn't save your question at the moment. Please try again later.",
                                Error = ErrorTypeEnum.SqlErrorAddQuestion
                            };
                        }
                        catch (Exception ex)
                        {
                            tTransaction.Rollback();
                            Log.Error(ex, "Unexpected error occurred while inserting question of type {Type}", pQuestion.TypeQuestion);
                            return new Result<int>
                            {
                                Success = false,
                                Message = "Unexpected error occurred while adding question.",
                                Error = ErrorTypeEnum.UnknownErrorAddQuestion
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Database connection failed.");
                return new Result<int>
                {
                    Success = false,
                    Message = "Failed to connect to the database. Contact the administrator to resolve the issue.",
                    Error = ErrorTypeEnum.DatabaseConnectionFailed
                };
            }


        }
        public Result<object> DeleteQuestion(int pId)
        {
            
            try
            {
                Result<object> tResult = new Result<object>();
                tResult.Success = true;
                tResult.Error = ErrorTypeEnum.None;
                using (var tConnection = new SqlConnection(mConnectionString))
                {
                    using (var tCommand = tConnection.CreateCommand())
                    {
                        tCommand.CommandText = $"DELETE FROM {DbConsts.QUESTIONS_TABLE_NAME} WHERE {DbConsts.QUESTIONS_ID} = @{DbConsts.QUESTIONS_ID}";
                        tCommand.Parameters.Add($"@{DbConsts.QUESTIONS_ID}", System.Data.SqlDbType.Int).Value = pId;


                        tConnection.Open();
                        tCommand.ExecuteNonQuery();
                        return tResult;

                    }


                }
            }
            catch (SqlException sqlEx)
            {
                Log.Error(sqlEx, "SQL error occurred while deleting question ,Id: {Id}", pId);
                return new Result<object>
                {
                    Success = false,
                    Message = "We couldn't delete your question at the moment. Please try again later.",
                    Error = ErrorTypeEnum.SqlErrorDeleteQuestion
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while deleting question. QuestionId: {Id}", pId);
                return new Result<object>
                {
                    Success = false,
                    Message = "We couldn't delete your question due to an unexpected error. Please try again or contact support.",
                    Error = ErrorTypeEnum.UnknownErrorDeleteQuestion
                };
            }

        }
        public Result<int> EditQuestion(Question pQuestion)
        {
           
            try
            {
                Result<int> tResult = new Result<int>();
                tResult.Success = true;
                tResult.Error = ErrorTypeEnum.None;
                tResult.Data = 0;
                using (var tConnection = new SqlConnection(mConnectionString))
                {
                    tConnection.Open();

                    using (var tTransaction = tConnection.BeginTransaction())
                    {


                        try
                        {
                            using (var tCommand = tConnection.CreateCommand())
                            {
                                //edit object data in general question table in database 
                                tCommand.CommandText = $"UPDATE {DbConsts.QUESTIONS_TABLE_NAME} " +
                                    $"SET {DbConsts.QUESTIONS_QUESTIONS_TEXT} = @{DbConsts.QUESTIONS_QUESTIONS_TEXT} " +
                                    $",{DbConsts.QUESTIONS_QUESTIONS_ORDER}=@{DbConsts.QUESTIONS_QUESTIONS_ORDER} "+
                                    $"WHERE {DbConsts.QUESTIONS_TABLE_NAME}.{DbConsts.QUESTIONS_ID} = @{DbConsts.QUESTIONS_ID}";
                                tCommand.Parameters.Add($"@{DbConsts.QUESTIONS_ID}", System.Data.SqlDbType.Int).Value = pQuestion.Id;
                                tCommand.Parameters.Add($"@{DbConsts.QUESTIONS_QUESTIONS_TEXT}", System.Data.SqlDbType.NVarChar,60).Value = pQuestion.Text;
                                tCommand.Parameters.Add($"@{DbConsts.QUESTIONS_QUESTIONS_ORDER}", System.Data.SqlDbType.Int).Value = pQuestion.Order;

                                tCommand.Transaction = tTransaction;
                                tCommand.ExecuteNonQuery();

                                //edit object data in specific question type 
                                switch (pQuestion.TypeQuestion)
                                {
                                    case TypeQuestionEnum.SliderQuestion:
                                        tCommand.CommandText = $"UPDATE {DbConsts.SLIDER_TABLE_NAME} " +
                                            $"SET {DbConsts.SLIDER_START_VALUE} = @{DbConsts.SLIDER_START_VALUE} ," +
                                            $"{DbConsts.SLIDER_END_VALUE} = @{DbConsts.SLIDER_END_VALUE} " +
                                            $", {DbConsts.SLIDER_START_CAPTION} =@{DbConsts.SLIDER_START_CAPTION} " +
                                            $", {DbConsts.SLIDER_END_CAPTION} = @{DbConsts.SLIDER_END_CAPTION}" +
                                            $" WHERE {DbConsts.SLIDER_TABLE_NAME}.{DbConsts.SLIDER_ID} = @{DbConsts.SLIDER_ID} ";

                                        var tSliderQuestion = (SliderQuestion)pQuestion;
                                        tCommand.Parameters.Add($"@{DbConsts.SLIDER_START_VALUE}", System.Data.SqlDbType.Int).Value = tSliderQuestion.StartValue;
                                        tCommand.Parameters.Add($"@{DbConsts.SLIDER_END_VALUE}", System.Data.SqlDbType.Int).Value = tSliderQuestion.EndValue;
                                        tCommand.Parameters.Add($"@{DbConsts.SLIDER_START_CAPTION}", System.Data.SqlDbType.NVarChar,30).Value = tSliderQuestion.StartCaption;
                                        tCommand.Parameters.Add($"@{DbConsts.SLIDER_END_CAPTION}", System.Data.SqlDbType.NVarChar,30).Value = tSliderQuestion.EndCaption;



                                        break;
                                    case TypeQuestionEnum.SmileyFacesQuestion:
                                        tCommand.CommandText = $"UPDATE {DbConsts.SMILEY_TABLE_NAME} " +
                                            $"SET {DbConsts.SMILEY_SMILEY_COUNT} = @{DbConsts.SMILEY_SMILEY_COUNT} " +
                                            $"WHERE {DbConsts.SMILEY_TABLE_NAME}.{DbConsts.SMILEY_ID} = @{DbConsts.SMILEY_ID} ";
                                        var tSmileyFaceQuestion = (Models.SmileyFacesQuestion)pQuestion;
                                        tCommand.Parameters.Add($"@{DbConsts.SMILEY_SMILEY_COUNT}", System.Data.SqlDbType.Int).Value = tSmileyFaceQuestion.SmileyCount;

                                        break;
                                    case TypeQuestionEnum.StarsQuestion:
                                        tCommand.CommandText = $"UPDATE {DbConsts.STARS_TABLE_NAME} " +
                                            $"SET {DbConsts.STARS_STARS_COUNT} = @{DbConsts.STARS_STARS_COUNT} " +
                                            $"WHERE {DbConsts.STARS_TABLE_NAME}.{DbConsts.STARS_ID} = @{DbConsts.STARS_ID} ";
                                        var tStarsQuestion = (StarsQuestion)pQuestion;
                                        tCommand.Parameters.Add($"@{DbConsts.STARS_STARS_COUNT}", System.Data.SqlDbType.Int).Value = tStarsQuestion.StarsCount;

                                        break;

                                }
                                tCommand.ExecuteNonQuery();


                            }
                            tTransaction.Commit();
                            tConnection.Close();
                            return tResult;

                        }
                        catch (SqlException ex)
                        {
                            tTransaction.Rollback();
                            Log.Error(ex, "SQL error occurred while updating question. QuestionId: {Id}", pQuestion.Id);
                            return new Result<int>
                            {
                                Success = false,
                                Message = "We couldn't update your question at the moment. Please try again later.",
                                Error = ErrorTypeEnum.SqlErrorUpdateLastModified
                            };
                        }
                        catch (Exception ex)
                        {
                            tTransaction.Rollback();
                            Log.Error(ex, "Unexpected error occurred while inserting question of type {Type}", pQuestion.TypeQuestion);
                            return new Result<int>
                            {
                                Success = false,
                                Message = "Unexpected error occurred while adding question.",
                                Error = ErrorTypeEnum.UnknownErrorUpdateLastModified
                            };
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Database connection failed.");
                return new Result<int>
                {
                    Success = false,
                    Message = "Failed to connect to the database. Contact the administrator to resolve the issue.",
                    Error = ErrorTypeEnum.DatabaseConnectionFailed
                };
            }

        }
        public Result<int> EditOrder(List<int> pIds, List<int> pOrders)
        {
           

            try
            {
                Result<int> tResult = new Result<int>();
                tResult.Success = true;
                tResult.Error = ErrorTypeEnum.None;
                using (var tConnection = new SqlConnection(mConnectionString))
                {

                    tConnection.Open();

                    using (var tTransaction = tConnection.BeginTransaction())
                    {
                        try
                        {
                            //edit order of all questions based on indexes in local list .
                            using (var tCommand = tConnection.CreateCommand())
                            {
                                var tStringBuilder = new StringBuilder();
                                tStringBuilder.Append($"UPDATE {DbConsts.QUESTIONS_TABLE_NAME} SET {DbConsts.QUESTIONS_QUESTIONS_ORDER} = CASE {DbConsts.QUESTIONS_ID} ");
                                for (int i = 0; i < pIds.Count; i++)
                                {
                                    tStringBuilder.Append($"WHEN '{pIds[i]}' THEN {pOrders[i]}\n");
                                }
                                tStringBuilder.Append($"\n END WHERE {DbConsts.QUESTIONS_ID} IN (");

                                for (int i = 0; i < pIds.Count - 1; i++)
                                    tStringBuilder.Append($"'{pIds[i]}',");
                                tStringBuilder.Append($"'{pIds[pIds.Count - 1]}'");

                                tStringBuilder.Append(");");
                                tCommand.CommandText = tStringBuilder.ToString();
                                tCommand.Transaction = tTransaction;
                                tCommand.ExecuteNonQuery();
                                tTransaction.Commit();
                                tConnection.Close();
                            }
                            return tResult;


                        }

                        catch (SqlException ex)
                        {
                            tTransaction.Rollback();
                            Log.Error(ex, "SQL error occurred while updating order. ");
                            return new Result<int>
                            {
                                Success = false,
                                Message = "We couldn't update order at the moment. Please try again later.",
                                Error = ErrorTypeEnum.SqlErrorEditOrder
                            };
                        }
                        catch (Exception ex)
                        {
                            tTransaction.Rollback();
                            Log.Error(ex, "Unexpected error occurred while updating order ");
                            return new Result<int>
                            {
                                Success = false,
                                Message = "Unexpected error occurred while update order.",
                                Error = ErrorTypeEnum.UnknownErrorEditOrder
                            };
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Database connection failed.");
                return new Result<int>
                {
                    Success = false,
                    Message = "Failed to connect to the database. Contact the administrator to resolve the issue.",
                    Error = ErrorTypeEnum.DatabaseConnectionFailed
                };
            }
        }
        public Result<int> GetCount()
        {
            try
            {
                Result<int> tResult = new Result<int> { Success = true, Error = ErrorTypeEnum.None, Data = 0 };

                // Get count of question in database 

                int tQuestionCount;
                using (var tConnection = new SqlConnection(mConnectionString))
                {
                    using (var tCommand = tConnection.CreateCommand())
                    {
                        tCommand.CommandText = $"SELECT COUNT({DbConsts.QUESTIONS_ID}) FROM Questions";



                        tConnection.Open();
                        var tReader = tCommand.ExecuteReader();
                        tReader.Read();
                        tQuestionCount = Convert.ToInt32(tReader[0]);
                        tResult.Data = tQuestionCount;

                        return tResult;

                    }


                }
            }
            catch (SqlException sqlEx)
            {
                Log.Error(sqlEx, "SQL error occurred while retrieving question count.");
                return new Result<int>
                {
                    Success = false,
                    Message = "We couldn't retrieve the question count at the moment. Please try again later.",
                    Error = ErrorTypeEnum.SqlErrorGetCount
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while retrieving question count.");
                return new Result<int>
                {
                    Success = false,
                    Message = "We couldn't retrieve the question count due to an unexpected error. Please contact support.",
                    Error = ErrorTypeEnum.UnknownErrorGetCount
                };
            }
        }
        public Result<Question> GetQuestion(int pId)
        {
            Question tQuestion = null;

            try
            {
                Result<Question> tResult = new Result<Question> { Success = true, Error = ErrorTypeEnum.None };

                //Get question object (general and specific type information ) based on Id 
                using (var tConnection = new SqlConnection(mConnectionString))
                {
                    using (var tCommand = tConnection.CreateCommand())
                    {

                        try
                        {

                            tCommand.CommandText = $"SELECT * FROM {DbConsts.QUESTIONS_TABLE_NAME} WHERE {DbConsts.QUESTIONS_ID} =@{DbConsts.QUESTIONS_ID}";
                            tCommand.Parameters.Add($"@{DbConsts.QUESTIONS_ID}", System.Data.SqlDbType.Int).Value = pId;

                            tConnection.Open();
                            var tReader = tCommand.ExecuteReader();
                            tReader.Read();

                            switch ((TypeQuestionEnum)tReader[DbConsts.QUESTIONS_QUESTIONS_TYPE])
                            {
                                case TypeQuestionEnum.SliderQuestion:
                                    tConnection.Close();
                                    tCommand.CommandText = $"SELECT {DbConsts.QUESTIONS_TABLE_NAME}.*,{DbConsts.SLIDER_TABLE_NAME}.* " +
                                        $"FROM {DbConsts.QUESTIONS_TABLE_NAME} INNER JOIN {DbConsts.SLIDER_TABLE_NAME}  " +
                                        $"ON {DbConsts.QUESTIONS_TABLE_NAME}.{DbConsts.QUESTIONS_ID} = @{DbConsts.QUESTIONS_ID} " +
                                        $"WHERE {DbConsts.QUESTIONS_TABLE_NAME}.{DbConsts.QUESTIONS_ID} = @{DbConsts.QUESTIONS_ID};";
                                    tConnection.Open();

                                    tReader = tCommand.ExecuteReader();
                                    tReader.Read();
                                    tQuestion = new SliderQuestion(
                                        (int)tReader[DbConsts.QUESTIONS_ID],
                                        (string)tReader[DbConsts.QUESTIONS_QUESTIONS_TEXT],
                                        (int)tReader[DbConsts.QUESTIONS_QUESTIONS_ORDER],
                                        (int)tReader[DbConsts.SLIDER_START_VALUE],
                                        (int)tReader[DbConsts.SLIDER_END_VALUE],
                                        (string)tReader[DbConsts.SLIDER_START_CAPTION],
                                        (string)tReader[DbConsts.SLIDER_END_CAPTION]
                                        );
                                    break;
                                case TypeQuestionEnum.SmileyFacesQuestion:
                                    tConnection.Close();

                                    tCommand.CommandText = $"SELECT {DbConsts.QUESTIONS_TABLE_NAME}.*,{DbConsts.SMILEY_TABLE_NAME}.* FROM {DbConsts.QUESTIONS_TABLE_NAME} " +
                                        $"INNER JOIN {DbConsts.SMILEY_TABLE_NAME}  ON {DbConsts.QUESTIONS_TABLE_NAME}.{DbConsts.QUESTIONS_ID}= @{DbConsts.QUESTIONS_ID} " +
                                        $"WHERE {DbConsts.QUESTIONS_TABLE_NAME}.{DbConsts.QUESTIONS_ID} = @{DbConsts.QUESTIONS_ID};";
                                    tConnection.Open();

                                    tReader = tCommand.ExecuteReader();
                                    tReader.Read();
                                    tQuestion = new Models.SmileyFacesQuestion(
                                        (int)tReader[DbConsts.QUESTIONS_ID],
                                        (string)tReader[DbConsts.QUESTIONS_QUESTIONS_TEXT],
                                        (int)tReader[DbConsts.QUESTIONS_QUESTIONS_ORDER],
                                        (int)tReader[DbConsts.SMILEY_SMILEY_COUNT]
                                        );
                                    break;
                                case TypeQuestionEnum.StarsQuestion:
                                    tConnection.Close();

                                    tCommand.CommandText = $"SELECT {DbConsts.QUESTIONS_TABLE_NAME}.*,{DbConsts.STARS_TABLE_NAME}.* FROM {DbConsts.QUESTIONS_TABLE_NAME} " +
                                        $"INNER JOIN {DbConsts.STARS_TABLE_NAME}  ON  {DbConsts.QUESTIONS_TABLE_NAME}.{DbConsts.QUESTIONS_ID}= @{DbConsts.QUESTIONS_ID}" +
                                        $"   WHERE {DbConsts.QUESTIONS_TABLE_NAME}.{DbConsts.QUESTIONS_ID} = @{DbConsts.QUESTIONS_ID};";
                                    tConnection.Open();

                                    tReader = tCommand.ExecuteReader();
                                    tReader.Read();
                                    tQuestion = new StarsQuestion(
                                        (int)tReader[DbConsts.QUESTIONS_ID],
                                        (string)tReader[DbConsts.QUESTIONS_QUESTIONS_TEXT],
                                        (int)tReader[DbConsts.QUESTIONS_QUESTIONS_ORDER],
                                        (int)tReader[DbConsts.STARS_STARS_COUNT]
                                        );
                                    break;
                            }
                            tResult.Data = tQuestion;

                        }
                        catch (SqlException sqlEx)
                        {

                            tResult.Success = false;
                            tResult.Message = "We couldn't retrieve the question . Please try again later.";
                            tResult.Error = ErrorTypeEnum.SqlErrorGetQuestion;
                            Log.Error(sqlEx, "SQL error occurred while retrieving question with Id {id}.", tQuestion.Id);
                        }
                        catch (Exception ex)
                        {
                            tResult.Success = false;
                            tResult.Message = "We couldn't retrieve the question count due to an unexpected error. Please contact support.";
                            tResult.Error = ErrorTypeEnum.UnknownErrorGetQuestion;
                            Log.Error(ex, "Unexpected error occurred while retrieving question with Id {id}.", tQuestion.Id);

                        }
                        return tResult;

                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while retrieving question with Id {id}.", tQuestion.Id);
                return new Result<Question>
                {
                    Success = false,
                    Message = "We couldn't retrieve the question count due to an unexpected error. Please contact support.",
                    Error = ErrorTypeEnum.UnknownErrorGetQuestion
                };
            }



        }
        public Result<DateTime> GetLastModified()
        {
            

            try
            {
                Result<DateTime> tResult = new Result<DateTime>()
                {
                    Success = true,
                    Error = ErrorTypeEnum.None,

                };
                using (var tConnection = new SqlConnection(mConnectionString))
                {
                    using (var tCommand = tConnection.CreateCommand())
                    {
                        try
                        {
                            tCommand.CommandText = $"SELECT * FROM {DbConsts.DATABASE_CHANGE_TABLE_NAME}";

                            tConnection.Open();
                            var tReader = tCommand.ExecuteReader();
                            tReader.Read();
                            DateTime tLastModified = (DateTime)tReader[0];
                            tResult.Data = tLastModified;
                            return tResult;
                        }
                        catch (SqlException sqlEx)
                        {

                            tResult.Success = false;
                            tResult.Message = "We couldn't retrieve Last Modified . Please try again later.";
                            tResult.Error = ErrorTypeEnum.SqlErrorGetLastModified;
                            Log.Error(sqlEx, "SQL error occurred while retrieving Last Modified .");
                        }
                        catch (Exception ex)
                        {
                            tResult.Success = false;
                            tResult.Message = "We couldn't retrieve Last Modified due to an unexpected error. Please contact support.";
                            tResult.Error = ErrorTypeEnum.UnknownErrorGetLastModified;
                            //Log.Error(ex, "Unexpected error occurred while retrieving Last Modified.");

                        }
                        return tResult;


                    }


                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "Unexpected error occurred while retrieving Last Modified.");
                return new Result<DateTime>
                {
                    Success = false,
                    Message = "We couldn't retrieve Last Modified due to an unexpected error. Please contact support.",
                    Error = ErrorTypeEnum.UnknownErrorGetLastModified
                };
            }

        }
        public Result<int> UpdateLastModified()  
        {
            

            try
            {
                Questions = new List<Question>();
                Result<int> tResult = new Result<int>()
                {
                    Success = true,
                    Error = ErrorTypeEnum.None,

                };
                using (var tConnection = new SqlConnection(mConnectionString))
                {
                    using (var tCommand = tConnection.CreateCommand())
                    {
                        try
                        {
                            tCommand.CommandText = $"UPDATE {DbConsts.DATABASE_CHANGE_TABLE_NAME} SET {DbConsts.DATABASE_CHANGE_LAST_MODIFIED}= SYSDATETIME();";
                            tConnection.Open();
                            tCommand.ExecuteNonQuery();
                            

                        }
                        catch (SqlException sqlEx)
                        {

                            tResult.Success = false;
                            tResult.Message = "We couldn't update Last Modified . Please try again later.";
                            tResult.Error = ErrorTypeEnum.SqlErrorUpdateLastModified;
                            Log.Error(sqlEx, "SQL error occurred while updating Last Modified .");
                        }
                        catch (Exception ex)
                        {
                            tResult.Success = false;
                            tResult.Message = "We couldn't update Last Modified due to an unexpected error. Please contact support.";
                            tResult.Error = ErrorTypeEnum.UnknownErrorUpdateLastModified;
                            Log.Error(ex, "Unexpected error occurred while updating Last Modified.");

                        }
                        return tResult;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while updating Last Modified.");
                return new Result<int>
                {
                    Success = false,
                    Message = "We couldn't update Last Modified due to an unexpected error. Please contact support.",
                    Error = ErrorTypeEnum.UnknownErrorUpdateLastModified
                };
            }


        }
        public Result<bool> ChangeConnectionString(string pConnectionString)
        {
            

            try
            {
                var tResult = new Result<bool>
                {
                    Success = true,
                    Error = ErrorTypeEnum.None
                };
                Exception tExceptionError;
                var tTestResult = SqlConnectionTest.TestConnection(pConnectionString, out tExceptionError);
                if (!tTestResult.Success)
                {
                    Log.Error(tExceptionError, "Failed to set connection string.");
                    tResult.Success = false;
                    tResult.Error = ErrorTypeEnum.ConnectionStringInvalid;
                    tResult.Message = "Cannot connect to the database. Please check your server name, database name, username, and password.";
                    return tResult;
                }

                mConnectionString = pConnectionString;
                Configuration tConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                tConfig.ConnectionStrings.ConnectionStrings["DbConnectionString"].ConnectionString = pConnectionString;
                tConfig.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");

                tResult.Data = true;
                return tResult;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while changing the connection string.");
                return new Result<bool>
                {
                    Success = false,
                    Message = "We couldn't change connection string due to an unexpected error. Please contact support.",
                    Error = ErrorTypeEnum.UnknownErrorChangeConnectionString
                };
            }


        }

        public Result<bool> ConnectoinTest(string pConnectionString)
        {
            

            try
            {
                var tResult = new Result<bool>
                {
                    Success = true,
                    Error = ErrorTypeEnum.None
                };
                Exception tExceptionError;
                var tTestResult = SqlConnectionTest.TestConnection(pConnectionString, out tExceptionError);
                if (!tTestResult.Success)
                {
                    Log.Error(tExceptionError, "Failed connection test.");
                    tResult.Success = false;
                    tResult.Error = ErrorTypeEnum.ConnectionStringInvalid;
                    tResult.Message = "Cannot connect to the database. Please check your server name, database name, username, and password.";
                }
                return tResult;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while testing connection.");
                return new Result<bool>
                {
                    Success = false,
                    Message = "We couldn't test connection due to an unexpected error. Please contact support.",
                    Error = ErrorTypeEnum.UnknownErrorConnectionTest
                };
            }

        }

        public Result<string> GetConnectionString()
        {
            try
            {
                Configuration tConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var tConnectionStringt=tConfig.ConnectionStrings.ConnectionStrings["DbConnectionString"].ConnectionString;
                return new Result<string>
                {
                    Data = tConnectionStringt,
                    Success = true,
                    Error = ErrorTypeEnum.None
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while getting connection string.");
                return new Result<string>
                {
                    Success = false,
                    Error = ErrorTypeEnum.UnknownErrorGetConnectionString,
                    Message = "We couldn't retrieve the connection string due to an unexpected error. Please contact support."
                };
            }
        }

        /// <summary>
        /// Sets the connection string for ASP.NET web applications.
        /// Unlike ChangeConnectionString, this does not attempt to write to exe.config.
        /// </summary>
        public Result<bool> SetConnectionString(string pConnectionString)
        {
            try
            {
                var tResult = new Result<bool>
                {
                    Success = true,
                    Error = ErrorTypeEnum.None
                };

                // Test the connection first
                Exception tExceptionError;
                var tTestResult = SqlConnectionTest.TestConnection(pConnectionString, out tExceptionError);
                if (!tTestResult.Success)
                {
                    Log.Error(tExceptionError, "Failed to set connection string.");
                    tResult.Success = false;
                    tResult.Error = ErrorTypeEnum.ConnectionStringInvalid;
                    tResult.Message = "Cannot connect to the database. Please check your server name, database name, username, and password.";
                    return tResult;
                }

                // Only set the in-memory connection string (no exe.config save)
                mConnectionString = pConnectionString;

                tResult.Data = true;
                return tResult;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while setting the connection string.");
                return new Result<bool>
                {
                    Success = false,
                    Message = "We couldn't set connection string due to an unexpected error. Please contact support.",
                    Error = ErrorTypeEnum.UnknownErrorChangeConnectionString
                };
            }
        }


    }


}