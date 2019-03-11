namespace BEL.DataAccessLayer
{
    using System;
    using System.Collections.Specialized;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.WCF;
    using BEL.CommonDataContract;

    /// <summary>
    /// General Business Exception
    /// Author: Jay Ashara
    /// Date: 2015-10-16
    /// </summary>
    [ConfigurationElementType(typeof(CustomHandlerData))]
    public class BusinessExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessExceptionHandler"/> class.
        /// </summary>
        /// <param name="ignore">The ignore.</param>
        public BusinessExceptionHandler(NameValueCollection ignore)
        {
            if (ignore != null)
            {
            }
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="handlingInstanceId">The handling instance id.</param>
        /// <returns>exception object</returns>
        public Exception HandleException(Exception exception, Guid handlingInstanceId)
        {
            if (exception != null)
            {
                //Explicitly add Logging/ErrorLog ExceptionSheilding
                BusinessExceptionError model = new BusinessExceptionError()
                {
                    ActivityID = handlingInstanceId,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    InnerExceptionMessage = exception.InnerException != null ? exception.InnerException.Message : string.Empty,
                    InnerExceptionStackTrace = exception.InnerException != null ? exception.InnerException.StackTrace : string.Empty
                };
                ErrorLogging.LogError(this.GetType().Name, "HandleException", "Error ID:" + handlingInstanceId.ToString(), exception);
                return new FaultContractWrapperException(model, handlingInstanceId);
            }
            else
            {
                return exception;
            }
        }
    }
}
