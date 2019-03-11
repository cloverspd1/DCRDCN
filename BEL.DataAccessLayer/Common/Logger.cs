namespace BEL.DataAccessLayer
{   
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using BEL.CommonDataContract;    

    /// <summary>
    /// Logger Class
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        private static log4net.ILog Log { get; set; }

        /// <summary>
        /// Initializes static members of the <see cref="Logger"/> class.
        /// </summary>
        static Logger()
        {
            Log = log4net.LogManager.GetLogger(typeof(Logger));
        }

        /// <summary>
        /// Errors the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public static void Error(object msg)
        {
            Log.Error(msg);
        }

        /// <summary>
        /// Errors the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="ex">The ex.</param>
        public static void Error(object msg, Exception ex)
        {
            Log.Error(msg, ex);
        }

        /// <summary>
        /// Errors the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public static void Error(Exception ex)
        {
            if (ex != null)
            {
                Log.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Informations the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="values">The values.</param>
        public static void Info(object msg, params object[] values)
        {
            Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(100);
                    try
                    {
                        string finalResult = string.Empty;
                        foreach (var item in values)
                        {
                            finalResult += item.GetType().Name + "=" + JsonConvert.SerializeObject(item);
                        }
                        Log.Info("WCF - " + msg + ", " + finalResult);
                    }
                    catch (Exception exception)
                    {
                        Guid handlingInstanceId = Guid.NewGuid();
                        BusinessExceptionError model = new BusinessExceptionError()
                        {
                            ActivityID = handlingInstanceId,
                            Message = exception.Message,
                            StackTrace = exception.StackTrace,
                            InnerExceptionMessage = exception.InnerException != null ? exception.InnerException.Message : string.Empty,
                            InnerExceptionStackTrace = exception.InnerException != null ? exception.InnerException.StackTrace : string.Empty
                        };
                        ErrorLogging.LogError("Logger.Info", "HandleException", "Error ID:" + handlingInstanceId.ToString(), exception);
                    }
                });
        }
    }
}