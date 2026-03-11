using Microsoft.Data.SqlClient;
using Shared;
using System;

namespace Repository
{
    public static class SqlConnectionTest
    {
        public static Result<bool> TestConnection(string pConnectionString ,out Exception pExceptionError)
        {
            try
            {
                using (var tConnection = new SqlConnection(pConnectionString))
                {
                    tConnection.Open();
                }
                pExceptionError = null; 
                return new Result<bool>
                {
                    Success = true,
                    Data = true,
                    Error = ErrorTypeEnum.None
                };
            }
            catch (Exception ex)
            {
                pExceptionError = ex;   
                return new Result<bool>
                {
                    Success = false,
                    Data = false,
                    Error = ErrorTypeEnum.UnknownErrorConnectionTest,
                    Message = ex.Message
                };
            }
        }
    }
}
