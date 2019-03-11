import * as React from 'react';
import styles from './ViewApproverForm.module.scss';
import { IViewApproverFormProps, ISPList } from './IViewApproverFormProps';
import { escape } from '@microsoft/sp-lodash-subset';
import { DefaultButton, Label } from 'office-ui-fabric-react/lib/';
import pnp from "sp-pnp-js";
import { Web } from "sp-pnp-js";
import { UrlQueryParameterCollection } from '@microsoft/sp-core-library';

export default class ViewApproverForm extends React.Component<IViewApproverFormProps, {}> {

  private listdata: ISPList;
  queryParameters: UrlQueryParameterCollection;
  web: any;
  // private approver : string;
  // private creator : string;


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
      Attachments: undefined,
      FileName: undefined
    }

    this.web = new Web(this.props.context.pageContext.web.absoluteUrl);
    this.queryParameters = new UrlQueryParameterCollection(window.location.href);
    if (this.queryParameters.getValue("ApproverId")) {
      const Id: number = parseInt(this.queryParameters.getValue("ApproverId"));
      console.log("Id value is : " + Id);
      this.getListData(Id);
      // this.getApproverName(Id);
      // this.getCreatorName(Id);
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
          CreatedDate: item.Created,
          ApprovedRejectedDate: item.ApproveRejectedDate,
          ApprovedBy: item.ApprovedByDisplay,
          CreatedBy: item.CreatedByDisplay,
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
  //         if(k == "ApprovedBy")  
  //         {   
  //           approverName = data[k]; 
  //         } 
  //       } 
  //       return approverName;
  //   }).then(response=>{
  //       this.approver=response;   
  //       console.log("this.approver========" +this.approver);
  //   });
  //   this.setState(this.approver);  
  // }

  // public getCreatorName(Id : number)
  // {
  //   let creatorName;
  //   this.web.lists.getByTitle('Connect%20Approval').items.getById(Id).fieldValuesAsText.get().then(function(data) {
  //       //Populate all field values for the List Item
  //       for (var k in data) 
  //       {
  //         console.log(k + " - " + data[k]);  
  //         if(k == "creator")  
  //         {   
  //         creatorName = data[k]; 
  //         } 
  //       } 
  //       return creatorName;
  //   }).then(response=>{
  //       this.creator=response;   
  //       console.log("this.creator========" +this.creator);
  //   });
  //   this.setState(this.creator);  
  // }

  public render(): React.ReactElement<IViewApproverFormProps> {

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
    var appRejDate = '';

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
      <div className={styles.viewApproverForm}  >
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
              <label>Approver Comment</label>
            </div>
            <div className="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6" style={lblValue}>
              <label>{this.listdata.ApproverComment}</label>
            </div>
          </div>
          <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={divPadding} >
            <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={lblHeader}>
              <label>Approved/Rejected Date</label>
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
