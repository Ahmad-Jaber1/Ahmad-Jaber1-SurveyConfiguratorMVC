using Microsoft.Data.SqlClient;
using Models;
using Shared;
using SurveyConfiguratorTask.Models;
using System;
using System.Collections.Generic;

namespace Services
{
    public interface IQuestionService
    {
        event Action CheckUpdateEvent;

        Result<int> AddQuestion(TypeQuestionEnum pType, AddQuestionDto pQuestionDto);
        Result<bool> ChangeConnectionString(string pConnectionString);
        Result<bool> CheckConnection();
        void CheckForUpdates();
        Result<bool> ConnectionTest(string pConnectionString);
        Result<int> DeleteQuestion(int pId);
        Result<int> EditOrder();
        Result<int> EditQuestion(int pId, EditQuestionDto pEditQuestionDto);
        void FormClosing();
        Result<SqlConnectionStringBuilder> GetConnectionString();
        Result<int> GetCount();
        Result<DateTime> GetLastModified();
        Result<Question> GetQuestion(int pId);
        List<Question> GetQuestionsList();
        Result<List<Question>> QuestionsLoad();
        Result<bool> SetConnectionString(string pConnectionString);
    }
}