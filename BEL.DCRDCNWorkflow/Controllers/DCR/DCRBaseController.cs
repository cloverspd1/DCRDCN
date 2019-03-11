namespace BEL.DCRDCNWorkflow.Controllers
{
    using BEL.CommonDataContract;
    using BEL.DataAccessLayer;
    using BEL.DCRDCNWorkflow;
    using BEL.DCRDCNWorkflow.BusinessLayer;
    using BEL.DCRDCNWorkflow.Models;
    using BEL.DCRDCNWorkflow.Models.DCR;
    using BEL.DCRDCNWorkflow.Models.Master;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// MVR Base Controller
    /// </summary>
    public class DCRBaseController : BaseController
    {
        /// <summary>
        /// Get DCR Details
        /// </summary>
        /// <param name="objDict">Object Parameter</param>
        /// <returns>DCR Contract Object</returns>
        public DCRContract GetDCRDetails(IDictionary<string, string> objDict)
        {
            return DCRBusinessLayer.Instance.GetDCRDetails(objDict);
        }

        /// <summary>
        /// Get DCR Details
        /// </summary>
        /// <param name="objDict">Object Parameter</param>
        /// <returns>DCR Contract Object</returns>
        public DCRAdminContract GetAdminDCRDetails(IDictionary<string, string> objDict)
        {
            return DCRBusinessLayer.Instance.GetAdminDCRDetails(objDict);
        }

        /// <summary>
        /// Saves the section.
        /// </summary>
        /// <typeparam name="T">dict parameter</typeparam>
        /// <param name="section">The section.</param>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>return status</returns>
        protected ActionStatus SaveSection(ISection section, Dictionary<string, string> objDict)
        {
            ActionStatus status = new ActionStatus();
            status = DCRBusinessLayer.Instance.SaveBySection(section, objDict);
            return status;
        }

        /// <summary>
        /// Gets the Vendor.
        /// </summary>
        /// <param name="q">The q.</param>
        /// <returns>json object</returns>
        public JsonResult GetVendors(string q)
        {
            string data = DCRBusinessLayer.Instance.GetVendorMasterData(q);
            if (!string.IsNullOrEmpty(data))
            {
                var master = JSONHelper.ToObject<VendorMaster>(data);
                return this.Json((from item in master.Items select new { id = item.Value + " - " + item.Title, name = item.Value + " - " + item.Title }).ToList(), JsonRequestBehavior.AllowGet);
            }
            return this.Json("[]", JsonRequestBehavior.AllowGet);

        }
    }
}