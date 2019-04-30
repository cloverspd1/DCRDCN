namespace BEL.DCRDCNWorkflow.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;
    using Newtonsoft.Json;
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.Common;
    using BEL.DCRDCNWorkflow.Models.DCN;
    using BEL.DCRDCNWorkflow.Models.Common;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using BEL.DCRDCNWorkflow.BusinessLayer;
    using BEL.DCRDCNWorkflow.Models.DCR;

    /// <summary>
    /// PR Controller used for saving Market visit report
    /// </summary>
    public class DCNController : DCNBaseController
    {
        #region "Index"

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// Index View
        /// </returns>      
        [SharePointContextFilter]
        public ActionResult Index(int id = 0, int? DCRID = 0)
        {
            {
                Dictionary<string, string> objDict = new Dictionary<string, string>();
                objDict.Add(Parameter.FROMNAME, FormNameConstants.DCNFORM);
                objDict.Add(Parameter.ITEMID, id.ToString());
                objDict.Add(Parameter.USEREID, this.CurrentUser.UserId);
                ViewBag.UserID = this.CurrentUser.UserId;
                ViewBag.UserName = this.CurrentUser.FullName;
                IContract contract = this.GetDCNDetails(objDict);
                contract.UserDetails = this.CurrentUser;
                DCNContract dcnContract = contract as DCNContract;
                if (dcnContract != null)
                {
                    var form = dcnContract.Forms.FirstOrDefault();
                    if (form != null)
                    {
                        var designdocSection = form.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCNSectionName.DESIGNENGINEERSECTION)) as BEL.DCRDCNWorkflow.Models.DCN.DesignEngineerSection;
                        if (designdocSection != null)
                        {
                            this.SetTranListintoTempData<Specification>(designdocSection.Specification, TempKeys.Specification.ToString() + "_" + id);
                            this.SetTranListintoTempData<RevisedApplicableDocuments>(designdocSection.RevisedApplicable, TempKeys.RevisedApplicableDoc.ToString() + "_" + id);
                            this.SetTranListintoTempData<VendorDCN>(designdocSection.VendorDCRList, TempKeys.DCRVendor.ToString() + "_" + id);
                        }

                        var qaIncharge = form.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCNSectionName.QAINCHARGESECTION)) as DCNQAInchargeSection;
                        if (qaIncharge != null)
                        {
                            this.SetTranListintoTempData<VendorDCN>(qaIncharge.VendorDCNList, TempKeys.DCNVendor.ToString() + "_" + id);
                        }
                    }
                }
                return this.View(contract);
            }
        }
        #endregion

        #region "Get DCR Data for DCN"
        /// <summary>
        /// Gets the TMS information.
        /// </summary>
        /// <returns>
        /// TMS Model
        /// </returns>
        [HttpGet]
        public ActionResult RetrieveAllDCR()
        {
            Dictionary<string, string> objDict = new Dictionary<string, string>();
            objDict.Add("UserEmail", this.CurrentUser.UserId);
            List<DCRDetails> dcrdetail = this.RetrieveAllDCRNos(objDict);
            //if(dcrdetail != null  &&  dcrdetail.Count != 0)
            //{
            //    dcrdetail = dcrdetail.OrderByDescending(p=>p.ID).ToList()
            //}
            return this.PartialView("_BindDCRNo", dcrdetail.OrderByDescending(p => p.ID).ToList());
        }

        [HttpGet]
        public ActionResult GetDCRDetails(string itemID)
        {
            DCRContract contract = null;
            Dictionary<string, string> objDict = new Dictionary<string, string>();
            objDict.Add("FormName", FormNameConstants.DCRFORM);
            objDict.Add("ItemId", itemID);
            objDict.Add(Parameter.USEREID, this.CurrentUser.UserId);
            ViewBag.UserID = this.CurrentUser.UserId;
            ViewBag.UserName = this.CurrentUser.FullName;

            contract = this.GetDCRDetails(objDict);
            contract.UserDetails = this.CurrentUser;
            if (contract.Forms != null && contract.Forms.Count > 0)
            {
                if (Convert.ToInt32(itemID) == 0)
                {
                    ViewBag.RequestDepartment = contract.UserDetails.Department;
                }

                SCMInchargeSection scmInchargeSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.SCMINCHARGESECTION) as SCMInchargeSection;
                if (scmInchargeSection != null && scmInchargeSection.IsActive)
                {
                    this.SetTranListintoTempData<VendorDCR>(scmInchargeSection.VendorDCRList, TempKeys.DCRVendor.ToString() + "_" + itemID);
                }
                SCMSection scmSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.SCMSECTION) as SCMSection;
                if (scmSection != null && scmSection.IsActive)
                {
                    this.SetTranListintoTempData<VendorDCR>(scmSection.VendorDCRList, TempKeys.DCRVendor.ToString() + "_" + itemID);
                }
                return PartialView("~/Views/DCR/_PrintDCR.cshtml", contract);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the Get All DCR.
        /// </summary>
        /// <returns>
        /// List DCRDetails
        /// </returns>
        [HttpGet]
        public JsonResult RetrieveDCRNoDetails(string itemID)
        {
            DCRDetails dcrdetails = new DCRDetails();
            if (itemID != null)
            {
                Dictionary<string, string> objDict = new Dictionary<string, string>();
                objDict.Add("FormName", FormNameConstants.DCRFORM);
                objDict.Add("ItemId", itemID.ToString());
                objDict.Add("UserEmail", this.CurrentUser.UserId);
                dcrdetails = this.RetrieveDCRNoDetails(objDict);
                List<VendorDCN> list = new List<VendorDCN>();

                if (dcrdetails != null && !string.IsNullOrEmpty(dcrdetails.DCRNo))
                {
                    dcrdetails.DCNNo = dcrdetails.DCRNo.Replace("DCR", "DCR-DCN");
                    if (dcrdetails.VendorDCRList != null)
                    {
                        foreach (VendorDCN item in dcrdetails.VendorDCRList)
                        {
                            item.ItemAction = ItemActionStatus.NEW;
                            item.ID = 0;
                            list.Add(item);
                        }
                        this.SetTempData<List<VendorDCN>>(TempKeys.DCRVendor + "_0", list.OrderBy(x => x.Index).ToList());

                    }
                }
            }
            return Json(dcrdetails, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Save Design Engineer Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveDesignDocumentEngineerSection(BEL.DCRDCNWorkflow.Models.DCN.DesignEngineerSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {

                if (this.ValidateModelState(model))
                {
                    var specification = this.GetTempData<List<Specification>>(TempKeys.Specification.ToString() + "_" + GetFormIdFromUrl());

                    ////specification.ForEach(p =>
                    ////{
                    ////    p.Files = new List<FileDetails>();
                    ////    p.Files.AddRange(FileListHelper.GenerateFileBytes(p.FNLPresentSpecification));
                    ////    p.Files.AddRange(FileListHelper.GenerateFileBytes(p.FNLRevisedSpecification));
                    ////    p.FNLPresentSpecification = string.Join(",", FileListHelper.GetFileNames(p.FNLPresentSpecification));
                    ////    p.FNLRevisedSpecification = string.Join(",", FileListHelper.GetFileNames(p.FNLRevisedSpecification));
                    ////});

                    model.Specification = specification.ToList<ITrans>();

                    var revisedApplicable = this.GetTempData<List<RevisedApplicableDocuments>>(TempKeys.RevisedApplicableDoc.ToString() + "_" + GetFormIdFromUrl());
                    model.RevisedApplicable = revisedApplicable.ToList<ITrans>();

                    var vendor = this.GetTempData<List<VendorDCN>>(TempKeys.DCRVendor.ToString() + "_" + GetFormIdFromUrl());
                    model.VendorDCRList = vendor.ToList<ITrans>();

                    if (model.DCRNo == null)
                    {
                        status.IsSucceed = false;
                        status.Messages = new List<string>() { this.GetResourceValue("Text_SelectDCRNO", System.Web.Mvc.Html.ResourceNames.DCN) };
                        return this.Json(status);
                    }
                    //if (model.Specification.Count == 0 && model.RevisedApplicable.Count == 0)
                    //{
                    //    status.IsSucceed = false;
                    //    status.Messages = new List<string>() { this.GetResourceValue("Text_SpecificationValidation", System.Web.Mvc.Html.ResourceNames.DCN) };
                    //    return this.Json(status);
                    //}
                    //if (model.RevisedApplicable.Count == 0)
                    //{
                    //    status.IsSucceed = false;
                    //    status.Messages = new List<string>() { this.GetResourceValue("Text_RevisedApplicableValidation", System.Web.Mvc.Html.ResourceNames.DCN) };
                    //    return this.Json(status);
                    //}

                    //string[] dcrNo = (model.DCRNo).Split('-');
                    //model.DCNNo = "DCR-DCN-" + dcrNo[1] + '-' + dcrNo[2];
                    model.DCNNo = model.DCRNo.Replace("DCR", "DCR-DCN");
                    model.Title = model.DCNNo;
                    if (model.ActionStatus == ButtonActionStatus.NextApproval || model.ActionStatus == ButtonActionStatus.SaveAsDraft)
                    {
                        model.ApproversList.ForEach(p =>
                        {
                            if (p.Role == DCNRoles.DESIGNENGINEER)
                            {
                                p.Approver = model.DesignEngineer;
                            }
                            if (p.Role == DCNRoles.DESIGNENGINEERINCHARGE)
                            {
                                //p.Approver = model.DesignEngineerIncharge;    /* Comment by Pooja*/
                                p.Approver = string.Empty;                      ////As per new CR to Remove Design Engineer Incharge from DCR and DCN
                            }
                            if (p.Role == DCNRoles.DCRPROCESSINCHARGE)
                            {
                                p.Approver = model.DCRProcessIncharge;
                            }
                            if (p.Role == DCNRoles.QAINCHARGE)
                            {
                                p.Approver = model.QAIncharge;
                            }
                            if (p.Role == DCNRoles.SCM)
                            {
                                //p.Approver = model.SCMIncharge;   //185531313
                            }
                        });
                    }
                    Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);
                    status = this.SaveSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.DCN);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.DCN);
                }
            }

            return this.Json(status);
        }
        #endregion

        #region "Save Design Engineer Incharge Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveDesignEngineerSection(BEL.DCRDCNWorkflow.Models.DCN.DesignEngineerInchargeSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);
                    if (model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        if (model.ApproveRework == "Approve")
                        {
                            model.ActionStatus = ButtonActionStatus.NextApproval;
                        }
                        else
                        {
                            model.ActionStatus = ButtonActionStatus.SendBack;
                            objDict[Parameter.SENDTOLEVEL] = "0";
                            model.SendBackTo = "0";
                        }
                    }

                    status = this.SaveSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.DCN);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.DCN);
                }
            }

            return this.Json(status);
        }
        #endregion

        #region "Save DCR Process Incharge"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveDCRProcessInchargeSection(BEL.DCRDCNWorkflow.Models.DCN.DCRProcessInchargeSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);

                    if (model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        if (model.DCRProcessICApproveRework == "Approve")
                        {
                            model.ActionStatus = ButtonActionStatus.NextApproval;
                        }
                        else if (model.DCRProcessICApproveRework == "Reject")
                        {
                            model.ActionStatus = ButtonActionStatus.Rejected;
                        }
                        else
                        {
                            model.ActionStatus = ButtonActionStatus.SendBack;
                            objDict[Parameter.SENDTOLEVEL] = "0";
                            model.SendBackTo = "0";
                        }
                    }
                    //if (!string.IsNullOrEmpty(model.SendBackTo))
                    //{
                    //    objDict[Parameter.SENDTOLEVEL] = model.SendBackTo;
                    //}
                    status = this.SaveSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.DCN);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.DCN);
                }
            }

            return this.Json(status);
        }
        #endregion

        #region "Save DCN SCM Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveDCNSCMSection(DCNSCMSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);
                    if (!string.IsNullOrEmpty(model.SendBackTo))
                    {
                        objDict[Parameter.SENDTOLEVEL] = model.SendBackTo;
                    }
                    status = this.SaveSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.DCN);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.DCN);
                }
            }

            return this.Json(status);
        }
        #endregion

        #region "Save QA Incharge"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveQAInchargeSection(DCNQAInchargeSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    //var vendor = this.GetTempData<List<VendorDCN>>(TempKeys.DCNVendor.ToString() + "_" + GetFormIdFromUrl());
                    //model.VendorDCNList = vendor.ToList<ITrans>();
                    if (model.VendorDCNPostList != null && model.VendorDCNPostList.Count != 0)
                    {
                        model.VendorDCNPostList.ToList().ForEach(p => p.ItemAction = ItemActionStatus.UPDATED);
                        model.VendorDCNList = model.VendorDCNPostList.ToList<ITrans>();
                    }
                    if (model.ActionStatus == ButtonActionStatus.Complete)
                    {
                        model.QADateofApproval = DateTime.Now;
                    }
                    Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);
                    status = this.SaveSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.DCN);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.DCN);
                }
            }

            return this.Json(status);
        }
        #endregion

        #region "CRUD Vendor Detail"

        /// <summary>
        /// Gets the vendor details grid.
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetVendorGrid()
        {
            List<Vendor> list = new List<Vendor>();
            list = this.GetTempData<List<Vendor>>(TempKeys.DCRVendor.ToString() + "_" + GetFormIdFromUrl());
            return this.PartialView("_VendorGrid", list.ToList<ITrans>());
        }
        #endregion

        #region "CRUD Specification Detail"

        /// <summary>
        /// Adds the edit Specification item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult AddEditSpecification(int index = 0)
        {
            List<Specification> list = new List<Specification>();
            list = this.GetTempData<List<Specification>>(TempKeys.Specification.ToString() + "_" + GetFormIdFromUrl());
            Specification item = null;
            if (index == 0)
            {
                item = new Specification() { Index = 0, RequestDate = DateTime.Now, RequestBy = this.CurrentUser.UserId };
            }
            else
            {
                item = list.FirstOrDefault(x => x.Index == index);
                item.FNLPresentSpecification = item.Files != null && item.Files.Count > 0 && !item.FNLPresentSpecification.Contains("[{") ? JsonConvert.SerializeObject(item.Files.Where(x => !string.IsNullOrEmpty(item.FNLPresentSpecification) && item.FNLPresentSpecification.Split(',').Contains(x.FileName)).ToList()) : item.FNLPresentSpecification;
                item.FNLRevisedSpecification = item.Files != null && item.Files.Count > 0 && !item.FNLRevisedSpecification.Contains("[{") ? JsonConvert.SerializeObject(item.Files.Where(x => !string.IsNullOrEmpty(item.FNLRevisedSpecification) && item.FNLRevisedSpecification.Split(',').Contains(x.FileName)).ToList()) : item.FNLRevisedSpecification;
            }

            return this.PartialView("_AddSpecificationDoc", item);
        }

        /// <summary>
        /// Saves the Specification detail item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Action Result</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveSpecification(Specification model)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    List<Specification> list = new List<Specification>();

                    list = this.GetTempData<List<Specification>>(TempKeys.Specification.ToString() + "_" + GetFormIdFromUrl());

                    if (model.Index == 0)
                    {
                        model.Index = list.Count + 1;
                        model.ItemAction = ItemActionStatus.NEW;

                    }
                    else
                    {
                        list.RemoveAll(x => x.Index == model.Index);
                    }
                    if (model.ID > 0)
                    {
                        model.ItemAction = ItemActionStatus.UPDATED;


                    }

                    model.Files = new List<FileDetails>();
                    if (!string.IsNullOrEmpty(model.FNLPresentSpecification) && model.FNLPresentSpecification.Contains("[{"))
                        model.Files.AddRange(JsonConvert.DeserializeObject<List<FileDetails>>(model.FNLPresentSpecification));
                    if (!string.IsNullOrEmpty(model.FNLRevisedSpecification) && model.FNLRevisedSpecification.Contains("[{"))
                        model.Files.AddRange(JsonConvert.DeserializeObject<List<FileDetails>>(model.FNLRevisedSpecification));
                    model.Files.ForEach(p =>
                    {
                        if (p.Status == FileStatus.New || p.Status == FileStatus.Delete)
                        {
                            //if (p.FileURL.StartsWith(FileListHelper.BaseUrl))
                            //{
                            //    p.FileURL = "~/" + p.FileURL.Replace(FileListHelper.BaseUrl, string.Empty).Trim('/');
                            //}
                            //p.FileContent = FileListHelper.DownloadFileBytes(p.FileURL);
                            if (p.FileURL.StartsWith(FileListHelper.ApplicatinBaseUrl))
                            {
                                p.FileURL = "~/" + p.FileURL.Replace(FileListHelper.ApplicatinBaseUrl, string.Empty).Trim('/');
                                if (p.FileURL.Contains("/Uploads/") || p.FileURL.Contains("/Sample/"))
                                {
                                    p.FileContent = FileListHelper.DownloadFileBytes(p.FileURL);
                                }
                            }
                        }
                        else
                        {
                            p.FileContent = CommonBusinessLayer.Instance.DownloadFile(p.FileURL, "DCN");
                        }
                    });
                    model.FNLPresentSpecification = string.Join(",", FileListHelper.GetFileNames(model.FNLPresentSpecification));
                    model.FNLRevisedSpecification = string.Join(",", FileListHelper.GetFileNames(model.FNLRevisedSpecification));

                    list.Add(model);
                    this.SetTempData<List<Specification>>(TempKeys.Specification.ToString() + "_" + GetFormIdFromUrl(), list.OrderBy(x => x.Index).ToList());
                    status.Messages.Add(this.GetResourceValue("Text_SpecificationSave", System.Web.Mvc.Html.ResourceNames.DCN));
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.DCN);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.DCN);
                }
            }
            return this.Json(status);
        }

        /// <summary>
        /// Gets the Specification details grid.
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetSpecification()
        {
            List<Specification> list = new List<Specification>();
            list = this.GetTempData<List<Specification>>(TempKeys.Specification.ToString() + "_" + GetFormIdFromUrl()).Where(x => x.ItemAction != ItemActionStatus.DELETED).ToList();
            return this.PartialView("_SpecificationGrid", list.ToList<ITrans>());
        }

        /// <summary>
        /// Deletes the Specification detail.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        public ActionResult DeleteSpecification(int index)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            List<Specification> list = new List<Specification>();
            list = this.GetTempData<List<Specification>>(TempKeys.Specification.ToString() + "_" + GetFormIdFromUrl());
            Specification item = list.FirstOrDefault(x => x.Index == index);
            list.RemoveAll(x => x.Index == index);
            if (item != null && item.ID > 0)
            {
                item.ItemAction = ItemActionStatus.DELETED;
                list.Add(item);
            }
            this.SetTempData<List<Specification>>(TempKeys.Specification.ToString() + "_" + GetFormIdFromUrl(), list.OrderBy(x => x.Index).ToList());

            status.Messages.Add(this.GetResourceValue("Text_SpecificationDeleted", System.Web.Mvc.Html.ResourceNames.DCN));
            return this.Json(status, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "CRUD Revised Applicable Doc Detail"

        /// <summary>
        /// Adds the edit Revised Applicable Doc item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult AddEditRevisedAppDoc(int index = 0)
        {
            List<RevisedApplicableDocuments> list = new List<RevisedApplicableDocuments>();
            list = this.GetTempData<List<RevisedApplicableDocuments>>(TempKeys.RevisedApplicableDoc.ToString() + "_" + GetFormIdFromUrl());
            RevisedApplicableDocuments item = null;
            if (index == 0)
            {
                item = new RevisedApplicableDocuments() { Index = 0, RequestDate = DateTime.Now, RequestBy = this.CurrentUser.UserId };
            }
            else
            {
                item = list.FirstOrDefault(x => x.Index == index);
                //item.FileNameList = item.Files != null && !item.FileNameList.Contains("[{") ? JsonConvert.SerializeObject(item.Files) : item.FileNameList;
                item.FileNameList = item.Files != null ? JsonConvert.SerializeObject(item.Files) : !string.IsNullOrEmpty(item.FileNameList) && !item.FileNameList.Contains("[{") ? item.FileNameList : string.Empty;
            }
            return this.PartialView("_AddRevisedDoc", item);
        }

        /// <summary>
        /// Saves the Revised Applicable Doc item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Action Result</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveRevisedAppDoc(RevisedApplicableDocuments model)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    List<RevisedApplicableDocuments> list = new List<RevisedApplicableDocuments>();
                    list = this.GetTempData<List<RevisedApplicableDocuments>>(TempKeys.RevisedApplicableDoc.ToString() + "_" + GetFormIdFromUrl());
                    if (model.Index == 0)
                    {
                        model.Index = list.Count + 1;
                        model.ItemAction = ItemActionStatus.NEW;
                        model.RequestDate = !model.RequestDate.HasValue ? DateTime.Now : model.RequestDate.Value;
                    }
                    else
                    {
                        list.RemoveAll(x => x.Index == model.Index);
                    }
                    if (model.ID > 0)
                    {
                        model.ItemAction = ItemActionStatus.UPDATED;
                    }

                    model.Files = new List<FileDetails>();
                    if (!string.IsNullOrEmpty(model.FileNameList))
                        model.Files.AddRange(JsonConvert.DeserializeObject<List<FileDetails>>(model.FileNameList));

                    model.Files.ForEach(p =>
                    {
                        if (p.Status == FileStatus.New || p.Status == FileStatus.Delete)
                        {
                            if (p.FileURL.StartsWith(FileListHelper.ApplicatinBaseUrl))
                            {
                                p.FileURL = "~/" + p.FileURL.Replace(FileListHelper.ApplicatinBaseUrl, string.Empty).Trim('/');
                                if (p.FileURL.Contains("/Uploads/") || p.FileURL.Contains("/Sample/"))
                                {
                                    p.FileContent = FileListHelper.DownloadFileBytes(p.FileURL);
                                }
                            }
                        }
                        else
                        {
                            p.FileContent = CommonBusinessLayer.Instance.DownloadFile(p.FileURL, "DCN");
                        }
                    });
                    model.FileNameList = string.Join(",", FileListHelper.GetFileNames(model.FileNameList));

                    list.Add(model);
                    this.SetTempData<List<RevisedApplicableDocuments>>(TempKeys.RevisedApplicableDoc.ToString() + "_" + GetFormIdFromUrl(), list.OrderBy(x => x.Index).ToList());
                    status.Messages.Add(this.GetResourceValue("Text_RevisedAppDocSave", System.Web.Mvc.Html.ResourceNames.DCN));
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.DCN);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.DCN);
                }
            }
            return this.Json(status);
        }

        /// <summary>
        /// Gets the Revised App Doc grid.
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetRevisedAppDoc()
        {
            List<RevisedApplicableDocuments> list = new List<RevisedApplicableDocuments>();
            list = this.GetTempData<List<RevisedApplicableDocuments>>(TempKeys.RevisedApplicableDoc.ToString() + "_" + GetFormIdFromUrl()).Where(x => x.ItemAction != ItemActionStatus.DELETED).ToList();
            return this.PartialView("_RevisedapplicabledocumentsGrid", list.ToList<ITrans>());
        }

        /// <summary>
        /// Deletes the Revised App Doc.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        public ActionResult DeleteRevisedAppDoc(int index)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            List<RevisedApplicableDocuments> list = new List<RevisedApplicableDocuments>();
            list = this.GetTempData<List<RevisedApplicableDocuments>>(TempKeys.RevisedApplicableDoc.ToString() + "_" + GetFormIdFromUrl());
            RevisedApplicableDocuments item = list.FirstOrDefault(x => x.Index == index);
            list.RemoveAll(x => x.Index == index);
            if (item != null && item.ID > 0)
            {
                item.ItemAction = ItemActionStatus.DELETED;
                list.Add(item);
            }
            this.SetTempData<List<RevisedApplicableDocuments>>(TempKeys.RevisedApplicableDoc.ToString() + "_" + GetFormIdFromUrl(), list.OrderBy(x => x.Index).ToList());
            status.Messages.Add(this.GetResourceValue("Text_RevisedAppDocDeleted", System.Web.Mvc.Html.ResourceNames.DCN));
            return this.Json(status, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "CRUD VendorDCN Detail"

        /// <summary>
        /// Adds the edit Revised Applicable Doc item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult AddEditVendorDCN(int index = 0)
        {
            List<VendorDCN> list = new List<VendorDCN>();
            list = this.GetTempData<List<VendorDCN>>(TempKeys.DCNVendor.ToString() + "_" + GetFormIdFromUrl());
            VendorDCN item = null;
            item = list.FirstOrDefault(x => x.Index == index);
            return this.PartialView("_EditVendorDCN", item);
        }

        /// <summary>
        /// Saves the RVendorDCN item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Action Result</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveVendorDCN(VendorDCN model)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    List<VendorDCN> list = new List<VendorDCN>();
                    list = this.GetTempData<List<VendorDCN>>(TempKeys.DCNVendor.ToString() + "_" + GetFormIdFromUrl());

                    if (model.Index == 0)
                    {
                        model.Index = list.Count + 1;
                        model.ItemAction = ItemActionStatus.NEW;
                    }
                    else
                    {
                        list.RemoveAll(x => x.Index == model.Index);
                    }
                    if (model.ID > 0)
                    {
                        model.ItemAction = ItemActionStatus.UPDATED;
                    }
                    list.Add(model);
                    this.SetTempData<List<VendorDCN>>(TempKeys.DCNVendor.ToString() + "_" + GetFormIdFromUrl(), list.OrderBy(x => x.Index).ToList());
                    status.Messages.Add(this.GetResourceValue("Text_VendorDCNSave", System.Web.Mvc.Html.ResourceNames.DCN));
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.DCN);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.DCN);
                }
            }
            return this.Json(status);
        }

        /// <summary>
        /// Gets the VendorDCN grid.
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetVendorDCN()
        {
            List<VendorDCN> list = new List<VendorDCN>();
            list = this.GetTempData<List<VendorDCN>>(TempKeys.DCNVendor.ToString() + "_" + GetFormIdFromUrl()).Where(x => x.ItemAction != ItemActionStatus.DELETED).ToList();
            return this.PartialView("_qaVendorGrid", list.ToList<ITrans>());
        }



        #endregion
    }
}