namespace BEL.DCRDCNWorkflow.Common
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Threading.Tasks;
  
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
                    Log.Info("MVC - " + msg + ", " + finalResult);
                }
                catch (Exception exe)
                {
                    try
                    {
                        string id = Guid.NewGuid().ToString();
                        //DAL
                        //using (CommonProxy.CommonServiceClient client = new CommonProxy.CommonServiceClient())
                        //{
                        //    string errorExcetption = JSONHelper.ToJSON<Exception>(exe);
                        //    client.ErrorLog(errorExcetption, id);
                        //}
                    }
                    catch
                    {
                    }
                }
            });
        }
    }
}