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
    using Newtonsoft.Json.Linq;
    using Microsoft.SharePoint.Client.UserProfiles;

    /// <summary>
    /// Master data Helper Class
    /// </summary>
    public class MasterDataHelper : BELDataAccessLayer
    {
        /// <summary>
        /// Get Master data for Dropdown, checkbox, Radiobutton field
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="masters">The master names.</param>
        /// <returns>
        /// list of master
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Will do later"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Necessary to have common code")]
        public List<IMaster> GetMasterData(ClientContext context, Web web, List<IMaster> masters)
        {
            List<IMaster> masterDataList = new List<IMaster>();
            if (context != null && web != null && masters != null && masters.Count != 0)
            {
                //string[] strMasterNames = masterNames.Split(',');
                //  for (int i = 0; i < strMasterNames.Length; i++)
                foreach (IMaster mstr in masters)
                {
                    IMaster master = null;
                    if (this.GetConfigVariable("UseRESTAPI").ToLower().Equals("true"))
                    {
                        master = this.GetMasterDataOfUsingREST(context, web, mstr);
                    }
                    else
                    {
                        master = this.GetMasterDataOf(context, web, mstr);
                    }
                    masterDataList.Add(master);
                }
            }
            return masterDataList;
        }

        /// <summary>
        /// Gets the master data of.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="master">The master.</param>
        /// <returns>
        /// master object
        /// </returns>
        private IMaster GetMasterDataOf(ClientContext context, Web web, IMaster master)
        {
            dynamic objMaster = Convert.ChangeType(master, master.GetType());
            string listName = Convert.ToString(((PropertyInfo)objMaster.GetType().GetProperty("ListName")).GetValue(objMaster));
            objMaster = GlobalCachingProvider.Instance.GetItem(listName, false) as IMaster;
            if (objMaster == null)
            {
                objMaster = Convert.ChangeType(master, master.GetType());
                string scope = Convert.ToString(((PropertyInfo)objMaster.GetType().GetProperty("Scope")).GetValue(objMaster));
                Type itemType = ((PropertyInfo)objMaster.GetType().GetProperty("ItemType")).GetValue(objMaster);
                PropertyInfo[] properties = itemType.GetProperties();
                string viewFields = this.GetViewFields(properties);
                if (scope.Equals(ListScope.GLOBAL))
                {
                    string siteURL = this.GetSiteURL(SiteURLs.ROOTSITEURL); //this.GetConfigVariable(SiteURLs.ROOTSITEURL);
                    context = this.CreateClientContext(siteURL);
                    web = this.CreateWeb(context);
                }
                List spList = web.Lists.GetByTitle(listName);
                CamlQuery qry = new CamlQuery();
                qry.ViewXml = @"<View><Query></Query>" + viewFields + "</View>";
                ListItemCollection items = spList.GetItems(CamlQuery.CreateAllItemsQuery());
                context.Load(items);
                context.ExecuteQuery();
                objMaster = this.AssignPropertyValues(objMaster, context, web, items, itemType);
                GlobalCachingProvider.Instance.AddItem(listName, objMaster);
            }

            return objMaster;
        }

        /// <summary>
        /// Gets the master data of using rest.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="master">The master.</param>
        /// <returns>IMaster object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "NA")]
        private IMaster GetMasterDataOfUsingREST(ClientContext context, Web web, IMaster master)
        {
            dynamic objMaster = Convert.ChangeType(master, master.GetType());
            string listName = Convert.ToString(((PropertyInfo)objMaster.GetType().GetProperty("ListName")).GetValue(objMaster));
            objMaster = GlobalCachingProvider.Instance.GetItem(listName, false) as IMaster;
            if (objMaster == null)
            {
                string siteURL = web.Url;
                objMaster = Convert.ChangeType(master, master.GetType());
                string scope = Convert.ToString(((PropertyInfo)objMaster.GetType().GetProperty("Scope")).GetValue(objMaster));
                Type itemType = ((PropertyInfo)objMaster.GetType().GetProperty("ItemType")).GetValue(objMaster);
                if (scope.Equals(ListScope.GLOBAL))
                {
                    siteURL = this.GetSiteURL(SiteURLs.ROOTSITEURL); //this.GetConfigVariable(SiteURLs.ROOTSITEURL);
                }
                dynamic listItem = Activator.CreateInstance(itemType);
                PropertyInfo[] itemProperties = listItem.GetType().GetProperties();
                string selectClause = string.Empty;
                string expandClause = string.Empty;
                foreach (PropertyInfo property in itemProperties)
                {
                    if (property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation))
                    {
                        string propertyName = property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation;
                        if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson)
                        {
                            selectClause = selectClause.Trim(',') + "," + propertyName + "/EMail" + "," + propertyName + "/Title";
                            expandClause = expandClause.Trim(',') + "," + propertyName;
                        }
                        else if (property.GetCustomAttribute<FieldColumnNameAttribute>().IsLookup)
                        {
                            string lookupFieldName = property.GetCustomAttribute<FieldColumnNameAttribute>().LookupFieldName;
                            selectClause = selectClause.Trim(',') + "," + propertyName + "/ID" + "," + propertyName + "/" + lookupFieldName;
                            expandClause = expandClause.Trim(',') + "," + propertyName;
                        }
                        else
                        {
                            selectClause = selectClause.Trim(',') + "," + propertyName;
                        }
                    }
                }
                JObject jobj = RESTHelper.GetDataUsingRest(siteURL + "/_api/web/lists/GetByTitle('" + listName + "')/Items?$select=" + selectClause.Trim(',') + "&$expand=" + expandClause.Trim(',') + "&$top=" + int.MaxValue, "GET");
                JArray jarr = (JArray)jobj["d"]["results"];
                objMaster = this.AssignPropertyValuesREST(objMaster, context, web, jarr, itemType, itemProperties);
                GlobalCachingProvider.Instance.AddItem(listName, objMaster);
            }
            return objMaster;
        }

        /// <summary>
        /// Assigns the property values.
        /// </summary>
        /// <param name="objMaster">The object master.</param>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="items">The items.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <returns>
        /// master object
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "NA")]
        private IMaster AssignPropertyValues(dynamic objMaster, ClientContext context, Web web, ListItemCollection items, Type itemType)
        {
            List<IMasterItem> masterItems = new List<IMasterItem>();
            foreach (ListItem item in items)
            {
                dynamic listItem = Activator.CreateInstance(itemType);
                PropertyInfo[] itemProperties = listItem.GetType().GetProperties();
                foreach (PropertyInfo property in itemProperties)
                {
                    if (property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation))
                    {
                        if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson && !property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                        {
                            if (item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation] is FieldUserValue[])
                            {
                                property.SetValue(listItem, BELDataAccessLayer.GetEmailsFromPersonField(context, web, (FieldUserValue[])item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation]));
                            }
                            else
                            {
                                property.SetValue(listItem, BELDataAccessLayer.GetEmailsFromPersonField(context, web, (FieldUserValue)item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation]));
                            }
                        }
                        else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson && property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                        {
                            if (item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation] is FieldUserValue[])
                            {
                                property.SetValue(listItem, BELDataAccessLayer.GetNameFromPersonField(context, web, (FieldUserValue[])item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation]));
                            }
                            else
                            {
                                property.SetValue(listItem, BELDataAccessLayer.GetNameFromPersonField(context, web, (FieldUserValue)item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation]));
                            }
                        }
                        else if (property.GetCustomAttribute<FieldColumnNameAttribute>().IsLookup)
                        {
                            if (item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation] is FieldLookupValue[])
                            {
                                FieldLookupValue[] lookupFields = item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation] as FieldLookupValue[];
                                List<string> reletedto = new List<string>();
                                foreach (FieldLookupValue lookupField in lookupFields)
                                {
                                    reletedto.Add(lookupField.LookupValue);
                                }
                                property.SetValue(listItem, reletedto);
                            }
                            else
                            {
                                FieldLookupValue lookupField = item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation] as FieldLookupValue;
                                if (lookupField != null)
                                {
                                    property.SetValue(listItem, lookupField.LookupValue);
                                }
                            }
                        }
                        else
                        {
                            property.SetValue(listItem, item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation]);
                        }
                    }
                }
                masterItems.Add(listItem);
            }
            PropertyInfo prop = objMaster.GetType().GetProperty("Items");
            prop.SetValue(objMaster, masterItems, null);
            return objMaster;
        }

        /// <summary>
        /// Assigns the property values rest.
        /// </summary>
        /// <param name="objMaster">The object master.</param>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="items">The items.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="itemProperties">The item properties.</param>
        /// <returns>
        /// master object
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "NA")]
        private IMaster AssignPropertyValuesREST(dynamic objMaster, ClientContext context, Web web, JArray items, Type itemType, PropertyInfo[] itemProperties)
        {
            List<IMasterItem> masterItems = new List<IMasterItem>();
            foreach (JToken item in items)
            {
                dynamic listItem = Activator.CreateInstance(itemType);
                foreach (PropertyInfo property in itemProperties)
                {
                    if (property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation))
                    {
                        string propertyName = property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation;
                        if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson && !property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                        {
                            string personEmail = string.Empty;
                            if (property.GetCustomAttribute<IsPersonAttribute>().IsMultiple)
                            {
                                if (((JContainer)item[propertyName])["results"] != null)
                                {
                                    foreach (dynamic person in ((JContainer)item[propertyName])["results"])
                                    {
                                        personEmail = personEmail.Trim(',') + "," + person["EMail"].ToString();
                                    }
                                }
                            }
                            else
                            {
                                if (((dynamic)item[propertyName]).EMail != null)
                                {
                                    personEmail = ((dynamic)item[propertyName])["EMail"].ToString();
                                }
                            }
                            property.SetValue(listItem, personEmail.Trim(','));
                        }
                        else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson && property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                        {
                            string personName = string.Empty;
                            if (property.GetCustomAttribute<IsPersonAttribute>().IsMultiple)
                            {
                                if (((JContainer)item[propertyName])["results"] != null)
                                {
                                    foreach (dynamic person in ((JContainer)item[propertyName])["results"])
                                    {
                                        personName = personName.Trim(',') + "," + person["Title"].ToString();
                                    }
                                }
                            }
                            else
                            {
                                if (((dynamic)item[propertyName]).Title != null)
                                {
                                    personName = ((dynamic)item[propertyName])["Title"].ToString();
                                }
                            }
                            property.SetValue(listItem, personName.Trim(','));
                        }
                        else if (property.GetCustomAttribute<FieldColumnNameAttribute>().IsLookup)
                        {
                            string lookupFieldName = property.GetCustomAttribute<FieldColumnNameAttribute>().LookupFieldName;
                            if (property.GetCustomAttribute<FieldColumnNameAttribute>().IsMultipleLookup)
                            {
                                if (((JContainer)item[propertyName])["results"] != null)
                                {
                                    List<string> reletedto = new List<string>();
                                    foreach (dynamic lookup in ((JContainer)item[propertyName])["results"])
                                    {
                                        reletedto.Add(lookup[lookupFieldName].ToString());
                                    }
                                    property.SetValue(listItem, reletedto);
                                }
                            }
                            else
                            {
                                if (((dynamic)item[propertyName])[lookupFieldName] != null)
                                {
                                    property.SetValue(listItem, ((dynamic)item[propertyName])[lookupFieldName].ToString());
                                }
                            }
                        }
                        else
                        {
                            if (property.PropertyType.FullName == typeof(int).FullName || property.PropertyType.FullName == typeof(int?).FullName)
                            {
                                property.SetValue(listItem, Convert.ToInt32(item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation].ToString()));
                            }
                            else if (property.PropertyType.FullName == typeof(double).FullName || property.PropertyType.FullName == typeof(double?).FullName)
                            {
                                property.SetValue(listItem, Convert.ToDouble(item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation].ToString()));
                            }
                            else if (property.PropertyType.FullName == typeof(DateTime).FullName || property.PropertyType.FullName == typeof(DateTime?).FullName)
                            {
                                if (item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation].ToString().Length != 0)
                                {
                                    property.SetValue(listItem, Convert.ToDateTime(item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation].ToString()));
                                }
                            }
                            else
                            {
                                property.SetValue(listItem, item[property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation].ToString());
                            }
                        }
                    }
                }
                masterItems.Add(listItem);
            }
            PropertyInfo prop = objMaster.GetType().GetProperty("Items");
            prop.SetValue(objMaster, masterItems, null);
            return objMaster;
        }

        /// <summary>
        /// Gets the view fields.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>
        /// string value
        /// </returns>
        private string GetViewFields(PropertyInfo[] properties)
        {
            StringBuilder viewFields = new StringBuilder("<ViewFields>");
            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation))
                {
                    viewFields.Append("<FieldRef Name='" + property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation + "' />");
                }
            }
            viewFields.Append("</ViewFields>");
            return viewFields.ToString();
        }

        /// <summary>
        /// Get All Employee with Detail
        /// </summary>
        /// <param name="context">Context Object</param>
        /// <param name="web">Web Object</param>
        /// <returns>List of User Detail</returns>
        public List<UserDetails> GetAllEmployee(ClientContext context, Web web)
        {
            List<UserDetails> userInfoList = GlobalCachingProvider.Instance.GetItem(web.ServerRelativeUrl + "/" + ListNames.EmployeeMasterList, false) as List<UserDetails>;
            if (userInfoList == null)
            {
                var siteUsers = from user in web.SiteUsers
                                where user.PrincipalType == Microsoft.SharePoint.Client.Utilities.PrincipalType.User
                                select user;
                var usersResult = context.LoadQuery(siteUsers);
                context.ExecuteQuery();

                var peopleManager = new PeopleManager(context);
                var userProfilesResult = new List<PersonProperties>();
                foreach (var user in usersResult)
                {
                    var userProfile = peopleManager.GetPropertiesFor(user.LoginName);
                    context.Load(userProfile);
                    userProfilesResult.Add(userProfile);
                }
                context.ExecuteQuery();

                if (userProfilesResult != null && userProfilesResult.Count != 0)
                {
                    var result = from userProfile in userProfilesResult
                                 where userProfile.ServerObjectIsNull != null && userProfile.ServerObjectIsNull.Value != true
                                 select new UserDetails()
                                 {
                                     UserId = usersResult.Where(p => p.LoginName == userProfile.AccountName).FirstOrDefault() != null ? usersResult.Where(p => p.LoginName == userProfile.AccountName).FirstOrDefault().Id.ToString() : string.Empty,
                                     FullName = userProfile.DisplayName,
                                     UserEmail = userProfile.Email,
                                     LoginName = userProfile.AccountName,
                                     Department = userProfile.UserProfileProperties.ContainsKey("Department") ? Convert.ToString(userProfile.UserProfileProperties["Department"]) : string.Empty,
                                     ReportingManager = userProfile.UserProfileProperties.ContainsKey("Manager") ? Convert.ToString(userProfile.UserProfileProperties["Manager"]) : string.Empty
                                 };
                    userInfoList = result.ToList();
                    GlobalCachingProvider.Instance.AddItem(web.ServerRelativeUrl + "/" + ListNames.EmployeeMasterList, userInfoList);
                }
            }
            return userInfoList;
        }
    }
}