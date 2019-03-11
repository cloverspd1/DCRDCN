namespace BEL.DCRDCNWorkflow.Controllers.DCR
{
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.BusinessLayer;
    using BEL.DCRDCNWorkflow.Common;
    using BEL.DCRDCNWorkflow.Models;
    using BEL.DCRDCNWorkflow.Models.Common;
    using BEL.DCRDCNWorkflow.Models.DCR;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class DCRAdminController : DCRBaseController
    {
        // GET: DCRAdmin
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), SharePointContextFilter]
        public ActionResult DCRAdminIndex(int id = 0)
        {
            DCRAdminContract contract = null;
            if (id > 0 && DCRBusinessLayer.Instance.IsAdminUser(this.CurrentUser.UserId))
            {
                Dictionary<string, string> objDict = new Dictionary<string, string>();
                objDict.Add("FormName", FormNames.DCRADMINFORM);
                objDict.Add("ItemId", id.ToString());
                objDict.Add(Parameter.USEREID, this.CurrentUser.UserId);
                ViewBag.UserID = this.CurrentUser.UserId;
                ViewBag.UserName = this.CurrentUser.FullName;

                contract = this.GetAdminDCRDetails(objDict);

                if (contract.Forms != null && contract.Forms.Count > 0)
                {

                    DCRAdminDetailSection dcrAdminDetailSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.DCRDETAILADMINSECTION) as DCRAdminDetailSection;
                    ApplicationStatusSection dcrApprovalSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == SectionNameConstant.APPLICATIONSTATUS) as ApplicationStatusSection;
                    if (dcrAdminDetailSection != null)
                    {
                        this.SetTranListintoTempData<VendorDCR>(dcrAdminDetailSection.VendorDCRList, TempKeys.DCRVendor.ToString() + "_" + id);
                        dcrAdminDetailSection.Division = dcrAdminDetailSection.Division.LastIndexOf(",") != -1 ? dcrAdminDetailSection.Division.TrimEnd(',') : dcrAdminDetailSection.Division;
                        if (dcrApprovalSection != null)
                        {
                            dcrAdminDetailSection.ApproversList = dcrApprovalSection.ApplicationStatusList;
                            dcrAdminDetailSection.QAIComment = dcrAdminDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.QAINCHARGE).Comments;
                            dcrAdminDetailSection.DCRProcessIComment = dcrAdminDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DCRPROCESSINCHARGE).Comments;
                            dcrAdminDetailSection.DCRInchargeNavigatorComment = dcrAdminDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DCRINCHARGENAVIGATOR).Comments;
                            // dcrAdminDetailSection.DesignEngineerComment = dcrAdminDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DESIGNDEVELOPENGINEER).Comments;
                            dcrAdminDetailSection.DesignEngineerIComment = dcrAdminDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DESIGNENGINEERINCHARGE) != null ? dcrAdminDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DESIGNENGINEERINCHARGE).Comments : string.Empty;
                            dcrAdminDetailSection.SCMIComment = dcrAdminDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.SCMINCHARGE).Comments;
                            dcrAdminDetailSection.LUMIComment = dcrAdminDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.LUMMARKETINGINCHARGE).Comments;
                            dcrAdminDetailSection.KAPIComment = dcrAdminDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.KAPMARKETINGINCHARGE).Comments;
                            dcrAdminDetailSection.DAPIComment = dcrAdminDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DAPMARKETINGINCHARGE).Comments;
                            dcrAdminDetailSection.FANIComment = dcrAdminDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.FANSMARKETINGINCHARGE).Comments;
                            dcrAdminDetailSection.LightingIComment = dcrAdminDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.LIGHTINGMARKETINGINCHARGE).Comments;
                            dcrAdminDetailSection.MRMarketingIComment = dcrAdminDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.MORPHYRICHARDSMARKETINGINCHARGE).Comments;
                        }
                    }
                    return this.View(contract);
                }
                else
                {
                    return this.RedirectToAction("NotAuthorize", "Master");
                }
            }
            else
            {
                return this.RedirectToAction("NotAuthorize", "Master");
            }


        }

        [HttpGet]
        public ActionResult AddVendor()
        {
            return this.PartialView("_AddVendor");
        }

        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), HttpPost, ValidateAntiForgeryToken]//ValidateAntiForgeryTokenOnAllPosts
        public ActionResult SaveDCRAdminForm(DCRAdminDetailSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.OldDCRNo))
                {
                    ModelState.Remove("DesignEngineer");
                    ModelState.Remove("DDHODDISPOSAL");
                    ModelState.Remove("IfNotConsidered");
                    ModelState.Remove("ReworkComments");
                    ModelState.Remove("MarketingApprovalRequired");
                }


                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        if (model.BusinessUnit.Contains("(CP)"))
                        {
                            model.DCRInchargeNavigator = string.Empty;
                            model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DCRINCHARGENAVIGATOR).Status = ApproverStatus.NOTREQUIRED;
                        }
                        else
                        {
                            model.DCRInchargeNavigator = model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DCRINCHARGENAVIGATOR).Approver;
                            model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DCRPROCESSINCHARGE).Approver = string.Empty;
                        }
                    }
                    if (model.ApproversList != null)
                    {
                        if (model.QAIComment == null)
                        {
                            model.QAIComment = " ";
                        }
                        model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.QAINCHARGE).Comments = model.QAIComment;
                        if (model.DCRProcessIComment == null)
                        {
                            model.DCRProcessIComment = " ";
                        }
                        model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DCRPROCESSINCHARGE).Comments = model.DCRProcessIComment;
                        if (model.DCRInchargeNavigatorComment == null)
                        {
                            model.DCRInchargeNavigatorComment = " ";
                        }
                        model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DCRINCHARGENAVIGATOR).Comments = model.DCRInchargeNavigatorComment;
                        if (model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DESIGNENGINEERINCHARGE) != null)
                        {
                            if (model.DesignEngineerIComment == null)
                            {
                                model.DesignEngineerIComment = " ";
                            }
                            model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DESIGNENGINEERINCHARGE).Comments = model.DesignEngineerIComment;
                        }
                        if (model.SCMIComment == null) 
                        {
                            model.SCMIComment = " ";
                        }
                        model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.SCMINCHARGE).Comments = model.SCMIComment;
                        if (model.LUMIComment == null)
                        {
                            model.LUMIComment = " ";
                        }
                        model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.LUMMARKETINGINCHARGE).Comments = model.LUMIComment;
                        if (model.KAPIComment == null)
                        {
                            model.KAPIComment = " ";
                        }
                        model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.KAPMARKETINGINCHARGE).Comments = model.KAPIComment;
                        if (model.DAPIComment == null)
                        {
                            model.DAPIComment = " ";
                        }
                        model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DAPMARKETINGINCHARGE).Comments = model.DAPIComment;
                        if (model.FANIComment==null)
                        {
                            model.FANIComment = " ";
                        }
                        model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.FANSMARKETINGINCHARGE).Comments = model.FANIComment;
                        if(model.LightingIComment==null)
                        {
                            model.LightingIComment = " ";
                        }
                        model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.LIGHTINGMARKETINGINCHARGE).Comments = model.LightingIComment;
                        if (model.MRMarketingIComment == null)
                        {
                            model.MRMarketingIComment = " ";
                        }
                        model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.MORPHYRICHARDSMARKETINGINCHARGE).Comments = model.MRMarketingIComment;

                    }

                    ////file Operation
                    model.Files = new List<FileDetails>();
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.FNLDesignChangeAttachment));
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.FNLExpectedResultsAttachment));
                    model.FNLDesignChangeAttachment = string.Join(",", FileListHelper.GetFileNames(model.FNLDesignChangeAttachment));
                    model.FNLExpectedResultsAttachment = string.Join(",", FileListHelper.GetFileNames(model.FNLExpectedResultsAttachment));

                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.FNLDCRAttachment));
                    model.FNLDCRAttachment = string.Join(",", FileListHelper.GetFileNames(model.FNLDCRAttachment));

                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.FNLDEAttachment));
                    model.FNLDEAttachment = string.Join(",", FileListHelper.GetFileNames(model.FNLDEAttachment));
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.FNLQATestReport));
                    model.FNLQATestReport = string.Join(",", FileListHelper.GetFileNames(model.FNLQATestReport));

                    var list = this.GetTempData<List<VendorDCR>>(TempKeys.DCRVendor.ToString() + "_" + GetFormIdFromUrl());
                    model.VendorDCRList = list.ToList<ITrans>();
                    //model.RejectedComment = model.IfNotConsidered + " " + model.CommentsAfterProtoTesting;

                    Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);
                    status = this.SaveSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.DCR);
                }

                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.DCR);
                }
            }

            return this.Json(status);
        }

        #region "CRUD Vendor Detail"



        /// <summary>
        /// Adds the edit Supplier item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult AddEditVendor(int index = 0)
        {
            List<VendorDCR> list = new List<VendorDCR>();
            list = this.GetTempData<List<VendorDCR>>(TempKeys.DCRVendor.ToString() + "_" + GetFormIdFromUrl());
            VendorDCR item = null;
            if (index == 0)
            {
                item = new VendorDCR() { Index = 0, RequestDate = DateTime.Now, RequestBy = this.CurrentUser.UserId };
            }
            else
            {
                item = list.FirstOrDefault(x => x.Index == index);
            }
            return this.PartialView("_AddVendor", item);
        }

        /// <summary>
        /// Saves the Supplier detail item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Action Result</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveVendor(VendorDCR model)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    List<VendorDCR> list = new List<VendorDCR>();
                    list = this.GetTempData<List<VendorDCR>>(TempKeys.DCRVendor.ToString() + "_" + GetFormIdFromUrl());
                    if (list.Any(p => p.VendorName == model.VendorName && p.Index != model.Index))
                    {
                        status.IsSucceed = false;
                        status.Messages.Add(this.GetResourceValue("Text_UniqeVendor", System.Web.Mvc.Html.ResourceNames.DCR));
                        status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.DCR);
                        return this.Json(status);
                    }
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
                    this.SetTempData<List<VendorDCR>>(TempKeys.DCRVendor.ToString() + "_" + GetFormIdFromUrl(), list.OrderBy(x => x.Index).ToList());
                    status.Messages.Add(this.GetResourceValue("Text_VendorSave", System.Web.Mvc.Html.ResourceNames.DCR));
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.DCR);

                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.DCR);
                }
            }
            return this.Json(status);
        }

        /// <summary>
        /// Gets the Supplier details grid.
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetVendorGrid()
        {
            List<VendorDCR> list = new List<VendorDCR>();
            list = this.GetTempData<List<VendorDCR>>(TempKeys.DCRVendor.ToString() + "_" + GetFormIdFromUrl()).Where(x => x.ItemAction != ItemActionStatus.DELETED).ToList();
            return this.PartialView("_VendorGrid", list.ToList<ITrans>());
        }

        /// <summary>
        /// Deletes the Supplier detail.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        public ActionResult VendorDelete(int index)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            List<VendorDCR> list = new List<VendorDCR>();
            list = this.GetTempData<List<VendorDCR>>(TempKeys.DCRVendor.ToString() + "_" + GetFormIdFromUrl());
            VendorDCR item = list.FirstOrDefault(x => x.Index == index);
            list.RemoveAll(x => x.Index == index);
            if (item != null && item.ID > 0)
            {
                item.ItemAction = ItemActionStatus.DELETED;
                list.Add(item);
            }
            this.SetTempData<List<VendorDCR>>(TempKeys.DCRVendor.ToString() + "_" + GetFormIdFromUrl(), list.OrderBy(x => x.Index).ToList());
            status.Messages.Add(this.GetResourceValue("Text_VendorDeleted", System.Web.Mvc.Html.ResourceNames.DCR));
            return this.Json(status, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}