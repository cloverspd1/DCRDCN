namespace BEL.DCRDCNWorkflow.Models
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.Models.Common;

    /// <summary>
    /// Discount Requisition Section
    /// </summary>
    [DataContract, Serializable]
    public class WIPDCRDetailsSection : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WIPDCRDetailsSection"/> class.
        /// </summary>
        public WIPDCRDetailsSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WIPDCRDetailsSection"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public WIPDCRDetailsSection(bool isSet)
        {
            if (isSet)
            {
                String query = @"<View>
                                            <Query>
                                                  <Where>
                                                    <And>                                                     
                                                      <Neq>
                                                            <FieldRef Name='Status' />
                                                              <Value Type='Text'>" + FormStatus.COMPLETED + @"</Value>
                                                       </Neq>                                                      
                                                       <Neq>
                                                            <FieldRef Name='Status' />
                                                              <Value Type='Text'>" + FormStatus.REJECTED + @"</Value>
                                                       </Neq>
                                                </And>
                                        </Where></Query></View>";
                this.ListDetails = new List<ListItemDetail>() { new ListItemDetail(DCRDCNListNames.DCRLIST, true, query) };
                this.SectionName = DCRSectionName.DCRDETAILSECTION;
            }
        }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        [DataMember]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        [DataMember]
        public string DCRNo { get; set; }

        /// <summary>
        /// Product Name
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, DataType(DataType.Date)]
        public DateTime? RequestDate { get; set; }


        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        [DataMember, IsPerson(true, false,true), IsViewer]
        public string ProposedBy { get; set; }

        /// <summary>
        /// Gets or sets the tour to date.
        /// </summary>
        /// <value>
        /// The tour to date.
        /// </value>
        [DataMember, Required, FieldColumnName("DescriptionOfDesignChangePropose")]
        public string DescriptionOfDesignChangePropose { get; set; }

        /// <summary>
        /// Design engineer status
        /// </summary>
        [DataMember, IsListColumn(false)]
        public string DnD {
            get
            {
                if (this.ApproversList != null && this.ApproversList.Count != 0)
                {
                    string status = GetRolewiseStatus(DCRRoles.DESIGNDEVELOPENGINEER); //ApproversList.All(p => p.Role.Contains(DCRRoles.SCM) && p.Status == "Approved") ? "Completed" : "Pending";
                    return status;
                }
                return "-";
            }
        }

        /// <summary>
        /// SCM Status
        /// </summary>
        [DataMember, IsListColumn(false)]
        public string SCM
        {
            get
            {
                if (this.ApproversList != null && this.ApproversList.Count != 0)
                {
                    string status = GetRolewiseStatus(DCRRoles.SCM); //ApproversList.All(p => p.Role.Contains(DCRRoles.SCM) && p.Status == "Approved") ? "Completed" : "Pending";
                    return status;
                }
                return "-";
            }
        }

        /// <summary>
        /// QA Status
        /// </summary>
        [DataMember, IsListColumn(false)]
        public string QA
        {
            get
            {
                if (this.ApproversList != null && this.ApproversList.Count != 0)
                {
                    string status = GetRolewiseStatus(DCRRoles.QA); //ApproversList.All(p => p.Role.Contains(DCRRoles.SCM) && p.Status == "Approved") ? "Completed" : "Pending";
                    return status;
                }
                return "-";
            }
        }

        /// <summary>
        /// CC Status
        /// </summary>
        [DataMember, IsListColumn(false)]
        public string CC
        {
            get
            {
                if (this.ApproversList != null && this.ApproversList.Count != 0)
                {
                    string status = GetRolewiseStatus(DCRRoles.CCINCHARGE); //ApproversList.All(p => p.Role.Contains(DCRRoles.SCM) && p.Status == "Approved") ? "Completed" : "Pending";
                    return status;
                }
                return "-";
            }
        }

        /// <summary>
        /// Marketing Status
        /// </summary>
        [DataMember, IsListColumn(false)]
        public string Marketing
        {
            get
            {
                if (this.ApproversList != null && this.ApproversList.Count != 0)
                {
                    string status = GetRolewiseStatus(DCRRoles.MARKETING); //ApproversList.All(p => p.Role.Contains(DCRRoles.SCM) && p.Status == "Approved") ? "Completed" : "Pending";
                    return status;
                }
                return "-";
            }
        }

        /// <summary>
        /// DAP Status
        /// </summary>
        [DataMember, IsListColumn(false)]
        public string DAPMarketing {
            get
            {
                if (this.ApproversList != null && this.ApproversList.Count != 0 && this.BusinessUnit.Trim().ToLower().Contains("cp"))
                {
                    string status = GetRolewiseStatus(DCRRoles.DAPMARKETING); 
                    return status;
                }
                return "-";
            }
        }

        /// <summary>
        /// KAP Marketing Status
        /// </summary>
        [DataMember, IsListColumn(false)]
        public string KAPMarketing { 
            get
            {
                if (this.ApproversList != null && this.ApproversList.Count != 0 && this.BusinessUnit.Trim().ToLower().Contains("cp"))
                {
                    string status = GetRolewiseStatus(DCRRoles.KAPMARKETING);
                    return status;
                }
                return "-";
            } 
        }

        /// <summary>
        /// Fans Markeitng status
        /// </summary>
        [DataMember, IsListColumn(false)]
        public string FANSMarketing {
            get
            {
                if (this.ApproversList != null && this.ApproversList.Count != 0 && this.BusinessUnit.Trim().ToLower().Contains("cp"))
                {
                    string status = GetRolewiseStatus(DCRRoles.FANSMARKETING);
                    return status;
                }
                return "-";
            } 
        }

        /// <summary>
        /// Lighting Markeitng status
        /// </summary>
        [DataMember, IsListColumn(false)]
        public string LightingMarketing {
            get
            {
                if (this.ApproversList != null && this.ApproversList.Count != 0  && this.BusinessUnit.Trim().ToLower().Contains("cp") )
                {
                    string status = GetRolewiseStatus(DCRRoles.LIGHTINGMARKETING);
                    return status;
                }
                return "-";
            } 

        }

        /// <summary>
        /// MR Markeitng status
        /// </summary>
        [DataMember, IsListColumn(false)]
        public string MRMarketing {
            get
            {
                if (this.ApproversList != null && this.ApproversList.Count != 0 && this.BusinessUnit.Trim().ToLower().Contains("cp"))
                {
                    string status = GetRolewiseStatus(DCRRoles.MORPHYRICHARDSMARKETING);
                    return status;
                }
                return "-";
            } 
        }

        /// <summary>
        /// LUM Markeitng status
        /// </summary>
        [DataMember, IsListColumn(false)]
        public string LUMMarketing
        {
            get
            {
                if (this.ApproversList != null && this.ApproversList.Count != 0 && this.BusinessUnit.Trim().ToLower().Contains("lum"))
                {
                    string status = GetRolewiseStatus(DCRRoles.LUMMARKETING);
                    return status;
                }
                return "-";
            }
        }

        [DataMember, IsListColumn(false)]
        public string Costing
        {
            get
            {
                if (this.ApproversList != null && this.ApproversList.Count != 0 && this.BusinessUnit.Trim().ToLower().Contains("lum"))
                {
                    string status = GetRolewiseStatus(DCRRoles.COSTINGUSER);
                    return status;
                }
                return "-";
            }
        }

        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>
        /// The name of the section.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string SectionName { get; set; }

        /// <summary>
        /// Gets or sets the list details.
        /// </summary>
        /// <value>
        /// The list details.
        /// </value>
        [DataMember, IsListColumn(false)]
        public List<ListItemDetail> ListDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        [DataMember, IsListColumn(false)]
        public bool IsActive { get; set; }
        /// <summary>
        /// Gets or sets the action status.
        /// </summary>
        /// <value>
        /// The action status.
        /// </value>
        [DataMember, IsListColumn(false)]
        public ButtonActionStatus ActionStatus { get; set; }

        ///// <summary>
        ///// Gets or sets the current approver.
        ///// </summary>
        ///// <value>
        ///// The current approver.
        ///// </value>
        //[DataMember, IsListColumn(false), IsApproverDetails(true, DCRDCNListNames.DCNAPPAPPROVALMATRIX)]
        //public ApplicationStatus CurrentApprover { get; set; }

        /// <summary>
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverMatrixField(true, DCRDCNListNames.DCRAPPAPPROVALMATRIX)]
        public List<ApplicationStatus> ApproversList { get; set; }

        /// <summary>
        /// Gets or sets the button caption.
        /// </summary>
        /// <value>
        /// The button caption.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string ButtonCaption { get; set; }

        /// <summary>
        /// Gets or sets the form belong to.
        /// </summary>
        /// <value>
        /// The form belong to.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string FormBelongTo { get; set; }

        /// <summary>
        /// Gets or sets the application belong to.
        /// </summary>
        /// <value>
        /// The application belong to.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string ApplicationBelongTo { get; set; }

        /// Gets or sets the visit location.
        /// </summary>
        /// <value>
        /// The visit location.
        /// </value>
        [DataMember, Required]
        public string BusinessUnit { get; set; }


        public string GetRolewiseStatus(string role)
        {
            if (this.ApproversList != null && this.ApproversList.Count != 0 && this.ApproversList.Any(p => p.Role.Contains(role)))
            {
                string status = "-";
                List<ApplicationStatus> approvers = ApproversList.Where(p => p.Role.Contains(role) && !string.IsNullOrEmpty(p.Approver)).ToList();
                if (approvers != null && approvers.Count != 0)
                {
                    if (approvers.Any(p => p.Status == "Not Required"))
                    {
                        status = "-";
                    }
                    else if(approvers.All(p => p.Status == "Approved"))
                    {
                        status = "Completed";
                    }
                    else 
                    {
                        status = "Pending";
                    } 
                }
                

                //if (ApproversList.Any(p => p.Role.Contains(role) && !string.IsNullOrEmpty(p.Approver) && p.Status == "Not Required"))
                //{
                //    status = "-";
                //}
                //else if (ApproversList.Any(p => p.Role.Contains(role) &&  !string.IsNullOrEmpty(p.Approver) && p.Status == "Approved"))
                //{
                //    status = "Completed";
                //}
                //else 
                //{
                //    status = "Pending";
                //} 
                return status;
                
            }
            return "-";
        }
    }
}
