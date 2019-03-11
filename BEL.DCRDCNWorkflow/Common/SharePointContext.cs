namespace BEL.DCRDCNWorkflow.Common
{
    using Microsoft.IdentityModel.S2S.Protocols.OAuth2;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Net;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Configuration;

    /// <summary>
    /// Encapsulates all the information from SharePoint.
    /// </summary>
    public abstract class SharePointContext
    {
        /// <summary>
        /// The sp host URL key
        /// </summary>
        public const string SPHostUrlKey = "SPHostUrl";

        /// <summary>
        /// The sp application web URL key
        /// </summary>
        public const string SPAppWebUrlKey = "SPAppWebUrl";

        /// <summary>
        /// The sp language key
        /// </summary>
        public const string SPLanguageKey = "SPLanguage";

        /// <summary>
        /// The sp client tag key
        /// </summary>
        public const string SPClientTagKey = "SPClientTag";

        /// <summary>
        /// The sp product number key
        /// </summary>
        public const string SPProductNumberKey = "SPProductNumber";

        /// <summary>
        /// The access token lifetime tolerance
        /// </summary>
        protected static readonly TimeSpan AccessTokenLifetimeTolerance = TimeSpan.FromMinutes(5.0);

        /// <summary>
        /// The sp host URL
        /// </summary>
        private readonly Uri spHostUrl;

        /// <summary>
        /// The sp application web URL
        /// </summary>
        private readonly Uri spAppWebUrl;

        /// <summary>
        /// The sp language
        /// </summary>
        private readonly string spLanguage;

        /// <summary>
        /// The sp client tag
        /// </summary>
        private readonly string spClientTag;

        /// <summary>
        /// The sp product number
        /// </summary>
        private readonly string spProductNumber;

        // <AccessTokenString, UtcExpiresOn>

        /// <summary>
        /// The user access token for sp host
        /// </summary>
        protected Tuple<string, DateTime> userAccessTokenForSPHost;

        /// <summary>
        /// The user access token for sp application web
        /// </summary>
        protected Tuple<string, DateTime> userAccessTokenForSPAppWeb;

        /// <summary>
        /// The application only access token for sp host
        /// </summary>
        protected Tuple<string, DateTime> appOnlyAccessTokenForSPHost;

        /// <summary>
        /// The application only access token for sp application web
        /// </summary>
        protected Tuple<string, DateTime> appOnlyAccessTokenForSPAppWeb;

        /// <summary>
        /// Gets the SharePoint host url from QueryString of the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The specified HTTP request.</param>
        /// <returns>
        /// The SharePoint host url. Returns <c>null</c> if the HTTP request doesn't contain the SharePoint host url.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">http Request</exception>
        public static Uri GetSPHostUrl(HttpRequestBase httpRequest)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException("httpRequest");
            }

            string spHostUrlString = TokenHelper.EnsureTrailingSlash(httpRequest.QueryString[SPHostUrlKey]);
            Uri spHostUrl;
            if (Uri.TryCreate(spHostUrlString, UriKind.Absolute, out spHostUrl) &&
                (spHostUrl.Scheme == Uri.UriSchemeHttp || spHostUrl.Scheme == Uri.UriSchemeHttps))
            {
                return spHostUrl;
            }

            return null;
        }

        /// <summary>
        /// Gets the SharePoint host url from QueryString of the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The specified HTTP request.</param>
        /// <returns>
        /// The SharePoint host url. Returns <c>null</c> if the HTTP request doesn't contain the SharePoint host url.
        /// </returns>
        public static Uri GetSPHostUrl(HttpRequest httpRequest)
        {
            return GetSPHostUrl(new HttpRequestWrapper(httpRequest));
        }

        /// <summary>
        /// Gets the sp host URL.
        /// </summary>
        /// <value>
        /// The sp host URL.
        /// </value>
        public Uri SPHostUrl
        {
            get { return this.spHostUrl; }
        }

        /// <summary>
        /// Gets the sp application web URL.
        /// </summary>
        /// <value>
        /// The sp application web URL.
        /// </value>
        public Uri SPAppWebUrl
        {
            get { return this.spAppWebUrl; }
        }

        /// <summary>
        /// Gets the sp language.
        /// </summary>
        /// <value>
        /// The sp language.
        /// </value>
        public string SPLanguage
        {
            get { return this.spLanguage; }
        }

        /// <summary>
        /// Gets the sp client tag.
        /// </summary>
        /// <value>
        /// The sp client tag.
        /// </value>
        public string SPClientTag
        {
            get { return this.spClientTag; }
        }

        /// <summary>
        /// Gets the sp product number.
        /// </summary>
        /// <value>
        /// The sp product number.
        /// </value>
        public string SPProductNumber
        {
            get { return this.spProductNumber; }
        }

        /// <summary>
        /// Gets the user access token for sp host.
        /// </summary>
        /// <value>
        /// The user access token for sp host.
        /// </value>
        public abstract string UserAccessTokenForSPHost
        {
            get;
        }

        /// <summary>
        /// Gets the user access token for sp application web.
        /// </summary>
        /// <value>
        /// The user access token for sp application web.
        /// </value>
        public abstract string UserAccessTokenForSPAppWeb
        {
            get;
        }

        /// <summary>
        /// Gets the application only access token for sp host.
        /// </summary>
        /// <value>
        /// The application only access token for sp host.
        /// </value>
        public abstract string AppOnlyAccessTokenForSPHost
        {
            get;
        }

        /// <summary>
        /// Gets the application only access token for sp application web.
        /// </summary>
        /// <value>
        /// The application only access token for sp application web.
        /// </value>
        public abstract string AppOnlyAccessTokenForSPAppWeb
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointContext" /> class.
        /// </summary>
        /// <param name="spHostUrl">The sp host URL.</param>
        /// <param name="spAppWebUrl">The sp application web URL.</param>
        /// <param name="spLanguage">The sp language.</param>
        /// <param name="spClientTag">The sp client tag.</param>
        /// <param name="spProductNumber">The sp product number.</param>
        /// <exception cref="System.ArgumentNullException">spHostUrl
        /// or
        /// spLanguage
        /// or
        /// spClientTag
        /// or
        /// spProductNumber</exception>
        protected SharePointContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag, string spProductNumber)
        {
            if (spHostUrl == null)
            {
                throw new ArgumentNullException("spHostUrl");
            }

            if (string.IsNullOrEmpty(spLanguage))
            {
                throw new ArgumentNullException("spLanguage");
            }

            if (string.IsNullOrEmpty(spClientTag))
            {
                throw new ArgumentNullException("spClientTag");
            }

            if (string.IsNullOrEmpty(spProductNumber))
            {
                throw new ArgumentNullException("spProductNumber");
            }

            this.spHostUrl = spHostUrl;
            this.spAppWebUrl = spAppWebUrl;
            this.spLanguage = spLanguage;
            this.spClientTag = spClientTag;
            this.spProductNumber = spProductNumber;
        }

        /// <summary>
        /// Creates a user ClientContext for the SharePoint host.
        /// </summary>
        /// <returns>
        /// A ClientContext instance.
        /// </returns>
        public ClientContext CreateUserClientContextForSPHost()
        {
            return CreateClientContext(this.SPHostUrl, this.UserAccessTokenForSPHost);
        }

        /// <summary>
        /// Creates a user ClientContext for the SharePoint app web.
        /// </summary>
        /// <returns>
        /// A ClientContext instance.
        /// </returns>
        public ClientContext CreateUserClientContextForSPAppWeb()
        {
            return CreateClientContext(this.SPAppWebUrl, this.UserAccessTokenForSPAppWeb);
        }

        /// <summary>
        /// Creates app only ClientContext for the SharePoint host.
        /// </summary>
        /// <returns>
        /// A ClientContext instance.
        /// </returns>
        public ClientContext CreateAppOnlyClientContextForSPHost()
        {
            return CreateClientContext(this.SPHostUrl, this.AppOnlyAccessTokenForSPHost);
        }

        /// <summary>
        /// Creates an app only ClientContext for the SharePoint app web.
        /// </summary>
        /// <returns>
        /// A ClientContext instance.
        /// </returns>
        public ClientContext CreateAppOnlyClientContextForSPAppWeb()
        {
            return CreateClientContext(this.SPAppWebUrl, this.AppOnlyAccessTokenForSPAppWeb);
        }

        /// <summary>
        /// Gets the database connection string from SharePoint for autohosted app.
        /// </summary>
        /// <returns>
        /// The database connection string. Returns <c>null</c> if the app is not autohosted or there is no database.
        /// </returns>
        public string GetDatabaseConnectionString()
        {
            string dbConnectionString = null;

            using (ClientContext clientContext = this.CreateAppOnlyClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var result = AppInstance.RetrieveAppDatabaseConnectionString(clientContext);

                    clientContext.ExecuteQuery();

                    dbConnectionString = result.Value;
                }
            }

            if (dbConnectionString == null)
            {
                const string LocalDBInstanceForDebuggingKey = "LocalDBInstanceForDebugging";

                var dbConnectionStringSettings = WebConfigurationManager.ConnectionStrings[LocalDBInstanceForDebuggingKey];

                dbConnectionString = dbConnectionStringSettings != null ? dbConnectionStringSettings.ConnectionString : null;
            }

            return dbConnectionString;
        }

        /// <summary>
        /// Determines if the specified access token is valid.
        /// It considers an access token as not valid if it is null, or it has expired.
        /// </summary>
        /// <param name="accessToken">The access token to verify.</param>
        /// <returns>
        /// True if the access token is valid.
        /// </returns>
        protected static bool IsAccessTokenValid(Tuple<string, DateTime> accessToken)
        {
            return accessToken != null &&
                   !string.IsNullOrEmpty(accessToken.Item1) &&
                   accessToken.Item2 > DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a ClientContext with the specified SharePoint site url and the access token.
        /// </summary>
        /// <param name="spSiteUrl">The site url.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns>
        /// A ClientContext instance.
        /// </returns>
        private static ClientContext CreateClientContext(Uri spSiteUrl, string accessToken)
        {
            if (spSiteUrl != null && !string.IsNullOrEmpty(accessToken))
            {
                return TokenHelper.GetClientContextWithAccessToken(spSiteUrl.AbsoluteUri, accessToken);
            }

            return null;
        }
    }

    /// <summary>
    /// Redirection status.
    /// </summary>
    public enum RedirectionStatus
    {
        /// <summary>
        /// The ok
        /// </summary>
        Ok,

        /// <summary>
        /// The should redirect
        /// </summary>
        ShouldRedirect,

        /// <summary>
        /// The can not redirect
        /// </summary>
        CanNotRedirect
    }

    /// <summary>
    /// Provides SharePointContext instances.
    /// </summary>
    public abstract class SharePointContextProvider
    {
        /// <summary>
        /// The current
        /// </summary>
        private static SharePointContextProvider current;

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static SharePointContextProvider Current
        {
            get { return SharePointContextProvider.current; }
        }

        /// <summary>
        /// Initializes static members of the <see cref="SharePointContextProvider" /> class.
        /// </summary>
        static SharePointContextProvider()
        {
            if (!TokenHelper.IsHighTrustApp())
            {
                SharePointContextProvider.current = new SharePointAcsContextProvider();
            }
            else
            {
                SharePointContextProvider.current = new SharePointHighTrustContextProvider();
            }
        }

        /// <summary>
        /// Registers the specified SharePointContextProvider instance as current.
        /// It should be called by Application_Start() in Global.asax.
        /// </summary>
        /// <param name="provider">The SharePointContextProvider to be set as current.</param>
        /// <exception cref="System.ArgumentNullException">share point provider</exception>
        public static void Register(SharePointContextProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            SharePointContextProvider.current = provider;
        }

        /// <summary>
        /// Checks if it is necessary to redirect to SharePoint for user to authenticate.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="redirectUrl">The redirect url to SharePoint if the status is ShouldRedirect. <c>Null</c> if the status is Ok or CanNotRedirect.</param>
        /// <returns>
        /// Redirection status.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">http Context</exception>
        public static RedirectionStatus CheckRedirectionStatus(HttpContextBase httpContext, out Uri redirectUrl)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            redirectUrl = null;

            if (SharePointContextProvider.Current.GetSharePointContext(httpContext) != null)
            {
                return RedirectionStatus.Ok;
            }

            const string SPHasRedirectedToSharePointKey = "SPHasRedirectedToSharePoint";

            if (!string.IsNullOrEmpty(httpContext.Request.QueryString[SPHasRedirectedToSharePointKey]))
            {
                return RedirectionStatus.CanNotRedirect;
            }

            Uri spHostUrl = SharePointContext.GetSPHostUrl(httpContext.Request);

            if (spHostUrl == null)
            {
                return RedirectionStatus.CanNotRedirect;
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(httpContext.Request.HttpMethod, "POST"))
            {
                return RedirectionStatus.CanNotRedirect;
            }

            Uri requestUrl = httpContext.Request.Url;

            var queryNameValueCollection = HttpUtility.ParseQueryString(requestUrl.Query);

            // Removes the values that are included in {StandardTokens}, as {StandardTokens} will be inserted at the beginning of the query string.
            queryNameValueCollection.Remove(SharePointContext.SPHostUrlKey);
            queryNameValueCollection.Remove(SharePointContext.SPAppWebUrlKey);
            queryNameValueCollection.Remove(SharePointContext.SPLanguageKey);
            queryNameValueCollection.Remove(SharePointContext.SPClientTagKey);
            queryNameValueCollection.Remove(SharePointContext.SPProductNumberKey);

            // Adds SPHasRedirectedToSharePoint=1.
            queryNameValueCollection.Add(SPHasRedirectedToSharePointKey, "1");

            UriBuilder returnUrlBuilder = new UriBuilder(requestUrl);
            returnUrlBuilder.Query = queryNameValueCollection.ToString();

            // Inserts StandardTokens.
            const string StandardTokens = "{StandardTokens}";
            string returnUrlString = returnUrlBuilder.Uri.AbsoluteUri;
            returnUrlString = returnUrlString.Insert(returnUrlString.IndexOf("?") + 1, StandardTokens + "&");

            // Constructs redirect url.
            string redirectUrlString = TokenHelper.GetAppContextTokenRequestUrl(spHostUrl.AbsoluteUri, Uri.EscapeDataString(returnUrlString));

            redirectUrl = new Uri(redirectUrlString, UriKind.Absolute);

            return RedirectionStatus.ShouldRedirect;
        }

        /// <summary>
        /// Checks if it is necessary to redirect to SharePoint for user to authenticate.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="redirectUrl">The redirect url to SharePoint if the status is ShouldRedirect. <c>Null</c> if the status is Ok or CanNotRedirect.</param>
        /// <returns>
        /// Redirection status.
        /// </returns>
        public static RedirectionStatus CheckRedirectionStatus(HttpContext httpContext, out Uri redirectUrl)
        {
            return CheckRedirectionStatus(new HttpContextWrapper(httpContext), out redirectUrl);
        }

        /// <summary>
        /// Creates a SharePointContext instance with the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if errors occur.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">http Request</exception>
        public SharePointContext CreateSharePointContext(HttpRequestBase httpRequest)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException("httpRequest");
            }

            // SPHostUrl
            Uri spHostUrl = SharePointContext.GetSPHostUrl(httpRequest);
            if (spHostUrl == null)
            {
                return null;
            }

            // SPAppWebUrl
            string spAppWebUrlString = TokenHelper.EnsureTrailingSlash(httpRequest.QueryString[SharePointContext.SPAppWebUrlKey]);
            Uri spAppWebUrl;
            if (!Uri.TryCreate(spAppWebUrlString, UriKind.Absolute, out spAppWebUrl) ||
                !(spAppWebUrl.Scheme == Uri.UriSchemeHttp || spAppWebUrl.Scheme == Uri.UriSchemeHttps))
            {
                spAppWebUrl = null;
            }

            // SPLanguage
            string spLanguage = httpRequest.QueryString[SharePointContext.SPLanguageKey];
            if (string.IsNullOrEmpty(spLanguage))
            {
                return null;
            }

            // SPClientTag
            string spClientTag = httpRequest.QueryString[SharePointContext.SPClientTagKey];
            if (string.IsNullOrEmpty(spClientTag))
            {
                return null;
            }

            // SPProductNumber
            string spProductNumber = httpRequest.QueryString[SharePointContext.SPProductNumberKey];
            if (string.IsNullOrEmpty(spProductNumber))
            {
                return null;
            }

            return this.CreateSharePointContext(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber, httpRequest);
        }

        /// <summary>
        /// Creates a SharePointContext instance with the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if errors occur.
        /// </returns>
        public SharePointContext CreateSharePointContext(HttpRequest httpRequest)
        {
            return this.CreateSharePointContext(new HttpRequestWrapper(httpRequest));
        }

        /// <summary>
        /// Gets a SharePointContext instance associated with the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if not found and a new instance can't be created.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">http Context</exception>
        public SharePointContext GetSharePointContext(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            Uri spHostUrl = SharePointContext.GetSPHostUrl(httpContext.Request);
            if (spHostUrl == null)
            {
                return null;
            }

            SharePointContext spContext = this.LoadSharePointContext(httpContext);

            if (spContext == null || !this.ValidateSharePointContext(spContext, httpContext))
            {
                spContext = this.CreateSharePointContext(httpContext.Request);

                if (spContext != null)
                {
                    this.SaveSharePointContext(spContext, httpContext);
                }
            }

            return spContext;
        }

        /// <summary>
        /// Gets a SharePointContext instance associated with the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if not found and a new instance can't be created.
        /// </returns>
        public SharePointContext GetSharePointContext(HttpContext httpContext)
        {
            return this.GetSharePointContext(new HttpContextWrapper(httpContext));
        }

        /// <summary>
        /// Creates a SharePointContext instance.
        /// </summary>
        /// <param name="spHostUrl">The SharePoint host url.</param>
        /// <param name="spAppWebUrl">The SharePoint app web url.</param>
        /// <param name="spLanguage">The SharePoint language.</param>
        /// <param name="spClientTag">The SharePoint client tag.</param>
        /// <param name="spProductNumber">The SharePoint product number.</param>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if errors occur.
        /// </returns>
        protected abstract SharePointContext CreateSharePointContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag, string spProductNumber, HttpRequestBase httpRequest);

        /// <summary>
        /// Validates if the given SharePointContext can be used with the specified HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// True if the given SharePointContext can be used with the specified HTTP context.
        /// </returns>
        protected abstract bool ValidateSharePointContext(SharePointContext spContext, HttpContextBase httpContext);

        /// <summary>
        /// Loads the SharePointContext instance associated with the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if not found.
        /// </returns>
        protected abstract SharePointContext LoadSharePointContext(HttpContextBase httpContext);

        /// <summary>
        /// Saves the specified SharePointContext instance associated with the specified HTTP context.
        /// <c>null</c> is accepted for clearing the SharePointContext instance associated with the HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext instance to be saved, or <c>null</c>.</param>
        /// <param name="httpContext">The HTTP context.</param>
        protected abstract void SaveSharePointContext(SharePointContext spContext, HttpContextBase httpContext);
    }

    #region ACS

    /// <summary>
    /// Encapsulates all the information from SharePoint in ACS mode.
    /// </summary>
    public class SharePointAcsContext : SharePointContext
    {
        /// <summary>
        /// The context token
        /// </summary>
        private readonly string contextToken;

        /// <summary>
        /// The context token object
        /// </summary>
        private readonly SharePointContextToken contextTokenObj;

        /// <summary>
        /// Gets the context token.
        /// </summary>
        /// <value>
        /// The context token.
        /// </value>
        public string ContextToken
        {
            get { return this.contextTokenObj.ValidTo > DateTime.UtcNow ? this.contextToken : null; }
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>
        /// The cache key.
        /// </value>
        public string CacheKey
        {
            get { return this.contextTokenObj.ValidTo > DateTime.UtcNow ? this.contextTokenObj.CacheKey : null; }
        }

        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        public string RefreshToken
        {
            get { return this.contextTokenObj.ValidTo > DateTime.UtcNow ? this.contextTokenObj.RefreshToken : null; }
        }

        /// <summary>
        /// Gets the user access token for sp host.
        /// </summary>
        /// <value>
        /// The user access token for sp host.
        /// </value>
        public override string UserAccessTokenForSPHost
        {
            get
            {
                return GetAccessTokenString(ref this.userAccessTokenForSPHost, () => TokenHelper.GetAccessToken(this.contextTokenObj, this.SPHostUrl.Authority));
            }
        }

        /// <summary>
        /// Gets the user access token for sp application web.
        /// </summary>
        /// <value>
        /// The user access token for sp application web.
        /// </value>
        public override string UserAccessTokenForSPAppWeb
        {
            get
            {
                if (this.SPAppWebUrl == null)
                {
                    return null;
                }

                return GetAccessTokenString(ref this.userAccessTokenForSPAppWeb, () => TokenHelper.GetAccessToken(this.contextTokenObj, this.SPAppWebUrl.Authority));
            }
        }

        /// <summary>
        /// Gets the application only access token for sp host.
        /// </summary>
        /// <value>
        /// The application only access token for sp host.
        /// </value>
        public override string AppOnlyAccessTokenForSPHost
        {
            get
            {
                return GetAccessTokenString(ref this.appOnlyAccessTokenForSPHost, () => TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, this.SPHostUrl.Authority, TokenHelper.GetRealmFromTargetUrl(this.SPHostUrl)));
            }
        }

        /// <summary>
        /// Gets the application only access token for sp application web.
        /// </summary>
        /// <value>
        /// The application only access token for sp application web.
        /// </value>
        public override string AppOnlyAccessTokenForSPAppWeb
        {
            get
            {
                if (this.SPAppWebUrl == null)
                {
                    return null;
                }

                return GetAccessTokenString(ref this.appOnlyAccessTokenForSPAppWeb, () => TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, this.SPAppWebUrl.Authority, TokenHelper.GetRealmFromTargetUrl(this.SPAppWebUrl)));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointAcsContext" /> class.
        /// </summary>
        /// <param name="spHostUrl">The sp host URL.</param>
        /// <param name="spAppWebUrl">The sp application web URL.</param>
        /// <param name="spLanguage">The sp language.</param>
        /// <param name="spClientTag">The sp client tag.</param>
        /// <param name="spProductNumber">The sp product number.</param>
        /// <param name="contextToken">The context token.</param>
        /// <param name="contextTokenObj">The context token object.</param>
        /// <exception cref="System.ArgumentNullException">contextToken
        /// or
        /// contextTokenObj</exception>
        public SharePointAcsContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag, string spProductNumber, string contextToken, SharePointContextToken contextTokenObj)
            : base(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber)
        {
            if (string.IsNullOrEmpty(contextToken))
            {
                throw new ArgumentNullException("contextToken");
            }

            if (contextTokenObj == null)
            {
                throw new ArgumentNullException("contextTokenObj");
            }

            this.contextToken = contextToken;
            this.contextTokenObj = contextTokenObj;
        }

        /// <summary>
        /// Ensures the access token is valid and returns it.
        /// </summary>
        /// <param name="accessToken">The access token to verify.</param>
        /// <param name="tokenRenewalHandler">The token renewal handler.</param>
        /// <returns>
        /// The access token string.
        /// </returns>
        private static string GetAccessTokenString(ref Tuple<string, DateTime> accessToken, Func<OAuth2AccessTokenResponse> tokenRenewalHandler)
        {
            RenewAccessTokenIfNeeded(ref accessToken, tokenRenewalHandler);

            return SharePointContext.IsAccessTokenValid(accessToken) ? accessToken.Item1 : null;
        }

        /// <summary>
        /// Renews the access token if it is not valid.
        /// </summary>
        /// <param name="accessToken">The access token to renew.</param>
        /// <param name="tokenRenewalHandler">The token renewal handler.</param>
        private static void RenewAccessTokenIfNeeded(ref Tuple<string, DateTime> accessToken, Func<OAuth2AccessTokenResponse> tokenRenewalHandler)
        {
            if (SharePointContext.IsAccessTokenValid(accessToken))
            {
                return;
            }

            try
            {
                OAuth2AccessTokenResponse oAuth2AccessTokenResponse = tokenRenewalHandler();

                DateTime expiresOn = oAuth2AccessTokenResponse.ExpiresOn;

                if ((expiresOn - oAuth2AccessTokenResponse.NotBefore) > SharePointContext.AccessTokenLifetimeTolerance)
                {
                    // Make the access token get renewed a bit earlier than the time when it expires
                    // so that the calls to SharePoint with it will have enough time to complete successfully.
                    expiresOn -= SharePointContext.AccessTokenLifetimeTolerance;
                }

                accessToken = Tuple.Create(oAuth2AccessTokenResponse.AccessToken, expiresOn);
            }
            catch (WebException)
            {
            }
        }
    }

    /// <summary>
    /// Default provider for SharePointAcsContext.
    /// </summary>
    public class SharePointAcsContextProvider : SharePointContextProvider
    {
        /// <summary>
        /// The sp context key
        /// </summary>
        private const string SPContextKey = "SPContext";

        /// <summary>
        /// The sp cache key key
        /// </summary>
        private const string SPCacheKeyKey = "SPCacheKey";

        /// <summary>
        /// Creates a SharePointContext instance.
        /// </summary>
        /// <param name="spHostUrl">The SharePoint host url.</param>
        /// <param name="spAppWebUrl">The SharePoint app web url.</param>
        /// <param name="spLanguage">The SharePoint language.</param>
        /// <param name="spClientTag">The SharePoint client tag.</param>
        /// <param name="spProductNumber">The SharePoint product number.</param>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if errors occur.
        /// </returns>
        protected override SharePointContext CreateSharePointContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag, string spProductNumber, HttpRequestBase httpRequest)
        {
            if (httpRequest != null)
            {
                string contextTokenString = TokenHelper.GetContextTokenFromRequest(httpRequest);
                if (string.IsNullOrEmpty(contextTokenString))
                {
                    return null;
                }

                SharePointContextToken contextToken = null;
                try
                {
                    contextToken = TokenHelper.ReadAndValidateContextToken(contextTokenString, httpRequest.Url.Authority);
                }
                catch (WebException)
                {
                    return null;
                }
                catch (AudienceUriValidationFailedException)
                {
                    return null;
                }

                return new SharePointAcsContext(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber, contextTokenString, contextToken);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Validates if the given SharePointContext can be used with the specified HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// True if the given SharePointContext can be used with the specified HTTP context.
        /// </returns>
        protected override bool ValidateSharePointContext(SharePointContext spContext, HttpContextBase httpContext)
        {
            if (spContext != null && httpContext != null)
            {
                SharePointAcsContext spAcsContext = spContext as SharePointAcsContext;

                if (spAcsContext != null)
                {
                    Uri spHostUrl = SharePointContext.GetSPHostUrl(httpContext.Request);
                    string contextToken = TokenHelper.GetContextTokenFromRequest(httpContext.Request);
                    HttpCookie spCacheKeyCookie = httpContext.Request.Cookies[SPCacheKeyKey];
                    string spCacheKey = spCacheKeyCookie != null ? spCacheKeyCookie.Value : null;

                    return spHostUrl == spAcsContext.SPHostUrl &&
                           !string.IsNullOrEmpty(spAcsContext.CacheKey) &&
                           spCacheKey == spAcsContext.CacheKey &&
                           !string.IsNullOrEmpty(spAcsContext.ContextToken) &&
                           (string.IsNullOrEmpty(contextToken) || contextToken == spAcsContext.ContextToken);
                }
            }

            return false;
        }

        /// <summary>
        /// Loads the SharePointContext instance associated with the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if not found.
        /// </returns>
        protected override SharePointContext LoadSharePointContext(HttpContextBase httpContext)
        {
            if (httpContext != null)
            {
                return httpContext.Session[SPContextKey] as SharePointAcsContext;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Saves the specified SharePointContext instance associated with the specified HTTP context.
        /// <c>null</c> is accepted for clearing the SharePointContext instance associated with the HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext instance to be saved, or <c>null</c>.</param>
        /// <param name="httpContext">The HTTP context.</param>
        protected override void SaveSharePointContext(SharePointContext spContext, HttpContextBase httpContext)
        {
            if (spContext != null && httpContext != null)
            {
                SharePointAcsContext spAcsContext = spContext as SharePointAcsContext;

                if (spAcsContext != null)
                {
                    HttpCookie spCacheKeyCookie = new HttpCookie(SPCacheKeyKey)
                    {
                        Value = spAcsContext.CacheKey,
                        Secure = true,
                        HttpOnly = true
                    };

                    httpContext.Response.AppendCookie(spCacheKeyCookie);
                }

                httpContext.Session[SPContextKey] = spAcsContext;
            }
        }
    }

    #endregion ACS

    #region HighTrust

    /// <summary>
    /// Encapsulates all the information from SharePoint in HighTrust mode.
    /// </summary>
    public class SharePointHighTrustContext : SharePointContext
    {
        /// <summary>
        /// The logon user identity
        /// </summary>
        private readonly WindowsIdentity logonUserIdentity;

        /// <summary>
        /// Gets the logon user identity.
        /// </summary>
        /// <value>
        /// The logon user identity.
        /// </value>
        public WindowsIdentity LogonUserIdentity
        {
            get { return this.logonUserIdentity; }
        }

        /// <summary>
        /// Gets the user access token for sp host.
        /// </summary>
        /// <value>
        /// The user access token for sp host.
        /// </value>
        public override string UserAccessTokenForSPHost
        {
            get
            {
                return GetAccessTokenString(ref this.userAccessTokenForSPHost, () => TokenHelper.GetS2SAccessTokenWithWindowsIdentity(this.SPHostUrl, this.LogonUserIdentity));
            }
        }

        /// <summary>
        /// Gets the user access token for sp application web.
        /// </summary>
        /// <value>
        /// The user access token for sp application web.
        /// </value>
        public override string UserAccessTokenForSPAppWeb
        {
            get
            {
                if (this.SPAppWebUrl == null)
                {
                    return null;
                }

                return GetAccessTokenString(ref this.userAccessTokenForSPAppWeb, () => TokenHelper.GetS2SAccessTokenWithWindowsIdentity(this.SPAppWebUrl, this.LogonUserIdentity));
            }
        }

        /// <summary>
        /// Gets the application only access token for sp host.
        /// </summary>
        /// <value>
        /// The application only access token for sp host.
        /// </value>
        public override string AppOnlyAccessTokenForSPHost
        {
            get
            {
                return GetAccessTokenString(ref this.appOnlyAccessTokenForSPHost, () => TokenHelper.GetS2SAccessTokenWithWindowsIdentity(this.SPHostUrl, null));
            }
        }

        /// <summary>
        /// Gets the application only access token for sp application web.
        /// </summary>
        /// <value>
        /// The application only access token for sp application web.
        /// </value>
        public override string AppOnlyAccessTokenForSPAppWeb
        {
            get
            {
                if (this.SPAppWebUrl == null)
                {
                    return null;
                }

                return GetAccessTokenString(ref this.appOnlyAccessTokenForSPAppWeb, () => TokenHelper.GetS2SAccessTokenWithWindowsIdentity(this.SPAppWebUrl, null));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointHighTrustContext" /> class.
        /// </summary>
        /// <param name="spHostUrl">The sp host URL.</param>
        /// <param name="spAppWebUrl">The sp application web URL.</param>
        /// <param name="spLanguage">The sp language.</param>
        /// <param name="spClientTag">The sp client tag.</param>
        /// <param name="spProductNumber">The sp product number.</param>
        /// <param name="logonUserIdentity">The logon user identity.</param>
        /// <exception cref="System.ArgumentNullException">logon User Identity</exception>
        public SharePointHighTrustContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag, string spProductNumber, WindowsIdentity logonUserIdentity)
            : base(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber)
        {
            if (logonUserIdentity == null)
            {
                throw new ArgumentNullException("logonUserIdentity");
            }

            this.logonUserIdentity = logonUserIdentity;
        }

        /// <summary>
        /// Gets the access token string.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="tokenRenewalHandler">The token renewal handler.</param>
        /// <returns>
        /// Valid Access Token
        /// </returns>
        private static string GetAccessTokenString(ref Tuple<string, DateTime> accessToken, Func<string> tokenRenewalHandler)
        {
            RenewAccessTokenIfNeeded(ref accessToken, tokenRenewalHandler);

            return SharePointContext.IsAccessTokenValid(accessToken) ? accessToken.Item1 : null;
        }

        /// <summary>
        /// Renews the access token if needed.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="tokenRenewalHandler">The token renewal handler.</param>
        private static void RenewAccessTokenIfNeeded(ref Tuple<string, DateTime> accessToken, Func<string> tokenRenewalHandler)
        {
            if (SharePointContext.IsAccessTokenValid(accessToken))
            {
                return;
            }

            DateTime expiresOn = DateTime.UtcNow.Add(TokenHelper.HighTrustAccessTokenLifetime);

            if (TokenHelper.HighTrustAccessTokenLifetime > SharePointContext.AccessTokenLifetimeTolerance)
            {
                // Make the access token get renewed a bit earlier than the time when it expires
                // so that the calls to SharePoint with it will have enough time to complete successfully.
                expiresOn -= SharePointContext.AccessTokenLifetimeTolerance;
            }

            accessToken = Tuple.Create(tokenRenewalHandler(), expiresOn);
        }
    }

    /// <summary>
    /// Default provider for SharePointHighTrustContext.
    /// </summary>
    public class SharePointHighTrustContextProvider : SharePointContextProvider
    {
        /// <summary>
        /// The sp context key
        /// </summary>
        private const string SPContextKey = "SPContext";

        /// <summary>
        /// Creates a SharePointContext instance.
        /// </summary>
        /// <param name="spHostUrl">The SharePoint host url.</param>
        /// <param name="spAppWebUrl">The SharePoint app web url.</param>
        /// <param name="spLanguage">The SharePoint language.</param>
        /// <param name="spClientTag">The SharePoint client tag.</param>
        /// <param name="spProductNumber">The SharePoint product number.</param>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if errors occur.
        /// </returns>
        protected override SharePointContext CreateSharePointContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag, string spProductNumber, HttpRequestBase httpRequest)
        {
            if (httpRequest != null)
            {
                WindowsIdentity logonUserIdentity = httpRequest.LogonUserIdentity;
                if (logonUserIdentity == null || !logonUserIdentity.IsAuthenticated || logonUserIdentity.IsGuest || logonUserIdentity.User == null)
                {
                    return null;
                }

                return new SharePointHighTrustContext(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber, logonUserIdentity);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Validates if the given SharePointContext can be used with the specified HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// True if the given SharePointContext can be used with the specified HTTP context.
        /// </returns>
        protected override bool ValidateSharePointContext(SharePointContext spContext, HttpContextBase httpContext)
        {
            if (spContext != null && httpContext != null)
            {
                SharePointHighTrustContext spHighTrustContext = spContext as SharePointHighTrustContext;

                if (spHighTrustContext != null)
                {
                    Uri spHostUrl = SharePointContext.GetSPHostUrl(httpContext.Request);
                    WindowsIdentity logonUserIdentity = httpContext.Request.LogonUserIdentity;

                    return spHostUrl == spHighTrustContext.SPHostUrl &&
                           logonUserIdentity != null &&
                           logonUserIdentity.IsAuthenticated &&
                           !logonUserIdentity.IsGuest &&
                           logonUserIdentity.User == spHighTrustContext.LogonUserIdentity.User;
                }
            }

            return false;
        }

        /// <summary>
        /// Loads the SharePointContext instance associated with the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if not found.
        /// </returns>
        protected override SharePointContext LoadSharePointContext(HttpContextBase httpContext)
        {
            if (httpContext != null)
            {
                return httpContext.Session[SPContextKey] as SharePointHighTrustContext;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Saves the specified SharePointContext instance associated with the specified HTTP context.
        /// <c>null</c> is accepted for clearing the SharePointContext instance associated with the HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext instance to be saved, or <c>null</c>.</param>
        /// <param name="httpContext">The HTTP context.</param>
        protected override void SaveSharePointContext(SharePointContext spContext, HttpContextBase httpContext)
        {
            if (spContext != null && httpContext != null)
            {
                httpContext.Session[SPContextKey] = spContext as SharePointHighTrustContext;
            }
        }
    }

    #endregion HighTrust
}
