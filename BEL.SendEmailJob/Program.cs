using BEL.DataAccessLayer;
using BEL.CommonDataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.Exchange.WebServices.Data;
using System.IO;
using System.Net.Mail;
using System.Web;

namespace BEL.SendEmailJob
{
    class Program
    {
        /// <summary>
        /// Send Email Job Main Method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                string rootURL = BELDataAccessLayer.Instance.GetSiteURL(SiteURLs.ROOTSITEURL);
                using (ClientContext clientContext = BELDataAccessLayer.Instance.CreateClientContext(rootURL))
                {
                    Web web = BELDataAccessLayer.Instance.CreateWeb(clientContext);
                    List emailTemplateList = web.Lists.GetByTitle(ListNames.EmailNotification);
                    CamlQuery qry = new CamlQuery();
                    qry.ViewXml = @"<View>
                                    <Query>
                                        <Where>                                               
                                                <Eq>
                                                    <FieldRef Name='IsSent' />
                                                    <Value Type='Boolean'>0</Value>
                                                </Eq>                                         
                                        </Where>
                                    </Query>
                                    <RowLimit>" + Convert.ToInt32(BELDataAccessLayer.Instance.GetConfigVariable("MailsPerSchedule")) + @"</RowLimit>
                                </View>";
                    ////<Where><And>
                    ////                            <Eq>
                    ////                                <FieldRef Name='IsSent' />
                    ////                                <Value Type='Boolean'>0</Value>
                    ////                            </Eq>
                    ////                            <Eq>
                    ////                                <FieldRef Name='IsSentFail' />
                    ////                                <Value Type='Boolean'>0</Value>
                    ////                            </Eq>
                    ////                        </And>
                    ////                    </Where>
                    ListItemCollection items = emailTemplateList.GetItems(qry);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    Logger.Info("Item Count" + items.Count);
                    if (items.Count > 0)
                    {
                        foreach (ListItem item in items)
                        {
                            string mode = BELDataAccessLayer.Instance.GetConfigVariable("EmailMode");
                            if (mode.ToLower().Equals("live"))
                            {
                                bool mailstatus = SendMailEWS(clientContext, item);
                                item["IsSent"] = mailstatus;
                                if (!mailstatus)
                                {
                                    item["IsSentFail"] = !mailstatus;
                                }
                               
                                Logger.Info("Item Count" + item["IsSent"]);
                            }
                            else
                            {
                                item["IsSent"] = SendMail(clientContext, item);
                            }
                            item.Update();
                            clientContext.ExecuteQuery();
                        }
                    }

                }
                Logger.Info("item  status " + ItemActionStatus.NEW);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error While Execute Main Method");
                Console.Write(ex.StackTrace + "==>" + ex.Message);
                Logger.Error("Error While Execute Main Method");
                Logger.Error(ex.Message);
            }
        }
        /// <summary>
        /// Sends the mail using EWS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static bool SendMailEWS(ClientContext context, ListItem item)
        {
            bool isSent = false;
            try
            {
                if (item != null)
                {
                    ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
                    string emailUserName = BELDataAccessLayer.Instance.GetConfigVariable("EmailUserName");
                    string emailPassword = BELDataAccessLayer.Instance.GetConfigVariable("EmailPassword");
                    string emailExchangeURL = BELDataAccessLayer.Instance.GetConfigVariable("EmailExchangeURL");
                    service.Credentials = new WebCredentials(emailUserName, emailPassword);
                    service.Url = new Uri(emailExchangeURL);
                    //service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, Convert.ToString(item["From"]));
                    EmailMessage msg = new EmailMessage(service);
                    string[] toUsers = Convert.ToString(item["To"]).Split(',');
                    foreach (string to in toUsers)
                    {
                        if (!string.IsNullOrEmpty(to))
                        {
                            msg.ToRecipients.Add(new EmailAddress(to.Trim()));
                        }
                        else
                        {
                            isSent = true;
                            return isSent;
                        }
                    }
                    string[] ccUsers = Convert.ToString(item["CC"]).Split(',');

                    //remove duplicate email from cc list.
                    ccUsers = ccUsers.Except(toUsers).ToArray();

                    foreach (string cc in ccUsers)
                    {
                        if (!string.IsNullOrEmpty(cc))
                        {
                            msg.CcRecipients.Add(new EmailAddress(cc.Trim()));
                        }
                    }
                    string[] bccUsers = Convert.ToString(item["BCC"]).Split(',');

                    foreach (string bcc in bccUsers)
                    {
                        if (!string.IsNullOrEmpty(bcc))
                        {
                            msg.BccRecipients.Add(new EmailAddress(bcc.Trim()));
                        }
                    }
                    msg.Subject = item["Subject"].ToString();
                    string htmlcontnet = HttpUtility.HtmlDecode(item["Body"].ToString());
                    htmlcontnet = htmlcontnet.Replace("#URL", BELDataAccessLayer.Instance.GetConfigVariable(SiteURLs.ROOTURL));
                    msg.Body = new MessageBody(BodyType.HTML, htmlcontnet);

                    if (Convert.ToString(item["Attachments"]) == "True")
                    {
                        context.Load(item.AttachmentFiles);
                        context.ExecuteQuery();
                        if (item.AttachmentFiles != null)
                        {
                            foreach (Microsoft.SharePoint.Client.Attachment file in item.AttachmentFiles)
                            {
                                byte[] bytes = BELDataAccessLayer.Instance.GetFileBytesByUrl(context, file.ServerRelativeUrl);
                                msg.Attachments.AddFileAttachment(file.FileName, bytes);
                            }
                        }
                    }
                    msg.SendAndSaveCopy();
                }
                isSent = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("From User : " + Convert.ToString(item["From"]));
                Console.Write(ex.StackTrace + "==>" + ex.Message);
                Logger.Info("From User : " + Convert.ToString(item["From"]));
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
                isSent = false;
            }
            return isSent;
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static bool SendMail(ClientContext context, ListItem item)
        {
            bool isSent = false;
            try
            {
                if (item != null)
                {
                    using (MailMessage msg = new MailMessage())
                    {
                        msg.From = new MailAddress(Convert.ToString(item["From"]));
                        msg.Sender = new MailAddress(Convert.ToString(item["From"]));
                        //msg.From = new MailAddress("belinfo@gmail.com");
                        //msg.Sender = new MailAddress("belinfo@gmail.com");
                        string[] toUsers = Convert.ToString(item["To"]).Split(',');
                        foreach (string to in toUsers)
                        {
                            if (!string.IsNullOrEmpty(to))
                            {
                                msg.To.Add(new MailAddress(to.Trim()));
                            }
                            else
                            {
                                isSent = true;
                                return isSent;
                            }
                        }
                        string[] ccUsers = Convert.ToString(item["CC"]).Split(',');
                        foreach (string cc in ccUsers)
                        {
                            if (!string.IsNullOrEmpty(cc))
                            {
                                msg.CC.Add(new MailAddress(cc.Trim()));
                            }
                        }
                        msg.Subject = item["Subject"].ToString();
                        msg.Body = HttpUtility.HtmlDecode(item["Body"].ToString());
                        //msg.Body = HttpUtility.UrlKeyValueDecode(item["Body"].ToString());
                        msg.IsBodyHtml = true;
                        if (Convert.ToString(item["Attachments"]) == "True")
                        {
                            context.Load(item.AttachmentFiles);
                            context.ExecuteQuery();
                            if (item.AttachmentFiles != null)
                            {
                                foreach (Microsoft.SharePoint.Client.Attachment file in item.AttachmentFiles)
                                {
                                    byte[] bytes = BELDataAccessLayer.Instance.GetFileBytesByUrl(context, file.ServerRelativeUrl);
                                    msg.Attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(bytes), file.FileName));
                                }
                            }
                        }
                        SmtpClient smtp = new SmtpClient();
                        try
                        {
                            smtp.Send(msg);
                        }
                        finally
                        {
                            if (smtp != null)
                            {
                                smtp.Dispose();
                            }
                        }
                    }
                }
                isSent = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error While SendMail through Gmail");
                Console.Write(ex.StackTrace + "==>" + ex.Message);
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
                isSent = false;
            }
            return isSent;
        }
    }
}
