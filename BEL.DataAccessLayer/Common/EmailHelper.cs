namespace BEL.DataAccessLayer
{
    using Microsoft.Exchange.WebServices.Data;
    using Microsoft.SharePoint.Client;
    using BEL.CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Configuration;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// The Email Helper
    /// </summary>
    public class EmailHelper
    {
        /// <summary>
        /// Gets the email template.
        /// </summary>
        /// <param name="appContext">The application context.</param>
        /// <param name="appWeb">The application web.</param>
        /// <param name="templateName">Name of the template.</param>
        /// <param name="listItemDetails">The list item details.</param>
        /// <param name="customValues">The custom values.</param>
        /// <param name="role">The role.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <returns>
        /// Return Subject and Body Dictionary
        /// </returns>
        public Dictionary<string, string> GetEmailBody(ClientContext appContext, Web appWeb, string templateName, List<ListItemDetail> listItemDetails, Dictionary<string, string> customValues, string role, string applicationName = ApplicationNameConstants.DCRAPP, string formName = FormNameConstants.DCRFORM)
        {
            Dictionary<string, string> emailTemplate = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(templateName))
            {
                //BELDataAccessLayer helper = new BELDataAccessLayer();
                //string siteURL = BELDataAccessLayer.Instance.GetConfigVariable(SiteURLs.ROOTSITEURL);
                string siteURL = BELDataAccessLayer.Instance.GetSiteURL(SiteURLs.ROOTSITEURL);
                using (ClientContext context = BELDataAccessLayer.Instance.CreateClientContext(siteURL))
                {
                    List emailTemplateList = context.Web.Lists.GetByTitle(ListNames.EmailTemplateList);
                    CamlQuery qry = new CamlQuery();
                    qry.ViewXml = @"<View>
                                    <Query>
                                        <Where>
                                            <And>
                                                <And>
                                                    <And>
                                                        <Or>
                                                            <IsNull><FieldRef Name='Role' /></IsNull>
                                                            <Contains>
                                                                <FieldRef Name='Role' />
                                                                <Value Type='Text'>" + role + @"</Value>
                                                            </Contains>
                                                        </Or>
                                                        <Eq>
                                                            <FieldRef Name='FormName' />
                                                            <Value Type='Text'>" + formName + @"</Value>
                                                        </Eq>
                                                    </And>
                                                    <Eq>
                                                        <FieldRef Name='ApplicationName' />
                                                        <Value Type='Text'>" + applicationName + @"</Value>
                                                    </Eq>
                                                </And>
                                                <Eq>
                                                    <FieldRef Name='Title' />
                                                    <Value Type='Text'>" + templateName + @"</Value>
                                                </Eq>
                                            </And>
                                        </Where>
                                    </Query>
                                </View>";
                    ListItemCollection items = emailTemplateList.GetItems(qry);
                    context.Load(items);
                    context.ExecuteQuery();
                    if (items.Count > 0)
                    {
                        ListItem emailListItem = null;
                        if (items.Count > 1)
                        {
                            emailListItem = items.FirstOrDefault(i => Convert.ToString(i["Role"]) != string.Empty);
                        }
                        else
                        {
                            emailListItem = items.FirstOrDefault();
                        }
                        emailTemplate["Subject"] = Convert.ToString(emailListItem["Subject"]);
                        emailTemplate["Body"] = Convert.ToString(emailListItem["Body"]);
                        if (listItemDetails != null || customValues != null)
                        {
                            ListItemDetail listDetail = listItemDetails[0];
                            ListItem item = listDetail.ListItemObject;
                            string listName = listDetail.ListName;
                            if (item == null)
                            {
                                List spList = appWeb.Lists.GetByTitle(listName);
                                item = spList.GetItemById(listDetail.ItemId);
                                appContext.Load(item);
                                appContext.Load(spList, p => p.DefaultDisplayFormUrl);
                                appContext.ExecuteQuery();
                                listDetail.ListItemObject = item;
                                customValues["ItemLink"] = "#URL" + spList.DefaultDisplayFormUrl + "?ID=" + listDetail.ItemId;
                                customValues["ItemLinkClickHere"] = "<a href='#URL" + spList.DefaultDisplayFormUrl + "?ID=" + listDetail.ItemId + "' >Click Here</a>";
                            }

                            emailTemplate = this.CreateEmailBody(appContext, appWeb, emailTemplate, listItemDetails, customValues);
                        }
                    }
                }
            }
            return emailTemplate;
        }

        /// <summary>
        /// Saves the mail.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="tmplName">Name of the TMPL.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="from">The From.</param>
        /// <param name="to">The To.</param>
        /// <param name="cc">The cc.</param>
        /// <param name="isRepeat">if set to <c>true</c> [is repeat].</param>
        /// <param name="files">The files.</param>
        /// <param name="bcc">The BCC.</param>
        /// <returns>
        /// true or false
        /// </returns>
        public bool SendMail(string applicationName, string formName, string tmplName, string subject, string body, string from, string to, string cc, bool isRepeat, List<FileDetails> files = null, string bcc = "") //, string type)
        {
            bool isMailSaved = false;
            if (!string.IsNullOrEmpty(applicationName) && !string.IsNullOrEmpty(formName) && !string.IsNullOrEmpty(tmplName) && !string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(body) && !string.IsNullOrEmpty(from) && (!string.IsNullOrEmpty(to) || !string.IsNullOrEmpty(cc) || !string.IsNullOrEmpty(bcc)))
            {
                if (!string.IsNullOrEmpty(to))
                {
                    string[] tolst = to.Trim(',').Split(',');
                    to = string.Join(",", tolst.Distinct().ToArray());
                }
                if (!string.IsNullOrEmpty(cc))
                {
                    string[] cclst = cc.Trim(',').Split(',');
                    cc = string.Join(",", cclst.Distinct().ToArray());
                }
                if (!string.IsNullOrEmpty(bcc))
                {
                    string[] bcclst = bcc.Trim(',').Split(',');
                    bcc = string.Join(",", bcclst.Distinct().ToArray());
                }

                //BELDataAccessLayer helper = new BELDataAccessLayer();
                // string siteURL = BELDataAccessLayer.Instance.GetConfigVariable(SiteURLs.ROOTSITEURL);
                string siteURL = BELDataAccessLayer.Instance.GetSiteURL(SiteURLs.ROOTSITEURL);
                using (ClientContext context = BELDataAccessLayer.Instance.CreateClientContext(siteURL))
                {
                    if (!string.IsNullOrEmpty(to))
                    {
                        Web web = BELDataAccessLayer.Instance.CreateWeb(context);
                        List emailNotificationList = web.Lists.GetByTitle(ListNames.EmailNotification);
                        ListItem item = emailNotificationList.AddItem(new ListItemCreationInformation());
                        item["From"] = from.Trim(',');
                        item.Update();

                        item["To"] = to.Trim(',');
                        item.Update();

                        if (!string.IsNullOrEmpty(cc))
                        {
                            item["CC"] = cc.Trim(',');
                            item.Update();
                        }
                        if (!string.IsNullOrEmpty(bcc))
                        {
                            item["BCC"] = bcc.Trim(',');
                            item.Update();
                        }
                        item["Title"] = tmplName;
                        item["ApplicationName"] = applicationName;
                        item["FormName"] = formName;
                        item["Subject"] = subject;
                        item["Body"] = body;
                        item["IsRepeat"] = isRepeat;
                        ////item["Type"] = type;
                        item.Update();
                        context.Load(item);
                        context.ExecuteQuery();
                        isMailSaved = true;
                        if (files != null && files.Count > 0)
                        {
                            isMailSaved = BELDataAccessLayer.Instance.SaveAttachment(context, files, ref item);
                        }
                    }
                }
            }
            return isMailSaved;
        }

        /// <summary>
        /// Prepares the mail body.
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <param name="subjectKeywords">The subject keywords.</param>
        /// <param name="bodyKeywords">The body keywords.</param>
        /// <returns>
        /// Prepared Mail Subject and Body Dictionary
        /// </returns>
        public Dictionary<string, string> PrepareMailBody(Dictionary<string, string> mail, Dictionary<string, string> subjectKeywords, Dictionary<string, string> bodyKeywords)
        {
            if (mail != null && subjectKeywords != null && bodyKeywords != null)
            {
                foreach (KeyValuePair<string, string> subjectKeyVal in subjectKeywords)
                {
                    mail["Subject"] = mail["Subject"].Replace("[[" + subjectKeyVal.Key + "]]", subjectKeyVal.Value);
                }
                foreach (KeyValuePair<string, string> bodyKeyVal in bodyKeywords)
                {
                    mail["Body"] = mail["Body"].Replace("[[" + bodyKeyVal.Key + "]]", bodyKeyVal.Value);
                }
            }
            return mail;
        }

        /// <summary>
        /// Generates the reference no.
        /// </summary>
        /// <param name="appName">Name of the application.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <returns>Reference Number</returns>
        private string GenerateMailReferenceNo(string appName, string itemId)
        {
            DateTime dateTime = DateTime.Now;
            return appName + "-" + (dateTime.Month < 4 ? dateTime.Year - 1 : dateTime.Year) + "-" + (dateTime.Month >= 4 ? dateTime.Year + 1 : dateTime.Year) + "-" + dateTime.ToString("ddMMyyyy") + itemId;
        }

        /// <summary>
        /// Creates the email body.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="emailBody">The email body.</param>
        /// <param name="listItemDetails">The list item details.</param>
        /// <param name="customValues">The custom values.</param>
        /// <returns>
        /// Email Subject and Body
        /// </returns>
        public Dictionary<string, string> CreateEmailBody(ClientContext context, Web web, Dictionary<string, string> emailBody, List<ListItemDetail> listItemDetails, Dictionary<string, string> customValues)
        {
            Dictionary<string, string> emailBodyWithCustomData = new Dictionary<string, string>();
            Dictionary<string, string> emailBodyWithAllData = new Dictionary<string, string>();
            if (context != null && web != null && emailBody != null)
            {
                if (customValues != null && customValues.Count > 0)
                {
                    foreach (KeyValuePair<string, string> strEmail in emailBody)
                    {
                        Regex r = new Regex(@"\[\[Custom:(.+?)\]\]");
                        string preparedEmail = HttpUtility.HtmlDecode(strEmail.Value);
                        MatchCollection mc = r.Matches(preparedEmail);
                        foreach (Match match in mc)
                        {
                            string fieldName = match.Groups[1].Value;
                            try
                            {
                                Logger.Info("Email Body Replacement Listname : Custom , FieldName : " + fieldName);
                                preparedEmail = preparedEmail.Replace("[[Custom:" + fieldName + "]]", this.GetFieldValueString(customValues[fieldName]));
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("Error While Replace parameter, Error Message: " + ex.Message + " Stack trace :  " + ex.StackTrace);
                            }
                        }
                        emailBodyWithCustomData[strEmail.Key] = preparedEmail;
                    }
                    ////emailBody = emailBodyWithCustomData;
                    ////foreach (KeyValuePair<string, string> customField in customValues)
                    ////{
                    ////    emailBody["Subject"] = emailBody["Subject"].Replace(customField.Key, customField.Value);
                    ////    emailBody["Body"] = emailBody["Body"].Replace(customField.Key, customField.Value);
                    ////}
                }

                if (emailBodyWithCustomData == null || emailBodyWithCustomData.Count == 0)
                {
                    emailBodyWithCustomData = emailBody;
                }
                if (listItemDetails != null && listItemDetails.Count > 0)
                {
                    for (int i = 0; i < listItemDetails.Count; i++)
                    {
                        ListItemDetail listDetail = listItemDetails[i];
                        ListItem item = listDetail.ListItemObject;
                        string listName = listDetail.ListName;
                        if (item == null)
                        {
                            List spList = web.Lists.GetByTitle(listName);
                            item = spList.GetItemById(listDetail.ItemId);
                            context.Load(item);
                            context.ExecuteQuery();
                        }
                        foreach (KeyValuePair<string, string> strEmail in emailBodyWithCustomData)
                        {
                            Regex r = new Regex(@"\[\[" + listName + @":(.+?)\]\]");
                            string preparedEmail = HttpUtility.HtmlDecode(strEmail.Value);
                            MatchCollection mc = r.Matches(preparedEmail);
                            foreach (Match match in mc)
                            {
                                string fieldName = match.Groups[1].Value;
                                try
                                {
                                    Logger.Info("Email Body Replacement Listname : " + listName + " , FieldName : " + fieldName);
                                    preparedEmail = preparedEmail.Replace("[[" + listName + ":" + fieldName + "]]", this.GetFieldValueString(item[fieldName]));
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error("Error While Replace parameter, Error Message: " + ex.Message + " Stack trace :  " + ex.StackTrace);
                                }
                            }
                            emailBodyWithAllData[strEmail.Key] = preparedEmail;
                        }
                    }
                }
            }
            if (emailBodyWithAllData.Count == 0)
            {
                emailBodyWithAllData = emailBodyWithCustomData;
            }
            return emailBodyWithAllData;
        }

        /// <summary>
        /// Gets the field value string.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        /// field Value in string
        /// </returns>
        private string GetFieldValueString(object field)
        {
            string strValue = string.Empty;
            if (field != null)
            {
                switch (field.GetType().Name)
                {
                    case "FieldUserValue":
                        strValue = (field as FieldUserValue).LookupValue.ToString();
                        break;
                    case "FieldUserValue[]":
                        FieldUserValue[] users = field as FieldUserValue[];
                        foreach (FieldUserValue user in users)
                        {
                            strValue = strValue.Trim(',') + "," + user.LookupValue;
                        }
                        strValue = strValue.Trim(',');
                        break;
                    case "DateTime":
                        strValue = Convert.ToDateTime(field).ToString("dd/MM/yyyy");
                        break;
                    case "FieldLookupValue":
                        string format = "yyyy-MM-ddThh:mm:ssZ";
                        DateTime dateTime;
                        if (DateTime.TryParseExact((field as FieldLookupValue).LookupValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            strValue = dateTime.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            if (field != null)
                            {
                                var tempfield = field;
                                strValue = ((FieldLookupValue)tempfield).LookupValue.ToString();
                            }
                        }
                        break;
                    case "Boolean":
                        strValue = Convert.ToBoolean(field) ? "Yes" : "No";
                        break;
                    default:
                        strValue = field.ToString();
                        break;
                }
            }
            return strValue;
        }

        /// <summary>
        /// Sends the mail now.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="toUsers">To users.</param>
        /// <param name="ccUsers">The cc users.</param>
        /// <param name="bccUsers">The BCC users.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="fromUser">From user.</param>
        /// <param name="attachments">The attachments.</param>
        /// <returns>
        /// true or false
        /// </returns>
        public bool SendMailNow(ClientContext context, Web web, List<string> toUsers, List<string> ccUsers, List<string> bccUsers, string subject, string body, string fromUser, Microsoft.SharePoint.Client.AttachmentCollection attachments)
        {
            bool isSent = false;
            try
            {
                //// BELDataAccessLayer helper = new BELDataAccessLayer();
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
                string emailUserName = BELDataAccessLayer.Instance.GetConfigVariable("EmailUserName");
                string emailPassword = BELDataAccessLayer.Instance.GetConfigVariable("EmailPassword");
                string emailExchangeURL = BELDataAccessLayer.Instance.GetConfigVariable("EmailExchangeURL");
                service.Credentials = new WebCredentials(emailUserName, emailPassword);
                service.Url = new Uri(emailExchangeURL);
                service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, fromUser);
                EmailMessage msg = new EmailMessage(service);
                ////msg.From = new EmailAddress(fromUser);
                ////msg.Sender = new EmailAddress(fromUser);
                if (toUsers != null)
                {
                    foreach (string to in toUsers)
                    {
                        if (!string.IsNullOrEmpty(to))
                        {
                            msg.ToRecipients.Add(new EmailAddress(to));
                        }
                    }
                }
                if (ccUsers != null)
                {
                    foreach (string cc in ccUsers)
                    {
                        if (!string.IsNullOrEmpty(cc))
                        {
                            msg.CcRecipients.Add(new EmailAddress(cc));
                        }
                    }
                }
                if (bccUsers != null)
                {
                    foreach (string bcc in bccUsers)
                    {
                        if (!string.IsNullOrEmpty(bcc))
                        {
                            msg.BccRecipients.Add(new EmailAddress(bcc));
                        }
                    }
                }
                msg.Subject = subject;
                msg.Body = new MessageBody(BodyType.HTML, HttpUtility.HtmlDecode(body));

                if (attachments != null)
                {
                    foreach (Microsoft.SharePoint.Client.Attachment file in attachments)
                    {
                        byte[] bytes = BELDataAccessLayer.Instance.GetFileBytesByUrl(context, file.ServerRelativeUrl);
                        msg.Attachments.AddFileAttachment(file.FileName, bytes);
                    }
                }
                msg.SendAndSaveCopy();
                isSent = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                isSent = false;
            }
            return isSent;
        }

        /// <summary>
        /// Sends the mail now.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="toUsers">To users.</param>
        /// <param name="ccUsers">The cc users.</param>
        /// <param name="bccUsers">The BCC users.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="fromUser">From user.</param>
        /// <param name="attachments">The attachments.</param>
        /// <returns> is sent</returns>
        public bool SendMailWithByteNow(ClientContext context, Web web, List<string> toUsers, List<string> ccUsers, List<string> bccUsers, string subject, string body, string fromUser, Dictionary<string, byte[]> attachments)
        {
            bool isSent = false;
            try
            {
                ////BELDataAccessLayer helper = new BELDataAccessLayer();
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
                string emailUserName = BELDataAccessLayer.Instance.GetConfigVariable("EmailUserName");
                string emailPassword = BELDataAccessLayer.Instance.GetConfigVariable("EmailPassword");
                string emailExchangeURL = BELDataAccessLayer.Instance.GetConfigVariable("EmailExchangeURL");
                service.Credentials = new WebCredentials(emailUserName, emailPassword);
                service.Url = new Uri(emailExchangeURL);
                service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, fromUser);
                EmailMessage msg = new EmailMessage(service);
                ////msg.From = new EmailAddress(fromUser);
                ////msg.Sender = new EmailAddress(fromUser);
                if (toUsers != null)
                {
                    foreach (string to in toUsers)
                    {
                        if (!string.IsNullOrEmpty(to))
                        {
                            msg.ToRecipients.Add(new EmailAddress(to));
                        }
                    }
                }
                if (ccUsers != null)
                {
                    foreach (string cc in ccUsers)
                    {
                        if (!string.IsNullOrEmpty(cc))
                        {
                            msg.CcRecipients.Add(new EmailAddress(cc));
                        }
                    }
                }
                if (bccUsers != null)
                {
                    foreach (string bcc in bccUsers)
                    {
                        if (!string.IsNullOrEmpty(bcc))
                        {
                            msg.BccRecipients.Add(new EmailAddress(bcc));
                        }
                    }
                }
                msg.Subject = subject;
                msg.Body = new MessageBody(BodyType.HTML, HttpUtility.HtmlDecode(body));

                if (attachments != null && attachments.Count > 0)
                {
                    foreach (KeyValuePair<string, byte[]> file in attachments)
                    {
                        msg.Attachments.AddFileAttachment(file.Key, file.Value);
                    }
                }
                msg.SendAndSaveCopy();
                isSent = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                isSent = false;
            }
            return isSent;
        }
    }
}