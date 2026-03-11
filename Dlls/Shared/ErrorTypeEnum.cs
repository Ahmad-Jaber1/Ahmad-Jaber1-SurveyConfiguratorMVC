using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public enum ErrorTypeEnum
    {
        None = 0, // No error

        // Connection errors
        ConnectionStringNotSet = 1, // "Database connection string is not set or invalid. Go to Settings → Database Connection to set it up."
        ConnectionStringInvalid = 2, // "Cannot connect to the database. Please check your server name, database name, username, and password."
        DatabaseConnectionFailed = 3, // "Failed to connect to the database. Contact the administrator to resolve the issue."
        DatabaseUnavailable = 4, //The database could not be reached. It may be offline, deleted, or temporarily unavailable. Please verify the database server and try again.
        // SQL errors
        SqlErrorSliderLoad = 10, // "SQL error occurred while loading slider questions. Please contact support."
        SqlErrorSmileyLoad = 11, // "SQL error occurred while loading smiley questions."
        SqlErrorStarsLoad = 12, // "SQL error occurred while loading star questions."
        SqlErrorAddQuestion = 13, // "We couldn't save your question at the moment. Please try again later."
        SqlErrorDeleteQuestion = 14, // "We couldn't delete your question at the moment. Please try again later."
        SqlErrorEditQuestion = 15, // "We couldn't update your question at the moment. Please try again later."
        SqlErrorEditOrder = 16, // "We couldn't update order at the moment. Please try again later."
        SqlErrorGetCount = 17, // "We couldn't retrieve the question count at the moment. Please try again later."
        SqlErrorGetQuestion = 18, // "We couldn't retrieve the question. Please try again later."
        SqlErrorGetLastModified = 19, // "We couldn't retrieve Last Modified. Please try again later."
        SqlErrorUpdateLastModified = 20, // "We couldn't update Last Modified. Please try again later."

        // Unknown / unexpected errors
        UnknownErrorSliderLoad = 30, // "Unexpected error occurred while loading slider questions."
        UnknownErrorSmileyLoad = 31, // "Unexpected error occurred while loading smiley questions."
        UnknownErrorStarsLoad = 32, // "Unexpected error occurred while loading star questions."
        UnknownErrorAddQuestion = 33, // "Unexpected error occurred while adding question."
        UnknownErrorDeleteQuestion = 34, // "We couldn't delete your question due to an unexpected error. Please try again or contact support."
        UnknownErrorEditQuestion = 35, // "We couldn't edit question due to an unexpected error. Please try again or contact support."
        UnknownErrorEditOrder = 36, // "We couldn't edit order due to an unexpected error.Please try again or contact support."
        UnknownErrorGetCount = 37, // "We couldn't retrieve the question count due to an unexpected error. Please try again or contact support."
        UnknownErrorGetQuestion = 38, // "We couldn't retrieve the question count due to an unexpected error. Please try again or contact support."
        UnknownErrorGetLastModified = 39, // "We couldn't retrieve Last Modified due to an unexpected error. Please try again or contact support."
        UnknownErrorUpdateLastModified = 40, // "We couldn't update Last Modified due to an unexpected error. Please try again or contact support."
        UnknownErrorChangeConnectionString = 41, // "We couldn't change connection string due to an unexpected error. Please try again or contact support.."
        UnknownErrorConnectionTest = 42, // "We couldn't test connection  due to an unexpected error. Please try again or contact support."
        UnknownErrorGetConnectionString = 43, // "We couldn't retrieve the connection string due to an unexpected error. Please try again or contact support."
        UnknownErrorQuestionsLoad = 44,//"Unexpected error occurred while loading questions."
        
        // Unsupported operations
        UnsupportedQuestionType = 50, // "Unsupported question type."


        Validation_QuestionDataNull = 100, // "Question data cannot be empty. Please provide a valid question."
        Validation_QuestionTypeInvalid = 101, // "The selected question type is not valid. Please choose a supported type."
        Validation_OrderOutOfRangeAdd = 102, // "Order value must not be grater than number of question + 1 !. Please enter a valid number."
        Validation_OrderOutOfRangeEdit = 103, // "Order value must be between 1 and {Max}. Please enter a valid number."
        Validation_EditQuestionDataNull = 104, // "Please provide a question. This field cannot be empty."
        NotFound_DeleteQuestion = 105, // "Question not found."
        NotFound_EditQuestion = 106, // "The specified question was not found. Please check your selection."
        NotFound_GetQuestion = 107, // "The specified question was not found. Please check your selection."
        //Connection_NotSetOrInvalid = 116, // "Database connection is not set or invalid. Go to Settings → Database Connection to set it up."

        Database_Offline = 120 ,
    }
}
