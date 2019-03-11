import * as React from 'react';
import styles from './ViewRequest.module.scss';
import { IViewRequestProps, ISPList } from './IViewRequestProps';
import { escape } from '@microsoft/sp-lodash-subset';
import { PrimaryButton, DefaultButton, Label } from 'office-ui-fabric-react/lib/';
import pnp from "sp-pnp-js";
import { Web } from "sp-pnp-js";
import { UrlQueryParameterCollection } from '@microsoft/sp-core-library';
//import { Conversation } from 'sp-pnp-js/lib/graph/conversations';

export default class ViewRequest extends React.Component<IViewRequestProps, {}> {

  private listdata: ISPList;
  public queryParameters: UrlQueryParameterCollection;
  public web: any;
  //private approver : string;

  public componentWillMount() {
    this.listdata = {
      ID: undefined,
      Title: undefined,
      Category: undefined,
      SubCategory: undefined,
      Description: undefined,
      Status: undefined,
      ApproverComment: undefined,
      ApprovedBy: undefined,
      ApprovedRejectedDate: undefined,
      ReasonForReject: undefined,
      RequestStatus: undefined,
      SuperUserComment: undefined,
      SuperAdminCommentDate: undefined,
      Attachments: undefined,
      FileName: undefined
    };

    this.web = new Web(this.props.context.pageContext.web.absoluteUrl);
    this.queryParameters = new UrlQueryParameterCollection(window.location.href);
    if (this.queryParameters.getValue("ConnectId")) {
      const Id: number = parseInt(this.queryParameters.getValue("ConnectId"));
      console.log("Id value is : " + Id);
      this.getListData(Id);
      //  this.getApproverName(Id);
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
            ApprovedRejectedDate: item.ApprovedRejectedDate,
            ApprovedBy: item.ApprovedBy,
            ReasonForReject: item.ReasonForReject,
            RequestStatus: item.RequestStatus,
            SuperUserComment: item.SuperUserComment,
            SuperAdminCommentDate: item.SuperAdminCommentDate,
            Attachments: item.Attachments,
            FileName: item.FileName

          };
        });
        this.setState(this.listdata);
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
          ApproverComment: item.Approver_x0020_Comments,
          ApprovedRejectedDate: item.ApproveRejectedDate,
          ApprovedBy: item.ApprovedByDisplay,
          ReasonForReject: item.ReasonForReject,
          RequestStatus: item.RequestStatus,
          SuperUserComment: item.SuperUserComment,
          SuperAdminCommentDate: item.SuperAdminCommentDate,
          Attachments: (item.AttachmentFiles.length > 0 ? item.AttachmentFiles[0].ServerRelativeUrl : null),
          FileName: (item.AttachmentFiles.length > 0 ? item.AttachmentFiles[0].FileName : null),
        });
      });

      return data;
    });
  }

  // public getApproverName(Id : number)
  // {
  //   let approverName;
  //   this.web.lists.getByTitle('Connect%20Approval').items.getById(Id).fieldValuesAsText.get().then(function(data) {
  //       //Populate all field values for the List Item
  //       for (var k in data) 
  //       {
  //         console.log(k + " - " + data[k]);  
  //         if(k == "Approver")  
  //         {   
  //         approverName = data[k]; 
  //         } 
  //       } 
  //       return approverName;
  //   }).then(response=>{
  //       this.approver=response;   
  //       console.log("this.approver========" +this.approver);
  //   });
  //   this.setState(this.approver);  
  // }

  public render(): React.ReactElement<IViewRequestProps> {
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
    };
    const lblValue = {

      fontFamily: '"Segoe UI Regular WestEuropean","Segoe UI",Tahoma,Arial,sans-serif',
      fontWeight: 400,
      fontSize: '14px',
      padding: '0 0 5px',
      borderBottom: 'solid thin #ababab',
    };

    var comment = '';
    var appRejDate = '';

    if (this.listdata.Status && this.listdata.Status == "Reject" && this.listdata.ApprovedRejectedDate) {
      document.getElementById("divReason").hidden = false;
      comment = this.listdata.ApproverComment;
      appRejDate = new Date(this.listdata.ApprovedRejectedDate).toLocaleDateString('en-GB');
    }
    else {
      comment = this.listdata.SuperUserComment;
      if (this.listdata.SuperAdminCommentDate) {
        appRejDate = new Date(this.listdata.SuperAdminCommentDate).toLocaleDateString('en-GB');
      }
    }

    var backgroudStyle = {
      backgroundImage: 'url(https://bajajelect.sharepoint.com/teams/ConnectApp/SiteAssets/Images/topography.png)',
      backgroundPosition: 'center',
      backgroundRepeat: 'no-repeat',
      backgroundSize: 'cover',
    };

    return (
      <div className={styles.viewRequest} >

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
              <label>Request Status</label>
            </div>
            <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
              <label>{this.listdata.RequestStatus}</label>
            </div>
          </div>

          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" hidden id='divReason' style={divPadding} >
            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
              <label>Reason For Reject</label>
            </div>
            <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
              <label>{this.listdata.ReasonForReject}</label>
            </div>
          </div>


          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
              <label>JMD Comment</label>
            </div>
            <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
              <label>{comment}</label>
            </div>
          </div>
          {/* <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
                <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}> 
                  <label>Approved By</label>
                </div>
                <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}> 
                  <label>{this.listdata.ApprovedBy}</label>
                </div>
              </div>   */}
          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
              <label>Comment Date</label>
            </div>
            <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
              <label>{appRejDate}</label>
            </div>
          </div>

          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >


            <DefaultButton
              text='Close'
              iconProps={{ iconName: "Cancel" }}
              style={{ backgroundColor: '#A73434', color: '#fff' }}
              href='https://bajajelect.sharepoint.com/teams/ConnectApp/'
            />
          </div>

        </div>


      </div>
    );
  }
}
