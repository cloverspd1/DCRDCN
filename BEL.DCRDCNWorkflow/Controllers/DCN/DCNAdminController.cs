
namespace BEL.DCRDCNWorkflow.Controllers.DCN
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.Common;
    using BEL.DCRDCNWorkflow.Models;
    using BEL.DCRDCNWorkflow.Models.Common;
    using BEL.DCRDCNWorkflow.BusinessLayer;
    using BEL.DCRDCNWorkflow.Models.DCN;
    using Newtonsoft.Json;


    public class DCNAdminController : DCNBaseController
    {
        // GET: DCNAdmin
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), SharePointContextFilter]
        public ActionResult DCNAdminIndex(int id = 0)
        {
            if (id > 0 && DCRBusinessLayer.Instance.IsAdminUser(this.CurrentUser.UserId))
            {
                Dictionary<string, string> objDict = new Dictionary<string, string>();
                objDict.Add(Parameter.FROMNAME, FormNames.DCNADMINFORM);
                objDict.Add(Parameter.ITEMID, id.ToString());
                objDict.Add(Parameter.USEREID, this.CurrentUser.UserId);
                ViewBag.UserID = this.CurrentUser.UserId;
                ViewBag.UserName = this.CurrentUser.FullName;
                DCNAdminContract dcnContract = this.GetDCNAdminDetails(objDict);
                dcnContract.UserDetails = this.CurrentUser;

                if (dcnContract != null)
                {
                    var form = dcnContract.Forms.FirstOrDefault();
                    if (form != null)
                    {
                        var dcnDetailAdminSection = form.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCNSectionName.DCNDETAILADMINSECTION)) as DCNDetailAdminSection;
                        ApplicationStatusSection dcnApprovalSection = dcnContract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == SectionNameConstant.APPLICATIONSTATUS) as ApplicationStatusSection;
                        if (dcnDetailAdminSection != null)
                        {
                            this.SetTranListintoTempData<Specification>(dcnDetailAdminSection.Specification, TempKeys.Specification.ToString() + "_" + id);
                            this.SetTranListintoTempData<RevisedApplicableDocuments>(dcnDetailAdminSection.RevisedApplicable, TempKeys.RevisedApplicableDoc.ToString() + "_" + id);
                            this.SetTranListintoTempData<VendorDCN>(dcnDetailAdminSection.VendorDCRList, TempKeys.DCRVendor.ToString() + "_" + id);
                            this.SetTranListintoTempData<VendorDCN>(dcnDetailAdminSection.VendorDCNList, TempKeys.DCNVendor.ToString() + "_" + id);
                            if (dcnApprovalSection != null)
                            {
                                dcnDetailAdminSection.ApproversList = dcnApprovalSection.ApplicationStatusList;
                                dcnDetailAdminSection.DCRProcessIComment = dcnDetailAdminSection.ApproversList.FirstOrDefault(p => p.Role == DCNRoles.DCRPROCESSINCHARGE).Comments;
                                dcnDetailAdminSection.SCMComment = dcnDetailAdminSection.ApproversList.FirstOrDefault(p => p.Role == DCNRoles.SCM) != null ? dcnDetailAdminSection.ApproversList.FirstOrDefault(p => p.Role == DCNRoles.SCM).Comments : string.Empty;
                                //dcnDetailAdminSection.DesignEngComment = dcnDetailAdminSection.ApproversList.FirstOrDefault(p => p.Role == DCNRoles.DESIGNENGINEER).Comments;
                            }

                        }

                    }
                    return this.View(dcnContract);
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

        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveDCNAdminForm(DCNDetailAdminSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {

                if (this.ValidateModelState(model))
                {

                    var specification = this.GetTempData<List<Specification>>(TempKeys.Specification.ToString() + "_" + GetFormIdFromUrl());
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
                    //model.DCNNo = model.DCRNo.Replace("DCR", "DCR-DCN");


                    if (model.VendorDCNPostList != null && model.VendorDCNPostList.Count != 0)
                    {
                        model.VendorDCNPostList.ToList().ForEach(p => p.ItemAction = ItemActionStatus.UPDATED);
                        model.VendorDCNList = model.VendorDCNPostList.ToList<ITrans>();
                    }

                    if (model.ApproversList != null)
                    {
                        if (model.DCRProcessIComment == null)
                        {
                            model.DCRProcessIComment = " ";
                        }
                        model.ApproversList.FirstOrDefault(p => p.Role == DCNRoles.DCRPROCESSINCHARGE).Comments = model.DCRProcessIComment;
                        //if(model.DesignEngComment==null)
                        //{
                        //    model.DesignEngComment = " ";
                        //}
                        //model.ApproversList.FirstOrDefault(p => p.Role == DCNRoles.DESIGNENGINEER).Comments = model.DesignEngComment;
                        if (model.ApproversList.FirstOrDefault(p => p.Role == DCNRoles.SCM) != null)
                        {
                            if (model.SCMComment == null)
                            {
                                model.ApproversList.FirstOrDefault(p => p.Role == DCNRoles.SCM).Comments = " ";
                            }
                            else
                            {
                                model.ApproversList.FirstOrDefault(p => p.Role == DCNRoles.SCM).Comments = model.SCMComment;
                            }
                        }                       
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
                item.FNLPresentSpecification = item.Files != null && item.Files.Count > 0 && !item.FNLRevisedSpecification.Contains("[{") ? JsonConvert.SerializeObject(item.Files.Where(x => !string.IsNullOrEmpty(item.FNLPresentSpecification) && item.FNLPresentSpecification.Split(',').Contains(x.FileName)).ToList()) : item.FNLPresentSpecification;
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
                    if (!string.IsNullOrEmpty(model.FNLPresentSpecification))
                        model.Files.AddRange(JsonConvert.DeserializeObject<List<FileDetails>>(model.FNLPresentSpecification));
                    if (!string.IsNullOrEmpty(model.FNLRevisedSpecification))
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
                            //if (p.FileURL.StartsWith(FileListHelper.ApplicatinBaseUrl))
                            //{
                            //    p.FileURL = "~/" + p.FileURL.Replace(FileListHelper.ApplicatinBaseUrl, string.Empty).Trim('/');
                            //}
                            if (p.FileURL.StartsWith(FileListHelper.ApplicatinBaseUrl))
                            {
                                p.FileURL = "~/" + p.FileURL.Replace(FileListHelper.ApplicatinBaseUrl, string.Empty).Trim('/');
                                if (p.FileURL.Contains("/Uploads/") || p.FileURL.Contains("/Sample/"))
                                {
                                    p.FileContent = FileListHelper.DownloadFileBytes(p.FileURL);
                                }
                            }
                            else
                            {
                                p.FileContent = CommonBusinessLayer.Instance.DownloadFile(p.FileURL, "DCN");
                            }
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
                item.FileNameList = item.Files != null && !item.FileNameList.Contains("[{") ? JsonConvert.SerializeObject(item.Files) : item.FileNameList;
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
                            else
                            {
                                p.FileContent = CommonBusinessLayer.Instance.DownloadFile(p.FileURL, "DCN");
                            }
                            // p.FileContent = FileListHelper.DownloadFileBytes(p.FileURL);
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
    }
}