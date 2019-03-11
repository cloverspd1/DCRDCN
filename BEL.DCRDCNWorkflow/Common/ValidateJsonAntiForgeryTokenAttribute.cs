namespace BEL.DCRDCNWorkflow.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Http.Controllers;
    //using System.Web.Mvc;
    using System.Web.Http;
    using System.Web.Http.Filters;
    using System.Web.Mvc;
    using System.Net.Http;
    
    public class ValidateJsonAntiForgeryTokenAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            try
            {
                string cookieToken = "";
                string formToken = "";


                // if (filterContext.RequestContext.HttpContext.Request.Headers["UID"] != null)
                if (actionContext.Request.Headers.GetValues("UID") != null)
                {
                    IEnumerable<string> tokenHeaders = null;
                    tokenHeaders = actionContext.Request.Headers.GetValues("UID");
                    string[] tokens = tokenHeaders.First().Split(':');
                    if (tokens.Length == 2)
                    {
                        cookieToken = tokens[0].Trim();
                        formToken = tokens[1].Trim();
                    }
                }
                if (HttpContext.Current.Cache[HttpContext.Current.User.Identity.Name + "_formToken"].ToString().Trim().ToLower() != formToken.ToLower().Trim()) throw new UnauthorizedAccessException();
                AntiForgery.Validate(cookieToken, formToken);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new UnauthorizedAccessException();
                // Your error handling here
            }
        }

    }
}