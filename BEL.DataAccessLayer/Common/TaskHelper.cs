namespace BEL.DataAccessLayer.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.SharePoint.Client;
    using Newtonsoft.Json.Linq;
    using BEL.CommonDataContract;
    using BEL.SPDataContract.Common;

    /// <summary>
    /// Task Helper
    /// </summary>
    public class TaskHelper : Helper
    {
        /// <summary>
        /// Gets the task form data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="userEmail">The user email.</param>
        /// <param name="form">The form.</param>
        /// <param name="objParams">The object parameters.</param>
        /// <returns>
        /// form object
        /// </returns>
        public IForm GetTaskFormData(ClientContext context, Web web, string applicationName, string formName, int itemId, string userEmail, IForm form, Dictionary<string, string> objParams)
        {
            if (context != null && web != null && !string.IsNullOrEmpty(applicationName) && !string.IsNullOrEmpty(formName) && !string.IsNullOrEmpty(userEmail) && form != null)
            {
                ISection taskSection = form.SectionsList.FirstOrDefault(p => p.GetType() == typeof(TaskSection));
                if (taskSection != null)
                {
                    TaskSection actualSection = taskSection as TaskSection;
                    string role = string.Empty;
                    dynamic task = Convert.ChangeType(actualSection.Task, actualSection.Task.GetType());
                    actualSection.Task = this.GetTaskById(context, web, itemId, task.TaskListName, task);
                    role = this.GetCurrentUserRoleForTask(context, web, itemId, task.TaskListName, userEmail);
                    if (!string.IsNullOrEmpty(role))
                    {
                        form.SectionsList[form.SectionsList.IndexOf(taskSection)] = actualSection;
                        int mainRequestId = 0;
                        if (objParams != null && objParams.ContainsKey(Parameter.TASKLISTNAME))
                        {
                            List taskList = web.Lists.GetByTitle(objParams[Parameter.TASKLISTNAME]);
                            ListItem taskItem = taskList.GetItemById(actualSection.Task.RequestID);
                            context.Load(taskItem);
                            context.ExecuteQuery();
                            mainRequestId = Convert.ToInt32((taskItem["RequestID"] as FieldLookupValue).LookupValue);
                        }
                        else
                        {
                            mainRequestId = actualSection.Task.RequestID;
                        }
                        Dictionary<string, bool> activeAndHiddenSections = this.GetEnabledSectionsFromLocal(context, web, form.ApprovalMatrixListName, mainRequestId, role, FormType.TASK);
                        form = this.GetFormData(context, web, applicationName, formName, mainRequestId, userEmail, form, role, activeAndHiddenSections, new Dictionary<string, string> { { Parameter.FORMTYPE, FormType.TASK } });
                        if (actualSection.Task.Status.Equals(TaskStatus.COMPLETED))
                        {
                            foreach (ISection section in form.SectionsList)
                            {
                                section.IsActive = false;
                            }
                        }
                    }
                    else
                    {
                        form = null;
                    }
                }
            }
            return form;
        }

        /// <summary>
        /// Gets the current user role for task.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="userEmail">The user email.</param>
        /// <returns>user role for task</returns>
        public string GetCurrentUserRoleForTask(ClientContext context, Web web, int itemId, string listName, string userEmail)
        {
            string userRole = string.Empty;
            if (context != null && web != null && !string.IsNullOrEmpty(listName) && itemId > 0)
            {
                try
                {
                    List spList = web.Lists.GetByTitle(listName);
                    ListItem item = spList.GetItemById(itemId);
                    context.Load(item);
                    RoleAssignmentCollection roles = item.RoleAssignments;
                    context.Load(roles);

                    User usr = web.EnsureUser(Helper.ClaimPrefix + userEmail);
                    context.Load(usr);
                    RoleAssignment role = roles.GetByPrincipal(usr);
                    context.Load(role.RoleDefinitionBindings);
                    context.ExecuteQuery();
                    RoleDefinitionBindingCollection roleBindings = role.RoleDefinitionBindings;
                    if (roleBindings != null)
                    {
                        RoleDefinition viewerRole = roleBindings.Where(p => p.RoleTypeKind == RoleType.Reader).FirstOrDefault();
                        if (viewerRole != null)
                        {
                            userRole = UserRoles.TASKVIEWER;
                        }
                        RoleDefinition contributorRole = roleBindings.Where(p => p.RoleTypeKind == RoleType.Contributor).FirstOrDefault();
                        if (contributorRole != null)
                        {
                            userRole = UserRoles.TASKOWNER;
                        }
                    }
                }
                catch
                {
                    //No Role Found for user
                    userRole = string.Empty;
                }
            }
            return userRole;
        }

        /// <summary>
        /// Saves the task form by section.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="taskSection">The task section.</param>
        /// <param name="userEmail">The user email.</param>
        /// <param name="objParams">The object parameters.</param>
        /// <returns>
        /// Action Status
        /// </returns>
        public ActionStatus SaveTaskFormBySection(ClientContext context, Web web, string applicationName, string formName, TaskSection taskSection, string userEmail, Dictionary<string, string> objParams)
        {
            ActionStatus status = new ActionStatus();
            bool isSuccess = false;
            if (objParams == null)
            {
                objParams = new Dictionary<string, string>();
            }
            if (context != null && web != null && taskSection != null && !string.IsNullOrEmpty(applicationName) && !string.IsNullOrEmpty(formName) && !string.IsNullOrEmpty(userEmail))
            {
                if (taskSection.Task != null)
                {
                    if (!objParams.ContainsKey(Parameter.USEREMAIL))
                    {
                        objParams[Parameter.USEREMAIL] = userEmail;
                    }
                    switch (taskSection.ActionStatus)
                    {
                        case ButtonActionStatus.ReviseDate:
                            if (!objParams.ContainsKey(Parameter.ACTIONPER))
                            {
                                objParams[Parameter.ACTIONPER] = "Revised Date";
                            }
                            isSuccess = this.SaveTaskFromUpdateView(context, web, taskSection.ListDetails[0].ListName, taskSection.Task, userEmail, objParams);
                            break;
                        case ButtonActionStatus.Save:
                            if (!objParams.ContainsKey(Parameter.ACTIONPER))
                            {
                                objParams[Parameter.ACTIONPER] = "Update Task";
                            }
                            isSuccess = this.SaveTaskFromUpdateView(context, web, taskSection.ListDetails[0].ListName, taskSection.Task, userEmail, objParams);
                            break;
                        case ButtonActionStatus.ReAssign:
                            if (!objParams.ContainsKey(Parameter.ACTIONPER))
                            {
                                objParams[Parameter.ACTIONPER] = "Task ReAssigned";
                            }
                            isSuccess = this.ReassignTask(context, web, taskSection.Task, taskSection.ListDetails[0].ListName, applicationName, formName, userEmail, objParams);
                            break;
                        case ButtonActionStatus.Complete:
                            if (!objParams.ContainsKey(Parameter.ACTIONPER))
                            {
                                objParams[Parameter.ACTIONPER] = "Task Completed";
                            }
                            taskSection.Task.ActionBy = string.Empty;
                            taskSection.Task.Status = TaskStatus.COMPLETED;
                            taskSection.Task.ActualEndDate = DateTime.Now;
                            //isSuccess = this.UpdateTaskById(context, web, taskSection.Task, taskSection.ListDetails[0].ListName);
                            isSuccess = this.SaveTaskFromUpdateView(context, web, taskSection.ListDetails[0].ListName, taskSection.Task, userEmail, objParams);
                            isSuccess = this.CompleteTask(context, web, taskSection.Task.ID, taskSection.ListDetails[0].ListName, TaskStatus.COMPLETED, applicationName, formName, objParams);
                            break;
                        case ButtonActionStatus.RemovedTask:
                            if (!objParams.ContainsKey(Parameter.ACTIONPER))
                            {
                                objParams[Parameter.ACTIONPER] = "Task Removed";
                            }
                            taskSection.Task.ActionBy = string.Empty;
                            taskSection.Task.Status = TaskStatus.REMOVED;
                            taskSection.Task.ActualEndDate = DateTime.Now;
                            //isSuccess = this.UpdateTaskById(context, web, taskSection.Task, taskSection.ListDetails[0].ListName);
                            isSuccess = this.SaveTaskFromUpdateView(context, web, taskSection.ListDetails[0].ListName, taskSection.Task, userEmail, objParams);
                            isSuccess = this.RemovedTask(context, web, taskSection.Task.ID, taskSection.ListDetails[0].ListName, TaskStatus.COMPLETED, applicationName, formName, objParams);
                            break;
                        case ButtonActionStatus.Forward:
                            if (!objParams.ContainsKey(Parameter.EMAILTEMPLATE))
                            {
                                objParams[Parameter.EMAILTEMPLATE] = EmailTemplateName.FORWARDTASKMAIL;
                            }
                            isSuccess = this.ForwardTask(context, web, taskSection.Task, taskSection.ListDetails[0].ListName, applicationName, formName, objParams);
                            break;
                        default:
                            break;
                    }
                }
                status.IsSucceed = isSuccess;
                if (isSuccess)
                {
                    status.Messages.Add(ApplicationConstants.SUCCESSMESSAGE + "_" + taskSection.ActionStatus.ToString());
                }
                else
                {
                    status.Messages.Add(ApplicationConstants.ERRORMESSAGE + "_" + taskSection.ActionStatus.ToString());
                }
            }
            return status;
        }

        /// <summary>
        /// Saves the task from update view.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="task">The task.</param>
        /// <param name="userEmail">user email</param>
        /// <param name="otherParam">The other parameter.</param>
        /// <returns>
        /// true or false
        /// </returns>
        private bool SaveTaskFromUpdateView(ClientContext context, Web web, string listName, ITask task, string userEmail, Dictionary<string, string> otherParam)
        {
            List taskList = web.Lists.GetByTitle(listName);
            ListItem item = taskList.GetItemById(task.ID);
            item["Status"] = TaskStatus.INPROGRESS;
            item.Update();
            context.Load(item);
            context.ExecuteQuery();
            PropertyInfo[] itemProperties = task.GetType().GetProperties();
            List<FileDetails> files = new List<FileDetails>();
            foreach (PropertyInfo property in itemProperties)
            {
                string listCoumnName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                try
                {
                    bool isUpdateField = property.GetCustomAttribute<SaveOnUpdateTaskAttribute>() != null && property.GetCustomAttribute<SaveOnUpdateTaskAttribute>().IsSaveOnUpdateTask;
                    bool isFile = property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile;
                    bool isAppend = property.GetCustomAttribute<IsAppendFieldAttribute>() != null && property.GetCustomAttribute<IsAppendFieldAttribute>().IsAppendField;
                    bool isPerson = property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson;
                    if (isUpdateField)
                    {
                        if (isPerson)
                        {
                            string users = Convert.ToString(property.GetValue(task)).Trim(',');
                            item[listCoumnName] = this.GetMultiplePersonField(context, web, users, property);
                        }
                        else if (isFile)
                        {
                            files = property.GetValue(task) != null ? property.GetValue(task) as List<FileDetails> : null;
                        }
                        else if (isAppend)
                        {
                            if (property.GetValue(task) != null)
                            {
                                item[listCoumnName] = property.GetValue(task);
                                string userName = Helper.GetNameFromEmail(context, web, userEmail);
                                item[listCoumnName + "History"] = Convert.ToString(item[listCoumnName + "History"]) + "<br/> " + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt") + " ~ " + item[listCoumnName] + " ~ " + userName + " ~ " + otherParam[Parameter.ACTIONPER];
                            }
                        }
                        else
                        {
                            item[listCoumnName] = property.GetValue(task);
                        }
                    }
                    item.Update();
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("Error While SaveTaskFromUpdateView  - ListColumnName:{0}, Message:{1}, StackTrace: {2}", listCoumnName, ex.Message, ex.StackTrace));
                }
            }
            context.ExecuteQuery();
            if (files != null && files.Count > 0)
            {
                this.SaveAttachment(context, files, ref item);
            }
            return true;
        }

        /// <summary>
        /// Updates the task by identifier.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="task">The task.</param>
        /// <param name="listName">Name of the list.</param>
        /// <returns>true or false</returns>
        public bool UpdateTaskById(ClientContext context, Web web, ITask task, string listName)
        {
            bool isUpdated = false;
            if (context != null && web != null && task != null && !string.IsNullOrEmpty(listName))
            {
                if (task.ID > 0)
                {
                    List taskList = web.Lists.GetByTitle(listName);
                    ListItem item = taskList.GetItemById(task.ID);
                    context.Load(item);
                    context.ExecuteQuery();
                    /*06/07/2016 Ashok Optimization*/
                    this.UpdateTaskItem(context, web, ref item, task, null);
                    //if (this.UpdateTaskItem(context, web, ref item, task, null))
                    //{
                    //    context.Load(item);
                    //    context.ExecuteQuery();
                    //}
                    /*06/07/2016 Ashok Optimization*/
                    isUpdated = true;
                }
            }
            return isUpdated;
        }

        /// <summary>
        /// Forwards the task.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="task">The task.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="objParams">The object parameters.</param>
        /// <returns>
        /// true or false
        /// </returns>
        private bool ForwardTask(ClientContext context, Web web, ITask task, string listName, string applicationName, string formName, Dictionary<string, string> objParams)
        {
            bool isSuccess = false;
            EmailHelper eHelper = new EmailHelper();
            List<ListItemDetail> listDetails = new List<ListItemDetail>();
            listDetails.Add(new ListItemDetail() { ListName = listName, ItemId = task.ID });
            if (objParams != null && objParams.ContainsKey(Parameter.EMAILTEMPLATE) && objParams.ContainsKey(Parameter.USEREMAIL))
            {
                Dictionary<string, string> email = eHelper.GetEmailBody(context, web, EmailTemplateName.FORWARDTASKMAIL, listDetails, null, UserRoles.TASKOWNER, applicationName, formName);
                if (email != null && email.Count >= 2)
                {
                    isSuccess = eHelper.SendMail(applicationName, formName, EmailTemplateName.FORWARDTASKMAIL, email["Subject"], email["Body"], objParams[Parameter.USEREMAIL], task.ForwardTo, string.Empty, false);
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// Reassigns the task.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="task">The task.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="currentUserEmail">current email</param>
        /// <param name="objParams">The object parameters.</param>
        /// <returns>
        /// true or false
        /// </returns>
        public bool ReassignTask(ClientContext context, Web web, ITask task, string listName, string applicationName, string formName, string currentUserEmail, Dictionary<string, string> objParams = null)
        {
            bool isUpdated = false;
            if (objParams == null)
            {
                objParams = new Dictionary<string, string>();
            }
            if (!objParams.ContainsKey(Parameter.ACTIONPER))
            {
                objParams[Parameter.ACTIONPER] = "Task ReAssigned";
            }
            if (context != null && web != null && task != null && !string.IsNullOrEmpty(listName))
            {
                if (task.ID > 0)
                {
                    List taskList = web.Lists.GetByTitle(listName);
                    ListItem item = taskList.GetItemById(task.ID);
                    context.Load(item);
                    context.ExecuteQuery();

                    ////Mark CC field value get and set to Mail start
                    string markCC = string.Empty;
                    //FieldCollection listFields = taskList.Fields;
                    //context.Load(listFields);
                    //context.ExecuteQuery();
                    //Field listField = listFields.FirstOrDefault(p => p.InternalName == "MarkCC");
                    //if (listField != null)
                    //{
                    //    markCC = Helper.GetEmailsFromPersonField(context, web, item["MarkCC"] as FieldUserValue[]);
                    //}
                    //Mark CC field value get and set to Mail end

                    string userEmail = string.Empty;
                    string oldUserName = string.Empty;
                    if (task.GetType().GetProperty("ActionBy").GetCustomAttribute<IsPersonAttribute>().IsMultiple)
                    {
                        userEmail = Helper.GetEmailsFromPersonField(context, web, item["ActionBy"] as FieldUserValue[]);
                        oldUserName = Helper.GetNameFromPersonField(context, web, item["ActionBy"] as FieldUserValue[]);
                        item["ActionBy"] = Helper.GetFieldUserValueFromPerson(context, web, task.ActionBy.Split(','));
                    }
                    else
                    {
                        userEmail = Helper.GetEmailsFromPersonField(context, web, item["ActionBy"] as FieldUserValue);
                        oldUserName = Helper.GetNameFromPersonField(context, web, item["ActionBy"] as FieldUserValue);
                        item["ActionBy"] = Helper.GetFieldUserValueFromPerson(context, web, task.ActionBy);
                    }
                    if (task.GetType().GetProperty("MarkCC") != null)
                    {
                        PropertyInfo markccprop = task.GetType().GetProperty("MarkCC");
                        markCC = Convert.ToString(markccprop.GetValue(task));
                        Logger.Info("Reassing Mark CC Value " + markCC);
                        if (!string.IsNullOrEmpty(markCC))
                        {
                            item["MarkCC"] = Helper.GetFieldUserValueFromPerson(context, web, markCC.Split(','));
                        }
                    }
                    item["Comment"] = task.Comment;
                    item.Update();
                    context.Load(item);
                    context.ExecuteQuery();
                    string commentHistory = Convert.ToString(item["CommentHistory"]);
                    string userName = Helper.GetNameFromEmail(context, web, userEmail);
                    commentHistory += "<br/> " + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt") + " ~ " + task.Comment + " ( Task ReAssign from " + oldUserName + " to " + Helper.GetNameFromEmail(context, web, task.ActionBy) + " ) ~ " + userName + " ~ " + objParams[Parameter.ACTIONPER];
                    item["CommentHistory"] = commentHistory;
                    item.Update();
                    context.Load(item);
                    this.ReplaceTaskItemPermission(context, web, ref item, userEmail, new Dictionary<string, string> { { task.ActionBy, SharePointPermission.CONTRIBUTOR } });
                    context.Load(item);
                    context.ExecuteQuery();
                    isUpdated = true;
                    EmailHelper eHelper = new EmailHelper();
                    List<ListItemDetail> listItemDetails = new List<ListItemDetail>() { new ListItemDetail() { ListName = listName, ItemId = task.ID, ListItemObject = item } };
                    Dictionary<string, string> email = eHelper.GetEmailBody(context, web, EmailTemplateName.TASKREASSIGNEDMAIL, listItemDetails, null, UserRoles.TASKOWNER, applicationName, formName);
                    if (email != null && email.Count >= 2)
                    {
                        eHelper.SendMail(applicationName, formName, EmailTemplateName.TASKREASSIGNEDMAIL, email["Subject"], email["Body"], userEmail, task.ActionBy, markCC, false);
                    }
                }
            }
            return isUpdated;
        }

        /// <summary>
        /// Replaces the task item permission.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="item">The item.</param>
        /// <param name="oldUserEmails">The old user emails.</param>
        /// <param name="newUser">The new user.</param>
        /// <returns>true or false</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Need to pass item to reduce sharepoint call"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need to pass item to reduce sharepoint call")]
        public bool ReplaceTaskItemPermission(ClientContext context, Web web, ref ListItem item, string oldUserEmails, Dictionary<string, string> newUser)
        {
            bool permissionAssigned = false;
            if (context != null && web != null && !string.IsNullOrEmpty(oldUserEmails) && newUser != null && item != null)
            {
                string[] oldUsers = oldUserEmails.Split(',');
                string claimprefix = Helper.ClaimPrefix;
                foreach (string oldUserEmail in oldUsers)
                {
                    User oldUser = web.EnsureUser(claimprefix + oldUserEmail);
                    context.Load(oldUser);
                    item.RoleAssignments.GetByPrincipal(oldUser).DeleteObject();
                }
                if (newUser.Count == 1)
                {
                    RoleDefinitionBindingCollection collRoleDefinitionBindingAssignee = new RoleDefinitionBindingCollection(context);
                    string[] newUsers = newUser.First().Key.Split(',');
                    collRoleDefinitionBindingAssignee.Add(web.RoleDefinitions.GetByName(newUser.First().Value));
                    User[] objNewUser = new User[newUsers.Length];
                    int i = 0;
                    foreach (string newUserEmail in newUsers)
                    {
                        if (!string.IsNullOrEmpty(newUserEmail))
                        {
                            //objNewUser[i] = Helper.EnsureUser(context, web, newUserEmail);
                            objNewUser[i] = web.EnsureUser(claimprefix + newUserEmail);
                            context.Load(objNewUser[i]);
                            //Set permission type
                            item.RoleAssignments.Add(objNewUser[i], collRoleDefinitionBindingAssignee);
                        }
                        i++;
                    }
                }
                context.ExecuteQuery();
                permissionAssigned = true;
            }
            return permissionAssigned;
        }

        /// <summary>
        /// Updates the task status.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="status">The status.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="otherParam">other Param</param>
        /// <returns>
        /// true or false
        /// </returns>
        public bool CompleteTask(ClientContext context, Web web, int itemId, string listName, string status, string applicationName, string formName, Dictionary<string, string> otherParam)
        {
            bool isTaskStatusUpdated = false;
            if (context != null && web != null && itemId > 0 && !string.IsNullOrEmpty(listName) && otherParam != null)
            {
                List taskList = web.Lists.GetByTitle(listName);
                ListItem item = taskList.GetItemById(itemId);
                context.Load(item);
                context.ExecuteQuery();

                //Mark CC field value get and set to Mail start
                string markCC = string.Empty;
                FieldCollection listFields = taskList.Fields;
                context.Load(listFields);
                context.ExecuteQuery();
                Field listField = listFields.FirstOrDefault(p => p.InternalName == "MarkCC");
                if (listField != null)
                {
                    markCC = Helper.GetEmailsFromPersonField(context, web, item["MarkCC"] as FieldUserValue[]);
                }
                //Mark CC field value get and set to Mail end

                string commentHistory = Convert.ToString(item["CommentHistory"]);
                string userName = Helper.GetNameFromEmail(context, web, otherParam[Parameter.USEREMAIL]);
                commentHistory += "<br/> " + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt") + " ~ Task Completed. ~ " + userName;
                item["Status"] = TaskStatus.COMPLETED;
                item["ActualEndDate"] = DateTime.Now;
                item["CommentHistory"] = commentHistory;
                item.Update();
                context.Load(item);
                context.ExecuteQuery();
                if (item["ActionBy"] != null)
                {
                    string userEmail = string.Empty;
                    if (item["ActionBy"].GetType().Name.Equals("FieldUserValue"))
                    {
                        userEmail = Helper.GetEmailsFromPersonField(context, web, item["ActionBy"] as FieldUserValue);
                    }
                    else
                    {
                        userEmail = Helper.GetEmailsFromPersonField(context, web, item["ActionBy"] as FieldUserValue[]);
                    }
                    if (this.ReplaceTaskItemPermission(context, web, ref item, userEmail, new Dictionary<string, string> { { userEmail, SharePointPermission.READER } }))
                    {
                        context.Load(item);
                        context.ExecuteQuery();
                        RoleAssignmentCollection roles = item.RoleAssignments;
                        context.Load(roles, ItemRole => ItemRole.Include(role => role.Member, role => role.RoleDefinitionBindings.Where(p => p.Name == SharePointPermission.READER)));
                        context.ExecuteQuery();
                        string to = string.Empty;
                        if (item["RequestBy"] != null)
                        {
                            to = Helper.GetEmailsFromPersonField(context, web, item["RequestBy"] as FieldUserValue);
                        }
                        //foreach (var role in roles)
                        //{
                        //    if (role.RoleDefinitionBindings.Count >= 1)
                        //    {
                        //        var user = (Microsoft.SharePoint.Client.User)role.Member;
                        //        to = to.Trim(',') + "," + user.Email;
                        //    }
                        //}
                        EmailHelper eHelper = new EmailHelper();
                        List<ListItemDetail> listItemDetails = new List<ListItemDetail>() { new ListItemDetail() { ListName = listName, ItemId = itemId, ListItemObject = item } };
                        Dictionary<string, string> email = eHelper.GetEmailBody(context, web, EmailTemplateName.TASKCOMPLETE, listItemDetails, null, UserRoles.TASKOWNER, applicationName, formName);
                        if (email != null && email.Count >= 2)
                        {
                            eHelper.SendMail(applicationName, formName, EmailTemplateName.TASKCOMPLETE, email["Subject"], email["Body"], userEmail, to, markCC, false);
                        }
                        isTaskStatusUpdated = true;
                    }
                }
            }
            return isTaskStatusUpdated;
        }

        /// <summary>
        /// Removeds the task.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="status">The status.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="otherParam">The other parameter.</param>
        /// <returns>True or false</returns>
        public bool RemovedTask(ClientContext context, Web web, int itemId, string listName, string status, string applicationName, string formName, Dictionary<string, string> otherParam)
        {
            bool isTaskStatusUpdated = false;
            if (context != null && web != null && itemId > 0 && !string.IsNullOrEmpty(listName) && otherParam != null)
            {
                List taskList = web.Lists.GetByTitle(listName);
                ListItem item = taskList.GetItemById(itemId);
                context.Load(item);
                context.ExecuteQuery();

                //Mark CC field value get and set to Mail start
                string markCC = string.Empty;
                FieldCollection listFields = taskList.Fields;
                context.Load(listFields);
                context.ExecuteQuery();
                Field listField = listFields.FirstOrDefault(p => p.InternalName == "MarkCC");
                if (listField != null)
                {
                    markCC = Helper.GetEmailsFromPersonField(context, web, item["MarkCC"] as FieldUserValue[]);
                }
                //Mark CC field value get and set to Mail end

                //string commentHistory = Convert.ToString(item["CommentHistory"]);
                //string userName = Helper.GetNameFromEmail(context, web, otherParam[Parameter.USEREMAIL]);
                //commentHistory += "<br/> " + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt") + " ~ Task Completed. ~ " + userName;
                item["Status"] = TaskStatus.REMOVED;
                item["ActualEndDate"] = DateTime.Now;
                //item["CommentHistory"] = commentHistory;
                item.Update();
                context.Load(item);
                context.ExecuteQuery();
                if (item["ActionBy"] != null)
                {
                    string userEmail = string.Empty;
                    if (item["ActionBy"].GetType().Name.Equals("FieldUserValue"))
                    {
                        userEmail = Helper.GetEmailsFromPersonField(context, web, item["ActionBy"] as FieldUserValue);
                    }
                    else
                    {
                        userEmail = Helper.GetEmailsFromPersonField(context, web, item["ActionBy"] as FieldUserValue[]);
                    }
                    if (this.ReplaceTaskItemPermission(context, web, ref item, userEmail, new Dictionary<string, string> { { userEmail, SharePointPermission.READER } }))
                    {
                        context.Load(item);
                        context.ExecuteQuery();
                        RoleAssignmentCollection roles = item.RoleAssignments;
                        context.Load(roles, ItemRole => ItemRole.Include(role => role.Member, role => role.RoleDefinitionBindings.Where(p => p.Name == SharePointPermission.READER)));
                        context.ExecuteQuery();
                        string to = string.Empty;
                        if (item["RequestBy"] != null)
                        {
                            to = Helper.GetEmailsFromPersonField(context, web, item["RequestBy"] as FieldUserValue);
                        }
                        //foreach (var role in roles)
                        //{
                        //    if (role.RoleDefinitionBindings.Count >= 1)
                        //    {
                        //        var user = (Microsoft.SharePoint.Client.User)role.Member;
                        //        to = to.Trim(',') + "," + user.Email;
                        //    }
                        //}
                        EmailHelper eHelper = new EmailHelper();
                        List<ListItemDetail> listItemDetails = new List<ListItemDetail>() { new ListItemDetail() { ListName = listName, ItemId = itemId, ListItemObject = item } };
                        Dictionary<string, string> email = eHelper.GetEmailBody(context, web, EmailTemplateName.TASKREMOVED, listItemDetails, null, UserRoles.TASKOWNER, applicationName, formName);
                        if (email != null && email.Count >= 2)
                        {
                            eHelper.SendMail(applicationName, formName, EmailTemplateName.TASKREMOVED, email["Subject"], email["Body"], userEmail, to, markCC, false);
                        }
                        isTaskStatusUpdated = true;
                    }
                }
            }
            return isTaskStatusUpdated;
        }

        #region Get/Set Task

        /// <summary>
        /// Gets the task details.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="task">The task.</param>
        /// <returns>Task Object</returns>
        public ITask GetTaskById(ClientContext context, Web web, int itemId, string listName, ITask task)
        {
            if (context != null && web != null && task != null && !string.IsNullOrEmpty(listName) && itemId > 0)
            {
                List taskList = web.Lists.GetByTitle(listName);
                ListItem taskItem = taskList.GetItemById(itemId);
                context.Load(taskItem);
                context.ExecuteQuery();
                if (taskItem != null)
                {
                    PropertyInfo[] properties = task.GetType().GetProperties();
                    task = this.SetTaskProperties(context, web, taskItem, task, properties);
                    task.TaskAction = TaskActionStatus.NOCHANGE;
                    task.ID = Convert.ToInt32(taskItem["ID"]);
                }
            }
            return task;
        }

        /// <summary>
        /// Gets the task list data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="lookupID">The lookup identifier.</param>
        /// <returns>List of Tasks</returns>
        public List<ITask> GetTaskListData(ClientContext context, Web web, Type taskType, string listName, int lookupID)
        {
            List<ITask> tasks = new List<ITask>();
            if (context != null && web != null && taskType != null && !string.IsNullOrEmpty(listName) && lookupID > 0)
            {
                if (!string.IsNullOrEmpty(listName))
                {
                    if (this.GetConfigVariable("UseRESTAPI").ToLower().Equals("true"))
                    {
                        tasks = this.GetTaskListDataUsingREST(context, web, taskType, listName, lookupID);
                    }
                    else
                    {
                        tasks = this.GetTaskListDataUsingCSOM(context, web, taskType, listName, lookupID);
                    }
                }
            }
            return tasks;
        }

        /// <summary>
        /// Gets the task list data using REST.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="lookupID">The lookup identifier.</param>
        /// <returns>List of Tasks</returns>
        public List<ITask> GetTaskListDataUsingREST(ClientContext context, Web web, Type taskType, string listName, int lookupID)
        {
            List<ITask> tasks = new List<ITask>();
            if (context != null && web != null && taskType != null && !string.IsNullOrEmpty(listName) && lookupID > 0)
            {
                if (!string.IsNullOrEmpty(listName))
                {
                    PropertyInfo[] propertyInfo = taskType.GetProperties();
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
                                string lookupFieldName = property.GetCustomAttribute<FieldColumnNameAttribute>().LookupFieldNameForTask;
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
                    tasks = this.AssignTaskPropertyValuesREST(context, web, jarr, taskType);
                }
            }
            return tasks;
        }

        /// <summary>
        /// Gets the task list data using CSOM.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="lookupID">The lookup identifier.</param>
        /// <returns>List of Tasks</returns>
        public List<ITask> GetTaskListDataUsingCSOM(ClientContext context, Web web, Type taskType, string listName, int lookupID)
        {
            List<ITask> tasks = new List<ITask>();
            if (context != null && web != null && taskType != null && !string.IsNullOrEmpty(listName) && lookupID > 0)
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
                    tasks = this.AssignTaskPropertyValues(context, web, items, taskType);
                }
            }
            return tasks;
        }

        /// <summary>
        /// Sets the tasks.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="items">The items.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <returns>List of tasks</returns>
        public List<ITask> SetTasks(ClientContext context, Web web, ListItemCollection items, Type itemType)
        {
            return this.AssignTaskPropertyValues(context, web, items, itemType);
        }

        /// <summary>
        /// Assigns the task property values.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="items">The items.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <returns>Return list of ITask</returns>
        private List<ITask> AssignTaskPropertyValues(ClientContext context, Web web, ListItemCollection items, Type itemType)
        {
            List<ITask> taskItems = new List<ITask>();
            foreach (ListItem item in items)
            {
                ITask task = Activator.CreateInstance(itemType) as ITask;
                task = this.SetTaskProperties(context, web, item, task, task.GetType().GetProperties());
                task.Status = string.IsNullOrEmpty(task.Status) ? TaskStatus.NOTSTARTED : task.Status;
                task.TaskAction = TaskActionStatus.NOCHANGE;
                task.ID = Convert.ToInt32(item["ID"]);
                task.Index = taskItems.Count + 1;
                taskItems.Add(task);
            }
            return taskItems;
        }

        /// <summary>
        /// Assigns the task property values.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="items">The items.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <returns>Return list of ITask</returns>
        private List<ITask> AssignTaskPropertyValuesREST(ClientContext context, Web web, JArray items, Type itemType)
        {
            List<ITask> taskItems = new List<ITask>();
            foreach (JToken item in items)
            {
                ITask task = Activator.CreateInstance(itemType) as ITask;
                task = this.SetTaskPropertiesREST(context, web, item, task, task.GetType().GetProperties());
                task.Status = string.IsNullOrEmpty(task.Status) ? TaskStatus.NOTSTARTED : task.Status;
                task.TaskAction = TaskActionStatus.NOCHANGE;
                task.ID = Convert.ToInt32(item["ID"]);
                task.Index = taskItems.Count + 1;
                taskItems.Add(task);
            }
            return taskItems;
        }

        /// <summary>
        /// Sets the task properties.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="listItem">The list item.</param>
        /// <param name="taskItem">The task item.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>task Object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Will do Later")]
        private ITask SetTaskPropertiesREST(ClientContext context, Web web, JToken listItem, ITask taskItem, PropertyInfo[] properties)
        {
            Helper helper = new Helper();
            foreach (PropertyInfo property in properties)
            {
                bool isListColumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                bool isTask = property.GetCustomAttribute<IsTaskAttribute>() != null && property.GetCustomAttribute<IsTaskAttribute>().IsTaskField;
                bool isTran = property.GetCustomAttribute<IsTranAttribute>() != null && property.GetCustomAttribute<IsTranAttribute>().IsTranField;
                bool isAppend = property.GetCustomAttribute<IsAppendFieldAttribute>() != null && property.GetCustomAttribute<IsAppendFieldAttribute>().IsAppendField;
                string listCoumnName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                bool isFile = property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile;
                if (isListColumn)
                {
                    if (property.GetCustomAttribute<FieldColumnNameAttribute>() != null && property.GetCustomAttribute<FieldColumnNameAttribute>().IsLookup)
                    {
                        string lookupFieldName = property.GetCustomAttribute<FieldColumnNameAttribute>().LookupFieldNameForTask;
                        if (property.GetCustomAttribute<FieldColumnNameAttribute>().IsMultipleLookup)
                        {
                            if (((JContainer)listItem[listCoumnName])["results"] != null)
                            {
                                List<string> reletedto = new List<string>();
                                foreach (dynamic lookup in ((JContainer)listItem[listCoumnName])["results"])
                                {
                                    reletedto.Add(lookup[lookupFieldName].ToString());
                                }
                                property.SetValue(taskItem, reletedto);
                            }
                        }
                        else
                        {
                            if (((dynamic)listItem[listCoumnName])[lookupFieldName] != null)
                            {
                                object value = ((JValue)((dynamic)listItem[listCoumnName])[lookupFieldName]).Value;
                                Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                                property.SetValue(taskItem, safeValue);
                            }
                        }
                    }
                    else if (property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson)
                    {
                        string personEmail = string.Empty;
                        if (property.GetCustomAttribute<IsPersonAttribute>().IsMultiple)
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
                        property.SetValue(taskItem, personEmail.Trim(','));
                    }
                    else if (isFile)
                    {
                        if (((dynamic)((dynamic)listItem["AttachmentFiles"])).__deferred != null)
                        {
                            string url = ((dynamic)((dynamic)listItem["AttachmentFiles"])).__deferred.uri.Value;
                            List<FileDetails> objAttachmentFiles = helper.GetAttachmentsUsingREST(url);
                            property.SetValue(taskItem, objAttachmentFiles);
                        }
                    }
                    else if (isAppend)
                    {
                        object value = ((JValue)listItem[listCoumnName]).Value;
                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                        property.SetValue(taskItem, safeValue);

                        object valueHist = ((JValue)listItem[listCoumnName + "History"]).Value;
                        Type tHist = Nullable.GetUnderlyingType(taskItem.GetType().GetProperty(listCoumnName + "History").PropertyType) ?? taskItem.GetType().GetProperty(listCoumnName + "History").PropertyType;
                        object safeValueHist = (value == null) ? null : Convert.ChangeType(valueHist, tHist);
                        taskItem.GetType().GetProperty(listCoumnName + "History").SetValue(taskItem, safeValueHist);
                    }
                    else
                    {
                        object value = ((JValue)listItem[listCoumnName]).Value;
                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                        property.SetValue(taskItem, safeValue);
                    }
                }
                else if (isTask)
                {
                    string listName = property.GetCustomAttribute<IsTaskAttribute>().TaskListName;
                    Type tSubTask = property.GetCustomAttribute<IsTaskAttribute>().TaskType;
                    if (!string.IsNullOrEmpty(listName))
                    {
                        List<ITask> subTasks = this.GetTaskListData(context, web, tSubTask, listName, Convert.ToInt32(listItem["ID"]));
                        property.SetValue(taskItem, subTasks);
                    }
                }
                else if (isTran)
                {
                    string listName = property.GetCustomAttribute<IsTranAttribute>().TranListName;
                    Type tSubTran = property.GetCustomAttribute<IsTranAttribute>().TranType;
                    if (!string.IsNullOrEmpty(listName))
                    {
                        TransactionHelper tranHelper = new TransactionHelper();
                        List<ITrans> subTrans = tranHelper.GetTransactionListData(context, web, tSubTran, listName, Convert.ToInt32(listItem["ID"]));
                        property.SetValue(taskItem, subTrans);
                    }
                }
            }
            return taskItem;
        }

        /// <summary>
        /// Sets the task properties.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="listItem">The list item.</param>
        /// <param name="taskItem">The task item.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>task Object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Will do Later")]
        private ITask SetTaskProperties(ClientContext context, Web web, ListItem listItem, ITask taskItem, PropertyInfo[] properties)
        {
            foreach (PropertyInfo property in properties)
            {
                try
                {
                    bool isListColumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                    bool isTask = property.GetCustomAttribute<IsTaskAttribute>() != null && property.GetCustomAttribute<IsTaskAttribute>().IsTaskField;
                    bool isTran = property.GetCustomAttribute<IsTranAttribute>() != null && property.GetCustomAttribute<IsTranAttribute>().IsTranField;
                    bool isAppend = property.GetCustomAttribute<IsAppendFieldAttribute>() != null && property.GetCustomAttribute<IsAppendFieldAttribute>().IsAppendField;
                    string listCoumnName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                    bool isFile = property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile;
                    if (isListColumn)
                    {
                        if (property.GetCustomAttribute<FieldColumnNameAttribute>() != null && property.GetCustomAttribute<FieldColumnNameAttribute>().IsLookup)
                        {
                            // Below conversion is added by : Jay Ashara for mdmrm rolling point
                            FieldLookupValue lookupField = listItem[listCoumnName] as FieldLookupValue;
                            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            object safeValue = (lookupField.LookupValue == null) ? null : Convert.ChangeType(lookupField.LookupValue, t);
                            property.SetValue(taskItem, safeValue);
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
                            if (users != null)
                            {
                                string personEmails = Helper.GetEmailsFromPersonField(context, web, users);
                                property.SetValue(taskItem, personEmails);
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
                                property.SetValue(taskItem, objAttachmentFiles);
                            }
                        }
                        else if (isAppend)
                        {
                            property.SetValue(taskItem, Convert.ToString(listItem[listCoumnName]));
                            taskItem.GetType().GetProperty(listCoumnName + "History").SetValue(taskItem, Convert.ToString(listItem[listCoumnName + "History"]));
                        }
                        else
                        {
                            object value = listItem[listCoumnName];
                            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                            property.SetValue(taskItem, safeValue);
                        }
                    }
                    else if (isTask)
                    {
                        string listName = property.GetCustomAttribute<IsTaskAttribute>().TaskListName;
                        Type tSubTask = property.GetCustomAttribute<IsTaskAttribute>().TaskType;
                        if (!string.IsNullOrEmpty(listName))
                        {
                            List<ITask> subTasks = this.GetTaskListData(context, web, tSubTask, listName, Convert.ToInt32(listItem["ID"]));
                            property.SetValue(taskItem, subTasks);
                        }
                    }
                    else if (isTran)
                    {
                        string listName = property.GetCustomAttribute<IsTranAttribute>().TranListName;
                        Type tSubTran = property.GetCustomAttribute<IsTranAttribute>().TranType;
                        if (!string.IsNullOrEmpty(listName))
                        {
                            TransactionHelper tranHelper = new TransactionHelper();
                            List<ITrans> subTrans = tranHelper.GetTransactionListData(context, web, tSubTran, listName, Convert.ToInt32(listItem["ID"]));
                            property.SetValue(taskItem, subTrans);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("Error While Save Task  - Task Title:{0}, Message:{1}, StackTrace: {2}", taskItem.Title, ex.Message, ex.StackTrace));
                }
            }
            return taskItem;
        }

        /// <summary>
        /// Saves the task items.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="tasks">The tasks.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="objParams">The object parameters.</param>
        /// <returns>
        /// true or false
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Optimize later on")]
        public bool SaveTaskItems(ClientContext context, Web web, List<ITask> tasks, string listName, Dictionary<string, string> objParams)
        {
            if (context != null && web != null && tasks != null)
            {
                try
                {
                    tasks.RemoveAll(p => p.TaskAction == TaskActionStatus.NOCHANGE);
                    foreach (ITask task in tasks)
                    {
                        this.SaveTaskItem(context, web, listName, task, objParams);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("Error While Save All Task  - Message:{0}, StackTrace: {1}", ex.Message, ex.StackTrace));
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Saves the task item.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="task">The task.</param>
        /// <param name="objParams">The object parameters.</param>
        /// <returns>Task Id</returns>
        public int SaveTaskItem(ClientContext context, Web web, string listName, ITask task, Dictionary<string, string> objParams)
        {
            int itemId = 0;
            if (context != null && web != null && !string.IsNullOrEmpty(listName) && task != null)
            {
                List list = web.Lists.GetByTitle(listName);
                ListItem item = null;
                try
                {
                    if (task.ID > 0)
                    {
                        task.Status = string.IsNullOrEmpty(task.Status) ? TaskStatus.NOTSTARTED : task.Status;
                        item = list.GetItemById(task.ID);
                    }
                    else
                    {
                        task.Status = string.IsNullOrEmpty(task.Status) ? TaskStatus.NOTSTARTED : task.Status;
                        item = list.AddItem(new ListItemCreationInformation());
                    }

                    if (task.TaskAction == TaskActionStatus.DELETED)
                    {
                        item["Status"] = TaskStatus.DELETED;
                        item.Update();
                        context.ExecuteQuery();
                        itemId = item.Id;
                    }
                    else
                    {
                        if (objParams != null && objParams.ContainsKey(Parameter.APPROVERMATRIXUSER))
                        {
                            PropertyInfo viewers = task.GetType().GetProperty("Viewers");
                            if (viewers != null && !string.IsNullOrEmpty(objParams[Parameter.APPROVERMATRIXUSER]))
                            {
                                viewers.SetValue(task, objParams[Parameter.APPROVERMATRIXUSER]);
                            }
                        }
                        itemId = this.UpdateTaskItem(context, web, ref item, task, objParams);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("Error While Save Task  - Task Title:{0}, Message:{1}, StackTrace: {2}", task.Title, ex.Message, ex.StackTrace));
                }
            }
            return itemId;
        }

        /// <summary>
        /// Saves the task items from section.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="section">The section.</param>
        /// <param name="lookupId">The lookup identifier.</param>
        /// <returns>true or false</returns>
        public bool SaveTaskItemsFromSection(ClientContext context, Web web, ISection section, int lookupId)
        {
            bool isSuccess = true;
            if (context != null && web != null && section != null)
            {
                PropertyInfo[] properties = section.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    bool isTask = property.GetCustomAttribute<IsTaskAttribute>() != null && property.GetCustomAttribute<IsTaskAttribute>().IsTaskField;
                    if (isTask)
                    {
                        string listName = property.GetCustomAttribute<IsTaskAttribute>().TaskListName;
                        if (!string.IsNullOrEmpty(listName) && property.GetValue(section) != null)
                        {
                            List<ITask> tasks = (List<ITask>)property.GetValue(section);
                            tasks.ForEach(p => p.RequestID = lookupId);
                            Logger.Info("Calling SaveTaskItems");
                            this.SaveTaskItems(context, web, tasks, listName, null);
                            Logger.Info("Called SaveTaskItems");
                        }
                    }
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// Updates the task item.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="item">The item.</param>
        /// <param name="task">The task.</param>
        /// <param name="objParams">The object parameters.</param>
        /// <returns>
        /// Task Id
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Can't refactor")]
        private int UpdateTaskItem(ClientContext context, Web web, ref ListItem item, ITask task, Dictionary<string, string> objParams)
        {
            PropertyInfo[] itemProperties = task.GetType().GetProperties();
            List<string> subTasklistNames = new List<string>();
            List<List<ITask>> subTasks = new List<List<ITask>>();
            List<string> transListName = new List<string>();
            List<List<ITrans>> transList = new List<List<ITrans>>();
            List<FileDetails> files = new List<FileDetails>();
            //Dictionary<string, object> itemDictionary = new Dictionary<string, object>();
            foreach (PropertyInfo property in itemProperties)
            {
                bool isListCoumn = property.GetCustomAttribute<IsListColumnAttribute>() == null || property.GetCustomAttribute<IsListColumnAttribute>().IsListColumn;
                bool isTask = property.GetCustomAttribute<IsTaskAttribute>() != null && property.GetCustomAttribute<IsTaskAttribute>().IsTaskField;
                bool isTran = property.GetCustomAttribute<IsTranAttribute>() != null && property.GetCustomAttribute<IsTranAttribute>().IsTranField;
                bool isFile = property.GetCustomAttribute<IsFileAttribute>() != null && property.GetCustomAttribute<IsFileAttribute>().IsFile;
                //bool isAppend = property.GetCustomAttribute<IsAppendFieldAttribute>() != null && property.GetCustomAttribute<IsAppendFieldAttribute>().IsAppendField;
                bool isPerson = property.GetCustomAttribute<IsPersonAttribute>() != null && property.GetCustomAttribute<IsPersonAttribute>().IsPerson;
                string listCoumnName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                if (isListCoumn)
                {
                    if (isPerson)
                    {
                        string users = Convert.ToString(property.GetValue(task)).Trim(',');
                        item[listCoumnName] = this.GetMultiplePersonField(context, web, users, property);
                    }
                    else if (isFile)
                    {
                        files = property.GetValue(task) != null ? property.GetValue(task) as List<FileDetails> : null;
                    }
                    else
                    {
                        item[listCoumnName] = property.GetValue(task);
                    }
                }
                else if (isTran)
                {
                    string tranListName = property.GetCustomAttribute<IsTranAttribute>().TranListName;
                    List<ITrans> tranList = property.GetValue(task) as List<ITrans>;
                    if (tranList != null && tranList.Count > 0 && !string.IsNullOrEmpty(tranListName))
                    {
                        transListName.Add(tranListName);
                        transList.Add(tranList);
                    }
                }
                else if (isTask)
                {
                    string subTasklistName = property.GetCustomAttribute<IsTaskAttribute>().TaskListName;
                    List<ITask> subTask = property.GetValue(task) != null ? property.GetValue(task) as List<ITask> : null;
                    if (!string.IsNullOrEmpty(subTasklistName) && subTask != null && subTask.Count > 0)
                    {
                        subTasklistNames.Add(subTasklistName);
                        subTasks.Add(subTask);
                    }
                }
                item.Update();
            }
            //foreach (KeyValuePair<string, object> itemVal in itemDictionary)
            //{
            //    item[itemVal.Key] = itemVal.Value;
            //}
            //item.Update();
            context.Load(item);
            context.ExecuteQuery();
            item["TaskId"] = item.Id;
            item.Update();
            context.Load(item);
            context.ExecuteQuery();
            if (files != null && files.Count > 0)
            {
                this.SaveAttachment(context, files, ref item);
            }
            if (subTasks != null && subTasks.Count > 0 && subTasks.Count == subTasklistNames.Count)
            {
                string itemId = item["ID"].ToString();
                for (int i = 0; i < subTasks.Count; i++)
                {
                    if (subTasks[i] != null && subTasks[i].Count > 0)
                    {
                        subTasks[i].ForEach(p => p.RequestID = Convert.ToInt32(itemId));
                        this.SaveTaskItems(context, web, subTasks[i], subTasklistNames[i], objParams);
                    }
                }
            }
            if (transList != null && transList.Count > 0 && transList.Count == transListName.Count)
            {
                string itemId = item["ID"].ToString();
                for (int i = 0; i < subTasks.Count; i++)
                {
                    if (transList[i] != null && transList[i].Count > 0)
                    {
                        TransactionHelper tranHelper = new TransactionHelper();
                        transList[i].ForEach(p => p.RequestID = Convert.ToInt32(itemId));
                        tranHelper.SaveTranItems(context, web, transList[i], transListName[i], objParams);
                    }
                }
            }

            /*06/07/2016 Commented By Ashok for Task Permission at Last*/
            //if (objParams != null)
            //{
            //    if (objParams.ContainsKey(Parameter.SETPERMISSION) && objParams[Parameter.SETPERMISSION] == "true")
            //    {
            //        string taskViewers = objParams.ContainsKey(Parameter.APPROVERMATRIXUSER) ? objParams[Parameter.APPROVERMATRIXUSER] : string.Empty;
            //        Dictionary<string, string> permissionDictionary = new Dictionary<string, string>();
            //        if (!string.IsNullOrEmpty(task.ActionBy))
            //        {
            //            permissionDictionary.Add(task.ActionBy, SharePointPermission.CONTRIBUTOR);
            //        }
            //        if (!string.IsNullOrEmpty(taskViewers) && !permissionDictionary.ContainsKey(taskViewers))
            //        {
            //            permissionDictionary.Add(taskViewers, SharePointPermission.READER);
            //        }
            //        this.SetItemPermission(context, web, ref item, permissionDictionary);
            //    }
            //}
            return item.Id;
        }

        /// <summary>
        /// Sends the task assign mail.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="taskListName">Name of the task list.</param>
        /// <param name="lookupId">The lookup identifier.</param>
        /// <param name="taskViewers">The task viewers.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="mailTemplateName">Name of the mail template.</param>
        /// <returns>
        /// true or false
        /// </returns>
        public bool SendTaskAssignMail(ClientContext context, Web web, string taskListName, string lookupId, string taskViewers = "", Dictionary<string, string> param = null, string mailTemplateName = EmailTemplateName.TASKASSIGNEDMAIL)
        {
            if (context != null & web != null && param != null && !string.IsNullOrEmpty(taskListName))
            {
                string applicationName = param[Parameter.APPLICATIONNAME], formName = param[Parameter.FROMNAME];
                string[] listNames = taskListName.Split(',');
                foreach (string listName in listNames)
                {
                    List spList = web.Lists.GetByTitle(listName);
                    CamlQuery qry = new CamlQuery();
                    qry.ViewXml = @"<View><Query><Where><And>
                                                     <Eq>
                                                      <FieldRef Name='RequestID' LookupId = 'TRUE'  />
                                                      <Value Type='Lookup' >" + lookupId + @"</Value>
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
                    for (int i = 0; i < items.Count; i++)
                    {
                        Dictionary<string, string> permissionDictionary = new Dictionary<string, string>();
                        string to = string.Empty;
                        if (items[i]["ActionBy"] != null)
                        {
                            switch (items[i]["ActionBy"].GetType().Name)
                            {
                                case "FieldUserValue":
                                    to = Helper.GetEmailsFromPersonField(context, web, items[i]["ActionBy"] as FieldUserValue);
                                    break;
                                case "FieldUserValue[]":
                                    to = Helper.GetEmailsFromPersonField(context, web, items[i]["ActionBy"] as FieldUserValue[]);
                                    break;
                                default:
                                    to = string.Empty;
                                    break;
                            }
                            if (!string.IsNullOrEmpty(to))
                            {
                                permissionDictionary.Add(to, SharePointPermission.CONTRIBUTOR);
                            }
                        }
                        /* Set Task Item Permission */
                        if (!string.IsNullOrEmpty(taskViewers) && !permissionDictionary.ContainsKey(taskViewers))
                        {
                            permissionDictionary.Add(taskViewers, SharePointPermission.READER);
                        }
                        ListItem item = items[i];
                        this.SetItemPermission(context, web, ref item, permissionDictionary);
                        /* Set Task Item Permission End */
                        /* Send Mail Notification for Task Assign */
                        if (!string.IsNullOrEmpty(applicationName) && !string.IsNullOrEmpty(formName) && param.ContainsKey(Parameter.SENDTASKNOTIFICATION) && param[Parameter.SENDTASKNOTIFICATION] == "True")
                        {
                            List<ListItemDetail> listItemDetails = new List<ListItemDetail>();
                            listItemDetails.Add(new ListItemDetail() { ListName = taskListName, ItemId = Convert.ToInt32(items[i]["ID"]), ListItemObject = items[i] });
                            EmailHelper eHelper = new EmailHelper();
                            Dictionary<string, string> emailTemplate = eHelper.GetEmailBody(context, web, mailTemplateName, null, null, UserRoles.TASKOWNER, applicationName, formName);
                            if (emailTemplate != null && emailTemplate.Count > 0)
                            {
                                Dictionary<string, string> emailBodyWithData = eHelper.CreateEmailBody(context, web, emailTemplate, listItemDetails, null);
                                string from = Helper.GetEmailsFromPersonField(context, web, items[i]["RequestBy"] as FieldUserValue);
                                if (!string.IsNullOrEmpty(to))
                                {
                                    eHelper.SendMail(applicationName, formName, mailTemplateName, emailBodyWithData["Subject"], emailBodyWithData["Body"], from, to, string.Empty, false);
                                }
                            }
                        }
                        /* Send Mail Notification for Task Assigned */
                    }
                }
            }
            return true;
        }
        #endregion
    }
}
