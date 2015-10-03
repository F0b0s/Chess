using System;
using NLog;

namespace UCIProxy.Common
{
    public static class LogHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public static void LogInfo(string message)
        {
            Logger.Info(message);
        }

        public static void LogError(Exception exception)
        {
            Logger.Error(exception, exception.Message);
        }

        public static void LogError(Exception exception, string message)
        {
            Logger.Error(exception, message);
        }
    }
}