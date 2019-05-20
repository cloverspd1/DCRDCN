namespace BEL.DCRDCNWorkflow.Controllers
{
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.BusinessLayer;
    using BEL.DCRDCNWorkflow.Common;
    using BEL.DCRDCNWorkflow.Models.Common;
    using BEL.DCRDCNWorkflow.Models.DCR;
    using iTextSharp.text;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// PR Controller used for saving Market visit report
    /// </summary>
    public class DCRController : DCRBaseController
    {
        #region " Global Section & Save "


        #region "DCR Index"
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// Index View
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        [SharePointContextFilter]
        public ActionResult Index(int id = 0, bool IsRetrieve = false)
        {
            DCRContract contract = null;
            if (!IsRetrieve)
            {
                Dictionary<string, string> objDict = new Dictionary<string, string>();
                objDict.Add("FormName", FormNameConstants.DCRFORM);
                objDict.Add("ItemId", id.ToString());
                objDict.Add(Parameter.USEREID, this.CurrentUser.UserId);
                ViewBag.UserID = this.CurrentUser.UserId;
                ViewBag.UserName = this.CurrentUser.FullName;


                contract = this.GetDCRDetails(objDict);
                contract.UserDetails = this.CurrentUser;
                if (contract.Forms != null && contract.Forms.Count > 0)
                {
                    if (id == 0)
                    {
                        ViewBag.RequestDepartment = contract.UserDetails.Department;
                    }

                    //To fix issue of file attchment in DCR Process Incharge
                    /*   var dcrDetailSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.DCRDETAILSECTION) as DCRDetailSection;
                    
                       if (dcrDetailSection !=null)
                       {
                           // 22 Feb 2018 Written By Priya for handle case of fields getting blank in case of Rework
                           if (dcrDetailSection.DDHODDISPOSAL != "Not Consider")
                           {
                               dcrDetailSection.MarketingApprovalRequired = dcrProcessInchrageDetailSection.MarketingApprovalRequired;
                               dcrDetailSection.TargetDateOfImplementation = dcrProcessInchrageDetailSection.TargetDateOfImplementation;
                               dcrDetailSection.DDHODDISPOSAL = dcrProcessInchrageDetailSection.DDHODDISPOSAL;
                           }
                           if (dcrProcessInchrageDetailSection.DDHODDISPOSAL != null)
                           {
                               if (dcrProcessInchrageDetailSection.Files != null && dcrProcessInchrageDetailSection.Files.Count > 0)
                               {
                                   dcrDetailSection.FNLDCRAttachment = dcrProcessInchrageDetailSection.FNLDCRAttachment;
                               }
                           }
                       }
                 
                       if ( dcrDetailSection != null)
                       {
                           if (dcrDetailSection.ConsiderRework != null)
                           {
                               if (dcrDetailSection.ConsiderRework == "Rework")
                               {
                                   if (dcrProcessInchrageDetailSection.Files != null && dcrProcessInchrageDetailSection.Files.Count > 0)
                                   {
                                       dcrDetailSection.FNLDCRAttachment = dcrProcessInchrageDetailSection.FNLDCRAttachment;
                                   }
                               }
                           }
                       }*/

                    SCMInchargeSection scmInchargeSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.SCMINCHARGESECTION) as SCMInchargeSection;
                    if (scmInchargeSection != null && scmInchargeSection.IsActive)
                    {
                        this.SetTranListintoTempData<VendorDCR>(scmInchargeSection.VendorDCRList, TempKeys.DCRVendor.ToString() + "_" + id);
                    }
                    SCMSection scmSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.SCMSECTION) as SCMSection;
                    if (scmSection != null && scmSection.IsActive)
                    {
                        this.SetTranListintoTempData<VendorDCR>(scmSection.VendorDCRList, TempKeys.DCRVendor.ToString() + "_" + id);
                    }

                    /* Removed restriction to rework for 1 time only, user can rework as much as required */
                    //185477511 - Addition a rework option for DCR Process In charge, in addition to 'Considered' and 'Not Considered' start
                    //if (id > 0)
                    //{   
                    //    DCRProcessInchargeSection dcrProcessInchargeSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCRSectionName.DCRPROCESSINCHARGESECTION)) as DCRProcessInchargeSection;
                    //    if (dcrProcessInchargeSection.IsActive == true)
                    //    {
                    //        ActivityLogSection activityLog = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCRSectionName.ACTIVITYLOG)) as ActivityLogSection;
                    //        bool isReworkDoneOnce = activityLog.ActivityLogs.Any(f => f.Activity.Trim() == "Rework" && f.SectionName == DCRSectionName.DCRPROCESSINCHARGESECTION);
                    //        dcrProcessInchargeSection.IsReworkRequired = !isReworkDoneOnce;
                    //    }
                    //}
                    //185477511 - Addition a rework option for DCR Process In charge, in addition to 'Considered' and 'Not Considered' end
                    return this.View(contract);
                }
                else
                {
                    return this.RedirectToAction("NotAuthorize", "Master");
                }
            }
            else
            {
                Dictionary<string, string> objDict = new Dictionary<string, string>();
                objDict.Add("FormName", FormNameConstants.DCRFORM);
                objDict.Add("ItemId", id.ToString());
                objDict.Add(Parameter.USEREID, this.CurrentUser.UserId);
                ViewBag.UserID = this.CurrentUser.UserId;
                ViewBag.UserName = this.CurrentUser.FullName;

                contract = this.GetDCRDetails(objDict);

                if (contract != null && contract.Forms != null && contract.Forms.Count != 0)
                {
                    contract.Forms[0].FormStatus = string.Empty;

                    Button btn = new Button();
                    btn.Name = "Resume";
                    btn.ButtonStatus = ButtonActionStatus.NextApproval;
                    btn.JsFunction = "ConfirmSubmit";
                    btn.IsVisible = true;
                    btn.Icon = "fa fa-save";
                    contract.Forms[0].Buttons.Add(btn);
                    contract.Forms[0].Buttons.Remove(contract.Forms[0].Buttons.FirstOrDefault(f => f.ButtonStatus == ButtonActionStatus.Print));

                    var dcrDetailSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.DCRDETAILSECTION) as DCRDetailSection;
                    if (dcrDetailSection != null)
                    {
                        ////If Login user is not DCR process Incharge then error Unauthorized.
                        if (dcrDetailSection.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DCRPROCESSINCHARGE).Approver != this.CurrentUser.UserId)
                        {
                            return this.RedirectToAction("NotAuthorize", "Master");
                        }
                        dcrDetailSection.OldDCRNo = dcrDetailSection.DCRNo;
                        dcrDetailSection.OldDCRCreatedDate = dcrDetailSection.RequestDate;
                        ////dcrDetailSection.OldDCRRejectedDate = dcrDetailSection.DCRRejectedDate;
                        dcrDetailSection.OldDCRId = id;
                        dcrDetailSection.ProposedBy = CurrentUser.UserId;
                        dcrDetailSection.ProposedByName = CurrentUser.FullName;
                        dcrDetailSection.DCRNo = string.Empty;
                        dcrDetailSection.RequestDate = null;
                        dcrDetailSection.WorkflowStatus = string.Empty;
                        dcrDetailSection.IsActive = true;
                        dcrDetailSection.ListDetails[0].ItemId = 0;
                        dcrDetailSection.DDHODDISPOSAL = string.Empty;
                        dcrDetailSection.CurrentApprover.Approver = CurrentUser.UserId;
                        dcrDetailSection.CurrentApprover.ApproverName = CurrentUser.FullName;
                        //  dcrDetailSection.ApproversList[0].Approver = dcrProcessIcSection.ApproversList[0].Approver;

                    }

                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.DCRINCHARGENAVIGATORSECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.DESIGNENGINEERINCHARGESECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.DESIGNENGINEERSECTION));

                    //185479517 , modify section name
                    if (contract.Forms[0].SectionsList.Any(x => x.SectionName == DCRSectionName.DESIGNDEVELOPENGINEERSECTION))
                    {
                        contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.DESIGNDEVELOPENGINEERSECTION));
                    }


                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.SCMINCHARGESECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.SCMSECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.QAINCHARGESECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.QASECTION));

                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.DAPMARKETINGINCHARGESECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.DAPMARKETINGSECTION));

                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.KAPMARKETINGINCHARGESECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.KAPMARKETINGSECTION));

                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.FANSMARKETINGINCHARGESECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.FANSMARKETINGSECTION));

                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.LIGHTINGMARKETINGINCHARGESECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.LIGHTINGMARKETINGSECTION));

                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.MORPHYRECHARDMARKETINGINCHARGESECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.MORPHYRECHARDMARKETINGSECTION));

                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.LUMMARKETINGINCHARGESECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.LUMMARKETINGSECTION));

                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.CCINCHARGESECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.COSTINGSECTION));

                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.FINALDCRPROCESSINCHARGESECTION));

                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.ACTIVITYLOG));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == DCRSectionName.APPLICATIONSTATUSSECTION));


                    return this.View(contract);
                }
                else
                {
                    return this.RedirectToAction("NotAuthorize", "Master");
                }

            }
        }
        #endregion

        #region "DCR Detail Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), HttpPost, ValidateAntiForgeryToken]//ValidateAntiForgeryTokenOnAllPosts
        public ActionResult SavedcrDetailSection(DCRDetailSection model)
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
                    ModelState.Remove("TargetDateOfImplementation");

                }
                else
                {
                    model.CurrentApprover.Approver = this.CurrentUser.UserId;
                }

                if (model.ActionStatus == ButtonActionStatus.NextApproval && model.ButtonCaption.Trim() == "Resume")
                {
                    if (model.DDHODDISPOSAL == "Consider")
                    {
                        ModelState.Remove("IfNotConsidered");
                        ModelState.Remove("ReworkComments");
                        //model.ActionStatus = ButtonActionStatus.NextApproval;
                        model.IfNotConsidered = string.Empty;
                        model.ReworkComments = string.Empty;
                    }
                    else if (model.DDHODDISPOSAL == "Not Consider")
                    {
                        //ModelState.Remove("DesignEngineerIncharge");
                        ModelState.Remove("DesignEngineer");
                        ModelState.Remove("TargetDateOfImplementation");
                        model.ReworkComments = string.Empty;
                        model.CurrentApprover.Comments = string.Empty;
                    }
                    else if (model.DDHODDISPOSAL == "Rework")
                    {
                        //ModelState.Remove("DesignEngineerIncharge");
                        ModelState.Remove("DesignEngineer");
                        ModelState.Remove("TargetDateOfImplementation");
                        model.IfNotConsidered = string.Empty;
                        model.CurrentApprover.Comments = string.Empty;
                    }
                }

                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        ////Remove DCR Navigator section for LUM BU
                        ////if (model.BusinessUnit.Contains("(CP)"))
                        ////{
                        model.DCRInchargeNavigator = string.Empty;
                        model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DCRINCHARGENAVIGATOR).Status = ApproverStatus.NOTREQUIRED;
                        model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DCRINCHARGENAVIGATOR).Approver = string.Empty;
                        ////}
                        ////else
                        ////{
                        ////    model.DCRInchargeNavigator = model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DCRINCHARGENAVIGATOR).Approver;
                        ////    model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.DCRPROCESSINCHARGE).Approver = string.Empty;
                        ////    model.ApproversList.FirstOrDefault(p => p.Role == DCRRoles.FINALDCRPROCESSINCHARGE).Approver = string.Empty;
                        ////}

                    }


                    model.Files = new List<FileDetails>();
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.FNLDesignChangeAttachment));
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.FNLExpectedResultsAttachment));
                    model.FNLDesignChangeAttachment = string.Join(",", FileListHelper.GetFileNames(model.FNLDesignChangeAttachment));
                    model.FNLExpectedResultsAttachment = string.Join(",", FileListHelper.GetFileNames(model.FNLExpectedResultsAttachment));
                    if (model.FNLDesignChangeAttachment == "" || model.FNLExpectedResultsAttachment == "" || (model.FNLDesignChangeAttachment != model.FNLExpectedResultsAttachment))
                    {

                        if (model.DDHODDISPOSAL == "Rework")
                        {
                            if (model.FNLDCRAttachment != null && model.FNLDCRAttachment != string.Empty)
                            {
                                dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(model.FNLDCRAttachment);
                                jsonObject[0].Status = "2";
                                var modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
                                model.Files.AddRange(FileListHelper.GenerateFileBytes(modifiedJsonString));
                                model.FNLDCRAttachment = string.Join(",", FileListHelper.GetFileNames(modifiedJsonString));
                            }
                            model.TargetDateOfImplementation = null;
                           //// model.MarketingApprovalRequired = false;
                        }
                        else if (model.ConsiderRework == "Rework")
                        {
                            if (model.FNLDCRAttachment != null && model.FNLDCRAttachment != "" && model.FNLDCRAttachment != string.Empty)
                            {
                                model.Files.AddRange(FileListHelper.GenerateFileBytes(model.FNLDCRAttachment));
                                model.FNLDCRAttachment = string.Join(",", FileListHelper.GetFileNames(model.FNLDCRAttachment));
                            }
                        }


                        Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);
                        status = this.SaveSection(model, objDict);
                        status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.DCR);
                    }
                    else
                    {
                        List<string> errorList = new List<string>();
                        string error = "You cannot upload files with same name";
                        errorList.Add(error);

                        status.IsSucceed = false;
                        status.Messages = errorList;
                        return this.Json(status);
                    }
                }

                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.DCR);
                }
            }

            return this.Json(status);
        }
        #endregion

        #region "DCR Process Navigator"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveDCRInchargeNavigatorSection(DCRInchargeNavigatorSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    model.ApproversList.ForEach(p =>
                    {
                        if (p.Role == DCRRoles.DCRPROCESSINCHARGE || p.Role == DCRRoles.FINALDCRPROCESSINCHARGE)
                        {
                            p.Approver = model.DCRIncharge;
                        }
                    });

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
        #endregion

        #region "DCR Process Incharge"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveDCRProcessInchargeSection(DCRProcessInchargeSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);
                if (model.ActionStatus == ButtonActionStatus.NextApproval)
                {
                    if (model.DDHODDISPOSAL == "Consider")
                    {
                        ModelState.Remove("IfNotConsidered");
                        ModelState.Remove("ReworkComments");
                        model.ActionStatus = ButtonActionStatus.NextApproval;
                        model.IfNotConsidered = string.Empty;
                        model.ReworkComments = string.Empty;
                    }
                    else if (model.DDHODDISPOSAL == "Not Consider")
                    {
                        //ModelState.Remove("DesignEngineerIncharge");
                        ModelState.Remove("DesignEngineer");
                        ModelState.Remove("TargetDateOfImplementation");
                        model.ActionStatus = ButtonActionStatus.Rejected;
                        model.OldDCRRejectedDate = DateTime.Now;
                        model.RejectedComment = model.IfNotConsidered;
                        model.ReworkComments = string.Empty;
                        model.CurrentApprover.Comments = string.Empty;
                    }
                    else if (model.DDHODDISPOSAL == "Rework")
                    {
                        //ModelState.Remove("DesignEngineerIncharge");
                        ModelState.Remove("DesignEngineer");
                        ModelState.Remove("TargetDateOfImplementation");
                        model.ActionStatus = ButtonActionStatus.SendBack;
                        model.SendBackTo = "0";
                        objDict[Parameter.SENDTOLEVEL] = "0";
                        model.RejectedComment = model.ReworkComments;
                        model.IfNotConsidered = string.Empty;
                        model.CurrentApprover.Comments = string.Empty;
                    }
                }

                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        model.ApproversList.ForEach(p =>
                        {
                            /* Comment by Pooja
                            As per new CR to Remove Design Engineer Incharge from DCR and DCN */
                            //if (p.Role == DCRRoles.DESIGNENGINEERINCHARGE)
                            //{
                            //    p.Approver = model.DesignEngineerIncharge;            
                            //}
                            if (p.Role == DCRRoles.DESIGNENGINEER)
                            {
                                p.Approver = model.DesignEngineer;
                            }
                            if (model.MarketingApprovalRequired == false && p.Role.Contains("Marketing"))
                            {
                                p.Status = ApproverStatus.NOTREQUIRED;
                            }
                        });
                    }
                    model.Files = new List<FileDetails>();
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.FNLDCRAttachment));
                    model.FNLDCRAttachment = string.Join(",", FileListHelper.GetFileNames(model.FNLDCRAttachment));


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
        #endregion

        #region "Design Engineer"

        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveDesignEngineerSection(DesignEngineerSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    model.Files = new List<FileDetails>();
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.FNLDEAttachment));
                    model.FNLDEAttachment = string.Join(",", FileListHelper.GetFileNames(model.FNLDEAttachment));

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

        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveDesignEngineerInchargeSection(DesignEngineerInchargeSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {
                ///185490391
                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);

                if (model.ActionStatus == ButtonActionStatus.NextApproval)
                {
                    if (model.ConsiderRework == "Consider")
                    {
                        ModelState.Remove("DEIReworkComments");
                        model.ActionStatus = ButtonActionStatus.NextApproval;
                        model.DEIReworkComments = string.Empty;
                    }
                    else if (model.ConsiderRework == "Rework")
                    {
                        ModelState.Remove("DesignEngineer");
                        model.ActionStatus = ButtonActionStatus.SendBack;
                        model.SendBackTo = "0";
                        objDict[Parameter.SENDTOLEVEL] = "0";
                        //  model.CurrentApprover.Comments = string.Empty;
                    }
                    else if (model.ConsiderRework == "Not Consider")
                    {
                        ModelState.Remove("DesignEngineer");
                        ModelState.Remove("DEIReworkComments");
                        model.ActionStatus = ButtonActionStatus.Rejected;
                        model.DEIReworkComments = string.Empty;
                        model.OldDCRRejectedDate = DateTime.Now;
                    }
                }
                if (this.ValidateModelState(model))
                {

                    //185490391 - add btn for design engineer Incharge at level 3 and he will submit only, not delegate
                    if (model.ActionStatus == ButtonActionStatus.Delegate || model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        if (model.ActionStatus == ButtonActionStatus.NextApproval)
                        {
                            model.ApproversList.ForEach(p =>
                            {
                                if (p.Role == DCRRoles.DESIGNDEVELOPENGINEER)
                                {
                                    p.Approver = model.DesignEngineer;
                                }
                            });
                        }
                        //else
                        //{
                        //    model.DesignEngineer = this.CurrentUser.UserId;
                        //    model.DesignEngineerName = this.CurrentUser.FullName;
                        //}
                    }

                    //185490391 - as there is no attachment in design engineer incharge section
                    ///model.Files = new List<FileDetails>();
                    ///model.Files.AddRange(FileListHelper.GenerateFileBytes(model.FNLDEAttachment));
                    ///model.FNLDEAttachment = string.Join(",", FileListHelper.GetFileNames(model.FNLDEAttachment));


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
        #endregion

        #region "SCM"

        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveSCMInchargeSection(SCMInchargeSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {

                if (model.ActionStatus == ButtonActionStatus.NextApproval)
                {
                    ModelState.Remove("SCMUser");
                }
                if (!string.IsNullOrEmpty(model.BusinessUnit) && model.BusinessUnit.Contains("LUM"))
                {
                    model.CostReducedIncreasedByRs = 0;
                    model.TotalBenefitLossInRupeesLakhs = 0;
                    ModelState.Remove("EffectOnCostOfPartAndProduct");
                    ModelState.Remove("CostReducedIncreasedByRs");
                    ModelState.Remove("TotalBenefitLossInRupeesLakhs");
                }
                if (model.EffectOnCostOfPartAndProduct == "Cost Remains Same")
                {
                    model.CostReducedIncreasedByRs = 0;
                    ModelState.Remove("CostReducedIncreasedByRs");
                }
                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.Delegate || model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        if (model.ActionStatus == ButtonActionStatus.Delegate)
                        {
                            model.ApproversList.ForEach(p =>
                            {
                                if (p.Role == DCRRoles.SCM)
                                {
                                    p.Approver = model.SCMUser;
                                }
                            });
                        }
                        else
                        {
                            model.SCMUser = this.CurrentUser.UserId;
                            model.SCMUserName = this.CurrentUser.FullName;
                            //model.ApproversList.ForEach(p =>
                            //{
                            //    if (p.Role == DCRRoles.SCM)
                            //    {
                            //        p.Approver = this.CurrentUser.UserId;
                            //        p.Status = ApproverStatus.NOTREQUIRED;
                            //    }
                            //});
                        }
                    }

                    var list = this.GetTempData<List<VendorDCR>>(TempKeys.DCRVendor.ToString() + "_" + GetFormIdFromUrl());
                    model.VendorDCRList = list.ToList<ITrans>();
                    if (model.ActionStatus == ButtonActionStatus.NextApproval && (list == null || list.Count == 0 || !list.Any(m => m.ItemAction != ItemActionStatus.DELETED)))
                    {
                        status.IsSucceed = false;
                        status.Messages = new List<string>() { this.GetResourceValue("Text_VendorRequired", System.Web.Mvc.Html.ResourceNames.DCR) };
                        return this.Json(status);
                    }


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

        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveSCMSection(SCMSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {
                if (model.EffectOnCostOfPartAndProduct == "Cost Remains Same")
                {
                    model.CostReducedIncreasedByRs = 0;
                    ModelState.Remove("CostReducedIncreasedByRs");
                }
                if (!string.IsNullOrEmpty(model.BusinessUnit) && model.BusinessUnit.Contains("LUM"))
                {
                    model.CostReducedIncreasedByRs = 0;
                    model.TotalBenefitLossInRupeesLakhs = 0;
                    ModelState.Remove("EffectOnCostOfPartAndProduct");
                    ModelState.Remove("CostReducedIncreasedByRs");
                    ModelState.Remove("TotalBenefitLossInRupeesLakhs");
                }
                if (this.ValidateModelState(model))
                {
                    var list = this.GetTempData<List<VendorDCR>>(TempKeys.DCRVendor.ToString() + "_" + GetFormIdFromUrl());
                    model.VendorDCRList = list.ToList<ITrans>();
                    if (model.ActionStatus == ButtonActionStatus.NextApproval && (list == null || list.Count == 0 || !list.Any(m => m.ItemAction != ItemActionStatus.DELETED)))
                    {
                        status.IsSucceed = false;
                        status.Messages = new List<string>() { this.GetResourceValue("Text_VendorRequired", System.Web.Mvc.Html.ResourceNames.DCR) };
                        return this.Json(status);
                    }

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
        #endregion

        #region "QA"

        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveQASection(QASection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    model.Files = new List<FileDetails>();
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.FNLQATestReport));
                    model.FNLQATestReport = string.Join(",", FileListHelper.GetFileNames(model.FNLQATestReport));

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

        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveQAInchargeSection(QAInchargeSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (model.ActionStatus == ButtonActionStatus.NextApproval)
                {
                    ModelState.Remove("QAUser");
                    ModelState.Remove("OverallComment");
                }
                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.Delegate || model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        if (model.ActionStatus == ButtonActionStatus.Delegate)
                        {
                            model.ApproversList.ForEach(p =>
                            {
                                if (p.Role == DCRRoles.QA)
                                {
                                    p.Approver = model.QAUser;
                                }
                            });
                        }
                        else
                        {
                            model.QAUser = this.CurrentUser.UserId;
                            model.QAUserName = this.CurrentUser.FullName;
                            //model.ApproversList.ForEach(p =>
                            //{
                            //    if (p.Role == DCRRoles.QA)
                            //    {
                            //        p.Approver = this.CurrentUser.UserId;
                            //        p.Status = ApproverStatus.NOTREQUIRED;
                            //    }
                            //});
                        }
                    }
                    model.Files = new List<FileDetails>();
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.FNLQATestReport));
                    model.FNLQATestReport = string.Join(",", FileListHelper.GetFileNames(model.FNLQATestReport));

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
        #endregion

        #region "DAP Marketing"
        /// <summary>
        /// Save DAP Marketing Incharge Section
        /// </summary>
        /// <param name="model">Model Object</param>
        /// <returns>Action Status</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveDAPMarketingInchargeSection(DAPMarketingInchargeSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.Delegate || model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        if (model.ActionStatus == ButtonActionStatus.Delegate)
                        {
                            model.ApproversList.ForEach(p =>
                            {
                                if (p.Role == DCRRoles.DAPMARKETING)
                                {
                                    p.Approver = model.DAPMarketingUser;
                                }
                            });
                        }
                        else
                        {
                            model.DAPMarketingUser = this.CurrentUser.UserId;
                            model.DAPMarketingUserName = this.CurrentUser.FullName;
                            //model.ApproversList.ForEach(p =>
                            //{
                            //    if (p.Role == DCRRoles.DAPMARKETING)
                            //    {
                            //        p.Approver = this.CurrentUser.UserId;
                            //        p.Status = ApproverStatus.NOTREQUIRED;
                            //    }
                            //});
                        }
                    }

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

        /// <summary>
        /// Save DAP Marketing Section
        /// </summary>
        /// <param name="model">Model Object</param>
        /// <returns>Action Status</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveDAPMarketingSection(DAPMarketingSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
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
        #endregion

        #region "KAP Marketing"
        /// <summary>
        /// Save KAP Marketing Incharge Section
        /// </summary>
        /// <param name="model">Model Object</param>
        /// <returns>Action Status</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveKAPMarketingInchargeSection(KAPMarketingInchargeSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.Delegate || model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        if (model.ActionStatus == ButtonActionStatus.Delegate)
                        {
                            model.ApproversList.ForEach(p =>
                            {
                                if (p.Role == DCRRoles.KAPMARKETING)
                                {
                                    p.Approver = model.KAPMarketingUser;
                                }
                            });
                        }
                        else
                        {
                            model.KAPMarketingUser = this.CurrentUser.UserId;
                            model.KAPMarketingUserName = this.CurrentUser.FullName;
                            //model.ApproversList.ForEach(p =>
                            //{
                            //    if (p.Role == DCRRoles.KAPMARKETING)
                            //    {
                            //        p.Approver = this.CurrentUser.UserId;
                            //        p.Status = ApproverStatus.NOTREQUIRED;
                            //    }
                            //});
                        }
                    }

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

        /// <summary>
        /// Save KAP Marketing Section
        /// </summary>
        /// <param name="model">Model Object</param>
        /// <returns>Action Status</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveKAPMarketingSection(KAPMarketingSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
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
        #endregion

        #region "FANS Marketing"
        /// <summary>
        /// Save FANS Marketing Incharge Section
        /// </summary>
        /// <param name="model">Model Object</param>
        /// <returns>Action Status</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveFANSMarketingInchargeSection(FANSMarketingInchargeSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.Delegate || model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        if (model.ActionStatus == ButtonActionStatus.Delegate)
                        {
                            model.ApproversList.ForEach(p =>
                            {
                                if (p.Role == DCRRoles.FANSMARKETING)
                                {
                                    p.Approver = model.FANSMarketingUser;
                                }
                            });
                        }
                        else
                        {
                            model.FANSMarketingUser = this.CurrentUser.UserId;
                            model.FANSMarketingUserName = this.CurrentUser.FullName;
                            //model.ApproversList.ForEach(p =>
                            //{
                            //    if (p.Role == DCRRoles.FANSMARKETING)
                            //    {
                            //        p.Approver = this.CurrentUser.UserId;
                            //        p.Status = ApproverStatus.NOTREQUIRED;
                            //    }
                            //});
                        }
                    }
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

        /// <summary>
        /// Save FANS Marketing Section
        /// </summary>
        /// <param name="model">Model Object</param>
        /// <returns>Action Status</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveFANSMarketingSection(FANSMarketingSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
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
        #endregion

        #region "Lighting Marketing"
        /// <summary>
        /// Save Lighting Marketing Incharge Section
        /// </summary>
        /// <param name="model">Model Object</param>
        /// <returns>Action Status</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveLightingMarketingInchargeSection(LightingMarketingInchargeSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.Delegate || model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        if (model.ActionStatus == ButtonActionStatus.Delegate)
                        {
                            model.ApproversList.ForEach(p =>
                            {
                                if (p.Role == DCRRoles.LIGHTINGMARKETING)
                                {
                                    p.Approver = model.LightingMarketingUser;
                                }
                            });
                        }
                        else
                        {
                            model.LightingMarketingUser = this.CurrentUser.UserId;
                            model.LightingMarketingUserName = this.CurrentUser.FullName;
                            //model.ApproversList.ForEach(p =>
                            //{
                            //    if (p.Role == DCRRoles.LIGHTINGMARKETING)
                            //    {
                            //        p.Approver = this.CurrentUser.UserId;
                            //        p.Status = ApproverStatus.NOTREQUIRED;
                            //    }
                            //});
                        }
                    }
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

        /// <summary>
        /// Save Lighting Marketing Section
        /// </summary>
        /// <param name="model">Model Object</param>
        /// <returns>Action Status</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveLightingMarketingSection(LightingMarketingSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
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
        #endregion

        #region "MorphyRichards Marketing"
        /// <summary>
        /// Save Morphy Richards Marketing Incharge Section
        /// </summary>
        /// <param name="model">Model Object</param>
        /// <returns>Action Status</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveMorphyRichardsMarketingInchargeSection(MorphyRechardMarketingInchargeSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.Delegate || model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        if (model.ActionStatus == ButtonActionStatus.Delegate)
                        {
                            model.ApproversList.ForEach(p =>
                            {
                                if (p.Role == DCRRoles.MORPHYRICHARDSMARKETING)
                                {
                                    p.Approver = model.MRMarketingUser;
                                }
                            });
                        }
                        else
                        {
                            model.MRMarketingUser = this.CurrentUser.UserId;
                            model.MRMarketingUserName = this.CurrentUser.FullName;
                            //model.ApproversList.ForEach(p =>
                            //{
                            //    if (p.Role == DCRRoles.MORPHYRICHARDSMARKETING)
                            //    {
                            //        p.Approver = this.CurrentUser.UserId;
                            //        p.Status = ApproverStatus.NOTREQUIRED;
                            //    }
                            //});
                        }
                    }
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

        /// <summary>
        /// Save Morphy Richards Marketing Section
        /// </summary>
        /// <param name="model">Model Object</param>
        /// <returns>Action Status</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveMorphyRichardsMarketingSection(MorphyRechardMarketingSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
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
        #endregion

        #region "LUM Marketing"
        /// <summary>
        /// Save LUM Marketing Incharge Section
        /// </summary>
        /// <param name="model">Model Object</param>
        /// <returns>Action Status</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveLUMMarketingInchargeSection(LUMMarketingInchargeSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.Delegate || model.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        if (model.ActionStatus == ButtonActionStatus.Delegate)
                        {
                            model.ApproversList.ForEach(p =>
                            {
                                if (p.Role == DCRRoles.LUMMARKETING)
                                {
                                    p.Approver = model.LUMMarketingUser;
                                }
                            });
                        }
                        else
                        {
                            model.LUMMarketingUser = this.CurrentUser.UserId;
                            model.LUMMarketingUserName = this.CurrentUser.FullName;
                            //model.ApproversList.ForEach(p =>
                            //{
                            //    if (p.Role == DCRRoles.LUMMARKETING)
                            //    {
                            //        p.Approver = this.CurrentUser.UserId;
                            //        p.Status = ApproverStatus.NOTREQUIRED;
                            //    }
                            //});
                        }
                    }
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

        /// <summary>
        /// Save LUM Marketing Section
        /// </summary>
        /// <param name="model">Model Object</param>
        /// <returns>Action Status</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveLUMMarketingSection(LUMMarketingSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
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
        #endregion

        #region "CC Head"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveCCHeadSection(CCInchargeSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
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
        #endregion

        #region "Save Costing Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveCostingSection(CostingSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    if (model.VendorDCRPostList != null && model.VendorDCRPostList.Count != 0)
                    {
                        model.VendorDCRPostList.ToList().ForEach(p => p.ItemAction = ItemActionStatus.UPDATED);
                        model.VendorDCRList = model.VendorDCRPostList.ToList<ITrans>();
                    }

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
        #endregion

        #region "Final DCR Process Incharge"
        /// <summary>
        /// Saves the specified model. 
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveFinalDCRProcessInchargeSection(FinalDCRProcessInchargeSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null)
            {
                if (model.ActionStatus != ButtonActionStatus.SaveAndNoStatusUpdate)
                {

                    if (model.IsApproved == "Approved")
                    {
                        model.ActionStatus = ButtonActionStatus.Complete;
                        
                    }
                    else
                    {
                        ModelState.Remove("FinalDesignEngineer");
                        ModelState.Remove("FinalDesignEngineerName");
                        model.ActionStatus = ButtonActionStatus.Rejected;
                        model.OldDCRRejectedDate = DateTime.Now;
                        model.RejectedComment = model.CurrentApprover.Comments;
                    }
                }
                if (this.ValidateModelState(model))
                {

                    Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);
                    if (model.ActionStatus == ButtonActionStatus.Complete)
                    {
                        objDict.Add("FinalDesignEngineer", model.FinalDesignEngineer);
                    }
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
        #endregion
        #endregion

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
        public ActionResult DeleteVendor(int index)
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
            this.SetTempData<List<VendorDCR>>(TempKeys.DCRVendor, list.OrderBy(x => x.Index).ToList());
            status.Messages.Add(this.GetResourceValue("Text_VendorDeleted", System.Web.Mvc.Html.ResourceNames.DCR));
            return this.Json(status, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add Vendor
        /// </summary>
        /// <returns>Add Vendor</returns>
        [HttpGet]
        public ActionResult AddVendor()
        {
            return this.PartialView("_AddVendor");
        }
        #endregion

        #region "DCR WIP Report"
        [SharePointContextFilter]
        public ActionResult WIPReport()
        {
            Dictionary<string, string> objDict = new Dictionary<string, string>();
            objDict.Add("FormName", FormNameConstants.DCRFORM);
            objDict.Add(Parameter.USEREID, this.CurrentUser.UserId);
            objDict.Add(Parameter.USEREMAIL, this.CurrentUser.UserEmail);
            List<ISection> reportData = DCRBusinessLayer.Instance.RetrieveWIPDCR(objDict);
            return this.View("Reports/WIPReport", reportData);
        }
        #endregion

    }
}