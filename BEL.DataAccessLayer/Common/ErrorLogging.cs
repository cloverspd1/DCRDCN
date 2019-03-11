namespace BEL.DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web;

    /// <summary>
    /// Error Logging
    /// </summary>
    public class ErrorLogging
    {
        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public static void LogError(Exception ex)
        {
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public static void LogError(string msg)
        {
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="ex">The ex.</param>
        public static void LogError(string className, string methodName, string msg, Exception ex)
        {
            if (!string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(methodName) && !string.IsNullOrEmpty(msg) && ex != null)
            {
                Exception newExe = ex;
                string stackTrace = "[Message: " + newExe.Message + "]" + System.Environment.NewLine + newExe.StackTrace;
                while (newExe.InnerException != null)
                {
                    stackTrace += "[Message: " + newExe.Message + "]" + System.Environment.NewLine + ex.InnerException.StackTrace;
                    newExe = newExe.InnerException;
                }
                Logger.Error(stackTrace);
            }
        }
    }
}