namespace BEL.DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using BEL.CommonDataContract;
    using System.Reflection;
    using Microsoft.SharePoint.Client;
    using System.Net;
    using System.Configuration;
    using System.IO;
    using Microsoft.SharePoint.Client.Taxonomy;
    using Microsoft.SharePoint.Client.Utilities;
    using System.Web.Caching;
    using System.Text;
    using System.Runtime.Caching;
    using System.Data.SqlClient;
    using System.Data;
    using Microsoft.SharePoint.Client.UserProfiles;
    using System.Xml.Serialization;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using System.Security;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// Helper Class
    /// </summary>
    public class BELDataAccessLayer
    {
        #region "Constructure"
        /// <summary>
        /// Lazy Instance
        /// </summary>
        private static readonly Lazy<BELDataAccessLayer> Lazy =
          new Lazy<BELDataAccessLayer>(() => new BELDataAccessLayer());

        /// <summary>
        /// Gets the Instance.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        public static BELDataAccessLayer Instance
        {
            get
            {
                return Lazy.Value;
            }
        }

        ////private BELDataAccessLayer()
        ////{
        ////    try
        ////    {
        ////        BELDataAccessLayer helper = new BELDataAccessLayer();
        ////        string siteURL = helper.GetSiteURL(SiteURLs.DCRSITE);
        ////        if (!string.IsNullOrEmpty(siteURL))
        ////        {
        ////            this.context = helper.CreateClientContext(siteURL);
        ////            this.web = helper.CreateWeb(this.context);
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        ErrorLogging.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "Error While Initializing context.", ex);
        ////    }
        ////}

        /// <summary>
        /// Child Items
        /// </summary>
        private const string ChildItems = "_Child_Items_";

        /// <summary>
        /// Object Type
        /// </summary>
        private const string ObjectType = "_ObjectType_";
        #endregion

        #region "Varialbe
        /// <summary>
        /// Gets or sets the name of the SP user.
        /// </summary>
        /// <value>
        /// The name of the SP user.
        /// </value>
        private UserDetails SPCurrentUser
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
        #endregion

        #region Static Methods
        /// <summary>
        /// Generates the reference no.
        /// </summary>
        /// <param name="appName">Name of the application.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="requestDate">The request date.</param>
        /// <returns>
        /// Reference Number
        /// </returns>
        public static string GenerateReferenceNo(string appName, string itemId, DateTime requestDate)
        {
            if (requestDate == null)
            {
                requestDate = DateTime.Now;
            }

            switch (appName)
            {
                case ApplicationNameConstants.DCRAPP:
                    appName = "DCR";
                    break;
                case FormNameConstants.DCRFORM:
                    appName = "DCR";
                    break;
                case FormNameConstants.DCNFORM:
                    appName = "DCR-DCN";
                    break;
            }

            //string refNo = appName + "-" + (requestDate.Month < 4 ? requestDate.Year - 1 : requestDate.Year) + "-" + (requestDate.Month >= 4 ? requestDate.Year + 1 : requestDate.Year) + "-" + requestDate.ToString("ddMMyyyy") + "-" + itemId;
            string refNo = appName + "-" + requestDate.Year + "-" + string.Format("{0:0000}", itemId);
            Logger.Info("Helper.GenerateReferenceNo", refNo);
            return refNo;
        }

        /// <summary>
        /// Get Email from Person Field
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="user">The user.</param>
        /// <returns>
        /// string value
        /// </returns>
        public static string GetEmailsFromPersonField(ClientContext context, Web web, FieldUserValue user)
        {
            string userEmail = string.Empty;
            BELDataAccessLayer helper = new BELDataAccessLayer();
            if (helper.GetConfigVariable("UseRESTAPI").ToLower().Equals("true"))
            {
                userEmail = GetEmailsFromPersonFieldUsingREST(context, web, user);
            }
            else
            {
                userEmail = GetEmailsFromPersonFieldUsingCSOM(context, web, user);
            }
            return userEmail;
        }

        /// <summary>
        /// Truncates the specified total length.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="totalLength">The total length.</param>
        /// <returns>
        /// string 10 character
        /// </returns>
        public static string Truncate(string str, int totalLength)
        {
            if (string.IsNullOrEmpty(str) || str.Length < totalLength)
            {
                return str;
            }
            return str.Substring(0, totalLength);
        }

        /// <summary>
        /// To the XML.
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>XML string</returns>
        public static string ToXML<T>(T obj)
        {
            try
            {
                using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(stringWriter, obj);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.Info("Errro while XML serialization {0}, Message {1}", ex.StackTrace, ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the emails from person field using rest.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="user">The user.</param>
        /// <returns>User Email</returns>
        public static string GetEmailsFromPersonFieldUsingREST(ClientContext context, Web web, FieldUserValue user)
        {
            string userEmail = string.Empty;
            if (context != null && web != null && user != null)
            {
                JObject jobj = RESTHelper.GetDataUsingRest(web.Url + "/_api/web/getuserbyid(" + user.LookupId + ")", "GET");
                userEmail = Convert.ToString(jobj["d"]["Email"]);
            }
            return userEmail;
        }

        /// <summary>
        /// Get Email from Person Field Using CSOM
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="user">The user.</param>
        /// <returns>
        /// string value
        /// </returns>
        public static string GetEmailsFromPersonFieldUsingCSOM(ClientContext context, Web web, FieldUserValue user)
        {
            string personsEmail = string.Empty;
            if (user != null && web != null && context != null)
            {
                //*//Logger.Info("Helper.GetEmailsFromPersonField", user.LookupId);
                //User userDetails = web.SiteUsers.GetById(user.LookupId);
                //context.Load(userDetails);
                //context.ExecuteQuery();
                //Logger.Info("Helper.GetEmailsFromPersonField", userDetails.Email);
                //if (!string.IsNullOrEmpty(userDetails.Email))
                //{
                //    personsEmail = userDetails.Email;
                //}
                if (user != null && user.LookupId != 0)
                {
                    personsEmail = user.LookupId.ToString();
                }
            }
            return personsEmail.ToLower();
        }

        /// <summary>
        /// Get Comma Seprated Emails from Person Field
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="users">The users.</param>
        /// <returns>
        /// string value
        /// </returns>
        public static string GetEmailsFromPersonField(ClientContext context, Web web, FieldUserValue[] users)
        {
            string personsEmail = string.Empty;
            if (users != null && users.Length > 0 && web != null && context != null)
            {
                foreach (FieldUserValue user in users)
                {
                    string userEmail = GetEmailsFromPersonField(context, web, user);
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        if (string.IsNullOrEmpty(personsEmail))
                        {
                            personsEmail = userEmail;
                        }
                        else
                        {
                            personsEmail = personsEmail.Trim(',') + "," + userEmail;
                        }
                    }
                }
            }
            return personsEmail.ToLower();
        }

        /// <summary>
        /// Ensures the user.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="userID">The user email.</param>
        /// <returns>User Object</returns>
        public static User EnsureUser(ClientContext context, Web web, string userID)
        {
            User resolvedUser = null;
            if (context != null && web != null && !string.IsNullOrEmpty(userID))
            {
                try
                {
                    if (!string.IsNullOrEmpty(userID))
                    {
                        resolvedUser = web.GetUserById(Convert.ToInt32(userID));
                        context.Load(resolvedUser);
                        context.ExecuteQuery();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Info("Error while resolve : " + userID);
                    Logger.Error(ex);
                }
            }
            return resolvedUser;
        }

        /// <summary>
        /// Ensures the user.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="userIDs">The user emails.</param>
        /// <returns>User Array</returns>
        public static User[] EnsureUser(ClientContext context, Web web, string[] userIDs)
        {
            User[] resolvedUsers = null;
            if (context != null && web != null && userIDs != null)
            {
                resolvedUsers = new User[userIDs.Length];
                for (int i = 0; i < userIDs.Length; i++)
                {
                    User resolvedUser = EnsureUser(context, web, userIDs[i]);
                    resolvedUsers[i] = resolvedUser;
                }
            }
            return resolvedUsers;
        }

        /// <summary>
        /// Gets the name from person field.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="users">The users.</param>
        /// <returns>Get person names</returns>
        public static string GetNameFromPersonField(ClientContext context, Web web, FieldUserValue[] users)
        {
            string personsName = string.Empty;
            if (users != null && users.Length > 0 && web != null && context != null)
            {
                foreach (FieldUserValue user in users)
                {
                    Logger.Info("Helper.GetNameFromPersonField", user.LookupId);
                    string userName = user.LookupValue; //GetNameFromPersonField(context, web, user);
                    Logger.Info("Helper.GetNameFromPersonField", userName);
                    if (!string.IsNullOrEmpty(userName))
                    {
                        personsName = personsName.Trim(',') + ", " + userName;
                    }
                }
            }
            return personsName.Trim(',').Trim();
        }

        /// <summary>
        /// Gets the name from person field.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="user">The user.</param>
        /// <returns>User Name</returns>
        public static string GetNameFromPersonField(ClientContext context, Web web, FieldUserValue user)
        {
            string personsName = string.Empty;
            if (user != null && web != null && context != null)
            {
                BELDataAccessLayer helper = new BELDataAccessLayer();
                if (helper.GetConfigVariable("UseRESTAPI").ToLower().Equals("true"))
                {
                    personsName = GetNameFromPersonFieldUsingREST(context, web, user);
                }
                else
                {
                    personsName = GetNameFromPersonFieldUsingCSOM(context, web, user);
                }
            }
            return personsName;
        }

        /// <summary>
        /// Gets the name from person field REST.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="user">The user.</param>
        /// <returns>User Name</returns>
        public static string GetNameFromPersonFieldUsingREST(ClientContext context, Web web, FieldUserValue user)
        {
            string personsName = string.Empty;
            if (user != null && web != null && context != null)
            {
                JObject jobj = RESTHelper.GetDataUsingRest(web.Url + "/_api/web/getuserbyid(" + user.LookupId + ")", "GET");
                if (!string.IsNullOrEmpty(Convert.ToString(jobj["d"]["Title"])))
                {
                    personsName = Convert.ToString(jobj["d"]["Title"]);
                }
            }
            return personsName;
        }

        /// <summary>
        /// Gets the name from person field CSOM.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="user">The user.</param>
        /// <returns>User Name</returns>
        public static string GetNameFromPersonFieldUsingCSOM(ClientContext context, Web web, FieldUserValue user)
        {
            string personsName = string.Empty;
            if (user != null && web != null && context != null)
            {
                ////User userDetails = web.SiteUsers.GetById(user.LookupId);
                ////context.Load(userDetails);
                ////context.ExecuteQuery();
                Logger.Info("Helper.GetNameFromPersonField", user.LookupValue);
                ////if (!string.IsNullOrEmpty(userDetails.Title))
                ////{
                ////    personsName = userDetails.Title;
                ////}
                personsName = user.LookupValue;
            }
            return personsName;
        }

        /// <summary>
        /// Gets the name from email.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="emails">The emails.</param>
        /// <returns>
        /// User names
        /// </returns>
        public static string GetNameFromEmail(ClientContext context, Web web, string emails)
        {
            string userName = string.Empty;
            if (context != null && web != null && !string.IsNullOrEmpty(emails))
            {
                BELDataAccessLayer helper = new BELDataAccessLayer();
                if (helper.GetConfigVariable("UseRESTAPI").ToLower().Equals("true"))
                {
                    userName = GetNameFromEmailUsingREST(context, web, emails);
                }
                else
                {
                    userName = GetNameFromEmailUsingCSOM(context, web, emails);
                }
            }
            return userName.Trim(',');
        }

        /// <summary>
        /// Gets the name from email.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="emails">The emails.</param>
        /// <returns>
        /// User names
        /// </returns>
        public static string GetNameFromEmailUsingREST(ClientContext context, Web web, string emails)
        {
            string userName = string.Empty;
            if (context != null && web != null && !string.IsNullOrEmpty(emails))
            {
                string[] emailIds = emails.Split(',');
                foreach (string email in emailIds)
                {
                    Logger.Info("Helper.GetNameFromEmail", email);
                    JObject jobj = RESTHelper.GetDataUsingRest(web.Url + "/_api/web/siteusers/getbyemail('" + email + "')", "GET");
                    Logger.Info("Helper.GetNameFromEmail got an user with name", Convert.ToString(jobj["d"]["Title"]));
                    if (!string.IsNullOrEmpty(Convert.ToString(jobj["d"]["Title"])))
                    {
                        userName = userName.Trim(',') + "," + Convert.ToString(jobj["d"]["Title"]);
                    }
                }
            }
            return userName.Trim(',');
        }

        /// <summary>
        /// Gets the name from email Using CSOM.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="userIDs">The emails.</param>
        /// <returns>
        /// User names
        /// </returns>
        public static string GetNameFromEmailUsingCSOM(ClientContext context, Web web, string userIDs)
        {
            string userName = string.Empty;
            if (context != null && web != null && !string.IsNullOrEmpty(userIDs))
            {
                string[] emailIds = userIDs.Split(',');
                foreach (string userID in emailIds)
                {
                    Logger.Info("Helper.GetNameFromEmail", userID);
                    User user = EnsureUser(context, web, userID);
                    Logger.Info("Helper.GetNameFromEmail got an user with name", user.Title);
                    if (!string.IsNullOrEmpty(user.Title))
                    {
                        userName = userName.Trim(',') + "," + user.Title;
                    }
                }
            }
            return userName.Trim(',');
        }

        /// <summary>
        /// Gets the field user value from person.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="userID">The email.</param>
        /// <returns>FielduserValue Object</returns>
        public static FieldUserValue GetFieldUserValueFromPerson(ClientContext context, Web web, string userID)
        {
            FieldUserValue objuser = null;
            if (context != null && web != null && !string.IsNullOrEmpty(userID))
            {
                Logger.Info("Helper.GetFieldUserValueFromPerson", userID);
                objuser = new FieldUserValue();
                objuser.LookupId = Convert.ToInt32(userID);
            }
            return objuser;
        }

        /// <summary>
        /// Gets the field user value from person.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="emails">The emails.</param>
        /// <returns>FielduserValue[] Object</returns>
        public static FieldUserValue[] GetFieldUserValueFromPerson(ClientContext context, Web web, string[] emails)
        {
            FieldUserValue[] users = null;
            if (emails != null && emails.Length > 0)
            {
                users = new FieldUserValue[emails.Length];
                int i = 0;
                foreach (string email in emails)
                {
                    users[i] = GetFieldUserValueFromPerson(context, web, email);
                    i++;
                }
            }
            return users;
        }

        /// <summary>
        /// Handles the MixedAuthRequest event of the Ctx control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WebRequestEventArgs"/> instance containing the event data.</param>
        public static void Ctx_MixedAuthRequest(object sender, WebRequestEventArgs e)
        {
            try
            {
                //Add the header that tells SharePoint to use Windows authentication.
                if (e != null)
                {
                    e.WebRequestExecutor.RequestHeaders.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// Gets the claim prefix.
        /// </summary>
        /// <value>
        /// The claim prefix.
        /// </value>
        public static string ClaimPrefix
        {
            get
            {
                return Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ClaimPrefix"]);
            }
        }

        /// <summary>
        /// Get Name Using User ID
        /// </summary>
        /// <param name="context">Context Object</param>
        /// <param name="web">Web Object</param>
        /// <param name="ids">List of id</param>
        /// <returns>Name value</returns>
        public static string GetNameUsingUserID(ClientContext context, Web web, string ids)
        {
            string userName = string.Empty;
            if (context != null && web != null && !string.IsNullOrEmpty(ids))
            {
                MasterDataHelper masterHelper = new MasterDataHelper();
                List<UserDetails> userInfoList = masterHelper.GetAllEmployee(context, web);
                string[] userIds = ids.Split(',');
                foreach (string id in userIds)
                {
                    Logger.Info("Helper.GetNameUsingUserID", id);
                    UserDetails detail = userInfoList.FirstOrDefault(p => p.UserId == id);

                    if (detail != null && !string.IsNullOrEmpty(detail.FullName))
                    {
                        Logger.Info("Helper.GetNameUsingUserID got an user with name", detail.FullName);
                        userName = userName.Trim(',') + "," + detail.FullName;
                    }
                }
            }
            return userName.Trim(',');
        }

        /// <summary>
        /// Get Name Using User ID
        /// </summary>
        /// <param name="context">Context object</param>
        /// <param name="web">Web object</param>
        /// <param name="ids">List of id</param>
        /// <returns>Email value</returns>
        public static string GetEmailUsingUserID(ClientContext context, Web web, string ids)
        {
            string userName = string.Empty;
            if (context != null && web != null && !string.IsNullOrEmpty(ids))
            {
                MasterDataHelper masterHelper = new MasterDataHelper();
                List<UserDetails> userInfoList = masterHelper.GetAllEmployee(context, web);
                string[] userIds = ids.Split(',');
                foreach (string id in userIds)
                {
                    Logger.Info("Helper.GetEmailUsingUserID", id);
                    UserDetails detail = userInfoList.FirstOrDefault(p => p.UserId == id);

                    if (detail != null && !string.IsNullOrEmpty(detail.UserEmail))
                    {
                        Logger.Info("Helper.GetEmailUsingUserID got an user with email", detail.UserEmail);
                        userName = userName.Trim(',') + "," + detail.UserEmail;
                    }
                }
            }
            return userName.Trim(',');
        }

        #endregion

        #region Common Methods
        /// <summary>
        /// Gets the site URL.
        /// </summary>
        /// <param name="siteName">Name of the site.</param>
        /// <returns>
        /// string value
        /// </returns>
        public string GetSiteURL(string siteName)
        {
            string siteURL = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[SiteURLs.ROOTURL]) && !string.IsNullOrEmpty(ConfigurationManager.AppSettings[siteName]))
                {
                    siteURL = ConfigurationManager.AppSettings[SiteURLs.ROOTURL] + ConfigurationManager.AppSettings[siteName];
                }
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(ex);
                ErrorLogging.LogError(siteName + " not found in web.config");
            }
            return siteURL;
        }

        /// <summary>
        /// Get Confguration Variable of given Key
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns>
        /// Value of Configuration variable
        /// </returns>
        public string GetConfigVariable(string keyName)
        {
            string keyValue = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[keyName]))
                {
                    keyValue = ConfigurationManager.AppSettings[keyName];
                }
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(ex);
                ErrorLogging.LogError(keyName + " not found in web.config");
            }
            return keyValue;
        }

        /// <summary>
        /// Create client context of given site URL
        /// </summary>
        /// <param name="strURL">The string URL.</param>
        /// <returns>
        /// return clientcontext object
        /// </returns>
        public ClientContext CreateClientContext(string strURL)
        {
            ClientContext context = null;
            if (context == null)
            {
                using (context = new ClientContext(strURL))
                {
                    string userName = this.GetConfigVariable("spUserName");
                    string password = this.GetConfigVariable("spPassword");
                    ////string domain = this.GetConfigVariable("spDomain");
                    ////context.Credentials = new NetworkCredential(userName, password);
                    ////context.ExecutingWebRequest += new EventHandler<WebRequestEventArgs>(Ctx_MixedAuthRequest);

                    var passWord = new SecureString();
                    foreach (char c in password.ToCharArray())
                    {
                        passWord.AppendChar(c);
                    }
                    context.Credentials = new SharePointOnlineCredentials(userName, passWord);
                    ////var web = context.Web;
                    ////context.Load(web);
                    ////context.ExecuteQuery();
                }
            }

            return context;
        }

        /// <summary>
        /// Create Web Object based on context passed as parameter
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// web object
        /// </returns>
        public Web CreateWeb(ClientContext context)
        {
            Web web = null;
            if (context != null)
            {
                web = context.Web;
                context.Load(web);
                context.ExecuteQuery();
            }

            return web;
        }

        /// <summary>
        /// Gets the file bytes by URL.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// byte array
        /// </returns>
        public byte[] GetFileBytesByUrl(ClientContext context, string url)
        {
            using (var fileInformation = Microsoft.SharePoint.Client.File.OpenBinaryDirect(context, url))
            {
                IList<byte> content = new List<byte>();
                int b;
                while ((b = fileInformation.Stream.ReadByte()) != -1)
                {
                    content.Add((byte)b);
                }
                return content.ToArray();
            }
        }

        /// <summary>
        /// Gets the taxonomy field value collection.
        /// </summary>
        /// <param name="terms">The terms.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="sectionNames">The section names.</param>
        /// <returns>Returns Dictionary of SectionName and IsActive</returns>
        public Dictionary<string, bool> GetTaxonomyFieldValueCollection(TaxonomyFieldValueCollection terms, bool isActive, Dictionary<string, bool> sectionNames)
        {
            if (sectionNames == null)
            {
                sectionNames = new Dictionary<string, bool>();
            }
            if (terms != null)
            {
                foreach (TaxonomyFieldValue term in terms)
                {
                    sectionNames.Add(term.Label, isActive);
                }
            }
            return sectionNames;
        }

        /// <summary>
        /// Gets the taxonomy field value collection.
        /// </summary>
        /// <param name="terms">The terms.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="sectionNames">The section names.</param>
        /// <returns>Returns Dictionary of SectionName and IsActive</returns>
        public Dictionary<string, bool> GetTaxonomyFieldValueCollection(List<TaxonomyFieldValue> terms, bool isActive, Dictionary<string, bool> sectionNames)
        {
            if (sectionNames == null)
            {
                sectionNames = new Dictionary<string, bool>();
            }
            if (terms != null)
            {
                foreach (TaxonomyFieldValue term in terms)
                {
                    sectionNames.Add(term.Label, isActive);
                }
            }
            return sectionNames;
        }

        /// <summary>
        /// Gets the taxonomy field value collection.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="sectionNames">The section names.</param>
        /// <returns>Returns Dictionary of SectionName and IsActive</returns>
        public Dictionary<string, bool> GetTaxonomyFieldValueCollection(Dictionary<string, object> dictionary, bool isActive, Dictionary<string, bool> sectionNames)
        {
            if (!dictionary.ContainsKey(ObjectType) || !dictionary[ObjectType].Equals("SP.Taxonomy.TaxonomyFieldValueCollection"))
            {
                throw new InvalidOperationException("Dictionary value represents no TaxonomyFieldValueCollection.");
            }
            if (!dictionary.ContainsKey(ChildItems))
            {
                throw new InvalidOperationException(string.Format("Missing '{0}' key in TaxonomyFieldValueCollection field.", ChildItems));
            }
            var terms = new List<TaxonomyFieldValue>();
            foreach (var value in (object[])dictionary[ChildItems])
            {
                var childDictionary = (Dictionary<string, object>)value;
                terms.Add(this.ConvertDictionaryToTaxonomyFieldValue(childDictionary));
            }
            if (sectionNames == null)
            {
                sectionNames = new Dictionary<string, bool>();
            }
            if (terms != null)
            {
                foreach (TaxonomyFieldValue term in terms)
                {
                    sectionNames.Add(term.Label, isActive);
                }
            }
            return sectionNames;
        }

        /// <summary>
        /// Convert Dictionary To Taxonomy Field Value
        /// </summary>
        /// <param name="dictionary">dictionary value</param>
        /// <returns>Taxonomy Field Value</returns>
        public TaxonomyFieldValue ConvertDictionaryToTaxonomyFieldValue(Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey(ObjectType) || !dictionary[ObjectType].Equals("SP.Taxonomy.TaxonomyFieldValue"))
            {
                throw new InvalidOperationException("Dictionary value represents no TaxonomyFieldValue.");
            }

            return new TaxonomyFieldValue
            {
                Label = dictionary["Label"].ToString(),
                TermGuid = dictionary["TermGuid"].ToString(),
                WssId = int.Parse(dictionary["WssId"].ToString(), CultureInfo.InvariantCulture)
            };
        }

        /// <summary>
        /// Determines whether [is group member] [the specified context].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="userID">The user email.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>true or false</returns>
        public bool IsGroupMember(ClientContext context, Web web, string userID, string groupName)
        {
            bool isAuthorized = false;
            try
            {
                if (context != null && web != null && !string.IsNullOrEmpty(groupName) && !string.IsNullOrEmpty(userID))
                {
                    GroupCollection collGroup = context.Web.SiteGroups;
                    Group oGroup = collGroup.GetByName(groupName);
                    UserCollection collUser = oGroup.Users;
                    context.Load(collUser);
                    context.ExecuteQuery();
                    ////  User user = collUser.GetByLoginName(userID);
                    //*//User user = collUser.GetByEmail(userID);
                    User user = collUser.GetById(Convert.ToInt32(userID));
                    context.Load(user);
                    context.ExecuteQuery();
                    if (user != null)
                    {
                        isAuthorized = true;
                    }
                }
            }
            catch (Exception ex)
            {
                isAuthorized = false;
            }
            return isAuthorized;
        }

        /// <summary>
        /// Gets all files from list.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="lookupId">The lookup identifier.</param>
        /// <param name="mainListName">Name of the main list.</param>
        /// <param name="childListNames">The child list names.</param>
        /// <returns>
        /// List of Files
        /// </returns>
        public List<FileDetails> GetAllFilesFromList(ClientContext context, Web web, int lookupId, string mainListName, List<string> childListNames)
        {
            List<FileDetails> files = new List<FileDetails>();
            if (context != null && web != null)
            {
                AttachmentCollection attachments = null;
                if (!string.IsNullOrEmpty(mainListName))
                {
                    List mainList = web.Lists.GetByTitle(mainListName);
                    ListItem item = mainList.GetItemById(lookupId);
                    context.Load(item);
                    context.Load(context.Site);
                    context.ExecuteQuery();
                    if (Convert.ToString(item["Attachments"]) == "True")
                    {
                        context.Load(item.AttachmentFiles);
                        context.ExecuteQuery();
                        attachments = item.AttachmentFiles; //attachmentsFolder.Files;
                        List<FileDetails> mainListFiles = this.GetAttachmentsWithFileData(context, web, attachments);
                        if (mainListFiles != null && mainListFiles.Count > 0)
                        {
                            files.AddRange(mainListFiles);
                        }
                    }
                }
                if (childListNames != null && childListNames.Count > 0)
                {
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = @"<View>
                                         <Query>
                                                <Where>
                                                      <Eq>
                                                         <FieldRef Name='RequestID' LookupId='TRUE'/>
                                                         <Value Type='Lookup'>" + lookupId + @"</Value>
                                                       </Eq>
                                                     </Where>
                                            </Query>
                                            </View>";
                    foreach (string childListName in childListNames)
                    {
                        List childList = web.Lists.GetByTitle(childListName);

                        ListItemCollection childItems = childList.GetItems(query);
                        context.Load(childItems);
                        context.ExecuteQuery();
                        foreach (ListItem childItem in childItems)
                        {
                            attachments = null;
                            if (Convert.ToString(childItem["Attachments"]) == "True")
                            {
                                context.Load(childItem.AttachmentFiles);
                                context.ExecuteQuery();
                                attachments = childItem.AttachmentFiles; //attachmentsFolder.Files;
                                List<FileDetails> childListFiles = this.GetAttachmentsWithFileData(context, web, attachments);
                                if (childListFiles != null && childListFiles.Count > 0)
                                {
                                    files.AddRange(childListFiles);
                                }
                            }
                        }
                    }
                }
            }
            return files;
        }

        /// <summary>
        /// Gets the attachments with file data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="attachments">The attachments.</param>
        /// <returns>
        /// List of Files
        /// </returns>
        private List<FileDetails> GetAttachmentsWithFileData(ClientContext context, Web web, AttachmentCollection attachments)
        {
            List<FileDetails> objAttachmentFiles = new List<FileDetails>();
            if (attachments != null)
            {
                FileDetails fileDetail = null;
                foreach (var file in attachments)
                {
                    fileDetail = new FileDetails();
                    fileDetail.FileId = Guid.NewGuid().ToString();
                    fileDetail.FileName = file.FileName;
                    fileDetail.FileURL = file.ServerRelativeUrl;
                    fileDetail.FileContent = this.GetFileBytesByUrl(context, fileDetail.FileURL);
                    objAttachmentFiles.Add(fileDetail);
                }
            }
            return objAttachmentFiles;
        }

        /// <summary>
        /// Gets all users from group.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>all users email seprated by comma</returns>
        public string GetAllUsersFromGroup(ClientContext context, Web web, string groupName)
        {
            string allUsers = string.Empty;
            if (context != null && web != null && !string.IsNullOrEmpty(groupName))
            {
                Group group = web.SiteGroups.GetByName(groupName);
                UserCollection users = group.Users;
                context.Load(users);
                context.ExecuteQuery();
                foreach (User usr in users)
                {
                    allUsers = allUsers.Trim(',') + "," + usr.Email;
                }
                allUsers = allUsers.Trim(',');
            }
            return allUsers;
        }

        #endregion

        #region Save Related Methods

        /// <summary>
        /// Save Data in List Section Wise
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="section">The section.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="mailCustomValues">The mail custom values.</param>
        /// <param name="emailAttachments">The email attachments.</param>
        /// <returns>
        /// list of listitem details
        /// </returns>
        public List<ListItemDetail> SaveData(ClientContext context, Web web, ISection section, Dictionary<string, string> param, Dictionary<string, string> mailCustomValues = null, List<FileDetails> emailAttachments = null)
        {
            Logger.Info("helper.SaveData", param);
            List<ListItemDetail> objListItems = new List<ListItemDetail>();
            ListItemDetail objListItem = null;
            if (context != null && web != null && section != null)
            {
                dynamic actualSection = Convert.ChangeType(section, section.GetType());
                int listCount = actualSection.ListDetails.Count;
                if (listCount > 0)
                {
                    foreach (ListItemDetail listDetail in actualSection.ListDetails)
                    {
                        try
                        {
                            List list = web.Lists.GetByTitle(listDetail.ListName);
                            ListItem item = null;
                            bool isNewItem = false;
                            if (listDetail.ItemId > 0)
                            {
                                item = list.GetItemById(listDetail.ItemId);
                                context.Load(item);
                                context.ExecuteQuery();
                            }
                            else
                            {
                                item = list.AddItem(new ListItemCreationInformation());
                                isNewItem = true;
                            }
                            objListItem = new ListItemDetail();
                            if (param != null)
                            {
                                param[Parameter.ISNEWITEM] = isNewItem.ToString();
                            }
                            int itemId = this.SaveDataInList(context, web, actualSection, item, listDetail.ListName, param, mailCustomValues, emailAttachments);

                            objListItem.ListName = listDetail.ListName;
                            objListItem.ItemId = itemId;
                            objListItems.Add(objListItem);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(string.Format("Error While SaveData - List Name:{0},List ItemID:{1} Message:{2}, StackTrace: {3}", objListItem.ListName, objListItem.ItemId, ex.Message, ex.StackTrace));
                        }
                    }
                }
            }
            return objListItems;
        }

        /// <summary>
        /// Saves the attachment.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="files">The files.</param>
        /// <param name="item">The item.</param>
        /// <returns>true or false</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Already vaildated"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "By reference is required."), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "File memory stream closed. need to b opened at the time of save data")]
        public bool SaveAttachment(ClientContext context, List<FileDetails> files, ref ListItem item)
        {
            if (item != null && context != null && files != null)
            {
                foreach (FileDetails file in files)
                {
                    try
                    {
                        Logger.Info(string.Format("Helper.SaveAttachment FileName:{0}, FileSize:{1}, Status:{2}", file.FileName, file.FileId, file.Status));
                        if (file.Status == FileStatus.Delete)
                        {
                            var objFile = context.Web.GetFileByServerRelativeUrl(file.FileURL);
                            objFile.DeleteObject();
                            context.ExecuteQuery();
                        }
                        else if (file.Status == FileStatus.New)
                        {
                            MemoryStream mStream = new MemoryStream(file.FileContent);
                            AttachmentCreationInformation aci = new AttachmentCreationInformation();
                            aci.ContentStream = mStream;
                            aci.FileName = file.FileName;
                            item.AttachmentFiles.Add(aci);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(string.Format("Error While Save Attachment  - Item id:{0}, Message:{1}, StackTrace: {2}", item.Id, ex.Message, ex.StackTrace));
                    }
                }
                context.ExecuteQuery();
                Logger.Info("Helper.SaveAttachment file saved");
            }
            return true;
        }

        /// <summary>
        /// Save Section Data in list of all the related fields
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="actualSection">The actual section.</param>
        /// <param name="item">The item.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="mailCustomValues">The mail custom values.</param>
        /// <param name="emailAttachments">The email attachments.</param>
        /// <returns>
        /// int value
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "N/A"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Can not able to change anything here."), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "could not able to reduce size.")]
        public int SaveDataInList(ClientContext context, Web web, dynamic actualSection, ListItem item, string listName, Dictionary<string, string> param, Dictionary<string, string> mailCustomValues = null, List<FileDetails> emailAttachments = null)
        {
            int itemId = 0;
            List<FileDetails> files = null;
            List<ApplicationStatus> approverMatrixList = null;
            ApplicationStatus currentApproverDetails = null;
            string approverMatrixListName = string.Empty;
            List<List<ITask>> tasksList = new List<List<ITask>>();
            List<List<ITrans>> transList = new List<List<ITrans>>();
            Dictionary<string, dynamic> formFieldValues = new Dictionary<string, dynamic>();
            List<string> tasksListName = new List<string>();
            List<string> transListName = new List<string>();
            Dictionary<string, object> itemDictionary = new Dictionary<string, object>();
            string userIDs = string.Empty;
            if (context != null && web != null && param != null && item != null)
            {
                Logger.Info("Saving Actual Section Properties");

                /*
                 *  Save Data in Main List
                 */
                PropertyInfo[] properties = actualSection.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    bool isListCoumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn, isCurrentListCoumn = property.GetCustomAttribute<FieldBelongToListAttribute>() == null || property.GetCustomAttribute<FieldBelongToListAttribute>().ListNames.Contains(listName);
                    bool notSaved = property.GetCustomAttribute<NotSavedColumnAttribute>() != null && property.GetCustomAttribute<NotSavedColumnAttribute>().NotSavedColumn;

                    if (isListCoumn && isCurrentListCoumn && !notSaved)
                    {
                        string propertyName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                        if (property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile)
                        {
                            files = property.GetValue(actualSection) != null ? property.GetValue(actualSection) as List<FileDetails> : null;
                        }
                        else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson && !property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                        {
                            string users = (property.GetValue(actualSection) ?? string.Empty).Trim(',');
                            itemDictionary[propertyName] = this.GetMultiplePersonField(context, web, users, property);
                        }
                        else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson && property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                        {
                            ////Name Field not requried to save.
                        }
                        else
                        {
                            itemDictionary[propertyName] = property.GetValue(actualSection);
                        }
                    }
                    if (property.GetCustomAttribute<IsTaskAttribute>() != null && property.GetCustomAttribute<IsTaskAttribute>().IsTaskField)
                    {
                        string taskListName = property.GetCustomAttribute<IsTaskAttribute>().TaskListName;
                        List<ITask> taskList = property.GetValue(actualSection) as List<ITask>;
                        if (taskList != null && taskList.Count > 0 && !string.IsNullOrEmpty(taskListName))
                        {
                            tasksListName.Add(taskListName);
                            tasksList.Add(taskList);
                        }
                    }
                    ////Save Tran Data.
                    if (property.GetCustomAttribute<IsTranAttribute>() != null && property.GetCustomAttribute<IsTranAttribute>().IsTranField)
                    {
                        string tranListName = property.GetCustomAttribute<IsTranAttribute>().TranListName;
                        List<ITrans> tranList = property.GetValue(actualSection) as List<ITrans>;
                        if (tranList != null && tranList.Count > 0 && !string.IsNullOrEmpty(tranListName))
                        {
                            transListName.Add(tranListName);
                            transList.Add(tranList);
                        }
                    }

                    if (property.GetCustomAttribute<IsApproverDetailsAttribute>() != null && property.GetCustomAttribute<IsApproverDetailsAttribute>().IsApproverDetailsField)
                    {
                        approverMatrixListName = property.GetCustomAttribute<IsApproverDetailsAttribute>().ApproverMatrixListName;
                        var propValue = property.GetValue(actualSection);
                        if (propValue != null)
                        {
                            currentApproverDetails = propValue as ApplicationStatus;
                        }
                    }
                    if (property.GetCustomAttribute<IsApproverMatrixFieldAttribute>() != null && property.GetCustomAttribute<IsApproverMatrixFieldAttribute>().IsApproverMatrixField)
                    {
                        approverMatrixListName = property.GetCustomAttribute<IsApproverMatrixFieldAttribute>().ApproverMatrixListName;
                        var propValue = property.GetValue(actualSection);
                        if (propValue != null)
                        {
                            if (approverMatrixList == null)
                            {
                                approverMatrixList = propValue;
                            }
                            else
                            {
                                approverMatrixList.AddRange(propValue);
                            }
                        }
                    }
                    if (property.GetCustomAttribute<IsViewerAttribute>() != null && property.GetCustomAttribute<IsViewerAttribute>().IsViewer && property.GetCustomAttribute<IsPersonAttribute>() != null && !property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                    {
                        if (!string.IsNullOrEmpty(userIDs))
                        {
                            if (!string.IsNullOrEmpty(property.GetValue(actualSection)) && !userIDs.Contains(property.GetValue(actualSection)))
                            {
                                userIDs = userIDs.Trim(',') + "," + property.GetValue(actualSection);
                            }
                        }
                        else
                        {
                            userIDs = property.GetValue(actualSection);
                        }
                    }
                }
                foreach (KeyValuePair<string, object> field in itemDictionary)
                {
                    item[field.Key] = itemDictionary[field.Key];
                }
                item["_ModerationStatus"] = 0;
                item.Update();
                context.Load(item);
                item.RefreshLoad();  //  Added for Version Conflict!
                context.Load(item);
                context.ExecuteQuery();

                /*  Data saved in Main List  */
                itemId = Convert.ToInt32(item["ID"].ToString());
                Logger.Info("Actual Section Properties Saved", itemId);
                bool isNewItem = param.ContainsKey(Parameter.ISNEWITEM) ? Convert.ToBoolean(param[Parameter.ISNEWITEM]) : false;
                if (isNewItem)
                {
                    if (param.ContainsKey(Parameter.APPLICATIONNAME))
                    {
                        formFieldValues.Add("ReferenceNo", GenerateReferenceNo(param[Parameter.FROMNAME], item["ID"].ToString(), Convert.ToDateTime(item["RequestDate"])));
                        Logger.Info("Reference Number Generated '" + formFieldValues["ReferenceNo"] + "'");
                    }
                }

                /* Save Approver Matrix in List*/
                List<ApplicationStatus> approversDataFromList = null;
                AppApprovalMatrixHelper approverHelper = new AppApprovalMatrixHelper();
                string currentUserID = param[Parameter.USEREID], impersonatorEmail = param[Parameter.USEREID];
                int currLevel = 0, nextLevel = 0, previousLevel = 0;
                string nextApprover = string.Empty, formLevel = string.Empty, nextApproverRole = string.Empty, currentUserRole = string.Empty;
                formLevel = this.GetFormLevel(context, web, listName, itemId);
                previousLevel = Convert.ToInt32(formLevel.Split('|')[0]);
                currLevel = Convert.ToInt32(formLevel.Split('|')[1]);
                nextLevel = currLevel;
                bool isSuperAdminCase = true;
                if (!string.IsNullOrEmpty(approverMatrixListName))
                {
                    Logger.Info("Saving Local Approver Matrix '" + approverMatrixListName + "'" + " \nList=" + JsonConvert.SerializeObject(approversDataFromList));
                    if (isNewItem)
                    {
                        approversDataFromList = approverHelper.GetGlobalAppApproverMatrix(param[Parameter.APPLICATIONNAME], param[Parameter.FROMNAME]);

                        ////if (approversDataFromList.Any(p => p.Role.Contains(UserRoles.CREATOR)) && !string.IsNullOrEmpty(actualSection.RequestBy))
                        ////{
                        ////    approversDataFromList.FirstOrDefault(p => p.Role.Contains(UserRoles.CREATOR)).Approver = actualSection.RequestBy;
                        ////    approversDataFromList.FirstOrDefault(p => p.Role.Contains(UserRoles.CREATOR)).RequestID = itemId;
                        ////}

                        if (approversDataFromList.Any(p => p.Role.Contains(UserRoles.CREATOR)) && !string.IsNullOrEmpty(actualSection.ProposedBy))
                        {
                            approversDataFromList.FirstOrDefault(p => p.Role.Contains(UserRoles.CREATOR)).Approver = actualSection.ProposedBy;
                            approversDataFromList.FirstOrDefault(p => p.Role.Contains(UserRoles.CREATOR)).RequestID = itemId;
                        }
                        approversDataFromList.ForEach(p =>
                        {
                            p.Status = ApproverStatus.NOTASSIGNED;
                        });
                    }
                    else
                    {
                        approversDataFromList = approverHelper.GetAppApprovalMatrix(context, web, itemId, approverMatrixListName);
                    }
                    if (!approversDataFromList.Any(p => Convert.ToInt32(p.Levels) == currLevel && !string.IsNullOrEmpty(p.Approver) && p.Approver.Contains(currentUserID)))
                    {
                        if (this.IsGroupMember(context, web, currentUserID, UserRoles.ADMINISTRATOR) || this.IsGroupMember(context, web, currentUserID, UserRoles.DIVISIONADMINISTRATOR))
                        {
                            /**
                             * If User is Super Admin /Division Admin >
                             * Get User from Approval Matrix with Current Pending Form Level & Approval is required
                             * if No Required approver is found > find First User from Current Level
                             * if Both of above Condition fails > it will check currentApproverDetails
                             * if currentApproverDetails found then it will set current user as currentApproverDetails's User
                             */
                            if (approversDataFromList.Any(p => Convert.ToInt32(p.Levels) == currLevel && !string.IsNullOrEmpty(p.Approver) && !string.IsNullOrEmpty(p.Status) && p.Status == ApproverStatus.PENDING && p.IsOptional != true))
                            {
                                currentUserID = approversDataFromList.FirstOrDefault(p => Convert.ToInt32(p.Levels) == currLevel && !string.IsNullOrEmpty(p.Approver) && !string.IsNullOrEmpty(p.Status) && p.Status == ApproverStatus.PENDING && p.IsOptional != true).Approver;
                            }
                            else if (approversDataFromList.Any(p => Convert.ToInt32(p.Levels) == currLevel && !string.IsNullOrEmpty(p.Approver) && !string.IsNullOrEmpty(p.Status) && p.Status == ApproverStatus.PENDING))
                            {
                                currentUserID = approversDataFromList.FirstOrDefault(p => Convert.ToInt32(p.Levels) == currLevel && !string.IsNullOrEmpty(p.Approver) && !string.IsNullOrEmpty(p.Status) && p.Status == ApproverStatus.PENDING).Approver;
                            }
                            else if (currentApproverDetails != null && approversDataFromList.Any(p => p.Role == currentApproverDetails.Role))
                            {
                                currentUserID = currentApproverDetails.Approver;
                            }
                            /*Current Approver Logic End*/
                        }
                        else
                        {
                            isSuperAdminCase = false;
                        }
                    }
                    else
                    {
                        isSuperAdminCase = false;
                    }
                }

                if (approverMatrixList != null)
                {
                    approversDataFromList.ForEach(p =>
                    {
                        p.RequestID = itemId;
                        if (approverMatrixList.FirstOrDefault(m => m.Role == p.Role && m.Levels != null && m.Levels == p.Levels) != null)
                        {
                            p.Approver = approverMatrixList.FirstOrDefault(m => m.Role == p.Role && m.Levels == p.Levels) != null ? approverMatrixList.FirstOrDefault(m => m.Role == p.Role && m.Levels == p.Levels).Approver : p.Approver;
                            p.Status = approverMatrixList.FirstOrDefault(m => m.Role == p.Role && m.Levels == p.Levels) != null && approverMatrixList.FirstOrDefault(m => m.Role == p.Role && m.Levels == p.Levels).Status != null ? approverMatrixList.FirstOrDefault(m => m.Role == p.Role && m.Levels == p.Levels).Status : p.Status;
                            p.Comments = approverMatrixList.FirstOrDefault(m => m.Role == p.Role && m.Levels == p.Levels) != null && !string.IsNullOrEmpty(approverMatrixList.FirstOrDefault(m => m.Role == p.Role && m.Levels == p.Levels).Comments) ? approverMatrixList.FirstOrDefault(m => m.Role == p.Role && m.Levels == p.Levels).Comments : p.Comments;
                        }
                        else
                        {
                            p.Approver = approverMatrixList.FirstOrDefault(m => m.Role == p.Role) != null ? approverMatrixList.FirstOrDefault(m => m.Role == p.Role).Approver : p.Approver;
                            p.Status = (approverMatrixList.FirstOrDefault(m => m.Role == p.Role) != null && approverMatrixList.FirstOrDefault(m => m.Role == p.Role).Status != null) ? approverMatrixList.FirstOrDefault(m => m.Role == p.Role).Status : p.Status;
                            p.Comments = approverMatrixList.FirstOrDefault(m => m.Role == p.Role && m.Levels == p.Levels) != null && !string.IsNullOrEmpty(approverMatrixList.FirstOrDefault(m => m.Role == p.Role && m.Levels == p.Levels).Comments) ? approverMatrixList.FirstOrDefault(m => m.Role == p.Role && m.Levels == p.Levels).Comments : p.Comments;
                        }
                    });
                }
                if (approversDataFromList != null)
                {
                    approversDataFromList.ForEach(p =>
                       {
                           if (p.Role.Equals(UserRoles.VIEWER) && !string.IsNullOrEmpty(userIDs))
                           {
                               if (string.IsNullOrEmpty(p.Approver))
                               {
                                   p.Approver = userIDs;
                               }
                               else
                               {
                                   p.Approver = p.Approver + "," + userIDs;
                               }
                               p.Approver = p.Approver.Trim(',');
                           }
                           Logger.Info("Set Viewer  Approver ID='" + p.Approver + "' and RequestId='" + p.RequestID + "'");
                       });
                }

                if (currentApproverDetails != null && approversDataFromList.Any(p => p.Role == currentApproverDetails.Role))
                {
                    if (!string.IsNullOrEmpty(currentApproverDetails.Comments))
                    {
                        if (!string.IsNullOrEmpty(currentApproverDetails.Levels))
                        {
                            approversDataFromList.FirstOrDefault(p => p.Role == currentApproverDetails.Role && p.Levels == currentApproverDetails.Levels).Comments = currentApproverDetails.Comments;
                        }
                        else
                        {
                            approversDataFromList.FirstOrDefault(p => p.Role == currentApproverDetails.Role).Comments = currentApproverDetails.Comments;
                        }
                    }
                    if (!string.IsNullOrEmpty(currentApproverDetails.Approver))
                    {
                        approversDataFromList.FirstOrDefault(p => p.Role == currentApproverDetails.Role).Approver = currentApproverDetails.Approver;
                        ////if (isSuperAdminCase)
                        //// {
                        ////if (!string.IsNullOrEmpty(currentApproverDetails.Levels))
                        ////{
                        ////    approversDataFromList.FirstOrDefault(p => p.Role == currentApproverDetails.Role && p.Levels == currentApproverDetails.Levels).Comments = "Action By Admin '" + BELDataAccessLayer.GetNameUsingUserID(context, web, impersonatorEmail) + "' On BehalfOf '" + currentApproverDetails.ApproverName + "', Admin Comment : " + currentApproverDetails.Comments;
                        ////}
                        ////else
                        ////{
                        ////    approversDataFromList.FirstOrDefault(p => p.Role == currentApproverDetails.Role).Comments = "Action By Admin '" + BELDataAccessLayer.GetNameUsingUserID(context, web, impersonatorEmail) + "' On BehalfOf '" + currentApproverDetails.ApproverName + "', Admin Comment : " + currentApproverDetails.Comments;
                        ////}
                        //// }
                    }
                    if (!string.IsNullOrEmpty(currentApproverDetails.ReasonForChange))
                    {
                        approversDataFromList.FirstOrDefault(p => p.Role == currentApproverDetails.Role).ReasonForChange = currentApproverDetails.ReasonForChange;
                    }
                    if (!string.IsNullOrEmpty(currentApproverDetails.ReasonForDelay))
                    {
                        approversDataFromList.FirstOrDefault(p => p.Role == currentApproverDetails.Role).ReasonForDelay = currentApproverDetails.ReasonForDelay;
                    }
                    if (currentApproverDetails.Files != null && currentApproverDetails.Files.Count > 0)
                    {
                        if (approversDataFromList.FirstOrDefault(p => p.Role == currentApproverDetails.Role).Files == null)
                        {
                            approversDataFromList.FirstOrDefault(p => p.Role == currentApproverDetails.Role).Files = new List<FileDetails>();
                        }
                        approversDataFromList.FirstOrDefault(p => p.Role == currentApproverDetails.Role).Files = currentApproverDetails.Files;
                    }
                }
                param[Parameter.USEREID] = currentUserID;
                string taskOwners = string.Empty, taskViewers = string.Empty;
                for (int i = 0; i < tasksList.Count; i++)
                {
                    foreach (ITask task in tasksList[i])
                    {
                        taskOwners = this.GetAllTaskOwners(task, taskOwners);
                    }
                }
                Logger.Info("Tasks Saved if any");
                string approvalMatrixUsers = string.Empty;
                if (!string.IsNullOrEmpty(approverMatrixListName))
                {
                    //approversDataFromList.ForEach(p =>
                    //{
                    //    if (!string.IsNullOrEmpty(p.Approver) && p.Role != UserRoles.TASKVIEWER)
                    //    {
                    //        if (!approvalMatrixUsers.Contains(p.Approver))
                    //        {
                    //            approvalMatrixUsers = approvalMatrixUsers.Trim(',') + "," + p.Approver;
                    //        }
                    //    }
                    //});
                    // taskViewers = approvalMatrixUsers.Trim(',');
                    //if (approversDataFromList.Any(p => p.Role == UserRoles.TASKOWNER))
                    //{
                    //    approversDataFromList.FirstOrDefault(p => p.Role == UserRoles.TASKOWNER).Approver = taskOwners.Trim(',');
                    //}
                    //if (approversDataFromList.Any(p => p.Role == UserRoles.TASKVIEWER))
                    //{
                    //    approversDataFromList.FirstOrDefault(p => p.Role == UserRoles.TASKVIEWER).Approver = taskViewers.Trim(',');
                    //}
                    Logger.Info("Tasks approvers Saved if any");
                }

                /* Save Task in List*/
                ////if (tasksList != null && tasksList.Count > 0 && tasksList.Count == tasksListName.Count)
                ////{
                ////    AsyncHelper.Call(obj => { this.SaveAllTasksOfSection(context, web, tasksList, tasksListName, approvalMatrixUsers, itemId, param); });
                ////}

                /* Save Trans in List*/
                if (transList != null && transList.Count > 0 && transList.Count == transListName.Count)
                {
                    AsyncHelper.Call(obj => { this.SaveAllTransOfSection(context, web, transList, transListName, approvalMatrixUsers, itemId, param); });
                }
                Logger.Info("Started form level assignment");

                ButtonActionStatus actionPerformed = actualSection.ActionStatus;
                if (!string.IsNullOrEmpty(approverMatrixListName))
                {
                    approversDataFromList.ForEach(p =>
                    {
                        if (!string.IsNullOrEmpty(p.Role) && !string.IsNullOrEmpty(p.Approver))
                        {
                            string userRole = p.Role.Replace(" ", string.Empty);
                            formFieldValues[userRole.Substring(0, userRole.Length > 32 ? 32 : userRole.Length)] = GetFieldUserValueFromPerson(context, web, p.Approver.Trim(',').Split(','));
                        }
                    });
                    if (approversDataFromList.Any(p => Convert.ToInt32(p.Levels) == currLevel && !string.IsNullOrEmpty(p.Approver) && p.Approver.Contains(currentUserID)))
                    {
                        currentUserRole = approversDataFromList.FirstOrDefault(p => Convert.ToInt32(p.Levels) == currLevel && !string.IsNullOrEmpty(p.Approver) && p.Approver.Contains(currentUserID)).Role;
                    }
                    Logger.Info("Calling approversDataFromList.UpdateStatusofApprovalMatrix => currlevel=" + currLevel + ", previousLevel=" + previousLevel + ", param=" + JsonConvert.SerializeObject(param) + ", actionperformed=" + actionPerformed.ToString() + ", list=" +
                        JsonConvert.SerializeObject(approversDataFromList));
                    approversDataFromList = approverHelper.UpdateStatusofApprovalMatrix(approversDataFromList, currLevel, previousLevel, param, actionPerformed);
                    Logger.Info("called approversDataFromList.UpdateStatusofApprovalMatrix => currlevel=" + currLevel + ", previousLevel=" + previousLevel + ", param=" + JsonConvert.SerializeObject(param) + ", actionperformed=" + actionPerformed.ToString() + ", list=" +
                        JsonConvert.SerializeObject(approversDataFromList));
                    param[Parameter.USEREID] = impersonatorEmail;
                    //// 30/03/2019 Jatin: added in if condition actionPerformed != ButtonActionStatus.SendBack && actionPerformed != ButtonActionStatus.Forward
                    if (!approversDataFromList.Any(p => p.IsOptional == false && !string.IsNullOrEmpty(p.Approver) && p.Status != ApproverStatus.APPROVED && Convert.ToInt32(p.Levels) == currLevel)
                        && actionPerformed != ButtonActionStatus.SendBack && actionPerformed != ButtonActionStatus.Forward)
                    {
                        ApplicationStatus approver = approversDataFromList.Where(p => Convert.ToInt32(p.Levels) > currLevel && !string.IsNullOrEmpty(p.Approver) && p.Status != ApproverStatus.NOTREQUIRED).OrderBy(p => Convert.ToInt32(p.Levels)).FirstOrDefault();
                        if (approver != null)
                        {
                            nextLevel = Convert.ToInt32(approver.Levels);
                            List<ApplicationStatus> listofNextApprovers = approversDataFromList.Where(p => Convert.ToInt32(p.Levels) == nextLevel).ToList();
                            listofNextApprovers.ForEach(p =>
                            {
                                if (!string.IsNullOrEmpty(p.Approver))
                                {
                                    if (string.IsNullOrEmpty(nextApprover))
                                    {
                                        nextApproverRole = p.Role;
                                        nextApprover = p.Approver;
                                    }
                                    else
                                    {
                                        if (!nextApprover.Contains(p.Approver))
                                        {
                                            nextApproverRole = nextApproverRole.Trim(',') + "," + p.Role;
                                            nextApprover = nextApprover.Trim(',') + "," + p.Approver;
                                        }
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        if (actionPerformed == ButtonActionStatus.NextApproval || actionPerformed == ButtonActionStatus.Delegate)
                        {
                            ApplicationStatus approver = approversDataFromList.Where(p => Convert.ToInt32(p.Levels) > currLevel && !string.IsNullOrEmpty(p.Approver) && p.Status != ApproverStatus.NOTREQUIRED).OrderBy(p => Convert.ToInt32(p.Levels)).FirstOrDefault();
                            if (approver != null)
                            {
                                List<ApplicationStatus> listofNextApprovers = approversDataFromList.Where(p => Convert.ToInt32(p.Levels) == nextLevel && p.Status == ApproverStatus.PENDING).ToList();
                                listofNextApprovers.ForEach(p =>
                                {
                                    if (!string.IsNullOrEmpty(p.Approver))
                                    {
                                        if (string.IsNullOrEmpty(nextApprover))
                                        {
                                            nextApproverRole = p.Role;
                                            nextApprover = p.Approver;
                                        }
                                        else
                                        {
                                            if (!nextApprover.Contains(p.Approver))
                                            {
                                                nextApproverRole = nextApproverRole.Trim(',') + "," + p.Role;
                                                nextApprover = nextApprover.Trim(',') + "," + p.Approver;
                                            }
                                        }
                                    }
                                });
                            }
                            currLevel = previousLevel;
                        }
                    }
                    ////Handel Send Back or back to Creator Case.
                    if (actionPerformed == ButtonActionStatus.SendBack && param.ContainsKey(Parameter.SENDTOLEVEL) && param[Parameter.SENDTOLEVEL] != null && !string.IsNullOrEmpty(param[Parameter.SENDTOLEVEL]))
                    {
                        nextLevel = Convert.ToInt32(param[Parameter.SENDTOLEVEL]);
                        List<ApplicationStatus> listofNextApprovers = approversDataFromList.Where(p => Convert.ToInt32(p.Levels) == nextLevel && p.Status == ApproverStatus.PENDING).ToList();
                        nextApprover = string.Empty; // 30/03/2019 Jatin: to reset nextapprover field
                        listofNextApprovers.ForEach(p =>
                        {
                            if (!string.IsNullOrEmpty(p.Approver))
                            {
                                if (string.IsNullOrEmpty(nextApprover))
                                {
                                    nextApproverRole = p.Role;
                                    nextApprover = p.Approver;
                                }
                                else
                                {
                                    if (!nextApprover.Contains(p.Approver))
                                    {
                                        nextApproverRole = nextApproverRole.Trim(',') + "," + p.Role;
                                        nextApprover = nextApprover.Trim(',') + "," + p.Approver;
                                    }
                                }
                            }
                        });
                    }
                    if (actionPerformed == ButtonActionStatus.SendForward && param.ContainsKey(Parameter.SENDTOLEVEL) && param[Parameter.SENDTOLEVEL] != null && !string.IsNullOrEmpty(param[Parameter.SENDTOLEVEL]))
                    {
                        nextLevel = Convert.ToInt32(param[Parameter.SENDTOLEVEL]);
                        ApplicationStatus approver = approversDataFromList.Where(p => Convert.ToInt32(p.Levels) >= nextLevel && !string.IsNullOrEmpty(p.Approver)).OrderBy(p => Convert.ToInt32(p.Levels)).FirstOrDefault();
                        if (approver != null)
                        {
                            nextLevel = Convert.ToInt32(approver.Levels);
                            List<ApplicationStatus> listofNextApprovers = approversDataFromList.Where(p => !string.IsNullOrEmpty(p.Approver) && Convert.ToInt32(p.Levels) == nextLevel).ToList();
                            nextApprover = string.Empty; // 30/03/2019 Jatin: to reset nextapprover field
                            listofNextApprovers.ForEach(p =>
                            {
                                if (!string.IsNullOrEmpty(p.Approver))
                                {
                                    if (string.IsNullOrEmpty(nextApprover))
                                    {
                                        nextApproverRole = p.Role;
                                        nextApprover = p.Approver;
                                    }
                                    else
                                    {
                                        if (!nextApprover.Contains(p.Approver))
                                        {
                                            nextApproverRole = nextApproverRole.Trim(',') + "," + p.Role;
                                            nextApprover = nextApprover.Trim(',') + "," + p.Approver;
                                        }
                                    }
                                }
                            });
                        }
                    }
                }
                Logger.Info("End of form level assignment");
                bool makeAllUsersViewer = false;
                Logger.Info("Case Action Performed '" + actionPerformed.ToString() + "'");
                bool isTaskAssignMailSend = false;
                switch (actionPerformed)
                {
                    case ButtonActionStatus.SaveAsDraft:
                        nextLevel = currLevel;
                        currLevel = previousLevel;
                        formFieldValues.Add("Status", FormStatus.SAVEASDRAFT);
                        formFieldValues.Add("NextApprover", GetFieldUserValueFromPerson(context, web, currentUserID.Trim(',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
                        break;
                    case ButtonActionStatus.SaveAndStatusUpdate:
                    case ButtonActionStatus.SaveAndStatusUpdateWithEmail:
                    case ButtonActionStatus.ConfirmSave:
                        formFieldValues.Add("Status", FormStatus.SAVE);
                        break;
                    case ButtonActionStatus.Save:
                        formFieldValues.Add("Status", FormStatus.SAVE);
                        makeAllUsersViewer = true;
                        break;
                    case ButtonActionStatus.Submit:
                        nextLevel = currLevel;
                        currLevel = previousLevel;
                        formFieldValues.Add("Status", FormStatus.SUBMITTED);
                        makeAllUsersViewer = true;
                        break;
                    case ButtonActionStatus.UpdateAndRepublish:
                        nextLevel = currLevel;
                        currLevel = previousLevel;
                        formFieldValues.Add("Status", FormStatus.UPDATEANDREPUBLISH);
                        break;
                    case ButtonActionStatus.Reschedule:
                        nextLevel = currLevel;
                        currLevel = previousLevel;
                        formFieldValues.Add("Status", FormStatus.RESCHEDULED);
                        formFieldValues.Add("IsReschedule", false);
                        break;
                    case ButtonActionStatus.ReadyToPublish:
                        nextLevel = currLevel;
                        currLevel = previousLevel;
                        formFieldValues.Add("Status", FormStatus.READYTOPUBLISH);
                        break;
                    case ButtonActionStatus.Delegate:
                    case ButtonActionStatus.NextApproval:
                        formFieldValues.Add("LastActionPerformed", actionPerformed.ToString());
                        formFieldValues.Add("LastActionBy", GetFieldUserValueFromPerson(context, web, currentUserID));
                        formFieldValues.Add("LastActionByRole", currentUserRole);
                        formFieldValues.Add("PendingWith", nextApproverRole);
                        if (!string.IsNullOrEmpty(nextApprover))
                        {
                            FieldUserValue[] nextApproverFieldValue = GetFieldUserValueFromPerson(context, web, nextApprover.Trim(',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                            formFieldValues.Add("NextApprover", nextApproverFieldValue);
                            formFieldValues.Add("FormLevel", currLevel + "|" + nextLevel);
                            formFieldValues.Add("ApprovalStatus", FormApprovalStatus.INPROGRESS);
                            formFieldValues.Add("Status", FormStatus.SUBMITTED);
                        }
                        else
                        {
                            nextLevel = currLevel;
                            formFieldValues.Add("NextApprover", string.Empty);
                            formFieldValues.Add("FormLevel", currLevel + "|" + currLevel);
                            formFieldValues.Add("ApprovalStatus", FormApprovalStatus.COMPLETED);
                            formFieldValues.Add("Status", FormStatus.COMPLETED);
                            makeAllUsersViewer = true;
                            isTaskAssignMailSend = true;
                        }
                        break;
                    case ButtonActionStatus.BackToCreator:
                        formFieldValues.Add("LastActionPerformed", actionPerformed.ToString());
                        formFieldValues.Add("LastActionBy", GetFieldUserValueFromPerson(context, web, currentUserID));
                        formFieldValues.Add("LastActionByRole", currentUserRole);
                        formFieldValues.Add("PendingWith", nextApproverRole);
                        formFieldValues.Add("NextApprover", null);
                        formFieldValues.Add("FormLevel", currLevel + "|" + nextLevel);
                        formFieldValues.Add("Status", FormStatus.SENTBACK);
                        break;
                    case ButtonActionStatus.Cancel:
                        formFieldValues.Add("Status", FormStatus.CANCELLED);
                        formFieldValues.Add("NextApprover", string.Empty);
                        formFieldValues.Add("PendingWith", string.Empty);
                        nextLevel = currLevel;
                        currLevel = previousLevel;
                        makeAllUsersViewer = true;
                        break;
                    case ButtonActionStatus.Rejected:
                        formFieldValues.Add("Status", FormStatus.REJECTED);
                        formFieldValues.Add("NextApprover", string.Empty);
                        formFieldValues.Add("PendingWith", string.Empty);
                        nextLevel = currLevel;
                        currLevel = previousLevel;
                        makeAllUsersViewer = true;
                        break;
                    case ButtonActionStatus.Complete:
                        formFieldValues.Add("ApprovalStatus", FormApprovalStatus.COMPLETED);
                        formFieldValues.Add("Status", FormStatus.COMPLETED);
                        formFieldValues.Add("FormLevel", currLevel + "|" + currLevel);
                        formFieldValues.Add("NextApprover", string.Empty);
                        formFieldValues.Add("PendingWith", string.Empty);
                        makeAllUsersViewer = true;
                        isTaskAssignMailSend = true;
                        break;
                    case ButtonActionStatus.SendBack:
                        formFieldValues.Add("LastActionPerformed", actionPerformed.ToString());
                        if (param.ContainsKey(Parameter.SENDTOLEVEL) && param[Parameter.SENDTOLEVEL] != null && !string.IsNullOrEmpty(param[Parameter.SENDTOLEVEL]))
                        {
                            if (!string.IsNullOrEmpty(nextApprover))
                            {
                                FieldUserValue[] nextApproverFieldValue = GetFieldUserValueFromPerson(context, web, nextApprover.Trim(',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                                formFieldValues.Add("NextApprover", nextApproverFieldValue);
                            }
                            formFieldValues.Add("LastActionBy", GetFieldUserValueFromPerson(context, web, currentUserID));
                            formFieldValues.Add("LastActionByRole", currentUserRole);
                            formFieldValues.Add("PendingWith", nextApproverRole);
                            formFieldValues.Add("FormLevel", currLevel + "|" + nextLevel);
                            formFieldValues.Add("Status", FormStatus.SENTBACK);
                        }
                        break;
                    case ButtonActionStatus.SendForward:
                        formFieldValues.Add("LastActionPerformed", actionPerformed.ToString());
                        if (param.ContainsKey(Parameter.SENDTOLEVEL) && param[Parameter.SENDTOLEVEL] != null && !string.IsNullOrEmpty(param[Parameter.SENDTOLEVEL]))
                        {
                            formFieldValues.Add("LastActionBy", GetFieldUserValueFromPerson(context, web, currentUserID));
                            formFieldValues.Add("LastActionByRole", currentUserRole);
                            formFieldValues.Add("PendingWith", nextApproverRole);
                            if (!string.IsNullOrEmpty(nextApprover))
                            {
                                FieldUserValue[] nextApproverFieldValue = GetFieldUserValueFromPerson(context, web, nextApprover.Trim(',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                                formFieldValues.Add("NextApprover", nextApproverFieldValue);
                                formFieldValues.Add("FormLevel", currLevel + "|" + nextLevel);
                                formFieldValues.Add("ApprovalStatus", FormApprovalStatus.INPROGRESS);
                                formFieldValues.Add("Status", FormStatus.SUBMITTED);
                            }
                            else
                            {
                                //Complete if no approver found
                                formFieldValues.Add("ApprovalStatus", FormApprovalStatus.COMPLETED);
                                formFieldValues.Add("Status", FormStatus.COMPLETED);
                                formFieldValues.Add("FormLevel", currLevel + "|" + currLevel);
                                formFieldValues.Add("NextApprover", string.Empty);
                                makeAllUsersViewer = true;
                                isTaskAssignMailSend = true;
                            }
                        }
                        break;
                    default:
                        nextLevel = currLevel;
                        currLevel = previousLevel;
                        break;
                }
                if (formFieldValues.Count > 0)
                {
                    if (formFieldValues.ContainsKey("Status"))
                    {
                        formFieldValues = this.UpdateWorkFlowStatus(formFieldValues, param);
                    }
                    this.SaveFormFields(context, web, listName, itemId, formFieldValues);
                    foreach (var formItem in formFieldValues)
                    {
                        PropertyInfo prop = actualSection.GetType().GetProperty(formItem.Key);
                        if (formFieldValues.ContainsKey(formItem.Key) && prop != null)
                        {
                            try
                            {
                                object value = formItem.Key == "NextApprover" ? value = nextApprover : formItem.Value;
                                Type t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                                prop.SetValue(actualSection, safeValue);
                                Logger.Info("Save Form Field Name='" + prop.Name + "'='" + safeValue.ToString() + "'");
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(string.Format("Error While Save Fields FieldName:{0}, Message{1}, StackTrace{2}", formItem.Key, ex.Message, ex.StackTrace));
                            }
                        }
                    }
                }
                if (files != null && files.Count > 0)
                {
                    this.SaveAttachment(context, files, ref item);
                }

                AsyncHelper.Call(obj =>
                {
                    Logger.Info("Calling Activity Log -> " + JsonConvert.SerializeObject(param));
                    string activity = Convert.ToString(param[Parameter.ACTIONPER]).Split('|')[0];
                    this.SaveActivityLog(context, web, param[Parameter.ACTIVITYLOG], itemId, actualSection, param[Parameter.USEREID], activity);
                    Logger.Info("Calling Activity Log -> Complete");
                });

                /* Save Approver Matrix & Give Permission*/
                if (approversDataFromList != null)
                {
                    Dictionary<string, string> usersWithRole = approverHelper.GetPermissionDictionary(approversDataFromList, nextLevel, currLevel, makeAllUsersViewer);
                    Logger.Info("Calling SetItemPermission -> itemid=" + itemId + ", listname=" + listName + ", " + JsonConvert.SerializeObject(usersWithRole));
                    this.SetItemPermission(context, web, itemId, listName, usersWithRole);
                    Logger.Info("Calling SetItemPermission -> Complete");
                    Logger.Info("Calling SaveAproverMatrix -> " + approverMatrixListName);
                    approverHelper.SaveApproverMatrix(context, web, approversDataFromList, approverMatrixListName);
                    Logger.Info("Calling SaveAproverMatrix -> Complete");
                }
                /* Save Approver Matrix & Give Permission End*/

                /* Send Email commemt by priya*/
                AsyncHelper.Call(obj =>
                {
                    this.SendMail(actionPerformed, context, web, Convert.ToString(param[Parameter.USEREID]), itemId, approversDataFromList, listName, nextLevel, currLevel, param, mailCustomValues, emailAttachments);
                });
                /*Send Email End*/
            }
            return itemId;
        }

        /// <summary>
        /// Saves all trans of section.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="transList">The trans list.</param>
        /// <param name="transListName">Name of the trans list.</param>
        /// <param name="approvalMatrixUsers">The approval matrix users.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="objparam">The objparam.</param>
        /// <returns>
        /// true or false
        /// </returns>
        private bool SaveAllTransOfSection(ClientContext context, Web web, List<List<ITrans>> transList, List<string> transListName, string approvalMatrixUsers, int itemId, Dictionary<string, string> objparam)
        {
            bool isSuccess = true;
            for (int i = 0; i < transList.Count && isSuccess; i++)
            {
                transList[i].ForEach(p => p.RequestID = itemId);
                TransactionHelper transactionHelper = new TransactionHelper();
                Logger.Info("Helper.SaveAllTransOfSection transaction '" + transListName[i] + "' list = " + JsonConvert.SerializeObject(transList[i]));
                isSuccess = transactionHelper.SaveTranItems(context, web, transList[i], transListName[i], new Dictionary<string, string> { { Parameter.SETPERMISSION, "true" }, { Parameter.APPROVERMATRIXUSER, approvalMatrixUsers.Trim(',') } });
                Logger.Info("Helper.SaveAllTransOfSection transaction done");
            }
            return isSuccess;
        }

        /// <summary>
        /// Updates the work flow status.
        /// </summary>
        /// <param name="formFieldValues">The form field values.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>return dict</returns>
        private Dictionary<string, dynamic> UpdateWorkFlowStatus(Dictionary<string, dynamic> formFieldValues, Dictionary<string, string> param)
        {
            string workflowStatus = string.Empty;
            if (formFieldValues.ContainsKey("Status"))
            {
                string status = Convert.ToString(formFieldValues["Status"]);
                switch (status)
                {
                    case FormStatus.SUBMITTED:
                        workflowStatus = "Pending With " + (formFieldValues.ContainsKey("PendingWith") ? Convert.ToString(formFieldValues["PendingWith"]) : string.Empty);
                        ////if (param.ContainsKey(Parameter.APPLICATIONNAME) && param[Parameter.APPLICATIONNAME] == ApplicationNameConstants.QCAPP)
                        ////{
                        ////    workflowStatus = workflowStatus.Replace(",SuperAdmin", string.Empty);
                        ////}
                        break;
                    case FormStatus.SENTBACK:
                        workflowStatus = "Sent back by " + (formFieldValues.ContainsKey("LastActionByRole") ? Convert.ToString(formFieldValues["LastActionByRole"]) : string.Empty);
                        ////if (param.ContainsKey(Parameter.APPLICATIONNAME) && param[Parameter.APPLICATIONNAME] == ApplicationNameConstants.QCAPP)
                        ////{
                        ////    workflowStatus = workflowStatus.Replace(",SuperAdmin", string.Empty);
                        ////}
                        break;
                    default:
                        workflowStatus = status;
                        break;
                }
            }
            formFieldValues.Add("WorkflowStatus", workflowStatus);
            return formFieldValues;
        }

        /// <summary>
        /// Gets all task owners.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="approvers">The approvers.</param>
        /// <returns>all Task Owners</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Can't refactor")]
        private string GetAllTaskOwners(ITask task, string approvers)
        {
            dynamic actualTask = Convert.ChangeType(task, task.GetType());
            PropertyInfo[] properties = task.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                bool isTask = property.GetCustomAttribute<IsTaskAttribute>() != null && property.GetCustomAttribute<IsTaskAttribute>().IsTaskField;
                if (property.Name == "ActionBy" && !string.IsNullOrEmpty(property.GetValue(actualTask)) && approvers.IndexOf(Convert.ToString(property.GetValue(actualTask))) == -1)
                {
                    approvers = approvers.Trim(',') + "," + Convert.ToString(property.GetValue(actualTask));
                }
                else if (isTask && property.GetValue(actualTask) != null)
                {
                    ITask subTask = property.GetValue(actualTask) as ITask;
                    if (subTask != null)
                    {
                        approvers = this.GetAllTaskOwners(subTask, approvers);
                    }
                }
            }
            return approvers;
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="actionPerformed">The action performed.</param>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="currentUserID">The current user id.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="approversDataFromList">The approvers data from list.</param>
        /// <param name="listname">The listname.</param>
        /// <param name="nextLevel">The next level.</param>
        /// <param name="currLevel">The curr level.</param>
        /// <param name="paraml">The paraml.</param>
        /// <param name="mailCustomValues">The mail custom values.</param>
        /// <param name="emailAttachments">The email attachments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Can not able to change anything here."), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "could not able to reduce size.")]
        private void SendMail(ButtonActionStatus actionPerformed, ClientContext context, Web web, string currentUserID, int itemId, List<ApplicationStatus> approversDataFromList, string listname, int nextLevel, int currLevel, Dictionary<string, string> paraml, Dictionary<string, string> mailCustomValues = null, List<FileDetails> emailAttachments = null)
        {
            string from = string.Empty, to = string.Empty, cc = string.Empty, role = string.Empty, tmplName = string.Empty, strAllusers = string.Empty, nextApproverIds = string.Empty;
            EmailHelper eHelper = new EmailHelper();
            Dictionary<string, string> email = new Dictionary<string, string>();
            List<ListItemDetail> itemdetail = new List<ListItemDetail>();
            try
            {
                if (currLevel < 0)
                {
                    currLevel = 0;
                }
                itemdetail.Add(new ListItemDetail() { ItemId = itemId, IsMainList = true, ListName = listname });
                strAllusers = this.GetEmailUsers(approversDataFromList, nextLevel, currLevel);
                approversDataFromList.ForEach(p =>
                 {
                     if (Convert.ToInt32(p.Levels) == nextLevel && !string.IsNullOrEmpty(p.Approver))
                     {
                         nextApproverIds = nextApproverIds.Trim(',') + "," + p.Approver;
                     }
                 });
                nextApproverIds = nextApproverIds.Trim(',');
                if (mailCustomValues == null)
                {
                    mailCustomValues = new Dictionary<string, string>();
                }
                mailCustomValues[Parameter.CURRENTAPPROVERNAME] = GetNameUsingUserID(context, web, currentUserID);
                mailCustomValues[Parameter.NEXTAPPROVERNAME] = GetNameUsingUserID(context, web, nextApproverIds);
            }
            catch (Exception ex)
            {
                Logger.Info("############### Error in SendMail for following details : currlevel " + currLevel + "nextLevel " + nextLevel + " from : " + currentUserID + " To : " + to + " Action Performed :" + actionPerformed + " Nextapprovalemails :" + nextApproverIds + "Stacktrace:" + ex.StackTrace);
                Logger.Error(ex);
            }
            switch (actionPerformed)
            {
                case ButtonActionStatus.SaveAsDraft:

                    break;
                case ButtonActionStatus.SaveAndStatusUpdateWithEmail:
                case ButtonActionStatus.SaveAndNoStatusUpdateWithEmail:
                    if (!string.IsNullOrEmpty(strAllusers) && approversDataFromList != null && approversDataFromList.Count != 0)
                    {
                        from = currentUserID;
                        to = strAllusers.Trim(',');
                        role = approversDataFromList.FirstOrDefault(p => Convert.ToInt32(p.Levels) == currLevel).Role;
                        tmplName = EmailTemplateName.NEWREQUESTMAIL;
                        email = eHelper.GetEmailBody(context, web, EmailTemplateName.NEWREQUESTMAIL, itemdetail, mailCustomValues, role, paraml[Parameter.APPLICATIONNAME], paraml[Parameter.FROMNAME]);
                    }
                    break;
                case ButtonActionStatus.Save:
                    if (!string.IsNullOrEmpty(strAllusers) && approversDataFromList != null && approversDataFromList.Count != 0)
                    {
                        from = currentUserID;
                        to = strAllusers.Trim(',');
                        role = approversDataFromList.FirstOrDefault(p => Convert.ToInt32(p.Levels) == currLevel).Role;
                        tmplName = EmailTemplateName.NEWREQUESTMAIL;
                        email = eHelper.GetEmailBody(context, web, EmailTemplateName.NEWREQUESTMAIL, itemdetail, mailCustomValues, role, paraml[Parameter.APPLICATIONNAME], paraml[Parameter.FROMNAME]);
                    }
                    break;
                case ButtonActionStatus.Delegate:
                case ButtonActionStatus.NextApproval:
                    if (approversDataFromList != null && approversDataFromList.Count != 0)
                    {
                        from = currentUserID;
                        string allToUsers = string.Empty;
                        approversDataFromList.ForEach(p =>
                        {
                            if (Convert.ToInt32(p.Levels) == nextLevel && !string.IsNullOrEmpty(p.Approver) && p.Status == ApproverStatus.PENDING) ////Status also need to check
                            {
                                allToUsers = allToUsers.Trim(',') + "," + p.Approver;
                            }
                        });
                        to = allToUsers.Trim(',');
                        cc = approversDataFromList.Where(p => p.Role == UserRoles.CREATOR).First().Approver;
                        role = approversDataFromList.FirstOrDefault(p => Convert.ToInt32(p.Levels) == currLevel).Role;
                        tmplName = EmailTemplateName.APPROVALMAIL;
                        if (!approversDataFromList.Any(x => Convert.ToInt32(x.Levels) == nextLevel && !string.IsNullOrEmpty(x.Approver) && x.Status == ApproverStatus.APPROVED))
                        {
                            email = eHelper.GetEmailBody(context, web, EmailTemplateName.APPROVALMAIL, itemdetail, mailCustomValues, role, paraml[Parameter.APPLICATIONNAME], paraml[Parameter.FROMNAME]);
                        }
                        ////email = eHelper.GetEmailBody(context, web, EmailTemplateName.APPROVALMAIL, itemdetail, mailCustomValues, role, paraml[Parameter.APPLICATIONNAME], paraml[Parameter.FROMNAME]);
                    }
                    //// creato to all viewer
                    break;
                case ButtonActionStatus.SendBack:
                case ButtonActionStatus.BackToCreator:
                    if (approversDataFromList != null && approversDataFromList.Count != 0)
                    {
                        from = currentUserID;
                        string allToUsers = string.Empty;
                        approversDataFromList.ForEach(p =>
                        {
                            if (Convert.ToInt32(p.Levels) == nextLevel && !string.IsNullOrEmpty(p.Approver))
                            {
                                allToUsers = allToUsers.Trim(',') + "," + p.Approver;
                            }
                            if (Convert.ToInt32(p.Levels) == currLevel && !string.IsNullOrEmpty(p.Approver))
                            {
                                cc = cc.Trim(',') + "," + p.Approver;
                            }
                        });
                        to = allToUsers.Trim(',');
                        cc = (cc.Trim(',') + "," + approversDataFromList.FirstOrDefault(p => p.Role == UserRoles.CREATOR).Approver).Trim(',');
                        role = approversDataFromList.FirstOrDefault(p => Convert.ToInt32(p.Levels) == currLevel).Role;
                        tmplName = EmailTemplateName.SENDBACKMAIL;
                        email = eHelper.GetEmailBody(context, web, EmailTemplateName.SENDBACKMAIL, itemdetail, mailCustomValues, role, paraml[Parameter.APPLICATIONNAME], paraml[Parameter.FROMNAME]);
                    }
                    //// creato to all viewer
                    break;
                case ButtonActionStatus.Cancel:
                    if (!string.IsNullOrEmpty(strAllusers) && approversDataFromList != null && approversDataFromList.Count != 0)
                    {
                        from = currentUserID;
                        to = strAllusers.Trim(',');
                        role = approversDataFromList.FirstOrDefault(p => Convert.ToInt32(p.Levels) == nextLevel).Role;
                        tmplName = EmailTemplateName.REQUESTCANCELED;
                        email = eHelper.GetEmailBody(context, web, EmailTemplateName.REQUESTCANCELED, itemdetail, mailCustomValues, role, paraml[Parameter.APPLICATIONNAME], paraml[Parameter.FROMNAME]);
                    }
                    break;
                case ButtonActionStatus.Rejected:
                    if (!string.IsNullOrEmpty(strAllusers) && approversDataFromList != null && approversDataFromList.Count != 0)
                    {
                        from = currentUserID;
                        to = strAllusers.Trim(',');
                        role = approversDataFromList.FirstOrDefault(p => Convert.ToInt32(p.Levels) == nextLevel).Role;
                        tmplName = EmailTemplateName.REQUESTCANCELED;
                        email = eHelper.GetEmailBody(context, web, EmailTemplateName.REQUESTCANCELED, itemdetail, mailCustomValues, role, paraml[Parameter.APPLICATIONNAME], paraml[Parameter.FROMNAME]);
                    }
                    break;
                case ButtonActionStatus.Complete:
                    if (!string.IsNullOrEmpty(strAllusers) && approversDataFromList != null && approversDataFromList.Count != 0)
                    {
                        from = currentUserID;
                        to = approversDataFromList.FirstOrDefault(p => p.Role == UserRoles.CREATOR).Approver;
                        cc = strAllusers.Trim(',');
                        role = approversDataFromList.FirstOrDefault(p => Convert.ToInt32(p.Levels) == currLevel).Role;
                        tmplName = EmailTemplateName.REQUESTCLOSERMAIL;
                        email = eHelper.GetEmailBody(context, web, EmailTemplateName.REQUESTCLOSERMAIL, itemdetail, mailCustomValues, role, paraml[Parameter.APPLICATIONNAME], paraml[Parameter.FROMNAME]);
                    }
                    break;
                default:
                    break;
            }
            string activity = paraml != null && paraml.ContainsKey(Parameter.ACTIONPER) ? Convert.ToString(paraml[Parameter.ACTIONPER]).Split('|')[0] : string.Empty;
            if (!string.IsNullOrEmpty(activity))
            {
                switch (activity.Trim())
                {
                    case ButtonCaption.SendOOAP:
                    case ButtonCaption.Level1completed:
                    case ButtonCaption.Level2completed:
                    case ButtonCaption.MeetingConducted:
                        cc = strAllusers.Trim(',');
                        break;
                    default:
                        break;
                }
            }
            if (email != null && email.Count == 2)
            {
                from = GetEmailUsingUserID(context, web, from);
                to = GetEmailUsingUserID(context, web, to);
                cc = GetEmailUsingUserID(context, web, cc);
                eHelper.SendMail(paraml[Parameter.APPLICATIONNAME], paraml[Parameter.FROMNAME], tmplName, email["Subject"], email["Body"], from, to, cc, false, emailAttachments);
            }
        }

        /// <summary>
        /// Gets the email users.
        /// </summary>
        /// <param name="approversDataFromList">The approvers data from list.</param>
        /// <param name="nextLevel">The next level.</param>
        /// <param name="currLevel">The curr level.</param>
        /// <returns>get emails string</returns>
        public string GetEmailUsers(List<ApplicationStatus> approversDataFromList, int nextLevel, int currLevel)
        {
            string strUsersEmail = string.Empty;
            AppApprovalMatrixHelper approverHelper = new AppApprovalMatrixHelper();
            Dictionary<string, string> usersWithRole = approverHelper.GetPermissionDictionary(approversDataFromList, nextLevel, currLevel, true);

            foreach (var user in usersWithRole)
            {
                if (user.Value == SharePointPermission.READER || user.Value == SharePointPermission.CONTRIBUTOR)
                {
                    if (!string.IsNullOrEmpty(user.Key))
                    {
                        strUsersEmail = strUsersEmail.Trim(',') + user.Key + ",";
                    }
                }
            }
            ////strUsersEmail = usersWithRole.FirstOrDefault(x => x.Value == RoleType.Reader).Key + "," + usersWithRole.FirstOrDefault(x => x.Value == RoleType.Contributor).Key;
            return strUsersEmail.Trim(',');
        }

        /// <summary>
        /// Saves the form fields.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="formFieldValues">The form field values.</param>
        public void SaveFormFields(ClientContext context, Web web, string listName, int itemId, Dictionary<string, dynamic> formFieldValues)
        {
            if (context != null && web != null && !string.IsNullOrEmpty(listName) && itemId > 0 && formFieldValues != null && formFieldValues.Count > 0)
            {
                try
                {
                    Logger.Info("Helper.SaveFormFields in list '" + listName + "' id='" + itemId + "' with values = " + JsonConvert.SerializeObject(formFieldValues));
                    List mainList = web.Lists.GetByTitle(listName);
                    ListItem item = mainList.GetItemById(itemId);
                    FieldCollection listFields = mainList.Fields;
                    context.Load(listFields);
                    context.ExecuteQuery();
                    foreach (KeyValuePair<string, dynamic> fieldValue in formFieldValues)
                    {
                        try
                        {
                            Field listField = listFields.FirstOrDefault(p => p.InternalName == fieldValue.Key);
                            if (listField != null)
                            {
                                if (listField.TypeAsString.Equals("User"))
                                {
                                    if (fieldValue.Value.GetType().Name.Equals("FieldUserValue"))
                                    {
                                        item[fieldValue.Key] = fieldValue.Value;
                                    }
                                    else
                                    {
                                        item[fieldValue.Key] = fieldValue.Value[0];
                                    }
                                }
                                else if (listField.TypeAsString.Equals("UserMulti"))
                                {
                                    if (fieldValue.Value.GetType().Name.Equals("FieldUserValue"))
                                    {
                                        item[fieldValue.Key] = new FieldUserValue[] { fieldValue.Value };
                                    }
                                    else
                                    {
                                        item[fieldValue.Key] = fieldValue.Value;
                                    }
                                }
                                else
                                {
                                    item[fieldValue.Key] = fieldValue.Value;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(string.Format("Error While Save Form Fields {0} -  Message:{1}, StackTrace: {2}", fieldValue.Key, ex.Message, ex.StackTrace));
                        }
                    }
                    item.Update();
                    item.RefreshLoad(); // Jay Ashara - Added for Version Conflict!
                    context.Load(item);
                    context.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("Error While SaveFormFields  -  Message:{0}, StackTrace: {1}", ex.Message, ex.StackTrace));
                }
            }
        }

        /// <summary>
        /// Gets the multiple person field.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="emails">The emails.</param>
        /// <param name="property">The property.</param>
        /// <returns>FieldUserValue or FieldUserValue[] Object</returns>
        public dynamic GetMultiplePersonField(ClientContext context, Web web, string emails, PropertyInfo property)
        {
            FieldUserValue[] userValues = null;
            if (!string.IsNullOrEmpty(emails))
            {
                string[] arrEmails = emails.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                userValues = GetFieldUserValueFromPerson(context, web, arrEmails);
                if (!property.GetCustomAttribute<IsPersonAttribute>().IsMultiple)
                {
                    return userValues[0];
                }
            }
            return userValues;
        }
        #endregion

        #region User Role & Permission
        /// <summary>
        /// Gets the current user role.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="userID">The user email.</param>
        /// <param name="mainListName">Name of the main list.</param>
        /// <param name="approvalMatrixListName">Name of the approval matrix list.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="activeAndHiddenSections">The active and hidden sections.</param>
        /// <returns>Current user Role</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "8#", Justification = "needed to get different Data in single server call")]
        public string GetCurrentUserRole(ClientContext context, Web web, string applicationName, string formName, string userID, string mainListName, string approvalMatrixListName, int itemId, ref Dictionary<string, bool> activeAndHiddenSections)
        {
            string role = string.Empty;
            if (context != null && web != null && !string.IsNullOrEmpty(applicationName) && !string.IsNullOrEmpty(formName) && itemId > 0 && !string.IsNullOrEmpty(userID) && !string.IsNullOrEmpty(mainListName) && !string.IsNullOrEmpty(approvalMatrixListName))
            {
                /**
                 * Admin Changes
                 * if(User is Site Collection Admin)
                 *      No Need to check Permission get Current Level Role
                 * else
                 *      check Permission and get Role
                 */
                string permission = this.GetUserPermissionOnItem(context, web, mainListName, itemId, userID);
                switch (permission)
                {
                    case UserRoles.CONTRIBUTOR:
                        string formLevel = this.GetFormLevel(context, web, mainListName, itemId);
                        string currentLevel = formLevel.Split('|').Length > 1 ? formLevel.Split('|')[1] : "0";
                        role = this.GetCurrentUserRoleFromApprovalMatrix(context, web, approvalMatrixListName, itemId, currentLevel, userID, ref activeAndHiddenSections);
                        break;
                    case UserRoles.VIEWER:
                        role = UserRoles.VIEWER;
                        break;
                    default:
                        break;
                }
                /**
                 * Need get active & hidden Section for Viewers
                 */
            }
            return role;
        }

        /// <summary>
        /// Gets the user permission on item.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="mainListName">Name of the main list.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="userID">The user email.</param>
        /// <returns>User permission Weather Contributor or Viewr or Empty</returns>
        private string GetUserPermissionOnItem(ClientContext context, Web web, string mainListName, int itemId, string userID)
        {
            string userRole = string.Empty;
            User usr = EnsureUser(context, web, userID);
            try
            {
                List spList = web.Lists.GetByTitle(mainListName);
                ListItem item = spList.GetItemById(itemId);
                context.Load(item);
                RoleAssignmentCollection roles = item.RoleAssignments;
                context.Load(roles);
                context.ExecuteQuery();
                RoleAssignment role = roles.GetByPrincipal(usr);
                context.Load(role.RoleDefinitionBindings);
                context.ExecuteQuery();
                RoleDefinitionBindingCollection roleBindings = role.RoleDefinitionBindings;
                if (roleBindings != null)
                {
                    RoleDefinition viewerRole = roleBindings.Where(p => p.Name == SharePointPermission.READER).FirstOrDefault();
                    if (viewerRole != null)
                    {
                        userRole = UserRoles.VIEWER;
                    }
                    RoleDefinition contributorRole = roleBindings.Where(p => p.Name == SharePointPermission.CONTRIBUTOR).FirstOrDefault();
                    if (contributorRole != null)
                    {
                        userRole = UserRoles.CONTRIBUTOR;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("No Role Found for Item ID : " + itemId + " User ID : " + userID + "; If User Inside the group then also this error comes.");
                Logger.Error(ex);
                ////No Role Found for user
                userRole = string.Empty;
            }
            return userRole;
        }

        /// <summary>
        /// Sets the item permission.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="usersWithPermission">The users with permission.</param>
        /// <returns>
        /// true or false
        /// </returns>
        public bool SetItemPermission(ClientContext context, Web web, int itemId, string listName, Dictionary<string, string> usersWithPermission)
        {
            bool permissionAssigned = false;
            if (context != null && web != null && usersWithPermission != null && itemId > 0 && !string.IsNullOrEmpty(listName))
            {
                List spList = web.Lists.GetByTitle(listName);
                ListItem item = spList.GetItemById(itemId);
                context.Load(item);
                context.ExecuteQuery();
                permissionAssigned = this.SetItemPermission(context, web, ref item, usersWithPermission);
                Logger.Info("Called SetItemPermission Permission Assigned ->" + permissionAssigned);
            }
            return permissionAssigned;
        }

        /// <summary>
        /// Sets the item permission.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="item">The item.</param>
        /// <param name="usersWithPermission">The users with permission.</param>
        /// <returns>
        /// true or false
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Pass by reference is needed"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need to pass item object by reference as it not stored in list")]
        public bool SetItemPermission(ClientContext context, Web web, ref ListItem item, Dictionary<string, string> usersWithPermission)
        {
            bool permissionAssigned = false;
            if (context != null && web != null && usersWithPermission != null && item != null)
            {
                Logger.Info("Condition PAssed (context != null && web != null && usersWithPermission != null && item != null)");
                item.ResetRoleInheritance();
                Logger.Info("item ResetRoleInheritance Called");
                item.BreakRoleInheritance(false, true);
                Logger.Info("item BreakRoleInheritance(false,true); Called");
                RoleDefinitionBindingCollection collRoleDefinitionBindingAssignee = null;
                foreach (KeyValuePair<string, string> userWithPermission in usersWithPermission)
                {
                    try
                    {
                        collRoleDefinitionBindingAssignee = new RoleDefinitionBindingCollection(context);
                        string userIDs = userWithPermission.Key;
                        Logger.Info("Get User Emails from RoleDefinitionBindingCollection -> " + userIDs);
                        if (!string.IsNullOrEmpty(userIDs))
                        {
                            char[] charSeparators = new char[] { ',' };
                            string[] users = userIDs.Trim(',').Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                            User[] objUser = EnsureUser(context, web, users);
                            Logger.Info("Adding collRoleDefinitionBindingAssignee -> web.RoleDefinitions.GetByName(" + userWithPermission.Value + ")");
                            collRoleDefinitionBindingAssignee.Add(web.RoleDefinitions.GetByName(userWithPermission.Value)); //Set permission type
                            for (int i = 0; i < objUser.Length; i++)
                            {
                                item.RoleAssignments.Add(objUser[i], collRoleDefinitionBindingAssignee);
                            }
                            context.ExecuteQuery();

                            Logger.Info("User Permission set for -> " + userIDs);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(string.Format("Error While Set Item Permission  - Item ID:{0}, Message:{1}, StackTrace: {2}", item.Id, ex.Message, ex.StackTrace));
                    }
                }

                try
                {
                    collRoleDefinitionBindingAssignee = new RoleDefinitionBindingCollection(context);
                    collRoleDefinitionBindingAssignee.Add(web.RoleDefinitions.GetByName(SharePointPermission.CONTRIBUTOR)); //Set permission type
                    Group adminGroup = web.SiteGroups.GetByName(UserRoles.ADMINISTRATOR);
                    context.Load(adminGroup);
                    item.RoleAssignments.Add(adminGroup, collRoleDefinitionBindingAssignee);
                    context.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    Logger.Error("Error while granting Permission to " + UserRoles.ADMINISTRATOR + " Group");
                    Logger.Error(ex);
                }
                ////try
                ////{
                ////    collRoleDefinitionBindingAssignee = new RoleDefinitionBindingCollection(context);
                ////    collRoleDefinitionBindingAssignee.Add(web.RoleDefinitions.GetByName(SharePointPermission.CONTRIBUTOR)); //Set permission type
                ////    Group divadminGroup = web.SiteGroups.GetByName(UserRoles.DIVISIONADMINISTRATOR);
                ////    context.Load(divadminGroup);
                ////    item.RoleAssignments.Add(divadminGroup, collRoleDefinitionBindingAssignee);
                ////    context.ExecuteQuery();
                ////}
                ////catch (Exception ex)
                ////{
                ////    Logger.Error("Error while granting Permission to " + UserRoles.DIVISIONADMINISTRATOR + " Group");
                ////    Logger.Error(ex);
                ////}
                try
                {
                    collRoleDefinitionBindingAssignee = new RoleDefinitionBindingCollection(context);
                    collRoleDefinitionBindingAssignee.Add(web.RoleDefinitions.GetByName(SharePointPermission.READER)); //Set permission type
                    Group adminGroup = web.SiteGroups.GetByName(UserRoles.VIEWER);
                    context.Load(adminGroup);
                    item.RoleAssignments.Add(adminGroup, collRoleDefinitionBindingAssignee);
                    context.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    Logger.Error("Error while granting Permission to " + UserRoles.VIEWER + " Group");
                    Logger.Error(ex);
                }

                ////assign sharepoint group "CPViewer" and "LUMViewer" according to business unit
                string groupName = string.Empty;
                try
                {
                    collRoleDefinitionBindingAssignee = new RoleDefinitionBindingCollection(context);
                    collRoleDefinitionBindingAssignee.Add(web.RoleDefinitions.GetByName(SharePointPermission.READER)); //Set permission type
                    groupName = !string.IsNullOrEmpty(Convert.ToString(item[Constants.DCRNO])) ? (Convert.ToString(item[Constants.DCRNO]).Contains(Constants.CP) ? Constants.CP + UserRoles.VIEWER : Convert.ToString(item[Constants.DCRNO]).Contains(Constants.LUM) ? Constants.LUM + UserRoles.VIEWER : string.Empty) : string.Empty;
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        Group buGroup = web.SiteGroups.GetByName(groupName);
                        context.Load(buGroup);
                        item.RoleAssignments.Add(buGroup, collRoleDefinitionBindingAssignee);
                        context.ExecuteQuery();
                    }
                    else
                    {
                        Logger.Info("Group Name is null");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Group Name ==" + groupName);
                    Logger.Error("Error while granting Permission to " + groupName + " Group");
                    Logger.Error(ex);
                }
            }
            return permissionAssigned;
        }
        #endregion

        #region Data Retrieve Related Methods

        /// <summary>
        /// Gets the form data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="userID">The user email.</param>
        /// <param name="form">The form.</param>
        /// <returns>
        /// Return IForm
        /// </returns>
        public IForm GetFormData(ClientContext context, Web web, string applicationName, string formName, int itemId, string userID, IForm form)
        {
            Dictionary<string, bool> activeAndHiddenSections = new Dictionary<string, bool>();
            bool isSuperAdmin = false;
            bool isDivisionAdmin = false;
            bool isForceCloseRequired = false;
            Logger.Info("Called Helper GetFormData");
            if (context != null && web != null && !string.IsNullOrEmpty(applicationName) && !string.IsNullOrEmpty(formName) && !string.IsNullOrEmpty(userID) && form != null)
            {
                Logger.Info("Helper GetFormData Condition passed for appname=" + applicationName + ", formName" + formName + ", User Id=" + userID);
                string role = string.Empty;
                if (itemId > 0)
                {
                    role = this.GetCurrentUserRole(context, web, applicationName, formName, userID, form.MainListName, form.ApprovalMatrixListName, itemId, ref activeAndHiddenSections);
                    if (string.IsNullOrEmpty(role))
                    {
                        /* Super Admin & Division Admin Case*/
                        isSuperAdmin = this.IsGroupMember(context, web, userID, UserRoles.ADMIN);
                        ////isDivisionAdmin = this.IsGroupMember(context, web, userID, UserRoles.DIVISIONADMINISTRATOR);
                        if (isSuperAdmin)
                        {
                            role = "DCR Admin";
                        }
                        else if (this.IsGroupMember(context, web, userID, UserRoles.VIEWER) || this.IsGroupMember(context, web, userID, UserRoles.CPVIEWER) || this.IsGroupMember(context, web, userID, UserRoles.LUMVIEWER))
                        {
                            role = UserRoles.VIEWER;
                        }
                    }
                    Logger.Info("Called Helper.GetCurrentUserRole and Role=" + role);
                }
                else
                {
                    Logger.Info("Called Helper.IsGroupMember true");
                    activeAndHiddenSections = this.GetEnableSectionNames(applicationName, formName, UserRoles.CREATOR);
                    role = UserRoles.CREATOR;
                    Logger.Info("Called Helper.GetEnableSectionNames " + JsonConvert.SerializeObject(activeAndHiddenSections));
                }
                if (!string.IsNullOrEmpty(role))
                {
                    form = this.GetFormData(context, web, applicationName, formName, itemId, userID, form, role, activeAndHiddenSections, new Dictionary<string, string> { { Parameter.FORMTYPE, FormType.MAIN } });
                    if (form.SectionsList.Any(p => p.IsActive == true))
                    {
                        form.SectionsList.FirstOrDefault(p => p.IsActive == true).FormBelongTo = userID;
                    }
                    if (isForceCloseRequired && form.Buttons.Count != 0 && role != UserRoles.VIEWER)
                    {
                        form.Buttons.Add(new Button() { ButtonStatus = ButtonActionStatus.Complete, IsVisible = true, JsFunction = "ConfirmSubmit", FormType = "Main", Icon = "fa fa-save", Name = "Force Close/Complete" });
                    }
                }
                else
                {
                    form = null;
                }
            }
            return form;
        }

        /// <summary>
        /// Requests the belongs to division.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="mainListName">Name of the main list.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="userIDs">The user email.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <returns>
        /// true or false
        /// </returns>
        private bool RequestBelongsToDivision(ClientContext context, Web web, string mainListName, int itemId, string userIDs, string applicationName, string formName)
        {
            List<DivisionAdminDetails> divAdminDetails = this.GetDivisionAdminData(applicationName, formName, userIDs);
            bool isBelongsToDivision = false;
            if (divAdminDetails != null && divAdminDetails.Count > 0)
            {
                string divisionFieldName = divAdminDetails.FirstOrDefault().DivisionFieldName;
                List spList = web.Lists.GetByTitle(mainListName);
                ListItem item = spList.GetItemById(itemId);
                context.Load(item);
                context.ExecuteQuery();
                if (item != null)
                {
                    string divisionName = Convert.ToString(item[divisionFieldName]);
                    isBelongsToDivision = divAdminDetails.Any(p => !string.IsNullOrEmpty(p.DivisionCodes) && p.DivisionCodes.ToLower().Contains(divisionName.ToLower()));
                }
            }
            return isBelongsToDivision;
        }

        /// <summary>
        /// Gets the division admin data.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="userIDs">The user email.</param>
        /// <returns>Filtered Division Master</returns>
        private List<DivisionAdminDetails> GetDivisionAdminData(string applicationName, string formName, string userIDs)
        {
            List<DivisionAdminDetails> divisionAdminDetails = new List<DivisionAdminDetails>();
            if (!string.IsNullOrEmpty(applicationName) && !string.IsNullOrEmpty(formName) && !string.IsNullOrEmpty(userIDs))
            {
                divisionAdminDetails = GlobalCachingProvider.Instance.GetItem(ListNames.DivisionAdminMasterList, false) as List<DivisionAdminDetails>;
                if (divisionAdminDetails == null)
                {
                    divisionAdminDetails = this.GetFullDivisionMaster();
                    GlobalCachingProvider.Instance.AddItem(ListNames.DivisionAdminMasterList, divisionAdminDetails);
                }
                Logger.Info("Helper.GetDivisionAdminData for appName='" + applicationName + "', formName='" + formName + "', userEmail='" + userIDs + "'");
                divisionAdminDetails = (from b in divisionAdminDetails
                                        where
                                        !string.IsNullOrEmpty(b.ApplicationName) &&
                                        Convert.ToString(b.ApplicationName).ToLower() == applicationName.ToLower() &&
                                        !string.IsNullOrEmpty(b.FormName) &&
                                        Convert.ToString(b.FormName).ToLower() == formName.ToLower() &&
                                        !string.IsNullOrEmpty(b.DivisionAdmins) &&
                                        Convert.ToString(b.DivisionAdmins).ToLower().Contains(userIDs.ToLower())
                                        select b).ToList();
            }
            return divisionAdminDetails;
        }

        /// <summary>
        /// Gets the full division master.
        /// </summary>
        /// <returns>all Division master</returns>
        private List<DivisionAdminDetails> GetFullDivisionMaster()
        {
            List<DivisionAdminDetails> divisionAdminDetails = new List<DivisionAdminDetails>();
            Logger.Info("Helper.GetFullDivisionMaster");
            string siteURL = this.GetSiteURL(SiteURLs.ROOTSITEURL); //this.GetConfigVariable(SiteURLs.ROOTSITEURL);
            ClientContext context = this.CreateClientContext(siteURL);
            Web web = this.CreateWeb(context);
            context.Load(web);
            context.ExecuteQuery();
            List spList = web.Lists.GetByTitle(ListNames.DivisionAdminMasterList);
            AppApprovalMatrixHelper apphelper = new AppApprovalMatrixHelper();
            ListItemCollection items = spList.GetItems(CamlQuery.CreateAllItemsQuery());
            context.Load(items);
            context.ExecuteQuery();
            foreach (ListItem item in items)
            {
                divisionAdminDetails.Add(
                    new DivisionAdminDetails()
                    {
                        DivisionCodes = Convert.ToString(item["Title"]),
                        ApplicationName = apphelper.GetTaxonomyFieldValue(item["ApplicationName"] as TaxonomyFieldValue),
                        FormName = apphelper.GetTaxonomyFieldValue(item["FormName"] as TaxonomyFieldValue),
                        DivisionFieldName = Convert.ToString(item["DivisionFieldName"]),
                        DivisionAdmins = GetEmailsFromPersonField(context, web, item["DivisionAdmins"] as FieldUserValue[])
                    });
            }
            return divisionAdminDetails;
        }

        /// <summary>
        /// Gets the current user role from approval matrix.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="approvalMatrixListName">Name of the approval matrix list.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="userID">The user email.</param>
        /// <param name="activeAndHiddenSections">The active and hidden sections.</param>
        /// <returns>User Role</returns>
        private string GetCurrentUserRoleFromApprovalMatrix(ClientContext context, Web web, string approvalMatrixListName, int itemId, string currentLevel, string userID, ref Dictionary<string, bool> activeAndHiddenSections)
        {
            string role = string.Empty;
            if (context != null && web != null && !string.IsNullOrEmpty(approvalMatrixListName) && itemId > 0)
            {
                ////*// User user = EnsureUser(context, web, userID);

                List approverList = web.Lists.GetByTitle(approvalMatrixListName);
                CamlQuery query = new CamlQuery();
                /**
                 * Admin Changes
                 * if(user.IsSiteAdmin)
                 * Remove Approver Check
                 */
                query.ViewXml = @"<View>
                                    <Query>
                                        <Where>
                                            <And>
                                                <Eq>
                                                    <FieldRef Name='RequestID' />
                                                    <Value Type='Lookup'>" + itemId + @"</Value>
                                                </Eq>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='Levels' />
                                                        <Value Type='Text'>" + currentLevel + @"</Value>
                                                    </Eq>
                                                    <Contains>
                                                        <FieldRef Name='Approver' LookupId='True' />
                                                        <Value Type='Lookup'>" + userID + @"</Value>
                                                    </Contains>
                                                </And>
                                            </And>
                                        </Where>
                                    </Query>
                                    </View>";  //*//user.Title
                ListItemCollection items = approverList.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();
                ListItem item = items.FirstOrDefault();
                if (item != null)
                {
                    role = Convert.ToString(item["Role"]);
                    if (Convert.ToString(item["SectionName"]).Length > 0)
                    {
                        activeAndHiddenSections = Convert.ToString(item["SectionName"]).Trim(',').Split(',').ToDictionary(v => v, v => true);
                    }
                    if (Convert.ToString(item["HiddenSection"]).Length > 0)
                    {
                        activeAndHiddenSections = activeAndHiddenSections.Concat(Convert.ToString(item["HiddenSection"]).Trim(',').Split(',').ToDictionary(v => v, v => false)).ToDictionary(x => x.Key, x => x.Value);
                    }
                }
            }
            return role;
        }

        /// <summary>
        /// Gets the pending user email and role from approval matrix.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="approvalMatrixListName">Name of the approval matrix list.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="userID">The user email.</param>
        /// <param name="activeAndHiddenSections">The active and hidden sections.</param>
        /// <returns>Role and UserEmail</returns>
        private string GetPendingUserEmailAndRoleFromApprovalMatrix(ClientContext context, Web web, string approvalMatrixListName, int itemId, string currentLevel, string userID, ref Dictionary<string, bool> activeAndHiddenSections)
        {
            string role = string.Empty;
            if (context != null && web != null && !string.IsNullOrEmpty(approvalMatrixListName) && itemId > 0)
            {
                List approverList = web.Lists.GetByTitle(approvalMatrixListName);
                CamlQuery query = new CamlQuery();
                query.ViewXml = @"<View>
                                    <Query>
                                        <Where>
                                            <And>
                                                <Eq>
                                                    <FieldRef Name='Status' />
                                                    <Value Type='Text'>" + ApproverStatus.PENDING + @"</Value>
                                                </Eq>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='RequestID' />
                                                        <Value Type='Lookup'>" + itemId + @"</Value>
                                                    </Eq>
                                                    <And>
                                                        <Eq>
                                                            <FieldRef Name='Levels' />
                                                            <Value Type='Text'>" + currentLevel + @"</Value>
                                                        </Eq>
                                                        <IsNotNull>
                                                            <FieldRef Name='Approver' />
                                                        </IsNotNull>
                                                    </And>
                                                </And>
                                            </And>
                                        </Where>
                                    </Query>
                                    </View>";
                ListItemCollection items = approverList.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();
                ListItem item = items.FirstOrDefault();
                if (item != null)
                {
                    role = Convert.ToString(item["Role"]) + "|" + GetEmailsFromPersonField(context, web, item["Approver"] as FieldUserValue[]);
                    if (Convert.ToString(item["SectionName"]).Length > 0)
                    {
                        activeAndHiddenSections = Convert.ToString(item["SectionName"]).Trim(',').Split(',').ToDictionary(v => v, v => true);
                    }
                    if (Convert.ToString(item["HiddenSection"]).Length > 0)
                    {
                        activeAndHiddenSections = activeAndHiddenSections.Concat(Convert.ToString(item["HiddenSection"]).Trim(',').Split(',').ToDictionary(v => v, v => false)).ToDictionary(x => x.Key, x => x.Value);
                    }
                }
            }
            return role;
        }

        /// <summary>
        /// Gets the form level.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="mainListName">Name of the main list.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <returns>Form Level</returns>
        private string GetFormLevel(ClientContext context, Web web, string mainListName, int itemId)
        {
            string formLevel = string.Empty;
            List spList = web.Lists.GetByTitle(mainListName);
            ListItem item = spList.GetItemById(itemId);
            context.Load(item);
            context.ExecuteQuery();
            if (item != null)
            {
                formLevel = Convert.ToString(item["FormLevel"]);
            }
            return formLevel;
        }

        /// <summary>
        /// Gets the form data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="userID">The user email.</param>
        /// <param name="form">The form.</param>
        /// <param name="role">The role.</param>
        /// <param name="activeAndHiddenSections">The active and hidden sections.</param>
        /// <param name="otherParamDictionary">The other parameter dictionary.</param>
        /// <returns>
        /// Form Object
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Need to check fields")]
        public IForm GetFormData(ClientContext context, Web web, string applicationName, string formName, int itemId, string userID, IForm form, string role, Dictionary<string, bool> activeAndHiddenSections, Dictionary<string, string> otherParamDictionary)
        {
            AppApprovalMatrixHelper approverHelper = new AppApprovalMatrixHelper();
            Dictionary<string, string> otherParam = new Dictionary<string, string>();

            ListItem tempitem = null;
            List<ApplicationStatus> approverMatrixFromList = null;
            if (context != null && web != null && !string.IsNullOrEmpty(applicationName) && !string.IsNullOrEmpty(formName) && !string.IsNullOrEmpty(userID) && form != null && !string.IsNullOrEmpty(role) && activeAndHiddenSections != null)
            {
                if (itemId > 0)
                {
                    approverMatrixFromList = approverHelper.GetAppApprovalMatrix(context, web, itemId, form.ApprovalMatrixListName, true);
                }
                else
                {
                    approverMatrixFromList = approverHelper.GetGlobalAppApproverMatrix(applicationName, formName);
                }

                Logger.Info("called Helper.GetFormData");
                string formStatus = string.Empty;
                List<ISection> sections = new List<ISection>();
                for (int i = 0; i < form.SectionsList.Count; i++)
                {
                    bool isHidden = false, isActive = false;
                    string currentSectionName = form.SectionsList[i].SectionName;
                    if (!string.IsNullOrEmpty(currentSectionName) && activeAndHiddenSections.ContainsKey(currentSectionName))
                    {
                        if (activeAndHiddenSections[currentSectionName])
                        {
                            isActive = true;
                        }
                        else
                        {
                            isHidden = true;
                        }
                    }
                    if (itemId > 0 && approverMatrixFromList.Any(p => p.SectionName == currentSectionName && (string.IsNullOrEmpty(p.Approver) || p.Status == ApproverStatus.NOTREQUIRED)))
                    {
                        isHidden = true;
                    }
                    if (!isHidden)
                    {
                        Type sectionType = form.SectionsList[i].GetType();
                        if (sectionType == typeof(ApplicationStatusSection))
                        {
                            ////Get Local App Approval Matrix.
                            ApplicationStatusSection appStatusSection = new ApplicationStatusSection(true) { IsActive = isActive };
                            if (!string.IsNullOrEmpty(form.ApprovalMatrixListName) && itemId > 0)
                            {
                                appStatusSection.ApplicationStatusList = approverMatrixFromList;
                                appStatusSection.ApplicationStatusList.RemoveAll(p => Convert.ToInt32(p.Levels) < 0);
                                appStatusSection.ApplicationStatusList = appStatusSection.ApplicationStatusList.OrderBy(p => Convert.ToInt32(p.Levels)).ToList();
                            }
                            form.SectionsList[i] = appStatusSection;
                        }
                        else if (sectionType == typeof(ActivityLogSection))
                        {
                            form.SectionsList[i] = this.GetActivityLog(context, web, form.SectionsList[i].ListDetails[0].ListName, itemId, isActive);
                        }
                        else
                        {
                            form.SectionsList[i].ListDetails[0].ItemId = itemId;
                            if (tempitem != null)
                            {
                                form.SectionsList[i].ListDetails[0].ListItemObject = tempitem;
                            }
                            form.SectionsList[i] = this.GetData(context, web, applicationName, formName, role, (ISection)form.SectionsList[i], isActive, userID, ref approverMatrixFromList, otherParamDictionary);
                            PropertyInfo statusField = form.SectionsList[i].GetType().GetProperty("Status");
                            if (statusField != null)
                            {
                                formStatus = Convert.ToString(statusField.GetValue(form.SectionsList[i]));
                            }
                            if (form.SectionsList[i].ListDetails[0].ListItemObject != null)
                            {
                                tempitem = form.SectionsList[i].ListDetails[0].ListItemObject;
                                if (form.SectionsList[i].ListDetails[0].ListItemObject.FieldValues.ContainsKey("FormLevel"))
                                {
                                    if (!otherParam.ContainsKey(Parameter.CURRENTFROMLEVEL))
                                    {
                                        string fromlevel = form.SectionsList[i].ListDetails[0].ListItemObject["FormLevel"].ToString();
                                        otherParam.Add(Parameter.CURRENTFROMLEVEL, !string.IsNullOrEmpty(fromlevel) ? fromlevel.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[1].ToString().Trim() : string.Empty);
                                    }
                                }
                                if (form.SectionsList[i].ListDetails[0].ListItemObject.FieldValues.ContainsKey("Status") && form.SectionsList[i].ListDetails[0].ListItemObject.FieldValues.ContainsKey("NextApprover"))
                                {
                                    string approvers = string.Empty;
                                    if (form.SectionsList[i].ListDetails[0].ListItemObject["NextApprover"] != null)
                                    {
                                        approvers = GetNameFromPersonField(context, web, form.SectionsList[i].ListDetails[0].ListItemObject["NextApprover"] as FieldUserValue[]);
                                    }
                                    if ((Convert.ToString(form.SectionsList[i].ListDetails[0].ListItemObject["Status"]) == FormStatus.SUBMITTED || Convert.ToString(form.SectionsList[i].ListDetails[0].ListItemObject["Status"]) == FormStatus.SENTBACK) && !string.IsNullOrEmpty(approvers))
                                    {
                                        form.FormStatus = "Pending With " + approvers;
                                    }
                                    else if (Convert.ToString(form.SectionsList[i].ListDetails[0].ListItemObject["Status"]) == FormStatus.COMPLETED)
                                    {
                                        form.FormStatus = "Completed";
                                    }
                                }
                                form.SectionsList[i].ListDetails[0].ListItemObject = null;
                            }
                        }
                        form.SectionsList[i].FormBelongTo = role;
                        sections.Add(form.SectionsList[i]);
                    }
                }
                if (sections.Count > 0)
                {
                    form.SectionsList = sections;
                }
                string formType = FormType.MAIN;
                if (otherParamDictionary != null && otherParamDictionary.ContainsKey(Parameter.FORMTYPE))
                {
                    formType = otherParamDictionary[Parameter.FORMTYPE];
                }
                form.Buttons = this.GetButtons(applicationName, formName, role, formStatus, otherParam, formType);
            }
            return form;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="role">The role.</param>
        /// <param name="section">The section.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="userID">The user email.</param>
        /// <param name="approverMatrixFromList">Approval Matrix Object</param>
        /// <param name="otherParamDictionary">The other parameter dictionary.</param>
        /// <returns>
        /// Section Data
        /// </returns>
        public ISection GetData(ClientContext context, Web web, string applicationName, string formName, string role, ISection section, bool isActive, string userID, ref List<ApplicationStatus> approverMatrixFromList, Dictionary<string, string> otherParamDictionary = null)
        {
            dynamic actualSection = null;
            if (context != null && web != null && section != null)
            {
                actualSection = Convert.ChangeType(section, section.GetType());
                actualSection.IsActive = isActive;
                int listCount = actualSection.ListDetails.Count;
                if (listCount > 0)
                {
                    actualSection = this.FillSectionFromList(context, web, applicationName, formName, role, actualSection, userID, otherParamDictionary, ref approverMatrixFromList);
                }
            }
            return actualSection;
        }

        /// <summary>
        /// Get Data from Sections.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="section">The section.</param>
        /// <param name="userID">The user email.</param>
        /// <param name="otherParamDictionary">The other parameter dictionary.</param>
        /// <returns>
        /// the I Section
        /// </returns>
        public List<ISection> GetItemsForSection(ClientContext context, Web web, string applicationName, string formName, ISection section, string userID, Dictionary<string, string> otherParamDictionary = null)
        {
            dynamic actualSection = null;
            if (context != null && web != null && section != null)
            {
                actualSection = Convert.ChangeType(section, section.GetType());
                int listCount = actualSection.ListDetails.Count;
                if (listCount > 0)
                {
                    actualSection = this.FillSectionsFromList(context, web, applicationName, formName, actualSection, userID, otherParamDictionary);
                }
            }
            return actualSection;
        }

        /// <summary>
        /// Fills the section from list.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="role">The role.</param>
        /// <param name="actualSection">The actual section.</param>
        /// <param name="userID">The user email.</param>
        /// <param name="otherParamDictionary">The other parameter dictionary.</param>
        /// <param name="approverMatrixFromList">Approval Matrix Object.</param>
        /// <returns>
        /// Section With Data
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Couldn't able to divide its complex"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Couldn't able to divide its complex")]
        private ISection FillSectionFromList(ClientContext context, Web web, string applicationName, string formName, string role, dynamic actualSection, string userID, Dictionary<string, string> otherParamDictionary, ref List<ApplicationStatus> approverMatrixFromList)
        {
            //List<ApplicationStatus> approverMatrixFromList = null;
            TransactionHelper tranHelper = new TransactionHelper();
            MasterDataHelper masterHelper = new MasterDataHelper();

            int i = 0;
            string sectionOwner = role;
            foreach (ListItemDetail listDetail in actualSection.ListDetails)
            {
                List list = web.Lists.GetByTitle(listDetail.ListName);
                ListItem item = null;
                PropertyInfo[] properties = actualSection.GetType().GetProperties();
                if (listDetail.ItemId > 0)
                {
                    if (actualSection.ListDetails[i].ListItemObject != null)
                    {
                        item = actualSection.ListDetails[i].ListItemObject;
                    }
                    else
                    {
                        item = list.GetItemById(listDetail.ItemId);
                        context.Load(item);
                        context.Load(context.Site);
                        context.ExecuteQuery();
                    }
                    AttachmentCollection attachments = null;
                    if (Convert.ToString(item["Attachments"]) == "True")
                    {
                        context.Load(item.AttachmentFiles);
                        context.ExecuteQuery();
                        attachments = item.AttachmentFiles; //attachmentsFolder.Files;
                    }

                    actualSection.ListDetails[i].ListItemObject = item;
                    foreach (PropertyInfo property in properties)
                    {
                        bool isListColumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                        bool isCurrentListCoumn = property.GetCustomAttribute<FieldBelongToListAttribute>() == null || property.GetCustomAttribute<FieldBelongToListAttribute>().ListNames.Contains(listDetail.ListName);

                        if (isListColumn && isCurrentListCoumn)
                        {
                            string propertyName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                            bool islookup = property.GetCustomAttribute<FieldColumnNameAttribute>() != null ? property.GetCustomAttribute<FieldColumnNameAttribute>().IsLookup : false;
                            if (property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile)
                            {
                                if (attachments != null)
                                {
                                    List<FileDetails> objAttachmentFiles = this.GetAttachments(attachments);
                                    property.SetValue(actualSection, objAttachmentFiles);
                                }
                            }
                            else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson && !property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                            {
                                FieldUserValue[] users = null;
                                if (item[propertyName] != null)
                                {
                                    if (property.GetCustomAttribute<IsPersonAttribute>().IsMultiple)
                                    {
                                        users = item[propertyName] as FieldUserValue[];
                                    }
                                    else
                                    {
                                        users = new FieldUserValue[1];
                                        users[0] = item[propertyName] as FieldUserValue;
                                    }
                                }
                                if (users != null)
                                {
                                    string personEmails = string.Empty;

                                    if (otherParamDictionary != null && otherParamDictionary.ContainsKey(Parameter.EMAILFIELDTYPE) && Convert.ToString(otherParamDictionary[Parameter.EMAILFIELDTYPE]).ToLower().Equals("name"))
                                    {
                                        personEmails = GetNameFromPersonField(context, web, users);
                                    }
                                    else
                                    {
                                        personEmails = GetEmailsFromPersonField(context, web, users);
                                    }
                                    property.SetValue(actualSection, personEmails);
                                }
                            }
                            else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson && property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                            {
                                FieldUserValue[] users = null;
                                if (item[propertyName] != null)
                                {
                                    if (property.GetCustomAttribute<IsPersonAttribute>().IsMultiple)
                                    {
                                        users = item[propertyName] as FieldUserValue[];
                                    }
                                    else
                                    {
                                        users = new FieldUserValue[1];
                                        users[0] = item[propertyName] as FieldUserValue;
                                    }
                                }
                                if (users != null)
                                {
                                    string personEmails = string.Empty;
                                    personEmails = GetNameFromPersonField(context, web, users);
                                    property.SetValue(actualSection, personEmails);
                                }
                            }
                            else if (islookup)
                            {
                                FieldLookupValue lookupField = item[propertyName] as FieldLookupValue;
                                property.SetValue(actualSection, lookupField.LookupId);
                            }
                            else
                            {
                                object value = item[propertyName];
                                Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                                if (t.Name.ToUpper().Trim() == Constants.DateTimeString.ToUpper().Trim() && safeValue != null)
                                {
                                    var timeInfo = TimeZoneInfo.FindSystemTimeZoneById(Constants.TimeZoneName);
                                    safeValue = TimeZoneInfo.ConvertTimeFromUtc((DateTime)safeValue, timeInfo);
                                }
                                property.SetValue(actualSection, safeValue);
                            }
                        }
                        else if (property.GetCustomAttribute<ContainsMasterDataAttribute>() != null && property.GetCustomAttribute<ContainsMasterDataAttribute>().ContainsMasterData)
                        {
                            List<IMaster> masters = property.GetValue(actualSection);
                            masters = masterHelper.GetMasterData(context, web, masters);
                            property.SetValue(actualSection, masters);
                        }
                        else if (property.GetCustomAttribute<IsTranAttribute>() != null && property.GetCustomAttribute<IsTranAttribute>().IsTranField)
                        {
                            if (listDetail.IsMainList)
                            {
                                string listName = property.GetCustomAttribute<IsTranAttribute>().TranListName;
                                Type tranType = property.GetCustomAttribute<IsTranAttribute>().TranType;
                                if (!string.IsNullOrEmpty(listName) && tranType != null)
                                {
                                    List<ITrans> trans = tranHelper.GetTransactionListData(context, web, tranType, listName, listDetail.ItemId);
                                    property.SetValue(actualSection, trans);
                                }
                            }
                        }
                        else if (property.GetCustomAttribute<IsApproverMatrixFieldAttribute>() != null && property.GetCustomAttribute<IsApproverMatrixFieldAttribute>().IsApproverMatrixField)
                        {
                            AppApprovalMatrixHelper approverHelper = new AppApprovalMatrixHelper();
                            string approverMatrixListName = property.GetCustomAttribute<IsApproverMatrixFieldAttribute>().ApproverMatrixListName;
                            if (approverMatrixFromList == null)
                            {
                                approverMatrixFromList = approverHelper.GetAppApprovalMatrix(context, web, listDetail.ItemId, approverMatrixListName);
                            }
                            sectionOwner = role;
                            if (approverMatrixFromList.Any(p => !string.IsNullOrEmpty(p.SectionName) && p.SectionName.Contains(actualSection.SectionName)))
                            {
                                sectionOwner = approverMatrixFromList.FirstOrDefault(p => !string.IsNullOrEmpty(p.SectionName) && p.SectionName.Contains(actualSection.SectionName)).Role;
                            }
                            List<ApplicationStatus> fillByMatrix = approverMatrixFromList.Where(p => !string.IsNullOrEmpty(p.FillByRole) && p.FillByRole.Contains(sectionOwner)).ToList();
                            property.SetValue(actualSection, fillByMatrix);
                        }
                        else if (property.GetCustomAttribute<IsApproverDetailsAttribute>() != null && property.GetCustomAttribute<IsApproverDetailsAttribute>().IsApproverDetailsField)
                        {
                            AppApprovalMatrixHelper approverHelper = new AppApprovalMatrixHelper();
                            string approverMatrixListName = property.GetCustomAttribute<IsApproverDetailsAttribute>().ApproverMatrixListName;
                            if (approverMatrixFromList == null)
                            {
                                approverMatrixFromList = approverHelper.GetAppApprovalMatrix(context, web, listDetail.ItemId, approverMatrixListName);
                            }
                            sectionOwner = role;
                            if (approverMatrixFromList.Any(p => !string.IsNullOrEmpty(p.SectionName) && p.SectionName.Contains(actualSection.SectionName)))
                            {
                                sectionOwner = approverMatrixFromList.FirstOrDefault(p => !string.IsNullOrEmpty(p.SectionName) && p.SectionName.Contains(actualSection.SectionName)).Role;
                            }
                            ApplicationStatus currentUserDetails = this.GetCurrentApproverDetails(role, sectionOwner, approverMatrixFromList);
                            property.SetValue(actualSection, currentUserDetails);
                        }
                    }
                }
                else
                {
                    AppApprovalMatrixHelper approverHelper = new AppApprovalMatrixHelper();
                    foreach (PropertyInfo property in properties)
                    {
                        if (property.GetCustomAttribute<ContainsMasterDataAttribute>() != null && property.GetCustomAttribute<ContainsMasterDataAttribute>().ContainsMasterData)
                        {
                            List<IMaster> masters = property.GetValue(actualSection);
                            masters = masterHelper.GetMasterData(context, web, masters);
                            property.SetValue(actualSection, masters);
                        }
                        else if (property.GetCustomAttribute<IsApproverDetailsAttribute>() != null && property.GetCustomAttribute<IsApproverDetailsAttribute>().IsApproverDetailsField)
                        {
                            if (approverMatrixFromList == null)
                            {
                                approverMatrixFromList = approverHelper.GetGlobalAppApproverMatrix(applicationName, formName);
                            }
                            sectionOwner = role;
                            if (approverMatrixFromList.Any(p => !string.IsNullOrEmpty(p.SectionName) && p.SectionName.Contains(actualSection.SectionName)))
                            {
                                sectionOwner = approverMatrixFromList.FirstOrDefault(p => !string.IsNullOrEmpty(p.SectionName) && p.SectionName.Contains(actualSection.SectionName)).Role;
                            }
                            ApplicationStatus currentUserDetails = approverMatrixFromList.FirstOrDefault(p => !string.IsNullOrEmpty(p.Role) && p.Role == sectionOwner);
                            if (currentUserDetails != null)
                            {
                                if (currentUserDetails.Role.Contains(role))
                                {
                                    currentUserDetails.Approver = userID;
                                }
                            }
                            property.SetValue(actualSection, currentUserDetails);
                        }
                        else if (property.GetCustomAttribute<IsApproverMatrixFieldAttribute>() != null && property.GetCustomAttribute<IsApproverMatrixFieldAttribute>().IsApproverMatrixField)
                        {
                            if (approverMatrixFromList == null)
                            {
                                approverMatrixFromList = approverHelper.GetGlobalAppApproverMatrix(applicationName, formName);
                            }
                            sectionOwner = role;
                            if (approverMatrixFromList.Any(p => !string.IsNullOrEmpty(p.SectionName) && p.SectionName.Contains(actualSection.SectionName)))
                            {
                                sectionOwner = approverMatrixFromList.FirstOrDefault(p => !string.IsNullOrEmpty(p.SectionName) && p.SectionName.Contains(actualSection.SectionName)).Role;
                            }
                            List<ApplicationStatus> fillByMatrix = approverMatrixFromList.Where(p => !string.IsNullOrEmpty(p.FillByRole) && p.FillByRole.Contains(sectionOwner)).ToList();
                            fillByMatrix.Remove(fillByMatrix.FirstOrDefault(p => p.Role == UserRoles.VIEWER));
                            property.SetValue(actualSection, fillByMatrix);
                        }
                    }
                }
                i++;
            }
            return actualSection;
        }

        /// <summary>
        /// Fills the section from list.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="actualSection">The actual section.</param>
        /// <param name="userID">The user email.</param>
        /// <param name="otherParamDictionary">The other parameter dictionary.</param>
        /// <returns>
        /// list of ISection 
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Couldn't able to divide its complex"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Couldn't able to divide its complex")]
        private List<ISection> FillSectionsFromList(ClientContext context, Web web, string applicationName, string formName, dynamic actualSection, string userID, Dictionary<string, string> otherParamDictionary)
        {
            List<ISection> sections = new List<ISection>();
            List<ApplicationStatus> approverMatrixFromList = null;
            TransactionHelper tranHelper = new TransactionHelper();
            MasterDataHelper masterHelper = new MasterDataHelper();
            //// otherParamDictionary.Add("ISATTACHMENT", null);
            //// int i = 0;
            ListItemDetail listDetail = null;
            List list = null;
            if (actualSection.ListDetails != null && actualSection.ListDetails.Count > 0)
            {
                listDetail = actualSection.ListDetails[0];
                list = web.Lists.GetByTitle(actualSection.ListDetails[0].ListName);
                CamlQuery qry = new CamlQuery();
                qry.ViewXml = listDetail.CamlQuery;
                ListItemCollection items = list.GetItems(qry);
                context.Load(items);
                context.ExecuteQuery();

                foreach (var item in items)
                {
                    if (item != null)
                    {
                        approverMatrixFromList = null;
                        actualSection = Activator.CreateInstance(actualSection.GetType()) as dynamic;
                        AttachmentCollection attachments = null;
                        PropertyInfo[] properties = actualSection.GetType().GetProperties();
                        ////if (otherParamDictionary[SectionNameConstant.ISATTACHMENT] != null && otherParamDictionary[SectionNameConstant.ISATTACHMENT].Equals("TRUE"))
                        ////{
                        ////    if (Convert.ToString(item["Attachments"]) == "True")
                        ////    {
                        ////        context.Load(item.AttachmentFiles);
                        ////        context.ExecuteQuery();
                        ////        attachments = item.AttachmentFiles; //attachmentsFolder.Files;
                        ////    }
                        ////}
                        ////actualSection.ListDetails[i].ListItemObject = item;
                        foreach (PropertyInfo property in properties)
                        {
                            bool isListColumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                            bool isCurrentListCoumn = property.GetCustomAttribute<FieldBelongToListAttribute>() == null || property.GetCustomAttribute<FieldBelongToListAttribute>().ListNames.Contains(listDetail.ListName);

                            if (isListColumn && isCurrentListCoumn)
                            {
                                string propertyName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                                bool islookup = property.GetCustomAttribute<FieldColumnNameAttribute>() != null ? property.GetCustomAttribute<FieldColumnNameAttribute>().IsLookup : false;
                                if (property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile)
                                {
                                    if (attachments != null)
                                    {
                                        List<FileDetails> objAttachmentFiles = this.GetAttachments(attachments);
                                        property.SetValue(actualSection, objAttachmentFiles);
                                    }
                                }
                                else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson)
                                {
                                    FieldUserValue[] users = null;
                                    if (item[propertyName] != null)
                                    {
                                        if (property.GetCustomAttribute<IsPersonAttribute>().IsMultiple)
                                        {
                                            users = item[propertyName] as FieldUserValue[];
                                        }
                                        else
                                        {
                                            users = new FieldUserValue[1];
                                            users[0] = item[propertyName] as FieldUserValue;
                                        }
                                    }
                                    if (users != null)
                                    {
                                        string personEmails = string.Empty;
                                        if (property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                                        {
                                            personEmails = GetNameFromPersonField(context, web, users);
                                        }
                                        else
                                        {
                                            personEmails = GetEmailsFromPersonField(context, web, users);
                                        }
                                        property.SetValue(actualSection, personEmails);
                                    }
                                }
                                else if (islookup)
                                {
                                    FieldLookupValue lookupField = item[propertyName] as FieldLookupValue;
                                    property.SetValue(actualSection, lookupField.LookupId);
                                }
                                else
                                {
                                    object value = item[propertyName];
                                    Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                    object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                                    property.SetValue(actualSection, safeValue);
                                }
                            }
                            else if (property.GetCustomAttribute<ContainsMasterDataAttribute>() != null && property.GetCustomAttribute<ContainsMasterDataAttribute>().ContainsMasterData)
                            {
                                List<IMaster> masters = property.GetValue(actualSection);
                                masters = masterHelper.GetMasterData(context, web, masters);
                                property.SetValue(actualSection, masters);
                            }
                            else if (property.GetCustomAttribute<IsTranAttribute>() != null && property.GetCustomAttribute<IsTranAttribute>().IsTranField)
                            {
                                if (listDetail.IsMainList)
                                {
                                    string listName = property.GetCustomAttribute<IsTranAttribute>().TranListName;
                                    Type tranType = property.GetCustomAttribute<IsTranAttribute>().TranType;
                                    if (!string.IsNullOrEmpty(listName) && tranType != null)
                                    {
                                        List<ITrans> trans = tranHelper.GetTransactionListData(context, web, tranType, listName, listDetail.ItemId);
                                        property.SetValue(actualSection, trans);
                                    }
                                }
                            }
                            else if (property.GetCustomAttribute<IsApproverMatrixFieldAttribute>() != null && property.GetCustomAttribute<IsApproverMatrixFieldAttribute>().IsApproverMatrixField)
                            {
                                AppApprovalMatrixHelper approverHelper = new AppApprovalMatrixHelper();
                                string approverMatrixListName = property.GetCustomAttribute<IsApproverMatrixFieldAttribute>().ApproverMatrixListName;
                                if (approverMatrixFromList == null)
                                {
                                    approverMatrixFromList = approverHelper.GetAppApprovalMatrix(context, web, item.Id, approverMatrixListName, true);
                                }
                                ////// sectionOwner = role;
                                // if (approverMatrixFromList.Any(p => !string.IsNullOrEmpty(p.SectionName) && p.SectionName.Contains(actualSection.SectionName)))
                                // {
                                // ////    sectionOwner = approverMatrixFromList.FirstOrDefault(p => !string.IsNullOrEmpty(p.SectionName) && p.SectionName.Contains(actualSection.SectionName)).Role;
                                // }
                                // List<ApplicationStatus> fillByMatrix = approverMatrixFromList.Where(p => !string.IsNullOrEmpty(p.FillByRole) && p.FillByRole.Contains(sectionOwner)).ToList();
                                property.SetValue(actualSection, approverMatrixFromList);
                            }
                            else if (property.GetCustomAttribute<IsApproverDetailsAttribute>() != null && property.GetCustomAttribute<IsApproverDetailsAttribute>().IsApproverDetailsField)
                            {
                                AppApprovalMatrixHelper approverHelper = new AppApprovalMatrixHelper();
                                string approverMatrixListName = property.GetCustomAttribute<IsApproverDetailsAttribute>().ApproverMatrixListName;
                                if (approverMatrixFromList == null)
                                {
                                    approverMatrixFromList = approverHelper.GetAppApprovalMatrix(context, web, item.Id, approverMatrixListName);
                                }
                                ////// sectionOwner = role;
                                // if (approverMatrixFromList.Any(p => !string.IsNullOrEmpty(p.SectionName) && p.SectionName.Contains(actualSection.SectionName)))
                                // {
                                /////     sectionOwner = approverMatrixFromList.FirstOrDefault(p => !string.IsNullOrEmpty(p.SectionName) && p.SectionName.Contains(actualSection.SectionName)).Role;
                                // }
                                // ApplicationStatus currentUserDetails = this.GetCurrentApproverDetails(role, sectionOwner, approverMatrixFromList);
                                // property.SetValue(actualSection, currentUserDetails);
                            }
                        }
                        sections.Add(actualSection);
                    }
                }
            }
            return sections;
        }

        /// <summary>
        /// Gets the current approver details.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="sectionOwner">The section owner.</param>
        /// <param name="approverMatrix">The approver matrix.</param>
        /// <returns>
        /// Application status
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "NA")]
        public ApplicationStatus GetCurrentApproverDetails(string role, string sectionOwner, List<ApplicationStatus> approverMatrix)
        {
            ApplicationStatus approverDetail = null;
            if (approverMatrix.Count(p => !string.IsNullOrEmpty(p.Role) && p.Role == sectionOwner && p.Status != ApproverStatus.APPROVED) > 1)
            {
                List<ApplicationStatus> roleApprovers = approverMatrix.OrderBy(p => Convert.ToInt16(p.Levels)).Where(p => !string.IsNullOrEmpty(p.Role) && p.Role == sectionOwner && p.Status != ApproverStatus.APPROVED).ToList();
                roleApprovers.ForEach(p =>
                {
                    if (approverDetail == null && approverMatrix.Any(x => Convert.ToInt32(x.Levels) == Convert.ToInt32(p.Levels) && (x.IsOptional == false && x.Status == ApproverStatus.PENDING)) || approverMatrix.Any(x => Convert.ToInt32(x.Levels) == Convert.ToInt32(p.Levels) && x.Status == ApproverStatus.APPROVED))
                    {
                        approverDetail = approverMatrix.OrderBy(x => Convert.ToInt16(x.Levels)).FirstOrDefault(x => !string.IsNullOrEmpty(x.Role) && x.Role == sectionOwner && Convert.ToInt32(x.Levels) == Convert.ToInt32(p.Levels));
                    }
                });
            }
            else
            {
                approverDetail = approverMatrix.OrderBy(p => Convert.ToInt16(p.Levels)).FirstOrDefault(p => !string.IsNullOrEmpty(p.Role) && p.Role == sectionOwner);
            }
            return approverDetail;
        }

        /// <summary>
        /// Get List of Attachments
        /// </summary>
        /// <param name="attachments">The attachments.</param>
        /// <returns>
        /// list of fill details
        /// </returns>
        public List<FileDetails> GetAttachments(AttachmentCollection attachments)
        {
            List<FileDetails> objAttachmentFiles = new List<FileDetails>();
            if (attachments != null)
            {
                FileDetails fileDetail = null;
                foreach (var file in attachments)
                {
                    fileDetail = new FileDetails();
                    fileDetail.FileId = Guid.NewGuid().ToString();
                    fileDetail.FileName = file.FileName;
                    fileDetail.FileURL = file.ServerRelativeUrl;
                    objAttachmentFiles.Add(fileDetail);
                }
            }
            return objAttachmentFiles;
        }

        /// <summary>
        /// Get List of Attachments
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// list of fill details
        /// </returns>
        public List<FileDetails> GetAttachmentsUsingREST(string url)
        {
            List<FileDetails> objAttachmentFiles = new List<FileDetails>();
            JObject jobj = RESTHelper.GetDataUsingRest(url, "GET");
            JArray jarr = (JArray)jobj["d"]["results"];
            FileDetails fileDetail = null;
            foreach (var jToken in jarr)
            {
                if (((dynamic)jToken).ServerRelativeUrl != null)
                {
                    fileDetail = new FileDetails();
                    fileDetail.FileId = Guid.NewGuid().ToString();
                    fileDetail.FileName = ((dynamic)jToken).FileName.Value;
                    fileDetail.FileURL = ((dynamic)jToken).ServerRelativeUrl.Value;
                    objAttachmentFiles.Add(fileDetail);
                }
            }
            return objAttachmentFiles;
        }

        /// <summary>
        /// Gets the enable section names.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="role">The role.</param>
        /// <returns>
        /// list of string value
        /// </returns>
        public Dictionary<string, bool> GetEnableSectionNames(string applicationName, string formName, string role)
        {
            Dictionary<string, bool> sectionNames = new Dictionary<string, bool>();
            Logger.Info("Helper.GetButtons for role '" + role + "', appName='" + applicationName + "', formName='" + formName + "'");
            if (!string.IsNullOrEmpty(applicationName) && !string.IsNullOrEmpty(formName) && !string.IsNullOrEmpty(role))
            {
                string siteURL = this.GetSiteURL(SiteURLs.ROOTSITEURL); // this.GetConfigVariable(SiteURLs.ROOTSITEURL);
                ClientContext context = this.CreateClientContext(siteURL);
                Web web = this.CreateWeb(context);
                context.Load(web);
                context.ExecuteQuery();
                List slaList = web.Lists.GetByTitle(ListNames.SLAList);
                CamlQuery query = new CamlQuery();
                query.ViewXml = @"<View>
                                        <Query>
                                            <Where>
                                                <And>
                                                    <And>
                                                        <Eq>
                                                            <FieldRef Name='ApplicationName' />
                                                            <Value Type='Text'>" + applicationName + @"</Value>
                                                        </Eq>
                                                        <Eq>
                                                            <FieldRef Name='FormName' />
                                                            <Value Type='Text'>" + formName + @"</Value>
                                                        </Eq>
                                                    </And>
                                                    <Eq>
                                                        <FieldRef Name='Role' />
                                                        <Value Type='Text'>" + role + @"</Value>
                                                    </Eq>
                                                </And>
                                            </Where>
                                        </Query>
                                        </View>";
                ListItemCollection items = slaList.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();
                foreach (ListItem item in items)
                {
                    sectionNames = this.GetTaxonomyFieldValueCollection(item["SectionName"] as TaxonomyFieldValueCollection, true, sectionNames);
                    sectionNames = this.GetTaxonomyFieldValueCollection(item["HiddenSection"] as TaxonomyFieldValueCollection, false, sectionNames);
                }
            }
            return sectionNames;
        }

        #endregion

        #region Buttons
        /// <summary>
        /// Gets the buttons.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="role">The role.</param>
        /// <param name="status">The status.</param>
        /// <param name="otherParam">The other parameter.</param>
        /// <param name="formType">Type of the form.</param>
        /// <returns>
        /// list of buttons
        /// </returns>
        public List<Button> GetButtons(string applicationName, string formName, string role, string status, Dictionary<string, string> otherParam, string formType = FormType.MAIN)
        {
            List<Button> buttons = new List<Button>();
            if (!string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(applicationName) && !string.IsNullOrEmpty(formName) && !string.IsNullOrEmpty(formType))
            {
                buttons = GlobalCachingProvider.Instance.GetItem(ListNames.ButtonsList, false) as List<Button>;
                if (buttons == null)
                {
                    buttons = this.GetAllButtons();
                    //// buttons = buttons.Where(b => b.Role.ToLower().Split(',').Select(r => r.Trim()).Contains(role.ToLower())).ToList<Button>();
                    GlobalCachingProvider.Instance.AddItem(ListNames.ButtonsList, buttons);
                }
                Logger.Info("Helper.GetButtons for role '" + role + "', appName='" + applicationName + "', formName='" + formName + "', status='" + status + "', other params=" + JsonConvert.SerializeObject(otherParam));
                if (string.IsNullOrEmpty(status))
                {
                    status = FormStatus.NEW;
                }
                buttons = (from b in buttons
                           where
                           !string.IsNullOrEmpty(b.Role) &&
                           Convert.ToString(b.Role).ToLower().Split(',').Contains(role.ToLower()) &&
                           !string.IsNullOrEmpty(b.ApplicationName) &&
                           Convert.ToString(b.ApplicationName).ToLower() == applicationName.ToLower() &&
                           !string.IsNullOrEmpty(b.FormName) &&
                           Convert.ToString(b.FormName).ToLower() == formName.ToLower() &&
                           !string.IsNullOrEmpty(b.FormStatus) &&
                            Convert.ToString(b.FormStatus).ToLower().Split(',').Contains(status.ToLower()) &&
                           !string.IsNullOrEmpty(b.FormType) &&
                           Convert.ToString(b.FormType).ToLower() == formType.ToLower()
                           select b).ToList();

                if (otherParam != null && otherParam.ContainsKey(Parameter.CURRENTFROMLEVEL) && !string.IsNullOrEmpty(otherParam[Parameter.CURRENTFROMLEVEL]))
                {
                    buttons = (from b in buttons
                               where string.IsNullOrEmpty(b.Levels) || Convert.ToString(b.Levels) == otherParam[Parameter.CURRENTFROMLEVEL].ToString()
                               select b).ToList();
                }
            }
            return buttons;
        }

        /// <summary>
        /// Gets all buttons.
        /// </summary>
        /// <returns>List of all buttons</returns>
        private List<Button> GetAllButtons()
        {
            Logger.Info("Get All Buttons from Sharepoint");
            List<Button> buttons = new List<Button>();
            Logger.Info("Helper.GetAllButtons");
            string siteURL = this.GetSiteURL(SiteURLs.ROOTSITEURL); // this.GetConfigVariable(SiteURLs.ROOTSITEURL);
            ClientContext context = this.CreateClientContext(siteURL);
            Web web = this.CreateWeb(context);
            context.Load(web);
            context.ExecuteQuery();
            List spList = web.Lists.GetByTitle(ListNames.ButtonsList);
            AppApprovalMatrixHelper apphelper = new AppApprovalMatrixHelper();
            ListItemCollection items = spList.GetItems(CamlQuery.CreateAllItemsQuery());
            context.Load(items);
            context.ExecuteQuery();
            foreach (ListItem item in items)
            {
                string jsFunction = string.Empty;
                ButtonActionStatus buttonStatus = ButtonActionStatus.None;
                if (item["JsFunctionName"] != null)
                {
                    jsFunction = ((Microsoft.SharePoint.Client.FieldLookupValue)item["JsFunctionName"]).LookupValue;
                }
                if (item["ButtonActionValue_x003a_Value"] != null)
                {
                    buttonStatus = (ButtonActionStatus)Convert.ToInt32(((Microsoft.SharePoint.Client.FieldLookupValue)item["ButtonActionValue_x003a_Value"]).LookupValue);
                }
                buttons.Add(new Button()
                {
                    Name = Convert.ToString(item["Title"]),
                    Icon = Convert.ToString(item["Icon"]),
                    ButtonStatus = buttonStatus,
                    JsFunction = jsFunction,
                    IsVisible = (bool)item["IsVisible"],
                    SendBackTo = Convert.ToString(item["SendBackTo"]),
                    SendToRole = Convert.ToString(item["SendToRole"]),
                    Levels = Convert.ToString(item["Levels"]),
                    Role = Convert.ToString(item["Role"]).Trim(','),
                    ApplicationName = apphelper.GetTaxonomyFieldValue(item["ApplicationName"] as TaxonomyFieldValue),
                    FormName = apphelper.GetTaxonomyFieldValueCollection(item["FormName"] as TaxonomyFieldValueCollection).Trim(','),
                    FormStatus = Convert.ToString(item["FormStatus"]),
                    FormType = Convert.ToString(item["FormType"]),
                    Sequence = Convert.ToInt32(item["Sequence"]),
                    ToolTip = Convert.ToString(item["ToolTip"])
                });
            }
            return buttons.OrderBy(x => x.Sequence).ToList();
        }
        #endregion

        /////// <summary>
        /////// Gets all buttons.
        /////// </summary>
        /////// <returns>List of all buttons</returns>
        ////private List<Button> GetAllButtons(string applicationName , string formName, string role, string formType)
        ////{
        ////    Logger.Info("Get All Buttons from Sharepoint");
        ////    List<Button> buttons = new List<Button>();
        ////    Logger.Info("Helper.GetAllButtons");
        ////    string siteURL = this.GetSiteURL(SiteURLs.ROOTSITEURL); // this.GetConfigVariable(SiteURLs.ROOTSITEURL);
        ////    ClientContext context = this.CreateClientContext(siteURL);
        ////    Web web = this.CreateWeb(context);
        ////    context.Load(web);
        ////    context.ExecuteQuery();
        ////    List spList = web.Lists.GetByTitle(ListNames.ButtonsList);
        ////    AppApprovalMatrixHelper apphelper = new AppApprovalMatrixHelper();
        ////    ListItemCollection items = spList.GetItems(CamlQuery.CreateAllItemsQuery());
        ////    context.Load(items);
        ////    context.ExecuteQuery();
        ////    foreach (ListItem item in items)
        ////    {
        ////        string jsFunction = string.Empty;
        ////        ButtonActionStatus buttonStatus = ButtonActionStatus.None;
        ////        if (item["JsFunctionName"] != null)
        ////        {
        ////            jsFunction = ((Microsoft.SharePoint.Client.FieldLookupValue)item["JsFunctionName"]).LookupValue;
        ////        }
        ////        if (item["ButtonActionValue_x003a_Value"] != null)
        ////        {
        ////            buttonStatus = (ButtonActionStatus)Convert.ToInt32(((Microsoft.SharePoint.Client.FieldLookupValue)item["ButtonActionValue_x003a_Value"]).LookupValue);
        ////        }

        ////        buttons.Add(new Button()
        ////        {
        ////            Name = Convert.ToString(item["Title"]),
        ////            Icon = Convert.ToString(item["Icon"]),
        ////            ButtonStatus = buttonStatus,
        ////            JsFunction = jsFunction,
        ////            IsVisible = (bool)item["IsVisible"],
        ////            SendBackTo = Convert.ToString(item["SendBackTo"]),
        ////            SendToRole = Convert.ToString(item["SendToRole"]),
        ////            Levels = Convert.ToString(item["Levels"]),
        ////            Role = Convert.ToString(item["Role"]).Trim(','),
        ////            ApplicationName = apphelper.GetTaxonomyFieldValue(item["ApplicationName"] as TaxonomyFieldValue),
        ////            FormName = apphelper.GetTaxonomyFieldValueCollection(item["FormName"] as TaxonomyFieldValueCollection).Trim(','),
        ////            FormStatus = Convert.ToString(item["FormStatus"]),
        ////            FormType = Convert.ToString(item["FormType"]),
        ////            Sequence = Convert.ToInt32(item["Sequence"]),
        ////            ToolTip = Convert.ToString(item["ToolTip"])
        ////        });
        ////    }
        ////    return buttons.OrderBy(x => x.Sequence).ToList();
        ////}

        #region ActivityLog

        /// <summary>
        /// Gets the activity log.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="lookupId">The lookup identifier.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <returns>
        /// section object
        /// </returns>
        public ISection GetActivityLog(ClientContext context, Web web, string listName, int lookupId, bool isActive)
        {
            ISection activitySection = new ActivityLogSection();
            if (context != null && web != null && !string.IsNullOrEmpty(listName) && lookupId > 0)
            {
                if (this.GetConfigVariable("UseRESTAPI").ToLower().Equals("true"))
                {
                    activitySection = this.GetActivityLogUsingREST(context, web, listName, lookupId, isActive);
                }
                else
                {
                    activitySection = this.GetActivityLogUsingCSOM(context, web, listName, lookupId, isActive);
                }
            }
            return activitySection;
        }

        /// <summary>
        /// Gets the activity log Using CSOM.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="lookupId">The lookup identifier.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <returns>
        /// section object
        /// </returns>
        public ISection GetActivityLogUsingCSOM(ClientContext context, Web web, string listName, int lookupId, bool isActive)
        {
            ActivityLogSection activitySection = new ActivityLogSection() { IsActive = isActive };
            if (context != null && web != null && !string.IsNullOrEmpty(listName) && lookupId > 0)
            {
                List auditList = web.Lists.GetByTitle(listName);
                CamlQuery query = new CamlQuery();
                query.ViewXml = @"<View>
                                                <Query>
                                                    <Where>
                                                        <Eq>
                                                            <FieldRef Name='RequestID' />
                                                            <Value Type='Lookup'>" + lookupId + @"</Value>
                                                        </Eq>
                                                    </Where>
                                                </Query>
                                                </View>";
                ListItemCollection items = auditList.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();
                int i = 1;
                foreach (ListItem item in items)
                {
                    ActivityLog log = new ActivityLog();
                    log.No = i;
                    log.Changes = Convert.ToString(item["Changes"]);
                    log.Activity = Convert.ToString(item["Activity"]);
                    log.PerformedOn = (DateTime)item["ActivityDate"];
                    log.Created = (DateTime)item["Created"];
                    log.PerformedBy = ((FieldUserValue)item["ActivityBy"]).LookupValue;
                    log.SectionName = Convert.ToString(item["SectionName"]);
                    activitySection.ActivityLogs.Add(log);
                }
            }
            return activitySection;
        }

        /// <summary>
        /// Gets the activity log Using REST.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="lookupId">The lookup identifier.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <returns>
        /// section object
        /// </returns>
        public ISection GetActivityLogUsingREST(ClientContext context, Web web, string listName, int lookupId, bool isActive)
        {
            ActivityLogSection activitySection = new ActivityLogSection() { IsActive = isActive };
            if (context != null && web != null && !string.IsNullOrEmpty(listName) && lookupId > 0)
            {
                string selectClause = "Changes,Activity,ActivityDate,Created,ActivityBy/Title,SectionName";
                string expandClause = "ActivityBy";
                JObject jobj = RESTHelper.GetDataUsingRest(web.Url + "/_api/web/lists/GetByTitle('" + listName + "')/Items?$select=" + selectClause.Trim(',') + "&$expand=" + expandClause.Trim(',') + "&$filter=RequestID eq '" + lookupId + "'&$top=" + int.MaxValue, "GET");
                JArray jarr = (JArray)jobj["d"]["results"];
                foreach (JToken jToken in jarr)
                {
                    ActivityLog log = new ActivityLog();
                    log.No = activitySection.ActivityLogs.Count + 1;
                    log.Changes = Convert.ToString(jToken["Changes"]);
                    log.Activity = Convert.ToString(jToken["Activity"]);
                    log.PerformedOn = (DateTime)jToken["ActivityDate"];
                    log.Created = (DateTime)jToken["Created"];
                    if (((dynamic)jToken["ActivityBy"]).Title != null)
                    {
                        log.PerformedBy = ((dynamic)jToken["ActivityBy"])["Title"].ToString();
                    }
                    log.SectionName = Convert.ToString(jToken["SectionName"]);
                    activitySection.ActivityLogs.Add(log);
                }
            }
            return activitySection;
        }

        /// <summary>
        /// Gets the activity string.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns>Activity String</returns>
        public string GetActivityString(ISection section)
        {
            string strActivity = string.Empty;
            if (section != null)
            {
                PropertyInfo[] properties = section.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    bool isListCoumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                    bool isCurrentApproverField = property.GetCustomAttribute<IsApproverDetailsAttribute>() != null && property.GetCustomAttribute<IsApproverDetailsAttribute>().IsApproverDetailsField;
                    if (isListCoumn)
                    {
                        //string propertyName = property.Name;
                        string propertyName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                        if (propertyName.Contains("FNL") || propertyName.Contains("FileNameList") || propertyName.Contains("FileList") || propertyName.Contains("ID") || propertyName.ToLower().EndsWith("id"))
                        {
                            continue;
                        }
                        object propertyValue = property.GetValue(section);
                        bool isFile = property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile;
                        if (isFile)
                        {
                            string strFileNames = string.Empty;
                            if (propertyValue != null)
                            {
                                foreach (FileDetails file in (List<FileDetails>)propertyValue)
                                {
                                    if (string.IsNullOrEmpty(strFileNames))
                                    {
                                        strFileNames = file.FileName;
                                    }
                                    else
                                    {
                                        strFileNames = strFileNames.Trim(',') + "," + file.FileName;
                                    }
                                }
                                propertyValue = strFileNames;
                            }
                        }
                        bool isPerson = property.GetCustomAttribute<IsPersonAttribute>() != null;
                        if (isPerson)
                        {
                            if (property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                            {
                                propertyValue = property.GetValue(section);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        ////bool isPersonName = property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().ReturnName;
                        ////if (isPersonName)
                        ////{
                        ////    propertyValue = property.GetValue(section);
                        ////}                 
                        if (string.IsNullOrEmpty(strActivity))
                        {
                            strActivity = propertyName + "\t" + propertyValue;
                        }
                        else
                        {
                            strActivity += "\n" + propertyName + "\t" + propertyValue;
                        }
                    }
                    else if (isCurrentApproverField)
                    {
                        object propertyValue = property.GetValue(section);
                        if (propertyValue != null)
                        {
                            ApplicationStatus approverDetails = (ApplicationStatus)propertyValue;
                            string approverActivityLog = "Assigned date" + "\t" + (approverDetails.AssignDate.HasValue ? approverDetails.AssignDate.Value.ToString("dd/MM/yyyy") : string.Empty);
                            approverActivityLog += "\nApproved/Updated date" + "\t" + DateTime.Now.ToString("dd/MM/yyyy");
                            approverActivityLog += "\nApproved/Updated time" + "\t" + DateTime.Now.ToString("hh:mm tt");
                            approverActivityLog += "\n" + "Approver Comment" + "\t" + approverDetails.Comments;
                            if (string.IsNullOrEmpty(strActivity))
                            {
                                strActivity = approverActivityLog;
                            }
                            else
                            {
                                strActivity += "\n" + approverActivityLog;
                            }
                        }
                    }
                }
            }
            return strActivity;
        }

        /// <summary>
        /// Saves the activity log.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="lookupId">The lookup identifier.</param>
        /// <param name="section">The section.</param>
        /// <param name="activityBy">The activity by.</param>
        /// <param name="activity">The activity.</param>
        /// <returns>true or false</returns>
        public bool SaveActivityLog(ClientContext context, Web web, string listName, int lookupId, ISection section, string activityBy, string activity)
        {
            bool isSuccess = false;
            if (context != null && web != null && !string.IsNullOrEmpty(listName) && lookupId > 0 && !string.IsNullOrEmpty(activityBy))
            {
                Logger.Info("Called Activity Log Condition Passed-> listname=" + listName + ", lookupid=" + lookupId + ", activityby=" + activityBy);
                string changes = string.Empty;
                string sectionName = string.Empty;
                if (section != null)
                {
                    changes = this.GetActivityString(section);
                    sectionName = section.SectionName;
                }
                FieldUserValue usr = GetFieldUserValueFromPerson(context, web, activityBy);
                List auditList = web.Lists.GetByTitle(listName);
                ListItem itemActivityLog = auditList.AddItem(new ListItemCreationInformation());
                itemActivityLog["Activity"] = activity;
                itemActivityLog["Changes"] = changes;
                itemActivityLog["ActivityDate"] = DateTime.Now;
                itemActivityLog["ActivityBy"] = usr;
                itemActivityLog["RequestID"] = lookupId;
                itemActivityLog["SectionName"] = sectionName;
                itemActivityLog.Update();
                context.Load(itemActivityLog);
                context.ExecuteQuery();
                isSuccess = true;
                Logger.Info("Activity Log Parameter Saved -> " +
                    "Activity=" + activity + ", \n" +
                    "Changes=" + changes + ", \n" +
                    "ActivityDate=" + DateTime.Now.ToShortDateString() + ", \n" +
                    "ActivityBy=" + activityBy + ", \n" +
                    "RequestID=" + lookupId + ", \n" +
                    "SectionName=" + sectionName + ", \n");
            }
            return isSuccess;
        }

        /// <summary>
        /// Saves the activity log.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="lookupId">The lookup identifier.</param>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="changesLog">The changes log.</param>
        /// <param name="activityBy">The activity by.</param>
        /// <param name="activity">The activity.</param>
        /// <returns>
        /// Save Custom Activity Log
        /// </returns>
        public bool SaveActivityLog(ClientContext context, Web web, string listName, int lookupId, string sectionName, string changesLog, string activityBy, string activity)
        {
            bool isSuccess = false;
            if (context != null && web != null && !string.IsNullOrEmpty(listName) && lookupId > 0 && !string.IsNullOrEmpty(activityBy))
            {
                Logger.Info("Called Save Activity Log", listName, lookupId, sectionName, changesLog, activityBy, activity);
                string changes = changesLog;
                FieldUserValue usr = GetFieldUserValueFromPerson(context, web, activityBy);
                List auditList = web.Lists.GetByTitle(listName);
                ListItem itemActivityLog = auditList.AddItem(new ListItemCreationInformation());
                itemActivityLog["Activity"] = activity;
                itemActivityLog["Changes"] = changes;
                itemActivityLog["ActivityDate"] = DateTime.Now;
                itemActivityLog["ActivityBy"] = usr;
                itemActivityLog["RequestID"] = lookupId;
                itemActivityLog["SectionName"] = sectionName;
                itemActivityLog.Update();
                context.Load(itemActivityLog);
                context.ExecuteQuery();
                isSuccess = true;
            }
            return isSuccess;
        }
        #endregion

        #region Employee Master

        /// <summary>
        /// Gets the user information.
        /// </summary>
        /// <param name="userEmail">The user email.</param>
        /// <returns>
        /// user details
        /// </returns>
        public UserDetails GetUserInformation(string userEmail)
        {
            UserDetails user = new UserDetails();
            if (!string.IsNullOrEmpty(userEmail))
            {
                string siteURL = this.GetSiteURL(SiteURLs.ROOTSITEURL);
                ClientContext context = this.CreateClientContext(siteURL);
                Web web = this.CreateWeb(context);
                context.Load(web);
                context.ExecuteQuery();
                List userList = web.Lists.GetByTitle(ListNames.EmployeeMasterList);
                CamlQuery qry = new CamlQuery();
                qry.ViewXml = @"<View>
                                            <Query>
                                                <Where>
                                                <Eq>
                                                    <FieldRef Name='Email' />
                                                    <Value Type='Text'>" + userEmail + @"</Value>
                                                </Eq>
                                                </Where>
                                            </Query>
                                            </View>";
                ListItemCollection items = userList.GetItems(qry);
                context.Load(items);
                context.ExecuteQuery();
                if (items.Count > 0)
                {
                    ListItem item = items[0];
                    user = this.SetUserInformation(context, web, item);
                }
            }
            return user;
        }

        /// <summary>
        /// Get User Information
        /// </summary>
        /// <param name="context">Context Object</param>
        /// <param name="web">web Object</param>
        /// <param name="userID">User ID</param>
        /// <returns>User Detail</returns>
        public UserDetails GetUserInformation(ClientContext context, Web web, string userID)
        {
            UserDetails user = new UserDetails();
            MasterDataHelper masterHelper = new MasterDataHelper();
            List<UserDetails> userInfoList = masterHelper.GetAllEmployee(context, web);
            UserDetails detail = userInfoList.FirstOrDefault(p => p.UserId == userID);
            return detail;
        }

        /// <summary>
        /// Sets the user information.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        /// User Details
        /// </returns>
        private UserDetails SetUserInformation(ClientContext context, Web web, ListItem item)
        {
            UserDetails user = new UserDetails();
            user.Company = Convert.ToString(item["CompanyName"]);
            user.Department = Convert.ToString(item["Department"]); //item["Division"] != null ? ((FieldLookupValue)item["Division"]).LookupValue : string.Empty;
            user.RoleType = string.Empty;
            user.FullName = Convert.ToString(item["Title"]);
            user.UserEmail = Convert.ToString(item["Email"]);
            user.UserId = Convert.ToString(item["ID"]);
            ////user.Location = Convert.ToString(item["Location"]);
            user.Workspace = Convert.ToString(item["Workspace"]);
            user.ContactNo = Convert.ToString(item["ContactNo"]);
            user.EmployeeCode = Convert.ToString(item["EmployeeCode"]);
            user.Designation = Convert.ToString(item["Designation"]);
            if (item["ReportingManager"] != null)
            {
                user.ReportingManager = GetEmailsFromPersonField(context, web, item["ReportingManager"] as FieldUserValue);
            }
            else
            {
                user.ReportingManager = string.Empty;
            }

            return user;
        }

        /// <summary>
        /// Gets the invalid users.
        /// </summary>
        /// <param name="emailList">The email list.</param>
        /// <returns>invalid user list</returns>
        public List<string> GetInvalidUsers(List<string> emailList)
        {
            List<string> invaildUsers = new List<string>();
            if (emailList != null && emailList.Count > 0)
            {
                invaildUsers.AddRange(emailList);
                string siteURL = this.GetSiteURL(SiteURLs.ROOTSITEURL);
                ClientContext context = this.CreateClientContext(siteURL);
                Web web = this.CreateWeb(context);
                context.Load(web);
                context.ExecuteQuery();
                string strQuery = string.Join(System.Environment.NewLine, emailList.Select(x => string.Format("<Value Type='Text'>{0}</Value>", x)).ToArray());
                List userList = web.Lists.GetByTitle(ListNames.EmployeeMasterList);
                CamlQuery qry = new CamlQuery();
                qry.ViewXml = @"<View>
                                <Query>
                                    <Where>
                                    <In>
                                        <FieldRef Name='Email' />
                                        <Values>
                                            " + strQuery + @"
                                        </Values>
                                    </In>
                                    </Where>
                                </Query>
                                </View>";
                ListItemCollection items = userList.GetItems(qry);
                context.Load(items);
                context.ExecuteQuery();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        string email = Convert.ToString(item["Email"]);
                        if (emailList.Contains(email))
                        {
                            invaildUsers.Remove(email);
                        }
                    }
                }
            }
            return invaildUsers;
        }
        #endregion

        #region Delete Item(s)

        /// <summary>
        /// Deletes the data of request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="lookupId">The lookup identifier.</param>
        /// <param name="mainListName">Name of the main list.</param>
        /// <param name="childListNames">The child list names.</param>
        /// <returns>true or false</returns>
        public bool DeleteDataOfRequest(ClientContext context, Web web, int lookupId, string mainListName, List<string> childListNames)
        {
            bool isSuccess = false;
            if (context != null && web != null)
            {
                if (childListNames != null && childListNames.Count > 0)
                {
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = @"<View>
                                         <Query>
                                                <Where>
                                                      <Eq>
                                                         <FieldRef Name='RequestID' LookupId='TRUE'/>
                                                         <Value Type='Lookup'>" + lookupId + @"</Value>
                                                       </Eq>
                                                     </Where>
                                            </Query>
                                            </View>";
                    foreach (string childListName in childListNames)
                    {
                        List childList = web.Lists.GetByTitle(childListName);
                        ListItemCollection childItems = childList.GetItems(query);
                        context.Load(childItems);
                        context.ExecuteQuery();
                        int length = childItems.Count - 1;
                        while (true)
                        {
                            if (length == -1)
                            {
                                break;
                            }
                            childItems[length].DeleteObject();
                            context.ExecuteQuery();
                            length--;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(mainListName))
                {
                    List mainList = web.Lists.GetByTitle(mainListName);
                    ListItem item = mainList.GetItemById(lookupId);
                    item.DeleteObject();
                    context.ExecuteQuery();
                }
                isSuccess = true;
            }
            return isSuccess;
        }

        #endregion
    }
}