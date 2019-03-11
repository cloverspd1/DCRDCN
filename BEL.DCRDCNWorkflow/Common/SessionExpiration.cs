namespace BEL.DCRDCNWorkflow.Common
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    
    /// <summary>
    /// Session Expiration Class.
    /// </summary>
    public class SessionExpiration : ActionFilterAttribute
    {
        /// <summary>
        /// Called when [action executing].
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
          base.OnActionExecuting(filterContext);
        }
    }
}