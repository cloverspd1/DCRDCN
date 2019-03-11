namespace BEL.DCRDCNWorkflow.Controllers
{
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.BusinessLayer;
    using BEL.DCRDCNWorkflow.Common;
    using BEL.DCRDCNWorkflow.Models;
    using BEL.DCRDCNWorkflow.Models.DCN;
    using BEL.DCRDCNWorkflow.Models.DCR;
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// MVR Base Controller
    /// </summary>
    public class DCNBaseController : BaseController
    {
        public IContract GetDCNDetails(IDictionary<string, string> objDict)
        {
            IContract contract = DCNBusinessLayer.Instance.GetDCNDetails(objDict);
            return contract;
        }

        public DCRContract GetDCRDetails(IDictionary<string, string> objDict)
        {
            return DCRBusinessLayer.Instance.GetDCRDetails(objDict);
        }

        public DCNAdminContract GetDCNAdminDetails(IDictionary<string, string> objDict)
        {
            return DCNBusinessLayer.Instance.GetDCNAdminDetails(objDict);
           
        }

        public List<DCRDetails> RetrieveAllDCRNos(IDictionary<string, string> objDict)
        {
            return DCNBusinessLayer.Instance.RetrieveAllDCRNos(objDict);            
        }

        public DCRDetails RetrieveDCRNoDetails(IDictionary<string, string> objDict)
        {
            DCRDetails dcrdetail = DCNBusinessLayer.Instance.RetrieveDCRNoDetails(objDict);
            return dcrdetail;
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
            status = DCNBusinessLayer.Instance.SaveBySection(section, objDict);
            return status;
        }
    }
}