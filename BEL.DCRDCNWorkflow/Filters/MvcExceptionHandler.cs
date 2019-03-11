using BEL.CommonDataContract;
using BEL.DCRDCNWorkflow.Common;
using BEL.DCRDCNWorkflow.Controllers;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BEL.DCRDCNWorkflow
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class MvcExceptionHandler : FilterAttribute, IExceptionFilter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext != null && filterContext.Exception != null)
            {
                //HttpContext httpContext = HttpContext.Current;
                //var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
                var currentController = string.Empty;
                var currentAction = string.Empty;

                ////if (currentRouteData != null)
                ////{
                ////    if (!string.IsNullOrEmpty(Convert.ToString(currentRouteData.Values["controller"])))
                ////    {
                ////        currentController = currentRouteData.Values["controller"].ToString();
                ////    }

                ////    if (!string.IsNullOrEmpty(Convert.ToString(currentRouteData.Values["action"])))
                ////    {
                ////        currentAction = currentRouteData.Values["action"].ToString();
                ////    }
                ////}
                //// var model = new HandleErrorInfo(filterContext.Exception, currentController, currentAction);
                var ex = filterContext.Exception;
                string id = Guid.NewGuid().ToString();
                

                ////if (filterContext.HttpContext.Request.IsAjaxRequest())
                ////{
                ////    AjaxResponse ajaxResponse = new AjaxResponse();
                ////    ajaxResponse.Data = new ViewDataDictionary<HandleErrorInfo>(model);
                ////    ajaxResponse.Status = AjaxResponseStatusCodes.Error;
                ////    filterContext.Result = new JsonResult
                ////    {
                ////        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                ////        Data = ajaxResponse
                ////    };
                ////}
                ////else
                ////{
                ////    httpContext.ClearError();
                ////    httpContext.Response.Clear();
                ////    httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
                ////    httpContext.Response.Redirect("~/Master/Error?msg=" + msg);
                ////}

                ////filterContext.ExceptionHandled = true;
                ////filterContext.HttpContext.Response.Clear();
                ////if (filterContext.Exception.GetType() == typeof(HttpException))
                ////{
                ////    HttpException exception = filterContext.Exception as HttpException;
                ////    filterContext.HttpContext.Response.StatusCode = exception.GetHttpCode();
                ////}

                ////filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

                Logger.Error("Id: " + id);
                Logger.Error("StatusCode: " + filterContext.HttpContext.Response.StatusCode);
                Logger.Error("Controller: " + currentController);
                Logger.Error("Action: " + currentAction);
                Logger.Error(ex);
            }
        }
    }
}