namespace BEL.DataAccessLayer
{
    using BEL.CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Async Call Helper
    /// </summary>
    public static class AsyncHelper
    {
        /// <summary>
        /// Calls the specified delegate function.
        /// </summary>
        /// <param name="delegateFunction">The delegate function.</param>
        /// <param name="isAsync">if set to <c>true</c> [is asynchronous].</param>
        /// <returns>
        /// Task object
        /// </returns>
        public static Task Call(Action<object> delegateFunction, bool isAsync)
        {
            return AsyncHelper.Call(delegateFunction, 100, isAsync);
        }

        /// <summary>
        /// Calls the specified delegate function.
        /// </summary>
        /// <param name="delegateFunction">The delegate function.</param>
        /// <param name="taskDelay">The task delay.</param>
        /// <param name="isAsync">if set to <c>true</c> [is asynchronous].</param>
        /// <returns>
        /// Task Object
        /// </returns>
        public static Task Call(Action<object> delegateFunction, int taskDelay = 100, bool isAsync = true)
        {
           // BELDataAccessLayer helper = new BELDataAccessLayer();
            isAsync = BELDataAccessLayer.Instance.GetConfigVariable("AllowAsync").ToLower().Equals("true") ? isAsync : false;
            if (isAsync)
            {
                return Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(taskDelay);
                    try
                    {
                        delegateFunction(new object());
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
                        ErrorLogging.LogError(delegateFunction.GetType().Name, "HandleException", "Error ID:" + handlingInstanceId.ToString(), exception);
                    }
                });
            }
            else
            {
                if (delegateFunction != null)
                {
                    delegateFunction(new object());
                }
                return null;
            }
        }
    }
}