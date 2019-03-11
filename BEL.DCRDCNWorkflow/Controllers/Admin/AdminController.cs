namespace BEL.DCRDCNWorkflow.Controllers
{
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.BusinessLayer;
    using BEL.DCRDCNWorkflow.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Admin
    /// </summary>
    public partial class AdminController : BaseController
    {
       
        public ActionResult Index()
        {
            User spUser = null;

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateAppOnlyClientContextForSPAppWeb())
            {
                if (clientContext != null)
                {
                    spUser = clientContext.Web.CurrentUser;

                    clientContext.Load(spUser, user => user.Title);

                    clientContext.ExecuteQuery();

                    ViewBag.UserName = spUser.Title;
                }
            }

            return View();
        }
                
        #region "Cache Clear"
        /// <summary>
        /// Generates the RMLSMW.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        public ActionResult ClearCache(string ids)
        {
            ActionStatus status = new ActionStatus();
            if (!string.IsNullOrEmpty(ids))
            {

                List<string> keys = ids.Split(',').ToList();
                CommonBusinessLayer.Instance.RemoveCacheKeys(keys);
                status.IsSucceed = true;
                status.Messages.Add("Selected Cache key(s) has been Cleared.");

            }
            return this.Json(status);
        }

        /// <summary>
        /// Gets the LSMW list.
        /// </summary>
        /// <returns>return list of LSMW</returns>
         [SharePointContextFilter]
        public ActionResult CacheList()
        {
            List<string> strList = CommonBusinessLayer.Instance.GetCacheKeys();
            strList = strList.OrderBy(i => i).ToList();
            return this.View("CacheList", strList);
        }


        #endregion


    }
}