namespace BEL.DCRDCNWorkflow.BusinessLayer
{
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.Models.DCR;
    using BEL.DCRDCNWorkflow.Models.Common;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using BEL.DataAccessLayer;
    using BEL.DCRDCNWorkflow.Models.Master;
    using Newtonsoft.Json;
    using Microsoft.SharePoint.Client.UserProfiles;
    using BEL.DCRDCNWorkflow.Models;

    public class DCRBusinessLayer
    {
        private static readonly Lazy<DCRBusinessLayer> lazy =
           new Lazy<DCRBusinessLayer>(() => new DCRBusinessLayer());

        public static DCRBusinessLayer Instance { get { return lazy.Value; } }

        /// <summary>
        /// The padlock
        /// </summary>
        private static readonly object Padlock = new object();

        private DCRBusinessLayer()
        {
            string siteURL = BELDataAccessLayer.Instance.GetSiteURL(SiteURLs.DCRDCNSITEURL);
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
        /// The context
        /// </summary>
        private ClientContext context = null;

        /// <summary>
        /// The web
        /// </summary>
        private Web web = null;




        #region "GET DATA"
        /// <summary>
        /// Gets the DCR details.
        /// </summary>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>byte array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public DCRContract GetDCRDetails(IDictionary<string, string> objDict)
        {
            DCRContract contract = new DCRContract();

            // BELDataAccessLayer helper = new BELDataAccessLayer(); // KKSoni Change Helper class name to BELDataAccessLayer
            if (objDict != null && objDict.ContainsKey(Parameter.FROMNAME) && objDict.ContainsKey(Parameter.ITEMID) && objDict.ContainsKey(Parameter.USEREID))
            {
                string formName = objDict[Parameter.FROMNAME];
                int itemId = Convert.ToInt32(objDict[Parameter.ITEMID]);
                string userID = objDict[Parameter.USEREID];
                //contract.UserDetails = BELDataAccessLayer.Instance.GetUserInformation(userEmail);
                IForm dcrForm = new DCRForm(true);
                dcrForm = BELDataAccessLayer.Instance.GetFormData(this.context, this.web, ApplicationNameConstants.DCRAPP, formName, itemId, userID, dcrForm);
                if (dcrForm != null && dcrForm.SectionsList != null && dcrForm.SectionsList.Count > 0)
                {
                    var sectionDetails = dcrForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCRSectionName.DCRDETAILSECTION)) as DCRDetailSection;
                    var approvalMatrix = dcrForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(SectionNameConstant.APPLICATIONSTATUS)) as ApplicationStatusSection;
                    if (sectionDetails != null)
                    {
                        ////Get Logged in user Department.
                        if (itemId == 0)
                        {
                            List<IMasterItem> approvers = sectionDetails.MasterData.FirstOrDefault(p => p.NameOfMaster == DCRDCNListNames.APPROVERMASTER).Items;
                            List<ApproverMasterListItem> approverList = approvers.ConvertAll(p => (ApproverMasterListItem)p);

                            if (sectionDetails.MarketingApprovalRequired == null || sectionDetails.MarketingApprovalRequired == false)
                            {
                                sectionDetails.MarketingApprovalRequired = true;
                            }

                            ////Get Department User
                            sectionDetails.ApproversList.ForEach(p =>
                            {
                                if (p.Role == DCRRoles.DCRINCHARGENAVIGATOR)
                                {
                                    p.Approver = approverList.FirstOrDefault(q => q.Role == DCRRoles.DCRINCHARGENAVIGATOR).UserID;
                                    p.ApproverName = approverList.FirstOrDefault(q => q.Role == DCRRoles.DCRINCHARGENAVIGATOR).UserName;
                                }
                                else if (p.Role == DCRRoles.DCRPROCESSINCHARGE || p.Role == DCRRoles.FINALDCRPROCESSINCHARGE)
                                {
                                    p.Approver = approverList.FirstOrDefault(q => q.Role == DCRRoles.DCRPROCESSINCHARGE).UserID;
                                    p.ApproverName = approverList.FirstOrDefault(q => q.Role == DCRRoles.DCRPROCESSINCHARGE).UserName;
                                }
                                else if (p.Role == DCRRoles.QAINCHARGE)
                                {
                                    p.Approver = approverList.FirstOrDefault(q => q.Role == DCRRoles.QAINCHARGE).UserID;
                                    p.ApproverName = approverList.FirstOrDefault(q => q.Role == DCRRoles.QAINCHARGE).UserName;
                                }
                                else if (p.Role == DCRRoles.SCMINCHARGE)
                                {
                                    p.Approver = approverList.FirstOrDefault(q => q.Role == DCRRoles.SCMINCHARGE).UserID;
                                    p.ApproverName = approverList.FirstOrDefault(q => q.Role == DCRRoles.SCMINCHARGE).UserName;
                                }
                                else if (p.Role == DCRRoles.CCINCHARGE)
                                {
                                    p.Approver = approverList.FirstOrDefault(q => q.Role == DCRRoles.CCINCHARGE).UserID;
                                    p.ApproverName = approverList.FirstOrDefault(q => q.Role == DCRRoles.CCINCHARGE).UserName;
                                }

                                else if (p.Role == DCRRoles.DCRINCHARGENAVIGATOR)
                                {
                                    p.Approver = approverList.FirstOrDefault(q => q.Role == DCRRoles.DCRINCHARGENAVIGATOR).UserID;
                                    p.ApproverName = approverList.FirstOrDefault(q => q.Role == DCRRoles.DCRINCHARGENAVIGATOR).UserName;
                                }
                                else if (p.Role == DCRRoles.DESIGNENGINEERINCHARGE)
                                {
                                    p.Approver = approverList.FirstOrDefault(q => q.Role == DCRRoles.DESIGNENGINEERINCHARGE).UserID;
                                    p.ApproverName = approverList.FirstOrDefault(q => q.Role == DCRRoles.DESIGNENGINEERINCHARGE).UserName;
                                }
                            });
                        }
                        else
                        {
                            sectionDetails.ApproversList.Remove(sectionDetails.ApproversList.FirstOrDefault(p => p.Role == UserRoles.VIEWER));
                        }

                        sectionDetails.FNLDesignChangeAttachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? (sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.FNLDesignChangeAttachment) && sectionDetails.FNLDesignChangeAttachment.Split(',').Contains(x.FileName)).ToList().Any() ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.FNLDesignChangeAttachment) && sectionDetails.FNLDesignChangeAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty) : string.Empty;
                        sectionDetails.FNLExpectedResultsAttachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? (sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.FNLExpectedResultsAttachment) && sectionDetails.FNLExpectedResultsAttachment.Split(',').Contains(x.FileName)).ToList().Any() ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.FNLExpectedResultsAttachment) && sectionDetails.FNLExpectedResultsAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty) : string.Empty;
                        sectionDetails.FNLDCRAttachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? (sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.FNLDCRAttachment) && sectionDetails.FNLDCRAttachment.Split(',').Contains(x.FileName)).ToList().Any() ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.FNLDCRAttachment) && sectionDetails.FNLDCRAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty) : string.Empty; // added by kksoni 
                        if (itemId != 0)
                        {
                            if ((sectionDetails.Status == FormStatus.COMPLETED || sectionDetails.Status == FormStatus.REJECTED) && !approvalMatrix.ApplicationStatusList.Any(p => (p.Role == DCRRoles.DCRPROCESSINCHARGE || p.Role == DCRRoles.SCMINCHARGE || p.Role == DCRRoles.SCM) && !String.IsNullOrEmpty(p.Approver) && p.Approver.Split(',').Contains(userID.Trim())))
                            {
                                if (dcrForm.Buttons.FirstOrDefault(p => p.Name == "Print") != null)
                                {
                                    dcrForm.Buttons.FirstOrDefault(p => p.Name == "Print").IsVisible = false;
                                }
                            }
                        }
                    }

                    var dcrProcessInchargeSection = dcrForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCRSectionName.DCRPROCESSINCHARGESECTION)) as DCRProcessInchargeSection;
                    if (dcrProcessInchargeSection != null)
                    {
                        dcrProcessInchargeSection.FNLDCRAttachment = dcrProcessInchargeSection.Files != null && dcrProcessInchargeSection.Files.Count > 0 ? (dcrProcessInchargeSection.Files.Where(x => !string.IsNullOrEmpty(dcrProcessInchargeSection.FNLDCRAttachment) && dcrProcessInchargeSection.FNLDCRAttachment.Split(',').Contains(x.FileName)).ToList().Any() ? JsonConvert.SerializeObject(dcrProcessInchargeSection.Files.Where(x => !string.IsNullOrEmpty(dcrProcessInchargeSection.FNLDCRAttachment) && dcrProcessInchargeSection.FNLDCRAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty) : string.Empty;

                    }

                    var deInchargeSection = dcrForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCRSectionName.DESIGNENGINEERINCHARGESECTION)) as DesignEngineerInchargeSection;
                    if (deInchargeSection != null)
                    {
                        //remove design engineer section from design engineer incharge section
                        //// deInchargeSection.FNLDEAttachment = deInchargeSection.Files != null && deInchargeSection.Files.Count > 0 ? JsonConvert.SerializeObject(deInchargeSection.Files.Where(x => !string.IsNullOrEmpty(deInchargeSection.FNLDEAttachment) && deInchargeSection.FNLDEAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                    }

                    var deSection = dcrForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCRSectionName.DESIGNENGINEERSECTION)) as DesignEngineerSection;
                    if (deSection == null) //185479517, Modify Section Name 
                    {
                        deSection = dcrForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCRSectionName.DESIGNDEVELOPENGINEERSECTION)) as DesignEngineerSection;
                    }
                    if (deSection != null)
                    {
                        deSection.FNLDEAttachment = deSection.Files != null && deSection.Files.Count > 0 ? (deSection.Files.Where(x => !string.IsNullOrEmpty(deSection.FNLDEAttachment) && deSection.FNLDEAttachment.Split(',').Contains(x.FileName)).ToList().Any() ? JsonConvert.SerializeObject(deSection.Files.Where(x => !string.IsNullOrEmpty(deSection.FNLDEAttachment) && deSection.FNLDEAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty) : string.Empty;

                        //185479719- Remove unwanted Delegate button at DESIGNENGINEERSECTION Start
                        if (deSection.IsActive)
                        {
                            if (dcrForm.Buttons.FirstOrDefault(p => p.Name == "Delegate") != null)
                            {
                                dcrForm.Buttons.FirstOrDefault(p => p.Name == "Delegate").IsVisible = false;
                            }
                        }
                        //185479719- Remove unwanted Delegate button at DESIGNENGINEERSECTION End
                    }

                    var qAInchargeSection = dcrForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCRSectionName.QAINCHARGESECTION)) as QAInchargeSection;
                    if (qAInchargeSection != null)
                    {
                        qAInchargeSection.FNLQATestReport = qAInchargeSection.Files != null && qAInchargeSection.Files.Count > 0 ? (qAInchargeSection.Files.Where(x => !string.IsNullOrEmpty(qAInchargeSection.FNLQATestReport) && qAInchargeSection.FNLQATestReport.Split(',').Contains(x.FileName)).ToList().Any() ? JsonConvert.SerializeObject(qAInchargeSection.Files.Where(x => !string.IsNullOrEmpty(qAInchargeSection.FNLQATestReport) && qAInchargeSection.FNLQATestReport.Split(',').Contains(x.FileName)).ToList()) : string.Empty) : string.Empty;
                    }
                    var qaSection = dcrForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCRSectionName.QASECTION)) as QASection;
                    if (qaSection != null)
                    {
                        qaSection.FNLQATestReport = qaSection.Files != null && qaSection.Files.Count > 0 ? (qaSection.Files.Where(x => !string.IsNullOrEmpty(qaSection.FNLQATestReport) && qaSection.FNLQATestReport.Split(',').Contains(x.FileName)).ToList().Any() ? JsonConvert.SerializeObject(qaSection.Files.Where(x => !string.IsNullOrEmpty(qaSection.FNLQATestReport) && qaSection.FNLQATestReport.Split(',').Contains(x.FileName)).ToList()) : string.Empty) : string.Empty;
                    }
                    contract.Forms.Add(dcrForm);
                }
            }
            return contract;
        }
        #endregion

        #region "GET DATA FOR ADMIN"
        /// <summary>
        /// Gets the DCR details.
        /// </summary>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>byte array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public DCRAdminContract GetAdminDCRDetails(IDictionary<string, string> objDict)
        {
            DCRAdminContract contract = new DCRAdminContract();

            // BELDataAccessLayer helper = new BELDataAccessLayer(); // KKSoni Change Helper class name to BELDataAccessLayer
            if (objDict != null && objDict.ContainsKey(Parameter.FROMNAME) && objDict.ContainsKey(Parameter.ITEMID) && objDict.ContainsKey(Parameter.USEREID))
            {
                string formName = objDict[Parameter.FROMNAME];
                int itemId = Convert.ToInt32(objDict[Parameter.ITEMID]);
                string userID = objDict[Parameter.USEREID];
                //contract.UserDetails = BELDataAccessLayer.Instance.GetUserInformation(userEmail);
                IForm dcrAdminForm = new DCRAdminForm(true);
                dcrAdminForm = BELDataAccessLayer.Instance.GetFormData(this.context, this.web, ApplicationNameConstants.DCRAPP, formName, itemId, userID, dcrAdminForm);
                if (dcrAdminForm != null && dcrAdminForm.SectionsList != null && dcrAdminForm.SectionsList.Count > 0)
                {
                    var sectionDetails = dcrAdminForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCRSectionName.DCRDETAILADMINSECTION)) as DCRAdminDetailSection;
                    if (sectionDetails != null)
                    {


                        ////Get Logged in user Department.
                        if (itemId == 0)
                        {
                            List<IMasterItem> approvers = sectionDetails.MasterData.FirstOrDefault(p => p.NameOfMaster == DCRDCNListNames.APPROVERMASTER).Items;
                            List<ApproverMasterListItem> approverList = approvers.ConvertAll(p => (ApproverMasterListItem)p);

                            ////Get Department User
                            sectionDetails.ApproversList.ForEach(p =>
                            {
                                if (p.Role == DCRRoles.DCRINCHARGENAVIGATOR)
                                {
                                    p.Approver = approverList.FirstOrDefault(q => q.Role == DCRRoles.DCRINCHARGENAVIGATOR).UserID;
                                    p.ApproverName = approverList.FirstOrDefault(q => q.Role == DCRRoles.DCRINCHARGENAVIGATOR).UserName;
                                }
                                else if (p.Role == DCRRoles.DCRPROCESSINCHARGE || p.Role == DCRRoles.FINALDCRPROCESSINCHARGE)
                                {
                                    p.Approver = approverList.FirstOrDefault(q => q.Role == DCRRoles.DCRPROCESSINCHARGE).UserID;
                                    p.ApproverName = approverList.FirstOrDefault(q => q.Role == DCRRoles.DCRPROCESSINCHARGE).UserName;
                                }
                                else if (p.Role == DCRRoles.QAINCHARGE)
                                {
                                    p.Approver = approverList.FirstOrDefault(q => q.Role == DCRRoles.QAINCHARGE).UserID;
                                    p.ApproverName = approverList.FirstOrDefault(q => q.Role == DCRRoles.QAINCHARGE).UserName;
                                }
                                else if (p.Role == DCRRoles.SCMINCHARGE)
                                {
                                    p.Approver = approverList.FirstOrDefault(q => q.Role == DCRRoles.SCMINCHARGE).UserID;
                                    p.ApproverName = approverList.FirstOrDefault(q => q.Role == DCRRoles.SCMINCHARGE).UserName;
                                }
                                else if (p.Role == DCRRoles.CCINCHARGE)
                                {
                                    p.Approver = approverList.FirstOrDefault(q => q.Role == DCRRoles.CCINCHARGE).UserID;
                                    p.ApproverName = approverList.FirstOrDefault(q => q.Role == DCRRoles.CCINCHARGE).UserName;
                                }
                                else if (p.Role == DCRRoles.DCRINCHARGENAVIGATOR)
                                {
                                    p.Approver = approverList.FirstOrDefault(q => q.Role == DCRRoles.DCRINCHARGENAVIGATOR).UserID;
                                    p.ApproverName = approverList.FirstOrDefault(q => q.Role == DCRRoles.DCRINCHARGENAVIGATOR).UserName;
                                }
                            });
                        }

                        sectionDetails.FNLDesignChangeAttachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.FNLDesignChangeAttachment) && sectionDetails.FNLDesignChangeAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        sectionDetails.FNLExpectedResultsAttachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.FNLExpectedResultsAttachment) && sectionDetails.FNLExpectedResultsAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        if (itemId != 0)
                        {
                            if (sectionDetails.Status == FormStatus.COMPLETED && sectionDetails.ApproversList.Any(p => p.Role == DCNRoles.DCRPROCESSINCHARGE && p.Approver == userID))
                            {
                                if (dcrAdminForm.Buttons.FirstOrDefault(p => p.Name == "Print") != null)
                                {
                                    dcrAdminForm.Buttons.FirstOrDefault(p => p.Name == "Print").IsVisible = false;
                                }
                            }
                        }
                        sectionDetails.FNLDCRAttachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.FNLDCRAttachment) && sectionDetails.FNLDCRAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        sectionDetails.FNLDEAttachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.FNLDEAttachment) && sectionDetails.FNLDEAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        sectionDetails.FNLQATestReport = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.FNLQATestReport) && sectionDetails.FNLQATestReport.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                    }

                    contract.Forms.Add(dcrAdminForm);
                }
            }
            return contract;
        }

        public bool IsAdminUser(string userid)
        {
            return BELDataAccessLayer.Instance.IsGroupMember(context, web, userid, UserRoles.ADMIN);
        }
        #endregion

        #region "SAVE DATA"
        /// <summary>
        /// Saves the by section.
        /// </summary>
        /// <param name="sections">The sections.</param>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>return status</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public ActionStatus SaveBySection(ISection sectionDetails, Dictionary<string, string> objDict)
        {
            lock (Padlock)
            {
                ActionStatus status = new ActionStatus();
                DCRNoCount currentDCRNo = null;
                //BELDataAccessLayer helper = new BELDataAccessLayer();
                if (sectionDetails != null && objDict != null)
                {
                    objDict[Parameter.ACTIVITYLOG] = DCRDCNListNames.DCRACTIVITYLOG;
                    objDict[Parameter.APPLICATIONNAME] = ApplicationNameConstants.DCRAPP;
                    objDict[Parameter.FROMNAME] = FormNameConstants.DCRFORM;
                    DCRDetailSection section = null;
                    DesignEngineerSection designEngSection = null;
                    if (sectionDetails.SectionName == DCRSectionName.DCRDETAILSECTION)
                    {
                        section = sectionDetails as DCRDetailSection;
                        if (string.IsNullOrEmpty(section.DCRNo) && sectionDetails.ActionStatus == ButtonActionStatus.SaveAsDraft)
                        {
                            section.Title = section.DCRNo = "View";
                        }
                        else if (sectionDetails.ActionStatus == ButtonActionStatus.NextApproval && (string.IsNullOrEmpty(section.DCRNo) || section.DCRNo == "View"))
                        {
                            ////section = sectionDetails as DCRDetailSection;
                            currentDCRNo = GetDCRNo(section.BusinessUnit);
                            section.RequestDate = DateTime.Now;
                            if (currentDCRNo != null)
                            {
                                currentDCRNo.CurrentValue = currentDCRNo.CurrentValue + 1;
                                Logger.Info("DCR Current Value + 1 = " + currentDCRNo.CurrentValue);
                                section.DCRNo = string.Format("DCR-{0}-{1}-{2}", currentDCRNo.BusinessUnit, DateTime.Today.Year, string.Format("{0:0000}", currentDCRNo.CurrentValue));
                                section.Title = section.DCRNo;
                                Logger.Info("DCR No is " + section.DCRNo);
                                status.ExtraData = section.DCRNo;
                            }
                        }
                    }
                    if (sectionDetails.SectionName == DCRSectionName.DESIGNENGINEERSECTION && sectionDetails.ActionStatus == ButtonActionStatus.NextApproval)
                    {
                        designEngSection = sectionDetails as DesignEngineerSection;
                        if (designEngSection != null)
                        {
                            designEngSection.FinalDesignEngineer = objDict[Parameter.USEREID];
                        }
                    }
                    List<ListItemDetail> objSaveDetails = BELDataAccessLayer.Instance.SaveData(this.context, this.web, sectionDetails, objDict);
                    ListItemDetail itemDetails = objSaveDetails.Where(p => p.ListName.Equals(DCRDCNListNames.DCRLIST)).FirstOrDefault<ListItemDetail>();
                    if (sectionDetails.SectionName == DCRSectionName.DCRDETAILSECTION)
                    {
                        if (section != null && !string.IsNullOrEmpty(section.OldDCRNo))
                        {
                            Dictionary<string, dynamic> values = new Dictionary<string, dynamic>();
                            values.Add("IsDCRRetrieved", true);
                            BELDataAccessLayer.Instance.SaveFormFields(this.context, this.web, DCRDCNListNames.DCRLIST, section.OldDCRId, values);
                        }

                        if (itemDetails.ItemId > 0 && currentDCRNo != null)
                        {
                            //// AsyncHelper.Call(obj =>
                            ////  {
                            UpdateDCRNoCount(currentDCRNo);
                            Logger.Info("Update DCR No " + section.DCRNo);
                            ////  });
                        }
                    }

                    if (sectionDetails.SectionName == DCRSectionName.DESIGNENGINEERSECTION && sectionDetails.ActionStatus == ButtonActionStatus.NextApproval)
                    {

                        if (designEngSection != null && designEngSection.DateForCompletionOfTesting != null && designEngSection.DateForReceiptOfSamples != null)
                        {
                            EmailHelper eHelper = new EmailHelper();

                            Dictionary<string, string> mailCustomValues = null; List<FileDetails> emailAttachments = null;
                            Dictionary<string, string> email = new Dictionary<string, string>();
                            List<ListItemDetail> itemdetail = new List<ListItemDetail>();
                            string applicationName = ApplicationNameConstants.DCRAPP; string formName = FormNameConstants.DCRFORM;

                            string from = string.Empty, to = string.Empty, cc = string.Empty, role = string.Empty, tmplName = string.Empty;
                            from = objDict[Parameter.USEREID];
                            from = BELDataAccessLayer.GetEmailUsingUserID(context, web, from);
                            to = designEngSection.ProposedBy;
                            to = BELDataAccessLayer.GetEmailUsingUserID(context, web, to);

                            cc = from;
                            role = DCRRoles.DESIGNENGINEER;
                            tmplName = EmailTemplateName.DESIGNENGGTOCREATOR;
                            itemdetail.Add(new ListItemDetail() { ItemId = itemDetails.ItemId, IsMainList = true, ListName = DCRDCNListNames.DCRLIST });
                            if (mailCustomValues == null)
                            {
                                mailCustomValues = new Dictionary<string, string>();
                            }
                            mailCustomValues[Parameter.CURRENTAPPROVERNAME] = BELDataAccessLayer.GetNameUsingUserID(context, web, objDict[Parameter.USEREID]);

                            email = eHelper.GetEmailBody(context, web, tmplName, itemdetail, mailCustomValues, role, applicationName, formName);
                            eHelper.SendMail(applicationName, formName, tmplName, email["Subject"], email["Body"], from, to, cc, false, emailAttachments);

                        }
                    }

                    if (itemDetails.ItemId > 0)
                    {
                        status.IsSucceed = true;
                        status.ItemID = itemDetails.ItemId;
                        switch (sectionDetails.ActionStatus)
                        {
                            case ButtonActionStatus.SaveAsDraft:
                                status.Messages.Add("Text_SaveDraftSuccess");
                                break;
                            case ButtonActionStatus.SaveAndNoStatusUpdate:
                                status.Messages.Add("Text_SaveSuccess");
                                break;
                            case ButtonActionStatus.NextApproval:
                                status.Messages.Add(ApplicationConstants.SUCCESSMESSAGE);
                                break;
                            case ButtonActionStatus.Delegate:
                                status.Messages.Add("Text_DelegatedSuccess");
                                break;
                            case ButtonActionStatus.Complete:
                                status.Messages.Add("Text_CompleteSuccess");
                                break;
                            case ButtonActionStatus.Rejected:
                                status.Messages.Add("Text_RejectedSuccess");
                                break;
                            default:
                                status.Messages.Add(ApplicationConstants.SUCCESSMESSAGE);
                                break;
                        }
                    }
                    else
                    {
                        status.IsSucceed = false;
                        status.Messages.Add(ApplicationConstants.ERRORMESSAGE);
                    }

                }
                return status;
            }
        }

        /// <summary>
        /// Get DCR No Logic
        /// </summary>
        /// <param name="context">Site context</param>
        /// <param name="web">Web Object</param>
        /// <returns>DCR No</returns>
        public string GetDCRNo()
        {
            try
            {
                int dcrno = 0;
                List spList = this.web.Lists.GetByTitle(DCRDCNListNames.DCRLIST);
                CamlQuery query = new CamlQuery();
                string startDate = (new DateTime(DateTime.Now.Year, 1, 1)).ToString("yyyy-MM-ddTHH:mm:ssZ");
                string endDate = (new DateTime(DateTime.Now.Year, 12, 31)).ToString("yyyy-MM-ddTHH:mm:ssZ");
                query.ViewXml = @"<View><Query>
                                      <Where>
                                        <And>
                                          <Geq>
                                            <FieldRef Name='Created' />
                                              <Value IncludeTimeValue='FALSE' Type='DateTime'>" + startDate + @"</Value>
                                          </Geq>
                                          <Leq>
                                            <FieldRef Name='Created' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>" + endDate + @"</Value>
                                          </Leq>
                                        </And>
                                      </Where>
                                    </Query>
                                         <ViewFields><FieldRef Name='ID' /></ViewFields>   </View>";
                //query.ViewXml = @"<View><ViewFields><FieldRef Name='ID' /></ViewFields></View>";
                ListItemCollection items = spList.GetItems(query);
                this.context.Load(items);
                this.context.ExecuteQuery();
                if (items != null && items.Count != 0)
                {
                    dcrno = items.Count;
                }
                return dcrno.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return string.Empty;
            }


        }

        /// <summary>
        /// Get DCR No Logic
        /// </summary>
        /// <param name="businessunit">Business Unit</param>
        /// <returns>DCR No Count</returns>
        public DCRNoCount GetDCRNo(string businessunit)
        {
            try
            {
                List<DCRNoCount> lstdcrCount = new List<DCRNoCount>();
                ////List<DCRNoCount> lstdcrCount = GlobalCachingProvider.Instance.GetItem(DCRDCNListNames.DCRNOCOUNT, false) as List<DCRNoCount>;

                ////if (lstdcrCount == null || lstdcrCount.FirstOrDefault(p => businessunit.Contains(p.BusinessUnit) && p.Year == DateTime.Today.Year) == null)
                ////{
                List spList = this.web.Lists.GetByTitle(DCRDCNListNames.DCRNOCOUNT);
                CamlQuery query = new CamlQuery();
                query.ViewXml = @"<View><ViewFields><FieldRef Name='Title' /><FieldRef Name='Year' /><FieldRef Name='CurrentValue' /></ViewFields>   </View>";
                ListItemCollection items = spList.GetItems(query);
                this.context.Load(items);
                this.context.ExecuteQuery();
                if (items != null && items.Count != 0)
                {
                    foreach (ListItem item in items)
                    {
                        DCRNoCount obj = new DCRNoCount();
                        obj.ID = item.Id;
                        obj.BusinessUnit = Convert.ToString(item["Title"]);
                        obj.Year = Convert.ToInt32(item["Year"]);
                        obj.CurrentValue = Convert.ToInt32(item["CurrentValue"]);
                        if (obj.Year != DateTime.Today.Year)
                        {
                            obj.Year = DateTime.Today.Year;
                            obj.CurrentValue = 0;
                        }

                        lstdcrCount.Add(obj);
                    }
                }

                if (lstdcrCount != null)
                {
                    return lstdcrCount.FirstOrDefault(p => businessunit.Contains(p.BusinessUnit) && p.Year == DateTime.Today.Year);
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Update DCR No Count
        /// </summary>
        /// <param name="currentValue">Current Value</param>
        public void UpdateDCRNoCount(DCRNoCount currentValue)
        {
            if (currentValue != null && currentValue.ID != 0)
            {
                try
                {

                    Logger.Info("Aync update DCR Current value currentValue : " + currentValue.CurrentValue + " BusinessUnit : " + currentValue.BusinessUnit);
                    List spList = this.web.Lists.GetByTitle(DCRDCNListNames.DCRNOCOUNT);
                    ListItem item = spList.GetItemById(currentValue.ID);

                    item.RefreshLoad(); // Pooja Atkotiya - Added for Version Conflict!
                    context.Load(item);
                    context.ExecuteQuery();

                    item["CurrentValue"] = currentValue.CurrentValue;
                    item["Year"] = currentValue.Year;
                    item.Update();
                    //Version conflict error
                    item.RefreshLoad(); // Pooja Atkotiya - Added for Version Conflict!
                    context.Load(item);
                    context.ExecuteQuery();

                }
                catch (Exception ex)
                {
                    Logger.Error("Error while Update DCR no Current Value");
                    Logger.Error(ex);
                }
            }
        }
        #endregion

        public UserDetails getUSerDetail(int id)
        {
            UserDetails detail = new UserDetails();
            var peopleManager = new PeopleManager(this.context);
            User usr = this.web.GetUserById(id);
            PersonProperties personProperties = peopleManager.GetMyProperties();
            this.context.Load(usr, p => p.Id, p => p.LoginName, p => p.Email, p => p.Title);
            this.context.Load(personProperties);
            this.context.ExecuteQuery();
            detail.UserId = usr.Id.ToString();
            detail.LoginName = usr.LoginName;
            detail.Department = personProperties.UserProfileProperties["Department"];
           // detail.FullName = personProperties.DisplayName;
            detail.FullName = usr.Title;
            detail.UserEmail = usr.Email; //personProperties.Email;
            return detail;
        }

        public string GetVendorMasterData(string title)
        {
            MasterDataHelper masterHelper = new MasterDataHelper();
            IMaster vendormaster = masterHelper.GetMasterData(context, web, new List<IMaster>() { new VendorMaster() }).FirstOrDefault();
            string vendorjson = JSONHelper.ToJSON<IMaster>(vendormaster);
            var filterdVendor = JSONHelper.ToObject<IMaster>(vendorjson);
            if (filterdVendor != null)
            {
                filterdVendor.Items = filterdVendor.Items.Where(x => (x.Title ?? string.Empty).ToLower().Trim().Contains((title ?? string.Empty).ToLower().Trim()) || (x.Value ?? string.Empty).ToLower().Trim().Contains((title ?? string.Empty).ToLower().Trim())).ToList();
                return JSONHelper.ToJSON<IMaster>(filterdVendor);
            }
            else
            {
                return null;
            }
        }

        #region "Report"

        /// <summary>
        /// Gets the DCN details.
        /// </summary>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>byte array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "test")]
        public List<ISection> RetrieveWIPDCR(IDictionary<string, string> objDict)
        {
            Dictionary<string, string> otherParamDictionary = null;
            BELDataAccessLayer helper = new BELDataAccessLayer();
            List<ISection> sections = new List<ISection>();
            if (objDict != null && objDict.ContainsKey("UserEmail"))
            {
                string userEmail = objDict["UserEmail"];
                string formName = string.Empty;
                //IForm form = new WIPDCRForm(true);
                WIPDCRDetailsSection section = new WIPDCRDetailsSection(true);
                sections = helper.GetItemsForSection(this.context, this.web, ApplicationNameConstants.DCRAPP, formName, (ISection)section, userEmail, otherParamDictionary);

            }
            return sections;

        }
        #endregion
    }
}