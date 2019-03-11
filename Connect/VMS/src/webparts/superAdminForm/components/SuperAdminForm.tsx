import * as React from 'react';
import styles from './SuperAdminForm.module.scss';
import { ISuperAdminFormProps, ISPList } from './ISuperAdminFormProps';
import { escape } from '@microsoft/sp-lodash-subset';
import { TextField, DefaultButton, PrimaryButton, Label } from 'office-ui-fabric-react/lib/';
import pnp from "sp-pnp-js";
import { Web } from "sp-pnp-js";
import { UrlQueryParameterCollection } from '@microsoft/sp-core-library';
import { Dialog, DialogType, DialogFooter } from 'office-ui-fabric-react/lib/Dialog';
import {
  assign,
  autobind
} from 'office-ui-fabric-react/lib/Utilities';

export default class SuperAdminForm extends React.Component<ISuperAdminFormProps, {}> {
  private listdata: ISPList;
  queryParameters: UrlQueryParameterCollection;
  web: any;
  private errorDialog: boolean;
  private confirmDialog: boolean;
  private messegeDialog: boolean;
  private errorMsg: string[] = [" "];
  private saveMsg: string = "";
  private saveMsgTitle: string = "Success";
  public isSaveClicked: boolean = false;
  hideDialog: string;


  componentWillMount() {
    this.listdata = {
      ID: undefined,
      Title: undefined,
      Category: undefined,
      SubCategory: undefined,
      Description: undefined,
      Status: undefined,
      ApproverComment: undefined,
      CreatedDate: undefined,
      ApprovedBy: undefined,
      ApprovedRejectedDate: undefined,
      CreatedBy: undefined,
      SuperUserAcknowledged: undefined,
      SuperUserComment: undefined,
      Attachments :undefined,
      FileName :undefined
    }



    this.web = new Web(this.props.context.pageContext.web.absoluteUrl);
    this.queryParameters = new UrlQueryParameterCollection(window.location.href);
    if (this.queryParameters.getValue("ConnectID")) {
      const Id: number = parseInt(this.queryParameters.getValue("ConnectID"));
      console.log("Id value is : " + Id);
      this.getListData(Id);
    }
  }



  private getListData(Id: number): void {
    this._getListData(Id)
      .then((response) => {
        //ISPList data;
        response.map((item: ISPList) => {
          this.listdata = {
            ID: item.ID,
            Title: item.Title,
            Description: item.Description,
            Category: item.Category,
            SubCategory: item.SubCategory,
            Status: item.Status,
            ApproverComment: item.ApproverComment,
            CreatedDate: item.CreatedDate,
            ApprovedRejectedDate: item.ApprovedRejectedDate,
            ApprovedBy: item.ApprovedBy,
            CreatedBy: item.CreatedBy,
            SuperUserAcknowledged: item.SuperUserAcknowledged,
            SuperUserComment: item.SuperUserComment,
            Attachments: item.Attachments,
            FileName :item.FileName
          };
        });
        this.setState(this.listdata);
      });

  }

  @autobind
  private _showDialog() {
    this.errorMsg = [];
    const _superUserComment = document.getElementById('txtSuperUserComment')['value'].trim();

    var isValid: boolean = true;
    if (!_superUserComment) {
      document.getElementById("txtSuperUserComment").parentElement.style.borderColor = "red";
      document.getElementById("txtSuperUserComment").style.backgroundColor = "lightyellow";
      isValid = false;
      this.errorMsg.push("JMD Connect Super User Comment is required.");
    }
    else if (_superUserComment.length > 1000) {
      document.getElementById("txtSuperUserComment").parentElement.style.borderColor = "red";
      document.getElementById("txtSuperUserComment").style.backgroundColor = "lightyellow";
      isValid = false;
      this.errorMsg.push("JMD Connect Super User Comment should not be longer than 1000 characters.");
    }
    else {
      document.getElementById("txtSuperUserComment").style.backgroundColor = "white";
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

  hideConfirmDialog() {
    this.confirmDialog = false;
    this.hideDialog == "false";
    this.setState(this.hideDialog);
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
          ApproverComment: item.Approver_x0020_Comments,
          CreatedDate: item.Created,
          ApprovedRejectedDate: item.ApproveRejectedDate,
          ApprovedBy: item.ApprovedByDisplay,
          CreatedBy: item.CreatedByDisplay,
          SuperUserAcknowledged: item.SuperUserAcknowledged,
          SuperUserComment: item.SuperUserComment,
          Attachments: (item.AttachmentFiles.length>0? item.AttachmentFiles[0].ServerRelativeUrl:null),
          FileName : (item.AttachmentFiles.length>0? item.AttachmentFiles[0].FileName:null),
        });
      });

      return data;
    });
  }

  public _saveSuperUserForm() {
    if(!this.isSaveClicked)
    {
      this.isSaveClicked=true;
    var _superUserComment = document.getElementById('txtSuperUserComment')['value'].trim();

    if (this.queryParameters.getValue("ConnectID")) {
      var id = parseInt(this.queryParameters.getValue("ConnectID"));
      this.web.lists.getByTitle("Connect%20Approval").items.getById(id).update({
        SuperUserAcknowledged: 'Acknowledged',
        SuperUserComment: _superUserComment,
        RequestStatus: 'Closed'
      }).then((result): void => {
        // alert("Acknowledgement has been sent to Requestor");
      
        this.hideConfirmDialog();
        window.location.href = "https://bajajelect.sharepoint.com/teams/ConnectApp/";
        //this.saveMsgTitle = "Success";
        //this.saveMsg = "Acknowledgement has been sent to requestor.";
        //this.messegeDialog = true;
        //this.hideDialog == "true";
        //this.setState(this.hideDialog);
      }, (error: any): void => {
        console.log(error);
        // alert("Oops!Something went wrong!!!");
        this.saveMsgTitle = "Error";
        this.saveMsg = "Oops!Something went wrong!!!";
        this.hideDialog == "true";
        this.setState(this.hideDialog);
        this.messegeDialog = true;
      });
    }

  }
  }


  public render(): React.ReactElement<ISuperAdminFormProps> {
    const divPadding = {
      paddingTop: 8,
      paddingBottom: 8,
      paddingLeft: 3,
      paddingRight: 3,
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

    const divAppCommentLabel = {
      padding: '10px 3px 0 3px',
      position: 'relative',
    };

    const divAppCommentTB = {
      padding: '0 3px 10px 3px',
      position: 'relative',
    };

    const btnStyle = {
      paddingTop: 10,
      paddingLeft: 10,
    }
    const errorStyle = {
      color: 'red'
    };

    var appRejDate = '';

    if (this.listdata.SuperUserAcknowledged && this.listdata.SuperUserAcknowledged == "Acknowledged") {
      document.getElementById("btnSave").hidden = true;
      document.getElementById("btnSave").style.visibility = "hidden";
      document.getElementById("txtSuperUserComment").setAttribute("disabled", "disabled");
    }


    if (this.listdata.ApprovedRejectedDate) {
      appRejDate = new Date(this.listdata.ApprovedRejectedDate).toLocaleDateString('en-GB');
    }

    var createdDate = '';
    if (this.listdata.CreatedDate) {
      createdDate = new Date(this.listdata.CreatedDate).toLocaleDateString('en-GB');
    }
    var backgroudStyle = {
      backgroundImage: 'url(https://bajajelect.sharepoint.com/teams/ConnectApp/SiteAssets/Images/topography.png)',
      backgroundPosition: 'center',
      backgroundRepeat: 'no-repeat',
      backgroundSize: 'cover',
    }
    return (
      <form formNoValidate className={styles.superAdminForm}   >

        <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={backgroudStyle}>


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

          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding}>
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

          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
              <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
                <label>Attachment</label>
              </div>
              <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
                <label><a href={this.listdata.Attachments} target="_blank">{this.listdata.FileName}</a></label>
              </div>
            </div>


          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
              <label>Created By</label>
            </div>

            <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
              <label>{this.listdata.CreatedBy}</label>
            </div>
          </div>

          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
              <label>Created Date</label>
            </div>
            <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
              <label>{createdDate}</label>
            </div>
          </div>

          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
              <label>Status</label>
            </div>
            <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
              <label>{this.listdata.Status}</label>
            </div>
          </div>
          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
              <label>Category Head Approved By</label>
            </div>
            <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
              <label>{this.listdata.ApprovedBy}</label>
            </div>
          </div>

          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
              <label>Category Head Approved Date</label>
            </div>
            <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
              <label>{appRejDate}</label>
            </div>
          </div>

          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
              <label>Category Head Comment</label>
            </div>
            <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
              <label>{this.listdata.ApproverComment}</label>
            </div>
          </div>


          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divAppCommentLabel}>
            <label style={lblHeader}>
              <span style={errorStyle}>*</span>
              JMD Connect Super User Comment
            </label>
          </div>

          <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={divAppCommentTB}>
            <TextField
              id="txtSuperUserComment"
              multiline
              maxLength={1000}
              rows={4}
              placeholder="Enter text here"
              className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12"
              value={this.listdata.SuperUserComment}
            />
          </div>



          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12">
            <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6">
              <div className="ms-Grid-col ms-u-sm12 ms-u-md8 ms-u-lg8" style={btnStyle}>
                <PrimaryButton
                  id="btnSave"
                  //type='Submit'
                  text='Acknowledge'
                  style={{ backgroundColor: '#127316' }}
                  iconProps={{ iconName: "Mail" }}
                  onClick={this._showDialog}
                />
              </div>
              <div className="ms-Grid-col ms-u-sm12 ms-u-md4 ms-u-lg4" style={btnStyle}>
                <DefaultButton
                  text="Cancel"
                  style={{ backgroundColor: '#A73434', color : '#fff' }}
                  iconProps={{ iconName: "Cancel" }}
                  href="https://bajajelect.sharepoint.com/teams/ConnectApp/" />
              </div>
            </div>
            <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6"></div>
          </div>



        </div>



        <Dialog

          type = {DialogType.largeHeader}
          isOpen={this.confirmDialog}
          onDismiss={() => this._closeDialog()}
          title='JMD Connect'
          subText="Are you sure you want to acknowledge requestor?" >
          <DialogFooter>
            <PrimaryButton onClick={() => this._saveSuperUserForm()} text='Yes' />
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
