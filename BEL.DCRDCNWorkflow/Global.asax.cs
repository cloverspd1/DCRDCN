namespace BEL.DCRDCNWorkflow
{
    using Newtonsoft.Json;
    using BEL.DCRDCNWorkflow.Common;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.ServiceModel;
    using System.Threading;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using BEL.CommonDataContract;
    using System.Net;


    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Gets a value indicating whether [environment live].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [environment live]; otherwise, <c>false</c>.
        /// </value>
        protected bool EnvironmentLive
        {
            get
            {
                bool environmentLive = false;
                bool.TryParse(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["EnvironmentLive"]), out environmentLive);
                return environmentLive;
            }
        }

        /// <summary>
        /// Application_s the start.
        /// </summary>
        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleTable.EnableOptimizations = this.EnvironmentLive;
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new MvcExceptionHandler());
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
            log4net.Config.XmlConfigurator.Configure();
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Email;
            Logger.Info("Application_Start_Done");
        }

        protected void Application_EndRequest()
        {
            var context = new HttpContextWrapper(Context);

            //Do a direct 401 unautorized
            if (Context.Response.StatusCode == 302 && context.Request.IsAjaxRequest())
            {
                Context.Response.Clear();
                Context.Response.StatusCode = 401;
            }
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                Exception exe = Server.GetLastError();
                if (this.EnvironmentLive)
                {
                    HttpException exec = (HttpException)exe;
                    if (exec != null)
                    {
                        Server.ClearError();
                        string id = Guid.NewGuid().ToString();
                        bool isAjaxCall = string.Equals("XMLHttpRequest", Context.Request.Headers["x-requested-with"], StringComparison.OrdinalIgnoreCase);
                        string msg = "An error has occurred while serving your request. Please contact your administrator for more information.Error Id: " + id;

                        // if (exe.GetType() == typeof(FaultException<BusinessExceptionError>) || exe.GetType() == typeof(FaultException))
                        if ((exec.GetHttpCode() == 401) || (exec.GetHttpCode() == 402) || (exec.GetHttpCode() == 403) || (exec.GetHttpCode() == 404) || (exec.GetHttpCode() == 500) || (exec.GetHttpCode() == 503))
                        {
                            msg = exec.Message;

                            // Logger.Error("BusinessExceptionError OR FaultException");
                            Logger.Error(exec.GetHttpCode() + " Error:");
                            Logger.Error(msg);
                        }
                        else if (exec.GetType() == typeof(HttpException))
                        {
                            msg = exec.Message;

                            Logger.Error("HttpException");
                            Logger.Error(msg);
                        }
                        else
                        {
                            Logger.Error("Error ID :" + id, exec);
                        }
                        if (isAjaxCall)
                        {
                            Response.Clear();
                            Response.ContentType = "application/json";
                            ActionStatus status = new ActionStatus();
                            status.IsSucceed = false;
                            status.Messages.Add(msg);
                            Response.Write(JsonConvert.SerializeObject(status));
                            Response.End();
                        }
                        else
                        {
                            if (!Request.Url.ToString().Contains("Master/Error"))
                            {
                                Response.Redirect("~/Master/Error?msg=" + msg);
                            }
                        }
                    }
                }
                else
                {
                    if (exe != null)
                    {
                        Logger.Error(exe);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// Handles the BeginRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            CultureInfo newCulture = new CultureInfo("en-GB");
            newCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            newCulture.DateTimeFormat.DateSeparator = "/";
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = newCulture;
            string formParams = string.Empty;
            bool isAjaxCall = string.Equals("XMLHttpRequest", Context.Request.Headers["x-requested-with"], StringComparison.OrdinalIgnoreCase);
            if (Request.Form != null && Request.Form.Count > 0)
            {
                formParams = "Form Data: " + JsonConvert.SerializeObject(Request.Form);
            }
            Logger.Info("Begin" + (isAjaxCall ? " Ajax" : string.Empty) + " Request " + Request.Url.ToString() + formParams);
        }
        //Security Fixes
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            ////****
            //////Session is Available here
            ////HttpContext context = HttpContext.Current;
            ////if ((!Request.RawUrl.Contains("/Error") && !Request.RawUrl.Contains("/Content") && !Request.RawUrl.Contains("/SessionExpired"))
            ////       && Request.Cookies["HS"] != null && Request.Cookies["HS"].Value.Length > 0 && context != null && context.Session != null)
            ////{
            ////    string sessionHSCookie = Request.Cookies["HS"].Value;
            ////    string unHSValue = System.Web.Mvc.Html.Helper.GenerateHashKey(context);

            ////    if (sessionHSCookie != unHSValue || sessionHSCookie == null || sessionHSCookie.Length <= 0)
            ////    {
            ////        Response.Cookies.Add(new HttpCookie("HS", ""));
            ////        if (context.Session != null)
            ////        {
            ////            context.Session.RemoveAll();
            ////        }
            ////        throw new HttpException((int)System.Net.HttpStatusCode.Forbidden, "Session Expired - Invalid Request");
            ////    }
            ////}
            ////else if ((!Request.RawUrl.Contains("/Error") && !Request.RawUrl.Contains("/Content") && !Request.RawUrl.Contains("/SessionExpired")) &&
            ////    (Request.Cookies["HS"] == null || Request.Cookies["HS"].Value.Length <= 0) && context.Session != null)
            ////{
            ////    //if (Session["SPUser"] != null)
            ////    //{
            ////    //    Response.Cookies.Add(new HttpCookie("HS", ""));
            ////    //    //throw new HttpException("Invalid Request - User is not authorized to access the link.");
            ////    //    context.Session.RemoveAll();
            ////    //    throw new HttpException((int)System.Net.HttpStatusCode.Forbidden, "Session Expired - Invalid Request");
            ////    //}

            ////}
            ////****
        }
    }
}
