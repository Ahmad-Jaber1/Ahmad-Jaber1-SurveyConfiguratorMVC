using Localization;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;

namespace Localization
{
    public static class ErrorLocalizer
    {
        public static string GetMessage(string pVariable ,params string[] pParameters)
        {
            try
            {
                var tMessage = Errors.ResourceManager.GetString(
                        pVariable, Thread.CurrentThread.CurrentUICulture);
                
                
                return string.Format(tMessage, pParameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while try get error message using localizer .");

                throw;
            }

        }
        
    }
}
