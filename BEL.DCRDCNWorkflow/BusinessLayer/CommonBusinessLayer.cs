namespace BEL.DCRDCNWorkflow.BusinessLayer
{

    using BEL.CommonDataContract;
    using BEL.DataAccessLayer;
    using BEL.DCRDCNWorkflow.Common;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.UserProfiles;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    public sealed class CommonBusinessLayer
    {
        /// <summary>
        ///  Lazy Instance
        /// </summary>
        private static readonly Lazy<CommonBusinessLayer> lazy = new Lazy<CommonBusinessLayer>(() => new CommonBusinessLayer());

        /// <summary>
        /// Instance
        /// </summary>
        public static CommonBusinessLayer Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        /// <summary>
        /// The context
        /// </summary>
        private ClientContext context = null;

        /// <summary>
        /// The web
        /// </summary>
        private Web web = null;

        private CommonBusinessLayer()
        {
            BELDataAccessLayer helper = new BELDataAccessLayer();
            string siteURL = helper.GetSiteURL(SiteURLs.DCRDCNSITEURL);
            if (!string.IsNullOrEmpty(siteURL))
            {
                if (this.context == null)
                {
                    this.context = BELDataAccessLayer.Instance.CreateClientContext(siteURL);
                }
                if (this.web == null)
                {
                    this.web = BELDataAccessLayer.Instance.CreateWeb(this.context);
                }
            }
        }
        /// <summary>
        /// Download File
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="applicationName">application Name</param>
        /// <returns>Byte Array</returns>
        public byte[] DownloadFile(string url, string applicationName)
        {
            BELDataAccessLayer helper = new BELDataAccessLayer();
            ////string siteURL = helper.GetSiteURL(applicationName);
            ////context = helper.CreateClientContext(siteURL);
            return helper.GetFileBytesByUrl(this.context, url);
        }

        /// <summary>
        /// Validates the users.
        /// </summary>
        /// <param name="emailList">The email list.</param>
        /// <returns>list of invalid users</returns>
        public List<string> ValidateUsers(List<string> emailList)
        {
            BELDataAccessLayer helper = new BELDataAccessLayer();
            return helper.GetInvalidUsers(emailList);
        }

        /// <summary>
        /// Removes the cache keys.
        /// </summary>
        /// <param name="keys">The keys.</param>
        public void RemoveCacheKeys(List<string> keys)
        {
            if (keys != null && keys.Count != 0)
            {
                foreach (string key in keys)
                {
                    GlobalCachingProvider.Instance.RemoveItem(key);
                }
            }
        }

        /// <summary>
        /// Gets the cache keys.
        /// </summary>
        /// <returns>list of string</returns>
        public List<string> GetCacheKeys()
        {
            return GlobalCachingProvider.Instance.GetAllKeys();
        }

        /// <summary>
        /// Gets the file name list.
        /// </summary>
        /// <param name="sectionDetails">The section details.</param>
        /// <param name="type">The type.</param>
        /// <returns>ISection Detail</returns>
        public ISection GetFileNameList(ISection sectionDetails, Type type)
        {
            if (sectionDetails == null)
            {
                return null;
            }
            dynamic dysectionDetails = Convert.ChangeType(sectionDetails, type);
            dysectionDetails.FileNameList = string.Empty;
            if (dysectionDetails.Files != null && dysectionDetails.Files.Count > 0)
            {
                dysectionDetails.FileNameList = JsonConvert.SerializeObject(dysectionDetails.Files);
            }
            return dysectionDetails;
        }


        /// <summary>
        /// Gets the file name list from current approver.
        /// </summary>
        /// <param name="sectionDetails">The section details.</param>
        /// <param name="type">The type.</param>
        /// <returns>I Section</returns>
        public ISection GetFileNameListFromCurrentApprover(ISection sectionDetails, Type type)
        {
            if (sectionDetails == null)
            {
                return null;
            }
            dynamic dysectionDetails = Convert.ChangeType(sectionDetails, type);
            dysectionDetails.FileNameList = string.Empty;
            if (dysectionDetails.CurrentApprover != null && dysectionDetails.CurrentApprover.Files != null && dysectionDetails.CurrentApprover.Files.Count > 0)
            {
                dysectionDetails.FileNameList = JsonConvert.SerializeObject(dysectionDetails.CurrentApprover.Files);
            }
            return dysectionDetails;
        }

        public UserDetails GetLoginUserDetail(string id)
        {

            MasterDataHelper masterHelper = new MasterDataHelper();
            List<UserDetails> userInfoList = masterHelper.GetAllEmployee(context, web);
            UserDetails detail = userInfoList.FirstOrDefault(p => p.UserId == id);
            return detail;
            //UserDetails user = new UserDetails();

            //List<UserDetails> userInfoList = GlobalCachingProvider.Instance.GetItem(ListNames.EmployeeMasterList, false) as List<UserDetails>;
            //if (userInfoList == null)
            //{
            //    userInfoList = this.GetAllEmployee();
            //    GlobalCachingProvider.Instance.AddItem(ListNames.EmployeeMasterList, userInfoList);
            //}
            //if (userInfoList != null)
            //{
            //    user = userInfoList.FirstOrDefault(p => p.UserEmail.ToLower() == Email.ToLower());
            //}
            // return user;

        }

        /// <summary>
        /// Gets all employee.
        /// </summary>
        /// <returns>List of users</returns>
        public List<UserDetails> GetAllEmployee()
        {
            List<UserDetails> userInfoList = new List<UserDetails>();
            //ListItemCollection items = web.SiteUserInfoList.GetItems(CamlQuery.CreateAllItemsQuery());
            //context.Load(items);
            //context.ExecuteQuery();
            //if (items != null)
            //{
            //    foreach (var item in items)
            //    {
            //        UserDetails user = new UserDetails();
            //        user.FullName = Convert.ToString(item["Title"]);
            //        user.UserEmail = Convert.ToString(item["EMail"]);
            //        //user.EmployeeCode = Convert.ToString(item["EmployeeCode"]);
            //        user.Department = Convert.ToString(item["Department"]);
            //        user.ReportingManager = Convert.ToString(item["Manager"]);
            //        user.UserId = item.Id.ToString(); //item["Alias"] != null ? (item["Alias"] as FieldUserValue).LookupId.ToString() : "0";
            //        userInfoList.Add(user);
            //    }
            //}


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
                                 FullName = userProfile.Title,
                                 UserEmail = userProfile.Email,
                                 LoginName = userProfile.AccountName,
                                 Department = userProfile.UserProfileProperties.ContainsKey("Department") ? Convert.ToString(userProfile.UserProfileProperties["Department"]) : string.Empty,
                                 ReportingManager = userProfile.UserProfileProperties.ContainsKey("Manager") ? Convert.ToString(userProfile.UserProfileProperties["Manager"]) : string.Empty
                             };
                userInfoList = result.ToList();
                GlobalCachingProvider.Instance.AddItem(web.ServerRelativeUrl + "/" + ListNames.EmployeeMasterList, userInfoList);
            }

            return userInfoList;
        }

        public User getCurrentUser(string userid)
        {
            return BELDataAccessLayer.EnsureUser(this.context, this.web, userid);
        }


    }
}