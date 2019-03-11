namespace BEL.DCRDCNWorkflow.App_Start
{
    using BEL.DCRDCNWorkflow.Common;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Services;
    using System.Linq;
    using System.Web;

    public class CustomSessionAuthenticationModule : SessionAuthenticationModule
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
        /// Handles the <see cref="E:System.Web.HttpApplication.AuthenticateRequest" /> event from the ASP.NET pipeline.
        /// </summary>
        /// <param name="sender">The source for the event. This will be an <see cref="T:System.Web.HttpApplication" /> object.</param>
        /// <param name="eventArgs">The data for the event.</param>
        protected override void OnAuthenticateRequest(object sender, EventArgs eventArgs)
        {
            if (this.EnvironmentLive)
            {
                Logger.Info("OnAuthenticateRequest start");
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest != null && this.ContainsSessionTokenCookie(httpRequest.Cookies) && httpRequest.HttpMethod == "POST" && httpRequest.Form["wresult"] != null && httpRequest.Form["wctx"] != null)
                {
                    if (httpRequest.UrlReferrer == null)
                    {
                        Logger.Info("Alert! httpRequest.UrlReferrer is null.");
                    }
                    Logger.Info("OnAuthenticateRequest  httpRequest.UrlReferrer.AbsoluteUri: {0}", httpRequest.UrlReferrer.AbsoluteUri);
                    var wctx = HttpUtility.ParseQueryString(httpRequest.UrlReferrer.AbsoluteUri);
                    if (wctx != null && wctx.Count != 0)
                    {
                        HttpContext.Current.Response.Redirect(wctx["ru"]);
                    }
                }
                Logger.Info("OnAuthenticateRequest End");
            }
            base.OnAuthenticateRequest(sender, eventArgs);
        }
    }

    /// <summary>
    /// Dynamic Realm Federation Authentication Module
    /// </summary>
    public class DynamicRealmFederationAuthenticationModule : WSFederationAuthenticationModule
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
        /// Raises the <see cref="E:System.IdentityModel.Services.WSFederationAuthenticationModule.RedirectingToIdentityProvider" /> event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnRedirectingToIdentityProvider(RedirectingToIdentityProviderEventArgs e)
        {
            Logger.Info("OnRedirectingToIdentityProvider start");
            if (this.EnvironmentLive && e != null)
            {
                var wctx = HttpUtility.ParseQueryString(e.SignInRequestMessage.GetParameter("wctx"));
                if (wctx != null && wctx.Count != 0)
                {
                    string ru = wctx["ru"];
                    ru = ru.Replace("&", "%26");
                    ru = ru.Replace("=", "%3D");
                    Logger.Info(string.Format("OnRedirectingToIdentityProvider  ReturnURL: {0}", ru));
                    e.SignInRequestMessage.Realm = this.DetermineDynamicRealm() + ru;
                    Logger.Info(string.Format("OnRedirectingToIdentityProvider  Realm: {0}", e.SignInRequestMessage.Realm));
                }
            }
            Logger.Info("OnRedirectingToIdentityProvider End");
            base.OnRedirectingToIdentityProvider(e);
        }

        /// <summary>
        /// Builds the requested address.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>URI Details</returns>
        private static Uri BuildRequestedAddress(HttpRequest request)
        {
            //Guard.That(() => request).IsNotNull();
            //Guard.That(() => request.Headers).IsNotNull();
            //Guard.That(() => request.Url).IsNotNull();

            var originalRequest = request.Url;
            var serverName = request.Headers["Host"];
            var address = string.Concat(originalRequest.Scheme, "://", serverName);

            address += originalRequest.PathAndQuery;

            return new Uri(address);
        }

        /// <summary>
        /// Determines the dynamic realm.
        /// </summary>
        /// <returns>String details</returns>
        private string DetermineDynamicRealm()
        {
            // Set the realm to be the current domain name
            string realm;
            const string SecureHttp = "https://";
            var hostUri = BuildRequestedAddress(HttpContext.Current.Request);
            var port = string.Empty;

            if (Realm.StartsWith(SecureHttp, StringComparison.OrdinalIgnoreCase))
            {
                realm = SecureHttp;

                if (hostUri.Port != 443)
                {
                    port = ":" + hostUri.Port;
                }
            }
            else
            {
                realm = "http://";

                if (hostUri.Port != 80)
                {
                    port = ":" + hostUri.Port;
                }
            }

            realm += hostUri.Host + port;

            return realm;
        }
    }
}