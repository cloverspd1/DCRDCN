namespace BEL.DataAccessLayer
{
    using System;
    using BEL.CommonDataContract;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Reflection;
    using Microsoft.SharePoint.Client;
    using System.Net;
    using System.Configuration;
    using System.IO;
    using Microsoft.SharePoint.Client.Taxonomy;
    using Microsoft.SharePoint.Client.Utilities;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using System.Globalization;

    /// <summary>
    /// Application Status Helper
    /// </summary>
    public class AppApprovalMatrixHelper : BELDataAccessLayer
    {
        /// <summary>
        /// Child Items
        /// </summary>
        private const string ChildItems = "_Child_Items_";

        /// <summary>
        /// Object Type
        /// </summary>
        private const string ObjectType = "_ObjectType_";

        /// <summary>
        /// Gets the sla matrix.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <returns>list of SLA Matrix</returns>
        public List<SLAMatrix> GetSLAMatrix(string applicationName, string formName)
        {
            List<SLAMatrix> slamatrix = new List<SLAMatrix>();

            //BELDataAccessLayer helper = new BELDataAccessLayer();
            string siteURL = BELDataAccessLayer.Instance.GetConfigVariable("RootSiteURL");
            ClientContext context = BELDataAccessLayer.Instance.CreateClientContext(siteURL);
            Web web = BELDataAccessLayer.Instance.CreateWeb(context);
            context.Load(web);
            context.ExecuteQuery();
            List spList = web.Lists.GetByTitle(ListNames.SLAList);
            CamlQuery query = new CamlQuery();
            query.ViewXml = @"<View>
                                            <Query>
                                                <Where>
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
                                                     </Where>
                                            </Query>
                                            </View>";
            ListItemCollection items = spList.GetItems(query);
            context.Load(items);
            context.ExecuteQuery();
            if (items != null && items.Count != 0)
            {
                foreach (ListItem item in items)
                {
                    slamatrix.Add(new SLAMatrix()
                    {
                        ID = item.Id,
                        Title = item["Title"].ToString(),
                        ApplicationName = this.GetTaxonomyFieldValue(item["ApplicationName"] as TaxonomyFieldValue),
                        FormName = this.GetTaxonomyFieldValue(item["FormName"] as TaxonomyFieldValue),
                        SectionName = this.GetTaxonomyFieldValueCollection(item["SectionName"] as TaxonomyFieldValueCollection),
                        Levels = Convert.ToInt32(item["Levels"]),
                        Role = Convert.ToString(item["Role"]),
                        FillByRole = Convert.ToString(item["FillByRole"]),
                        Division = Convert.ToString(item["Division"]),
                        SubDivision = Convert.ToString(item["SubDivision"]),
                        Days = Convert.ToInt32(item["Days"]),
                        IsAutoApproval = (bool)item["IsAutoApproval"],
                        IsOptional = (bool)item["IsOptional"]
                    });
                }
            }

            return slamatrix;
        }

        /// <summary>
        /// Gets the taxonomy field value collection.
        /// </summary>
        /// <param name="terms">The terms.</param>
        /// <returns>string value</returns>
        public string GetTaxonomyFieldValueCollection(TaxonomyFieldValueCollection terms)
        {
            string value = string.Empty;
            if (terms != null)
            {
                foreach (TaxonomyFieldValue term in terms)
                {
                    value += Convert.ToString(term.Label) + ",";
                }
            }
            return value;
        }

        /// <summary>
        /// Gets the taxonomy field value collection.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>string value</returns>
        public string GetTaxonomyFieldValueCollection(Dictionary<string, object> dictionary)
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
            string strvalue = string.Empty;
            if (terms != null)
            {
                foreach (TaxonomyFieldValue term in terms)
                {
                    strvalue += Convert.ToString(term.Label) + ",";
                }
            }
            return strvalue;
        }

        /// <summary>
        /// Gets the taxonomy field value.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>string value</returns>
        public string GetTaxonomyFieldValue(TaxonomyFieldValue term)
        {
            string value = string.Empty;
            if (term != null)
            {
                value = Convert.ToString(term.Label);
            }
            return value;
        }

        /// <summary>
        /// Gets the taxonomy field value.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>string value</returns>
        public string GetTaxonomyFieldValue(Dictionary<string, object> dictionary)
        {
            if (dictionary != null)
            {
                if (!dictionary.ContainsKey(ObjectType) || !dictionary[ObjectType].Equals("SP.Taxonomy.TaxonomyFieldValue"))
                {
                    throw new InvalidOperationException("Dictionary value represents no TaxonomyFieldValue.");
                }
                if (dictionary.ContainsKey("Label"))
                {
                    return dictionary["Label"].ToString();
                }
            }
            return string.Empty;
        }

        #region "App Approval Matrix"
        /// <summary>
        /// Gets the approver matrix.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="role">The role.</param>
        /// <returns>List of Approvers Role filled by Current User Role for Create</returns>
        public List<ApplicationStatus> GetGlobalAppApproverMatrix(string applicationName, string formName, string role = "")
        {
            List<ApplicationStatus> approverMatrixList = new List<ApplicationStatus>();
            approverMatrixList = GlobalCachingProvider.Instance.GetItem(ListNames.SLAList, false) as List<ApplicationStatus>;
            if (approverMatrixList == null)
            {
                approverMatrixList = this.GetAllGlobalAppApproverMatrix();
                GlobalCachingProvider.Instance.AddItem(ListNames.SLAList, approverMatrixList);
            }
            List<ApplicationStatus> approverMatrixList1 = (from p in approverMatrixList
                                                           where !string.IsNullOrEmpty(p.ApplicationName) && !string.IsNullOrEmpty(p.FormName) && p.ApplicationName.ToLower() == applicationName.ToLower() && p.FormName.ToLower() == formName.ToLower()
                                                           select p).ToList();

            if (!string.IsNullOrEmpty(role))
            {
                approverMatrixList1 = (from p in approverMatrixList1
                                       where !string.IsNullOrEmpty(p.FillByRole) && p.FillByRole.ToLower().Contains(role.ToLower())
                                       select p).ToList();
            }

            return approverMatrixList1;
        }

        /// <summary>
        /// Gets the global application approver matrix for current role.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="role">The role.</param>
        /// <returns>Application Status</returns>
        public ApplicationStatus GetGlobalAppApproverMatrixForCurrentRole(string applicationName, string formName, string role)
        {
            ApplicationStatus appStatus = null;
            string siteURL = this.GetSiteURL(SiteURLs.ROOTSITEURL); //this.GetConfigVariable(SiteURLs.ROOTSITEURL);
            ClientContext context = this.CreateClientContext(siteURL);
            Web web = this.CreateWeb(context);
            context.Load(web);
            context.ExecuteQuery();
            List slaList = web.Lists.GetByTitle(ListNames.SLAList);
            CamlQuery qry = new CamlQuery();
            qry.ViewXml = @"<View>
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
                                                <Contains>
                                                    <FieldRef Name='Role' />
                                                    <Value Type='Text'>" + role + @"</Value>
                                                </Contains>
                                            </And>
                                        </Where>
                                    </Query>
                                </View>";
            ListItemCollection items = slaList.GetItems(qry);
            context.Load(items);
            context.ExecuteQuery();
            foreach (ListItem item in items)
            {
                appStatus = new ApplicationStatus()
                {
                    //ID = item.Id,
                    Title = item["Title"].ToString(),
                    ApplicationName = this.GetTaxonomyFieldValue(item["ApplicationName"] as TaxonomyFieldValue),
                    FormName = this.GetTaxonomyFieldValue(item["FormName"] as TaxonomyFieldValue),
                    SectionName = this.GetTaxonomyFieldValueCollection(item["SectionName"] as TaxonomyFieldValueCollection),
                    HiddenSection = this.GetTaxonomyFieldValueCollection(item["HiddenSection"] as TaxonomyFieldValueCollection),
                    Levels = Convert.ToString(item["Levels"]),
                    Role = Convert.ToString(item["Role"]),
                    FillByRole = Convert.ToString(item["FillByRole"]),
                    Division = Convert.ToString(item["Division"]),
                    SubDivision = Convert.ToString(item["SubDivision"]),
                    Days = Convert.ToInt32(item["Days"]),
                    IsAutoApproval = (bool)item["IsAutoApproval"],
                    IsOptional = item["IsOptional"] != null ? (bool)item["IsOptional"] : false
                };
            }
            return appStatus;
        }

        /// <summary>
        /// Gets the application approval matrix.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="approvalMatrixListName">Name of the approval matrix list.</param>
        /// <param name="getAllRecords">The get All Records.</param>
        /// <returns>List of Application Status</returns>
        public List<ApplicationStatus> GetAppApprovalMatrix(ClientContext context, Web web, int itemId, string approvalMatrixListName, bool getAllRecords = false)
        {
            List<ApplicationStatus> approvalMatrix = new List<ApplicationStatus>();
            if (context != null && web != null)
            {
                //BELDataAccessLayer helper = new BELDataAccessLayer();
                if (BELDataAccessLayer.Instance.GetConfigVariable("UseRESTAPI").ToLower().Equals("true"))
                {
                    approvalMatrix = this.GetAppApprovalMatrixUsingREST(context, web, itemId, approvalMatrixListName);
                }
                else
                {
                    approvalMatrix = this.GetAppApprovalMatrixUsingCSOM(context, web, itemId, approvalMatrixListName, getAllRecords);
                }
            }
            return approvalMatrix;
        }

        /// <summary>
        /// Gets the application approval matrix.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="approvalMatrixListName">Name of the approval matrix list.</param>
        /// <param name="getAllRecords">The get All Records.</param>
        /// <returns>List of Application Status</returns>
        public List<ApplicationStatus> GetAppApprovalMatrixUsingCSOM(ClientContext context, Web web, int itemId, string approvalMatrixListName, bool getAllRecords = false)
        {
            List<ApplicationStatus> approvalMatrix = new List<ApplicationStatus>();
            if (context != null && web != null)
            {
                context.Load(web);
                context.ExecuteQuery();
                List spList = web.Lists.GetByTitle(approvalMatrixListName);
                CamlQuery query = new CamlQuery();
                if (getAllRecords)
                {
                    query.ViewXml = @"<View>
                                         <Query>
                                                <Where>
                                                      <Eq>
                                                         <FieldRef Name='RequestID' LookupId='TRUE'/>
                                                         <Value Type='Lookup'>" + itemId + @"</Value>
                                                       </Eq>
                                                     </Where>
                                            </Query>
                                            </View>";
                }
                else
                {
                    query.ViewXml = @"<View>
                                         <Query>
                                                <Where>
                                                    <And>
                                                      <Eq>
                                                         <FieldRef Name='RequestID' LookupId='TRUE'/>
                                                         <Value Type='Lookup'>" + itemId + @"</Value>
                                                       </Eq>
                                                        <Neq>
                                                         <FieldRef Name='Status' />
                                                         <Value Type='Text'>" + ApproverStatus.NOTREQUIRED + @"</Value>
                                                       </Neq>
                                                     </And>
                                                </Where>
                                            </Query>
                                            </View>";
                }
                ListItemCollection items = spList.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();

                if (items != null && items.Count > 0)
                {
                    approvalMatrix = this.AssignAppApproverMatrixValues(context, web, items);
                }
            }
            return approvalMatrix;
        }

        /// <summary>
        /// Gets the application approval matrix.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="approvalMatrixListName">Name of the approval matrix list.</param>
        /// <returns>List of Application Status</returns>
        public List<ApplicationStatus> GetAppApprovalMatrixUsingREST(ClientContext context, Web web, int itemId, string approvalMatrixListName)
        {
            List<ApplicationStatus> approvalMatrix = new List<ApplicationStatus>();
            if (context != null && web != null)
            {
                PropertyInfo[] propertyInfo = typeof(ApplicationStatus).GetProperties();
                string selectClause = "ID";
                string expandClause = string.Empty;
                foreach (PropertyInfo property in propertyInfo)
                {
                    bool isListColumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                    string propertyName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                    bool isFile = property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile;
                    if (isListColumn)
                    {
                        if (property.GetCustomAttribute<FieldColumnNameAttribute>() != null && property.GetCustomAttribute<FieldColumnNameAttribute>().IsLookup)
                        {
                            string lookupFieldName = property.GetCustomAttribute<FieldColumnNameAttribute>().LookupFieldName;
                            selectClause = selectClause.Trim(',') + "," + propertyName + "/ID" + "," + propertyName + "/" + lookupFieldName;
                            expandClause = expandClause.Trim(',') + "," + propertyName;
                        }
                        else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson)
                        {
                            selectClause = selectClause.Trim(',') + "," + propertyName + "/EMail" + "," + propertyName + "/Title";
                            expandClause = expandClause.Trim(',') + "," + propertyName;
                        }
                        else if (isFile)
                        {
                            selectClause = selectClause.Trim(',') + "," + "AttachmentFiles";
                        }
                        else
                        {
                            selectClause = selectClause.Trim(',') + "," + propertyName;
                        }
                    }
                }
                JObject jobj = RESTHelper.GetDataUsingRest(web.Url + "/_api/web/lists/GetByTitle('" + approvalMatrixListName + "')/Items?$select=" + selectClause.Trim(',') + "&$expand=" + expandClause.Trim(',') + "&$filter=RequestID eq '" + itemId + "'&$top=" + int.MaxValue, "GET");
                JArray jarr = (JArray)jobj["d"]["results"];
                approvalMatrix = this.AssignAppApproverMatrixValuesREST(context, web, jarr);
            }
            return approvalMatrix;
        }

        /// <summary>
        /// Assigns the application approver matrix values.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="items">The items.</param>
        /// <returns>List of Application Status</returns>
        private List<ApplicationStatus> AssignAppApproverMatrixValuesREST(ClientContext context, Web web, JArray items)
        {
            List<ApplicationStatus> appApproversMatrix = new List<ApplicationStatus>();
            foreach (JToken item in items)
            {
                ApplicationStatus approver = new ApplicationStatus();
                approver = this.SetApproverPropertiesREST(context, web, item, approver, typeof(ApplicationStatus).GetProperties());
                appApproversMatrix.Add(approver);
            }
            appApproversMatrix = appApproversMatrix.OrderBy(p => Convert.ToInt32(p.Levels)).ToList();
            return appApproversMatrix;
        }

        /// <summary>
        /// Sets the approver properties.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="item">The item.</param>
        /// <param name="approver">The approver.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>Application Status</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "NA")]
        private ApplicationStatus SetApproverPropertiesREST(ClientContext context, Web web, JToken item, ApplicationStatus approver, PropertyInfo[] propertyInfo)
        {
            // BELDataAccessLayer helper = new BELDataAccessLayer();
            foreach (PropertyInfo property in propertyInfo)
            {
                bool isListColumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                string listCoumnName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                bool isFile = property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile;
                if (isListColumn)
                {
                    if (property.GetCustomAttribute<FieldColumnNameAttribute>() != null && property.GetCustomAttribute<FieldColumnNameAttribute>().IsLookup)
                    {
                        string lookupFieldName = property.GetCustomAttribute<FieldColumnNameAttribute>().LookupFieldName;
                        if (property.GetCustomAttribute<FieldColumnNameAttribute>().IsMultipleLookup)
                        {
                            if (((JContainer)item[listCoumnName])["results"] != null)
                            {
                                List<string> reletedto = new List<string>();
                                foreach (dynamic lookup in ((JContainer)item[listCoumnName])["results"])
                                {
                                    reletedto.Add(lookup[lookupFieldName].ToString());
                                }
                                property.SetValue(approver, reletedto);
                            }
                        }
                        else
                        {
                            if (((dynamic)item[listCoumnName])[lookupFieldName] != null)
                            {
                                object value = ((JValue)((dynamic)item[listCoumnName])[lookupFieldName]).Value;
                                Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                                property.SetValue(approver, safeValue);
                            }
                        }
                    }
                    else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson && !property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                    {
                        string personEmail = string.Empty;
                        if (property.GetCustomAttribute<IsPersonAttribute>().IsMultiple)
                        {
                            if (((JContainer)item[listCoumnName])["results"] != null)
                            {
                                foreach (dynamic person in ((JContainer)item[listCoumnName])["results"])
                                {
                                    personEmail = personEmail.Trim(',') + "," + person["EMail"].ToString();
                                }
                            }
                        }
                        else
                        {
                            if (((dynamic)item[listCoumnName]).EMail != null)
                            {
                                personEmail = ((dynamic)item[listCoumnName])["EMail"].ToString();
                            }
                        }
                        property.SetValue(approver, personEmail.Trim(','));
                    }
                    else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson && property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                    {
                        string personName = string.Empty;
                        if (property.GetCustomAttribute<IsPersonAttribute>().IsMultiple)
                        {
                            if (((JContainer)item[listCoumnName])["results"] != null)
                            {
                                foreach (dynamic person in ((JContainer)item[listCoumnName])["results"])
                                {
                                    personName = personName.Trim(',') + "," + person["Title"].ToString();
                                }
                            }
                        }
                        else
                        {
                            if (((dynamic)item[listCoumnName]).Title != null)
                            {
                                personName = ((dynamic)item[listCoumnName])["Title"].ToString();
                            }
                        }
                        property.SetValue(listCoumnName, personName.Trim(','));
                    }
                    else if (isFile)
                    {
                        if (((dynamic)((dynamic)item["AttachmentFiles"])).__deferred != null)
                        {
                            string url = ((dynamic)((dynamic)item["AttachmentFiles"])).__deferred.uri.Value;
                            List<FileDetails> objAttachmentFiles = BELDataAccessLayer.Instance.GetAttachmentsUsingREST(url);
                            property.SetValue(approver, objAttachmentFiles);
                        }
                    }
                    else
                    {
                        object value = ((JValue)item[listCoumnName]).Value;
                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                        property.SetValue(approver, safeValue);
                    }
                }
            }
            approver.ID = Convert.ToInt32(item["ID"]);
            return approver;
        }

        /// <summary>
        /// Deletes the approval matrix of request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="approvalMatrixListName">Name of the approval matrix list.</param>
        /// <returns>true or false</returns>
        public bool DeleteApprovalMatrixOfRequest(ClientContext context, Web web, int itemId, string approvalMatrixListName)
        {
            bool isSuccess = false;
            if (context != null && web != null)
            {
                context.Load(web);
                context.ExecuteQuery();
                List spList = web.Lists.GetByTitle(approvalMatrixListName);
                CamlQuery query = new CamlQuery();
                query.ViewXml = @"<View>
                                         <Query>
                                                <Where>
                                                      <Eq>
                                                         <FieldRef Name='RequestID' LookupId='TRUE'/>
                                                         <Value Type='Lookup'>" + itemId + @"</Value>
                                                       </Eq>
                                                     </Where>
                                            </Query>
                                            </View>";
                ListItemCollection items = spList.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();
                if (items != null && items.Count > 0)
                {
                    int length = items.Count - 1;
                    while (true)
                    {
                        if (length == -1)
                        {
                            break;
                        }
                        items[length].DeleteObject();
                        context.ExecuteQuery();
                        length--;
                    }
                }
                isSuccess = true;
            }
            return isSuccess;
        }

        /// <summary>
        /// Gets the application approval matrix by role.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="role">The role.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="approverMatrixListName">Name of the approver matrix list.</param>
        /// <param name="userEmail">The user email.</param>
        /// <returns>Application Status</returns>
        public ApplicationStatus GetAppApprovalMatrixByRole(ClientContext context, Web web, string role, int itemId, string approverMatrixListName, string userEmail)
        {
            ApplicationStatus approver = null;
            if (context != null && web != null && !string.IsNullOrEmpty(approverMatrixListName) && itemId > 0)
            {
                User user = BELDataAccessLayer.EnsureUser(context, web, userEmail);
                List approverList = web.Lists.GetByTitle(approverMatrixListName);
                CamlQuery query = new CamlQuery();
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
                                                        <FieldRef Name='Role' />
                                                        <Value Type='Text'>" + role + @"</Value>
                                                    </Eq>
                                                    <Contains>
                                                        <FieldRef Name='Approver' />
                                                        <Value Type='User'>" + user.Title + @"</Value>
                                                    </Contains>
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
                    approver = new ApplicationStatus();
                    approver = this.SetApproverProperties(context, web, item, approver, approver.GetType().GetProperties());
                }
            }
            return approver;
        }

        /// <summary>
        /// Sets the application approval matrix by role.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="role">The role.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="approverMatrixListName">Name of the approver matrix list.</param>
        /// <param name="userEmail">The user email.</param>
        /// <param name="newUserEmailToAppend">The new user email to append.</param>
        /// <returns>true or false</returns>
        public bool SetAppApprovalMatrixByRole(ClientContext context, Web web, string role, int itemId, string approverMatrixListName, string userEmail, string newUserEmailToAppend)
        {
            if (context != null && web != null && !string.IsNullOrEmpty(approverMatrixListName) && itemId > 0)
            {
                User user = BELDataAccessLayer.EnsureUser(context, web, userEmail);
                List approverList = web.Lists.GetByTitle(approverMatrixListName);
                CamlQuery query = new CamlQuery();
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
                                                        <FieldRef Name='Role' />
                                                        <Value Type='Text'>" + role + @"</Value>
                                                    </Eq>
                                                    <Contains>
                                                        <FieldRef Name='Approver' />
                                                        <Value Type='User'>" + user.Title + @"</Value>
                                                    </Contains>
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
                    string approverEmails = BELDataAccessLayer.GetEmailsFromPersonField(context, web, item["Approver"] as FieldUserValue[]).Trim(',') + "," + newUserEmailToAppend;
                    item["Approver"] = BELDataAccessLayer.GetFieldUserValueFromPerson(context, web, approverEmails);
                    item.Update();
                    context.Load(item);
                    context.ExecuteQuery();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the approver matrix.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="fillByRole">The role.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="listName">Name of the list.</param>
        /// <returns>
        /// List of Approvers Role filled by Current User Role for Edit
        /// </returns>
        public List<ApplicationStatus> GetAppApproverMatrixByFillbyRole(ClientContext context, Web web, string fillByRole, int itemId, string listName)
        {
            List<ApplicationStatus> approverMatrixList = new List<ApplicationStatus>();
            if (context != null && web != null && !string.IsNullOrEmpty(fillByRole) && itemId > 0 && !string.IsNullOrEmpty(listName))
            {
                List approvalMatrixList = web.Lists.GetByTitle(listName);
                CamlQuery qry = new CamlQuery();
                qry.ViewXml = @"<View>
                                    <Query>
                                        <Where>
                                            <And>
                                                <Eq>
                                                    <FieldRef Name='RequestID' />
                                                    <Value Type='Lookup'>" + itemId + @"</Value>
                                                </Eq>
                                                <Contains>
                                                    <FieldRef Name='FillByRole' />
                                                    <Value Type='Text'>" + fillByRole + @"</Value>
                                                </Contains>
                                            </And>
                                        </Where>
                                    </Query>
                                </View>";
                ListItemCollection items = approvalMatrixList.GetItems(qry);
                context.Load(items);
                context.ExecuteQuery();
                if (items != null && items.Count > 0)
                {
                    approverMatrixList = this.AssignAppApproverMatrixValues(context, web, items);
                    approverMatrixList.Remove(approverMatrixList.FirstOrDefault(p => p.Role == UserRoles.VIEWER));
                }
            }
            return approverMatrixList;
        }

        /// <summary>
        /// Gets the Application Status.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="lookupId">The lookup identifier.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="getAllRecords">The get All Records.</param>
        /// <returns>
        /// section object
        /// </returns>
        public ISection GetAppApproverMatrixSection(ClientContext context, Web web, string listName, int lookupId, bool isActive, bool getAllRecords = false)
        {
            ApplicationStatusSection appStatusSection = new ApplicationStatusSection(true) { IsActive = isActive };
            if (context != null && web != null && !string.IsNullOrEmpty(listName) && lookupId > 0)
            {
                appStatusSection.ApplicationStatusList = this.GetAppApprovalMatrix(context, web, lookupId, listName, getAllRecords);
                appStatusSection.ApplicationStatusList.RemoveAll(p => Convert.ToInt32(p.Levels) < 0);
                appStatusSection.ApplicationStatusList = appStatusSection.ApplicationStatusList.OrderBy(p => Convert.ToInt32(p.Levels)).ToList();
            }

            return appStatusSection;
        }

        /// <summary>
        /// Assigns the application approver matrix values.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="items">The items.</param>
        /// <returns>List of Application Status</returns>
        public List<ApplicationStatus> AssignAppApproverMatrixValues(ClientContext context, Web web, ListItemCollection items)
        {
            List<ApplicationStatus> appApproversMatrix = new List<ApplicationStatus>();
            if (context != null && web != null && items != null)
            {
                foreach (ListItem item in items)
                {
                    ApplicationStatus approver = new ApplicationStatus();
                    approver = this.SetApproverProperties(context, web, item, approver, approver.GetType().GetProperties());
                    appApproversMatrix.Add(approver);
                }
                appApproversMatrix = appApproversMatrix.OrderBy(p => Convert.ToInt32(p.Levels)).ToList();
            }
            return appApproversMatrix;
        }

        /// <summary>
        /// Sets the approver properties.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="item">The item.</param>
        /// <param name="approver">The approver.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>Application Status</returns>
        private ApplicationStatus SetApproverProperties(ClientContext context, Web web, ListItem item, ApplicationStatus approver, PropertyInfo[] propertyInfo)
        {
            //BELDataAccessLayer helper = new BELDataAccessLayer();
            foreach (PropertyInfo property in propertyInfo)
            {
                bool isListColumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                string listCoumnName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                bool isFile = property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile;
                if (isListColumn)
                {
                    if (property.GetCustomAttribute<FieldColumnNameAttribute>() != null && property.GetCustomAttribute<FieldColumnNameAttribute>().IsLookup)
                    {
                        FieldLookupValue lookupField = item[listCoumnName] as FieldLookupValue;
                        property.SetValue(approver, lookupField.LookupId);
                    }
                    else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson && !property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                    {
                        FieldUserValue[] users = null;
                        if (item[listCoumnName] != null)
                        {
                            if (property.GetCustomAttribute<IsPersonAttribute>().IsMultiple)
                            {
                                users = item[listCoumnName] as FieldUserValue[];
                            }
                            else
                            {
                                users = new FieldUserValue[1];
                                users[0] = item[listCoumnName] as FieldUserValue;
                            }
                        }
                        if (users != null)
                        {
                            string personEmails = BELDataAccessLayer.GetEmailsFromPersonField(context, web, users);
                            property.SetValue(approver, personEmails);
                        }
                    }
                    else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson && property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                    {
                        FieldUserValue[] users = null;
                        if (item[listCoumnName] != null)
                        {
                            if (property.GetCustomAttribute<IsPersonAttribute>().IsMultiple)
                            {
                                users = item[listCoumnName] as FieldUserValue[];
                            }
                            else
                            {
                                users = new FieldUserValue[1];
                                users[0] = item[listCoumnName] as FieldUserValue;
                            }
                        }
                        if (users != null)
                        {
                            string personEmails = BELDataAccessLayer.GetNameFromPersonField(context, web, users);
                            property.SetValue(approver, personEmails);
                        }
                    }
                    else if (isFile)
                    {
                        if (Convert.ToString(item["Attachments"]) == "True")
                        {
                            context.Load(item.AttachmentFiles);
                            context.ExecuteQuery();
                            AttachmentCollection attachments = item.AttachmentFiles;
                            List<FileDetails> objAttachmentFiles = BELDataAccessLayer.Instance.GetAttachments(attachments);
                            property.SetValue(approver, objAttachmentFiles);
                        }
                    }
                    else
                    {
                        object value = item[listCoumnName];
                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                        property.SetValue(approver, safeValue);
                    }
                }
            }
            approver.ID = Convert.ToInt32(item["ID"]);

            return approver;
        }

        /// <summary>
        /// Gets all employee.
        /// </summary>
        /// <returns>List of users</returns>
        private List<ApplicationStatus> GetAllGlobalAppApproverMatrix()
        {
            Logger.Info("Get All SLA Matrix Data  from Sharepoint root Site");
            List<ApplicationStatus> approverMatrixList = new List<ApplicationStatus>();
            string siteURL = this.GetSiteURL(SiteURLs.ROOTSITEURL); //this.GetConfigVariable(SiteURLs.ROOTSITEURL);
            ClientContext context = this.CreateClientContext(siteURL);
            Web web = this.CreateWeb(context);
            context.Load(web);
            context.ExecuteQuery();
            List slaList = web.Lists.GetByTitle(ListNames.SLAList);
            ListItemCollection items = slaList.GetItems(CamlQuery.CreateAllItemsQuery());
            context.Load(items);
            context.ExecuteQuery();

            foreach (ListItem item in items)
            {
                approverMatrixList.Add(new ApplicationStatus()
                {
                    //ID = item.Id,
                    Title = Convert.ToString(item["Title"]),
                    ApplicationName = this.GetTaxonomyFieldValue(item["ApplicationName"] as TaxonomyFieldValue),
                    FormName = this.GetTaxonomyFieldValue(item["FormName"] as TaxonomyFieldValue),
                    SectionName = this.GetTaxonomyFieldValueCollection(item["SectionName"] as TaxonomyFieldValueCollection).Trim(','),
                    HiddenSection = this.GetTaxonomyFieldValueCollection(item["HiddenSection"] as TaxonomyFieldValueCollection).Trim(','),
                    Levels = Convert.ToString(item["Levels"]),
                    Role = Convert.ToString(item["Role"]),
                    FillByRole = Convert.ToString(item["FillByRole"]),
                    /* Approver Copy from Global Matrix*/
                    Approver = BELDataAccessLayer.GetEmailsFromPersonField(context, web, item["Approver"] as FieldUserValue[]),
                    /* Approver Copy from Global Matrix*/
                    Division = Convert.ToString(item["Division"]),
                    //// SubDivision = Convert.ToString(item["SubDivision"]),
                    Days = Convert.ToInt32(item["Days"]),
                    IsAutoApproval = (bool)item["IsAutoApproval"],
                    IsOptional = item["IsOptional"] != null ? (bool)item["IsOptional"] : false,
                    IsEscalate = item["IsEscalate"] != null ? (bool)item["IsEscalate"] : false,
                    IsReminder = item["IsReminder"] != null ? (bool)item["IsReminder"] : false,
                    FormType = item["FormType"] != null ? Convert.ToString(item["FormType"]) : FormType.MAIN
                });
            }
            return approverMatrixList;
        }
        #endregion

        #region "Save App Approval Matrix"
        /// <summary>
        /// Saves the approver matrix.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="approverMatrixList">The approver matrix list.</param>
        /// <param name="listName">Name of the list.</param>
        /// <returns>True or False</returns>
        public bool SaveApproverMatrix(ClientContext context, Web web, List<ApplicationStatus> approverMatrixList, string listName) // Jatin 
        {
            if (context != null && web != null && approverMatrixList != null)
            {
                List list = web.Lists.GetByTitle(listName);
                ListItem[] approverItems = new ListItem[approverMatrixList != null ? approverMatrixList.Count : 0];
                int i = 0;
                foreach (ApplicationStatus approver in approverMatrixList)
                {
                    if (approver.ID > 0)
                    {
                        approverItems[i] = list.GetItemById(approver.ID);
                        context.Load(approverItems[i]);
                        context.ExecuteQuery();
                    }
                    else
                    {
                        approverItems[i] = list.AddItem(new ListItemCreationInformation());
                    }
                    PropertyInfo[] itemProperties = approver.GetType().GetProperties();
                    Dictionary<string, dynamic> itemValues = new Dictionary<string, object>();
                    List<FileDetails> files = null;
                    foreach (PropertyInfo property in itemProperties)
                    {
                        bool isListCoumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                        bool isFile = property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile;
                        bool isPerson = property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson;
                        bool isReturnName = property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().ReturnName;
                        string listCoumnName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                        if (isListCoumn)
                        {
                            if (isPerson)
                            {
                                if (!isReturnName)
                                {
                                    string approvers = Convert.ToString(property.GetValue(approver)).Trim(',');
                                    itemValues[listCoumnName] = this.GetMultiplePersonField(context, web, approvers, property);
                                }
                            }
                            else if (isFile)
                            {
                                files = property.GetValue(approver) != null ? property.GetValue(approver) as List<FileDetails> : null;
                            }
                            else
                            {
                                itemValues[listCoumnName] = property.GetValue(approver);
                            }
                        }
                    }
                    foreach (KeyValuePair<string, object> itemValue in itemValues)
                    {
                        approverItems[i][itemValue.Key] = itemValue.Value;
                    }
                    approverItems[i]["_ModerationStatus"] = 0;
                    approverItems[i].Update();
                    approverItems[i].RefreshLoad();  //  Added for Version Conflict!
                    context.Load(approverItems[i]);
                    context.ExecuteQuery();
                    if (files != null)
                    {
                        this.SaveAttachment(context, files, ref approverItems[i]);
                    }
                    i++;
                }
            }
            return true;
        }

        /// <summary>
        /// Updates the task item.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="item">The item.</param>
        /// <param name="approver">The approver.</param>
        /// <returns>
        /// true or false
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Can not able to change anything here."), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "could not able to reduce size.")]
        private bool SaveAppApprovalMatrixValue(ClientContext context, Web web, ref ListItem item, ApplicationStatus approver)
        {
            PropertyInfo[] itemProperties = approver.GetType().GetProperties();
            Dictionary<string, object> itemValues = new Dictionary<string, object>();
            List<FileDetails> files = null;
            foreach (PropertyInfo property in itemProperties)
            {
                bool isListCoumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                bool isFile = property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile;
                bool isPerson = property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson;
                bool isReturnName = property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().ReturnName;
                string listCoumnName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                if (isListCoumn)
                {
                    if (isPerson && !isReturnName)
                    {
                        string approvers = Convert.ToString(property.GetValue(approver)).Trim(',');
                        itemValues[listCoumnName] = this.GetMultiplePersonField(context, web, approvers, property);
                    }
                    else if (isFile)
                    {
                        files = property.GetValue(approver) != null ? property.GetValue(approver) as List<FileDetails> : null;
                    }
                    else
                    {
                        itemValues[listCoumnName] = property.GetValue(approver);
                    }
                }
                item.Update();
            }
            foreach (KeyValuePair<string, object> itemValue in itemValues)
            {
                item[itemValue.Key] = itemValue.Value;
            }
            item.Update();
            item.RefreshLoad();
            context.Load(item);
            context.ExecuteQuery();
            if (files != null)
            {
                this.SaveAttachment(context, files, ref item);
            }
            return true;
        }
        #endregion

        #region "Updates Satus "
        /// <summary>
        /// Updates the status of approval matrix.
        /// </summary>
        /// <param name="approvers">The approvers.</param>
        /// <param name="currLevel">The next level.</param>
        /// <param name="preLevel">The current level.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="actionPerformed">The action performed.</param>
        /// <returns>
        /// List Application Status
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Can not able to change anything here."), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "could not able to reduce size.")]
        public List<ApplicationStatus> UpdateStatusofApprovalMatrix(List<ApplicationStatus> approvers, int currLevel, int preLevel, Dictionary<string, string> param, ButtonActionStatus actionPerformed)
        {
            if (approvers != null && param != null)
            {
                if (currLevel != preLevel)
                {
                    string userEmail = param[Parameter.USEREID];

                    if (approvers != null && approvers.Count != 0 && !string.IsNullOrEmpty(userEmail))
                    {
                        int nextLevel = currLevel;

                        switch (actionPerformed)
                        {
                            case ButtonActionStatus.SaveAndStatusUpdate:
                            case ButtonActionStatus.SaveAndStatusUpdateWithEmail:
                            case ButtonActionStatus.SaveAndNoStatusUpdate:
                            case ButtonActionStatus.SaveAndNoStatusUpdateWithEmail:
                            case ButtonActionStatus.Submit:
                            case ButtonActionStatus.Reschedule:
                            case ButtonActionStatus.UpdateAndRepublish:
                            case ButtonActionStatus.ReadyToPublish:
                            case ButtonActionStatus.Save:
                            case ButtonActionStatus.SaveAsDraft:
                            case ButtonActionStatus.None:
                                Logger.Info("Save as draft condition => any approver=" + approvers.Any(p => p.Levels == currLevel.ToString()).ToString());
                                ////if (approvers.Any(p => p.Levels == currLevel.ToString() && p.Approver.Contains(userEmail)))
                                if (approvers.Any(p => p.Levels == currLevel.ToString()))
                                {
                                    approvers.ForEach(p =>
                                    {
                                        Logger.Info(p.Levels + "==" + currLevel.ToString() + " && " + p.Status + "==" + ApproverStatus.NOTASSIGNED);
                                        if (p.Levels == currLevel.ToString() && p.Status == ApproverStatus.NOTASSIGNED)
                                        {
                                            Logger.Info("condition true for => " + JsonConvert.SerializeObject(p));
                                            p.Status = ApproverStatus.PENDING;
                                            p.DueDate = this.GetDueDate(DateTime.Now, Convert.ToInt32(p.Days));
                                            p.AssignDate = DateTime.Now;
                                        }
                                    });
                                    ////approvers.FirstOrDefault(p => p.Levels == currLevel.ToString() && p.Approver.Contains(userEmail)).Status = ApproverStatus.PENDING;
                                    ////approvers.FirstOrDefault(p => p.Levels == currLevel.ToString() && p.Approver.Contains(userEmail)).DueDate = DateTime.Now.AddDays(approvers.FirstOrDefault(p => p.Levels == currLevel.ToString() && p.Approver.Contains(userEmail)).Days);
                                    ////approvers.FirstOrDefault(p => p.Levels == currLevel.ToString() && p.Approver.Contains(userEmail)).AssignDate = DateTime.Now;
                                }
                                break;
                            case ButtonActionStatus.Delegate:
                            case ButtonActionStatus.NextApproval:
                                if (approvers.Any(p => !string.IsNullOrEmpty(p.Approver) && p.Levels == currLevel.ToString() && p.Approver.Split(',').Contains(userEmail)))
                                {
                                    approvers.FirstOrDefault(p => !string.IsNullOrEmpty(p.Approver) && p.Levels == currLevel.ToString() && p.Approver.Split(',').Contains(userEmail)).Status = ApproverStatus.APPROVED;
                                }
                                if (!approvers.Any(p => p.IsOptional == false && !string.IsNullOrEmpty(p.Approver) && p.Status != ApproverStatus.APPROVED && Convert.ToInt32(p.Levels) == currLevel))
                                {
                                    ApplicationStatus approver = approvers.Where(p => Convert.ToInt32(p.Levels) > currLevel && !string.IsNullOrEmpty(p.Approver) && p.Status != ApproverStatus.NOTREQUIRED).OrderBy(p => Convert.ToInt32(p.Levels)).FirstOrDefault();
                                    if (approver != null)
                                    {
                                        nextLevel = Convert.ToInt32(approver.Levels);
                                    }
                                }
                                approvers.ForEach(p =>
                                {
                                    if (!string.IsNullOrEmpty(p.Approver) && p.Levels == currLevel.ToString() && p.Approver.Split(',').Contains(userEmail))
                                    {
                                        p.ApproveBy = userEmail;
                                        p.ApprovalDate = DateTime.Now;
                                        p.Status = ApproverStatus.APPROVED;
                                    }
                                    else if (Convert.ToInt32(p.Levels) == nextLevel && (p.Status != ApproverStatus.APPROVED && p.Status != ApproverStatus.NOTREQUIRED))
                                    {
                                        p.DueDate = this.GetDueDate(DateTime.Now, Convert.ToInt32(p.Days));
                                        p.AssignDate = DateTime.Now;
                                        p.Status = ApproverStatus.PENDING;
                                    }
                                    else if (Convert.ToInt32(p.Levels) > nextLevel && p.Status != ApproverStatus.NOTREQUIRED)
                                    {
                                        p.Status = ApproverStatus.NOTASSIGNED;
                                        p.AssignDate = null;
                                        p.DueDate = null;
                                        p.Comments = string.Empty;
                                    }
                                });

                                break;
                            case ButtonActionStatus.BackToCreator:
                            case ButtonActionStatus.SendBack:
                                //if (approvers.Any(p => p.Levels == currLevel.ToString() && p.Approver.Contains(userEmail)))
                                //{
                                //    approvers.FirstOrDefault(p => p.Levels == currLevel.ToString() && p.Approver.Contains(userEmail)).Status = ApproverStatus.APPROVED;
                                //}
                                string sendtoRole = string.Empty;
                                if (param.ContainsKey(Parameter.SENDTOLEVEL) && param[Parameter.SENDTOLEVEL] != null && !string.IsNullOrEmpty(param[Parameter.SENDTOLEVEL]))
                                {
                                    nextLevel = Convert.ToInt32(param[Parameter.SENDTOLEVEL]);
                                }
                                if (param.ContainsKey(Parameter.SENDTOROLE) && param[Parameter.SENDTOROLE] != null && !string.IsNullOrEmpty(param[Parameter.SENDTOROLE]))
                                {
                                    sendtoRole = Convert.ToString(param[Parameter.SENDTOROLE]);
                                }

                                approvers.ForEach(p =>
                                {
                                    if (!string.IsNullOrEmpty(p.Approver) && p.Levels == currLevel.ToString() && p.Approver.Split(',').Contains(userEmail))
                                    {
                                        p.ApproveBy = userEmail;
                                        p.ApprovalDate = DateTime.Now;
                                        p.Status = ApproverStatus.SENDBACK;
                                    }
                                    else if (Convert.ToInt32(p.Levels) == nextLevel)
                                    {
                                        if (string.IsNullOrEmpty(sendtoRole) || (!string.IsNullOrEmpty(sendtoRole) && p.Role == sendtoRole))
                                        {
                                            p.DueDate = this.GetDueDate(DateTime.Now, Convert.ToInt32(p.Days));
                                            p.AssignDate = DateTime.Now;
                                            p.Status = ApproverStatus.PENDING;
                                        }
                                    }
                                    else if (Convert.ToInt32(p.Levels) > nextLevel)
                                    {
                                        p.Status = ApproverStatus.NOTASSIGNED;
                                        ////p.AssignDate = null;
                                        ////p.DueDate = null;
                                        //// p.Comments = string.Empty;
                                    }
                                });
                                break;
                            case ButtonActionStatus.SendForward:
                                if (param.ContainsKey(Parameter.SENDTOLEVEL) && param[Parameter.SENDTOLEVEL] != null && !string.IsNullOrEmpty(param[Parameter.SENDTOLEVEL]))
                                {
                                    nextLevel = Convert.ToInt32(param[Parameter.SENDTOLEVEL]);
                                    ApplicationStatus approver = approvers.Where(p => Convert.ToInt32(p.Levels) >= nextLevel && !string.IsNullOrEmpty(p.Approver)).OrderBy(p => Convert.ToInt32(p.Levels)).FirstOrDefault();
                                    if (approver != null)
                                    {
                                        nextLevel = Convert.ToInt32(approver.Levels);
                                    }
                                }
                                approvers.ForEach(p =>
                                {
                                    if (!string.IsNullOrEmpty(p.Approver) && p.Levels == currLevel.ToString() && p.Approver.Split(',').Contains(userEmail))
                                    {
                                        p.ApproveBy = userEmail;
                                        p.ApprovalDate = DateTime.Now;
                                        p.Status = ApproverStatus.SENDFORWARD;
                                    }
                                    else if (Convert.ToInt32(p.Levels) == nextLevel)
                                    {
                                        p.DueDate = this.GetDueDate(DateTime.Now, Convert.ToInt32(p.Days));
                                        p.AssignDate = DateTime.Now;
                                        p.Status = ApproverStatus.PENDING;
                                    }
                                    else if (Convert.ToInt32(p.Levels) > nextLevel)
                                    {
                                        p.Status = ApproverStatus.NOTASSIGNED;
                                        p.AssignDate = null;
                                        p.DueDate = null;
                                        p.Comments = string.Empty;
                                    }
                                });
                                break;
                            case ButtonActionStatus.Cancel:
                                break;
                            case ButtonActionStatus.Rejected:
                                if (approvers.Any(p => p.Levels == currLevel.ToString() && (!string.IsNullOrWhiteSpace(p.Approver) && p.Approver.Split(',').Contains(userEmail))))
                                {
                                    approvers.FirstOrDefault(p => p.Levels == currLevel.ToString() && (!string.IsNullOrWhiteSpace(p.Approver) && p.Approver.Split(',').Contains(userEmail))).Status = ApproverStatus.APPROVED;
                                    approvers.FirstOrDefault(p => p.Levels == currLevel.ToString() && (!string.IsNullOrWhiteSpace(p.Approver) && p.Approver.Split(',').Contains(userEmail))).ApprovalDate = DateTime.Now;
                                    approvers.FirstOrDefault(p => p.Levels == currLevel.ToString() && (!string.IsNullOrWhiteSpace(p.Approver) && p.Approver.Split(',').Contains(userEmail))).ApproveBy = userEmail;
                                }
                                break;
                            case ButtonActionStatus.Complete:
                                if (approvers.Any(p => p.Levels == currLevel.ToString() && !string.IsNullOrWhiteSpace(p.Approver) && p.Approver.Split(',').Contains(userEmail)))
                                {
                                    approvers.FirstOrDefault(p => p.Levels == currLevel.ToString() && (!string.IsNullOrWhiteSpace(p.Approver) && p.Approver.Split(',').Contains(userEmail))).Status = ApproverStatus.APPROVED;
                                    approvers.FirstOrDefault(p => p.Levels == currLevel.ToString() && (!string.IsNullOrWhiteSpace(p.Approver) && p.Approver.Split(',').Contains(userEmail))).ApprovalDate = DateTime.Now;
                                    approvers.FirstOrDefault(p => p.Levels == currLevel.ToString() && (!string.IsNullOrWhiteSpace(p.Approver) && p.Approver.Split(',').Contains(userEmail))).ApproveBy = userEmail;
                                }
                                break;
                            case ButtonActionStatus.MeetingConducted:
                            case ButtonActionStatus.MeetingNotConducted:
                            default:
                                break;
                        }
                    }
                }
            }
            return approvers;
        }

        /// <summary>
        /// Gets the due date.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="days">The days.</param>
        /// <returns>End Date</returns>
        private DateTime GetDueDate(DateTime startDate, int days)
        {
            string strHolidays = this.GetConfigVariable("HolidaysList");
            string[] holidays = strHolidays.Split(',');
            ////Count from Next Day
            startDate = startDate.AddDays(1);
            for (int i = 0; i < days; i++)
            {
                DateTime date = startDate.AddDays(i);
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                    case DayOfWeek.Sunday:
                        days++;
                        break;
                    default:
                        if (holidays.Contains(date.ToString("dd/MM")))
                        {
                            days++;
                        }
                        break;
                }
            }
            return startDate.AddDays(days - 1);
        }

        /// <summary>
        /// Count Days
        /// </summary>
        /// <param name="day">Day of week</param>
        /// <param name="start">Start Date</param>
        /// <param name="end">End Date</param>
        /// <returns>Day Count</returns>
        public static int CountDays(DayOfWeek day, DateTime start, DateTime end)
        {
            TimeSpan ts = end - start;                       // Total duration
            int count = (int)Math.Floor(ts.TotalDays / 7);   // Number of whole weeks
            int remainder = (int)(ts.TotalDays % 7);         // Number of remaining days
            int sinceLastDay = (int)(end.DayOfWeek - day);   // Number of days since last [day]
            if (sinceLastDay < 0)
            {
                sinceLastDay += 7;
                //// Adjust for negative days since last [day]
            }
            //// If the days in excess of an even week are greater than or equal to the number days since the last [day], then count this one, too.
            if (remainder >= sinceLastDay)
            {
                count++;
            }
            return count;
        }

        /// <summary>
        /// Gets the permission dictionary.
        /// </summary>
        /// <param name="approvers">The approvers.</param>
        /// <param name="curLevel">The current level.</param>
        /// <param name="preLevel">The pre level.</param>
        /// <param name="isAllUserViewer">if set to <c>true</c> [is all user viewer].</param>
        /// <returns>
        /// Get Permission Dictionary
        /// </returns>
        public Dictionary<string, string> GetPermissionDictionary(List<ApplicationStatus> approvers, int curLevel, int preLevel, bool isAllUserViewer)
        {
            Dictionary<string, string> permissions = new Dictionary<string, string>();
            if (approvers != null && approvers.Count != 0)
            {
                string strReader = string.Empty, strContributer = string.Empty;
                approvers.ForEach(p =>
                {
                    if (!string.IsNullOrEmpty(p.Approver))
                    {
                        if (permissions.Count(x => x.Key == p.Approver) == 0)
                        {
                            if (p.Levels == curLevel.ToString() && p.Status == ApproverStatus.PENDING)
                            {
                                /* All users 
                                 * 1) who are pending on current level
                                 */
                                if (!strContributer.Contains(p.Approver))
                                {
                                    strContributer = strContributer.Trim(',') + "," + p.Approver;
                                }
                            }
                            ////Phase 2 :All members who will be in the DCR Process should be able to know the status of all DCR/DCN. 
                            //// else if (Convert.ToInt32(p.Levels) <= preLevel || (p.Levels == curLevel.ToString() && p.Status != ApproverStatus.PENDING))
                            else if (p.Status != ApproverStatus.PENDING)
                            {
                                /* All users 
                                 * 1) who are less then previous level
                                 * 2) who are not pending on current level
                                 */
                                if (!strReader.Contains(p.Approver))
                                {
                                    strReader = strReader.Trim(',') + "," + p.Approver;
                                }
                            }
                        }
                    }
                });
                if (strReader.Trim(',') == strContributer.Trim(','))
                {
                    permissions.Add(strContributer.Trim(','), isAllUserViewer ? SharePointPermission.READER : SharePointPermission.CONTRIBUTOR);
                }
                else
                {
                    if (isAllUserViewer)
                    {
                        permissions.Add(strReader.Trim(',') + "," + strContributer.Trim(','), SharePointPermission.READER);
                    }
                    else
                    {
                        permissions.Add(strReader.Trim(','), SharePointPermission.READER);
                        permissions.Add(strContributer.Trim(','), isAllUserViewer ? SharePointPermission.READER : SharePointPermission.CONTRIBUTOR);
                    }
                }
            }
            return permissions;
        }
        #endregion

        /// <summary>
        /// Gets the application approval matrix users email.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="approverListName">Name of the approver list.</param>
        /// <param name="fromLevel">From level.</param>
        /// <param name="toLevel">To level.</param>
        /// <returns>
        /// User Emails
        /// </returns>
        public string GetAppApprovalMatrixUsersEmail(ClientContext context, Web web, int requestId, string approverListName, string fromLevel = "", string toLevel = "")
        {
            string userEmails = string.Empty;
            if (context != null && web != null && requestId > 0 && !string.IsNullOrEmpty(approverListName))
            {
                List spList = web.Lists.GetByTitle(approverListName);
                CamlQuery query = new CamlQuery();
                string qry = @"<Eq>
                                    <FieldRef Name='RequestID' LookupId='TRUE'/>
                                    <Value Type='Lookup'>" + requestId + @"</Value>
                               </Eq>";
                if (!string.IsNullOrEmpty(fromLevel))
                {
                    qry = @"<And>" + qry + @"
                                <Geq>
                                    <FieldRef Name='Levels' />
                                    <Value Type='Text'>" + fromLevel + @"</Value>
                                </Geq>
                            </And>";
                }
                if (!string.IsNullOrEmpty(toLevel))
                {
                    qry = @"<And>" + qry + @"
                                <Leq>
                                    <FieldRef Name='Levels' />
                                    <Value Type='Text'>" + toLevel + @"</Value>
                                </Leq>
                            </And>";
                }
                query.ViewXml = @"<View>
                                    <Query>
                                        <Where>
                                            " + qry + @"
                                        </Where>
                                    </Query>
                                </View>";
                ListItemCollection items = spList.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();
                if (items.Count > 0)
                {
                    foreach (ListItem item in items)
                    {
                        userEmails = userEmails.Trim(',') + "," + BELDataAccessLayer.GetEmailsFromPersonField(context, web, item["Approvers"] as FieldUserValue[]);
                    }
                }
            }
            return userEmails.Trim(',');
        }
    }
}