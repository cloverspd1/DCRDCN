using BEL.DataAccessLayer;
using BEL.CommonDataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Data;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.UserProfiles;

namespace BEL.ReminderAndEscalationJob
{
    class Program
    {
        /// <summary>
        /// Job Main Method
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
                    List configList = web.Lists.GetByTitle(ListNames.APPLICATIONESCALATIONAUTOAPPROVALCONFIGURATION);
                    CamlQuery configCaml = new CamlQuery();
                    configCaml.ViewXml = @"<View>
                                    <Query>
                                        <Where>
                                            <Eq>
                                               <FieldRef Name='IsActive' />
                                                <Value Type='Boolean'>1</Value>
                                            </Eq>
                                        </Where>
                                    </Query>
                                </View>";
                    ListItemCollection items = configList.GetItems(configCaml);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();


                    if (items != null)
                    {
                        //BELDataAccessLayer helper = new BELDataAccessLayer();
                        string site = BELDataAccessLayer.Instance.GetConfigVariable("RootURL");
                        string applicationName = string.Empty;
                        string formName = string.Empty;
                        foreach (ListItem item in items)
                        {
                            if (item["ApplicationName"] != null)
                            {
                                applicationName = (item["ApplicationName"] as TaxonomyFieldValue).Label;
                            }
                            if (item["FormName"] != null)
                            {
                                formName = (item["FormName"] as TaxonomyFieldValue).Label;
                            }
                            string mainListName = Convert.ToString(item["MainListName"]);
                            string approverMatrixListName = Convert.ToString(item["ApproverMatrixListName"]);
                            string activityLogListName = Convert.ToString(item["ActivityLogListName"]);
                            string siteCollectionURL = site + Convert.ToString(item["SiteCollectionName"]);
                            string actionStatus = Convert.ToString(((Microsoft.SharePoint.Client.FieldLookupValue)(item["Status_x003a_Status"])).LookupValue);
                            using (ClientContext siteContext = BELDataAccessLayer.Instance.CreateClientContext(siteCollectionURL))
                            {
                                DataTable tblEscalation = GetEscalationTable();
                                Web siteWeb = BELDataAccessLayer.Instance.CreateWeb(siteContext);
                                switch (Convert.ToInt32(actionStatus))
                                {
                                    case 1: //Escalation only
                                        tblEscalation = EsclateRequests(siteContext, siteWeb, mainListName, approverMatrixListName, activityLogListName, tblEscalation, Convert.ToInt32(actionStatus));
                                        break;
                                    case 2: //Reminder only
                                        tblEscalation = EsclateRequests(siteContext, siteWeb, mainListName, approverMatrixListName, activityLogListName, tblEscalation, Convert.ToInt32(actionStatus));
                                        break;
                                    case 3: //Escalation and Reminder
                                        tblEscalation = EsclateRequests(siteContext, siteWeb, mainListName, approverMatrixListName, activityLogListName, tblEscalation, 1);
                                        tblEscalation = EsclateRequests(siteContext, siteWeb, mainListName, approverMatrixListName, activityLogListName, tblEscalation, 2);
                                        break;
                                    default:
                                        break;
                                }
                                if (tblEscalation != null && tblEscalation.Rows.Count != 0)
                                {
                                    // SendEscalationEmail(tblEscalation, siteContext, siteWeb);
                                    SendReminderEmail(tblEscalation, clientContext, web);
                                }
                            }

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
                Logger.Error(ex);
            }

        }

        /// <summary>
        /// Esclates the requests.
        /// </summary>
        /// <param name="siteContext">The site context.</param>
        /// <param name="siteWeb">The site web.</param>
        /// <param name="mainListName">Name of the main list.</param>
        /// <param name="approverMatrixListName">Name of the approver matrix list.</param>
        /// <param name="activityLogListName">Name of the activity log list.</param>
        public static DataTable EsclateRequests(ClientContext siteContext, Web siteWeb, string mainListName, string approverMatrixListName, string activityLogListName, DataTable tblEscalation, int actionStatus)
        {
            List approverList = siteWeb.Lists.GetByTitle(approverMatrixListName);
            List mainList = siteWeb.Lists.GetByTitle(mainListName);
            List activityLogList = siteWeb.Lists.GetByTitle(activityLogListName); // Check why we are getting this. 
            CamlQuery query = new CamlQuery();
            string qry = string.Empty;
            //DateTime? yesterDayDate = null;
            string yesterDayDate = string.Empty;
            switch (Convert.ToInt32(actionStatus))
            {
                case 1: //Escalation only
                    //yesterDayDate = DateTime.Now.AddDays(-3);
                    yesterDayDate = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");
                    //yesterDayDate = DateTime.Now.AddDays(-3).ToString("yyyy-MM-ddTHH:mm:ssZ");
                    //                    qry = @"<And>
                    //                                    <IsNotNull>
                    //                                        <FieldRef Name='Approver' />
                    //                                    </IsNotNull>
                    //                            <And>
                    //                                <Eq>
                    //                                    <FieldRef Name='DueDate' />
                    //                                    <Value Type='DateTime' IncludeTimeValue='FALSE'>" + yesterDayDate + @"</Value>
                    //                                </Eq>
                    //                                <And>
                    //                                    <Eq>
                    //                                        <FieldRef Name='Status' />
                    //                                        <Value Type='Text'>" + ApproverStatus.PENDING + @"</Value>
                    //                                    </Eq>
                    //                                    <Eq>
                    //                                        <FieldRef Name='IsEscalate' />
                    //                                        <Value Type='Boolean'>1</Value>
                    //                                    </Eq>
                    //                                </And>
                    //                        </And></And>";
                    break;
                case 2: //Reminder only
                    //yesterDayDate = DateTime.Now.AddDays(-1);
                    yesterDayDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    //yesterDayDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ssZ");
                    //                    qry = @"<And>
                    //                                    <IsNotNull>
                    //                                        <FieldRef Name='Approver' />
                    //                                    </IsNotNull>
                    //                                <And>
                    //                                <Eq>
                    //                                    <FieldRef Name='DueDate' />
                    //                                    <Value Type='DateTime' IncludeTimeValue='FALSE'>" + yesterDayDate + @"</Value>
                    //                                </Eq>
                    //                                <And>
                    //                                    <Eq>
                    //                                        <FieldRef Name='Status' />
                    //                                        <Value Type='Text'>" + ApproverStatus.PENDING + @"</Value>
                    //                                    </Eq>
                    //                                    <Eq>
                    //                                        <FieldRef Name='IsReminder' />
                    //                                        <Value Type='Boolean'>1</Value>
                    //                                    </Eq>
                    //                                </And>
                    //                        </And></And>";
                    break;
                default:
                    break;
            }

            //            query.ViewXml = @"<View>
            //                                    <Query>
            //                                        <Where>
            //                                            " + qry + @"
            //                                        </Where>
            //                                    </Query>
            //                                </View>";
            ListItemCollectionPosition position = null;
            var page = 1;
            do
            {
                query.ViewXml = @"<View>
                                        <Query>
                                               <OrderBy>
                                                       <FieldRef Name='ID' Ascending='False' />
                                               </OrderBy>
                                        </Query>
                                        <RowLimit>5000</RowLimit>
                                 </View>";

                query.ListItemCollectionPosition = position;

                ListItemCollection approverDetails = approverList.GetItems(query);
                siteContext.Load(approverDetails);
                siteContext.ExecuteQuery();

                position = approverDetails.ListItemCollectionPosition;

                if (approverDetails != null)
                {
                    for (int i = 0; i < approverDetails.Count; i++)
                    {
                        string status = Convert.ToString(approverDetails[i]["Status"]);
                        //DateTime? date = null;
                        string date = string.Empty;

                        if (status == "Pending" && approverDetails[i]["Approver"] != null && approverDetails[i]["DueDate"] != null)
                        {
                            date = Convert.ToDateTime(approverDetails[i]["DueDate"]).ToString("yyyy-MM-dd");
                            //date = Convert.ToDateTime(approverDetails[i]["DueDate"]);
                            //if (actionStatus == 1 && Convert.ToBoolean(approverDetails[i]["IsEscalate"]) && date.HasValue && yesterDayDate.HasValue && date == yesterDayDate) //Escalation only
                            if (actionStatus == 1 && Convert.ToBoolean(approverDetails[i]["IsEscalate"]) && !string.IsNullOrWhiteSpace(date) && !string.IsNullOrWhiteSpace(yesterDayDate) && date == yesterDayDate) //Escalation only
                            {
                                string esclationTo = string.Empty;   ////not required
                                tblEscalation = EsclateRequest(siteContext, siteWeb, approverDetails[i], mainListName, approverMatrixListName, tblEscalation, ref esclationTo, actionStatus);
                            }
                            if (actionStatus == 2 && Convert.ToBoolean(approverDetails[i]["IsReminder"]) && !string.IsNullOrWhiteSpace(date) && !string.IsNullOrWhiteSpace(yesterDayDate) && date == yesterDayDate)//Reminder only
                            {
                                string esclationTo = string.Empty;   ////not required
                                tblEscalation = EsclateRequest(siteContext, siteWeb, approverDetails[i], mainListName, approverMatrixListName, tblEscalation, ref esclationTo, actionStatus);
                            }
                        }

                        //string esclationTo = string.Empty;   ////not required
                        //tblEscalation = EsclateRequest(siteContext, siteWeb, approverDetails[i], mainListName, approverMatrixListName, tblEscalation, ref esclationTo, actionStatus);
                    }
                    page++;
                }
            } while (position != null);

            return tblEscalation;
        }
        /// <summary>
        /// Esclates the request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="item">The item.</param>
        /// <param name="mainListName">Name of the main list.</param>
        /// <param name="approverListName">Name of the approver list.</param>
        /// <returns>to users</returns>
        public static DataTable EsclateRequest(ClientContext context, Web web, ListItem item, string mainListName, string approverListName, DataTable tblEscalation, ref string toUser, int actionStatus)
        {
            try
            {
                DataRow dr = tblEscalation.NewRow();
                string userIDs = BELDataAccessLayer.GetEmailsFromPersonField(context, web, item["Approver"] as FieldUserValue[]);

                if (Convert.ToInt32(actionStatus) == 1)   //For Escalation Only
                {
                    string to = string.Empty;
                    foreach (string id in userIDs.Split(','))
                    {
                        UserDetails userDetail = BELDataAccessLayer.Instance.GetUserInformation(context, web, id);
                        if (userDetail != null)
                        {
                            to += "," + GetManagerEmail(context, web, userDetail.ReportingManager);
                            dr["UserFullName"] += userDetail.FullName + ", ";
                            dr["UserEmail"] += userDetail.UserEmail + ", ";
                            dr["CCEmail"] += userDetail.UserEmail + ",";
                        }
                    }
                    dr["ToEmail"] = to.Trim(',');
                    dr["UserFullName"] = dr["UserFullName"].ToString().Trim(',');

                }
                else if (Convert.ToInt32(actionStatus) == 2) //For Reminder Only
                {
                    dr["UserEmail"] = dr["ToEmail"] = BELDataAccessLayer.GetEmailUsingUserID(context, web, userIDs);
                    dr["PendingWith"] = dr["UserFullName"] = BELDataAccessLayer.GetNameUsingUserID(context, web, userIDs);
                }
                EmailHelper eHelper = new EmailHelper();/// Check if we can remove this. 
                //List<ListItemDetail> listDetails = new List<ListItemDetail>();
                int reqId = (item["RequestID"] as FieldLookupValue).LookupId;
                List mainList = web.Lists.GetByTitle(mainListName);
                ListItem mainItem = mainList.GetItemById(reqId);
                context.Load(mainItem);
                context.Load(mainList, m => m.DefaultDisplayFormUrl);
                context.ExecuteQuery();


                // dr["CCEmail"] = toUser;
                dr["EscOrReminder"] = actionStatus;
                dr["ApplicationName"] = Convert.ToString(item["ApplicationName"]);
                dr["FormName"] = Convert.ToString(item["FormName"]);
                // dr["ReferenceNo"] = (mainItem["DCRNo"] != null ? Convert.ToString(mainItem["DCRNo"]) : string.Empty);
                dr["RequestDate"] = (mainItem["RequestDate"] != null ? Convert.ToDateTime(mainItem["RequestDate"]).ToString("dd-MM-yyyy") : string.Empty);
                dr["AssignDate"] = (item["AssignDate"] != null ? Convert.ToDateTime(item["AssignDate"]).ToLocalTime().ToString("dd-MM-yyyy") : string.Empty);
                //dr["BusinessUnit"] = (mainItem["BusinessUnit"] != null ? Convert.ToString(mainItem["BusinessUnit"]) : string.Empty);
                // dr["ProductName"] = (mainItem["ProductName"] != null ? Convert.ToString(mainItem["ProductName"]) : string.Empty);
                string link = string.Empty;
                string referenceNo = string.Empty;
                string emailSignature = string.Empty;
                link = "#URL" + mainList.DefaultDisplayFormUrl + "?ID=" + mainItem["ID"];
                dr["ListName"] = mainListName;
                dr["ID"] = mainItem.Id;
                if (mainListName == "DCR")
                {
                    referenceNo = (mainItem["DCRNo"] != null ? Convert.ToString(mainItem["DCRNo"]) : string.Empty);
                    emailSignature = "DCRDCN Team";
                }
                else if (mainListName == "DCN")
                {
                    referenceNo = (mainItem["DCNNo"] != null ? Convert.ToString(mainItem["DCNNo"]) : string.Empty);
                    emailSignature = "DCRDCN Team";
                }
                else if (mainListName == "NPD")
                {
                    referenceNo = (mainItem["ProjectNo"] != null ? Convert.ToString(mainItem["ProjectNo"]) : string.Empty);
                    emailSignature = "NPD Team";
                }
                else if (mainListName == "Feedbacks")
                {
                    referenceNo = (mainItem["FeedbackNo"] != null ? Convert.ToString(mainItem["FeedbackNo"]) : string.Empty);
                    emailSignature = "CC Department";
                }
                else if (mainListName == "ExistingArtwork")
                {
                    referenceNo = (mainItem["ReferenceNo"] != null ? Convert.ToString(mainItem["ReferenceNo"]) : string.Empty);
                    emailSignature = "Artwork Team";
                }
                else if (mainListName == "NewArtwork")
                {
                    referenceNo = (mainItem["ReferenceNo"] != null ? Convert.ToString(mainItem["ReferenceNo"]) : string.Empty);
                    emailSignature = "Artwork Team";
                }
                else if (mainListName == "ItemCodePreProcess")
                {
                    referenceNo = (mainItem["Title"] != null ? Convert.ToString(mainItem["Title"]) : string.Empty);
                    emailSignature = "Item Code Document Management S​​ystem (SharePoint​)​​";
                }
                dr["Link"] = link;
                dr["ReferenceNo"] = referenceNo;
                dr["EmailSignature"] = emailSignature;
                dr["ActionStatus"] = actionStatus;
                dr["DueDate"] = (item["DueDate"] != null ? Convert.ToDateTime(item["DueDate"]).ToLocalTime().ToString("dd-MM-yyyy") : string.Empty);

                if (item["DueDate"] != null)
                {
                    dr["PendingSince"] = GetPendingDays(Convert.ToDateTime(item["DueDate"]));
                }
                else
                {
                    dr["PendingSince"] = "0";
                }
                string workflowstatus = Convert.ToString(mainItem["WorkflowStatus"]);
                if (!workflowstatus.Equals("Draft"))
                {
                    tblEscalation.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Request Escalation: " + ex.Message);
                Console.Write(ex.StackTrace + "==>" + ex.Message);
                Logger.Error("Error Request Escalation: " + ex.Message);
            }
            return tblEscalation;
        }

        public static DataTable GetEscalationTable()
        {
            DataTable tblEscalation = new DataTable();
            tblEscalation.Columns.Add(new DataColumn("UserFullName", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("UserEmail", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("FromEmail", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("ToEmail", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("CCEmail", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("Subject", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("Body", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("ApplicationName", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("FormName", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("ReferenceNo", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("RequestDate", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("AssignDate", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("Link", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("PendingSince", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("DueDate", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("ActionStatus", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("ID", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("ListName", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("EscOrReminder", typeof(int)));
            tblEscalation.Columns.Add(new DataColumn("BusinessUnit", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("ProductName", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("PendingWith", typeof(string)));
            tblEscalation.Columns.Add(new DataColumn("EmailSignature", typeof(string)));

            return tblEscalation;
        }

        private static string GetPendingDays(DateTime dueDate)
        {
            int totaldays = Convert.ToInt32((DateTime.Now.Date - dueDate).TotalDays);
            return Convert.ToString(totaldays);
        }

        public static void SendEscalationEmail(DataTable tblEscalation, ClientContext clientContext, Web web)
        {
            //var allUsers = from row in tblEscalation.AsEnumerable()
            //                     select row.Field<string>("UserEmail");



            //foreach (var userEmail in allUsers)
            //{
            //DataRow[] filteredDataTable = tblEscalation.Select(@"UserEmail = '" + userEmail + "'");

            string appName = string.Empty;
            string formName = string.Empty;
            string template = string.Empty;

            //foreach (DataRow dr in filteredDataTable)
            foreach (DataRow dr in tblEscalation.Rows)
            {
                appName = dr["ApplicationName"].ToString();
                formName = dr["FormName"].ToString();
                EmailHelper eHelper = new EmailHelper();
                template = dr["ActionStatus"].ToString();
                Dictionary<string, string> custom = new Dictionary<string, string>();
                custom["DCRNo"] = dr["ReferenceNo"].ToString();
                custom["DCNNo"] = dr["ReferenceNo"].ToString();
                custom["Link"] = dr["Link"].ToString(); ;
                custom["AssignDate"] = dr["AssignDate"].ToString();
                custom["UserName"] = dr["UserFullName"].ToString();
                ListItemDetail listDetail = new ListItemDetail();
                listDetail.ItemId = Convert.ToInt32(dr["ID"].ToString());
                listDetail.ListName = dr["ListName"].ToString();
                List<ListItemDetail> listDetails = new List<ListItemDetail>();
                listDetails.Add(listDetail);
                Dictionary<string, string> email = new Dictionary<string, string>();
                switch (Convert.ToInt32(template))
                {
                    case 1: //Escalation only
                        email = eHelper.GetEmailBody(clientContext, web, EmailTemplateName.ESCLATIONMAILTEMPLATE, listDetails, custom, null, appName, formName);
                        eHelper.SendMail(appName, formName, EmailTemplateName.ESCLATIONMAILTEMPLATE, email["Subject"], email["Body"], BELDataAccessLayer.Instance.GetConfigVariable("FromEmailAddress"), dr["ToEmail"].ToString(), dr["CCEmail"].ToString(), false);
                        break;
                    case 2: //Reminder only
                        email = eHelper.GetEmailBody(clientContext, web, EmailTemplateName.REMINDEREMAILTEMPLATE, listDetails, custom, null, appName, formName);
                        eHelper.SendMail(appName, formName, EmailTemplateName.REMINDEREMAILTEMPLATE, email["Subject"], email["Body"], BELDataAccessLayer.Instance.GetConfigVariable("FromEmailAddress"), dr["ToEmail"].ToString(), dr["CCEmail"].ToString(), false);
                        break;
                    default:
                        break;
                }
            }
            // }
        }

        /// <summary>
        /// Prepare Email Body and Send to User
        /// </summary>
        /// <param name="tblEscalation">Data Table</param>
        /// <param name="clientContext">Client Context</param>
        /// <param name="web">Web Object</param>
        public static void SendReminderEmail(DataTable tblEscalation, ClientContext clientContext, Web web)
        {
            string templateName = string.Empty;
            string to = string.Empty;
            string cc = string.Empty;
            string username = string.Empty;
            string siteurl = string.Empty;
            for (int j = 1; j <= 2; j++)
            {
                templateName = j == 1 ? "Escalation" : "Reminder";

                var tempdistinctUsers = (from row in tblEscalation.AsEnumerable()
                                         where row.Field<int>("EscOrReminder") == j
                                         select row.Field<string>("UserEmail")).Distinct();

                var distinctUsers = tempdistinctUsers.SelectMany(l => l.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Distinct().ToList();

                foreach (var userEmail in distinctUsers)
                {

                    if (!string.IsNullOrEmpty(userEmail.Trim()))
                    {
                        DataRow[] filteredDataTable = tblEscalation.Select(@"UserEmail Like '%" + userEmail + "%' AND EscOrReminder = " + j);
                        to = j == 1 ? filteredDataTable[0]["ToEmail"].ToString() : userEmail;
                        cc = j == 1 ? userEmail : string.Empty;
                        //                        string htmlTbl = @"<p>Dear User,<p>
                        //                                    <p>Below Request is pending for your action. Kindly take corrective action to avoid any further reminders. 
                        //                                            <a href ='#URL" + BELDataAccessLayer.Instance.GetConfigVariable("DCRDCNSiteURL") + @"/'> Click Here</a> to View all pending artworks.            
                        //                                            <p>   
                        //                                                         
                        //                                    <table width='100%' cellspacing='2' cellpadding='2' border='2'>
                        //                                    <tr>
                        //                                    <th width='5%'>Sr No</th>
                        //                                    <th width='15%'>Type</th>
                        //                                    <th>Request No</th>
                        //                                    <th>Assigned Date</th>
                        //                                    <th>Due Date</th>
                        //                                    </tr>";
                        string htmlTbl;
                        if (j == 1)
                        {
                            htmlTbl = @"<p>Dear User,<p>
                                    <p>Below requests are pending to <b>#pendingusername.​</b>​ Kindly take appropriate action on pending requests.                                        
                                            <p>          
                                    <table width='100%' cellspacing='2' cellpadding='2' border='2'>
                                    <tr>
                                    <th width='5%'>Sr No</th>
                                    <th width='15%'>Type</th>
                                    <th>Reference No</th>
                                    <th>Assigned Date</th>
                                    <th>Due Date</th>
                                    </tr>";
                        }
                        else
                        {
                            htmlTbl = @"<p>Dear User,<p>
                                    <p>Below requests are pending for your action. Kindly take corrective action to avoid any further reminders.
                                    <a href ='#URL1'> Click Here</a> to view pending requests. 
                                            <p>          
                                    <table width='100%' cellspacing='2' cellpadding='2' border='2'>
                                    <tr>
                                    <th width='5%'>Sr No</th>
                                    <th width='15%'>Type</th>
                                    <th>Reference No</th>
                                    <th>Assigned Date</th>
                                    <th>Due Date</th>
                                    </tr>";
                        }
                        int i = 1;
                        string emailSignature = string.Empty;
                        foreach (DataRow dr in filteredDataTable)
                        {
                            try
                            {
                                username = Convert.ToString(dr["UserFullName"]);
                                emailSignature = Convert.ToString(dr["EmailSignature"]);
                                siteurl = Convert.ToString(dr["Link"]);
                                string frmName = dr["FormName"].ToString();
                                frmName = frmName.Replace("Form", string.Empty);
                                if (j == 1)
                                {
                                    htmlTbl += @"<tr>
                                        <td>" + i.ToString() + @"</td>
                                        <td>" + dr["ListName"] + @"</td> 
                                        <td>" + dr["ReferenceNo"] + @"</td>
                                        <td>" + dr["AssignDate"] + @"</td>
                                        <td>" + dr["DueDate"] + @"</td>
                                    </tr>";
                                }
                                else
                                {
                                    htmlTbl += @"<tr>
                                        <td>" + i.ToString() + @"</td>
                                        <td>" + dr["ListName"] + @"</td> 
                                        <td><a href='" + dr["link"] + "'>" + dr["ReferenceNo"] + @"</a></td>
                                        <td>" + dr["AssignDate"] + @"</td>
                                        <td>" + dr["DueDate"] + @"</td>
                                    </tr>";
                                }
                                i++;
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }
                        }
                        int index = siteurl.LastIndexOf("/Lists");
                        if (index > 0)
                        {
                            siteurl = siteurl.Substring(0, index);
                            siteurl = siteurl.Replace("#URL", BELDataAccessLayer.Instance.GetConfigVariable("RootURL"));
                        }
                        htmlTbl += "</table><p><i>This is an auto generated email, please do not reply.</i></p><p>Thanks & Regards,<br/>" + emailSignature + "</p>";
                        if (!String.IsNullOrWhiteSpace(username))
                        {
                            int tempindex = username.LastIndexOf(',');
                            if (tempindex > 0)
                            {
                                username = username.Remove(tempindex, 1);
                            }
                            tempindex = username.LastIndexOf(' ');
                            if (tempindex > 0)
                            {
                                username = username.Remove(tempindex, 1);
                            }
                            htmlTbl = htmlTbl.Replace("#pendingusername", username);
                        }
                        if (!String.IsNullOrWhiteSpace(siteurl))
                        {
                            htmlTbl = htmlTbl.Replace("#URL1", siteurl);
                        }
                        htmlTbl = htmlTbl.Replace("#URL", BELDataAccessLayer.Instance.GetConfigVariable("RootURL"));
                        htmlTbl = htmlTbl.Replace("Feedbacks", "Feedback");
                        htmlTbl = htmlTbl.Replace("https://bajajelect.sharepoint.com", "#URL");
                        EmailHelper eHelper = new EmailHelper();
                        ////eHelper.SendMail("All", "All", templateName + "Template", templateName + " - Requests pending for your action", htmlTbl, BELDataAccessLayer.Instance.GetConfigVariable("FromEmailAddress"), userEmail, string.Empty, false);
                        eHelper.SendMail("All", "All", templateName + "Template", templateName + " - Requests pending for your action", htmlTbl, BELDataAccessLayer.Instance.GetConfigVariable("FromEmailAddress"), to, cc, false);
                    }
                }
            }
        }
        public static string GetManagerEmail(ClientContext context, Web web, string loginName)
        {
            try
            {
                List<UserDetails> userInfoList = GlobalCachingProvider.Instance.GetItem(web.ServerRelativeUrl + "/" + ListNames.EmployeeMasterList, false) as List<UserDetails>;
                if (userInfoList != null && userInfoList.FirstOrDefault(p => p.LoginName == loginName) != null)
                {
                    return userInfoList.FirstOrDefault(p => p.LoginName == loginName).UserEmail;
                }
                else if (userInfoList != null)
                {
                    var peopleManager = new PeopleManager(context);
                    var userProfilesResult = new List<PersonProperties>();
                    var userProfile = peopleManager.GetPropertiesFor(loginName);
                    context.Load(userProfile);
                    context.ExecuteQuery();
                    UserDetails userObj = new UserDetails()
                    {
                        //UserId = usersResult.Where(p => p.LoginName == userProfile.AccountName).FirstOrDefault() != null ? usersResult.Where(p => p.LoginName == userProfile.AccountName).FirstOrDefault().Id.ToString() : string.Empty,
                        FullName = userProfile.Title,
                        UserEmail = userProfile.Email,
                        LoginName = userProfile.AccountName,
                        Department = userProfile.UserProfileProperties.ContainsKey("Department") ? Convert.ToString(userProfile.UserProfileProperties["Department"]) : string.Empty,
                        ReportingManager = userProfile.UserProfileProperties.ContainsKey("Manager") ? Convert.ToString(userProfile.UserProfileProperties["Manager"]) : string.Empty
                    };
                    userInfoList.Add(userObj);
                    GlobalCachingProvider.Instance.AddItem(web.ServerRelativeUrl + "/" + ListNames.EmployeeMasterList, userInfoList);
                    return userObj.UserEmail;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error While GetManagerEmail: " + ex.Message);
                Console.Write(ex.StackTrace + "==>" + ex.Message);
                Logger.Error(ex);
                return string.Empty;
            }
        }
    }
}
