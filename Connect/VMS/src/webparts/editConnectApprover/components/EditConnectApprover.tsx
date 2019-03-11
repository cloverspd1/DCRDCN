import * as React from 'react';
import styles from './EditConnectApprover.module.scss';
import { IEditConnectApproverProps, ISPList, IKeyText } from './IEditConnectApproverProps';
import { escape } from '@microsoft/sp-lodash-subset';
import { TextField, PrimaryButton, DefaultButton, Label } from 'office-ui-fabric-react/lib/';
import pnp from "sp-pnp-js";
import { Web } from "sp-pnp-js";
import { Dialog, DialogType, DialogFooter } from 'office-ui-fabric-react/lib/Dialog';
import { Dropdown, IDropdown, DropdownMenuItemType, IDropdownOption } from 'office-ui-fabric-react/lib/Dropdown';
import { UrlQueryParameterCollection } from '@microsoft/sp-core-library';
import {
  assign,
  autobind
} from 'office-ui-fabric-react/lib/Utilities';


export default class EditConnectApprover extends React.Component<IEditConnectApproverProps, {}> {
  private listdata: ISPList;
  private Visible: string = 'hidden';
  public queryParameters: UrlQueryParameterCollection
  public web: any;
  private statusOption: IDropdownOption[];
  private reasonForRejectSelectedValue: { key: string | number | undefined, value: string };
  private reasonForRejectData: IDropdownOption[];

  public hideDialog: string;
  private errorDialog: boolean;
  private confirmDialog: boolean;
  private messegeDialog: boolean;
  private errorMsg: string[] = [" "];
  private saveMsg: string = "";
  private saveMsgTitle: string = "Success";
  public isSaveClick: boolean = false;
  public componentWillMount() {

    this.listdata = {
      ID: undefined,
      Title: undefined,
      Category: undefined,
      SubCategory: undefined,
      Description: undefined,
      Status: undefined,
      //Approver :undefined,
      ApproverComment: undefined,
      //  ConnectID:undefined,
      Created: undefined,
      Creator: undefined,
      ReasonForReject: undefined,
      Attachments: undefined,
      FileName: undefined
    }

    this.web = new Web(this.props.context.pageContext.web.absoluteUrl);
    this.queryParameters = new UrlQueryParameterCollection(window.location.href);

    if (this.queryParameters.getValue("ApproverId")) {
      const Id: number = parseInt(this.queryParameters.getValue("ApproverId"));
      console.log("ApproverId value is : " + Id);
      this.getListData(Id);
    }
  }

  @autobind
  public getReasonForReject(item: IDropdownOption) {
    this.listdata.Status = item.text;

    if (this.listdata.Status == "Reject") {
      this.Visible = 'visible';
      document.getElementById("divReason").hidden = false;
      var data: IDropdownOption[];
      this._getReasonForRejectData()
        .then((response) => {
          data = this._renderReasonForRejectList(response);
          this.setState(this.reasonForRejectData = data);
        });
    }
    else {
      this.Visible = 'hidden';
      data = null;
      document.getElementById("divReason").hidden = true;
    }
  }

  private _getReasonForRejectData(): Promise<ISPList[]> {
    return this.web.lists.getByTitle("ReasonForRejectMaster").items.orderBy("Title").get().then((response) => {
      return response;
    });
  }

  private _renderReasonForRejectList(items: ISPList[]) {
    var data = [];
    items.forEach((item: ISPList) => {
      data.push({
        key: item.Title,
        text: item.Title
      });

    });  
    return data;
  }
  @autobind
  private reasonSelected(item: IDropdownOption) {
    this.reasonForRejectSelectedValue = { key: item.key, value: item.text };
  }

  private getListData(Id: number): void {
    this._getListData(Id)
      .then((response) => {
        response.map((item: ISPList) => {
          this.listdata = {
            ID: item.ID,
            Title: item.Title,
            Description: item.Description,
            Category: item.Category,
            SubCategory: item.SubCategory,
            Status: item.Status,
            //Approver :item.Approver,
            ApproverComment: item.ApproverComment,
            // ConnectID :item.ConnectID,
            Created: item.Created,
            Creator: item.Creator,
            ReasonForReject: item.ReasonForReject,
            Attachments: item.Attachments,
            FileName: item.FileName
          };

        });
        this.setState(this.listdata);
        if (this.listdata.Status == 'Reject') {
          this.getReasonForReject({ key: this.listdata.Status, text: this.listdata.Status })
          this.setState(this.listdata);
        }

      });

  }

  private _getListData(Id): Promise<ISPList[]> {
    return this.web.lists.getByTitle("Connect%20Approval").items.expand("AttachmentFiles").filter("Id eq " + Id).get().then((response) => {
      var data = [];
      response.map((item) => {
        data.push({
          ID: item.ID,
          Title: item.Title,
          Description: item.Feedback_x0020_Description,
          Category: item.Category,
          SubCategory: item.Sub_x0020_Category,
          Status: item.Status,
          // Approver: item.Approver,
          ApproverComment: item.Approver_x0020_Comments,
          //ConnectID :item.Feedback_x0020_Title,
          Created: item.Created,
          Creator: item.CreatedByDisplay,
          ReasonForReject: item.ReasonForReject,
          Attachments: (item.AttachmentFiles.length > 0 ? item.AttachmentFiles[0].ServerRelativeUrl : null),
          FileName: (item.AttachmentFiles.length > 0 ? item.AttachmentFiles[0].FileName : null),
        });
      });
      return data;
    });
  }



  @autobind
  private _showDialog() {
    this.errorMsg = [];

    const _approverComment = document.getElementById('txtApproverComment')["value"].trim();
    const _status = document.getElementById("ddlStatus-option").textContent;

    var _reason = '';
    if (_status == "Reject") {
      _reason = document.getElementById('ddlReasons-option').textContent;
    }

    var isValid: boolean = true;

    if (_status == '' || _status == null || _status == undefined || _status.toLowerCase().match("select an option")) {
      document.getElementById("ddlStatus-option").style.borderColor = "red";
      document.getElementById("ddlStatus-option").style.backgroundColor = "lightyellow";
      isValid = false;
      this.errorMsg.push("Status is required.");
    }
    else {
      document.getElementById("ddlStatus-option").style.backgroundColor = "white";
    }


    if (_status == "Reject" && (_reason == '' || _reason == null || _reason == undefined || _reason.toLowerCase().match("select an option"))) {
      document.getElementById("ddlReasons-option").style.borderColor = "red";
      document.getElementById("ddlReasons-option").style.backgroundColor = "lightyellow";
      isValid = false;
      this.errorMsg.push("Reason for reject is required.");
    }
    else {
      document.getElementById("ddlReasons-option").style.backgroundColor = "white";
    }

    if (!_approverComment) {
      document.getElementById("txtApproverComment").parentElement.style.borderColor = "red";
      document.getElementById("txtApproverComment").style.backgroundColor = "lightyellow";
      isValid = false;
      this.errorMsg.push("Category Head Comment is required.");
    }
    else if (_approverComment.length > 1000) {
      document.getElementById("txtApproverComment").parentElement.style.borderColor = "red";
      document.getElementById("txtApproverComment").style.backgroundColor = "lightyellow";
      isValid = false;
      this.errorMsg.push("Category Head Comment should not be longer than 1000 characters.");
    }
    else {
      document.getElementById("txtApproverComment").parentElement.style.borderColor = "";
      document.getElementById("txtApproverComment").style.backgroundColor = "white";
    }

    if (isValid) {
      this.hideDialog == "true";
      this.setState(this.hideDialog);
      this.confirmDialog = true;
    }
    else {
      this.hideDialog == "true";
      this.setState(this.hideDialog);
      this.errorDialog = true;
    }

  }


  @autobind
  private _closeDialog() {
    this.hideDialog == "false";
    this.setState(this.hideDialog);
    this.errorDialog = false;
    this.confirmDialog = false;
  }
  @autobind
  private _closemessegeDialog() {
    this.hideDialog == "false";
    this.setState(this.hideDialog);
    this.messegeDialog = false;
    window.location.href = "https://bajajelect.sharepoint.com/teams/ConnectApp";

  }

  public hideConfirmDialog() {
    this.confirmDialog = false;
    this.hideDialog == "false";
    this.setState(this.hideDialog);
  }

  public _saveApprovalConnectForm() {
    if (!this.isSaveClick) {
      this.isSaveClick = true;
      var _approverComment = document.getElementById('txtApproverComment')['value'].trim();
      var _status = document.getElementById("ddlStatus-option").textContent;

      var _reason = '';
      var _requestStatus = 'In Progress';
      if (_status == "Reject") {
        _reason = document.getElementById('ddlReasons-option').textContent;
        _requestStatus = 'Closed';
      }

      if (this.queryParameters.getValue("ApproverId")) {
        var id = parseInt(this.queryParameters.getValue("ApproverId"));

        this.web.lists.getByTitle("Connect%20Approval").items.getById(id).update({
          Status: _status,
          ReasonForReject: _reason,
          Approver_x0020_Comments: _approverComment,
          RequestStatus: _requestStatus
        }).then((result): void => {
          this.hideConfirmDialog();
          window.location.href = "https://bajajelect.sharepoint.com/teams/ConnectApp/";
          //this.saveMsgTitle = "Success";
          //this.saveMsg = "Connect Call updated successfully.";
          //this.messegeDialog = true;
          //this.hideDialog == "true";
          //this.setState(this.hideDialog);

        }, (error: any): void => {
          console.log(error);
  
          this.saveMsgTitle = "Error";
          this.saveMsg = "Oops!Something went wrong!!!";
          this.hideDialog == "true";
          this.setState(this.hideDialog);
          this.messegeDialog = true;
        });
      }
    }


  }

  public render(): React.ReactElement<IEditConnectApproverProps> {

    this.statusOption = [
      { key: 'Approve', text: 'Approve' },
      { key: 'Reject', text: 'Reject' }]

    const divPadding = {
      padding: '10px 3px',
      position: 'relative',
    };
    const lblHeader = {
      width: '100%',
      padding: '5px 0',
      margin: 0,
      fontWeight: 400,
      fontFamily: '"Segoe UI Semibold WestEuropean","Segoe UI Semibold","Segoe UI",Tahoma,Arial,sans-serif',
      fontSize: '14px',
      color: "#002271"
    }
    const lblValue = {

      fontFamily: '"Segoe UI Regular WestEuropean","Segoe UI",Tahoma,Arial,sans-serif',
      fontWeight: 400,
      fontSize: '14px',
      padding: '0 0 5px',
      borderBottom: 'solid thin #ababab',
    }
    const errorStyle = {
      color: 'red'
    };
    const btnStyle = {
      paddingTop: 10,
      paddingLeft: 10,
    }

    const divAppCommentLabel = {
      padding: '10px 3px 0 3px',
      position: 'relative',
    };

    const divAppCommentTB = {
      padding: '0 2px 10px 4px',
      position: 'relative',
    };

    var divStyle = {
      width: '100%',
      padding: '5px 0',
      margin: 0,
      fontWeight: 400,
      fontFamily: '"Segoe UI Semibold WestEuropean","Segoe UI Semibold","Segoe UI",Tahoma,Arial,sans-serif',
      fontSize: '14px',
      color: "#002271",
      visibility: this.Visible,
    }

    if (this.listdata.Status && this.listdata.Status == "Reject") {
      document.getElementById("divReason").hidden = false;
      document.getElementById("divReason").style.visibility = 'visible';
    }
    else if (this.listdata.Status) {
      document.getElementById("divReason").hidden = true;
      document.getElementById("divReason").style.visibility = 'hidden';
    }

    var createdDate = '';
    if (this.listdata.Created) {
      createdDate = new Date(this.listdata.Created).toLocaleDateString('en-GB');
    }
    var backgroudStyle = {
      backgroundImage: 'url(https://bajajelect.sharepoint.com/teams/ConnectApp/SiteAssets/Images/topography.png)',
      backgroundPosition: 'center',
      backgroundRepeat: 'no-repeat',
      backgroundSize: 'cover',
    }


    return (
      <form formNoValidate >
        <div className={styles.editConnectApprover} >

          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={backgroudStyle}>

            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
              <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
                <label>Category</label>
              </div>
              <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
                <label>{this.listdata.Category}</label>
              </div>
            </div>

            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
              <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
                <label>Sub Category</label>
              </div>
              <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
                <label>{this.listdata.SubCategory}</label>
              </div>
            </div>

            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding}>
              <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
                <label>Title</label>
              </div>
              <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
                <label>{this.listdata.Title}</label>
              </div>
            </div>

            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
              <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
                <label>Description</label>
              </div>
              <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
                <label>{this.listdata.Description}</label>
              </div>
            </div>

            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
              <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
                <label>Created By</label>
              </div>
              <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
                <label>{this.listdata.Creator}</label>
              </div>
            </div>

            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
              <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
                <label>Created On</label>
              </div>
              <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
                <label>{createdDate}</label>
              </div>
            </div>

            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
              <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
                <label>Attachment</label>
              </div>
              <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
                <label><a href={this.listdata.Attachments} target="_blank">{this.listdata.FileName}</a></label>
              </div>
            </div>

            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding}>
              <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader} >
                <label>
                  <span style={errorStyle}>*</span>
                  Status
              <Dropdown
                    placeHolder='Select an Option'
                    id='ddlStatus'
                    selectedKey={this.listdata.Status}
                    ariaLabel='status'
                    className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6"
                    options={this.statusOption}
                    onChanged={this.getReasonForReject}
                  />
                </label>
              </div>
            </div>

            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding}>
              <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" hidden id="divReason" style={lblHeader} >
                <label>
                  <span style={errorStyle}>*</span>
                  Reason for Rejected
              <Dropdown
                    placeHolder='Select an Option'
                    id='ddlReasons'
                    selectedKey={this.listdata.ReasonForReject}
                    ariaLabel='reason'
                    className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6"
                    options={this.reasonForRejectData}
                  />
                </label>
              </div>
            </div>


            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divAppCommentLabel}>
              <label style={lblHeader}>
                <span style={errorStyle}>*</span>
                Category Head Comment
              </label>
            </div>
            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divAppCommentTB}>
              <TextField
                id="txtApproverComment"
                multiline
                maxLength={1000}
                // underlined
                rows={4}
                placeholder="Enter text here"
                className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6"
                value={this.listdata.ApproverComment}
                style={lblValue}
              />
            </div>


            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12">
              <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6">
                <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={btnStyle}>
                  <PrimaryButton
                    text='Submit'
                    onClick={this._showDialog}
                    iconProps={{ iconName: "CommentAdd" }}
                    style={{ backgroundColor: '#127316' }}
                  />
                </div>
                <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={btnStyle}>
                  <DefaultButton
                    style={{ backgroundColor: '#A73434', color: '#fff' }}
                    text="Cancel"
                    iconProps={{ iconName: "Cancel" }}
                    href="https://bajajelect.sharepoint.com/teams/ConnectApp/" />
                </div>
              </div>
              <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6"></div>
            </div>

          </div>

        </div>
        <Dialog

          type = {DialogType.largeHeader}
          isOpen={this.confirmDialog}
          onDismiss={() => this._closeDialog()}
          title='JMD Connect'
          subText="Are you sure you want to submit the request?" >
          <DialogFooter>
            <PrimaryButton id="btnYes" onClick={() => this._saveApprovalConnectForm()} text='Yes' />
            <DefaultButton onClick={() => this._closeDialog()} text='No' />
          </DialogFooter>
        </Dialog>
        <Dialog

          type = {DialogType.largeHeader}
          isOpen={this.messegeDialog}
          onDismiss={() => this._closemessegeDialog()}
          title={this.saveMsgTitle}
          subText={this.saveMsg} >
        </Dialog>
        <Dialog

          type = {DialogType.largeHeader}
          isOpen={this.errorDialog}
          onDismiss={() => this._closeDialog()}
          title='Error'>
          <ul>
            {this.errorMsg.filter(x => { return (x !== (undefined || null || '' || ' ')) }).map(element => <li>{element}</li>)}
          </ul>
          <DialogFooter>
            <DefaultButton onClick={() => this._closeDialog()} text='Cancel' />
          </DialogFooter>
        </Dialog>
      </form>
    );
  }
}
