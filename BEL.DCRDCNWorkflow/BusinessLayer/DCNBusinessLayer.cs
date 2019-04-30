namespace BEL.DCRDCNWorkflow.BusinessLayer
{
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.Models.DCN;
    using BEL.DCRDCNWorkflow.Models.Common;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BEL.DataAccessLayer;
    using BEL.DCRDCNWorkflow.Models;

    public class DCNBusinessLayer
    {
         private static readonly Lazy<DCNBusinessLayer> lazy =
         new Lazy<DCNBusinessLayer>(() => new DCNBusinessLayer());

        public static DCNBusinessLayer Instance { get { return lazy.Value; } }

        /// <summary>
        /// The context
        /// </summary>
        private ClientContext context = null;

        /// <summary>
        /// The web
        /// </summary>
        private Web web = null;

        private DCNBusinessLayer()
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

        #region "GET DATA"
        /// <summary>
        /// Gets the DCN details.
        /// </summary>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>byte array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "test")]
        public IContract GetDCNDetails(IDictionary<string, string> objDict)
        {
            DCNContract contract = new DCNContract();
            BELDataAccessLayer helper = new BELDataAccessLayer();
            if (objDict != null && objDict.ContainsKey(Parameter.FROMNAME) && objDict.ContainsKey(Parameter.ITEMID) && objDict.ContainsKey(Parameter.USEREID))
            {
                string formName = objDict[Parameter.FROMNAME];
                int itemId = Convert.ToInt32(objDict[Parameter.ITEMID]);
                string userID = objDict[Parameter.USEREID];
                IForm dcnForm = new DCNForm(true);
                dcnForm = helper.GetFormData(this.context, this.web, ApplicationNameConstants.DCRAPP, formName, itemId, userID, dcnForm);
                if (dcnForm != null && dcnForm.SectionsList != null && dcnForm.SectionsList.Count > 0)
                {
                    ////Print Button Hide not required for GET DCN Detail Method
                    ////var sectionDetails = dcnForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCNSectionName.DESIGNDOCUMENTENGINEERSECTION)) as DesignDocumentEngineerSection;
                    ////if (sectionDetails != null)
                    ////{
                    ////    if (itemId != 0)
                    ////    {
                    ////        if ((sectionDetails.Status == FormStatus.COMPLETED || sectionDetails.Status == FormStatus.REJECTED) && sectionDetails.ApproversList.Any(p => p.Role == DCNRoles.DCRPROCESSINCHARGE && p.Approver != userID))
                    ////        {
                    ////            if (dcnForm.Buttons.FirstOrDefault(p => p.Name == "Print") != null)
                    ////            {
                    ////                dcnForm.Buttons.FirstOrDefault(p => p.Name == "Print").IsVisible = false;
                    ////            }
                    ////        }
                    ////    }
                    ////}
                    contract.Forms.Add(dcnForm);
                }
            }
            return contract;

        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "test")]
        public DCNAdminContract GetDCNAdminDetails(IDictionary<string, string> objDict)
        {
            DCNAdminContract contract = new DCNAdminContract();
            BELDataAccessLayer helper = new BELDataAccessLayer();
            if (objDict != null && objDict.ContainsKey(Parameter.FROMNAME) && objDict.ContainsKey(Parameter.ITEMID) && objDict.ContainsKey(Parameter.USEREID))
            {
                string formName = objDict[Parameter.FROMNAME];
                int itemId = Convert.ToInt32(objDict[Parameter.ITEMID]);
                string userID = objDict[Parameter.USEREID];
                IForm dcnAdminForm = new DCNAdminForm(true);
                dcnAdminForm = helper.GetFormData(this.context, this.web, ApplicationNameConstants.DCRAPP, formName, itemId, userID, dcnAdminForm);
                if (dcnAdminForm != null && dcnAdminForm.SectionsList != null && dcnAdminForm.SectionsList.Count > 0)
                {
                    var sectionDetails = dcnAdminForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCNSectionName.DCNDETAILADMINSECTION)) as DCNDetailAdminSection;
                    if (sectionDetails != null)
                    {
                        if (itemId != 0)
                        {
                            if ((sectionDetails.Status == FormStatus.COMPLETED || sectionDetails.Status == FormStatus.REJECTED) && !sectionDetails.ApproversList.Any(p => (p.Role == DCNRoles.DCRPROCESSINCHARGE || p.Role == DCNRoles.SCM) && !String.IsNullOrEmpty(p.Approver) && p.Approver.Split(',').Contains(userID.Trim())))
                            {
                                if (dcnAdminForm.Buttons.FirstOrDefault(p => p.Name == "Print") != null)
                                {
                                    dcnAdminForm.Buttons.FirstOrDefault(p => p.Name == "Print").IsVisible = false;
                                }
                            }
                        }
                    }
                    contract.Forms.Add(dcnAdminForm);
                }
            }
            return contract;

        }

        /// <summary>
        /// Gets the DCN details.
        /// </summary>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>byte array</returns>
        public List<DCRDetails> RetrieveAllDCRNos(IDictionary<string, string> objDict)
        {
            List<DCRDetails> dcrs = new List<DCRDetails>();
            List spList = this.web.Lists.GetByTitle(DCRDCNListNames.DCRLIST);
            CamlQuery query = new CamlQuery();
            if (objDict != null && objDict.ContainsKey("UserEmail"))
            {
                User loggedinuser = BELDataAccessLayer.EnsureUser(context, web, objDict["UserEmail"].ToString());
                query.ViewXml = @"<View>
                                        <Query>
                                                  <Where>
                                                        <And>
                                                            <Eq>
                                                                    <FieldRef Name='FinalDesignEngineer' /><Value Type='User'>" + loggedinuser.Title + @"</Value>
                                                            </Eq>
                                                        <And>
                                                            <Eq>
                                                                    <FieldRef Name='WorkflowStatus' />
                                                                    <Value Type='Text'>" + FormStatus.APPROVED + @"</Value>
                                                            </Eq>
                                                            <Eq>
                                                                    <FieldRef Name='IsDCNGenerated' />
                                                                    <Value Type='Boolean'>0</Value>
                                                            </Eq>
                                                    </And>
                                                   </And>
                                                 </Where>
                                        </Query>
                            </View>";
            }
            else
            {
                query.ViewXml = @"<View>
                                                        <Query>
                                                                  <Where>
                                                                    <And>
                                                                      <Eq>
                                                                            <FieldRef Name='WorkflowStatus' />
                                                                            <Value Type='Text'>" + FormStatus.APPROVED + @"</Value>
                                                                       </Eq>
                                                                       <Eq>
                                                                             <FieldRef Name='IsDCNGenerated' />
                                                                             <Value Type='Boolean'>0</Value>
                                                                       </Eq>
                                                                    </And>
                                                                 </Where>
                                                        </Query>
                                            </View>";
            }
            //  query.ViewXml = @"<View><ViewFields><FieldRef Name='ID' /><FieldRef Name='DCRNo' /><FieldRef Name='ProductName' /><FieldRef Name='RequestDate' /><FieldRef Name='ProposedBy' /></ViewFields></View>";
            ListItemCollection items = spList.GetItems(query);
            this.context.Load(items);
            this.context.ExecuteQuery();
            if (items != null && items.Count != 0)
            {
                foreach (ListItem item in items)
                {
                    DCRDetails dcr = new DCRDetails();
                    dcr.DCRNo = Convert.ToString(item["DCRNo"]);
                    dcr.ID = item.Id;
                    FieldUserValue user = item["ProposedBy"] as FieldUserValue;
                    dcr.ProposedBy = BELDataAccessLayer.GetNameFromPersonField(context, web, user);
                    dcr.ProductName = Convert.ToString(item["ProductName"]);
                    dcr.RequestDate = Convert.ToDateTime(item["RequestDate"]);
                    dcrs.Add(dcr);
                }
            }
            return dcrs;

        }

        /// <summary>
        /// Gets the DCN details.
        /// </summary>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>byte array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "test")]
        public DCRDetails RetrieveDCRNoDetails(IDictionary<string, string> objDict)
        {
            DCRForDCNContract contract = new DCRForDCNContract();
            DCRDetails dcrdetails = new DCRDetails();
            if (objDict != null)
            {
                string formName = objDict["FormName"];
                int itemId = Convert.ToInt32(objDict["ItemId"]);
                string userEmail = objDict["UserEmail"];
                //contract.UserDetails = BELDataAccessLayer.Instance.GetUserInformation(userEmail);
                IForm dcrForm = new DCRForDCNForm(true);
                dcrForm = BELDataAccessLayer.Instance.GetFormData(this.context, this.web, ApplicationNameConstants.DCRAPP, formName, itemId, userEmail, dcrForm);
                if (dcrForm != null && dcrForm.SectionsList != null && dcrForm.SectionsList.Count > 0)
                {
                    contract.Forms.Add(dcrForm);
                }
                if (contract.Forms != null && contract.Forms.Count > 0)
                {
                    var form = contract.Forms.FirstOrDefault();
                    if (form != null)
                    {
                        var dcrdetail = form.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCRSectionName.DCRDETAILSECTION));
                        var dcrSection = dcrdetail as DCRDetailsSection;
                        var dcrapproval = form.SectionsList.FirstOrDefault(f => f.SectionName.Equals(SectionNameConstant.APPLICATIONSTATUS)) as ApplicationStatusSection;
                        var app = dcrapproval.ApplicationStatusList;
                        dcrdetails.DCRNo = dcrSection.DCRNo;
                        dcrdetails.DCRID = itemId;
                        dcrdetails.RequestDepartment = dcrSection.Department;
                        dcrdetails.ProductName = dcrSection.ProductName;
                        dcrdetails.ProposedBy = dcrSection.ProposedBy;
                        dcrdetails.ProposedByName = dcrSection.ProposedByName;

                        dcrdetails.DesignEngineer = dcrSection.FinalDesignEngineer;
                        dcrdetails.DesignEngineerName = dcrSection.FinalDesignEngineerName;

                        app.ForEach(p =>
                        {
                            if (p.Role == DCRRoles.QAINCHARGE)
                            {
                                dcrdetails.QAIncharge = p.Approver;
                                dcrdetails.QAInchargeName = p.ApproverName;
                            }
                            if (p.Role == DCRRoles.SCMINCHARGE)
                            {
                                dcrdetails.SCMIncharge = dcrdetails.SCMIncharge + "," + p.Approver;
                                dcrdetails.SCMInchargeName = dcrdetails.SCMInchargeName + "," + p.ApproverName;
                            }
                            if (p.Role == DCRRoles.SCM)   
                            {
                                dcrdetails.SCMIncharge = dcrdetails.SCMIncharge + "," + p.Approver;
                                dcrdetails.SCMInchargeName = dcrdetails.SCMInchargeName + "," + p.ApproverName;
                            }
                            if (p.Role == DCRRoles.DCRPROCESSINCHARGE)
                            {
                                dcrdetails.DCRProcessIncharge = p.Approver;
                                dcrdetails.DCRProcessInchargeName = p.ApproverName;
                            }
                            if (p.Role == DCRRoles.CCINCHARGE)
                            {
                                dcrdetails.CCIncharge = p.Approver;
                                dcrdetails.CCInchargeName = p.ApproverName;
                            }
                            //if (p.Role == DCRRoles.DESIGNENGINEERINCHARGE)
                            //{
                            //    dcrdetails.DesignEngineer = p.Approver;
                            //    dcrdetails.DesignEngineerName = Convert.ToString(p.ApproverName);
                            //}
                            if (p.Role == DCRRoles.DESIGNDEVELOPENGINEER)
                            {
                                //dcrdetails.DesignEngineer = dcrdetails.DesignEngineer + "," + p.Approver;
                                //dcrdetails.DesignEngineerName = dcrdetails.DesignEngineerName + "," + p.ApproverName;

                            }
                            if (p.Role == DCRRoles.DESIGNENGINEERINCHARGE)
                            {
                                dcrdetails.DesignEngineerIncharge = p.Approver;
                                dcrdetails.DesignEngineerInchargeName = p.ApproverName;
                            }
                            if (p.Role == DCRRoles.KAPMARKETINGINCHARGE)
                            {
                                dcrdetails.KAPMarketingIncharge = Convert.ToString(p.Approver);
                                dcrdetails.KAPMarketingInchargeName = Convert.ToString(p.ApproverName);
                            }
                            if (p.Role == DCRRoles.DAPMARKETINGINCHARGE)
                            {
                                dcrdetails.DAPMarketingIncharge = Convert.ToString(p.Approver);
                                dcrdetails.DAPMarketingInchargeName = Convert.ToString(p.ApproverName);
                            }
                            if (p.Role == DCRRoles.FANSMARKETINGINCHARGE)
                            {
                                dcrdetails.FANSMarketingIncharge = Convert.ToString(p.Approver);
                                dcrdetails.FANSMarketingInchargeName = Convert.ToString(p.ApproverName);
                            }
                            if (p.Role == DCRRoles.LIGHTINGMARKETINGINCHARGE)
                            {
                                dcrdetails.LightingMarketingIncharge = Convert.ToString(p.Approver);
                                dcrdetails.LightingMarketingInchargeName = Convert.ToString(p.ApproverName);
                            }
                            if (p.Role == DCRRoles.MORPHYRICHARDSMARKETINGINCHARGE)
                            {
                                dcrdetails.MRMarketingIncharge = Convert.ToString(p.Approver);
                                dcrdetails.MRMarketingInchargeName = Convert.ToString(p.ApproverName);
                            }
                            if (p.Role == DCRRoles.LUMMARKETINGINCHARGE)
                            {
                                dcrdetails.LUMMarketingIncharge = Convert.ToString(p.Approver);
                                dcrdetails.LUMMarketingInchargeName = Convert.ToString(p.ApproverName);
                            }
                        });

                        string[] scmIncharge = !string.IsNullOrWhiteSpace(dcrdetails.SCMIncharge) ? dcrdetails.SCMIncharge.Trim(',').Split(',') : null;
                        dcrdetails.SCMIncharge = (scmIncharge != null && scmIncharge.Length > 0) ? string.Join(",", scmIncharge.Distinct().ToArray()) : string.Empty;
                        string[] scmInchargename = !string.IsNullOrWhiteSpace(dcrdetails.SCMInchargeName) ? dcrdetails.SCMInchargeName.Trim(',').Split(',') : null;
                        dcrdetails.SCMInchargeName = (scmInchargename != null && scmInchargename.Length > 0) ? string.Join(",", scmInchargename.Distinct().ToArray()) : string.Empty;

                        string designengineer = !string.IsNullOrWhiteSpace(dcrdetails.DesignEngineer) ? dcrdetails.DesignEngineer.Trim() : string.Empty;
                        dcrdetails.DesignEngineer = designengineer; // string.Join(",", designengineer.Distinct().ToArray());
                        string designengineername = !string.IsNullOrWhiteSpace(dcrdetails.DesignEngineerName) ? dcrdetails.DesignEngineerName.Trim() : string.Empty;
                        dcrdetails.DesignEngineerName = designengineername; //string.Join(",", designengineername.Distinct().ToArray());

                        var scrdetail = form.SectionsList.FirstOrDefault(f => f.SectionName.Equals(DCRSectionName.SCMINCHARGESECTION));
                        var scrdetails = scrdetail as VendorDCRSection;

                        if (scrdetails != null)
                        {
                            dcrdetails.VendorDCRList = new List<ITrans>();
                            foreach (Vendor vendor in scrdetails.VendorDCRList)
                            {
                                VendorDCN vendorDCN = new VendorDCN();
                                vendorDCN.DateOfImplementation = vendor.DateOfImplementation;
                                vendorDCN.ID = vendor.ID;
                                vendorDCN.RequestBy = vendor.RequestBy;
                                vendorDCN.RequestDate = vendor.RequestDate;
                                vendorDCN.VendorName = vendor.VendorName;
                                vendorDCN.Quantity = vendor.Quantity;
                                vendorDCN.FGStock = vendor.FGStock;
                                vendorDCN.ExistingComponentStock = vendor.ExistingComponentStock;
                                dcrdetails.VendorDCRList.Add(vendorDCN);
                            }
                        }
                    }
                }
            }
            return dcrdetails;
        }
        #endregion

        #region "SAVE DATA"
        /// <summary>
        /// Saves the by section.
        /// </summary>
        /// <param name="sections">The sections.</param>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>return status</returns>
        public ActionStatus SaveBySection(ISection sectionDetails, Dictionary<string, string> objDict)
        {
            ActionStatus status = new ActionStatus();

            BELDataAccessLayer helper = new BELDataAccessLayer();
            if (sectionDetails != null && objDict != null)
            {
                objDict[Parameter.ACTIVITYLOG] = DCRDCNListNames.DCNACTIVITYLOG;
                objDict[Parameter.APPLICATIONNAME] = ApplicationNameConstants.DCRAPP;
                objDict[Parameter.FROMNAME] = FormNameConstants.DCNFORM;
                DesignEngineerSection section = null;
                if (sectionDetails.SectionName == DCNSectionName.DESIGNENGINEERSECTION)
                {
                    if (sectionDetails.ListDetails != null && sectionDetails.ListDetails.Count != 0 && sectionDetails.ListDetails[0].ItemId == 0)
                    {
                        section = sectionDetails as DesignEngineerSection;
                        if (ISDCNGenerataed(section.DCRNo))
                        {
                            status.IsSucceed = false;
                            status.Messages.Add("Text_IsDCNGenerated");
                            return status;
                        }
                    }
                }

                List<ListItemDetail> objSaveDetails = helper.SaveData(this.context, this.web, sectionDetails, objDict);
                ListItemDetail itemDetails = objSaveDetails.Where(p => p.ListName.Equals(DCRDCNListNames.DCNLIST)).FirstOrDefault<ListItemDetail>();
                if (sectionDetails.SectionName == DCNSectionName.DESIGNENGINEERSECTION)
                {
                    if (sectionDetails.ListDetails != null && sectionDetails.ListDetails.Count != 0 && sectionDetails.ListDetails[0].ItemId == 0)
                    {
                        //var sectiondcn = sectionDetails as DesignDocumentEngineerSection;
                        Dictionary<string, dynamic> values = new Dictionary<string, dynamic>();
                        values.Add("DCNNo", section.DCNNo);
                        values.Add("IsDCNGenerated", "1");
                        BELDataAccessLayer.Instance.SaveFormFields(this.context, this.web, DCRDCNListNames.DCRLIST, section.DCRID, values);
                        status.ExtraData = section.DCNNo;
                    }
                }

                if (itemDetails.ItemId > 0)
                {
                    status.ItemID = itemDetails.ItemId;
                    status.IsSucceed = true;
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
                        case ButtonActionStatus.Complete:
                            status.Messages.Add("Text_CompleteSuccess");
                            break;
                        case ButtonActionStatus.SendBack:
                            status.Messages.Add("Text_SendBackSuccess");
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

        #endregion


        public bool ISDCNGenerataed(string dcrNo)
        {
            try
            {
                bool isDCNGenerataed = false;
                List spList = this.web.Lists.GetByTitle(DCRDCNListNames.DCRLIST);
                CamlQuery query = new CamlQuery();

                query.ViewXml = @"<View>
                                        <Query>
                                                  <Where>
                                                    <And>
                                                      <Eq>
                                                            <FieldRef Name='DCRNo' />
                                                            <Value Type='Text'>" + dcrNo + @"</Value>
                                                       </Eq>
                                                       <Eq>
                                                             <FieldRef Name='IsDCNGenerated' />
                                                             <Value Type='Boolean'>1</Value>
                                                       </Eq>
                                                    </And>
                                                 </Where>
                                        </Query>
                            </View>";
                ListItemCollection items = spList.GetItems(query);
                this.context.Load(items);
                this.context.ExecuteQuery();
                if (items != null && items.Count != 0)
                {
                    isDCNGenerataed = true;
                }
                return isDCNGenerataed;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return true;
            }


        }
    }
}