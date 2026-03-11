using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Shared
{
    public  class LogConfiguration
    {
        public static void Configure()
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Error()
                        .WriteTo.File(
                            "logs\\app_log.txt",
                            outputTemplate:
                                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} | {Level} | {SourceContext}{NewLine}" +
                                "Message    : {Message:lj}{NewLine}" +
                                "Exception  :{NewLine}{Exception}{NewLine}" +
                                "============================================================{NewLine}"
                        )
                        .CreateLogger();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while configurate logging. ");
                throw; 
            }
        }

        public static void Configure(string pBasePath)
        {
            try
            {
                string tLogPath = Path.Combine(pBasePath, "logs", "app_log.txt");

                Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Error()
                        .WriteTo.File(
                            tLogPath,
                            outputTemplate:
                                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} | {Level} | {SourceContext}{NewLine}" +
                                "Message    : {Message:lj}{NewLine}" +
                                "Exception  :{NewLine}{Exception}{NewLine}" +
                                "============================================================{NewLine}"
                        )
                        .CreateLogger();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while configurate logging. ");
                throw;
            }
        }
    }
}
