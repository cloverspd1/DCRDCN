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
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Transaction Helper Class
    /// </summary>
    public class TransactionHelper : BELDataAccessLayer
    {
        /// <summary>
        /// Lazy Instance
        /// </summary>
        private static readonly Lazy<TransactionHelper> Lazy =
          new Lazy<TransactionHelper>(() => new TransactionHelper());

        /// <summary>
        /// Gets the Instance.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        public static TransactionHelper Instance
        {
            get
            {
                return Lazy.Value;
            }
        }

        #region "Get Tran Data"
        /// <summary>
        /// Gets the transaction list data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="tranType">Type of the tran.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="lookupID">The lookup identifier.</param>
        /// <returns>List of Transaction Data</returns>
        public List<ITrans> GetTransactionListData(ClientContext context, Web web, Type tranType, string listName, int lookupID)
        {
            List<ITrans> tranData = new List<ITrans>();
            if (context != null && web != null && tranType != null && !string.IsNullOrEmpty(listName) && lookupID > 0)
            {
                if (!string.IsNullOrEmpty(listName))
                {
                    List spList = web.Lists.GetByTitle(listName);
                    CamlQuery qry = new CamlQuery();
                    qry.ViewXml = @"<View><Query><Where><And>
                                                     <Eq>
                                                      <FieldRef Name='RequestID' LookupId = 'TRUE'  />
                                                      <Value Type='Lookup' >" + lookupID + @"</Value>
                                                     </Eq>
                                                     <Neq>
                                                      <FieldRef Name='Status'   />
                                                      <Value Type='Text'>" + TaskStatus.DELETED + @"</Value>
                                                     </Neq>
                                                      </And>
                                                     </Where>
                                                </Query></View>";
                    ListItemCollection items = spList.GetItems(qry);
                    context.Load(items);
                    context.ExecuteQuery();
                    tranData = this.AssignTranPropertyValues(context, web, items, tranType);
                }
            }
            return tranData;
        }

        /// <summary>
        /// Gets the transaction list data CSOM.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="tranType">Type of the tran.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="lookupID">The lookup identifier.</param>
        /// <returns>List of Transaction Data</returns>
        public List<ITrans> GetTransactionListDataUsingCSOM(ClientContext context, Web web, Type tranType, string listName, int lookupID)
        {
            List<ITrans> tranData = new List<ITrans>();
            if (context != null && web != null && tranType != null && !string.IsNullOrEmpty(listName) && lookupID > 0)
            {
                if (!string.IsNullOrEmpty(listName))
                {
                    List spList = web.Lists.GetByTitle(listName);
                    CamlQuery qry = new CamlQuery();
                    qry.ViewXml = @"<View><Query><Where><And>
                                                     <Eq>
                                                      <FieldRef Name='RequestID' LookupId = 'TRUE'  />
                                                      <Value Type='Lookup' >" + lookupID + @"</Value>
                                                     </Eq>
                                                     <Neq>
                                                      <FieldRef Name='Status'   />
                                                      <Value Type='Text'>" + TaskStatus.DELETED + @"</Value>
                                                     </Neq>
                                                      </And>
                                                     </Where>
                                                </Query></View>";
                    ListItemCollection items = spList.GetItems(qry);
                    context.Load(items);
                    context.ExecuteQuery();
                    tranData = this.AssignTranPropertyValues(context, web, items, tranType);
                }
            }
            return tranData;
        }

        /// <summary>
        /// Gets the transaction list data REST.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="tranType">Type of the tran.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="lookupID">The lookup identifier.</param>
        /// <returns>List of Transaction Data</returns>
        public List<ITrans> GetTransactionListDataUsingREST(ClientContext context, Web web, Type tranType, string listName, int lookupID)
        {
            List<ITrans> tranData = new List<ITrans>();
            if (context != null && web != null && tranType != null && !string.IsNullOrEmpty(listName) && lookupID > 0)
            {
                if (!string.IsNullOrEmpty(listName))
                {
                    PropertyInfo[] propertyInfo = tranType.GetProperties();
                    string selectClause = "ID";
                    string expandClause = string.Empty;
                    foreach (PropertyInfo property in propertyInfo)
                    {
                        bool isListColumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                        string propertyName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                        bool isFile = property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile;
                        bool isAppend = property.GetCustomAttribute<IsAppendFieldAttribute>() != null && property.GetCustomAttribute<IsAppendFieldAttribute>().IsAppendField;
                        if (isListColumn)
                        {
                            if (property.GetCustomAttribute<FieldColumnNameAttribute>() != null && property.GetCustomAttribute<FieldColumnNameAttribute>().IsLookup)
                            {
                                string lookupFieldName = property.GetCustomAttribute<FieldColumnNameAttribute>().LookupFieldNameForTrans;
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
                            else if (isAppend)
                            {
                                selectClause = selectClause.Trim(',') + "," + propertyName + "," + propertyName + "History";
                            }
                            else
                            {
                                selectClause = selectClause.Trim(',') + "," + propertyName;
                            }
                        }
                    }
                    JObject jobj = RESTHelper.GetDataUsingRest(web.Url + "/_api/web/lists/GetByTitle('" + listName + "')/Items?$select=" + selectClause.Trim(',') + "&$expand=" + expandClause.Trim(',') + "&$filter=RequestID eq '" + lookupID + "' and Status ne '" + TaskStatus.DELETED + "'&$top=" + int.MaxValue, "GET");
                    JArray jarr = (JArray)jobj["d"]["results"];
                    tranData = this.AssignTranPropertyValuesREST(context, web, jarr, tranType);
                }
            }
            return tranData;
        }

        /// <summary>
        /// Assigns the tran property values.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="items">The items.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <returns>List of Transaction data</returns>
        private List<ITrans> AssignTranPropertyValues(ClientContext context, Web web, ListItemCollection items, Type itemType)
        {
            List<ITrans> tranItems = new List<ITrans>();
            foreach (ListItem item in items)
            {
                ITrans tranItem = Activator.CreateInstance(itemType) as ITrans;
                tranItem = this.SetTranProperties(context, web, item, tranItem, tranItem.GetType().GetProperties());
                tranItem.ItemAction = ItemActionStatus.NOCHANGE;
                tranItem.ID = Convert.ToInt32(item["ID"]);
                tranItem.Index = tranItems.Count + 1;
                tranItems.Add(tranItem);
            }
            return tranItems;
        }

        /// <summary>
        /// Sets the tran properties.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="listItem">The list item.</param>
        /// <param name="tranItem">The tran item.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="getSubItems">if set to <c>true</c> [get sub items].</param>
        /// <returns>
        /// Transaction item
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Cant Manage")]
        private ITrans SetTranProperties(ClientContext context, Web web, ListItem listItem, ITrans tranItem, PropertyInfo[] properties, bool getSubItems = true)
        {
            foreach (PropertyInfo property in properties)
            {
                bool isListColumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                bool isTask = property.GetCustomAttribute<IsTaskAttribute>() != null && property.GetCustomAttribute<IsTaskAttribute>().IsTaskField;
                bool isTran = property.GetCustomAttribute<IsTranAttribute>() != null && property.GetCustomAttribute<IsTranAttribute>().IsTranField;
                string listCoumnName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                bool isFile = property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile;
                if (isListColumn)
                {
                    if (property.GetCustomAttribute<FieldColumnNameAttribute>() != null && property.GetCustomAttribute<FieldColumnNameAttribute>().IsLookup)
                    {
                        FieldLookupValue lookupField = listItem[listCoumnName] as FieldLookupValue;
                        property.SetValue(tranItem, lookupField.LookupId);
                    }
                    else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson)
                    {
                        FieldUserValue[] users = null;
                        if (listItem[listCoumnName] != null)
                        {
                            if (property.GetCustomAttribute<IsPersonAttribute>().IsMultiple)
                            {
                                users = listItem[listCoumnName] as FieldUserValue[];
                            }
                            else
                            {
                                users = new FieldUserValue[1];
                                users[0] = listItem[listCoumnName] as FieldUserValue;
                            }
                        }
                        if (users != null && !property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                        {
                            string personEmails = BELDataAccessLayer.GetEmailsFromPersonField(context, web, users);
                            property.SetValue(tranItem, personEmails);
                        }
                        else if (users != null && property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                        {
                            string personEmails = BELDataAccessLayer.GetNameFromPersonField(context, web, users);
                            property.SetValue(tranItem, personEmails);
                        }
                    }
                    else if (isFile)
                    {
                        if (Convert.ToString(listItem["Attachments"]) == "True")
                        {
                            context.Load(listItem.AttachmentFiles);
                            context.ExecuteQuery();
                            AttachmentCollection attachments = listItem.AttachmentFiles;
                            List<FileDetails> objAttachmentFiles = this.GetAttachments(attachments);
                            property.SetValue(tranItem, objAttachmentFiles);
                        }
                    }
                    else
                    {
                        object value = listItem[listCoumnName];
                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                        property.SetValue(tranItem, safeValue);
                    }
                }
                else if (isTran)
                {
                    if (getSubItems)
                    {
                        string listName = property.GetCustomAttribute<IsTranAttribute>().TranListName;
                        Type tSubTran = property.GetCustomAttribute<IsTranAttribute>().TranType;
                        if (!string.IsNullOrEmpty(listName))
                        {
                            List<ITrans> subTrans = this.GetTransactionListData(context, web, tSubTran, listName, Convert.ToInt32(listItem["ID"]));
                            property.SetValue(tranItem, subTrans);
                        }
                    }
                }
                ////else if (isTask)
                ////{
                ////    string listName = property.GetCustomAttribute<IsTaskAttribute>().TaskListName;
                ////    Type tSubTask = property.GetCustomAttribute<IsTaskAttribute>().TaskType;
                ////    if (!string.IsNullOrEmpty(listName))
                ////    {
                ////        List<ITask> subTasks = this.GetTaskListData(context, web, tSubTask, listName, Convert.ToInt32(listItem["ID"]));
                ////        property.SetValue(taskItem, subTasks);
                ////    }
                ////}
            }
            return tranItem;
        }

        /// <summary>
        /// Assigns the tran property values REST.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="items">The items.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <returns>List of Transaction data</returns>
        private List<ITrans> AssignTranPropertyValuesREST(ClientContext context, Web web, JArray items, Type itemType)
        {
            List<ITrans> tranItems = new List<ITrans>();
            foreach (JToken item in items)
            {
                ITrans tranItem = Activator.CreateInstance(itemType) as ITrans;
                tranItem = this.SetTranPropertiesREST(context, web, item, tranItem, tranItem.GetType().GetProperties());
                tranItem.ItemAction = ItemActionStatus.NOCHANGE;
                tranItem.ID = Convert.ToInt32(item["ID"]);
                tranItem.Index = tranItems.Count + 1;
                tranItems.Add(tranItem);
            }
            return tranItems;
        }

        /// <summary>
        /// Sets the tran properties REST.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="listItem">The list item.</param>
        /// <param name="tranItem">The tran item.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="getSubItems">if set to <c>true</c> [get sub items].</param>
        /// <returns>
        /// Transaction item
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Cant Manage")]
        private ITrans SetTranPropertiesREST(ClientContext context, Web web, JToken listItem, ITrans tranItem, PropertyInfo[] properties, bool getSubItems = true)
        {
            BELDataAccessLayer helper = new BELDataAccessLayer();
            foreach (PropertyInfo property in properties)
            {
                try
                {
                    bool isListColumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                    bool isTask = property.GetCustomAttribute<IsTaskAttribute>() != null && property.GetCustomAttribute<IsTaskAttribute>().IsTaskField;
                    bool isTran = property.GetCustomAttribute<IsTranAttribute>() != null && property.GetCustomAttribute<IsTranAttribute>().IsTranField;
                    string listCoumnName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                    bool isFile = property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile;
                    if (isListColumn)
                    {
                        if (property.GetCustomAttribute<FieldColumnNameAttribute>() != null && property.GetCustomAttribute<FieldColumnNameAttribute>().IsLookup)
                        {
                            string lookupFieldName = property.GetCustomAttribute<FieldColumnNameAttribute>().LookupFieldNameForTrans;
                            if (property.GetCustomAttribute<FieldColumnNameAttribute>().IsMultipleLookup)
                            {
                                if (((JContainer)listItem[listCoumnName])["results"] != null)
                                {
                                    List<string> reletedto = new List<string>();
                                    foreach (dynamic lookup in ((JContainer)listItem[listCoumnName])["results"])
                                    {
                                        reletedto.Add(lookup[lookupFieldName].ToString());
                                    }
                                    property.SetValue(tranItem, reletedto);
                                }
                            }
                            else
                            {
                                if (((dynamic)listItem[listCoumnName])[lookupFieldName] != null)
                                {
                                    object value = ((JValue)((dynamic)listItem[listCoumnName])[lookupFieldName]).Value;
                                    Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                    object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                                    property.SetValue(tranItem, safeValue);
                                }
                            }
                        }
                        else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson)
                        {
                            if (property.GetCustomAttribute<IsPersonAttribute>().IsPerson)
                            {
                                string personEmail = string.Empty;
                                if (!property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                                {
                                    if (((JContainer)listItem[listCoumnName])["results"] != null)
                                    {
                                        foreach (dynamic person in ((JContainer)listItem[listCoumnName])["results"])
                                        {
                                            personEmail = personEmail.Trim(',') + "," + person["EMail"].ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    if (((dynamic)listItem[listCoumnName]).EMail != null)
                                    {
                                        personEmail = ((dynamic)listItem[listCoumnName])["EMail"].ToString();
                                    }
                                }
                                property.SetValue(tranItem, personEmail.Trim(','));
                            }
                            else if (property.GetCustomAttribute<IsPersonAttribute>().ReturnName)
                            {
                                string personEmail = string.Empty;
                                if (property.GetCustomAttribute<IsPersonAttribute>().IsMultiple)
                                {
                                    if (((JContainer)listItem[listCoumnName])["results"] != null)
                                    {
                                        foreach (dynamic person in ((JContainer)listItem[listCoumnName])["results"])
                                        {
                                            personEmail = personEmail.Trim(',') + "," + person["Title"].ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    if (((dynamic)listItem[listCoumnName]).Title != null)
                                    {
                                        personEmail = ((dynamic)listItem[listCoumnName])["Title"].ToString();
                                    }
                                }
                                property.SetValue(tranItem, personEmail.Trim(','));
                            }
                        }
                        else if (isFile)
                        {
                            if (((dynamic)((dynamic)listItem["AttachmentFiles"])).__deferred != null)
                            {
                                string url = ((dynamic)((dynamic)listItem["AttachmentFiles"])).__deferred.uri.Value;
                                List<FileDetails> objAttachmentFiles = helper.GetAttachmentsUsingREST(url);
                                property.SetValue(tranItem, objAttachmentFiles);
                            }
                        }
                        else
                        {
                            object value = ((JValue)listItem[listCoumnName]).Value;
                            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                            property.SetValue(tranItem, safeValue);
                        }
                    }
                    else if (isTran)
                    {
                        if (getSubItems)
                        {
                            string listName = property.GetCustomAttribute<IsTranAttribute>().TranListName;
                            Type tSubTran = property.GetCustomAttribute<IsTranAttribute>().TranType;
                            if (!string.IsNullOrEmpty(listName))
                            {
                                List<ITrans> subTrans = this.GetTransactionListData(context, web, tSubTran, listName, Convert.ToInt32(listItem["ID"]));
                                property.SetValue(tranItem, subTrans);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("Error While Save tran Item  - Message:{0}, StackTrace: {1}", ex.Message, ex.StackTrace));
                }
            }
            return tranItem;
        }
        #endregion

        #region "Save Tran Data"

        /// <summary>
        /// Saves the tran items.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="objParams">The object parameters.</param>
        /// <returns>
        /// true or false
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Optimize later on")]
        public bool SaveTranItems(ClientContext context, Web web, List<ITrans> trans, string listName, Dictionary<string, string> objParams)
        {
            if (context != null && web != null && trans != null)
            {
                try
                {
                    bool hasFile = false;
                    trans.RemoveAll(p => p.ItemAction == ItemActionStatus.NOCHANGE);
                    ListItem[] tranItems = new ListItem[trans != null ? trans.Count : 0];
                    int i = 0;
                    List list = web.Lists.GetByTitle(listName);
                    foreach (ITrans item in trans)
                    {
                        try
                        {
                            if (item.ID > 0)
                            {
                                tranItems[i] = list.GetItemById(item.ID);
                            }
                            else
                            {
                                tranItems[i] = list.AddItem(new ListItemCreationInformation());
                            }

                            if (item.ItemAction == ItemActionStatus.DELETED)
                            {
                                tranItems[i]["Status"] = ItemStatus.DELETED;
                                tranItems[i].Update();
                            }
                            else
                            {
                                hasFile = this.UpdateTranItem(context, web, ref tranItems[i], item, objParams);
                            }
                            if (!hasFile)
                            {
                                context.Load(tranItems[i]);
                            }
                            i++;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(string.Format("Error While Save Tran item  - Tran ID:{0}, Listname:{1}  Message:{2}, StackTrace: {3}", item.ID, listName, ex.Message, ex.StackTrace));
                        }
                    }

                    context.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("Error While Save All Item  - Message:{0}, StackTrace: {1}", ex.Message, ex.StackTrace));
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Updates the tran item.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="item">The item.</param>
        /// <param name="tran">The tran.</param>
        /// <param name="objParams">The object parameters.</param>
        /// <returns>
        /// return true false
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Cannot do")]
        private bool UpdateTranItem(ClientContext context, Web web, ref ListItem item, ITrans tran, Dictionary<string, string> objParams)
        {
            bool hasFile = false;
            PropertyInfo[] itemProperties = tran.GetType().GetProperties();
            List<FileDetails> files = null;
            List<string> subTasklistNames = new List<string>();
            List<List<ITask>> subTasks = new List<List<ITask>>();
            List<string> transListName = new List<string>();
            List<List<ITrans>> transList = new List<List<ITrans>>();
            Dictionary<string, object> itemValues = new Dictionary<string, object>();
            foreach (PropertyInfo property in itemProperties)
            {
                bool isListCoumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                bool isTask = property.GetCustomAttribute<IsTaskAttribute>() != null && property.GetCustomAttribute<IsTaskAttribute>().IsTaskField;
                bool isTran = property.GetCustomAttribute<IsTranAttribute>() != null && property.GetCustomAttribute<IsTranAttribute>().IsTranField;
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
                            string users = Convert.ToString(property.GetValue(tran)).Trim(',');
                            itemValues[listCoumnName] = this.GetMultiplePersonField(context, web, users, property);
                        }
                    }
                    else if (isFile)
                    {
                        files = property.GetValue(tran) != null ? property.GetValue(tran) as List<FileDetails> : null;
                    }
                    else
                    {
                        itemValues[listCoumnName] = property.GetValue(tran);
                    }
                }
                else if (isTran)
                {
                    string tranListName = property.GetCustomAttribute<IsTranAttribute>().TranListName;
                    List<ITrans> tranList = property.GetValue(tran) as List<ITrans>;
                    if (tranList != null && tranList.Count > 0 && !string.IsNullOrEmpty(tranListName))
                    {
                        transListName.Add(tranListName);
                        transList.Add(tranList);
                    }
                }
                else if (isTask)
                {
                    string subTasklistName = property.GetCustomAttribute<IsTaskAttribute>().TaskListName;
                    List<ITask> subTask = property.GetValue(tran) != null ? property.GetValue(tran) as List<ITask> : null;
                    if (!string.IsNullOrEmpty(subTasklistName) && subTask != null && subTask.Count > 0)
                    {
                        subTasklistNames.Add(subTasklistName);
                        subTasks.Add(subTask);
                    }
                }
            }
            foreach (KeyValuePair<string, object> itemValue in itemValues)
            {
                item[itemValue.Key] = itemValue.Value;
            }
            item.Update();
            context.Load(item);
            context.ExecuteQuery();
            if (files != null)
            {
                this.SaveAttachment(context, files, ref item);
                hasFile = true;
            }

            if (transList != null && transList.Count > 0 && transList.Count == transListName.Count)
            {
                string itemId = item["ID"].ToString();
                for (int i = 0; i < subTasks.Count; i++)
                {
                    if (transList[i] != null && transList[i].Count > 0)
                    {
                        transList[i].ForEach(p => p.RequestID = Convert.ToInt32(itemId));
                        this.SaveTranItems(context, web, transList[i], transListName[i], objParams);
                    }
                }
            }

            return hasFile;
        }
        #endregion

        /// <summary>
        /// Gets the transaction list data by identifier.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="type">The type.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="transId">The trans identifier.</param>
        /// <param name="getSubItems">if set to <c>true</c> [get sub items].</param>
        /// <returns>Itrans object</returns>
        public ITrans GetTransactionListDataById(ClientContext context, Web web, Type type, string listName, int transId, bool getSubItems = true)
        {
            ITrans tranItem = null;

            if (context != null && web != null && type != null && transId > 0 && !string.IsNullOrEmpty(listName))
            {
                tranItem = Activator.CreateInstance(type) as ITrans;
                List transList = web.Lists.GetByTitle(listName);
                ListItem transListItem = transList.GetItemById(transId);

                context.Load(transListItem);
                context.ExecuteQuery();
                tranItem = this.SetTranProperties(context, web, transListItem, tranItem, tranItem.GetType().GetProperties(), getSubItems);
            }

            return tranItem;
        }
    }
}