namespace BEL.DCRDCNWorkflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Routing;
    using BEL.DCRDCNWorkflow.Common;
    using BEL.CommonDataContract;

    /// <summary>
    /// Validate AntiForgery Token On AllPosts
    /// </summary>
    public class ValidateAntiForgeryTokenOnAllPosts : AuthorizeAttribute
    {

        public string DefaultClaimType
        {
            get
            {
                return Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DefaultClaimType"]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [environment live].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [environment live]; otherwise, <c>false</c>.
        /// </value>
        public bool EnvironmentLive
        {
            get
            {
                bool environmentLive = false;
                bool.TryParse(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["EnvironmentLive"]), out environmentLive);
                return environmentLive;
            }
        }
    
        /// <summary>
        /// Gets or sets the name of the SP user.
        /// </summary>
        /// <value>
        /// The name of the SP user.
        /// </value>
        private UserDetails CurrentUser
        {
            get
            {
                if (HttpContext.Current.Session["SPCurrentUser"] != null)
                {
                    return (UserDetails)HttpContext.Current.Session["SPCurrentUser"];
                }
                return null;
            }

            set
            {
                HttpContext.Current.Session["SPCurrentUser"] = value;
            }
        }

        /// <summary>
        /// Called when a process requests authorization.
        /// </summary>
        /// <param name="filterContext">The filter context, which encapsulates information for using <see cref="T:System.Web.Mvc.AuthorizeAttribute" />.</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool isvalidrequest = false;

            string cookieToken = string.Empty;
            string formToken = string.Empty;

            if (filterContext != null)
            {
                var request = filterContext.HttpContext.Request;

                //  Only validate POSTs
                if (request.HttpMethod == WebRequestMethods.Http.Post)
                {
                    //  Ajax POSTs and normal form posts have to be treated differently when it comes
                    //  to validating the AntiForgeryToken
                    //if (request.IsAjaxRequest())
                    //{

                    if (request.Headers.GetValues("UID") != null)
                    {
                        IEnumerable<string> tokenHeaders = null;
                        tokenHeaders = request.Headers.GetValues("UID");
                        string[] tokens = tokenHeaders.First().Split(':');
                        if (tokens.Length == 2)
                        {
                            cookieToken = tokens[0].Trim();
                            formToken = tokens[1].Trim();
                        }
                        if (HttpContext.Current.Cache[this.CurrentUser.UserEmail + "_formToken"].ToString().Trim().ToLower() == formToken.ToLower().Trim())
                            isvalidrequest = true;
                    }

                    Logger.Info("IS Ajax Request True");
                    if (!request.Headers.AllKeys.Contains("Referer"))
                    {
                        isvalidrequest = false;
                    }

                    if (request.UrlReferrer == null)
                    {
                        isvalidrequest = false;
                    }
                }
                Logger.Info("Is Valid Request : " + isvalidrequest.ToString());
                if (isvalidrequest && !string.IsNullOrEmpty(cookieToken) && !string.IsNullOrEmpty(formToken))
                {
                    AntiForgery.Validate(cookieToken, formToken);
                }
                else
                {
                    return;
                }

            }
        }
    }
}